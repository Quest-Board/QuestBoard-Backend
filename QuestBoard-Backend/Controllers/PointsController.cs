using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuestBoard.Context;
using QuestBoard.Models;

namespace QuestBoard.Controllers
{
    [Route("api/[controller]/[action]")]
    public class PointsController : Controller
    {
        private readonly QuestboardContext _context;

        private readonly UserManager<User> _userManager;

        public PointsController(QuestboardContext context, UserManager<User> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public class PointsModification
        {
            [Required]
            public int Points { get; set; }

            [Required]
            public string Email { get; set; }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddAsync([FromBody]PointsModification modification)
        {
            User user = await _userManager.FindByEmailAsync(modification.Email);

            if (user == null)
            {
                return NotFound("No user with that email exists");
            }

            user.Points += modification.Points;
            _context.SaveChanges();

            return Ok(new { points = user.Points });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveAsync([FromBody] PointsModification modification)
        {
            User user = await _userManager.FindByEmailAsync(modification.Email);

            if (user == null)
            {
                return NotFound("No user with that email exists");
            }

            user.Points += modification.Points;
            _context.SaveChanges();

            return Ok(new { points = user.Points });
        }
    }
}
