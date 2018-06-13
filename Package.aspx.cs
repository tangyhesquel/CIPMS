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
using commonfunction;

public partial class Package : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    [WebMethod]
    public static String Accessfunction(string factory, string svTYPE, string userbarcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr = sqlstatement.useraccessfunc(sqlCon, userbarcode);
        string result = "";
        int i = 0;
        while (sqlDr.Read())
        {
            if (i == 0)
                result += "{\"FUNCTION_ENG\":\"" + sqlDr["FUNCTION_ENG"] + "\",\"MODULE\":\"" + sqlDr["MODULE"] + "\"}";
            else 
                result += ",{\"FUNCTION_ENG\":\"" + sqlDr["FUNCTION_ENG"] + "\",\"MODULE\":\"" + sqlDr["MODULE"] + "\"}";
            i++;
        }
        result = "[" + result + "]";
        sqlDr.Close();
        sqlCon.Close();
        return result;
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
        SqlDataReader sqlDr = sqlstatement.part(sqlCon, factory);
        string result = "";
        while (sqlDr.Read())
            result += "<option value='" + sqlDr["PART_CD"] + "'>" + sqlDr["PART_DESC"] + "</option>";
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Getpartlayno(string factory, string svTYPE, string jo)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Packagesql packagesql = new Packagesql();
        SqlDataReader sqlDr;
        //获取该JO的部件
        sqlDr = packagesql.part(sqlCon, jo);
        string parthtml = "";
        while (sqlDr.Read())
            parthtml += "<option value='" + sqlDr["PART_CD"] + "'>" + sqlDr["PART_DESC"] + "</option>";
        sqlDr.Close();
        //获取该JO的床次
        sqlDr = packagesql.layno(sqlCon, jo);
        string laynohtml = "";
        while (sqlDr.Read())
        {
            laynohtml += "<option value='" + sqlDr["LAY_NO"] + "'>" + sqlDr["LAY_NO"] + "</option>";
        }
        sqlDr.Close();

        //获取该JO的color-- select distinct COLOR_CODE from JO_DT where JO_NO='16F01172CB03'  tangyh 2017.03.27
        sqlDr = packagesql.colorcode(sqlCon, jo);
        string colorhtml = "";
        while (sqlDr.Read())
        {
            colorhtml += "<option value='" + sqlDr["COLOR_CODE"] + "'>" + sqlDr["COLOR_CODE"] + "</option>";
        }
        sqlDr.Close();

        if (parthtml == "" || laynohtml == "" || colorhtml == "")
        {
            sqlCon.Close();
            return "false";
        }
        return "[{ \"PARTHTML\": \"" + parthtml + "\", \"LAYNOHTML\": \"" + laynohtml + "\", \"COLORHTML\": \"" + colorhtml + "\" }]";
    }

    [WebMethod]
    public static String Cartonbarcodecheck(string factory, string svTYPE, string barcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Packagesql packagesql = new Packagesql();
        SqlDataReader sqlDr = packagesql.cartonbarcodecheck(sqlCon, barcode);
        string result = "";
        if (sqlDr.Read())
            result = "true";
        else
            result = "false";
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Checkfirstmaincarton(string factory, string svTYPE, string docno, string userbarcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Packagesql packagesql = new Packagesql();
        SqlDataReader sqlDr;

        string result = "";

        string closepart = "";
        string remainpart = "";
        //部件翻译
        sqlDr = packagesql.getparttranslation(sqlCon, docno, userbarcode);
        int i = 0;
        while(sqlDr.Read())
        {
            if (i == 0)
                closepart = sqlDr["PART_DESC"].ToString();
            else
                closepart += "," + sqlDr["PART_DESC"].ToString();
            i++;
        }
        sqlDr.Close();

        //剩余的部件
        sqlDr = packagesql.getremainpart(sqlCon, docno, userbarcode);
        i = 0;
        while (sqlDr.Read())
        {
            if (i == 0)
                remainpart = sqlDr["PART_DESC"].ToString();
            else
                remainpart += "," + sqlDr["PART_DESC"].ToString();
            i++;
        }
        sqlDr.Close();

        //找出扫描的裁片所在GO，判断是否已经关过主箱码
        sqlDr = packagesql.checkgoclosed(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
        {
            sqlCon.Close();
            return "false";
        }
        sqlDr.Close();

        sqlCon.Close();
        result += "[{ \"CLOSEPART\": \""+closepart+"\", \"REMAINPART\": \""+remainpart+"\" }]";
        return result;
    }

    [WebMethod]
    public static String Query(string factory, string svTYPE, string userbarcode, string functioncd)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open(); 
        Packagesql packagesql = new Packagesql();
        SqlDataReader sqlDr = packagesql.query(sqlCon, userbarcode, functioncd);
        string result = "";
        while (sqlDr.Read())
            result += "<option value='" + sqlDr["DATE_TIME"].ToString() + "'>" + sqlDr["DATE_TIME"].ToString() + "</option>";
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Receivecheckload(string userbarcode, string olddocno, string newdocno, string factory, string svTYPE, string process, string dbtrans)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        SqlDataReader sqlDr;
        Sqlstatement commonsql = new Sqlstatement();
        Packagesql packagesql = new Packagesql();

        //更新user_scanning_dft
        string sql = "update CIPMS_USER_SCANNING_DFT set DOC_NO='"+newdocno+"' where DOC_NO='"+olddocno+"' and USER_BARCODE='"+userbarcode+"'";
        SqlCommand cmd = new SqlCommand(sql, sqlCon);
        cmd.ExecuteNonQuery();

        string html = "";
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlCon;
        sqlComGet.CommandText = "SELECT PROCESS_TYPE,BUNDLE_ID,CARTON_BARCODE,a.FACTORY_CD,PROCESS_CD,PRODUCTION_LINE_CD,PROCESS_TYPE,BARCODE,a.PART_CD,b.PART_DESC,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + newdocno + "' and USER_BARCODE='" + userbarcode + "')";
        sqlDr = sqlComGet.ExecuteReader();
        while (sqlDr.Read())
        {
            string carton_barcode = "";
            if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
            html += "<tr id='" + sqlDr["BUNDLE_ID"] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center'>" + carton_barcode + "</td>";
            int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["PROCESS_CD"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["BARCODE"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["PART_DESC"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["BUNDLE_NO"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["LAY_NO"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["SIZE_CD"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["COLOR_CD"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["QTY"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + output.ToString() + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["DISCREPANCY_QTY"] + "</td>";
            html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></tr>";
        }
        sqlDr.Close();

        //result += "[{ \"HTML\": \"" + html + "\", ";

        //获取totalbundle和totalpcs和totalgarmentpcs
        //string bundle = "0";
        //sqlDr = commonsql.totalbundles(sqlCon, newdocno, userbarcode);
        //if (sqlDr.Read())
        //    bundle = sqlDr["count"].ToString();
        //sqlDr.Close();
        //int qty = 0;
        //int reduce = 0;
        //string pcs = "0";
        //sqlDr = commonsql.totalpcs(sqlCon, newdocno, userbarcode);
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
        //sqlDr = commonsql.totalgarment(sqlCon, newdocno, userbarcode);
        //if (sqlDr.Read())
        //    if (sqlDr["QTY"].ToString() != "")
        //        garment = sqlDr["QTY"].ToString();
        //sqlDr.Close();
        //result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \""+garment+"\" }]";

        result = result += "[{ \"HTML\": \"" + html + GetPcsSummary(sqlCon, newdocno, userbarcode,"F");

        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String checkscan(string temp, string userbarcode, string newdocno, string factory, string svTYPE, string process, string bundlebarcode, string part, string dbtrans, string docnobarcode,string colorcode)
    {
        string result = "";
        string[] partarray = part.Split(new char[] { ',' });
        part = "";
        int partnum = 0;
        for (int i = 0; i < partarray.Length; i++)
        {
            if (i == 0) part += "'" + partarray[i] + "'";
            else part += ",'" + partarray[i] + "'";
            partnum++;
        }
        part = "(" + part + ")";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        SqlDataReader sqlDr;
        Sqlstatement commonsql = new Sqlstatement();
        Packagesql packagesql = new Packagesql();

        //判断用户扫描的裁片条码是否是外发
        if (temp == "oas")
        {
            string result1 = "true", result2 = "true";
            sqlDr = packagesql.checkoas1(sqlCon, bundlebarcode);
            if (!sqlDr.Read())
                result1 = "false";
            sqlDr.Close();
            sqlDr = packagesql.checkoas2(sqlCon, bundlebarcode);
            if (sqlDr.Read())
                result2 = "false";
            sqlDr.Close();
            if (result1 == "false" || result2 == "false")
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false8";//该裁片没有外发或者已经外发接收了
            }
        }

        //判断扫描的bundle part是否存在先
        sqlDr = commonsql.isbundleexist(sqlCon, bundlebarcode, part);
        if (sqlDr.Read())
        {
            if (sqlDr["QTY"].ToString() != partnum.ToString())
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false1";//不存在
            }
        }
        else
        {
            sqlDr.Close();
            sqlCon.Close();
            return "false1";//不存在
        }
        sqlDr.Close();

        //判断是否是这个颜色
        //tangyh 2017.03.28 
        if (colorcode != "null" && colorcode != "--选择--" && colorcode != "")
        {
            sqlDr = packagesql.checkbundlecolor(sqlCon, bundlebarcode, colorcode, "B");
            if (sqlDr.Read())
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false7";
            }
            sqlDr.Close();
        }

        string parts = "";
        string html = "";
        string sql = "";
        //查找未扫描的bundle part
        sqlDr = commonsql.unscan(sqlCon, bundlebarcode, part, newdocno, userbarcode);
        int m = 0;
        while (sqlDr.Read())
        {
            if (sqlDr["DOC_NO"].ToString() != docnobarcode.Substring(3))
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false5";//不在流水单号里
            }
            if (m == 0)
            {
                parts += sqlDr["PART_DESC"];
                sql += "('" + sqlDr["BUNDLE_ID"] + "','" + newdocno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Receive',getdate())";
            }
            else
            {
                parts += "," + sqlDr["PART_DESC"];
                sql += ",('" + sqlDr["BUNDLE_ID"] + "','" + newdocno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Receive',getdate())";
            }
            string carton_barcode = "";
            if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
            html += "<tr id='" + sqlDr["BUNDLE_ID"] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center'>" + carton_barcode + "</td>";
            int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["PROCESS_CD"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["BARCODE"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["PART_DESC"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["BUNDLE_NO"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["LAY_NO"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["SIZE_CD"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["COLOR_CD"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["QTY"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + output.ToString() + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["DISCREPANCY_QTY"] + "</td>";
            html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></tr>";
            m++;
        }
        sqlDr.Close();
        if (m == 0)
        {
            sqlCon.Close();
            return "false";//已扫描过
        }

        //把未扫描的bundle part添加到扫描表
        commonsql.unscaninsert(sqlCon, sql);

        result += "[{ \"HTML\": \"" + html + "\", ";

        //获取totalbundle和totalpcs
        string bundle = "0";
        sqlDr = commonsql.totalbundles(sqlCon, newdocno, userbarcode);
        if (sqlDr.Read())
            bundle = sqlDr["count"].ToString();
        sqlDr.Close();
        int qty = 0;
        int reduce = 0;
        string pcs = "0";
        sqlDr = commonsql.totalpcs(sqlCon, newdocno, userbarcode);
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
        sqlDr = commonsql.totalgarment(sqlCon, newdocno, userbarcode);
        if (sqlDr.Read())
            if (sqlDr["QTY"].ToString() != "")
                garment = sqlDr["QTY"].ToString();
        sqlDr.Close();
        result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \""+garment+"\", \"PART\": \""+parts+"\" }]";
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Receivecheck(string userbarcode, string factory, string svTYPE, string docno, string cartonbarcode, string translation)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement commonsql = new Sqlstatement();

        string result = "";
        int qty = 0;
        int bundle = 0;
        string detail1 = "null";
        //lack
        int i = 0;
        SqlDataReader sqlDr = commonsql.receivecheckmore(sqlCon, cartonbarcode.Substring(3), docno, userbarcode);
        while (sqlDr.Read())
        {
            qty += int.Parse(sqlDr["QTY"].ToString());
            bundle++;
            if (i == 0)
                detail1 = sqlDr["BARCODE"] + translation + sqlDr["PART_DESC"];
            else
                detail1 += "****" + sqlDr["BARCODE"] + translation + sqlDr["PART_DESC"];
            i++;
        }
        sqlDr.Close();
        result = "{ \"lack\": [{ \"bundle\": \"" + bundle.ToString() + "\", \"qty\": \"" + qty.ToString() + "\", \"detail\": \"" + detail1 + "\" }],";
        if (detail1 == "null")
            result += " \"CHECKRESULT\": [{ \"checkresult\": \"Y\" }]}";
        else
            result += " \"CHECKRESULT\": [{ \"checkresult\": \"N\" }]}";

        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Bundlescan(string userbarcode, string docno, string factory, string svTYPE, string process, string barcode, string part, string jo, string dbtrans, string colorcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        SqlDataReader sqlDr;
        Sqlstatement commonsql = new Sqlstatement();
        Packagesql packagesql = new Packagesql();
        Transactionsql transfersql = new Transactionsql();

        string result = "{ \"bundles\": [";
        int partnum = 0;
        string returnpart = "";
        int m = 0;
        if (part != "null")
        {
            string[] partarray = part.Split(new char[] { ',' });
            part = "";
            for (int i = 0; i < partarray.Length; i++)
            {
                if (i == 0) part += "'" + partarray[i] + "'";
                else part += ",'" + partarray[i] + "'";
                partnum++;
            }
            part = "(" + part + ")";
        }

        //判断bundle part是否存在
        if(part != "null"){
            sqlDr = packagesql.checkbundleexist1(sqlCon, barcode, part);
            if (!sqlDr.Read())
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false6";
            }
            sqlDr.Close();
        }
        else{
            sqlDr = packagesql.checkbundleexist2(sqlCon, barcode);
            if (!sqlDr.Read())
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false6";
            }
            sqlDr.Close();
        }

        //判断是否是这个颜色
        //tangyh 2017.03.28 
        if (colorcode != "null" && colorcode != "--选择--" && colorcode != "")
        {
            sqlDr = packagesql.checkbundlecolor(sqlCon, barcode, colorcode,"B");
            if (sqlDr.Read())
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false7";
            }
            sqlDr.Close();
        }

        //判断扫描的裁片是否在JO里
        if (jo != "")
        {
            try
            {
                sqlDr = packagesql.checkbundleinjo(sqlCon, barcode);
            }
            catch (Exception ex)
            {
                sqlCon.Close();
                return "error1";
            }
            if (sqlDr.Read())
            {
                if (sqlDr["JOB_ORDER_NO"].ToString() != jo)
                {
                    sqlDr.Close();
                    sqlCon.Close();
                    return "false2";//bundle所在的JO和用户输入的不一致
                }
            }
            else
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false1";//查询不到bundle的JO数据
            }
            sqlDr.Close();
        }

        //
        if (part == "null")
        {
            int i = 0;
            partnum = 0;
            part = "";
            Boolean tempmain = false;
            Boolean tempsecond = false;

            //判断该bundle是否关过主箱码
            try
            {
                sqlDr = packagesql.checkmaincarton(sqlCon, barcode);
            }
            catch (Exception ex)
            {
                sqlCon.Close();
                return "error2";
            }
            if (sqlDr.Read())
                tempmain = true;//该bundle关过主箱码
            sqlDr.Close();

            //如果关过主箱码，判断是否关过次箱码
            if (tempmain == true)
            {
                try
                {
                    sqlDr = packagesql.checksecondcarton(sqlCon, barcode);
                }
                catch (Exception ex)
                {
                    sqlCon.Close();
                    return "error3";
                }
                if (sqlDr.Read())
                    tempsecond = true;
                sqlDr.Close();
            }

            //如果关过主箱码，没有关过次箱码，则加载除主箱码剩余的部件
            if (tempmain == true && tempsecond == false)
            {
                try
                {
                    sqlDr = packagesql.getremainpart(sqlCon, barcode);
                }
                catch (Exception ex)
                {
                    sqlCon.Close();
                    return "error4";
                }
                while (sqlDr.Read())
                {
                    if (i == 0) 
                        part += "'" + sqlDr["PART_CD"].ToString() + "'";
                    else 
                        part += ",'" + sqlDr["PART_CD"].ToString() + "'";
                    partnum++;
                    i++;
                }
                sqlDr.Close();
                if (i == 0)
                {
                    sqlCon.Close();
                    return "false3";//该bundle的所有部件已经关进主箱码
                }
            }
            //否则加载所有的部件
            else if (tempmain == true && tempsecond == true)
            {
                sqlCon.Close();
                return "false4";//该bundle的所有部件已经关进主箱码和次箱码
                //sqlDr = packagesql.getwholepart(sqlCon, barcode);
                //while (sqlDr.Read())
                //{
                //    if (i == 0) part += "'" + sqlDr["PART_CD"].ToString() + "'";
                //    else part += ",'" + sqlDr["PART_CD"].ToString() + "'";
                //    partnum++;
                //    i++;
                //}
                //sqlDr.Close();
            }
            //如果没有关过主箱码，则判断该GO是否关过主箱码
            else if (tempmain == false)
            {
                Boolean temp = false;//go关箱标记
                try
                {
                    sqlDr = packagesql.getmaincartonpart(sqlCon, factory, barcode);
                }
                catch(Exception ex)
                {
                    sqlCon.Close();
                    return "error5";
                }
                while (sqlDr.Read())//加载关过主箱码的部件
                {
                    if (i == 0) 
                        part += "'" + sqlDr["PART_CD"].ToString() + "'";
                    else 
                        part += ",'" + sqlDr["PART_CD"].ToString() + "'";
                    partnum++;
                    i++;
                    temp = true;
                }
                sqlDr.Close();
                if (temp == false)//加载该bundle全部的部件
                {
                    try
                    {
                        sqlDr = packagesql.getwholepart(sqlCon, barcode);
                    }
                    catch (Exception ex)
                    {
                        sqlCon.Close();
                        return "error6";
                    }
                    while (sqlDr.Read())
                    {
                        if (i == 0) 
                            part += "'" + sqlDr["PART_CD"].ToString() + "'";
                        else 
                            part += ",'" + sqlDr["PART_CD"].ToString() + "'";
                        partnum++;
                        i++;
                    }
                    sqlDr.Close();
                }
            }
            part = "(" + part + ")";
        }

        try
        {
            sqlDr = commonsql.partdesc(sqlCon, part);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error9";
        }
        while (sqlDr.Read())
        {
            if (m == 0)
                returnpart = sqlDr["PART_DESC"].ToString();
            else
                returnpart += "," + sqlDr["PART_DESC"].ToString();
            m++;
        }
        sqlDr.Close();

        //判断扫描的bundle part是否存在先
        //sqlDr = commonsql.isbundleexist(sqlCon, barcode, part);
        //if (sqlDr.Read())
        //{
        //    if (sqlDr["QTY"].ToString() != partnum.ToString())
        //    {
        //        sqlDr.Close();
        //        sqlCon.Close();
        //        return "false1";
        //    }
        //}
        //else
        //{
        //    sqlDr.Close();
        //    sqlCon.Close();
        //    return "false1";
        //}
        //sqlDr.Close();

        string html = "";
        jo = "";
        string isprocess = "true";//判断是否在本部门
        //查找未扫描的bundle part
        //sqlDr = commonsql.unscan(sqlCon, barcode, part, docno, userbarcode);
        //while (sqlDr.Read())
        //{
        //    //if (sqlDr["PROCESS_CD"].ToString() != process || sqlDr["PROCESS_TYPE"].ToString() != "I")
        //    //    isprocess = "false";
        //    if (sqlDr["PROCESS_CD"].ToString() != process)
        //    {
        //        sqlDr.Close();
        //        sqlCon.Close();
        //        return "false3";
        //    }
        //    jo = sqlDr["JOB_ORDER_NO"].ToString();
        //    if (m == 0)
        //        sql += "('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Package',getdate())";
        //    else
        //        sql += ",('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Package',getdate())";
            //string carton_barcode = "";
            //if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
            //    carton_barcode = "";
            //else
            //    carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
            //if (m == 0)
            //    returnpart = sqlDr["PART_DESC"].ToString();
            //else
            //    returnpart += "," + sqlDr["PART_DESC"].ToString();
            //int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
            //if (m == 0)
            //    html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"];
            //else
            //    html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"];
            ////html += "<tr id='" + sqlDr["BUNDLE_ID"] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + carton_barcode + "</td>";
            //html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["PROCESS_CD"] + "</td>";
            //html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["BARCODE"] + "</td>";
            //html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PART_DESC"] + "</td>";
            //html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
            //html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["BUNDLE_NO"] + "</td>";
            //html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["LAY_NO"] + "</td>";
            //html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["COLOR_CD"] + "</td>";
            //html += "<td style='vertical-align:middle; text-align:center; width:4%;'>" + sqlDr["SIZE_CD"] + "</td>";
            //html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["QTY"] + "</td>";
            //html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + output.ToString() + "</td>";
            //html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["DEFECT"] + "</td>";
            //html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["DISCREPANCY_QTY"] + "</td>";
            //html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></td></tr>";
        //    m++;
        //}
        //sqlDr.Close();
        //if (m == 0)
        //{
        //    sqlCon.Close();
        //    return "false";//已扫描过
        //}

        //把未扫描的bundle part添加到扫描表
        try
        {
            commonsql.unscaninsert(sqlCon, "Package", docno, userbarcode, barcode, part,"F");
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error7";
        }
        m = 0;
        try
        {
            sqlDr = commonsql.scanned(sqlCon, docno, userbarcode);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error8";
        }
        while (sqlDr.Read())
        {
            if (sqlDr["PROCESS_CD"].ToString() != process || sqlDr["PROCESS_TYPE"].ToString() != "I")
                isprocess = "false";
            string carton_barcode = "";
            if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                carton_barcode = sqlDr["CARTON_BARCODE"].ToString();

            int output=0;
            if (sqlDr["PROCESS_CD"].ToString()=="DC")
               output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
            else
                output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DEFECT"].ToString());
            if (m == 0)
                      html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"];
            else
                html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"];
            m++;
        }
        sqlDr.Close();

        if (m == 0)
        {
            sqlCon.Close();
            return "false5";
        }

        string closecarton = "false";
        string opencarton = "false";
        //string matching = "false";

        //判断所有已扫描条码是否都在本部门
        //sqlDr = commonsql.isprocess(sqlCon, docno, userbarcode, process);
        //if (sqlDr.Read())
        //    isprocess = "false";
        //sqlDr.Close();

        if (isprocess == "true")
        {
            //关箱:判断是否有裁片已经关箱了；
            sqlDr = packagesql.closecartonall(sqlCon, docno, userbarcode);
            if (!sqlDr.Read())
                closecarton = "true";
            sqlDr.Close();
            //判断是否所有的裁片都在同一个组别
            if (closecarton == "true")
            {
                sqlDr = packagesql.closecartonproduction(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                {
                    if (sqlDr["QTY"].ToString() != "1")
                        closecarton = "false";
                }
                else
                {
                    closecarton = "false";
                }
                sqlDr.Close();
            }

            //判断能否进行开箱
            string cartonqty1 = "";
            string cartonqty2 = "";
            if (closecarton == "false")
            {
                //判断扫描的bundle所在的箱的箱内所有的bundle数量和扫描的bundle数量是否一致。一致则说明扫描箱内bundle齐全
                //扫描的在箱内的bundle part数
                sqlDr = packagesql.cartonbundleqty1(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    cartonqty1 = sqlDr["QTY"].ToString();
                sqlDr.Close();
                //实际箱内所有的bundle part数
                sqlDr = packagesql.cartonbundleqty2(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    cartonqty2 = sqlDr["QTY"].ToString();
                sqlDr.Close();
                if (cartonqty1 != "0" && cartonqty2 != "0" && cartonqty1 == cartonqty2)
                    opencarton = "true";
            }
        }

        //查询床次
        string layno = "0";
        m = 0;
        sqlDr = packagesql.querylayno(sqlCon, barcode);
        while (sqlDr.Read())
        {
            if (m == 0)
                layno = sqlDr["LAY_NO"].ToString();
            else
                layno += "," + sqlDr["LAY_NO"].ToString();
            m++;
        }
        sqlDr.Close();

        //所有的裁片都非提交状态
        string issubmit = "false";
        sqlDr = transfersql.checkcartonsubmit(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            issubmit = "true";
        sqlDr.Close();

        result += "{ \"PART\": \""+returnpart+"\", \"HTML\": \"" + html + "\", \"LAYNO\": \"" + layno + "\", \"JO\": \"" + jo + "\" }],\"btnstatus\": [{ \"ISSUBMIT\": \""+issubmit+"\", \"ISPROCESS\": \"" + isprocess + "\", \"CLOSECARTON\": \"" + closecarton + "\", \"OPENCARTON\": \"" + opencarton + "\", ";

        //获取totalbundle和totalpcs
        //string partbundle = "0";
        //string bundle = "0";
        //sqlDr = commonsql.totalbundles(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //{
        //    bundle = sqlDr["distinctcount"].ToString();
        //    partbundle = sqlDr["count"].ToString();
        //}
        //sqlDr.Close();
        //int qty = 0;
        //int reduce = 0;
        //string pcs = "0";
        //sqlDr = commonsql.totalpcs(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //{
        //    if(sqlDr["qty"].ToString() != "")
        //        qty = int.Parse(sqlDr["qty"].ToString());
        //    if (sqlDr["reduce"].ToString() != "")
        //    reduce = int.Parse(sqlDr["reduce"].ToString());
        //}
        //sqlDr.Close();
        //pcs = (qty - reduce).ToString();
        //string garment = "0";
        //sqlDr = commonsql.totalgarment(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    if (sqlDr["QTY"].ToString() != "")
        //        garment = sqlDr["QTY"].ToString();
        //sqlDr.Close();

        //result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALGARMENTPCS\": \"" + garment + "\", \"TOTALPART\": \"" + partbundle + "\", \"TOTALPCS\": \"" + pcs + "\" }]}";

        result = result + GetPcsSummary(sqlCon, docno, userbarcode, "F");

        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Cartonscan(string factory, string svTYPE, string process, string barcode, string docno, string userbarcode, string dbtrans, string get_othercarton,string colorcode)
    {
        string result = "{ \"bundles\": [";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement commonsql = new Sqlstatement();
        Packagesql packagesql = new Packagesql();
        Transactionsql transfersql = new Transactionsql();

        //判断箱子的状态：是否存在，是否是关箱状态，是否在本厂的本部门
        SqlDataReader sqlDr;
        try
        {
               sqlDr = commonsql.cartonstatus(sqlCon, barcode,get_othercarton);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error1";
        }

        string statuscarton = "";
        string process_cd = "";
        string process_type = "";
        //string colorcode1 = "";

        bool hasrecord = false;
        while (sqlDr.Read())
        {
            statuscarton = sqlDr["CARTON_STATUS"].ToString();
            process_cd = sqlDr["PROCESS_CD"].ToString();
            process_type = sqlDr["PROCESS_TYPE"].ToString();

            if (statuscarton != "C")
            {
                sqlDr.Close();
                sqlCon.Close();
                return "opened";//该箱码已开箱                
            }

            hasrecord = true;
        }
        if (hasrecord == false && get_othercarton=="F")
        {
            sqlDr.Close();
            sqlCon.Close();
            return "notexist";//不存在该箱码
        }
        else
        {
            sqlDr.Close();
        }

        //判断是否是这个颜色
        //tangyh 2017.03.28 
        if (colorcode != "null" && colorcode != "--选择--" && colorcode != "")
        {
            sqlDr = packagesql.checkbundlecolor(sqlCon, barcode, colorcode,"C");
            if (sqlDr.Read())
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false7";
            }
            sqlDr.Close();
        }

        //箱内未扫描的bundle barcode
        //string sql = "";
        string html = "";
        //sqlDr = commonsql.unscanbundlepart(sqlCon, barcode, docno, userbarcode);
        //int j = 0;
        //while (sqlDr.Read())
        //{
        //    if (sqlDr["PROCESS_CD"].ToString() != process)
        //    {
        //        sqlDr.Close();
        //        sqlCon.Close();
        //        return "false";
        //    }
        //    if (j == 0)
        //        sql += "('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Package',getdate())";
        //    else
        //        sql += ",('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Package',getdate())";
        //    html += "<tr id='" + sqlDr["BUNDLE_ID"] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["CARTON_BARCODE"] + "</td>";
        //    int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
        //    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["PROCESS_CD"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["BARCODE"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PART_DESC"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["BUNDLE_NO"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["LAY_NO"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["COLOR_CD"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:4%;'>" + sqlDr["SIZE_CD"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["QTY"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + output.ToString() + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["DEFECT"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["DISCREPANCY_QTY"] + "</td>";
        //    html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></td></tr>";
        //    j++;
        //}
        //sqlDr.Close();
        //if (j == 0)
        //{
        //    sqlCon.Close();
        //    return "empty";//该箱码为空
        //}

        //把未扫描的插到扫描表里
        //commonsql.unscaninsert(sqlCon, sql);

        string isprocess = "true";
        string closecarton = "false";
        string opencarton = "false";

        try
        {
            commonsql.unscaninsert(sqlCon, docno, userbarcode, "Package", barcode, get_othercarton,false,"");
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error2";
        }

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
                isprocess = "false";
            string carton_barcode = "";
            if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                carton_barcode = sqlDr["CARTON_BARCODE"].ToString();

            int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());

            if (m == 0)
                html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"];
            else
                html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"];
            m++;
        }
        sqlDr.Close();

        if (m == 0)
        {
            sqlCon.Close();
            return "false1";
        }

        //判断所有已扫描条码是否都在本部门
        //sqlDr = commonsql.isprocess(sqlCon, docno, userbarcode, process);
        //if (sqlDr.Read())
        //    isprocess = "false";
        //sqlDr.Close();

        if (isprocess == "true")
        {
            //关箱:判断是否有裁片已经关箱了；
            sqlDr = packagesql.closecartonall(sqlCon, docno, userbarcode);
            if (!sqlDr.Read())
                closecarton = "true";
            sqlDr.Close();
            //判断是否所有的裁片都在同一个组别
            if (closecarton == "true")
            {
                sqlDr = packagesql.closecartonproduction(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                {
                    if (sqlDr["QTY"].ToString() != "1")
                        closecarton = "false";
                }
                else
                {
                    closecarton = "false";
                }
                sqlDr.Close();
            }

            //判断能否进行开箱
            string cartonqty1 = "";
            string cartonqty2 = "";
            if (closecarton == "false")
            {
                //判断扫描的bundle所在的箱的箱内所有的bundle数量和扫描的bundle数量是否一致。一致则说明扫描箱内bundle齐全
                //扫描的在箱内的bundle part数
                sqlDr = packagesql.cartonbundleqty1(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    cartonqty1 = sqlDr["QTY"].ToString();
                sqlDr.Close();
                //实际箱内所有的bundle part数
                sqlDr = packagesql.cartonbundleqty2(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    cartonqty2 = sqlDr["QTY"].ToString();
                sqlDr.Close();
                if (cartonqty1 != "0" && cartonqty2 != "0" && cartonqty1 == cartonqty2)
                    opencarton = "true";
            }
        }

        //所有的裁片都非提交状态
        string issubmit = "false";
        sqlDr = transfersql.checkcartonsubmit(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            issubmit = "true";
        sqlDr.Close();

        result += "{ \"HTML\": \"" + html + "\" }], \"btnstatus\": [{ \"ISSUBMIT\": \"" + issubmit + "\", \"ISPROCESS\": \"" + isprocess + "\", \"CLOSECARTON\": \"" + closecarton + "\", \"OPENCARTON\":\"" + opencarton + "\", ";

        //获取totalbundle和totalpcs
        //string partbundle = "0";
        //string bundle = "0";
        //sqlDr = commonsql.totalbundles(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //{
        //    bundle = sqlDr["distinctcount"].ToString();
        //    partbundle = sqlDr["count"].ToString();
        //}
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
        //result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALGARMENTPCS\": \"" + garment + "\", \"TOTALPART\": \"" + partbundle + "\", \"TOTALPCS\": \"" + pcs + "\" }]}";
        result = result + GetPcsSummary(sqlCon, docno, userbarcode, "F");
        
        return result;
    }

    [WebMethod]
    public static String Laynobundle(string userbarcode, string docno, string layno, string joborderno, string factory, string svTYPE, string process, string part, string dbtrans,string colorcd)
    {
        string result = "{ \"bundles\": [";
        string[] partarray = part.Split(new char[] { ',' });

        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement commonsql = new Sqlstatement();
        Packagesql packagesql = new Packagesql();
        Transactionsql transfersql = new Transactionsql();
        SqlDataReader sqlDr;

        string queryway = "";
        Boolean loadmainpart = false;
        Boolean loadsecondpart = false;
        //判断part是主箱码的part还是次箱码的part,如果都不是则提示错误
        if (part != "null")
        {
            List<string> mainpart = new List<string>();
            List<string> secondpart = new List<string>();
            sqlDr = packagesql.getmaincartonpart(sqlCon, joborderno, int.Parse(layno));
            int samenum = 0;
            while (sqlDr.Read())
            {
                foreach (string n in partarray)
                    if (n == sqlDr["PART_CD"].ToString())
                        samenum++;
                mainpart.Add(sqlDr["PART_CD"].ToString());
            }
            sqlDr.Close();
            if (mainpart.Count == 0)//该床次不存在主箱码，则加载用户选择的part
            {
                int j = 0;
                part = "";
                foreach (string n in partarray)
                {
                    if (j == 0)
                        part += "'" + n + "'";
                    else
                        part += ",'" + n + "'";
                    j++;
                }
                part = "(" + part + ")";
                queryway = "1";//加载该床次的所有bundle的用户选择的part
            }
            else
            {
                if (samenum == partarray.Length && partarray.Length == mainpart.Count)//用户选择的part为主箱码的part
                {
                    int j = 0;
                    part = "";
                    foreach (string n in partarray)
                    {
                        loadmainpart = true;
                        if (j == 0)
                            part += "'" + n + "'";
                        else
                            part += ",'" + n + "'";
                        j++;
                    }
                    part = "(" + part + ")";
                    if (loadmainpart == true)
                    {
                        //判断主箱码是否已经满了
                        sqlDr = packagesql.checkifallinmaincarton(sqlCon, joborderno, int.Parse(layno));
                        if (!sqlDr.Read())
                        {
                            sqlDr.Close();
                            sqlCon.Close();
                            return "false13";//主箱码已满，加载数据为空
                        }
                    }
                    queryway = "2";//该床次未装主箱码的裁片的part
                }
                if (loadmainpart == false)
                {
                    sqlDr = packagesql.getremainpart(sqlCon, joborderno, int.Parse(layno));//次箱码的部件
                    samenum = 0;
                    while (sqlDr.Read())
                    {
                        foreach (string n in partarray)
                            if (n == sqlDr["PART_CD"].ToString())
                                samenum++;
                        secondpart.Add(sqlDr["PART_CD"].ToString());
                    }
                    sqlDr.Close();
                    if (samenum == partarray.Length && partarray.Length == secondpart.Count)//用户选择的part为次箱码的part
                    {
                        int j = 0;
                        part = "";
                        foreach (string n in partarray)
                        {
                            loadmainpart = true;
                            if (j == 0)
                                part += "'" + n + "'";
                            else
                                part += ",'" + n + "'";
                            j++;
                        }
                        part = "(" + part + ")";
                        queryway = "6";//加载该床次次箱码的bundle part
                    }
                    else
                    {
                        sqlCon.Close();
                        return "false12";//用户选择的既不是主箱码的部件，也不是次箱码的部件
                    }
                }
            }
        }
        else
        {
            //查找该go关过主箱码的部件，并加载还没关主箱码的裁片的部件
            int j = 0;
            part = "";
            sqlDr = packagesql.getmaincartonpart(sqlCon, joborderno, int.Parse(layno));//查找主箱码的部件
            while (sqlDr.Read())
            {
                loadmainpart = true;
                if (j == 0)
                    part += "'" + sqlDr["PART_CD"].ToString() + "'";
                else
                    part += ",'" + sqlDr["PART_CD"].ToString() + "'";
                j++;
            }
            part = "(" + part + ")";
            sqlDr.Close();
            //判断该layno的所有裁片是否都已经装进主箱码
            if (loadmainpart == true)
            {
                sqlDr = packagesql.checkifallinmaincarton(sqlCon, joborderno, int.Parse(layno));
                if (!sqlDr.Read())
                {
                    loadmainpart = false;
                    loadsecondpart = true;
                }
                else
                {  
                    queryway = "2";//该床次主箱码的部件的bundle part
                }
                sqlDr.Close();
            }
            else//加载该layno所有的bundle part
            {
                j = 0;
                part = "";
                sqlDr = packagesql.getpart(sqlCon, joborderno);
                while (sqlDr.Read())
                {
                    if (j == 0)
                        part += "'" + sqlDr["PART_CD"].ToString() + "'";
                    else
                        part += ",'" + sqlDr["PART_CD"].ToString() + "'";
                    j++;
                }
                part = "(" + part + ")";
                queryway = "1";//加载该床次所有的bundle part
                sqlDr.Close();
            }
            if (loadsecondpart == true)//次箱码的部件
            {
                j = 0;
                part = "";
                sqlDr = packagesql.getremainpart(sqlCon, joborderno, int.Parse(layno));//次箱码的部件
                while (sqlDr.Read())
                {
                    if (j == 0)
                        part += "'" + sqlDr["PART_CD"].ToString() + "'";
                    else
                        part += ",'" + sqlDr["PART_CD"].ToString() + "'";
                    j++;
                }
                if(j == 0)
                {
                    sqlDr.Close();
                    sqlCon.Close();
                    return "false11";//主箱码装了该layno的所有bundle part。。。没有次箱码
                }
                part = "(" + part + ")";
                queryway = "6";//加载该床次的次箱码的部件的bundle
                sqlDr.Close();
            }
        }

        // 查找未扫描的bundle part
        //string sql = "";
        string html = "";
        //int r = 0;
        string isprocess = "true";//判断是否在本部门
        //if (queryway == "1")
        //    sqlDr = packagesql.queryway1(sqlCon, joborderno, int.Parse(layno), part, docno, userbarcode);//该床次所有的bundle part
        //else if (queryway == "2")
        //    sqlDr = packagesql.queryway2(sqlCon, joborderno, int.Parse(layno), part, docno, userbarcode);//该床次未关主箱的bundle part
        //else
        //    sqlDr = packagesql.queryway6(sqlCon, joborderno, int.Parse(layno), part, docno, userbarcode);//该床次未关次箱的bundle part
        //while (sqlDr.Read())
        //{
        //    if (sqlDr["PROCESS_CD"].ToString() == process && sqlDr["PROCESS_TYPE"].ToString() == "I")
        //    {
        //        if (r == 0)
        //            sql += "('" + sqlDr["BUNDLE_ID"] + "', '" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Package',getdate())";
        //        else
        //            sql += ",('" + sqlDr["BUNDLE_ID"] + "', '" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Package',getdate())";
        //        string carton_barcode = "";
        //        if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
        //            carton_barcode = "";
        //        else
        //            carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
        //        html += "<tr id='" + sqlDr["BUNDLE_ID"] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + carton_barcode + "</td>";
        //        int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
        //        html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["PROCESS_CD"] + "</td>";
        //        html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["BARCODE"] + "</td>";
        //        html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PART_DESC"] + "</td>";
        //        html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
        //        html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["BUNDLE_NO"] + "</td>";
        //        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["LAY_NO"] + "</td>";
        //        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["COLOR_CD"] + "</td>";
        //        html += "<td style='vertical-align:middle; text-align:center; width:4%;'>" + sqlDr["SIZE_CD"] + "</td>";
        //        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["QTY"] + "</td>";
        //        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + output.ToString() + "</td>";
        //        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["DEFECT"] + "</td>";
        //        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["DISCREPANCY_QTY"] + "</td>";
        //        html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></td></tr>";
        //        r++;
        //    }
        //}
        //sqlDr.Close();
        //if (r == 0)
        //{
        //    if (queryway == "6" || queryway == "2")
        //    {
        //        sqlCon.Close();
        //        return "false4";
        //    }
        //    else
        //    {
        //        sqlCon.Close();
        //        return "false";//已全部扫描了
        //    }
        //}

        if (queryway == "1")
            packagesql.unscannedinsert1(sqlCon, docno, userbarcode, joborderno, layno, part, "Package",colorcd);//该床次所有的bundle part
        else if (queryway == "2")
            packagesql.unscannedinsert2(sqlCon, docno, userbarcode, joborderno, layno, part, "Package",colorcd);//该床次未关主箱的bundle part
        else
            packagesql.unscannedinsert3(sqlCon, docno, userbarcode, joborderno, layno, part, "Package",colorcd);//该床次未关次箱的bundle part

        //把未扫描的插到扫描表里
        //commonsql.unscaninsert(sqlCon, sql);

        int m = 0;
        try
        {
            sqlDr = commonsql.scanned(sqlCon, docno, userbarcode);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error8";
        }
        while (sqlDr.Read())
        {
            if (sqlDr["PROCESS_CD"].ToString() != process || sqlDr["PROCESS_TYPE"].ToString() != "I")
                isprocess = "false";
            string carton_barcode = "";
            if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
            int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
            if (m == 0)
                html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"];
            else
                html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"];
            m++;
        }
        sqlDr.Close();

        if (m == 0)
        {
            sqlCon.Close();
            return "false5";
        }

        string closecarton = "false";
        string opencarton = "false";

        //判断所有已扫描条码是否都在本部门
        //sqlDr = commonsql.isprocess(sqlCon, docno, userbarcode, process);
        //if (sqlDr.Read())
        //    isprocess = "false";
        //sqlDr.Close();

        if (isprocess == "true")
        {
            //关箱:判断是否有裁片已经关箱了；
            sqlDr = packagesql.closecartonall(sqlCon, docno, userbarcode);
            if (!sqlDr.Read())
                closecarton = "true";
            sqlDr.Close();
            //判断是否所有的裁片都在同一个组别
            if (closecarton == "true")
            {
                sqlDr = packagesql.closecartonproduction(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                {
                    if (sqlDr["QTY"].ToString() != "1")
                        closecarton = "false";
                }
                else
                {
                    closecarton = "false";
                }
                sqlDr.Close();
            }

            //判断能否进行开箱
            string cartonqty1 = "";
            string cartonqty2 = "";
            if (closecarton == "false")
            {
                //判断扫描的bundle所在的箱的箱内所有的bundle数量和扫描的bundle数量是否一致。一致则说明扫描箱内bundle齐全
                //扫描的在箱内的bundle part数
                sqlDr = packagesql.cartonbundleqty1(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    cartonqty1 = sqlDr["QTY"].ToString();
                sqlDr.Close();
                //实际箱内所有的bundle part数
                sqlDr = packagesql.cartonbundleqty2(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    cartonqty2 = sqlDr["QTY"].ToString();
                sqlDr.Close();
                if (cartonqty1 != "0" && cartonqty2 != "0" && cartonqty1 == cartonqty2)
                    opencarton = "true";
            }
        }

        //所有的裁片都非提交状态
        string issubmit = "false";
        sqlDr = transfersql.checkcartonsubmit(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            issubmit = "true";
        sqlDr.Close();

        result += "{ \"PART\": \"\", \"HTML\": \"" + html + "\" }],\"btnstatus\": [{ \"ISSUBMIT\": \"" + issubmit + "\", \"ISPROCESS\": \"" + isprocess + "\", \"CLOSECARTON\": \"" + closecarton + "\", \"OPENCARTON\": \"" + opencarton + "\", ";

        //获取totalbundle和totalpcs
        //string partbundle = "0";
        //string bundle = "";
        //sqlDr = commonsql.totalbundles(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //{
        //    bundle = sqlDr["distinctcount"].ToString();
        //    partbundle = sqlDr["count"].ToString();
        //}
        //sqlDr.Close();
        //int qty = 0;
        //int reduce = 0;
        //string pcs = "";
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

        //result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALGARMENTPCS\": \"" + garment + "\", \"TOTALPART\": \"" + partbundle + "\", \"TOTALPCS\": \"" + pcs + "\" }]}";

        result = result+GetPcsSummary(sqlCon, docno, userbarcode,"F");

        sqlCon.Close();
        return result;
    }
    //tangyh 2017.03.28
    private static string GetPcsSummary(SqlConnection sqlCon,string docno,string userbarcode,string onlyfield)
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
            result = "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALGARMENTPCS\": \"" + garment + "\", \"TOTALPART\": \"" + partbundle + "\", \"TOTALPCS\": \"" + pcs;
        }
        else
        {
            result = "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALGARMENTPCS\": \"" + garment + "\", \"TOTALPART\": \"" + partbundle + "\", \"TOTALPCS\": \"" + pcs + "\" }]}";
        }

        return result;
    }

    [WebMethod]
    public static String Emptylist(string factory, string svTYPE, string userbarcode)
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
    public static String Save(string userbarcode, string docno, string factory, string svTYPE, string module,string carno="")
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement commonsql = new Sqlstatement();
        commonsql.save(sqlCon, docno, userbarcode, module,carno);
        //返回保存的时间
        SqlDataReader sqlDr;
        string result = "";
        //sqlDr = commonsql.savemaxdate(sqlCon, userbarcode, "Package");
        sqlDr = commonsql.savemaxdate(sqlCon, userbarcode, module);
        if(sqlDr.Read())
            result = sqlDr["CREATE_DATE"].ToString();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Querychange(string docno, string date, string factory, string svTYPE, string process, string userbarcode, string functioncd, string dbtrans)
    {
        string result = "{ \"bundles\": [";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement commonsql = new Sqlstatement();
        Packagesql packagesql = new Packagesql();
        SqlDataReader sqlDr;

        //string sql = "";
        string html = "";
        //int j = 0;
        string isprocess = "true";//判断是否在本部门
        //未扫描的bundle barcode
        //sqlDr = commonsql.querychange(sqlCon, date, userbarcode, docno, functioncd);
        //while (sqlDr.Read())
        //{
        //    if (sqlDr["PROCESS_TYPE"].ToString() != "I" || sqlDr["PROCESS_CD"].ToString() != process)
        //        isprocess = "false";
        //    if (j == 0)
        //        sql += "('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Package',getdate())";
        //    else
        //        sql += ",('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Package',getdate())";
        //    string carton_barcode = "";
        //    if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
        //        carton_barcode = "";
        //    else
        //        carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
        //    html += "<tr id='" + sqlDr["BUNDLE_ID"] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + carton_barcode + "</td>";
        //    int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
        //    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["PROCESS_CD"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + sqlDr["BARCODE"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PART_DESC"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["BUNDLE_NO"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["LAY_NO"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["COLOR_CD"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:4%;'>" + sqlDr["SIZE_CD"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["QTY"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + output.ToString() + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["DEFECT"] + "</td>";
        //    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + sqlDr["DISCREPANCY_QTY"] + "</td>";
        //    html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></td></tr>";
        //    j++;
        //}
        //sqlDr.Close();
        //if (j == 0)
        //{
        //    sqlCon.Close();
        //    return "false";//已全部扫描
        //}

        //把未扫描的插到扫描表里
        //commonsql.unscaninsert(sqlCon, sql);
        commonsql.unscannedinsert(sqlCon, date, userbarcode, docno, "Package");

        int m = 0;
        try
        {
            sqlDr = commonsql.scanned(sqlCon, docno, userbarcode);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error8";
        }
        while (sqlDr.Read())
        {
            if (sqlDr["PROCESS_CD"].ToString() != process || sqlDr["PROCESS_TYPE"].ToString() != "I")
                isprocess = "false";
            string carton_barcode = "";
            if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
            int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
            if (m == 0)
                html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"];
            else
                html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"];
            m++;
        }
        sqlDr.Close();

        if (m == 0)
        {
            sqlCon.Close();
            return "false5";
        }

        string closecarton = "false";
        string opencarton = "false";

        //判断所有已扫描条码是否都在本部门
        //sqlDr = commonsql.isprocess(sqlCon, docno, userbarcode, process);
        //if (sqlDr.Read())
        //    isprocess = "false";
        //sqlDr.Close();

        if (isprocess == "true")
        {
            //关箱:判断是否有裁片已经关箱了；
            sqlDr = packagesql.closecartonall(sqlCon, docno, userbarcode);
            if (!sqlDr.Read())
                closecarton = "true";
            sqlDr.Close();
            //判断是否所有的裁片都在同一个组别
            if (closecarton == "true")
            {
                sqlDr = packagesql.closecartonproduction(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                {
                    if (sqlDr["QTY"].ToString() != "1")
                        closecarton = "false";
                }
                else
                {
                    closecarton = "false";
                }
                sqlDr.Close();
            }

            //判断能否进行开箱
            string cartonqty1 = "";
            string cartonqty2 = "";
            if (closecarton == "false")
            {
                //判断扫描的bundle所在的箱的箱内所有的bundle数量和扫描的bundle数量是否一致。一致则说明扫描箱内bundle齐全
                //扫描的在箱内的bundle part数
                sqlDr = packagesql.cartonbundleqty1(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    cartonqty1 = sqlDr["QTY"].ToString();
                sqlDr.Close();
                //实际箱内所有的bundle part数
                sqlDr = packagesql.cartonbundleqty2(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    cartonqty2 = sqlDr["QTY"].ToString();
                sqlDr.Close();
                if (cartonqty1 != "0" && cartonqty2 != "0" && cartonqty1 == cartonqty2)
                    opencarton = "true";
            }
        }

        result += "{ \"HTML\": \"" + html + "\" }],\"btnstatus\": [{ \"ISPROCESS\": \"" + isprocess + "\", \"CLOSECARTON\": \"" + closecarton + "\", \"OPENCARTON\": \"" + opencarton + "\", ";

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
        //    qty = int.Parse(sqlDr["qty"].ToString());
        //    reduce = int.Parse(sqlDr["reduce"].ToString());
        //}
        //sqlDr.Close();
        //pcs = (qty - reduce).ToString();
        //string garment = "0";
        //sqlDr = commonsql.totalgarment(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    if (sqlDr["QTY"].ToString() != "")
        //        garment = sqlDr["QTY"].ToString();
        //sqlDr.Close();
        //result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \""+garment+"\" }]}";

        result = result + GetPcsSummary(sqlCon, docno, userbarcode,"F");

        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Reprint(string userbarcode, string factory, string svTYPE, string process, string barcode, string part, string languagearray)
    {
        if (part != "null")
        {
            string[] partarray = part.Split(new char[] { ',' });
            int j = 0;
            part = "";
            for (int i = 0; i < partarray.Length; i++)
            {
                if (j == 0)
                    part += "'" + partarray[i] + "'";
                else
                    part += ",'" + partarray[i] + "'";
                j++;
            }
            part = "(" + part + ")";
        }
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        Packagesql packagesql = new Packagesql();
        SqlDataReader sqlDr;
        string cartonbarcode = "";

        //查询箱码是否关箱
        CommonFunc commonfunc = new CommonFunc();
        string barcodetype = commonfunc.GetBarcodeType(barcode);
        if (barcodetype == "C")
        {
            cartonbarcode = barcode;
            sqlDr = sqlstatement.checkcarton(sqlCon, barcode);
            if (!sqlDr.Read())
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false1";//用户扫描的箱码不存在或者为开箱状态
            }
            sqlDr.Close();
        }

        //查询bundle是否在箱内
        if (barcodetype == "B" && part != "null")
        {
            sqlDr = sqlstatement.reprint(sqlCon, barcode, part);
            if (sqlDr.Read())
            {
                cartonbarcode = sqlDr["CARTON_BARCODE"].ToString(); ;
            }
            else
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false2";//用户扫描的条码不在箱内
            }
            sqlDr.Close();
        }
        else if (barcodetype == "B" && part == "null")
        {
            sqlCon.Close();
            return "false3";//需要用户选择部件
        }

        Cartonbarcodeprint cartonhtml = new Cartonbarcodeprint();
        string html = cartonhtml.Cartonbarcodehtml(sqlCon, cartonbarcode, languagearray);

        string result = "[{ \"HTML\":\"" + html + "\", \"CARTON\":\"" + cartonbarcode + "\" }]";
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Opencarton(string docno, string userbarcode, string factory, string svTYPE, string process)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Packagesql packagesql = new Packagesql();
        Sqlstatement commonsql = new Sqlstatement();
        SqlDataReader sqlDr;
        string result = "";

        //判断扫描的bundle part是否都在本部门
        sqlDr = packagesql.opencheckbundle(sqlCon, docno, userbarcode, process);
        if (sqlDr.Read())
        {
            sqlDr.Close();
            sqlCon.Close();
            return "false3";//bundle part的状态异常
        }
        sqlDr.Close();

        //判断扫描的bundle所在的箱的箱内所有的bundle数量和扫描的bundle数量是否一致。一致则说明扫描箱内bundle齐全
        //扫描的在箱内的bundle part数
        string cartonqty1 = "";
        string cartonqty2 = "";
        sqlDr = packagesql.cartonbundleqty1(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            cartonqty1 = sqlDr["QTY"].ToString();
        sqlDr.Close();
        //实际箱内所有的bundle part数
        sqlDr = packagesql.cartonbundleqty2(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            cartonqty2 = sqlDr["QTY"].ToString();
        sqlDr.Close();
        if (cartonqty1 == "0" || cartonqty2 == "0" || cartonqty1 != cartonqty2)
        {
            sqlDr.Close();
            sqlCon.Close();
            return "false2";//没有bundle在箱内或者箱内bundle不齐全
        }

        //判断用户扫描的裁片是否有主箱码。如果有主箱码，则该主箱码是否有副箱码，如果有则不允许开箱。如果没有，则全部开箱
        sqlDr = packagesql.checkmainhassecond(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
        {
            sqlDr.Close();
            sqlCon.Close();
            return "false1";//主箱码存在副箱码
        }
        sqlDr.Close();

        //找出要开的箱码
        int i = 0;
        List<string> cartonlist = new List<string>();
        sqlDr = packagesql.cartonlist(sqlCon, docno, userbarcode);
        while (sqlDr.Read())
        {
            if (i == 0)
            {
                cartonlist.Add(sqlDr["CARTON_BARCODE"].ToString());
                result += sqlDr["CARTON_BARCODE"].ToString();
            }
            else
            {
                cartonlist.Add(sqlDr["CARTON_BARCODE"].ToString());
                result += "," + sqlDr["CARTON_BARCODE"].ToString();
            }
            i++;
        }
        sqlDr.Close();

        foreach (string cartonbarcode in cartonlist)
        {
            //判断seq是否等于本carton_Seq+1，如果是，说明开的是最大的箱码，更新seq
            //Boolean temp1 = false;
            //if (cartonbarcode.Length == 13)
            //{
            //    sqlDr = packagesql.getcartonfirstseq(sqlCon, cartonbarcode.Substring(0, 9));
            //    if (sqlDr.Read())
            //        if (int.Parse(sqlDr["FIRST_SEQ"].ToString()) == (int.Parse(cartonbarcode.Substring(10, 3)) + 1))
            //            temp1 = true;
            //    sqlDr.Close();
            //}
            //else
            //{
            //    sqlDr = packagesql.getcartonsecondseq(sqlCon, cartonbarcode.Substring(0, 9), int.Parse(cartonbarcode.Substring(10, 3)));
            //    if (sqlDr.Read())
            //        if (sqlDr["SECOND_SEQ"].ToString() == cartonbarcode.Substring(14, 1))
            //            temp1 = true;
            //    sqlDr.Close();
            //}

            //如果是主箱码则删掉carton_second的数据
            CommonFunc commonfunc = new CommonFunc();
            int cartonbarcodetype = commonfunc.JudgeCartonType(cartonbarcode);
            if (cartonbarcodetype == 1)
                packagesql.deletesecond(sqlCon, cartonbarcode.Substring(0, 9), int.Parse(cartonbarcode.Substring(10, 3)));

            //更新CARTON_DT表
            if (cartonbarcodetype == 1)
                packagesql.updatecartondt2(sqlCon, userbarcode, cartonbarcode);
            else if (cartonbarcodetype == 2)
                packagesql.updatecartondt(sqlCon, userbarcode, cartonbarcode);

            //更新jo_bundle的status
            packagesql.updatejobundlestatus(sqlCon, cartonbarcode);

            //更新bundle_for_scanning的箱状态
            packagesql.opencartonbundlestatus(sqlCon, userbarcode, cartonbarcode);

            //更新箱的状态
            packagesql.opencartonstatus(sqlCon, userbarcode, cartonbarcode);

            //Boolean temp2 = false;
            //string maxcartonseq = "";
            //if (temp1 == true && cartonbarcode.Length == 13)
            //{
            //    //获取最大箱码
            //    sqlDr = packagesql.getmaxcartonseq(sqlCon, cartonbarcode.Substring(0, 9));
            //    if(sqlDr.Read())
            //        if (sqlDr["MAX_CARTON_SEQ"].ToString() != "")
            //        {
            //            maxcartonseq = sqlDr["MAX_CARTON_SEQ"].ToString();
            //            temp2 = true;
            //        }
            //    sqlDr.Close();
            //    if (temp2 == true)//更新carton_first_seq = max_carton_Seq+1
            //    {
            //        packagesql.updatecartonfirst1(sqlCon, cartonbarcode.Substring(0, 9), int.Parse(maxcartonseq) + 1, userbarcode);
            //    }
            //    else//更新carton_first_seq=1
            //    {
            //        packagesql.updatecartonfirst1(sqlCon, cartonbarcode.Substring(0, 9), userbarcode);
            //    }
            //}
            //else if (temp1 == true && cartonbarcode.Length == 15)
            //{
            //    //获取最大箱码
            //    sqlDr = packagesql.getmaxcartonsecondseq(sqlCon, cartonbarcode.Substring(0, 9));
            //    if(sqlDr.Read())
            //        if (sqlDr["MAX_CARTON_SEQ"].ToString() != "")
            //        {
            //            maxcartonseq = sqlDr["MAX_CARTON_SEQ"].ToString();
            //            temp2 = true;
            //        }
            //    sqlDr.Close();
            //    if (temp2 == true)//更新carton_second_seq = max_carton_second_Seq+1
            //    {
            //        packagesql.updatecartonsecondseq(sqlCon, cartonbarcode.Substring(0, 9), int.Parse(cartonbarcode.Substring(10, 3)), int.Parse(maxcartonseq) + 1, userbarcode);
            //    }
            //    else//更新carton_first_seq=1
            //    {
            //        packagesql.updatecartonsecondseq(sqlCon, cartonbarcode.Substring(0, 9), int.Parse(cartonbarcode.Substring(10, 3)), userbarcode);
            //    }
            //}

            //添加到箱的操作历史表
            packagesql.insertcartonhistory(sqlCon, factory, process, userbarcode, cartonbarcode);
        }

        //清空扫描表
        commonsql.emptylist(sqlCon, userbarcode);

        //返回成功开箱的箱码
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Closecarton(string docno, string userbarcode, string factory, string svTYPE, string process, string cartonbarcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement commonsql = new Sqlstatement();
        Packagesql packagesql = new Packagesql();
        SqlDataReader sqlDr;

        //查看旧箱码是否为开箱状态
        sqlDr = packagesql.cartonstatus(sqlCon, cartonbarcode);
        if (sqlDr.Read())
        {
            if (sqlDr["CARTON_STATUS"].ToString() != "O")
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false1";//旧箱非开箱状态
            }
            //add by jacob 20151118 to check carton status
            else if (sqlDr["PROCESS_CD"].ToString() != process)
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false4";//旧箱非本部门
            }
            else if (sqlDr["PROCESS_TYPE"].ToString() != "I")
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false5";//旧箱非本厂
            }
        }
        else
        {
            sqlDr.Close();
            sqlCon.Close();
            return "false2";//箱码不存在
        }
        sqlDr.Close();

        //判断扫描的bundle part是否都在本部门并且都为开箱状态
        sqlDr = packagesql.closecheckbundle(sqlCon, docno, userbarcode, process);
        if (sqlDr.Read())
        {
            sqlDr.Close();
            sqlCon.Close();
            return "false3";//bundle part的状态异常
        }
        sqlDr.Close();

        //判断次箱码能否关箱
        CommonFunc commonfunc = new CommonFunc();
        int cartonbarcodetype = commonfunc.JudgeCartonType(cartonbarcode);
        if (cartonbarcodetype == 2)
        {
            sqlDr = packagesql.checkmaincartonclose(sqlCon, commonfunc.GetMainCarton(cartonbarcode));
            if (!sqlDr.Read())
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false6";//该次箱码的主箱码为开箱状态
            }
            sqlDr.Close();
        }

        //判断是否需要更新seq
        Boolean temp = false;
        int cartonfirstseq = 0;
        int cartonsecondseq = 0;
        if (cartonbarcodetype == 1)
        {
            sqlDr = packagesql.getcartonfirstseq(sqlCon, commonfunc.GetMainCarton(cartonbarcode));
            if (sqlDr.Read())
                if (int.Parse(commonfunc.GetCartonSeq(cartonbarcode)) >= int.Parse(sqlDr["FIRST_SEQ"].ToString()))
                {
                    cartonfirstseq = int.Parse(commonfunc.GetCartonSeq(cartonbarcode));
                    temp = true;
                }
            sqlDr.Close();
        }
        else
        {
            sqlDr = packagesql.getcartonsecondseq2(sqlCon, commonfunc.GetMainCarton(cartonbarcode), int.Parse(commonfunc.GetCartonSeq(cartonbarcode)));
            if (sqlDr.Read())
                if ((int)Convert.ToChar(commonfunc.GetCartonSeq(cartonbarcode)) >= (int)Convert.ToChar(sqlDr["SECOND_SEQ"].ToString()))
                {
                    cartonsecondseq = (int)Convert.ToChar(commonfunc.GetCartonSeq(cartonbarcode));
                    temp = true;
                }
            sqlDr.Close();
        }

        if (temp == true)
        {
            if (commonfunc.GetBarcodeType(cartonbarcode) == "C")
                packagesql.updatecartonfirst2(sqlCon, commonfunc.GetMainCarton(cartonbarcode), cartonfirstseq + 1, userbarcode);
            else
                packagesql.updatecartonsecondseq(sqlCon, commonfunc.GetMainCarton(cartonbarcode), int.Parse(commonfunc.GetCartonSeq(cartonbarcode)), cartonsecondseq+1, userbarcode);
        }

        //更新CARTON_DT表
        if (commonfunc.GetBarcodeType(cartonbarcode) == "C")
            packagesql.updatecartonfirstdt(sqlCon, docno, userbarcode, int.Parse(commonfunc.GetCartonSeq(cartonbarcode)), cartonbarcode);

        //把barcode part更新为关箱
        packagesql.bundleclose(sqlCon, cartonbarcode, userbarcode, docno);

        //更新箱子的状态为关
        commonsql.updatecartonstatus(sqlCon, userbarcode, cartonbarcode, "C");

        //插入箱的操作历史表
        string sql = "('" + cartonbarcode + "','Close Carton','" + factory + "','" + process + "','" + userbarcode + "',getdate())";
        commonsql.cartonhistory(sqlCon, sql);

        //清空用户扫描表
        commonsql.emptylist(sqlCon, userbarcode);

        sqlCon.Close();
        return "success";
    }

    [WebMethod]
    public static String Closenewcarton(string docno, string userbarcode, string factory, string svTYPE, string process, string production, string languagearray)
    {
        string[] translation = languagearray.Split(new char[] { '*' });
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatment = new Sqlstatement();
        Packagesql packagesql = new Packagesql();
        SqlDataReader sqlDr;

        //判断扫描的裁片是同一个GO
        sqlDr = packagesql.checkgo(sqlCon, docno, userbarcode);
        if(sqlDr.Read())
            if (sqlDr["QTY"].ToString() != "1")
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false1";//扫描的裁片不是同一个GO
            }
        sqlDr.Close();

        //判断扫描的bundle part是否都在本部门并且都为开箱状态
        sqlDr = packagesql.closecheckbundle(sqlCon, docno, userbarcode, process);
        if (sqlDr.Read())
        {
            sqlDr.Close();
            sqlCon.Close();
            return "false3";//bundle part的状态异常
        }
        sqlDr.Close();

        //扫描的都是同样的部件
        //1找出其中一个JO BUNDLE
        string jo = "";
        string bundle = "";
        sqlDr = packagesql.getonejobundle(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
        {
            jo = sqlDr["JOB_ORDER_NO"].ToString();
            bundle = sqlDr["BUNDLE_NO"].ToString();
        }
        sqlDr.Close();
        //2根据这个JO BUNDLE 找出parts
        string partnum = "0";
        sqlDr = packagesql.getpartnum(sqlCon, jo, bundle, docno, userbarcode);
        if (sqlDr.Read())
            partnum = sqlDr["QTY"].ToString();
        sqlDr.Close();
        //3找出扫描的bundle的数量
        string bundlenum = "0";
        sqlDr = packagesql.getbundlenum(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            bundlenum = sqlDr["QTY"].ToString();
        sqlDr.Close();
        //4扫描的行数
        string scanrowqty = "0";
        sqlDr = packagesql.getscanrownum(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            scanrowqty = sqlDr["QTY"].ToString();
        sqlDr.Close();
        //5 3和4比较，不同则错误
        if (int.Parse(bundlenum)*int.Parse(partnum) != int.Parse(scanrowqty))
        {
            sqlCon.Close();
            return "false6";
        }
        //6找出扫描的parts的数量，和2比较，不同则错误
        string scanpartnum = "0";
        sqlDr = packagesql.getscanpartnum(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            scanpartnum = sqlDr["QTY"].ToString();
        sqlDr.Close();
        if (scanpartnum != partnum)
        {
            sqlCon.Close();
            return "false6";
        }

        //同一个GO的主箱码的部件一样
        //副箱码装其余所有的部件
        string cartonbarcode = "";
        //找出扫描的裁片所在GO，判断是否已经关过主箱码
        string temp = "false";
        sqlDr = packagesql.checkgoclosed(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            temp = "true";//已有主箱码
        sqlDr.Close();
        //如果未关过，则关为主箱码；
        if (temp == "false")
        {
            SqlCommand cmd = new SqlCommand("CIPMS_GETFIRSTSEQ", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@DOCNO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@FACTORY", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEW_CARTON_BARCODE", SqlDbType.NVarChar, 30);
            cmd.Parameters["@NEW_CARTON_BARCODE"].Direction = ParameterDirection.Output;
            cmd.Parameters["@DOCNO"].Value = docno;
            cmd.Parameters["@USERBARCODE"].Value = userbarcode;
            cmd.Parameters["@FACTORY"].Value = factory;
            cmd.Parameters["@PROCESS"].Value = process;
            cmd.ExecuteNonQuery();
            cartonbarcode = cmd.Parameters["@NEW_CARTON_BARCODE"].Value.ToString();
        }
        //如果关过，则核查扫描的裁片部件与关过的主箱码的部件是否一样
        else
        {
            //如果一样，则关为主箱码；如果不一样，判断扫描的裁片的部件是不是剩余的所有部件
            temp = "false";
            sqlDr = packagesql.checkpartsame1(sqlCon, factory, docno, userbarcode);
            if (sqlDr.Read())
                temp = "true";
            sqlDr.Close();
            //下面这句有疑问，temp == "false"表示核查扫描的裁片部件与关过的主箱码的部件是一样吧？ tangyh 2017.03.23
            if (temp == "false") 
            {
                sqlDr = packagesql.checkpartsame2(sqlCon, factory, docno, userbarcode);
                if (sqlDr.Read())
                    temp = "true";//part有差异
                sqlDr.Close();
            }
            if (temp == "false")//关为主箱码
            {
                SqlCommand cmd = new SqlCommand("CIPMS_GETFIRSTSEQ", sqlCon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@DOCNO", SqlDbType.NVarChar);
                cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
                cmd.Parameters.Add("@FACTORY", SqlDbType.NVarChar);
                cmd.Parameters.Add("@PROCESS", SqlDbType.NVarChar);
                cmd.Parameters.Add("@NEW_CARTON_BARCODE", SqlDbType.NVarChar, 30);
                cmd.Parameters["@NEW_CARTON_BARCODE"].Direction = ParameterDirection.Output;
                cmd.Parameters["@DOCNO"].Value = docno;
                cmd.Parameters["@USERBARCODE"].Value = userbarcode;
                cmd.Parameters["@FACTORY"].Value = factory;
                cmd.Parameters["@PROCESS"].Value = process;
                cmd.ExecuteNonQuery();
                cartonbarcode = cmd.Parameters["@NEW_CARTON_BARCODE"].Value.ToString();
            }
            else
            {
                temp = "false";
                //判断所有扫描的裁片是否具有同一个主箱码
                if (temp == "false")
                {
                    sqlDr = packagesql.checksamemaincarton(sqlCon, docno, userbarcode);
                    if (sqlDr.Read())
                        if (sqlDr["QTY"].ToString() != "1")
                        {
                            sqlDr.Close();
                            sqlCon.Close();
                            temp = "true";
                            return "false2";//扫描的裁片部件不具有同一个主箱码
                        }
                    sqlDr.Close();
                }
                //判断扫描的裁片的部件是不是剩余的所有部件:扫描的JO BUNDLE未关箱的PART和扫描的PART比较
                sqlDr = packagesql.checkpartsame3(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                {
                    sqlDr.Close();
                    sqlCon.Close();
                    temp = "true";
                    return "false4";
                }
                sqlDr.Close();
                if (temp == "false")
                {
                    sqlDr = packagesql.checkpartsame4(sqlCon, docno, userbarcode);
                    if (sqlDr.Read())
                    {
                        sqlDr.Close();
                        sqlCon.Close();
                        temp = "true";
                        return "false4";
                    }
                    sqlDr.Close();
                }
                if (temp == "false")//剩余的part已扫描，生成副箱码
                {
                    SqlCommand cmd = new SqlCommand("CIPMS_GETSECONDSEQ", sqlCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@DOCNO", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@FACTORY", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@PROCESS", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@NEW_CARTON_BARCODE", SqlDbType.NVarChar, 30);
                    cmd.Parameters["@NEW_CARTON_BARCODE"].Direction = ParameterDirection.Output;
                    cmd.Parameters["@DOCNO"].Value = docno;
                    cmd.Parameters["@USERBARCODE"].Value = userbarcode;
                    cmd.Parameters["@FACTORY"].Value = factory;
                    cmd.Parameters["@PROCESS"].Value = process;
                    cmd.ExecuteNonQuery();
                    cartonbarcode = cmd.Parameters["@NEW_CARTON_BARCODE"].Value.ToString();
                }
            }
        }

        //清空扫描表
        sqlstatment.emptylist(sqlCon, userbarcode);

        Cartonbarcodeprint cartonhtml = new Cartonbarcodeprint();
        string html = cartonhtml.Cartonbarcodehtml(sqlCon, cartonbarcode, languagearray);

        string result = "[{ \"HTML\":\"" + html + "\", \"CARTON\":\"" + cartonbarcode + "\" }]";
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Check(string userbarcode, string factory, string svTYPE, string docno, string cartonbarcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement commonsql = new Sqlstatement();

        string result = "";
        int qty = 0;
        int bundle = 0;
        string detail = "null";
        //扫描比箱少
        int i = 0;
        SqlDataReader sqlDr = commonsql.checkmore(sqlCon, cartonbarcode, docno, userbarcode);
        while (sqlDr.Read())
        {
            qty += int.Parse(sqlDr["QTY"].ToString());
            bundle++;
            if (i == 0)
                detail = sqlDr["BARCODE"] + " " + sqlDr["PART_DESC"];
            else
                detail += "****" + sqlDr["BARCODE"] + " " + sqlDr["PART_DESC"];
            i++;
        }
        sqlDr.Close();
        result = "{ \"less\": [{ \"bundle\": \"" + bundle.ToString() + "\", \"qty\": \"" + qty.ToString() + "\", \"detail\": \"" + detail + "\" }],";

        qty = 0;
        bundle = 0;
        detail = "null";
        //扫描比箱多
        i = 0;
        sqlDr = commonsql.checkless(sqlCon, cartonbarcode, docno, userbarcode);
        while (sqlDr.Read())
        {
            qty += int.Parse(sqlDr["QTY"].ToString());
            bundle++;
            if (i == 0)
                detail = sqlDr["BARCODE"] + " " + sqlDr["PART_DESC"];
            else
                detail += "****" + sqlDr["BARCODE"] + " " + sqlDr["PART_DESC"];
            i++;
        }
        sqlDr.Close();
        result += " \"more\": [{ \"bundle\": \"" + bundle.ToString() + "\", \"qty\": \"" + qty.ToString() + "\", \"detail\": \"" + detail + "\" }]}";

        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Delete(string docno, string process, string userbarcode, string factory, string svTYPE, string id)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement commonsql = new Sqlstatement();
        Packagesql packagesql = new Packagesql();
        Transactionsql transfersql = new Transactionsql();
        SqlDataReader sqlDr;

        //删除扫描历史表的某行
        try
        {
            commonsql.deletepart(sqlCon, docno, userbarcode, id);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error1";
        }

        //判断扫描表是否为空
        try
        {
            sqlDr = commonsql.isempty(sqlCon, docno, userbarcode);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error2";
        }
        if (!sqlDr.Read())
        {
            sqlCon.Close();
            return "false1";//扫描表为空，UI界面初始化
        }
        sqlDr.Close();

        string isprocess = "true";
        string closecarton = "false";
        string opencarton = "false";
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
                isprocess = "false";
            string carton_barcode = "";
            if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
            int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
            if (m == 0)
                html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"];
            else
                html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"];
            m++;
        }
        sqlDr.Close();

        //判断所有已扫描条码是否都在本部门
        //sqlDr = commonsql.isprocess(sqlCon, docno, userbarcode, process);
        //if (sqlDr.Read())
        //    isprocess = "false";
        //sqlDr.Close();

        //按钮的状态判断
        if (isprocess == "true")
        {
            //关箱:判断是否有裁片已经关箱了；
            sqlDr = packagesql.closecartonall(sqlCon, docno, userbarcode);
            if (!sqlDr.Read())
                closecarton = "true";
            sqlDr.Close();
            //判断是否所有的裁片都在同一个组别
            if (closecarton == "true")
            {
                sqlDr = packagesql.closecartonproduction(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                {
                    if (sqlDr["QTY"].ToString() != "1")
                        closecarton = "false";
                }
                else
                {
                    closecarton = "false";
                }
                sqlDr.Close();
            }

            //判断能否进行开箱
            string cartonqty1 = "";
            string cartonqty2 = "";
            if (closecarton == "false")
            {
                //判断扫描的bundle所在的箱的箱内所有的bundle数量和扫描的bundle数量是否一致。一致则说明扫描箱内bundle齐全
                //扫描的在箱内的bundle part数
                sqlDr = packagesql.cartonbundleqty1(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    cartonqty1 = sqlDr["QTY"].ToString();
                sqlDr.Close();
                //实际箱内所有的bundle part数
                sqlDr = packagesql.cartonbundleqty2(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    cartonqty2 = sqlDr["QTY"].ToString();
                sqlDr.Close();
                if (cartonqty1 != "0" && cartonqty2 != "0" && cartonqty1 == cartonqty2)
                    opencarton = "true";
            }
        }

        //所有的裁片都非提交状态
        string issubmit = "false";
        sqlDr = transfersql.checkcartonsubmit(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            issubmit = "true";
        sqlDr.Close();

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
        //    qty = int.Parse(sqlDr["qty"].ToString());
        //    reduce = int.Parse(sqlDr["reduce"].ToString());
        //}
        //sqlDr.Close();
        //pcs = (qty - reduce).ToString();
        //string garment = "0";
        //sqlDr = commonsql.totalgarment(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    if (sqlDr["QTY"].ToString() != "")
        //        garment = sqlDr["QTY"].ToString();
        //sqlDr.Close();
        //sqlCon.Close();
        //string result = "[{ \"HTML\": \"" + html + "\", \"ISSUBMIT\": \"" + issubmit + "\", \"ISPROCESS\": \"" + isprocess + "\", \"OPENCARTON\": \"" + opencarton + "\", \"CLOSECARTON\": \"" + closecarton + "\", \"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \"" + garment + "\" }]";
        string result = "{[{ \"HTML\": \"" + html + "\", \"ISSUBMIT\": \"" + issubmit + "\", \"ISPROCESS\": \"" + isprocess + "\", \"OPENCARTON\": \"" + opencarton + "\", \"CLOSECARTON\": \"" + closecarton + "\", ";
        result = result + GetPcsSummary(sqlCon, docno, userbarcode, "F");
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
        Packagesql packagesql = new Packagesql();
        Transactionsql transfersql = new Transactionsql();
        SqlDataReader sqlDr;

        //删除箱内的bundle
        try
        {
            commonsql.deletecarton(sqlCon, docno, userbarcode, cartonbarcode);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error1";
        }

        //判断扫描表是否为空
        try
        {
            sqlDr = commonsql.isempty(sqlCon, docno, userbarcode);
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return "error2";
        }
        if (!sqlDr.Read())
        {
            sqlCon.Close();
            return "false1";
        }
        sqlDr.Close();

        string isprocess = "true";
        string closecarton = "false";
        string opencarton = "false";

        int m = 0;
        string html = "";
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
                isprocess = "false";
            string carton_barcode = "";
            if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
            int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
            if (m == 0)
                html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"];
            else
                html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"];
            m++;
        }
        sqlDr.Close();

        //判断所有已扫描条码是否都在本部门
        //sqlDr = commonsql.isprocess(sqlCon, docno, userbarcode, process);
        //if (sqlDr.Read())
        //    isprocess = "false";
        //sqlDr.Close();

        if (isprocess == "true")
        {
            //关箱:判断是否有裁片已经关箱了；
            sqlDr = packagesql.closecartonall(sqlCon, docno, userbarcode);
            if (!sqlDr.Read())
                closecarton = "true";
            sqlDr.Close();
            //判断是否所有的裁片都在同一个组别
            if (closecarton == "true")
            {
                sqlDr = packagesql.closecartonproduction(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                {
                    if (sqlDr["QTY"].ToString() != "1")
                        closecarton = "false";
                }
                else
                {
                    closecarton = "false";
                }
                sqlDr.Close();
            }

            //判断能否进行开箱
            string cartonqty1 = "";
            string cartonqty2 = "";
            if (closecarton == "false")
            {
                //判断扫描的bundle所在的箱的箱内所有的bundle数量和扫描的bundle数量是否一致。一致则说明扫描箱内bundle齐全
                //扫描的在箱内的bundle part数
                sqlDr = packagesql.cartonbundleqty1(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    cartonqty1 = sqlDr["QTY"].ToString();
                sqlDr.Close();
                //实际箱内所有的bundle part数
                sqlDr = packagesql.cartonbundleqty2(sqlCon, docno, userbarcode);
                if (sqlDr.Read())
                    cartonqty2 = sqlDr["QTY"].ToString();
                sqlDr.Close();
                if (cartonqty1 != "0" && cartonqty2 != "0" && cartonqty1 == cartonqty2)
                    opencarton = "true";
            }
        }

        //所有的裁片都非提交状态
        string issubmit = "false";
        sqlDr = transfersql.checkcartonsubmit(sqlCon, docno, userbarcode);
        if (sqlDr.Read())
            issubmit = "true";
        sqlDr.Close();

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
        //    qty = int.Parse(sqlDr["qty"].ToString());
        //    reduce = int.Parse(sqlDr["reduce"].ToString());
        //}
        //sqlDr.Close();
        //pcs = (qty - reduce).ToString();
        //string garment = "0";
        //sqlDr = commonsql.totalgarment(sqlCon, docno, userbarcode);
        //if (sqlDr.Read())
        //    if (sqlDr["QTY"].ToString() != "")
        //        garment = sqlDr["QTY"].ToString();
        //sqlDr.Close();
        //sqlCon.Close();
        //string result = "[{ \"HTML\": \""+html+"\", \"ISSUBMIT\": \""+issubmit+"\", \"ISPROCESS\": \"" + isprocess + "\", \"OPENCARTON\": \"" + opencarton + "\", \"CLOSECARTON\": \"" + closecarton + "\", \"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \""+garment+"\" }]";
        string result = "{[{ \"HTML\": \""+html+"\", \"ISSUBMIT\": \""+issubmit+"\", \"ISPROCESS\": \"" + isprocess + "\", \"OPENCARTON\": \"" + opencarton + "\", \"CLOSECARTON\": \"" + closecarton + "\", ";
        result = result + GetPcsSummary(sqlCon, docno, userbarcode, "F");
        return result;
    }

    //20170615-tangyh
    [WebMethod]
    public static String Queryfactory(string factory, string svTYPE)
    {
        string HTML = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement commonsql = new Sqlstatement();
        Packagesql packagesql = new Packagesql();

        //HTML += "<option value='" + "" + "'>" + "" + "</option>";

        SqlDataReader sqlDr = packagesql.queryfactory(sqlCon);
        while (sqlDr.Read())
        {
            HTML += "<option value='" + sqlDr["FACTORY_CD"] + "'>" + sqlDr["FACTORY_CD"] + "</option>";
        }
        //HTML = "{[{ \"HTML\": \"" + HTML + "\" }]}";
        sqlDr.Close();
        sqlCon.Close();
        return HTML;
    }
}
