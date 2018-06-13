using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;

/// <summary>
///Bundleinputsql 的摘要说明
/// </summary>
public class Bundleinputsql
{
	public Bundleinputsql()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}
    public SqlDataReader checkexists1(SqlConnection sqlConn, string userbarcode, string docno, string jo, string bundle, string part)
    {
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlConn;
        sqlComGet.CommandText = "SELECT JOB_ORDER_NO FROM CIPMS_USER_SCANNING_DFT WHERE JOB_ORDER_NO='" + jo + "' AND BUNDLE_NO='" + bundle + "' AND PART_CD='" + part + "' AND DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "'";
        return sqlComGet.ExecuteReader();
    }
    public SqlDataReader checkexists2(SqlConnection sqlConn, string jo, string bundle, string part)
    {
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlConn;
        sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING where JOB_ORDER_NO='" + jo + "' and BUNDLE_NO='" + bundle + "' and PART_CD='" + part + "'";
        return sqlComGet.ExecuteReader();
    }
    public SqlDataReader checkcipmsbundle(SqlConnection sqlConn, string docno, string userbarcode)
    {
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlConn;
        sqlComGet.CommandText = "select b.SEQ_ID from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_USER_SCANNING_DFT as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO and a.PART_CD=b.PART_CD where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "'";
        return sqlComGet.ExecuteReader();
    }
    public SqlDataReader checkcipmsbarcode(SqlConnection sqlConn, string docno, string userbarcode)
    {
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlConn;
        sqlComGet.CommandText = "select b.SEQ_ID from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_USER_SCANNING_DFT as b on a.BARCODE=b.BUNDLE_BARCODE where b.DOC_NO='"+docno+"' and b.USER_BARCODE='"+userbarcode+"'";
        return sqlComGet.ExecuteReader();
    }
    public void insertscanlist(SqlConnection sqlConn, string seqid, string userbarcode, string docno, string bundlebarcode, string part, string jo, string layno, string bundleno, string color, string size, string qty, string functioncd)
    {
        string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE,JOB_ORDER_NO,COLOR_CD,SIZE_CD,QTY,LAY_NO,BUNDLE_NO,SEQ_ID) values (-1,'" + docno + "','" + userbarcode + "','" + bundlebarcode + "','" + part + "','" + functioncd + "',GETDATE(),'" + jo + "','" + color + "','" + size + "'," + qty + ",'" + layno + "'," + bundleno + "," + seqid + ")";
        SqlCommand cmd = new SqlCommand(sql, sqlConn);
        cmd.ExecuteNonQuery();
    }
    public void deletescan(SqlConnection sqlConn, string docno, string userbarcode, string jo, string bundle, string part)
    {
        string sql = "delete from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "' and JOB_ORDER_NO='" + jo + "' and BUNDLE_NO=" + bundle + " and PART_CD='" + part + "'";
        SqlCommand cmd = new SqlCommand(sql, sqlConn);
        cmd.ExecuteNonQuery();
    }
    public void insertcipmsbundle(SqlConnection sqlConn, string docno, string userbarcode, string factory, string process, string garmenttype)
    {
        string sql = "insert into CIPMS_BUNDLE_FOR_SCANNING (FACTORY_CD,PROCESS_CD,PRODUCTION_LINE_CD,JOB_ORDER_NO,SIZE_CD,COLOR_CD,LAY_NO,BUNDLE_NO,BARCODE,PART_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_BARCODE,CARTON_STATUS,DOC_NO,GARMENT_TYPE,PROCESS_TYPE,USER_CREATE_ID,CREATE_DATE) select '" + factory + "','" + process + "','NA',JOB_ORDER_NO,SIZE_CD,COLOR_CD,LAY_NO,BUNDLE_NO,BUNDLE_BARCODE,PART_CD,QTY,0,0,'0','O','0','" + garmenttype + "','I','" + userbarcode + "',GETDATE() from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "'";
        SqlCommand cmd = new SqlCommand(sql, sqlConn);
        cmd.ExecuteNonQuery();
    }
    public void insertcipmswiphd(SqlConnection sqlConn, string docno, string userbarcode, string factory, string process, string garmenttype)
    {
        string sql = "insert into CIPMS_JO_WIP_HD (FACTORY_CD,JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PROCESS_CD,PRODUCTION_LINE_CD,PROCESS_TYPE,PART_CD,IN_QTY,OUT_QTY,PULL_IN_QTY,PULL_OUT_QTY,DISCREPANCY_QTY,WIP,INTRANS_QTY,INTRANS_IN,INTRANS_OUT,BUNDLE_REDUCE,MATCHING,GARMENT_TYPE,PRODUCTION_FACTORY) select '" + factory + "',JOB_ORDER_NO,COLOR_CD,SIZE_CD,'" + process + "','NA','I',PART_CD,SUM(QTY),0,0,0,0,SUM(QTY),0,0,0,0,0,'" + garmenttype + "','" + factory + "' from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "' group by JOB_ORDER_NO,COLOR_CD,SIZE_CD,PART_CD";
        SqlCommand cmd = new SqlCommand(sql, sqlConn);
        cmd.ExecuteNonQuery();
    }
    public void insertcipmswipbundle(SqlConnection sqlConn, string docno, string userbarcode, string process, string garmenttype)
    {
        string sql = "insert into CIPMS_JO_WIP_BUNDLE (STOCK_ID,BUNDLE_NO,IN_QTY,OUT_QTY,WIP,INTRANS_QTY,DISCREPANCY_QTY,MATCHING,DEFECT,TRANSFER_IN,TRANSFER_OUT,PART_CD,EMPLOYEE_OUTPUT) select b.STOCK_ID,a.BUNDLE_NO,a.QTY,0,a.QTY,0,0,0,0,0,0,a.PART_CD,'0' from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_JO_WIP_HD as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CODE and a.SIZE_CD=b.SIZE_CODE and a.PART_CD=b.PART_CD and b.PROCESS_CD='" + process + "' and b.PRODUCTION_LINE_CD='NA' and b.PROCESS_TYPE='I' where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "'";
        SqlCommand cmd = new SqlCommand(sql, sqlConn);
        cmd.ExecuteNonQuery();
    }
}