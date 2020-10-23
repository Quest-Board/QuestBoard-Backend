using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuestBoard.Context;
using QuestBoard.Models;
using System;
using System.Threading.Tasks;

namespace QuestBoard.Controllers
{
    [Route("api/[controller]/[action]")]
    public class BoardController : Controller
    {
        private readonly QuestboardContext _context;

        private readonly UserManager<User> _userManager;

        public BoardController(QuestboardContext context, UserManager<User> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(context));
        }

        public IActionResult Index()
        {
            return View();
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
    }
}
