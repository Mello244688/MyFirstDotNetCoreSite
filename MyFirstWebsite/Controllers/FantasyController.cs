using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyFirstWebsite.Models;
using MyFirstWebsite.Services;
using MyFirstWebsite.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFirstWebsite.Controllers
{
    public class FantasyController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IFantasyProsDataGrabber fantasyProsDataGrabber;
        private readonly AppDbContext appDbContext;
        private readonly UserManager<ApplicationUser> userManager;

        protected UserManager<ApplicationUser> UserManager { get; set; }

        public FantasyController(IConfiguration configuration, IFantasyProsDataGrabber fantasyProsDataGrabber, AppDbContext appDbContext, UserManager<ApplicationUser> userManager)
        {
            this.configuration = configuration;
            this.fantasyProsDataGrabber = fantasyProsDataGrabber;
            this.appDbContext = appDbContext;
            this.userManager = userManager;
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index()
        {
            FantasyViewModel fantasyViewModel = new FantasyViewModel();

            fantasyViewModel.Players = fantasyProsDataGrabber.GetPlayers(ScoringType.Standard);
            fantasyViewModel.PageHeading = "Standard";

            return View(fantasyViewModel);
        }

        [HttpPost]
        public IActionResult Index(FantasyViewModel model)
        {
            model.Players = GetPlayers(model.ScoringType);
            model.PageHeading = GetPageHeading(model.ScoringType);

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
            var team = new Team()
            {
                LeagueName = newDraft.LeagueName,
                TeamName = newDraft.TeamName,
                LineUp = new HashSet<TeamPlayer>(),
                UserId = userManager.GetUserId(User),
                Draft = new Draft()
            };

            var draft = new Draft()
            {
                ScoringType = newDraft.ScoringType,
                TeamsInDraft = new HashSet<Team>(),
                AvailablePlayers = new HashSet<DraftPlayer>()
            };

            var availableDraftPlayers = new HashSet<DraftPlayer>();

            foreach (var player in GetPlayers(newDraft.ScoringType))
            {
                var draftPlayer = new DraftPlayer();
                draftPlayer.Draft = draft;
                draftPlayer.Player = player;
                availableDraftPlayers.Add(draftPlayer);
            }

            draft.AvailablePlayers = availableDraftPlayers;
            
            team.Draft = draft;
            draft.TeamsInDraft.Add(team);

            appDbContext.Drafts.Add(draft);
            appDbContext.Teams.Add(team);

            appDbContext.SaveChanges();

            return RedirectToAction(nameof(Draft), new { id = team.Id});
        }

        [Authorize]
        public IActionResult Draft(int id)
        {
            DraftViewModel draftViewModel = new DraftViewModel();
            var userId = userManager.GetUserId(User);

            var draft = appDbContext.Teams
                .Where(t => t.UserId == userId && t.DraftId == id)
                .Select(t => t.Draft)
                .Include(t => t.AvailablePlayers)
                .ThenInclude(p => p.Player)
                .Include(t => t.TeamsInDraft)
                .ThenInclude(t => t.LineUp);

            var team = draft.FirstOrDefault().TeamsInDraft.Where(ts => ts.UserId == userId).FirstOrDefault();

            draftViewModel.Players = draft.FirstOrDefault().AvailablePlayers.Select(p => p.Player).OrderBy(p => p.Rank).ToHashSet();
            draftViewModel.MyPlayers = team.LineUp.Select(p => p.Player).ToHashSet();
            draftViewModel.LeagueName = team.LeagueName;
            draftViewModel.TeamName = team.TeamName;

            return View(draftViewModel);
        }

        [Authorize]
        public IActionResult Teams()
        {
            var teams = appDbContext.Teams.Where(t => t.UserId == userManager.GetUserId(User)).ToHashSet();
            
            return View(teams);
        }

        private string GetPageHeading(ScoringType type)
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

        private ICollection<Player> GetPlayers(ScoringType scoringType)
        {
            var players = new HashSet<Player>();

            players = fantasyProsDataGrabber.GetPlayers(scoringType).ToHashSet();

            return players;
        }

    }//class
} //namespace
