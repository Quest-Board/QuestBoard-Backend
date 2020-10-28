using System;
using QuestBoard.Models;

namespace QuestBoard.Models
{
    public class MemberOf
    {
        public string MemberID { get; set; }
        public User Member { get; set; }

        public int BoardId { get; set; }
        public Board Board { get; set; }
    }
}
