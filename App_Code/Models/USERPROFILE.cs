using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///USERPROFILE 的摘要说明
/// </summary>
namespace Model
{
    public partial class USERPROFILE
    {
        public string USER_BARCODE { get; set; }
        public string EMPLOYEE_NO { get; set; }
        public string NAME { get; set; }
        public string FACTORY_CD { get; set; }
        public string PRC_CD { get; set; }
        public string PRODUCTION_LINE_CD { get; set; }
        public string SHIFT { get; set; }
        public string DEFAULTMODULE { get; set; }
        public string FUNCTION_ENG { get; set; }
        public string FUNCTION_CHS { get; set; }
        public string MODULE_CD { get; set; }
    }
}