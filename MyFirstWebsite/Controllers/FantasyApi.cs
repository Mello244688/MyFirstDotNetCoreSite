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
            HashSet<Player> availablePlayers = appDbContext.Teams
                .Where(t => t.DraftId == draftId && t.DraftPosition == 0)
                .Select(t => t.Players).FirstOrDefault()
                .OrderBy(p => p.Rank).ToHashSet();

            return Json(availablePlayers);
        }

        [Route("api/[controller]/AddToTeam/{draftId}/{teamNum}")]
        [HttpPut]
        public void AddPlayerToTeam([FromBody]Player player, int draftId, int teamNum)
        {
            Team team = GetTeam(draftId, teamNum);

            team.Players.Add(player);

            appDbContext.SaveChanges();
        }

        [Route("api/[controller]/GetUserTeam/{draftId}")]
        [HttpGet]
        public IActionResult GetUserTeam(int draftId)
        {
            int userDraftNum = appDbContext.Drafts
                .Where(d => d.Id == draftId)
                .Select(d => d.UserDraftPosition).FirstOrDefault();

            HashSet<Player> players = GetTeam(draftId, userDraftNum).Players
                .Select(p => new Player { Name = p.Name, Position = p.Position, Rank = p.Rank, PositionDrafted = p.PositionDrafted, PlayerUrl = p.PlayerUrl})
                .OrderBy(p => p.PositionDrafted).ToHashSet();

            return Json(players);
        }

        [Route("api/[controller]/GetNumberOfTeams/{draftId}")]
        [HttpGet]
        public int GetNumberOfTeams(int draftId)
        {
            return appDbContext.Drafts
                .Where(d => d.Id == draftId)
                .Select(d => d.NumberOfTeams).FirstOrDefault();
        }

        [Route("api/[controller]/GetDraftPosition/{draftId}")]
        [HttpGet]
        public int GetDraftPosition(int draftId)
        {
            return appDbContext.Drafts
                .Where(d => d.Id == draftId)
                .Select(d => d.UserDraftPosition).FirstOrDefault();
        }

        [Route("api/[controller]/GetDraftPickNumber/{draftId}")]
        [HttpGet]
        public int GetDraftPickNumber(int draftId)
        {
            int pickNumber = GetPlayersDrafted(draftId).Count() + 1;

            return pickNumber;
        }

        [Route("api/[controller]/GetDraftBoard/{draftId}")]
        [HttpGet]
        public IActionResult GetDraftBoard(int draftId)
        {
            DraftboardViewModel draftboardViewModel = new DraftboardViewModel();
            var draft = appDbContext.Drafts.Where(d => d.Id == draftId).FirstOrDefault();

            draftboardViewModel.Players = GetPlayersDrafted(draftId).ToHashSet();
            draftboardViewModel.NumberOfTeams = draft.NumberOfTeams;

            return PartialView("_Card", draftboardViewModel);
        }

        [Route("api/[controller]/GetAddPlayerForm")]
        [HttpGet]
        public IActionResult GetAddPlayerForm()
        {
            return PartialView("_AddPlayerForm");
        }

        [Route("api/[controller]/DeleteDraft/")]
        [HttpDelete]
        public void DeleteDraft([FromBody] Draft draft)
        {
            appDbContext.Drafts.Remove(draft);

            appDbContext.SaveChanges();
        }

        [Route("api/[controller]/GetAllUserDrafts/")]
        [HttpGet]
        public IActionResult GetAllUserDrafts()
        {
            var drafts = appDbContext.ApplicationUsers
                .Where(u => u.Id == userManager.GetUserId(User))
                .Select(u => u.Drafts).FirstOrDefault().ToList();

            return Json(drafts);
        }

        /*
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
        */
        //private Player GetNewPlayer(List<DraftedPlayer> playersDrafted, int i)
        //{
        //    return new Player
        //    {
        //        Id = playersDrafted.ElementAt(i).Id,
        //        Name = playersDrafted.ElementAt(i).Name,
        //        Position = playersDrafted.ElementAt(i).Position,
        //        Rank = playersDrafted.ElementAt(i).Rank
        //    };
        //}

        private Team GetTeam(int draftId, int teamNum)
        {
            var test = appDbContext.Teams
                .Where(t => t.DraftId == draftId && t.DraftPosition == teamNum)
                .Include(t => t.Players).FirstOrDefault();

            return test;
        }

        private ICollection<Player> GetPlayersDrafted(int draftId)
        {
            List<Team> teams = appDbContext.Drafts
                .Where(d => d.Id == draftId)
                .Include(d => d.Teams)
                    .ThenInclude(ts => ts.Players)
                .Select(d => d.Teams).FirstOrDefault().ToList();

            List<Player> draftedPlayers = new List<Player>();

            foreach (var team in teams.Skip(1)) //first is available players
            {
                draftedPlayers.AddRange(team.Players);
            }

            return draftedPlayers.OrderBy(p => p.PositionDrafted).ToHashSet();
        }
    }
}
