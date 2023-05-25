using SocialAPI.Data;
using SocialAPI.Models;
using SocialAPI.Repository.IRepostiory;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SocialAPI.Repository
{
    public class SegnalazioneRepository : Repository<Segnalazione>, ISegnalazioneRepository
    {
        private readonly ApplicationDbContext _db;
        public SegnalazioneRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
   
    }
}
