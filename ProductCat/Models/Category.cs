using System.ComponentModel.DataAnnotations;

namespace ProductCat.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }
        public virtual ICollection<Product> Products { get; set; }

    }
}
