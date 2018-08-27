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

        public ICollection<Team> TeamsInDraft { get; set; }

        public ICollection<DraftPlayer> AvailablePlayers { get; set; }
        public ICollection<DraftDraftedPlayer> PlayersDrafted { get; set; }
    }
}
