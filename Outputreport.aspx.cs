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

public partial class Outputreport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod]
    public static String getnextprocess(string factory, string svTYPE, string process, string flowtype)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Transactionsql sqlstatement = new Transactionsql();
        SqlDataReader sqlDr = sqlstatement.nextprocess(sqlCon, factory, process, flowtype);

        string result = "";
        while (sqlDr.Read())
        {
            string test = sqlDr["PRC_CD"].ToString();
            if (test != process)
            {
                result += "<option title='" + sqlDr["CIPMS_PART"] + "' value='" + sqlDr["PRC_CD"] + "'>" + sqlDr["NM"] + "</option>";
            }
        }
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Getoutput(string factory, string svTYPE, string jo, string go, string process, string nextprocess, string fromdate, string todate, string bypart,int addi)
    {
        string bydate = "false";
        if (fromdate != "" && todate != "")
            bydate = "true";
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        Outputreportsql outputreportsql = new Outputreportsql();
        SqlDataReader sqlDr;
        //SqlDataReader sqlDr2;
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
      //  if (fromdate != "" && todate != "")
 //     todate += " 23:59:59";

        string table1 = "", table2 = "";

        var widthnum = 0;

        if (factory == "YMG")
        {
            if (addi == 0)
                widthnum = (100 - 10) / (6 + 0);
            else if (addi == 1)
                widthnum = (100 - 10) / (4 + 1);
            else if (addi == 2 || addi == 4)
                widthnum = (100 - 10) / (6 + 2);
            else if (addi == 3 || addi == 5)
                widthnum = (100 - 10) / (6 + 3);
            else if (addi == 6)
                widthnum = (100 - 10) / (6 + 4);
            else if (addi == 7)
                widthnum = (100 - 10) / (6 + 5);
        }
        else
        {
            if (addi == 0)
                widthnum = (100 - 10) / (5 + 0);
            else if (addi == 1)
                widthnum = (100 - 10) / (5 + 1);
            else if (addi == 2 || addi == 4)
                widthnum = (100 - 10) / (5 + 2);
            else if (addi == 3 || addi == 5)
                widthnum = (100 - 10) / (5 + 3);
            else if (addi == 6)
                widthnum = (100 - 10) / (5 + 4);
            else if (addi == 7)
                widthnum = (100 - 10) / (5 + 5);
        }

        if (process == "all")
        {
            if (jo == "" && go == "" && bydate == "false")
            {
                sqlCon.Close();
                return "nodata";
            }
            else
            {
                sqlDr = outputreportsql.queryoutput(sqlCon, factory, "", "", go, jo, process, nextprocess, fromdate, todate, bypart, addi);
            }
            ////if (jo == "" && go == "")
            ////{
            ////    if (bydate == "true" && bypart == "false")
            ////    {
            ////        //sqlDr = outputreportsql.querydate(sqlCon, factory, fromdate, todate);
            ////        sqlDr = outputreportsql.queryoutput(sqlCon,factory,"","",go,jo,process,nextprocess,fromdate,todate,bypart,addi);
            ////    }
            ////    else if (bydate == "false" && bypart == "true")
            ////    {
            ////        sqlCon.Close();
            ////        return "nodata";
            ////    }
            ////    else if (bydate == "true" && bypart == "true")
            ////    {
            ////        sqlDr = outputreportsql.querydatepart(sqlCon, factory, fromdate, todate);
            ////    }
            ////    else
            ////    {
            ////        sqlCon.Close();
            ////        return "nodata";
            ////    }
            ////}
            ////else
            ////{
            ////    if (jo != "" && go != "")
            ////    {
            ////        if (jo.IndexOf(go.Substring(1)) == -1)
            ////        {
            ////            sqlCon.Close();
            ////            return "false1";
            ////        }
            ////    }
            ////    if (jo != "")
            ////    {
            ////        if (bydate == "false" && bypart == "false")
            ////        {
            ////            sqlDr = outputreportsql.queryjo(sqlCon, factory, jo);
            ////        }
            ////        else if (bydate == "true" && bypart == "false")
            ////        {
            ////            sqlDr = outputreportsql.queryjodateall(sqlCon, factory, jo, fromdate, todate);
            ////        }
            ////        else if (bydate == "false" && bypart == "true")
            ////        {
            ////            sqlDr = outputreportsql.queryjopart(sqlCon, factory, jo);
            ////        }
            ////        else if (bydate == "true" && bypart == "true")
            ////        {
            ////            sqlDr = outputreportsql.queryjodatepart(sqlCon, factory, jo, fromdate, todate);
            ////        }
            ////        else
            ////        {
            ////            sqlCon.Close();
            ////            return "nodate";
            ////        }
            ////    }
            ////    else
            ////    {
            ////        if (bydate == "false" && bypart == "false")
            ////        {
            ////            sqlDr = outputreportsql.querygo(sqlCon, factory, go);
            ////        }
            ////        else if (bydate == "true" && bypart == "false")
            ////        {
            ////            sqlDr = outputreportsql.querygodate(sqlCon, factory, go, fromdate, todate);
            ////        }
            ////        else if (bydate == "false" && bypart == "true")
            ////        {
            ////            sqlDr = outputreportsql.querygopart(sqlCon, factory, go);
            ////        }
            ////        else if (bydate == "true" && bypart == "true")
            ////        {
            ////            sqlDr = outputreportsql.querygodatepart(sqlCon, factory, go, fromdate, todate);
            ////        }
            ////        else
            ////        {
            ////            sqlCon.Close();
            ////            return "nodata";
            ////        }
            ////    }
            ////}

            string cutqty = "", prtqty = "", embqty = "", matchingqty = "", dcqty = "", fuseqty = "";
            int j = 0;
            int ttlcutqty = 0, ttlprtqty = 0, ttlembqty = 0, ttlmatchingqty = 0, ttldcqty = 0,ttlfuseqty=0;
            int[,] ttlpart = new int[20, 5];
            List<string> partlist = new List<string>();

            while (sqlDr.Read())
            {
                //comment by Jacob 2015-11-23
                //sqlDr2 = outputreportsql.getmatchingbydate(sqlCon, factory, sqlDr["GARMENT_TYPE"].ToString(), sqlDr["PROCESS_TYPE"].ToString(), sqlDr["DATE"].ToString(), sqlDr["DATE"].ToString() + " 23:59:59");
                //if (sqlDr2.Read())
                //{
                //    if (sqlDr2["MATCHING"].ToString() != "")
                //        ttlmatchingqty += int.Parse(sqlDr2["MATCHING"].ToString());
                //    if (sqlDr2["MATCHING"].ToString() == "0")
                //        matchingqty = "";
                //    else
                //        matchingqty = sqlDr2["MATCHING"].ToString();
                //}
                //else
                //    matchingqty = "";
                //sqlDr2.Close();

                ttlcutqty += int.Parse(sqlDr["CUT"].ToString());
                ttlprtqty += int.Parse(sqlDr["PRT"].ToString());
                ttlembqty += int.Parse(sqlDr["EMB"].ToString());
                ttlfuseqty += int.Parse(sqlDr["FUSE"].ToString());
                ttlmatchingqty += int.Parse(sqlDr["MATCHING"].ToString());
                ttldcqty += int.Parse(sqlDr["DC"].ToString());

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
                if (sqlDr["FUSE"].ToString() == "0")
                    fuseqty = "";
                else
                    fuseqty = sqlDr["FUSE"].ToString();
                if (sqlDr["MATCHING"].ToString() == "0")
                    matchingqty = "";
                else
                    matchingqty = sqlDr["MATCHING"].ToString();
                if (sqlDr["DC"].ToString() == "0")
                    dcqty = "";
                else
                    dcqty = sqlDr["DC"].ToString();

                if (bypart == "false" || bypart == "true")
                {
                    if (j % 2 == 1)
                        table1 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    else
                        table1 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["DATE"].ToString() + "</td>";

                    
                    table1 += AddField(addi, sqlDr, widthnum);              

                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + cutqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + prtqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + embqty + "</td>";
                    if (factory == "YMG")
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + fuseqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + matchingqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + dcqty + "</td></tr>";

                    j++;
                }
            
            }
            sqlDr.Close();

            if (jo == "" && go == "")
            {
                if (bypart == "false")
                {
                    table1 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";

                    table1 += AddTotal(addi, widthnum);

                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + ttlcutqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + ttlprtqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + ttlembqty + "</td>";
                    if (factory == "YMG")
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + ttlfuseqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + ttlmatchingqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + ttldcqty + "</td></tr>";
 
                }
                else
                {
                    int i = 0;
                    foreach (string n in partlist)
                    {
                        table1 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:15%;'>TTL</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:15%;'>" + n + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:14%;'>" + ttlpart[i, 0] + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:14%;'>" + ttlpart[i, 1] + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:14%;'>" + ttlpart[i, 2] + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:14%;'>" + ttlpart[i, 3] + "</td>";
                        table1 += "<td style='vertical-align:middle; text-align:center; width:14%;'>" + ttlpart[i, 4] + "</td></tr>";
                        i++;
                    }
                }
            }//有GO、JO条件查询
            else
            {
                if (bypart == "false" || bypart == "true")
                {
                    widthnum=0;
                    string style = "";
                    table1 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";

                    
                    if (addi == 1)
                    {
                        widthnum = (100 - 10) / (5 + 1);
                        style = "vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;";
                        table1 += "<td style='"+style+"'>" + "" + "</td>";
                    }
                    if ((addi == 2) || (addi == 4))
                    {
                        widthnum = (100 - 10) / (5 + 2);
                        style = "vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;";
                        table1 += "<td style='" + style + "'>" + "" + "</td>";
                        table1 += "<td style='" + style + "'>" + "" + "</td>";
                    }
                    if ((addi == 3) || (addi == 5))
                    {
                        widthnum = (100 - 10) / (5 + 3);
                        style = "vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;";
                        table1 += "<td style='" + style + "'>" + "" + "</td>";
                        table1 += "<td style='" + style + "'>" + "" + "</td>";
                        table1 += "<td style='" + style + "'>" + "" + "</td>";
                    }
                    if (addi == 6)
                    {
                        widthnum = (100 - 10) / (5 + 4);
                        style = "vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;";
                        table1 += "<td style='" + style + "'>" + "" + "</td>";
                        table1 += "<td style='" + style + "'>" + "" + "</td>";
                        table1 += "<td style='" + style + "'>" + "" + "</td>";
                        table1 += "<td style='" + style + "'>" + "" + "</td>";
                    }
                    if (addi == 7)
                    {
                        widthnum = (100 - 10) / (5 + 5);
                        style = "vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;";
                        table1 += "<td style='" + style + "'>" + "" + "</td>";
                        table1 += "<td style='" + style + "'>" + "" + "</td>";
                        table1 += "<td style='" + style + "'>" + "" + "</td>";
                        table1 += "<td style='" + style + "'>" + "" + "</td>";
                        table1 += "<td style='" + style + "'>" + "" + "</td>";
                    }

                    //table1 += "<td style='" + style + "'>" + ttlcutqty + "</td>";
                    //table1 += "<td style='" + style + "'>" + ttlprtqty + "</td>";
                    //table1 += "<td style='" + style + "'>" + ttlembqty + "</td>";
                    //if (factory=="YMG")
                    //    table1 += "<td style='" + style + "'>" + ttlfuseqty + "</td>";
                    //table1 += "<td style='" + style + "'>" + ttlmatchingqty + "</td>";
                    //table1 += "<td style='" + style + "'>" + ttldcqty + "</td>";

                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + ttlcutqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + ttlprtqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + ttlembqty + "</td>";
                    if (factory == "YMG")
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + ttlfuseqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + ttlmatchingqty + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + ttldcqty + "</td></tr>";

                }
                //else
                //{
                //    int i = 0;
                //    foreach (string n in partlist)
                //    {
                //        table1 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                //        table1 += "<td style='vertical-align:middle; text-align:center; width:15%;'>TTL</td>";
                //        table1 += "<td style='vertical-align:middle; text-align:center; width:15%;'>" + n + "</td>";
                //        table1 += "<td style='vertical-align:middle; text-align:center; width:14%;'>" + ttlpart[i, 0] + "</td>";
                //        table1 += "<td style='vertical-align:middle; text-align:center; width:14%;'>" + ttlpart[i, 1] + "</td>";
                //        table1 += "<td style='vertical-align:middle; text-align:center; width:14%;'>" + ttlpart[i, 2] + "</td>";
                //        table1 += "<td style='vertical-align:middle; text-align:center; width:14%;'>" + ttlpart[i, 3] + "</td>";
                //        table1 += "<td style='vertical-align:middle; text-align:center; width:14%;'>" + ttlpart[i, 4] + "</td></tr>";
                //        i++;
                //    }
                //}
            }
        }
        else  //(process <> "all")
        {
            if (jo == "" && go == "" && bydate == "false")
            {
                sqlCon.Close();
                return "nodata";
            }
            else
            {
                sqlDr = outputreportsql.queryoutput(sqlCon, factory, "K", "I", go, jo, process, nextprocess, fromdate, todate, bypart, addi);
            }

            //if (jo == "" && go == "")
            //{
            //    if (bydate == "true" && bypart == "false")
            //    {
            //        sqlDr = outputreportsql.querydate(sqlCon, factory, process, nextprocess, "K", "I", fromdate, todate);
            //    }
            //    else if (bydate == "false" && bypart == "true")
            //    {
            //        sqlCon.Close();
            //        return "nodata";
            //    }
            //    else if (bydate == "true" && bypart == "true")
            //    {
            //        sqlDr = outputreportsql.querydate(sqlCon, factory, process, nextprocess, "K", "I", fromdate, todate);
            //    }
            //    else
            //    {
            //        sqlCon.Close();
            //        return "false2";
            //    }
            //}
            //else
            //{
            //    sqlCon.Close();
            //    return "nodata";
            //}

            string cutqty = "", prtqty = "", embqty = "", matchingqty = "", dcqty = "",fuseqty ="";
            int j = 0;
            int ttlcutqty = 0, ttlprtqty = 0, ttlembqty = 0, ttlmatchingqty = 0, ttldcqty = 0, ttlfuseqty=0;
            int[] outputarray = new int[100];
            List<string> cutlinelist = new List<string>();

            while (sqlDr.Read())
            {
                int i = 0;
                foreach (string n in cutlinelist)
                {
                    if (sqlDr["CUT_LINE"].ToString() == n)
                        break;
                    i++;
                }
                if (i == cutlinelist.Count)
                {
                    cutlinelist.Add(sqlDr["CUT_LINE"].ToString());
                }
                outputarray[i] = outputarray[i] + int.Parse(sqlDr[process].ToString());
                
                if (sqlDr[process].ToString() != "0")
                {
                    if (j % 2 == 1)
                        table1 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    else
                        table1 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["DATE"].ToString() + "</td>";

                    

                    string style="";

                    table1 += AddField(addi, sqlDr, widthnum);

                    if (addi == 1)
                    {
                        widthnum = (100 - 10) / (3 + 1);
                        style = "vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;";
                        //table1 += "<td style='" + style + "'>" + "" + "</td>";
                    }
                    else if ((addi == 2) || (addi == 444))
                    {
                        widthnum = (100 - 10) / (3 + 2);
                        style = "vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;";
                        //table1 += "<td style='" + style + "'>" + "" + "</td>";
                        //table1 += "<td style='" + style + "'>" + "" + "</td>";
                    }
                    else if ((addi == 3) || (addi == 5))
                    {
                        widthnum = (100 - 10) / (3 + 3);
                        style = "vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;";
                        //table1 += "<td style='" + style + "'>" + "" + "</td>";
                        //table1 += "<td style='" + style + "'>" + "" + "</td>";
                        //table1 += "<td style='" + style + "'>" + "" + "</td>";
                    }
                    else if ((addi == 6) || (addi == 4))
                    {
                        widthnum = (100 - 10) / (3 + 4);
                        style = "vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;";
                        //table1 += "<td style='" + style + "'>" + "" + "</td>";
                        //table1 += "<td style='" + style + "'>" + "" + "</td>";
                        //table1 += "<td style='" + style + "'>" + "" + "</td>";
                        //table1 += "<td style='" + style + "'>" + "" + "</td>";
                    }
                    else if (addi == 7)
                    {
                        widthnum = (100 - 10) / (3 + 5);
                        style = "vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;";
                        //table1 += "<td style='" + style + "'>" + "" + "</td>";
                        //table1 += "<td style='" + style + "'>" + "" + "</td>";
                        //table1 += "<td style='" + style + "'>" + "" + "</td>";
                        //table1 += "<td style='" + style + "'>" + "" + "</td>";
                        //table1 += "<td style='" + style + "'>" + "" + "</td>";
                    }

                    style = "vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;";
                    table1 += "<td style='" + style + "'>" + process + "</td>";
                    table1 += "<td style='" + style + "'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                    table1 += "<td style='" + style + "'>" + sqlDr[process].ToString() + "</td></tr>";

                    //table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + process + "</td>";
                    //table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                    //table1 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr[process].ToString() + "</td></tr>";
                    j++;
                }
            }
            sqlDr.Close();

            int m = 0;
            int output = 0;
            foreach (string n in cutlinelist)
            {
                if (outputarray[m] != 0)
                {

                   // table1 += AddTotal(addi, widthnum);

                    table1 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>"+"TTL BY Line</td>";
                    //table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + "" + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + process + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + n + "</td>";
                    table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + outputarray[m] + "</td></tr>";
                    output += outputarray[m];
                    m++;
                }
            }
            table1 += "<tr class='table-tr-3' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
            table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>"+"TTL BY Process</td>";
           // table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>"+ "" + "</td>";
            table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>"+process + "</td>";
            table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>"+"</td>";
            table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>"+ + output + "</td></tr>";
        }

        if (process != "all")
        {
            if (bypart == "false")
            {
                sqlDr = outputreportsql.querytable2(sqlCon, factory, process, nextprocess, "K", "I", fromdate, todate);
            }
            else
            {
                sqlDr = outputreportsql.querytable2part(sqlCon, factory, process, nextprocess, "K", "I", fromdate, todate);
            }
            int j = 0;
            int output = 0;
            while (sqlDr.Read())
            {
                if (bypart == "false")
                {

                    if (sqlDr["OUT_QTY"].ToString() != "0")
                    {
                        if (j % 2 == 1)
                            table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["ORDER_QTY"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["CUT_QTY"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["OUT_QTY"].ToString() + "</td></tr>";
                        output += int.Parse(sqlDr["OUT_QTY"].ToString());
                        j++;
                    }
                }
                else
                {
                    if (sqlDr["OUT_QTY"].ToString() != "0")
                    {
                        if (j % 2 == 1)
                            table2 += "<tr class='table-tr-1' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        else
                            table2 += "<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + sqlDr["CUT_LINE"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["ORDER_QTY"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["CUT_QTY"].ToString() + "</td>";
                        table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + sqlDr["OUT_QTY"].ToString() + "</td></tr>";
                        j++;
                    }
                }
            }
            sqlDr.Close();
            if (bypart == "false")
            {
                table2 += "<tr class='table-tr-2' onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
                table2 += "<td style='vertical-align:middle; text-align:center; width:10%;'>TTL</td>";
                //table2 += AddTotal(addi);

                table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'></td>";
                table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'></td>";
                table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'></td>";
                table2 += "<td style='vertical-align:middle; text-align:center; width:18%;'>" + output + "</td></tr>";
            }
        }

        sqlCon.Close();
        string result;
        result = "[{ \"TABLE1\": \"" + table1 + "\", \"TABLE2\":\"" + table2 + "\" }]";

        try
        {
            return result;
        }
        catch(Exception ex)
        {
            return null;
        }
        finally
        {

        }
    }

    public static string AddTotal(int i, int widthnum)
    {
        string table2="";
        if (i == 0) { table2 = ""; }

        else if ((i==1))
        {
            table2 += "<td style='vertical-align:middle; text-align:center; width::" + widthnum.ToString() + "%;" + "'>" + "</td>";
        }
        else if ((i == 2) || (i == 4))
        {
            table2 += "<td style='vertical-align:middle; text-align:center; width::" + widthnum.ToString() + "%;" + "'>" + "</td>";
            table2 += "<td style='vertical-align:middle; text-align:center; width::" + widthnum.ToString() + "%;" + "'>" + "</td>";
        }
        else if ((i == 3) || (i == 5))
        {
            table2 += "<td style='vertical-align:middle; text-align:center; width::" + widthnum.ToString() + "%;" + "'>" + "</td>";
            table2 += "<td style='vertical-align:middle; text-align:center; width::" + widthnum.ToString() + "%;" + "'>" + "</td>";
            table2 += "<td style='vertical-align:middle; text-align:center; width::" + widthnum.ToString() + "%;" + "'>" + "</td>";
        }
        else if ((i == 6))
        {
            table2 += "<td style='vertical-align:middle; text-align:center; width::" + widthnum.ToString() + "%;" + "'>" + "</td>";
            table2 += "<td style='vertical-align:middle; text-align:center; width::" + widthnum.ToString() + "%;" + "'>" + "</td>";
            table2 += "<td style='vertical-align:middle; text-align:center; width::" + widthnum.ToString() + "%;" + "'>" + "</td>";
            table2 += "<td style='vertical-align:middle; text-align:center; width::" + widthnum.ToString() + "%;" + "'>" + "</td>";
        }
        else if ((i == 7))
        {
            table2 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + "</td>";
            table2 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + "</td>";
            table2 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + "</td>";
            table2 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + "</td>";
            table2 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" +"</td>";
        }

        return table2;
    }

    public static string AddField(int addi,SqlDataReader sqlDr,int widthnum)
    {
        string table1 = "";
            if (addi == 0)
            {
                table1 += "";
            }
            else if (addi == 1)
            {
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["PART_DESC"].ToString() + "</td>";
            }
            else if (addi == 2)
            {
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["SEW_LINE"].ToString() + "</td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
            }
            else if (addi == 3)
            {
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["SEW_LINE"].ToString() + "</td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                
            }
            else if (addi == 4)
            {
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
            }
            else if (addi == 5)
            {
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
            }
            else if (addi == 6)
            {
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["SEW_LINE"].ToString() + "</td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
            }
            else if (addi == 7)
            {
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["SEW_LINE"].ToString() + "</td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["JOB_ORDER_NO"].ToString() + "</td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["PART_DESC"].ToString() + "</td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["COLOR_CD"].ToString() + "</td>";
                table1 += "<td style='vertical-align:middle; text-align:center; width:" + widthnum.ToString() + "%;" + "'>" + sqlDr["SIZE_CD"].ToString() + "</td>";
            } 

        return table1;
    }
}
