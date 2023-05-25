using SocialAPI.Data;
using SocialAPI.Models;
using SocialAPI.Repository.IRepostiory;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SocialAPI.Repository
{
    public class MessaggioRepository : Repository<Messaggio>, IMessaggioRepository
    {
        private readonly ApplicationDbContext _db;
        public MessaggioRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
   
    }
}
