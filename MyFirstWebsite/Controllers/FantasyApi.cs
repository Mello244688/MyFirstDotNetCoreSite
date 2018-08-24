using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private AppDbContext appDbContext;

        public FantasyApi(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
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


        ////GET: api/<controller>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        ////GET api/<controller>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
