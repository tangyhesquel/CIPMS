using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;
using NameSpace;
using System.Data;

public partial class Reduce : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod]
    public static String layno(string factory, string svTYPE, string joborderno)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Packagesql packagesql = new Packagesql();
        Reducesql reducesql = new Reducesql();
        SqlDataReader sqlDr;
        int i = 0, j = 0;
        string laynohtml = "", bundlehtml = "";
        //获取layno
        sqlDr = packagesql.layno(sqlCon, joborderno);
        while (sqlDr.Read())
        {
            laynohtml += "<option value='" + sqlDr["LAY_NO"] + "'>" + sqlDr["LAY_NO"] + "</option>";
            i++;
        }
        sqlDr.Close();
        //获取bundleno
        sqlDr = reducesql.getbundleno(sqlCon, joborderno);
        while (sqlDr.Read())
        {
            bundlehtml += "<option value='" + sqlDr["BUNDLE_NO"] + "'>" + sqlDr["BUNDLE_NO"] + "</option>";
            j++;
        }
        sqlDr.Close();
        if (i == 0 || j == 0)
        {
            return "false1";
        }
        sqlCon.Close();
        string result = "[{ \"laynohtml\": \"" + laynohtml + "\", \"bundlehtml\": \"" + bundlehtml + "\" }]";
        return result;
    }

    [WebMethod]
    public static String bundleno(string factory, string svTYPE, string joborderno, string layno)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Packagesql packagesql = new Packagesql();
        Reducesql reducesql = new Reducesql();
        SqlDataReader sqlDr;
        int i = 0;
        string bundlehtml = "";
        //获取bundleno
        sqlDr = reducesql.getbundleno(sqlCon, joborderno, layno);
        while (sqlDr.Read())
        {
            bundlehtml += "<option value='" + sqlDr["BUNDLE_NO"] + "'>" + sqlDr["BUNDLE_NO"] + "</option>";
            i++;
        }
        sqlDr.Close();
        if (i == 0 )
        {
            return "false1";
        }
        sqlCon.Close();
        string result = "[{ \"bundlehtml\": \"" + bundlehtml + "\" }]";
        return result;
    }

    [WebMethod]
    public static String Minwip(string userbarcode, string factory, string svTYPE, string process, string barcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        Transactionsql transfersql = new Transactionsql();
        SqlDataReader sqlDr;
        string minqty = "0";
        string isprocess = "true";
        //找到该bundle的所有part所在的部门以及的最小WIP
        int i = 0, j = 0;
        sqlDr = sqlstatement.minqty(sqlCon, barcode);
        string sameprocess = "";
        while (sqlDr.Read())
        {
            if (sameprocess != sqlDr["PROCESS_CD"].ToString())
            {
                sameprocess = sqlDr["PROCESS_CD"].ToString();
                i++;
            }
            //if (sqlDr["PROCESS_CD"].ToString() != "DC")
                //i = 1;
            minqty = sqlDr["MINQTY"].ToString();
            j++;
        }
        sqlDr.Close();

        //UPDATE 2015/11/12 15:00 BY JACOB
        if (i > 1 || j == 0 || sameprocess != process)
            isprocess = "false";
        //if (process != "DC")
            //isprocess = "false";

        //所有的裁片都非提交状态
        string issubmit = "false";
        sqlDr = transfersql.checkbundlesubmit(sqlCon, barcode);
        if (sqlDr.Read())
            issubmit = "true";
        sqlDr.Close();

        sqlCon.Close();
        return "[{\"WIP\":\"" + minqty + "\", \"ISSUBMIT\": \"" + issubmit + "\", \"ISPROCESS\":\"" + isprocess + "\"}]";
    }

    [WebMethod]
    public static String Minwip2(string userbarcode, string factory, string svTYPE, string process, string joborderno, string bundleno)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        Transactionsql transfersql = new Transactionsql();
        Reducesql reducesql = new Reducesql();
        SqlDataReader sqlDr;
        string minqty = "0";
        string isprocess = "true";

        string barcode = "";
        sqlDr = reducesql.getbarcode(sqlCon, joborderno, bundleno);
        if(sqlDr.Read())
        {
            if(sqlDr["BARCODE"].ToString().Length != 14)
            {
                sqlCon.Close();
                return "false7";
            }
            else
                barcode = sqlDr["BARCODE"].ToString();
        }
        else
        {
            sqlCon.Close();
            return "false8";
        }
        sqlDr.Close();
        //找到该bundle的所有part所在的部门以及的最小WIP
        int i = 0, j = 0;
        sqlDr = sqlstatement.minqty(sqlCon, barcode);
        string sameprocess = "";
        while (sqlDr.Read())
        {
            if (sameprocess != sqlDr["PROCESS_CD"].ToString())
            {
                sameprocess = sqlDr["PROCESS_CD"].ToString();
                i++;
            }
            //if (sqlDr["PROCESS_CD"].ToString() != "DC")
            //i = 1;
            minqty = sqlDr["MINQTY"].ToString();
            j++;
        }
        sqlDr.Close();
        if (i > 1 || j == 0 || sameprocess != process)
            isprocess = "false";
        //if (process != "DC")
        //isprocess = "false";

        //所有的裁片都非提交状态
        string issubmit = "false";
        sqlDr = transfersql.checkbundlesubmit(sqlCon, barcode);
        if (sqlDr.Read())
            issubmit = "true";
        sqlDr.Close();

        sqlCon.Close();
        return "[{ \"WIP\":\"" + minqty + "\", \"BARCODE\": \""+barcode+"\", \"ISSUBMIT\": \"" + issubmit + "\", \"ISPROCESS\":\"" + isprocess + "\" }]";
    }

    [WebMethod]
    public static String Defectinformation(string userbarcode, string factory, string svTYPE, string process, string barcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;
        string result = "";
        sqlDr = sqlstatement.defectinformation(sqlCon, barcode);
        while (sqlDr.Read())
        {
            result += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
            result += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PROCESS_CD"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center; width:30%;'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["BUNDLE_NO"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PART_DESC"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center; width:30%;'>" + sqlDr["REASON_DESC"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT_QTY"] + "</td></tr>";
        }
        sqlDr.Close();
        sqlCon.Close();
        if (result == "")
            return "null";
        else
            return result;
    }

    [WebMethod]
    public static String Reduceinformation(string userbarcode, string factory, string svTYPE, string process, string barcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;
        string result = "";
        sqlDr = sqlstatement.reduceinformation(sqlCon, barcode);
        while (sqlDr.Read())
        {
            result += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
            result += "<td style='vertical-align:middle; text-align:center; width:30%;'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["COLOR_CD"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["SIZE_CD"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["BUNDLE_NO"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center; width:30%;'>" + sqlDr["REASON_DESC"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["REDUCE_QTY"] + "</td></tr>";
        }
        sqlDr.Close();
        sqlCon.Close();
        if (result == "")
            return "null";
        else
            return result;
    }

    [WebMethod]
    public static String Confirm(string factory, string svTYPE, string userbarcode, string cipmsprocess, string barcode, string reduceqty, string reducereason)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlCommand cmd;

        int ReduceQty = int.Parse(reduceqty);
        string process = "CUT";
        try
        {
            cmd = new SqlCommand("CIPMS_SP_GTN_REDUCE", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@BARCODE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@REDUCEQTY", SqlDbType.Decimal);
            cmd.Parameters.Add("@REASONCD", SqlDbType.NChar);
            cmd.Parameters.Add("@USERBARCODE", SqlDbType.NChar);
            cmd.Parameters.Add("@FACTORY", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@CANREDUCE", SqlDbType.NChar, 1);
            cmd.Parameters["@CANREDUCE"].Direction = ParameterDirection.Output;
            cmd.Parameters["@BARCODE"].Value = barcode;
            cmd.Parameters["@REDUCEQTY"].Value = ReduceQty;
            cmd.Parameters["@REASONCD"].Value = reducereason;
            cmd.Parameters["@USERBARCODE"].Value = userbarcode;
            cmd.Parameters["@FACTORY"].Value = factory;
            cmd.Parameters["@PROCESS"].Value = process;
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            return "false" + ex.Message;
        }
        finally
        {
            sqlCon.Close();
        }

        return "success";
    }
}
