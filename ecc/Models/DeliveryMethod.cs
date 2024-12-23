using System.ComponentModel.DataAnnotations;

namespace OSP.Models
{
    public class DeliveryMethod
    {
        [Key]
        public int DeliveryMethodId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Charge { get; set; }
    }
}
