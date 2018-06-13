using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///Docno_Print 的摘要说明
/// </summary>
namespace Model
{
    public partial class Docno_Print
    {
        public string user { get; set; }
        public string datetime { get; set; }
        public string fromdept { get; set; }
        public string fromline { get; set; }
        public string toline { get; set; }
        public string parts { get; set; }
        public string remark { get; set; }
        public string customer { get; set; }
        public string summary { get; set; }
        public string jo { get; set; }
        public string doc { get; set; }
        public string color { get; set; }
        public string layno { get; set; }
        public string bundle { get; set; }
        public string totalqty { get; set; }
        public string totalcutqty { get; set; }
        public string currenttotalqty { get; set; }
        public string totaloutputqty { get; set; }
        public string residualqty { get; set; }
        public string cartonnum { get; set; }
        public string automatching { get; set; }
        public string print_bundlenum { get; set; }//总扎数/菲数
        public string print_carton_no { get; set; }//箱号
        public string print_bundle_barcode { get; set; }//裁片菲码
        public string print_part { get; set; }//幅位
        public string print_color { get; set; }//布色
        public string print_lay { get; set; }//床次
        public string print_bundleno { get; set; }//扎号
        public string print_size { get; set; }
        public string print_qty { get; set; }
        public string print_untity { get; set; }

    }
}