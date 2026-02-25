using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CustomerFluent.Data;
using CustomerFluent.Models.Identity;
using CustomerFluent.Services;
using CustomerFluent.Repositories;
using CustomerFluent.Services.CustomerService;
using CustomerFluent.Validators;
using CustomerFluent.Mappings;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using Serilog.Events;
using System.Threading.RateLimiting;
using System.IO;
using System.Text.RegularExpressions;

// Create early logger for startup errors
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting CustomerFluent API application");

    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
            path: "Logs/customerFluent-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext}: {Message:lj} {Properties:j}{NewLine}{Exception}"));

    // Add services to the container
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

        // Enable sensitive data logging in development
        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }
    });

    // Add Identity Services
    builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true;

        // Sign in settings
        options.SignIn.RequireConfirmedEmail = false; // Set to true in production
        options.SignIn.RequireConfirmedPhoneNumber = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

    // Add JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Log.Warning("JWT Authentication failed: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Log.Debug("JWT Token validated for user: {UserName}",
                    context.Principal?.Identity?.Name ?? "Unknown");
                return Task.CompletedTask;
            }
        };
    });

    // FIXED: Corrected Per-User Rate Limiting Configuration for 2 requests max
    builder.Services.AddRateLimiter(rateLimiterOptions =>
    {
        Log.Information("🔧 Configuring per-user rate limiting (2 requests max)...");

        // AuthPolicy - FIXED: Only 2 requests allowed, then rate limited
        rateLimiterOptions.AddPolicy("AuthPolicy", httpContext =>
            RateLimitPartition.GetTokenBucketLimiter(
                partitionKey: GetFixedPartitionKey(httpContext, "auth"),
                factory: partition => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 2,  // Only 2 tokens initially - EXACTLY what you want
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0,  // No queueing - immediate rejection after 2 requests
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1),  // Long replenishment period
                    TokensPerPeriod = 1,  // Only 1 token per minute (very slow replenishment)
                    AutoReplenishment = true
                }));

        // UserPolicy - For authenticated endpoints (same logic)
        rateLimiterOptions.AddPolicy("UserPolicy", httpContext =>
            RateLimitPartition.GetTokenBucketLimiter(
                partitionKey: GetFixedPartitionKey(httpContext, "user"),
                factory: partition => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 2,  // Only 2 tokens for authenticated users too
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                    TokensPerPeriod = 1,
                    AutoReplenishment = true
                }));

        // AdminPolicy - Higher limits for admins
        rateLimiterOptions.AddPolicy("AdminPolicy", httpContext =>
            RateLimitPartition.GetTokenBucketLimiter(
                partitionKey: GetFixedPartitionKey(httpContext, "admin"),
                factory: partition => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 20,  // Admins get more tokens
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(30),
                    TokensPerPeriod = 5,
                    AutoReplenishment = true
                }));

        rateLimiterOptions.RejectionStatusCode = 429;

        rateLimiterOptions.OnRejected = async (context, token) =>
        {
            var partitionKey = GetFixedPartitionKey(context.HttpContext, "rejected");

            // ENHANCED LOGGING
            Log.Warning("🚨 RATE LIMIT TRIGGERED! User: {PartitionKey}, Endpoint: {Endpoint}, Method: {Method}",
                partitionKey, context.HttpContext.Request.Path, context.HttpContext.Request.Method);

            context.HttpContext.Response.StatusCode = 429;
            context.HttpContext.Response.Headers.ContentType = "application/json";

            TimeSpan? retryAfter = null;
            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterValue))
            {
                retryAfter = retryAfterValue;
                context.HttpContext.Response.Headers.RetryAfter =
                    ((int)retryAfterValue.TotalSeconds).ToString();
            }

            var response = new
            {
                error = "Rate limit exceeded",
                message = $"Maximum 2 requests allowed per user. User: {partitionKey}",
                retryAfter = retryAfter?.TotalSeconds,
                partitionKey = partitionKey,
                endpoint = context.HttpContext.Request.Path.ToString(),
                timestamp = DateTime.UtcNow,
                maxRequestsAllowed = 2
            };

            await context.HttpContext.Response.WriteAsync(
                System.Text.Json.JsonSerializer.Serialize(response), token);
        };

        Log.Information("✅ Rate limiting configured: 2 requests max per user");
    });

    // Add Authorization with Policies
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));
        options.AddPolicy("RequireAuthentication", policy => policy.RequireAuthenticatedUser());
    });

    // Repository pattern registration
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<ICustomerService, CustomerService>();

    // Authentication services
    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddScoped<IAuthService, AuthService>();

    // AutoMapper registration
    builder.Services.AddAutoMapper(typeof(MappingProfile));

    // FluentValidation registration
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();
    builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

    // Add controllers
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // Add Swagger with JWT support
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "CustomerFluent API", Version = "v1" });

        // Add JWT Authentication to Swagger
        c.AddSecurityDefinition("Bearer", new()
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new()
        {
            {
                new()
                {
                    Reference = new()
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // Add HTTP request logging
    builder.Services.AddHttpLogging(logging =>
    {
        logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
        logging.RequestHeaders.Add("X-Correlation-ID");
        logging.ResponseHeaders.Add("X-Correlation-ID");
    });

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CustomerFluentCORS", policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://localhost:3000") // Add your frontend URLs
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        string adminRole = "Admin";
        string adminUser = "admiin";
        string adminPassword = "Admin123";

        // Ensure role exists
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = adminRole });
        }

        // Ensure user exists
        var user = await userManager.FindByNameAsync(adminUser);
        if (user == null)
        {
            user = new ApplicationUser { UserName = adminUser, Email = "admiin@example.com", EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, adminRole);
            }
        }
        else
        {
            // Ensure user has the role
            if (!await userManager.IsInRoleAsync(user, adminRole))
            {
                await userManager.AddToRoleAsync(user, adminRole);
            }
        }
    }

    // Seed Roles and Admin User
    await SeedRolesAndAdminUser(app);

    // Configure the HTTP request pipeline
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (httpContext, elapsed, ex) => ex != null
            ? LogEventLevel.Error
            : httpContext.Response.StatusCode > 499
                ? LogEventLevel.Error
                : LogEventLevel.Information;

        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.FirstOrDefault());
            diagnosticContext.Set("ConnectionId", httpContext.Connection.Id);
        };
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "CustomerFluent API V1");
        });
        Log.Information("Swagger UI enabled for development environment");
    }

    app.UseHttpsRedirection();
    app.UseCors("CustomerFluentCORS");

    // CRITICAL: Add Rate Limiting Middleware (must be before Authentication)
    app.UseRateLimiter();
    Log.Information("🛡️ Rate limiting middleware enabled");

    // Authentication and Authorization middleware
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("CustomerFluent API configured with FIXED per-user rate limiting");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// FIXED: Simplified and More Reliable Partition Key Method
static string GetFixedPartitionKey(HttpContext httpContext, string context)
{
    try
    {
        string partitionKey;

        // For authenticated users, use their username/ID
        if (httpContext.User?.Identity?.IsAuthenticated == true)
        {
            var userName = httpContext.User.Identity.Name;
            var userId = httpContext.User.FindFirst("sub")?.Value ??
                        httpContext.User.FindFirst("nameid")?.Value ??
                        httpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            partitionKey = $"user:{userName ?? userId ?? "unknown"}";
            Log.Information("🔑 AUTHENTICATED - Key: {PartitionKey} | Context: {Context} | Path: {Path}",
                partitionKey, context, httpContext.Request.Path);
        }
        else
        {
            // For auth endpoints, extract username from request body
            if (context == "auth" && httpContext.Request.Method == "POST")
            {
                var extractedUser = SafeExtractUsernameFromRequest(httpContext);
                if (!string.IsNullOrEmpty(extractedUser))
                {
                    partitionKey = $"auth-user:{extractedUser}";
                    Log.Information("🔑 AUTH USER - Key: {PartitionKey} | User: {ExtractedUser} | Path: {Path}",
                        partitionKey, extractedUser, httpContext.Request.Path);
                }
                else
                {
                    // Fallback to IP for auth
                    var ipAddress = GetClientIpAddress(httpContext);
                    partitionKey = $"auth-ip:{ipAddress}";
                    Log.Information("🔑 AUTH IP - Key: {PartitionKey} | IP: {IP} | Path: {Path}",
                        partitionKey, ipAddress, httpContext.Request.Path);
                }
            }
            else
            {
                // For other anonymous requests, use IP
                var ipAddress = GetClientIpAddress(httpContext);
                partitionKey = $"anon:{ipAddress}";
                Log.Information("🔑 ANONYMOUS - Key: {PartitionKey} | IP: {IP} | Path: {Path}",
                    partitionKey, ipAddress, httpContext.Request.Path);
            }
        }

        return partitionKey;
    }
    catch (Exception ex)
    {
        Log.Error(ex, "❌ Error getting partition key, using fallback");
        return $"fallback:{GetClientIpAddress(httpContext)}";
    }
}

// IMPROVED: More reliable username extraction
static string? SafeExtractUsernameFromRequest(HttpContext httpContext)
{
    try
    {
        if (httpContext.Request.Method != "POST" ||
            !httpContext.Request.Path.StartsWithSegments("/api/auth"))
        {
            return null;
        }

        // Check if body can be read
        if (httpContext.Request.ContentLength == 0 || 
            httpContext.Request.ContentLength == null)
        {
            return null;
        }

        // Enable buffering
        httpContext.Request.EnableBuffering();
        
        var originalPosition = httpContext.Request.Body.Position;
        httpContext.Request.Body.Position = 0;

        using var reader = new StreamReader(httpContext.Request.Body, leaveOpen: true);
        var body = reader.ReadToEndAsync().GetAwaiter().GetResult(); // Use async version
        httpContext.Request.Body.Position = originalPosition;

        if (string.IsNullOrEmpty(body))
        {
            Log.Debug("🔍 Empty request body");
            return null;
        }

        Log.Debug("🔍 Request body: {Body}", body);

        // More robust JSON parsing
        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(body);
            if (document.RootElement.TryGetProperty("userName", out var userNameElement))
            {
                var username = userNameElement.GetString();
                Log.Information("✅ Extracted userName: {UserName}", username);
                return username;
            }
            
            if (document.RootElement.TryGetProperty("email", out var emailElement))
            {
                var email = emailElement.GetString();
                Log.Information("✅ Extracted email: {Email}", email);
                return email;
            }
        }
        catch (System.Text.Json.JsonException ex)
        {
            Log.Warning("Failed to parse JSON, falling back to regex: {Error}", ex.Message);
            
            // Fallback to regex
            var userNameMatch = Regex.Match(body, "\"userName\"\\s*:\\s*\"([^\"]+)\"", RegexOptions.IgnoreCase);
            if (userNameMatch.Success)
            {
                var username = userNameMatch.Groups[1].Value;
                Log.Information("✅ Regex extracted userName: {UserName}", username);
                return username;
            }
        }

        Log.Warning("❌ Could not extract username from request body");
        return null;
    }
    catch (Exception ex)
    {
        Log.Error(ex, "❌ Error extracting username from request");
        return null;
    }
}

// Helper to get client IP address
static string GetClientIpAddress(HttpContext httpContext)
{
    // Try to get real IP from headers (for reverse proxies)
    var xForwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
    if (!string.IsNullOrEmpty(xForwardedFor))
    {
        return xForwardedFor.Split(',')[0].Trim();
    }

    var xRealIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
    if (!string.IsNullOrEmpty(xRealIp))
    {
        return xRealIp;
    }

    // Fallback to connection remote IP
    return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip";
}

// Seed initial roles and admin user + test users
static async Task SeedRolesAndAdminUser(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Create roles if they don't exist
        string[] roles = { "Admin", "User", "Manager" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole
                {
                    Name = roleName,
                    Description = $"{roleName} role with specific permissions"
                };

                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    logger.LogInformation("Role {RoleName} created successfully", roleName);
                }
                else
                {
                    logger.LogError("Error creating role {RoleName}: {Errors}",
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }

        // Create admin user if it doesn't exist
        const string adminEmail = "admin@customerFluent.com";
        const string adminUserName = "admin";
        const string adminPassword = "Admin123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                FirstName = "System",
                LastName = "Administrator",
                Email = adminEmail,
                UserName = adminUserName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Admin user created successfully with email: {AdminEmail}", adminEmail);
            }
            else
            {
                logger.LogError("Error creating admin user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // Create test users with User role for rate limiting tests
        var testUsers = new[]
        {
            new { UserName = "testuser1", Email = "testuser1@example.com", FirstName = "Test", LastName = "User1" },
            new { UserName = "testuser2", Email = "testuser2@example.com", FirstName = "Test", LastName = "User2" },
            new { UserName = "testuser3", Email = "testuser3@example.com", FirstName = "Test", LastName = "User3" }
        };

        foreach (var testUserInfo in testUsers)
        {
            var existingUser = await userManager.FindByNameAsync(testUserInfo.UserName);
            if (existingUser == null)
            {
                var testUser = new ApplicationUser
                {
                    FirstName = testUserInfo.FirstName,
                    LastName = testUserInfo.LastName,
                    Email = testUserInfo.Email,
                    UserName = testUserInfo.UserName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(testUser, "TestUser123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(testUser, "User");
                    logger.LogInformation("Test user {UserName} created successfully", testUserInfo.UserName);
                }
                else
                {
                    logger.LogError("Error creating test user {UserName}: {Errors}",
                        testUserInfo.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error seeding roles and users");
    }
}
