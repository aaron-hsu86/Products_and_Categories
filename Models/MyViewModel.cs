#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations.Schema;
namespace _6_Products_and_Categories.Models;

[NotMapped]
public class MyViewModel
{
    public Product Product {get;set;}
    public List<Product> AllProducts {get;set;}
    public List<Product> AssociatedProducts {get;set;}
    public List<Product> UnassociatedProducts {get;set;}
    
    public Category Category {get;set;}
    public List<Category> AllCategories {get;set;}
    public List<Category> AssociatedCategories {get;set;}
    public List<Category> UnassociatedCategories {get;set;}
}