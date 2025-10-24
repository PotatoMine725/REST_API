namespace Bai2MVC.Models
{
    public class SANPHAM
    {
        // Phải khớp với tên thuộc tính trong Model API (SANPHAM.cs)
        public int? MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? SoLuong { get; set; }
        public string HinhAnh { get; set; }
        public string MoTa { get; set; }
        public int? MaDanhMuc { get; set; }
    }
}