using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///GROUP_WIP_BYLINE 的摘要说明
/// </summary>
namespace Model
{
    public partial class GROUP_WIP_BYLINE
    {
        public int SEQ { get; set; }
        public string PROCESS_CD { get; set; }
        public string CUT_LINE { get; set; }
        public string SEW_LINE { get; set; }
        public int WIP { get; set; }
    }
}