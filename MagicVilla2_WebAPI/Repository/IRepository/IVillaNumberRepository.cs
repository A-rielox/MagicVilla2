using MagicVilla2_WebAPI.Models;
using MagicVilla2_WebAPI.Models.Dto;

namespace MagicVilla2_WebAPI.Repository.IRepository
{
    public interface IVillaNumberRepository : IRepository<VillaNumber>
    {
        Task<VillaNumber> UpdateAsync(VillaNumber entity);
    }
}
