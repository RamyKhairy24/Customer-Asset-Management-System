using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CustomerFluent.DTOs;
using CustomerFluent.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.RateLimiting;

namespace CustomerFluent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("AuthPolicy")] // Add this for auth endpoints
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid registration attempt with validation errors");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Registration attempt for username: {UserName}", registerDto.UserName);
                
                var result = await _authService.RegisterAsync(registerDto);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation("User {UserName} registered successfully", registerDto.UserName);
                    return Ok(result);
                }

                _logger.LogWarning("Registration failed for username: {UserName}. Reason: {Message}", 
                    registerDto.UserName, result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for username: {UserName}", registerDto.UserName);
                return StatusCode(500, new AuthResponseDto 
                { 
                    IsSuccess = false, 
                    Message = "An error occurred during registration" 
                });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login attempt with validation errors");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Login attempt for username: {UserName}", loginDto.UserName);
                
                var result = await _authService.LoginAsync(loginDto);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation("User {UserName} logged in successfully", loginDto.UserName);
                    return Ok(result);
                }

                _logger.LogWarning("Login failed for username: {UserName}. Reason: {Message}", 
                    loginDto.UserName, result.Message);
                return Unauthorized(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for username: {UserName}", loginDto.UserName);
                return StatusCode(500, new AuthResponseDto 
                { 
                    IsSuccess = false, 
                    Message = "An error occurred during login" 
                });
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult<AuthResponseDto>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new AuthResponseDto 
                    { 
                        IsSuccess = false, 
                        Message = "User not found" 
                    });
                }

                _logger.LogInformation("Password change attempt for user: {UserId}", userId);
                
                var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation("Password changed successfully for user: {UserId}", userId);
                    return Ok(result);
                }

                _logger.LogWarning("Password change failed for user: {UserId}. Reason: {Message}", 
                    userId, result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change");
                return StatusCode(500, new AuthResponseDto 
                { 
                    IsSuccess = false, 
                    Message = "An error occurred during password change" 
                });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Forgot password request for email: {Email}", forgotPasswordDto.Email);
                
                var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
                
                // Always return success for security reasons (don't reveal if email exists)
                return Ok(new { message = "If the email exists, a password reset link has been sent." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password request for email: {Email}", forgotPasswordDto.Email);
                return StatusCode(500, new { message = "An error occurred processing your request" });
            }
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Password reset attempt for email: {Email}", resetPasswordDto.Email);
                
                var result = await _authService.ResetPasswordAsync(resetPasswordDto);
                
                if (result)
                {
                    _logger.LogInformation("Password reset successful for email: {Email}", resetPasswordDto.Email);
                    return Ok(new { message = "Password has been reset successfully" });
                }

                _logger.LogWarning("Password reset failed for email: {Email}", resetPasswordDto.Email);
                return BadRequest(new { message = "Invalid or expired reset token" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for email: {Email}", resetPasswordDto.Email);
                return StatusCode(500, new { message = "An error occurred during password reset" });
            }
        }

        [HttpGet("user-info")]
        [Authorize]
        public async Task<ActionResult<UserInfoDto>> GetUserInfo()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var userInfo = await _authService.GetUserInfoAsync(userId);
                if (userInfo == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user info");
                return StatusCode(500, new { message = "An error occurred getting user information" });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                _logger.LogInformation("Logout request for user: {UserId}", userId);
                
                var result = await _authService.LogoutAsync(userId);
                
                if (result)
                {
                    _logger.LogInformation("User {UserId} logged out successfully", userId);
                    return Ok(new { message = "Logged out successfully" });
                }

                return BadRequest(new { message = "Logout failed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }

        [HttpGet("validate-token")]
        [Authorize]
        public ActionResult ValidateToken()
        {
            try
            {
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                return Ok(new 
                { 
                    isValid = true, 
                    userId = userId,
                    userName = userName,
                    message = "Token is valid" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return Unauthorized(new { isValid = false, message = "Invalid token" });
            }
        }
    }
}