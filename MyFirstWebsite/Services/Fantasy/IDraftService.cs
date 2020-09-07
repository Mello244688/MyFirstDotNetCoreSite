using MyFirstWebsite.Models;
using MyFirstWebsite.Repositories;
using MyFirstWebsite.ViewModels;

namespace MyFirstWebsite.Services.Fantasy
{
    public interface IDraftService : IDraftRepository
    {
        string GetPageHeading(ScoringType type);
        int GetPick(Draft draft);
        int GetRound(int pick, int numberOfTeams);
        Draft SetupDraft(NewViewModel newViewModel, string userId);
    }
}
