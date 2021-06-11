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
        private readonly ITeamService _teamService;

        public DraftService(AppDbContext appDbContext
                            , IFantasyProsDataGrabber fantasyProsDataGrabber
                            , ITeamService teamService) : base(appDbContext)
        {
            _fantasyProsDataGrabber = fantasyProsDataGrabber;
            _teamService = teamService;
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

        public void UpdateDraftTeams(List<Player> playersDrafted, int draftId)
        {
            Draft draft = GetDraft(draftId);
            List<Team> teams = draft.Teams.OrderBy(t => t.DraftPosition).ToList();
            List<Player> players = _teamService.GetDraftedPlayers(draftId).OrderBy(p => p.Rank).ToList();
            playersDrafted = playersDrafted.OrderBy(p => p.Rank).ToList();

            if (playersDrafted.Count != players.Count)
            {
                // return error
            }

            foreach (var team in teams.Skip(1)) //skip first
            {
                team.Players.Clear();
            }

            for (int i = 0; i < playersDrafted.Count; i++)
            {
                players[i].PositionDrafted = playersDrafted[i].PositionDrafted;
                players[i].Team = null;
            }

            players = players.OrderBy(p => p.PositionDrafted).ToList();
 
            for (int i = 0; i < players.Count; i++)
            {
                int round = (int)Math.Ceiling((double)players[i].PositionDrafted / draft.NumberOfTeams);
                int teamNum;

                if (round % 2 == 1)
                {
                    teamNum = players[i].PositionDrafted - (round - 1) * draft.NumberOfTeams;
                }
                else
                {
                    teamNum = round * draft.NumberOfTeams + 1 - players[i].PositionDrafted;
                }

                Team team = teams.Where(t => t.DraftPosition == teamNum).FirstOrDefault();
                players[i].Team = team;
                players[i].TeamId = team.Id;
                team.Players.Add(players[i]);
            }

            UpdateDraft(draft);
        }
    }
}
