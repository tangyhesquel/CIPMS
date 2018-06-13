using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///PRINT_CARTONINFORMATION_GET 的摘要说明
/// </summary>
namespace Model
{
    public partial class PRINT_CARTONINFORMATION_GET
    {
        public string CARTON_PART { get; set; }
        public string OUT_CARTON_PART { get; set; }
        public string TTLBUNDLENO { get; set; }
        public string CIPMS_PROCESS_CD { get; set; }
        public string CIPMS_PRODUCTION_LINE_CD { get; set; }
        public string CUT_LINE { get; set; }
        public string PRODUCTION_LINE_CD { get; set; }
        public string GARMENT_ORDER_NO { get; set; }
        public string SHORT_NAME { get; set; }
    }
}