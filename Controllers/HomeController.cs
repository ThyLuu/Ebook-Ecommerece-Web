using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApp.Models;
using System.Linq;
using System.Security.Claims; // Thêm dòng này để sử dụng ClaimTypes
using Microsoft.AspNetCore.Authentication; // Thêm dòng này để sử dụng FindFirstValue


namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly OrganicContext _context;

        public HomeController(OrganicContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Departments = _context.Departments.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            var products = _context.Products.ToList();

            // Lấy MemberId từ claims của người dùng đã đăng nhập
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Lấy đúng MemberId từ Claims

            // Debugging: In MemberId để kiểm tra xem có đúng không
            Console.WriteLine($"Logged in MemberId: {memberId}");

            if (memberId != null)
            {
                // Truy vấn giỏ hàng của người dùng dựa trên MemberId
                var cartItems = _context.Carts
                    .Where(c => c.MemberId == memberId)  // Sử dụng MemberId từ Claims
                    .ToList();

                Console.WriteLine($"Cart items found for {memberId}: {cartItems.Count} items");

                // Tính tổng số lượng sản phẩm trong giỏ hàng
                var totalQuantity = cartItems.Sum(c => c.Quantity);  // Tính tổng số lượng
                ViewBag.CartQuantity = totalQuantity;
            }
            else
            {
                ViewBag.CartQuantity = 0; // Nếu chưa đăng nhập thì giỏ hàng có số lượng là 0
            }

            return View(products);
        }

        // Action để hiển thị chi tiết sản phẩm
        public IActionResult Details(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Departments = _context.Departments.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.ProductsRelation = _context.Products.Where(p => p.CategoryId == product.CategoryId && p.ProductId != id).ToList();

            return View(product);
        }

        // Action để hiển thị giỏ hàng (nếu cần)
        [Authorize] // Chỉ cho phép người dùng đã đăng nhập vào
        public IActionResult Cart()
        {
            var memberId = User.Identity?.Name;
            if (memberId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var cartItems = _context.Carts.Where(c => c.MemberId == memberId).ToList();
            return View(cartItems);
        }
    }
}
