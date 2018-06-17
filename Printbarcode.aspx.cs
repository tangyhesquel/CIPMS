using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Data;
using PrintBarcode;
using NameSpace;
using Code;
using MSCL;
using Model;
using Newtonsoft.Json;

public partial class Printbarcode : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    //add on 2018-02-26
    [WebMethod]
    public static String printpartselect(string factory, string svTYPE, string jo,string layno)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlCon;
        sqlComGet.CommandText = "SELECT DISTINCT PART_CD FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) WHERE FACTORY_CD='" + factory + "' AND PRINT_PART='Y' AND JOB_ORDER_NO='" + jo + "' AND LAY_NO IN (" + layno + ")";
        sqlComGet.CommandTimeout = 3000;
        sqlDr = sqlComGet.ExecuteReader();
        while (sqlDr.Read())
        {
            result +=  sqlDr["PART_CD"] + ",";
        }
        result = result.Trim().TrimEnd(',');
        result = "\"PRINT_PART\": \"" + result + "\" ";
        result = "[{" + result + "}]";
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Joborderno(string factory, string svTYPE, string joborderno)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlCon;
        //added by lijer on 20161228 控制开了工序才能打菲
        //GEG Quan :CIPMS不用控制
        //sqlComGet.CommandText = "SELECT   1 FROM     PRD_JO_ROUTE_LIST WHERE    PRD_JO_ROUTE_LIST.FACTORY_CD = '" + factory + "'AND PRD_JO_ROUTE_LIST.JOB_ORDER_NO = '" + joborderno + "'AND PRD_JO_ROUTE_LIST.APPROVE_STATUS = 'Y'AND CHECK_POINT_CD <> 'NO'AND EXISTS ( SELECT 1 FROM   dbo.GEN_SYSTEM_SETTING WHERE  SYSTEM_KEY = 'CIPMS_BEFORE_PRINT_BARCODE_CHECK_PROCESS'AND SYSTEM_VALUE = 'Y' )";
        //sqlDr = sqlComGet.ExecuteReader();
        //string process = "N";
        //while (sqlDr.Read())
        //{
        //    process = "Y";
        //}
        //sqlDr.Close();

        string process = "Y";
        result += "\"PROCESS\": \"" + process + "\", ";
        //end added by lijer on 20161228 控制开了工序才能打菲
        sqlComGet.CommandText = "SELECT LAY_NO,STATUS FROM (SELECT LAY_NO,'N' AS STATUS FROM CUT_BUNDLE_HD with (nolock) WHERE factory_cd='" + factory + "' AND CIPMS_PRINT_BUNDLE IN ('Y','C') and JOB_ORDER_NO='" + joborderno + "' GROUP BY LAY_NO EXCEPT SELECT LAY_NO,'N' AS STATUS FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) WHERE factory_cd='" + factory + "'  and JOB_ORDER_NO='" + joborderno + "' GROUP BY LAY_NO UNION SELECT LAY_NO,'Y' AS STATUS FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) WHERE JOB_ORDER_NO='" + joborderno + "' GROUP BY LAY_NO)AS A ORDER BY A.LAY_NO";
//        //兼容针织单给梭织做打菲的床号显示问题
//        sqlComGet.CommandText = @"SELECT  LAY_NO ,  STATUS
//                                                            FROM    ( SELECT    ISNULL(B.NEW_LAY_NO,A.LAY_NO) AS LAY_NO , 'N' AS STATUS
//                                                                      FROM      CUT_BUNDLE_HD A WITH ( NOLOCK ) 
//		                                                              LEFT JOIN CUT_LAY B  WITH(NOLOCK)  ON B.JOB_ORDER_NO=A.JOB_ORDER_NO AND B.LAY_NO=A.LAY_NO AND B.FACTORY_CD=A.FACTORY_CD
//                                                                      WHERE     A.FACTORY_CD = '" + factory + @"'
//                                                                                AND CIPMS_PRINT_BUNDLE IN ( 'Y', 'C' )
//                                                                                AND A.JOB_ORDER_NO = '" + joborderno +@"'
//					                                                            GROUP BY  B.NEW_LAY_NO,A.LAY_NO
//                                                                      EXCEPT
//                                                                      SELECT    LAY_NO ,
//                                                                                'N' AS STATUS
//                                                                      FROM      CIPMS_BUNDLE_FOR_SCANNING WITH ( NOLOCK )
//                                                                      WHERE     FACTORY_CD = '" + factory +@"'
//                                                                                AND JOB_ORDER_NO = '" + joborderno +@"'
//                                                                      GROUP BY  LAY_NO
//                                                                      UNION
//                                                                      SELECT    LAY_NO ,
//                                                                                'Y' AS STATUS
//                                                                      FROM      CIPMS_BUNDLE_FOR_SCANNING WITH ( NOLOCK )
//                                                                      WHERE     JOB_ORDER_NO = '" + joborderno +@"'
//                                                                      GROUP BY  LAY_NO
//                                                                    ) AS A
//                                                            ORDER BY A.LAY_NO";
        sqlComGet.CommandTimeout = 3000;
        sqlDr = sqlComGet.ExecuteReader();
        string layno = "";
        while (sqlDr.Read())
        {
            layno += "<option value='" + sqlDr["LAY_NO"].ToString() + "'>" + sqlDr["LAY_NO"].ToString() + " / " + sqlDr["STATUS"].ToString() + "</option>";
        }
        sqlDr.Close();
        result += "\"LAYNO\": \"" + layno + "\", ";
        //查找该jo的所有part
        string part = "";
        //sqlComGet.CommandText = "select c.PART_CD,c.PART_DESC from PRD_GO_ROUTE_LIST_HD as a inner join PRD_GO_ROUTE_LIST_DT as b on a.ROUTE_HD_ID = b.ROUTE_HD_ID inner join CIPMS_PART_MASTER as c on b.PART_DESC=c.PART_DESC inner join JO_HD as d on a.GO_NO=d.SC_NO where d.JO_NO='" + joborderno + "' group by c.PART_CD,c.PART_DESC,c.ID order by c.ID";
        //UPDATE 20151113 BY JACOB
        //sqlComGet.CommandText = "select d.PART_CD,d.PART_DESC from PRD_GO_ROUTE_LIST_HD as a inner join PRD_GO_ROUTE_LIST_DT as b on a.ROUTE_HD_ID = b.ROUTE_HD_ID inner join GEN_PART_MASTER as c on a.FACTORY_CD = c.FACTORY_CD and b.PART_CD = c.PART_CD inner join CIPMS_PART_MASTER as d on c.PART_DESC=d.PART_DESC inner join JO_HD as e on a.GO_NO=e.SC_NO where e.JO_NO='" + joborderno + "' group by d.PART_CD,d.PART_DESC,d.ID order by d.ID";
        //check是否已经有GO，如果有，则拿就得part

        int i = 0;
        string go = "S" + joborderno.Substring(0, 8);

        List<string> partlist = new List<string>();
        sqlComGet.CommandText = "SELECT DISTINCT PART_CD FROM GEN_GO_PART_INFO WHERE CIPMS_COMBINE_PART_FLAG='Y' AND FACTORY_CD='"+factory+"' AND GO_NO='"+go+"' AND FLAG='Y'";
        sqlDr = sqlComGet.ExecuteReader();
        while (sqlDr.Read())
        {
            partlist.Add(sqlDr["PART_CD"].ToString());
        }
        sqlDr.Close();
        //---------------
        sqlComGet.CommandText = "select A.PART_CD,B.PART_DESC from CIPMS_BUNDLE_FOR_SCANNING AS A with (nolock) INNER JOIN CIPMS_PART_MASTER AS B with (nolock) ON A.FACTORY_CD=B.FACTORY_CD AND A.PART_CD=B.PART_CD where A.FACTORY_CD='" + factory + "' AND A.GARMENT_ORDER_NO='" + go + "' AND B.ACTIVE='Y' group by A.PART_CD,B.PART_DESC,B.SEQ_NO ORDER BY B.SEQ_NO";
        sqlDr = sqlComGet.ExecuteReader();
        while (sqlDr.Read())
        {
            i++;
            if (partlist.Contains(sqlDr["PART_CD"].ToString()))
                part += "<input checked='checked' type='checkbox' name='part' id='" + i.ToString() + "' value='" + sqlDr["PART_CD"] + "'/><label for='" + i.ToString() + "' name='part' value='"+sqlDr["PART_CD"]+"'>" + sqlDr["PART_DESC"] + "</label>";
            else
                part += "<input type='checkbox' name='part' id='" + i.ToString() + "' value='" + sqlDr["PART_CD"] + "'/><label for='" + i.ToString() + "' name='part' value='" + sqlDr["PART_CD"] + "'>" + sqlDr["PART_DESC"] + "</label>";
        }
        sqlDr.Close();

        if (i == 0)
        {
            sqlComGet.CommandText = "select d.PART_CD,d.PART_DESC from PRD_GO_ROUTE_LIST_HD as a with (nolock) inner join PRD_GO_ROUTE_LIST_DT as b with (nolock) on a.ROUTE_HD_ID = b.ROUTE_HD_ID inner join GEN_PART_MASTER as c on a.FACTORY_CD = c.FACTORY_CD and b.PART_CD = c.PART_CD inner join CIPMS_PART_MASTER as d on c.PART_DESC=d.PART_DESC inner join JO_HD as e on a.GO_NO=e.SC_NO where e.JO_NO='" + joborderno + "' AND d.ACTIVE='Y' group by d.PART_CD,d.PART_DESC,d.SEQ_NO order by d.SEQ_NO";
            sqlDr = sqlComGet.ExecuteReader();
            while (sqlDr.Read())
            {
                i++;
                if (partlist.Contains(sqlDr["PART_CD"].ToString()))
                    part += "<input checked='checked' type='checkbox' name='part' id='" + i.ToString() + "' value='" + sqlDr["PART_CD"] + "'/><label for='" + i.ToString() + "' name='part' value='" + sqlDr["PART_CD"] + "'>" + sqlDr["PART_DESC"] + "</label>";
                else
                    part += "<input type='checkbox' name='part' id='" + i.ToString() + "' value='" + sqlDr["PART_CD"] + "'/><label for='" + i.ToString() + "' name='part' value='" + sqlDr["PART_CD"] + "'>" + sqlDr["PART_DESC"] + "</label>";
            }
            sqlDr.Close();
        }

        result += "\"PART\": \"" + part + "\",";
        //added on 2018-02-26--start
        string printpart = "";
        //string carno = "";
        i = 0;
       sqlComGet.CommandText = "select A.PART_CD,B.PART_DESC from CIPMS_BUNDLE_FOR_SCANNING AS A with (nolock) INNER JOIN CIPMS_PART_MASTER AS B with (nolock) ON A.FACTORY_CD=B.FACTORY_CD AND A.PART_CD=B.PART_CD where A.FACTORY_CD='" + factory + "' AND A.GARMENT_ORDER_NO='" + go + "' AND B.ACTIVE='Y'  group by A.PART_CD,B.PART_DESC,B.SEQ_NO ORDER BY B.SEQ_NO";
//        sqlComGet.CommandText = @" SELECT  A.PART_CD ,  B.PART_DESC ,ISNULL(A.CAR_NO,'') AS CAR_NO
//                                                    FROM    CIPMS_BUNDLE_FOR_SCANNING AS A WITH ( NOLOCK )
//                                                            INNER JOIN CIPMS_PART_MASTER AS B WITH ( NOLOCK ) ON A.FACTORY_CD = B.FACTORY_CD   AND A.PART_CD = B.PART_CD
//                                                    WHERE   A.FACTORY_CD = '"+ factory +@"'
//                                                            AND A.GARMENT_ORDER_NO = '" + go + @"' AND A.JOB_ORDER_NO='" + joborderno + @"'
//                                                            AND B.ACTIVE = 'Y'
//                                                    GROUP BY A.PART_CD ,
//                                                            B.PART_DESC ,
//		                                                    A.CAR_NO,
//                                                            B.SEQ_NO 
//                                                    ORDER BY B.SEQ_NO ";

        sqlDr = sqlComGet.ExecuteReader();
        while (sqlDr.Read())
        {
            i++;
            //if (sqlDr["PRINT_PART"].ToString()=="Y")
            //    printpart += "<input checked='checked' type='checkbox' name='printpart'  id='print" + i.ToString() + "' value='" + sqlDr["PART_CD"] + "'/><label for='print" + i.ToString() + "'  name='printpart' value='" + sqlDr["PART_CD"] + "'>" + sqlDr["PART_DESC"] + "</label>";
            //else
                printpart += "<input type='checkbox' name='printpart'  id='print" + i.ToString() + "' value='" + sqlDr["PART_CD"] + "'/><label for='print" + i.ToString() + "'  name='printpart' value='" + sqlDr["PART_CD"] + "'>" + sqlDr["PART_DESC"] + "</label>";
                //carno = sqlDr["CAR_NO"].ToString();
        }
        sqlDr.Close();

        if (i == 0)
        {
            sqlComGet.CommandText = "select d.PART_CD,d.PART_DESC from PRD_GO_ROUTE_LIST_HD as a with (nolock) inner join PRD_GO_ROUTE_LIST_DT as b with (nolock) on a.ROUTE_HD_ID = b.ROUTE_HD_ID inner join GEN_PART_MASTER as c on a.FACTORY_CD = c.FACTORY_CD and b.PART_CD = c.PART_CD inner join CIPMS_PART_MASTER as d on c.PART_DESC=d.PART_DESC inner join JO_HD as e on a.GO_NO=e.SC_NO where e.JO_NO='" + joborderno + "' AND d.ACTIVE='Y' group by d.PART_CD,d.PART_DESC,d.SEQ_NO order by d.SEQ_NO";
            sqlDr = sqlComGet.ExecuteReader();
            while (sqlDr.Read())
            {
                i++;
                //if (partlist.Contains(sqlDr["PART_CD"].ToString()))
                //    printpart += "<input checked='checked' type='checkbox' name='printpart'  id='print" + i.ToString() + "' value='" + sqlDr["PART_CD"] + "'/><label for='print" + i.ToString() + "' name='printpart' value='" + sqlDr["PART_CD"] + "'>" + sqlDr["PART_DESC"] + "</label>";
                //else
                    printpart += "<input type='checkbox' name='printpart'  id='print" + i.ToString() + "' value='" + sqlDr["PART_CD"] + "'/><label for='print" + i.ToString() + "' name='printpart' value='" + sqlDr["PART_CD"] + "'>" + sqlDr["PART_DESC"] + "</label>";
            }
            sqlDr.Close();
        }

        result += "\"PRINTPART\": \"" + printpart + "\"";
      //  result += "\"CARNO\": \"" + carno + "\"";
        //added on 2018-02-26--end

        result = "[{" + result + "}]";
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String JobordernoBundle(string factory, string svTYPE, string joborderno,string layno)
    {
        string result = "";
        string[] laynolist = layno.Split(new char[] { ',' });

        string laynolist1=",";
        foreach (string layno1 in laynolist)
        {
            if (laynolist1 == ",")
            {
                laynolist1 = layno1;
            }
            else
            {
                laynolist1 = laynolist1 + "," + layno1;
            }
        }
        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlCon;
        if (laynolist1=="'")
            sqlComGet.CommandText = "SELECT distinct bundle_no FROM CUT_BUNDLE_HD WHERE factory_cd='" + factory + "' AND CIPMS_PRINT_BUNDLE IN ('Y','C') and status='Y' and JOB_ORDER_NO='" + joborderno + "')";
        else
            sqlComGet.CommandText = "SELECT distinct bundle_no FROM CUT_BUNDLE_HD WHERE factory_cd='" + factory + "' AND CIPMS_PRINT_BUNDLE IN ('Y','C') and status='Y' and JOB_ORDER_NO='" + joborderno + "' and lay_no in (" + laynolist1 + ")";

        sqlDr = sqlComGet.ExecuteReader();
        string BUNDLE = "";
        while (sqlDr.Read())
        {
            BUNDLE += "<option value='" + sqlDr["bundle_no"].ToString() + "'>" + sqlDr["bundle_no"].ToString() + "</option>";
        }
        sqlDr.Close();
        result += "\"BUNDLE\": \"" + BUNDLE + "\"";
        result = "[{" + result + "}]";
        sqlCon.Close();
        return result;
    }

    public static string jo = "";
    public static string barcode = "";
    public static string client = "";
    public static string color = "";
    public static string layno = "";
    public static string bundleno = "";
    public static string size = "";
    public static string qty = "";
    public static string partcd = "";
    public static string productionlinecd = "";
    public static string batchno = "";
    public static string styleno = "";
    public static string markers = "";
    [WebMethod]
    public static String Print(string process, string userbarcode, string factory, string svTYPE, string joborderno, string lay_no, string part, string combinepart, string combinepartcd, string partnum,string printpart,string printpartcd,string printpartnum, string bundle)
    {
        //解析不合并的part
        string[] partlist = part.Split(new char[] { '/' });
        string[] laynolist = lay_no.Split(new char[] { ',' });
        string[] bundlelist = bundle.Split(new char[] { ',' });
        string[] printpartlist = printpart.Split(new char[] {'/'});

        string bundlelist1 = ",";
        if (bundle!="null")
        {
            foreach (string bundle1 in bundlelist)
            {
                if (bundlelist1 == ",")
                {
                    bundlelist1 = bundle1;
                }
                else
                {
                    bundlelist1 = bundlelist1 + "," + bundle1;
                }
            }
        }

        

        //判断保存用户条形码图像的文件夹是否存在：不存在就新建，存在就覆盖
        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Print_barcode sqlstatement = new Print_barcode();
        SqlDataReader sqlDr;
        string result = "";

        string date = "";
        string faillayno = "null";
        string failreason = "";
        int i = 0;
        foreach (string layno in laynolist)
        {
            try
            {
                date = DateTime.Now.ToString("yyMMdd");
                //调用存储过程获取目标裁片的信息并生成条形码值
                string barcodeflag = factory + date;//生成条码前缀标记
                //调用存储过程insert bundle information into CIPMS from MES
                SqlCommand cmd = new SqlCommand("CIPMS_INSERT_BUNDLE_INFORMATION", sqlCon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;

                cmd.Parameters.Add("@JOBORDERNO", SqlDbType.NVarChar);
                cmd.Parameters.Add("@LAYNO", SqlDbType.Int);
                cmd.Parameters.Add("@FACTORY", SqlDbType.NVarChar);
                cmd.Parameters.Add("@PROCESS", SqlDbType.NVarChar);
                cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
                cmd.Parameters.Add("@BARCODEFLAG", SqlDbType.NVarChar);
                cmd.Parameters.Add("@DATE", SqlDbType.NVarChar);
                cmd.Parameters.Add("@PRINT_PART_CD", SqlDbType.NVarChar);
                cmd.Parameters.Add("@IS_INSERT_SUCCESSFUL", SqlDbType.NVarChar, 100);
                cmd.Parameters["@IS_INSERT_SUCCESSFUL"].Direction = ParameterDirection.Output;

                cmd.Parameters["@JOBORDERNO"].Value = joborderno;
                cmd.Parameters["@LAYNO"].Value = int.Parse(layno);
                cmd.Parameters["@FACTORY"].Value = factory;
                cmd.Parameters["@PROCESS"].Value = process;
                cmd.Parameters["@USERBARCODE"].Value = userbarcode;
                cmd.Parameters["@BARCODEFLAG"].Value = barcodeflag;
                cmd.Parameters["@DATE"].Value = date;
                cmd.Parameters["@PRINT_PART_CD"].Value = printpartcd;//added on 2018-02-26
                cmd.ExecuteNonQuery();

                if (cmd.Parameters["@IS_INSERT_SUCCESSFUL"].Value.ToString() == "3")
                {
                    failreason = "3";
                    if (i == 0)
                    {
                        faillayno = layno;
                    }
                    else
                    {
                        faillayno = "," + layno;
                    }
                }
            }
            catch (Exception ex)
            {
                if (i == 0)
                {
                    faillayno = layno;
                }
                else
                {
                    faillayno = "," + layno;
                }
            }
            finally
            {
                i++;
            }
        }

        try
        {
            //string[] partcdlist = combinepartcd.Split(new char[]{'/'});
            //string partsting = "";
            //int k = 0;
            //foreach(string temp in partcdlist)
            //{
            //    if (k == 0)
            //        partsting += "'" + temp + "'";
            //    else
            //        partsting += ",'" + temp + "'";
            //    k++;
            //}
            SqlCommand cmd = new SqlCommand("CIPMS_UPDATE_COMBINE_PART", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@FACTORY_CD", SqlDbType.NVarChar);
            cmd.Parameters.Add("@JOB_ORDER_NO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PART_CD", SqlDbType.NVarChar);
            cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
            cmd.Parameters["@FACTORY_CD"].Value = factory;
            cmd.Parameters["@JOB_ORDER_NO"].Value = joborderno;
            cmd.Parameters["@PART_CD"].Value = combinepartcd;
            cmd.Parameters["@USERBARCODE"].Value = userbarcode;
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
        }

        //查询制单数
        string joqty = "";
        sqlDr = sqlstatement.joqty(sqlCon, joborderno);
        if (sqlDr.Read())
            joqty = sqlDr["JOQTY"].ToString();
        sqlDr.Close();

        string html = "";
        int j = 0;
        List<BUNDLE_INFO> bundleinfolist = new List<BUNDLE_INFO>();
        BUNDLE_INFO onebundleinfo;
        BUNDLE_INFO combinebundleinfo;
        foreach (string layno in laynolist)
        {
            if (j > 0)
                html += "@@";
            j++;
            sqlDr = sqlstatement.jolaynosku(sqlCon, joborderno, layno,bundlelist1);
            date = DateTime.Now.ToString();
            int sizeseq = 0;
            int colorseq = 0;
            string laytranid = "-1";
            i = 0;
            while (sqlDr.Read())
            {
                combinebundleinfo = new BUNDLE_INFO();

                combinebundleinfo.JOB_ORDER_NO = sqlDr["JOB_ORDER_NO"].ToString();
                if (combinebundleinfo.JOB_ORDER_NO.Length > 14)
                    combinebundleinfo.JOB_ORDER_NO = combinebundleinfo.JOB_ORDER_NO.Substring(0, 14);

                combinebundleinfo.BUNDLE_BARCODE = sqlDr["BARCODE"].ToString();
                if (combinebundleinfo.BUNDLE_BARCODE.Length > 15)
                    combinebundleinfo.BUNDLE_BARCODE = combinebundleinfo.BUNDLE_BARCODE.Substring(0, 15);

                combinebundleinfo.COLOR = sqlDr["COLOR_CD"].ToString();

                combinebundleinfo.BUNDLE_NO = sqlDr["BUNDLE_NO"].ToString();
                if (combinebundleinfo.BUNDLE_NO.Length > 4)
                    combinebundleinfo.BUNDLE_NO = combinebundleinfo.BUNDLE_NO.Substring(0, 4);

                combinebundleinfo.SIZE = sqlDr["SIZE_CD"].ToString();
                if (combinebundleinfo.SIZE.Length > 12)
                    combinebundleinfo.SIZE = combinebundleinfo.SIZE.Substring(0, 12);

                combinebundleinfo.QTY = sqlDr["QTY"].ToString();
                if (combinebundleinfo.QTY.Length > 8)
                    combinebundleinfo.QTY = combinebundleinfo.QTY.Substring(0, 8);

                combinebundleinfo.CUT_QTY = joqty;
                if (combinebundleinfo.CUT_QTY.Length > 10)
                    combinebundleinfo.CUT_QTY = combinebundleinfo.CUT_QTY.Substring(0, 10);

                combinebundleinfo.CLIENT = sqlDr["SHORT_NAME"].ToString();
                if (combinebundleinfo.CLIENT.Length > 12)
                    combinebundleinfo.CLIENT = combinebundleinfo.CLIENT.Substring(0, 12);

                combinebundleinfo.STYLE_NO = sqlDr["STYLE_NO"].ToString();
                if (combinebundleinfo.STYLE_NO.Length > 10)
                    combinebundleinfo.STYLE_NO = combinebundleinfo.STYLE_NO.Substring(0, 10);

                combinebundleinfo.BATCH_NO = sqlDr["BATCH_NO"].ToString();
                if (combinebundleinfo.BATCH_NO.Length > 40)
                    combinebundleinfo.BATCH_NO = combinebundleinfo.BATCH_NO.Substring(0, 40);

                combinebundleinfo.PRODUCTION_LINE = sqlDr["CUT_LINE"].ToString();
                if (combinebundleinfo.PRODUCTION_LINE.Length > 8)
                    combinebundleinfo.PRODUCTION_LINE = combinebundleinfo.PRODUCTION_LINE.Substring(0, 8);

                combinebundleinfo.SHADE_LOT = sqlDr["SHADE_LOT"].ToString();
                if (combinebundleinfo.SHADE_LOT.Length > 8)
                    combinebundleinfo.SHADE_LOT = combinebundleinfo.SHADE_LOT.Substring(0, 8);

                combinebundleinfo.PATTERN_NO = sqlDr["PATTERN_NO"].ToString();
                if (combinebundleinfo.PATTERN_NO.Length > 8)
                    combinebundleinfo.PATTERN_NO = combinebundleinfo.PATTERN_NO.Substring(0, 8);

                combinebundleinfo.LAY_NO = layno;
                if (combinebundleinfo.LAY_NO.Length > 12)
                    combinebundleinfo.LAY_NO = combinebundleinfo.LAY_NO.Substring(0, 12);

                combinebundleinfo.DATE = date;
                if (sqlDr["LAY_TRANS_ID"].ToString() != laytranid)
                {
                    sizeseq += 1;
                    laytranid = sqlDr["LAY_TRANS_ID"].ToString();
                    colorseq = 0;
                }
                colorseq += 1;

                combinebundleinfo.MARKERS = GetBundleHearString(sizeseq, colorseq);
                if (combinebundleinfo.MARKERS.Length > 8)
                    combinebundleinfo.MARKERS = combinebundleinfo.MARKERS.Substring(0, 8);

                //构建前台HTML5的标签
                foreach (string n in partlist)
                {
                    if (n != "")
                    {
                        onebundleinfo = new BUNDLE_INFO();

                        onebundleinfo.JOB_ORDER_NO = sqlDr["JOB_ORDER_NO"].ToString();
                        if (onebundleinfo.JOB_ORDER_NO.Length > 14)
                            onebundleinfo.JOB_ORDER_NO = onebundleinfo.JOB_ORDER_NO.Substring(0, 14);

                        onebundleinfo.BUNDLE_BARCODE = sqlDr["BARCODE"].ToString();
                        if (onebundleinfo.BUNDLE_BARCODE.Length > 15)
                            onebundleinfo.BUNDLE_BARCODE = onebundleinfo.BUNDLE_BARCODE.Substring(0, 15);

                        onebundleinfo.COLOR = sqlDr["COLOR_CD"].ToString();

                        onebundleinfo.BUNDLE_NO = sqlDr["BUNDLE_NO"].ToString();
                        if (onebundleinfo.BUNDLE_NO.Length > 4)
                            onebundleinfo.BUNDLE_NO = onebundleinfo.BUNDLE_NO.Substring(0, 4);

                        onebundleinfo.SIZE = sqlDr["SIZE_CD"].ToString();
                        if (onebundleinfo.SIZE.Length > 8)
                            onebundleinfo.SIZE = onebundleinfo.SIZE.Substring(0, 8);

                        onebundleinfo.QTY = sqlDr["QTY"].ToString();
                        if (onebundleinfo.QTY.Length > 8)
                            onebundleinfo.QTY = onebundleinfo.QTY.Substring(0, 8);

                        onebundleinfo.CUT_QTY = joqty;
                        if (onebundleinfo.CUT_QTY.Length > 10)
                            onebundleinfo.CUT_QTY = onebundleinfo.CUT_QTY.Substring(0, 10);

                        onebundleinfo.CLIENT = sqlDr["SHORT_NAME"].ToString();
                        if (onebundleinfo.CLIENT.Length > 12)
                            onebundleinfo.CLIENT = onebundleinfo.CLIENT.Substring(0, 12);

                        onebundleinfo.STYLE_NO = sqlDr["STYLE_NO"].ToString();
                        if (onebundleinfo.STYLE_NO.Length > 10)
                            onebundleinfo.STYLE_NO = onebundleinfo.STYLE_NO.Substring(0, 10);

                        onebundleinfo.BATCH_NO = sqlDr["BATCH_NO"].ToString();
                        if (onebundleinfo.BATCH_NO.Length > 40)//30
                            onebundleinfo.BATCH_NO = onebundleinfo.BATCH_NO.Substring(0, 40);

                        onebundleinfo.PRODUCTION_LINE = sqlDr["CUT_LINE"].ToString();
                        if (onebundleinfo.PRODUCTION_LINE.Length > 8)
                            onebundleinfo.PRODUCTION_LINE = onebundleinfo.PRODUCTION_LINE.Substring(0, 8);

                        onebundleinfo.SHADE_LOT = sqlDr["SHADE_LOT"].ToString();
                        if (onebundleinfo.SHADE_LOT.Length > 8)
                            onebundleinfo.SHADE_LOT = onebundleinfo.SHADE_LOT.Substring(0, 8);

                        onebundleinfo.PATTERN_NO = sqlDr["PATTERN_NO"].ToString();
                        if (onebundleinfo.PATTERN_NO.Length > 8)
                            onebundleinfo.PATTERN_NO = onebundleinfo.PATTERN_NO.Substring(0, 8);

                        onebundleinfo.MARKERS = GetBundleHearString(sizeseq, colorseq);
                        if (onebundleinfo.MARKERS.Length > 8)
                            onebundleinfo.MARKERS = onebundleinfo.MARKERS.Substring(0, 8);

                        onebundleinfo.PART = n;

                        //添加标识部位是否为印花部位
                        if (printpartlist.ToList().IndexOf(n)>=0)
                        {
                            onebundleinfo.PRINTPART_FLAG = "Y";
                        }
                        else
                        {
                            onebundleinfo.PRINTPART_FLAG = "N";
                        }

                        onebundleinfo.LAY_NO = layno;
                        if (onebundleinfo.LAY_NO.Length > 8)
                            onebundleinfo.LAY_NO = onebundleinfo.LAY_NO.Substring(0, 8);
                        
                        onebundleinfo.DATE = date;
                        bundleinfolist.Add(onebundleinfo);
                        //if (i == 0)
                        //    html += barcode + "|" + client + "|" + joborderno + "|" + color + "|" + layno + "|" + bundleno + "|" + date + "|" + joqty + "|" + batchno + "|" + n + "|" + markers + "|" + styleno + "|" + productionlinecd + "|" + size + "|" + qty;
                        //else
                        //    html += "*****" + barcode + "|" + client + "|" + joborderno + "|" + color + "|" + layno + "|" + bundleno + "|" + date + "|" + joqty + "|" + batchno + "|" + n + "|" + markers + "|" + styleno + "|" + productionlinecd + "|" + size + "|" + qty;
                        //i++;
                    }
                }
                if (partnum != "0")
                {
                    combinebundleinfo.PART = combinepart;

                    bundleinfolist.Add(combinebundleinfo);
                    //if (i == 0)
                    //    html += barcode + "|" + client + "|" + joborderno + "|" + color + "|" + layno + "|" + bundleno + "|" + date + "|" + joqty + "|" + batchno + "|" + combinepart + "|" + markers + "|" + styleno + "|" + productionlinecd + "|" + size + "|" + qty;
                    //else
                    //    html += "*****" + barcode + "|" + client + "|" + joborderno + "|" + color + "|" + layno + "|" + bundleno + "|" + date + "|" + joqty + "|" + batchno + "|" + combinepart + "|" + markers + "|" + styleno + "|" + productionlinecd + "|" + size + "|" + qty;
                    //i++;
                }
            }
            sqlDr.Close();
        }
        sqlCon.Close();

        result = "{ \"BUNDLEINFO\": " + JsonConvert.SerializeObject(bundleinfolist) + ", \"OTHER\": [{ \"FAILLAYNO\": \"" + faillayno + "\", \"FAILREASON\": \"" + failreason + "\" }] }";

        //result = "[{ \"HTML\":\"" + JsonConvert.SerializeObject(bundleinfolist) + "\", \"FAILLAYNO\":\"" + faillayno + "\", \"FAILREASON\":\"" + failreason + "\" }]";
        return result;
    }
    public static String GetBundleHearString(int sizeSeq, int colorSeq)
    {
        string strBundleHead = "";
        int i;
        string result = "";
        do
        {
            i = sizeSeq % 26;
            if (i > 0)
                strBundleHead = ((char)(64 + i)).ToString() + strBundleHead;
            else
                strBundleHead = "Z" + strBundleHead;
            if (sizeSeq == 26)
                sizeSeq = 0;
            else
                sizeSeq = sizeSeq / 26;
        } while (sizeSeq > 0);

        result = strBundleHead + colorSeq.ToString();
        return result;
    }
}