using SocialAPI.Data;
using SocialAPI.Models;
using SocialAPI.Repository.IRepostiory;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SocialAPI.Repository
{
    public class AnnuncioRepository : Repository<Annuncio>, IAnnuncioRepository
    {
        private readonly ApplicationDbContext _db;
        public AnnuncioRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

  
        public async Task<Annuncio> UpdateAsync(Annuncio entity)
        {
            _db.Annunci.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
