using SocialAPI.Data;
using SocialAPI.Models;
using SocialAPI.Repository.IRepostiory;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SocialAPI.Repository
{
    public class ChatRepository : Repository<Chat>, IChatRepository
    {
        private readonly ApplicationDbContext _db;
        public ChatRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
   
    }
}
