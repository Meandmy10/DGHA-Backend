using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace API.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets all roles
        /// </summary>
        /// <returns>All Roles</returns>
        /// <response code="200">Returns all roles</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IEnumerable<IdentityRole>> GetRoles()
        {
            return await _roleManager.Roles.ToListAsync()
                                           .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets requested role
        /// </summary>
        /// <param name="roleId">Role Id</param>
        /// <returns>Requested role</returns>
        /// <response code="200">Returns requested role</response>
        /// <response code="404">Role not found</response>
        [HttpGet("{roleId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IdentityRole> GetRole(string roleId)
        {
            return await _roleManager.FindByIdAsync(roleId)
                                     .ConfigureAwait(false);
        }

        /// <summary>
        /// Posts new role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns>Newly created role</returns>
        /// <response code="201">Returns Created Role</response>
        /// <response code="400">Returns request errors</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IdentityRole>> PostRole(string roleName)
        {
            IdentityRole role = new IdentityRole(roleName);

            var result = await _roleManager.CreateAsync(role)
                                           .ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var createdRole = await _roleManager.FindByNameAsync(roleName)
                                                  .ConfigureAwait(false);

            return CreatedAtAction("GetRole", new { roleId = createdRole.Id }, createdRole);
        }

        /// <summary>
        /// Put updated role
        /// NOTE: Not Impimented yet
        /// </summary>
        /// <param name="roleId">Role Id</param>
        /// <param name="role">Updated Role</param>
        [HttpPut("{roleId}")]
        public void PutRole(int roleId, IdentityRole role)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete specified role
        /// </summary>
        /// <param name="roleId">Role Id</param>
        /// <returns>Deleted Role</returns>
        /// <response code="200">Returns Deleted Role</response>
        /// <response code="404">Role not found</response>
        [HttpDelete("{roleId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IdentityRole>> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId)
                                         .ConfigureAwait(false);

            if (role == null)
            {
                return NotFound();
            }

            await _roleManager.DeleteAsync(role)
                              .ConfigureAwait(false);

            return role;
        }
    }
}
