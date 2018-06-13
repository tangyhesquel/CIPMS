using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///FeizhiModel 的摘要说明
/// </summary>
namespace Model
{
    public partial class BUNDLE_INFO
    {
        public string JOB_ORDER_NO { get; set; }
        public string BUNDLE_BARCODE { get; set; }
        public string COLOR { get; set; }
        public string SIZE { get; set; }
        public string BUNDLE_NO { get; set; }
        public string QTY { get; set; }
        public string CUT_QTY { get; set; }
        public string CLIENT { get; set; }
        public string STYLE_NO { get; set; }
        public string BATCH_NO { get; set; }
        public string PRODUCTION_LINE { get; set; }
        public string MARKERS { get; set; }
        public string PART { get; set; }
        public string LAY_NO { get; set; }
        public string DATE { get; set; }
        public string SHADE_LOT { get; set; }
        public string PATTERN_NO { get; set; }
        public string PRINTPART_FLAG { get; set; }
    }
}