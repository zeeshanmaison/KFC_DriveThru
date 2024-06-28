using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Application.Queries
{
    public class RetailTransactionResponse
    {
        public string TransactionId { get; set; }
        public string DataAreaId { get; set; }
        public string Currency { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal NetPrice { get; set; }
        public DateTime TansDate { get; set; }
        public int PaymentMode { get; set; }
        public string Store { get; set; }
        public string TenderTypeId { get; set; }
        public decimal AmountCur { get; set; }
        public string ThirdPartyOrderId { get; set; }
        public string Json { get; set; }
        public IList<RetailTransactionSalesTrans> RetailTransactionSalesTrans { get; set; }

        public RetailTransactionResponse()
        { 
            RetailTransactionSalesTrans = new List<RetailTransactionSalesTrans>();
        }
    }
}
