using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Models
{
    public interface IDraftTeamsRepository
    {
        IEnumerable<Team> GetAllDraftTeams();
    }
}
