# Aggressive Rate Limiting Test - Very Low Limits
param(
    [string]$BaseUrl = "https://localhost:7262",
    [int]$RequestCount = 5,  # Only 5 requests
    [int]$DelayBetweenRequests = 10,  # Almost no delay
    [string]$TestEndpoint = "login"
)

Write-Host "🚀 AGGRESSIVE Rate Limiting Test" -ForegroundColor Red
Write-Host "================================" -ForegroundColor Red
Write-Host "Testing with 5 rapid requests to force rate limiting" -ForegroundColor Yellow
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

$results = @()

for ($i = 1; $i -le $RequestCount; $i++) {
    Write-Host "Request $i..." -NoNewline
    
    try {
        $uri = "$BaseUrl/api/auth/$TestEndpoint"
        
        if ($PSVersionTable.PSVersion.Major -ge 6) {
            $response = Invoke-WebRequest -Uri $uri -Method POST -Body $loginData -Headers $headers -SkipCertificateCheck -ErrorAction Stop
        } else {
            $response = Invoke-WebRequest -Uri $uri -Method POST -Body $loginData -Headers $headers -ErrorAction Stop
        }
        
        Write-Host " ✅ $($response.StatusCode)" -ForegroundColor Green
        $results += @{ Status = $response.StatusCode; Success = $true }
        
    } catch {
        $statusCode = if ($_.Exception.Response) { $_.Exception.Response.StatusCode.value__ } else { 0 }
        
        if ($statusCode -eq 429) {
            Write-Host " ⛔ 429 RATE LIMITED!" -ForegroundColor Red
        } else {
            Write-Host " ❌ $statusCode" -ForegroundColor Yellow
        }
        
        $results += @{ Status = $statusCode; Success = $false }
    }
    
    if ($i -lt $RequestCount) {
        Start-Sleep -Milliseconds $DelayBetweenRequests
    }
}

$successCount = ($results | Where-Object { $_.Success }).Count
$rateLimitedCount = ($results | Where-Object { $_.Status -eq 429 }).Count

Write-Host ""
Write-Host "Results: $successCount successful, $rateLimitedCount rate limited" -ForegroundColor Cyan
if ($rateLimitedCount -gt 0) {
    Write-Host "✅ RATE LIMITING IS WORKING!" -ForegroundColor Green
} else {
    Write-Host "❌ Rate limiting NOT working" -ForegroundColor Red
}