using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.DTO.Response
{
    public class InlineQueryResponse
    {
        public long Channel { get; set; }
        public string InventLocationId { get; set; }
        public decimal TAXVALUE { get; set; }
        public string SourceTaxGroup { get; set; }
        public string TAXGROUP { get; set; }
        public string TAXCODE { get; set; }
        public string stmtCalcBatchEndTime { get; set; }
        public string BusinessDate { get; set; }
        public string VariantId { get; set; }
        public string UnitId { get; set; }
        public string PosId { get; set; }
        public string Url { get; set; }
        public string BearerToken { get; set; }
    }
}
