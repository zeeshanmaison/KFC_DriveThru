using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.DTO.Request
{
    public class SalesLine
    {
        public string TRANSACTIONID { get; set; }
        public string ItemId { get; set; }
        public decimal Price { get; set; }
        public decimal NETAMOUNT { get; set; }
        public decimal NETAMOUNTINCLTAX { get; set; }
        public int Qty { get; set; }
        public decimal LineNum { get; set; }

        public string ORIGINALTAXGROUP { get; set; }
        public string ORIGINALTAXITEMGROUP { get; set; }
        public string TaxGroup { get; set; }
        public string TAXITEMGROUP { get; set; }
        public string InventlocationID { get; set; }
        public decimal? OriginalPrice { get; set; }

        // at our end
        public string StaffID { get; set; }
        public int? DATAAREAID { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TAXRATEPERCENT { get; set; }
        public decimal? LineTaxAmount { get; set; }
        public decimal? TAXEXEMPTPRICEINCLUSIVEORIGINALPRICE { get; set; }
        public DateTime? SHIPPINGDATEREQUESTED { get; set; }
        public string Unit { get; set; }
        public string VARIANTID { get; set; }
        public string BATCHTERMINALID { get; set; }
        public string CREATEDONPOSTERMINAL { get; set; }
        public DateTime? TIMEWHENTOTALPRESSED { get; set; }
        public DateTime? TIMEWHENTRANSCLOSED { get; set; }
        public string RECEIPTDATEREQUESTED { get; set; }
        public string SUSPENDEDTRANSACTIONID { get; set; }
        public string ExchRatemst { get; set; }
        public int? ISPAYMENTCAPTURED { get; set; }
        public decimal? REFUNDABLEAMOUNT { get; set; }
        public decimal ItemPriceExcTax { get; set; }
        public string LineComment { get; set; }



    }
}
