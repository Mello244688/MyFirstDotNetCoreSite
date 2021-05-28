using MyFirstWebsite.Models;
using MyFirstWebsite.Repositories.Fantasy;
using MyFirstWebsite.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyFirstWebsite.Services.Fantasy
{
    public class DraftService : DraftRepository, IDraftService
    {
        private readonly IFantasyProsDataGrabber _fantasyProsDataGrabber;

        public DraftService(AppDbContext appDbContext
                            , IFantasyProsDataGrabber fantasyProsDataGrabber
                            , ITeamService teamService) : base(appDbContext)
        {
            _fantasyProsDataGrabber = fantasyProsDataGrabber;
        }

        public int getDraftPosition(int draftId)
        {
            return GetDraft(draftId).UserDraftPosition;
        }

        public int GetNumberOfTeams(int draftId)
        {
            return GetDraft(draftId).NumberOfTeams;
        }

        public string GetPageHeading(ScoringType type)
        {
            var heading = "";

            if (type.Equals(ScoringType.HalfPPR))
            {
                heading = "Half Point PPR";
            }
            else if (type.Equals(ScoringType.PPR))
            {
                heading = "PPR";
            }
            else
            {
                heading = "Standard";
            }

            return heading;
        }

        public int GetPick(Draft draft)
        {
            List<Player> draftedPlayers = new List<Player>();

            foreach (var team in draft.Teams.Skip(1)) //first is available players
            {
                draftedPlayers.AddRange(team.Players);
            }

            return draftedPlayers.Count + 1;
        }

        public int GetPickById(int draftId)
        {
            return GetPick(GetDraft(draftId));
        }

        public int GetRound(int pick, int numberOfTeams)
        {
            return (int)Math.Ceiling((float)pick / numberOfTeams);
        }

        public Draft SetupDraft(NewViewModel newDraft, string userId)
        {
            ICollection<Team> teams = new List<Team>();
            Draft draft = new Draft();

            //Setup all the teams. The first team is the available player pool.
            for (int i = 0; i <= newDraft.NumberOfTeams; i++)
            {
                Team team;

                //Available players
                if (i == 0)
                {
                    team = new Team()
                    {
                        TeamName = "AvailablePlayers",
                        Players = new HashSet<Player>(),
                        DraftPosition = i,
                        Draft = draft
                    };


                    // Add all players to first team (draft's available players)
                    team.Players = _fantasyProsDataGrabber.GetPlayers(newDraft.ScoringType);
                }
                // Actual User
                else if (i == newDraft.DraftPosition)
                {
                    team = new Team()
                    {
                        TeamName = newDraft.TeamName,
                        Players = new HashSet<Player>(),
                        DraftPosition = i,
                        Draft = draft
                    };
                }
                else
                {
                    team = new Team()
                    {
                        TeamName = $"Team {i}",
                        Players = new HashSet<Player>(),
                        DraftPosition = i,
                        Draft = draft
                    };
                }

                teams.Add(team);
            }

            draft.ScoringType = newDraft.ScoringType;
            draft.NumberOfTeams = newDraft.NumberOfTeams;
            draft.Teams = teams;
            draft.UserDraftPosition = newDraft.DraftPosition;
            draft.LeagueName = newDraft.LeagueName;
            draft.UserId = userId;
            draft.DateCreated = DateTime.Now;

            return draft;
        }
    }
}
