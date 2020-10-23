using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QuestBoard.Models;

namespace QuestBoard_Backend.Models.BoardModels
{
    public class Board
    {
        public Board()
        {
            Members = new List<User>();
        }

        [Key]
        public int Id { get; set; }

        public string BoardName { get; set; }

        public List<string> Columns { get; set; }

        public virtual ICollection<User> Members { get; set; }

        public User Owner { get; set; }
    }
}
