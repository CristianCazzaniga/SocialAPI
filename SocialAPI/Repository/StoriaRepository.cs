using SocialAPI.Data;
using SocialAPI.Models;
using SocialAPI.Repository.IRepostiory;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SocialAPI.Repository
{
    public class StoriaRepository : Repository<Storia>, IStoriaRepostitory
    {
        private readonly ApplicationDbContext _db;
        public StoriaRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

  
   
    }
}
