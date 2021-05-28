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

        public Team GetTeam(int teamId)
        {
            return _appDbContext.Teams
                .Where(ts => ts.Id == teamId)
                .Include(t => t.Players)
                .FirstOrDefault();
        }

        public Team GetTeam(int draftId, int draftPosition)
        {
            return _appDbContext.Teams
                .Where(ts => ts.DraftId == draftId && ts.DraftPosition == draftPosition)
                .Include(t => t.Players)
                .FirstOrDefault();
        }

        public void Save()
        {
            _appDbContext.SaveChanges();
        }

        public void UpdateTeam(Team team)
        {
            _appDbContext.Attach(team);
        }
    }
}
