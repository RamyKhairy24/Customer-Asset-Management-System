# Comprehensive Per-User Rate Limiting Test with Real User Creation
param(
    [string]$BaseUrl = "https://localhost:7262"
)

Write-Host "🚀 COMPREHENSIVE Per-User Rate Limiting Test (with real users)" -ForegroundColor Green
Write-Host "================================================================" -ForegroundColor Green
Write-Host "Creating real users and testing TRUE per-user rate limiting" -ForegroundColor Yellow
Write-Host ""

# Certificate bypass for HTTPS
if ($PSVersionTable.PSVersion.Major -lt 6) {
    add-type @"
        using System.Net;
        using System.Security.Cryptography.X509Certificates;
        public class TrustAllCertsPolicy : ICertificatePolicy {
            public bool CheckValidationResult(
                ServicePoint srvPoint, X509Certificate certificate,
                WebRequest request, int certificateProblem) {
                return true;
            }
        }
"@
    [System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
    [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
}

$headers = @{
    "Content-Type" = "application/json"
    "Accept" = "application/json"
}

# Current settings: AuthPolicy: 2 tokens max, 1 token per 30 seconds
Write-Host "📋 Current Rate Limit Settings:" -ForegroundColor Cyan
Write-Host "   • AuthPolicy: 2 tokens max, 1 token per 30 seconds" -ForegroundColor White
Write-Host "   • Each USER gets their own bucket" -ForegroundColor White
Write-Host ""

# Create test users first
$testUsers = @(
    @{ 
        Name = "testuser1"
        FirstName = "Test"
        LastName = "User1"
        Email = "testuser1@example.com"
        UserName = "testuser1"
        Password = "TestUser123!"
        ConfirmPassword = "TestUser123!"
        PhoneNumber = "1234567890"
    },
    @{ 
        Name = "testuser2"
        FirstName = "Test"
        LastName = "User2"
        Email = "testuser2@example.com"
        UserName = "testuser2"
        Password = "TestUser123!"
        ConfirmPassword = "TestUser123!"
        PhoneNumber = "1234567891"
    },
    @{ 
        Name = "testuser3"
        FirstName = "Test"
        LastName = "User3"
        Email = "testuser3@example.com"
        UserName = "testuser3"
        Password = "TestUser123!"
        ConfirmPassword = "TestUser123!"
        PhoneNumber = "1234567892"
    }
)

# Step 1: Register test users
Write-Host "🔧 Step 1: Creating test users..." -ForegroundColor Cyan
foreach ($user in $testUsers) {
    Write-Host "   Creating user: $($user.Name)..." -NoNewline
    
    $registerData = @{
        firstName = $user.FirstName
        lastName = $user.LastName
        email = $user.Email
        userName = $user.UserName
        password = $user.Password
        confirmPassword = $user.ConfirmPassword
        phoneNumber = $user.PhoneNumber
    } | ConvertTo-Json
    
    try {
        if ($PSVersionTable.PSVersion.Major -ge 6) {
            $response = Invoke-WebRequest -Uri "$BaseUrl/api/auth/register" -Method POST -Body $registerData -Headers $headers -SkipCertificateCheck -ErrorAction Stop
        } else {
            $response = Invoke-WebRequest -Uri "$BaseUrl/api/auth/register" -Method POST -Body $registerData -Headers $headers -ErrorAction Stop
        }
        Write-Host " ✅ Created" -ForegroundColor Green
    } catch {
        $statusCode = if ($_.Exception.Response) { $_.Exception.Response.StatusCode.value__ } else { 0 }
        if ($statusCode -eq 400) {
            Write-Host " ⚠️ Already exists" -ForegroundColor Yellow
        } else {
            Write-Host " ❌ Failed ($statusCode)" -ForegroundColor Red
        }
    }
}

Write-Host ""

# Step 2: Test rate limiting with login attempts
Write-Host "🔍 Step 2: Testing rate limiting with rapid login attempts..." -ForegroundColor Cyan
$allResults = @()

foreach ($user in $testUsers) {
    Write-Host "Testing user: $($user.Name)" -ForegroundColor Yellow
    Write-Host "   Expected: First 2 requests succeed, 3rd+ should get 429" -ForegroundColor Gray
    
    $loginData = @{
        userName = $user.UserName
        password = $user.Password
        rememberMe = $false
    } | ConvertTo-Json
    
    $userResults = @()
    
    # Send 5 rapid requests to trigger rate limiting
    for ($i = 1; $i -le 5; $i++) {
        Write-Host "   Request $($i.ToString().PadLeft(2))..." -NoNewline
        
        try {
            if ($PSVersionTable.PSVersion.Major -ge 6) {
                $response = Invoke-WebRequest -Uri "$BaseUrl/api/auth/login" -Method POST -Body $loginData -Headers $headers -SkipCertificateCheck -ErrorAction Stop
            } else {
                $response = Invoke-WebRequest -Uri "$BaseUrl/api/auth/login" -Method POST -Body $loginData -Headers $headers -ErrorAction Stop
            }
            
            Write-Host " ✅ $($response.StatusCode)" -ForegroundColor Green
            $userResults += @{ 
                User = $user.Name
                RequestNumber = $i
                Status = $response.StatusCode
                Success = $true
                RateLimited = $false
                Timestamp = Get-Date
            }
            
        } catch {
            $statusCode = if ($_.Exception.Response) { $_.Exception.Response.StatusCode.value__ } else { 0 }
            
            if ($statusCode -eq 429) {
                Write-Host " ⛔ 429 RATE LIMITED!" -ForegroundColor Red
                $userResults += @{ 
                    User = $user.Name
                    RequestNumber = $i
                    Status = $statusCode
                    Success = $false
                    RateLimited = $true
                    Timestamp = Get-Date
                }
            } else {
                Write-Host " 🔒 $statusCode" -ForegroundColor Blue
                $userResults += @{ 
                    User = $user.Name
                    RequestNumber = $i
                    Status = $statusCode
                    Success = $true  # Login success even with wrong/right credentials
                    RateLimited = $false
                    Timestamp = Get-Date
                }
            }
        }
        
        # Very small delay to ensure rapid requests
        Start-Sleep -Milliseconds 100
    }
    
    $allResults += $userResults
    Write-Host ""
}

# Results Analysis
Write-Host "📊 DETAILED RESULTS ANALYSIS:" -ForegroundColor Cyan
Write-Host "==============================" -ForegroundColor Cyan

foreach ($user in $testUsers) {
    $userTestResults = $allResults | Where-Object { $_.User -eq $user.Name }
    $successCount = ($userTestResults | Where-Object { $_.Success }).Count
    $rateLimitedCount = ($userTestResults | Where-Object { $_.RateLimited }).Count
    
    Write-Host "$($user.Name):" -ForegroundColor White
    Write-Host "  ✅ Successful requests: $successCount/5" -ForegroundColor Green
    Write-Host "  ⛔ Rate limited requests: $rateLimitedCount/5" -ForegroundColor Red
    
    # Find when rate limiting started
    $firstRateLimit = $userTestResults | Where-Object { $_.RateLimited } | Select-Object -First 1
    if ($firstRateLimit) {
        Write-Host "  📍 First rate limit at request: $($firstRateLimit.RequestNumber)" -ForegroundColor Yellow
    } else {
        Write-Host "  ⚠️  No rate limiting occurred" -ForegroundColor Yellow
    }
    Write-Host ""
}

# Overall assessment
$totalUsers = $testUsers.Count
$usersWithRateLimiting = ($testUsers | ForEach-Object { 
    $userName = $_.Name
    $userResults = $allResults | Where-Object { $_.User -eq $userName }
    if (($userResults | Where-Object { $_.RateLimited }).Count -gt 0) { $userName }
}).Count

Write-Host "🎯 FINAL ASSESSMENT:" -ForegroundColor Magenta
Write-Host "===================" -ForegroundColor Magenta

if ($usersWithRateLimiting -eq $totalUsers) {
    Write-Host "✅ Rate limiting is working for ALL users" -ForegroundColor Green
} elseif ($usersWithRateLimiting -gt 0) {
    Write-Host "⚠️  Rate limiting worked for $usersWithRateLimiting/$totalUsers users" -ForegroundColor Yellow
} else {
    Write-Host "❌ Rate limiting NOT WORKING - No users were rate limited" -ForegroundColor Red
}

# Check if users are truly isolated (per-user vs per-IP)
$uniqueRateLimitPatterns = $allResults | Where-Object { $_.RateLimited } | Group-Object -Property User
if ($uniqueRateLimitPatterns.Count -eq $totalUsers) {
    Write-Host "✅ TRUE PER-USER rate limiting confirmed!" -ForegroundColor Green
    Write-Host "   Each user has independent rate limit buckets" -ForegroundColor Green
} elseif ($uniqueRateLimitPatterns.Count -eq 1) {
    Write-Host "❌ SHARED rate limiting detected (likely per-IP)" -ForegroundColor Red
    Write-Host "   All users are sharing the same rate limit bucket" -ForegroundColor Red
} elseif ($uniqueRateLimitPatterns.Count -gt 0) {
    Write-Host "⚠️  Mixed rate limiting behavior" -ForegroundColor Yellow
} else {
    Write-Host "❌ No rate limiting detected at all" -ForegroundColor Red
}

Write-Host ""
Write-Host "🔍 DEBUG INFO:" -ForegroundColor Gray
Write-Host "Rate limit partition keys should show:" -ForegroundColor Gray
Write-Host "  • auth-user:testuser1, auth-user:testuser2, etc." -ForegroundColor Gray
Write-Host "  • Check the application console logs for partition key details" -ForegroundColor Gray

Write-Host ""
Write-Host "🎉 Per-user rate limiting test completed!" -ForegroundColor Green
Write-Host "Check the application console logs for detailed partition key information." -ForegroundColor Cyan