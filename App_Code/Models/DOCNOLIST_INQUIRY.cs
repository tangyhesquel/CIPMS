using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///DOCNOLIST_INQUIRY 的摘要说明
/// </summary>
namespace Model
{
    public partial class DOCNOLIST_INQUIRY
    {
        public string DOC_NO { get; set; }
        public string CREATE_DATE { get; set; }
        public string SUBMIT_DATE { get; set; }
        public string CONFIRM_DATE { get; set; }
        public string PROCESS_PRODUCTION_FACTORY { get; set; }
        public string PROCESS_CD { get; set; }
        public string PRODUCTION_LINE_CD { get; set; }
        public string NEXT_PROCESS_PRODUCTION_FACTORY { get; set; }
        public string NEXT_PROCESS_CD { get; set; }
        public string NEXT_PRODUCTION_LINE_CD { get; set; }
        public string STATUS { get; set; }
        public int QTY { get; set; }
        public string SELECTED { get; set; }
    }
}