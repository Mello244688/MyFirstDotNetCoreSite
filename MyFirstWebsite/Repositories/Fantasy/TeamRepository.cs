using MyFirstWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Repositories.Fantasy
{
    public class TeamRepository : ITeamRepository
    {
        public List<Team> GetAllTeams(int draftId)
        {
            throw new NotImplementedException();
        }

        public Team GetTeam(int teamId, int draftId)
        {
            throw new NotImplementedException();
        }
    }
}
