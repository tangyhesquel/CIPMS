using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///GROUP_WIP_BYJO 的摘要说明
/// </summary>
namespace Model
{
    public class GROUP_WIP_BYJO
    {
        public int SEQ { get; set; }
        public string CUT_LINE { get; set; }
        public string JOB_ORDER_NO { get; set; }
        public string COLOR_CD { get; set; }
        public string SIZE_CD { get; set; }
        public string BUNDLE_NO { get; set; }
        public string PART_DESC { get; set; }
        public int ORDER_QTY { get; set; }
        public int CUT_QTY { get; set; }
        public int WIP { get; set; }
        public string SEW_LINE { get; set; }
    }
}