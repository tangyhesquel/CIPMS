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

public partial class Receive : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    //解析构造part字符串
    public static String Partanalysis(string part)
    {
        string[] partarray = part.Split(new char[] { ',' });
        for (int i = 0; i < partarray.Length; i++)
        {
            if (i == 0) part = "'" + partarray[i] + "'";
            else part += ",'" + partarray[i] + "'";
        }
        part = "(" + part + ")";
        return part;
    }

    [WebMethod]
    public static String Partselect(string factory, string svTYPE)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;
        string result = "";
        sqlDr = sqlstatement.partselect(sqlCon, factory);
        while (sqlDr.Read())
        {
            result += "<option value='" + sqlDr["PART_CD"] + "'>" + sqlDr["PART_DESC"] + "</option>";
        }
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Production(string factory, string svTYPE, string process)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;
        string result = "";
        sqlDr = sqlstatement.peerproduction(sqlCon, factory, process);
        while (sqlDr.Read())
        {
            result += "<option value='" + sqlDr["PRODUCTION_LINE_CD"] + "'>" + sqlDr["PRODUCTION_LINE_NAME"] + "</option>";
        }
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    //判断是否需要选择接收组别
    [WebMethod]
    public static String Checkneedproduction(string factory, string svTYPE, string lastprocess, string process, string lastprocesstype, string processtype)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        Receivesql receivesql = new Receivesql();
        sqlCon.Open();
        SqlDataReader sqlDr;
        sqlDr = receivesql.checkneedproduction(sqlCon, factory, lastprocess, process, lastprocesstype, processtype);
        if (sqlDr.Read())
            if (sqlDr["NEED_PRODUCTION"].ToString() == "Y")
                result = "Y";
            else
                result = "N";
        else
            result = "N";
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    //判断接收的裁片是否在箱子里面
    [WebMethod]
    public static String Opencarton(string userbarcode, string newdocno, string factory, string svTYPE)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        SqlDataReader sqlDr;
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlCon;
        sqlComGet.CommandText = "select b.BUNDLE_ID from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + newdocno + "' and a.USER_BARCODE='" + userbarcode + "' and b.CARTON_STATUS='C'";
        sqlDr = sqlComGet.ExecuteReader();
        if (sqlDr.Read())
            result = "true";
        else
            result = "false";
        return result;
    }

    [WebMethod]
    public static String Checkbarcode(string docno, string barcode, string factory, string svTYPE, string part)
    {
        string[] partarray = part.Split(new char[] { ',' });
        for (int i = 0; i < partarray.Length; i++)
        {
            if (i == 0) part = "'" + partarray[i] + "'";
            else part += ",'" + partarray[i] + "'";
        }
        part = "(" + part + ")";
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Receivesql receivesql = new Receivesql();
        SqlDataReader sqlDr;
        sqlDr = receivesql.checkbarcode(sqlCon, barcode, part);
        int j = 0;
        while (sqlDr.Read())
            j++;
        sqlDr.Close();
        string result="";
        if (j == partarray.Length)
            result = "true";//都在本厂
        else
            result = "false";
        sqlCon.Close();
        return result;
    }

    //barcode scan
    [WebMethod]
    public static String Barcodescan(string factory, string svTYPE, string process, string barcode, string docno, string userbarcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement commonsql = new Sqlstatement();
        Receivesql receivesql = new Receivesql();
        SqlDataReader sqlDr;
        string DOC_NO = "";
        string result = "";

        //根据用户扫描的Barcode找到其所在的DOC_NO。insert该DOC_NO
        string barcodetype = GetBarcodeType(barcode);

        if (barcodetype == "D")
        {
            DOC_NO = barcode.Substring(3);
            sqlDr = commonsql.getdocnobydocno(sqlCon, DOC_NO);
            if (sqlDr.Read())
            {
                DOC_NO= sqlDr["DOC_NO"].ToString();
            }
            else
            {
                sqlCon.Close();
                return "false1";//没有找到提交状态的流水单
            }
            sqlDr.Close();
        }
        else if (barcodetype == "B")
        {
            sqlDr = commonsql.getdocnobybundle(sqlCon, barcode);
            int i = 0;
            while (sqlDr.Read())
            {
                DOC_NO = sqlDr["DOC_NO"].ToString();
                i++;
            }
            sqlDr.Close();
            if (i == 0)
            {
                sqlCon.Close();
                return "false1";
            }
            else if (i > 1)
            {
                sqlCon.Close();
                return "false4";//扫描的裁片有多个流水单
            }
        }

        //CARTON BARCODE
        else if (barcodetype == "C")
        {
            sqlDr = commonsql.getdocnobycarton(sqlCon, barcode);
            int i = 0;
            while (sqlDr.Read())
            {
                DOC_NO = sqlDr["DOC_NO"].ToString();
                i++;
            }
            sqlDr.Close();
            if (i == 0)
            {
                sqlCon.Close();
                return "false1";
            }
            else if (i > 1)
            {
                sqlCon.Close();
                return "false4";//扫描的裁片有多个流水单
            }
        }

        //判断是否是同一个用户进行接收
        string submituserid = "";
        string sameuserflag = "false";
        sqlDr = receivesql.checksameuserconfirm(sqlCon, DOC_NO);
        if (sqlDr.Read())
            sameuserflag = "true";
        sqlDr.Close();
        if (sameuserflag == "true")
        {
            //查找提交用户
            sqlDr = commonsql.getsubmituser(sqlCon, DOC_NO);
            if (sqlDr.Read())
                submituserid = sqlDr["CREATE_USER_ID"].ToString();
            sqlDr.Close();
            if (submituserid != userbarcode)
            {
                //sqlCon.Close();
                //return "false2";//接收者必须和发送者一致 tangyh 2017.05.04
            }
            
        }
        else
        {
            //判断接收用户是否是下道部门的
            sqlDr = receivesql.checkuserreceive(sqlCon, userbarcode, DOC_NO);
            if (!sqlDr.Read())
            {
                sqlCon.Close();
                return "false3";//必须下道部门的用户进行接收
            }
            sqlDr.Close();
        }

        //把流水单插入到扫描表
        commonsql.unscandocnoinsert(sqlCon, docno, userbarcode, DOC_NO, "Receive");

        CheckBundleStatus cbs = new CheckBundleStatus();
        //cbs.setisneedbundleprocess(true);//裁片是否在本部门
        cbs.setisneedonesend(true);//裁片是否只有一个发送部门组别
        cbs.setisneedonereceive(true);//裁片是否只有一个接收部门组别
        cbs.setisneedonecutgarmenttype(true);//裁片是否是同一个裁床garment_type
        if (process == "DC")
        {
            cbs.setisneedonesewgarmenttype(true);//裁片是否是同一个车缝garment_type
            cbs.setisneedfullbundle(true);//裁片的幅位是否齐全
        }

        if (!cbs.checkfunc(sqlCon, docno, userbarcode, factory, process))
        {
            sqlCon.Close();
            return "error1";//检查裁片的状态的存储过程发生错误
        }

        if (cbs.issubmit == "false" || cbs.isfullbundle == "false" || cbs.isbundleprocess == "false" || cbs.isonereceive == "false" || cbs.isonesend == "false" || cbs.isonecutgarmenttype == "false" || cbs.isonesewgarmenttype == "false")
        {
            //把扫描的delete掉
            commonsql.deletescanned(sqlCon, docno, userbarcode, factory+DOC_NO);
            result = "[{ \"STATUS\":\"N\", \"ISCARTON\":\"" + cbs.iscarton + "\", \"ISSUBMIT\":\"" + cbs.issubmit + "\", \"ISFULLBUNDLE\":\"" + cbs.isfullbundle + "\", \"ISPROCESS\":\"" + cbs.isbundleprocess + "\", \"ISONERECEIVE\":\"" + cbs.isonereceive + "\", \"ISONESEND\":\"" + cbs.isonesend + "\", \"ISONECUTGARMENTTYPE\":\"" + cbs.isonecutgarmenttype + "\", \"ISONESEWGARMENTTYPE\":\"" + cbs.isonesewgarmenttype + "\" }]";
            sqlCon.Close();
            return result;
        }
        else
        {
            int n = 0;
            if (cbs.sendprocess == "SEW")
                sqlDr = commonsql.bundlescannedbybundle(sqlCon, docno, userbarcode);
            else
            {
                sqlDr = commonsql.bundlescannedbypart(sqlCon, docno, userbarcode);
            }
            string html = "";
            string parts = "";
            while (sqlDr.Read())
            {
                string carton_barcode = "";
                if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                    carton_barcode = "";
                else
                    carton_barcode = sqlDr["CARTON_BARCODE"].ToString();

                if (cbs.sendprocess != "SEW")
                {
                    if (n == 0)
                        parts = sqlDr["PART_DESC"].ToString();
                    else
                        parts += "," + sqlDr["PART_DESC"].ToString();
                }

                int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
                if (cbs.sendprocess == "SEW")
                {
                    if (n == 0)
                        html += sqlDr["BARCODE"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DOC_NO"];
                    else
                        html += "@" + sqlDr["BARCODE"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DOC_NO"];
                }
                else
                {
                    if (n == 0)
                        html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DOC_NO"];
                    else
                        html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DOC_NO"];
                }
                n++;
            }
            result += "[{ \"STATUS\":\"Y\", \"DOC_NO\":\""+factory+DOC_NO+"\", \"RECEIVEFACTORY\":\"" + cbs.sendfactory + "\", \"RECEIVEPROCESS\":\"" + cbs.sendprocess + "\", \"RECEIVEPRODUCTION\":\"" + cbs.sendproduction + "\", \"SENDFACTORY\":\"" + cbs.receivefactory + "\", \"SENDPROCESS\":\"" + cbs.receiveprocess + "\", \"SENDPRODUCTION\":\"" + cbs.receiveproduction + "\", \"PART\":\"" + parts + "\", \"HTML\":\"" + html + "\", ";
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
                if(sqlDr["qty"].ToString() != "")
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

            result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \"" + garment + "\" }]";
            sqlCon.Close();

            return result;
        }
    }

    [WebMethod]
    public static String OASBarcodescan(string docno, string factory, string svTYPE, string barcode, string process, string productionline, string userbarcode, string part, string chooseproduction, string productionselected)
    {
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
        Connect connectstring = new Connect();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement commonsql = new Sqlstatement();
        Receivesql receivesql = new Receivesql();
        SqlDataReader sqlDr;

        string html = "";
        //判断该bundle是否是外发状态。
        sqlDr = receivesql.bundleinformation(sqlCon, barcode, part);
        int j = 0;
        string doc_no = "";
        string sql = "";
        while (sqlDr.Read())
        {
            if (j == 0)
                doc_no = sqlDr["DOC_NO"].ToString();
            else
                if (doc_no != sqlDr["DOC_NO"].ToString())
                {
                    sqlDr.Close();
                    sqlCon.Close();
                    return "false1";//单号不一
                }
            if (sqlDr["PROCESS_TYPE"].ToString() != "O" || sqlDr["PROCESS_TYPE"].ToString() != "E")
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false";//该条码不是外发状态
            }
            if (j == 0)
                sql += "('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Receive',getdate())";
            else
                sql += ",('" + sqlDr["BUNDLE_ID"] + "','" + docno + "','" + userbarcode + "','" + sqlDr["BARCODE"] + "','" + sqlDr["PART_CD"] + "','Receive',getdate())";
            string carton_barcode = "";
            if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
            html += "<tr id='" + sqlDr["BUNDLE_ID"] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center'>" + sqlDr["DOC_NO"] + "</td>";
            int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
            html += "<td style='vertical-align:middle; text-align:center'>" + carton_barcode + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["PROCESS_CD"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["BARCODE"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["PART_CD"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["BUNDLE_NO"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["LAY_NO"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["SIZE_CD"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["COLOR_CD"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["QTY"] + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + output.ToString() + "</td>";
            html += "<td style='vertical-align:middle; text-align:center'>" + sqlDr["DEFECT"] + "</td></tr>";
            j++;
        }
        sqlDr.Close();

        if (j == 0 || j != partnum)
        {
            sqlCon.Close();
            return "false";//不存在该条码
        }

        //插入到扫描表
        commonsql.unscaninsert(sqlCon, sql);

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

        //返回流水单号和bundle信息
        sqlCon.Close();
        return "[{ \"HTML\": \"" + html + "\", \"DOC_NO\": \"" + doc_no + "\", \"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \"" + garment + "\" }]";
    }

    [WebMethod]
    public static String Receivecheckload(string docno, string userbarcode, string olddocno, string newdocno, string factory, string svTYPE, string process)
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
        string sql = "update CIPMS_USER_SCANNING_DFT set DOC_NO='" + newdocno + "' where DOC_NO='" + olddocno + "' and USER_BARCODE='" + userbarcode + "'";
        SqlCommand cmd = new SqlCommand(sql, sqlCon);
        cmd.ExecuteNonQuery();

        string html = "";
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlCon;
        sqlComGet.CommandText = "SELECT DOC_NO,PROCESS_TYPE,BUNDLE_ID,CARTON_BARCODE,a.FACTORY_CD,PROCESS_CD,PRODUCTION_LINE_CD,PROCESS_TYPE,BARCODE,a.PART_CD,b.PART_DESC,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + newdocno + "' and USER_BARCODE='" + userbarcode + "')";
        sqlDr = sqlComGet.ExecuteReader();
        while (sqlDr.Read())
        {
            string carton_barcode = "";
            if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                carton_barcode = "";
            else
                carton_barcode = sqlDr["CARTON_BARCODE"].ToString();
            html += "<tr id='" + sqlDr["BUNDLE_ID"] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center'>" + sqlDr["DOC_NO"] + "</td>";
            int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
            html += "<td style='vertical-align:middle; text-align:center'>" + carton_barcode + "</td>";
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
        }
        sqlDr.Close();

        result += "[{ \"HTML\": \"" + html + "\", ";

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

        //找出流转单号的上道submit工厂、部门、组别
        sqlDr = commonsql.reclastsubmit(sqlCon, docno.Substring(3));
        if (sqlDr.Read())
        {
            result += "\"FACTORY\":\"" + sqlDr["FACTORY_CD"] + "\", \"PROCESS\":\"" + sqlDr["PROCESS_CD"] + "\", \"PRODUCTIONLINE\":\"" + sqlDr["PRODUCTION_LINE_CD"] + "\", ";
        }
        sqlDr.Close();

        result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \""+garment+"\" }]";
        sqlCon.Close();
        return result;
    }

    //process为I的transaction接收
    [WebMethod]
    public static String Transactionconfirm(string factory, string svTYPE, string process, string docno, string userbarcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement commonsql = new Sqlstatement();
        SqlCommand cmd;

        try
        {
            cmd = new SqlCommand("CIPMS_SHIPPER_RECEIVE", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@DOCNO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@JUSTCHECK", SqlDbType.NChar);
            cmd.Parameters.Add("@IS_CONFIRM_SUCCESSFULLY", SqlDbType.NVarChar, 100);
            cmd.Parameters["@IS_CONFIRM_SUCCESSFULLY"].Direction = ParameterDirection.Output;
            cmd.Parameters["@DOCNO"].Value = docno;
            cmd.Parameters["@USERBARCODE"].Value = userbarcode;
            cmd.Parameters["@JUSTCHECK"].Value = "N";
            cmd.ExecuteNonQuery();
            if (cmd.Parameters["@IS_CONFIRM_SUCCESSFULLY"].Value.ToString() == "1")
            {
                return "success";
            }
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

    //process为I的GTN transaction接收
    [WebMethod]
    public static String GTNTransactionconfirm(string factory, string svTYPE, string process, string docno, string userbarcode)
    {
        //处理docnolist
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        SqlCommand cmd;
        string date = DateTime.Today.ToString("yyyyMMdd");

        try
        {
            cmd = new SqlCommand("CIPMS_GTN_RECEIVE", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@DOCNO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@DATE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@IS_CONFIRM_SUCCESSFULLY", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@IS_EXCEED_WIP", SqlDbType.NChar, 1);
            cmd.Parameters["@IS_CONFIRM_SUCCESSFULLY"].Direction = ParameterDirection.Output;
            cmd.Parameters["@IS_EXCEED_WIP"].Direction = ParameterDirection.Output;
            cmd.Parameters["@DOCNO"].Value = docno;
            cmd.Parameters["@USERBARCODE"].Value = userbarcode;
            cmd.Parameters["@DATE"].Value = date;
            cmd.ExecuteNonQuery();
            if (cmd.Parameters["@IS_CONFIRM_SUCCESSFULLY"].Value.ToString() == "1" && cmd.Parameters["@IS_EXCEED_WIP"].Value.ToString() == "N")
            {
                return "success";
            }
            else if (cmd.Parameters["@IS_EXCEED_WIP"].Value.ToString() == "Y")
            {
                return "Y";
            }
            else if (cmd.Parameters["@IS_CONFIRM_SUCCESSFULLY"].Value.ToString() != "1")
            {
                return "false"+cmd.Parameters["@IS_CONFIRM_SUCCESSFULLY"].Value.ToString();
            }
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

    //process为I的transaction拒绝接收
    [WebMethod]
    public static String Transactionreject(string docno, string factory, string svTYPE, string process, string productionline, string userbarcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;
        string result = "";
        string barcode = "";

        //根据docno的前缀连接相应的数据库服务器

        //根据扫描表找出所有docno(barcode)
        sqlDr = sqlstatement.recconfirmdocno(sqlCon, userbarcode, docno);
        int i = 0;
        while (sqlDr.Read())
        {
            if (i == 0)
            {
                result = sqlDr["DOC_NO"].ToString();
                barcode = "'" + sqlDr["DOC_NO"] + "'";
            }
            else
            {
                result += "," + sqlDr["DOC_NO"].ToString();
                barcode += ",'" + sqlDr["DOC_NO"] + "'";
            }
            i++;
        }
        sqlDr.Close();
        barcode = "(" + barcode + ")";

        //更新transfer_hd表的status、reject_user、reject_date
        sqlstatement.updaterecrejecttranhd(sqlCon, barcode, "R", userbarcode);

        //更新wip_bundle的intrans_qty=0
        sqlstatement.updaterecrejectwipbundle(sqlCon, barcode);

        //更新wip_hd的intrans_qty = intrans_qty-qty
        sqlstatement.updaterecrejectwiphd(sqlCon, barcode);

        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Save(string userbarcode, string docno, string factory, string svTYPE, string module)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;

        //根据已经扫描的bundle_id找出所在的所有doc_no，并且保存下来
        sqlstatement.recsave(sqlCon, docno, userbarcode, module);
        sqlDr = sqlstatement.recsave(sqlCon, docno, userbarcode);
        string result = "";
        int i = 0;
        while (sqlDr.Read())
        {
            if (i == 0)
                result += sqlDr["DOC_NO"];
            else
                result += "," + sqlDr["DOC_NO"];
        }
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Query(string factory, string svTYPE, string userbarcode, string module)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;
        string result = "";
        //根据已经扫描的bundle_id找出所在的所有doc_no，并且保存下来
        sqlDr = sqlstatement.recquery(sqlCon, module, userbarcode);
        while (sqlDr.Read())
            result += "<option value='" + sqlDr["DOC_NO"] + "'>" + sqlDr["DOC_NO"] + "</option>";
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Querychange(string factory, string svTYPE, string process, string docno, string userbarcode, string date)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connect = new Connect();
        sqlCon.ConnectionString = connect.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement commonsql = new Sqlstatement();
        SqlDataReader sqlDr;
        SqlCommand cmd;
        string result = "";

        //把流水单插入到扫描表
        commonsql.unscanqueryinsert(sqlCon, docno, userbarcode, date, "Receive");

        try
        {
            cmd = new SqlCommand("CIPMS_SHIPPER_RECEIVE", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@DOCNO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@JUSTCHECK", SqlDbType.NChar);
            cmd.Parameters.Add("@IS_CONFIRM_SUCCESSFULLY", SqlDbType.NVarChar, 100);
            cmd.Parameters["@IS_CONFIRM_SUCCESSFULLY"].Direction = ParameterDirection.Output;
            cmd.Parameters["@DOCNO"].Value = docno;
            cmd.Parameters["@USERBARCODE"].Value = userbarcode;
            cmd.Parameters["@JUSTCHECK"].Value = "Y";
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            return "false" + ex.Message;
        }

        CheckBundleStatus cbs = new CheckBundleStatus();
        cbs.setisneedonecutgarmenttype(true);//裁片是否是同一个裁床garment_type
        if (process == "DC")
        {
            cbs.setisneedonesewgarmenttype(true);//裁片是否是同一个车缝garment_type
            cbs.setisneedfullbundle(true);//裁片的幅位是否齐全
        }

        if (!cbs.checkfunc(sqlCon, docno, userbarcode, factory, process))
        {
            sqlCon.Close();
            return "error1";//检查裁片的状态的存储过程发生错误
        }

        if (cbs.issubmit == "false" || cbs.isfullbundle == "false" || cbs.isbundleprocess == "false" || cbs.isonereceive == "false" || cbs.isonesend == "false" || cbs.isonecutgarmenttype == "false" || cbs.isonesewgarmenttype == "false")
        {
            //把扫描的delete掉
            commonsql.deletescanned(sqlCon, docno, userbarcode, date, "Receive");
            result = "[{ \"STATUS\":\"N\", \"ISCARTON\":\"" + cbs.iscarton + "\", \"ISSUBMIT\":\"" + cbs.issubmit + "\", \"ISFULLBUNDLE\":\"" + cbs.isfullbundle + "\", \"ISPROCESS\":\"" + cbs.isbundleprocess + "\", \"ISONERECEIVE\":\"" + cbs.isonereceive + "\", \"ISONESEND\":\"" + cbs.isonesend + "\", \"ISONECUTGARMENTTYPE\":\"" + cbs.isonecutgarmenttype + "\", \"ISONESEWGARMENTTYPE\":\"" + cbs.isonesewgarmenttype + "\" }]";
            sqlCon.Close();
            return result;
        }
        else
        {
            int n = 0;
            if (cbs.sendprocess == "SEW")
                sqlDr = commonsql.bundlescannedbybundle(sqlCon, docno, userbarcode);
            else
            {
                sqlDr = commonsql.bundlescannedbypart(sqlCon, docno, userbarcode);
            }
            string html = "";
            string parts = "";
            while (sqlDr.Read())
            {
                string carton_barcode = "";
                if (sqlDr["CARTON_BARCODE"].ToString() == "0" || sqlDr["CARTON_STATUS"].ToString() != "C")
                    carton_barcode = "";
                else
                    carton_barcode = sqlDr["CARTON_BARCODE"].ToString();

                if (cbs.sendprocess != "SEW")
                {
                    if (n == 0)
                        parts = sqlDr["PART_DESC"].ToString();
                    else
                        parts += "," + sqlDr["PART_DESC"].ToString();
                }

                int output = int.Parse(sqlDr["QTY"].ToString()) - int.Parse(sqlDr["DISCREPANCY_QTY"].ToString());
                if (cbs.sendprocess == "SEW")
                {
                    if (n == 0)
                        html += sqlDr["BARCODE"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DOC_NO"];
                    else
                        html += "@" + sqlDr["BARCODE"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DOC_NO"];
                }
                else
                {
                    if (n == 0)
                        html += sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DOC_NO"];
                    else
                        html += "@" + sqlDr["BUNDLE_ID"] + "|" + carton_barcode + "|" + sqlDr["PROCESS_CD"] + "|" + sqlDr["BARCODE"] + "|" + sqlDr["PART_DESC"] + "|" + sqlDr["JOB_ORDER_NO"] + "|" + sqlDr["BUNDLE_NO"] + "|" + sqlDr["LAY_NO"] + "|" + sqlDr["COLOR_CD"] + "|" + sqlDr["SIZE_CD"] + "|" + sqlDr["QTY"] + "|" + output.ToString() + "|" + sqlDr["DEFECT"] + "|" + sqlDr["DISCREPANCY_QTY"] + "|" + sqlDr["DOC_NO"];
                }
                n++;
            }
            result += "[{ \"STATUS\":\"Y\", \"RECEIVEFACTORY\":\"" + cbs.sendfactory + "\", \"RECEIVEPROCESS\":\"" + cbs.sendprocess + "\", \"RECEIVEPRODUCTION\":\"" + cbs.sendproduction + "\", \"SENDFACTORY\":\"" + cbs.receivefactory + "\", \"SENDPROCESS\":\"" + cbs.receiveprocess + "\", \"SENDPRODUCTION\":\"" + cbs.receiveproduction + "\", \"PART\":\"" + parts + "\", \"HTML\":\"" + html + "\", ";
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

            result += "\"TOTALBUNDLES\": \"" + bundle + "\", \"TOTALPCS\": \"" + pcs + "\", \"TOTALGARMENTPCS\": \"" + garment + "\" }]";
            sqlCon.Close();

            return result;
        }
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
            {
                return "B";
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
}
