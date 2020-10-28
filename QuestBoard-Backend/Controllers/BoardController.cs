using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuestBoard.Context;
using QuestBoard.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace QuestBoard.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        private readonly QuestboardContext _context;

        private readonly UserManager<User> _userManager;

        private readonly ILogger<BoardController> _log;

        public BoardController(QuestboardContext context, UserManager<User> userManager, ILogger<BoardController> log)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(string BoardName)
        {
            if (BoardName == null)
            {
                return BadRequest("Email is invalid or null");
            }

            User user = await _userManager.GetUserAsync(HttpContext.User).ConfigureAwait(false);

            if (user == null)
            {
                return NotFound("User does not exist");
            }

            Board board = new Board()
            {
                Owner = user,
                BoardName = BoardName
            };

            _context.Boards.Add(board);

            _context.SaveChanges();

            return Ok(new { Success = true });
        }

        public class MemberEmail
        {
            [Required]
            [Display(Name = "Email")]
            [EmailAddress]
            public string Email { get; set; }
        }

        [Authorize]
        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> AddMember(int id, [FromBody]MemberEmail Email)
        {
            if (Email == null || Email.Email == null)
            {
                return BadRequest("Email body can not be null");
            }

            Board board = await _context.Boards.FindAsync(id);

            if (board == null)
            {
                Console.WriteLine("No board was found with id {id}", id);
                return NotFound("No board with that ID exists");
            }

            User ToAdd = _context.Users.Find(Email.Email);

            if (ToAdd == null)
            {
                Console.WriteLine("No user was found with email {email}", Email.Email);
                return NotFound("No user with that email exists");
            }

            User user = await _userManager.GetUserAsync(HttpContext.User).ConfigureAwait(false);

            if (board.Owner.Email != user.Email)
            {
                return Forbid("Only the owner is permited to make changes");
            }

            board.Members.Add(ToAdd);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true });
        }
    }
}
