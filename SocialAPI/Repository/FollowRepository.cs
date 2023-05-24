using SocialAPI.Data;
using SocialAPI.Models;
using SocialAPI.Repository.IRepostiory;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SocialAPI.Repository
{
    public class SeguiRepository : Repository<Segui>, ISeguiRepository
    {
        private readonly ApplicationDbContext _db;
        public SeguiRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
   
    }
}
