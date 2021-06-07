using MyFirstWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Repositories.Fantasy
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly AppDbContext _appDbContext;
        public PlayerRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public Player GetPlayer(int playerId, int draftId)
        {
            throw new NotImplementedException();
        }

        public Player GetPlayer(Player player, Team team)
        {
            return _appDbContext.Players
                .Where(p => p.Name == player.Name && p.Position == player.Position && p.TeamId == team.Id)
                .FirstOrDefault();
        }

        public Player GetPlayer(Player player, int draftId)
        {
            return _appDbContext.Players
                .Where(p => p.Name == player.Name && p.Position == player.Position && p.Team.DraftId == draftId)
                .FirstOrDefault();
        }

        public List<Team> GetPlayers(int teamId, int draftId)
        {
            throw new NotImplementedException();
        }
    }
}
