using Microsoft.AspNetCore.Identity;
using CustomerFluent.Models.Identity;
using CustomerFluent.DTOs;
using AutoMapper;

namespace CustomerFluent.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<UserInfoDto?> GetUserInfoAsync(string userId);
        Task<bool> LogoutAsync(string userId);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService,
            IMapper mapper,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Check if user exists
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "User with this email already exists"
                    };
                }

                existingUser = await _userManager.FindByNameAsync(registerDto.UserName);
                if (existingUser != null)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Username is already taken"
                    };
                }

                // Create new user
                var user = new ApplicationUser
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email,
                    UserName = registerDto.UserName,
                    PhoneNumber = registerDto.PhoneNumber,
                    EmailConfirmed = true // For demo purposes
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    // Add default role
                    await _userManager.AddToRoleAsync(user, "User");

                    _logger.LogInformation("User {UserName} registered successfully", user.UserName);

                    var roles = await _userManager.GetRolesAsync(user);
                    var token = await _jwtService.GenerateTokenAsync(user, roles);

                    return new AuthResponseDto
                    {
                        IsSuccess = true,
                        Message = "Registration successful",
                        Token = token,
                        Expiration = DateTime.UtcNow.AddMinutes(60), // Match your JWT expiration
                        User = new UserInfoDto
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            FullName = user.FullName,
                            Roles = roles
                        }
                    };
                }

                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Registration failed. Please try again."
                };
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginDto.UserName);
                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Invalid username or password"
                    };
                }

                if (!user.IsActive)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Account is deactivated"
                    };
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

                if (result.Succeeded)
                {
                    user.LastLoginDate = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    var roles = await _userManager.GetRolesAsync(user);
                    var token = await _jwtService.GenerateTokenAsync(user, roles);

                    _logger.LogInformation("User {UserName} logged in successfully", user.UserName);

                    return new AuthResponseDto
                    {
                        IsSuccess = true,
                        Message = "Login successful",
                        Token = token,
                        Expiration = DateTime.UtcNow.AddMinutes(60),
                        User = new UserInfoDto
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            FullName = user.FullName,
                            Roles = roles
                        }
                    };
                }

                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid username or password"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Login failed. Please try again."
                };
            }
        }

        public async Task<AuthResponseDto> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "User not found"
                    };
                }

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Password changed for user {UserName}", user.UserName);
                    return new AuthResponseDto
                    {
                        IsSuccess = true,
                        Message = "Password changed successfully"
                    };
                }

                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Password change failed. Please try again."
                };
            }
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
                if (user == null)
                {
                    // Don't reveal that the user doesn't exist
                    return true;
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                
                // TODO: Send email with reset token
                // For now, just log it (in production, implement email service)
                _logger.LogInformation("Password reset token for user {Email}: {Token}", user.Email, token);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in forgot password for email {Email}", forgotPasswordDto.Email);
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
                if (user == null)
                {
                    return false;
                }

                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Password reset successful for user {Email}", user.Email);
                    return true;
                }

                _logger.LogWarning("Password reset failed for user {Email}: {Errors}", 
                    user.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in reset password for email {Email}", resetPasswordDto.Email);
                return false;
            }
        }

        public async Task<UserInfoDto?> GetUserInfoAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return null;
                }

                var roles = await _userManager.GetRolesAsync(user);

                return new UserInfoDto
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    Roles = roles
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user info for user {UserId}", userId);
                return null;
            }
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User {UserId} logged out", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user {UserId}", userId);
                return false;
            }
        }
    }
}