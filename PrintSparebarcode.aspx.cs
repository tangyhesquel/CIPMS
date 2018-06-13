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

public partial class PrintSparebarcode : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    [WebMethod]
    public static String GetGoMaxLayno(string process, string userbarcode, string factory, string svTYPE, string gono,string currentlayno)
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

        string flag = "N";
        int maxLayno = 0;
        string isExistLayNo = "N";

        //首先判断该床号是否存在
        sqlComGet.CommandText = @"SELECT  TOP 1  1  AS  MAX_LAYNO  from  CIPMS_SPARE_CODE_PRINT  WITH(NOLOCK)  WHERE SC_NO='" + gono + "'   and LAY_NO='" + currentlayno + "'";
        sqlComGet.CommandTimeout = 3000;
        sqlDr = sqlComGet.ExecuteReader();
         while (sqlDr.Read())
        {
             if(sqlDr["MAX_LAYNO"].ToString()=="1")
            {
                 isExistLayNo = "Y";
                 flag = "Y";
                maxLayno = 0;
            }
        }
         sqlDr.Close();

         if (isExistLayNo=="N")
        {
                    //获取GO 最大床号
                    sqlComGet.CommandText =
                        @"SELECT   ISNULL(MAX(LAY_NO),0)  AS MAX_LAYNO   from  CIPMS_SPARE_CODE_PRINT  WITH(NOLOCK)  WHERE SC_NO='" +
                        gono + "'  ";
                    sqlComGet.CommandTimeout = 3000;
                    sqlDr = sqlComGet.ExecuteReader();
                    string layno = "";
                    while (sqlDr.Read())
                    {
                        maxLayno = int.Parse(sqlDr["MAX_LAYNO"].ToString());
                    }
                    sqlDr.Close();
                    if (int.Parse(currentlayno) - maxLayno == 1)
                    {
                        flag = "Y";

                    }
                    else
                    {
                        flag = "N";
                    }
        }
         result += "\"FLAG\": \"" + flag + "\", ";
         result += "\"LAYNO\": \"" + maxLayno + "\", ";
        result = "[{" + result + "}]";
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String GonoInfo(string factory, string svTYPE, string gono)
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

        string process = "Y";
        result += "\"PROCESS\": \"" + process + "\", ";

        //获取GO 床号
        sqlComGet.CommandText = @"SELECT   DISTINCT LAY_NO   from  CIPMS_SPARE_CODE_PRINT  WITH(NOLOCK)  WHERE SC_NO='" + gono + "'  ORDER BY LAY_NO ";
        sqlComGet.CommandTimeout = 3000;
        sqlDr = sqlComGet.ExecuteReader();
        string layno = "";
        while (sqlDr.Read())
        {
            layno += "<option value='" + sqlDr["LAY_NO"] + "'>" + sqlDr["LAY_NO"] + "</option>";
        }
        sqlDr.Close();
        result += "\"LAYNO\": \"" + layno + "\", ";

        //最大的床号
        sqlComGet.CommandText = @"SELECT   ISNULL(MAX(LAY_NO),0)+1  AS MAX_LAY   from  CIPMS_SPARE_CODE_PRINT  WITH(NOLOCK)  WHERE SC_NO='" + gono + "' ";
        sqlComGet.CommandTimeout = 3000;
        sqlDr = sqlComGet.ExecuteReader();
        string maxlayno = "";
        while (sqlDr.Read())
        {
            maxlayno += sqlDr["MAX_LAY"];
        }
        sqlDr.Close();
        result += "\"MAXLAYNO\": \"" + maxlayno + "\", ";

        var isfirtlay = "N";
        if (maxlayno == "1")
        {
            isfirtlay = "Y";
        }
        result += "\"ISFIRSTLAY\": \"" + isfirtlay + "\", ";

        //获取第一床选的部位
        string lastlaySelectPart = "";
        if (maxlayno != "" || maxlayno !="1")
        {
            sqlComGet.CommandText = @"SELECT  ISNULL(STUFF((SELECT  DISTINCT '/'+  PART_CD FROM CIPMS_SPARE_CODE_PRINT  WHERE  SC_NO='" + gono + "' and  LAY_NO='1'    FOR XML PATH('')), 1, 1, ''),'') AS PART_CD ";
            sqlComGet.CommandTimeout = 3000;
            sqlDr = sqlComGet.ExecuteReader();
            while (sqlDr.Read())
            {
                lastlaySelectPart += sqlDr["PART_CD"];
            }
            sqlDr.Close();
       
        }
        result += "\"LASTLAYNO_SELECT_PART\": \"" + lastlaySelectPart + "\", ";


        //获取颜色SELECT  '    ' AS COLOR_CD UNION  
        sqlComGet.CommandText = @"SELECT   DISTINCT COLOR_CD   from  dbo.CUT_BUNDLE_HD  WITH(NOLOCK)  WHERE JOB_ORDER_NO IN (SELECT   JO_NO   FROM  dbo.JO_HD  WITH(NOLOCK) WHERE SC_NO='" + gono + "')  AND STATUS='Y'";
        sqlComGet.CommandTimeout = 3000;
        sqlDr = sqlComGet.ExecuteReader();

        string colorcd = "";
        while (sqlDr.Read())
        {
            colorcd += "<option value='" + sqlDr["COLOR_CD"] + "'>" + sqlDr["COLOR_CD"]+ "</option>";
        }
        sqlDr.Close();
        result += "\"COLORCD\": \"" + colorcd + "\", ";

        //纸样SELECT  '    ' AS PATTERN_NO UNION  
        sqlComGet.CommandText = @" SELECT DISTINCT PATTERN_NO  FROM  cut_lay  WITH(NOLOCK)  WHERE JOB_ORDER_NO IN (SELECT   JO_NO   FROM  dbo.JO_HD  WITH(NOLOCK)  WHERE SC_NO='" + gono + "')";
        sqlComGet.CommandTimeout = 3000;
        sqlDr = sqlComGet.ExecuteReader();
        string patternNo = "";
        while (sqlDr.Read())
        {
            patternNo += "<option value='" + sqlDr["PATTERN_NO"] + "'>" + sqlDr["PATTERN_NO"] + "</option>";
        }
        sqlDr.Close();
        result += "\"PATTERNNO\": \"" + patternNo + "\", ";

        //缸号SELECT  '    ' AS BATCH_NO UNION 
        sqlComGet.CommandText = @" SELECT   DISTINCT BATCH_NO   FROM  dbo.CUT_LAY_DT  WITH(NOLOCK)  WHERE LAY_DT_ID IN (
                                                            SELECT   DISTINCT LAY_DT_ID  from  dbo.CUT_BUNDLE_HD WHERE JOB_ORDER_NO IN (SELECT   JO_NO   FROM  dbo.JO_HD WHERE SC_NO='" + gono + "') AND STATUS='Y' )";
        sqlComGet.CommandTimeout = 3000;
        sqlDr = sqlComGet.ExecuteReader();
        string batchNo = "";
        while (sqlDr.Read())
        {
            batchNo += "<option value='" + sqlDr["BATCH_NO"] + "'>" + sqlDr["BATCH_NO"] + "</option>";
        }
        sqlDr.Close();
        result += "\"BATCHNO\": \"" + batchNo + "\", ";



        //色级SELECT  '    ' AS SHADE_LOT UNION  
        sqlComGet.CommandText = @"SELECT  DISTINCT SHADE_LOT  FROM  dbo.CUT_LAY_DT WHERE LAY_DT_ID IN (
                                                                SELECT   DISTINCT LAY_DT_ID  from  dbo.CUT_BUNDLE_HD WHERE JOB_ORDER_NO IN (SELECT   JO_NO   FROM  dbo.JO_HD WHERE SC_NO='" + gono +"') AND STATUS='Y' )";
        sqlComGet.CommandTimeout = 3000;
        sqlDr = sqlComGet.ExecuteReader();
        string shadeLot = "";
        while (sqlDr.Read())
        {
            shadeLot += "<option value='" + sqlDr["SHADE_LOT"] + "'>" + sqlDr["SHADE_LOT"] + "</option>";
        }
        sqlDr.Close();
        result += "\"SHADELOT\": \"" + shadeLot + "\", ";

        //SIZE
        sqlComGet.CommandText = @"SELECT   DISTINCT SIZE_CD+'('+SIZE_CD2+')' AS SIZE_CD, B.SEQUENCE   FROM  CUT_LAY_HD A  WITH(NOLOCK)  
                                                        LEFT JOIN dbo.SC_SIZE B  WITH(NOLOCK) ON B.SIZE_CODE=A.SIZE_CD   AND B.SC_NO='"+gono+@"'
                                                        WHERE LAY_TRANS_ID IN (
                                                          SELECT   DISTINCT  LAY_TRANS_ID  from  dbo.CUT_BUNDLE_HD  WITH(NOLOCK)   
                                                           WHERE JOB_ORDER_NO IN (SELECT   JO_NO   FROM  dbo.JO_HD  WITH(NOLOCK) WHERE SC_NO='" + gono + "') AND STATUS='Y') ORDER BY B.SEQUENCE";
        sqlComGet.CommandTimeout = 3000;
        sqlDr = sqlComGet.ExecuteReader();
        string size = "";
        while (sqlDr.Read())
        {
            size += "<tr><td style='width:30%;text-align:right'><label  for='" + sqlDr["SIZE_CD"] + "'  name='sizelabel'>" + sqlDr["SIZE_CD"] + "</label></td><td style='width:70%'><input type='text'  name='SIZE_NUM'   id='" + sqlDr["SIZE_CD"] + "' /></td></tr>";
        }
        sqlDr.Close();
        result += "\"SIZE\": \"" + size + "\", ";




        //查找该Go的所有part
        sqlComGet.CommandText = @"SELECT D.PART_CD,D.PART_DESC FROM PRD_GO_ROUTE_LIST_HD AS A WITH (NOLOCK)
                                                            INNER JOIN PRD_GO_ROUTE_LIST_DT AS B WITH (NOLOCK) ON A.ROUTE_HD_ID = B.ROUTE_HD_ID 
                                                            INNER JOIN GEN_PART_MASTER AS C ON A.FACTORY_CD = C.FACTORY_CD AND B.PART_CD = C.PART_CD 
                                                            INNER JOIN CIPMS_PART_MASTER AS D ON C.PART_DESC=D.PART_DESC
                                                            WHERE A.GO_NO='"+ gono +"' GROUP BY D.PART_CD,D.PART_DESC,D.SEQ_NO ORDER BY D.SEQ_NO";
        sqlDr = sqlComGet.ExecuteReader();
        string part = "";
        int i = 0;
        while (sqlDr.Read())
        {
            i++;
            part += "<input type='checkbox' name='part'  id='" + i.ToString() + "' value='" + sqlDr["PART_CD"] + "'/><label for='" + i.ToString() + "' name='part' value='" + sqlDr["PART_CD"] + "'>" + sqlDr["PART_DESC"] + "</label>";
        }
        sqlDr.Close();
        result += "\"PART\": \"" + part + "\"";

        result = "[{" + result + "}]";
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static string GetGoFirstLayInfo(string factory, string svTYPE, string gono, string layno)
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

        string process = "Y";
        result += "\"PROCESS\": \"" + process + "\", ";

        //最大的床号
        sqlComGet.CommandText = @"SELECT   ISNULL(MAX(LAY_NO),0)+1  AS MAX_LAY   from  CIPMS_SPARE_CODE_PRINT  WITH(NOLOCK)  WHERE SC_NO='" + gono + "' ";
        sqlComGet.CommandTimeout = 3000;
        sqlDr = sqlComGet.ExecuteReader();
        string maxlayno = "";
        while (sqlDr.Read())
        {
            maxlayno += sqlDr["MAX_LAY"];
        }
        sqlDr.Close();
        result += "\"MAXLAYNO\": \"" + maxlayno + "\", ";

        var isfirtlay = "N";
        if (layno == "1")
        {
            isfirtlay = "Y";
        }
        result += "\"ISFIRSTLAY\": \"" + isfirtlay + "\", ";

        //获取第一床选的部位
        string lastlaySelectPart = "";
        if (maxlayno != "" || maxlayno != "1")
        {
            sqlComGet.CommandText = @"SELECT  ISNULL(STUFF((SELECT  DISTINCT '/'+  PART_CD FROM CIPMS_SPARE_CODE_PRINT  WHERE  SC_NO='" + gono + "' and  LAY_NO='1'    FOR XML PATH('')), 1, 1, ''),'') AS PART_CD ";
            sqlComGet.CommandTimeout = 3000;
            sqlDr = sqlComGet.ExecuteReader();
            while (sqlDr.Read())
            {
                lastlaySelectPart += sqlDr["PART_CD"];
            }
            sqlDr.Close();

        }
        result += "\"LASTLAYNO_SELECT_PART\": \"" + lastlaySelectPart + "\", ";


        //获取颜色,纸样,缸号,色级
        sqlComGet.CommandText = @"SELECT   DISTINCT COLOR_CODE,PATTERN_NO,BATCH_NO,SHADE_LOT   from   CIPMS_SPARE_CODE_PRINT  WITH(NOLOCK)  WHERE  SC_NO='" + gono + "' and  LAY_NO='" + layno + "' ";
        sqlComGet.CommandTimeout = 3000;
        sqlDr = sqlComGet.ExecuteReader();

        string colorcd = "";
        string patternNo = "";
        string batchNo = "";
        string shadeLot = "";
        while (sqlDr.Read())
        {
            colorcd += sqlDr["COLOR_CODE"];
            patternNo += sqlDr["PATTERN_NO"];
            batchNo +=sqlDr["BATCH_NO"];
            shadeLot += sqlDr["SHADE_LOT"];
        }
        sqlDr.Close();
        result += "\"COLORCD\": \"" + colorcd + "\", ";
        result += "\"PATTERNNO\": \"" + patternNo + "\", ";
        result += "\"BATCHNO\": \"" + batchNo + "\", ";
        result += "\"SHADELOT\": \"" + shadeLot + "\", ";

        //SIZE
        sqlComGet.CommandText = @"SELECT  DISTINCT SIZE_CD+'/'+CAST(SIZE_QTY AS NVARCHAR(10))  AS SIZE_CD  from   CIPMS_SPARE_CODE_PRINT  WITH(NOLOCK)  WHERE  sC_NO='"+gono+"'  and LAY_NO='"+layno+"'";
        sqlComGet.CommandTimeout = 3000;
        sqlDr = sqlComGet.ExecuteReader();
        string size = "";
        while (sqlDr.Read())
        {
            size +=  sqlDr["SIZE_CD"] +"$";
        }
        sqlDr.Close();
        result += "\"SIZE\": \"" + size.TrimEnd('$') + "\", ";

        result = "[{" + result + "}]";
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String GonoColorInfo(string factory, string svTYPE, string gono, string colorcd)
    {
        string result = "";
        string[] colorcdlist = colorcd.Split(new char[] { ',' });

        string colorlist1=",";
        foreach (string color in colorcdlist)
        {
            if (colorlist1 == ",")
            {
                colorlist1 = "'"+color+"',";
            }
            else
            {
                colorlist1 = colorlist1 + "'" + color+"',";
            }
        }
       colorlist1= colorlist1.TrimEnd(',');
        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        sqlCon.Open();

        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr;
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlCon;


        //纸样
        if (colorlist1 == "")
        {
            sqlComGet.CommandText = @"SELECT DISTINCT PATTERN_NO  FROM  cut_lay  WITH(NOLOCK)  WHERE JOB_ORDER_NO IN (SELECT   JO_NO   FROM  dbo.JO_HD  WITH(NOLOCK)  WHERE SC_NO='" + gono + "')";
        }else
        {
            sqlComGet.CommandText = @"  SELECT DISTINCT A.PATTERN_NO FROM  cut_lay A  WITH(NOLOCK) 
                                                                INNER JOIN CUT_LAY_HD HD WITH(NOLOCK) ON HD.LAY_ID = A.LAY_ID
                                                                INNER JOIN dbo.CUT_LAY_DT DT  WITH(NOLOCK)  ON DT.LAY_TRANS_ID = HD.LAY_TRANS_ID
                                                                WHERE A.JOB_ORDER_NO IN (SELECT   JO_NO   FROM  dbo.JO_HD  WITH(NOLOCK)  WHERE SC_NO='"+gono+"') AND DT.COLOR_CD IN ("+colorlist1+")";

        }

        sqlComGet.CommandTimeout = 3000;
        sqlDr = sqlComGet.ExecuteReader();
        string patternNo = "";
        while (sqlDr.Read())
        {
            patternNo += "<option value='" + sqlDr["PATTERN_NO"] + "'>" + sqlDr["PATTERN_NO"] + "</option>";
        }
        sqlDr.Close();
        result += "\"PATTERNNO\": \"" + patternNo + "\", ";


        //缸号
        if (colorlist1 == "")
        {
            sqlComGet.CommandText = @"SELECT   DISTINCT BATCH_NO   FROM  dbo.CUT_LAY_DT  WITH(NOLOCK)  WHERE LAY_DT_ID IN (
                                                            SELECT   DISTINCT LAY_DT_ID  from  dbo.CUT_BUNDLE_HD WHERE JOB_ORDER_NO IN (SELECT   JO_NO   FROM  dbo.JO_HD WHERE SC_NO='" +gono + "') AND STATUS='Y' )";
        }
        else
        {
            sqlComGet.CommandText = @"SELECT   DISTINCT BATCH_NO   FROM  dbo.CUT_LAY_DT  WITH(NOLOCK)  WHERE LAY_DT_ID IN (
                                                            SELECT   DISTINCT LAY_DT_ID  from  dbo.CUT_BUNDLE_HD WHERE JOB_ORDER_NO IN (SELECT   JO_NO   FROM  dbo.JO_HD WHERE SC_NO='" + gono + "') AND STATUS='Y' )  AND COLOR_CD IN (" + colorlist1 + ")";
        }
        sqlComGet.CommandTimeout = 3000;
        sqlDr = sqlComGet.ExecuteReader();
        string batchNo = "";
        while (sqlDr.Read())
        {
            batchNo += "<option value='" + sqlDr["BATCH_NO"] + "'>" + sqlDr["BATCH_NO"] + "</option>";
        }
        sqlDr.Close();
        result += "\"BATCHNO\": \"" + batchNo + "\", ";



        //色级
        if (colorlist1 == "")
        {
            sqlComGet.CommandText =
                @"SELECT  DISTINCT SHADE_LOT  FROM  dbo.CUT_LAY_DT WHERE LAY_DT_ID IN (
                                                                SELECT   DISTINCT LAY_DT_ID  from  dbo.CUT_BUNDLE_HD WHERE JOB_ORDER_NO IN (SELECT   JO_NO   FROM  dbo.JO_HD WHERE SC_NO='" +
                gono + "') AND STATUS='Y' )";
        }
        else
        {
            sqlComGet.CommandText = @"SELECT  DISTINCT SHADE_LOT  FROM  dbo.CUT_LAY_DT WHERE LAY_DT_ID IN (
                                                                SELECT   DISTINCT LAY_DT_ID  from  dbo.CUT_BUNDLE_HD WHERE JOB_ORDER_NO IN (SELECT   JO_NO   FROM  dbo.JO_HD WHERE SC_NO='" +
                                                    gono + "') AND STATUS='Y' )  AND COLOR_CD IN (" + colorlist1 + ")";
        }

        sqlComGet.CommandTimeout = 3000;
        sqlDr = sqlComGet.ExecuteReader();
        string shadeLot = "";
        while (sqlDr.Read())
        {
            batchNo += "<option value='" + sqlDr["SHADE_LOT"] + "'>" + sqlDr["SHADE_LOT"] + "</option>";
        }
        sqlDr.Close();
        result += "\"SHADELOT\": \"" + shadeLot + "\"";
        result = "[{" + result + "}]";
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Print(string process, string userbarcode, string factory, string svTYPE, string gono, string layno,string colorcd, string part, string sizenum, string patternno, string batchno, string shadelot,string flag)
    {
        //处理数据

        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        SqlDataReader sqlDr;
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlCon;

     
        string result = "";
        sqlComGet.CommandText = @" EXEC CIPMS_PRINT_SPRATE_CODE  '" + factory + "','" + gono + "' ,'" + layno + "','" + colorcd + "' ,'" + part + "' ,'" + sizenum + "' ,'" + patternno + "' ,'" + batchno + "' ,'" + shadelot + "','" + userbarcode + "','"+flag+"'";
        sqlComGet.CommandTimeout = 3000;
        sqlDr = sqlComGet.ExecuteReader();
        List<BUNDLE_INFO> bundleinfolist = new List<BUNDLE_INFO>();
        while (sqlDr.Read())
        {

            BUNDLE_INFO onebundleinfo = new BUNDLE_INFO();
            onebundleinfo.CLIENT = sqlDr["CUSTOMER_NAME"].ToString();
            if (onebundleinfo.CLIENT.Length > 9) { onebundleinfo.CLIENT = onebundleinfo.CLIENT.Substring(0, 9); }


            onebundleinfo.STYLE_NO = sqlDr["STYLE_NO"].ToString();
            if (onebundleinfo.STYLE_NO.Length > 10){ onebundleinfo.STYLE_NO = onebundleinfo.STYLE_NO.Substring(0, 10); }
           
            onebundleinfo.JOB_ORDER_NO = sqlDr["SC_NO"].ToString();
            if (onebundleinfo.JOB_ORDER_NO.Length > 10) { onebundleinfo.JOB_ORDER_NO = onebundleinfo.JOB_ORDER_NO.Substring(0, 10); }

            onebundleinfo.QTY = sqlDr["SIZE_QTY"].ToString();
            if (onebundleinfo.QTY.Length > 10) { onebundleinfo.QTY = onebundleinfo.QTY.Substring(0, 10); }

            onebundleinfo.COLOR = sqlDr["COLOR_CODE"].ToString();
            if (onebundleinfo.COLOR.Length > 10) { onebundleinfo.COLOR = onebundleinfo.COLOR.Substring(0, 10); }

            onebundleinfo.BUNDLE_NO = sqlDr["BUNDLE_NO"].ToString();
            onebundleinfo.LAY_NO = sqlDr["LAY_NO"].ToString();

            onebundleinfo.SIZE = sqlDr["SIZE_CD"].ToString();
            if (onebundleinfo.SIZE.Length > 10) { onebundleinfo.SIZE = onebundleinfo.SIZE.Substring(0, 10); }

            onebundleinfo.CUT_QTY = sqlDr["COLOR_QTY"].ToString();
            if (onebundleinfo.CUT_QTY.Length > 10) { onebundleinfo.CUT_QTY = onebundleinfo.CUT_QTY.Substring(0, 10); }

            onebundleinfo.PRODUCTION_LINE = sqlDr["PRODUCT_LINE"].ToString();
            if (onebundleinfo.PRODUCTION_LINE.Length > 24) { onebundleinfo.PRODUCTION_LINE = onebundleinfo.PRODUCTION_LINE.Substring(0, 24); }

            onebundleinfo.PART = sqlDr["PART_DESC"].ToString();
            if (onebundleinfo.PART.Length > 10) { onebundleinfo.PART = onebundleinfo.PART.Substring(0, 10); }

            onebundleinfo.PATTERN_NO = sqlDr["PATTERN_NO"].ToString() == "null" ? "" : sqlDr["PATTERN_NO"].ToString();
            if (onebundleinfo.PATTERN_NO.Length > 8) { onebundleinfo.PATTERN_NO = onebundleinfo.PATTERN_NO.Substring(0, 8); }

            onebundleinfo.SHADE_LOT = sqlDr["SHADE_LOT"].ToString() == "null" ? "" : sqlDr["SHADE_LOT"].ToString();
            if (onebundleinfo.SHADE_LOT.Length > 8) { onebundleinfo.SHADE_LOT = onebundleinfo.SHADE_LOT.Substring(0, 8); }

            onebundleinfo.BATCH_NO = sqlDr["BATCH_NO"].ToString() == "null" ? "" : sqlDr["BATCH_NO"].ToString();
            if (onebundleinfo.BATCH_NO.Length > 8) { onebundleinfo.BATCH_NO = onebundleinfo.BATCH_NO.Substring(0, 8); }

            onebundleinfo.DATE = DateTime.Now.ToString(); 
            onebundleinfo.PRINTPART_FLAG = "";
            onebundleinfo.MARKERS = "";
            bundleinfolist.Add(onebundleinfo);

        }
        sqlDr.Close();
        sqlCon.Close();

        result = " {\"BUNDLEINFO\": " +  JsonConvert.SerializeObject(bundleinfolist) + " }";
        return result;
    }
    
    
 
}