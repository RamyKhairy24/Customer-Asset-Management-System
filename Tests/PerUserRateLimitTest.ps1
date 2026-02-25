# Per-User Rate Limiting Test
param(
    [string]$BaseUrl = "https://localhost:7262"
)

Write-Host "🚀 Per-User Rate Limiting Test" -ForegroundColor Green
Write-Host "==============================" -ForegroundColor Green
Write-Host "Testing rate limiting per user account" -ForegroundColor Yellow
Write-Host ""

# Certificate bypass
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

Write-Host "📋 Expected behavior:" -ForegroundColor Cyan
Write-Host "   • Each user should have their own rate limit bucket" -ForegroundColor White
Write-Host "   • User 'admin' gets 10 tokens, 2 per minute" -ForegroundColor White
Write-Host "   • Different users don't interfere with each other" -ForegroundColor White
Write-Host ""

# Test 1: Admin user rapid requests
Write-Host "🔍 Test 1: Admin user (should hit rate limit)" -ForegroundColor Cyan

$adminData = @{
    userName = "admin"
    password = "Admin123!"
    rememberMe = $false
} | ConvertTo-Json

$adminResults = @()

for ($i = 1; $i -le 12; $i++) {
    Write-Host "Admin Request $($i.ToString().PadLeft(2))..." -NoNewline
    
    try {
        if ($PSVersionTable.PSVersion.Major -ge 6) {
            $response = Invoke-WebRequest -Uri "$BaseUrl/api/auth/login" -Method POST -Body $adminData -Headers $headers -SkipCertificateCheck -ErrorAction Stop
        } else {
            $response = Invoke-WebRequest -Uri "$BaseUrl/api/auth/login" -Method POST -Body $adminData -Headers $headers -ErrorAction Stop
        }
        
        Write-Host " ✅ $($response.StatusCode)" -ForegroundColor Green
        $adminResults += @{ RequestNumber = $i; Status = $response.StatusCode; Success = $true; User = "admin" }
        
    } catch {
        $statusCode = if ($_.Exception.Response) { $_.Exception.Response.StatusCode.value__ } else { 0 }
        
        if ($statusCode -eq 429) {
            Write-Host " ⛔ 429 RATE LIMITED!" -ForegroundColor Red
        } else {
            Write-Host " ❌ $statusCode" -ForegroundColor Yellow
        }
        
        $adminResults += @{ RequestNumber = $i; Status = $statusCode; Success = $false; User = "admin" }
    }
    
    Start-Sleep -Milliseconds 50  # Small delay
}

# Test 2: Different user (should NOT be affected by admin's rate limit)
Write-Host ""
Write-Host "🔍 Test 2: Different user attempts (should succeed)" -ForegroundColor Cyan

# Try a different username that doesn't exist (for testing partitioning)
$testUserData = @{
    userName = "testuser"
    password = "WrongPassword123!"
    rememberMe = $false
} | ConvertTo-Json

$testResults = @()

for ($i = 1; $i -le 5; $i++) {
    Write-Host "Test User Request $($i.ToString().PadLeft(1))..." -NoNewline
    
    try {
        if ($PSVersionTable.PSVersion.Major -ge 6) {
            $response = Invoke-WebRequest -Uri "$BaseUrl/api/auth/login" -Method POST -Body $testUserData -Headers $headers -SkipCertificateCheck -ErrorAction Stop
        } else {
            $response = Invoke-WebRequest -Uri "$BaseUrl/api/auth/login" -Method POST -Body $testUserData -Headers $headers -ErrorAction Stop
        }
        
        Write-Host " ✅ $($response.StatusCode)" -ForegroundColor Green
        $testResults += @{ RequestNumber = $i; Status = $response.StatusCode; Success = $true; User = "testuser" }
        
    } catch {
        $statusCode = if ($_.Exception.Response) { $_.Exception.Response.StatusCode.value__ } else { 0 }
        
        if ($statusCode -eq 429) {
            Write-Host " ⛔ 429 RATE LIMITED!" -ForegroundColor Red
        } elseif ($statusCode -eq 401) {
            Write-Host " 🔒 401 UNAUTHORIZED (expected)" -ForegroundColor Blue
        } else {
            Write-Host " ❌ $statusCode" -ForegroundColor Yellow
        }
        
        $testResults += @{ RequestNumber = $i; Status = $statusCode; Success = ($statusCode -eq 401); User = "testuser" }
    }
    
    Start-Sleep -Milliseconds 50
}

# Results Analysis
Write-Host ""
Write-Host "📊 Results Analysis:" -ForegroundColor Cyan
Write-Host "===================" -ForegroundColor Cyan

$adminSuccess = ($adminResults | Where-Object { $_.Success }).Count
$adminRateLimited = ($adminResults | Where-Object { $_.Status -eq 429 }).Count

$testSuccess = ($testResults | Where-Object { $_.Success }).Count
$testRateLimited = ($testResults | Where-Object { $_.Status -eq 429 }).Count

Write-Host "Admin user (12 requests):" -ForegroundColor White
Write-Host "  ✅ Successful: $adminSuccess" -ForegroundColor Green
Write-Host "  ⛔ Rate Limited: $adminRateLimited" -ForegroundColor Red

Write-Host "Test user (5 requests):" -ForegroundColor White
Write-Host "  ✅ Successful: $testSuccess" -ForegroundColor Green
Write-Host "  ⛔ Rate Limited: $testRateLimited" -ForegroundColor Red

Write-Host ""
if ($adminRateLimited -gt 0 -and $testRateLimited -eq 0) {
    Write-Host "🎉 SUCCESS! Per-user rate limiting is working correctly!" -ForegroundColor Green
    Write-Host "   • Admin user hit rate limit after ~10 requests" -ForegroundColor Green
    Write-Host "   • Test user was NOT affected by admin's rate limit" -ForegroundColor Green
} elseif ($adminRateLimited -gt 0 -and $testRateLimited -gt 0) {
    Write-Host "⚠️  Partial success: Rate limiting works, but users may be sharing buckets" -ForegroundColor Yellow
} else {
    Write-Host "❌ Per-user rate limiting not working as expected" -ForegroundColor Red
}

Write-Host ""
Write-Host "🎯 Per-user rate limiting test completed!" -ForegroundColor Green