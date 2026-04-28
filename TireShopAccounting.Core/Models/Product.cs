using System;
using System.ComponentModel.DataAnnotations;

namespace TireShopAccounting.Core.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام کالا الزامی است")]
        [StringLength(100, ErrorMessage = "نام کالا نباید بیشتر از 100 کاراکتر باشد")]
        public string Name { get; set; }

        [Required(ErrorMessage = "برند الزامی است")]
        [StringLength(50)]
        public string Brand { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "موجودی نمی‌تواند منفی باشد")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "قیمت خرید نمی‌تواند منفی باشد")]
        public decimal BuyPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "قیمت فروش نمی‌تواند منفی باشد")]
        public decimal SellPrice { get; set; }

        // محاسبه سود واحد
        public decimal ProfitPerUnit => SellPrice - BuyPrice;

        // نمایش کامل اطلاعات کالا
        public string DisplayName => $"{Name} - {Brand}";

        // بررسی موجودی کم
        public bool IsLowStock => Quantity < 5;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
    }
}
