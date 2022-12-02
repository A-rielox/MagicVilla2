using MagicVilla_WebAPI.Data;
using MagicVilla2_WebAPI.Models;
using MagicVilla2_WebAPI.Models.Dto;
using MagicVilla2_WebAPI.Repository.IRepository;

namespace MagicVilla2_WebAPI.Repository
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly ApplicationDbContext _db;

        public VillaNumberRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        ///////////////////////////////////////////////
        /// ///////////////////////////////////////////////
        ///
        public async Task<VillaNumber> UpdateAsync(VillaNumber entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _db.VillaNumbers.Update(entity);

            await _db.SaveChangesAsync();

            return entity;
        }
    }
}
