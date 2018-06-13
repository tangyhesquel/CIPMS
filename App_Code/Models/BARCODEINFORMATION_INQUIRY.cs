using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///BARCODEINFORMATION_INQUIRY 的摘要说明
/// </summary>
namespace Model
{
    public partial class BARCODEINFORMATION_INQUIRY
    {
        public string FACTORY_CD { get; set; }
        public string PROCESS_CD { get; set; }
        public string PRODUCTION_LINE_CD { get; set; }
        public string CUT_LINE { get; set; }
        public string GARMENT_ORDER_NO { get; set; }
        public string JOB_ORDER_NO { get; set; }
        public string COLOR_CD { get; set; }
        public string SIZE_CD { get; set; }
        public string LAY_NO { get; set; }
        public string BUNDLE_NO { get; set; }
        public string BARCODE { get; set; }
        public string PART_CD { get; set; }
        public string PART_DESC { get; set; }
        public string QTY { get; set; }
        public string DISCREPANCY_QTY { get; set; }
        public string CARTON_BARCODE { get; set; }
        public string CARTON_STATUS { get; set; }
        public string DOC_NO { get; set; }
        public string WIP { get; set; }
        public string IS_MATCHING { get; set; }
        public string SEW_LINE { get; set; }
    }
}