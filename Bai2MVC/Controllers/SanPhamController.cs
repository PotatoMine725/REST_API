using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http; // Thêm
using System.Net.Http.Headers; // Thêm (có thể cần cho POST/PUT)
using System.Threading.Tasks; // Thêm
using System.Web;
using System.Web.Mvc;
using Bai2MVC.Models; // Namespace Models
using Newtonsoft.Json; // Thêm

namespace Bai2MVC.Controllers
{
    public class SanPhamController : Controller
    {
        // Địa chỉ API
        private const string BaseApiUrl = "http://localhost:5273/";

        // GET: SanPham (Danh sách sản phẩm)
        public async Task<ActionResult> Index()
        {
            List<SANPHAM> sanPhams = new List<SANPHAM>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseApiUrl);
                // Giả định endpoint lấy tất cả sản phẩm là /SanPham
                HttpResponseMessage response = await client.GetAsync("SanPham");
                if (response.IsSuccessStatusCode)
                {
                    sanPhams = await response.Content.ReadAsAsync<List<SANPHAM>>(); // Dùng ReadAsAsync tiện hơn
                }
                else
                {
                    ViewBag.ErrorMessage = "Lỗi tải danh sách sản phẩm từ API.";
                }
            }
            return View(sanPhams);
        }

        // GET: SanPham/Details/5 (Chi tiết sản phẩm)
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SANPHAM sanPham = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseApiUrl);
                // Giả định endpoint lấy chi tiết sản phẩm là /SanPham/{id}
                HttpResponseMessage response = await client.GetAsync($"SanPham/{id}");
                if (response.IsSuccessStatusCode)
                {
                    sanPham = await response.Content.ReadAsAsync<SANPHAM>();
                }
            }

            if (sanPham == null)
            {
                return HttpNotFound();
            }
            return View(sanPham);
        }

        // Helper private: Lấy danh sách Danh mục từ API (cho Dropdown)
        private async Task<List<DANHMUC>> GetCategoriesApi()
        {
            using (var client = new HttpClient { BaseAddress = new Uri(BaseApiUrl) })
            {
                // Giả định endpoint lấy danh mục là /APIsp/GetDanhMuc
                HttpResponseMessage response = await client.GetAsync("APIsp/GetDanhMuc");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<List<DANHMUC>>();
                }
            }
            return new List<DANHMUC>();
        }


        // GET: SanPham/Create (Hiển thị form tạo mới)
        public async Task<ActionResult> Create()
        {
            // Lấy danh mục từ API cho dropdown
            ViewBag.MaDanhMuc = new SelectList(await GetCategoriesApi(), "Madanhmuc", "TenDanhmuc"); // Khớp tên thuộc tính Model DANHMUC
            return View();
        }

        // POST: SanPham/Create (Xử lý tạo mới)
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Đổi tên tham số MaDanhMuc để khớp với ViewBag
        public async Task<ActionResult> Create([Bind(Include = "TenSanPham,DonGia,SoLuong,MoTa,MaDanhMuc")] SANPHAM sanpham, HttpPostedFileBase HinhAnhUpload) // Đổi tên file upload
        {
            // Xử lý upload hình ảnh (giữ nguyên logic cũ nhưng lưu vào thư mục ~/HinhAnhSP)
            if (HinhAnhUpload != null && HinhAnhUpload.ContentLength > 0)
            {
                var fileName = System.IO.Path.GetFileName(HinhAnhUpload.FileName);
                var path = System.IO.Path.Combine(Server.MapPath("~/HinhAnhSP"), fileName);
                try
                {
                    HinhAnhUpload.SaveAs(path);
                    sanpham.HinhAnh = fileName; // Lưu tên file vào model
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("HinhAnhUpload", "Lỗi lưu file ảnh: " + ex.Message);
                }
            }
            else
            {
                sanpham.HinhAnh = ""; // Hoặc tên file mặc định nếu không upload
            }


            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseApiUrl);
                    // Giả định endpoint tạo sản phẩm là POST /SanPham
                    HttpResponseMessage response = await client.PostAsJsonAsync("SanPham", sanpham);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Lỗi từ API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    }
                }
            }

            // Nếu ModelState không hợp lệ hoặc API lỗi, load lại dropdown và trả về View
            ViewBag.MaDanhMuc = new SelectList(await GetCategoriesApi(), "Madanhmuc", "TenDanhmuc", sanpham.MaDanhMuc);
            return View(sanpham);
        }

        // GET: SanPham/Edit/5 (Hiển thị form chỉnh sửa)
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SANPHAM sanPham = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseApiUrl);
                HttpResponseMessage response = await client.GetAsync($"SanPham/{id}");
                if (response.IsSuccessStatusCode)
                {
                    sanPham = await response.Content.ReadAsAsync<SANPHAM>();
                }
            }

            if (sanPham == null)
            {
                return HttpNotFound();
            }

            // Lấy danh mục cho dropdown
            ViewBag.MaDanhMuc = new SelectList(await GetCategoriesApi(), "Madanhmuc", "TenDanhmuc", sanPham.MaDanhMuc);
            return View(sanPham);
        }

        // POST: SanPham/Edit/5 (Xử lý chỉnh sửa)
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Thêm Bind để chỉ định các thuộc tính cần cập nhật, MaSanPham lấy từ route
        public async Task<ActionResult> Edit([Bind(Include = "MaSanPham,TenSanPham,DonGia,SoLuong,MoTa,MaDanhMuc,HinhAnh")] SANPHAM sanpham, HttpPostedFileBase HinhAnhUpload)
        {
            string oldImagePath = sanpham.HinhAnh; // Lưu lại tên ảnh cũ phòng trường hợp không upload ảnh mới

            // Xử lý upload hình ảnh mới (nếu có)
            if (HinhAnhUpload != null && HinhAnhUpload.ContentLength > 0)
            {
                var fileName = System.IO.Path.GetFileName(HinhAnhUpload.FileName);
                var path = System.IO.Path.Combine(Server.MapPath("~/HinhAnhSP"), fileName);
                try
                {
                    HinhAnhUpload.SaveAs(path);
                    sanpham.HinhAnh = fileName; // Cập nhật tên file mới
                                                // Cân nhắc xóa file ảnh cũ ở đây nếu cần
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("HinhAnhUpload", "Lỗi lưu file ảnh mới: " + ex.Message);
                }
            }
            else
            {
                // Nếu không upload ảnh mới, giữ nguyên ảnh cũ
                sanpham.HinhAnh = oldImagePath;
            }


            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseApiUrl);
                    // Giả định endpoint cập nhật sản phẩm là PUT /SanPham/{id}
                    // Lưu ý: API cần biết cách xử lý thuộc tính HinhAnh (có thể chỉ cần tên file)
                    HttpResponseMessage response = await client.PutAsJsonAsync($"SanPham/{sanpham.MaSanPham}", sanpham);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Lỗi từ API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    }
                }
            }

            // Nếu ModelState không hợp lệ hoặc API lỗi, load lại dropdown và trả về View
            ViewBag.MaDanhMuc = new SelectList(await GetCategoriesApi(), "Madanhmuc", "TenDanhmuc", sanpham.MaDanhMuc);
            return View(sanpham);
        }

        // GET: SanPham/Delete/5 (Hiển thị trang xác nhận xóa)
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SANPHAM sanPham = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseApiUrl);
                HttpResponseMessage response = await client.GetAsync($"SanPham/{id}");
                if (response.IsSuccessStatusCode)
                {
                    sanPham = await response.Content.ReadAsAsync<SANPHAM>();
                }
            }

            if (sanPham == null)
            {
                return HttpNotFound();
            }
            return View(sanPham); // Trả về View Delete.cshtml
        }

        // POST: SanPham/Delete/5 (Xác nhận và thực hiện xóa)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseApiUrl);
                // Giả định API có endpoint xóa sản phẩm là DELETE /SanPham/{id}
                HttpResponseMessage response = await client.DeleteAsync($"SanPham/{id}");

                if (response.IsSuccessStatusCode)
                {
                    // Cân nhắc xóa file ảnh liên quan trên server ở đây
                    return RedirectToAction("Index");
                }
                else
                {
                    // Nếu xóa lỗi, quay lại trang xác nhận với thông báo lỗi
                    TempData["ApiError"] = $"Lỗi xóa từ API: {response.StatusCode}";
                    return RedirectToAction("Delete", new { id = id });
                }
            }
        }
    }
}