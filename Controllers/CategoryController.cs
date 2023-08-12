using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _6_Products_and_Categories.Models;
using Microsoft.EntityFrameworkCore;

namespace _6_Products_and_Categories.Controllers;

public class CategoryController : Controller
{
    private readonly ILogger<CategoryController> _logger;
    public MyContext db;

    public CategoryController(ILogger<CategoryController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }

    [HttpGet("categories")]
    public IActionResult Index()
    {
        MyViewModel MyCategories = new MyViewModel()
        {
            AllCategories = db.Categories.ToList()
        };
        return View(MyCategories);
    }

    [HttpGet("categories/{categoryId}")]
    public IActionResult ViewOne(int categoryId)
    {
        var categoryQuery = db.Categories.Where(c=> c.CategoryId == categoryId).Include(c => c.Products).ThenInclude(a => a.Product);

        List<Product> associatedProductsQuery = categoryQuery.SelectMany(c => c.Products).Select(a => a.Product).ToList();
        List<Product> allProducts = db.Products.ToList();
        List<Product> unassociatedProducts = allProducts.Except(associatedProductsQuery).ToList();

        MyViewModel oneCategory = categoryQuery.Select(c => new MyViewModel
        {
            Category = c,
            AssociatedProducts = associatedProductsQuery,
            UnassociatedProducts = unassociatedProducts
        }).FirstOrDefault();

        return View(oneCategory);
    }

    [HttpPost("category/create")]
    public IActionResult Create(Category newCategory)
    {
        if(!ModelState.IsValid)
        {
            MyViewModel MyCategories = new MyViewModel()
            {
                AllCategories = db.Categories.ToList()
            };
            return View("Index", MyCategories);
        }
        db.Categories.Add(newCategory);
        db.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpPost("category/product/add")]
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
        return RedirectToAction("ViewOne", new {categoryId = newAssociation.CategoryId});
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
