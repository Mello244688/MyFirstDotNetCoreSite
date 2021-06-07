using MyFirstWebsite.Models;
using MyFirstWebsite.Repositories.Fantasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Services.Fantasy
{
    public class PlayerService : PlayerRepository, IPlayerService
    {
        public PlayerService(AppDbContext appDbContext) : base(appDbContext)
        {

        }

    }
}
