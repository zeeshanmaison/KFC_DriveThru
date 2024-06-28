using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Application.Queries
{
    public class RetailTransactionSalesTrans
    {
        public string TransactionId { get; set; }
        public string Store { get; set; }
        public string ItemId { get; set; }
        public decimal Linenum { get; set; }
        public decimal Quantity { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal NetAmountInclTax { get; set; }
        public DateTime TransdDate { get; set; }
        public decimal Price { get; set; }
        public decimal NetPrice { get; set; }
    }
}
