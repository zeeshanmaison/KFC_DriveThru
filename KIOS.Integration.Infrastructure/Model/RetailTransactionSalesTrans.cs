using DriveThru.Integration.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Infrastructure.Model
{
    [Table("RetailTransactionSalesTrans")]
    public class RetailTransactionSalesTrans : BasicEntity
    {
        public string? TransactionId { get; set; }
        public string? Store { get; set; }
        public string?  ItemId { get; set; }
        public string? ItemName { get; set; }
        public decimal Linenum { get; set; }
        public decimal Quantity { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal NetAmountInclTax { get; set; }
        public DateTime TransdDate { get; set; }
        public decimal Price { get; set; }
        public decimal NetPrice { get; set; }
        public string? Comment { get; set; }
    }
}
