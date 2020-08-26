using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyFirstWebsite.Models;
using MyFirstWebsite.Services;
using MyFirstWebsite.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFirstWebsite.Controllers
{
    public class FantasyController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IFantasyProsDataGrabber _fantasyProsDataGrabber;
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        protected UserManager<ApplicationUser> UserManager { get; set; }

        public FantasyController(IConfiguration configuration, IFantasyProsDataGrabber fantasyProsDataGrabber, AppDbContext appDbContext, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _fantasyProsDataGrabber = fantasyProsDataGrabber;
            _appDbContext = appDbContext;
            _userManager = userManager;
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
                    team.Players = GetPlayers(newDraft.ScoringType);
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
                        TeamName = $"Team {i}" ,
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
            draft.UserId = _userManager.GetUserId(User);

            _appDbContext.Drafts.Add(draft);

            _appDbContext.SaveChanges();

            return RedirectToAction(nameof(Draft), new { id = draft.Id});
        }

        [Authorize]
        public IActionResult Draft(int id)
        {
            DraftViewModel draftViewModel = new DraftViewModel();
            var userId = _userManager.GetUserId(User);

            Draft draft = _appDbContext.Drafts
                .Where(d => d.Id == id && d.UserId.Equals(userId))
                .Include(d => d.Teams)
                    .ThenInclude(t => t.Players)
                .FirstOrDefault();

            Team userTeam = draft.Teams
                .Where(t => t.DraftPosition == draft.UserDraftPosition).FirstOrDefault();

            Team availablePlayers = draft.Teams
                .Where(t => t.DraftPosition == 0).FirstOrDefault();

            draftViewModel.LeagueName = draft.LeagueName;
            draftViewModel.DraftPosition = draft.UserDraftPosition;
            draftViewModel.NumberOfTeams = draft.NumberOfTeams;
            draftViewModel.MyPlayers = userTeam.Players.ToHashSet();
            draftViewModel.AvailablePlayers = availablePlayers.Players.ToHashSet();

            int pick = GetPick(draft);
            draftViewModel.pick = pick;
            draftViewModel.round = GetRound(pick, draft.NumberOfTeams);

            return View(draftViewModel);
        }

        [Authorize]
        public IActionResult Drafts() //user draft teams
        {
            var drafts = _appDbContext.Drafts
                .Where(d => d.UserId == _userManager.GetUserId(User))
                .Include(t => t.Teams);
            
            return View(drafts);
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
            HashSet<Player> players = _fantasyProsDataGrabber.GetPlayers(scoringType).ToHashSet();

            return players;
        }

        private int GetPick(Draft draft)
        {
            List<Player> draftedPlayers = new List<Player>();

            foreach (var team in draft.Teams.Skip(1)) //first is available players
            {
                draftedPlayers.AddRange(team.Players);
            }

            return draftedPlayers.Count + 1;
        }

        private int GetRound(int pick, int numberOfTeams)
        {
            return (int)Math.Ceiling((float)pick / numberOfTeams);
        }

    }//class
} //namespace
