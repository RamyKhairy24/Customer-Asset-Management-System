# Test Script: Verify First 2 Requests Succeed for Every User
param(
    [string]$BaseUrl = "https://localhost:7262"
)

Write-Host "🎯 USER RATE LIMIT SUCCESSION TEST" -ForegroundColor Green
Write-Host "==================================" -ForegroundColor Green
Write-Host "Testing that requests 1 & 2 succeed for EVERY user" -ForegroundColor Yellow
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

# Test users (using pre-seeded users from your Program.cs)
$testUsers = @(
    @{ 
        UserName = "testuser1"
        Password = "TestUser123!"
        ExpectedPartitionKey = "auth-user:testuser1"
    },
    @{ 
        UserName = "testuser2"
        Password = "TestUser123!"
        ExpectedPartitionKey = "auth-user:testuser2"
    },
    @{ 
        UserName = "testuser3" 
        Password = "TestUser123!"
        ExpectedPartitionKey = "auth-user:testuser3"
    },
    @{ 
        UserName = "admin"
        Password = "Admin123!"
        ExpectedPartitionKey = "auth-user:admin"
    }
)

# Rate limit settings from your Program.cs
Write-Host "📋 Rate Limit Settings (AuthPolicy):" -ForegroundColor Cyan
Write-Host "   • TokenLimit: 5 tokens initially" -ForegroundColor White
Write-Host "   • ReplenishmentPeriod: 10 seconds" -ForegroundColor White
Write-Host "   • TokensPerPeriod: 2 tokens every 10 seconds" -ForegroundColor White
Write-Host "   • Per-User buckets (auth-user:username)" -ForegroundColor White
Write-Host ""

$overallResults = @()
$allUsersPassedFirst2 = $true

foreach ($user in $testUsers) {
    Write-Host "🔍 Testing User: $($user.UserName)" -ForegroundColor Yellow
    Write-Host "   Expected: Requests 1-5 should succeed, 6+ may get rate limited" -ForegroundColor Gray
    
    $loginData = @{
        userName = $user.UserName
        password = $user.Password
        rememberMe = $false
    } | ConvertTo-Json
    
    $userResults = @()
    $firstTwoSucceeded = $true
    
    # Test exactly 7 requests to see rate limiting pattern
    for ($i = 1; $i -le 7; $i++) {
        Write-Host "   Request $($i.ToString().PadLeft(2))..." -NoNewline
        
        try {
            if ($PSVersionTable.PSVersion.Major -ge 6) {
                $response = Invoke-WebRequest -Uri "$BaseUrl/api/auth/login" -Method POST -Body $loginData -Headers $headers -SkipCertificateCheck -ErrorAction Stop
            } else {
                $response = Invoke-WebRequest -Uri "$BaseUrl/api/auth/login" -Method POST -Body $loginData -Headers $headers -ErrorAction Stop
            }
            
            $statusCode = $response.StatusCode
            $success = $true
            $rateLimited = $false
            
            if ($i -le 2) {
                Write-Host " ✅ $statusCode (EXPECTED SUCCESS)" -ForegroundColor Green
            } else {
                Write-Host " ✅ $statusCode" -ForegroundColor Green
            }
            
        } catch {
            $statusCode = if ($_.Exception.Response) { $_.Exception.Response.StatusCode.value__ } else { 0 }
            $success = $false
            
            if ($statusCode -eq 429) {
                $rateLimited = $true
                if ($i -le 2) {
                    Write-Host " ❌ 429 RATE LIMITED (UNEXPECTED!)" -ForegroundColor Red
                    $firstTwoSucceeded = $false
                } else {
                    Write-Host " ⛔ 429 RATE LIMITED (Expected)" -ForegroundColor Yellow
                }
            } else {
                if ($i -le 2) {
                    Write-Host " ❌ $statusCode (UNEXPECTED FAILURE!)" -ForegroundColor Red
                    $firstTwoSucceeded = $false
                } else {
                    Write-Host " 🔒 $statusCode" -ForegroundColor Blue
                }
            }
        }
        
        $userResults += @{
            User = $user.UserName
            RequestNumber = $i
            StatusCode = $statusCode
            Success = $success
            RateLimited = $rateLimited
            Timestamp = Get-Date
        }
        
        # Small delay between requests
        Start-Sleep -Milliseconds 50
    }
    
    # Analyze user results
    $successfulFirst2 = ($userResults | Where-Object { $_.RequestNumber -le 2 -and $_.Success }).Count
    $rateLimitedFirst2 = ($userResults | Where-Object { $_.RequestNumber -le 2 -and $_.RateLimited }).Count
    
    Write-Host "   📊 First 2 requests: $successfulFirst2/2 successful, $rateLimitedFirst2/2 rate limited" -ForegroundColor Cyan
    
    if ($successfulFirst2 -eq 2) {
        Write-Host "   ✅ PASSED: First 2 requests succeeded" -ForegroundColor Green
    } else {
        Write-Host "   ❌ FAILED: First 2 requests did not succeed" -ForegroundColor Red
        $allUsersPassedFirst2 = $false
    }
    
    $overallResults += $userResults
    Write-Host ""
    
    # Wait before testing next user to reset any shared rate limits
    Write-Host "   ⏳ Waiting 2 seconds before next user..." -ForegroundColor Gray
    Start-Sleep -Seconds 2
}

# Final Analysis
Write-Host "🎯 FINAL SUCCESSION TEST RESULTS:" -ForegroundColor Magenta
Write-Host "==================================" -ForegroundColor Magenta

foreach ($user in $testUsers) {
    $userTestResults = $overallResults | Where-Object { $_.User -eq $user.UserName }
    $first2Results = $userTestResults | Where-Object { $_.RequestNumber -le 2 }
    $successCount = ($first2Results | Where-Object { $_.Success }).Count
    
    Write-Host "$($user.UserName): " -NoNewline -ForegroundColor White
    if ($successCount -eq 2) {
        Write-Host "✅ PASSED (2/2 successful)" -ForegroundColor Green
    } else {
        Write-Host "❌ FAILED ($successCount/2 successful)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "🏆 OVERALL RESULT:" -ForegroundColor Magenta
if ($allUsersPassedFirst2) {
    Write-Host "✅ SUCCESS: All users had their first 2 requests succeed!" -ForegroundColor Green
    Write-Host "   Per-user rate limiting is working correctly" -ForegroundColor Green
} else {
    Write-Host "❌ FAILURE: Some users did not have their first 2 requests succeed!" -ForegroundColor Red
    Write-Host "   Rate limiting may be too aggressive or incorrectly configured" -ForegroundColor Red
}

# Additional insights
$totalFirst2Requests = $testUsers.Count * 2
$successfulFirst2Requests = ($overallResults | Where-Object { $_.RequestNumber -le 2 -and $_.Success }).Count
$rateLimitedFirst2Requests = ($overallResults | Where-Object { $_.RequestNumber -le 2 -and $_.RateLimited }).Count

Write-Host ""
Write-Host "📈 STATISTICS:" -ForegroundColor Cyan
Write-Host "   Total first-2 requests tested: $totalFirst2Requests" -ForegroundColor White
Write-Host "   Successful first-2 requests: $successfulFirst2Requests" -ForegroundColor Green
Write-Host "   Rate limited first-2 requests: $rateLimitedFirst2Requests" -ForegroundColor Red
Write-Host "   Success rate: $(($successfulFirst2Requests / $totalFirst2Requests * 100).ToString('F1'))%" -ForegroundColor Yellow

Write-Host ""
Write-Host "🔍 DEBUGGING TIPS:" -ForegroundColor Gray
Write-Host "• Check application console for partition key logs" -ForegroundColor Gray
Write-Host "• Look for: 'AUTH USER - Key: auth-user:username'" -ForegroundColor Gray
Write-Host "• Ensure each user gets their own bucket" -ForegroundColor Gray
Write-Host "• With TokenLimit=5, first 5 requests should always succeed" -ForegroundColor Gray

Write-Host ""
Write-Host "🎉 Succession test completed!" -ForegroundColor Green

#.\Tests\UserRateLimitSuccessionTest.ps1