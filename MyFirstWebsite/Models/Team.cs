using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFirstWebsite.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public int DraftPosition { get; set; }

        public int DraftId { get; set; }
        public Draft Draft { get; set; }

        public ICollection<Player> Players { get; set; }

    }
}