// File: QLBH_REST/Controllers/SanPhamController.cs (Đã hoàn thiện)
using Microsoft.AspNetCore.Mvc;
using QLBH_REST.Models;

namespace QLBH_REST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SanPhamController : ControllerBase
    {
        public ISanPham ISanPham;
        public SanPhamController(ISanPham spRepo)
        {
            ISanPham = spRepo;
        }

        // GET: /SanPham
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SANPHAM>>> GetAllSanPham()
        {
            try
            {
                var sanphams = await ISanPham.GetAllSanPham();
                return Ok(sanphams);
            }
            catch (Exception ex)
            {
                // Trả về 500 khi có lỗi Server
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi kết nối hoặc truy vấn dữ liệu.");
            }
        }

        // GET: /SanPham/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SANPHAM>> GetSanPhamByID(int id)
        {
            try
            {
                var sanpham = await ISanPham.GetSanPhamByID(id);

                if (sanpham == null)
                {
                    // Trả về 404 Not Found nếu không tìm thấy
                    return NotFound($"Không tìm thấy sản phẩm với mã: {id}");
                }

                return Ok(sanpham);
            }
            catch (Exception ex)
            {
                // Trả về 500 khi có lỗi Server
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi kết nối hoặc truy vấn dữ liệu.");
            }
        }

        // POST: /SanPham
        [HttpPost]
        public async Task<ActionResult<SANPHAM>> AddSanPham(SANPHAM sanpham)
        {
            try
            {
                if (sanpham == null)
                {
                    return BadRequest("Dữ liệu sản phẩm không hợp lệ.");
                }

                var newSanPham = await ISanPham.AddSanPham(sanpham);

                // Trả về 201 Created với URI của tài nguyên mới
                return CreatedAtAction(nameof(GetSanPhamByID), new { id = newSanPham.MaSanPham }, newSanPham);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi khi thêm sản phẩm vào cơ sở dữ liệu.");
            }
        }

        // PUT: /SanPham/1
        [HttpPut("{id:int}")]
        public async Task<ActionResult<SANPHAM>> UpdateSanPham(int id, SANPHAM sanpham)
        {
            try
            {
                // Kiểm tra ID trong URL và ID trong body có khớp không
                if (id != sanpham.MaSanPham)
                {
                    return BadRequest("Mã sản phẩm trong URL và trong dữ liệu không khớp.");
                }

                var updatedSanPham = await ISanPham.UpdateSanPham(id, sanpham);

                if (updatedSanPham == null)
                {
                    // Trả về 404 Not Found nếu không tìm thấy
                    return NotFound($"Không tìm thấy sản phẩm với mã: {id} để cập nhật.");
                }

                // Trả về 200 OK
                return Ok(updatedSanPham);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi khi cập nhật sản phẩm trong cơ sở dữ liệu.");
            }
        }

        // DELETE: /SanPham/1
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<SANPHAM>> DeleteSanPham(int id)
        {
            try
            {
                var deletedSanPham = await ISanPham.DeleteSanPham(id);

                if (deletedSanPham == null)
                {
                    // Trả về 404 Not Found nếu không tìm thấy
                    return NotFound($"Không tìm thấy sản phẩm với mã: {id} để xóa.");
                }

                // Trả về 200 OK (có thể dùng 204 No Content nếu không muốn trả về đối tượng)
                return Ok(deletedSanPham);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi khi xóa sản phẩm khỏi cơ sở dữ liệu.");
            }
        }
    }
}