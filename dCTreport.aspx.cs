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

public partial class dCTreport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod]
    public static String GetdCTtabledate(string factory, string svTYPE, string fromdate, string todate)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        dCTreportsql dctreportsql = new dCTreportsql();
        SqlDataReader sqlDr;
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        string html = "";
        if (fromdate == todate)
            todate += "23:59:59";
        sqlDr = dctreportsql.getqty(sqlCon, fromdate, todate, factory, "I");
        string dctcut="",dctprt="",dctemb="",dctmatching="",dctdc="";
        if (sqlDr.Read())
        {
            if (sqlDr["OUT_QTY_CUT"].ToString() == "0")
                dctcut = "0";
            else
                dctcut = ((int.Parse(sqlDr["OPENING_QTY_CUT"].ToString()) + int.Parse(sqlDr["END_WIP_CUT"].ToString())) / 2 / int.Parse(sqlDr["OUT_QTY_CUT"].ToString())).ToString();
            if (sqlDr["OUT_QTY_PRT"].ToString() == "0")
                dctcut = "0";
            else
                dctcut = ((int.Parse(sqlDr["OPENING_QTY_PRT"].ToString()) + int.Parse(sqlDr["END_WIP_PRT"].ToString())) / 2 / int.Parse(sqlDr["OUT_QTY_PRT"].ToString())).ToString();
            if (sqlDr["OUT_QTY_EMB"].ToString() == "0")
                dctcut = "0";
            else
                dctcut = ((int.Parse(sqlDr["OPENING_QTY_EMB"].ToString()) + int.Parse(sqlDr["END_WIP_EMB"].ToString())) / 2 / int.Parse(sqlDr["OUT_QTY_EMB"].ToString())).ToString();
            if (sqlDr["OUT_QTY_MATCHING"].ToString() == "0")
                dctcut = "0";
            else
                dctcut = ((int.Parse(sqlDr["OPENING_QTY_MATCHING"].ToString()) + int.Parse(sqlDr["END_WIP_MATCHING"].ToString())) / 2 / int.Parse(sqlDr["OUT_QTY_MATCHING"].ToString())).ToString();
            if (sqlDr["OUT_QTY_DC"].ToString() == "0")
                dctcut = "0";
            else
                dctcut = ((int.Parse(sqlDr["OPENING_QTY_DC"].ToString()) + int.Parse(sqlDr["END_WIP_DC"].ToString())) / 2 / int.Parse(sqlDr["OUT_QTY_DC"].ToString())).ToString();
            html += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
            html += "<td style='vertical-align:middle; text-align:center; width:20%'>" + dctcut + "</td>";
            html += "<td style='vertical-align:middle; text-align:center; width:20%'>" + dctprt + "</td>";
            html += "<td style='vertical-align:middle; text-align:center; width:20%'>" + dctemb + "</td>";
            html += "<td style='vertical-align:middle; text-align:center; width:20%'>" + dctmatching + "</td>";
            html += "<td style='vertical-align:middle; text-align:center; width:20%'>" + dctdc + "</td></tr>";
        }
        sqlDr.Close();
        sqlCon.Close();

        return html;
    }
}