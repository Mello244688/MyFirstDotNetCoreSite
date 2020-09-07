using MyFirstWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Repositories
{
    public interface IDraftRepository
    {
        Draft GetDraft(int id);
        List<Draft> GetAllDrafts(string UserId);
        void InsertDraft(Draft draft);
        void DeleteDraft(Draft draft);
        void DeleteDraftById(int id);
        void UpdateDraft(Draft draft);
        void Save();

    }
}
