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
using System.IO;
using Code;
using System.Text.RegularExpressions;

public partial class Input : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod]
    public static String Empty(string factory, string svTYPE, string userbarcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement commonsql = new Sqlstatement();
        commonsql.emptylist(sqlCon, userbarcode);
        sqlCon.Close();
        return "success";
    }

    [WebMethod]
    public static String Part(string factory, string svTYPE)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        SqlDataReader sqlDr;
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlCon;
        sqlComGet.CommandText = "select PART_CD,PART_DESC from CIPMS_PART_MASTER";
        sqlDr = sqlComGet.ExecuteReader();
        string result = "";
        while (sqlDr.Read())
        {
            result += "<option value='" + sqlDr["PART_CD"] + "'>" + sqlDr["PART_DESC"] + "</option>";
        }
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Delete(string docno, string userbarcode, string factory, string svTYPE, string jo, string bundle, string part)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Bundleinputsql bundleinputsql = new Bundleinputsql();
        bundleinputsql.deletescan(sqlCon, docno, userbarcode, jo, bundle, part);
        return "success";
    }

    [WebMethod]
    public static String Print(string factory, string svTYPE, string process, string docno, string userbarcode)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Bundleinputsql bundleinputsql = new Bundleinputsql();
        SqlDataReader sqlDr;
        //判断在BUNDLE_FOR_SCANNING是否已经存在该JO,BUNDLE,PART，提示用户
        string message1 = "";
        int i = 0;
        sqlDr = bundleinputsql.checkcipmsbundle(sqlCon, docno, userbarcode);
        while (sqlDr.Read())
        {
            if (i == 0)
                message1 += sqlDr["SEQ_ID"].ToString();
            else
                message1 += "," + sqlDr["SEQ_ID"].ToString();
        }
        sqlDr.Close();
        string message2 = "";
        i = 0;
        sqlDr = bundleinputsql.checkcipmsbarcode(sqlCon, docno, userbarcode);
        while (sqlDr.Read())
        {
            if (i == 0)
                message2 += sqlDr["SEQ_ID"].ToString();
            else
                message2 += "," + sqlDr["SEQ_ID"].ToString();
        }
        sqlDr.Close();
        if (message1 != "" && message2 != "")
        {
            result = "[{ \"MESSAGE1\":\""+message1+"\", \"MESSAGE2\":\""+message2+"\" }]";
            sqlCon.Close();
            return result;
        }
        else if (message1 != "" && message2 == "")
        {
            result = "[{ \"MESSAGE1\":\""+message1+"\", \"MESSAGE2\":\"null\" }]";
            sqlCon.Close();
            return result;
        }
        else if (message1 == "" && message2 != "")
        {
            result = "[{ \"MESSAGE1\":\"null\", \"MESSAGE2\":\"" + message2 + "\" }]";
            sqlCon.Close();
            return result;
        }
        else
        {
            //插入裁片信息
            bundleinputsql.insertcipmsbundle(sqlCon, docno, userbarcode, factory, process, "K");
            bundleinputsql.insertcipmswiphd(sqlCon, docno, userbarcode, factory, process, "K");
            bundleinputsql.insertcipmswipbundle(sqlCon, docno, userbarcode, process, "K");
            //清空扫描表
            Sqlstatement commonsql = new Sqlstatement();
            commonsql.emptylist(sqlCon, userbarcode);
        }
        sqlCon.Close();
        return "success";
    }

    [WebMethod]
    public static String Confirm1(string userbarcode, string factory, string svTYPE, string process, string production, string productionselected, string joborderno, string bundlebarcode, string color, string size, string qty, string part)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        SqlDataReader sqlDr;
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlCon;
        //判断bundle barcode是否已经存在
        sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING where BARCODE='" + bundlebarcode + "'";
        sqlDr = sqlComGet.ExecuteReader();
        if (sqlDr.Read())
        {
            sqlDr.Close();
            sqlCon.Close();
            return "false";
        }
        sqlDr.Close();

        //不存在则insert到cipms_bundle_for_scanning
        if (production == productionselected)
            production = "NA";
        string sql = "insert into CIPMS_BUNDLE_FOR_SCANNING (FACTORY_CD,PROCESS_CD,PRODUCTION_LINE_CD,JOB_ORDER_NO,SIZE_CD,COLOR_CD,BARCODE,PART_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_BARCODE,CARTON_STATUS,DOC_NO,GARMENT_TYPE,PROCESS_TYPE,USER_CREATE_ID,CREATE_DATE) values ('" + factory + "','" + process + "','" + production + "','" + joborderno + "','" + size + "','" + color + "','" + bundlebarcode + "','" + part + "'," + qty + ",0,0,'0','O','0','K','I','" + userbarcode + "',GETDATE())";
        SqlCommand cmd = new SqlCommand(sql, sqlCon);
        cmd.ExecuteNonQuery();

        //插入到WIP_HD
        sql = "insert into CIPMS_JO_WIP_HD (FACTORY_CD,JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PROCESS_CD,PRODUCTION_LINE_CD,PROCESS_TYPE,PART_CD,IN_QTY,OUT_QTY,DISCREPANCY_QTY,WIP,INTRANS_QTY,INTRANS_IN,INTRANS_OUT,BUNDLE_REDUCE,MATCHING,GARMENT_TYPE,PRODUCTION_FACTORY)select '" + factory + "',a.JOB_ORDER_NO,a.COLOR_CD,a.SIZE_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,'I',a.PART_CD,0,0,0,0,0,0,0,0,0,'K','" + factory + "' from (select JOB_ORDER_NO,COLOR_CD,SIZE_CD,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD from CIPMS_BUNDLE_FOR_SCANNING where BARCODE='" + bundlebarcode + "' and PART_CD='" + part + "' except select JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD from CIPMS_JO_WIP_HD)as a";
        cmd = new SqlCommand(sql, sqlCon);
        cmd.ExecuteNonQuery();
        sql = "update CIPMS_JO_WIP_HD set WIP=a.WIP+b.QTY,IN_QTY=a.IN_QTY+b.QTY from CIPMS_JO_WIP_HD as a, (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD,c.QTY from CIPMS_BUNDLE_FOR_SCANNING as c where c.BARCODE='" + bundlebarcode + "' and c.PART_CD='" + part + "')as b where a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CODE=b.COLOR_CD and a.SIZE_CODE=b.SIZE_CD and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD";
        cmd = new SqlCommand(sql, sqlCon);
        cmd.ExecuteNonQuery();

        //插入到WIP_BUNDLE
        sql = "insert into CIPMS_JO_WIP_BUNDLE (STOCK_ID,IN_QTY,OUT_QTY,WIP,INTRANS_QTY,DISCREPANCY_QTY,MATCHING,DEFECT,TRANSFER_IN,TRANSFER_OUT,PART_CD,EMPLOYEE_OUTPUT) select a.STOCK_ID,b.QTY-b.DISCREPANCY_QTY,0,b.QTY-b.DISCREPANCY_QTY,0,0,0,0,0,0,b.PART_CD,'0' from CIPMS_JO_WIP_HD as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CODE=b.COLOR_CD and a.SIZE_CODE=b.SIZE_CD and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD where b.BARCODE='" + bundlebarcode + "' and b.PART_CD='" + part + "'";
        cmd = new SqlCommand(sql, sqlCon);
        cmd.ExecuteNonQuery();
        sqlCon.Close();
        return "success";
    }

    [WebMethod]
    public static String Confirm(string seq, string factory, string svTYPE, string userbarcode, string docno, string jo, string bundle, string color, string size, string part, string qty, string layno, string dbtrans)
    {
        string html = "";
        string[] partarray = part.Split(new char[] { ',' });
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Bundleinputsql bundleinputsql = new Bundleinputsql();
        SqlDataReader sqlDr;
        string date = DateTime.Today.ToString("yyMMdd");
        int j = int.Parse(seq) + 1;
        foreach (string m in partarray)
        {
            string check1 = "true", check2 = "true";
            sqlDr = bundleinputsql.checkexists1(sqlCon, userbarcode, docno, jo, bundle, m);
            if (sqlDr.Read())
                check1 = "false";
            sqlDr.Close();
            sqlDr = bundleinputsql.checkexists2(sqlCon, jo, bundle, m);
            if (sqlDr.Read())
                check2 = "false";
            sqlDr.Close();
            if (check1 == "true" && check2 == "true")//不存在，则插入到历史扫描表并得到新的条码并插入到html字符串
            {
                //调用存储过程获取新的条码
                SqlCommand cmd = new SqlCommand("CIPMS_BUNDLE_BARCODE_CREATE", sqlCon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@FACTORY", SqlDbType.NVarChar);
                cmd.Parameters.Add("@BARCODETYPE", SqlDbType.NVarChar);
                cmd.Parameters.Add("@DATE", SqlDbType.NVarChar);
                cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
                cmd.Parameters.Add("@NEW_BUNDLE_BARCODE", SqlDbType.NVarChar, 30);
                cmd.Parameters["@NEW_BUNDLE_BARCODE"].Direction = ParameterDirection.Output;
                cmd.Parameters["@DATE"].Value = date;
                cmd.Parameters["@USERBARCODE"].Value = userbarcode;
                cmd.Parameters["@FACTORY"].Value = factory.Substring(0, 3);
                cmd.Parameters["@BARCODETYPE"].Value = "prtbarcode";
                cmd.ExecuteNonQuery();
                string newbundlebarcode = cmd.Parameters["@NEW_BUNDLE_BARCODE"].Value.ToString();
                //插入到扫描历史表
                bundleinputsql.insertscanlist(sqlCon, j.ToString(), userbarcode, docno, newbundlebarcode, m, jo, layno,bundle, color, size, qty, "externalinput");
                html += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center'>" + j + "</td><td style='vertical-align:middle; text-align:center'>" + jo + "</td><td style='vertical-align:middle; text-align:center'>" + bundle + "</td><td style='vertical-align:middle; text-align:center'>" + m + "</td><td style='vertical-align:middle; text-align:center'>" + layno + "</td><td style='vertical-align:middle; text-align:center'>" + color + "</td><td style='vertical-align:middle; text-align:center'>" + size + "</td><td style='vertical-align:middle; text-align:center'>" + qty + "</td><td style='vertical-align:middle; text-align:center'>" + newbundlebarcode + "</td><td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></tr>";
                j++;
            }
        }
        sqlCon.Close();
        if (j == 0)
        {
            return "false";
        }
        return html;
    }

    [WebMethod]
    public static String Checkdata(string seq, string factory, string svTYPE, string userbarcode, string docno, string jsondata, string part, string dbtrans)
    {
        string result = "";
        string html = "";
        string[] partarray = part.Split(new char[] { ',' });
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Bundleinputsql bundleinputsql = new Bundleinputsql();
        SqlDataReader sqlDr;
        string[] data = Regex.Split(jsondata, "////", RegexOptions.IgnoreCase);
        string date = DateTime.Today.ToString("yyMMdd");
        int j = 0;
        int seqtemp = int.Parse(seq)+1;
        foreach (string n in data)
        {
            string[] datalist = n.Split(new char[]{','});
            //判断该JO BUNDLE PART 是否已经存在或者已经扫描
            foreach (string m in partarray)
            {
                string check1 = "true", check2 = "true";
                sqlDr = bundleinputsql.checkexists1(sqlCon, userbarcode, docno, datalist[0], datalist[2], m);
                if (sqlDr.Read())
                    check1 = "false";
                sqlDr.Close();
                sqlDr = bundleinputsql.checkexists2(sqlCon, datalist[0], datalist[2], m);
                if (sqlDr.Read())
                    check2 = "false";
                sqlDr.Close();
                if (check1 == "true" && check2 == "true")//不存在，则插入到历史扫描表并得到新的条码并插入到html字符串
                {
                    //调用存储过程获取新的条码
                    SqlCommand cmd = new SqlCommand("CIPMS_BUNDLE_BARCODE_CREATE", sqlCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FACTORY", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@BARCODETYPE", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@DATE", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@NEW_BUNDLE_BARCODE", SqlDbType.NVarChar, 30);
                    cmd.Parameters["@NEW_BUNDLE_BARCODE"].Direction = ParameterDirection.Output;
                    cmd.Parameters["@DATE"].Value = date;
                    cmd.Parameters["@USERBARCODE"].Value = userbarcode;
                    cmd.Parameters["@FACTORY"].Value = factory.Substring(0, 3);
                    cmd.Parameters["@BARCODETYPE"].Value = "prtbarcode";
                    cmd.ExecuteNonQuery();
                    string newbundlebarcode = cmd.Parameters["@NEW_BUNDLE_BARCODE"].Value.ToString();
                    //插入到扫描历史表
                    bundleinputsql.insertscanlist(sqlCon, seqtemp.ToString(), userbarcode, docno, newbundlebarcode, m, datalist[0], datalist[1], datalist[2], datalist[3], datalist[4], datalist[5], "externalinput");
                    html += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center'>" + seqtemp + "</td><td style='vertical-align:middle; text-align:center'>" + datalist[0] + "</td><td style='vertical-align:middle; text-align:center'>" + datalist[2] + "</td><td style='vertical-align:middle; text-align:center'>" + m + "</td><td style='vertical-align:middle; text-align:center'>" + datalist[1] + "</td><td style='vertical-align:middle; text-align:center'>" + datalist[3] + "</td><td style='vertical-align:middle; text-align:center'>" + datalist[4] + "</td><td style='vertical-align:middle; text-align:center'>" + datalist[5] + "</td><td style='vertical-align:middle; text-align:center'>" + newbundlebarcode + "</td><td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></tr>";
                    j++;
                    seqtemp++;
                }
            }
        }
        sqlCon.Close();
        if (j == 0)
            return "false";
        result = html;
        return result;
    }
}
