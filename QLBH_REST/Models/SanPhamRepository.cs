using Microsoft.EntityFrameworkCore;

namespace QLBH_REST.Models
{
    public class SanPhamRepository : ISanPham
    {
        private readonly DBQLBH dbqlbh;
        public SanPhamRepository(DBQLBH db)
        {
            dbqlbh = db;
        }
        public async Task<IEnumerable<SANPHAM>> GetAllSanPham()
        {
            return await this.dbqlbh.tbSANPHAM.ToListAsync();
        }

        public async Task<SANPHAM> GetSanPhamByID(int id)
        {
            return await this.dbqlbh.tbSANPHAM.FirstOrDefaultAsync(x => x.MaSanPham == id);
        }

        public async Task<SANPHAM> AddSanPham(SANPHAM sp)
        {
            throw new NotImplementedException();
        }

        public async Task<SANPHAM> UpdateSanPham(int id, SANPHAM sp)
        {
            throw new NotImplementedException();
        }

        public async Task<SANPHAM> DeleteSanPham(int id)
        {
            throw new NotImplementedException();
        }
    }
}
