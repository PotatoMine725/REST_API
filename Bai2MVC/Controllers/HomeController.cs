using System.Collections.Generic;
using System.Web.Mvc;
using System.Net.Http; // Cần NuGet: Microsoft.AspNet.WebApi.Client
using System.Threading.Tasks; // Cần để dùng async/await
using Newtonsoft.Json; // Cần NuGet: Newtonsoft.Json
using Bai2MVC.Models; // Đảm bảo bạn đã tạo thư mục Models và class SANPHAM/DANHMUC

namespace Bai2MVC.Controllers
{
    public class HomeController : Controller
    {
        // Địa chỉ cơ sở của API - Sử dụng cổng HTTP 5272 từ launchSettings.json
        private const string BaseApiUrl = "http://localhost:5273/";

        // 1. Action Index: Lấy TẤT CẢ sản phẩm
        // Chuyển từ ActionResult sang Task<ActionResult> và dùng async/await
        public async Task<ActionResult> Index()
        {
            List<SANPHAM> sanPhams = new List<SANPHAM>();

            // Tạo HttpClient
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(BaseApiUrl);

                // Gọi endpoint GetAllSanPham từ API: /SanPham
                HttpResponseMessage response = await client.GetAsync("SanPham");

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    // Deserialize bằng Newtonsoft.Json
                    sanPhams = JsonConvert.DeserializeObject<List<SANPHAM>>(responseData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể kết nối với API hoặc truy vấn thất bại.";
                }
            }
            // Trả về View Index.cshtml với danh sách sản phẩm làm Model
            return View(sanPhams);
        }

        // 2. Action DanhSach: Lọc sản phẩm theo Danh mục (tương tự pageDanhSach trong tài liệu)
        public async Task<ActionResult> DanhSach(int? id) // 'id' là Madanhmuc
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            List<SANPHAM> sanPhams = new List<SANPHAM>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(BaseApiUrl);

                // Giả định API của bạn có endpoint lọc sản phẩm theo danh mục: /SanPham/ByDanhMuc/{id}
                string url = $"SanPham/ByDanhMuc/{id}";

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    sanPhams = JsonConvert.DeserializeObject<List<SANPHAM>>(responseData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Không tìm thấy sản phẩm trong danh mục này hoặc lỗi kết nối.";
                }
            }
            // Trả về View DanhSach.cshtml (hoặc Index.cshtml nếu bạn muốn dùng chung)
            return View("Index",sanPhams);
        }

        // 3. Phương thức NonAction: Lấy danh sách Danh mục (để dùng cho sidebar)
        [NonAction]
        public async Task<List<DANHMUC>> GetCategories()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(BaseApiUrl);
                // Giả định bạn sử dụng endpoint GetDanhMuc từ code mẫu của thầy: /APIsp/GetDanhMuc
                HttpResponseMessage response = await client.GetAsync("APIsp/GetDanhMuc");

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<DANHMUC>>(content);
                }
            }
            return new List<DANHMUC>();
        }

        // Các Action khác (About, Contact) giữ nguyên hoặc xóa nếu không cần
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}