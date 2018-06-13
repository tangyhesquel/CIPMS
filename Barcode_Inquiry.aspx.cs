using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;
using System.Data;
using System.IO;

public partial class Barcode_Inquiry : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    //获取用户信息
    [WebMethod]
    public static String GetUserProfile(string factory, string svTYPE, string userbarcode)
    {
        COMMONBLL commonbll = new COMMONBLL(factory, svTYPE);
        return commonbll.GetUserProfile(userbarcode);
    }

    //获取流水单列表
    [WebMethod]
    public static String GetDOCNOList(string factory, string svTYPE, string DOC_NO, string JOB_ORDER_NO, string STATUS, string SENDPROCESS, string RECEIVEPROCESS, string CREATEDATEFROM, string CREATEDATETO)
    {
        BARCODEINQUIRYBLL barcodeinquirybll = new BARCODEINQUIRYBLL(factory, svTYPE);
        return barcodeinquirybll.GetDOCNOList(DOC_NO, JOB_ORDER_NO, STATUS, SENDPROCESS, RECEIVEPROCESS, CREATEDATEFROM, CREATEDATETO);
    }

    //获取流水单详情
    [WebMethod]
    public static String GetDOCNODetailList(string factory, string svTYPE, string DOC_NO, string BYPART)
    {
        BARCODEINQUIRYBLL barcodeinquirybll = new BARCODEINQUIRYBLL(factory, svTYPE);
        return barcodeinquirybll.GetDOCNODetailList(DOC_NO, BYPART);
    }

    //获取部门列表
    [WebMethod]
    public static String GetProcessList(string factory, string svTYPE, string garmenttype)
    {
        COMMONBLL commonbll = new COMMONBLL(factory, svTYPE);
        return commonbll.GetProcessList(factory, garmenttype);
    }

    //如果接收部门或者发送部门是SEW，则detail不显示part；
    //否则detail显示part
}