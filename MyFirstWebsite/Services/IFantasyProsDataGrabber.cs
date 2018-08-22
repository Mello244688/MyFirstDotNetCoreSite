using MyFirstWebsite.Models;
using MyFirstWebsite.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Services
{
    public interface IFantasyProsDataGrabber
    {
        ICollection<Player> GetPlayers(ScoringType scoringType);
        string GetUrl(ScoringType scoringType);
    }
}
