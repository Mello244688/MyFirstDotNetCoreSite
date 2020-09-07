using MyFirstWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Repositories.Fantasy
{
    public class PlayerRepository : IPlayerRepository
    {
        public Player GetPlayer(int id)
        {
            throw new NotImplementedException();
        }

        public List<Team> GetPlayers(int teamId, int draftId)
        {
            throw new NotImplementedException();
        }
    }
}
