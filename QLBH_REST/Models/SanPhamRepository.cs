// File: QLBH_REST/Models/SanPhamRepository.cs (Đã hoàn thiện)
using Microsoft.EntityFrameworkCore;

namespace QLBH_REST.Models
{
    public class SanPhamRepository : ISanPham
    {
        // DBQLBH là DbSet<SANPHAM> tbSANPHAM { get; set; }
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
            // Dùng FindAsync để tìm theo khóa chính MaSanPham
            return await this.dbqlbh.tbSANPHAM.FindAsync(id);
        }

        public async Task<SANPHAM> AddSanPham(SANPHAM sp)
        {
            var result = await dbqlbh.tbSANPHAM.AddAsync(sp);
            await dbqlbh.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<SANPHAM> UpdateSanPham(int id, SANPHAM sp)
        {
            var existingSp = await dbqlbh.tbSANPHAM.FindAsync(id);

            if (existingSp != null)
            {
                // Cập nhật từng thuộc tính. Lưu ý: Không cập nhật MaSanPham
                existingSp.TenSanPham = sp.TenSanPham;
                existingSp.DonGia = sp.DonGia;
                existingSp.SoLuong = sp.SoLuong;
                existingSp.HinhAnh = sp.HinhAnh;
                existingSp.MoTa = sp.MoTa;
                existingSp.MaDanhMuc = sp.MaDanhMuc; // Đã là int?

                await dbqlbh.SaveChangesAsync();
                return existingSp;
            }
            return null;
        }

        public async Task<SANPHAM> DeleteSanPham(int id)
        {
            var sanphamToDelete = await dbqlbh.tbSANPHAM.FindAsync(id);
            if (sanphamToDelete != null)
            {
                dbqlbh.tbSANPHAM.Remove(sanphamToDelete);
                await dbqlbh.SaveChangesAsync();
                return sanphamToDelete;
            }
            return null;
        }
    }
}