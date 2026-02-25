using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CustomerFluent.Models.Identity;
using CustomerFluent.DTOs;

namespace CustomerFluent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<UserManagementController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetAllUsers()
        {
            try
            {
                var users = _userManager.Users.ToList();
                var userDtos = new List<UserInfoDto>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDtos.Add(new UserInfoDto
                    {
                        Id = user.Id,
                        UserName = user.UserName!,
                        Email = user.Email!,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        FullName = user.FullName,
                        Roles = roles
                    });
                }

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, "An error occurred while retrieving users");
            }
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult<UserInfoDto>> GetUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var userDto = new UserInfoDto
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    Roles = roles
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", id);
                return StatusCode(500, "An error occurred while retrieving user");
            }
        }

        [HttpPost("users/{id}/roles")]
        public async Task<ActionResult> AssignRole(string id, [FromBody] AssignRoleDto assignRoleDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var roleExists = await _roleManager.RoleExistsAsync(assignRoleDto.RoleName);
                if (!roleExists)
                {
                    return BadRequest("Role does not exist");
                }

                var result = await _userManager.AddToRoleAsync(user, assignRoleDto.RoleName);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Role {RoleName} assigned to user {UserId}", assignRoleDto.RoleName, id);
                    return Ok(new { message = "Role assigned successfully" });
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role to user {UserId}", id);
                return StatusCode(500, "An error occurred while assigning role");
            }
        }

        [HttpDelete("users/{id}/roles/{roleName}")]
        public async Task<ActionResult> RemoveRole(string id, string roleName)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var result = await _userManager.RemoveFromRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Role {RoleName} removed from user {UserId}", roleName, id);
                    return Ok(new { message = "Role removed successfully" });
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role from user {UserId}", id);
                return StatusCode(500, "An error occurred while removing role");
            }
        }

        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllRoles()
        {
            try
            {
                var roles = _roleManager.Roles.Select(r => new 
                { 
                    id = r.Id, 
                    name = r.Name, 
                    description = r.Description 
                }).ToList();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles");
                return StatusCode(500, "An error occurred while retrieving roles");
            }
        }
    }

    public class AssignRoleDto
    {
        public string RoleName { get; set; } = null!;
    }
}