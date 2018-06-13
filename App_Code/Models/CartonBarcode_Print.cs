using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///CartonBarcodePrintModel 的摘要说明
/// </summary>
namespace Model
{
    public partial class CartonBarcode_Print
    {
        public string customer { get; set; }
        public string date { get; set; }
        public string process { get; set; }
        public string production { get; set; }
        public string sewline { get; set; }
        public string ttlbundle { get; set; }
        public string shortpart { get; set; }
        public string part { get; set; }
        public string summary { get; set; }
        public string jo { get; set; }
        public string color { get; set; }
        public string cutqty { get; set; }
        public string layno { get; set; }
        public string bundle { get; set; }
        public string totalqty { get; set; }
        public string cartonbarcode { get; set; }
    }
}