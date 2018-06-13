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

public partial class Wipreport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod]
    public static String Process(string factory, string svTYPE)
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
            if (sqlDr["PRC_CD"].ToString() != "SEW")
            {
                if (sqlDr["PRC_CD"].ToString() == "DC")
                    result += "<option value='MATCHING'>MATCHING</option>";
                result += "<option value='" + sqlDr["PRC_CD"] + "'>" + sqlDr["NM"] + "</option>";
            }
        }
        return result;
    }

    [WebMethod]
    public static String Goquery(string factory, string svTYPE, string joborderno, string go, string process, string bybundle, string bypart, string bycolorsize)
    {
        string result = "[{ ";
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        Wipreportsql wipreportsql = new Wipreportsql();
        SqlDataReader sqlDr;
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();

        Boolean needttl = false;
        string table1 = "";
        string table2 = "";

        if (process == "all")
        {
            if (joborderno == "" && go == "")//查找各部门的所有JO的WIP和
            {
                sqlDr = wipreportsql.queryallprocess(sqlCon);
            }
            else
            {
                needttl = true;
                if (joborderno != "" && go != "")
                {
                    if (joborderno.IndexOf(go.Substring(1)) == -1)
                    {
                        sqlCon.Close();
                        return "false1";
                    }
                }
                if (joborderno != "")//by jo
                {
                    if (bypart == "true" && bybundle == "false" && bycolorsize == "false")
                    {
                        sqlDr = wipreportsql.queryjopart(sqlCon, joborderno);
                        //需要TTL
                    }
                    else if (bypart == "false" && bybundle == "true" && bycolorsize == "false")
                    {
                        sqlDr = wipreportsql.queryjobundle(sqlCon, joborderno);
                        //需要TTL
                    }
                    else if (bypart == "false" && bybundle == "false" && bycolorsize == "true")
                    {
                        sqlDr = wipreportsql.queryjocolorsize(sqlCon, joborderno);
                        //需要TTL
                    }
                    else if (bypart == "true" && bybundle == "true" && bycolorsize == "false")
                    {
                        sqlDr = wipreportsql.queryjobundlepart(sqlCon, joborderno);
                        //需要TTL
                    }
                    else if (bypart == "false" && bybundle == "true" && bycolorsize == "true")
                    {
                        sqlDr = wipreportsql.queryjobundlecolorsize(sqlCon, joborderno);
                        //需要TTL
                    }
                    else if (bypart == "true" && bybundle == "false" && bycolorsize == "true")
                    {
                        sqlDr = wipreportsql.queryjopartcolorsize(sqlCon, joborderno);
                        //需要TTL
                    }
                    else if (bypart == "true" && bybundle == "true" && bycolorsize == "true")
                    {
                        sqlDr = wipreportsql.queryjopartbundlecolorsize(sqlCon, joborderno);
                        //需要TTL
                    }
                    else
                    {
                        sqlDr = wipreportsql.queryjo(sqlCon, joborderno);
                        //需要TTL
                    }
                }
                else//by go
                {
                    if (bypart == "true" && bybundle == "false" && bycolorsize == "false")
                    {
                        sqlDr = wipreportsql.querygopart(sqlCon, go);
                        //需要TTL
                    }
                    else if (bypart == "false" && bybundle == "true" && bycolorsize == "false")
                    {
                        sqlDr = wipreportsql.querygobundle(sqlCon, go);
                        //需要TTL
                    }
                    else if (bypart == "false" && bybundle == "false" && bycolorsize == "true")
                    {
                        sqlDr = wipreportsql.querygocolorsize(sqlCon, go);
                        //需要TTL
                    }
                    else if (bypart == "true" && bybundle == "true" && bycolorsize == "false")
                    {
                        sqlDr = wipreportsql.querygobundlepart(sqlCon, go);
                        //需要TTL
                    }
                    else if (bypart == "false" && bybundle == "true" && bycolorsize == "true")
                    {
                        sqlDr = wipreportsql.querygobundlecolorsize(sqlCon, go);
                        //需要TTL
                    }
                    else if (bypart == "true" && bybundle == "false" && bycolorsize == "true")
                    {
                        sqlDr = wipreportsql.querygopartcolorsize(sqlCon, go);
                        //需要TTL
                    }
                    else if (bypart == "true" && bybundle == "true" && bycolorsize == "true")
                    {
                        sqlDr = wipreportsql.querygopartbundlecolorsize(sqlCon, go);
                        //需要TTL
                    }
                    else
                    {
                        sqlDr = wipreportsql.querygo(sqlCon, go);
                        //需要TTL
                    }
                }
            }
            string cutqty = "", embqty = "", prtqty = "", matchingqty = "", dcqty = "", tosewqty = "";
            int ttlcutqty = 0, ttlembqty = 0, ttlprtqty = 0, ttlmatchingqty = 0, ttldcqty = 0, ttltosewqty =0;
            int j = 0;
            string jo = "", color = "", size = "", bundle = "";
            while (sqlDr.Read())
            {
                if (sqlDr["CUT"].ToString() == "0")
                    cutqty = "";
                else
                    cutqty = sqlDr["CUT"].ToString();
                if (sqlDr["PRT"].ToString() == "0")
                    prtqty = "";
                else
                    prtqty = sqlDr["PRT"].ToString();
                if (sqlDr["EMB"].ToString() == "0")
                    embqty = "";
                else
                    embqty = sqlDr["EMB"].ToString();
                if (sqlDr["MATCHING"].ToString() == "0")
                    matchingqty = "";
                else
                    matchingqty = sqlDr["MATCHING"].ToString();
                if (sqlDr["DC"].ToString() == "0")
                    dcqty = "";
                else
                    dcqty = sqlDr["DC"].ToString();
                if (sqlDr["TOSEW"].ToString() == "0")
                    tosewqty = "";
                else
                    tosewqty = sqlDr["TOSEW"].ToString();
                ttlcutqty += int.Parse(sqlDr["CUT"].ToString());
                ttlprtqty += int.Parse(sqlDr["PRT"].ToString());
                ttlembqty += int.Parse(sqlDr["EMB"].ToString());
                ttlmatchingqty += int.Parse(sqlDr["MATCHING"].ToString());
                ttldcqty += int.Parse(sqlDr["DC"].ToString());
                ttltosewqty += int.Parse(sqlDr["TOSEW"].ToString());
                if (joborderno == "" && go == "")
                {
                    if (j % 2 == 1)
                        table1 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    else
                        table1 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:15%;'>" + cutqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:15%;'>" + prtqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:15%;'>" + embqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:15%;'>" + matchingqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:15%;'>" + dcqty + "</td></tr>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:15%;'>" + tosewqty + "</td></tr>";
                    j++;
                }
                else
                {
                    if (bybundle == "false" && bypart == "false" && bycolorsize == "false")
                    {
                        if (j % 2 == 1)
                            table1 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table1 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + cutqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + prtqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + embqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + matchingqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + dcqty + "</td></tr>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + tosewqty + "</td></tr>";
                        j++;
                    }
                    else if (bybundle == "true" && bypart == "false" && bycolorsize == "false")
                    {
                        if (j % 2 == 1)
                            table1 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table1 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + (j + 1) + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:20%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + sqlDr["BUNDLE_NO"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + cutqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + prtqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + embqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + matchingqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + dcqty + "</td></tr>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + tosewqty + "</td></tr>";
                        j++;
                    }
                    else if (bybundle == "false" && bypart == "true" && bycolorsize == "false")
                    {
                        if (j % 2 == 1)
                            table1 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table1 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + (j + 1) + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + cutqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + prtqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + embqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + matchingqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + dcqty + "</td></tr>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + tosewqty + "</td></tr>";
                        j++;
                    }
                    else if (bybundle == "false" && bypart == "false" && bycolorsize == "true")
                    {
                        if (j % 2 == 1)
                            table1 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table1 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + (j + 1) + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + cutqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + prtqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + embqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + matchingqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + dcqty + "</td></tr>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + tosewqty + "</td></tr>";
                        j++;
                    }
                    else if (bybundle == "true" && bypart == "true" && bycolorsize == "false")
                    {
                        if (bundle != sqlDr["BUNDLE_NO"].ToString())
                        {
                            bundle = sqlDr["BUNDLE_NO"].ToString();
                            if (j % 2 == 1)
                                table1 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table1 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + (j + 1) + "</td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + sqlDr["CUT_LINE"] + "</td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:19%;'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + sqlDr["BUNDLE_NO"] + "</td>";
                            j++;
                        }
                        else
                        {
                            if (j % 2 == 0)
                                table1 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table1 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'></td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'></td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:19%;'></td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'></td>";
                        }
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + cutqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + prtqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + embqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + matchingqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + dcqty + "</td></tr>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + tosewqty + "</td></tr>";
                    }
                    else if (bybundle == "false" && bypart == "true" && bycolorsize == "true")
                    {
                        if (color != sqlDr["COLOR_CD"].ToString() || size != sqlDr["SIZE_CD"].ToString())
                        {
                            color = sqlDr["COLOR_CD"].ToString();
                            size = sqlDr["SIZE_CD"].ToString();
                            if (j % 2 == 1)
                                table1 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table1 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + (j + 1) + "</td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:20%;'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["COLOR_CD"] + "</td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["SIZE_CD"] + "</td>";
                            j++;
                        }
                        else
                        {
                            if (j % 2 == 0)
                                table1 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table1 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:16%;'></td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                        }
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + cutqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + prtqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + embqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + matchingqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + dcqty + "</td></tr>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + tosewqty + "</td></tr>";
                    }
                    else if (bybundle == "true" && bypart == "false" && bycolorsize == "true")
                    {
                        if (j % 2 == 1)
                            table1 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table1 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + (j + 1) + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:12%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["BUNDLE_NO"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + cutqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + prtqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + embqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + matchingqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + dcqty + "</td></tr>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + tosewqty + "</td></tr>";
                        j++;
                    }
                    else
                    {
                        if (color != sqlDr["COLOR_CD"].ToString() || size != sqlDr["SIZE_CD"].ToString() || bundle != sqlDr["BUNDLE_NO"].ToString())
                        {
                            color = sqlDr["COLOR_CD"].ToString();
                            size = sqlDr["SIZE_CD"].ToString();
                            bundle = sqlDr["BUNDLE_NO"].ToString();
                            if (j % 2 == 1)
                                table1 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table1 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + (j + 1) + "</td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:12%;'>" + sqlDr["JOB_ORDER_NO"] + "</td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["COLOR_CD"] + "</td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["SIZE_CD"] + "</td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["BUNDLE_NO"].ToString() + "</td>";
                            j++;
                        }
                        else
                        {
                            if (j % 2 == 0)
                                table1 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table1 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:12%;'></td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                            table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                        }
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + cutqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + prtqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + embqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + matchingqty + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + dcqty + "</td></tr>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + tosewqty + "</td></tr>";
                    }
                }
            }
            if (needttl == true)
            {
                if (ttlcutqty == 0)
                    cutqty = "";
                else
                    cutqty = ttlcutqty.ToString();
                if (ttlprtqty == 0)
                    prtqty = "";
                else
                    prtqty = ttlprtqty.ToString();
                if (ttlembqty == 0)
                    embqty = "";
                else
                    embqty = ttlembqty.ToString();
                if (ttlmatchingqty == 0)
                    matchingqty = "";
                else
                    matchingqty = ttlmatchingqty.ToString();
                if (ttldcqty == 0)
                    dcqty = "";
                else
                    dcqty = ttldcqty.ToString();
                if (ttltosewqty == 0)
                    tosewqty = "";
                else
                    tosewqty = ttltosewqty.ToString();

                if (bybundle == "false" && bypart == "false" && bycolorsize == "false")
                {
                    table1 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:20%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:20%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + prtqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + embqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + matchingqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + dcqty + "</td></tr>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + tosewqty + "</td></tr>";
                }
                else if (bybundle == "true" && bypart == "false" && bycolorsize == "false")
                {
                    table1 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:20%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + prtqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + embqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + matchingqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + dcqty + "</td></tr>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + tosewqty + "</td></tr>";
                }
                else if (bybundle == "false" && bypart == "true" && bycolorsize == "false")
                {
                    table1 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:20%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + prtqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + embqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + matchingqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + dcqty + "</td></tr>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + tosewqty + "</td></tr>";
                }
                else if (bybundle == "false" && bypart == "false" && bycolorsize == "true")
                {
                    table1 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>TTL</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:19%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + cutqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + prtqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + embqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + matchingqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + dcqty + "</td></tr>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + tosewqty + "</td></tr>";
                }
                else if (bybundle == "true" && bypart == "true" && bycolorsize == "false")
                {
                    table1 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>TTL</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:19%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + cutqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + prtqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + embqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + matchingqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + dcqty + "</td></tr>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + tosewqty + "</td></tr>";
                }
                else if (bybundle == "false" && bypart == "true" && bycolorsize == "true")
                {
                    table1 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>TTL</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:20%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + cutqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + prtqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + embqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + matchingqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + dcqty + "</td></tr>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + tosewqty + "</td></tr>";
                }
                else if (bybundle == "true" && bypart == "false" && bycolorsize == "true")
                {
                    table1 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>TTL</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:20%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + cutqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + prtqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + embqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + matchingqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + dcqty + "</td></tr>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + tosewqty + "</td></tr>";
                }
                else
                {
                    table1 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>TTL</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:12%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'></td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + cutqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + prtqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + embqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + matchingqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + dcqty + "</td></tr>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + tosewqty + "</td></tr>";
                }
            }
            sqlDr.Close();
        }
        else
        {
            int ttlqty = 0;
            int j = 0;
            if (process != "DC" && process != "MATCHING")
            {
                sqlDr = wipreportsql.queryline(sqlCon, process);
                while (sqlDr.Read())
                {
                    if (j % 2 == 1)
                        table1 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    else
                        table1 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + process + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["PCS_WIP"].ToString() + "</td></tr>";
                    ttlqty += int.Parse(sqlDr["PCS_WIP"].ToString());
                    j++;
                }
                table1 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>TTL</td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'></td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + ttlqty + "</td></tr>";
                sqlDr.Close();
            }
            else
            {
                if (process == "MATCHING")
                    sqlDr = wipreportsql.querysewline(sqlCon, "DC");
                else
                    sqlDr = wipreportsql.querysewline(sqlCon, process);
                while (sqlDr.Read())
                {
                    if (j % 2 == 1)
                        table1 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    else
                        table1 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + process + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "</td>";
                    if (process != "MATCHING")
                    {
                        table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["AFTER_MATCHING_QTY"].ToString() + "</td></tr>";
                        ttlqty += int.Parse(sqlDr["AFTER_MATCHING_QTY"].ToString());
                    }
                    else
                    {
                        table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["BEFORE_MATCHING_QTY"].ToString() + "</td></tr>";
                        ttlqty += int.Parse(sqlDr["BEFORE_MATCHING_QTY"].ToString());
                    }
                    j++;
                }
                table1 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>TTL</td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'></td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + ttlqty + "</td></tr>";
                sqlDr.Close();
            }
        }
        result += "\"TABLE1\": \"" + table1 + "\", ";

        if (process != "all")
        {
            string orderqty = "", cutqty = "", wip = "", beforematchingqty = "", aftermatchingqty = "";
            string jo = "", color = "", size = "", bundle = "", cutline = ""; ;
            int ttlwip = 0;
            int j = 0;
            string tempprocess = process;
            if (tempprocess == "MATCHING")
                tempprocess = "DC";
            if (joborderno == "" && go == "")//查询所有JO
            {
                if (bybundle == "false" && bypart == "false" && bycolorsize == "false")
                {
                    if (tempprocess == "DC")
                        sqlDr = wipreportsql.queryoneprocess(sqlCon, tempprocess);
                    else
                        sqlDr = wipreportsql.queryoneprocess2(sqlCon, tempprocess);
                }
                else if (bybundle == "true" && bypart == "false" && bycolorsize == "false")
                {
                    if (tempprocess == "DC")
                        sqlDr = wipreportsql.queryoneprocessbundle(sqlCon, tempprocess);
                    else
                        sqlDr = wipreportsql.queryoneprocessbundle2(sqlCon, tempprocess);
                }
                else if (bybundle == "false" && bypart == "true" && bycolorsize == "false")
                {
                    if (tempprocess == "DC")
                        sqlDr = wipreportsql.queryoneprocesspart(sqlCon, tempprocess);
                    else
                        sqlDr = wipreportsql.queryoneprocesspart2(sqlCon, tempprocess);
                }
                else if (bybundle == "false" && bypart == "false" && bycolorsize == "true")
                {
                    if (tempprocess == "DC")
                        sqlDr = wipreportsql.queryoneprocesscolorsize(sqlCon, tempprocess);
                    else
                        sqlDr = wipreportsql.queryoneprocesscolorsize2(sqlCon, tempprocess);
                }
                else if (bybundle == "true" && bypart == "true" && bycolorsize == "false")
                {
                    if (tempprocess == "DC")
                        sqlDr = wipreportsql.queryoneprocessbundlepart(sqlCon, tempprocess);
                    else
                        sqlDr = wipreportsql.queryoneprocessbundlepart2(sqlCon, tempprocess);
                }
                else if (bybundle == "false" && bypart == "true" && bycolorsize == "true")
                {
                    if (tempprocess == "DC")
                        sqlDr = wipreportsql.queryoneprocesspartcolorsize(sqlCon, tempprocess);
                    else
                        sqlDr = wipreportsql.queryoneprocesspartcolorsize2(sqlCon, tempprocess);
                }
                else if (bybundle == "true" && bypart == "false" && bycolorsize == "true")
                {
                    if (tempprocess == "DC")
                        sqlDr = wipreportsql.queryoneprocessbundlecolorsize(sqlCon, tempprocess);
                    else
                        sqlDr = wipreportsql.queryoneprocessbundlecolorsize2(sqlCon, tempprocess);
                }
                else
                {
                    if (tempprocess == "DC")
                        sqlDr = wipreportsql.queryoneprocessbundlepartcolorsize(sqlCon, tempprocess);
                    else
                        sqlDr = wipreportsql.queryoneprocessbundlepartcolorsize2(sqlCon, tempprocess);
                }
            }
            else
            {
                if (joborderno != "" && go != "")
                {
                    if (joborderno.IndexOf(go.Substring(1)) == -1)
                    {
                        sqlCon.Close();
                        return "false1";
                    }
                }
                if (joborderno != "")//by jo
                {
                    if (bybundle == "false" && bypart == "false" && bycolorsize == "false")
                    {
                        if (tempprocess == "DC")
                            sqlDr = wipreportsql.queryoneprocessjo(sqlCon, tempprocess, joborderno);
                        else
                            sqlDr = wipreportsql.queryoneprocessjo2(sqlCon, tempprocess, joborderno);
                    }
                    else if (bybundle == "true" && bypart == "false" && bycolorsize == "false")
                    {
                        if (tempprocess == "DC")
                            sqlDr = wipreportsql.queryoneprocessbundle(sqlCon, tempprocess, joborderno);
                        else
                            sqlDr = wipreportsql.queryoneprocessbundle2(sqlCon, tempprocess, joborderno);
                    }
                    else if (bybundle == "false" && bypart == "true" && bycolorsize == "false")
                    {
                        if (tempprocess == "DC")
                            sqlDr = wipreportsql.queryoneprocesspart(sqlCon, tempprocess, joborderno);
                        else
                            sqlDr = wipreportsql.queryoneprocesspart2(sqlCon, tempprocess, joborderno);
                    }
                    else if (bybundle == "false" && bypart == "false" && bycolorsize == "true")
                    {
                        if (tempprocess == "DC")
                            sqlDr = wipreportsql.queryoneprocesscolorsize(sqlCon, tempprocess, joborderno);
                        else
                            sqlDr = wipreportsql.queryoneprocesscolorsize2(sqlCon, tempprocess, joborderno);
                    }
                    else if (bybundle == "true" && bypart == "true" && bycolorsize == "false")
                    {
                        if (tempprocess == "DC")
                            sqlDr = wipreportsql.queryoneprocessbundlepart(sqlCon, tempprocess, joborderno);
                        else
                            sqlDr = wipreportsql.queryoneprocessbundlepart2(sqlCon, tempprocess, joborderno);
                    }
                    else if (bybundle == "false" && bypart == "true" && bycolorsize == "true")
                    {
                        if (tempprocess == "DC")
                            sqlDr = wipreportsql.queryoneprocesspartcolorsize(sqlCon, tempprocess, joborderno);
                        else
                            sqlDr = wipreportsql.queryoneprocesspartcolorsize2(sqlCon, tempprocess, joborderno);
                    }
                    else if (bybundle == "true" && bypart == "false" && bycolorsize == "true")
                    {
                        if (tempprocess == "DC")
                            sqlDr = wipreportsql.queryoneprocessbundlecolorsize(sqlCon, tempprocess, joborderno);
                        else
                            sqlDr = wipreportsql.queryoneprocessbundlecolorsize2(sqlCon, tempprocess, joborderno);
                    }
                    else
                    {
                        if (tempprocess == "DC")
                            sqlDr = wipreportsql.queryoneprocessbundlepartcolorsize(sqlCon, tempprocess, joborderno);
                        else
                            sqlDr = wipreportsql.queryoneprocessbundlepartcolorsize2(sqlCon, tempprocess, joborderno);
                    }
                }
                else//by go
                {
                    if (bybundle == "false" && bypart == "false" && bycolorsize == "false")
                    {
                        if (tempprocess == "DC")
                            sqlDr = wipreportsql.queryoneprocessgo(sqlCon, tempprocess, go);
                        else
                            sqlDr = wipreportsql.queryoneprocessgo2(sqlCon, tempprocess, go);
                    }
                    else if (bybundle == "true" && bypart == "false" && bycolorsize == "false")
                    {
                        if (tempprocess == "DC")
                            sqlDr = wipreportsql.queryoneprocessbundlego(sqlCon, tempprocess, go);
                        else
                            sqlDr = wipreportsql.queryoneprocessbundlego2(sqlCon, tempprocess, go);
                    }
                    else if (bybundle == "false" && bypart == "true" && bycolorsize == "false")
                    {
                        if (tempprocess == "DC")
                            sqlDr = wipreportsql.queryoneprocesspartgo(sqlCon, tempprocess, go);
                        else
                            sqlDr = wipreportsql.queryoneprocesspartgo2(sqlCon, tempprocess, go);
                    }
                    else if (bybundle == "false" && bypart == "false" && bycolorsize == "true")
                    {
                        if (tempprocess == "DC")
                            sqlDr = wipreportsql.queryoneprocesscolorsizego(sqlCon, tempprocess, go);
                        else
                            sqlDr = wipreportsql.queryoneprocesscolorsizego2(sqlCon, tempprocess, go);
                    }
                    else if (bybundle == "true" && bypart == "true" && bycolorsize == "false")
                    {
                        if (tempprocess == "DC")
                            sqlDr = wipreportsql.queryoneprocessbundlepartgo(sqlCon, tempprocess, go);
                        else
                            sqlDr = wipreportsql.queryoneprocessbundlepartgo2(sqlCon, tempprocess, go);
                    }
                    else if (bybundle == "false" && bypart == "true" && bycolorsize == "true")
                    {
                        if (tempprocess == "DC")
                            sqlDr = wipreportsql.queryoneprocesspartcolorsizego(sqlCon, tempprocess, go);
                        else
                            sqlDr = wipreportsql.queryoneprocesspartcolorsizego2(sqlCon, tempprocess, go);
                    }
                    else if (bybundle == "true" && bypart == "false" && bycolorsize == "true")
                    {
                        if (tempprocess == "DC")
                            sqlDr = wipreportsql.queryoneprocessbundlecolorsizego(sqlCon, tempprocess, go);
                        else
                            sqlDr = wipreportsql.queryoneprocessbundlecolorsizego2(sqlCon, tempprocess, go);
                    }
                    else
                    {
                        if (tempprocess == "DC")
                            sqlDr = wipreportsql.queryoneprocessbundlepartcolorsizego(sqlCon, tempprocess, go);
                        else
                            sqlDr = wipreportsql.queryoneprocessbundlepartcolorsizego2(sqlCon, tempprocess, go);
                    }
                }
            }

            if (process == "MATCHING")
            {
                if (bybundle == "false" && bypart == "false" && bycolorsize == "false")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["BEFORE_MATCHING_QTY"].ToString() == "0")
                        {
                            wip = "";
                            continue;
                        }
                        else
                            wip = sqlDr["BEFORE_MATCHING_QTY"].ToString();
                        ttlwip += int.Parse(sqlDr["BEFORE_MATCHING_QTY"].ToString());
                        if (j % 2 == 1)
                            table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + wip + "</td></tr>";
                        j++;
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "true" && bypart == "false" && bycolorsize == "false")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["BEFORE_MATCHING_QTY"].ToString() == "0")
                        {
                            wip = "";
                            continue;
                        }
                        else
                            wip = sqlDr["BEFORE_MATCHING_QTY"].ToString();
                        ttlwip += int.Parse(sqlDr["BEFORE_MATCHING_QTY"].ToString());
                        if (j % 2 == 1)
                            table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:40%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["BUNDLE_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                        j++;
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:40%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "false" && bypart == "true" && bycolorsize == "false")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["BEFORE_MATCHING_QTY"].ToString() == "0")
                        {
                            wip = "";
                            continue;
                        }
                        else
                            wip = sqlDr["BEFORE_MATCHING_QTY"].ToString();
                        ttlwip += int.Parse(sqlDr["BEFORE_MATCHING_QTY"].ToString());
                        if (j % 2 == 1)
                            table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:40%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                        j++;
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:40%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "false" && bypart == "false" && bycolorsize == "true")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["BEFORE_MATCHING_QTY"].ToString() == "0")
                        {
                            wip = "";
                            continue;
                        }
                        else
                            wip = sqlDr["BEFORE_MATCHING_QTY"].ToString();
                        ttlwip += int.Parse(sqlDr["BEFORE_MATCHING_QTY"].ToString());
                        if (j % 2 == 1)
                            table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:30%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                        j++;
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:30%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "true" && bypart == "true" && bycolorsize == "false")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["BEFORE_MATCHING_QTY"].ToString() == "0")
                        {
                            wip = "";
                            continue;
                        }
                        else
                            wip = sqlDr["BEFORE_MATCHING_QTY"].ToString();
                        ttlwip += int.Parse(sqlDr["BEFORE_MATCHING_QTY"].ToString());

                        if (cutline != sqlDr["PRODUCTION_LINE_CD"].ToString() || jo != sqlDr["JOB_ORDER_NO"].ToString() || bundle != sqlDr["BUNDLE_NO"].ToString())
                        {
                            cutline = sqlDr["PRODUCTION_LINE_CD"].ToString();
                            jo = sqlDr["JOB_ORDER_NO"].ToString();
                            bundle = sqlDr["BUNDLE_NO"].ToString();
                            if (j % 2 == 1)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:30%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["BUNDLE_NO"].ToString() + "</td>";
                            j++;
                        }
                        else
                        {
                            if (j % 2 == 0)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:30%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                        }

                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:30%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "false" && bypart == "true" && bycolorsize == "true")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["BEFORE_MATCHING_QTY"].ToString() == "0")
                        {
                            wip = "";
                            continue;
                        }
                        else
                            wip = sqlDr["BEFORE_MATCHING_QTY"].ToString();
                        ttlwip += int.Parse(sqlDr["BEFORE_MATCHING_QTY"].ToString());

                        if (cutline != sqlDr["PRODUCTION_LINE_CD"].ToString() || jo != sqlDr["JOB_ORDER_NO"].ToString() || color != sqlDr["COLOR_CD"].ToString() || size != sqlDr["SIZE_CD"].ToString())
                        {
                            cutline = sqlDr["PRODUCTION_LINE_CD"].ToString();
                            jo = sqlDr["JOB_ORDER_NO"].ToString();
                            color = sqlDr["COLOR_CD"].ToString();
                            size = sqlDr["SIZE_CD"].ToString();
                            if (j % 2 == 1)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:20%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
                            j++;
                        }
                        else
                        {
                            if (j % 2 == 0)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:20%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                        }

                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:20%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "true" && bypart == "false" && bycolorsize == "true")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["BEFORE_MATCHING_QTY"].ToString() == "0")
                        {
                            wip = "";
                            continue;
                        }
                        else
                            wip = sqlDr["BEFORE_MATCHING_QTY"].ToString();
                        ttlwip += int.Parse(sqlDr["BEFORE_MATCHING_QTY"].ToString());

                        if (j % 2 == 1)
                            table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:20%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["BUNDLE_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                        j++;
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:20%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["BEFORE_MATCHING_QTY"].ToString() == "0")
                        {
                            wip = "";
                            continue;
                        }
                        else
                            wip = sqlDr["BEFORE_MATCHING_QTY"].ToString();
                        ttlwip += int.Parse(sqlDr["BEFORE_MATCHING_QTY"].ToString());

                        if (cutline != sqlDr["PRODUCTION_LINE_CD"].ToString() || jo != sqlDr["JOB_ORDER_NO"].ToString() || color != sqlDr["COLOR_CD"].ToString() || size != sqlDr["SIZE_CD"].ToString() || bundle != sqlDr["BUNDLE_NO"].ToString())
                        {
                            cutline = sqlDr["PRODUCTION_LINE_CD"].ToString();
                            jo = sqlDr["JOB_ORDER_NO"].ToString();
                            color = sqlDr["COLOR_CD"].ToString();
                            size = sqlDr["SIZE_CD"].ToString();
                            bundle = sqlDr["BUNDLE_NO"].ToString();
                            if (j % 2 == 1)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["BUNDLE_NO"].ToString() + "</td>";
                            j++;
                        }
                        else
                        {
                            if (j % 2 == 0)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                        }
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
            }
            else if (process == "DC")
            {
                if (bybundle == "false" && bypart == "false" && bycolorsize == "false")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["AFTER_MATCHING_QTY"].ToString() == "0")
                        {
                            wip = "";
                            continue;
                        }
                        else
                            wip = sqlDr["AFTER_MATCHING_QTY"].ToString();
                        ttlwip += int.Parse(sqlDr["AFTER_MATCHING_QTY"].ToString());
                        if (j % 2 == 1)
                            table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + wip + "</td></tr>";
                        j++;
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "true" && bypart == "false" && bycolorsize == "false")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["AFTER_MATCHING_QTY"].ToString() == "0")
                        {
                            wip = "";
                            continue;
                        }
                        else
                            wip = sqlDr["AFTER_MATCHING_QTY"].ToString();
                        ttlwip += int.Parse(sqlDr["AFTER_MATCHING_QTY"].ToString());
                        if (j % 2 == 1)
                            table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:40%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["BUNDLE_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                        j++;
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:40%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "false" && bypart == "true" && bycolorsize == "false")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["AFTER_MATCHING_QTY"].ToString() == "0")
                        {
                            wip = "";
                            continue;
                        }
                        else
                            wip = sqlDr["AFTER_MATCHING_QTY"].ToString();
                        ttlwip += int.Parse(sqlDr["AFTER_MATCHING_QTY"].ToString());
                        if (j % 2 == 1)
                            table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:40%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                        j++;
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:40%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "false" && bypart == "false" && bycolorsize == "true")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["AFTER_MATCHING_QTY"].ToString() == "0")
                        {
                            wip = "";
                            continue;
                        }
                        else
                            wip = sqlDr["AFTER_MATCHING_QTY"].ToString();
                        ttlwip += int.Parse(sqlDr["AFTER_MATCHING_QTY"].ToString());
                        if (j % 2 == 1)
                            table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:30%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                        j++;
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:30%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "true" && bypart == "true" && bycolorsize == "false")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["AFTER_MATCHING_QTY"].ToString() == "0")
                        {
                            wip = "";
                            continue;
                        }
                        else
                            wip = sqlDr["AFTER_MATCHING_QTY"].ToString();
                        ttlwip += int.Parse(sqlDr["AFTER_MATCHING_QTY"].ToString());

                        if (cutline != sqlDr["PRODUCTION_LINE_CD"].ToString() || jo != sqlDr["JOB_ORDER_NO"].ToString() || bundle != sqlDr["BUNDLE_NO"].ToString())
                        {
                            cutline = sqlDr["PRODUCTION_LINE_CD"].ToString();
                            jo = sqlDr["JOB_ORDER_NO"].ToString();
                            bundle = sqlDr["BUNDLE_NO"].ToString();
                            if (j % 2 == 1)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:30%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["BUNDLE_NO"].ToString() + "</td>";
                            j++;
                        }
                        else
                        {
                            if (j % 2 == 0)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:30%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                        }

                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:30%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "false" && bypart == "true" && bycolorsize == "true")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["AFTER_MATCHING_QTY"].ToString() == "0")
                        {
                            wip = "";
                            continue;
                        }
                        else
                            wip = sqlDr["AFTER_MATCHING_QTY"].ToString();
                        ttlwip += int.Parse(sqlDr["AFTER_MATCHING_QTY"].ToString());

                        if (cutline != sqlDr["PRODUCTION_LINE_CD"].ToString() || jo != sqlDr["JOB_ORDER_NO"].ToString() || color != sqlDr["COLOR_CD"].ToString() || size != sqlDr["SIZE_CD"].ToString())
                        {
                            cutline = sqlDr["PRODUCTION_LINE_CD"].ToString();
                            jo = sqlDr["JOB_ORDER_NO"].ToString();
                            color = sqlDr["COLOR_CD"].ToString();
                            size = sqlDr["SIZE_CD"].ToString();
                            if (j % 2 == 1)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:20%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
                            j++;
                        }
                        else
                        {
                            if (j % 2 == 0)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:20%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                        }

                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:20%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "true" && bypart == "false" && bycolorsize == "true")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["AFTER_MATCHING_QTY"].ToString() == "0")
                        {
                            wip = "";
                            continue;
                        }
                        else
                            wip = sqlDr["AFTER_MATCHING_QTY"].ToString();
                        ttlwip += int.Parse(sqlDr["AFTER_MATCHING_QTY"].ToString());

                        if (j % 2 == 1)
                            table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:20%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["BUNDLE_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                        j++;
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:20%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["AFTER_MATCHING_QTY"].ToString() == "0")
                        {
                            wip = "";
                            continue;
                        }
                        else
                            wip = sqlDr["AFTER_MATCHING_QTY"].ToString();
                        ttlwip += int.Parse(sqlDr["AFTER_MATCHING_QTY"].ToString());

                        if (cutline != sqlDr["PRODUCTION_LINE_CD"].ToString() || jo != sqlDr["JOB_ORDER_NO"].ToString() || color != sqlDr["COLOR_CD"].ToString() || size != sqlDr["SIZE_CD"].ToString() || bundle != sqlDr["BUNDLE_NO"].ToString())
                        {
                            cutline = sqlDr["PRODUCTION_LINE_CD"].ToString();
                            jo = sqlDr["JOB_ORDER_NO"].ToString();
                            color = sqlDr["COLOR_CD"].ToString();
                            size = sqlDr["SIZE_CD"].ToString();
                            bundle = sqlDr["BUNDLE_NO"].ToString();
                            if (j % 2 == 1)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PRODUCTION_LINE_CD"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["BUNDLE_NO"].ToString() + "</td>";
                            j++;
                        }
                        else
                        {
                            if (j % 2 == 0)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                        }
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
            }
            else
            {
                if (bybundle == "false" && bypart == "false" && bycolorsize == "false")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["PCS_WIP"].ToString() == "0")
                            wip = "";
                        else
                            wip = sqlDr["PCS_WIP"].ToString();
                        ttlwip += int.Parse(sqlDr["PCS_WIP"].ToString());
                        if (j % 2 == 1)
                            table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + wip + "</td></tr>";
                        j++;
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "true" && bypart == "false" && bycolorsize == "false")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["PCS_WIP"].ToString() == "0")
                            wip = "";
                        else
                            wip = sqlDr["PCS_WIP"].ToString();
                        ttlwip += int.Parse(sqlDr["PCS_WIP"].ToString());
                        if (j % 2 == 1)
                            table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:40%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["BUNDLE_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                        j++;
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:40%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "false" && bypart == "true" && bycolorsize == "false")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["PCS_WIP"].ToString() == "0")
                            wip = "";
                        else
                            wip = sqlDr["PCS_WIP"].ToString();
                        ttlwip += int.Parse(sqlDr["PCS_WIP"].ToString());
                        if (j % 2 == 1)
                            table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:40%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                        j++;
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:40%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "false" && bypart == "false" && bycolorsize == "true")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["PCS_WIP"].ToString() == "0")
                            wip = "";
                        else
                            wip = sqlDr["PCS_WIP"].ToString();
                        ttlwip += int.Parse(sqlDr["PCS_WIP"].ToString());
                        if (j % 2 == 1)
                            table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:30%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                        j++;
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:30%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "true" && bypart == "true" && bycolorsize == "false")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["WIP"].ToString() == "0")
                            wip = "";
                        else
                            wip = sqlDr["WIP"].ToString();
                        ttlwip += int.Parse(sqlDr["WIP"].ToString());

                        if (cutline != sqlDr["CUT_LINE"].ToString() || jo != sqlDr["JOB_ORDER_NO"].ToString() || bundle != sqlDr["BUNDLE_NO"].ToString())
                        {
                            cutline = sqlDr["CUT_LINE"].ToString();
                            jo = sqlDr["JOB_ORDER_NO"].ToString();
                            bundle = sqlDr["BUNDLE_NO"].ToString();
                            if (j % 2 == 1)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:30%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["BUNDLE_NO"].ToString() + "</td>";
                            j++;
                        }
                        else
                        {
                            if (j % 2 == 0)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:30%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                        }

                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:30%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "false" && bypart == "true" && bycolorsize == "true")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["PCS_WIP"].ToString() == "0")
                            wip = "";
                        else
                            wip = sqlDr["PCS_WIP"].ToString();
                        ttlwip += int.Parse(sqlDr["PCS_WIP"].ToString());

                        if (cutline != sqlDr["CUT_LINE"].ToString() || jo != sqlDr["JOB_ORDER_NO"].ToString() || color != sqlDr["COLOR_CD"].ToString() || size != sqlDr["SIZE_CD"].ToString())
                        {
                            cutline = sqlDr["CUT_LINE"].ToString();
                            jo = sqlDr["JOB_ORDER_NO"].ToString();
                            color = sqlDr["COLOR_CD"].ToString();
                            size = sqlDr["SIZE_CD"].ToString();
                            if (j % 2 == 1)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:20%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
                            j++;
                        }
                        else
                        {
                            if (j % 2 == 0)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:20%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                        }

                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:20%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else if (bybundle == "true" && bypart == "false" && bycolorsize == "true")
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["PCS_WIP"].ToString() == "0")
                            wip = "";
                        else
                            wip = sqlDr["PCS_WIP"].ToString();
                        ttlwip += int.Parse(sqlDr["PCS_WIP"].ToString());

                        if (j % 2 == 1)
                            table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:20%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["BUNDLE_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                        j++;
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:20%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
                else
                {
                    while (sqlDr.Read())
                    {
                        if (sqlDr["ORDER_QTY"].ToString() == "0")
                            orderqty = "";
                        else
                            orderqty = sqlDr["ORDER_QTY"].ToString();
                        if (sqlDr["CUT_QTY"].ToString() == "0")
                            cutqty = "";
                        else
                            cutqty = sqlDr["CUT_QTY"].ToString();
                        if (sqlDr["WIP"].ToString() == "0")
                            wip = "";
                        else
                            wip = sqlDr["WIP"].ToString();
                        ttlwip += int.Parse(sqlDr["WIP"].ToString());

                        if (cutline != sqlDr["CUT_LINE"].ToString() || jo != sqlDr["JOB_ORDER_NO"].ToString() || color != sqlDr["COLOR_CD"].ToString() || size != sqlDr["SIZE_CD"].ToString() || bundle != sqlDr["BUNDLE_NO"].ToString())
                        {
                            cutline = sqlDr["CUT_LINE"].ToString();
                            jo = sqlDr["JOB_ORDER_NO"].ToString();
                            color = sqlDr["COLOR_CD"].ToString();
                            size = sqlDr["SIZE_CD"].ToString();
                            bundle = sqlDr["BUNDLE_NO"].ToString();
                            if (j % 2 == 1)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + (j + 1) + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["BUNDLE_NO"].ToString() + "</td>";
                            j++;
                        }
                        else
                        {
                            if (j % 2 == 0)
                                table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            else
                                table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                            table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                        }
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + orderqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + cutqty + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + wip + "</td></tr>";
                    }
                    sqlDr.Close();
                    table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'></td>";
                    table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + ttlwip + "</td></tr>";
                }
            }
        }

        result += "\"TABLE2\": \"" + table2 + "\" }]";
        sqlCon.Close();
        return result;
    }





    [WebMethod]
    public static String GetProcessList(string factory, string svTYPE)
    {
        GetWIPReportBLL getwipreportbll = new GetWIPReportBLL();
        return getwipreportbll.GetProcessList(factory, svTYPE);
    }

    [WebMethod]
    public static String GetWIPReport(string factory, string svTYPE, string go, string jo, string process, string bybundle, string bypart, string bycolor, string bysize, string bysewline, string byprocesspcs)
    {
        GetWIPReportBLL getwipreportbll = new GetWIPReportBLL();
        return getwipreportbll.GetWIPReport(factory, svTYPE, go, jo, process, bybundle, bypart, bycolor, bysize,bysewline,byprocesspcs);
    }

    [WebMethod]
    public static String GetWIPReportByProcess(string factory, string svTYPE, string go, string jo, string process, string bybundle, string bypart, string bycolor, string bysize, string bysewline, string byprocesspcs)
    {
        GetWIPReportBLL getwipreportbll = new GetWIPReportBLL();
        return getwipreportbll.GetWIPReportByProcess(factory, svTYPE, go, jo, process, bybundle, bypart, bycolor, bysize, bysewline, byprocesspcs);
    }
}
