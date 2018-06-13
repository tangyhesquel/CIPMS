using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using NameSpace;
using System.Data;
using Model;

/// <summary>
///COMMONDAL 的摘要说明
/// </summary>
public class COMMONDAL
{
    public SqlConnection sqlCon { get; set; }
    public Connect connectstring { get; set; }
    public SqlCommand sqlComGet { get; set; }

    public COMMONDAL(string factory, string svTYPE)
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
        sqlCon = new SqlConnection();
        connectstring = new Connect();
        sqlComGet = new SqlCommand();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
	}

    //获取部门列表
    public SqlDataReader GetProcessList(string factory, string garmenttype)
    {
        try
        {
            sqlComGet.Connection = sqlCon;
            sqlComGet.CommandText = "SELECT PRC_CD,NM,CIPMS_CHS FROM GEN_PRC_CD_MST WHERE FACTORY_CD='" + factory + "'and GARMENT_TYPE='" + garmenttype + "' and DISPLAY_SEQ < 40 AND CIPMS_FLAG='Y' GROUP BY PRC_CD,NM,DISPLAY_SEQ,CIPMS_CHS ORDER BY DISPLAY_SEQ";
            return sqlComGet.ExecuteReader();
        }
        catch (Exception ex)
        {
        }
        finally
        {
        }
        return null;
    }

    //根据制单列表
    public List<JOBORDERNO> GetJO(string factory)
    {
        try
        {
            sqlComGet.Connection = sqlCon;
            sqlComGet.CommandText = "SELECT JOB_ORDER_NO FROM CIPMS_BUNDLE_FOR_SCANNING WITH(NOLOCK) WHERE FACTORY_CD='" + factory + "' GROUP BY JOB_ORDER_NO";
            SqlDataReader sqlDr = sqlComGet.ExecuteReader();
            List<JOBORDERNO> jolist = new List<JOBORDERNO>();
            while (sqlDr.Read())
            {
                JOBORDERNO jo = new JOBORDERNO();
                jo.JOB_ORDER_NO = sqlDr["JOB_ORDER_NO"].ToString();
                jolist.Add(jo);
            }
            sqlDr.Close();

            return jolist;
        }
        catch (Exception ex)
        {
        }
        finally
        {
            sqlCon.Close();
        }
        return null;
    }

    //获取用户信息
    public DataSet GetUserProfile(string userbarcode)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("USP_CIPMS_USERPROFILE_GET", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
            cmd.Parameters["@USERBARCODE"].Value = userbarcode;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            return ds;
        }
        catch (Exception ex)
        {
        }
        return null;
    }

    //获取箱子所在部门
    public DataSet GetCartonInformation(string BARCODE) 
    {
        try
        {
            SqlCommand cmd = new SqlCommand("USP_CIPMS_CARTONINFORMATION_GET", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@BARCODE", SqlDbType.NVarChar);
            cmd.Parameters["@BARCODE"].Value = BARCODE;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            return ds;
        }
        catch (Exception ex)
        {
        }
        finally
        {
        }
        return null;
    }


    //获取箱码打印
    public string PrintCartonBarcode(string cartonbarcode, CartonBarcode_Print cartonbarcodecolumn)
    {
        //string[] translation = languagearray.Split(new char[] { '*' });
        try
        {
            SqlDataReader sqlDr;
            Packagesql packagesql = new Packagesql();
            Sqlstatement sqlstatement = new Sqlstatement();

            string process = "";
            string cut_line = "";
            string sew_line = "";
            //查找箱码所在的process
            sqlDr = sqlstatement.getcartonprocprod(sqlCon, cartonbarcode);
            if (sqlDr.Read())
            {
                process = sqlDr["PROCESS_CD"].ToString();
            }
            sqlDr.Close();
            //获取箱码的裁床组别
            sqlDr = sqlstatement.getcutline(sqlCon, cartonbarcode);
            int f = 0;
            while (sqlDr.Read())
            {
                if (f == 0)
                    cut_line += sqlDr["CUT_LINE"].ToString();
                else
                    cut_line += "/" + sqlDr["CUT_LINE"].ToString();
                f++;
            }
            sqlDr.Close();
            //获取箱码的车缝组别
            sqlDr = sqlstatement.getsewline(sqlCon, cartonbarcode);
            f = 0;
            while (sqlDr.Read())
            {
                if (f == 0)
                    sew_line += sqlDr["PRODUCTION_LINE_CD"].ToString();
                else
                    sew_line += "/" + sqlDr["PRODUCTION_LINE_CD"].ToString();
                f++;
            }
            sqlDr.Close();

            //查找GO、客户
            string customer = "";
            string gono = "";
            sqlDr = packagesql.getgocus(sqlCon, cartonbarcode);
            if (sqlDr.Read())
            {
                customer = sqlDr["SHORT_NAME"].ToString();
                gono = sqlDr["GARMENT_ORDER_NO"].ToString();
            }
            sqlDr.Close();

            //获取日期
            string date = DateTime.Now.ToString();

            //查询箱内裁片的部位
            sqlDr = packagesql.getpackagepart(sqlCon, cartonbarcode);
            f = 0;
            string parts = "";
            while (sqlDr.Read())
            {
                if (f == 0)
                    parts += sqlDr["PART_DESC"].ToString();
                else
                    parts += "/" + sqlDr["PART_DESC"].ToString();
                f++;
            }
            sqlDr.Close();
            //获取箱外的部件
            sqlDr = packagesql.getshortpackagepart(sqlCon, cartonbarcode);
            f = 0;
            string shortparts = "";
            while (sqlDr.Read())
            {
                if (f == 0)
                    shortparts += sqlDr["PART_DESC"].ToString();
                else
                    shortparts += "/" + sqlDr["PART_DESC"].ToString();
                f++;
            }
            sqlDr.Close();

            //总扎数
            sqlDr = sqlstatement.getttlbundle(sqlCon, cartonbarcode);
            int ttlbundle = 0;
            while (sqlDr.Read())
            {
                ttlbundle++;
            }
            sqlDr.Close();

            //获取该GO箱内尺码的数量
            sqlDr = packagesql.getsize(sqlCon, cartonbarcode);
            List<string> size = new List<string>();
            string sql = "";//拼写查询字符串
            while (sqlDr.Read())
            {
                size.Add(sqlDr["SIZE_CD"].ToString());
                sql += ",SUM(CASE SIZE_CD WHEN '" + sqlDr["SIZE_CD"].ToString() + "' THEN QTY ELSE 0 END)AS '" + sqlDr["SIZE_CD"].ToString() + "'";
            }
            sqlDr.Close();

            string table1 = "<table style='font-size:14px;'><tr>";
            table1 += "<td style='width:3.5cm;'><div>GO NO. " + gono + "</div></td>";
            table1 += "<td style='width:3.5cm;'><div>" + cartonbarcodecolumn.customer + " " + customer + "</div></td>";
            table1 += "</tr></table>";
            table1 += "<div id='barcodediv' style='height:150'></div></br>";
            table1 += "<table style='font-size:12px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr>";
            table1 += "<td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + cartonbarcodecolumn.cartonbarcode + "</div></td>";
            table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + cartonbarcode + "</div></td>";
            table1 += "</tr>";
            table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + cartonbarcodecolumn.date + "</div></td>";
            table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + date + "</div></td>";
            table1 += "</tr>";
            table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + cartonbarcodecolumn.process + "</div></td>";
            table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + process + "</div></td>";
            table1 += "</tr>";
            table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + cartonbarcodecolumn.production + "</div></td>";
            table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + cut_line + "</div></td>";
            table1 += "</tr>";
            table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + cartonbarcodecolumn.sewline + "</div></td>";
            table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + sew_line + "</div></td>";
            table1 += "</tr>";
            table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + cartonbarcodecolumn.part + "</div></td>";
            table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + parts + "</div></td>";
            table1 += "</tr>";
            table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + cartonbarcodecolumn.shortpart + "</div></td>";
            table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + shortparts + "</div></td>";
            table1 += "</tr>";
            table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + cartonbarcodecolumn.ttlbundle + "</div></td>";
            table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + ttlbundle.ToString() + "</div></td>";
            table1 += "</tr></table><br />";
            table1 += "<div>" + cartonbarcodecolumn.summary + "</div>";

            //得到结果集
            SqlDataAdapter sda = new SqlDataAdapter(packagesql.getjocolordb(sqlCon, sql, cartonbarcode));
            DataTable dt = new DataTable();
            sda.Fill(dt);

            //循环结果集，构造箱码报表
            string colorrecord = "";
            string result = "";
            string ttl = "0";
            if (size.Count < 2)
            {
                string table2 = "";
                table2 += "<table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + cartonbarcodecolumn.color + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + cartonbarcodecolumn.cutqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + cartonbarcodecolumn.layno + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + cartonbarcodecolumn.bundle + "</div></td>";
                foreach (string n in size)
                    table2 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + n + "</div></td>";
                table2 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + cartonbarcodecolumn.totalqty + "</div></td><td style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.8cm'>TTL BY GO COLOR</div></td></tr>";
                foreach (DataRow dr in dt.Rows)
                {
                    //插入jo/color/size的qty
                    table2 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + dr[1] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
                    for (int k = 0; k < size.Count; k++)
                        table2 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[5 + k] + "</div></td>";

                    sqlDr = packagesql.gettotalqty(sqlCon, dr[0].ToString(), dr[1].ToString(), cartonbarcode);
                    string totalqty = "";
                    if (sqlDr.Read())
                    {
                        totalqty = sqlDr["TOTAL_QTY"].ToString();
                    }
                    sqlDr.Close();

                    table2 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + totalqty + "</div></td>";
                    if (colorrecord != dr[1].ToString())
                    {
                        colorrecord = dr[1].ToString();
                        //获取该color的行数
                        string colornum = "";
                        sqlDr = packagesql.getcolornum(sqlCon, dr[1].ToString(), cartonbarcode);
                        if (sqlDr.Read())
                            colornum = sqlDr["QTY"].ToString();
                        sqlDr.Close();
                        ttl = "0";
                        sqlDr = packagesql.getttlbygocolor(sqlCon, dr[1].ToString(), cartonbarcode);
                        if (sqlDr.Read())
                            ttl = sqlDr["QTY"].ToString();
                        sqlDr.Close();
                        table2 += "<td rowspan='" + colornum + "' style='width:0.8cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
                    }
                    else
                        table2 += "</tr>";
                }
                table2 += "</table>";
                result = table1 + table2;
            }
            else if (size.Count >= 2 && size.Count <= 8)
            {
                //构建表头
                string table2 = "", table3 = "";
                table2 += "<table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + cartonbarcodecolumn.color + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + cartonbarcodecolumn.cutqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + cartonbarcodecolumn.layno + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + cartonbarcodecolumn.bundle + "</div></td>";
                for (int m = 0; m < size.Count && m < 3; m++)
                    table2 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table2 += "</tr>";
                table3 += "<br /><table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + cartonbarcodecolumn.color + "</div></td>";
                for (int m = 3; m < size.Count; m++)
                    table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + cartonbarcodecolumn.totalqty + "</div></td><td style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.8cm'>TTL BY GO COLOR</div></td></tr>";
                foreach (DataRow dr in dt.Rows)
                {
                    //插入jo/color/size的qty
                    table2 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + dr[1] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
                    for (int m = 0; m < size.Count && m < 3; m++)
                        table2 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                    table2 += "</tr>";

                    table3 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + dr[1] + "</div></td>";
                    for (int m = 3; m < size.Count; m++)
                        table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";

                    sqlDr = packagesql.gettotalqty(sqlCon, dr[0].ToString(), dr[1].ToString(), cartonbarcode);
                    string totalqty = "";
                    if (sqlDr.Read())
                    {
                        totalqty = sqlDr["TOTAL_QTY"].ToString();
                    }
                    sqlDr.Close();

                    table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + totalqty + "</div></td>";
                    if (colorrecord != dr[1].ToString())
                    {
                        colorrecord = dr[1].ToString();
                        //获取该color的行数
                        string colornum = "";
                        sqlDr = packagesql.getcolornum(sqlCon, dr[1].ToString(), cartonbarcode);
                        if (sqlDr.Read())
                            colornum = sqlDr["QTY"].ToString();
                        sqlDr.Close();
                        ttl = "0";
                        sqlDr = packagesql.getttlbygocolor(sqlCon, dr[1].ToString(), cartonbarcode);
                        if (sqlDr.Read())
                            ttl = sqlDr["QTY"].ToString();
                        sqlDr.Close();
                        table3 += "<td rowspan='" + colornum + "' style='width:0.8cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
                    }
                    else
                        table3 += "</tr>";
                }
                table2 += "</table>";
                table3 += "</table>";
                result = table1 + table2 + table3;
            }
            else if (size.Count >= 9 && size.Count <= 12)
            {
                //构建表头
                string table2 = "", table3 = "", table4 = "";
                table2 += "<table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + cartonbarcodecolumn.color + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + cartonbarcodecolumn.cutqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + cartonbarcodecolumn.layno + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + cartonbarcodecolumn.bundle + "</div></td>";
                for (int m = 0; m < size.Count && m < 3; m++)
                    table2 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table2 += "</tr>";

                table3 += "<br /><table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + cartonbarcodecolumn.color + "</div></td>";
                for (int m = 3; m < size.Count && m < 9; m++)
                    table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table3 += "</tr>";

                table4 += "<br /><table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + cartonbarcodecolumn.color + "</div></td>";
                for (int m = 9; m < size.Count; m++)
                    table4 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table4 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + cartonbarcodecolumn.totalqty + "</div></td><td style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.8cm'>TTL BY GO COLOR</div></td></tr>";
                foreach (DataRow dr in dt.Rows)
                {
                    //插入jo/color/size的qty
                    table2 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + dr[1] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
                    for (int m = 0; m < size.Count && m < 3; m++)
                        table2 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                    table2 += "</tr>";

                    table3 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + dr[1] + "</div></td>";
                    for (int m = 3; m < size.Count && m < 9; m++)
                        table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                    table3 += "</tr>";

                    table4 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + dr[1] + "</div></td>";
                    for (int m = 9; m < size.Count; m++)
                        table4 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";

                    sqlDr = packagesql.gettotalqty(sqlCon, dr[0].ToString(), dr[1].ToString(), cartonbarcode);
                    string totalqty = "";
                    if (sqlDr.Read())
                    {
                        totalqty = sqlDr["TOTAL_QTY"].ToString();
                    }
                    sqlDr.Close();

                    table4 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + totalqty + "</div></td>";

                    if (colorrecord != dr[1].ToString())
                    {
                        colorrecord = dr[1].ToString();
                        //获取该color的行数
                        string colornum = "";
                        sqlDr = packagesql.getcolornum(sqlCon, dr[1].ToString(), cartonbarcode);
                        if (sqlDr.Read())
                            colornum = sqlDr["QTY"].ToString();
                        sqlDr.Close();
                        ttl = "0";
                        sqlDr = packagesql.getttlbygocolor(sqlCon, dr[1].ToString(), cartonbarcode);
                        if (sqlDr.Read())
                            ttl = sqlDr["QTY"].ToString();
                        sqlDr.Close();
                        table4 += "<td rowspan='" + colornum + "' style='width:0.8cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
                    }
                    else
                        table4 += "</tr>";
                }
                table2 += "</table>";
                table3 += "</table>";
                table4 += "</table>";
                result = table1 + table2 + table3 + table4;
            }
            else if (size.Count >= 13)
            {
                //构建表头
                string table2 = "", table3 = "", table4 = "", table5 = "";
                table2 += "<table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + cartonbarcodecolumn.color + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + cartonbarcodecolumn.cutqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + cartonbarcodecolumn.layno + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + cartonbarcodecolumn.bundle + "</div></td>";
                for (int m = 0; m < size.Count && m < 3; m++)
                    table2 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table2 += "</tr>";

                table3 += "<br /><table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + cartonbarcodecolumn.color + "</div></td>";
                for (int m = 3; m < size.Count && m < 9; m++)
                    table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table3 += "</tr>";

                table4 += "<br /><table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + cartonbarcodecolumn.color + "</div></td>";
                for (int m = 9; m < size.Count && m < 13; m++)
                    table4 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table4 += "</tr>";

                table5 += "<br /><table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + cartonbarcodecolumn.color + "</div></td>";
                for (int m = 13; m < size.Count; m++)
                    table5 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table5 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + cartonbarcodecolumn.totalqty + "</div></td><td style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.8cm'>TTL BY GO COLOR</div></td></tr>";
                foreach (DataRow dr in dt.Rows)
                {
                    //插入jo/color/size的qty
                    table2 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + dr[1] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
                    for (int m = 0; m < size.Count && m < 3; m++)
                        table2 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                    table2 += "</tr>";

                    table3 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + dr[1] + "</div></td>";
                    for (int m = 3; m < size.Count && m < 9; m++)
                        table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                    table3 += "</tr>";

                    table4 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + dr[1] + "</div></td>";
                    for (int m = 9; m < size.Count && m < 13; m++)
                        table4 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                    table4 += "</tr>";

                    table5 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + dr[1] + "</div></td>";
                    for (int m = 13; m < size.Count; m++)
                        table5 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";

                    sqlDr = packagesql.gettotalqty(sqlCon, dr[0].ToString(), dr[1].ToString(), cartonbarcode);
                    string totalqty = "";
                    if (sqlDr.Read())
                    {
                        totalqty = sqlDr["TOTAL_QTY"].ToString();
                    }
                    sqlDr.Close();

                    table5 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + totalqty + "</div></td>";

                    if (colorrecord != dr[1].ToString())
                    {
                        colorrecord = dr[1].ToString();
                        //获取该color的行数
                        string colornum = "";
                        sqlDr = packagesql.getcolornum(sqlCon, dr[1].ToString(), cartonbarcode);
                        if (sqlDr.Read())
                            colornum = sqlDr["QTY"].ToString();
                        sqlDr.Close();
                        ttl = "0";
                        sqlDr = packagesql.getttlbygocolor(sqlCon, dr[1].ToString(), cartonbarcode);
                        if (sqlDr.Read())
                            ttl = sqlDr["QTY"].ToString();
                        sqlDr.Close();
                        table5 += "<td rowspan='" + colornum + "' style='width:0.8cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
                    }
                    else
                        table5 += "</tr>";
                }
                table2 += "</table>";
                table3 += "</table>";
                table4 += "</table>";
                table5 += "</table>";
                result = table1 + table2 + table3 + table4 + table5;
            }
            return result;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
        finally
        {
            sqlCon.Close();
            sqlCon.Dispose();
        }
    }

    //获取流水单打印
    public string PrintDocno(string doc_no, Docno_Print docnocolumn)
    {
        try
        {
            SqlDataReader sqlDr;
            Sqlstatement sqlstatement = new Sqlstatement();
            Transactionsql transfersql = new Transactionsql();
            Packagesql packagesql = new Packagesql();

            string factory = "", process = "", nextprocess = "", productionline = "", nextproductionline = "", username = "", createdate = "";
            sqlDr = transfersql.getdocnoinformation(sqlCon, doc_no);
            if (sqlDr.Read())
            {
                factory = sqlDr["FACTORY_CD"].ToString();
                process = sqlDr["PROCESS_CD"].ToString();
                nextprocess = sqlDr["NEXT_PROCESS_CD"].ToString();
                productionline = sqlDr["PRODUCTION_LINE_CD"].ToString();
                nextproductionline = sqlDr["NEXT_PRODUCTION_LINE_CD"].ToString();
                username = sqlDr["NAME"].ToString();
                createdate = sqlDr["CREATE_DATE"].ToString();
            }
            sqlDr.Close();

            string result = "";
            //查询该流水单具有的部件
            sqlDr = sqlstatement.getdocnopart(sqlCon, doc_no);
            int i = 0;
            string parts = "";
            while (sqlDr.Read())
            {
                if (i == 0)
                    parts += sqlDr["PART_DESC"].ToString();
                else
                    parts += "/" + sqlDr["PART_DESC"].ToString();
                i++;
            }
            sqlDr.Close();

            int cartonnum = 0;
            sqlDr = sqlstatement.getcartonnum(sqlCon, doc_no);
            while (sqlDr.Read())
            {
                cartonnum++;
            }
            sqlDr.Close();

            //流水单第一个table3
            string table3 = "";
            table3 += "<div style='font-size:14px;width:7.0cm'>(" + factory + ")Garment Transfer Note-[" + process + "->" + nextprocess + "] R</div><br />";
            table3 += "<div style='font-size:14px;width:7.0cm'>" + docnocolumn.doc + factory + doc_no + docnocolumn.user + username + "</div>";
            table3 += "<div id='barcodediv' style='height:150'></div></br>";
            table3 += "<table style='font-size:13px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr>";
            table3 += "<td style='width:1.2cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.datetime + "</div></td>";
            table3 += "<td style='width:3.3cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + createdate + "</div></td>";
            table3 += "<td style='width:1.6cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.fromdept + "</div></td>";
            table3 += "<td style='width:0.9cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + process + "</div></td>";
            table3 += "</tr></table>";
            table3 += "<table style='font-size:13px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr>";
            table3 += "<td style='width:1.4cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.fromline + "</div></td>";
            table3 += "<td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + productionline + "</div></td>";
            table3 += "<td rowspan='5' style='width:1.1cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.remark + "</div></td>";
            table3 += "<td rowspan='5' style='width:1.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div></div></td></tr>";
            table3 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.toline + "</div></td>";
            table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + nextproductionline + "</div></td></tr>";
            table3 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.cartonnum + "</div></td>";
            table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + cartonnum + "</div></td></tr>";
            table3 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.parts + "</div></td>";
            table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + parts + "</div></td></tr>";
            table3 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.automatching + "</div></td>";
            table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + "" + "</div></td></tr></table><br />";

            //查找流水单有多少个GO
            List<string> golist = new List<string>();
            sqlDr = transfersql.gettransfergo(sqlCon, doc_no);
            while (sqlDr.Read())
            {
                golist.Add(sqlDr["GARMENT_ORDER_NO"].ToString());
            }
            sqlDr.Close();

            result = table3;

            //遍历所有GO，table2,table3
            foreach (string n in golist)
            {
                string customer = "";
                sqlDr = transfersql.getcustomer(sqlCon, n);
                if (sqlDr.Read())
                    customer = sqlDr["SHORT_NAME"].ToString();
                sqlDr.Close();

                result += "<table style='font-size:14px;'><tr><td style='width:3.5cm;'><div>GO NO." + n + "</div></td><td style='width:3.5cm;'><div>" + docnocolumn.customer + customer + "</div></td></tr></table><br />";

                //JO/COLOR/SIZE的table
                //获取该GO箱内尺码的数量
                sqlDr = transfersql.getsize(sqlCon, doc_no, n);
                List<string> size = new List<string>();
                string sql = "";//拼写查询字符串
                while (sqlDr.Read())
                {
                    size.Add(sqlDr["SIZE_CD"].ToString());
                    sql += ",SUM(CASE SIZE_CD WHEN '" + sqlDr["SIZE_CD"].ToString() + "' THEN QTY ELSE 0 END)AS '" + sqlDr["SIZE_CD"].ToString() + "'";
                }
                sqlDr.Close();

                //得到结果集
                SqlDataAdapter sda = new SqlDataAdapter(transfersql.getjocolordb(sqlCon, sql, doc_no));
                DataTable dt = new DataTable();
                sda.Fill(dt);

                //循环结果集，构造箱码报表
                string colorrecord = "";
                string ttl = "0";
                if (size.Count < 2)
                {
                    string table12 = "";
                    table12 += "<div>" + docnocolumn.summary + "</div>";
                    table12 += "<table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + docnocolumn.color + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + docnocolumn.layno + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + docnocolumn.bundle + "</div></td>";
                    foreach (string m in size)
                        table12 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + m + "</div></td>";
                    table12 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + docnocolumn.totalqty + "</div></td><td style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.8cm'>TTL BY GO COLOR</div></td></tr>";
                    foreach (DataRow dr in dt.Rows)
                    {
                        string colortemp = dr[1].ToString().Length < 7 ? dr[1].ToString() : dr[1].ToString().Substring(0, 7);
                        //插入jo/color/size的qty
                        table12 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + colortemp + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
                        for (int k = 0; k < size.Count; k++)
                            table12 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[5 + k] + "</div></td>";

                        sqlDr = packagesql.gettotalqtydocno(sqlCon, dr[0].ToString(), dr[1].ToString(), doc_no);
                        string totalqty = "";
                        if (sqlDr.Read())
                        {
                            totalqty = sqlDr["TOTAL_QTY"].ToString();
                        }
                        sqlDr.Close();

                        table12 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + totalqty + "</div></td>";
                        if (colorrecord != dr[1].ToString())
                        {
                            colorrecord = dr[1].ToString();
                            //获取该color的行数
                            string colornum = "";
                            sqlDr = transfersql.getcolornum(sqlCon, dr[1].ToString(), doc_no, n);
                            if (sqlDr.Read())
                                colornum = sqlDr["QTY"].ToString();
                            sqlDr.Close();
                            ttl = "0";
                            sqlDr = transfersql.getttlbygocolor(sqlCon, dr[1].ToString(), doc_no);
                            if (sqlDr.Read())
                                ttl = sqlDr["QTY"].ToString();
                            sqlDr.Close();
                            table12 += "<td rowspan='" + colornum + "' style='width:0.8cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
                        }
                        else
                            table12 += "</tr>";
                    }
                    table12 += "</table>";
                    result += table12 + "</table>";
                }
                else if (size.Count >= 2 && size.Count <= 8)
                {
                    //构建表头
                    string table14 = "", table13 = "";
                    table14 += "<div>" + docnocolumn.summary + "</div><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + docnocolumn.color + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + docnocolumn.layno + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + docnocolumn.bundle + "</div></td>";
                    for (int m = 0; m < size.Count && m < 3; m++)
                        table14 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                    table14 += "</tr>";
                    table13 += "<br /><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + docnocolumn.color + "</div></td>";
                    for (int m = 3; m < size.Count; m++)
                        table13 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                    table13 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + docnocolumn.totalqty + "</div></td><td style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.8cm'>TTL BY GO COLOR</div></td></tr>";
                    foreach (DataRow dr in dt.Rows)
                    {
                        string colortemp = dr[1].ToString().Length < 7 ? dr[1].ToString() : dr[1].ToString().Substring(0, 7);
                        //插入jo/color/size的qty
                        table14 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + colortemp + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
                        for (int m = 0; m < size.Count && m < 3; m++)
                            table14 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                        table14 += "</tr>";
                        table13 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + colortemp + "</div></td>";
                        for (int m = 3; m < size.Count; m++)
                            table13 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";

                        sqlDr = packagesql.gettotalqtydocno(sqlCon, dr[0].ToString(), dr[1].ToString(), doc_no);
                        string totalqty = "";
                        if (sqlDr.Read())
                        {
                            totalqty = sqlDr["TOTAL_QTY"].ToString();
                        }
                        sqlDr.Close();

                        table13 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + totalqty + "</div></td>";
                        if (colorrecord != dr[1].ToString())
                        {
                            colorrecord = dr[1].ToString();
                            //获取该color的行数
                            string colornum = "";
                            sqlDr = transfersql.getcolornum(sqlCon, dr[1].ToString(), doc_no, n);
                            if (sqlDr.Read())
                                colornum = sqlDr["QTY"].ToString();
                            sqlDr.Close();
                            ttl = "0";
                            sqlDr = transfersql.getttlbygocolor(sqlCon, dr[1].ToString(), doc_no);
                            if (sqlDr.Read())
                                ttl = sqlDr["QTY"].ToString();
                            sqlDr.Close();
                            table13 += "<td rowspan='" + colornum + "' style='width:0.9cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
                        }
                        else
                            table13 += "</tr>";
                    }
                    table14 += "</table>";
                    table13 += "</table>";
                    result += table14 + table13 + "</table>";
                }
                else if (size.Count >= 9 && size.Count <= 12)
                {
                    //构建表头
                    string table14 = "", table13 = "", table20 = "";
                    table14 += "<div>" + docnocolumn.summary + "</div><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + docnocolumn.color + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + docnocolumn.layno + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + docnocolumn.bundle + "</div></td>";
                    for (int m = 0; m < size.Count && m < 3; m++)
                        table14 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                    table14 += "</tr>";

                    table13 += "<div>" + docnocolumn.summary + "</div><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + docnocolumn.color + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + docnocolumn.layno + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + docnocolumn.bundle + "</div></td>";
                    for (int m = 3; m < size.Count && m < 9; m++)
                        table13 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                    table13 += "</tr>";

                    table20 += "<br /><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + docnocolumn.color + "</div></td>";
                    for (int m = 9; m < size.Count && m < 13; m++)
                        table20 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                    table20 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + docnocolumn.totalqty + "</div></td><td style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.8cm'>TTL BY GO COLOR</div></td></tr>";

                    foreach (DataRow dr in dt.Rows)
                    {
                        string colortemp = dr[1].ToString().Length < 7 ? dr[1].ToString() : dr[1].ToString().Substring(0, 7);
                        //插入jo/color/size的qty
                        table14 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + colortemp + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
                        for (int m = 0; m < size.Count && m < 3; m++)
                            table14 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                        table14 += "</tr>";

                        table13 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + colortemp + "</div></td>";
                        for (int m = 3; m < size.Count && m < 9; m++)
                            table13 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                        table13 += "</tr>";

                        table20 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + colortemp + "</div></td>";
                        for (int m = 9; m < size.Count && m < 13; m++)
                            table20 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";

                        sqlDr = packagesql.gettotalqtydocno(sqlCon, dr[0].ToString(), dr[1].ToString(), doc_no);
                        string totalqty = "";
                        if (sqlDr.Read())
                        {
                            totalqty = sqlDr["TOTAL_QTY"].ToString();
                        }
                        sqlDr.Close();

                        table20 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + totalqty + "</div></td>";
                        if (colorrecord != dr[1].ToString())
                        {
                            colorrecord = dr[1].ToString();
                            //获取该color的行数
                            string colornum = "";
                            sqlDr = transfersql.getcolornum(sqlCon, dr[1].ToString(), doc_no, n);
                            if (sqlDr.Read())
                                colornum = sqlDr["QTY"].ToString();
                            sqlDr.Close();
                            ttl = "0";
                            sqlDr = transfersql.getttlbygocolor(sqlCon, dr[1].ToString(), doc_no);
                            if (sqlDr.Read())
                                ttl = sqlDr["QTY"].ToString();
                            sqlDr.Close();
                            table20 += "<td rowspan='" + colornum + "' style='width:0.9cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
                        }
                        else
                            table20 += "</tr>";
                    }
                    table14 += "</table>";
                    table13 += "</table>";
                    table20 += "</table>";
                    result += table14 + table13 + table20 + "</table>";
                }
                else if (size.Count >= 13)
                {
                    //构建表头
                    string table14 = "", table13 = "", table20 = "", table21 = "";

                    table14 += "<div>" + docnocolumn.summary + "</div><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + docnocolumn.color + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + docnocolumn.layno + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + docnocolumn.bundle + "</div></td>";
                    for (int m = 0; m < size.Count && m < 3; m++)
                        table14 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                    table14 += "</tr>";

                    table13 += "<div>" + docnocolumn.summary + "</div><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + docnocolumn.color + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + docnocolumn.layno + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + docnocolumn.bundle + "</div></td>";
                    for (int m = 3; m < size.Count && m < 9; m++)
                        table13 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                    table13 += "</tr>";

                    table20 += "<div>" + docnocolumn.summary + "</div><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + docnocolumn.color + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + docnocolumn.layno + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + docnocolumn.bundle + "</div></td>";
                    for (int m = 9; m < size.Count && m < 13; m++)
                        table20 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                    table20 += "</tr>";

                    table21 += "<br /><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + docnocolumn.color + "</div></td>";
                    for (int m = 13; m < size.Count; m++)
                        table21 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                    table21 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + docnocolumn.totalqty + "</div></td><td style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.8cm'>TTL BY GO COLOR</div></td></tr>";
                    foreach (DataRow dr in dt.Rows)
                    {
                        string colortemp = dr[1].ToString().Length < 7 ? dr[1].ToString() : dr[1].ToString().Substring(0, 7);
                        //插入jo/color/size的qty
                        table14 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + colortemp + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
                        for (int m = 0; m < size.Count && m < 3; m++)
                            table14 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                        table14 += "</tr>";

                        table13 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + colortemp + "</div></td>";
                        for (int m = 3; m < size.Count && m < 9; m++)
                            table13 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                        table13 += "</tr>";

                        table20 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + colortemp + "</div></td>";
                        for (int m = 9; m < size.Count && m < 13; m++)
                            table20 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                        table20 += "</tr>";

                        table21 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + colortemp + "</div></td>";
                        for (int m = 13; m < size.Count; m++)
                            table21 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";

                        sqlDr = packagesql.gettotalqtydocno(sqlCon, dr[0].ToString(), dr[1].ToString(), doc_no);
                        string totalqty = "";
                        if (sqlDr.Read())
                        {
                            totalqty = sqlDr["TOTAL_QTY"].ToString();
                        }
                        sqlDr.Close();

                        table21 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + totalqty + "</div></td>";
                        if (colorrecord != dr[1].ToString())
                        {
                            colorrecord = dr[1].ToString();
                            //获取该color的行数
                            string colornum = "";
                            sqlDr = transfersql.getcolornum(sqlCon, dr[1].ToString(), doc_no, n);
                            if (sqlDr.Read())
                                colornum = sqlDr["QTY"].ToString();
                            sqlDr.Close();
                            ttl = "0";
                            sqlDr = transfersql.getttlbygocolor(sqlCon, dr[1].ToString(), doc_no);
                            if (sqlDr.Read())
                                ttl = sqlDr["QTY"].ToString();
                            sqlDr.Close();
                            table21 += "<td rowspan='" + colornum + "' style='width:0.9cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
                        }
                        else
                            table21 += "</tr>";
                    }
                    table14 += "</table>";
                    table13 += "</table>";
                    table20 += "</table>";
                    table21 += "</table>";
                    result += table14 + table13 + table20 + table21 + "</table>";
                }

                //table2，summary by color
                List<string> jolist2 = new List<string>();
                List<string> partlist2 = new List<string>();
                List<string> colorlist = new List<string>();
                List<string> partlistdesc = new List<string>();

                //查询该GO具有的JO/COLOR/PART
                sqlDr = transfersql.getjocolorpart(sqlCon, doc_no, n);
                while (sqlDr.Read())
                {
                    jolist2.Add(sqlDr["JOB_ORDER_NO"].ToString());
                    colorlist.Add(sqlDr["COLOR_CD"].ToString());
                    partlist2.Add(sqlDr["PART_CD"].ToString());
                    partlistdesc.Add(sqlDr["PART_DESC"].ToString());
                }
                sqlDr.Close();

                string table2 = "";
                table2 += "<br /><div name='hide1'>Summary BY Color</div><table name='hide2' style='font-size:11px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.color + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.parts + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.currenttotalqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.totaloutputqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.residualqty + "</div></td></tr>";
                //循环查询该JO/COLOR/PART
                for (int j = 0; j < jolist2.Count; j++)
                {
                    //获取实裁数
                    string totalcutqty = "0";
                    sqlDr = transfersql.getcutqty2(sqlCon, jolist2[j], colorlist[j], partlist2[j]);
                    if (sqlDr.Read())
                        if (sqlDr["QTY"].ToString() != "")
                            totalcutqty = sqlDr["QTY"].ToString();
                    sqlDr.Close();
                    //获取当前总计
                    string totalnowqty = "0";
                    sqlDr = transfersql.getnowqty2(sqlCon, doc_no, n, jolist2[j], colorlist[j], partlist2[j]);
                    if (sqlDr.Read())
                        if (sqlDr["QTY"].ToString() != "")
                            totalnowqty = sqlDr["QTY"].ToString();
                    sqlDr.Close();
                    //获取输出总计
                    string totaloutput = "0";
                    sqlDr = transfersql.getoutput2(sqlCon, factory, process, doc_no, jolist2[j], colorlist[j], partlist2[j]);
                    if (sqlDr.Read())
                        if (sqlDr["QTY"].ToString() != "")
                            totaloutput = sqlDr["QTY"].ToString();
                    sqlDr.Close();
                    //剩余数
                    string restqty = (int.Parse(totalcutqty) - int.Parse(totaloutput)).ToString();
                    //添加到summary by jo的table2里
                    table2 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + jolist2[j] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + colorlist[j] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + partlistdesc[j] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + totalcutqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + totalnowqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + totaloutput + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + restqty + "</div></td></tr>";
                }
                result += table2 + "</table>";

                //table4: summary by jo
                List<string> jolist1 = new List<string>();
                List<string> partlist1 = new List<string>();
                List<string> partlistdesc1 = new List<string>();

                //查询该GO具有的JO/PART
                sqlDr = transfersql.getjopart(sqlCon, doc_no, n);
                while (sqlDr.Read())
                {
                    jolist1.Add(sqlDr["JOB_ORDER_NO"].ToString());
                    partlist1.Add(sqlDr["PART_CD"].ToString());
                    partlistdesc1.Add(sqlDr["PART_DESC"].ToString());
                }
                sqlDr.Close();

                string table4 = "";
                table4 += "<br /><div name='hide3'>Summary BY JO</div><table name='hide4' style='font-size:11px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.parts + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.currenttotalqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.totaloutputqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + docnocolumn.residualqty + "</div></td></tr>";
                //循环查询该JO/PART
                for (int j = 0; j < jolist1.Count; j++)
                {
                    //获取实裁数
                    string totalcutqty = "0";
                    sqlDr = transfersql.getcutqty(sqlCon, jolist1[j], partlist1[j]);
                    if (sqlDr.Read())
                        if (sqlDr["QTY"].ToString() != "")
                            totalcutqty = sqlDr["QTY"].ToString();
                    sqlDr.Close();
                    //获取当前总计
                    string totalnowqty = "0";
                    sqlDr = transfersql.getnowqty(sqlCon, doc_no, jolist1[j], partlist1[j]);
                    if (sqlDr.Read())
                        if (sqlDr["QTY"].ToString() != "")
                            totalnowqty = sqlDr["QTY"].ToString();
                    sqlDr.Close();
                    //获取输出总计
                    string totaloutput = "0";
                    sqlDr = transfersql.getoutput(sqlCon, factory, process, doc_no, jolist1[j], partlist1[j]);
                    if (sqlDr.Read())
                        if (sqlDr["QTY"].ToString() != "")
                            totaloutput = sqlDr["QTY"].ToString();
                    sqlDr.Close();
                    //剩余数
                    string restqty = (int.Parse(totalcutqty) - int.Parse(totaloutput)).ToString();
                    //添加到summary by jo的table3里
                    table4 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + jolist1[j] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + partlistdesc1[j] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + totalcutqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + totalnowqty + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + totaloutput + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + restqty + "</div></td></tr>";
                }
                result += table4 + "</table>";
            }
            return result;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
        finally
        {
            sqlCon.Close();
            sqlCon.Dispose();
        }
    }


#region YMG 撤回菲纸功能(新的格式)
    ////获取箱码打印
    //public string PrintCartonBarcode(string cartonbarcode, CartonBarcode_Print cartonbarcodecolumn)
    //{
    //    //string[] translation = languagearray.Split(new char[] { '*' });
    //    try
    //    {
    //        SqlDataReader sqlDr;
    //        Packagesql packagesql = new Packagesql();
    //        Sqlstatement sqlstatement = new Sqlstatement();

    //        string process = "";
    //        string cut_line = "";
    //        string sew_line = "";
    //        //查找箱码所在的process
    //        sqlDr = sqlstatement.getcartonprocprod(sqlCon, cartonbarcode);
    //        if (sqlDr.Read())
    //        {
    //            process = sqlDr["PROCESS_CD"].ToString();
    //        }
    //        sqlDr.Close();
    //        //获取箱码的裁床组别
    //        sqlDr = sqlstatement.getcutline(sqlCon, cartonbarcode);
    //        int f = 0;
    //        while (sqlDr.Read())
    //        {
    //            if (f == 0)
    //                cut_line += sqlDr["CUT_LINE"].ToString();
    //            else
    //                cut_line += "/" + sqlDr["CUT_LINE"].ToString();
    //            f++;
    //        }
    //        sqlDr.Close();
    //        //获取箱码的车缝组别
    //        sqlDr = sqlstatement.getsewline(sqlCon, cartonbarcode);
    //        f = 0;
    //        while (sqlDr.Read())
    //        {
    //            if (f == 0)
    //                sew_line += sqlDr["PRODUCTION_LINE_CD"].ToString();
    //            else
    //                sew_line += "/" + sqlDr["PRODUCTION_LINE_CD"].ToString();
    //            f++;
    //        }
    //        sqlDr.Close();

    //        //查找GO、客户
    //        string customer = "";
    //        string gono = "";
    //        sqlDr = packagesql.getgocus(sqlCon, cartonbarcode);
    //        if (sqlDr.Read())
    //        {
    //            customer = sqlDr["SHORT_NAME"].ToString();
    //            gono = sqlDr["GARMENT_ORDER_NO"].ToString();
    //        }
    //        sqlDr.Close();

    //        //获取日期
    //        string date = DateTime.Now.ToString();

    //        //查询箱内裁片的部位
    //        sqlDr = packagesql.getpackagepart(sqlCon, cartonbarcode);
    //        f = 0;
    //        string parts = "";
    //        while (sqlDr.Read())
    //        {
    //            if (f == 0)
    //                parts += sqlDr["PART_DESC"].ToString();
    //            else
    //                parts += "/" + sqlDr["PART_DESC"].ToString();
    //            f++;
    //        }
    //        sqlDr.Close();
    //        //获取箱外的部件
    //        sqlDr = packagesql.getshortpackagepart(sqlCon, cartonbarcode);
    //        f = 0;
    //        string shortparts = "";
    //        while (sqlDr.Read())
    //        {
    //            if (f == 0)
    //                shortparts += sqlDr["PART_DESC"].ToString();
    //            else
    //                shortparts += "/" + sqlDr["PART_DESC"].ToString();
    //            f++;
    //        }
    //        sqlDr.Close();

    //        //总扎数
    //        sqlDr = sqlstatement.getttlbundle(sqlCon, cartonbarcode);
    //        int ttlbundle = 0;
    //        while (sqlDr.Read())
    //        {
    //            ttlbundle++;
    //        }
    //        sqlDr.Close();

    //        //获取该GO箱内尺码的数量
    //        sqlDr = packagesql.getsize(sqlCon, cartonbarcode);
    //        List<string> size = new List<string>();
    //        string sql = "";//拼写查询字符串
    //        while (sqlDr.Read())
    //        {
    //            size.Add(sqlDr["SIZE_CD"].ToString());
    //            sql += ",SUM(CASE SIZE_CD WHEN '" + sqlDr["SIZE_CD"].ToString() + "' THEN QTY ELSE 0 END)AS '" + sqlDr["SIZE_CD"].ToString() + "'";
    //        }
    //        sqlDr.Close();


    //        string table1 = "<table style='font-size:14px;'><tr>";
    //        table1 += "<td style='width:3.5cm;'><div>GO NO. " + gono + "</div></td>";
    //        table1 += "<td style='width:3.5cm;'><div>" + cartonbarcodecolumn.customer + " " + customer + "</div></td>";
    //        table1 += "</tr></table>";
    //        table1 += "<div id='barcodediv' style='height:150'></div></br>";
    //        table1 += "<table style='font-size:12px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr>";
    //        table1 += "<td style='width:3.0cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + cartonbarcodecolumn.cartonbarcode + "</div></td>";
    //        table1 += "<td style='width:3.5cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + cartonbarcode + "</div></td>";
    //        table1 += "</tr>";
    //        table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + cartonbarcodecolumn.date + "</div></td>";
    //        table1 += "<td style='width:3.5cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + date + "</div></td>";
    //        table1 += "</tr>";
    //        table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + cartonbarcodecolumn.process + "</div></td>";
    //        table1 += "<td style='width:3.5cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + process + "</div></td>";
    //        table1 += "</tr>";
    //        table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + cartonbarcodecolumn.production + "</div></td>";
    //        table1 += "<td style='width:3.5cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + cut_line + "</div></td>";
    //        table1 += "</tr>";
    //        table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + cartonbarcodecolumn.sewline + "</div></td>";
    //        table1 += "<td style='width:3.5cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + sew_line + "</div></td>";
    //        table1 += "</tr>";
    //        table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + cartonbarcodecolumn.part + "</div></td>";
    //        table1 += "<td style='width:3.5cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + parts + "</div></td>";
    //        table1 += "</tr>";
    //        table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + cartonbarcodecolumn.shortpart + "</div></td>";
    //        table1 += "<td style='width:3.5cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + shortparts + "</div></td>";
    //        table1 += "</tr>";
    //        table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + cartonbarcodecolumn.ttlbundle + "</div></td>";
    //        table1 += "<td style='width:3.5cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + ttlbundle.ToString() + "</div></td>";
    //        table1 += "</tr></table><br />";
    //        table1 += "<div>" + cartonbarcodecolumn.summary + "</div>";

    //        //得到结果集
    //        SqlDataAdapter sda = new SqlDataAdapter(packagesql.getjocolordb(sqlCon, sql, cartonbarcode));
    //        DataTable dt = new DataTable();
    //        sda.Fill(dt);

    //        //循环结果集，构造箱码报表
    //        string colorrecord = "";
    //        string result = "";
    //        string ttl = "0";
    //        if (size.Count < 2)
    //        {
    //            string table2 = "";
    //            table2 += "<table style='font-size:10px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + cartonbarcodecolumn.color + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + cartonbarcodecolumn.cutqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + cartonbarcodecolumn.layno + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + cartonbarcodecolumn.bundle + "</div></td>";
    //            foreach (string n in size)
    //                table2 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + n + "</div></td>";
    //            table2 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + cartonbarcodecolumn.totalqty + "</div></td><td style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:150px'>TTL BY GO COLOR</div></td></tr>";
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                //插入jo/color/size的qty
    //                table2 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + dr[1] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + dr[3] + "</div></td>";
    //                for (int k = 0; k < size.Count; k++)
    //                    table2 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[5 + k] + "</div></td>";

    //                sqlDr = packagesql.gettotalqty(sqlCon, dr[0].ToString(), dr[1].ToString(), cartonbarcode);
    //                string totalqty = "";
    //                if (sqlDr.Read())
    //                {
    //                    totalqty = sqlDr["TOTAL_QTY"].ToString();
    //                }
    //                sqlDr.Close();

    //                table2 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + totalqty + "</div></td>";
    //                if (colorrecord != dr[1].ToString())
    //                {
    //                    colorrecord = dr[1].ToString();
    //                    //获取该color的行数
    //                    string colornum = "";
    //                    sqlDr = packagesql.getcolornum(sqlCon, dr[1].ToString(), cartonbarcode);
    //                    if (sqlDr.Read())
    //                        colornum = sqlDr["QTY"].ToString();
    //                    sqlDr.Close();
    //                    ttl = "0";
    //                    sqlDr = packagesql.getttlbygocolor(sqlCon, dr[1].ToString(), cartonbarcode);
    //                    if (sqlDr.Read())
    //                        ttl = sqlDr["QTY"].ToString();
    //                    sqlDr.Close();
    //                    table2 += "<td rowspan='" + colornum + "' style='width:0.8cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
    //                }
    //                else
    //                    table2 += "</tr>";
    //            }
    //            table2 += "</table>";
    //            result = table1 + table2;
    //        }
    //        else if (size.Count >= 2 && size.Count <= 8)
    //        {
    //            //构建表头
    //            string table2 = "", table3 = "";
    //            table2 += "<table style='font-size:8px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + cartonbarcodecolumn.color + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + cartonbarcodecolumn.cutqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + cartonbarcodecolumn.layno + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + cartonbarcodecolumn.bundle + "</div></td>";
    //            for (int m = 0; m < size.Count && m < 3; m++)
    //                table2 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //            table2 += "</tr>";
    //            table3 += "<br /><table style='font-size:8px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + cartonbarcodecolumn.color + "</div></td>";
    //            for (int m = 3; m < size.Count; m++)
    //                table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //            table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + cartonbarcodecolumn.totalqty + "</div></td><td style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:150px'>TTL BY GO COLOR</div></td></tr>";
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                //插入jo/color/size的qty
    //                table2 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + dr[1] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + dr[3] + "</div></td>";
    //                for (int m = 0; m < size.Count && m < 3; m++)
    //                    table2 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[m + 5] + "</div></td>";
    //                table2 += "</tr>";

    //                table3 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + dr[1] + "</div></td>";
    //                for (int m = 3; m < size.Count; m++)
    //                    table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[m + 5] + "</div></td>";

    //                sqlDr = packagesql.gettotalqty(sqlCon, dr[0].ToString(), dr[1].ToString(), cartonbarcode);
    //                string totalqty = "";
    //                if (sqlDr.Read())
    //                {
    //                    totalqty = sqlDr["TOTAL_QTY"].ToString();
    //                }
    //                sqlDr.Close();

    //                table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + totalqty + "</div></td>";
    //                if (colorrecord != dr[1].ToString())
    //                {
    //                    colorrecord = dr[1].ToString();
    //                    //获取该color的行数
    //                    string colornum = "";
    //                    sqlDr = packagesql.getcolornum(sqlCon, dr[1].ToString(), cartonbarcode);
    //                    if (sqlDr.Read())
    //                        colornum = sqlDr["QTY"].ToString();
    //                    sqlDr.Close();
    //                    ttl = "0";
    //                    sqlDr = packagesql.getttlbygocolor(sqlCon, dr[1].ToString(), cartonbarcode);
    //                    if (sqlDr.Read())
    //                        ttl = sqlDr["QTY"].ToString();
    //                    sqlDr.Close();
    //                    table3 += "<td rowspan='" + colornum + "' style='width:0.8cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
    //                }
    //                else
    //                    table3 += "</tr>";
    //            }
    //            table2 += "</table>";
    //            table3 += "</table>";
    //            result = table1 + table2 + table3;
    //        }
    //        else if (size.Count >= 9 && size.Count <= 12)
    //        {
    //            //构建表头
    //            string table2 = "", table3 = "", table4 = "";
    //            table2 += "<table style='font-size:8px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + cartonbarcodecolumn.color + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + cartonbarcodecolumn.cutqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + cartonbarcodecolumn.layno + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + cartonbarcodecolumn.bundle + "</div></td>";
    //            for (int m = 0; m < size.Count && m < 3; m++)
    //                table2 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //            table2 += "</tr>";

    //            table3 += "<br /><table style='font-size:8px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + cartonbarcodecolumn.color + "</div></td>";
    //            for (int m = 3; m < size.Count && m < 9; m++)
    //                table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //            table3 += "</tr>";

    //            table4 += "<br /><table style='font-size:8px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + cartonbarcodecolumn.color + "</div></td>";
    //            for (int m = 9; m < size.Count; m++)
    //                table4 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //            table4 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + cartonbarcodecolumn.totalqty + "</div></td><td style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:150px'>TTL BY GO COLOR</div></td></tr>";
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                //插入jo/color/size的qty
    //                table2 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + dr[1] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + dr[3] + "</div></td>";
    //                for (int m = 0; m < size.Count && m < 3; m++)
    //                    table2 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[m + 5] + "</div></td>";
    //                table2 += "</tr>";

    //                table3 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + dr[1] + "</div></td>";
    //                for (int m = 3; m < size.Count && m < 9; m++)
    //                    table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[m + 5] + "</div></td>";
    //                table3 += "</tr>";

    //                table4 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + dr[1] + "</div></td>";
    //                for (int m = 9; m < size.Count; m++)
    //                    table4 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[m + 5] + "</div></td>";

    //                sqlDr = packagesql.gettotalqty(sqlCon, dr[0].ToString(), dr[1].ToString(), cartonbarcode);
    //                string totalqty = "";
    //                if (sqlDr.Read())
    //                {
    //                    totalqty = sqlDr["TOTAL_QTY"].ToString();
    //                }
    //                sqlDr.Close();

    //                table4 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + totalqty + "</div></td>";

    //                if (colorrecord != dr[1].ToString())
    //                {
    //                    colorrecord = dr[1].ToString();
    //                    //获取该color的行数
    //                    string colornum = "";
    //                    sqlDr = packagesql.getcolornum(sqlCon, dr[1].ToString(), cartonbarcode);
    //                    if (sqlDr.Read())
    //                        colornum = sqlDr["QTY"].ToString();
    //                    sqlDr.Close();
    //                    ttl = "0";
    //                    sqlDr = packagesql.getttlbygocolor(sqlCon, dr[1].ToString(), cartonbarcode);
    //                    if (sqlDr.Read())
    //                        ttl = sqlDr["QTY"].ToString();
    //                    sqlDr.Close();
    //                    table4 += "<td rowspan='" + colornum + "' style='width:0.8cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
    //                }
    //                else
    //                    table4 += "</tr>";
    //            }
    //            table2 += "</table>";
    //            table3 += "</table>";
    //            table4 += "</table>";
    //            result = table1 + table2 + table3 + table4;
    //        }
    //        else if (size.Count >= 13)
    //        {
    //            //构建表头
    //            string table2 = "", table3 = "", table4 = "", table5 = "";
    //            table2 += "<table style='font-size:8px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + cartonbarcodecolumn.color + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + cartonbarcodecolumn.cutqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + cartonbarcodecolumn.layno + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + cartonbarcodecolumn.bundle + "</div></td>";
    //            for (int m = 0; m < size.Count && m < 3; m++)
    //                table2 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //            table2 += "</tr>";

    //            table3 += "<br /><table style='font-size:8px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + cartonbarcodecolumn.color + "</div></td>";
    //            for (int m = 3; m < size.Count && m < 9; m++)
    //                table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //            table3 += "</tr>";

    //            table4 += "<br /><table style='font-size:8px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + cartonbarcodecolumn.color + "</div></td>";
    //            for (int m = 9; m < size.Count && m < 13; m++)
    //                table4 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //            table4 += "</tr>";

    //            table5 += "<br /><table style='font-size:8px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + cartonbarcodecolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + cartonbarcodecolumn.color + "</div></td>";
    //            for (int m = 13; m < size.Count; m++)
    //                table5 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //            table5 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + cartonbarcodecolumn.totalqty + "</div></td><td style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:150px'>TTL BY GO COLOR</div></td></tr>";
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                //插入jo/color/size的qty
    //                table2 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + dr[1] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + dr[3] + "</div></td>";
    //                for (int m = 0; m < size.Count && m < 3; m++)
    //                    table2 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + dr[m + 5] + "</div></td>";
    //                table2 += "</tr>";

    //                table3 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + dr[1] + "</div></td>";
    //                for (int m = 3; m < size.Count && m < 9; m++)
    //                    table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[m + 5] + "</div></td>";
    //                table3 += "</tr>";

    //                table4 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + dr[1] + "</div></td>";
    //                for (int m = 9; m < size.Count && m < 13; m++)
    //                    table4 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[m + 5] + "</div></td>";
    //                table4 += "</tr>";

    //                table5 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + dr[1] + "</div></td>";
    //                for (int m = 13; m < size.Count; m++)
    //                    table5 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[m + 5] + "</div></td>";

    //                sqlDr = packagesql.gettotalqty(sqlCon, dr[0].ToString(), dr[1].ToString(), cartonbarcode);
    //                string totalqty = "";
    //                if (sqlDr.Read())
    //                {
    //                    totalqty = sqlDr["TOTAL_QTY"].ToString();
    //                }
    //                sqlDr.Close();

    //                table5 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + totalqty + "</div></td>";

    //                if (colorrecord != dr[1].ToString())
    //                {
    //                    colorrecord = dr[1].ToString();
    //                    //获取该color的行数
    //                    string colornum = "";
    //                    sqlDr = packagesql.getcolornum(sqlCon, dr[1].ToString(), cartonbarcode);
    //                    if (sqlDr.Read())
    //                        colornum = sqlDr["QTY"].ToString();
    //                    sqlDr.Close();
    //                    ttl = "0";
    //                    sqlDr = packagesql.getttlbygocolor(sqlCon, dr[1].ToString(), cartonbarcode);
    //                    if (sqlDr.Read())
    //                        ttl = sqlDr["QTY"].ToString();
    //                    sqlDr.Close();
    //                    table5 += "<td rowspan='" + colornum + "' style='width:0.8cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
    //                }
    //                else
    //                    table5 += "</tr>";
    //            }
    //            table2 += "</table>";
    //            table3 += "</table>";
    //            table4 += "</table>";
    //            table5 += "</table>";
    //            result = table1 + table2 + table3 + table4 + table5;
    //        }
    //        return result;
    //    }
    //    catch(Exception ex)
    //    {
    //        return ex.ToString();
    //    }
    //    finally
    //    {
    //        sqlCon.Close();
    //        sqlCon.Dispose();
    //    }
    //}

    ////获取流水单打印
    //public string PrintDocno(string doc_no, Docno_Print docnocolumn)
    //{
    //    try
    //    {
    //        SqlDataReader sqlDr;
    //        Sqlstatement sqlstatement = new Sqlstatement();
    //        Transactionsql transfersql = new Transactionsql();
    //        Packagesql packagesql = new Packagesql();

    //        string factory = "", process = "", nextprocess = "", productionline = "", nextproductionline = "", username = "", createdate = "";
    //        sqlDr = transfersql.getdocnoinformation(sqlCon, doc_no);
    //        if (sqlDr.Read())
    //        {
    //            factory = sqlDr["FACTORY_CD"].ToString();
    //            process = sqlDr["PROCESS_CD"].ToString();
    //            nextprocess = sqlDr["NEXT_PROCESS_CD"].ToString();
    //            productionline = sqlDr["PRODUCTION_LINE_CD"].ToString();
    //            nextproductionline = sqlDr["NEXT_PRODUCTION_LINE_CD"].ToString();
    //            username = sqlDr["NAME"].ToString();
    //            createdate = sqlDr["CREATE_DATE"].ToString();
    //        }
    //        sqlDr.Close();

    //        string result = "";
    //        //查询该流水单具有的部件
    //        sqlDr = sqlstatement.getdocnopart(sqlCon, doc_no);
    //        int i = 0;
    //        string parts = "";
    //        while (sqlDr.Read())
    //        {
    //            if (i == 0)
    //                parts += sqlDr["PART_DESC"].ToString();
    //            else
    //                parts += "/" + sqlDr["PART_DESC"].ToString();
    //            i++;
    //        }
    //        sqlDr.Close();

    //        int cartonnum = 0;
    //        sqlDr = sqlstatement.getcartonnum(sqlCon, doc_no);
    //        while (sqlDr.Read())
    //        {
    //            cartonnum++;
    //        }
    //        sqlDr.Close();

    //        //流水单第一个table3
    //        string table3 = "";
    //        table3 += "<div style='font-size:14px;width:99%;'>";
    //        table3 += "<div style='width:100%;height:150; display: inline-block;'>";
    //        table3 += "<div style='font-size:14px;font-weight:bolder;margin-bottom:10px;'>(" + factory + ") Garment Transfer Note     [" + process + "->" + nextprocess + "] R <br/></div>";
    //        table3 += "<div id='barcodediv' style='height:150;float:right;width:150;text-align:right;'></div>";
    //        table3 += "<div style='float:left; width:75%;'> ";
    //        table3 += "<table style='font-size:13px; width:100%;border-collapse:collapse;height:150;'>";
    //        table3 += "<tr>";
    //        table3 += "<td style='width:80px;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.doc + "</div></td>";
    //        table3 += "<td   colspan='3' style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + factory + doc_no + "</div></td>";
    //        table3 += "<td   style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.user + "</div></td>";
    //        table3 += "<td  colspan='3' style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + username + "</div></td>";
    //        table3 += "</tr>";
    //        table3 += "<tr>";
    //        table3 += "<td style='width:31px;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.datetime + "</div></td>";
    //        table3 += "<td style='width:132px;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + createdate + "</div></td>";
    //        table3 += "<td style='width:57px;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" +docnocolumn.fromdept + "</div></td>";
    //        table3 += "<td style='width:0.9cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + process + "</div></td>";
    //        table3 += "<td style='width:57px;;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.fromline + "</div></td>";
    //        table3 += "<td style='width:3.0cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + productionline + "</div></td>";
    //        table3 += "<td style='width:57px;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.toline + "</div></td>";
    //        table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + nextproductionline + "</div></td>";
    //        table3 += "</tr>";
    //        table3 += "<tr>";
    //        table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.cartonnum + "</div></td>";
    //        table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + cartonnum + "</div></td>";
    //        table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.automatching + "</div></td>";
    //        table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" +  " "+ "</div></td>";
    //        table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.parts + "</div></td>";
    //        table3 += "<td  colspan='3' style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + parts + "</div></td>";
    //        table3 += "</tr>";
    //        table3 += "<tr>";
    //        table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;height:65px;'><div>" + docnocolumn.remark + "</div></td>";
    //        table3 += "<td colspan='7' style='width:1.5cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div></div></td>";
    //        table3 += "</tr>";
    //        table3 += "</table>";
    //        table3 += "</div></div>";
    //        table3 += "<div style='clear:both; '></div></div><br/><br/>";






    //        //查找流水单有多少个GO
    //        List<string> golist = new List<string>();
    //        sqlDr = transfersql.gettransfergo(sqlCon, doc_no);
    //        while (sqlDr.Read())
    //        {
    //            golist.Add(sqlDr["GARMENT_ORDER_NO"].ToString());
    //        }
    //        sqlDr.Close();

    //        result = table3;

    //        //遍历所有GO，table2,table3
    //        foreach (string n in golist)
    //        {
    //            string customer = "";
    //            sqlDr = transfersql.getcustomer(sqlCon, n);
    //            if (sqlDr.Read())
    //                customer = sqlDr["SHORT_NAME"].ToString();
    //            sqlDr.Close();

    //            result += "<table style='font-size:14px; margin-top:15px; '><tr><td style='width:3.5cm;'><div>GO NO." + n + "</div></td><td style='width:3.5cm;'><div>" + docnocolumn.customer + customer + "</div></td>";
    //            //PRT 总扎数/菲数，箱号 --added on 2018-02-28
    //            //是否显示PRINT 部位明细--added on 2018-02-28
    //            if (process == "CUT" && nextprocess == "PRT")
    //            {

    //                SqlDataAdapter spAdapter = new SqlDataAdapter(transfersql.Getprintpartdetail(sqlCon, doc_no, n));
    //                DataTable dtprint = new DataTable();
    //                spAdapter.Fill(dtprint);
    //                if (dtprint != null)
    //                {
    //                    if (dtprint.Rows.Count > 0)
    //                    {
    //                        var printTable = "";
    //                        printTable += "<td ><div>" + docnocolumn.print_bundlenum + ": " + dtprint.Rows.Count.ToString() + "</div></td>";//总扎数/菲数

    //                        //string[] sArray = dtprint.Rows[0]["CARTON_BARCODE"].ToString().Split('-');
    //                        printTable += "<td ><div>" + docnocolumn.print_carton_no + ": " + dtprint.Rows[0]["CAR_NO"].ToString() + " " + docnocolumn.print_untity + "</div></td>";//箱号 //车
    //                        printTable += "</tr></table><br />";


    //                        printTable += "<table style='font-size:13px; width:98%;border-collapse:collapse;height:150;'>";
    //                        printTable += "<tr><td  style='border-width:1px;border-style:solid;border-collapse:collapse; width:100px'><div>JO#</div></td>" +
    //                                      "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;width:120px''><div>" + docnocolumn.print_bundle_barcode + "</div></td>" +//裁片菲码
    //                                      "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.print_part + "</div></td>" +//幅位
    //                                      "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;width:120px''><div>" + docnocolumn.print_color + "</div></td>" +//颜色
    //                                      "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.print_lay + "</div></td>" +//床次
    //                                      "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.print_bundleno + "</div></td>" +//扎号
    //                                      "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.print_size+ "</div></td>" +//尺码
    //                                      "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn .print_qty+ "</div></td></tr>";//数量
    //                        foreach (DataRow dr in dtprint.Rows)
    //                        {
    //                            printTable += "<tr><td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + dr["JOB_ORDER_NO"] + "</div></td>" +
    //                                          "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + dr["BARCODE"] + "</div></td>" +
    //                                          "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + dr["PART_DESC"] + "</div></td>" +
    //                                          "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + dr["COLOR_CD"] + "</div></td>" +
    //                                          "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + dr["LAY_NO"] + "</div></td>" +
    //                                          "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + dr["BUNDLE_NO"] + "</div></td>" +
    //                                          "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + dr["SIZE_CD"] + "</div></td>" +
    //                                          "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + dr["QTY"] + "</div></td>" +
    //                                          "</tr>";
    //                        }

    //                        sqlDr = transfersql.Getprintpartsummary(sqlCon, doc_no, n);
    //                        if (sqlDr.Read())
    //                        {
    //                            printTable += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>TTL</div></td><td colspan='7' style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + sqlDr[0].ToString().TrimEnd('/') + "</div></td></tr>";
    //                        }
    //                        sqlDr.Close();

    //                        printTable += "</table><br/>";
    //                        result += printTable;
    //                    }
    //                }

    //            }
    //            else
    //            {
    //                result += "</tr></table><br />";
    //            }

    //            //------------------------------------------------------
    //            //JO/COLOR/SIZE的table
    //            //获取该GO箱内尺码的数量
    //            sqlDr = transfersql.getsize(sqlCon, doc_no, n);
    //            List<string> size = new List<string>();
    //            string sql = "";//拼写查询字符串
    //            while (sqlDr.Read())
    //            {
    //                size.Add(sqlDr["SIZE_CD"].ToString());
    //                sql += ",SUM(CASE SIZE_CD WHEN '" + sqlDr["SIZE_CD"].ToString() + "' THEN QTY ELSE 0 END)AS '" + sqlDr["SIZE_CD"].ToString() + "'";
    //            }
    //            sqlDr.Close();

    //            //得到结果集
    //            SqlDataAdapter sda = new SqlDataAdapter(transfersql.getjocolordb(sqlCon, sql, doc_no));
    //            DataTable dt = new DataTable();
    //            sda.Fill(dt);

    //            //循环结果集，构造箱码报表
    //            string colorrecord = "";
    //            string ttl = "0";
    //            if (size.Count < 2)
    //            {
    //                string table12 = "";
    //                table12 += "<div style='font-size:14px;'>" + docnocolumn.summary + "</div>";
    //                table12 += "<table style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + docnocolumn.color + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.layno + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.bundle + "</div></td>";
    //                foreach (string m in size)
    //                    table12 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + m + "</div></td>";
    //                table12 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.totalqty + "</div></td><td style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:150px'>TTL BY GO COLOR</div></td></tr>";
    //                foreach (DataRow dr in dt.Rows)
    //                {
    //                    string colortemp = dr[1].ToString().Length < 7 ? dr[1].ToString() : dr[1].ToString().Substring(0, 7);
    //                    //插入jo/color/size的qty
    //                    table12 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
    //                    for (int k = 0; k < size.Count; k++)
    //                        table12 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[5 + k] + "</div></td>";

    //                    sqlDr = packagesql.gettotalqtydocno(sqlCon, dr[0].ToString(), dr[1].ToString(), doc_no);
    //                    string totalqty = "";
    //                    if (sqlDr.Read())
    //                    {
    //                        totalqty = sqlDr["TOTAL_QTY"].ToString();
    //                    }
    //                    sqlDr.Close();

    //                    table12 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + totalqty + "</div></td>";
    //                    if (colorrecord != dr[1].ToString())
    //                    {
    //                        colorrecord = dr[1].ToString();
    //                        //获取该color的行数
    //                        string colornum = "";
    //                        sqlDr = transfersql.getcolornum(sqlCon, dr[1].ToString(), doc_no, n);
    //                        if (sqlDr.Read())
    //                            colornum = sqlDr["QTY"].ToString();
    //                        sqlDr.Close();
    //                        ttl = "0";
    //                        sqlDr = transfersql.getttlbygocolor(sqlCon, dr[1].ToString(), doc_no);
    //                        if (sqlDr.Read())
    //                            ttl = sqlDr["QTY"].ToString();
    //                        sqlDr.Close();
    //                        table12 += "<td rowspan='" + colornum + "' style='width:0.8cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
    //                    }
    //                    else
    //                        table12 += "</tr>";
    //                }
    //                table12 += "</table>";
    //                result += table12 + "</table>";
    //            }
    //            else if (size.Count >= 2 && size.Count <= 8)
    //            {
    //                //构建表头
    //                string table14 = "", table13 = "";
    //                table14 += "<div>" + docnocolumn.summary + "</div><table style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + docnocolumn.color + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.layno + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.bundle + "</div></td>";
    //                for (int m = 0; m < size.Count && m < 3; m++)
    //                    table14 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //                table14 += "</tr>";
    //                table13 += "<br /><table style='font-size:10px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + docnocolumn.color + "</div></td>";
    //                for (int m = 3; m < size.Count; m++)
    //                    table13 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //                table13 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.totalqty + "</div></td><td style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:150px'>TTL BY GO COLOR</div></td></tr>";
    //                foreach (DataRow dr in dt.Rows)
    //                {
    //                    string colortemp = dr[1].ToString().Length < 7 ? dr[1].ToString() : dr[1].ToString().Substring(0, 7);
    //                    //插入jo/color/size的qty
    //                    table14 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + dr[3] + "</div></td>";
    //                    for (int m = 0; m < size.Count && m < 3; m++)
    //                        table14 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[m + 5] + "</div></td>";
    //                    table14 += "</tr>";
    //                    table13 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + colortemp + "</div></td>";
    //                    for (int m = 3; m < size.Count; m++)
    //                        table13 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[m + 5] + "</div></td>";

    //                    sqlDr = packagesql.gettotalqtydocno(sqlCon, dr[0].ToString(), dr[1].ToString(), doc_no);
    //                    string totalqty = "";
    //                    if (sqlDr.Read())
    //                    {
    //                        totalqty = sqlDr["TOTAL_QTY"].ToString();
    //                    }
    //                    sqlDr.Close();

    //                    table13 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + totalqty + "</div></td>";
    //                    if (colorrecord != dr[1].ToString())
    //                    {
    //                        colorrecord = dr[1].ToString();
    //                        //获取该color的行数
    //                        string colornum = "";
    //                        sqlDr = transfersql.getcolornum(sqlCon, dr[1].ToString(), doc_no, n);
    //                        if (sqlDr.Read())
    //                            colornum = sqlDr["QTY"].ToString();
    //                        sqlDr.Close();
    //                        ttl = "0";
    //                        sqlDr = transfersql.getttlbygocolor(sqlCon, dr[1].ToString(), doc_no);
    //                        if (sqlDr.Read())
    //                            ttl = sqlDr["QTY"].ToString();
    //                        sqlDr.Close();
    //                        table13 += "<td rowspan='" + colornum + "' style='width:0.9cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
    //                    }
    //                    else
    //                        table13 += "</tr>";
    //                }
    //                table14 += "</table>";
    //                table13 += "</table>";
    //                result += table14 + table13 + "</table>";
    //            }
    //            else if (size.Count >= 9 && size.Count <= 12)
    //            {
    //                //构建表头
    //                string table14 = "", table13 = "", table20 = "";
    //                table14 += "<div>" + docnocolumn.summary + "</div><table style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + docnocolumn.color + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.layno + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.bundle + "</div></td>";
    //                for (int m = 0; m < size.Count && m < 3; m++)
    //                    table14 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //                table14 += "</tr>";

    //                table13 += "<div>" + docnocolumn.summary + "</div><table style='font-size:10px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + docnocolumn.color + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.layno + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.bundle + "</div></td>";
    //                for (int m = 3; m < size.Count && m < 9; m++)
    //                    table13 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //                table13 += "</tr>";

    //                table20 += "<br /><table style='font-size:10px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + docnocolumn.color + "</div></td>";
    //                for (int m = 9; m < size.Count && m < 13; m++)
    //                    table20 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //                table20 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.totalqty + "</div></td><td style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:150px'>TTL BY GO COLOR</div></td></tr>";

    //                foreach (DataRow dr in dt.Rows)
    //                {
    //                    string colortemp = dr[1].ToString().Length < 7 ? dr[1].ToString() : dr[1].ToString().Substring(0, 7);
    //                    //插入jo/color/size的qty
    //                    table14 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + dr[3] + "</div></td>";
    //                    for (int m = 0; m < size.Count && m < 3; m++)
    //                        table14 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[m + 5] + "</div></td>";
    //                    table14 += "</tr>";

    //                    table13 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + colortemp + "</div></td>";
    //                    for (int m = 3; m < size.Count && m < 9; m++)
    //                        table13 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[m + 5] + "</div></td>";
    //                    table13 += "</tr>";

    //                    table20 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + colortemp + "</div></td>";
    //                    for (int m = 9; m < size.Count && m < 13; m++)
    //                        table20 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[m + 5] + "</div></td>";

    //                    sqlDr = packagesql.gettotalqtydocno(sqlCon, dr[0].ToString(), dr[1].ToString(), doc_no);
    //                    string totalqty = "";
    //                    if (sqlDr.Read())
    //                    {
    //                        totalqty = sqlDr["TOTAL_QTY"].ToString();
    //                    }
    //                    sqlDr.Close();

    //                    table20 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + totalqty + "</div></td>";
    //                    if (colorrecord != dr[1].ToString())
    //                    {
    //                        colorrecord = dr[1].ToString();
    //                        //获取该color的行数
    //                        string colornum = "";
    //                        sqlDr = transfersql.getcolornum(sqlCon, dr[1].ToString(), doc_no, n);
    //                        if (sqlDr.Read())
    //                            colornum = sqlDr["QTY"].ToString();
    //                        sqlDr.Close();
    //                        ttl = "0";
    //                        sqlDr = transfersql.getttlbygocolor(sqlCon, dr[1].ToString(), doc_no);
    //                        if (sqlDr.Read())
    //                            ttl = sqlDr["QTY"].ToString();
    //                        sqlDr.Close();
    //                        table20 += "<td rowspan='" + colornum + "' style='width:0.9cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
    //                    }
    //                    else
    //                        table20 += "</tr>";
    //                }
    //                table14 += "</table>";
    //                table13 += "</table>";
    //                table20 += "</table>";
    //                result += table14 + table13 + table20 + "</table>";
    //            }
    //            else if (size.Count >= 13)
    //            {
    //                //构建表头
    //                string table14 = "", table13 = "", table20 = "", table21 = "";

    //                table14 += "<div>" + docnocolumn.summary + "</div><table style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + docnocolumn.color + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.layno + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.bundle + "</div></td>";
    //                for (int m = 0; m < size.Count && m < 3; m++)
    //                    table14 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //                table14 += "</tr>";

    //                table13 += "<div>" + docnocolumn.summary + "</div><table style='font-size:10px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + docnocolumn.color + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.layno + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.bundle + "</div></td>";
    //                for (int m = 3; m < size.Count && m < 9; m++)
    //                    table13 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //                table13 += "</tr>";

    //                table20 += "<div>" + docnocolumn.summary + "</div><table style='font-size:10px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + docnocolumn.color + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.layno + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.bundle + "</div></td>";
    //                for (int m = 9; m < size.Count && m < 13; m++)
    //                    table20 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //                table20 += "</tr>";

    //                table21 += "<br /><table style='font-size:10px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + docnocolumn.color + "</div></td>";
    //                for (int m = 13; m < size.Count; m++)
    //                    table21 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + size[m] + "</div></td>";
    //                table21 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + docnocolumn.totalqty + "</div></td><td style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:150px'>TTL BY GO COLOR</div></td></tr>";
    //                foreach (DataRow dr in dt.Rows)
    //                {
    //                    string colortemp = dr[1].ToString().Length < 7 ? dr[1].ToString() : dr[1].ToString().Substring(0, 7);
    //                    //插入jo/color/size的qty
    //                    table14 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
    //                    for (int m = 0; m < size.Count && m < 3; m++)
    //                        table14 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[m + 5] + "</div></td>";
    //                    table14 += "</tr>";

    //                    table13 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + colortemp + "</div></td>";
    //                    for (int m = 3; m < size.Count && m < 9; m++)
    //                        table13 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[m + 5] + "</div></td>";
    //                    table13 += "</tr>";

    //                    table20 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + colortemp + "</div></td>";
    //                    for (int m = 9; m < size.Count && m < 13; m++)
    //                        table20 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[m + 5] + "</div></td>";
    //                    table20 += "</tr>";

    //                    table21 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.0cm;word-break: break-all'>" + colortemp + "</div></td>";
    //                    for (int m = 13; m < size.Count; m++)
    //                        table21 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px;word-break: break-all'>" + dr[m + 5] + "</div></td>";

    //                    sqlDr = packagesql.gettotalqtydocno(sqlCon, dr[0].ToString(), dr[1].ToString(), doc_no);
    //                    string totalqty = "";
    //                    if (sqlDr.Read())
    //                    {
    //                        totalqty = sqlDr["TOTAL_QTY"].ToString();
    //                    }
    //                    sqlDr.Close();

    //                    table21 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + totalqty + "</div></td>";
    //                    if (colorrecord != dr[1].ToString())
    //                    {
    //                        colorrecord = dr[1].ToString();
    //                        //获取该color的行数
    //                        string colornum = "";
    //                        sqlDr = transfersql.getcolornum(sqlCon, dr[1].ToString(), doc_no, n);
    //                        if (sqlDr.Read())
    //                            colornum = sqlDr["QTY"].ToString();
    //                        sqlDr.Close();
    //                        ttl = "0";
    //                        sqlDr = transfersql.getttlbygocolor(sqlCon, dr[1].ToString(), doc_no);
    //                        if (sqlDr.Read())
    //                            ttl = sqlDr["QTY"].ToString();
    //                        sqlDr.Close();
    //                        table21 += "<td rowspan='" + colornum + "' style='width:0.9cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
    //                    }
    //                    else
    //                        table21 += "</tr>";
    //                }
    //                table14 += "</table>";
    //                table13 += "</table>";
    //                table20 += "</table>";
    //                table21 += "</table>";
    //                result += table14 + table13 + table20 + table21 + "</table>";
    //            }

    //            //table2，summary by color
    //            List<string> jolist2 = new List<string>();
    //            List<string> partlist2 = new List<string>();
    //            List<string> colorlist = new List<string>();
    //            List<string> partlistdesc = new List<string>();

    //            //查询该GO具有的JO/COLOR/PART
    //            sqlDr = transfersql.getjocolorpart(sqlCon, doc_no, n);
    //            while (sqlDr.Read())
    //            {
    //                jolist2.Add(sqlDr["JOB_ORDER_NO"].ToString());
    //                colorlist.Add(sqlDr["COLOR_CD"].ToString());
    //                partlist2.Add(sqlDr["PART_CD"].ToString());
    //                partlistdesc.Add(sqlDr["PART_DESC"].ToString());
    //            }
    //            sqlDr.Close();

    //            string table2 = "";
    //            table2 += "<br /><div name='hide1'>Summary BY Color</div><table name='hide2' style='font-size:11px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.color + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.parts + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.currenttotalqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.totaloutputqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.residualqty + "</div></td></tr>";
    //            //循环查询该JO/COLOR/PART
    //            for (int j = 0; j < jolist2.Count; j++)
    //            {
    //                //获取实裁数
    //                string totalcutqty = "0";
    //                sqlDr = transfersql.getcutqty2(sqlCon, jolist2[j], colorlist[j], partlist2[j]);
    //                if (sqlDr.Read())
    //                    if (sqlDr["QTY"].ToString() != "")
    //                        totalcutqty = sqlDr["QTY"].ToString();
    //                sqlDr.Close();
    //                //获取当前总计
    //                string totalnowqty = "0";
    //                sqlDr = transfersql.getnowqty2(sqlCon, doc_no, n, jolist2[j], colorlist[j], partlist2[j]);
    //                if (sqlDr.Read())
    //                    if (sqlDr["QTY"].ToString() != "")
    //                        totalnowqty = sqlDr["QTY"].ToString();
    //                sqlDr.Close();
    //                //获取输出总计
    //                string totaloutput = "0";
    //                sqlDr = transfersql.getoutput2(sqlCon, factory, process, doc_no, jolist2[j], colorlist[j], partlist2[j]);
    //                if (sqlDr.Read())
    //                    if (sqlDr["QTY"].ToString() != "")
    //                        totaloutput = sqlDr["QTY"].ToString();
    //                sqlDr.Close();
    //                //剩余数
    //                string restqty = (int.Parse(totalcutqty) - int.Parse(totaloutput)).ToString();
    //                //添加到summary by jo的table2里
    //                table2 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + jolist2[j] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + colorlist[j] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + partlistdesc[j] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + totalcutqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + totalnowqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + totaloutput + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + restqty + "</div></td></tr>";
    //            }
    //            result += table2 + "</table>";

    //            //table4: summary by jo
    //            List<string> jolist1 = new List<string>();
    //            List<string> partlist1 = new List<string>();
    //            List<string> partlistdesc1 = new List<string>();

    //            //查询该GO具有的JO/PART
    //            sqlDr = transfersql.getjopart(sqlCon, doc_no, n);
    //            while (sqlDr.Read())
    //            {
    //                jolist1.Add(sqlDr["JOB_ORDER_NO"].ToString());
    //                partlist1.Add(sqlDr["PART_CD"].ToString());
    //                partlistdesc1.Add(sqlDr["PART_DESC"].ToString());
    //            }
    //            sqlDr.Close();

    //            string table4 = "";
    //            table4 += "<br /><div name='hide3'>Summary BY JO</div><table name='hide4' style='font-size:11px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.jo + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.parts + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.totalcutqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.currenttotalqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.totaloutputqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + docnocolumn.residualqty + "</div></td></tr>";
    //            //循环查询该JO/PART
    //            for (int j = 0; j < jolist1.Count; j++)
    //            {
    //                //获取实裁数
    //                string totalcutqty = "0";
    //                sqlDr = transfersql.getcutqty(sqlCon, jolist1[j], partlist1[j]);
    //                if (sqlDr.Read())
    //                    if (sqlDr["QTY"].ToString() != "")
    //                        totalcutqty = sqlDr["QTY"].ToString();
    //                sqlDr.Close();
    //                //获取当前总计
    //                string totalnowqty = "0";
    //                sqlDr = transfersql.getnowqty(sqlCon, doc_no, jolist1[j], partlist1[j]);
    //                if (sqlDr.Read())
    //                    if (sqlDr["QTY"].ToString() != "")
    //                        totalnowqty = sqlDr["QTY"].ToString();
    //                sqlDr.Close();
    //                //获取输出总计
    //                string totaloutput = "0";
    //                sqlDr = transfersql.getoutput(sqlCon, factory, process, doc_no, jolist1[j], partlist1[j]);
    //                if (sqlDr.Read())
    //                    if (sqlDr["QTY"].ToString() != "")
    //                        totaloutput = sqlDr["QTY"].ToString();
    //                sqlDr.Close();
    //                //剩余数
    //                string restqty = (int.Parse(totalcutqty) - int.Parse(totaloutput)).ToString();
    //                //添加到summary by jo的table3里
    //                table4 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + jolist1[j] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + partlistdesc1[j] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + totalcutqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + totalnowqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + totaloutput + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + restqty + "</div></td></tr>";
    //            }
    //            result += table4 + "</table>";
    //        }
    //        return result;
    //    }
    //    catch (Exception ex)
    //    {
    //        return ex.ToString();
    //    }
    //    finally
    //    {
    //        sqlCon.Close();
    //        sqlCon.Dispose();
    //    }
    //}

#endregion

    public List<GO> GetGO(string factory)
    {
        try
        {
            sqlComGet.Connection = sqlCon;
            sqlComGet.CommandText = "SELECT GARMENT_ORDER_NO FROM CIPMS_BUNDLE_FOR_SCANNING WITH(NOLOCK) WHERE FACTORY_CD='" + factory + "' GROUP BY GARMENT_ORDER_NO ORDER BY GARMENT_ORDER_NO DESC";
            SqlDataReader sqlDr = sqlComGet.ExecuteReader();
            List<GO> golist = new List<GO>();
            while (sqlDr.Read())
            {
                GO go = new GO();
                go.GARMENT_ORDER_NO = sqlDr["GARMENT_ORDER_NO"].ToString();
                golist.Add(go);
            }
            sqlDr.Close();

            return golist;
        }
        catch (Exception ex)
        {
        }
        finally
        {
            sqlCon.Close();
        }
        return null;
    }

    public List<JOBORDERNO> GetJO(string factory, string GO)
    {
        try
        {
            sqlComGet.Connection = sqlCon;
            sqlComGet.CommandText = "SELECT JOB_ORDER_NO FROM CIPMS_BUNDLE_FOR_SCANNING WITH(NOLOCK) WHERE FACTORY_CD='" + factory + "' AND GARMENT_ORDER_NO='" + GO + "' GROUP BY JOB_ORDER_NO ORDER BY JOB_ORDER_NO DESC";
            SqlDataReader sqlDr = sqlComGet.ExecuteReader();
            List<JOBORDERNO> jolist = new List<JOBORDERNO>();
            while (sqlDr.Read())
            {
                JOBORDERNO jo = new JOBORDERNO();
                jo.JOB_ORDER_NO = sqlDr["JOB_ORDER_NO"].ToString();
                jolist.Add(jo);
            }
            sqlDr.Close();

            return jolist;
        }
        catch (Exception ex)
        {
        }
        finally
        {
            sqlCon.Close();
        }
        return null;
    }

    //根据制单获取颜色列表
    public List<COLOR> GetColorByJO(string JO)
    {
        try
        {
            sqlComGet.Connection = sqlCon;
            sqlComGet.CommandText = "SELECT COLOR_CD FROM CIPMS_BUNDLE_FOR_SCANNING WITH(NOLOCK) WHERE JOB_ORDER_NO='" + JO + "' GROUP BY COLOR_CD";
            SqlDataReader sqlDr = sqlComGet.ExecuteReader();
            List<COLOR> colorlist = new List<COLOR>();
            while (sqlDr.Read())
            {
                COLOR color = new COLOR();
                color.COLOR_CD = sqlDr["COLOR_CD"].ToString();
                colorlist.Add(color);
            }
            sqlDr.Close();

            return colorlist;
        }
        catch (Exception ex)
        {
        }
        finally
        {
        }
        return null;
    }

    //根据制单获取颜色列表
    public List<LAYNOLIST> GetLaynoByJO(string JO)
    {
        try
        {
            sqlComGet.Connection = sqlCon;
            sqlComGet.CommandText = "SELECT LAY_NO FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO='" + JO + "' GROUP BY LAY_NO ORDER BY LAY_NO ASC";
            SqlDataReader sqlDr = sqlComGet.ExecuteReader();
            List<LAYNOLIST> laynolist = new List<LAYNOLIST>();
            while (sqlDr.Read())
            {
                LAYNOLIST layno = new LAYNOLIST();
                layno.LAY_NO = sqlDr["LAY_NO"].ToString();
                laynolist.Add(layno);
            }
            sqlDr.Close();

            return laynolist;
        }
        catch (Exception ex)
        {
        }
        finally
        {
        }
        return null;
    }

    //根据制单获取颜色列表
    public List<BUNDLENOLIST> GetBundlenoByJO(string JO)
    {
        try
        {
            sqlComGet.Connection = sqlCon;
            sqlComGet.CommandText = "SELECT BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO='" + JO + "' GROUP BY BUNDLE_NO ORDER BY BUNDLE_NO ASC";
            SqlDataReader sqlDr = sqlComGet.ExecuteReader();
            List<BUNDLENOLIST> bundlenolist = new List<BUNDLENOLIST>();
            while (sqlDr.Read())
            {
                BUNDLENOLIST bundleno = new BUNDLENOLIST();
                bundleno.BUNDLE_NO = sqlDr["BUNDLE_NO"].ToString();
                bundlenolist.Add(bundleno);
            }
            sqlDr.Close();

            return bundlenolist;
        }
        catch (Exception ex)
        {
        }
        finally
        {
        }
        return null;
    }

    public List<LAYNOLIST> GetLaynoByJOColor(string JO, string COLOR)
    {
        try
        {
            sqlComGet.Connection = sqlCon;
            sqlComGet.CommandText = "SELECT LAY_NO FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO='" + JO + "', COLOR_CD='" + COLOR + "' GROUP BY LAY_NO ORDER BY LAY_NO ASC";
            SqlDataReader sqlDr = sqlComGet.ExecuteReader();
            List<LAYNOLIST> laynolist = new List<LAYNOLIST>();
            while (sqlDr.Read())
            {
                LAYNOLIST layno = new LAYNOLIST();
                layno.LAY_NO = sqlDr["LAY_NO"].ToString();
                laynolist.Add(layno);
            }
            sqlDr.Close();

            return laynolist;
        }
        catch (Exception ex)
        {
        }
        finally
        {
            sqlCon.Close();
        }
        return null;
    }

    public List<BUNDLENOLIST> GetBundlenoByJOColor(string JO, string COLOR)
    {
        try
        {
            sqlComGet.Connection = sqlCon;
            sqlComGet.CommandText = "SELECT BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO='" + JO + "' AND COLOR_CD='"+COLOR+"' GROUP BY BUNDLE_NO ORDER BY BUNDLE_NO ASC";
            SqlDataReader sqlDr = sqlComGet.ExecuteReader();
            List<BUNDLENOLIST> bundlenolist = new List<BUNDLENOLIST>();
            while (sqlDr.Read())
            {
                BUNDLENOLIST bundleno = new BUNDLENOLIST();
                bundleno.BUNDLE_NO = sqlDr["BUNDLE_NO"].ToString();
                bundlenolist.Add(bundleno);
            }
            sqlDr.Close();

            return bundlenolist;
        }
        catch (Exception ex)
        {
        }
        finally
        {
            sqlCon.Close();
        }
        return null;
    }

    public List<COLOR> GetColorByJOLayno(string JO, string LAYNO)
    {
        try
        {
            sqlComGet.Connection = sqlCon;
            sqlComGet.CommandText = "SELECT COLOR_CD FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO='" + JO + "' AND LAY_NO=" + LAYNO + " GROUP BY COLOR_CD ORDER BY COLOR_CD ASC";
            SqlDataReader sqlDr = sqlComGet.ExecuteReader();
            List<COLOR> colorlist = new List<COLOR>();
            while (sqlDr.Read())
            {
                COLOR color = new COLOR();
                color.COLOR_CD = sqlDr["COLOR_CD"].ToString();
                colorlist.Add(color);
            }
            sqlDr.Close();

            return colorlist;
        }
        catch (Exception ex)
        {
        }
        finally
        {
            sqlCon.Close();
        }
        return null;
    }

    public List<BUNDLENOLIST> GetBundlenoByJOLayno(string JO, string LAYNO)
    {
        try
        {
            sqlComGet.Connection = sqlCon;
            sqlComGet.CommandText = "SELECT BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO='" + JO + "' AND LAY_NO=" + LAYNO + " GROUP BY BUNDLE_NO ORDER BY BUNDLE_NO ASC";
            SqlDataReader sqlDr = sqlComGet.ExecuteReader();
            List<BUNDLENOLIST> bundlenolist = new List<BUNDLENOLIST>();
            while (sqlDr.Read())
            {
                BUNDLENOLIST bundleno = new BUNDLENOLIST();
                bundleno.BUNDLE_NO = sqlDr["BUNDLE_NO"].ToString();
                bundlenolist.Add(bundleno);
            }
            sqlDr.Close();

            return bundlenolist;
        }
        catch (Exception ex)
        {
        }
        finally
        {
            sqlCon.Close();
        }
        return null;
    }

    public List<BUNDLENOLIST> GetBundlenoByJOColorLayno(string JO, string COLOR, string LAYNO)
    {
        try
        {
            sqlComGet.Connection = sqlCon;
            sqlComGet.CommandText = "SELECT BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO='" + JO + "' AND COLOR_CD='" + COLOR + "' AND LAY_NO=" + LAYNO + " GROUP BY BUNDLE_NO ORDER BY BUNDLE_NO ASC";
            SqlDataReader sqlDr = sqlComGet.ExecuteReader();
            List<BUNDLENOLIST> bundlenolist = new List<BUNDLENOLIST>();
            while (sqlDr.Read())
            {
                BUNDLENOLIST bundleno = new BUNDLENOLIST();
                bundleno.BUNDLE_NO = sqlDr["BUNDLE_NO"].ToString();
                bundlenolist.Add(bundleno);
            }
            sqlDr.Close();

            return bundlenolist;
        }
        catch (Exception ex)
        {
        }
        finally
        {
            sqlCon.Close();
        }
        return null;
    }
}