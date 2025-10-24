using System.ComponentModel.DataAnnotations;

namespace QLBH_REST.Models
{
    public class SANPHAM
    {
        [Key]
        public int MaSanPham { get; set; }

        public string? TenSanPham { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? SoLuong { get; set; }
        public string? HinhAnh { get; set; }
        public string? MoTa { get; set; }
        public int MaDanhMuc { get; set; }
    }
}
