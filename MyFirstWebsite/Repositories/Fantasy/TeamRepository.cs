using Microsoft.EntityFrameworkCore;
using MyFirstWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Repositories.Fantasy
{
    public class TeamRepository : ITeamRepository
    {
        private readonly AppDbContext _appDbContext;

        public TeamRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public List<Team> GetAllTeams(int draftId)
        {
            return (List<Team>)_appDbContext.Teams
                .Where(ts => ts.DraftId == draftId)
                .Include(t => t.Players);
        }

        public Team GetTeam(int teamId, int draftId)
        {
            return _appDbContext.Teams
                .Where(ts => ts.Id == teamId && ts.DraftId == draftId)
                .Include(t => t.Players)
                .FirstOrDefault();
        }
    }
}
