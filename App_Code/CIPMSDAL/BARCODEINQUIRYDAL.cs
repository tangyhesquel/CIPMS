using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using NameSpace;
using System.Data;

/// <summary>
///BARCODEINQUIRYDAL 的摘要说明
/// </summary>
public class BARCODEINQUIRYDAL
{
    public SqlConnection sqlCon { get; set; }
    public Connect connectstring { get; set; }

    public BARCODEINQUIRYDAL(string factory, string svTYPE)
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
        sqlCon = new SqlConnection();
        connectstring = new Connect();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
	}

    public DataSet GetDOCNOList(string DOC_NO, string JOB_ORDER_NO, string STATUS, string SENDPROCESS, string RECEIVEPROCESS, string CREATEDATEFROM, string CREATEDATETO)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("USP_CIPMS_DOCNOLIST_INQUIRY", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@DOC_NO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@JOB_ORDER_NO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@STATUS", SqlDbType.NChar);
            cmd.Parameters.Add("@SENDPROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@RECEIVEPROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@CREATEDATEFROM", SqlDbType.NVarChar);
            cmd.Parameters.Add("@CREATEDATETO", SqlDbType.NVarChar);
            cmd.Parameters["@DOC_NO"].Value = DOC_NO;
            cmd.Parameters["@JOB_ORDER_NO"].Value = JOB_ORDER_NO;
            cmd.Parameters["@STATUS"].Value = STATUS;
            cmd.Parameters["@SENDPROCESS"].Value = SENDPROCESS;
            cmd.Parameters["@RECEIVEPROCESS"].Value = RECEIVEPROCESS;
            cmd.Parameters["@CREATEDATEFROM"].Value = CREATEDATEFROM;
            cmd.Parameters["@CREATEDATETO"].Value = CREATEDATETO;
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

    public DataSet GetDOCNODetailList(string DOC_NO, string BYPART)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("USP_CIPMS_DOCNOLISTDETAIL_INQUIRY", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@DOC_NO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@BYPART", SqlDbType.NChar);
            cmd.Parameters["@DOC_NO"].Value = DOC_NO;
            cmd.Parameters["@BYPART"].Value = BYPART;
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

    public DataSet GetBarcodeInformationDetail(string GO, string JO, string COLOR, string LAYNO, string BUNDLENO, string BARCODE, string BARCODETYPE)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("USP_BARCODE_INFORMATION_INQUIRY", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@GO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@JO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@COLOR", SqlDbType.NVarChar);
            cmd.Parameters.Add("@LAYNO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@BUNDLE_NO", SqlDbType.NVarChar);
            cmd.Parameters.Add("@BARCODE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@BARCODETYPE", SqlDbType.NChar);
            cmd.Parameters["@GO"].Value = GO;
            cmd.Parameters["@JO"].Value = JO;
            cmd.Parameters["@COLOR"].Value = COLOR;
            cmd.Parameters["@LAYNO"].Value = LAYNO;
            cmd.Parameters["@BUNDLE_NO"].Value = BUNDLENO;
            cmd.Parameters["@BARCODE"].Value = BARCODE;
            cmd.Parameters["@BARCODETYPE"].Value = BARCODETYPE;
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