using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.DTO.Request
{
    public class Invoice
    {
        public string InvoiceNumber { get; set; }
        public string POSID { get; set; }
        public string USIN { get; set; }
        public string BuyerNTN { get; set; }
        public string BuyerCNIC { get; set; }
        public string BuyerName { get; set; }
        public string BuyerPhoneNumber { get; set; }
        public DateTime DateTime { get; set; }
        public int PaymentMode { get; set; }
        public decimal TotalSaleValue { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalBillAmount { get; set; }
        public decimal TotalTaxCharged { get; set; }
        public decimal Discount { get; set; }
        public decimal FurtherTax { get; set; }
        public int InvoiceType { get; set; }

        public List<InvoiceItems> items = new List<InvoiceItems>();

    }
}
