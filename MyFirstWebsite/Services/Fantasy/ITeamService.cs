using MyFirstWebsite.Models;
using MyFirstWebsite.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Services.Fantasy
{
    public interface ITeamService : ITeamRepository
    {
        Team GetUserTeam(Draft draft);
        Team GetAvailablePlayers(Draft draft);
    }
}
