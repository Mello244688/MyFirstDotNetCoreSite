using MyFirstWebsite.Models;
using MyFirstWebsite.Repositories;
using MyFirstWebsite.Repositories.Fantasy;
using System.Linq;

namespace MyFirstWebsite.Services.Fantasy
{
    class TeamService : TeamRepository, ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IDraftRepository _draftRepository;

        public TeamService(AppDbContext appDbContext, ITeamRepository teamRepository, IDraftRepository draftRepository) : base(appDbContext)
        {
            _teamRepository = teamRepository;
            _draftRepository = draftRepository;
        }

        public Team GetAvailablePlayers(Draft draft)
        {
            return draft.Teams
                .Where(t => t.DraftPosition == 0)
                .FirstOrDefault();
        }

        public Team GetAvailablePlayersById(int draftId)
        {
            Draft draft = _draftRepository.GetDraft(draftId);
            
            return draft.Teams.Where(ts => ts.DraftPosition == 0).FirstOrDefault();
        }

        public Team GetUserTeam(Draft draft)
        {
            int userTeamId = draft.Teams
                .Where(t => t.DraftPosition == draft.UserDraftPosition)
                .Select(t => t.Id).FirstOrDefault();

            return _teamRepository.GetTeam(userTeamId);
        }

        public Team GetUserTeamById(int draftId)
        {
            Draft draft = _draftRepository.GetDraft(draftId);

            return draft.Teams.Where(ts => ts.DraftPosition == draft.UserDraftPosition).FirstOrDefault();
        }
    }
}
