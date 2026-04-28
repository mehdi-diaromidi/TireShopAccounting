using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TireShopAccounting.Core.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Discount { get; set; }

        public decimal FinalAmount => TotalAmount - Discount;

        // Navigation Properties
        public Customer Customer { get; set; }
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

        // شماره فاکتور به صورت قابل خواندن
        public string InvoiceNumber => $"F-{Id:D6}";

        // تاریخ شمسی
        public string PersianDate
        {
            get
            {
                var pc = new System.Globalization.PersianCalendar();
                return $"{pc.GetYear(Date):D4}/{pc.GetMonth(Date):D2}/{pc.GetDayOfMonth(Date):D2}";
            }
        }
    }
}
