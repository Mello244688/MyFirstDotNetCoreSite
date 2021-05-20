using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyFirstWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Repositories.Fantasy
{
    public class DraftRepository : IDraftRepository
    {
        private readonly AppDbContext _appDbContext;

        public DraftRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void DeleteDraft(Draft draft)
        {
            _appDbContext.Drafts.Remove(draft);
        }

        public void DeleteDraftById(int id)
        {
            Draft draft = new Draft { Id = id };
            _appDbContext.Attach(draft);
            _appDbContext.Remove(draft);
        }

        public List<Draft> GetAllDrafts(string userId)
        {
            return _appDbContext.Drafts
                .Where(ds => ds.UserId == userId)
                .Include(ts => ts.Teams).ToList();
        }

        public Draft GetDraft(int id)
        {
            Draft draft = _appDbContext.Drafts
                .Where(d => d.Id == id)
                .Include(d => d.Teams)
                    .ThenInclude(t => t.Players)
                .FirstOrDefault();

            return draft;
        }

        public void InsertDraft(Draft draft)
        {
            _appDbContext.Add(draft);
        }

        public void Save()
        {
            _appDbContext.SaveChanges();
        }

        public void UpdateDraft(Draft draft)
        {
            _appDbContext.Attach(draft);
        }
    }
}
