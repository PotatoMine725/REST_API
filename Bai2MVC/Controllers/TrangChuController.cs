using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http; // Thêm
using System.Threading.Tasks; // Thêm
using Newtonsoft.Json; // Thêm
using Bai2MVC.Models; // Đảm bảo Model SANPHAM tồn tại

namespace Bai2MVC.Controllers
{
    public class TrangChuController : Controller
    {
        // Địa chỉ API
        private const string BaseApiUrl = "http://localhost:5273/";

        // GET: TrangChu (Index) - Lấy tất cả sản phẩm
        public async Task<ActionResult> Index()
        {
            List<SANPHAM> sanPhams = new List<SANPHAM>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseApiUrl);
                // Gọi endpoint lấy tất cả sản phẩm (Giả định là /SanPham)
                HttpResponseMessage response = await client.GetAsync("SanPham");

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    sanPhams = JsonConvert.DeserializeObject<List<SANPHAM>>(responseData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể tải danh sách sản phẩm từ API.";
                }
            }
            // Lấy danh mục cho layout (nếu layout cần)
            ViewBag.Categories = await GetCategoriesApi();
            return View(sanPhams); // Trả về View Index.cshtml của TrangChu
        }

        // Action DanhSachSanPham giờ có thể gọi Index hoặc làm tương tự Index
        // Hoặc bạn có thể xóa nó đi nếu View Index đã đủ
        public async Task<ActionResult> DanhSachSanPham()
        {
            // Cách 1: Gọi lại logic của Index (lặp code)
            List<SANPHAM> sanPhams = new List<SANPHAM>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseApiUrl);
                HttpResponseMessage response = await client.GetAsync("SanPham");
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    sanPhams = JsonConvert.DeserializeObject<List<SANPHAM>>(responseData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể tải danh sách sản phẩm từ API.";
                }
            }
            // Lấy danh mục cho layout (nếu layout cần)
            ViewBag.Categories = await GetCategoriesApi();
            // Trả về view "DanhSachSanPham.cshtml" nếu có, hoặc dùng lại view Index
            return View(sanPhams);

            // Cách 2: Redirect về action Index của HomeController (nếu logic giống hệt)
            // return RedirectToAction("Index", "Home");
        }

        // Helper private: Lấy danh sách Danh mục từ API (Tương tự HomeController)
        private async Task<List<DANHMUC>> GetCategoriesApi()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseApiUrl);
                // Giả định endpoint lấy danh mục là /APIsp/GetDanhMuc
                HttpResponseMessage response = await client.GetAsync("APIsp/GetDanhMuc");

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<DANHMUC>>(content);
                }
            }
            return new List<DANHMUC>(); // Trả về list rỗng nếu lỗi
        }
    }
}