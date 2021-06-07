using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstWebsite.Models;

namespace MyFirstWebsite.Repositories
{
    public interface IPlayerRepository
    {
        Player GetPlayer(int playerId, int draftId);
        Player GetPlayer(Player player, Team team);
        Player GetPlayer(Player player, int draftId);
        List<Team> GetPlayers(int teamId, int draftId);
    }
}
