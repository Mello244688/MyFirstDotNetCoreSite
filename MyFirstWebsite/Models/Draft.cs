using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Models
{
    public class Draft
    {
        public int Id { get; set; }
        public ScoringType ScoringType { get; set; }
        public int NumberOfTeams { get; set; }
        public int UserDraftPosition { get; set; }
        public string LeagueName { get; set; }
        public DateTime DateCreated { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public ICollection<Team> Teams { get; set; }
    }
}
