using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuestBoard.Context;
using QuestBoard.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
                return NotFound("No board with that ID exists");
            }

            User ToAdd = await _userManager.FindByEmailAsync(Email.Email);

            if (ToAdd == null)
            {
                return NotFound("No user with that email exists");
            }

            User user = await _userManager.GetUserAsync(HttpContext.User).ConfigureAwait(false);

            if (board.Owner.Email != user.Email)
            {
                return Forbid("Only the owner is permited to make changes");
            }

            MemberOf MemberOf = new MemberOf
            {
                MemberID = ToAdd.Id,
                Member = ToAdd,
                BoardId = board.Id,
                Board = board
            };

            board.Members.Add(MemberOf);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true });
        }

        public class ColumnAddition
        {
            [Required]
            public string Category { get; set; }

            [Required]
            public int BoardID { get; set; }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddColumnAsync([FromBody]ColumnAddition column)
        {
            Board board = await _context.Boards.FindAsync(column.BoardID);

            if (board == null)
            {
                return NotFound("No board with that ID exists");
            }

            User user = await _userManager.GetUserAsync(HttpContext.User).ConfigureAwait(false);

            MemberOf memberOf = _context.MemberOf.ToList().Where(m => m.MemberID == user.Id && m.BoardId == board.Id).FirstOrDefault();

            if (memberOf == null)
            {
                return Forbid("You are not a member of this board");
            }

            Column ToAdd = new Column()
            {
                Category = column.Category,
            };

            board.Columns.Add(ToAdd);
            _context.SaveChanges();

            return Ok(new { Success = true });
        }

        public class CardAddition
        {
            [Required]
            public int BoardID { get; set; }

            [Required]
            public int ColumnID { get; set; }

            [Required]
            public string Title { get; set; }

            [Required]
            public string Description { get; set; }

            public string AssigneeEmail { get; set; }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCardToColumnAsync([FromBody]CardAddition card)
        {
            Column column = await _context.Columns.FindAsync(card.ColumnID);
            Board board = await _context.Boards.FindAsync(card.BoardID);

            if (column == null)
            {
                return NotFound("No column with that ID exists");
            }

            if (board == null)
            {
                return NotFound("No board with that ID exists");
            }

            User user = await _userManager.GetUserAsync(HttpContext.User).ConfigureAwait(false);

            MemberOf memberOf = _context.MemberOf.ToList().Where(m => m.MemberID == user.Id && m.BoardId == board.Id).FirstOrDefault();

            if (memberOf == null)
            {
                return Forbid("You are not a member of this board");
            }

            User Assignee = await _userManager.FindByEmailAsync(card.AssigneeEmail);

            Card toAdd = new Card()
            {
                Description = card.Description,
                Title = card.Title,
                Assigned = Assignee
            };

            column.Cards.Add(toAdd);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetBoardsAsync()
        {
            User user = await _userManager.GetUserAsync(HttpContext.User).ConfigureAwait(false);

            IEnumerable<Board> board = _context.Boards.ToList().Where(b => b.Owner == user);
            return Ok(JsonConvert.SerializeObject(board));
        }

        [Authorize]
        [HttpGet]
        [Route("{id")]
        public async Task<IActionResult> GetBoardInfoAsync(int id)
        {
            Board board = await _context.Boards.FindAsync(id);

            if (board == null)
            {
                return NotFound("No board with that id exists");
            }

            return Ok(JsonConvert.SerializeObject(board));
        }
    }
}
