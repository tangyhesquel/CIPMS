using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///DOCNOLISTDETAIL_INQUIRY 的摘要说明
/// </summary>
namespace Model
{
	public partial class DOCNOLISTDETAIL_INQUIRY
	{
        public string JOB_ORDER_NO { get; set; }
        public string COLOR_CD { get; set; }
        public string SIZE_CD { get; set; }
        public string LAY_NO { get; set; }
        public string BUNDLE_NO { get; set; }
        public string PART_DESC { get; set; }
        public string QTY { get; set; }
        public string BARCODE { get; set; }
        public string CARTON_BARCODE { get; set; }
        public string CARTON_STATUS { get; set; }
	}
}