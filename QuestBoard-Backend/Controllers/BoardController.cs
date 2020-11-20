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
                return BadRequest("Board name is invalid or null");
            }

            User user = await _userManager.GetUserAsync(HttpContext.User).ConfigureAwait(false);

            if (user == null)
            {
                return NotFound("User does not exist");
            }

            Board board = new Board()
            {
                Owner = user,
                BoardName = BoardName,
            };

            _context.Boards.Add(board);

            _context.SaveChanges();

            return Ok(new { id = board.Id });
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
                return NotFound(new { Success = false });
            }

            User user = await _userManager.GetUserAsync(HttpContext.User).ConfigureAwait(false);

            MemberOf memberOf = _context.MemberOf.ToList().Where(m => m.MemberID == user.Id && m.BoardId == board.Id).FirstOrDefault();

            if (memberOf == null && board.Owner != user)
            {
                return Forbid();
            }

            Column ToAdd = new Column
            {
                Category = column.Category,
                BoardId = board.Id,
            };

            _context.Columns.Add(ToAdd);
            board.Columns.Append(ToAdd);
            await _context.SaveChangesAsync();

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

            if (memberOf == null && board.Owner.Id != user.Id)
            {
                return Forbid();
            }

            User Assignee = await _userManager.FindByEmailAsync(card.AssigneeEmail);

            Card toAdd = new Card()
            {
                Description = card.Description,
                Title = card.Title,
                Assigned = Assignee,
                ColumnId = card.ColumnID,
            };

            _context.Cards.Add(toAdd);
            await _context.SaveChangesAsync();

            column.Cards.Add(toAdd);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true });
        }

        public class CardsInfo
        {
            public string Id { get; set; }

            public string Description { get; set; }
            public string Label { get; set; }

            public string Title { get; set; }
        }

        public class ColumnInfo
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Label { get; set; }
            public ICollection<CardsInfo> Cards { get; set; }
        }

        public class BoardInfo
        {
            public string Name { get; set; }
            public ICollection<ColumnInfo> Lanes { get; set; }

            public string Id { get; set; }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetBoardsAsync()
        {
            User user = await _userManager.GetUserAsync(HttpContext.User).ConfigureAwait(false);

            IEnumerable<int> memberOfBoards = _context.MemberOf.Where(m => m.MemberID == user.Id).Select(m => m.BoardId).ToList();

            IEnumerable<Board> boardas = _context.Boards.Where(b => b.Owner == user || memberOfBoards.Contains(b.Id)).ToList();

            ICollection<BoardInfo> boards = new List<BoardInfo>();

            foreach (Board b in boardas)
            {
                BoardInfo boardInfo = new BoardInfo
                {
                    Name = b.BoardName,
                    Id = b.Id.ToString(),
                    Lanes = new List<ColumnInfo>(),
                };

                int totalCount = 0;

                b.Columns = _context.Columns.Where(col => col.BoardId == b.Id).ToList();

                foreach (Column c in b.Columns)
                {
                    ColumnInfo column = new ColumnInfo
                    {
                        Id = c.ID.ToString(),
                        Title = c.Category,
                        Cards = new List<CardsInfo>(),
                    };

                    c.Cards = _context.Cards.Where(card => card.ColumnId == c.ID).ToList();

                    foreach (Card ca in c.Cards)
                    {
                        CardsInfo info = new CardsInfo
                        {
                            Description = ca.Description,
                            Id = ca.ID.ToString(),
                            Title = ca.Title,
                        };

                        column.Cards.Add(info);
                        totalCount++;
                    }

                    boardInfo.Lanes.Add(column);
                }

                foreach (ColumnInfo c in boardInfo.Lanes)
                {
                    c.Label = string.Format("{0}/{1}", c.Cards.Count, totalCount);
                }

                boards.Add(boardInfo);
            }

            return Ok(boards);
        }

        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetBoardInfoAsync(int id)
        {
            Board board = await _context.Boards.FindAsync(id);

            if (board == null)
            {
                return NotFound("No board with that id exists");
            }

            User user = await _userManager.GetUserAsync(HttpContext.User).ConfigureAwait(false);

            if (board.Owner != user || board.Members.Any(u => u.MemberID == user.Id))
            {
                return Forbid("Not Allowed to Acccess this Board");
            }

            return Ok(JsonConvert.SerializeObject(board));
        }

        public class CardMovement
        {
            public int CardID { get; set; }
            public int NewColumnID { get; set; }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MoveCardAsync([FromBody]CardMovement cardMovement) 
        {
            Column NewColumn = await _context.Columns.FindAsync(cardMovement.NewColumnID);
            Card ToMove = await _context.Cards.FindAsync(cardMovement.CardID);
            Column OldColumn = await _context.Columns.FindAsync(ToMove.ColumnId);

            ToMove.ColumnId = NewColumn.ID;

            OldColumn.Cards.Remove(ToMove);
            NewColumn.Cards.Append(ToMove);

            await _context.SaveChangesAsync();

            return Ok(new { card = ToMove.ID, column = NewColumn.ID });
        }

        [Authorize]
        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCardAsync(int CardID)
        {
            Card ToDelete = await _context.Cards.FindAsync(CardID);

            Column ToRemoveFrom = await _context.Columns.FindAsync(ToDelete.ColumnId);

            ToRemoveFrom.Cards.Remove(ToDelete);

            _context.Cards.Remove(ToDelete);

            await _context.SaveChangesAsync();

            return Ok(new { Success = true });
        }
    }
}
