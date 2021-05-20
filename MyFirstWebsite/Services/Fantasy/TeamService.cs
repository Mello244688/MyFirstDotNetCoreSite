using MyFirstWebsite.Models;
using MyFirstWebsite.Repositories;
using MyFirstWebsite.Repositories.Fantasy;
using System.Linq;

namespace MyFirstWebsite.Services.Fantasy
{
    class TeamService : TeamRepository, ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(AppDbContext appDbContext, ITeamRepository teamRepository) : base(appDbContext)
        {
            _teamRepository = teamRepository;
        }

        public Team GetAvailablePlayers(Draft draft)
        {
            return draft.Teams
                .Where(t => t.DraftPosition == 0).FirstOrDefault();
        }

        public Team GetUserTeam(Draft draft)
        {
            int userTeamId = draft.Teams
                .Where(t => t.DraftPosition == draft.UserDraftPosition)
                .Select(t => t.Id).FirstOrDefault();

            return _teamRepository.GetTeam(userTeamId, draft.Id);
        }
    }
}
