using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Model;
using Newtonsoft.Json;
using System.Data.SqlClient;

/// <summary>
///GetWIPReportBLL 的摘要说明
/// </summary>
public class GetWIPReportBLL
{
	public GetWIPReportBLL()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}

    public string GetWIPReport(string factory, string svTYPE, string go, string jo, string process, string bybundle, string bypart, string bycolor, string bysize,string bysewline,string byprocesspcs)
    {
        string Jsonresponse = "";

        if (bybundle == "false")
            bybundle = "N";
        else
            bybundle = "Y";
        if (bypart == "false")
            bypart = "N";
        else
            bypart = "Y";
        if (bycolor == "false")
            bycolor = "N";
        else
            bycolor = "Y";
        if (bysize == "false")
            bysize = "N";
        else
            bysize = "Y";

        if (bysewline == "false")
            bysewline = "N";
        else
            bysewline = "Y";

        if (byprocesspcs == "false")
            byprocesspcs = "N";
        else
            byprocesspcs = "Y";

        if (go != "")
            go = "%" + go + "%";
        if (jo != "")
            jo = "%" + jo + "%";

        GetWIPReportDAL getwipdata = new GetWIPReportDAL(factory, svTYPE);
        DataSet ds = getwipdata.GetWIPData(factory, go, jo, process, bybundle, bypart, bycolor, bysize ,bysewline,byprocesspcs);

        List<WIP_BUNDLE_DETAIL> table2 = new List<WIP_BUNDLE_DETAIL>();
        List<WIP_BUNDLE_DETAIL> table3 = new List<WIP_BUNDLE_DETAIL>();
        int tablenum = 0;
        if (ds.Tables.Count > 0)
        {
            
            foreach (DataTable dt in ds.Tables)
            {
                tablenum = tablenum + 1;
                if (tablenum == 1)
                {
                    int i = 1;
                    foreach (DataRow dr in dt.Rows)
                    {
                        WIP_BUNDLE_DETAIL row = new WIP_BUNDLE_DETAIL();
                        row.SEQ = i++;
                        row.JOB_ORDER_NO = dr[0].ToString();
                        row.COLOR_CD = dr[1].ToString();
                        row.SIZE_CD = dr[2].ToString();
                        row.BUNDLE_NO = dr[3].ToString();
                        row.PART_DESC = dr[4].ToString();
                        row.CUT_LINE = dr["CUT_LINE"].ToString();
                        row.SEW_LINE = dr["SEW_LINE"].ToString();
                        if (dr["CUT"].ToString() != "")
                            row.CUT = int.Parse(dr["CUT"].ToString());
                        else
                            row.CUT = 0;
                        if (dr["PRT"].ToString() != "")
                            row.PRT = int.Parse(dr["PRT"].ToString());
                        else
                            row.PRT = 0;
                        if (dr["EMB"].ToString() != "")
                            row.EMB = int.Parse(dr["EMB"].ToString());
                        else
                            row.EMB = 0;
                        if (dr["FUSE"].ToString() != "")
                            row.FUSE = int.Parse(dr["FUSE"].ToString());
                        else
                            row.FUSE = 0;
                        if (dr["MATCHING"].ToString() != "")
                            row.MATCHING = int.Parse(dr["MATCHING"].ToString());
                        else
                            row.MATCHING = 0;
                        if (dr["DC"].ToString() != "")
                            row.DC = int.Parse(dr["DC"].ToString());
                        else
                            row.DC = 0;
                        if (dr["SEW"].ToString() != "")
                            row.SEW = int.Parse(dr["SEW"].ToString());
                        else
                            row.SEW = 0;

                        if (dr["ORDER_QTY"].ToString() != "")
                            row.ORDER_QTY = int.Parse(dr["ORDER_QTY"].ToString());
                        else
                            row.ORDER_QTY = 0;
                        if (dr["CUT_QTY"].ToString() != "")
                            row.CUT_QTY = int.Parse(dr["CUT_QTY"].ToString());
                        else
                            row.CUT_QTY = 0;
                        if (dr["REDUCE_QTY"].ToString() != "")
                            row.REDUCE_QTY = int.Parse(dr["REDUCE_QTY"].ToString());
                        else
                            row.REDUCE_QTY = 0;
                        if (dr["ACTUAL_CUT_QTY"].ToString() != "")
                            row.ACTUAL_CUT_QTY = int.Parse(dr["ACTUAL_CUT_QTY"].ToString());
                        else
                            row.ACTUAL_CUT_QTY = 0;
                        if (dr["TOSEW"].ToString() != "")
                            row.TOSEW = int.Parse(dr["TOSEW"].ToString());
                        else
                            row.TOSEW = 0;

                        table2.Add(row);
                    }
                }

                
                if (tablenum == 2)
                {
                    int i = 1;
                    foreach (DataRow dr in dt.Rows)
                    {
                        WIP_BUNDLE_DETAIL row = new WIP_BUNDLE_DETAIL();
                        row.SEQ = i++;
                        row.JOB_ORDER_NO = dr[0].ToString();
                        row.COLOR_CD = dr[1].ToString();
                        row.SIZE_CD = dr[2].ToString();
                        row.BUNDLE_NO = dr[3].ToString();
                        row.PART_DESC = dr[4].ToString();
                        row.CUT_LINE = dr["CUT_LINE"].ToString();
                        row.SEW_LINE = dr["SEW_LINE"].ToString();
                        if (dr["CUT"].ToString() != "")
                            row.CUT = int.Parse(dr["CUT"].ToString());
                        else
                            row.CUT = 0;
                        if (dr["PRT"].ToString() != "")
                            row.PRT = int.Parse(dr["PRT"].ToString());
                        else
                            row.PRT = 0;
                        if (dr["EMB"].ToString() != "")
                            row.EMB = int.Parse(dr["EMB"].ToString());
                        else
                            row.EMB = 0;
                        if (dr["FUSE"].ToString() != "")
                            row.FUSE = int.Parse(dr["FUSE"].ToString());
                        else
                            row.FUSE = 0;
                        if (dr["MATCHING"].ToString() != "")
                            row.MATCHING = int.Parse(dr["MATCHING"].ToString());
                        else
                            row.MATCHING = 0;
                        if (dr["DC"].ToString() != "")
                            row.DC = int.Parse(dr["DC"].ToString());
                        else
                            row.DC = 0;
                        if (dr["SEW"].ToString() != "")
                            row.SEW = int.Parse(dr["SEW"].ToString());
                        else
                            row.SEW = 0;

                        if (dr["ORDER_QTY"].ToString() != "")
                            row.ORDER_QTY = int.Parse(dr["ORDER_QTY"].ToString());
                        else
                            row.ORDER_QTY = 0;
                        if (dr["CUT_QTY"].ToString() != "")
                            row.CUT_QTY = int.Parse(dr["CUT_QTY"].ToString());
                        else
                            row.CUT_QTY = 0;
                        if (dr["REDUCE_QTY"].ToString() != "")
                            row.REDUCE_QTY = int.Parse(dr["REDUCE_QTY"].ToString());
                        else
                            row.REDUCE_QTY = 0;
                        if (dr["ACTUAL_CUT_QTY"].ToString() != "")
                            row.ACTUAL_CUT_QTY = int.Parse(dr["ACTUAL_CUT_QTY"].ToString());
                        else
                            row.ACTUAL_CUT_QTY = 0;
                        if (dr["TOSEW"].ToString() != "")
                            row.TOSEW = int.Parse(dr["TOSEW"].ToString());
                        else
                            row.TOSEW = 0;

                        table3.Add(row);
                    
                    }
                }
            }
            Jsonresponse = "[{ \"SUCCESS\": true, \"TABLE2\": " + JsonConvert.SerializeObject(table2) + ", \"TABLE3\": " + JsonConvert.SerializeObject(table3)+" }]";
            return Jsonresponse;
        }
        else
        {
            Jsonresponse = "[{ \"SUCCESS\": false }]";
            return Jsonresponse;
        }
    }


    public string GetProcessList(string factory, string svTYPE)
    {
        COMMONDAL commondal = new COMMONDAL(factory, svTYPE);

        SqlDataReader sqlDr = commondal.GetProcessList(factory, "K");
        List<PROCESS> processlist = new List<PROCESS>();

        PROCESS firstprocess = new PROCESS();
        firstprocess.PRC_CD = "";
        firstprocess.NM = "All(全部)";
        firstprocess.CIPMS_CHS = "全部";
        processlist.Add(firstprocess);

        while (sqlDr.Read())
        {
            if (sqlDr["PRC_CD"].ToString() == "SEW")
                continue;
            PROCESS process = new PROCESS();
            process.PRC_CD = sqlDr["PRC_CD"].ToString();
            process.NM = sqlDr["NM"].ToString();
            process.CIPMS_CHS = sqlDr["CIPMS_CHS"].ToString();
            if (process.CIPMS_CHS.Length >= 3)
            {
                PROCESS matching = new PROCESS();
                matching.PRC_CD = "MATCHING";
                matching.NM = "MATCHING(待配套)";
                matching.CIPMS_CHS = "待配套";
                processlist.Add(matching);
                process.CIPMS_CHS = process.CIPMS_CHS.Substring(1);
            }
            processlist.Add(process);
        }
        sqlDr.Close();

        string JsonResponse = "";

        JsonResponse = "[{\"SUCCESS\":true, \"data\":";
        JsonResponse += JsonConvert.SerializeObject(processlist);
        JsonResponse += "}]";
        return JsonResponse;
    }



    public string GetWIPReportByProcess(string factory, string svTYPE, string go, string jo, string process, string bybundle, string bypart, string bycolor, string bysize,string bysewline, string byprocesspcs)
    {
        string Jsonresponse = "";

        if (bybundle == "false")
            bybundle = "N";
        else
            bybundle = "Y";
        if (bypart == "false")
            bypart = "N";
        else
            bypart = "Y";
        if (bycolor == "false")
            bycolor = "N";
        else
            bycolor = "Y";
        if (bysize == "false")
            bysize = "N";
        else
            bysize = "Y";

        if (bysewline == "false")
            bysewline = "N";
        else
            bysewline = "Y";

        if (byprocesspcs == "false")
            byprocesspcs = "N";
        else
            byprocesspcs = "Y";

        if (go != "")
            go = "%" + go + "%";
        if (jo != "")
            jo = "%" + jo + "%";

        GetWIPReportDAL getwipdata = new GetWIPReportDAL(factory, svTYPE);
        DataSet ds = getwipdata.GetWIPReportByProcess(factory, go, jo, process, bybundle, bypart, bycolor, bysize,bysewline,byprocesspcs);

        List<GROUP_WIP_BYLINE> table1 = new List<GROUP_WIP_BYLINE>();
        List<GROUP_WIP_BYJO> table2 = new List<GROUP_WIP_BYJO>();

        if (ds.Tables.Count > 1)
        {
            DataTable dt1 = ds.Tables[0];
            int i = 1;
            foreach (DataRow dr in dt1.Rows)
            {
                GROUP_WIP_BYLINE row = new GROUP_WIP_BYLINE();
                row.SEQ = i++;
                row.CUT_LINE = dr["CUT_LINE"].ToString();
                if (dr[1].ToString() != "")
                    row.WIP = int.Parse(dr["WIP"].ToString());
                else
                    row.WIP = 0;
                row.SEW_LINE = dr["SEW_LINE"].ToString();
                row.PROCESS_CD = process;

                table1.Add(row);
            }

            DataTable dt2 = ds.Tables[1];
            i = 1;
            foreach (DataRow dr in dt2.Rows)
            {
                GROUP_WIP_BYJO row = new GROUP_WIP_BYJO();
                row.SEQ = i++;
                row.CUT_LINE = dr["CUT_LINE"].ToString();
                row.JOB_ORDER_NO = dr["JOB_ORDER_NO"].ToString();
                row.COLOR_CD = dr[2].ToString();
                row.SIZE_CD = dr[3].ToString();
                row.BUNDLE_NO = dr[4].ToString();
                row.PART_DESC = dr[5].ToString();
                if (dr[6].ToString() != "")
                    row.ORDER_QTY = int.Parse(dr[6].ToString());
                else
                    row.ORDER_QTY = 0;
                if (dr[7].ToString() != "")
                    row.CUT_QTY = int.Parse(dr[7].ToString());
                else
                    row.CUT_QTY = 0;
                if (dr[8].ToString() != "")
                    row.WIP = int.Parse(dr[8].ToString());
                else
                    row.WIP = 0;
                row.SEW_LINE = dr[9].ToString();

                table2.Add(row);
            }
            Jsonresponse = "[{ \"SUCCESS\": true, \"TABLE1\": " + JsonConvert.SerializeObject(table1) + ", \"TABLE2\": " + JsonConvert.SerializeObject(table2) + " }]";
            return Jsonresponse;
        }
        else
        {
            Jsonresponse = "[{ \"SUCCESS\": false }]";
            return Jsonresponse;
        }
    }
}