using System;
using System.ComponentModel.DataAnnotations;

namespace TireShopAccounting.Core.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام مشتری الزامی است")]
        [StringLength(100, ErrorMessage = "نام نباید بیشتر از 100 کاراکتر باشد")]
        public string Name { get; set; }

        [Required(ErrorMessage = "شماره تماس الزامی است")]
        [Phone(ErrorMessage = "شماره تماس معتبر نیست")]
        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        // مانده حساب (مثبت = بدهکار، منفی = بستانکار)
        public decimal Balance { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // نمایش وضعیت مالی
        public string BalanceStatus
        {
            get
            {
                if (Balance > 0)
                    return $"بدهکار: {Balance:N0} تومان";
                else if (Balance < 0)
                    return $"بستانکار: {Math.Abs(Balance):N0} تومان";
                else
                    return "تسویه";
            }
        }

        public bool HasDebt => Balance > 0;
    }
}
