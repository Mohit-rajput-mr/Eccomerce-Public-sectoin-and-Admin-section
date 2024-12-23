using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OSP.Models {
    public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }

    // Existing relationship with Category
    public int? CategoryId { get; set; }
    public virtual Category Category { get; set; }

    // Temporary field for entering a new category
    public string NewCategory { get; set; }
}

}
