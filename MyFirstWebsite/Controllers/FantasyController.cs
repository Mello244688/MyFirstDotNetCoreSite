using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyFirstWebsite.Models;
using MyFirstWebsite.Services;
using MyFirstWebsite.Services.Fantasy;
using MyFirstWebsite.ViewModels;
using System.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFirstWebsite.Controllers
{
    public class FantasyController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IFantasyProsDataGrabber _fantasyProsDataGrabber;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDraftService _draftService;
        private readonly IPlayerService _playerService;
        private readonly ITeamService _teamService;

        public FantasyController(IConfiguration configuration
                                , IFantasyProsDataGrabber fantasyProsDataGrabber
                                , AppDbContext appDbContext
                                , UserManager<ApplicationUser> userManager
                                , IDraftService draftService
                                , IPlayerService playerService
                                , ITeamService teamService)
        {
            _configuration = configuration;
            _fantasyProsDataGrabber = fantasyProsDataGrabber;
            _userManager = userManager;
            _draftService = draftService;
            _playerService = playerService;
            _teamService = teamService;
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index()
        {
            FantasyViewModel fantasyViewModel = new FantasyViewModel();

            fantasyViewModel.Players = _fantasyProsDataGrabber.GetPlayers(ScoringType.Standard);
            fantasyViewModel.PageHeading = "Standard";

            return View(fantasyViewModel);
        }

        [HttpPost]
        public IActionResult Index(FantasyViewModel model)
        {
            model.Players = _fantasyProsDataGrabber.GetPlayers(model.ScoringType);
            model.PageHeading = _draftService.GetPageHeading(model.ScoringType);

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult New()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult New(NewViewModel newDraft)
        {
            string userId = _userManager.GetUserId(User);

            Draft draft = _draftService.SetupDraft(newDraft, userId);

            _draftService.InsertDraft(draft);

            _draftService.Save();

            return RedirectToAction(nameof(Draft), new { id = draft.Id});
        }

        [Authorize]
        public IActionResult Draft(int id)
        {
            DraftViewModel draftViewModel = new DraftViewModel();
            var userId = _userManager.GetUserId(User);

            Draft draft = _draftService.GetDraft(id);
            Team userTeam = _teamService.GetUserTeam(draft);
            Team availablePlayers = _teamService.GetAvailablePlayers(draft);

            draftViewModel.LeagueName = draft.LeagueName;
            draftViewModel.TeamName = userTeam.TeamName;
            draftViewModel.DraftPosition = draft.UserDraftPosition;
            draftViewModel.NumberOfTeams = draft.NumberOfTeams;
            draftViewModel.MyPlayers = userTeam.Players.ToHashSet();
            draftViewModel.AvailablePlayers = availablePlayers.Players.ToHashSet();

            int pick = _draftService.GetPick(draft);
            draftViewModel.pick = pick;
            draftViewModel.round = _draftService.GetRound(pick, draft.NumberOfTeams);

            return View(draftViewModel);
        }

        [Authorize]
        public IActionResult Drafts() //user draft teams
        {   
            return View(_draftService.GetAllDrafts(_userManager.GetUserId(User)));
        }
    }//class
} //namespace
