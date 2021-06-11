using MyFirstWebsite.Models;
using MyFirstWebsite.Repositories;
using MyFirstWebsite.ViewModels;
using System.Collections.Generic;

namespace MyFirstWebsite.Services.Fantasy
{
    public interface IDraftService : IDraftRepository
    {
        int getDraftPosition(int draftId);
        string GetPageHeading(ScoringType type);
        int GetPick(Draft draft);
        int GetPickById(int draftId);
        int GetRound(int pick, int numberOfTeams);
        int GetNumberOfTeams(int draftId);
        Draft SetupDraft(NewViewModel newViewModel, string userId);
        void UpdateDraftTeams(List<Player> playersDrafted, int draftId);
    }
}
