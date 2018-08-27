using MyFirstWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.ViewModels
{
    public class DraftViewModel
    {
        public HashSet<Player> MyPlayers { get; set; }
        public HashSet<Player> Players { get; set; }
        public string LeagueName { get; set; }
        public string TeamName { get; set; }
        public int NumberOfTeams { get; set; }
        public int DraftPosition { get; set; }

    }
}
