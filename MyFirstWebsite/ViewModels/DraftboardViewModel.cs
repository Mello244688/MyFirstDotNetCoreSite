using MyFirstWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.ViewModels
{
    public class DraftboardViewModel
    {
        public HashSet<DraftedPlayer> Players { get; set; }
        public int NumberOfTeams { get; set; }
    }
}
