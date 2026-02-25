# Diagnostic Rate Limiting Test - Find the exact issue
param(
    [string]$BaseUrl = "https://localhost:7262"
)

Write-Host "?? DIAGNOSTIC Rate Limiting Test" -ForegroundColor Magenta
Write-Host "=================================" -ForegroundColor Magenta
Write-Host "Testing with EXTREMELY aggressive limits to force rate limiting" -ForegroundColor Yellow
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

$loginData = @{
    userName = "admin"
    password = "Admin123!"
    rememberMe = $false
} | ConvertTo-Json

$headers = @{
    "Content-Type" = "application/json"
    "Accept" = "application/json"
}

# Test 1: Very rapid requests with no delay
Write-Host "?? Test 1: 20 rapid requests with NO delay" -ForegroundColor Cyan
$results = @()

for ($i = 1; $i -le 20; $i++) {
    Write-Host "Request $($i.ToString().PadLeft(2))..." -NoNewline
    
    try {
        $uri = "$BaseUrl/api/auth/login"
        
        if ($PSVersionTable.PSVersion.Major -ge 6) {
            $response = Invoke-WebRequest -Uri $uri -Method POST -Body $loginData -Headers $headers -SkipCertificateCheck -ErrorAction Stop
        } else {
            $response = Invoke-WebRequest -Uri $uri -Method POST -Body $loginData -Headers $headers -ErrorAction Stop
        }
        
        Write-Host " ? $($response.StatusCode)" -ForegroundColor Green
        $results += @{ RequestNumber = $i; Status = $response.StatusCode; Success = $true }
        
    } catch {
        $statusCode = if ($_.Exception.Response) { $_.Exception.Response.StatusCode.value__ } else { 0 }
        
        if ($statusCode -eq 429) {
            Write-Host " ? 429 RATE LIMITED!" -ForegroundColor Red
        } else {
            Write-Host " ? $statusCode" -ForegroundColor Yellow
        }
        
        $results += @{ RequestNumber = $i; Status = $statusCode; Success = $false }
    }
    
    # NO DELAY - this should definitely trigger rate limiting
}

$successCount = ($results | Where-Object { $_.Success }).Count
$rateLimitedCount = ($results | Where-Object { $_.Status -eq 429 }).Count

Write-Host ""
Write-Host "?? Results: $successCount successful, $rateLimitedCount rate limited" -ForegroundColor Cyan

if ($rateLimitedCount -gt 0) {
    Write-Host "? RATE LIMITING IS WORKING!" -ForegroundColor Green
} else {
    Write-Host "? Rate limiting STILL NOT working - checking more..." -ForegroundColor Red
    
    # Test 2: Check if the controller attribute is working
    Write-Host ""
    Write-Host "?? Test 2: Checking if [EnableRateLimiting] attribute is applied..." -ForegroundColor Cyan
    
    # Make a request and check response headers for rate limiting info
    try {
        if ($PSVersionTable.PSVersion.Major -ge 6) {
            $response = Invoke-WebRequest -Uri "$BaseUrl/api/auth/login" -Method POST -Body $loginData -Headers $headers -SkipCertificateCheck -ErrorAction Stop
        } else {
            $response = Invoke-WebRequest -Uri "$BaseUrl/api/auth/login" -Method POST -Body $loginData -Headers $headers -ErrorAction Stop
        }
        
        Write-Host "Response Headers:" -ForegroundColor Gray
        $response.Headers | Format-Table -AutoSize | Out-String | Write-Host -ForegroundColor Gray
        
        # Check for any rate limiting headers
        $rateLimitHeaders = $response.Headers | Where-Object { $_.Key -like "*rate*" -or $_.Key -like "*limit*" }
        if ($rateLimitHeaders) {
            Write-Host "Found rate limiting headers:" -ForegroundColor Green
            $rateLimitHeaders | Format-Table -AutoSize | Out-String | Write-Host -ForegroundColor Green
        } else {
            Write-Host "? No rate limiting headers found" -ForegroundColor Red
        }
        
    } catch {
        Write-Host "Error checking headers: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "?? Possible Issues:" -ForegroundColor Yellow
Write-Host "1. Rate limiting middleware not properly registered" -ForegroundColor Gray
Write-Host "2. [EnableRateLimiting] attribute not working on controller" -ForegroundColor Gray
Write-Host "3. Partition key causing each request to get its own bucket" -ForegroundColor Gray
Write-Host "4. Configuration values not being read correctly" -ForegroundColor Gray
Write-Host "5. Middleware order issue" -ForegroundColor Gray