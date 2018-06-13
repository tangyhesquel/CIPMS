using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using NameSpace;
using System.Data;

/// <summary>
///GetWIPReport 的摘要说明
/// </summary>
public class GetWIPReportDAL
{
    public SqlConnection sqlCon { get; set; }
    public Connect connectstring { get; set; }
    public SqlCommand sqlComGet { get; set; }

	public GetWIPReportDAL(string factory, string svTYPE)
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

    public DataSet GetWIPData(string factory, string go, string jo, string process, string bybundle, string bypart, string bycolor, string bysize,string bysewline,string byprocesspcs)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("USP_CIPMS_WIP_INQUIRY",sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@JOB_ORDER_NO", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@GARMENT_ORDER_NO", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@PROCESS_CD", SqlDbType.NVarChar, 10);
            cmd.Parameters.Add("@BYBUNDLE", SqlDbType.NChar);
            cmd.Parameters.Add("@BYPART", SqlDbType.NChar);
            cmd.Parameters.Add("@BYCOLOR", SqlDbType.NChar);
            cmd.Parameters.Add("@BYSIZE", SqlDbType.NChar);
            cmd.Parameters.Add("@BYSEWLINE", SqlDbType.NChar);
            cmd.Parameters.Add("@BYPROCESSPCS", SqlDbType.NChar);
            cmd.Parameters["@JOB_ORDER_NO"].Value = jo;
            cmd.Parameters["@GARMENT_ORDER_NO"].Value = go;
            cmd.Parameters["@PROCESS_CD"].Value = process;
            cmd.Parameters["@BYBUNDLE"].Value = bybundle;
            cmd.Parameters["@BYPART"].Value = bypart;
            cmd.Parameters["@BYCOLOR"].Value = bycolor;
            cmd.Parameters["@BYSIZE"].Value = bysize;
            cmd.Parameters["@BYSEWLINE"].Value = bysewline;
            cmd.Parameters["@BYPROCESSPCS"].Value = byprocesspcs;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            return ds;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public DataSet GetWIPReportByProcess(string factory, string go, string jo, string process, string bybundle, string bypart, string bycolor, string bysize, string bysewline, string byprocesspcs)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("USP_CIPMS_WIPBYPROCESS_INQUIRY", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@JOB_ORDER_NO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@GARMENT_ORDER_NO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PROCESS_CD", SqlDbType.NVarChar);
            cmd.Parameters.Add("@BYBUNDLE", SqlDbType.NChar);
            cmd.Parameters.Add("@BYPART", SqlDbType.NChar);
            cmd.Parameters.Add("@BYCOLOR", SqlDbType.NChar);
            cmd.Parameters.Add("@BYSIZE", SqlDbType.NChar);
            cmd.Parameters.Add("@BYSEWLINE", SqlDbType.NChar);
            cmd.Parameters.Add("@BYPROCESSPCS", SqlDbType.NChar);

            cmd.Parameters["@JOB_ORDER_NO"].Value = jo;
            cmd.Parameters["@GARMENT_ORDER_NO"].Value = go;
            cmd.Parameters["@PROCESS_CD"].Value = process;
            cmd.Parameters["@BYBUNDLE"].Value = bybundle;
            cmd.Parameters["@BYPART"].Value = bypart;
            cmd.Parameters["@BYCOLOR"].Value = bycolor;
            cmd.Parameters["@BYSIZE"].Value = bysize;
            cmd.Parameters["@BYSEWLINE"].Value = bysewline;
            cmd.Parameters["@BYPROCESSPCS"].Value = byprocesspcs;
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
}