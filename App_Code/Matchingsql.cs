using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;

/// <summary>
///Matchingsql 的摘要说明
/// </summary>
public class Matchingsql
{
	public Matchingsql()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}

    public SqlDataReader checkall(SqlConnection sqlConn, string docno, string userbarcode)
    {
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlConn;
        sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING as d with (nolock)  where CARTON_BARCODE in (select c.CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING as c with (nolock) inner join (select a.JOB_ORDER_NO,a.BUNDLE_NO from CIPMS_BUNDLE_FOR_SCANNING as a with (nolock) inner join CIPMS_USER_SCANNING_DFT as b with (nolock)  on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "' group by a.JOB_ORDER_NO,a.BUNDLE_NO) as d on c.JOB_ORDER_NO=d.JOB_ORDER_NO and c.BUNDLE_NO=d.BUNDLE_NO where c.CARTON_STATUS='C' group by c.CARTON_BARCODE) and CARTON_STATUS='C' except select BUNDLE_ID from CIPMS_USER_SCANNING_DFT with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "'";
        return sqlComGet.ExecuteReader();
    }
    public SqlDataReader checkmatching(SqlConnection sqlConn, string docno, string userbarcode)
    {
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlConn;
        sqlComGet.CommandText = "select d.BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING as d with (nolock)  inner join (select b.JOB_ORDER_NO,b.BUNDLE_NO from CIPMS_USER_SCANNING_DFT as a with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' group by b.JOB_ORDER_NO,b.BUNDLE_NO)as c on c.JOB_ORDER_NO=d.JOB_ORDER_NO and c.BUNDLE_NO=d.BUNDLE_NO group by d.BUNDLE_ID except select BUNDLE_ID from CIPMS_USER_SCANNING_DFT with (nolock)  where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "'";
        return sqlComGet.ExecuteReader();
    }
    public SqlDataReader checkhavematched(SqlConnection sqlConn, string docno, string userbarcode)
    {
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlConn;
        sqlComGet.CommandText = "select a.BUNDLE_ID from CIPMS_USER_SCANNING_DFT as a with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID inner join CIPMS_JO_WIP_HD as c with (nolock)  on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.COLOR_CD=c.COLOR_CODE and b.SIZE_CD=c.SIZE_CODE and b.PART_CD=c.PART_CD and b.FACTORY_CD=c.FACTORY_CD and b.PROCESS_CD=c.PROCESS_CD and b.GARMENT_TYPE=c.GARMENT_TYPE and b.PROCESS_TYPE=c.PROCESS_TYPE inner join CIPMS_JO_WIP_BUNDLE as d on c.STOCK_ID=d.STOCK_ID and b.BUNDLE_NO=d.BUNDLE_NO where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' and (d.STATUS='M' or d.STATUS='S')";
        return sqlComGet.ExecuteReader();
    }
    public void opencartonstatus(SqlConnection sqlConn, string userbarcode, string cartonlist)
    {
        string sql = "";
        SqlCommand cmd = new SqlCommand(sql, sqlConn);
        cmd.ExecuteNonQuery();
    }
    public SqlDataReader querychange(SqlConnection sqlConn, string date, string userbarcode, string docno, string functioncd, string process)
    {
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlConn;
        sqlComGet.CommandText = "SELECT BUNDLE_ID,CARTON_BARCODE,PROCESS_CD,PROCESS_TYPE,BARCODE,a.PART_CD,b.PART_DESC,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS FROM CIPMS_BUNDLE_FOR_SCANNING as a with (nolock) inner join CIPMS_PART_MASTER as b with (nolock)  on a.PART_CD=b.PART_CD where BUNDLE_ID IN (SELECT BUNDLE_ID from CIPMS_SAVE_AND_QUERY with (nolock)  WHERE CREATE_DATE='" + date + "' AND CREATE_USER_ID='" + userbarcode + "' AND FUNCTION_CD='" + functioncd + "' EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "') and PROCESS_CD='" + process + "'";
        return sqlComGet.ExecuteReader();
    }
    public void unscannedinsert(SqlConnection sqlConn, string docno, string userbarcode, string bundlebarcode, string process, string functioncd)
    {
        string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE BARCODE='" + bundlebarcode + "' and PROCESS_CD='" + process + "' EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT with (nolock)  WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
        SqlCommand cmd = new SqlCommand(sql, sqlConn);
        cmd.ExecuteNonQuery();
    }
    public void unscannedinsert2(SqlConnection sqlConn, string docno, string userbarcode, string cartonbarcode, string process, string functioncd)
    {
        string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) WHERE CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C' and PROCESS_CD='" + process + "' EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT with (nolock)  WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
        SqlCommand cmd = new SqlCommand(sql, sqlConn);
        cmd.ExecuteNonQuery();
    }
}