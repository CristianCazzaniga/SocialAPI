using SocialAPI.Data;
using SocialAPI.Models;
using SocialAPI.Repository.IRepostiory;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SocialAPI.Repository
{
    public class CommentoRepository : Repository<Commento>, ICommentoRepostitory
    {
        private readonly ApplicationDbContext _db;
        public CommentoRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

  
        public async Task<Commento> UpdateAsync(Commento entity)
        {
            entity.DataModifica = DateTime.Now;
            _db.Commenti.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
