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

public partial class Receive2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod]
    public static String Part(string factory, string svTYPE)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;
        string result = "";
        sqlDr = sqlstatement.part(sqlCon, factory);
        while (sqlDr.Read())
        {
            result += "<option value='" + sqlDr["PART_CD"] + "'>" + sqlDr["PART_DESC"] + "</option>";
        }
        return result;
    }

    [WebMethod]
    public static String Userinformation(string factory, string svTYPE, string userbarcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;
        string result = "";
        sqlDr = sqlstatement.userinformation(sqlCon, userbarcode);
        if (sqlDr.Read())
            result += "[{\"NAME\":\""+sqlDr["NAME"]+"\", \"PROCESS\":\""+sqlDr["PRC_CD"]+"\", \"PRODUCTION\":\""+sqlDr["PRODUCTION_LINE_CD"]+"\"}]";
        sqlDr.Close();
        sqlCon.Close();
        if (result == "")
            return "null";
        else
            return result;
    }

    [WebMethod]
    public static String Cartonbarcodescan(string factory, string svTYPE, string process, string userbarcode, string barcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;
        //判断该CARTON BARCODE是否在本部门并且为关箱状态
        sqlDr = sqlstatement.bundleishere(sqlCon, barcode);
        if (sqlDr.Read())
        {
            if (process != sqlDr["PROCESS_CD"].ToString())
            {
                string prc_cd = sqlDr["PROCESS_CD"].ToString();
                sqlCon.Close();
                return "false1"+prc_cd;//不在本道部门
            }
            if (sqlDr["CARTON_STATUS"].ToString() != "C")
            {
                sqlCon.Close();
                return "false2";//该箱为空
            }
        }
        else
        {
            sqlCon.Close();
            return "false3";//不存在该CARTON BARCODE
        }
        sqlDr.Close();

        //判断该barcode part是否已经被别人扫描了
        string temp = "false";
        List<string> who = new List<string>();
        sqlDr = sqlstatement.havebeenscanned(sqlCon, barcode);
        while (sqlDr.Read())
            if (sqlDr["EMPLOYEE_OUTPUT"].ToString() != "0")
            {
                temp = "true";
                who.Add(sqlDr["EMPLOYEE_OUTPUT"].ToString());
            }
        sqlDr.Close();
        if (temp == "true")
        {
            List<string> nlist = who.Distinct().ToList();
            string result1 = "";
            int j = 0;
            foreach (string n in nlist)
            {
                sqlDr = sqlstatement.whoscanned(sqlCon, n);
                if (sqlDr.Read())
                {
                    if (j == 0)
                        result1 = sqlDr["NAME"].ToString();
                    else
                        result1 = "," + sqlDr["NAME"].ToString();
                    j++;
                }
                sqlDr.Close();
            }
            sqlCon.Close();
            return "false4" + result1;
        }

        //根据barcode part查找出其SKU
        string result = "";
        int output = 0;
        int sumoutput = 0;
        int sumbundle = 0;
        sqlDr = sqlstatement.bundleinformation(sqlCon, barcode);
        while (sqlDr.Read())
        {
            output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
            sumoutput += output;
            sumbundle++;
            result += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center'>" + sqlDr["CARTON_BARCODE"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["BARCODE"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["PART_CD"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["BUNDLE_NO"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["LAY_NO"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["SIZE_CD"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["COLOR_CD"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["QTY"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + output.ToString() + "</td></tr>";
        }
        sqlDr.Close();
        //更新wip_bundle表里的user字段
        sqlstatement.updatewipbundleuser(sqlCon, barcode, userbarcode);
        sqlCon.Close();
        return "[{\"HTML\":\""+result+"\", \"OUT_QTY\":\""+sumoutput.ToString()+"\", \"BUNDLE\":\""+sumbundle.ToString()+"\"}]";
    }

    [WebMethod]
    public static String Bundlebarcodescan(string factory, string svTYPE, string process, string userbarcode, string barcode, string part)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;
        //判断该barcode part是否在本部门
        sqlDr = sqlstatement.bundleishere(sqlCon, barcode, part);
        if (sqlDr.Read())
        {
            if (process != sqlDr["PROCESS_CD"].ToString())
            {
                string prc_cd = sqlDr["PROCESS_CD"].ToString();
                sqlCon.Close();
                return "false1" + prc_cd;//不在本道部门
            }
        }
        else
        {
            sqlCon.Close();
            return "false2";//不存在该barcode part
        }
        sqlDr.Close();
        //判断该barcode part是否已经被别人扫描了
        string temp = "false";
        List<string> who = new List<string>();
        sqlDr = sqlstatement.havebeenscanned(sqlCon, barcode, part);
        while (sqlDr.Read())
            if (sqlDr["EMPLOYEE_OUTPUT"].ToString() != "0")
            {
                temp = "true";
                who.Add(sqlDr["EMPLOYEE_OUTPUT"].ToString());
            }
        sqlDr.Close();
        if (temp == "true")
        {
            List<string> nlist = who.Distinct().ToList();
            string result1 = "";
            int j = 0;
            foreach (string n in nlist)
            {
                sqlDr = sqlstatement.whoscanned(sqlCon, n);
                if (sqlDr.Read())
                {
                    if(j == 0)
                        result1 = sqlDr["NAME"].ToString();
                    else
                        result1 = "," + sqlDr["NAME"].ToString();
                    j++;
                }
                sqlDr.Close();
            }
            sqlCon.Close();
            return "false3" + result1;
        }
        //根据barcode part查找出其SKU
        string result = "";
        int output = 0;
        sqlDr = sqlstatement.bundleinformation(sqlCon, barcode, part);
        if (sqlDr.Read())
        {
            var carton_barcode = "";
            if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
            output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
            result += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center'>" + carton_barcode + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["BARCODE"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["PART_CD"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["BUNDLE_NO"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["LAY_NO"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["SIZE_CD"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["COLOR_CD"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["QTY"] + "</td>";
            result += "<td style='vertical-align:middle; text-align:center'>" + output.ToString() + "</td></tr>";
        }
        sqlDr.Close();
        //更新wip_bundle表里的user字段
        sqlstatement.updatewipbundleuser(sqlCon, barcode, part, userbarcode);
        sqlCon.Close();
        return "[{\"HTML\":\"" + result + "\", \"OUT_QTY\":\"" + output.ToString() + "\"}]";
    }
}
