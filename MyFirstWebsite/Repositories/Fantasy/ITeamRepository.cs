using MyFirstWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Repositories
{
    public interface ITeamRepository
    {
        Team GetTeam(int teamId);
        Team GetTeam(int draftId, int draftPosition);
        List<Team> GetAllTeams(int draftId);
        void Save();
        void UpdateTeam(Team team);
    }
}
