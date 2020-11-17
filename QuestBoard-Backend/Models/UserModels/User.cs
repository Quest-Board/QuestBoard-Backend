using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace QuestBoard.Models
{
    public class User : IdentityUser
    {
        public ICollection<MemberOf> Boards { get; set; }
        public int Points { get; set; }
        public UserRank Rank { get; set; }
    }

    public enum UserRank
    {
        Squire,
        Knight,
        King
    }
}
