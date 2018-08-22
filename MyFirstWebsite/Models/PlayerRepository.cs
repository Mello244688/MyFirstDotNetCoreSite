using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Models
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly AppDbContext appDbContext;

        public PlayerRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public Player GetPlayer(int id)
        {
            return appDbContext.Players.FirstOrDefault(r => r.Id == id);
        }

        public IEnumerable<Player> GetPlayers()
        {
            return appDbContext.Players;
        }
    }
}
