using MagicVilla2_WebAPI.Models;

namespace MagicVilla2_WebAPI.Repository.IRepository;

public interface IVillaRepository : IRepository<Villa>
{
    Task<Villa> UpdateAsync(Villa entity);
}
