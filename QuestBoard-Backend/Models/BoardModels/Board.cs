﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuestBoard.Models
{
    public class Board
    {
        public Board()
        {
            Columns = new List<Column>();
            Members = new List<MemberOf>();
        }

        [Key]
        public int Id { get; set; }

        public string BoardName { get; set; }

        public ICollection<Column> Columns { get; set; }

        public virtual ICollection<MemberOf> Members { get; set; }

        public User Owner { get; set; }
    }
}
