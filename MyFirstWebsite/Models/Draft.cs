using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Models
{
    public class Draft
    {
        public int Id { get; set; }
        public ScoringType ScoringType { get; set; }

        public ICollection<DraftPlayer> AvailablePlayers { get; set; }
        public ICollection<Team> TeamsInDraft { get; set; }
    }
}
