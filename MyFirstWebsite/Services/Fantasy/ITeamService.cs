﻿using MyFirstWebsite.Models;
using MyFirstWebsite.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Services.Fantasy
{
    public interface ITeamService : ITeamRepository
    {
        Team GetAvailablePlayers(Draft draft);
        Team GetAvailablePlayersById(int draftId);
        Team GetUserTeam(Draft draft);
        Team GetUserTeamById(int draftId);
        List<Player> GetDraftedPlayers(int draftId);

    }
}
