using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuestBoard.Models
{
    public class Column
    {
        [Key]
        public int ID { get; set; }
        public string Category { get; set; }

        public int BoardId { get; set; }
        public ICollection<Card> Cards { get; set; }
    }
}
