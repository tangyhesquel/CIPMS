using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// WipAndOutputData 的摘要说明
/// </summary>
/// 
namespace Model
{
    public partial class WipAndOutputData
    {
        public WipAndOutputData()
        {
        }
        public string CUT_LINE { get; set; }
        public string PRODUCTION_LINE_CD { get; set; }
        public string JOB_ORDER_NO { get; set; }
        public string COLOR_CD { get; set; }
        public int ORDER_QTY { get; set; }
        public int CUT_QTY_TODAY { get; set; }
        public int CUT_QTY_TOTAL { get; set; }
        public int REDUCE_QTY_TODAY { get; set; }
        public int REDUCE_QTY_TOTAL { get; set; }
        public int CUT_OUT_TODAY { get; set; }
        public int CUT_OUT_TOTAL { get; set; }
        public int CUT_OUT_WIP { get; set; }
        public int PRT_IN_TODAY { get; set; }
        public int PRT_IN_TOTAL { get; set; }
        public int PRT_OUT_TODAY { get; set; }
        public int PRT_OUT_TOTAL { get; set; }
        public int PRT_IN_OUT_WIP { get; set; }
        public int EMB_OUT_TODAY { get; set; }
        public int EMB_OUT_TOTAL { get; set; }
        public int EMB_OUT_WIP { get; set; }
        public int FUSING_OUT_TODAY { get; set; }
        public int FUSING_OUT_TOTAL { get; set; }
        public int FUSING_OUT_WIP { get; set; }
        public int MATCHING_OUT_TODAY { get; set; }
        public int MATCHING_OUT_TOTAL { get; set; }
        public int MATCHING_OUT_WIP { get; set; }
        public int DC_BEFORE_WIP { get; set; }
        public int DC_SEW_S_TOTAL { get; set; }
        public int DC_SEW_C { get; set; }
        public int DC_SEW_C_TOTAL { get; set; }
        public int TWIP { get; set; }
        public double DCT { get; set; }
        public int CUT_QTY { get; set; }
        public int SEQ { get; set; }

    }
}