using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuestBoard.Models
{
    public class Board
    {
        [Key]
        public int Id { get; set; }

        public string BoardName { get; set; }

        public List<string> Columns { get; set; }

        public virtual ICollection<MemberOf> Members { get; set; }

        public User Owner { get; set; }
    }
}
