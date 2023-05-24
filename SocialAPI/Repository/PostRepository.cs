using SocialAPI.Data;
using SocialAPI.Models;
using SocialAPI.Repository.IRepostiory;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SocialAPI.Repository
{
    public class PostRepository : Repository<Post>, IPostRepostitory
    {
        private readonly ApplicationDbContext _db;
        public PostRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

  
        public async Task<Post> UpdateAsync(Post entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _db.Posts.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
