using MyFirstWebsite.Models;
using System.Collections.Generic;

namespace MyFirstWebsite.Services
{
    public interface IFantasyProsDataGrabber
    {
        ICollection<Player> GetPlayers(ScoringType scoringType);
        string GetUrl(ScoringType scoringType);
    }
}
