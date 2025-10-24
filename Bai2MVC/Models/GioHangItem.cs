using System.Linq;

namespace Bai2MVC.Models
{
    // Lớp này CHỈ NÊN là một POCO (Plain Old C# Object)
    // Nó không nên chứa logic truy cập CSDL.
    public class GioHangItem
    {
        // 1. Thuộc tính chứa thông tin sản phẩm
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string HinhAnh { get; set; }
        public decimal DonGia { get; set; }

        // 2. Thuộc tính số lượng và thành tiền
        public int SoLuong { get; set; }
        public decimal ThanhTien
        {
            get { return SoLuong * DonGia; }
        }

        // 3. Constructor rỗng (Default constructor)
        // Cần thiết để Controller có thể tạo mới: new GioHangItem { ... }
        public GioHangItem()
        {
        }
    }
}