using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstWebsite.Models;
using MyFirstWebsite.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFirstWebsite.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class FantasyApi : Controller
    {
        private readonly AppDbContext appDbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public FantasyApi(AppDbContext appDbContext, UserManager<ApplicationUser> userManager)
        {
            this.appDbContext = appDbContext;
            this.userManager = userManager;
        }

        [HttpGet]
        [Route("api/[controller]/GetAvailablePlayers/{draftId}")]
        public IActionResult GetAvailablePlayers(int draftId)
        {
            var draft = appDbContext.Drafts.Where(d => d.Id == draftId)
                .Include(d => d.AvailablePlayers)
                .ThenInclude(dp => dp.Player)
                .Include(d => d.TeamsInDraft)
                .ThenInclude(ts => ts.LineUp)
                .FirstOrDefault();

            HashSet<Player> players = draft.AvailablePlayers
                .Select(p => new Player { Id = p.Player.Id, Name = p.Player.Name, Rank = p.Player.Rank, Position = p.Player.Position })
                .OrderBy(p => p.Rank).ToHashSet();

            return Json(players);
        }

        [Route("api/[controller]/RemovePlayer/{id}")]
        [HttpDelete]
        public void Delete([FromBody]Player player, int id)
        {
            var dPCollection = appDbContext.Players.Where(p => p.Id == player.Id)
                .Include(p => p.DraftPlayer);

            var draftPlayerToRemove = dPCollection.FirstOrDefault().DraftPlayer
                .Where(dp => dp.DraftId == id && dp.PlayerId == player.Id).FirstOrDefault();

            appDbContext.Drafts.Where(d => d.Id == id).FirstOrDefault().AvailablePlayers.Remove(draftPlayerToRemove);
            appDbContext.SaveChanges();
        }

        [Route("api/[controller]/AddToDrafted/{draftId}")]
        [HttpPut]
        public void AddPlayerToDrafted([FromBody]DraftedPlayer player, int draftId)
        {
            var draft = appDbContext.Drafts.Where(d => d.Id == draftId)
                .Include(d => d.PlayersDrafted)
                .ThenInclude(pd => pd.Player)
                .FirstOrDefault();

            var dDPlayer = new DraftDraftedPlayer();
            var dPlayer = new DraftedPlayer();

            dPlayer.Name = player.Name;
            dPlayer.Position = player.Position;
            dPlayer.Rank = player.Rank;
            dPlayer.PositionDrafted = player.PositionDrafted;

            dDPlayer.Draft = draft;
            dDPlayer.Player = dPlayer;

            draft.PlayersDrafted.Add(dDPlayer);

            appDbContext.SaveChanges();
        }

        [Route("api/[controller]/AddToTeam/{draftId}")]
        [HttpPut]
        public void AddPlayerToUserTeam([FromBody]Player player, int draftId)
        {
            TeamPlayer teamPlayer = new TeamPlayer();
            var team = GetTeam(draftId);

            player.Id = 0;
            teamPlayer.Player = player;
            teamPlayer.PlayerId = player.Id;
            teamPlayer.Team = team;
            teamPlayer.TeamId = team.Id;

            var lineUp = appDbContext.Teams.Where(t => t == GetTeam(draftId)).FirstOrDefault().LineUp;

            if (lineUp == null)
            {
                lineUp = new HashSet<TeamPlayer>();
            }
            lineUp.Add(teamPlayer);

            appDbContext.SaveChanges();
        }

        [Route("api/[controller]/GetUserTeam/{draftId}")]
        [HttpGet]
        public IActionResult GetUserTeam(int draftId)
        {
            var team = GetTeam(draftId);

            //var roster = team.LineUp.Select(p => new Player { Id = p.PlayerId, Name = p.Player.Name, Position = p.Player.Position, Rank = p.Player.Rank}).ToList();
            var draft = appDbContext.Drafts.Where(d => d.Id == draftId).Include(d => d.PlayersDrafted).ThenInclude(pd => pd.Player).FirstOrDefault();

            return Json(GetUserPlayers(draftId, team.Draft.NumberOfTeams, draft.PlayersDrafted.Select(p => p.Player).OrderBy(p => p.PositionDrafted).ToList()));
        }

        [Route("api/[controller]/GetNumberOfTeams/{draftId}")]
        [HttpGet]
        public int GetNumberOfTeams(int draftId)
        {
            return appDbContext.Drafts.Where(d => d.Id == draftId).Select(d => d.NumberOfTeams).FirstOrDefault();
        }

        [Route("api/[controller]/GetDraftPosition/{draftId}")]
        [HttpGet]
        public int GetDraftPosition(int draftId)
        {
            return appDbContext.Teams.Where(t => t.UserId == userManager.GetUserId(User) && t.DraftId == draftId).Select(t => t.DraftPosition).FirstOrDefault();
        }

        [Route("api/[controller]/GetDraftedPlayers/{draftId}")]
        [HttpGet]
        public IActionResult GetDraftedPlayers(int draftId)
        {
            HashSet<DraftedPlayer> players = GetPlayersDrafted(draftId);

            return Json(players);
        }

        [Route("api/[controller]/GetDraftBoard/{draftId}")]
        [HttpGet]
        public IActionResult GetDraftBoard(int draftId)
        {
            DraftboardViewModel draftboardViewModel = new DraftboardViewModel();
            var draft = appDbContext.Drafts.Where(d => d.Id == draftId).FirstOrDefault();

            draftboardViewModel.Players = GetPlayersDrafted(draftId);
            draftboardViewModel.NumberOfTeams = draft.NumberOfTeams;

            return PartialView("_Card", draftboardViewModel);
        }

        [Route("api/[controller]/GetAddPlayerForm")]
        [HttpGet]
        public IActionResult GetAddPlayerForm()
        {
            return PartialView("_AddPlayerForm");
        }

        [Route("api/[controller]/AddPlayerToDraft/{draftId}")]
        [HttpPut]
        public IActionResult AddPlayerToDraft([FromBody] Player player, int draftId)
        {
            appDbContext.Players.Add(player);
            var draft = appDbContext.Drafts.Where(d => d.Id == draftId).Include(d => d.AvailablePlayers).FirstOrDefault();
            DraftPlayer draftPlayer = new DraftPlayer();
            draftPlayer.Draft = draft;
            draftPlayer.DraftId = draft.Id;
            draftPlayer.Player = player;

            draft.AvailablePlayers.Add(draftPlayer);
            appDbContext.SaveChanges();

            return Json(new Player { Id = player.Id, Name = player.Name, Position = player.Position, Rank = player.Rank});
        }

        [Route("api/[controller]/DeleteTeam/")]
        [HttpDelete]
        public void DeleteTeam([FromBody] Team team)
        {
            appDbContext.Teams.Remove(team);

            appDbContext.SaveChanges();
        }

        [Route("api/[controller]/GetAllUserTeams/")]
        [HttpGet]
        public IActionResult GetAllUserTeams()
        {
            var teams = appDbContext.ApplicationUsers.Where(u => u.Id == userManager.GetUserId(User)).Select(u => u.UserTeams).FirstOrDefault().ToList();

            return Json(teams);
        }

        [Route("api/[controller]/SwapDraftedPlayers/{draftId}")]
        [HttpPut]
        public void SwapDraftedPlayers([FromBody] List<DraftedPlayer> draftedPlayers, int draftId)
        {
            var draft = appDbContext.Drafts.Where(d => d.Id == draftId).Include(d => d.PlayersDrafted).ThenInclude(pd => pd.Player).FirstOrDefault();

            foreach (var player in draftedPlayers)
            {
                draft.PlayersDrafted.Where(p => p.PlayerId == player.Id).FirstOrDefault().Player.PositionDrafted = player.PositionDrafted;
            }

            //UpdateUserTeam(draftId, draft.NumberOfTeams, draft.PlayersDrafted.Select(p => p.Player).OrderBy(p => p.PositionDrafted).ToList());

            appDbContext.SaveChanges();
                
        }

        private void UpdateUserTeam(int draftId, int numTeams, List<DraftedPlayer> playersDrafted)
        {
            var team = appDbContext.Teams.Where(t => t == GetTeam(draftId)).FirstOrDefault();
            var players = new List<Player>();
            List<TeamPlayer> lineup = new List<TeamPlayer>();

            for (int i = 0; i < playersDrafted.Count; i++)
            {
                decimal r = (i + numTeams) / numTeams;
                var round = Math.Floor(r);
                if (round % 2 == 0)
                {
                    if (r * numTeams - team.DraftPosition + 1 == i + 1)
                    {
                        //AddTeamPlayerToLineup(playersDrafted, team, lineup, i);
                    }
                }
                else
                {
                    if ((r - 1) * numTeams + team.DraftPosition == i + 1)
                    {
                        //AddTeamPlayerToLineup(playersDrafted, team, lineup, i);
                    }
                }
            }

            team.LineUp = lineup;
            appDbContext.SaveChanges();
        }

        private void AddTeamPlayerToLineup(List<DraftedPlayer> playersDrafted, Team team, List<TeamPlayer> lineup, int i)
        {
            lineup.Add(new TeamPlayer
            {
                Player = /*appDbContext.Players.Where(p => p.Id == playersDrafted.ElementAt(i).Id).Include(p => p.DraftPlayer).Include(p => p.TeamPlayer).FirstOrDefault()*/new Player
                {
                    //Id = playersDrafted.ElementAt(i).Id,
                    Name = playersDrafted.ElementAt(i).Name,
                    Position = playersDrafted.ElementAt(i).Position,
                    Rank = playersDrafted.ElementAt(i).Rank 
                },
                PlayerId = playersDrafted.ElementAt(i).Id,
                Team = new Team
                {
                    Draft = team.Draft,
                    DraftId = team.DraftId,
                    DraftPosition = team.DraftPosition,
                    Id = team.Id,
                    LeagueName = team.LeagueName,
                    TeamName = team.TeamName,
                    User = team.User,
                    UserId = team.UserId,
                    LineUp = lineup
                },
                TeamId = team.Id
            });
        }

        private Player GetNewPlayer(List<DraftedPlayer> playersDrafted, int i)
        {
            return new Player
            {
                Id = playersDrafted.ElementAt(i).Id,
                Name = playersDrafted.ElementAt(i).Name,
                Position = playersDrafted.ElementAt(i).Position,
                Rank = playersDrafted.ElementAt(i).Rank
            };
        }

        private List<Player> GetUserPlayers(int draftId, int numTeams, List<DraftedPlayer> playersDrafted)
        {
            var team = appDbContext.Teams.Where(t => t == GetTeam(draftId)).FirstOrDefault();
            List<Player> players = new List<Player>();

            for (int i = 0; i < playersDrafted.Count; i++)
            {
                decimal r = (i + numTeams) / numTeams;
                var round = Math.Floor(r);
                if (round % 2 == 0)
                {
                    if (r * numTeams - team.DraftPosition + 1 == i + 1)
                    {
                        players.Add(GetNewPlayer(playersDrafted, i));
                    }
                }
                else
                {
                    if ((r - 1) * numTeams + team.DraftPosition == i + 1)
                    {
                        players.Add(GetNewPlayer(playersDrafted, i));
                    }
                }
            }
            return players;
        }

        private HashSet<DraftedPlayer> GetPlayersDrafted(int draftId)
        {
            var draft = appDbContext.Drafts.Where(d => d.Id == draftId)
                            .Include(d => d.PlayersDrafted)
                            .ThenInclude(dp => dp.Player)
                            .FirstOrDefault();

            HashSet<DraftedPlayer> players = draft.PlayersDrafted
                .Select(p => new DraftedPlayer { Id = p.Player.Id, Name = p.Player.Name, Rank = p.Player.Rank, Position = p.Player.Position, PositionDrafted = p.Player.PositionDrafted })
                .OrderBy(p => p.PositionDrafted).ToHashSet();

            return players;
        }

        private Team GetTeam(int draftId)
        {
            return appDbContext.Teams
                .Where(t => t.UserId == userManager.GetUserId(User) && t.DraftId == draftId)
                .Include(t => t.LineUp)
                .ThenInclude(tp => tp.Player)
                .FirstOrDefault();
        }
    }
}
