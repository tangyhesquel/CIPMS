using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///WIP_BUNDLE_DETAIL 的摘要说明
/// </summary>
namespace Model
{
    public partial class WIP_BUNDLE_DETAIL
    {
        public int SEQ { get; set; }
        public string JOB_ORDER_NO { get; set; }
        public string COLOR_CD { get; set; }
        public string SIZE_CD { get; set; }
        public string BUNDLE_NO { get; set; }
        public string PART_DESC { get; set; }
        public string CUT_LINE { get; set; }
        public string SEW_LINE { get; set; }
        public int CUT { get; set; }
        public int PRT { get; set; }
        public int EMB { get; set; }
        public int FUSE { get; set; }
        public int MATCHING { get; set; }
        public int DC { get; set; }
        public int SEW { get; set; }

        public int ORDER_QTY { get; set; }
        public int CUT_QTY { get; set; }
        public int REDUCE_QTY { get; set; }
        public int ACTUAL_CUT_QTY { get; set; }
        public int TOSEW { get; set; }
    }

}