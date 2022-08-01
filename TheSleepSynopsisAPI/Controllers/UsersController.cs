using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheSleepSynopsisAPI.Domain.Models;
using TheSleepSynopsisAPI.Domain.Services;

namespace TheSleepSynopsisAPI.Controllers
{
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Returns a list of all registered users
        /// </summary>
        /// <param name="includeDeleted">Whether or not to include the UUIDS of users who have deleted their accounts</param>
        [HttpGet]
        [Produces(typeof(ICollection<User>))]
        public async Task<IActionResult> GetAllUsers(bool includeDeleted = false)
        {
            return Ok(await _userService.GetAllUsers(includeDeleted));
        }

        /// <summary>
        /// Returns the registered user with the specified UUID
        /// </summary>
        /// <param name="userUUID">The UUID of the registered user</param>
        [HttpGet("{userUUID}")]
        [Produces(typeof(User))]
        public async Task<IActionResult> GetUser(string userUUID)
        {
            return Ok(await _userService.GetUser(userUUID));
        }

        [HttpPost("Search/{username}")]
        [Produces(typeof(ICollection<User>))]
        public async Task<IActionResult> SearchUserByName(string name, bool matchExactly = false)
        {
            return Ok(await _userService.SearchByName(name, matchExactly));
        }
    }
}

