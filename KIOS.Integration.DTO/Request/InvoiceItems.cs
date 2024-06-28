using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.DTO.Request
{
    public class InvoiceItems
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal SaleValue { get; set; }
        public decimal TaxCharged { get; set; }
        public decimal TaxRate { get; set; }
        public string PCTCode { get; set; }
        public decimal FurtherTax { get; set; }
        public int InvoiceType { get; set; }
    }
}
