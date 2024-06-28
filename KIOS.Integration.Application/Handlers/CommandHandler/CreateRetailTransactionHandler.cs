using MediatR;
using DriveThru.Integration.Application.Commands;
using DriveThru.Integration.Infrastructure.Database;
using DriveThru.Integration.Infrastructure.Model;
using DriveThru.Integration.Core.Helpers;
using DriveThru.Integration.DTO.Response;
using DriveThru.Integration.DTO.Request;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace DriveThru.Integration.Application.Handlers.CommandHandler
{
    public class CreateRetailTransactionHandler : IRequestHandler<CreateRetailTransactionCommand, RetailTransaction>
    {
        private readonly AppDbContext _appDbContext;
        private string _connectionString_KFC;

        public CreateRetailTransactionHandler (IConfiguration configuration, AppDbContext appDbContext) 
        {
            _appDbContext = appDbContext;
            _connectionString_KFC = configuration.GetConnectionString("RSSUConnection");
        }

        public async Task<RetailTransaction> Handle(CreateRetailTransactionCommand request, CancellationToken cancellationToken)
        {

            string transactionId = string.Empty;
            string json = string.Empty;
            string itemName = string.Empty;
            
            RetailTransaction retailTransaction = new RetailTransaction
            {
                Json = JsonHelper.Serialize(request),
                TransactionId = "DT-"+ request.Store + "-" + StringHelper.GenerateRandomCode(6),
                DataAreaId = "kfc",
                Currency = "PKR",
                GrossAmount = request.GrossAmount,
                NetAmount = request.NetAmount,
                NetPrice = request.NetPrice,
                TansDate = DateTime.Now,
                PaymentMode = Convert.ToInt32(request.Payment_method),
                Store = request.Store,
                TenderTypeId = request.TenderTypeId,
                AmountCur = Convert.ToDecimal(request.AmountCur),
                ThirdPartyOrderId = request.ThirdPartyOrderId,
                IsActive = true, 
                IsDeleted = false 
            };
            transactionId = retailTransaction.TransactionId;
            await _appDbContext.RetailTransactions.AddAsync(retailTransaction);
            await _appDbContext.SaveChangesAsync();


            foreach (var item in request.salesLines)
            {
                itemName = ItemName(item.ItemId);
                RetailTransactionSalesTrans retailTransactionSalesTrans = new RetailTransactionSalesTrans
                {
                    TransactionId = transactionId,
                    ItemId = item.ItemId,
                    ItemName = itemName,
                    Linenum = item.LineNum,
                    Quantity = item.Qty,
                    TaxAmount = item.TaxAmount,
                    NetAmount = item.NETAMOUNT,
                    NetAmountInclTax = item.NETAMOUNTINCLTAX,
                    TransdDate = DateTime.Now,
                    Store = request.Store,
                    Price = item.Price,
                    NetPrice = item.NETAMOUNT,
                    Comment = item.LineComment,
                    CreatedOn = DateTime.Now

                };
                await _appDbContext.RetailTransactionSalesTrans.AddAsync(retailTransactionSalesTrans);
                await _appDbContext.SaveChangesAsync();
            }

            return retailTransaction;
        }

        private string ItemName(string itemId)
        {
            string itemName = string.Empty;
            string dataAreaId = "kfc";

            string qry = "select ax.ECORESPRODUCTTRANSLATION.DESCRIPTION from ax.INVENTTABLE " +
                        "Join ax.ECORESPRODUCTTRANSLATION On ax.ECORESPRODUCTTRANSLATION.PRODUCT = ax.INVENTTABLE.PRODUCT Where ITEMID ='" + itemId + "' And DATAAREAID = '" + dataAreaId + "' And LANGUAGEID='en-us'";

            DataSet dataSet = SqlHelper.ExecuteDataSet(_connectionString_KFC, qry, CommandType.Text);

            if (dataSet.Tables != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                DataTable dataTable = dataSet.Tables[0];
                if (dataTable.Rows.Count > 0)
                {
                    //Hard Code
                    itemName = dataTable.Rows[0]["Description"].ToString();
                }
            }

            return itemName;
        }
    }
}
