using MediatR;
using DriveThru.Integration.Infrastructure.Model;
using DriveThru.Integration.DTO.Request;
using DriveThru.Integration.Core.Enums;

namespace DriveThru.Integration.Application.Commands
{

    public class CreateRetailTransactionCommand : IRequest<RetailTransaction>
    {
        public string Id { get; set; }
        public string Company { get; set; }
        public string Terminal { get; set; }
        public long? Channel { get; set; }
        public string Currency { get; set; }
        public decimal POSFee { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal NetPrice { get; set; }
        public string ReciptId { get; set; }
        public DateTime? TransDate { get; set; }
        public int? TransTime { get; set; }
        public DateTime? BusinessDate { get; set; }
        public PaymentMethod Payment_method { get; set; }

        // at our end
        public string BatchId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string InventLocationId { get; set; }
        public decimal? NumberOfItemLines { get; set; }
        public decimal? NumberOfItems { get; set; }
        public decimal TotalTaxCharged { get; set; }
        public int? NumberOfPaymentLines { get; set; }
        public string Staff { get; set; }
        public string Store { get; set; }
        public string TransactionId { get; set; }
        public string HdsOrderId { get; set; }
        public int? Type { get; set; }
        public decimal? ExchRate { get; set; }
        public string DataAreaId { get; set; }
        public string Description { get; set; }
        public string BatchTerminalId { get; set; }
        public string CraeteDonPOSTerminal { get; set; }
        public DateTime? TimeWhenTotalPressed { get; set; }
        public DateTime? TimeWhenClosed { get; set; }
        public string ReceiptDataRequested { get; set; }
        public string SuspendedTransactionID { get; set; }
        public string TenderTypeId { get; set; }
        public decimal? LineNum { get; set; }
        public string LineQty { get; set; }
        public decimal? AmountCur { get; set; }
        public string AmountMst { get; set; }
        public string AmountTendered { get; set; }
        public decimal? ExchRateMST { get; set; }
        public int? IsPaymentCaptured { get; set; }
        public decimal? RefundableAmount { get; set; }
        public string ThirdPartyOrderId { get; set; }
        public string Source { get; set; }
        public string PickupMode { get; set; }
        public int? PaymentMode { get; set; }
        public string BusinessDateCustom { get; set; }
        public string TableNum { get; set; }
        public string Comment { get; set; }

        public IList<SalesLine> salesLines { get; set; }

        public CreateRetailTransactionCommand()
        {
            salesLines = new List<SalesLine>();
        }
    }
}
