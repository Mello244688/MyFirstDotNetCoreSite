using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Models
{
    public class DraftedPlayer
    {
        public int Id { get; set; }
        public int Rank { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public int PositionDrafted { get; set; }

        public ICollection<DraftDraftedPlayer> DraftDraftedPlayer { get; set; }
        //public ICollection<TeamPlayer> TeamPlayer { get; set; }
    }
}
