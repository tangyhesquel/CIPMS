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

public partial class Barcode_Information_Inquiry : System.Web.UI.Page
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
    public static String GetBarcodeInformationDetail(string factory, string svTYPE, string GO, string JO, string COLOR, string LAYNO, string BUNDLENO,string BARCODE)
    {
        BARCODEINQUIRYBLL barcodeinquirybll = new BARCODEINQUIRYBLL(factory, svTYPE);
        return barcodeinquirybll.GetBarcodeInformationDetail(GO, JO, COLOR, LAYNO, BUNDLENO, BARCODE);
    }

    //打印扎码或者箱码或者流水单
    [WebMethod]
    public static String PrintBarcode(string factory, string svTYPE, string BARCODE, string LANGUAGE)
    {
        COMMONBLL commonbll = new COMMONBLL(factory, svTYPE);
        return commonbll.PrintBarcode(BARCODE, LANGUAGE);
    }

    //根据JO获取颜色床次扎号列表
    [WebMethod]
    public static String GetColorLaynoBundleByJO(string factory, string svTYPE, string JO)
    {
        BARCODEINQUIRYBLL barcodeinquirybll = new BARCODEINQUIRYBLL();
        return barcodeinquirybll.GetColorLaynoBundleByJO(factory, svTYPE, JO);
    }

    //获取制单列表
    [WebMethod]
    public static String GetJO(string factory, string svTYPE)
    {
        BARCODEINQUIRYBLL barcodeinquirybll = new BARCODEINQUIRYBLL();
        return barcodeinquirybll.GetJO(factory, svTYPE);
    }

    //根据GO获取制单列表
    [WebMethod]
    public static String GetJOByGO(string factory, string svTYPE, string GO)
    {
        BARCODEINQUIRYBLL barcodeinquirybll = new BARCODEINQUIRYBLL();
        return barcodeinquirybll.GetJO(factory, svTYPE, GO);
    }

    //获取GO列表
    [WebMethod]
    public static String GetGO(string factory, string svTYPE)
    {
        BARCODEINQUIRYBLL barcodeinquirybll = new BARCODEINQUIRYBLL();
        return barcodeinquirybll.GetGO(factory, svTYPE);
    }



    //根据JO/LAYNO/COLOR获取BUNDLENO列表
    [WebMethod]
    public static String GetBundlenoByJOColorLayno(string factory, string svTYPE, string JO, string COLOR, string LAYNO)
    {
        BARCODEINQUIRYBLL barcodeinquirybll = new BARCODEINQUIRYBLL();
        return barcodeinquirybll.GetBundleByJOColorLayno(factory, svTYPE, JO, COLOR, LAYNO);
    }
}