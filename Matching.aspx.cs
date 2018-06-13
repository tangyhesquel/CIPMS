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
using NameSpace;
using Code;

public partial class Matching : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod]
    public static String Bundlescan(string userbarcode, string docno, string factory, string svTYPE, string process, string barcode, string dbtrans)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        SqlDataReader sqlDr;
        Sqlstatement commonsql = new Sqlstatement();
        Matchingsql matchingsql = new Matchingsql();
        Transactionsql transfersql = new Transactionsql();
        Packagesql packagesql = new Packagesql();

        string ismatching = "true";

        string barcodetype = GetBarcodeType(barcode);

        string issubmit = "false";

        if (barcodetype == "B")
        {
            int m = 0;
            //string sql = "";
            string html = "";
            try
            {
                matchingsql.unscannedinsert(sqlCon, docno, userbarcode, barcode, "DC", "Matching");
            }
            catch (Exception ex)
            {
                sqlCon.Close();
                return "error1";
            }

            m = 0;
            try
            {
                sqlDr = commonsql.scanned(sqlCon, docno, userbarcode);
            }
            catch (Exception ex)
            {
                sqlCon.Close();
                return "error2";
            }
            while (sqlDr.Read())
            {
                if (sqlDr["PROCESS_CD"].ToString() != process || sqlDr["PROCESS_TYPE"].ToString() != "I")
                    ismatching = "false";
                string carton_barcode = "";
                if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                    carton_barcode = "";
                else
                    carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
                int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
                if (m == 0)
                    html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
                else
                    html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
                m++;
            }
            sqlDr.Close();

            
            if (m == 0)
            {
                sqlCon.Close();
                return "false5";
            }

            //所有的裁片都非提交状态
            
            sqlDr = transfersql.checkcartonsubmit(sqlCon, docno, userbarcode);
            if (sqlDr.Read())
            {
                issubmit = "true";
                ismatching = "false";
            }
            sqlDr.Close();

            if (process != "DC")
            {
                ismatching = "false2";
            }
            else
            {

                //判断是否已经配套过
                sqlDr = matchingsql.checkhavematched(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    ismatching = "false3";
                sqlDr.Close();

                //判断所有已扫描的bundle是否配套完整
                sqlDr = matchingsql.checkmatching(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    ismatching = "false";
                sqlDr.Close();

                //判断扫描的裁片是否存在箱子并且箱子有裁片未被扫描
                sqlDr = matchingsql.checkall(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    ismatching = "false";
                sqlDr.Close();
            }

            result = "[{ \"HTML\": \"" + html + "\", \"ISMATCHING\": \"" + ismatching + "\", ";

            //获取totalbundle和totalpcs
            //string bundle = "0";
            //sqlDr = commonsql.totalbundles(sqlCon, docno, userbarcode);
            //if (sqlDr.Read())
            //    bundle = sqlDr["count"].ToString();
            //sqlDr.Close();
            //int qty = 0;
            //int reduce = 0;
            //string pcs = "0";
            //sqlDr = commonsql.totalpcs(sqlCon, docno, userbarcode);
            //if (sqlDr.Read())
            //{
            //    if (sqlDr["qty"].ToString() != "")
            //        qty = int.Parse(sqlDr["qty"].ToString());
            //    if (sqlDr["reduce"].ToString() != "")
            //        reduce = int.Parse(sqlDr["reduce"].ToString());
            //}
            //sqlDr.Close();
            //pcs = (qty - reduce).ToString();
            //string garment = "0";
            //sqlDr = commonsql.totalgarment(sqlCon, docno, userbarcode);
            //if (sqlDr.Read())
            //    if (sqlDr["QTY"].ToString() != "")
            //        garment = sqlDr["QTY"].ToString();
            //sqlDr.Close();

            //result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \""+garment+"\", \"ISSUBMIT\": \""+issubmit+"\" }]";

            result = result + GetPcsSummary(sqlCon, docno, userbarcode, "F");

            sqlCon.Close();
            return result;
        }
        //箱码
        else if (barcodetype == "C")
        {
            int m = 0;
            string sql = "";
            string html = "";
            //sqlDr = packagesql.matchingselectcarton(sqlCon, barcode, process, docno, userbarcode);
            //while (sqlDr.Read())
            //{
            //    if (sqlDr["PROCESS_CD"].ToString() == "DC" && sqlDr["PROCESS_TYPE"].ToString() == "I")
            //    {
            //        if (m == 0)
            //            sql += "('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Matching',getdate())";
            //        else
            //            sql += ",('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Matching',getdate())";
            //        string carton_barcode = "";
            //        if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
            //            carton_barcode = "";
            //        else
            //            carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
            //        html += "<tr id='" + sqlDr["BUNDLE_ID"] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%'>" + carton_barcode + "</td>";
            //        int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
            //        html += "<td style='vertical-align:middle; text-align:center; width:11%'>" + sqlDr["PROCESS_CD"] + "</td>";
            //        html += "<td style='vertical-align:middle; text-align:center; width:11%'>" + sqlDr["BARCODE"] + "</td>";
            //        html += "<td style='vertical-align:middle; text-align:center; width:10%'>" + sqlDr["PART_DESC"] + "</td>";
            //        html += "<td style='vertical-align:middle; text-align:center; width:10%'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
            //        html += "<td style='vertical-align:middle; text-align:center; width:8%'>" + sqlDr["BUNDLE_NO"] + "</td>";
            //        html += "<td style='vertical-align:middle; text-align:center; width:5%'>" + sqlDr["LAY_NO"] + "</td>";
            //        html += "<td style='vertical-align:middle; text-align:center; width:5%'>" + sqlDr["COLOR_CD"] + "</td>";
            //        html += "<td style='vertical-align:middle; text-align:center; width:4%'>" + sqlDr["SIZE_CD"] + "</td>";
            //        html += "<td style='vertical-align:middle; text-align:center; width:5%'>" + sqlDr["QTY"] + "</td>";
            //        html += "<td style='vertical-align:middle; text-align:center; width:5%'>" + output.ToString() + "</td>";
            //        html += "<td style='vertical-align:middle; text-align:center; width:5%'>" + sqlDr["DEFECT"] + "</td>";
            //        html += "<td style='vertical-align:middle; text-align:center; width:5%'>" + sqlDr["DISCREPANCY_QTY"] + "</td>";
            //        html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></tr>";
            //        m++;
            //    }
            //}
            //sqlDr.Close();
            ////添加到扫描表
            //if (m > 0)
            //    commonsql.unscaninsert(sqlCon, sql);
            //else
            //{
            //    sqlCon.Close();
            //    return "false4";//扫描的都不在DC
            //}

            try
            {
                matchingsql.unscannedinsert2(sqlCon, docno, userbarcode, barcode, "DC", "Matching");
            }
            catch (Exception ex)
            {
                sqlCon.Close();
                return "error1";
            }

            m = 0;
            try
            {
                sqlDr = commonsql.scanned(sqlCon, docno, userbarcode);
            }
            catch (Exception ex)
            {
                sqlCon.Close();
                return "error2";
            }
            while (sqlDr.Read())
            {
                if (sqlDr["PROCESS_CD"].ToString() != process || sqlDr["PROCESS_TYPE"].ToString() != "I")
                    ismatching = "false";
                string carton_barcode = "";
                if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                    carton_barcode = "";
                else
                    carton_barcode = sqlDr["CARTON_BARCODE"].ToString();

                int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
                if (m == 0)
                    html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
                else
                    html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
                m++;
            }
            sqlDr.Close();

            if (m == 0)
            {
                sqlCon.Close();
                return "false5";
            }

            //所有的裁片都非提交状态
            sqlDr = transfersql.checkcartonsubmit(sqlCon, docno, userbarcode);
            if (sqlDr.Read())
                issubmit = "true";
            sqlDr.Close();

            if (process != "DC")
            {
                ismatching = "false2";
            }
            else
            {
                //判断是否已经配套过
                sqlDr = matchingsql.checkhavematched(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    ismatching = "false3";
                sqlDr.Close();

                //判断扫描的裁片是否存在箱子并且箱子有裁片未被扫描
                sqlDr = matchingsql.checkall(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    ismatching = "false";
                sqlDr.Close();

                //判断所有已扫描的bundle是否配套完整
                sqlDr = matchingsql.checkmatching(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    ismatching = "false";
                sqlDr.Close();
            }

            result = "[{ \"HTML\": \"" + html + "\", \"ISMATCHING\": \"" + ismatching + "\", ";

            //获取totalbundle和totalpcs
            //string bundle = "0";
            //sqlDr = commonsql.totalbundles(sqlCon, docno, userbarcode);
            //if (sqlDr.Read())
            //    bundle = sqlDr["count"].ToString();
            //sqlDr.Close();
            //int qty = 0;
            //int reduce = 0;
            //string pcs = "0";
            //sqlDr = commonsql.totalpcs(sqlCon, docno, userbarcode);
            //if (sqlDr.Read())
            //{
            //    if (sqlDr["qty"].ToString() != "")
            //        qty = int.Parse(sqlDr["qty"].ToString());
            //    if (sqlDr["reduce"].ToString() != "")
            //        reduce = int.Parse(sqlDr["reduce"].ToString());
            //}
            //sqlDr.Close();
            //pcs = (qty - reduce).ToString();
            //string garment = "0";
            //sqlDr = commonsql.totalgarment(sqlCon, docno, userbarcode);
            //if (sqlDr.Read())
            //    if (sqlDr["QTY"].ToString() != "")
            //        garment = sqlDr["QTY"].ToString();
            //sqlDr.Close();

            //result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \""+garment+"\", \"ISSUBMIT\": \""+issubmit+"\" }]";

            result = result + GetPcsSummary(sqlCon, docno, userbarcode, "F");

            sqlCon.Close();
            return result;
        }
        else
        {
            sqlCon.Close();
            return "false1";
        }
    }

    [WebMethod]
    public static String Querychange(string docno, string date, string factory, string svTYPE, string process, string userbarcode, string functioncd, string dbtrans)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement commonsql = new Sqlstatement();
        Packagesql packagesql = new Packagesql();
        Matchingsql matchingsql = new Matchingsql();
        SqlDataReader sqlDr;

        string sql = "";
        string html = "";
        int j = 0;
        string ismatching = "true";//判断是否在本部门
        //未扫描的bundle barcode
        sqlDr = commonsql.querychange(sqlCon, date, userbarcode, docno, functioncd);
        while (sqlDr.Read())
        {
            if (sqlDr["PROCESS_TYPE"].ToString() == "I" && sqlDr["PROCESS_CD"].ToString() == "DC")
            {
                if (j == 0)
                    sql += "('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Matching',getdate())";
                else
                    sql += ",('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Matching',getdate())";
                string carton_barcode = "";
                if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                    carton_barcode = "";
                else
                    carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
                html += "<tr id='" + sqlDr["BUNDLE_ID"] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%'>" + carton_barcode + "</td>";
                int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
                html += "<td style='vertical-align:middle; text-align:center; width:11%'>" + sqlDr["PROCESS_CD"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center; width:11%'>" + sqlDr["BARCODE"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center; width:10%'>" + sqlDr["PART_DESC"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center; width:10%'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center; width:8%'>" + sqlDr["BUNDLE_NO"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center; width:5%'>" + sqlDr["LAY_NO"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center; width:5%'>" + sqlDr["COLOR_CD"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center; width:4%'>" + sqlDr["SIZE_CD"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center; width:5%'>" + sqlDr["QTY"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center; width:5%'>" + output.ToString() + "</td>";
                html += "<td style='vertical-align:middle; text-align:center; width:5%'>" + sqlDr["DISCREPANCY_QTY"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center; width:5%'>" + sqlDr["DEFECT"] + "</td>";
                html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></tr>";
                j++;
            }
        }
        sqlDr.Close();
        //添加到扫描表
        if (j > 0)
            commonsql.unscaninsert(sqlCon, sql);
        else
        {
            sqlCon.Close();
            return "false4";//扫描的都不在DC
        }

        if (process != "DC")
        {
            ismatching = "false";
        }
        else
        {
            //判断是否已经配套过
            sqlDr = matchingsql.checkhavematched(sqlCon, docno, userbarcode);
            if (sqlDr.Read())
                ismatching = "false3";
            sqlDr.Close();

            //判断扫描的裁片是否存在箱子并且箱子有裁片未被扫描
            sqlDr = matchingsql.checkall(sqlCon, docno, userbarcode);
            if (sqlDr.Read())
                ismatching = "false";
            sqlDr.Close();

            //判断所有已扫描的bundle是否配套完整
            sqlDr = matchingsql.checkmatching(sqlCon, docno, userbarcode);
            if (sqlDr.Read())
                ismatching = "false";
            sqlDr.Close();
        }

        result = "[{ \"HTML\": \"" + html + "\", \"ISMATCHING\": \"" + ismatching + "\", ";

        //获取totalbundle和totalpcs
        string bundle = "0";
        sqlDr = commonsql.totalbundles(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            bundle = sqlDr["count"].ToString();
        sqlDr.Close();
        int qty = 0;
        int reduce = 0;
        string pcs = "0";
        sqlDr = commonsql.totalpcs(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
        {
            qty = int.Parse(sqlDr["qty"].ToString());
            reduce = int.Parse(sqlDr["reduce"].ToString());
        }
        sqlDr.Close();
        pcs = (qty - reduce).ToString();
        string garment = "0";
        sqlDr = commonsql.totalgarment(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            if (sqlDr["QTY"].ToString() != "")
                garment = sqlDr["QTY"].ToString();
        sqlDr.Close();

        result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \""+garment+"\" }]";
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Matchingmethod(string userbarcode, string factory, string svTYPE, string docno, string process, string languagearray)
    {
        string[] translation = languagearray.Split(new char[] { '*' });
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement commonsql = new Sqlstatement();
        Matchingsql matchingsql = new Matchingsql();
        Packagesql packagesql = new Packagesql();
        SqlDataReader sqlDr;
        string result = "";

        if (process != "DC")
        {
            sqlCon.Close();
            return "false1";//不是DC不能操作matching
        }
        else
        {
            //判断裁片是否都在本部门
            sqlDr = packagesql.matchingcheckbundle(sqlCon, docno, userbarcode, process);
            if (sqlDr.Read())
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false3";//bundle part的状态异常
            }
            sqlDr.Close();

            string ismatching = "true";
            //判断所有已扫描的bundle是否配套完整
            sqlDr = matchingsql.checkmatching(sqlCon, docno, userbarcode);
            if (sqlDr.Read())
                ismatching = "false";
            sqlDr.Close();

            //判断是否已经配套过
            sqlDr = matchingsql.checkhavematched(sqlCon, docno, userbarcode);
            if (sqlDr.Read())
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false4";
            }

            //判断扫描的裁片是否存在箱子并且箱子有裁片未被扫描
            sqlDr = matchingsql.checkall(sqlCon, docno, userbarcode);
            if (sqlDr.Read())
                ismatching = "false";
            sqlDr.Close();
            if (ismatching == "false")
            {
                sqlCon.Close();
                return "false2";
            }

            string date = DateTime.Now.ToString("yyMMdd");
            //调用存储过程，返回新生成的箱码以及配套数量
            string barcodeflag = factory + date;//生成箱码前缀标记

            try
            {
                SqlCommand cmd = new SqlCommand("CIPMS_MATCHING", sqlCon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.Parameters.Add("@FACTORY", SqlDbType.NVarChar);
                cmd.Parameters.Add("@PROCESS", SqlDbType.NVarChar);
                cmd.Parameters.Add("@BARCODEFLAG", SqlDbType.NVarChar);
                cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
                cmd.Parameters.Add("@DOCNO", SqlDbType.NVarChar);
                cmd.Parameters.Add("@DATE", SqlDbType.NVarChar);
                cmd.Parameters.Add("@RETURNCARTON", SqlDbType.NVarChar, 20);
                cmd.Parameters["@RETURNCARTON"].Direction = ParameterDirection.Output;
                cmd.Parameters["@FACTORY"].Value = factory;
                cmd.Parameters["@PROCESS"].Value = process;
                cmd.Parameters["@BARCODEFLAG"].Value = barcodeflag;
                cmd.Parameters["@DATE"].Value = date;
                cmd.Parameters["@USERBARCODE"].Value = userbarcode;
                cmd.Parameters["@DOCNO"].Value = docno;
                cmd.ExecuteNonQuery();
                string cartonbarcode = cmd.Parameters["@RETURNCARTON"].Value.ToString();

                if (cartonbarcode == "N")
                {
                    sqlCon.Close();
                    return "false5";//没有关箱
                }
                else if (cartonbarcode == "11")
                {
                    //DataSet ds = new DataSet();
                    //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    //adapter.Fill(ds);
                    //string temp = "";
                    //foreach (DataRow mDr in ds.Tables[0].Rows)
                    //{
                    //    foreach (DataColumn mDc in ds.Tables[0].Columns)
                    //    {
                    //        temp = mDr[mDc].ToString();
                    //    }
                    //}
                    sqlCon.Close();
                    return "false6";//裁片不完整
                }
                else if (cartonbarcode == "12")
                {
                    sqlCon.Close();
                    return "false7";//配套结果不正确
                }
                //生成新的箱子
                else
                {
                    Cartonbarcodeprint cartonhtml = new Cartonbarcodeprint();
                    string html = cartonhtml.Cartonbarcodehtml(sqlCon, cartonbarcode, languagearray);
                    result = "[{ \"HTML\":\"" + html + "\", \"CARTON\":\"" + cartonbarcode + "\" }]";
                }
            }
            catch (Exception ex)
            {
                sqlCon.Close();
                return ex.Message;
            }
            finally
            {
            }
        }
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Delete(string docno, string process, string userbarcode, string factory, string svTYPE, string bundleno)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement commonsql = new Sqlstatement();
        Matchingsql matchingsql = new Matchingsql();
        Packagesql packagesql = new Packagesql();
        SqlDataReader sqlDr;

        string ismatching = "true";
        string html = "";

        //删除扫描历史表的某行
        commonsql.deletebundle(sqlCon, docno, userbarcode, bundleno);

        //判断扫描表是否为空
        sqlDr = commonsql.isempty(sqlCon, docno, userbarcode);
        if (!sqlDr.Read())
        {
            sqlCon.Close();
            return "false";//扫描表为空，UI界面初始化
        }
        sqlDr.Close();

        int m = 0;
        try
        {
            sqlDr = commonsql.scanned(sqlCon, docno, userbarcode);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error3";
        }
        while (sqlDr.Read())
        {
            if (sqlDr["PROCESS_CD"].ToString() != process || sqlDr["PROCESS_TYPE"].ToString() != "I")
                ismatching = "false";
            string carton_barcode = "";
            if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
            int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
            if (m == 0)
                html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
            else
                html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
            m++;
        }
        sqlDr.Close();

        //判断所有已扫描的bundle是否配套完整
        sqlDr = matchingsql.checkmatching(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            ismatching = "false";
        sqlDr.Close();

        //判断扫描的裁片是否存在箱子并且箱子有裁片未被扫描
        sqlDr = matchingsql.checkall(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            ismatching = "false";
        sqlDr.Close();

        //获取totalbundle和totalpcs
        string bundle = "0";
        sqlDr = commonsql.totalbundles(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            bundle = sqlDr["count"].ToString();
        sqlDr.Close();
        int qty = 0;
        int reduce = 0;
        string pcs = "0";
        sqlDr = commonsql.totalpcs(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
        {
            qty = int.Parse(sqlDr["qty"].ToString());
            reduce = int.Parse(sqlDr["reduce"].ToString());
        }
        sqlDr.Close();
        pcs = (qty - reduce).ToString();
        string garment = "0";
        sqlDr = commonsql.totalgarment(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            if (sqlDr["QTY"].ToString() != "")
                garment = sqlDr["QTY"].ToString();
        sqlDr.Close();

        sqlCon.Close();
        string result = "[{ \"HTML\": \""+html+"\", \"ISMATCHING\": \"" + ismatching + "\", \"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \""+garment+"\" }]";
        return result;
    }

    [WebMethod]
    public static String Deletecarton(string docno, string process, string userbarcode, string factory, string svTYPE, string cartonbarcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement commonsql = new Sqlstatement();
        Matchingsql matchingsql = new Matchingsql();
        Packagesql packagesql = new Packagesql();
        SqlDataReader sqlDr;

        //删除箱内的bundle
        commonsql.deletecarton(sqlCon, docno, userbarcode, cartonbarcode);

        //判断扫描表是否为空
        sqlDr = commonsql.isempty(sqlCon, docno, userbarcode);
        if (!sqlDr.Read())
        {
            sqlCon.Close();
            return "false";
        }
        sqlDr.Close();

        string ismatching = "true";
        string html = "";

        int m = 0;
        try
        {
            sqlDr = commonsql.scanned(sqlCon, docno, userbarcode);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error3";
        }
        while (sqlDr.Read())
        {
            if (sqlDr["PROCESS_CD"].ToString() != process || sqlDr["PROCESS_TYPE"].ToString() != "I")
                ismatching = "false";
            string carton_barcode = "";
            if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
            int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
            if (m == 0)
                html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
            else
                html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
            m++;
        }
        sqlDr.Close();

        //判断所有已扫描的bundle是否配套完整
        sqlDr = matchingsql.checkmatching(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            ismatching = "false";
        sqlDr.Close();

        //判断扫描的裁片是否存在箱子并且箱子有裁片未被扫描
        sqlDr = matchingsql.checkall(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            ismatching = "false";
        sqlDr.Close();

        //获取totalbundle和totalpcs
        string bundle = "0";
        sqlDr = commonsql.totalbundles(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            bundle = sqlDr["count"].ToString();
        sqlDr.Close();
        int qty = 0;
        int reduce = 0;
        string pcs = "0";
        sqlDr = commonsql.totalpcs(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
        {
            qty = int.Parse(sqlDr["qty"].ToString());
            reduce = int.Parse(sqlDr["reduce"].ToString());
        }
        sqlDr.Close();
        pcs = (qty - reduce).ToString();
        string garment = "0";
        sqlDr = commonsql.totalgarment(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            if (sqlDr["QTY"].ToString() != "")
                garment = sqlDr["QTY"].ToString();
        sqlDr.Close();

        sqlCon.Close();
        string result = "[{ \"HTML\": \"" + html + "\", \"ISMATCHING\": \"" + ismatching + "\", \"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \"" + garment + "\" }]";
        return result;
    }

    public static string GetBarcodeType(string barcode)
    {
        if (barcode != null && barcode != "")
        {
            var index = barcode.IndexOf("-");
            if (index > -1)//存在符号“-”，说明是箱码或者流水单号
            {
                //获取字符串第四个到第九个字符
                var keyword = barcode.Substring(3, 6);
                //正则表达式判断字符串是否包含字母
                if (!isNumberic(keyword))
                {
                    //说明字符串中存在英文字母，则说明是箱码
                    return "C";
                }
                else
                {
                    //不存在则说明是流水单号
                    return "D";
                }
            }
            else//不存在，说明是扎码B
            {//获取字符串第四个到第九个字符
                var keyword = barcode.Substring(3, 6);
                //正则表达式判断字符串是否包含字母
                if (!isNumberic(keyword))
                {
                    //说明字符串中存在英文字母，则说明是箱码
                    return "C";
                }
                else
                {
                    return "B";
                }
            }
        }
        else
            return null;
    }

    public static bool isNumberic(string keyword)
    {
        //判断是否为整数字符串
        //是的话则将其转换为数字并将其设为out类型的输出值、返回true, 否则为false
        int result = -1;   //result 定义为out 用来输出值
        try
        {
            //当数字字符串的为是少于4时，以下三种都可以转换，任选一种
            //如果位数超过4的话，请选用Convert.ToInt32() 和int.Parse()

            //result = int.Parse(message);
            //result = Convert.ToInt16(message);
            result = Convert.ToInt32(keyword);
            return true;
        }
        catch
        {
            return false;
        }
    }

    //tangyh 2017.03.28
    private static string GetPcsSummary(SqlConnection sqlCon, string docno, string userbarcode, string onlyfield)
    {
        SqlDataReader sqlDr;
        Sqlstatement commonsql = new Sqlstatement();
        string result = "";
        string partbundle = "0";
        string bundle = "";
        sqlDr = commonsql.totalbundles(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
        {
            bundle = sqlDr["distinctcount"].ToString();
            partbundle = sqlDr["count"].ToString();
        }
        sqlDr.Close();
        int qty = 0;
        int reduce = 0;
        string pcs = "";
        sqlDr = commonsql.totalpcs(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
        {
            if (sqlDr["qty"].ToString() != "")
                qty = int.Parse(sqlDr["qty"].ToString());
            if (sqlDr["reduce"].ToString() != "")
                reduce = int.Parse(sqlDr["reduce"].ToString());
        }
        sqlDr.Close();
        pcs = (qty - reduce).ToString();
        string garment = "0";
        sqlDr = commonsql.totalgarment(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            if (sqlDr["QTY"].ToString() != "")
                garment = sqlDr["QTY"].ToString();
        sqlDr.Close();
        if (onlyfield == "T")
        {
            result = "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALGARMENTPCS\": \"" + garment + "\", \"TOTALPART\": \"" + partbundle + "\", \"TOTALPCS\": \"" + pcs + "\"";
        }
        else
        {
            result = "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALGARMENTPCS\": \"" + garment + "\", \"TOTALPART\": \"" + partbundle + "\", \"TOTALPCS\": \"" + pcs + "\" }]";
        }

        return result;
    }
}