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

                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi kết nối hoặc truy vấn dữ liệu.");
            }
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SANPHAM>> GetSanPhamByID(int id)
        {
            try
            {
                var sanpham = await ISanPham.GetSanPhamByID(id);
                return Ok(sanpham);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status302Found, "Lỗi kết nối hoặc truy vấn dữ liệu.");
            }
        }
    }
}
