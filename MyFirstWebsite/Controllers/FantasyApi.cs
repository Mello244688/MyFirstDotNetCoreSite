using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyFirstWebsite.Models;
using MyFirstWebsite.Services.Fantasy;
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
        private readonly IDraftService _draftService;
        private readonly ITeamService _teamService;
        private readonly IPlayerService _playerService;

        public FantasyApi(AppDbContext appDbContext
                        , UserManager<ApplicationUser> userManager
                        , IDraftService draftService
                        , ITeamService teamService
                        , IPlayerService playerService)
        {
            this.appDbContext = appDbContext;
            this.userManager = userManager;
            _draftService = draftService;
            _teamService = teamService;
            _playerService = playerService;
        }

        [HttpGet]
        [Route("api/[controller]/GetAvailablePlayers/{draftId}")]
        public IActionResult GetAvailablePlayers(int draftId)
        {
            List<Player> availablePlayers = 
                _teamService.GetAvailablePlayersById(draftId).Players
                .Select(p => new Player { Name = p.Name, Position = p.Position, Rank = p.Rank, PositionDrafted = p.PositionDrafted, PlayerUrl = p.PlayerUrl })
                .OrderBy(p => p.Rank).ToList();

            return Json(availablePlayers);
        }

        [Route("api/[controller]/AddToTeam/{draftId}/{teamNum}")]
        [HttpPut]
        public IActionResult AddPlayerToTeam([FromBody]Player player, int draftId, int teamNum)
        {
            List<Team> teams = _teamService.GetAllTeams(draftId);
            Team teamAvailablePlayers = teams.Where(t => t.DraftPosition == 0).FirstOrDefault();
            Team userTeam = teams.Where(t => t.DraftPosition == teamNum).FirstOrDefault();

            Player playerToAdd = _playerService.GetPlayer(player, teamAvailablePlayers);
            playerToAdd.PositionDrafted = player.PositionDrafted;
            userTeam.Players.Add(playerToAdd);

            _teamService.UpdateTeam(userTeam);
            _teamService.Save();

            return Ok();
        }

        [Route("api/[controller]/GetUserTeam/{draftId}")]
        [HttpGet]
        public IActionResult GetUserTeam(int draftId)
        {
            List<Player> players = _teamService.GetUserTeamById(draftId).Players
                .Select(p => new Player 
                { 
                    Name = p.Name, 
                    Position = p.Position, 
                    Rank = p.Rank, 
                    PositionDrafted = p.PositionDrafted, 
                    PlayerUrl = p.PlayerUrl 
                })
                .OrderBy(p => p.PositionDrafted)
                .ToList();

            return Json(players);
        }

        [Route("api/[controller]/GetNumberOfTeams/{draftId}")]
        [HttpGet]
        public int GetNumberOfTeams(int draftId)
        {
            return _draftService.GetNumberOfTeams(draftId);
        }

        [Route("api/[controller]/GetDraftPosition/{draftId}")]
        [HttpGet]
        public int GetDraftPosition(int draftId)
        {
            return _draftService.getDraftPosition(draftId);
        }

        [Route("api/[controller]/GetDraftPickNumber/{draftId}")]
        [HttpGet]
        public int GetDraftPickNumber(int draftId)
        {
            return _draftService.GetPickById(draftId);
        }

        [Route("api/[controller]/GetDraftBoard/{draftId}")]
        [HttpGet]
        public IActionResult GetDraftBoard(int draftId)
        {
            DraftboardViewModel draftboardViewModel = new DraftboardViewModel();
            var draft = _draftService.GetDraft(draftId);

            draftboardViewModel.Players =
                _teamService.GetDraftedPlayers(draftId)
                .OrderBy(p => p.PositionDrafted)
                .ToHashSet();
            draftboardViewModel.NumberOfTeams = draft.NumberOfTeams;

            return PartialView("_FlexCard", draftboardViewModel);
        }

        [Route("api/[controller]/GetAddPlayerForm")]
        [HttpGet]
        public IActionResult GetAddPlayerForm()
        {
            return PartialView("_AddPlayerForm");
        }

        [Route("api/[controller]/DeleteDraft/{draftId}")]
        [ValidateAntiForgeryToken]
        [HttpDelete]
        public IActionResult DeleteDraft(int draftId)
        {
            _draftService.DeleteDraftById(draftId);
            _draftService.Save();

            return Ok();
        }

        [Route("api/[controller]/GetAllUserDrafts/")]
        [HttpGet]
        public IActionResult GetAllUserDrafts()
        {
            List<Draft> drafts = _draftService.GetAllDrafts(userManager.GetUserId(User))
                .Select(d => new Draft 
                { 
                    Id = d.Id, 
                    LeagueName = d.LeagueName,
                    UserDraftPosition = d.UserDraftPosition, 
                    DateCreated = d.DateCreated 
                }).ToList();

            return Json(drafts);
        }

        [Route("api/[controller]/GetDraftedPlayers/{draftId}")]
        [HttpGet]
        public IActionResult GetAllDraftedPlayers(int draftId)
        {
            List<Player> players = _teamService.GetDraftedPlayers(draftId)
                .Select(p => new Player
                {
                    Name = p.Name,
                    Position = p.Position,
                    Rank = p.Rank,
                    PositionDrafted = p.PositionDrafted
                })
                .OrderBy(p => p.PositionDrafted)
                .ToList();
            
            return Json(players);
        }

        [Route("api/[controller]/GetDraftBoardUi/{draftId}")]
        [HttpPut]
        public IActionResult GetDraftBoardUi([FromBody] List<Player> draftedPlayers, int draftId)
        {
            DraftboardViewModel draftboardViewModel = new DraftboardViewModel();

            draftboardViewModel.Players = draftedPlayers
                .OrderBy(p => p.PositionDrafted)
                .ToHashSet();
            draftboardViewModel.NumberOfTeams = _draftService.GetNumberOfTeams(draftId);

            return PartialView("_Card", draftboardViewModel);
        }

        [Route("api/[controller]/SwapDraftedPlayers/{draftId}")]
        [HttpPut]
        public void SwapDraftedPlayers([FromBody] List<Player> draftedPlayers, int draftId)
        {
            Player player1 = _playerService.GetPlayer(draftedPlayers[0], draftId);
            Player player2 = _playerService.GetPlayer(draftedPlayers[1], draftId);

        }
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

        [Route("api/[controller]/UpdateDraftTeams/{draftId}")]
        [HttpPut]
        public IActionResult UpdateDraftTeams([FromBody] List<Player> draftedPlayers, int draftId)
        {
            _draftService.UpdateDraftTeams(draftedPlayers, draftId);
            _draftService.Save();

            return Ok();
        }
    }
}
