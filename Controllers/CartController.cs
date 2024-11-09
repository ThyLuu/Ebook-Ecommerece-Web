using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using WebApp.Services;

namespace WebApp.Models;

[Authorize]
public class CartController : Controller
{
    OrganicContext context;
    VnPaymentService service;
    public CartController(OrganicContext context, VnPaymentService service){
        this.context = context;
        this.service= service;
    }

    public IActionResult Checkout(){
        ViewBag.Departments = context.Departments.ToList();
        ViewBag.Categories = context.Categories.ToList();
        string? memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(memberId)){
            return Redirect("/auth/login");
        }
        List<Cart> carts = context.Carts.Include(p => p.Product).Where(p => p.MemberId == memberId).ToList();
        ViewBag.Total = carts.Sum(p => p.Quantity * p.Product?.Price);
        ViewBag.Carts = carts;
        return View(new Invoice{
            GivenName = User.FindFirstValue(ClaimTypes.GivenName)!,
            Surname = User.FindFirstValue(ClaimTypes.Surname)
        });
    }

    public IActionResult VnPaymentResponse(VnPayment obj){
        // Thêm VnPayment vào database
        context.VnPayments.Add(obj);

        // Lưu thay đổi
        int ret = context.SaveChanges();

        if (ret > 0)
        {
            return Json(new { success = true, message = "Payment saved successfully." });
        }
        else
        {
            return Json(new { success = false, message = "Failed to save payment." });
        }
    }
    
    [HttpPost]
    public IActionResult Checkout(Invoice obj){
        string? memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(memberId)){
            return Redirect("/auth/login");
        }
        Random random = new Random();
        obj.InvoiceId = random.Next(999999, 99999999);
        obj.InvoiceDate = DateTime.Now;
        obj.MemberId = memberId;
        List<Cart> carts = context.Carts.Include(p => p.Product).Where(p => p.MemberId == memberId).ToList();
        
        obj.Amount = carts.Sum(p => p.Quantity * p.Product!.Price) * 1000;
        
        obj.InvoiceDetails = new List<InvoiceDetail>(carts.Count);
        foreach (var item in carts)
        {
            obj.InvoiceDetails.Add(new InvoiceDetail{
                InvoiceId = obj.InvoiceId,
                ProductId = item.ProductId,
                Price = item.Product!.Price,
                Quantity = item.Quantity
            });
        }
        context.Invoices.Add(obj);
        int ret = context.SaveChanges();
        if(ret > 0){
            string url = service.ToUrl(obj, HttpContext);
            System.Console.WriteLine("********************");
            System.Console.WriteLine(url);
            return Redirect(url);
            //return Redirect("/cart/success");
        }
        ViewBag.Departments = context.Departments.ToList();
        ViewBag.Categories = context.Categories.ToList();
        ViewBag.Total = carts.Sum(p => p.Quantity * p.Product?.Price);
        ViewBag.Carts = carts;
        return View(obj);
    }
    
    [HttpPost]
    public IActionResult Add(Cart obj)
    {
        string? memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(memberId))
        {
            return Redirect("/auth/login");
        }

        // Kiểm tra ProductId tồn tại
        var productExists = context.Products.Any(p => p.ProductId == obj.ProductId);
        if (!productExists)
        {
            ModelState.AddModelError("ProductError", "Product not found.");
            return RedirectToAction("Index");
        }

        obj.MemberId = memberId;
        
        var existingCart = context.Carts.FirstOrDefault(p => p.MemberId == obj.MemberId && p.ProductId == obj.ProductId);
        if (existingCart != null)
        {
            existingCart.Quantity += obj.Quantity;
            existingCart.UpdatedDate = DateTime.Now;
            context.Carts.Update(existingCart);
        }
        else
        {
            obj.CreatedDate = DateTime.Now;
            obj.UpdatedDate = DateTime.Now;
            context.Carts.Add(obj);
        }
        context.SaveChanges();
        return Redirect("/cart");
    }

    
    public IActionResult Index(){
        //Miss
        ViewBag.Departments = context.Departments.ToList();
       
        ViewBag.Categories = context.Categories.ToList();

        string? memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(memberId)){
            return Redirect("/auth/login");
        }
        return View(context.Carts.Include(p => p.Product).Where(p => p.MemberId == memberId).ToList());
    }

    [HttpPost]
    public IActionResult UpdateCart(Dictionary<int, Cart> updatedCarts)
    {
        var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(memberId))
        {
            return Redirect("/auth/login");
        }

        foreach (var item in updatedCarts)
        {
            var cart = context.Carts.FirstOrDefault(c => c.CartId == item.Key && c.MemberId == memberId);
            if (cart != null)
            {
                cart.Quantity = item.Value.Quantity;
                cart.UpdatedDate = DateTime.Now;
                context.Carts.Update(cart);
            }
        }

        context.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(memberId))
        {
            return Redirect("/auth/login");
        }

        var cart = context.Carts.FirstOrDefault(p => p.CartId == id && p.MemberId == memberId);
        if (cart != null)
        {
            context.Carts.Remove(cart);
            context.SaveChanges();
        }
        return Redirect("/cart");
    }
}