using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;
using NameSpace;
using System.Data;

/// <summary>
///Docnoproduce 的摘要说明
/// </summary>
public class Docnoproduce
{
	public Docnoproduce()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}


public String Docnostring(SqlConnection sqlCon, string userbarcode, string doc_no, string factory, string thisprocess, string thisproduction, string nextprocess, string nextproduction, string languagearray, string automatching)
    {
        string[] translation = languagearray.Split(new char[] { '*' });
        SqlDataReader sqlDr;
        Sqlstatement sqlstatement = new Sqlstatement();
        Transactionsql transfersql = new Transactionsql();
        Packagesql packagesql = new Packagesql();

        //找到用户的名字
        string username = "";
        sqlDr = sqlstatement.getname(sqlCon, userbarcode);
        if (sqlDr.Read())
            username = sqlDr["NAME"].ToString();
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
        table3 += "<div style='font-size:14px;width:7.0cm'>(" + factory + ")Garment Transfer Note-[" + thisprocess + "->" + nextprocess + "] R</div><br />";
        table3 += "<div style='font-size:14px;width:7.0cm'>" + translation[10] + factory + doc_no + translation[0] + username + "</div>";
        table3 += "<div id='barcodediv' style='height:150'></div></br>";
        table3 += "<table style='font-size:13px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr>";
        table3 += "<td style='width:1.2cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[1] + "</div></td>";
        table3 += "<td style='width:3.3cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + DateTime.Now.ToString() + "</div></td>";
        table3 += "<td style='width:1.6cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[2] + "</div></td>";
        table3 += "<td style='width:0.9cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + thisprocess + "</div></td>";
        table3 += "</tr></table>";
        table3 += "<table style='font-size:13px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr>";
        table3 += "<td style='width:1.4cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[3] + "</div></td>";
        table3 += "<td style='width:3.0cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + thisproduction + "</div></td>";
        table3 += "<td rowspan='5' style='width:1.1cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[6] + "</div></td>";
        table3 += "<td rowspan='5' style='width:1.5cm;border-width:1px;border-style:dotted;border-collapse:collapse;'><div></div></td></tr>";
        table3 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[4] + "</div></td>";
        table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + nextproduction + "</div></td></tr>";
        table3 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[19] + "</div></td>";
        table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + cartonnum + "</div></td></tr>";
        table3 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[5] + "</div></td>";
        table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + parts + "</div></td></tr>";
        table3 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[20] + "</div></td>";
        table3 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>"+automatching+"</div></td></tr></table><br />";

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

            result += "<table style='font-size:14px;'><tr><td style='width:3.5cm;'><div>GO NO." + n + "</div></td><td style='width:3.5cm;'><div>" + translation[7] + customer + "</div></td></tr></table><br />";

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
                table12 += "<div>" + translation[8] + "</div>";
                table12 += "<table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + translation[15] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[12] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + translation[13] + "</div></td>";
                foreach (string m in size)
                    table12 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + m + "</div></td>";
                table12 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[14] + "</div></td><td style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.8cm'>TTL BY GO COLOR</div></td></tr>";
                foreach (DataRow dr in dt.Rows)
                {
                    string colortemp = dr[1].ToString().Length < 70 ? dr[1].ToString() : dr[1].ToString().Substring(0, 7);
                    //插入jo/color/size的qty
                    table12 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
                    for (int k = 0; k < size.Count; k++)
                        table12 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[5 + k] + "</div></td>";

                    sqlDr = packagesql.gettotalqtydocno(sqlCon, dr[0].ToString(), dr[1].ToString(), doc_no);
                    string totalqty = "";
                    if (sqlDr.Read())
                    {
                        totalqty = sqlDr["TOTAL_QTY"].ToString();
                    }
                    sqlDr.Close();

                    table12 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" +totalqty + "</div></td>";
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
                table14 += "<div>" + translation[8] + "</div><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + translation[15] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[12] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + translation[13] + "</div></td>";
                for (int m = 0; m < size.Count && m < 3; m++)
                    table14 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table14 += "</tr>";
                table13 += "<br /><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td>";
                for (int m = 3; m < size.Count; m++)
                    table13 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table13 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[14] + "</div></td><td style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.8cm'>TTL BY GO COLOR</div></td></tr>";
                foreach (DataRow dr in dt.Rows)
                {
                    string colortemp = dr[1].ToString().Length < 70 ? dr[1].ToString() : dr[1].ToString().Substring(0, 7);
                    //插入jo/color/size的qty
                    table14 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
                    for (int m = 0; m < size.Count && m < 3; m++)
                        table14 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                    table14 += "</tr>";
                    table13 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td>";
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
                string table14 = "", table13 = "", table20="";
                table14 += "<div>" + translation[8] + "</div><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + translation[15] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[12] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + translation[13] + "</div></td>";
                for (int m = 0; m < size.Count && m < 3; m++)
                    table14 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table14 += "</tr>";

                table13 += "<div>" + translation[8] + "</div><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + translation[15] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[12] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + translation[13] + "</div></td>";
                for (int m = 3; m < size.Count && m < 9; m++)
                    table13 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table13 += "</tr>";

                table20 += "<br /><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + translation[15] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[12] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + translation[13] + "</div></td>";
                for (int m = 9; m < size.Count && m < 13; m++)
                    table20 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table20 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[14] + "</div></td><td style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.8cm'>TTL BY GO COLOR</div></td></tr>";
                
                foreach (DataRow dr in dt.Rows)
                {
                    string colortemp = dr[1].ToString().Length < 70 ? dr[1].ToString() : dr[1].ToString().Substring(0, 7);
                    //插入jo/color/size的qty
                    table14 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
                    for (int m = 0; m < size.Count && m < 3; m++)
                        table14 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                    table14 += "</tr>";

                    //table13 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td>";
                      table13 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
                    for (int m = 3; m < size.Count && m < 9; m++)
                        table13 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                    table13 += "</tr>";

                    //table20 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td>";
                    table20 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
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

                table14 += "<div>" + translation[8] + "</div><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + translation[15] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[12] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + translation[13] + "</div></td>";
                for (int m = 0; m < size.Count && m < 3; m++)
                    table14 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table14 += "</tr>";

                table13 += "<div>" + translation[8] + "</div><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + translation[15] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[12] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + translation[13] + "</div></td>";
                for (int m = 3; m < size.Count && m < 9; m++)
                    table13 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table13 += "</tr>";

                table20 += "<div>" + translation[8] + "</div><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + translation[15] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[12] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + translation[13] + "</div></td>";
                for (int m = 9; m < size.Count && m < 13; m++)
                    table20 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table20 += "</tr>";

                table21 += "<br /><table style='font-size:10px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td>";
                for (int m = 13; m < size.Count; m++)
                    table21 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
                table21 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[14] + "</div></td><td style='font-size:8px;border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.8cm'>TTL BY GO COLOR</div></td></tr>";
                foreach (DataRow dr in dt.Rows)
                {
                    string colortemp = dr[1].ToString().Length < 70 ? dr[1].ToString() : dr[1].ToString().Substring(0, 7);
                    //插入jo/color/size的qty
                     table14 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
                    for (int m = 0; m < size.Count && m < 3; m++)
                        table14 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                    table14 += "</tr>";

                    //table13 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td>";
                      table13 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
                    for (int m = 3; m < size.Count && m < 9; m++)
                        table13 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                    table13 += "</tr>";

                    //table20 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td>";
                      table20 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
                    for (int m = 9; m < size.Count && m < 13; m++)
                        table20 += "<td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
                    table20 += "</tr>";

                    //table21 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td>";
                      table21 += "<tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
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
            table2 += "<br /><div name='hide1'>Summary BY Color</div><table name='hide2' style='font-size:11px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[9] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[5] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[15] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[16] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[17] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[18] + "</div></td></tr>";
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
                sqlDr = transfersql.getoutput2(sqlCon, factory, thisprocess, doc_no, jolist2[j], colorlist[j], partlist2[j]);
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
            table4 += "<br /><div name='hide3'>Summary BY JO</div><table name='hide4' style='font-size:11px;border-width:1px;border-style:dotted;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[9] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[5] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[15] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[16] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[17] + "</div></td><td style='border-width:1px;border-style:dotted;border-collapse:collapse;'><div>" + translation[18] + "</div></td></tr>";
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
                sqlDr = transfersql.getoutput(sqlCon, factory, thisprocess, doc_no, jolist1[j], partlist1[j]);
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

#region YMG 菲纸撤回(已更改菲纸格式)--禁用
    //public String Docnostring(SqlConnection sqlCon, string userbarcode, string doc_no, string factory, string thisprocess, string thisproduction, string nextprocess, string nextproduction, string languagearray, string automatching)
    //{
    //    string[] translation = languagearray.Split(new char[] { '*' });
    //    SqlDataReader sqlDr;
    //    Sqlstatement sqlstatement = new Sqlstatement();
    //    Transactionsql transfersql = new Transactionsql();
    //    Packagesql packagesql = new Packagesql();

    //    //找到用户的名字
    //    string username = "";
    //    sqlDr = sqlstatement.getname(sqlCon, userbarcode);
    //    if (sqlDr.Read())
    //        username = sqlDr["NAME"].ToString();
    //    sqlDr.Close();

    //    string result = "";
    //    //查询该流水单具有的部件
    //    sqlDr = sqlstatement.getdocnopart(sqlCon, doc_no);
    //    int i = 0;
    //    string parts = "";
    //    while (sqlDr.Read())
    //    {
    //        if (i == 0)
    //            parts += sqlDr["PART_DESC"].ToString();
    //        else
    //            parts += "/" + sqlDr["PART_DESC"].ToString();
    //        i++;
    //    }
    //    sqlDr.Close();

    //    int cartonnum = 0;
    //    sqlDr = sqlstatement.getcartonnum(sqlCon, doc_no);
    //    while (sqlDr.Read())
    //    {
    //        cartonnum++;
    //    }
    //    sqlDr.Close();

    //    //流水单第一个table3
    //    string table3 = "";
    //    table3 += "<div style='font-size:14px;width:99%;'>";
    //    table3 += "<div style='width:100%;height:125; display: inline-block;'>";
    //    table3 += "<div style='font-size:14px;font-weight:bolder;margin-bottom:10px;'>(" + factory + ") Garment Transfer Note     [" + thisprocess + "->" + nextprocess + "] R <br/></div>";
    //    table3 += "<div id='barcodediv' style='height:150;float:right;width:150;text-align:right;'></div>";
    //    table3 += "<div style='float:left; width:75%;'> ";
    //    table3 += "<table style='font-size:13px; width:100%;border-collapse:collapse;height:150;'>";
    //    table3 += "<tr>";
    //    table3 += "<td style='width:80px;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[10] + "</div></td>";
    //    table3 += "<td   colspan='3' style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + factory + doc_no + "</div></td>";
    //    table3 += "<td   style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[0] + "</div></td>";
    //    table3 += "<td  colspan='3' style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + username + "</div></td>";
    //    table3 += "</tr>";
    //    table3 += "<tr>";
    //    table3 += "<td style='width:31px;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[1] + "</div></td>";
    //    table3 += "<td style='width:132px;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + DateTime.Now.ToString() + "</div></td>";
    //    table3 += "<td style='width:57px;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[2] + "</div></td>";
    //    table3 += "<td style='width:0.9cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + thisprocess + "</div></td>";
    //    table3 += "<td style='width:57px;;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[3] + "</div></td>";
    //    table3 += "<td style='width:3.0cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + thisproduction + "</div></td>";
    //    table3 += "<td style='width:57px;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[4] + "</div></td>";
    //    table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + nextproduction + "</div></td>";
    //    table3 += "</tr>";
    //    table3 += "<tr>";
    //    table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[19] + "</div></td>";
    //    table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + cartonnum + "</div></td>";
    //    table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[20] + "</div></td>";
    //    table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + automatching + "</div></td>";
    //    table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[5] + "</div></td>";
    //    table3 += "<td  colspan='3' style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + parts + "</div></td>";
    //    table3 += "</tr>";
    //    table3 += "<tr>";
    //    table3 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;height:65px;'><div>" + translation[6] + "</div></td>";
    //    table3 += "<td colspan='7' style='width:1.5cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div></div></td>";
    //    table3 += "</tr>";
    //    table3 += "</table>";
    //    table3 += "</div></div>";
    //    table3 += "</div><br/><br/><br/>";



    //    //查找流水单有多少个GO
    //    List<string> golist = new List<string>();
    //    sqlDr = transfersql.gettransfergo(sqlCon, doc_no);
    //    while (sqlDr.Read())
    //    {
    //        golist.Add(sqlDr["GARMENT_ORDER_NO"].ToString());
    //    }
    //    sqlDr.Close();

    //    result = table3;

    //    //遍历所有GO，table2,table3
    //    foreach (string n in golist)
    //    {
    //        string customer = "";
    //        sqlDr = transfersql.getcustomer(sqlCon, n);
    //        if (sqlDr.Read())
    //            customer = sqlDr["SHORT_NAME"].ToString();
    //        sqlDr.Close();

    //        result += "<table style='font-size:14px; margin-top:15px;'><tr><td style='width:3.5cm;'><div>GO NO. " + n + "</div></td><td style='width:3.5cm;'><div>" + translation[7] + " " + customer + "</div></td>";
    //        //PRT 总扎数/菲数，箱号 --added on 2018-02-28
    //        //是否显示PRINT 部位明细--added on 2018-02-28
    //        if (thisprocess == "CUT" && nextprocess == "PRT")
    //        {
               
    //            SqlDataAdapter spAdapter = new SqlDataAdapter(transfersql.Getprintpartdetail(sqlCon, doc_no, n));
    //            DataTable dtprint = new DataTable();
    //            spAdapter.Fill(dtprint);
    //            if (dtprint != null) {
    //                if (dtprint.Rows.Count > 0)
    //                {
    //                        var printTable = "";
    //                        printTable += "<td ><div>" + translation[21] + ": " + dtprint.Rows.Count.ToString() + "</div></td>";//总扎数/菲数
                            
                           
    //                        printTable += "<td ><div>" + translation[22] + ": " +dtprint.Rows[0]["CAR_NO"].ToString() + " " + translation[30] + "</div></td>";//箱号 //车
    //                        printTable += "</tr></table><br />";


    //                        printTable += "<table style='font-size:13px; width:98%;border-collapse:collapse;height:150;'>";
    //                        printTable += "<tr><td  style='border-width:1px;border-style:solid;border-collapse:collapse; width:100px'><div>JO#</div></td>" +
    //                                      "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;width:120px'><div>" + translation[23] + "</div></td>" +//裁片菲码
    //                                      "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[24] + "</div></td>" +//幅位
    //                                      "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;width:120px'><div>" + translation[25] + "</div></td>" +//颜色
    //                                      "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[26] + "</div></td>" +//床次
    //                                      "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[27] + "</div></td>" +//扎号
    //                                      "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[28] + "</div></td>" +//尺码
    //                                      "<td  style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[29] + "</div></td></tr>";//数量
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

    //                        sqlDr = transfersql.Getprintpartsummary(sqlCon, doc_no,n);
    //                        if (sqlDr.Read())
    //                        {
    //                            printTable += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>TTL</div></td><td colspan='7' style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + sqlDr[0].ToString().TrimEnd('/') + "</div></td></tr>";
    //                        }
    //                        sqlDr.Close();

    //                        printTable += "</table><br/>";
    //                        result += printTable;
    //                    }
    //                }
           
    //        }
    //        else
    //        {
    //            result += "</tr></table><br />";
    //        }

    //        //------------------------------------------------------
    //        //JO/COLOR/SIZE的table
    //        //获取该GO箱内尺码的数量
    //        sqlDr = transfersql.getsize(sqlCon, doc_no, n);
    //        List<string> size = new List<string>();
    //        string sql = "";//拼写查询字符串
    //        while (sqlDr.Read())
    //        {
    //            size.Add(sqlDr["SIZE_CD"].ToString());
    //            sql += ",SUM(CASE SIZE_CD WHEN '" + sqlDr["SIZE_CD"].ToString() + "' THEN QTY ELSE 0 END) AS '" + sqlDr["SIZE_CD"].ToString() + "'";
    //        }
    //        sqlDr.Close();

    //        //得到结果集
    //        SqlDataAdapter sda = new SqlDataAdapter(transfersql.getjocolordb(sqlCon, sql, doc_no));
    //        DataTable dt = new DataTable();
    //        sda.Fill(dt);

    //        //循环结果集，构造箱码报表
    //        string colorrecord = "";
    //        string ttl = "0";
    //        if (size.Count < 2)
    //        {
    //            string table12 = "";
    //            table12 += "<div style='font-size:14px;'>" + translation[8] + "</div>";
    //            table12 += "<table style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + translation[15] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + translation[12] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + translation[13] + "</div></td>";
    //            foreach (string m in size) { table12 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px;;word-break: break-all'>" + m + "</div></td>"; }
    //            table12 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width100px'>" + translation[14] + "</div></td><td style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:150px'>TTL BY GO COLOR</div></td></tr>";
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                string colortemp = dr[1].ToString().Length < 70 ? dr[1].ToString() : dr[1].ToString().Substring(0, 7);
    //                //插入jo/color/size的qty
    //                table12 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
    //                for (int k = 0; k < size.Count; k++)
    //                    table12 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[5 + k] + "</div></td>";

    //                sqlDr = packagesql.gettotalqtydocno(sqlCon, dr[0].ToString(), dr[1].ToString(), doc_no);
    //                string totalqty = "";
    //                if (sqlDr.Read())
    //                {
    //                    totalqty = sqlDr["TOTAL_QTY"].ToString();
    //                }
    //                sqlDr.Close();

    //                table12 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" +totalqty + "</div></td>";
    //                if (colorrecord != dr[1].ToString())
    //                {
    //                    colorrecord = dr[1].ToString();
    //                    //获取该color的行数
    //                    string colornum = "";
    //                    sqlDr = transfersql.getcolornum(sqlCon, dr[1].ToString(), doc_no, n);
    //                    if (sqlDr.Read())
    //                        colornum = sqlDr["QTY"].ToString();
    //                    sqlDr.Close();
    //                    ttl = "0";
    //                    sqlDr = transfersql.getttlbygocolor(sqlCon, dr[1].ToString(), doc_no);
    //                    if (sqlDr.Read())
    //                        ttl = sqlDr["QTY"].ToString();
    //                    sqlDr.Close();
    //                    table12 += "<td rowspan='" + colornum + "' style='width:0.8cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
    //                }
    //                else
    //                    table12 += "</tr>";
    //            }
    //            table12 += "</table>";
    //            result += table12 + "</table>";
    //        }
    //        else if (size.Count >= 2 && size.Count <= 8)
    //        {
    //            //构建表头
    //            string table14 = "", table13 = "";
    //            table14 += "<div>" + translation[8] + "</div><table style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + translation[15] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + translation[12] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + translation[13] + "</div></td>";
    //            for (int m = 0; m < size.Count && m < 3; m++)
    //                table14 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px;word-break: break-all'>" + size[m] + "</div></td>";
    //            table14 += "</tr>";
    //            table13 += "<br /><table style='font-size:10px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td>";
    //            for (int m = 3; m < size.Count; m++)
    //                table13 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + size[m] + "</div></td>";
    //            table13 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + translation[14] + "</div></td><td style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:150px'>TTL BY GO COLOR</div></td></tr>";
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                string colortemp = dr[1].ToString().Length < 70 ? dr[1].ToString() : dr[1].ToString().Substring(0, 7);
    //                //插入jo/color/size的qty
    //                table14 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
    //                for (int m = 0; m < size.Count && m < 3; m++)
    //                    table14 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
    //                table14 += "</tr>";
    //                table13 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td>";
    //                for (int m = 3; m < size.Count; m++)
    //                    table13 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";

    //                sqlDr = packagesql.gettotalqtydocno(sqlCon, dr[0].ToString(), dr[1].ToString(), doc_no);
    //                string totalqty = "";
    //                if (sqlDr.Read())
    //                {
    //                    totalqty = sqlDr["TOTAL_QTY"].ToString();
    //                }
    //                sqlDr.Close();

    //                table13 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + totalqty + "</div></td>";
    //                if (colorrecord != dr[1].ToString())
    //                {
    //                    colorrecord = dr[1].ToString();
    //                    //获取该color的行数
    //                    string colornum = "";
    //                    sqlDr = transfersql.getcolornum(sqlCon, dr[1].ToString(), doc_no, n);
    //                    if (sqlDr.Read())
    //                        colornum = sqlDr["QTY"].ToString();
    //                    sqlDr.Close();
    //                    ttl = "0";
    //                    sqlDr = transfersql.getttlbygocolor(sqlCon, dr[1].ToString(), doc_no);
    //                    if (sqlDr.Read())
    //                        ttl = sqlDr["QTY"].ToString();
    //                    sqlDr.Close();
    //                    table13 += "<td rowspan='" + colornum + "' style='width:0.9cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
    //                }
    //                else
    //                    table13 += "</tr>";
    //            }
    //            table14 += "</table>";
    //            table13 += "</table>";
    //            result += table14 + table13 + "</table>";
    //        }
    //        else if (size.Count >= 9 && size.Count <= 12)
    //        {
    //            //构建表头
    //            string table14 = "", table13 = "", table20="";
    //            table14 += "<div>" + translation[8] + "</div><table style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + translation[15] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + translation[12] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + translation[13] + "</div></td>";
    //            for (int m = 0; m < size.Count && m < 3; m++)
    //                table14 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px;word-break: break-all'>" + size[m] + "</div></td>";
    //            table14 += "</tr>";

    //            table13 += "<div>" + translation[8] + "</div><table style='font-size:10px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.7cm'>" + translation[15] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[12] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.6cm'>" + translation[13] + "</div></td>";
    //            for (int m = 3; m < size.Count && m < 9; m++)
    //                table13 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px;word-break: break-all'>" + size[m] + "</div></td>";
    //            table13 += "</tr>";

    //            table20 += "<br /><table style='font-size:10px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.7cm'>" + translation[15] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[12] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.6cm'>" + translation[13] + "</div></td>";
    //            for (int m = 9; m < size.Count && m < 13; m++)
    //                table20 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px;word-break: break-all'>" + size[m] + "</div></td>";
    //            table20 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[14] + "</div></td><td style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:150px'>TTL BY GO COLOR</div></td></tr>";
                
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                string colortemp = dr[1].ToString().Length < 70 ? dr[1].ToString() : dr[1].ToString().Substring(0, 7);
    //                //插入jo/color/size的qty
    //                table14 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
    //                for (int m = 0; m < size.Count && m < 3; m++)
    //                    table14 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
    //                table14 += "</tr>";

    //                //table13 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td>";
    //                  table13 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
    //                for (int m = 3; m < size.Count && m < 9; m++)
    //                    table13 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
    //                table13 += "</tr>";

    //                //table20 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td>";
    //                table20 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
    //                for (int m = 9; m < size.Count && m < 13; m++)
    //                    table20 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";

    //                sqlDr = packagesql.gettotalqtydocno(sqlCon, dr[0].ToString(), dr[1].ToString(), doc_no);
    //                string totalqty = "";
    //                if (sqlDr.Read())
    //                {
    //                    totalqty = sqlDr["TOTAL_QTY"].ToString();
    //                }
    //                sqlDr.Close();

    //                table20 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + totalqty + "</div></td>";
    //                if (colorrecord != dr[1].ToString())
    //                {
    //                    colorrecord = dr[1].ToString();
    //                    //获取该color的行数
    //                    string colornum = "";
    //                    sqlDr = transfersql.getcolornum(sqlCon, dr[1].ToString(), doc_no, n);
    //                    if (sqlDr.Read())
    //                        colornum = sqlDr["QTY"].ToString();
    //                    sqlDr.Close();
    //                    ttl = "0";
    //                    sqlDr = transfersql.getttlbygocolor(sqlCon, dr[1].ToString(), doc_no);
    //                    if (sqlDr.Read())
    //                        ttl = sqlDr["QTY"].ToString();
    //                    sqlDr.Close();
    //                    table20 += "<td rowspan='" + colornum + "' style='width:0.9cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
    //                }
    //                else
    //                    table20 += "</tr>";
    //            }
    //            table14 += "</table>";
    //            table13 += "</table>";
    //            table20 += "</table>";
    //            result += table14 + table13 + table20 + "</table>";
    //        }
    //        else if (size.Count >= 13)
    //        {
    //            //构建表头
    //            string table14 = "", table13 = "", table20 = "", table21 = "";

    //            table14 += "<div>" + translation[8] + "</div><table style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px'>" + translation[15] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + translation[12] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:30px'>" + translation[13] + "</div></td>";
    //            for (int m = 0; m < size.Count && m < 3; m++)
    //                table14 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px;word-break: break-all'>" + size[m] + "</div></td>";
    //            table14 += "</tr>";

    //            table13 += "<div>" + translation[8] + "</div><table style='font-size:10px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.7cm'>" + translation[15] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[12] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.6cm'>" + translation[13] + "</div></td>";
    //            for (int m = 3; m < size.Count && m < 9; m++)
    //                table13 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px;word-break: break-all'>" + size[m] + "</div></td>";
    //            table13 += "</tr>";

    //            table20 += "<div>" + translation[8] + "</div><table style='font-size:10px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.7cm'>" + translation[15] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + translation[12] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.6cm'>" + translation[13] + "</div></td>";
    //            for (int m = 9; m < size.Count && m < 13; m++)
    //                table20 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px;word-break: break-all'>" + size[m] + "</div></td>";
    //            table20 += "</tr>";

    //            table21 += "<br /><table style='font-size:10px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td>";
    //            for (int m = 13; m < size.Count; m++)
    //                table21 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px;word-break: break-all'>" + size[m] + "</div></td>";
    //            table21 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:50px;word-break: break-all'>" + translation[14] + "</div></td><td style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:150px'>TTL BY GO COLOR</div></td></tr>";
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                string colortemp = dr[1].ToString().Length < 70 ? dr[1].ToString() : dr[1].ToString().Substring(0, 7);
    //                //插入jo/color/size的qty
    //                 table14 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
    //                for (int m = 0; m < size.Count && m < 3; m++)
    //                    table14 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
    //                table14 += "</tr>";

    //                //table13 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td>";
    //                  table13 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
    //                for (int m = 3; m < size.Count && m < 9; m++)
    //                    table13 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
    //                table13 += "</tr>";

    //                //table20 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td>";
    //                  table20 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
    //                for (int m = 9; m < size.Count && m < 13; m++)
    //                    table20 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";
    //                table20 += "</tr>";

    //                //table21 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td>";
    //                  table21 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.9cm'>" + dr[0] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + colortemp + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.7cm'>" + dr[4] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[2] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.6cm'>" + dr[3] + "</div></td>";
    //                for (int m = 13; m < size.Count; m++)
    //                    table21 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + dr[m + 5] + "</div></td>";

    //                sqlDr = packagesql.gettotalqtydocno(sqlCon, dr[0].ToString(), dr[1].ToString(), doc_no);
    //                string totalqty = "";
    //                if (sqlDr.Read())
    //                {
    //                    totalqty = sqlDr["TOTAL_QTY"].ToString();
    //                }
    //                sqlDr.Close();

    //                table21 += "<td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:0.5cm'>" + totalqty + "</div></td>";
    //                if (colorrecord != dr[1].ToString())
    //                {
    //                    colorrecord = dr[1].ToString();
    //                    //获取该color的行数
    //                    string colornum = "";
    //                    sqlDr = transfersql.getcolornum(sqlCon, dr[1].ToString(), doc_no, n);
    //                    if (sqlDr.Read())
    //                        colornum = sqlDr["QTY"].ToString();
    //                    sqlDr.Close();
    //                    ttl = "0";
    //                    sqlDr = transfersql.getttlbygocolor(sqlCon, dr[1].ToString(), doc_no);
    //                    if (sqlDr.Read())
    //                        ttl = sqlDr["QTY"].ToString();
    //                    sqlDr.Close();
    //                    table21 += "<td rowspan='" + colornum + "' style='width:0.9cm;border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + ttl + "</div></td></tr>";
    //                }
    //                else
    //                    table21 += "</tr>";
    //            }
    //            table14 += "</table>";
    //            table13 += "</table>";
    //            table20 += "</table>";
    //            table21 += "</table>";
    //            result += table14 + table13 + table20 + table21 + "</table>";
    //        }
    //        //------------------------------------------------------
    //        //table2，summary by color
    //        List<string> jolist2 = new List<string>();
    //        List<string> partlist2 = new List<string>();
    //        List<string> colorlist = new List<string>();
    //        List<string> partlistdesc = new List<string>();

    //        //查询该GO具有的JO/COLOR/PART
    //        sqlDr = transfersql.getjocolorpart(sqlCon, doc_no, n);
    //        while (sqlDr.Read())
    //        {
    //            jolist2.Add(sqlDr["JOB_ORDER_NO"].ToString());
    //            colorlist.Add(sqlDr["COLOR_CD"].ToString());
    //            partlist2.Add(sqlDr["PART_CD"].ToString());
    //            partlistdesc.Add(sqlDr["PART_DESC"].ToString());
    //        }
    //        sqlDr.Close();

    //        string table2 = "";
    //        table2 += "<br /><div name='hide1'>Summary BY Color</div><table name='hide2' style='font-size:14px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:1.2cm;word-break: break-all'>" + translation[11] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[5] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[15] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[16] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[17] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[18] + "</div></td></tr>";
    //        //循环查询该JO/COLOR/PART
    //        for (int j = 0; j < jolist2.Count; j++)
    //        {
    //            //获取实裁数
    //            string totalcutqty = "0";
    //            sqlDr = transfersql.getcutqty2(sqlCon, jolist2[j], colorlist[j], partlist2[j]);
    //            if (sqlDr.Read())
    //                if (sqlDr["QTY"].ToString() != "")
    //                    totalcutqty = sqlDr["QTY"].ToString();
    //            sqlDr.Close();
    //            //获取当前总计
    //            string totalnowqty = "0";
    //            sqlDr = transfersql.getnowqty2(sqlCon, doc_no, n, jolist2[j], colorlist[j], partlist2[j]);
    //            if (sqlDr.Read())
    //                if (sqlDr["QTY"].ToString() != "")
    //                    totalnowqty = sqlDr["QTY"].ToString();
    //            sqlDr.Close();
    //            //获取输出总计
    //            string totaloutput = "0";
    //            sqlDr = transfersql.getoutput2(sqlCon, factory, thisprocess, doc_no, jolist2[j], colorlist[j], partlist2[j]);
    //            if (sqlDr.Read())
    //                if (sqlDr["QTY"].ToString() != "")
    //                    totaloutput = sqlDr["QTY"].ToString();
    //            sqlDr.Close();
    //            //剩余数
    //            string restqty = (int.Parse(totalcutqty) - int.Parse(totaloutput)).ToString();
    //            //添加到summary by jo的table2里
    //            table2 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + jolist2[j] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + colorlist[j] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + partlistdesc[j] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + totalcutqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + totalnowqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + totaloutput + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + restqty + "</div></td></tr>";
    //        }
    //        result += table2 + "</table>";
    //        //------------------------------------------------------
    //        //table4: summary by jo
    //        List<string> jolist1 = new List<string>();
    //        List<string> partlist1 = new List<string>();
    //        List<string> partlistdesc1 = new List<string>();

    //        //查询该GO具有的JO/PART
    //        sqlDr = transfersql.getjopart(sqlCon, doc_no, n);
    //        while (sqlDr.Read())
    //        {
    //            jolist1.Add(sqlDr["JOB_ORDER_NO"].ToString());
    //            partlist1.Add(sqlDr["PART_CD"].ToString());
    //            partlistdesc1.Add(sqlDr["PART_DESC"].ToString());
    //        }
    //        sqlDr.Close();

    //        string table4 = "";
    //        table4 += "<br /><div name='hide3'>Summary BY JO</div><table name='hide4' style='font-size:11px;border-width:1px;border-style:solid;border-collapse:collapse;'><tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div style='width:100px'>" + translation[9] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[5] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[15] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[16] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[17] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + translation[18] + "</div></td></tr>";
    //        //循环查询该JO/PART
    //        for (int j = 0; j < jolist1.Count; j++)
    //        {
    //            //获取实裁数
    //            string totalcutqty = "0";
    //            sqlDr = transfersql.getcutqty(sqlCon, jolist1[j], partlist1[j]);
    //            if (sqlDr.Read())
    //                if (sqlDr["QTY"].ToString() != "")
    //                    totalcutqty = sqlDr["QTY"].ToString();
    //            sqlDr.Close();
    //            //获取当前总计
    //            string totalnowqty = "0";
    //            sqlDr = transfersql.getnowqty(sqlCon, doc_no, jolist1[j], partlist1[j]);
    //            if (sqlDr.Read())
    //                if (sqlDr["QTY"].ToString() != "")
    //                    totalnowqty = sqlDr["QTY"].ToString();
    //            sqlDr.Close();
    //            //获取输出总计
    //            string totaloutput = "0";
    //            sqlDr = transfersql.getoutput(sqlCon, factory, thisprocess, doc_no, jolist1[j], partlist1[j]);
    //            if (sqlDr.Read())
    //                if (sqlDr["QTY"].ToString() != "")
    //                    totaloutput = sqlDr["QTY"].ToString();
    //            sqlDr.Close();
    //            //剩余数
    //            string restqty = (int.Parse(totalcutqty) - int.Parse(totaloutput)).ToString();
    //            //添加到summary by jo的table3里
    //            table4 += "<tr><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + jolist1[j] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + partlistdesc1[j] + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + totalcutqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + totalnowqty + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + totaloutput + "</div></td><td style='border-width:1px;border-style:solid;border-collapse:collapse;'><div>" + restqty + "</div></td></tr>";
    //        }
    //        result += table4 + "</table>";
    //    }
    //    return result;
    //}
#endregion
}