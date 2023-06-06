using SocialAPI.Models;
using System.Linq.Expressions;

namespace SocialAPI.Repository.IRepostiory
{
    public interface IPostRepostitory : IRepository<Post>
    {

        Task<Post> UpdateAsync(Post entity);
        Task<List<Post>> GetExplorePost();
    }
}
