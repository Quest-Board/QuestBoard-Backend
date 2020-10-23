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
            TeamMembers = new List<User>();
        }

        [Key]
        public int Id { get; set; }

        public string BoardOwnerEmail { get; set; }

        public List<string> columns { get; set; }

        public virtual ICollection<User> TeamMembers { get; set; }

        public User BoardOwner { get; set; }
    }
}
