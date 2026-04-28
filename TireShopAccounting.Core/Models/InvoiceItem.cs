using System.ComponentModel.DataAnnotations;

namespace TireShopAccounting.Core.Models
{
    public class InvoiceItem
    {
        public int Id { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "تعداد باید حداقل 1 باشد")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public decimal TotalPrice => Quantity * Price;

        // Navigation Properties
        public Product Product { get; set; }
        public Invoice Invoice { get; set; }
    }
}
