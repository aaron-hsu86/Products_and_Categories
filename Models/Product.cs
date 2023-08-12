#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace _6_Products_and_Categories.Models;
public class Product
{
    [Key]
    public int ProductId {get;set;}

    [Required]
    [UniqueProduct]
    [MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
    [MaxLength(46, ErrorMessage = "Name cannot be longer than 45 chars")]
    public string Name {get;set;}

    [Required]
    public string Description {get;set;}

    [Required]
    [Range(0, Int32.MaxValue, ErrorMessage ="Price cannot be negative")]
    public double Price {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    public List<Association> Categories {get;set;} = new List<Association>();
}
public class UniqueProductAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Though we have Required as a validation, sometimes we make it here anyways
        // In which case we must first verify the value is not null before we proceed
        if (value == null)
        {
            // If it was, return the required error
            return new ValidationResult("Product Name is required!");
        }

        // This will connect us to our database since we are not in our Controller
        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));
        // Check to see if there are any records of this email in our database
        if (_context.Products.Any(e => e.Name == value.ToString()))
        {
            // If yes, throw an error
            return new ValidationResult("Product Name must be unique!");
        }
        else
        {
            // If no, proceed
            return ValidationResult.Success;
        }
    }
}