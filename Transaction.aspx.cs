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

public partial class Flow_test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod]
    public static String Cleanscan(string factory, string svTYPE, string docno, string userbarcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        sqlstatement.cleanscan(sqlCon, docno, userbarcode);
        sqlCon.Close();
        return "success";
    }

    //获取PEER厂
    [WebMethod]
    public static String Peerfactory(string factory, string svTYPE)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr = sqlstatement.peerfactorydr(sqlCon);
        while (sqlDr.Read())
        {
            result += "<option value='" + sqlDr["FACTORY_ID"] + "'>" + sqlDr["FACTORY_ID"] + "</option>";
        }
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Peerprocess(string factory, string svTYPE)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr = sqlstatement.processdr(sqlCon, factory);
        while (sqlDr.Read())
        {
            result += "<option value='" + sqlDr["PRC_CD"] + "'>" + sqlDr["NM"] + "</option>";
        }
        return result;
    }

    [WebMethod]
    public static String Process(string factory, string svTYPE, string process, string flowtype)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Transactionsql sqlstatement = new Transactionsql();
        SqlDataReader sqlDr = sqlstatement.nextprocess(sqlCon, factory, process, flowtype);
        while (sqlDr.Read())
        {
            result += "<option title='"+sqlDr["CIPMS_PART"]+"' value='" + sqlDr["PRC_CD"] + "'>" + sqlDr["NM"] + "</option>";
        }
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Checkifneedtoreceive(string factory, string svTYPE, string process, string nextprocess)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;
        string result = "";
        sqlDr = sqlstatement.nextprocessneedtorec(sqlCon, factory, process, nextprocess);
        if (sqlDr.Read())
            result = "true";
        else
            result = "false";
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Production(string factory, string svTYPE, string process, string nextfactory, string nextprocess)
    {
        string result = "[{ \"HTML\":";
        string html = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr = sqlstatement.peerproduction(sqlCon, nextfactory, nextprocess);
        while (sqlDr.Read())
        {
            html += "<option value='" + sqlDr["PRODUCTION_LINE_CD"] + "'>" + sqlDr["PRODUCTION_LINE_NAME"] + "</option>";
        }
        sqlDr.Close();
        if (html == "")
            html = "NA";
        result += "\""+html+"\", \"FLAG\":";
        if (factory == nextfactory)
        {
            if (process == nextprocess)
                result += "\"False\"}]";
            else
            {
                sqlDr = sqlstatement.nextprocessneedtorec(sqlCon, factory, process, nextprocess);
                if (sqlDr.Read())
                    result += "\"True\"}]";
                else
                    result += "\"False\"}]";
                sqlDr.Close();
            }
        }
        else
            result += "\"True\"}]";
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Reprintdocno(string factory, string svTYPE, string docno, string userbarcode, string languagearray)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Transactionsql transfersql = new Transactionsql();
        SqlDataReader sqlDr;

        //查询流水单
        int i = 0;
        string doc_no = "";
        sqlDr = transfersql.getdocno(sqlCon, docno, userbarcode);
        while (sqlDr.Read())
        {
            doc_no = sqlDr["DOC_NO"].ToString();
            i++;
        }
        sqlDr.Close();
        if (i == 0)
        {
            sqlCon.Close();
            return "false1";//不存在流水单
        }
        else if (i > 1)
        {
            sqlCon.Close();
            return "false2";//存在多个流水单
        }

        //根据流水单构造html
        string newdocno = doc_no;
        newdocno = factory + newdocno;

        //查询流水单的信息
        string process = "", nextprocess = "", productionline = "", nextproductionline = "", username = "";
        sqlDr = transfersql.getdocnoinformation(sqlCon, doc_no);
        if (sqlDr.Read())
        {
            process = sqlDr["PROCESS_CD"].ToString();
            nextprocess = sqlDr["NEXT_PROCESS_CD"].ToString();
            productionline = sqlDr["PRODUCTION_LINE_CD"].ToString();
            nextproductionline = sqlDr["NEXT_PRODUCTION_LINE_CD"].ToString();
            username = sqlDr["NAME"].ToString();
        }
        sqlDr.Close();


        Docnoproduce docnoprint = new Docnoproduce();
        string result = docnoprint.Docnostring(sqlCon, userbarcode, doc_no, factory, process, productionline, nextprocess, nextproductionline, languagearray, "N");

        sqlCon.Close();
        return "[{ \"HTML\": \"" + result + "\", \"CARTON\": \"" + newdocno + "\" }]";
    }

    [WebMethod]
    public static String DOCNOscan(string userbarcode, string docno, string factory, string svTYPE, string process, string doc_no, string fromfactoryselected, string fromprocessselected, string fromproductionselected, string fromfactory, string dbtransdefect, string dbtransdelete, string get_othercarton,string part)
    {
        doc_no = doc_no.Substring(3);
        string result = "{ \"bundles\": [{";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Transactionsql transactionsql = new Transactionsql();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;

        //查找该流水单是否是提交状态
        string issubmit = "false";
        if (get_othercarton == "F")
        {
            sqlDr = transactionsql.checksubmit(sqlCon, doc_no);
            if (sqlDr.Read())
                issubmit = "true";
        }

        //流水单内所有裁片都在本部门
        string isprocess = "true";

        //sqlDr = transactionsql.querybundlepart(sqlCon, doc_no, docno, userbarcode);
        //int n = 0;
        //string sql = "";
        string html = "";
        string isfirst = "false";
#region
        //while (sqlDr.Read())
        //{
        //    if (fromfactoryselected == fromfactory)
        //    {
        //        fromfactoryselected = sqlDr["FACTORY_CD"].ToString();
        //        fromprocessselected = sqlDr["PROCESS_CD"].ToString();
        //        fromproductionselected = sqlDr["PRODUCTION_LINE_CD"].ToString();
        //        result += "\"FACTORY\": \"" + sqlDr["FACTORY_CD"].ToString() + "\", \"PROCESS\": \"" + sqlDr["PROCESS_CD"].ToString() + "\", \"PRODUCTION\": \"" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "\", ";
        //        isfirst = "true";
        //    }
        //    if (n == 0)
        //        sql += "('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Transaction',getdate())";
        //    else
        //        sql += ",('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Transaction',getdate())";
        //    string carton_barcode = "";
        //    if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
        //        carton_barcode = "";
        //    else
        //        carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
        //    html += "<tr id='" + sqlDr["BUNDLE_ID"] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:10%;'>" + carton_barcode + "</td>";
        //    int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
        //    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["PROCESS_CD"] + "</td>";
        //    if (sqlDr["PROCESS_CD"].ToString() != process)
        //        isprocess = "false";
        //    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["BARCODE"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["PART_DESC"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["BUNDLE_NO"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["LAY_NO"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["COLOR_CD"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:3%;'>" + sqlDr["SIZE_CD"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["QTY"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + output.ToString() + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["DISCREPANCY_QTY"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["DEFECT"] + "</td>";
        //    html += "<td><a data-role='button' >" + dbtransdefect + "</a></td>";
        //    html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtransdelete + "' /></td></tr>";
        //    n++;
        //}
        //sqlDr.Close();

        //已经全部扫描
        //if (n == 0)
        //{
        //    sqlCon.Close();
        //    return "false2";//已全部扫描过
        //}
        //if (sql != "")
        //{
        //    //把未扫描的bundle part添加到扫描表
        //    sqlstatement.unscaninsert(sqlCon, sql);
        //}
#endregion
        try
        {
            transactionsql.unscanneddocnoinsert(sqlCon, docno, userbarcode, doc_no, "Transaction", get_othercarton,part);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error1";
        }

        int m = 0;
        try
        {
            sqlDr = sqlstatement.scanned(sqlCon, docno, userbarcode);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error3";
        }
        while (sqlDr.Read())
        {
            if (fromfactoryselected == fromfactory)
            {
                fromfactoryselected = sqlDr["FACTORY_CD"].ToString();
                fromprocessselected = sqlDr["PROCESS_CD"].ToString();
                fromproductionselected = sqlDr["PRODUCTION_LINE_CD"].ToString();
                result += "\"FACTORY\": \"" + sqlDr["FACTORY_CD"].ToString() + "\", \"PROCESS\": \"" + sqlDr["PROCESS_CD"].ToString() + "\", \"PRODUCTION\": \"" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "\", ";
                //added 201-02-26
                result +="\"CARNO\": \"" + sqlDr["CAR_NO"].ToString() + "\", ";
                isfirst = "true";
            }
            if (sqlDr["PROCESS_CD"].ToString() != process || sqlDr["PROCESS_TYPE"].ToString() != "I")
                isprocess = "false";
            string carton_barcode = "";
            if (sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                carton_barcode = sqlDr["CARTON_BARCODE"].ToString();

            int output = 0;
    
            output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
           

            if (m == 0)
                html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
            else
                html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
            m++;
        }
        sqlDr.Close();

        result += "\"ISFIRST\": \"" + isfirst + "\", \"HTML\": \"" + html + "\" }],\"bundlestatus\": [{ ";

        sqlDr = transactionsql.checkreprint(sqlCon, docno, userbarcode);
        string canreprint = "false";
        int i = 0;
        while (sqlDr.Read())
        {
            if (sqlDr["DOC_NO"].ToString() != "0")
                i++;
        }
        sqlDr.Close();
        if (i == 1)
            canreprint = "true";
        
        //获取totalbundle和totalpcs
        //string bundle = "0";
        //string pcs = "0";
        //sqlDr = sqlstatement.totalbundles(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    bundle = sqlDr["count"].ToString();
        //sqlDr.Close();
        //int qty = 0;
        //int reduce = 0;
        //sqlDr = sqlstatement.totalpcs(sqlCon, docno, userbarcode);
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
        //sqlDr = sqlstatement.totalgarment(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    if (sqlDr["QTY"].ToString() != "")
        //        garment = sqlDr["QTY"].ToString();
        //sqlDr.Close();
        //result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \"" + garment + "\", \"ISPROCESS\": \"" + isprocess + "\", \"ISSUBMIT\": \"" + issubmit + "\", \"CANREPRINT\": \""+canreprint+"\"}]}";

        result = result + GetPcsSummary(sqlCon, docno, userbarcode, "T") + ", \"ISPROCESS\": \"" + isprocess + "\", \"ISSUBMIT\": \"" + issubmit + "\", \"CANREPRINT\": \""+canreprint+"\"}]}";

        sqlCon.Close();
        return result;
    }

    //外发bundle扫描
    [WebMethod]
    public static String OASBundlescan(string userbarcode, string docno, string factory, string svTYPE, string process, string nextprocess, string barcode, string part, string contract, string contractnoselect, string fromfactoryselected, string fromprocessselected, string fromproductionselected, string fromfactory)
    {
        string result = "{ \"bundles\": [{";
        string[] partarray = part.Split(new char[] { ',' });
        part = "";
        for (int i = 0; i < partarray.Length; i++)
        {
            if (i == 0) part += "'" + partarray[i] + "'";
            else part += ",'" + partarray[i] + "'";
        }
        part = "(" + part + ")";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        Transactionsql transactionsql = new Transactionsql();
        SqlDataReader sqlDr;

        //判断该JO、Bundle是否已经外发过
        sqlDr = transactionsql.checkreceived(sqlCon, barcode, nextprocess);
        if (sqlDr.Read())
        {
            sqlDr.Close();
            sqlCon.Close();
            return "false11";//该JO、Bundle已经外发过
        }
        sqlDr.Close();

        //判断用户是否已经选择了外发单号
        string first = "false";
        sqlDr = transactionsql.numofscan(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            if (sqlDr["QTY"].ToString() != "0" && contract == contractnoselect)
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false2";//用户还没有选择外发单号
            }
            else if(sqlDr["QTY"].ToString() == "0")
                first = "true";
        sqlDr.Close();
        string contractno = "";
        if (first == "true")
        {
            sqlDr = transactionsql.querycontract(sqlCon, barcode, nextprocess);
            while (sqlDr.Read())
            {
                contractno += "<option value='" + sqlDr["CONTRACT_NO"] + "'>" + sqlDr["CONTRACT_NO"] + "</option>";
            }
            sqlDr.Close();
        }

        //判断扫描的bundle part存在该外发单号
        if (first == "false")
        {
            sqlDr = transactionsql.hassamecontract(sqlCon, contract, barcode, nextprocess);
            if (!sqlDr.Read())
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false4";//该bundle不存在该外发单号
            }
            sqlDr.Close();
        }

        //判断该bundle part是否在箱内
        sqlDr = transactionsql.isincarton(sqlCon, barcode, part);
        while (sqlDr.Read())
        {
            if (sqlDr["CARTON_STATUS"].ToString() == "C")
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false3";//用户扫描的该bundle在箱内，不允许
            }
        }
        sqlDr.Close();

        //判断该bundle part是否在本部门
        string isprocess = "true";
        sqlDr = transactionsql.isinmyprocess(sqlCon, barcode, part, process);
        if (sqlDr.Read())
        {
            isprocess = "false";
        }
        sqlDr.Close();

        string isproduction = "true";
        string isfirst = "false";
        string wrongproduction = "NA";

        //查找未扫描的bundle part
        sqlDr = transactionsql.unscan(sqlCon, barcode, userbarcode, part, docno);
        string sql = "";
        string html = "";
        //int m = 0;
        int n = 0;
        string parts = "";
        while (sqlDr.Read())
        {
            //if (sqlDr["PROCESS_CD"].ToString() == process && sqlDr["CARTON_STATUS"].ToString() != "C")
            //{
            if (fromfactoryselected == fromfactory)
            {
                fromfactoryselected = sqlDr["FACTORY_CD"].ToString();
                fromprocessselected = sqlDr["PROCESS_CD"].ToString();
                fromproductionselected = sqlDr["PRODUCTION_LINE_CD"].ToString();
                isfirst = "true";
                result += "\"FACTORY\": \"" + sqlDr["FACTORY_CD"].ToString() + "\", \"PROCESS\": \"" + sqlDr["PROCESS_CD"].ToString() + "\", \"PRODUCTION\": \"" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "\", ";
            }
            else
            {
                if (sqlDr["PRODUCTION_LINE_CD"].ToString() != fromproductionselected)
                {
                    wrongproduction = sqlDr["PRODUCTION_LINE_CD"].ToString();
                    isproduction = "false";
                }
            }
            if (n == 0)
                sql += "('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Transaction',getdate())";
            else
                sql += ",('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Transaction',getdate())";
            string carton_barcode = "";
            if (sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
            html += "<tr id='" + sqlDr["BUNDLE_ID"] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center'>" + carton_barcode + "</td>";
         
            int output = 0;
            
            output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
           

            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["PROCESS_CD"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["BARCODE"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["PART_DESC"] + "</td>";
            if (n == 0)
                parts = sqlDr["PART_DESC"].ToString();
            else
                parts += sqlDr["PART_DESC"].ToString();
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["BUNDLE_NO"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["LAY_NO"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["SIZE_CD"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["COLOR_CD"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["QTY"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + output.ToString() + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["DISCREPANCY_QTY"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT"] + "</td>";
            html += "<td><a data-role='button' onclick='defectbtn(this)' href='Reducedialog.aspx' data-rel='dialog'>Defect</a></td>";
            html += "<td><input type='button' onclick='deletebtn(this)' value='Delete' /></td></tr>";
            n++;
            //}
            //m++;
        }
        result += "\"ISFIRST\": \"" + isfirst + "\", \"PART\": \"" + parts + "\", \"ISPRODUCTION\": \"" + isproduction + "\", \"WRONGPRODUCTION\": \"" + wrongproduction + "\", \"HTML\": \"" + html + "\" }],\"bundlestatus\": [{ ";
        sqlDr.Close();

        //已经全部扫描
        if (n == 0)
        {
            sqlCon.Close();
            return "false1";//已经全部扫描过
        }
        
        if (sql != "" && isproduction=="true")
        {
            //把未扫描的bundle part添加到扫描表
            sqlstatement.unscaninsert(sqlCon, sql);
        }

        //获取totalbundle和totalpcs
        //string bundle = "0";
        //sqlDr = sqlstatement.totalbundles(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    bundle = sqlDr["count"].ToString();
        //sqlDr.Close();
        //int qty = 0;
        //int reduce = 0;
        //string pcs = "0";
        //sqlDr = sqlstatement.totalpcs(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //{
        //    qty = int.Parse(sqlDr["qty"].ToString());
        //    reduce = int.Parse(sqlDr["reduce"].ToString());
        //}
        //sqlDr.Close();
        //pcs = (qty - reduce).ToString();
        //string garment = "0";
        //sqlDr = sqlstatement.totalgarment(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    if (sqlDr["QTY"].ToString() != "")
        //        garment = sqlDr["QTY"].ToString();
        //sqlDr.Close();
        //result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\" , \"TOTALGARMENTPCS\": \""+garment+"\", \"ISPROCESS\": \"" + isprocess + "\", \"CONTRACTNO\": \"" + contractno + "\"}]}";
        result = result + GetPcsSummary(sqlCon, docno, userbarcode, "T") + ", \"ISPROCESS\": \"" + isprocess + "\", \"CONTRACTNO\": \"" + contractno + "\"}]}";

        sqlCon.Close();
        return result;
    }

    //外发carton扫描
    [WebMethod]
    public static String OASCartonscan(string userbarcode, string docno, string factory, string svTYPE, string process, string nextprocess, string barcode, string contract, string contractnoselect, string get_othercarton)
    {
        string result = "{ \"bundles\": [";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        Transactionsql transactionsql = new Transactionsql();

        SqlDataReader sqlDr;
        string statuscarton = "";
        string process_cd = "";
        string process_type = "";
        string isprocess = "";
        string first = "false";
        string html = "";
        string sql = "";
        string parts = "";
        string contractno = "";

        if (get_othercarton == "F")
        {
            sqlDr = sqlstatement.cartonstatus(sqlCon, barcode);
            bool hasrecord = false;
            while (sqlDr.Read())
            {
                statuscarton = sqlDr["CARTON_STATUS"].ToString();
                process_cd = sqlDr["PROCESS_CD"].ToString();
                process_type = sqlDr["PROCESS_TYPE"].ToString();
                if (statuscarton != "C")
                {
                    sqlCon.Close();
                    return "false4";//该箱码为开箱状态
                }
                hasrecord = true;
            }
            if (hasrecord == false)
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false1";//该箱码不存在
            }
            else
            {
                sqlDr.Close();
            }

            if (sqlDr.Read())
            {
                statuscarton = sqlDr["CARTON_STATUS"].ToString();
                process_cd = sqlDr["PROCESS_CD"].ToString();
                process_type = sqlDr["PROCESS_TYPE"].ToString();
            }
            else
            {
                sqlCon.Close();
                return "false1";//该箱码不存在
            }
            sqlDr.Close();

            if (statuscarton != "C")
            {
                sqlCon.Close();
                return "false2";//该箱为空
            }

            //string isprocess = "";

            if (process_cd == process && process_type == "I")
                isprocess = "true";//箱子在本部门
            else
                isprocess = "false";

            //判断用户是否已经选择了外发单号
            //string first = "false";
            sqlDr = transactionsql.numofscan(sqlCon, docno, userbarcode);
            if (sqlDr.Read())
                if (sqlDr["QTY"].ToString() != "0" && contract == contractnoselect)
                {
                    sqlDr.Close();
                    sqlCon.Close();
                    return "false3";//用户还没有选择流水单号
                }
                else if (sqlDr["QTY"].ToString() == "0")
                    first = "true";
            sqlDr.Close();
            //string contractno = "";
            if (first == "true")
            {
                //找出箱内JO数
                int i = 0;
                string jolist = "";
                string onejo = "";
                sqlDr = transactionsql.jo(sqlCon, barcode);
                while (sqlDr.Read())
                {
                    if (i == 0)
                    {
                        jolist += "'" + sqlDr["JOB_ORDER_NO"] + "'";
                        onejo = sqlDr["JOB_ORDER_NO"].ToString();
                    }
                    else
                        jolist += ",'" + sqlDr["JOB_ORDER_NO"] + "'";
                    i++;
                }
                sqlDr.Close();
                jolist = "(" + jolist + ")";
                //取其中一个JO，取其所有外发单号
                List<string> cnlist = new List<string>();
                sqlDr = transactionsql.onecn(sqlCon, onejo, nextprocess);
                while (sqlDr.Read())
                {
                    cnlist.Add(sqlDr["CONTRACT_NO"].ToString());
                }
                sqlDr.Close();
                foreach (string n in cnlist)
                {
                    sqlDr = transactionsql.cnjoqty(sqlCon, n, jolist);
                    if (sqlDr["QTY"].ToString() == i.ToString())
                    {
                        contractno += "<option value='" + n + "'>" + n + "</option>";
                    }
                }
            }

            //判断扫描箱码里的所有JO是否存在该外发单号
            if (first == "false")
            {
                string joqty = "0";
                string ctnqty = "0";
                sqlDr = transactionsql.joqty(sqlCon, barcode);
                if (sqlDr.Read())
                    joqty = sqlDr["QTY"].ToString();
                sqlDr.Close();
                sqlDr = transactionsql.ctnqty(sqlCon, contract, barcode, nextprocess);
                if (sqlDr.Read())
                    ctnqty = sqlDr["QTY"].ToString();
                sqlDr.Close();
                if (joqty != ctnqty && joqty != "0")
                {
                    sqlCon.Close();
                    return "false4";//箱内存在JO不在该外发单号内
                }
            }

            
            //string html = "";
            //箱内未扫描的bundle barcode
            //string sql = "";
            sqlDr = sqlstatement.unscanbundlepart(sqlCon, barcode, docno, userbarcode);
            int j = 0;
            //string parts = "";
            while (sqlDr.Read())
            {
                if (j == 0)
                    sql += "('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Transaction',getdate())";
                else
                    sql += ",('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Transaction',getdate())";
                string carton_barcode = "";
                if (sqlDr["CARTON_STATUS"].ToString() != "C")
                    carton_barcode = "";
                else
                    carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
                html += "<tr id='" + sqlDr["BUNDLE_ID"] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center'>" + carton_barcode + "</td>";

                int output = 0;
                output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
                
                html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["PROCESS_CD"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["BARCODE"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["PART_CD"] + "</td>";
                if (j == 0)
                    parts = sqlDr["PART_CD"].ToString();
                else
                    parts += sqlDr["PART_CD"].ToString();
                html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["BUNDLE_NO"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["LAY_NO"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["SIZE_CD"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["COLOR_CD"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["QTY"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center'>" + output.ToString() + "</td>";
                html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["DISCREPANCY_QTY"] + "</td>";
                html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT"] + "</td>";
                html += "<td><a data-role='button' onclick='defectbtn(this)' href='Reducedialog.aspx' data-rel='dialog'>Defect</a></td>";
                html += "<td><input type='button' onclick='deletebtn(this)' value='Delete' /></td></tr>";
                j++;
            }
            sqlDr.Close();

            result += "{ \"HTML\": \"" + html + "\" }], \"cartonstatus\": [{ ";
        }
        else
        {
            result += "{ \"HTML\": \"" + "" + "\" }], \"cartonstatus\": [{ ";
        }
        //把未扫描的插到扫描表里
        if (sql != "" )
            sqlstatement.unscaninsert(sqlCon, sql);
        else
        {
            //已经全部扫描过了
            sqlCon.Close();
            return "false5";
        }

        if (get_othercarton == "T")
        {
            sqlstatement.unscaninsert(sqlCon, docno, userbarcode, "Transaction", barcode, get_othercarton, false,"");
        }

        //获取totalbundle和totalpcs
        //string bundle = "0";
        //sqlDr = sqlstatement.totalbundles(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    bundle = sqlDr["count"].ToString();
        //sqlDr.Close();
        //int qty = 0;
        //int reduce = 0;
        //string pcs = "0";
        //sqlDr = sqlstatement.totalpcs(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //{
        //    qty = int.Parse(sqlDr["qty"].ToString());
        //    reduce = int.Parse(sqlDr["reduce"].ToString());
        //}
        //sqlDr.Close();
        //pcs = (qty - reduce).ToString();
        //string garment = "0";
        //sqlDr = sqlstatement.totalgarment(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    if (sqlDr["QTY"].ToString() != "")
        //        garment = sqlDr["QTY"].ToString();
        //sqlDr.Close();
        //result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"PART\": \""+parts+"\", \"TOTALPCS\": \"" + pcs + "\" , \"TOTALGARMENTPCS\": \""+garment+"\", \"ISPROCESS\": \"" + isprocess + "\", \"CONTRACTNO\": \"" + contractno + "\"}]}";

        result = result + GetPcsSummary(sqlCon, docno, userbarcode, "T")  + ", \"PART\": \"" + parts + "\", \"ISPROCESS\": \"" + isprocess + "\", \"CONTRACTNO\": \"" + contractno + "\"}]}";
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String DCtoSEWscan(string userbarcode, string docno, string factory, string svTYPE, string process, string nextprocess, string barcode, string nextproduction, string productionselect, string fromfactoryselected, string fromprocessselected, string fromproductionselected, string fromfactory, string dbtransdelete)
    {
        string result = "{ \"bundles\": [{";

        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Transactionsql transactionsql = new Transactionsql();
        SqlDataReader sqlDr;

        //检查裁片是否已经配套完成
        sqlDr = transactionsql.checkifmatching(sqlCon, barcode);
        if (sqlDr.Read())
        {
            sqlDr.Close();
            sqlCon.Close();
            return "false9";//还没有配套完成
        }
        sqlDr.Close();

        //判断该bundle part是否在箱内
        sqlDr = transactionsql.isincartonnopart(sqlCon, barcode);
        while (sqlDr.Read())
        {
            if (sqlDr["CARTON_STATUS"].ToString() == "C")
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false3";//加载箱内所有裁片
            }
        }
        sqlDr.Close();

        string isproduction = "true";
        string isfirst = "false";
        string wrongproduction = "NA";

        //判断该bundle part是否在本部门
        string isprocess = "true";
        sqlDr = transactionsql.isinmyprocessnopart(sqlCon, barcode, process);
        if (sqlDr.Read())
        {
            isprocess = "false";
        }
        sqlDr.Close();

        if (isprocess == "true")
        {
            //判断箱内所有JO是否需要选择下道组别
            sqlDr = transactionsql.needtoselectproductioncarton(sqlCon, factory, barcode, nextprocess, "K", "I", factory);
            while (sqlDr.Read())
                if (sqlDr["WIP_CONTROL_BY_PRD_LINE"].ToString() == "Y" && nextproduction == productionselect)
                {
                    sqlDr.Close();
                    sqlCon.Close();
                    return "false1";//要求用户先选择组别
                }
            sqlDr.Close();
        }

        //查找未扫描的bundle part
        Sqlstatement sqlstatement = new Sqlstatement();
        string html = "";

        try
        {
            transactionsql.unscaninsert(sqlCon, barcode, docno, userbarcode, "transaction");
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error1";
        }

        int m = 0;
        try
        {
            sqlDr = transactionsql.unscanbundle(sqlCon, docno, userbarcode);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error3";
        }
        while (sqlDr.Read())
        {
            if (fromfactoryselected == fromfactory)
            {
                fromfactoryselected = sqlDr["FACTORY_CD"].ToString();
                fromprocessselected = sqlDr["PROCESS_CD"].ToString();
                fromproductionselected = sqlDr["PRODUCTION_LINE_CD"].ToString();
                isfirst = "true";
                result += "\"FACTORY\": \"" + sqlDr["FACTORY_CD"].ToString() + "\", \"PROCESS\": \"" + sqlDr["PROCESS_CD"].ToString() + "\", \"PRODUCTION\": \"" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "\", ";
            }
            else
            {
                if (sqlDr["PRODUCTION_LINE_CD"].ToString() != fromproductionselected)
                {
                    wrongproduction = sqlDr["PRODUCTION_LINE_CD"].ToString();
                    isproduction = "false";
                }
            }
            if (sqlDr["PROCESS_CD"].ToString() != process || sqlDr["PROCESS_TYPE"].ToString() != "I")
                isprocess = "false";
            string carton_barcode = "";
            if (sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                //carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
                carton_barcode = "";
            if (m == 0)
                html += sqlDr["BARCODE"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["CUT_QTY"] + "|" + int.Parse(sqlDr["OUTPUT"].ToString()).ToString() + "|" + sqlDr["DISCREPANCY_QTY"];
            else
                html += "@" + sqlDr["BARCODE"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["CUT_QTY"] + "|" + int.Parse(sqlDr["OUTPUT"].ToString()).ToString() + "|" + sqlDr["DISCREPANCY_QTY"];
            m++;
        }
        sqlDr.Close();

        result += "\"ISFIRST\": \"" + isfirst + "\", \"ISPRODUCTION\": \"" + isproduction + "\", \"WRONGPRODUCTION\": \"" + wrongproduction + "\", \"HTML\": \"" + html + "\" }],\"bundlestatus\": [{ ";
        
        //所有的裁片都非提交状态
        string issubmit = "false";
        sqlDr = transactionsql.checkcartonsubmit(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            issubmit = "true";
        sqlDr.Close();

        sqlDr = transactionsql.checkreprint(sqlCon, docno, userbarcode);
        string canreprint = "false";
        int i = 0;
        while (sqlDr.Read())
        {
            if (sqlDr["DOC_NO"].ToString() != "0")
                i++;
        }
        sqlDr.Close();
        if (i == 1)
            canreprint = "true";
        
        //获取totalbundle和totalpcs
        //string bundle = "0";
        //sqlDr = sqlstatement.totalbundlesbybundle(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    bundle = sqlDr["count"].ToString();
        //sqlDr.Close();
        //int qty = 0;
        //int reduce = 0;
        //string pcs = "0";
        //sqlDr = sqlstatement.totalpcs(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //{
        //    qty = int.Parse(sqlDr["qty"].ToString());
        //    reduce = int.Parse(sqlDr["reduce"].ToString());
        //}
        //sqlDr.Close();
        //pcs = (qty - reduce).ToString();
        //string garment = "0";
        //sqlDr = sqlstatement.totalgarment(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    if (sqlDr["QTY"].ToString() != "")
        //        garment = sqlDr["QTY"].ToString();
        //sqlDr.Close();
        //result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\" , \"TOTALGARMENTPCS\": \""+garment+"\", \"ISPROCESS\": \"" + isprocess + "\", \"ISSUBMIT\": \"" + issubmit + "\", \"CANREPRINT\": \""+canreprint+"\"}]}";
        result = result + GetPcsSummary(sqlCon, docno, userbarcode, "T") + ", \"ISPROCESS\": \"" + isprocess + "\", \"ISSUBMIT\": \"" + issubmit + "\", \"CANREPRINT\": \"" + canreprint + "\"}]}";
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Adjustmentscan(string lastfactory, string lastprocess, string lastproduction, string userbarcode, string docno, string factory, string svTYPE, string process, string nextprocess, string barcode, string part, string nextproduction, string productionselect, string fromfactoryselected, string fromprocessselected, string fromproductionselected, string fromfactory, string dbtransdefect, string dbtransdelete, string get_othercarton)
    {
        string result = "";
        string[] partarray = part.Split(new char[] { ',' });
        part = "";
        for (int i = 0; i < partarray.Length; i++)
        {
            if (i == 0) part += "'" + partarray[i] + "'";
            else part += ",'" + partarray[i] + "'";
        }
        part = "(" + part + ")";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        Transactionsql transactionsql = new Transactionsql();
        SqlDataReader sqlDr;
        SqlCommand cmd;

        if (get_othercarton == "F")
        {
            //判断该bundle part是否在箱内
            if (barcode.Length == 14)
            {
                sqlDr = transactionsql.isincarton(sqlCon, barcode, part);
                while (sqlDr.Read())
                {
                    if (sqlDr["CARTON_STATUS"].ToString() == "C")
                    {
                        sqlDr.Close();
                        sqlCon.Close();
                        return "false3";
                    }
                }
                sqlDr.Close();
            }
        }
        //获取接收部门
        if (barcode.Length == 14)
            if(process=="SEW")
                sqlstatement.unscaninsertbybundle(sqlCon, "Transaction", docno, userbarcode, barcode);
            else
                sqlstatement.unscaninsert(sqlCon, "Transaction", docno, userbarcode, barcode, part, get_othercarton);
        else if (barcode.Length == 16)
            transactionsql.unscanneddocnoinsert(sqlCon, docno, userbarcode, barcode.Substring(3), "Transaction", get_othercarton,"");
        else
            sqlstatement.unscaninsert(sqlCon, docno, userbarcode, "Transaction", barcode, get_othercarton,false,"");

        CheckBundleStatus cbs = new CheckBundleStatus();
        cbs.setisneedsubmit(true);//裁片是否是提交状态
        cbs.setisneedbundleprocess(true);//裁片是否在本部门
        cbs.setisneedonesend(true);//裁片是否只有一个发送部门组别
        cbs.setisneedonereceive(true);//裁片是否只有一个接收部门组别
        cbs.setisneedonecutgarmenttype(true);//裁片是否是同一个裁床garment_type
        cbs.setisneedonesewgarmenttype(true);//裁片是否是同一个车缝garment_type
        if (process == "SEW")
        {
            cbs.setisneedfullbundle(true);//裁片的幅位是否齐全
        }
        if (barcode.Length == 14)
        {
            cbs.setisneedcarton(true);
        }

        if (!cbs.checkfunc(sqlCon, docno, userbarcode, factory, process))
        {
            sqlCon.Close();
            return "error1";
        }

        if ((cbs.iscarton == "false" && barcode.Length == 14) || cbs.issubmit == "false" || cbs.isfullbundle == "false" || cbs.isbundleprocess == "false" || cbs.isonereceive == "false" || cbs.isonesend == "false" || cbs.isonecutgarmenttype == "false" || cbs.isonesewgarmenttype == "false")
        {
            //把扫描的delete掉
            sqlstatement.deletescanned(sqlCon, docno, userbarcode, barcode);
            result = "[{ \"STATUS\":\"N\", \"ISCARTON\":\"" + cbs.iscarton + "\", \"ISSUBMIT\":\"" + cbs.issubmit + "\", \"ISFULLBUNDLE\":\"" + cbs.isfullbundle + "\", \"ISPROCESS\":\"" + cbs.isbundleprocess + "\", \"ISONERECEIVE\":\"" + cbs.isonereceive + "\", \"ISONESEND\":\"" + cbs.isonesend + "\", \"ISONECUTGARMENTTYPE\":\"" + cbs.isonecutgarmenttype + "\", \"ISONESEWGARMENTTYPE\":\"" + cbs.isonesewgarmenttype + "\" }]";
            sqlCon.Close();
            return result;
        }
        else
        {
            int n = 0;
            if (process == "SEW")
                sqlDr = sqlstatement.bundlescannedbybundle(sqlCon, docno, userbarcode);
            else
            {
                sqlDr = sqlstatement.bundlescannedbypart(sqlCon, docno, userbarcode);
            }
            string html = "";
            string parts = "";
            string car_no = "";
            while (sqlDr.Read())
            {
                string carton_barcode = "";
                if (sqlDr["CARTON_STATUS"].ToString() != "C")
                    carton_barcode = "";
                else
                    carton_barcode = sqlDr["CARTON_BARCODE"].ToString();

                
                if (process != "SEW")
                {
                    car_no = sqlDr["CAR_NO"].ToString();
                    if (n == 0)
                        parts = sqlDr["PART_DESC"].ToString();
                    else
                        parts += "," + sqlDr["PART_DESC"].ToString();
                }

                int output = 0;
                output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());

                if (process == "SEW")
                {
                    if (n == 0)
                        html += sqlDr["BARCODE"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
                    else
                        html += "@" + sqlDr["BARCODE"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
                }
                else
                {
                    if (n == 0)
                        html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"];
                    else
                        html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"];
                }
                n++;
            }
            result += "[{ \"STATUS\":\"Y\" ,\"CARNO\":\"" + car_no + "\" ,\"RECEIVEFACTORY\":\"" + cbs.receivefactory + "\", \"RECEIVEPROCESS\":\"" + cbs.receiveprocess + "\", \"RECEIVEPRODUCTION\":\"" + cbs.receiveproduction + "\", \"SENDFACTORY\":\"" + cbs.sendfactory + "\", \"SENDPROCESS\":\"" + cbs.sendprocess + "\", \"SENDPRODUCTION\":\"" + cbs.sendproduction + "\", \"PART\":\"" + parts + "\", \"HTML\":\"" + html + "\", ";
            sqlDr.Close();

            //已经全部扫描
            if (n == 0)
            {
                sqlCon.Close();
                return "false2";//扫描为空
            }

            sqlDr = transactionsql.checkreprint(sqlCon, docno, userbarcode);
            string canreprint = "false";
            int j = 0;
            while (sqlDr.Read())
            {
                if (sqlDr["DOC_NO"].ToString() != "0")
                    j++;
            }
            sqlDr.Close();
            if (j == 1)
                canreprint = "true";

            //获取totalbundle和totalpcs
            //string bundle = "0";
            //string pcs = "0";
            //sqlDr = sqlstatement.totalbundles(sqlCon, docno, userbarcode);
            //if (sqlDr.Read())
            //    bundle = sqlDr["count"].ToString();
            //sqlDr.Close();
            //int qty = 0;
            //int reduce = 0;
            //sqlDr = sqlstatement.totalpcs(sqlCon, docno, userbarcode);
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
            //sqlDr = sqlstatement.totalgarment(sqlCon, docno, userbarcode);
            //if (sqlDr.Read())
            //    if (sqlDr["QTY"].ToString() != "")
            //        garment = sqlDr["QTY"].ToString();
            //sqlDr.Close();

            //result += "\"TOTALBUNDLES\":\"" + bundle + "\", \"TOTALPCS\":\"" + pcs + "\", \"TOTALGARMENTPCS\":\"" + garment + "\", \"CANREPRINT\":\"" + canreprint + "\" }]";
            result = result + GetPcsSummary(sqlCon, docno, userbarcode, "T") + ", \"CANREPRINT\":\"" + canreprint + "\" }]";
            sqlCon.Close();
            return result;
        }
    }

    [WebMethod]
    public static String Bundlescan(string userbarcode, string docno, string factory, string svTYPE, string process, string nextprocess, string barcode, string part, string nextproduction, string productionselect, string fromfactoryselected, string fromprocessselected, string fromproductionselected, string fromfactory, string dbtransdefect, string dbtransdelete, string get_othercarton)
    {
        string result = "{ \"bundles\": [{";
        string[] partarray = part.Split(new char[] { ',' });
        part = "";
        for (int i = 0; i < partarray.Length; i++)
        {
            if (i == 0) part += "'" + partarray[i] + "'";
            else part += ",'" + partarray[i] + "'";
        }
        part = "(" + part + ")";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Transactionsql transactionsql = new Transactionsql();
        SqlDataReader sqlDr;

        //判断该bundle part是否在箱内
        sqlDr = transactionsql.isincarton(sqlCon, barcode, part);
        while (sqlDr.Read())
        {
            if (sqlDr["CARTON_STATUS"].ToString() == "C")
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false3";
            }
        }
        sqlDr.Close();

        string isproduction = "true";
        string isfirst = "false";
        string wrongproduction = "NA";

        //判断该bundle part是否在本部门
        string isprocess = "true";
        sqlDr = transactionsql.isinmyprocess(sqlCon, barcode, part, process);
        if (sqlDr.Read())
        {
            isprocess = "false";
        }
        sqlDr.Close();

        if (isprocess == "true")
        {
            //判断箱内所有JO是否需要选择下道组别
            sqlDr = transactionsql.needtoselectproductioncarton(sqlCon, factory, barcode, nextprocess, "K", "I", factory);
            while (sqlDr.Read())
                if (sqlDr["WIP_CONTROL_BY_PRD_LINE"].ToString() == "Y" && nextproduction == productionselect)
                {
                    sqlDr.Close();
                    sqlCon.Close();
                    return "false1";//要求用户先选择组别
                }
            sqlDr.Close();
        }
        
        //查找未扫描的bundle part
        Sqlstatement sqlstatement = new Sqlstatement();
        //int n = 0;
        //string sql = "";
        //sqlDr = sqlstatement.unscan(sqlCon, barcode, part, docno, userbarcode);
        string html = "";
        string parts = "";
        int m = 0;
        try
        {
            sqlDr = sqlstatement.partdesc(sqlCon, part);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error9";
        }
        while (sqlDr.Read())
        {
            if (m == 0)
                parts = sqlDr["PART_DESC"].ToString();
            else
                parts += "," + sqlDr["PART_DESC"].ToString();
            m++;
        }
        sqlDr.Close();

        //while (sqlDr.Read())
        //{
        //    if (fromfactoryselected == fromfactory)
        //    {
        //        fromfactoryselected = sqlDr["FACTORY_CD"].ToString();
        //        fromprocessselected = sqlDr["PROCESS_CD"].ToString();
        //        fromproductionselected = sqlDr["PRODUCTION_LINE_CD"].ToString();
        //        isfirst = "true";
        //        result += "\"FACTORY\": \"" + sqlDr["FACTORY_CD"].ToString() + "\", \"PROCESS\": \"" + sqlDr["PROCESS_CD"].ToString() + "\", \"PRODUCTION\": \"" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "\", ";
        //    }
        //    else
        //    {
        //        if (sqlDr["PRODUCTION_LINE_CD"].ToString() != fromproductionselected)
        //        {
        //            wrongproduction = sqlDr["PRODUCTION_LINE_CD"].ToString();
        //            isproduction = "false";
        //        }
        //    }
        //    if (n == 0)
        //        sql += "('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Transaction',getdate())";
        //    else
        //        sql += ",('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Transaction',getdate())";
        //    string carton_barcode = "";
        //    if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
        //        carton_barcode = "";
        //    else
        //        carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
        //    if (n == 0)
        //        parts = sqlDr["PART_DESC"].ToString();
        //    else
        //        parts += "," + sqlDr["PART_DESC"].ToString();
        //    html += "<tr id='" + sqlDr["BUNDLE_ID"] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:10%;'>" + carton_barcode + "</td>";
        //    int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
        //    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["PROCESS_CD"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["BARCODE"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["PART_DESC"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["BUNDLE_NO"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["LAY_NO"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["COLOR_CD"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:3%;'>" + sqlDr["SIZE_CD"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["QTY"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + output.ToString() + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["DISCREPANCY_QTY"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["DEFECT"] + "</td>";
        //    html += "<td><a data-role='button' onclick='defectbtn(this)' href='Reducedialog.aspx' data-rel='dialog'>" + dbtransdefect + "</a></td>";
        //    html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtransdelete + "' /></td></tr>";
        //    n++;
        //}
        //sqlDr.Close();

        //已经全部扫描
        //if (n == 0)
        //{
        //    sqlCon.Close();
        //    return "false2";//已全部扫描过
        //}
        //if (sql != "" && isproduction == "true")
        //{
        //    //把未扫描的bundle part添加到扫描表
        //    sqlstatement.unscaninsert(sqlCon, sql);
        //}

        try
        {
            sqlstatement.unscaninsert(sqlCon, "Transaction", docno, userbarcode, barcode, part,get_othercarton);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error1";
        }

        m = 0;
        try
        {
            sqlDr = sqlstatement.scanned(sqlCon, docno, userbarcode);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error3";
        }
        while (sqlDr.Read())
        {
            if (fromfactoryselected == fromfactory)
            {
                fromfactoryselected = sqlDr["FACTORY_CD"].ToString();
                fromprocessselected = sqlDr["PROCESS_CD"].ToString();
                fromproductionselected = sqlDr["PRODUCTION_LINE_CD"].ToString();
                isfirst = "true";
                result += "\"FACTORY\": \"" + sqlDr["FACTORY_CD"].ToString() + "\", \"PROCESS\": \"" + sqlDr["PROCESS_CD"].ToString() + "\", \"PRODUCTION\": \"" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "\", ";
            }
            else
            {
                if (sqlDr["PRODUCTION_LINE_CD"].ToString() != fromproductionselected)
                {
                    wrongproduction = sqlDr["PRODUCTION_LINE_CD"].ToString();
                    isproduction = "false";
                }
            }
            if (sqlDr["PROCESS_CD"].ToString() != process || sqlDr["PROCESS_TYPE"].ToString() != "I")
                isprocess = "false";
            string carton_barcode = "";
            if (sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
 
            int output = 0;
            output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
            
            if (m == 0)
                html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
            else
                html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
            m++;
        }
        sqlDr.Close();

        result += "\"PART\": \"" + parts + "\", \"ISFIRST\": \"" + isfirst + "\", \"ISPRODUCTION\": \"" + isproduction + "\", \"WRONGPRODUCTION\": \"" + wrongproduction + "\", \"HTML\": \"" + html + "\" }],\"bundlestatus\": [{ ";
        
        //所有的裁片都非提交状态
        string issubmit = "false";
        sqlDr = transactionsql.checkcartonsubmit(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            issubmit = "true";
        sqlDr.Close();

        sqlDr = transactionsql.checkreprint(sqlCon, docno, userbarcode);
        string canreprint = "false";
        int l = 0;
        while (sqlDr.Read())
        {
            if (sqlDr["DOC_NO"].ToString() != "0")
                l++;
        }
        sqlDr.Close();
        if (l == 1)
            canreprint = "true";

        //获取totalbundle和totalpcs
        //string bundle = "0";
        //string pcs = "0";
        //sqlDr = sqlstatement.totalbundles(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    bundle = sqlDr["count"].ToString();
        //sqlDr.Close();
        //int qty = 0;
        //int reduce = 0;
        //sqlDr = sqlstatement.totalpcs(sqlCon, docno, userbarcode);
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
        //sqlDr = sqlstatement.totalgarment(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    if (sqlDr["QTY"].ToString() != "")
        //        garment = sqlDr["QTY"].ToString();
        //sqlDr.Close();
        //result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\" , \"TOTALGARMENTPCS\": \""+garment+"\", \"ISPROCESS\": \"" + isprocess + "\", \"ISSUBMIT\": \"" + issubmit + "\", \"CANREPRINT\": \""+canreprint+"\"}]}";

        result = result + GetPcsSummary(sqlCon, docno, userbarcode, "T") + ", \"ISPROCESS\": \"" + isprocess + "\", \"ISSUBMIT\": \"" + issubmit + "\", \"CANREPRINT\": \"" + canreprint + "\"}]}";

        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String DCtoSEWcartonscan(string userbarcode, string docno, string factory, string svTYPE, string process, string nextprocess, string barcode, string nextproduction, string productionselect, string fromfactoryselected, string fromprocessselected, string fromproductionselected, string fromfactory, string dbtransdefect, string dbtransdelete,string get_othercarton)
    {
        string result = "{ \"bundles\": [{";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        Transactionsql transactionsql = new Transactionsql();
        SqlDataReader sqlDr;

        string isprocess = "";
        string statuscarton = "";
        string process_cd = "";
        string process_type = "";

        if (get_othercarton == "F")
        {
            sqlDr = sqlstatement.cartonstatus(sqlCon, barcode);
            
            if (sqlDr.Read())
            {
                statuscarton = sqlDr["CARTON_STATUS"].ToString();
                process_cd = sqlDr["PROCESS_CD"].ToString();
                process_type = sqlDr["PROCESS_TYPE"].ToString();
            }
            else
            {
                sqlCon.Close();
                return "false1";//该箱码不存在
            }
            sqlDr.Close();

            //检查裁片是否已经配套完成
            sqlDr = transactionsql.checkifcartonmatching(sqlCon, barcode);
            if (sqlDr.Read())
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false9";//还没有配套完成
            }
            sqlDr.Close();

            //箱子是否在本部门
            
            if (process_cd == process && (process_type == "I"||process_type == "T"))
                isprocess = "true";
            else
                isprocess = "false";

            if (isprocess == "true")
            {
                //判断箱内所有JO是否需要选择下道组别
                sqlDr = transactionsql.needtoselectproductioncarton(sqlCon, factory, barcode, nextprocess, "K", "I", factory);
                while (sqlDr.Read())
                    if (sqlDr["WIP_CONTROL_BY_PRD_LINE"].ToString() == "Y" && nextproduction == productionselect)
                    {
                        sqlDr.Close();
                        sqlCon.Close();
                        return "false2";//要求用户先选择组别
                    }
                sqlDr.Close();
            }
        } //end of if (get_othercarton=="F")

        string html = "";
        if (statuscarton != "C" && get_othercarton == "F")
        {
            sqlCon.Close();
            return "false4";//该箱码为开箱状态
        }
        else
        {
            //箱内未扫描的bundle barcode
            //string sql = "";
            //sqlDr = sqlstatement.unscancartondctosew(sqlCon, barcode, docno, userbarcode);
            //int j = 0;
            string isfirst = "false";
            string wrongproduction = "NA";
            string isproduction = "true";
            //while (sqlDr.Read())
            //{
            //    if (fromfactoryselected != factory)
            //    {
            //        fromfactoryselected = sqlDr["FACTORY_CD"].ToString();
            //        fromprocessselected = sqlDr["PROCESS_CD"].ToString();
            //        fromproductionselected = sqlDr["PRODUCTION_LINE_CD"].ToString();
            //        isfirst = "true";
            //        result += "\"FACTORY\": \"" + sqlDr["FACTORY_CD"].ToString() + "\", \"PROCESS\": \"" + sqlDr["PROCESS_CD"].ToString() + "\", \"PRODUCTION\": \"" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "\", ";
            //    }
            //    else
            //    {
            //        if (sqlDr["PRODUCTION_LINE_CD"].ToString() != fromproductionselected)
            //        {
            //            wrongproduction = sqlDr["PRODUCTION_LINE_CD"].ToString();
            //            isproduction = "false";
            //        }
            //    }
            //    string carton_barcode = "";
            //    if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
            //        carton_barcode = "";
            //    else
            //        carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
            //    html += "<tr id='" + sqlDr["BARCODE"] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + carton_barcode + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["PROCESS_CD"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["BARCODE"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:7%;'>" + sqlDr["BUNDLE_NO"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:7%;'>" + sqlDr["LAY_NO"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:7%;'>" + sqlDr["COLOR_CD"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:7%;'>" + sqlDr["SIZE_CD"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:7%;'>" + sqlDr["CUT_QTY"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:7%;'>" + sqlDr["OUTPUT"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:7%;'>" + sqlDr["DISCREPANCY_QTY"] + "</td>";
            //    html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtransdelete + "' /></td></tr>";
            //    j++;
            //}
            //sqlDr.Close();

            //添加到扫描表

            try
            {
                transactionsql.unscaninsertcarton(sqlCon, barcode, docno, userbarcode, "transaction");
            }
            catch (Exception ex)
            {
                sqlCon.Close();
                return "error1";
            }

            int m = 0;
            try
            {
                sqlDr = transactionsql.unscanbundle(sqlCon, docno, userbarcode);
            }
            catch (Exception ex)
            {
                sqlCon.Close();
                return "error3";
            }
            while (sqlDr.Read())
            {
                if (fromfactoryselected != factory)
                {
                    fromfactoryselected = sqlDr["FACTORY_CD"].ToString();
                    fromprocessselected = sqlDr["PROCESS_CD"].ToString();
                    fromproductionselected = sqlDr["PRODUCTION_LINE_CD"].ToString();
                    isfirst = "true";
                    result += "\"FACTORY\": \"" + sqlDr["FACTORY_CD"].ToString() + "\", \"PROCESS\": \"" + sqlDr["PROCESS_CD"].ToString() + "\", \"PRODUCTION\": \"" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "\", ";
                }
                else
                {
                    if (sqlDr["PRODUCTION_LINE_CD"].ToString() != fromproductionselected)
                    {
                        wrongproduction = sqlDr["PRODUCTION_LINE_CD"].ToString();
                        isproduction = "false";
                    }
                }
                if (sqlDr["PROCESS_CD"].ToString() != process || sqlDr["PROCESS_TYPE"].ToString() != "I")
                    isprocess = "false";
                string carton_barcode = barcode;
                if (m == 0)
                    html += sqlDr["BARCODE"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["CUT_QTY"] + "|" + int.Parse(sqlDr["OUTPUT"].ToString()).ToString() + "|" + sqlDr["DISCREPANCY_QTY"];
                else
                    html += "@" + sqlDr["BARCODE"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["CUT_QTY"] + "|" + int.Parse(sqlDr["OUTPUT"].ToString()).ToString() + "|" + sqlDr["DISCREPANCY_QTY"];
                m++;
            }
            sqlDr.Close();

            result += "\"ISFIRST\": \"" + isfirst + "\", \"ISPRODUCTION\": \"" + isproduction + "\", \"WRONGPRODUCTION\": \"" + wrongproduction + "\", \"HTML\": \"" + html + "\" }], \"cartonstatus\": [{ ";
            
            //所有的裁片都非提交状态
            string issubmit = "false";
            sqlDr = transactionsql.checkcartonsubmit(sqlCon, docno, userbarcode);
            if (sqlDr.Read())
                issubmit = "true";
            sqlDr.Close();

            sqlDr = transactionsql.checkreprint(sqlCon, docno, userbarcode);
            string canreprint = "false";
            int i = 0;
            while (sqlDr.Read())
            {
                if (sqlDr["DOC_NO"].ToString() != "0")
                    i++;
            }
            sqlDr.Close();
            if (i == 1)
                canreprint = "true";

            //获取totalbundle和totalpcs
            //string bundle = "";
            //sqlDr = sqlstatement.totalbundlesbybundle(sqlCon, docno, userbarcode);
            //if (sqlDr.Read())
            //    bundle = sqlDr["count"].ToString();
            //sqlDr.Close();
            //int qty = 0;
            //int reduce = 0;
            //string pcs = "";
            //sqlDr = sqlstatement.totalpcs(sqlCon, docno, userbarcode);
            //if (sqlDr.Read())
            //{
            //    qty = int.Parse(sqlDr["qty"].ToString());
            //    reduce = int.Parse(sqlDr["reduce"].ToString());
            //}
            //sqlDr.Close();
            //pcs = (qty - reduce).ToString();
            //string garment = "0";
            //sqlDr = sqlstatement.totalgarment(sqlCon, docno, userbarcode);
            //if (sqlDr.Read())
            //    if (sqlDr["QTY"].ToString() != "")
            //        garment = sqlDr["QTY"].ToString();
            //sqlDr.Close();
            //result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \"" + garment + "\", \"ISPROCESS\": \"" + isprocess + "\", \"ISSUBMIT\": \"" + issubmit + "\", \"CANREPRINT\": \""+canreprint+"\"}]}";
            result = result + GetPcsSummary(sqlCon, docno, userbarcode, "T") + ", \"ISPROCESS\": \"" + isprocess + "\", \"ISSUBMIT\": \"" + issubmit + "\", \"CANREPRINT\": \"" + canreprint + "\"}]}";
            sqlCon.Close();
            return result;
        }
    }

    [WebMethod]
    public static String DCtoSEWdocnoscan(string userbarcode, string docno, string factory, string svTYPE, string process, string nextprocess, string barcode, string nextproduction, string productionselect, string fromfactoryselected, string fromprocessselected, string fromproductionselected, string fromfactory, string dbtransdefect, string dbtransdelete, string get_othercarton)
    {
        string result = "{ \"bundles\": [{";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        Transactionsql transactionsql = new Transactionsql();
        SqlDataReader sqlDr;

        //插入流水单的裁片到扫描表
        sqlstatement.unscandocnoinsert(sqlCon, docno, userbarcode, barcode, "transaction");

        CheckBundleStatus cbs = new CheckBundleStatus();
        cbs.setisneedmatching(true);//裁片是否已经配套
        cbs.setisneedsubmit(true);//裁片是否是提交状态
        cbs.setisneedbundleprocess(true);//裁片是否在本部门
        cbs.setisneedonesend(true);//裁片是否只有一个发送部门组别
        cbs.setisneedonecutgarmenttype(true);//裁片是否是同一个裁床garment_type
        cbs.setisneedonesewgarmenttype(true);//裁片是否是同一个车缝garment_type
        cbs.setisneedfullbundle(true);//裁片的幅位是否齐全

        if (!cbs.checkfunc(sqlCon, docno, userbarcode, factory, process))
        {
            sqlCon.Close();
            return "error1";
        }

       // if (cbs.issubmit == "false" || cbs.isfullbundle == "false" || cbs.isbundleprocess == "false" || cbs.isonereceive == "false" || cbs.isonesend == "false" || cbs.isonecutgarmenttype == "false" || cbs.isonesewgarmenttype == "false")
        //tangyh 2017.04.05
        if (cbs.isfullbundle == "false" || cbs.isbundleprocess == "false" || cbs.isonereceive == "false" || cbs.isonesend == "false" || cbs.isonecutgarmenttype == "false" || cbs.isonesewgarmenttype == "false")
        {
            //把扫描的delete掉
            sqlstatement.deletescanned(sqlCon, docno, userbarcode, barcode);
            result = "[{ \"STATUS\":\"N\", \"ISCARTON\":\"" + cbs.iscarton + "\", \"ISCARTONPROCESS\":\"" + cbs.iscartonprocess + "\", \"ISFULLCARTON\":\"" + cbs.isfullcarton + "\", \"ISMATCHING\":\"" + cbs.ismatching + "\", \"ISSUBMIT\":\"" + cbs.issubmit + "\", \"ISFULLBUNDLE\":\"" + cbs.isfullbundle + "\", \"ISBUNDLEPROCESS\":\"" + cbs.isbundleprocess + "\", \"ISONERECEIVE\":\"" + cbs.isonereceive + "\", \"ISONESEND\":\"" + cbs.isonesend + "\", \"ISONECUTGARMENTTYPE\":\"" + cbs.isonecutgarmenttype + "\", \"ISONESEWGARMENTTYPE\":\"" + cbs.isonesewgarmenttype + "\" }]";
            sqlCon.Close();
            return result;
        }

        //箱子是否在本部门
        string isprocess = "true";
        

        string html = "";
        string isfirst = "false";
        string wrongproduction = "NA";
        string isproduction = "true";

        //添加到扫描表
        try
        {
            //transactionsql.unscaninsertcarton(sqlCon, barcode, docno, userbarcode, "transaction");
            //tangyh 2017.04.05
            transactionsql.unDOCinsertDOC(sqlCon,barcode.Substring(3), docno, userbarcode, "transaction");
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error1";
        }

        int m = 0;
        try
        {
            sqlDr = transactionsql.unscanbundle(sqlCon, docno, userbarcode);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error3";
        }
        while (sqlDr.Read())
        {
            if (fromfactoryselected != factory)
            {
                fromfactoryselected = sqlDr["FACTORY_CD"].ToString();
                fromprocessselected = sqlDr["PROCESS_CD"].ToString();
                fromproductionselected = sqlDr["PRODUCTION_LINE_CD"].ToString();
                isfirst = "true";
                result += "\"FACTORY\": \"" + sqlDr["FACTORY_CD"].ToString() + "\", \"PROCESS\": \"" + sqlDr["PROCESS_CD"].ToString() + "\", \"PRODUCTION\": \"" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "\", ";
            }
            else
            {
                if (sqlDr["PRODUCTION_LINE_CD"].ToString() != fromproductionselected)
                {
                    wrongproduction = sqlDr["PRODUCTION_LINE_CD"].ToString();
                    isproduction = "false";
                }
            }
            if (sqlDr["PROCESS_CD"].ToString() != process || sqlDr["PROCESS_TYPE"].ToString() != "I")
                isprocess = "false";
            string carton_barcode = "";
            if (sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                //carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
                carton_barcode = "";
            if (m == 0)
                html += sqlDr["BARCODE"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["CUT_QTY"] + "|" + int.Parse(sqlDr["OUTPUT"].ToString()).ToString() + "|" + sqlDr["DISCREPANCY_QTY"];
            else
                html += "@" + sqlDr["BARCODE"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["CUT_QTY"] + "|" + int.Parse(sqlDr["OUTPUT"].ToString()).ToString() + "|" + sqlDr["DISCREPANCY_QTY"];
            m++;
        }
        sqlDr.Close();

        result += "\"ISFIRST\": \"" + isfirst + "\", \"ISPRODUCTION\": \"" + isproduction + "\", \"WRONGPRODUCTION\": \"" + wrongproduction + "\", \"HTML\": \"" + html + "\" }], \"cartonstatus\": [{ ";

        //所有的裁片都非提交状态
        string issubmit = "false";
        sqlDr = transactionsql.checkcartonsubmit(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            issubmit = "true";
        sqlDr.Close();

        sqlDr = transactionsql.checkreprint(sqlCon, docno, userbarcode);
        string canreprint = "false";
        int i = 0;
        while (sqlDr.Read())
        {
            if (sqlDr["DOC_NO"].ToString() != "0")
                i++;
        }
        sqlDr.Close();
        if (i == 1)
            canreprint = "true";

        //获取totalbundle和totalpcs
        //string bundle = "";
        //sqlDr = sqlstatement.totalbundlesbybundle(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    bundle = sqlDr["count"].ToString();
        //sqlDr.Close();
        //int qty = 0;
        //int reduce = 0;
        //string pcs = "";
        //sqlDr = sqlstatement.totalpcs(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //{
        //    if (sqlDr["qty"].ToString() != "")
        //        qty = int.Parse(sqlDr["qty"].ToString());
        //    if (sqlDr["reduce"].ToString() != "")
        //    reduce = int.Parse(sqlDr["reduce"].ToString());
        //}
        //sqlDr.Close();
        //pcs = (qty - reduce).ToString();
        //string garment = "0";
        //sqlDr = sqlstatement.totalgarment(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    if (sqlDr["QTY"].ToString() != "")
        //        garment = sqlDr["QTY"].ToString();
        //sqlDr.Close();
        //result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \"" + garment + "\", \"ISPROCESS\": \"" + isprocess + "\", \"ISSUBMIT\": \"" + issubmit + "\", \"CANREPRINT\": \"" + canreprint + "\"}]}";
        result = result + GetPcsSummary(sqlCon, docno, userbarcode, "T") + ", \"ISPROCESS\": \"" + isprocess + "\", \"ISSUBMIT\": \"" + issubmit + "\", \"CANREPRINT\": \"" + canreprint + "\"}]}";
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Cartonscan(string userbarcode, string docno, string factory, string svTYPE, string process, string nextprocess, string barcode, string nextproduction, string productionselect, string fromfactoryselected, string fromprocessselected, string fromproductionselected, string fromfactory, string dbtransdefect, string dbtransdelete, string get_othercarton, string part)
    {
        string result = "{ \"bundles\": [{";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        Transactionsql transactionsql = new Transactionsql();
        SqlDataReader sqlDr;
        string statuscarton = "";
        string process_cd = "";
        string process_type = "";
        string isprocess = "";
        if (get_othercarton == "F")
        {
            sqlDr = sqlstatement.cartonstatus(sqlCon, barcode);

            //if (sqlDr.Read())
            //{
            //    statuscarton = sqlDr["CARTON_STATUS"].ToString();
            //    process_cd = sqlDr["PROCESS_CD"].ToString();
            //    process_type = sqlDr["PROCESS_TYPE"].ToString();
            //}
            //else
            //{
            //    sqlCon.Close();
            //    return "false1";//该箱码不存在
            //}
            //sqlDr.Close();

            bool hasrecord = false;
            while (sqlDr.Read())
            {
                statuscarton = sqlDr["CARTON_STATUS"].ToString();
                process_cd = sqlDr["PROCESS_CD"].ToString();
                process_type = sqlDr["PROCESS_TYPE"].ToString();
                if (statuscarton != "C")
                {
                    sqlCon.Close();
                    return "false4";//该箱码为开箱状态
                }
                hasrecord = true;
            }
            if (hasrecord == false)
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false1";//该箱码不存在
            }
            else
            {
                sqlDr.Close();
            }

            //箱子是否在本部门
            
            if (process_cd == process && (process_type == "I"||process_type == "T"))
                isprocess = "true";
            else
                isprocess = "false";

            if (isprocess == "true")
            {
                //判断箱内所有JO是否需要选择下道组别
                sqlDr = transactionsql.needtoselectproductioncarton(sqlCon, factory, barcode, nextprocess, "K", "I", factory);
                while (sqlDr.Read())
                    if (sqlDr["WIP_CONTROL_BY_PRD_LINE"].ToString() == "Y" && nextproduction == productionselect)
                    {
                        sqlDr.Close();
                        sqlCon.Close();
                        return "false2";//要求用户先选择组别
                    }
                sqlDr.Close();
            }
        } //end of if (get_othercarton == "F")

        string html = "";
        if ((statuscarton != "C")&&(get_othercarton == "F"))
        {
            sqlCon.Close();
            return "false4";//该箱码为开箱状态
        }
        else
        {
            //箱内未扫描的bundle barcode
            //string sql = "";
            //sqlDr = sqlstatement.unscanbundlepart(sqlCon, barcode, docno, userbarcode);
            //int j = 0;
            string isfirst = "false";
            string wrongproduction = "NA";
            string isproduction = "true";
            //while (sqlDr.Read())
            //{
            //    if (fromfactoryselected != factory)
            //    {
            //        fromfactoryselected = sqlDr["FACTORY_CD"].ToString();
            //        fromprocessselected = sqlDr["PROCESS_CD"].ToString();
            //        fromproductionselected = sqlDr["PRODUCTION_LINE_CD"].ToString();
            //        isfirst = "true";
            //        result += "\"FACTORY\": \"" + sqlDr["FACTORY_CD"].ToString() + "\", \"PROCESS\": \"" + sqlDr["PROCESS_CD"].ToString() + "\", \"PRODUCTION\": \"" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "\", ";
            //    }
            //    else
            //    {
            //        if (sqlDr["PRODUCTION_LINE_CD"].ToString() != fromproductionselected)
            //        {
            //            wrongproduction = sqlDr["PRODUCTION_LINE_CD"].ToString();
            //            isproduction = "false";
            //        }
            //    }
            //    if (j == 0)
            //        sql += "('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Transaction',getdate())";
            //    else
            //        sql += ",('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Transaction',getdate())";
            //    string carton_barcode = "";
            //    if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
            //        carton_barcode = "";
            //    else
            //        carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
            //    html += "<tr id='" + sqlDr["BUNDLE_ID"] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:10%;'>" + carton_barcode + "</td>";
            //    int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
            //    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["PROCESS_CD"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["BARCODE"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["PART_DESC"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["BUNDLE_NO"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["LAY_NO"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["COLOR_CD"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:3%;'>" + sqlDr["SIZE_CD"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["QTY"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + output.ToString() + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["DISCREPANCY_QTY"] + "</td>";
            //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["DEFECT"] + "</td>";
            //    html += "<td><a data-role='button' onclick='defectbtn(this)' href='Reducedialog.aspx' data-rel='dialog'>" + dbtransdefect + "</a></td>";
            //    html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtransdelete + "' /></td></tr>";
            //    j++;
            //}
            //sqlDr.Close();

            ////把未扫描的插到扫描表里
            //if (sql != "")
            //    sqlstatement.unscaninsert(sqlCon, sql);
            //else
            //{
            //    //已经全部扫描过了
            //    sqlCon.Close();
            //    return "false3";
            //}

            try
            {
                sqlstatement.unscaninsert(sqlCon, docno, userbarcode, "Transaction", barcode, get_othercarton,false,part);
            }
            catch (Exception ex)
            {
                sqlCon.Close();
                return "error1";
            }

            int m = 0;
            try
            {
                sqlDr = sqlstatement.scanned(sqlCon, docno, userbarcode);
            }
            catch (Exception ex)
            {
                sqlCon.Close();
                return "error3";
            }
            while (sqlDr.Read())
            {
                if (fromfactoryselected != factory)
                {
                    fromfactoryselected = sqlDr["FACTORY_CD"].ToString();
                    fromprocessselected = sqlDr["PROCESS_CD"].ToString();
                    fromproductionselected = sqlDr["PRODUCTION_LINE_CD"].ToString();
                    isfirst = "true";
                    result += "\"FACTORY\": \"" + sqlDr["FACTORY_CD"].ToString() + "\", \"PROCESS\": \"" + sqlDr["PROCESS_CD"].ToString() + "\", \"PRODUCTION\": \"" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "\", ";
                }
                else
                {
                    if (sqlDr["PRODUCTION_LINE_CD"].ToString() != fromproductionselected)
                    {
                        wrongproduction = sqlDr["PRODUCTION_LINE_CD"].ToString();
                        isproduction = "false";
                    }
                }
                if (sqlDr["PROCESS_CD"].ToString() != process || sqlDr["PROCESS_TYPE"].ToString() != "I")
                    isprocess = "false";
                string carton_barcode = "";
                if (sqlDr["CARTON_STATUS"].ToString() != "C")
                    carton_barcode = "";
                else
                    carton_barcode = sqlDr["CARTON_BARCODE"].ToString();

                int output = 0;
           
                output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());

                if (m == 0)
                    html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
                else
                    html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
                m++;
            }
            sqlDr.Close();

            result += "\"ISFIRST\": \"" + isfirst + "\", \"ISPRODUCTION\": \"" + isproduction + "\", \"WRONGPRODUCTION\": \"" + wrongproduction + "\", \"HTML\": \"" + html + "\" }], \"cartonstatus\": [{ ";
            
            //所有的裁片都非提交状态
            string issubmit = "false";
            sqlDr = transactionsql.checkcartonsubmit(sqlCon, docno, userbarcode);
            if (sqlDr.Read())
                issubmit = "true";
            sqlDr.Close();

            sqlDr = transactionsql.checkreprint(sqlCon, docno, userbarcode);
            string canreprint = "false";
            int i = 0;
            while (sqlDr.Read())
            {
                if (sqlDr["DOC_NO"].ToString() != "0")
                    i++;
            }
            sqlDr.Close();
            if (i == 1)
                canreprint = "true";

            //获取totalbundle和totalpcs
            //string bundle = "";
            //sqlDr = sqlstatement.totalbundles(sqlCon, docno, userbarcode);
            //if (sqlDr.Read())
            //    bundle = sqlDr["count"].ToString();
            //sqlDr.Close();
            //int qty = 0;
            //int reduce = 0;
            //string pcs = "";
            //sqlDr = sqlstatement.totalpcs(sqlCon, docno, userbarcode);
            //if (sqlDr.Read())
            //{
            //    qty = int.Parse(sqlDr["qty"].ToString());
            //    reduce = int.Parse(sqlDr["reduce"].ToString());
            //}
            //sqlDr.Close();
            //pcs = (qty - reduce).ToString();
            //string garment = "0";
            //sqlDr = sqlstatement.totalgarment(sqlCon, docno, userbarcode);
            //if (sqlDr.Read())
            //    if (sqlDr["QTY"].ToString() != "")
            //        garment = sqlDr["QTY"].ToString();
            //sqlDr.Close();
            //result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \""+garment+"\", \"ISPROCESS\": \"" + isprocess + "\", \"ISSUBMIT\": \"" + issubmit + "\", \"CANREPRINT\": \""+canreprint+"\"}]}";
            result = result + GetPcsSummary(sqlCon, docno, userbarcode, "T") + ", \"ISPROCESS\": \"" + isprocess + "\", \"ISSUBMIT\": \"" + issubmit + "\", \"CANREPRINT\": \"" + canreprint + "\"}]}";
            sqlCon.Close();
            return result;
        }
    }

    [WebMethod]
    public static String OASconfirm(string factory, string svTYPE, string process, string production,string nextprocess, string contractno, string processtype, string docno, string userbarcode,string carno="")
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        string date = DateTime.Today.ToString("yyyyMMdd");

        //调用存储过程
        SqlCommand cmd = new SqlCommand("CIPMS_OAS_TRANSFER", sqlCon);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@DOCNO", SqlDbType.NVarChar);
        cmd.Parameters.Add("@DATE", SqlDbType.NVarChar);
        cmd.Parameters.Add("@FACTORY", SqlDbType.NVarChar);
        cmd.Parameters.Add("@PROCESS", SqlDbType.NVarChar);
        cmd.Parameters.Add("@PRODUCTION", SqlDbType.NVarChar);
        cmd.Parameters.Add("@NEXTPROCESS", SqlDbType.NVarChar);
        cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
        cmd.Parameters.Add("@CONTRACTNO", SqlDbType.VarChar);
        cmd.Parameters.Add("@BARCODETYPE", SqlDbType.NVarChar);
        cmd.Parameters.Add("@CAR_NO", SqlDbType.NVarChar,10); 
        cmd.Parameters.Add("@NEW_DOC_NO", SqlDbType.NVarChar,30);
        cmd.Parameters["@NEW_DOC_NO"].Direction = ParameterDirection.Output;
        cmd.Parameters["@DOCNO"].Value = docno;
        cmd.Parameters["@DATE"].Value = date;
        cmd.Parameters["@FACTORY"].Value = factory;
        cmd.Parameters["@PROCESS"].Value = process;
        cmd.Parameters["@PRODUCTION"].Value = production;
        cmd.Parameters["@NEXTPROCESS"].Value = nextprocess;
        cmd.Parameters["@USERBARCODE"].Value = userbarcode;
        cmd.Parameters["@CONTRACTNO"].Value = contractno;
        cmd.Parameters["@BARCODETYPE"].Value = "docno";
        cmd.Parameters["@CAR_NO"].Value = carno;
        cmd.ExecuteNonQuery();
        string result = cmd.Parameters["@NEW_DOC_NO"].Value.ToString();

        sqlCon.Close();
        return result;
    }
    
    [WebMethod]
    public static String Delete(string factory, string svTYPE, string process, string nextprocess, string docno, string userbarcode, string id, string bundlebarcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        Transactionsql transfersql = new Transactionsql();
        SqlDataReader sqlDr;

        //判断下道部门是不是by bundle
        string bybundle = "false";
        sqlDr = transfersql.checkifbybundle(sqlCon,process, nextprocess);//changed by lijer
        if (sqlDr.Read())
            bybundle = "true";
        sqlDr.Close();

        string html = "";
        //删除扫描表的该bundle part
        if (bybundle == "false")
        {
            sqlstatement.deletepart(sqlCon, docno, userbarcode, id);
            int m = 0;
            try
            {
                sqlDr = sqlstatement.scanned(sqlCon, docno, userbarcode);
            }
            catch (Exception ex)
            {
                sqlCon.Close();
                return "error3";
            }
            while (sqlDr.Read())
            {
                string carton_barcode = "";
                if (sqlDr["CARTON_STATUS"].ToString() != "C")
                    carton_barcode = "";
                else
                    carton_barcode = sqlDr["CARTON_BARCODE"].ToString();

                int output = 0;
        
                output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
                
                if (m == 0)
                    html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
                else
                    html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
                m++;
            }
            sqlDr.Close();
        }
        else
        {
            sqlstatement.deletebundle(sqlCon, docno, userbarcode, bundlebarcode);
            int m = 0;
            try
            {
                sqlDr = transfersql.unscanbundle(sqlCon, docno, userbarcode);
            }
            catch (Exception ex)
            {
                sqlCon.Close();
                return "error3";
            }
            while (sqlDr.Read())
            {
                string carton_barcode = "";
                if (sqlDr["CARTON_STATUS"].ToString() != "C")
                    carton_barcode = "";
                else
                    carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
                if (m == 0)
                    html += sqlDr["BARCODE"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["CUT_QTY"] + "|" + (int.Parse(sqlDr["OUTPUT"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString())).ToString() + "|" + sqlDr["DISCREPANCY_QTY"];
                else
                    html += "@" + sqlDr["BARCODE"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["CUT_QTY"] + "|" + (int.Parse(sqlDr["OUTPUT"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString())).ToString() + "|" + sqlDr["DISCREPANCY_QTY"];
                m++;
            }
            sqlDr.Close();
        }

        sqlDr = transfersql.checkreprint(sqlCon, docno, userbarcode);
        string canreprint = "false";
        int i = 0;
        while (sqlDr.Read())
        {
            if (sqlDr["DOC_NO"].ToString() != "0")
                i++;
        }
        sqlDr.Close();
        if (i == 1)
            canreprint = "true";

        //获取totalbundle和totalpcs
        //string bundle = "0";
        //sqlDr = sqlstatement.totalbundles(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    bundle = sqlDr["count"].ToString();
        //if(bundle == "0")
        //{
        //    sqlCon.Close();
        //    return "false1";//全部删除了
        //}
        //sqlDr.Close();
        //int qty = 0;
        //int reduce = 0;
        //string pcs = "0";
        //sqlDr = sqlstatement.totalpcs(sqlCon, docno, userbarcode);
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
        //sqlDr = sqlstatement.totalgarment(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    if (sqlDr["QTY"].ToString() != "")
        //        garment = sqlDr["QTY"].ToString();
        //sqlDr.Close();
        //string result = "{ \"bundlestatus\": [{\"HTML\": \"" + html + "\", \"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \"" + garment + "\", ";

                                                                 //result = "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALGARMENTPCS\": \"" + garment + "\", \"TOTALPART\": \"" + partbundle + "\", \"TOTALPCS\": \"" + pcs + "\"";
          string result = "{ \"bundlestatus\": [{\"HTML\": \"" + html + "\","+ GetPcsSummary(sqlCon, docno, userbarcode, "T");


        //判断是否满足流转
        //所有bundle part的process都在本部门
        string inprocess = "true";
        sqlDr = sqlstatement.trandeletejudge2(sqlCon, docno, userbarcode, process);
        if (sqlDr.Read())
        {
            if (int.Parse(sqlDr["NUMBER"].ToString()) > 1)
                inprocess = "false";
        }
        sqlDr.Close();

        //所有的裁片都非提交状态
        string issubmit = "false";
        sqlDr = transfersql.checkcartonsubmit(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            issubmit = "true";
        sqlDr.Close();

        //箱内的bundle part齐全
        string all = "true";
        sqlDr = sqlstatement.trandeletejudge1(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
        {
            all = "false";
        }
        sqlDr.Close();
        result += ",\"INPROCESS\": \"" + inprocess + "\", \"ALL\": \"" + all + "\", \"BYBUNDLE\": \"" + bybundle + "\", \"ISSUBMIT\": \"" + issubmit + "\", \"CANREPRINT\": \""+canreprint+"\"}]}";

        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Deletecarton(string factory, string svTYPE, string process, string nextprocess, string docno, string userbarcode, string cartonbarcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        Transactionsql transfersql = new Transactionsql();
        SqlDataReader sqlDr;

        //判断下道部门是不是by bundle
        string bybundle = "false";
        sqlDr = transfersql.checkifbybundle(sqlCon, process, nextprocess);//change by lijer
        if (sqlDr.Read())
            bybundle = "true";
        sqlDr.Close();

        string html = "";
        //删除该carton的所有bundle part
        sqlstatement.trandeletecarton(sqlCon, docno, userbarcode, cartonbarcode);
        if (bybundle == "false")
        {
            int m = 0;
            try
            {
                sqlDr = sqlstatement.scanned(sqlCon, docno, userbarcode);
            }
            catch (Exception ex)
            {
                sqlCon.Close();
                return "error3";
            }
            while (sqlDr.Read())
            {
                string carton_barcode = "";
                if (sqlDr["CARTON_STATUS"].ToString() != "C")
                    carton_barcode = "";
                else
                    carton_barcode = sqlDr["CARTON_BARCODE"].ToString();

                int output = 0;
                output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
                
                if (m == 0)
                    html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
                else
                    html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DEFECT"];
                m++;
            }
            sqlDr.Close();
        }
        else
        {
            int m = 0;
            try
            {
                sqlDr = transfersql.unscanbundle(sqlCon, docno, userbarcode);
            }
            catch (Exception ex)
            {
                sqlCon.Close();
                return "error3";
            }
            while (sqlDr.Read())
            {
                string carton_barcode = "";
                if (sqlDr["CARTON_STATUS"].ToString() != "C")
                    carton_barcode = "";
                else
                    carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
                if (m == 0)
                    html += sqlDr["BARCODE"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["CUT_QTY"] + "|" + (int.Parse(sqlDr["OUTPUT"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString())).ToString() + "|" + sqlDr["DISCREPANCY_QTY"];
                else
                    html += "@" + sqlDr["BARCODE"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["CUT_QTY"] + "|" + (int.Parse(sqlDr["OUTPUT"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString())).ToString() + "|" + sqlDr["DISCREPANCY_QTY"];
                m++;
            }
            sqlDr.Close();
        }

        //获取totalbundle和totalpcs
        //string bundle = "0";
        //sqlDr = sqlstatement.totalbundles(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    bundle = sqlDr["count"].ToString();
        //if(bundle=="0")
        //{
        //    sqlCon.Close();
        //    return "false1";//全部删除了
        //}
        //sqlDr.Close();
        //int qty = 0;
        //int reduce = 0;
        //string pcs = "0";
        //sqlDr = sqlstatement.totalpcs(sqlCon, docno, userbarcode);
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
        //sqlDr = sqlstatement.totalgarment(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    if (sqlDr["QTY"].ToString() != "")
        //        garment = sqlDr["QTY"].ToString();
        //sqlDr.Close();
        //string result = "{ \"bundlestatus\": [{\"HTML\": \""+html+"\", \"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \""+garment+"\", ";

        //string result = "{ \"bundlestatus\": [{\"HTML\": \""+html+"\", \"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \""+garment+"\", ";
          string result = "{ \"bundlestatus\": [{\"HTML\": \""+html+"\","+ GetPcsSummary(sqlCon, docno, userbarcode, "T");


          //if (onlyfield == "T")
          //{
          //    result = "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALGARMENTPCS\": \"" + garment + "\", \"TOTALPART\": \"" + partbundle + "\", \"TOTALPCS\": \"" + pcs + "\"";
          //}
          //else
          //{
          //    result = "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALGARMENTPCS\": \"" + garment + "\", \"TOTALPART\": \"" + partbundle + "\", \"TOTALPCS\": \"" + pcs + "\" }]}";
          //}

        sqlDr = transfersql.checkreprint(sqlCon, docno, userbarcode);
        string canreprint = "false";
        int i = 0;
        while (sqlDr.Read())
        {
            if (sqlDr["DOC_NO"].ToString() != "0")
                i++;
        }
        sqlDr.Close();
        if (i == 1)
            canreprint = "true";

        //判断是否满足流转
        //所有bundle part的process都在本部门
        string inprocess = "true";
        sqlDr = sqlstatement.trandeletejudge2(sqlCon, docno, userbarcode,process);
        if (sqlDr.Read())
        {
            if (int.Parse(sqlDr["NUMBER"].ToString()) > 1)
                inprocess = "false";
        }
        sqlDr.Close();

        //所有的裁片都非提交状态
        string issubmit = "false";
        sqlDr = transfersql.checkcartonsubmit(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            issubmit = "true";
        sqlDr.Close();

        //箱内的bundle part齐全
        string all = "true";
        sqlDr = sqlstatement.trandeletejudge1(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
        {
            all = "false";
        }
        sqlDr.Close();
        result += ",\"INPROCESS\": \"" + inprocess + "\", \"ALL\": \"" + all + "\", \"BYBUNDLE\": \""+bybundle+"\", \"ISSUBMIT\": \""+issubmit+"\", \"CANREPRINT\": \""+canreprint+"\"}]}";

        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Save(string userbarcode, string docno, string factory, string svTYPE, string module)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement commonsql = new Sqlstatement();
        SqlDataReader sqlDr = commonsql.savejudge(sqlCon, userbarcode, docno);
        if (!sqlDr.Read())
        {
            sqlCon.Close();
            return "false";
        }
        sqlDr.Close();
        commonsql.save(sqlCon, docno, userbarcode, module);
        commonsql.savedefect(sqlCon, docno, userbarcode);
        string result = "";
        sqlDr = commonsql.savemaxdate(sqlCon, userbarcode, module);
        if (sqlDr.Read())
            result = sqlDr["CREATE_DATE"].ToString();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Querychange(string docno, string date, string factory, string svTYPE, string process, string userbarcode, string functioncd, string dbtransdefect, string dbtransdelete)
    {
        string result = "{ \"bundles\": [";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        string sql = "";
        //未扫描的bundle barcode
        Sqlstatement sqlstatement = new Sqlstatement();
        Transactionsql transfersql = new Transactionsql();
        SqlDataReader sqlDr = sqlstatement.querychange(sqlCon, date, userbarcode, docno, functioncd);
        string html = "";
        int j = 0;
        while (sqlDr.Read())
        {
            if (j == 0)
                sql += "('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Transaction',getdate())";
            else
                sql += ",('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Transaction',getdate())";
            string carton_barcode = "";
            if (sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
            html += "<tr id='" + sqlDr["BUNDLE_ID"] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:10%;'>" + carton_barcode + "</td>";

            int output = 0;
            output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());

            html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["PROCESS_CD"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["BARCODE"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["PART_DESC"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["BUNDLE_NO"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["LAY_NO"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["COLOR_CD"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center; width:3%;'>" + sqlDr["SIZE_CD"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["QTY"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + output.ToString() + "</td>";
            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["DISCREPANCY_QTY"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["DEFECT"] + "</td>";
            html += "<td><a data-role='button' onclick='defectbtn(this)' href='Reducedialog.aspx' data-rel='dialog'>" + dbtransdefect + "</a></td>";
            html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtransdelete + "' /></td></tr>";
            j++;
        }
        sqlDr.Close();
        result += "{ \"HTML\": \"" + html + "\" }],\"bundlestatus\": [{";
        if (j == 0)
        {
            //全部已扫描
            sqlCon.Close();
            return "false1";
        }

        //把未扫描的bundle part添加到扫描表
        sqlstatement.unscaninsert(sqlCon, sql);


        //把defect添加到defect DFT表
        //sqlstatement.querydefect(sqlCon, date, userbarcode, docno);//tangyh 2017.12.21屏蔽,

        //获取totalbundle和totalpcs
        //string bundle = "0";
        //sqlDr = sqlstatement.totalbundles(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    bundle = sqlDr["count"].ToString();
        //sqlDr.Close();
        //int qty = 0;
        //int reduce = 0;
        //string pcs = "0";
        //sqlDr = sqlstatement.totalpcs(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //{
        //    qty = int.Parse(sqlDr["qty"].ToString());
        //    reduce = int.Parse(sqlDr["reduce"].ToString());
        //}
        //sqlDr.Close();
        //pcs = (qty - reduce).ToString();
        //string garment = "0";
        //sqlDr = sqlstatement.totalgarment(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    if (sqlDr["QTY"].ToString() != "")
        //        garment = sqlDr["QTY"].ToString();
        //sqlDr.Close();
        //result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \""+garment+"\", ";
        result = result + GetPcsSummary(sqlCon, docno, userbarcode, "T");

        //判断是否满足流转
        //所有bundle part的process都在本部门
        string inprocess = "true";
        sqlDr = sqlstatement.trandeletejudge2(sqlCon, docno, userbarcode,process);
        if (sqlDr.Read())
        {
            if (int.Parse(sqlDr["NUMBER"].ToString()) > 1)
                inprocess = "false";
        }
        sqlDr.Close();

        //所有的裁片都非提交状态
        string issubmit = "false";
        sqlDr = transfersql.checkcartonsubmit(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            issubmit = "true";
        sqlDr.Close();

        sqlDr = transfersql.checkreprint(sqlCon, docno, userbarcode);
        string canreprint = "false";
        int i = 0;
        while (sqlDr.Read())
        {
            if (sqlDr["DOC_NO"].ToString() != "0")
                i++;
        }
        sqlDr.Close();
        if (i == 1)
            canreprint = "true";

        //箱内的bundle part齐全
        string all = "true";
        sqlDr = sqlstatement.trandeletejudge1(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
        {
            all = "false";
        }
        sqlDr.Close();
        result += ",\"ISPROCESS\": \"" + inprocess + "\", \"ALL\": \"" + all + "\", \"ISSUBMIT\": \"" + issubmit + "\", \"CANREPRINT\": \"" + canreprint + "\"}]}";

        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Defectreason(string factory, string svTYPE)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr = sqlstatement.defectreason(sqlCon, factory);
        while (sqlDr.Read())
        {
            result += "<option value='" + sqlDr["REASON_CD"] + "'>" + sqlDr["REASON_DESC"] + "</option>";
        }
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Defectlist(string process, string factory, string svTYPE, string docno, string bundlebarcode, string partcd)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;

        //把部件转化成code
        string part = "";
        sqlDr = sqlstatement.parttransaction(sqlCon, partcd);
        if (sqlDr.Read())
            part = sqlDr["PART_CD"].ToString();
        sqlDr.Close();

        sqlDr = sqlstatement.defectlist(sqlCon, docno, bundlebarcode, part, factory,process);
        while (sqlDr.Read())
        {
            result += "<tr><td style='vertical-align:middle; text-align:center'>" + "<input type='checkbox' name='checkbox1' value='" + sqlDr["BUNDLE_ID"].ToString() + "'>" + "</td><td style='vertical-align:middle; text-align:center'>" + process + "</td><td style='vertical-align:middle; text-align:center'>" + bundlebarcode + "</td><td style='vertical-align:middle; text-align:center'>" + partcd + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT_QTY"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["REASON_DESC"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT_REASON_CD"] + "</td></tr>";
        }
        sqlDr.Close();
        sqlDr = sqlstatement.defectinformation(sqlCon, bundlebarcode, part);
        while (sqlDr.Read())
        {
            result += "<tr><td style='vertical-align:middle; text-align:center'>" + "<input type='checkbox' name='checkbox1' value='" + sqlDr["BUNDLE_ID"].ToString() + "'>" + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["PROCESS_CD"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["BARCODE"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["PART_DESC"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT_QTY"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["REASON_DESC"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT_REASON_CD"] + "</td></tr>";
        }
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Defectconfirm(string process, string factory, string svTYPE, string docno, string bundlebarcode, string bundlepart, string reason, string userbarcode, string qty, string bundleid)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;

        //把部件转化成code
        string partcd = "";
        sqlDr = sqlstatement.parttransaction(sqlCon, bundlepart);
        if (sqlDr.Read())
            partcd = sqlDr["PART_CD"].ToString();
        sqlDr.Close();

        sqlstatement.defectconfirm(sqlCon, docno, bundlebarcode, partcd, reason, qty, userbarcode, bundleid, process);
        sqlDr = sqlstatement.defectlist(sqlCon, docno, bundlebarcode, partcd, factory,process);
        string html = "";
        while (sqlDr.Read())
        {
            html += "<tr><td style='vertical-align:middle; text-align:center'>" + "<input type='checkbox' name='checkbox1' value='" + sqlDr["BUNDLE_ID"].ToString() + "'>" + "</td><td style='vertical-align:middle; text-align:center'>" + process + "</td><td style='vertical-align:middle; text-align:center'>" + bundlebarcode + "</td><td style='vertical-align:middle; text-align:center'>" + bundlepart + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT_QTY"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["REASON_DESC"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT_REASON_CD"] + "</td></tr>";
        }
        sqlDr.Close();
        sqlDr = sqlstatement.defectinformation(sqlCon, bundlebarcode, partcd);
        while (sqlDr.Read())
        {
            html += "<tr><td style='vertical-align:middle; text-align:center'>" + "<input type='checkbox' name='checkbox1' value='" + sqlDr["BUNDLE_ID"].ToString() + "'>" + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["PROCESS_CD"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["BARCODE"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["PART_DESC"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT_QTY"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["REASON_DESC"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT_REASON_CD"] + "</td></tr>";
        }
        sqlDr.Close();
        result = "[{\"HTML\": \"" + html + "\", ";
        //返回该bundle part defect的sum（qty）
        string sumqty = "0";
        sqlDr = sqlstatement.defectsumqty(sqlCon, docno, bundlebarcode, partcd);
        if(sqlDr.Read())
            sumqty = sqlDr["QTY"].ToString();
        sqlDr.Close();
        sqlDr = sqlstatement.defectsumqty(sqlCon, bundlebarcode,partcd);
        if (sqlDr.Read())
            sumqty = (int.Parse(sqlDr["DEFECT"].ToString())+int.Parse(sumqty)).ToString();
        sqlDr.Close();
        result += "\"QTY\":\"" + sumqty + "\"}]";
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String DefectDelete(string process, string factory, string svTYPE, string docno, string bundlebarcode, string bundlepart, string reason, string userbarcode, string qty, string bundleid)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;

        //把部件转化成code
        string partcd = "";
        sqlDr = sqlstatement.parttransaction(sqlCon, bundlepart);
        if (sqlDr.Read())
            partcd = sqlDr["PART_CD"].ToString();
        sqlDr.Close();

        if (sqlstatement.defectDelete(sqlCon, docno, bundlebarcode, partcd, reason, qty, userbarcode, bundleid, process)==false)
        {
            return result;
        }

        sqlDr = sqlstatement.defectlist(sqlCon, docno, bundlebarcode, partcd, factory,process);
        string html = "";
        while (sqlDr.Read())
        {
            html += "<tr><td style='vertical-align:middle; text-align:center'>" + "<input type='checkbox' name='checkbox1' value='" + sqlDr["BUNDLE_ID"].ToString() + "'>" + "</td><td style='vertical-align:middle; text-align:center'>" + process + "</td><td style='vertical-align:middle; text-align:center'>" + bundlebarcode + "</td><td style='vertical-align:middle; text-align:center'>" + bundlepart + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT_QTY"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["REASON_DESC"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT_REASON_CD"] + "</td></tr>";
        }
        sqlDr.Close();
        sqlDr = sqlstatement.defectinformation(sqlCon, bundlebarcode, partcd);
        while (sqlDr.Read())
        {
            html += "<tr><td style='vertical-align:middle; text-align:center'>" + "<input type='checkbox' name='checkbox1' value='" + sqlDr["BUNDLE_ID"].ToString() + "'>" + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["PROCESS_CD"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["BARCODE"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["PART_DESC"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT_QTY"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["REASON_DESC"] + "</td><td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT_REASON_CD"] + "</td></tr>";
        }
        sqlDr.Close();
        result = "[{\"HTML\": \"" + html + "\", ";
        //返回该bundle part defect的sum（qty）
        string sumqty = "0";
        sqlDr = sqlstatement.defectsumqty(sqlCon, docno, bundlebarcode, partcd);
        if (sqlDr.Read())
            sumqty = sqlDr["QTY"].ToString();
        sqlDr.Close();
        sqlDr = sqlstatement.defectsumqty(sqlCon, bundlebarcode, partcd);
        if (sqlDr.Read())
            sumqty = (int.Parse(sqlDr["DEFECT"].ToString()) + int.Parse(sumqty)).ToString();
        sqlDr.Close();
        result += "\"QTY\":\"" + sumqty + "\"}]";
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Unsubmit(string userbarcode, string docno, string factory, string svTYPE, string process, string doc_no)
    {
        doc_no = doc_no.Substring(3);
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Transactionsql transactionsql = new Transactionsql();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;

        //查询流水单是否是提交状态
        sqlDr = transactionsql.checkdocnostatus(sqlCon, doc_no);
        if (!sqlDr.Read())
        {
            sqlDr.Close();
            sqlCon.Close();
            return "false1";//该流水单已被接收或者不存在或者已被取消
        }
        sqlDr.Close();

        //所有的裁片都在该流水单
        sqlDr = transactionsql.checkdocnobundles1(sqlCon, doc_no, docno, userbarcode);
        if (sqlDr.Read())
        {
            sqlDr.Close();
            sqlCon.Close();
            return "false2";//流水单内裁片不完整或者多余了
        }
        sqlDr.Close();
        sqlDr = transactionsql.checkdocnobundles2(sqlCon, doc_no, docno, userbarcode);
        if (sqlDr.Read())
        {
            sqlDr.Close();
            sqlCon.Close();
            return "false2";
        }
        sqlDr.Close();

        //判断流水单是否在本部门提交的
        sqlDr = transactionsql.checkdocnoinprocess(sqlCon, doc_no, process);
        if (!sqlDr.Read())
        {
            sqlDr.Close();
            sqlCon.Close();
            return "false3";//流水单不在本部门提交
        }

        //将流水单的状态更改为U，bundle的状态更改为C，intrans_qty-
        transactionsql.updatedocnostatus(sqlCon, doc_no);

        sqlstatement.cleanscan(sqlCon, docno, userbarcode);

        return "success";
    }

    //SEW->CUT adjustment
    [WebMethod]
    public static String GTNadjustment(string factory, string svTYPE, string docno, string userbarcode, string thisfactory, string thisprocess, string thisproduction, string nextfactory, string processtype, string nextprocess, string nextproduction, string languagearray, string carno = "")
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlCommand cmd;
        string result = "";

        CheckBundleStatus cbs = new CheckBundleStatus();
        cbs.setisneedsubmit(true);//裁片是否是提交状态
        cbs.setisneedbundleprocess(true);//裁片是否在本部门
        cbs.setisneedonesend(true);//裁片是否只有一个发送部门组别
        cbs.setisneedonereceive(true);//裁片是否只有一个接收部门组别
        cbs.setisneedonecutgarmenttype(true);//裁片是否是同一个裁床garment_type
        cbs.setisneedonesewgarmenttype(true);//裁片是否是同一个车缝garment_type
        if (thisprocess == "SEW")
        {
            cbs.setisneedfullbundle(true);//裁片的幅位是否齐全
        }

        if (!cbs.checkfunc(sqlCon, docno, userbarcode, thisfactory, thisprocess))
        {
            sqlCon.Close();
            return "error1";
        }

        if (cbs.issubmit == "false" || cbs.isfullbundle == "false" || cbs.isbundleprocess == "false" || cbs.isonereceive == "false" || cbs.isonesend == "false" || cbs.isonecutgarmenttype == "false" || cbs.isonesewgarmenttype == "false")
        {
            result = "[{ \"STATUS\":\"N\", \"ISSUBMIT\":\"" + cbs.issubmit + "\", \"ISFULLBUNDLE\":\"" + cbs.isfullbundle + "\", \"ISPROCESS\":\"" + cbs.isbundleprocess + "\", \"ISONERECEIVE\":\"" + cbs.isonereceive + "\", \"ISONESEND\":\"" + cbs.isonesend + "\", \"ISONECUTGARMENTTYPE\":\"" + cbs.isonecutgarmenttype + "\", \"ISONESEWGARMENTTYPE\":\"" + cbs.isonesewgarmenttype + "\" }]";
            sqlCon.Close();
            return result;
        }
        else
        {
            //GTN流转存储过程进行裁片流转
            string date = DateTime.Now.ToString("yyyyMMdd");
            string newdocno = "";
            try
            {
                cmd = new SqlCommand("CIPMS_GTN_TRANSFER", sqlCon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.Parameters.Add("@SENDFACTORY", SqlDbType.NVarChar);
                cmd.Parameters.Add("@SENDPROCESS", SqlDbType.NVarChar);
                cmd.Parameters.Add("@SENDPRODUCTION", SqlDbType.NVarChar);
                cmd.Parameters.Add("@RECEIVEFACTORY", SqlDbType.NVarChar);
                cmd.Parameters.Add("@RECEIVEPROCESS", SqlDbType.NVarChar);
                cmd.Parameters.Add("@RECEIVEPRODUCTION", SqlDbType.NVarChar);
                cmd.Parameters.Add("@DOCNO", SqlDbType.VarChar);
                cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
                cmd.Parameters.Add("@DATE", SqlDbType.NVarChar);
                cmd.Parameters.Add("@CUTGARMENTTYPE", SqlDbType.NChar);
                cmd.Parameters.Add("@SEWGARMENTTYPE", SqlDbType.NChar);
                cmd.Parameters.Add("@RECEIVEPROCESSTYPE", SqlDbType.NChar);
                cmd.Parameters.Add("@TRANSACTIONTYPE", SqlDbType.NVarChar);
                cmd.Parameters.Add("@CAR_NO", SqlDbType.NVarChar,10);
                cmd.Parameters.Add("@NEW_DOC_NO", SqlDbType.NVarChar, 30);
                cmd.Parameters.Add("@IS_EXCEED_WIP", SqlDbType.NChar, 1);
                cmd.Parameters["@NEW_DOC_NO"].Direction = ParameterDirection.Output;
                cmd.Parameters["@IS_EXCEED_WIP"].Direction = ParameterDirection.Output;
                cmd.Parameters["@SENDFACTORY"].Value = thisfactory;
                cmd.Parameters["@SENDPROCESS"].Value = thisprocess;
                cmd.Parameters["@SENDPRODUCTION"].Value = thisproduction;
                cmd.Parameters["@RECEIVEFACTORY"].Value = nextfactory;
                cmd.Parameters["@RECEIVEPROCESS"].Value = nextprocess;
                cmd.Parameters["@RECEIVEPRODUCTION"].Value = nextproduction;
                cmd.Parameters["@DOCNO"].Value = docno;
                cmd.Parameters["@USERBARCODE"].Value = userbarcode;
                cmd.Parameters["@DATE"].Value = date;
                cmd.Parameters["@CUTGARMENTTYPE"].Value = cbs.cutgarmenttype;
                cmd.Parameters["@SEWGARMENTTYPE"].Value = cbs.sewgarmenttype;
                cmd.Parameters["@RECEIVEPROCESSTYPE"].Value = processtype;
                cmd.Parameters["@TRANSACTIONTYPE"].Value = "adjustment";
                cmd.Parameters["@CAR_NO"].Value = carno;
                cmd.ExecuteNonQuery();
                if (cmd.Parameters["@IS_EXCEED_WIP"].Value.ToString() == "N")
                {
                    newdocno = cmd.Parameters["@NEW_DOC_NO"].Value.ToString();
                }
                else
                {
                    string iswipexceed = cmd.Parameters["@IS_EXCEED_WIP"].Value.ToString();
                    sqlCon.Close();
                    return "false2";
                }
            }
            catch (Exception ex)
            {
                sqlCon.Close();
                return "error2";
            }

            if (newdocno == "N")
            {
                sqlCon.Close();
                return "error9";
            }
            else
            {
                string doc_no = newdocno;
                newdocno = factory + newdocno;

                Docnoproduce docnoprint = new Docnoproduce();
                try
                {
                    result = docnoprint.Docnostring(sqlCon, userbarcode, doc_no, factory, thisprocess, thisproduction, nextprocess, nextproduction, languagearray, "N");
                }
                catch (Exception ex)
                {
                    sqlCon.Close();
                    return "error3";
                }

                sqlCon.Close();
                return "[{ \"STATUS\":\"Y\", \"HTML\":\"" + result + "\", \"DOCNO\":\"" + newdocno + "\" }]";
            }
        }
    }
    
    //CUT->SEW confirm
    [WebMethod]
    public static String GTNconfirm(string factory, string svTYPE, string nextfactory, string processtype, string process, string nextprocess, string productionline, string nextproductionline, string docno, string userbarcode, string languagearray, string carno = "")
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlCommand cmd;
        string result = "";

        CheckBundleStatus cbs = new CheckBundleStatus();
        cbs.setisneedmatching(true);//裁片是否已经配套
        cbs.setisneedsubmit(true);//裁片是否是提交状态
        cbs.setisneedbundleprocess(true);//裁片是否在本部门
        cbs.setisneedonesend(true);//裁片是否只有一个发送部门组别
        cbs.setisneedonecutgarmenttype(true);//裁片是否是同一个裁床garment_type
        cbs.setisneedonesewgarmenttype(true);//裁片是否是同一个车缝garment_type
        cbs.setisneedfullbundle(true);//裁片的幅位是否齐全

        if (!cbs.checkfunc(sqlCon, docno, userbarcode, factory, process))
        {
            sqlCon.Close();
            return "error1";
        }

        if (cbs.issubmit == "false" || cbs.isfullbundle == "false" || cbs.isbundleprocess == "false" || cbs.ismatching == "false" || cbs.isonereceive == "false" || cbs.isonesend == "false" || cbs.isonecutgarmenttype == "false" || cbs.isonesewgarmenttype == "false")
        {
            result = "[{ \"STATUS\":\"N\", \"ISCARTON\":\"" + cbs.iscarton + "\", \"ISCARTONPROCESS\":\"" + cbs.iscartonprocess + "\", \"ISFULLCARTON\":\"" + cbs.isfullcarton + "\", \"ISMATCHING\":\"" + cbs.ismatching + "\", \"ISSUBMIT\":\"" + cbs.issubmit + "\", \"ISFULLBUNDLE\":\"" + cbs.isfullbundle + "\", \"ISBUNDLEPROCESS\":\"" + cbs.isbundleprocess + "\", \"ISONERECEIVE\":\"" + cbs.isonereceive + "\", \"ISONESEND\":\"" + cbs.isonesend + "\", \"ISONECUTGARMENTTYPE\":\"" + cbs.isonecutgarmenttype + "\", \"ISONESEWGARMENTTYPE\":\"" + cbs.isonesewgarmenttype + "\" }]";
            sqlCon.Close();
            return result;
        }

        string date = DateTime.Now.ToString("yyyyMMdd");
        string newdocno = "";
        try
        {
            cmd = new SqlCommand("CIPMS_GTN_TRANSFER", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@SENDFACTORY", SqlDbType.NVarChar);
            cmd.Parameters.Add("@SENDPROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@SENDPRODUCTION", SqlDbType.NVarChar);
            cmd.Parameters.Add("@RECEIVEFACTORY", SqlDbType.NVarChar);
            cmd.Parameters.Add("@RECEIVEPROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@RECEIVEPRODUCTION", SqlDbType.NVarChar);
            cmd.Parameters.Add("@DOCNO", SqlDbType.VarChar);
            cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@DATE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@CUTGARMENTTYPE", SqlDbType.NChar);
            cmd.Parameters.Add("@SEWGARMENTTYPE", SqlDbType.NChar);
            cmd.Parameters.Add("@RECEIVEPROCESSTYPE", SqlDbType.NChar);
            cmd.Parameters.Add("@TRANSACTIONTYPE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@CAR_NO", SqlDbType.NVarChar,10);
            cmd.Parameters.Add("@NEW_DOC_NO", SqlDbType.NVarChar,30);
            cmd.Parameters.Add("@IS_EXCEED_WIP", SqlDbType.NChar,1);
            cmd.Parameters["@NEW_DOC_NO"].Direction = ParameterDirection.Output;
            cmd.Parameters["@IS_EXCEED_WIP"].Direction = ParameterDirection.Output;

            cmd.Parameters["@SENDFACTORY"].Value = factory;
            cmd.Parameters["@SENDPROCESS"].Value = process;
            cmd.Parameters["@SENDPRODUCTION"].Value = productionline;
            cmd.Parameters["@RECEIVEFACTORY"].Value = nextfactory;
            cmd.Parameters["@RECEIVEPROCESS"].Value = nextprocess;
            cmd.Parameters["@RECEIVEPRODUCTION"].Value = nextproductionline;
            cmd.Parameters["@DOCNO"].Value = docno;
            cmd.Parameters["@USERBARCODE"].Value = userbarcode;
            cmd.Parameters["@DATE"].Value = date;
            cmd.Parameters["@CUTGARMENTTYPE"].Value = cbs.cutgarmenttype;
            cmd.Parameters["@SEWGARMENTTYPE"].Value = cbs.sewgarmenttype;
            cmd.Parameters["@RECEIVEPROCESSTYPE"].Value = processtype;
            cmd.Parameters["@TRANSACTIONTYPE"].Value = "transaction";
            cmd.Parameters["@CAR_NO"].Value = carno;
            cmd.ExecuteNonQuery();
            if (cmd.Parameters["@IS_EXCEED_WIP"].Value.ToString() == "N")
            {
                newdocno = cmd.Parameters["@NEW_DOC_NO"].Value.ToString();
            }
            else
            {
                sqlCon.Close();
                return "false2";
            }
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error2"+ex.Message;
        }

        if (newdocno == "N")
        {
            sqlCon.Close();
            return "error9";
        }
        else
        {
            newdocno = cmd.Parameters["@NEW_DOC_NO"].Value.ToString();
            string doc_no = newdocno;
            newdocno = factory + newdocno;
            string html = "";

            Docnoproduce docnoprint = new Docnoproduce();
            try
            {
                html = docnoprint.Docnostring(sqlCon, userbarcode, doc_no, factory, process, productionline, nextprocess, nextproductionline, languagearray, "N");
            }
            catch (Exception ex)
            {
                sqlCon.Close();
                result = "[{ \"STATUS\":\"V\", \"DOCNO\": \"" + newdocno + "\" }]";
            }

            sqlCon.Close();
            result = "[{ \"STATUS\":\"Y\", \"HTML\": \"" + html + "\", \"DOCNO\": \"" + newdocno + "\" }]";
            return result;
        }
    }

    //Process type 为I 的transaction submit
    [WebMethod]
    public static String Transactionsubmit(string username, string factory, string svTYPE, string nextfactory, string process, string nextprocess, string productionline, string nextproductionline, string productionselect, string docno, string userbarcode, string languagearray)
    {
        if (nextproductionline == productionselect)
            nextproductionline = "NA";

        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        string result = "";

        CheckBundleStatus cbs = new CheckBundleStatus();
        if (nextprocess == "SEW")
        {
            cbs.setisneedmatching(true);//裁片是否已经配套
            cbs.setisneedonesewgarmenttype(true);//裁片是否是同一个车缝garment_type
            cbs.setisneedfullbundle(true);//裁片的幅位是否齐全
        }
        cbs.setisneedsubmit(true);//裁片是否是提交状态
        cbs.setisneedbundleprocess(true);//裁片是否在本部门
        cbs.setisneedonesend(true);//裁片是否只有一个发送部门组别
        cbs.setisneedonecutgarmenttype(true);//裁片是否是同一个裁床garment_type

        if (!cbs.checkfunc(sqlCon, docno, userbarcode, factory, process))
        {
            sqlCon.Close();
            return "error1";
        }

        if (cbs.issubmit == "false" || cbs.isfullbundle == "false" || cbs.isbundleprocess == "false" || cbs.isonereceive == "false" || cbs.isonesend == "false" || cbs.isonecutgarmenttype == "false" || cbs.isonesewgarmenttype == "false")
        {
            result = "[{ \"STATUS\":\"N\", \"ISCARTON\":\"" + cbs.iscarton + "\", \"ISCARTONPROCESS\":\"" + cbs.iscartonprocess + "\", \"ISFULLCARTON\":\"" + cbs.isfullcarton + "\", \"ISMATCHING\":\"" + cbs.ismatching + "\", \"ISSUBMIT\":\"" + cbs.issubmit + "\", \"ISFULLBUNDLE\":\"" + cbs.isfullbundle + "\", \"ISBUNDLEPROCESS\":\"" + cbs.isbundleprocess + "\", \"ISONERECEIVE\":\"" + cbs.isonereceive + "\", \"ISONESEND\":\"" + cbs.isonesend + "\", \"ISONECUTGARMENTTYPE\":\"" + cbs.isonecutgarmenttype + "\", \"ISONESEWGARMENTTYPE\":\"" + cbs.isonesewgarmenttype + "\" }]";
            sqlCon.Close();
            return result;
        }

        string date = DateTime.Today.ToString("yyyyMMdd");
        string newdocno = "";
        try
        {
            //调用流转submit的存储过程，返回流转单号
            //调用存储过程
            SqlCommand cmd = new SqlCommand("CIPMS_TRANSFER_SUBMIT", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@DATE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@DOC_NO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@FACTORY", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PRODUCTION", SqlDbType.NVarChar);
            cmd.Parameters.Add("@GARMENTTYPE", SqlDbType.NChar);
            cmd.Parameters.Add("@PROCESSTYPE", SqlDbType.NChar);
            cmd.Parameters.Add("@NEXTFACTORY", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTPROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTPRODUCTION", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTGARMENTTYPE", SqlDbType.NChar);
            cmd.Parameters.Add("@NEXTPROCESSTYPE", SqlDbType.NChar);
            cmd.Parameters.Add("@STATUS", SqlDbType.NChar);
            cmd.Parameters.Add("@NEW_DOC_NO", SqlDbType.NVarChar, 30);
            cmd.Parameters["@NEW_DOC_NO"].Direction = ParameterDirection.Output;
            cmd.Parameters["@DATE"].Value = date;
            cmd.Parameters["@DOC_NO"].Value = docno;
            cmd.Parameters["@USERBARCODE"].Value = userbarcode;
            cmd.Parameters["@FACTORY"].Value = factory;
            cmd.Parameters["@PROCESS"].Value = process;
            cmd.Parameters["@PRODUCTION"].Value = productionline;
            cmd.Parameters["@GARMENTTYPE"].Value = cbs.cutgarmenttype;
            cmd.Parameters["@PROCESSTYPE"].Value = "I";
            cmd.Parameters["@NEXTFACTORY"].Value = nextfactory;
            cmd.Parameters["@NEXTPROCESS"].Value = nextprocess;
            cmd.Parameters["@NEXTPRODUCTION"].Value = nextproductionline;
            string nextgarmenttype = "";
            if(nextprocess=="SEW")
            {
                nextgarmenttype = cbs.sewgarmenttype;
            }
            else
            {
                nextgarmenttype = cbs.cutgarmenttype;
            }
            cmd.Parameters["@NEXTGARMENTTYPE"].Value = nextgarmenttype;
            cmd.Parameters["@NEXTPROCESSTYPE"].Value = "I";
            cmd.Parameters["@STATUS"].Value = "S";
            cmd.ExecuteNonQuery();
            newdocno = cmd.Parameters["@NEW_DOC_NO"].Value.ToString();
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error2"+ex.Message;
        }

        string doc_no = newdocno;
        newdocno = factory + newdocno;

        Docnoproduce docnoprint = new Docnoproduce();
        try
        {
            result = docnoprint.Docnostring(sqlCon, userbarcode, doc_no, factory, process, productionline, nextprocess, nextproductionline, languagearray, "N");
        }
        catch (Exception ex)
        {
            return "error3" + newdocno;
        }
        finally
        {
            sqlCon.Close();
        }
        return "[{ \"STATUS\":\"Y\", \"HTML\": \"" + result + "\", \"DOCNO\": \"" + newdocno + "\" }]";
    }

    //Process Type 为I 的Rework的submit
    [WebMethod]
    public static String Reworksubmit(string STATUS, string username, string factory, string svTYPE, string nextfactory, string process, string nextprocess, string productionline, string nextproductionline, string docno, string userbarcode, string productionselect, string languagearray)
    {
        if (nextproductionline == productionselect)
            nextproductionline = "NA";

        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);

        string date = DateTime.Today.ToString("yyyyMMdd");
        string newdocno = "";

        try
        {
            //调用流转submit的存储过程，返回流转单号
            //调用存储过程
            SqlCommand cmd = new SqlCommand("CIPMS_TRANSFER_SUBMIT", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@DOC_NO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@DATE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@FACTORY", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PRODUCTION", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTFACTORY", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTPROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTPRODUCTION", SqlDbType.NVarChar);
            cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTPROCESSTYPE", SqlDbType.Char);
            cmd.Parameters.Add("@STATUS", SqlDbType.NChar);
            cmd.Parameters.Add("@NEW_DOC_NO", SqlDbType.NVarChar, 30);
            cmd.Parameters["@NEW_DOC_NO"].Direction = ParameterDirection.Output;
            cmd.Parameters["@DOC_NO"].Value = docno;
            cmd.Parameters["@DATE"].Value = date;
            cmd.Parameters["@FACTORY"].Value = factory;
            cmd.Parameters["@PROCESS"].Value = process;
            cmd.Parameters["@PRODUCTION"].Value = productionline;
            cmd.Parameters["@NEXTFACTORY"].Value = nextfactory;
            cmd.Parameters["@NEXTPROCESS"].Value = nextprocess;
            cmd.Parameters["@NEXTPRODUCTION"].Value = nextproductionline;
            cmd.Parameters["@USERBARCODE"].Value = userbarcode;
            cmd.Parameters["@NEXTPROCESSTYPE"].Value = "I";
            cmd.Parameters["@STATUS"].Value = STATUS;
            cmd.ExecuteNonQuery();
            newdocno = cmd.Parameters["@NEW_DOC_NO"].Value.ToString();
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error1";
        }
        string doc_no = newdocno;
        newdocno = factory + newdocno;

        Docnoproduce docnoprint = new Docnoproduce();
        string result = "";
        try
        {
            result = docnoprint.Docnostring(sqlCon, userbarcode, doc_no, factory, process, productionline, nextprocess, nextproductionline, languagearray, "N");
        }
        catch (Exception ex)
        {
            return "error2"+newdocno;
        }
        finally
        {
            sqlCon.Close();
        }

        return "[{ \"DOCNO\": \"" + newdocno + "\", \"HTML\": \"" + result + "\" }]";

    }

    //Process TYPE 为P 的transaction的submit
    [WebMethod]
    public static String Peertransactionsubmit(string factory, string svTYPE, string nextfactory, string process, string nextprocess, string productionline, string nextproductionline, string docno, string userbarcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        string date = DateTime.Today.ToString("yyyyMMdd");
        string processtype = "";
        if (process == nextprocess)
            processtype = "X";//Process type 为P 的平级调动
        else
            processtype = "Y";//Process type 为P 的下道部门流转

        //调用流转submit的存储过程，返回流转单号
        //调用存储过程
        SqlCommand cmd = new SqlCommand("CIPMS_TRANSFER_SUBMIT", sqlCon);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@DOC_NO", SqlDbType.NVarChar);
        cmd.Parameters.Add("@DATE", SqlDbType.NVarChar);
        cmd.Parameters.Add("@FACTORY", SqlDbType.NVarChar);
        cmd.Parameters.Add("@PROCESS", SqlDbType.NVarChar);
        cmd.Parameters.Add("@PRODUCTION", SqlDbType.NVarChar);
        cmd.Parameters.Add("@NEXTFACTORY", SqlDbType.NVarChar);
        cmd.Parameters.Add("@NEXTPROCESS", SqlDbType.NVarChar);
        cmd.Parameters.Add("@NEXTPRODUCTION", SqlDbType.NVarChar);
        cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
        cmd.Parameters.Add("@NEXTPROCESSTYPE", SqlDbType.Char);
        cmd.Parameters.Add("@STATUS", SqlDbType.NChar);
        cmd.Parameters.Add("@NEW_DOC_NO", SqlDbType.NVarChar, 30);
        cmd.Parameters["@NEW_DOC_NO"].Direction = ParameterDirection.Output;
        cmd.Parameters["@DOC_NO"].Value = docno;
        cmd.Parameters["@DATE"].Value = date;
        cmd.Parameters["@FACTORY"].Value = factory;
        cmd.Parameters["@PROCESS"].Value = process;
        cmd.Parameters["@PRODUCTION"].Value = productionline;
        cmd.Parameters["@NEXTFACTORY"].Value = nextfactory;
        cmd.Parameters["@NEXTPROCESS"].Value = nextprocess;
        cmd.Parameters["@NEXTPRODUCTION"].Value = nextproductionline;
        cmd.Parameters["@USERBARCODE"].Value = userbarcode;
        cmd.Parameters["@NEXTPROCESSTYPE"].Value = "P";
        cmd.Parameters["@STATUS"].Value = processtype;
        cmd.ExecuteNonQuery();
        string newdocno = cmd.Parameters["@NEW_DOC_NO"].Value.ToString();

        return newdocno;
    }

    //Process TYPE 为P 的rework的submit
    [WebMethod]
    public static String Peerreworksubmit(string factory, string svTYPE, string nextfactory, string process, string nextprocess, string productionline, string nextproductionline, string docno, string userbarcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;
        string date = DateTime.Today.ToString("yyyyMMdd");
        date += "-";
        List<string> trandocnolist = new List<string>();

        //获取docno的sequence并生成新的docno
        sqlDr = sqlstatement.tranconfirm1(sqlCon, factory, "docno");
        int seq = 0;
        if (sqlDr.Read())
        {
            seq = int.Parse(sqlDr["SEQUENCE_NO"].ToString());
        }
        sqlDr.Close();

        //查找裁片所在的所有production
        sqlDr = sqlstatement.selecttranproduction1(sqlCon, docno, userbarcode);
        string sql = "";
        string trandocno = "";
        int i = 0;
        string processtype = "N";
        while (sqlDr.Read())
        {
            //构造insert的sql
            trandocno = date + seq.ToString().PadLeft(4, '0');
            trandocnolist.Add(trandocno + "," + sqlDr["PRODUCTION_LINE_CD"]);
            if (i == 0)
                sql += "('" + trandocno + "','" + factory + "','" + process + "','" + nextprocess + "','" + sqlDr["PRODUCTION_LINE_CD"] + "','NA','" + processtype + "','" + userbarcode + "',getdate(),'" + userbarcode + "',getdate(),'" + userbarcode + "',getdate(),'K','K','I','P','" + factory + "','" + nextfactory + "')";
            else
                sql += ",('" + trandocno + "','" + factory + "','" + process + "','" + nextprocess + "','" + sqlDr["PRODUCTION_LINE_CD"] + "','NA','" + processtype + "','" + userbarcode + "',getdate(),'" + userbarcode + "',getdate(),'" + userbarcode + "',getdate(),'K','K','I','P','" + factory + "','" + nextfactory + "')";
            seq++;
            i++;
        }
        sqlDr.Close();

        //插入到transfer_HD
        sqlstatement.inserttransferhd1(sqlCon, sql);

        //更新docno的sequence
        sqlstatement.tranconfirm2(sqlCon, factory, seq.ToString(), "docno");

        //插入到transfer_DT
        foreach (string n in trandocnolist)
        {
            trandocno = n.Split(new char[] { ',' })[0];
            productionline = n.Split(new char[] { ',' })[1];
            sqlstatement.inserttransferdt(sqlCon, docno, userbarcode, trandocno, productionline);
        }

        //插入到bundle_defect
        i = 0;
        foreach (string n in trandocnolist)
        {
            trandocno = n.Split(new char[] { ',' })[0];
            if (i == 0)
                sql = "'" + trandocno + "'";
            else
                sql += ",'" + trandocno + "'";
            i++;
        }
        sql = "(" + sql + ")";
        sqlstatement.inserttrandefect(sqlCon, docno, userbarcode, process, sql);

        //更新bundle_for_scanning的defect
        sqlstatement.updatetrandefect(sqlCon, sql);

        //更新wip_hd的intrans_qty++
        sqlstatement.updatewiphdqty(sqlCon, sql);

        //更新wip_bundle的intrans_qty++
        sqlstatement.updatewipbundleqty(sqlCon, sql);

        //更新bundle_for_scanning的DOC_NO
        sqlstatement.updatebfsdocno(sqlCon, sql, userbarcode);

        //查找出要流转的所有JO->找出其在CUT/PRT/EMB的WIP_CONTROL->查找出Peer服务器是否已经有该数据，有就删除->把这些WIP_CONTROL保存到Peer服务器的PRD_JO_PROCESS_WIP_CONTROL
        string dblink = "MES_DB_LINK_" + nextfactory;
        string peerfactorydblink = dblink + ".MESDB.dbo.CIPMS_BUNDLE_FOR_SCANNING";
        //删除Peer表的WIP_CONTROL
        sqlstatement.peerwipcontrol(sqlCon, sql, nextfactory, peerfactorydblink);
        //查出所有JO的WIP_CONTROL
        sqlDr = sqlstatement.querywipcontrol(sqlCon, sql);
        //WIP_CONTROL处理
        int j = 0;
        string temp = "";
        while (sqlDr.Read())
        {
            if (j == 0)
            {
                if (sqlDr["PRODUCTION_FACTORY"].ToString() == nextfactory)
                    temp += "('" + nextfactory + "','" + sqlDr["JOB_ORDER_NO"] + "','" + sqlDr["PROCESS_CD"] + "','" + sqlDr["WIP_CONTROL_BY_BUNDLE"] + "','" + sqlDr["WIP_CONTROL_BY_COLOR"] + "','" + sqlDr["WIP_CONTROL_BY_SIZE"] + "','" + sqlDr["WIP_CONTROL_BY_PRD_LINE"] + "','" + sqlDr["STAGE"] + "',GETDATE()" + ",'" + sqlDr["GARMENT_TYPE"] + "','I','" + nextfactory + "')";
                else if(sqlDr["PROCESS_TYPE"].ToString() == "O")
                    temp += "('" + nextfactory + "','" + sqlDr["JOB_ORDER_NO"] + "','" + sqlDr["PROCESS_CD"] + "','" + sqlDr["WIP_CONTROL_BY_BUNDLE"] + "','" + sqlDr["WIP_CONTROL_BY_COLOR"] + "','" + sqlDr["WIP_CONTROL_BY_SIZE"] + "','" + sqlDr["WIP_CONTROL_BY_PRD_LINE"] + "','" + sqlDr["STAGE"] + "',GETDATE()" + ",'" + sqlDr["GARMENT_TYPE"] + "','O','')";
                else
                    temp += "('" + nextfactory + "','" + sqlDr["JOB_ORDER_NO"] + "','" + sqlDr["PROCESS_CD"] + "','" + sqlDr["WIP_CONTROL_BY_BUNDLE"] + "','" + sqlDr["WIP_CONTROL_BY_COLOR"] + "','" + sqlDr["WIP_CONTROL_BY_SIZE"] + "','" + sqlDr["WIP_CONTROL_BY_PRD_LINE"] + "','" + sqlDr["STAGE"] + "',GETDATE()" + ",'" + sqlDr["GARMENT_TYPE"] + "','P','" + sqlDr["FACTORY_CD"] + "')";
            }
            else
            {
                if (sqlDr["PRODUCTION_FACTORY"].ToString() == nextfactory)
                    temp += ",('" + nextfactory + "','" + sqlDr["JOB_ORDER_NO"] + "','" + sqlDr["PROCESS_CD"] + "','" + sqlDr["WIP_CONTROL_BY_BUNDLE"] + "','" + sqlDr["WIP_CONTROL_BY_COLOR"] + "','" + sqlDr["WIP_CONTROL_BY_SIZE"] + "','" + sqlDr["WIP_CONTROL_BY_PRD_LINE"] + "','" + sqlDr["STAGE"] + "',GETDATE()" + ",'" + sqlDr["GARMENT_TYPE"] + "','I','" + nextfactory + "')";
                else if (sqlDr["PROCESS_TYPE"].ToString() == "O")
                    temp += ",('" + nextfactory + "','" + sqlDr["JOB_ORDER_NO"] + "','" + sqlDr["PROCESS_CD"] + "','" + sqlDr["WIP_CONTROL_BY_BUNDLE"] + "','" + sqlDr["WIP_CONTROL_BY_COLOR"] + "','" + sqlDr["WIP_CONTROL_BY_SIZE"] + "','" + sqlDr["WIP_CONTROL_BY_PRD_LINE"] + "','" + sqlDr["STAGE"] + "',GETDATE()" + ",'" + sqlDr["GARMENT_TYPE"] + "','O','')";
                else
                    temp += ",('" + nextfactory + "','" + sqlDr["JOB_ORDER_NO"] + "','" + sqlDr["PROCESS_CD"] + "','" + sqlDr["WIP_CONTROL_BY_BUNDLE"] + "','" + sqlDr["WIP_CONTROL_BY_COLOR"] + "','" + sqlDr["WIP_CONTROL_BY_SIZE"] + "','" + sqlDr["WIP_CONTROL_BY_PRD_LINE"] + "','" + sqlDr["STAGE"] + "',GETDATE()" + ",'" + sqlDr["GARMENT_TYPE"] + "','P','" + sqlDr["FACTORY_CD"] + "')";
            }
            j++;
        }
        //把WIP_CONTROL插入到Peer的数据库
        sqlstatement.insertwipcontrol(sqlCon, temp, peerfactorydblink);

        //清空扫描表
        sqlstatement.emptylist(sqlCon, userbarcode);
        sqlCon.Close();

        //返回生成的流水单号
        i = 0;
        foreach (string n in trandocnolist)
        {
            trandocno = n.Split(new char[] { ',' })[0];
            if (i == 0)
                sql = nextfactory + trandocno;
            else
                sql += "," + nextfactory + trandocno;
            i++;
        }
        return sql;
    }
    
    //Process为I的平级调动
    [WebMethod]
    public static String Transactionconfirm1(string factory, string svTYPE, string nextfactory, string process, string nextprocess, string productionline, string nextproductionline, string docno, string userbarcode, string languagearray, string carno = "")
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);

        string date = DateTime.Today.ToString("yyyyMMdd");
        string newdocno = "";

        try
        {
            //调用流转的存储过程
            SqlCommand cmd = new SqlCommand("CIPMS_TRANSFER_CONFIRM", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@DATE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@FACTORY", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PRODUCTION", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PROCESSTYPE", SqlDbType.NChar);
            cmd.Parameters.Add("@NEXTFACTORY", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTPROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTPRODUCTION", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTPROCESSTYPE", SqlDbType.NChar);
            cmd.Parameters.Add("@DOCNO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@TRANSFERTYPE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@BARCODETYPE", SqlDbType.Char);
            cmd.Parameters.Add("@CAR_NO", SqlDbType.Char,10);
            cmd.Parameters.Add("@NEW_DOC_NO", SqlDbType.NVarChar, 30);
            cmd.Parameters["@NEW_DOC_NO"].Direction = ParameterDirection.Output;
            cmd.Parameters["@DATE"].Value = date;
            cmd.Parameters["@FACTORY"].Value = factory;
            cmd.Parameters["@PROCESS"].Value = process;
            cmd.Parameters["@PRODUCTION"].Value = productionline;
            cmd.Parameters["@PROCESSTYPE"].Value = "I";
            cmd.Parameters["@NEXTFACTORY"].Value = nextfactory;
            cmd.Parameters["@NEXTPROCESS"].Value = nextprocess;
            cmd.Parameters["@NEXTPRODUCTION"].Value = nextproductionline;
            cmd.Parameters["@NEXTPROCESSTYPE"].Value = "I";
            cmd.Parameters["@DOCNO"].Value = docno;
            cmd.Parameters["@USERBARCODE"].Value = userbarcode;
            cmd.Parameters["@BARCODETYPE"].Value = "docno";
            cmd.Parameters["@TRANSFERTYPE"].Value = "transaction";
            cmd.Parameters["@CAR_NO"].Value = carno;
            cmd.ExecuteNonQuery();
            newdocno = cmd.Parameters["@NEW_DOC_NO"].Value.ToString();
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error1";
        }

        string doc_no = newdocno;
        newdocno = factory + newdocno;
        
        Docnoproduce docnoprint = new Docnoproduce();
        string result = "";
        try
        {
            result = docnoprint.Docnostring(sqlCon, userbarcode, doc_no, factory, process, productionline, nextprocess, nextproductionline, languagearray, "N");
        }
        catch (Exception ex)
        {
            return "error2" + newdocno;
        }
        finally
        {
            sqlCon.Close();
        }

        return "[{ \"HTML\": \"" + result + "\", \"DOCNO\": \"" + newdocno + "\" }]";
    }

    //Process为I的下道部门流转
    [WebMethod]
    public static String Transactionconfirm2(string factory, string svTYPE, string nextfactory, string process, string nextprocess, string productionline, string nextproductionline, string productionselect, string docno, string userbarcode, string languagearray, string carno = "")
    {
        if (nextproductionline == productionselect)
            nextproductionline = "NA";

        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        SqlCommand cmd;
        factory = factory.Substring(0, 3);
        string result = "";

        CheckBundleStatus cbs = new CheckBundleStatus();
        cbs.setisneedsubmit(true);//裁片是否是提交状态
        cbs.setisneedbundleprocess(true);//裁片是否在本部门
        cbs.setisneedcartonprocess(true);//箱码是否在本部门
        cbs.setisneedonesend(true);//裁片是否只有一个发送部门组别
        cbs.setisneedonecutgarmenttype(true);//裁片是否是同一个裁床garment_type
        cbs.setisneedonesewgarmenttype(true);//裁片是否是同一个车缝garment_type

        if (!cbs.checkfunc(sqlCon, docno, userbarcode, factory, process))
        {
            sqlCon.Close();
            return "error1";
        }

        if (cbs.issubmit == "false" || cbs.isfullbundle == "false" || cbs.isbundleprocess == "false" || cbs.isonereceive == "false" || cbs.isonesend == "false" || cbs.isonecutgarmenttype == "false" || cbs.isonesewgarmenttype == "false")
        {
            result = "[{ \"STATUS\":\"N\", \"ISCARTON\":\""+cbs.iscarton+"\", \"ISCARTONPROCESS\":\""+cbs.iscartonprocess+"\", \"ISFULLCARTON\":\""+cbs.isfullcarton+"\", \"ISMATCHING\":\""+cbs.ismatching+"\", \"ISSUBMIT\":\"" + cbs.issubmit + "\", \"ISFULLBUNDLE\":\"" + cbs.isfullbundle + "\", \"ISBUNDLEPROCESS\":\"" + cbs.isbundleprocess + "\", \"ISONERECEIVE\":\"" + cbs.isonereceive + "\", \"ISONESEND\":\"" + cbs.isonesend + "\", \"ISONECUTGARMENTTYPE\":\"" + cbs.isonecutgarmenttype + "\", \"ISONESEWGARMENTTYPE\":\"" + cbs.isonesewgarmenttype + "\" }]";
            sqlCon.Close();
            return result;
        }

        string date = DateTime.Today.ToString("yyyyMMdd");
        string newdocno = "";
        try
        {
            //调用流转的存储过程
            cmd = new SqlCommand("CIPMS_TRANSFER_CONFIRM", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@DATE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@FACTORY", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PRODUCTION", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PROCESSTYPE", SqlDbType.NChar);
            cmd.Parameters.Add("@NEXTFACTORY", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTPROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTPRODUCTION", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTPROCESSTYPE", SqlDbType.NChar);
            cmd.Parameters.Add("@DOCNO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@TRANSFERTYPE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@BARCODETYPE", SqlDbType.Char);
            cmd.Parameters.Add("@CAR_NO", SqlDbType.Char,10);
            cmd.Parameters.Add("@NEW_DOC_NO", SqlDbType.NVarChar, 30);
            cmd.Parameters["@NEW_DOC_NO"].Direction = ParameterDirection.Output;
            cmd.Parameters["@DATE"].Value = date;
            cmd.Parameters["@FACTORY"].Value = factory;
            cmd.Parameters["@PROCESS"].Value = process;
            cmd.Parameters["@PRODUCTION"].Value = productionline;
            cmd.Parameters["@PROCESSTYPE"].Value = "I";
            cmd.Parameters["@NEXTFACTORY"].Value = nextfactory;
            cmd.Parameters["@NEXTPROCESS"].Value = nextprocess;
            cmd.Parameters["@NEXTPRODUCTION"].Value = nextproductionline;
            cmd.Parameters["@NEXTPROCESSTYPE"].Value = "I";
            cmd.Parameters["@DOCNO"].Value = docno;
            cmd.Parameters["@USERBARCODE"].Value = userbarcode;
            cmd.Parameters["@BARCODETYPE"].Value = "docno";
            cmd.Parameters["@TRANSFERTYPE"].Value = "transaction";
            cmd.Parameters["@CAR_NO"].Value = carno;
            cmd.ExecuteNonQuery();
            newdocno = cmd.Parameters["@NEW_DOC_NO"].Value.ToString();
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error2"+ex.Message;
        }

        string doc_no = newdocno;
        newdocno = factory + newdocno;

        //调用自动配套存储过程
        string automatching = "";
        try
        {
            cmd = new SqlCommand("CIPMS_AUTO_MATCHING", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@FACTORY_CD", SqlDbType.NVarChar);
            cmd.Parameters.Add("@SEND_PROCESS_CD", SqlDbType.NVarChar);
            cmd.Parameters.Add("@RECEIVE_PROCESS_CD", SqlDbType.NVarChar);
            cmd.Parameters.Add("@DOC_NO", SqlDbType.NChar);
            cmd.Parameters.Add("@RESULT", SqlDbType.NVarChar, 100);
            cmd.Parameters["@RESULT"].Direction = ParameterDirection.Output;
            cmd.Parameters["@FACTORY_CD"].Value = factory;
            cmd.Parameters["@SEND_PROCESS_CD"].Value = process;
            cmd.Parameters["@RECEIVE_PROCESS_CD"].Value = nextprocess;
            cmd.Parameters["@DOC_NO"].Value = doc_no;
            cmd.ExecuteNonQuery();
            automatching = cmd.Parameters["@RESULT"].Value.ToString();
        }
        catch (Exception ex)
        {
            automatching = "11";
        }

        Docnoproduce docnoprint = new Docnoproduce();
        try
        {
            string automatchingflag = "N";
            if (automatching == "1")
                automatchingflag = "Y";
            result = docnoprint.Docnostring(sqlCon, userbarcode, doc_no, factory, process, productionline, nextprocess, nextproductionline, languagearray, automatchingflag);
        }
        catch (Exception ex)
        {
            return "[{ \"STATUS\":\"V\", \"HTML\": \"" + result + "\", \"DOCNO\": \"" + newdocno + "\", \"AUTOMATCHING\":\""+automatching+"\" }]";
        }
        finally
        {
            sqlCon.Close();
        }

        return "[{ \"STATUS\":\"Y\", \"HTML\": \"" + result + "\", \"DOCNO\": \"" + newdocno + "\", \"AUTOMATCHING\":\"" + automatching + "\" }]";
    }

    //Process为I的rework的confirm
    [WebMethod]
    public static String Reworkconfirm(string factory, string svTYPE, string nextfactory, string process, string nextprocess, string productionline, string nextproductionline, string productionselect, string docno, string userbarcode, string languagearray, string carno = "")
    {
        if (nextproductionline == productionselect)
            nextproductionline = "NA";

        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);

        string date = DateTime.Today.ToString("yyyyMMdd");
        string newdocno = "";
        //调用流转rework的存储过程，返回流转单号
        //调用存储过程
        try
        {
            SqlCommand cmd = new SqlCommand("CIPMS_TRANSFER_CONFIRM", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@DATE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@FACTORY", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PRODUCTION", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PROCESSTYPE", SqlDbType.NChar);
            cmd.Parameters.Add("@NEXTFACTORY", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTPROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTPRODUCTION", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTPROCESSTYPE", SqlDbType.NChar);
            cmd.Parameters.Add("@DOCNO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@TRANSFERTYPE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@BARCODETYPE", SqlDbType.Char);
            cmd.Parameters.Add("@CAR_NO", SqlDbType.Char,10);
            cmd.Parameters.Add("@NEW_DOC_NO", SqlDbType.NVarChar, 30);
         //   cmd.Parameters.Add("@IS_WIP_EXCCEED", SqlDbType.NChar, 1);  tangyh 2017.05.22
            cmd.Parameters["@NEW_DOC_NO"].Direction = ParameterDirection.Output;
        //    cmd.Parameters["@IS_WIP_EXCCEED"].Direction = ParameterDirection.Output;
            cmd.Parameters["@DATE"].Value = date;
            cmd.Parameters["@FACTORY"].Value = factory;
            cmd.Parameters["@PROCESS"].Value = process;
            cmd.Parameters["@PRODUCTION"].Value = productionline;
            cmd.Parameters["@PROCESSTYPE"].Value = "I";
            cmd.Parameters["@NEXTFACTORY"].Value = nextfactory;
            cmd.Parameters["@NEXTPROCESS"].Value = nextprocess;
            cmd.Parameters["@NEXTPRODUCTION"].Value = nextproductionline;
            cmd.Parameters["@NEXTPROCESSTYPE"].Value = "I";
            cmd.Parameters["@DOCNO"].Value = docno;
            cmd.Parameters["@USERBARCODE"].Value = userbarcode;
            cmd.Parameters["@BARCODETYPE"].Value = "docno";
            cmd.Parameters["@TRANSFERTYPE"].Value = "rework";
            cmd.Parameters["@CAR_NO"].Value = carno;
            cmd.ExecuteNonQuery();
            //if (cmd.Parameters["@IS_WIP_EXCCEED"].Value.ToString() == "N")
            if (cmd.Parameters["@NEW_DOC_NO"].Value.ToString()!="")
            {
                newdocno = cmd.Parameters["@NEW_DOC_NO"].Value.ToString();
            }
            else
            {
                //string iswipexcceed = cmd.Parameters["@IS_WIP_EXCCEED"].Value.ToString(); tangyh 2017.05.22
                sqlCon.Close();
                //return "false4" + iswipexcceed; tangyh 2017.05.22
                return "false4";
            }
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error1";
        }
        string doc_no = newdocno;
        newdocno = factory + newdocno;

        Docnoproduce docnoprint = new Docnoproduce();
        string result = "";
        try
        {
            result = docnoprint.Docnostring(sqlCon, userbarcode, doc_no, factory, process, productionline, nextprocess, nextproductionline, languagearray, "N");
        }
        catch (Exception ex)
        {
            return "false2" + newdocno;
        }
        finally
        {
            sqlCon.Close();
        }

        sqlCon.Close();
        return "[{ \"HTML\": \"" + result + "\", \"DOCNO\": \"" + newdocno + "\" }]";
    }

    //Process为I的Adjustment
    [WebMethod]
    public static String Adjustmentconfirm(string factory, string svTYPE, string docno, string userbarcode, string thisfactory, string thisprocess, string thisproduction, string nextfactory, string nextprocess, string nextproduction, string languagearray, string carno = "")
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        SqlCommand cmd;

        string date = DateTime.Today.ToString("yyyyMMdd");
        string newdocno = "";

        try
        {
            //调用流转的存储过程
            cmd = new SqlCommand("CIPMS_TRANSFER_CONFIRM", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@DATE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@FACTORY", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PRODUCTION", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PROCESSTYPE", SqlDbType.NChar);
            cmd.Parameters.Add("@NEXTFACTORY", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTPROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTPRODUCTION", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEXTPROCESSTYPE", SqlDbType.NChar);
            cmd.Parameters.Add("@DOCNO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@TRANSFERTYPE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@BARCODETYPE", SqlDbType.Char);
            cmd.Parameters.Add("@CAR_NO", SqlDbType.Char,10);
            cmd.Parameters.Add("@NEW_DOC_NO", SqlDbType.NVarChar, 30);
            cmd.Parameters["@NEW_DOC_NO"].Direction = ParameterDirection.Output;
            cmd.Parameters["@DATE"].Value = date;
            cmd.Parameters["@FACTORY"].Value = thisfactory;
            cmd.Parameters["@PROCESS"].Value = thisprocess;
            cmd.Parameters["@PRODUCTION"].Value = thisproduction;
            cmd.Parameters["@PROCESSTYPE"].Value = "I";
            cmd.Parameters["@NEXTFACTORY"].Value = nextfactory;
            cmd.Parameters["@NEXTPROCESS"].Value = nextprocess;
            cmd.Parameters["@NEXTPRODUCTION"].Value = nextproduction;
            cmd.Parameters["@NEXTPROCESSTYPE"].Value = "I";
            cmd.Parameters["@DOCNO"].Value = docno;
            cmd.Parameters["@USERBARCODE"].Value = userbarcode;
            cmd.Parameters["@BARCODETYPE"].Value = "docno";
            cmd.Parameters["@TRANSFERTYPE"].Value = "adjustment";
            cmd.Parameters["@CAR_NO"].Value = carno;
            cmd.ExecuteNonQuery();
            newdocno = cmd.Parameters["@NEW_DOC_NO"].Value.ToString();
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error2" + ex.Message;
        }

        string doc_no = newdocno;
        newdocno = factory + newdocno;

        Docnoproduce docnoprint = new Docnoproduce();
        string result = "";
        try
        {
            result = docnoprint.Docnostring(sqlCon, userbarcode, doc_no, factory, thisprocess, thisproduction, nextprocess, nextproduction, languagearray, "N");
        }
        catch (Exception ex)
        {
            return "error2" + newdocno;
        }
        finally
        {
            sqlCon.Close();
        }

        sqlCon.Close();
        return "[{ \"HTML\": \"" + result + "\", \"DOCNO\": \"" + newdocno + "\" }]";
    }

    [WebMethod]
    public static String Emptylist(string userbarcode, string factory, string svTYPE)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        sqlstatement.tranemptylist(sqlCon, userbarcode);
        sqlCon.Close();
        return "success";
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
            result = "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALGARMENTPCS\": \"" + garment + "\", \"TOTALPART\": \"" + partbundle + "\", \"TOTALPCS\": \"" + pcs+ "\"";
        }
        else
        {
            result = "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALGARMENTPCS\": \"" + garment + "\", \"TOTALPART\": \"" + partbundle + "\", \"TOTALPCS\": \"" + pcs + "\" }]}";
        }

        return result;
    }

}
