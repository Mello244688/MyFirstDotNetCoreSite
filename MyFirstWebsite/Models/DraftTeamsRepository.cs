using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Models
{
    public class DraftTeamsRepository : IDraftTeamsRepository
    {
        private readonly AppDbContext appDbContext;

        public DraftTeamsRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public IEnumerable<Team> GetAllDraftTeams()
        {
            return appDbContext.Teams;
        }
    }
}
