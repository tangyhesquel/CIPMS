using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;
using NameSpace;
using System.Data;

/// <summary>
///Cartonbarcodeprint 的摘要说明
/// </summary>
public class Cartonbarcodeprint
{
	public Cartonbarcodeprint()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}

    public String Cartonbarcodehtml(SqlConnection sqlCon, string cartonbarcode, string languagearray)
    {
        string[] translation = languagearray.Split(new char[] { '*' });

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
        table1 += "<td style='width:3.5cm;'><div>" + translation[0] + " " + customer + "</div></td>";
        table1 += "</tr></table>";
        table1 += "<div id='barcodediv' style='height:150'></div></br>";
        table1 += "<table style='font-size:12px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr>";
        table1 += "<td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[12] + "</div></td>";
        table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + cartonbarcode + "</div></td>";
        table1 += "</tr>";
        table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[1] + "</div></td>";
        table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + date + "</div></td>";
        table1 += "</tr>";
        table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[2] + "</div></td>";
        table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + process + "</div></td>";
        table1 += "</tr>";
        table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[3] + "</div></td>";
        table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + cut_line + "</div></td>";
        table1 += "</tr>";

        //tangyh 2017.03.27
        //获取印花绣花状态
        sqlDr = packagesql.queryEmbAndPrt(sqlCon, gono);
        string brt = "";
        string emb = "";

        if (sqlDr.Read())
        {
            brt = sqlDr["PRINTING"].ToString();
            emb = sqlDr["EMBROIDERY"].ToString();
        }
        sqlDr.Close();

        table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[16] + "</div></td>";
        table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + brt + "</div></td>";
        table1 += "</tr>";

        table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[17] + "</div></td>";
        table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + emb + "</div></td>";
        table1 += "</tr>";
        if ((brt == "Y") && (emb == "Y")) //2、	当印花与绣花同时为Y时，带出印后绣花这个字段
        {
            table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[18] + "</div></td>";
            table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + "" + "</div></td>";
            table1 += "</tr>";
        }

        if (brt == "Y") //1、	当印花为Y时，带出印花士啤这个字段；
        {
            table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[19] + "</div></td>";
            table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + "" + "</div></td>";
            table1 += "</tr>";
        }


        table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[13] + "</div></td>";
        table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + sew_line + "</div></td>";
        table1 += "</tr>";
        table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[4] + "</div></td>";
        table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + parts + "</div></td>";
        table1 += "</tr>";
        table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[15] + "</div></td>";
        table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + shortparts + "</div></td>";
        table1 += "</tr>";
        table1 += "<tr><td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[14] + "</div></td>";
        table1 += "<td style='width:3.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + ttlbundle.ToString() + "</div></td>";
        table1 += "</tr></table><br />";
        table1 += "<div>" + translation[5] + "</div>";

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
            table2 += "<table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + translation[6] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + translation[7] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + translation[8] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + translation[10] + "</div></td>";
            foreach (string n in size)
                table2 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + n + "</div></td>";
            table2 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[11] + "</div></td><td style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.8cm'>TTL BY GO COLOR</div></td></tr>";
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
            table2 += "<table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + translation[6] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + translation[7] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + translation[8] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + translation[10] + "</div></td>";
            for (int m = 0; m < size.Count && m < 3; m++)
                table2 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
            table2 += "</tr>";
            table3 += "<br /><table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + translation[6] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + translation[7] + "</div></td>";
            for (int m = 3; m < size.Count; m++)
                table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
            table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[11] + "</div></td><td style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.8cm'>TTL BY GO COLOR</div></td></tr>";
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
            string table2 = "", table3 = "", table4="";
            table2 += "<table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + translation[6] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + translation[7] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + translation[8] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + translation[10] + "</div></td>";
            for (int m = 0; m < size.Count && m < 3; m++)
                table2 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
            table2 += "</tr>";

            table3 += "<br /><table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + translation[6] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + translation[7] + "</div></td>";
            for (int m = 3; m < size.Count && m < 9; m++)
                table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
            table3 += "</tr>";

            table4 += "<br /><table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + translation[6] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + translation[7] + "</div></td>";
            for (int m = 9; m < size.Count; m++)
                table4 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
            table4 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[11] + "</div></td><td style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.8cm'>TTL BY GO COLOR</div></td></tr>";
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
            string table2 = "", table3 = "", table4 = "", table5="";
            table2 += "<table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + translation[6] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + translation[7] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + translation[8] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + translation[10] + "</div></td>";
            for (int m = 0; m < size.Count && m < 3; m++)
                table2 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
            table2 += "</tr>";

            table3 += "<br /><table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + translation[6] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + translation[7] + "</div></td>";
            for (int m = 3; m < size.Count && m < 9; m++)
                table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
            table3 += "</tr>";

            table4 += "<br /><table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + translation[6] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + translation[7] + "</div></td>";
            for (int m = 9; m < size.Count && m < 13; m++)
                table4 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
            table4 += "</tr>";

            table5 += "<br /><table style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.6cm'>" + translation[6] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.0cm'>" + translation[7] + "</div></td>";
            for (int m = 13; m < size.Count; m++)
                table5 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
            table5 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[11] + "</div></td><td style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.8cm'>TTL BY GO COLOR</div></td></tr>";
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
}