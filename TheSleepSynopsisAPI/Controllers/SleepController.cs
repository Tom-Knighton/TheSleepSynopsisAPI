using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheSleepSynopsisAPI.Domain.Models;
using TheSleepSynopsisAPI.Domain.Services;
using TheSleepSynopsisAPI.Utilities;

namespace TheSleepSynopsisAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class SleepController : Controller
    {
        private readonly ISleepService _sleepService;

        public SleepController(ISleepService sleepService)
        {
            _sleepService = sleepService;
        }


        [HttpPost]
        [Produces(typeof(SleepEntry))]
        public async Task<IActionResult> NewSleepEntry([FromBody] SleepEntry newSleep, string? forUserUUID = null)
        {
            return Ok(await _sleepService.CreateSleepEntry(newSleep, forUserUUID ?? AuthUtilities.GetUUIDFromIdentity(User)));
        }

        [HttpGet("Mine")]
        [Produces(typeof(ICollection<SleepEntry>))]
        public async Task<IActionResult> MySleepEntries()
        {
            return Ok(await _sleepService.GetSleepEntriesForUser(AuthUtilities.GetUUIDFromIdentity(User)));
        }

        [HttpGet("User/{userUUID}")]
        [Produces(typeof(ICollection<SleepEntry>))]
        public async Task<IActionResult> GetSleepEntries(string userUUID)
        {
            return Ok(await _sleepService.GetSleepEntriesForUser(userUUID));
        }

        [HttpGet("{sleepEntryUUID}")]
        [Produces(typeof(SleepEntry))]
        public async Task<IActionResult> GetSleepEntry(string sleepEntryUUID)
        {
            return Ok(await _sleepService.GetSleepEntry(sleepEntryUUID));
        }

        [HttpGet("Dreams/{dreamUUID}")]
        [Produces(typeof(Dream))]
        public async Task<IActionResult> GetDream(string dreamUUID)
        {
            return Ok(await _sleepService.GetDream(dreamUUID));
        }

        [HttpPut("Dreams/{dreamUUID}/Modify")]
        [Produces(typeof(Dream))]
        public async Task<IActionResult> ModifyDream(string dreamUUID, [FromBody] EditDreamDTO dto)
        {
            return Ok(await _sleepService.ModifyDream(dto));
        }
    }
}

