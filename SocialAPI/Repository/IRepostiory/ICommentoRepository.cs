using SocialAPI.Models;
using System.Linq.Expressions;

namespace SocialAPI.Repository.IRepostiory
{
    public interface ICommentoRepostitory : IRepository<Commento>
    {

        Task<Commento> UpdateAsync(Commento entity);

    }
}
