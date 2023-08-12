using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _6_Products_and_Categories.Models;
using Microsoft.EntityFrameworkCore;

namespace _6_Products_and_Categories.Controllers;

public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;
    public MyContext db;

    public ProductController(ILogger<ProductController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }

    [HttpGet("products")]
    public IActionResult Index()
    {
        MyViewModel MyProducts = new MyViewModel
        {
            AllProducts = db.Products.ToList()
        };
        return View(MyProducts);
    }

    [HttpGet("product/{productId}")]
    public IActionResult ViewOne(int productId)
    {
        var productQuery = db.Products.Where(p=> p.ProductId == productId).Include(p => p.Categories).ThenInclude(a => a.Category);

        List<Category> associatedCategoriesQuery = productQuery.SelectMany(p => p.Categories).Select(a => a.Category).ToList();
        List<Category> allCategories = db.Categories.ToList();
        List<Category> unassociatedCategories = allCategories.Except(associatedCategoriesQuery).ToList();

        MyViewModel oneProduct = productQuery.Select(p => new MyViewModel
        {
            Product = p,
            AssociatedCategories = associatedCategoriesQuery,
            UnassociatedCategories = unassociatedCategories
        }).FirstOrDefault();

        return View(oneProduct);
    }

    [HttpPost("product/create")]
    public IActionResult Create(Product newProduct)
    {
        if(!ModelState.IsValid)
        {
            MyViewModel MyProducts = new MyViewModel
            {
                AllProducts = db.Products.ToList()
            };
            return View("Index", MyProducts);
        }
        db.Products.Add(newProduct);
        db.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpPost("product/category/add")]
    public IActionResult ChangeAssociation(Association newAssociation)
    {
        Association? existingAssociation = db.Associations.FirstOrDefault(a => a.ProductId == newAssociation.ProductId && a.CategoryId == newAssociation.CategoryId);

        if(existingAssociation == null)
        {
            db.Associations.Add(newAssociation);
        }
        else
        {
            db.Associations.Remove(existingAssociation);
        }

        db.SaveChanges();
        return RedirectToAction("ViewOne", new {productId = newAssociation.ProductId});
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
