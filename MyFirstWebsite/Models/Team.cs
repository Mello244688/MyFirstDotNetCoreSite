using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFirstWebsite.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string LeagueName { get; set; }
        public string TeamName { get; set; }
        public int DraftPosition { get; set; }

        public ICollection<TeamPlayer> LineUp { get; set; }

        public ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public Draft Draft { get; set; }
        public int DraftId { get; set; }
    }
}