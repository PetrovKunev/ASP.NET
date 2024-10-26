using DeskMarket.Data;
using DeskMarket.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DeskMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace DeskMarket.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Where(p => p.IsDeleted == false)
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    IsSeller = User.Identity.IsAuthenticated && p.SellerId == User.FindFirstValue(ClaimTypes.NameIdentifier),
                    HasBought = _context.ProductsClients
                    .Any(pc => pc.ClientId == User.FindFirstValue(ClaimTypes.NameIdentifier) && pc.ProductId == p.Id),

                })
                .AsNoTracking()
                .ToListAsync();

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var categories = await _context.Categories.ToListAsync();
            if (categories == null || !categories.Any())
            {
                ModelState.AddModelError(string.Empty, "No categories found. Please add categories first.");
                return RedirectToAction("Index", "Category"); // или друго действие
            }

            var model = new AddProductViewModel
            {
                Categories = categories // Зареждане на категориите
            };
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage); // Тук виждаш всички грешки при валидация
                }

                model.Categories = await _context.Categories.ToListAsync();
                return View(model);
            }

            // Създаване на продукта
            var product = new Product
            {
                ProductName = model.ProductName,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                AddedOn = model.AddedOn,
                CategoryId = model.CategoryId,
                SellerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartProducts = await _context.ProductsClients
                .Where(pc => pc.ClientId == userId)
                .Include(pc => pc.Product)
                .Select(pc => new CartProductViewModel
                {
                    Id = pc.Product.Id,
                    ProductName = pc.Product.ProductName,
                    Price = pc.Product.Price,
                    ImageUrl = pc.Product.ImageUrl
                })
                .ToListAsync();

            return View(cartProducts);
        }

    }
}
