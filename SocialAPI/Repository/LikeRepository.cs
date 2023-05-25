using SocialAPI.Data;
using SocialAPI.Models;
using SocialAPI.Repository.IRepostiory;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SocialAPI.Repository
{
    public class LikeRepository : Repository<Like>, ILikeRepostitory
    {
        private readonly ApplicationDbContext _db;
        public LikeRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
   
    }
}
