using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestBoard.Context;
using QuestBoard.Models;
using QuestBoard_Backend.Models.BoardModels;

namespace QuestBoard_Backend.Controllers
{
    [Route("api/[controller]/[method]")]
    public class BoardController : Controller
    {
        private readonly QuestboardContext _context;

        public BoardController(QuestboardContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(string OwnerEmail)
        {
            if (OwnerEmail == null)
            {
                return BadRequest("Email is invalid or null");
            }

            User user = _context.Users.Where(u => u.Email == OwnerEmail).FirstOrDefault();

            if (user == null)
            {
                return NotFound("User does not exist");
            }

            Board board = new Board()
            {
                BoardOwner = user
            };

            _context.SaveChanges();

            return Ok(new { Success = true });
        }
    }
}
