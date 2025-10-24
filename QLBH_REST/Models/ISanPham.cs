namespace QLBH_REST.Models
{
    public interface ISanPham
    {
        public Task<IEnumerable<SANPHAM>> GetAllSanPham();
        public Task<SANPHAM> GetSanPhamByID(int id);
        public Task<SANPHAM> AddSanPham(SANPHAM sp);
        public Task<SANPHAM> UpdateSanPham(int id, SANPHAM sp);
        public Task<SANPHAM> DeleteSanPham(int id);
    }
}
