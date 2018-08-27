using MyFirstWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.ViewModels
{
    public class NewViewModel
    {
        public ScoringType ScoringType { get; set; }
        public string TeamName { get; set; }
        public string LeagueName { get; set; }
        public int DraftPosition { get; set; }
        public int NumberOfTeams { get; set; }
    }
}
