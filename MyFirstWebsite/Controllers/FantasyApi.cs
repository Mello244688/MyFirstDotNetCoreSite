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
        public void AddPlayerToDrafted([FromBody]Player player, int draftId)
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

            dDPlayer.Draft = draft;
            dDPlayer.Player = dPlayer;

            //dPlayer.DraftDraftedPlayer = draft.PlayersDrafted;
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

            var roster = team.LineUp.Select(p => new Player { Id = p.PlayerId, Name = p.Player.Name, Position = p.Player.Position, Rank = p.Player.Rank}).ToList();

            return Json(roster);
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
            HashSet<DraftedPlayer> players = GetPlayersDrafted(draftId);

            return PartialView("_Card", players);
        }

        private HashSet<DraftedPlayer> GetPlayersDrafted(int draftId)
        {
            var draft = appDbContext.Drafts.Where(d => d.Id == draftId)
                            .Include(d => d.PlayersDrafted)
                            .ThenInclude(dp => dp.Player)
                            .FirstOrDefault();

            HashSet<DraftedPlayer> players = draft.PlayersDrafted
                .Select(p => new DraftedPlayer { Id = p.Player.Id, Name = p.Player.Name, Rank = p.Player.Rank, Position = p.Player.Position })
                .OrderBy(p => p.Rank).ToHashSet();

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
