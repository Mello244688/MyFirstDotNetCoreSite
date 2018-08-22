using MyFirstWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.ViewModels
{
    public class FantasyViewModel
    {
        public ICollection<Player> Players { get; set; }
        public ScoringType ScoringType { get; set; }
        public string PageHeading { get; set; }
    }
}
