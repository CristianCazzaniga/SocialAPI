using SocialAPI.Models;
using System.Linq.Expressions;

namespace SocialAPI.Repository.IRepostiory
{
    public interface IAnnuncioRepository : IRepository<Annuncio>
    {

        Task<Annuncio> UpdateAsync(Annuncio entity);
        Task<List<Annuncio>> GetAnnunciCasual();
    }
}
