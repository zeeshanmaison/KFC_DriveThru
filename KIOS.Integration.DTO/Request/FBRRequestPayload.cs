using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.DTO.Request
{
    public class FBRRequestPayload
    {
        public string InvoiceNumber { get; set; }
        public string POSID { get; set; }
        public string USIN { get; set; }
        public DateTime DateTime { get; set; }
        public string BuyerNTN { get; set; }
        public string BuyerCNIC { get; set; }
        public string BuyerName { get; set; }
        public string BuyerPhoneNumber { get; set; }
        public decimal TotalBillAmount { get; set; }
        public double TotalQuantity { get; set; }
        public decimal TotalSaleValue { get; set; }
        public decimal TotalTaxCharged { get; set; }
        public double Discount { get; set; }
        public double FurtherTax { get; set; }
        public int PaymentMode { get; set; }
        public string RefUSIN { get; set; }
        public int InvoiceType { get; set; }

        public IList<Items> Items { get; set; }

        public FBRRequestPayload()
        {
            Items = new List<Items>();
        }
    }


    public class Items
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string PCTCode { get; set; }
        public decimal TaxRate { get; set; }
        public decimal SaleValue { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxCharged { get; set; }
        public double Discount { get; set; }
        public double FurtherTax { get; set; }
        public int InvoiceType { get; set; }
        public string RefUSIN { get; set; }
    }
}
