
using DriveThru.Integration.Application.Commands;
using DriveThru.Integration.Application.Services.Abstraction;
using DriveThru.Integration.Core.Enums;
using DriveThru.Integration.Core.Helpers;
using DriveThru.Integration.Core.Response;
using DriveThru.Integration.DTO.Request;
using DriveThru.Integration.DTO.Response;
using DriveThru.Integration.Infrastructure.Model;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Globalization;
using System.Net;
using System.Security.Policy;
using System.Text;


namespace DriveThru.Integration.Application.Services
{
    public class CreateOrderService : ICreateOrderService
    {
        private string _connectionString_KFC;
        private InlineQueryResponse lastRecordResponse;
        private readonly IConfiguration _configuration;
        private string _terminalId;
        private string _receiptId;
        private string _suspendedId;
        private string _transactionId;
        private string _fbRInvoiceNo;
        private long _channle = 0;
        private long _batchId = 0;
        private decimal? _totalBillAmount;
        private DataTable dataTable;
        private decimal _taxPrice;
        private decimal _orignalprice;
        private string inventLocationId = string.Empty;
        private decimal _grossAmountCustom;
        private string _staffId = string.Empty;
        private bool _isFBRFail = false;
        private string _isTaxImplemented = string.Empty;
        private string _itemName = string.Empty;
        private string _fbrInvoiceNumber = string.Empty;
        private string _srbInvoiceNumber = string.Empty;
        private decimal _taxValue;
        private readonly ISender _mediator;

        public CreateOrderService(IConfiguration configuration, ISender mediator)
        {
            _configuration = configuration;
            _connectionString_KFC = configuration.GetConnectionString("RSSUConnection");
            _terminalId = _configuration.GetSection("Keys:TerminalId").Value;
            _terminalId = _configuration.GetSection("Keys:TerminalId").Value;
            _isTaxImplemented = _configuration.GetSection("Keys:TaxApplied").Value;
            _mediator = mediator;
        }

        public async Task<ResponseModelWithClass<CreateOrderResponse>> CreateOrderKFC(DriveThru.Integration.Application.Commands.CreateRetailTransactionCommand request)
        {
            ResponseModelWithClass<CreateOrderResponse> response = new ResponseModelWithClass<CreateOrderResponse>();
            //LastRecordResponseFromRetailTransTableResponse lastRecordResponse = new LastRecordResponseFromRetailTransTableResponse();
            response.HttpStatusCode = (int)HttpStatusCode.Accepted;
            response.MessageType = (int)MessageType.Info;
            CreateOrderResponse responseModel = new CreateOrderResponse();
            Task<FBRResponse> fBRResponse = null;
            InlineQueryResponse inlineQueryResponseTax = new InlineQueryResponse();
            int affectedRows = 0;
            string batchId = string.Empty;
            DateTime createdDateTime = DateTime.Now;
            DateTime now = DateTime.Now;
            string date_now = now.Year + "-" + now.Month + "-" + now.Day;
            double seconds = TimeSpan.Parse(now.ToString("HH:mm:ss")).TotalSeconds;
            int affectedRowinsertInMCSDONUMBERTABLE = 0;
            int affectedRowCreateRETAILTRANSACTIONPAYMENTTRANS = 0;
            int affectedRowInsertCrtSALESTRANSACTION = 0;
            int affectedRowInsertRetailTransactionTaxTrans = 0;
            int affectedRowMarkupTrans = 0;
            string recieptId = string.Empty;
            string transactionId = " ";
            string suspendedTransactionId = " ";
            bool isDeleted = false;
            int? numberOfItemLines = 0;
            int? numberOfItems = 0;
            decimal? paymentAmount;
            int type = 0;
            int noOfPaymentLine = 0;
            decimal testValue;
            long? miliseconds;

            //fBRResponse = FBR_TestURL(request);

            // InsertCrtSALESTRANSACTION(request);
            //FBR(request);

            string storeProcedureName = "usp_create_axRetailTransactionTable";

            try
            {

                if (request.Payment_method == PaymentMethod.Cash)
                {

                    RetailTransaction retailTransaction = await _mediator.Send(request);
                    responseModel.RecipteId = retailTransaction.TransactionId;

                    response.Result = responseModel;
                    response.HttpStatusCode = (int)HttpStatusCode.OK;
                    response.MessageType = (int)MessageType.Success;
                    response.Message = "DriveThru Order created successfully.";

                    return response;

                }

                else if (request.Payment_method == PaymentMethod.CreditCard)
                {
                    if (request != null && request.salesLines.Count > 0)
                    {
                        double totalseconds = TimeSpan.Parse(now.ToString("HH:mm:ss")).TotalSeconds;
                        int transTime = Convert.ToInt32(totalseconds);


                        if (request != null && request.Store == null || request.Store == "string" || request.Store == string.Empty)
                        {
                            response.Result = responseModel = null;
                            response.HttpStatusCode = (int)HttpStatusCode.BadRequest;
                            response.MessageType = (int)MessageType.Warning;
                            response.Message = " store id not found. ";

                            return response;
                        }
                        if (_isTaxImplemented == "3" && request.POSFee == 0)
                        {
                            response.Result = responseModel = null;
                            response.HttpStatusCode = (int)HttpStatusCode.BadRequest;
                            response.MessageType = (int)MessageType.Warning;
                            response.Message = " POS Fee can not be 0 ";
                            return response;
                        }

                        recieptId = IncrementedId(request.Store, false, false);

                        _receiptId = recieptId;

                        transactionId = IncrementedId(request.Store, false, true);

                        _transactionId = transactionId;

                        if (request.Payment_method == PaymentMethod.Cash)
                        {
                            suspendedTransactionId = IncrementedId(request.Store, true, false);

                            _suspendedId = suspendedTransactionId;

                            type = 36;
                            noOfPaymentLine = 0;

                        }

                        if (request.Payment_method == PaymentMethod.CreditCard)
                        {
                            //request.PaymentAmount = paymentAmount;

                            type = 2;

                            if (request.salesLines.Count <= 1)
                            {
                                noOfPaymentLine = 1;
                            }
                            else
                            {
                                noOfPaymentLine = 2;
                            }
                        }

                        foreach (var noOfItemLines in request.salesLines)
                        {
                            numberOfItemLines++;
                            numberOfItems = numberOfItems + Convert.ToInt32(noOfItemLines.Qty++);
                        }

                        if (numberOfItemLines <= 0 && numberOfItems <= 0)
                        {
                            response.Result = responseModel = null;
                            response.HttpStatusCode = (int)HttpStatusCode.BadRequest;
                            response.MessageType = (int)MessageType.Warning;
                            response.Message = " numberOfItemLines and numberOfItems cannot be null or 0. ";

                            return response;
                        }

                        _staffId = StaffId(_terminalId);
                        batchId = GetBatchId(_terminalId, request.Store);
                        _batchId = (long)Convert.ToDecimal(batchId);
                        inventLocationId = GetInventLocation(request.Store);
                        lastRecordResponse = getFirstRecFromRetailTransactionTable(request.Company);
                        _channle = lastRecordResponse.Channel;
                        inlineQueryResponseTax = getTaxGroupandBusinessDate(request.Store, request.Payment_method);

                        _grossAmountCustom = CalculatePriceWithTax(request.NetAmount, inlineQueryResponseTax.TAXVALUE);


                        string description = " ThirdPartyOrder: " + request.ThirdPartyOrderId + " DriveThru " + "; Source: " + request.Source + ";";
                        InlineQueryResponse inlineQueryResponseTaxGroupandBusinessDate = getTaxGroupandBusinessDate(request.Store, request.Payment_method);


                        if (inlineQueryResponseTaxGroupandBusinessDate != null)
                        {
                            request.BusinessDateCustom = inlineQueryResponseTaxGroupandBusinessDate.BusinessDate;
                        }
                        StringBuilder query = new StringBuilder();

                        //query.Append("SELECT EMP_NAME, EMP_NAME FROM hr_sm_emply WHERE emp_code = @Code");
                        query.Append(storeProcedureName);

                        Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "TERMINAL", _terminalId },
                    { "BATCHID", (long)Convert.ToDecimal(batchId) },
                    { "CHANNEL", lastRecordResponse.Channel },
                    { "CURRENCY", request.Currency },
                    { "GROSSAMOUNT", Math.Round(request.GrossAmount, 2) * -1 },
                    { "PaymentAmount", request.GrossAmount },
                    //{ "PaymentAmount", 0.00 },
                    { "INVENTLOCATIONID", request.Store },
                    { "NETAMOUNT", Math.Round(request.NetAmount, 2) * -1 },
                    { "NETPRICE", Math.Round(request.NetPrice, 2) * -1 },
                    { "NUMBEROFITEMLINES", numberOfItemLines },
                    { "NUMBEROFITEMS", numberOfItems },
                    { "NUMBEROFPAYMENTLINES", noOfPaymentLine },
                    { "RECEIPTID", _receiptId },
                    { "SUSPENDEDTRANSACTIONID", suspendedTransactionId },
                    { "STAFF", _staffId },
                    { "STORE", request.Store },
                    { "TRANSACTIONID", _transactionId },
                    { "TRANSDATE", request.TransDate },
                    //{ "TRANSTIME", request.TransTime },
                    { "TRANSTIME", transTime},
                    { "TYPE", type },
                    { "EXCHRATE", 0 },
                    { "DATAAREAID", request.Company },
                    { "DESCRIPTION", description },
                    { "BATCHTERMINALID", _terminalId },
                    { "BusinessDate", request.BusinessDateCustom },
                    //{ "BusinessDate", request.BusinessDate },
                    { "CREATEDONPOSTERMINAL", _terminalId },
                    { "TIMEWHENTOTALPRESSED", "" + seconds },
                    { "TIMEWHENTRANSCLOSED", "" + seconds },
                    { "RECEIPTDATEREQUESTED", date_now },
                    { "HDSOrderID", request.HdsOrderId },
                    { "Payment_method", request.Payment_method},
                    { "ThirdPartyOrderId", request.ThirdPartyOrderId },
                    { "PickUpMode", request.PickupMode },
                    { "TableNum", request.TableNum },
                    { "Comment", request.Comment },
                    { "CreatedDateTime", createdDateTime.AddHours(-5) }
                };

                        // Fill Request Model
                        request.Terminal = _terminalId;
                        request.Channel = _channle;
                        request.NumberOfItemLines = numberOfItemLines;
                        request.NumberOfItems = numberOfItems;
                        request.NumberOfPaymentLines = noOfPaymentLine;
                        request.ReciptId = _receiptId;
                        request.SuspendedTransactionID = _suspendedId;
                        request.TransactionId = _transactionId;
                        request.Type = type;
                        request.BatchTerminalId = _terminalId;
                        request.CraeteDonPOSTerminal = _terminalId;


                        DataTable responseDataTable = new DataTable();

                        //string parsedQuery = TemplateHelper.ParseQueryTemplate(query.ToString(), parameters);
                        try
                        {
                            affectedRows = SqlHelper.ExecuteNonQuery(_connectionString_KFC, query.ToString(), CommandType.StoredProcedure, parameters);

                            if (affectedRows > 0)
                            {
                                ResponseModelWithClass<DataTable> dataTableLines = CreateretailTransSalesTransLines(request);

                                if (dataTableLines != null && dataTableLines.HttpStatusCode == (int)HttpStatusCode.OK)
                                {
                                    if (request.Payment_method == PaymentMethod.CreditCard)
                                    {
                                        affectedRowCreateRETAILTRANSACTIONPAYMENTTRANS = CreateRETAILTRANSACTIONPAYMENTTRANS(request);

                                        if (affectedRowCreateRETAILTRANSACTIONPAYMENTTRANS > 0)
                                        {
                                            affectedRowInsertRetailTransactionTaxTrans = CreateRetailTransactionTaxTrans(request);
                                        }
                                        else
                                        {
                                            //isDeleted = DeleteCurrentRecord(_receiptId);
                                        }
                                    }

                                    affectedRowInsertCrtSALESTRANSACTION = InsertCrtSALESTRANSACTION(request);

                                    if (affectedRowInsertCrtSALESTRANSACTION <= 0)
                                    {

                                        isDeleted = DeleteCurrentRecord(_receiptId);

                                        InsertSimplexRequestLog(request, "No record created in crt.RetailTransactionView" +" | "+ request.ThirdPartyOrderId);

                                        response.Result = responseModel = null;
                                        response.Message = "No record created no data inserted in crt.RetailTransactionView";
                                        response.HttpStatusCode = (int)HttpStatusCode.InternalServerError;
                                        response.MessageType = (int)MessageType.Error;
                                        return response;
                                    }

                                    affectedRowinsertInMCSDONUMBERTABLE = insertInMCSDONUMBERTABLE(request);

                                    if (affectedRowinsertInMCSDONUMBERTABLE >= 1)

                                    {
                                        try
                                        {
                                            if (_isTaxImplemented == "1")
                                            {
                                                fBRResponse = FBR(request);
                                                responseModel.FBRInvoiceNo = fBRResponse.Result.InvoiceNumber;

                                                _fbrInvoiceNumber = responseModel.FBRInvoiceNo;

                                                if (_fbrInvoiceNumber != string.Empty)
                                                {
                                                    int affectedRowMZNFBRInvoicing = InsertMZNFBRInvoicing(request, _fbrInvoiceNumber);

                                                    if (affectedRowMZNFBRInvoicing <= 0)
                                                    {
                                                        isDeleted = DeleteCurrentRecord(_receiptId);

                                                        InsertSimplexRequestLog(request, "FBR/SRB Log not created" + " | " + request.ThirdPartyOrderId);

                                                        if (isDeleted)
                                                        {
                                                            response.Result = responseModel = null;
                                                            response.Message = "No record created FBRInvoicing Log Not Created";
                                                            response.HttpStatusCode = (int)HttpStatusCode.InternalServerError;
                                                            response.MessageType = (int)MessageType.Error;
                                                            return response;
                                                        }
                                                    }
                                                }

                                            }
                                            else if (_isTaxImplemented == "2")
                                            {
                                                fBRResponse = FBR_TestURL(request);
                                                responseModel.FBRInvoiceNo = fBRResponse.Result.InvoiceNumber;
                                                _fbrInvoiceNumber = responseModel.FBRInvoiceNo;

                                            }
                                            else if (_isTaxImplemented == "3")
                                            {
                                                _srbInvoiceNumber = SRB(request);
                                                if (_srbInvoiceNumber == "invalid Username or password SRB")
                                                {
                                                    isDeleted = DeleteCurrentRecord(_receiptId);

                                                    InsertSimplexRequestLog(request, "invalid Username or password SRB" + " | " + request.ThirdPartyOrderId);

                                                    if (isDeleted)
                                                    {
                                                        response.Result = responseModel = null;
                                                        response.Message = "invalid Username or password SRB! No record created ";
                                                        response.HttpStatusCode = (int)HttpStatusCode.InternalServerError;
                                                        response.MessageType = (int)MessageType.Error;
                                                        return response;
                                                    }
                                                }
                                                responseModel.FBRInvoiceNo = "SRBInvoiceNo: " + _srbInvoiceNumber;
                                                _fbrInvoiceNumber = responseModel.FBRInvoiceNo;

                                                if (_fbrInvoiceNumber != string.Empty)
                                                {
                                                    int affectedRowMZNFBRInvoicingSRB = InsertMZNFBRInvoicing(request, _fbrInvoiceNumber);

                                                    //insert value in RetailTransactionMarkupTrans in case of SRB only

                                                    affectedRowMarkupTrans = InsertRetaailTransactionMarkupTrans(request);

                                                    if (affectedRowMZNFBRInvoicingSRB <= 0 || affectedRowMarkupTrans <= 0)
                                                    {
                                                        isDeleted = DeleteCurrentRecord(_receiptId);

                                                        InsertSimplexRequestLog(request, "SRB Issue occured" + " | " + request.ThirdPartyOrderId);

                                                        if (isDeleted)
                                                        {
                                                            response.Result = responseModel = null;
                                                            response.Message = "No record created SRBInvoicing Log Not Created";
                                                            response.HttpStatusCode = (int)HttpStatusCode.InternalServerError;
                                                            response.MessageType = (int)MessageType.Error;
                                                            return response;
                                                        }
                                                    }
                                                }
                                            }
                                            else if (_isTaxImplemented == "0")
                                            {
                                                responseModel.FBRInvoiceNo = "No FBR/SBR implemented";
                                            }

                                            InsertSimplexRequestLog(request, "RecordCreated! " + " | " + request.ThirdPartyOrderId);

                                            response.MessageType = (int)MessageType.Success;
                                            response.Message = "Success";
                                            response.HttpStatusCode = (int)HttpStatusCode.OK;
                                            responseModel.RecipteId = _receiptId;
                                            response.Result = responseModel;
                                        }
                                        catch (Exception ex)
                                        {
                                            isDeleted = DeleteCurrentRecord(_receiptId);

                                            if (isDeleted)
                                            {
                                                InsertSimplexRequestLog(request, ex.Message + " | " + request.ThirdPartyOrderId);

                                                response.Result = responseModel = null;
                                                response.Message = "No record created" + ex.Message;
                                                response.HttpStatusCode = (int)HttpStatusCode.InternalServerError;
                                                response.MessageType = (int)MessageType.Error;
                                                return response;
                                            }
                                            else
                                            {
                                                //response.Message = ex.Message;
                                            }

                                            throw;
                                        }
                                    }
                                    else
                                    {
                                        isDeleted = DeleteCurrentRecord(_receiptId);



                                        if (isDeleted)
                                        {
                                            response.Message = " No record created. ";
                                        }
                                        else
                                        {
                                            //response.Message = ex.Message;
                                        }
                                        response.Result = responseModel = null;
                                        response.HttpStatusCode = (int)HttpStatusCode.InternalServerError;
                                        response.MessageType = (int)MessageType.Error;
                                        return response;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //Delete all insertion against recipt id.
                            isDeleted = DeleteCurrentRecord(_receiptId);

                            InsertSimplexRequestLog(request, ex.Message + " | " + request.ThirdPartyOrderId);


                            if (isDeleted)
                            {
                                response.Message = " No record created. " + ex.Message;
                            }
                            else
                            {
                                response.Message = ex.Message;
                            }
                            response.Result = responseModel = null;
                            response.HttpStatusCode = (int)HttpStatusCode.InternalServerError;
                            response.MessageType = (int)MessageType.Error;
                            return response;

                        }
                    }
                    else
                    {
                        response.Result = null;
                        response.HttpStatusCode = (int)HttpStatusCode.BadRequest;
                        response.MessageType = (int)MessageType.Warning;
                    }
                }

                else
                {
                    response.Result = null;
                    response.HttpStatusCode = (int)HttpStatusCode.BadRequest;
                    response.MessageType = (int)MessageType.Error;
                    response.Message = "Payment method not correct";
                }

            }
            catch (Exception ex)
            {
                string catchMsg = string.Empty;

                if (request.Payment_method != PaymentMethod.Cash)
                {
                    int inserted = InsertSimplexRequestLog(request, ex.Message + " | " + request.ThirdPartyOrderId);
                    if (inserted > 0)
                    {
                        catchMsg = ex.Message + " | " + ex.InnerException.Message;
                    }
                    response.Message = ex.Message + " | " + catchMsg + " | Inner Execption: " + ex.InnerException;

                }
                
                response.Message = " | exception messgae: " + ex.Message + " | " + " | Inner Execption: " + ex.InnerException;

                response.Result = null;
                response.HttpStatusCode = (int)HttpStatusCode.InternalServerError;
                response.MessageType = (int)MessageType.Error;
                //throw ex.InnerException;
            }



            return response;
        }

        private ResponseModelWithClass<DataTable> CreateretailTransSalesTransLines(DriveThru.Integration.Application.Commands.CreateRetailTransactionCommand request)
        {
            ResponseModelWithClass<DataTable> response = new ResponseModelWithClass<DataTable>();
            response.HttpStatusCode = (int)HttpStatusCode.Accepted;
            response.MessageType = (int)MessageType.Info;
            int affectedRows = 0;
            string batchId = string.Empty;
            DateTime createdDateTime = DateTime.Now;
            DateTime now = DateTime.Now;
            string date_now = now.Year + "-" + now.Month + "-" + now.Day;
            double seconds = TimeSpan.Parse(now.ToString("HH:mm:ss")).TotalSeconds;
            var format = new NumberFormatInfo();
            format.NegativeSign = "-";
            format.NumberDecimalSeparator = ".";
            string itemName = string.Empty;
            string taxItemGroup = string.Empty;
            int? lineNum = 0;
            InvoiceItems invocieitems = new InvoiceItems();


            if (request != null && request.salesLines.Count > 0)
            {
                foreach (var item in request.salesLines)
                {
                    string itemId = item.ItemId;
                    int qty = item.Qty - 1;

                    string qry = "select ax.ECORESPRODUCTTRANSLATION.DESCRIPTION from ax.INVENTTABLE " +
                        "Join ax.ECORESPRODUCTTRANSLATION On ax.ECORESPRODUCTTRANSLATION.PRODUCT = ax.INVENTTABLE.PRODUCT Where ITEMID ='" + item.ItemId + "' And DATAAREAID = '" + request.Company + "' And LANGUAGEID='en-us'";

                    DataSet dataSet = SqlHelper.ExecuteDataSet(_connectionString_KFC, qry, CommandType.Text);

                    if (dataSet.Tables != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
                    {
                        DataTable dataTable = dataSet.Tables[0];
                        if (dataTable.Rows.Count > 0)
                        {
                            //Hard Code
                            itemName = dataTable.Rows[0]["Description"].ToString();
                            _itemName = itemName;
                            //itemName = "Xinger";
                        }
                    }

                    _totalBillAmount = _totalBillAmount + item.Price;

                    taxItemGroup = TaxItemGroup(request.Company, item.ItemId);
                    InlineQueryResponse inlineQueryResponseLine = new InlineQueryResponse();
                    InlineQueryResponse inlineQueryResponseLineTaxGroupandBusinessDate = new InlineQueryResponse();

                    inlineQueryResponseLine = getVariantIdandUnitId(request.Company, item.ItemId);

                    inlineQueryResponseLineTaxGroupandBusinessDate = getTaxGroupandBusinessDate(request.Store, request.Payment_method);

                    //_orignalprice = CalculatePriceExcuTax(item.Price, inlineQueryResponseLineTaxGroupandBusinessDate.TAXVALUE);
                    _taxPrice = CalculateTaxedPrice(item.Price, inlineQueryResponseLineTaxGroupandBusinessDate.TAXVALUE);
                    _taxPrice = _taxPrice * -1;

                    double totalseconds = TimeSpan.Parse(now.ToString("HH:mm:ss")).TotalSeconds;
                    int transTime = Convert.ToInt32(totalseconds);

                    /*
                     *TODO:
                    invocieitems.ItemCode = item.ItemId;
                    invocieitems.ItemName = itemName;
                    invocieitems.PCTCode = "4712";
                    invocieitems.Quantity = (decimal)Math.Round(Convert.ToDecimal(request.Qty)) * -1;
                    invocieitems.TaxRate = (decimal)Math.Round(decimal.Parse(request.t.TAXRATEPERCENT), 2);
                    invocieitems.SaleValue = (decimal)Math.Round(NetAmount, 2) * -1;
                    invocieitems.TaxCharged = (decimal)Math.Round(decimal.Parse(retailTransSalesTrans.LineTaxAmount), 2) * -1;
                    invocieitems.TotalAmount = (decimal)Math.Round(decimal.Parse(retailTransSalesTrans.OriginalPrice), 2);
                    invocieitems.InvoiceType = 1;

                    invoiceitemList.Add(invocieitems);
                    */

                    String lineQuery = "usp_CreateretailTransSalesTransLines";
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "TERMINALID", _terminalId },
                        { "CHANNEL", lastRecordResponse.Channel },
                        { "RECEIPTID", _receiptId },
                        { "STORE", request.Store },
                        { "TRANSACTIONID", _transactionId },
                        { "ITEMID", item.ItemId },
                        //{ "LINENUM", Convert.Todecimal(lineNum++) },
                        { "LINENUM", item.LineNum },
                        { "QTY", qty * -1 },
                        { "PRICE", Math.Round(item.Price,2) },
                        { "NETAMOUNT",  Math.Round(item.NETAMOUNT,2) * -1 },
                        { "NETAMOUNTINCLTAX", item.NETAMOUNTINCLTAX * -1 },
                        { "NETPRICE", Math.Round(item.NETAMOUNT,2) * -1 },
                        { "TaxGroup", inlineQueryResponseLineTaxGroupandBusinessDate.TAXGROUP},
                        { "TAXITEMGROUP", "ALL" },
                        { "InventlocationID", request.Store },
                        { "OriginalPrice", request.NetAmount },
                        { "ORIGINALTAXGROUP", inlineQueryResponseLineTaxGroupandBusinessDate.TAXGROUP },
                        { "ORIGINALTAXITEMGROUP", "ALL" },
                        { "StaffID", _staffId },
                        { "TRANSDATE", request.TransDate },
                        //{ "TRANSTIME", request.TransTime },
                        { "TRANSTIME", transTime },
                        { "DATAAREAID", request.Company },
                        { "Currency", request.Currency },
                        { "BusinessDate", request.BusinessDateCustom },
                        { "TaxAmount", Math.Round(item.TaxAmount, 2) * -1 },
                        { "TAXRATEPERCENT", inlineQueryResponseLineTaxGroupandBusinessDate.TAXVALUE },
                        { "TAXEXEMPTPRICEINCLUSIVEORIGINALPRICE", item.Price },
                        { "SHIPPINGDATEREQUESTED", date_now },
                        { "Unit", inlineQueryResponseLine.UnitId },
                        { "VARIANTID", inlineQueryResponseLine.VariantId },
                        { "RECEIPTDATEREQUESTED", date_now },
                        { "CreatedDateTime", createdDateTime.AddHours(-5) }
                    };

                    // Fill Request Model
                    item.TaxGroup = inlineQueryResponseLineTaxGroupandBusinessDate.TAXGROUP;
                    item.TAXITEMGROUP = taxItemGroup;

                    affectedRows = SqlHelper.ExecuteNonQuery(_connectionString_KFC, lineQuery, CommandType.StoredProcedure, parameters);

                    if (affectedRows > 0)
                    {
                        response.HttpStatusCode = (int)HttpStatusCode.OK;
                        response.MessageType = (int)MessageType.Success;
                    }
                }

            }

            return response;
        }

        private string GetBatchId(string terminal, string store)
        {
            string batchid = "";

            StringBuilder query = new StringBuilder();

            query.Append("select Top 1 SHIFTID from crt.RETAILSHIFTSTAGINGVIEW Where CURRENTTERMINALID ='" + terminal + "' AND STOREID = '" + store + "'  AND STATUS = 1"); ;

            DataSet dataSet = SqlHelper.ExecuteDataSet(_connectionString_KFC, query.ToString(), CommandType.Text);

            if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                DataTable dataTable = dataSet.Tables[0];

                if (dataTable != null && dataTable.Rows != null && dataTable.Rows.Count > 0)
                {
                    //foreach (DataRow item in dataTable.Rows)
                    //{

                    //    batchid = item["SHIFTID"].ToString();
                    //}

                    batchid = dataTable.Rows[0]["SHIFTID"].ToString();
                }

                if (batchid == "null" || batchid == "")
                {
                    batchid = "1";

                }
            }

            return batchid;
        }

        private int insertInMCSDONUMBERTABLE(DriveThru.Integration.Application.Commands.CreateRetailTransactionCommand request)
        {
            int affectedRows = 0;
            DateTime now = DateTime.Now;
            string date_now = now.Year + "-" + now.Month + "-" + now.Day;

            if (request != null && request.salesLines.Count > 0)
            {
                int incrementddoNumber = 0;

                incrementddoNumber = DoNumber(request.Terminal, request.Store, request.Company, request.TransactionId);

                if (incrementddoNumber > 0)
                {

                    //query.Append("SELECT EMP_NAME, EMP_NAME FROM hr_sm_emply WHERE emp_code = @Code");
                    string insertDoNumberQuery = "INSERT INTO ext.MCSDONUMBERTABLE(TERMINAL,BATCHID,CHANNEL,RECEIPTID,SHIFT,SHIFTDATE,STAFF,STORE,TRANSACTIONID,DATAAREAID,DONumber) " +
                                                          "VALUES (@TERMINAL,@BATCHID,@CHANNEL,@RECEIPTID,@SHIFT,@SHIFTDATE,@STAFF,@STORE,@TRANSACTIONID,@DATAAREAID,@DONumber)";

                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "TERMINAL", _terminalId },
                        { "BATCHID", 0 },
                        { "CHANNEL", lastRecordResponse.Channel },
                        { "RECEIPTID", _receiptId },
                        { "SHIFT", 0 },
                        { "SHIFTDATE", date_now },
                        //HArdCode
                        { "TRANSACTIONID", _transactionId },
                        { "STAFF", _staffId },
                        { "STORE", request.Store },
                        { "DONumber", incrementddoNumber },
                        { "DATAAREAID", request.Company }
                    };

                    affectedRows = SqlHelper.ExecuteNonQuery(_connectionString_KFC, insertDoNumberQuery, CommandType.Text, parameters);
                }
            }
            return affectedRows;
        }

        private int DoNumber(string terminal, string store, string company, string transactionId)
        {
            string donumberQuery = "", donumber = "";
            int incrementddoNumber = 0;
            DateTime now = DateTime.Now;
            string date_now = now.Year + "-" + now.Month + "-" + now.Day;

            donumberQuery = "select Top 1 DONumber from ext.MCSDONUMBERTABLE WHERE TERMINAL ='" + terminal + "' AND STORE = '" +
                store + "' AND DATAAREAID= '" + company + "' AND SHIFTDATE = '" +
                date_now + "' AND TransactionID = '" + transactionId + "'";

            DataSet dataSet = SqlHelper.ExecuteDataSet(_connectionString_KFC, donumberQuery.ToString(), CommandType.Text);


            if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                DataTable dataTable = dataSet.Tables[0];

                if (dataTable != null && dataTable.Rows != null && dataTable.Rows.Count > 0)
                {
                    donumber = (from DataRow dr in dataTable.Rows
                                select dr["DONumber"].ToString()).FirstOrDefault();

                    //foreach (DataRow item in dataTable.Rows)
                    //{
                    //    donumber = item["DONumber"].ToString();
                    //}
                }
            }

            if (donumber == null || donumber == "")
            {
                incrementddoNumber = 1;
            }
            else
            {
                //incrementddoNumber = Int32.Parse(donumber);
                incrementddoNumber = Convert.ToInt32(donumber);

            }

            return incrementddoNumber;
        }

        private int CreateRETAILTRANSACTIONPAYMENTTRANS(DriveThru.Integration.Application.Commands.CreateRetailTransactionCommand request)
        {
            ResponseModelWithClass<DataTable> response = new ResponseModelWithClass<DataTable>();
            response.HttpStatusCode = (int)HttpStatusCode.Accepted;
            response.MessageType = (int)MessageType.Info;
            int affectedRows = 0;
            string batchId = string.Empty;
            DateTime now = DateTime.Now;
            string date_now = now.Year + "-" + now.Month + "-" + now.Day;
            double seconds = TimeSpan.Parse(now.ToString("HH:mm:ss")).TotalSeconds;

            String paymentTransQuery = "INSERT INTO ax.RETAILTRANSACTIONPAYMENTTRANS(TERMINAL,CHANNEL,RECEIPTID," +
                "STORE,TRANSACTIONID,TenderType,Staff,QTY,AmountCur,AmountMst,AmountTendered,Currency," +
                "ExchRate,ExchRateMst,TransDate,TransTime,DataAreaId,BusinessDate,ISPAYMENTCAPTURED,REFUNDABLEAMOUNT, LINENUM ) " +
                                                     "VALUES (@TERMINAL,@CHANNEL,@RECEIPTID,@STORE,@TRANSACTIONID," +
                                                     "@TenderType,@Staff, @QTY,@AmountCur,@AmountMst," +
                                                     "@AmountTendered,@Currency,@ExchRate,@ExchRateMst,@TransDate," +
                                                     "@TransTime,@DataAreaId,@BusinessDate,@ISPAYMENTCAPTURED,@REFUNDABLEAMOUNT, @LINENUM )";

            if (request != null && request.salesLines.Count > 0)
            {
                double totalseconds = TimeSpan.Parse(now.ToString("HH:mm:ss")).TotalSeconds;
                int transTime = Convert.ToInt32(totalseconds);


                if (request.TenderTypeId != "" || request.TenderTypeId != null)
                {
                    if (request.TenderTypeId == "20" || request.TenderTypeId == "1")
                    {
                        request.PaymentMode = 1;
                    }
                    else if (request.TenderTypeId == "10" || request.TenderTypeId == "14" || request.TenderTypeId == "31" || request.TenderTypeId == "32" ||
                        request.TenderTypeId == "33" || request.TenderTypeId == "34")
                    {
                        request.PaymentMode = 6;
                    }

                    else if (request.TenderTypeId == "17" || request.TenderTypeId == "21")
                    {
                        request.PaymentMode = 3;
                    }
                }

                StringBuilder query = new StringBuilder();

                //query.Append("SELECT EMP_NAME, EMP_NAME FROM hr_sm_emply WHERE emp_code = @Code");
                query.Append(paymentTransQuery);

                decimal? refundableAmount;

                refundableAmount = request.AmountCur - _totalBillAmount;

                if (refundableAmount > 0)
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                         {
                             { "TERMINAL", _terminalId },
                             { "CHANNEL", _channle },
                             { "RECEIPTID", _receiptId },
                             { "STORE", request.Store },
                             { "TRANSACTIONID", _transactionId },
                             { "TenderType", request.TenderTypeId },
                             { "Staff", _staffId  },
                             { "QTY", 1.00},
                             { "AmountCur", request.AmountCur},
                             { "AmountMst", request.AmountCur},
                             { "AmountTendered", request.AmountCur},
                             { "Currency", request.Currency},
                             { "ExchRate", 100},
                             { "ExchRatemst", 100},
                             { "TRANSDATE", request.TransDate},
                             //{ "TRANSTIME", request.TransTime},
                             { "TRANSTIME", transTime},
                             { "DataAreaId", request.Company},
                             { "BusinessDate", request.BusinessDateCustom},
                             { "ISPAYMENTCAPTURED", 1},
                             { "REFUNDABLEAMOUNT", request.GrossAmount },
                             { "LINENUM", 1 }
                          };

                    affectedRows = SqlHelper.ExecuteNonQuery(_connectionString_KFC, paymentTransQuery, CommandType.Text, parameters);

                    Dictionary<string, object> parametersSecondLine = new Dictionary<string, object>
                         {
                             { "TERMINAL", _terminalId },
                             { "CHANNEL", _channle },
                             { "RECEIPTID", _receiptId },
                             { "STORE", request.Store },
                             { "TRANSACTIONID", _transactionId },
                             { "TenderType", request.TenderTypeId },
                             { "Staff", _staffId },
                             { "QTY", 1.00},
                             { "AmountCur", refundableAmount * -1},
                             { "AmountMst", refundableAmount * -1},
                             { "AmountTendered", refundableAmount * -1},
                             { "Currency", request.Currency},
                             { "ExchRate", 100},
                             { "ExchRatemst", 100},
                             { "TRANSDATE", request.TransDate},
                             //{ "TRANSTIME", request.TransTime},
                             { "TRANSTIME", transTime},
                             { "DataAreaId", request.Company},
                             { "BusinessDate", request.BusinessDateCustom},
                             { "ISPAYMENTCAPTURED", 1},
                             { "REFUNDABLEAMOUNT", 0 },
                             { "LINENUM", 2 }
                          };

                    affectedRows = SqlHelper.ExecuteNonQuery(_connectionString_KFC, paymentTransQuery, CommandType.Text, parametersSecondLine);
                }
                else
                {

                    Dictionary<string, object> parametersSecondLine = new Dictionary<string, object>
                         {
                             { "TERMINAL", _terminalId },
                             { "CHANNEL", _channle },
                             { "RECEIPTID", _receiptId },
                             { "STORE", request.Store },
                             { "TRANSACTIONID", _transactionId },
                             { "TenderType", request.TenderTypeId },
                             { "Staff", _staffId  },

                             { "QTY", 1.00},
                             { "AmountCur", request.AmountCur},
                             { "AmountMst", request.AmountCur},
                             { "AmountTendered", request.AmountCur},
                             { "Currency", request.Currency},
                             { "ExchRate", 100},
                             { "ExchRatemst", 100},
                             { "TRANSDATE", request.TransDate},
                             //{ "TRANSTIME", request.TransTime},
                             { "TRANSTIME", transTime},
                             { "DataAreaId", request.Company},
                             { "BusinessDate", request.BusinessDateCustom},
                             { "ISPAYMENTCAPTURED", 1},
                             { "REFUNDABLEAMOUNT", request.GrossAmount },
                             { "LINENUM", 1 }
                          };

                    affectedRows = SqlHelper.ExecuteNonQuery(_connectionString_KFC, paymentTransQuery, CommandType.Text, parametersSecondLine);
                }



            }
            else
            {
                response.Result = null;
                response.HttpStatusCode = (int)HttpStatusCode.BadRequest;
                response.MessageType = (int)MessageType.Warning;
                affectedRows = -2;
            }

            return affectedRows;
        }

        private InlineQueryResponse getFirstRecFromRetailTransactionTable(string dataAreaId)
        {
            DataSet ds = null;
            DataTable table = new DataTable();

            InlineQueryResponse lastRecordResponse = new InlineQueryResponse();

            string query = "Select Top 1 * from ax.RETAILTRANSACTIONTABLE where DATAAREAID = '" + dataAreaId + "'";

            ds = SqlHelper.ExecuteDataSet(_connectionString_KFC, query, CommandType.Text);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                table = ds.Tables[0];

                if (table.Rows.Count > 0)
                {
                    lastRecordResponse.Channel = (long)Convert.ToDecimal(table.Rows[0]["CHANNEL"]);
                    lastRecordResponse.InventLocationId = table.Rows[0]["INVENTLOCATIONID"].ToString();
                }
            }
            if (lastRecordResponse.Channel == 0 && lastRecordResponse.InventLocationId == null)
            {
                lastRecordResponse.Channel = 5637144590;
                lastRecordResponse.InventLocationId = "10008";
            }

            return lastRecordResponse;
        }

        private InlineQueryResponse getTaxGroupandBusinessDate(string storeId, PaymentMethod paymentMethod)
        {
            DataSet ds = null;
            DataTable table = new DataTable();
            string businessTime = string.Empty;
            DateTime bdate = DateTime.Now;
            string businessDate = "";
            int paymentType = -0;

            if (paymentMethod == PaymentMethod.CreditCard)
            {
                paymentType = 0;
            }
            if (paymentMethod == PaymentMethod.Cash)
            {
                paymentType = 1;
            }

            InlineQueryResponse inlineQueryResponse = new InlineQueryResponse();
            StringBuilder queryAppend = new StringBuilder();

            queryAppend.Append("usp_getTaxGroupAndBusinessDate");

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("storeId", storeId);
            parameters.Add("payment_method", paymentType);

            ds = SqlHelper.ExecuteDataSet(_connectionString_KFC, queryAppend.ToString(), CommandType.StoredProcedure, parameters);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                table = ds.Tables[0];

                if (table.Rows.Count > 0)
                {
                    inlineQueryResponse.SourceTaxGroup = table.Rows[0]["SOURCETAXGROUP"].ToString();
                    inlineQueryResponse.TAXCODE = table.Rows[0]["TAXCODE"].ToString();
                    inlineQueryResponse.TAXVALUE = Convert.ToDecimal(table.Rows[0]["TAXVALUE"]);
                    _taxValue = Convert.ToDecimal(table.Rows[0]["TAXVALUE"]);
                    inlineQueryResponse.TAXGROUP = table.Rows[0]["TAXGROUP"].ToString();
                    inlineQueryResponse.stmtCalcBatchEndTime = table.Rows[0]["stmtCalcBatchEndTime"].ToString();

                    businessTime = inlineQueryResponse.stmtCalcBatchEndTime;


                    TimeSpan time = TimeSpan.FromSeconds(double.Parse(businessTime));
                    TimeSpan businessTimeConverted = TimeSpan.Parse(time.ToString(@"hh\:mm\:ss"));


                    //DateTime dateTime = Convert.ToDateTime(bdate);
                    DateTime dateTime = DateTime.Now;

                    if (dateTime.TimeOfDay > businessTimeConverted)
                    {
                        string bddatetime = dateTime.Year + "-" + dateTime.Month + "-" + dateTime.Day;
                        businessDate = bddatetime;
                        inlineQueryResponse.BusinessDate = businessDate;
                    }
                    else
                    {
                        DateTime newDate = dateTime.AddDays(-1);
                        businessDate = newDate.Year + "-" + newDate.Month + "-" + newDate.Day;
                        inlineQueryResponse.BusinessDate = businessDate;
                    }
                }
            }

            return inlineQueryResponse;
        }

        private string IncrementedId(string storeId, bool isSuspended, bool isTransactionId)
        {
            string returnId = string.Empty;
            string receiptId = string.Empty;
            string transactionId = string.Empty;
            string suspendedTransactionId = string.Empty;
            DataTable dataTable = null;
            string query = string.Empty;
            Random rnd = new Random();
            string last_no = rnd.Next(999999).ToString();


            if (isSuspended)
            {
                query = "SELECT TOP 1 SUSPENDEDTRANSACTIONID  FROM ax.RETAILTRANSACTIONTABLE where RETAILTRANSACTIONTABLE.Terminal = '" + _terminalId + "' AND RETAILTRANSACTIONTABLE.Store = '" + storeId + "' AND SUSPENDEDTRANSACTIONID != '' ORDER BY CREATEDDATETIME DESC ";
            }
            if (isTransactionId)
            {
                query = "SELECT TOP 1 TRANSACTIONID  FROM ax.RETAILTRANSACTIONTABLE " +
                    "where RETAILTRANSACTIONTABLE.Terminal = '" + _terminalId + "' AND RETAILTRANSACTIONTABLE.Store = '" + storeId + "' AND TRANSACTIONID != '' ORDER BY CREATEDDATETIME DESC ";
            }
            if (!isSuspended && !isTransactionId)
            {
                query = "SELECT TOP 1 RECEIPTID  FROM ax.RETAILTRANSACTIONTABLE where RETAILTRANSACTIONTABLE.Terminal = '" + _terminalId + "' AND RETAILTRANSACTIONTABLE.Store = '" + storeId + "' AND RECEIPTID != '' ORDER BY CREATEDDATETIME DESC ";
            }

            DataSet dataSet = SqlHelper.ExecuteDataSet(_connectionString_KFC, query, CommandType.Text, null);

            if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
            {
                dataTable = dataSet.Tables[0];

                if (!isSuspended && !isTransactionId)
                {
                    receiptId = dataTable.Rows[0]["RECEIPTID"].ToString();

                    if (receiptId == "null" || receiptId == "")
                    {
                        #region  --- Receipt id with datetime ---

                        long DatetimeToseconds = Convert.ToInt64(DateTime.Now.ToUniversalTime().Subtract(
                        new DateTime()
                        ).TotalMilliseconds);

                        string secodsToString = DatetimeToseconds.ToString();

                        last_no = StringHelper.GetLast(secodsToString, 9);


                        #endregion

                        #region --- Receipt id with random int ---

                        //last_no = "000001";
                        //string lastNo = rnd.NextInt64(999).ToString();
                        //string lastNo2 = rnd.NextInt64(999).ToString();
                        //string lastNo3 = rnd.NextInt64(999).ToString();
                        //last_no = lastNo + lastNo2 + lastNo3;


                        #endregion

                    }
                    else
                    {
                        string[] authorsList = receiptId.Split('-');
                        if (authorsList.Length > 1)
                        {
                            #region  --- Receipt id with datetime ---

                            long DatetimeToseconds = Convert.ToInt64(DateTime.Now.ToUniversalTime()
                                .Subtract(new DateTime()).TotalMilliseconds);

                            string secodsToString = DatetimeToseconds.ToString();

                            last_no = StringHelper.GetLast(secodsToString, 9);


                            #endregion

                            #region --- Receipt id with random int ---


                            //string lastNo = rnd.NextInt64(999).ToString();
                            //string lastNo2 = rnd.NextInt64(999).ToString();
                            //string lastNo3 = rnd.NextInt64(999).ToString();

                            //last_no = lastNo + lastNo2 + lastNo3;


                            #endregion


                            #region --- Receipt Id with Incremented Counter ---
                            /*
                            long val;
                            if (!string.IsNullOrEmpty(authorsList[1].ToString()))
                            {
                                val = long.Parse(authorsList[1].ToString());
                                val = val + 5;
                            }
                            else
                            {
                                val = 0;
                            }

                            last_no = val.ToString();

                            */
                            #endregion
                        }
                        else
                        {
                            //last_no = "000001";

                            string lastNo = rnd.NextInt64(999).ToString();
                            string lastNo2 = rnd.NextInt64(999).ToString();
                            string lastNo3 = rnd.NextInt64(999).ToString();
                            last_no = lastNo + lastNo2 + lastNo3;

                        }
                    }

                    receiptId = storeId + _terminalId + "-" + last_no;

                    returnId = receiptId;
                }
                //same case for create TransactionID
                if (isSuspended)
                {
                    suspendedTransactionId = dataTable.Rows[0]["SUSPENDEDTRANSACTIONID"].ToString();

                    if (suspendedTransactionId == "null" || suspendedTransactionId == " " || suspendedTransactionId == "")
                    {
                        string lastNo = rnd.NextInt64(9999).ToString();
                        string lastNo2 = rnd.NextInt64(9999).ToString();
                        last_no = lastNo + lastNo2;

                    }
                    else
                    {
                        string[] authorsList = suspendedTransactionId.Split('-');
                        if (authorsList.Length > 1)
                        {
                            long val;
                            if (!string.IsNullOrEmpty(authorsList[2].ToString()))
                            {
                                val = long.Parse(authorsList[2].ToString());
                                val = val + 5;
                            }
                            else
                            {
                                val = 0;
                            }

                            /*
                            //long finalval = val + 1;
                            long finalval = val + rnd.NextInt64(999999);
                            last_no = finalval.ToString();
                            last_no = finalval.ToString("D6");
                            */
                            last_no = val.ToString();
                        }
                        else
                        {
                            //last_no = "000001";
                            string lastNo = rnd.NextInt64(999999).ToString();
                            last_no = lastNo;
                        }
                    }

                    suspendedTransactionId = storeId + "-" + _terminalId + "-" + last_no;

                    returnId = suspendedTransactionId;
                }
                if (isTransactionId)
                {
                    transactionId = dataTable.Rows[0]["TRANSACTIONID"].ToString();

                    if (transactionId == "null" || transactionId == " " || transactionId == "")
                    {
                        //last_no = "000001";
                        string lastNo = rnd.NextInt64(9999).ToString();
                        string lastNo2 = rnd.NextInt64(9999).ToString();
                        last_no = lastNo + lastNo2;

                    }
                    else
                    {
                        string[] authorsList = transactionId.Split('-');
                        if (authorsList.Length > 1)
                        {
                            string lastNo = rnd.NextInt64(9999).ToString();
                            string lastNo2 = rnd.NextInt64(9999).ToString();
                            last_no = lastNo + lastNo2;
                            /*
                            long val;
                            if (!string.IsNullOrEmpty(authorsList[2].ToString()))
                            {
                                val = long.Parse(authorsList[2].ToString());
                                val = val + 5;
                            }
                            else
                            {
                                val = 0;
                            }

                           
                            last_no = val.ToString();
                            */
                        }
                        else
                        {
                            string lastNo = rnd.NextInt64(9999).ToString();
                            string lastNo2 = rnd.NextInt64(9999).ToString();
                            last_no = lastNo + lastNo2;
                        }
                    }

                    transactionId = storeId + "-" + _terminalId + "-" + last_no;

                    returnId = transactionId;
                }
            }
            else
            {
                if (isSuspended)
                {
                    returnId = storeId + "-" + _terminalId + "-" + last_no;
                }
                if (isTransactionId)
                {
                    returnId = storeId + "-" + _terminalId + "-" + last_no;
                }
                if (!isSuspended && !isTransactionId)
                {
                    returnId = storeId + _terminalId + "-" + last_no;
                }
            }


            return returnId;
        }

        private InlineQueryResponse getVariantIdandUnitId(string company, string itemId)
        {
            string variantID = "";

            string queryVariantId = "Select Top 1 RETAILVARIANTID from ax.INVENTDIMCOMBINATION Where ax.INVENTDIMCOMBINATION.DATAAREAID ='" + company + "' AND ax.INVENTDIMCOMBINATION.ITEMID = '" + itemId + "' Order BY CREATEDDATETIME DESC";

            DataSet dataSetVriantId = SqlHelper.ExecuteDataSet(_connectionString_KFC, queryVariantId, CommandType.Text, null);

            if (dataSetVriantId != null && dataSetVriantId.Tables[0].Rows.Count > 0)
            {
                dataTable = dataSetVriantId.Tables[0];

                variantID = dataTable.Rows[0]["RETAILVARIANTID"].ToString();
            }

            string unitId = "";
            string queryUnitId = "select UNITID from ax.INVENTTABLEMODULE Where DATAAREAID = '" + company + "' AND MODULETYPE = 2 AND ITEMID = '" + itemId + "'";

            DataSet dataSetUnitId = SqlHelper.ExecuteDataSet(_connectionString_KFC, queryUnitId, CommandType.Text, null);

            if (dataSetUnitId != null && dataSetUnitId.Tables[0].Rows.Count > 0)
            {
                dataTable = dataSetUnitId.Tables[0];

                unitId = dataTable.Rows[0]["UNITID"].ToString();
            }

            InlineQueryResponse inlineQueryResponse = new InlineQueryResponse();

            inlineQueryResponse.VariantId = variantID;
            inlineQueryResponse.UnitId = unitId;

            return inlineQueryResponse;
        }

        private string TaxItemGroup(string company, string itemId)
        {
            string taxItemGroup = "";

            string queryVariantId = "SELECT TAXITEMGROUPID FROM ax.INVENTTABLEMODULE where DATAAREAID = '" + company + "' And MODULETYPE = 2 AND ITEMID = '" + itemId + "'";
            DataSet dataSetVriantId = SqlHelper.ExecuteDataSet(_connectionString_KFC, queryVariantId, CommandType.Text, null);

            if (dataSetVriantId != null && dataSetVriantId.Tables[0].Rows.Count > 0)
            {
                dataTable = dataSetVriantId.Tables[0];

                taxItemGroup = dataTable.Rows[0]["TAXITEMGROUPID"].ToString();
            }
            return taxItemGroup;
        }

        private string GetInventLocation(string storeId)
        {
            string inventLocationId = string.Empty;

            string query = "SELECT ax.RETAILSTORETABLE.RECID , ax.RETAILCHANNELTABLE.INVENTLOCATION FROM ax.RETAILSTORETABLE JOIN ax.RETAILCHANNELTABLE ON ax.RETAILCHANNELTABLE.RECID = ax.RETAILSTORETABLE.RECID" +
                                    "   where ax.RETAILSTORETABLE.StoreNumber = '" + storeId + "'";

            DataSet dataSet = SqlHelper.ExecuteDataSet(_connectionString_KFC, query, CommandType.Text, null);

            if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dataTable = dataSet.Tables[0];

                inventLocationId = dataTable.Rows[0]["INVENTLOCATION"].ToString();
            }
            else
            {
                inventLocationId = storeId;
            }

            return inventLocationId;
        }

        private int InsertCrtSALESTRANSACTION(DriveThru.Integration.Application.Commands.CreateRetailTransactionCommand request)
        {
            int affectedRows = 0;

            request.Id = _transactionId;
            request.DataAreaId = request.Company;



            var json = JsonConvert.SerializeObject(request);


            byte[] ba = Encoding.Default.GetBytes(json);
            int byteLenght = ba.Length;
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            hex.ToString();

            byte[] theBytes = Encoding.UTF8.GetBytes(hex.ToString());

            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ss");

            /*
            byte[] ba = Encoding.Default.GetBytes(json);
            int byteLenght = ba.Length;
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            hex.ToString();

            byte[] theBytes = Encoding.UTF8.GetBytes(hex.ToString());
            */


            string description = " ThirdPartyOrder: " + request.ThirdPartyOrderId + " DriveThru " + "; Source: " + request.Source + ";";

            string insertCrtSALESTRANSACTIONQuery = "INSERT INTO [crt].[SALESTRANSACTION]" +
        "([TRANSACTIONID], [CHANNELID], [TERMINALID], BYTELENGTH, TRANSACTIONDATA,CREATEDDATETIME,MODIFIEDDATETIME, [ISSUSPENDED], [TYPE], [STAFF], [AMOUNT], [STORAGETYPE], [READONLYREASON], [Comment])" +
        "VALUES(@TRANSACTIONID, @CHANNELID, @TERMINALID, @BYTELENGTH, @TRANSACTIONDATA,@CREATEDDATETIME,@MODIFIEDDATETIME, @ISSUSPENDED, @TYPE, @STAFF, @AMOUNT, @STORAGETYPE, @READONLYREASON, @Comment)";

            Dictionary<string, object> parameters = new Dictionary<string, object>
              {
                  { "TRANSACTIONID", _transactionId },
                  { "CHANNELID", _channle },
                  { "TERMINALID", _terminalId },
                  { "BYTELENGTH", 6224 },
                  { "TRANSACTIONDATA", theBytes },
                  { "CREATEDDATETIME", dateTime},
                  { "MODIFIEDDATETIME", dateTime},
                  { "ISSUSPENDED", 0 },
                  { "TYPE", 1 },
                  { "STAFF", _staffId },
                  { "AMOUNT", request.AmountCur },
                  { "STORAGETYPE", 2 },
                  { "READONLYREASON", "" },
                  { "Comment", description }
              };

            affectedRows = SqlHelper.ExecuteNonQuery(_connectionString_KFC, insertCrtSALESTRANSACTIONQuery, CommandType.Text, parameters);

            //}

            return affectedRows;
        }

        private decimal CalculateTaxedPrice(decimal itemPrice, decimal taxPercentage)
        {

            decimal taxRatePercentage = taxPercentage / 100;

            decimal TotaltaxPrice = itemPrice * taxRatePercentage;

            return TotaltaxPrice;
        }

        private decimal CalculatePriceWithTax(decimal itemPrice, decimal taxPercentage)
        {

            decimal taxRatePercentage = taxPercentage / 100;

            //decimal PriceExcTax = Convert.ToDecimal(itemPrice + (itemPrice * Convert.ToDouble(taxRatePercentage)));
            decimal PriceExcTax = itemPrice + (itemPrice * taxRatePercentage);

            return PriceExcTax;
        }

        private string StaffId(string terminal)
        {
            string staffId = string.Empty;

            string queryStaffId = "SELECT TOP 1 StaffID FROM crt.RETAILSHIFTSTAGINGVIEW where TerminalId = '" + _terminalId + "' And Status = 1 ";

            DataSet ds = SqlHelper.ExecuteDataSet(_connectionString_KFC, queryStaffId, CommandType.Text, null);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                dataTable = ds.Tables[0];

                staffId = dataTable.Rows[0]["StaffID"].ToString();
            }
            return staffId;
        }

        private bool DeleteCurrentRecord(string reciptId)
        {
            bool result = false;

            string deleteFrom_RetailTransactionTable = "Delete from ax.RETAILTRANSACTIONTABLE where RECEIPTID = '" + reciptId + "' ";
            string deleteFrom_RetailTransactionSalesTrans = "Delete from ax.RETAILTRANSACTIONSALESTRANS where RECEIPTID = '" + reciptId + "' ";
            string deleteFrom_MCSDoNumberTable = "Delete from ext.MCSDONUMBERTABLE where RECEIPTID = '" + reciptId + "' ";
            string deleteFrom_RETAILTRANSACTIONPAYMENTTRANS = "Delete from ax.RETAILTRANSACTIONPAYMENTTRANS where RECEIPTID = '" + reciptId + "' ";

            int IsdeleteFrom_RetailTransactionTable = 0;
            int IsdeleteFrom_RetailTransactionSalesTrans = 0;
            int IsdeleteFrom_MCSDoNumberTable = 0;
            int IsdeleteFrom_RETAILTRANSACTIONPAYMENTTRANS = 0;

            IsdeleteFrom_RetailTransactionTable = SqlHelper.ExecuteNonQuery(_connectionString_KFC, deleteFrom_RetailTransactionTable.ToString(), CommandType.Text, null);

            if (IsdeleteFrom_RetailTransactionTable > 0)
            {
                IsdeleteFrom_RetailTransactionSalesTrans = SqlHelper.ExecuteNonQuery(_connectionString_KFC, deleteFrom_RetailTransactionSalesTrans, CommandType.Text, null);

                if (IsdeleteFrom_RetailTransactionSalesTrans > 0)
                {
                    IsdeleteFrom_MCSDoNumberTable = SqlHelper.ExecuteNonQuery(_connectionString_KFC, deleteFrom_MCSDoNumberTable, CommandType.Text, null);

                    if (IsdeleteFrom_MCSDoNumberTable > 0)
                    {
                        IsdeleteFrom_RETAILTRANSACTIONPAYMENTTRANS = SqlHelper.ExecuteNonQuery(_connectionString_KFC, deleteFrom_RETAILTRANSACTIONPAYMENTTRANS, CommandType.Text, null);

                        if (IsdeleteFrom_RETAILTRANSACTIONPAYMENTTRANS > 0)
                        {
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        private int CreateRetailTransactionTaxTrans(DriveThru.Integration.Application.Commands.CreateRetailTransactionCommand request)
        {
            ResponseModelWithClass<DataTable> response = new ResponseModelWithClass<DataTable>();
            response.HttpStatusCode = (int)HttpStatusCode.Accepted;
            response.MessageType = (int)MessageType.Info;
            int affectedRows = 0;

            InlineQueryResponse inlineQueryResponseTax = new InlineQueryResponse();

            inlineQueryResponseTax = getTaxGroupandBusinessDate(request.Store, request.Payment_method);

            int lastCounterNo = GetREPLICATIONCOUNTERFROMORIGIN(request.Store);


            lastCounterNo = lastCounterNo + 1;
            string storeProcedureNamr = "usp_insert_RETAILTRANSACTIONTAXTRANS";


            if (request != null && request.salesLines.Count > 0)
            {
                StringBuilder query = new StringBuilder();
                query.Append(storeProcedureNamr);


                if (request != null && request.salesLines.Count > 0)
                {
                    foreach (var item in request.salesLines)
                    {
                        Dictionary<string, object> parameters = new Dictionary<string, object>
                         {
                             { "AMOUNT", item.TaxAmount },
                             { "CHANNEL", _channle },
                             { "ISINCLUDEDINPRICE", 1 },
                           //{ "REPLICATIONCOUNTERFROMORIGIN", lastCounterNo },
                             { "SALELINENUM", item.LineNum },
                             { "STOREID", request.Store },
                             { "TAXCODE", inlineQueryResponseTax.TAXCODE},
                             { "TERMINALID", _terminalId },
                             { "TRANSACTIONID", _transactionId },
                             { "TAXBASEAMOUNT", item.NETAMOUNT },
                             { "TAXPERCENTAGE", inlineQueryResponseTax.TAXVALUE},
                             { "ISEXEMPT", 0.00},
                             { "DATAAREAID", request.Company}
                          };

                        affectedRows = SqlHelper.ExecuteNonQuery(_connectionString_KFC, storeProcedureNamr, CommandType.StoredProcedure, parameters);

                    }

                }
            }
            else
            {
                response.Result = null;
                response.HttpStatusCode = (int)HttpStatusCode.BadRequest;
                response.MessageType = (int)MessageType.Warning;
                affectedRows = -2;
            }

            return affectedRows;
        }

        private int GetREPLICATIONCOUNTERFROMORIGIN(string storeId)
        {
            int lasrREPLICATIONCOUNTERFROMORIGIN = 0;
            Random randomInt = new Random();

            string query = "select Top 1 REPLICATIONCOUNTERFROMORIGIN from ax.RETAILTRANSACTIONTAXTRANS where StoreId = '" + storeId + "'" +
            "Order By REPLICATIONCOUNTERFROMORIGIN desc";

            DataSet dataSet = SqlHelper.ExecuteDataSet(_connectionString_KFC, query, CommandType.Text, null);

            if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
            {
                DataTable dataTable = dataSet.Tables[0];

                lasrREPLICATIONCOUNTERFROMORIGIN = Convert.ToInt32(dataTable.Rows[0]["REPLICATIONCOUNTERFROMORIGIN"].ToString());

                if (lasrREPLICATIONCOUNTERFROMORIGIN == 0)
                {
                    lasrREPLICATIONCOUNTERFROMORIGIN = randomInt.Next(30000, 90000);
                }
            }
            else
            {
                lasrREPLICATIONCOUNTERFROMORIGIN = randomInt.Next(30000, 90000);
            }

            return lasrREPLICATIONCOUNTERFROMORIGIN;
        }

        private int InsertMZNFBRInvoicing(DriveThru.Integration.Application.Commands.CreateRetailTransactionCommand request, string fbrInvoiceNumber)
        {
            int affectedRows = 0;

            string insertCrtSALESTRANSACTIONQuery = "usp_create_MZNFBRINVOICING";

            Dictionary<string, object> parameters = new Dictionary<string, object>
              {
                  { "StoreId", request.Store },
                  { "DataAreaId", request.Company },
                  { "TRANSACTIONID", _transactionId },
                  { "TERMINAL", _terminalId },
                  { "FBRINVOICENO", fbrInvoiceNumber },
                  { "FBRRESPONSE", "FBRInvoiceNumber:"+fbrInvoiceNumber+"code: 100, success" },
                  { "RESPONSE", "KIOSK-API Order , Transaction ID: " +_transactionId+" \"FBRInvoiceNumber:\"+fbrInvoiceNumber+\""}

              };

            affectedRows = SqlHelper.ExecuteNonQuery(_connectionString_KFC, insertCrtSALESTRANSACTIONQuery, CommandType.StoredProcedure, parameters);

            //}

            return affectedRows;
        }

        public string SRB(DriveThru.Integration.Application.Commands.CreateRetailTransactionCommand request)
        {
            string json = string.Empty;
            string SRBInvoiceNumber = string.Empty;
            string responseFBR = string.Empty;
            decimal saleValue = request.GrossAmount - request.NetAmount;
            FBRResponse fBRResponse = new FBRResponse();
            MZNPOSTERMINALINFOResponse mZNPOSTERMINALINFOResponse = getURLandToken(_terminalId);

            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(mZNPOSTERMINALINFOResponse.Url);

            myReq.ContentType = "application/x-www-form-urlencoded";
            myReq.Method = "POST";

            using (var streamWriter = new StreamWriter(myReq.GetRequestStream()))
            {

                json = "{'posId':" + mZNPOSTERMINALINFOResponse.PosId +
            ",'name':'" + "KFC" +
            "', 'ntn':'" + "0819531" +
            "', 'invoiceId':'" + _transactionId +
            "', 'invoiceType':" + 1 +
            ", 'invoiceDateTime':'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
            "', 'rateValue':" + _taxValue +
            ", 'saleValue':" + request.GrossAmount +
            ", 'taxAmount':" + saleValue +
            ", 'consumerName':'N/A', 'consumerNTN':'N/A', 'address':'N/A', 'tariffCode':'N/A', 'extraInf':'N/A'" +
            ", 'pos_user':'" + mZNPOSTERMINALINFOResponse.UserName + "'" +
            ", 'pos_pass':'" + mZNPOSTERMINALINFOResponse.Password +
            "'}";

                json = json.Replace("'", "\"");
                streamWriter.Write(json);


            }

            var httpResponse = (HttpWebResponse)myReq.GetResponse();
            try
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    //  ResponseJson = httpresponse.Content.ReadAsStringAsync().Result;
                    SRBInvoiceNumber = JObject.Parse(result).GetValue("srbInvoceId").ToString();
                    InsertSimplexRequestLog(request, "SRB Error");
                }
            }
            catch (Exception ex)
            {

                SRBInvoiceNumber = "invalid Username or password SRB";
            }
           


            return SRBInvoiceNumber;

        }
        public async Task<FBRResponse> FBR(DriveThru.Integration.Application.Commands.CreateRetailTransactionCommand request)
        {
            string invoiceNumber = string.Empty;
            string responseFBR = string.Empty;
            FBRResponse fBRResponse = new FBRResponse();
            MZNPOSTERMINALINFOResponse mZNPOSTERMINALINFOResponse = getURLandToken(_terminalId);
            try
            {
                if (mZNPOSTERMINALINFOResponse != null)
                {
                    string token = mZNPOSTERMINALINFOResponse.Token;
                    //string token = "1298b5eb-b252-3d97-8622-a4a69d5bf818";
                    //string url = "https://esp.fbr.gov.pk:8244/FBR/v1/api/Live/PostData";
                    string url = mZNPOSTERMINALINFOResponse.Url;
                    string responses = null;


                    FBRRequestPayload fBRPayload = new FBRRequestPayload();

                    List<Items> items = new List<Items>();

                    fBRPayload.POSID = mZNPOSTERMINALINFOResponse.PosId;
                    fBRPayload.USIN = "USIN0";
                    fBRPayload.DateTime = DateTime.Now;
                    fBRPayload.BuyerNTN = "1234567-8";
                    //payload.BuyerCNIC = "12345-1234567-8";
                    //payload.BuyerName = "Name";
                    //payload.BuyerPhoneNumber = "0000 - 0000000";
                    fBRPayload.TotalBillAmount = request.GrossAmount;
                    fBRPayload.TotalQuantity = 0.00;
                    fBRPayload.TotalSaleValue = request.NetPrice;
                    fBRPayload.TotalTaxCharged = request.TotalTaxCharged;
                    fBRPayload.Discount = 0.0;
                    fBRPayload.FurtherTax = 0.0;
                    fBRPayload.PaymentMode = 1;
                    fBRPayload.RefUSIN = null;
                    fBRPayload.InvoiceType = 1;

                    foreach (var item in request.salesLines)
                    {
                        Items itemsa = new Items()
                        {
                            ItemCode = item.ItemId,
                            ItemName = _itemName,
                            Quantity = Convert.ToInt32(item.Qty),
                            PCTCode = "11001010",
                            TaxRate = item.TAXRATEPERCENT,
                            //SaleValue = item.NETAMOUNT,
                            TotalAmount = item.NETAMOUNTINCLTAX,
                            TaxCharged = item.TaxAmount,
                            Discount = 0.0,
                            InvoiceType = 1,
                            FurtherTax = 0.0,
                            RefUSIN = null,
                        };

                        fBRPayload.Items.Add(itemsa);
                    }




                    if (token != null && token != string.Empty)
                    {
                        using (var httpClientHandler = new HttpClientHandler())
                        {
                            // NB: You should make this more robust by actually checking the certificate:
                            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                            using (var client = new HttpClient(httpClientHandler))
                            {
                                //for test
                                //var requesat = new HttpRequestMessage(HttpMethod.Post, "https://esp.fbr.gov.pk:8244/FBR/v1/api/Live/PostData");
                                //requesat.Headers.Add("Authorization", "Bearer 1298b5eb-b252-3d97-8622-a4a69d5bf818");

                                //For Real time
                                var requesat = new HttpRequestMessage(HttpMethod.Post, mZNPOSTERMINALINFOResponse.Url);
                                requesat.Headers.Add("Authorization", "Bearer " + mZNPOSTERMINALINFOResponse.Token);

                                string json = JsonHelper.Serialize(fBRPayload);

                                var content = new StringContent(json, null, "application/json");


                                requesat.Content = content;
                                var responsesa = await client.SendAsync(requesat);
                                responsesa.EnsureSuccessStatusCode();
                                responses = await responsesa.Content.ReadAsStringAsync();
                            }
                        }


                        fBRResponse.InvoiceNumber = JObject.Parse(responses).GetValue("InvoiceNumber").ToString();
                        fBRResponse.FBRFullResponse = JObject.Parse(responses).GetValue("Response").ToString();


                        #region --- old code send request ---
                        /*
                        var requestt = new HttpRequestMessage(HttpMethod.Post, url);
                        requestt.Headers.Add("Content", ""); //c5109alf - b43e - 3ea9 - a222 - 019c918e6930
                        requestt.Headers.Add("Authorization", "Bearer " + token);
                        //   requestt.Headers.Add("Authorization", Token);



                        requestt.Content = content1;

                        var response =  client.SendAsync(requestt);
                        value = await response.Content.ReadAsStringAsync();

                        invoiceNumber = JObject.Parse(value).GetValue("InvoiceNumber").ToString();
                        responseFBR = JObject.Parse(value).GetValue("Response").ToString();
                        */
                        #endregion
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }


            return fBRResponse;

        }

        public async Task<FBRResponse> FBR_TestURL(DriveThru.Integration.Application.Commands.CreateRetailTransactionCommand request)
        {
            string invoiceNumber = string.Empty;
            string responseFBR = string.Empty;
            FBRResponse fBRResponse = new FBRResponse();
            MZNPOSTERMINALINFOResponse mZNPOSTERMINALINFOResponse = getURLandToken(_terminalId);
            try
            {
                if (mZNPOSTERMINALINFOResponse != null)
                {
                    //string token = mZNPOSTERMINALINFOResponse.Token;
                    //string url = mZNPOSTERMINALINFOResponse.Url;

                    string token = "1298b5eb-b252-3d97-8622-a4a69d5bf818";
                    string url = "https://esp.fbr.gov.pk:8244/FBR/v1/api/Live/PostData";
                    string responses = null;


                    FBRRequestPayload fBRPayload = new FBRRequestPayload();

                    List<Items> items = new List<Items>();

                    fBRPayload.POSID = "1205";
                    //fBRPayload.POSID = mZNPOSTERMINALINFOResponse.PosId;
                    fBRPayload.USIN = "USIN0";
                    fBRPayload.DateTime = DateTime.Now;
                    fBRPayload.BuyerNTN = "1234567-8";
                    //payload.BuyerCNIC = "12345-1234567-8";
                    //payload.BuyerName = "Name";
                    //payload.BuyerPhoneNumber = "0000 - 0000000";
                    fBRPayload.TotalBillAmount = request.GrossAmount;
                    fBRPayload.TotalQuantity = 0.00;
                    fBRPayload.TotalSaleValue = request.NetPrice;
                    fBRPayload.TotalTaxCharged = request.TotalTaxCharged;
                    fBRPayload.Discount = 0.0;
                    fBRPayload.FurtherTax = 0.0;
                    fBRPayload.PaymentMode = 1;
                    fBRPayload.RefUSIN = null;
                    fBRPayload.InvoiceType = 1;

                    foreach (var item in request.salesLines)
                    {
                        Items itemsa = new Items()
                        {
                            ItemCode = item.ItemId,
                            ItemName = _itemName,
                            Quantity = Convert.ToInt32(item.Qty),
                            PCTCode = "11001010",
                            TaxRate = item.TAXRATEPERCENT,
                            //SaleValue = item.NETAMOUNT,
                            TotalAmount = item.NETAMOUNTINCLTAX,
                            TaxCharged = item.TaxAmount,
                            Discount = 0.0,
                            InvoiceType = 1,
                            FurtherTax = 0.0,
                            RefUSIN = null,
                        };

                        fBRPayload.Items.Add(itemsa);
                    }

                    // bool containsItem = request.salesLines.Any(x => x.BATCHTERMINALID == "123" && x.ItemId == "test" && x.StaffID == "GUID");

                    /*
                    if (token != null && token != string.Empty)
                    {
                        using (var httpClientHandler = new HttpClientHandler())
                        {
                            // NB: You should make this more robust by actually checking the certificate:
                            //httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                            using (var client = new HttpClient(httpClientHandler))
                            {
                                //for test
                                var requesat = new HttpRequestMessage(HttpMethod.Post, "https://esp.fbr.gov.pk:8244/FBR/v1/api/Live/PostData");
                                requesat.Headers.Add("Authorization", "Bearer 1298b5eb-b252-3d97-8622-a4a69d5bf818");

                                //For Real time
                                //var requesat = new HttpRequestMessage(HttpMethod.Post, mZNPOSTERMINALINFOResponse.Url);
                                //requesat.Headers.Add("Authorization", "Bearer " + mZNPOSTERMINALINFOResponse.Token);

                                string json = JsonHelper.Serialize(fBRPayload);

                                var content = new StringContent(json, null, "application/json");


                                requesat.Content = content;
                                var responsesa = await client.SendAsync(requesat);
                                responsesa.EnsureSuccessStatusCode();
                                responses = await responsesa.Content.ReadAsStringAsync();
                            }
                        }


                        fBRResponse.InvoiceNumber = JObject.Parse(responses).GetValue("InvoiceNumber").ToString();
                        fBRResponse.FBRFullResponse = JObject.Parse(responses).GetValue("Response").ToString();
                    }

                    */

                    #region --- old code send request ---


                    using (var httpClientHandler = new HttpClientHandler())
                    {
                        // NB: You should make this more robust by actually checking the certificate:
                        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => false;

                        using (var client = new HttpClient(httpClientHandler))
                        {
                            var requestt = new HttpRequestMessage(HttpMethod.Post, url);
                            requestt.Headers.Add("Content", ""); //c5109alf - b43e - 3ea9 - a222 - 019c918e6930
                            requestt.Headers.Add("Authorization", "Bearer " + token);
                            //   requestt.Headers.Add("Authorization", Token);




                            //var client = new HttpClient();
                            var requestF = new HttpRequestMessage(HttpMethod.Post, "https://esp.fbr.gov.pk:8244/FBR/v1/api/Live/PostData");
                            requestF.Headers.Add("Authorization", "Bearer 1298b5eb-b252-3d97-8622-a4a69d5bf818");
                            var content = new StringContent("{\r\n  \"invoiceNumber\": null,\r\n  \"posid\": \"1205\",\r\n  \"usin\": \"USIN0\",\r\n  \"dateTime\": \"2024-01-07T14:54:36.6141992+05:00\",\r\n  \"buyerNTN\": \"1234567-8\",\r\n  \"buyerCNIC\": null,\r\n  \"buyerName\": null,\r\n  \"buyerPhoneNumber\": null,\r\n  \"totalBillAmount\": 0,\r\n  \"totalQuantity\": 0,\r\n  \"totalSaleValue\": 0,\r\n  \"totalTaxCharged\": 0,\r\n  \"discount\": 0,\r\n  \"furtherTax\": 0,\r\n  \"paymentMode\": 1,\r\n  \"refUSIN\": null,\r\n  \"invoiceType\": 1,\r\n  \"items\": [\r\n    {\r\n      \"itemCode\": \"string\",\r\n      \"itemName\": \"\",\r\n      \"quantity\": 0,\r\n      \"pctCode\": \"11001010\",\r\n      \"taxRate\": 0,\r\n      \"saleValue\": 0,\r\n      \"totalAmount\": 0,\r\n      \"taxCharged\": 0,\r\n      \"discount\": 0,\r\n      \"furtherTax\": 0,\r\n      \"invoiceType\": 1,\r\n      \"refUSIN\": null\r\n    }\r\n  ]\r\n}", null, "application/json");
                            requestF.Content = content;
                            try
                            {
                                var response = await client.SendAsync(requestF);
                                response.EnsureSuccessStatusCode();
                                string a = await response.Content.ReadAsStringAsync();


                                invoiceNumber = JObject.Parse(a).GetValue("InvoiceNumber").ToString();
                                responseFBR = JObject.Parse(a).GetValue("Response").ToString();
                            }
                            catch (Exception ex)
                            {

                                throw;
                            }

                        }
                    }



                    #endregion
                }

            }
            catch (Exception ex)
            {

                throw;
            }


            return fBRResponse;

        }

        private MZNPOSTERMINALINFOResponse getURLandToken(string terminalId)
        {
            MZNPOSTERMINALINFOResponse mZNPOSTERMINALINFOResponse = new MZNPOSTERMINALINFOResponse();
            string returnString = string.Empty;
            DataSet ds = new DataSet();


            string query = "SELECT Top 1 * FROM EXT.MZNPOSTERMINALINFO where TERMINAL = @TERMINAL";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"TERMINAL", terminalId}
            };

            ds = SqlHelper.ExecuteDataSet(_connectionString_KFC, query, CommandType.Text, parameters);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dataTable = ds.Tables[0];


                mZNPOSTERMINALINFOResponse.UserName = dataTable.Rows[0]["UserName"].ToString();
                mZNPOSTERMINALINFOResponse.Password = dataTable.Rows[0]["Password"].ToString();
                mZNPOSTERMINALINFOResponse.Token = dataTable.Rows[0]["TOKEN"].ToString();
                mZNPOSTERMINALINFOResponse.Type = Convert.ToInt32(dataTable.Rows[0]["AuthorityType"]);

                mZNPOSTERMINALINFOResponse.Url = dataTable.Rows[0]["ONLINEURL"].ToString();
                mZNPOSTERMINALINFOResponse.PosId = dataTable.Rows[0]["POSID"].ToString();


            }
            return mZNPOSTERMINALINFOResponse;
        }

        public Task<ResponseModelWithClass<CreateOrderResponse>> CreateOrderKFCA(CreateRetailTransactionCommand request)
        {
            throw new NotImplementedException();
        }

        private int InsertRetaailTransactionMarkupTrans(DriveThru.Integration.Application.Commands.CreateRetailTransactionCommand request)
        {
            int affectedRows = 0;

            string insertRETAILTRANSACTIONMARKUPTRANS = "usp_insert_RETAILTRANSACTIONMARKUPTRANS";

            Dictionary<string, object> parameters = new Dictionary<string, object>
              {
                  { "CALCULATEDAMOUNT", request.POSFee},
                  { "TRANSACTIONID", _transactionId },
                  { "CHANNEL", _channle },
                  { "Store", request.Store },
                  { "TERMINALID", _terminalId },
                  { "Value", request.POSFee}
              };

            affectedRows = SqlHelper.ExecuteNonQuery(_connectionString_KFC, insertRETAILTRANSACTIONMARKUPTRANS, CommandType.StoredProcedure, parameters);

            //}

            return affectedRows;
        }

        private int InsertSimplexRequestLog(DriveThru.Integration.Application.Commands.CreateRetailTransactionCommand request, string msg)
        {
            int affectedRows = 0;

            string requestedJson = JsonConvert.SerializeObject(request);

            string InsertSimplexRequestLogQuery = "usp_SimplexRequestLog";

            Dictionary<string, object> parameters = new Dictionary<string, object>
              {
                  { "json", requestedJson },
                  { "ThirdPartyOrderId", request.ThirdPartyOrderId},
                  { "storeid", request.Store },
                  { "TerminalId", _terminalId },
                  { "ReceiptId", _receiptId },
                  { "Message", msg },
                  { "createdOn", DateTime.Now }

              };

            affectedRows = SqlHelper.ExecuteNonQuery(_connectionString_KFC, InsertSimplexRequestLogQuery, CommandType.StoredProcedure, parameters);

            //}

            return affectedRows;
        }
    }
}
