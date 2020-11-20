using System.ComponentModel.DataAnnotations;

namespace QuestBoard.Models
{
    public class Card
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int ColumnId { get; set; }
        public User Assigned { get; set; }
    }
}
