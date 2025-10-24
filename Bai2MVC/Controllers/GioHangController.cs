using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bai2MVC.Models;
using System.Net.Http; // Thêm
using System.Threading.Tasks; // Thêm
using Newtonsoft.Json; // Thêm

namespace Bai2MVC.Controllers
{
    public class GioHangController : Controller
    {
        // Địa chỉ API
        private const string BaseApiUrl = "http://localhost:5273/";
        private const string CART_SESSION = "GioHang";

        // Lấy Giỏ hàng từ Session
        public List<GioHangItem> LayGioHang()
        {
            var gioHang = Session[CART_SESSION] as List<GioHangItem>;
            if (gioHang == null)
            {
                gioHang = new List<GioHangItem>();
                Session[CART_SESSION] = gioHang;
            }
            return gioHang;
        }

        // GET: /GioHang (Action Index để xem giỏ hàng)
        public ActionResult Index()
        {
            List<GioHangItem> gioHang = LayGioHang();

            // Tính tổng (nếu view cần)
            ViewBag.TongSoLuong = gioHang.Sum(n => n.SoLuong);
            ViewBag.TongThanhTien = gioHang.Sum(n => n.ThanhTien);

            // Cần tạo View "Index.cshtml" trong thư mục "Views/GioHang"
            return View(gioHang);
        }

        // Action Thêm Giỏ Hàng (ĐÃ SỬA ĐỂ GỌI API)
        public async Task<ActionResult> ThemGioHang(int iMaSP, string strURL)
        {
            List<GioHangItem> gioHang = LayGioHang();
            GioHangItem sanPhamTrongGio = gioHang.Find(n => n.MaSanPham == iMaSP);

            if (sanPhamTrongGio != null)
            {
                sanPhamTrongGio.SoLuong++;
            }
            else
            {
                // Gọi API để lấy thông tin chi tiết sản phẩm
                SANPHAM spApi = null;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseApiUrl);
                    // Giả định API có endpoint "GET /SanPham/{id}"
                    HttpResponseMessage response = await client.GetAsync($"SanPham/{iMaSP}");

                    if (response.IsSuccessStatusCode)
                    {
                        spApi = await response.Content.ReadAsAsync<SANPHAM>();
                    }
                }

                // Nếu API tìm thấy sản phẩm
                if (spApi != null)
                {
                    // Tạo mới GioHangItem và gán giá trị từ API
                    sanPhamTrongGio = new GioHangItem
                    {
                        MaSanPham = iMaSP,
                        TenSanPham = spApi.TenSanPham, // Khớp tên thuộc tính Model SANPHAM
                        HinhAnh = spApi.HinhAnh,       // Khớp tên thuộc tính Model SANPHAM
                        DonGia = spApi.DonGia ?? 0,    // Khớp tên thuộc tính Model SANPHAM
                        SoLuong = 1
                    };
                    gioHang.Add(sanPhamTrongGio);
                }
                else
                {
                    // Xử lý nếu API không tìm thấy sản phẩm
                    TempData["ApiError"] = $"Không tìm thấy sản phẩm có mã {iMaSP} từ API.";
                    return Redirect(strURL); // Quay lại trang trước đó
                }
            }

            Session[CART_SESSION] = gioHang;
            return Redirect(strURL);
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public ActionResult XoaGioHang(int iMaSP)
        {
            List<GioHangItem> gioHang = LayGioHang();
            GioHangItem sanPham = gioHang.SingleOrDefault(n => n.MaSanPham == iMaSP);

            if (sanPham != null)
            {
                gioHang.Remove(sanPham);
            }
            Session[CART_SESSION] = gioHang;

            // Nếu giỏ hàng rỗng, quay về trang chủ, ngược lại về trang giỏ hàng
            return gioHang.Count == 0 ? RedirectToAction("Index", "Home") : RedirectToAction("Index");
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        [HttpPost]
        public ActionResult CapNhatGioHang(int iMaSP, FormCollection f)
        {
            // Lấy số lượng mới từ form, kiểm tra hợp lệ
            int newQuantity;
            if (int.TryParse(f["txtSoLuong"], out newQuantity) && newQuantity > 0)
            {
                List<GioHangItem> gioHang = LayGioHang();
                GioHangItem sanPham = gioHang.SingleOrDefault(n => n.MaSanPham == iMaSP);

                if (sanPham != null)
                {
                    sanPham.SoLuong = newQuantity;
                }
                Session[CART_SESSION] = gioHang;
            }
            else
            {
                // Xử lý nếu số lượng không hợp lệ (ví dụ: thông báo lỗi)
                TempData["CartError"] = "Số lượng cập nhật không hợp lệ.";
            }
            return RedirectToAction("Index"); // Luôn quay về trang giỏ hàng
        }

        // Action mới: Xóa tất cả giỏ hàng (Cần cho link trong View)
        public ActionResult XoaTatCaGioHang()
        {
            List<GioHangItem> gioHang = LayGioHang();
            gioHang.Clear();
            Session[CART_SESSION] = gioHang;
            // Chuyển hướng về trang chủ sau khi xóa
            return RedirectToAction("Index", "Home");
        }
    }
}