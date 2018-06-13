using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;

/// <summary>
///Reducesql 的摘要说明
/// </summary>
public class Reducesql
{
	public Reducesql()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}

    public SqlDataReader getbundleno(SqlConnection sqlConn, string jo)
    {
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlConn;
        sqlComGet.CommandText = "SELECT BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO='" + jo + "' AND LEN(BARCODE)=14 GROUP BY BUNDLE_NO ORDER BY BUNDLE_NO";
        return sqlComGet.ExecuteReader();
    }
    public SqlDataReader getbundleno(SqlConnection sqlConn, string jo, string layno)
    {
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlConn;
        sqlComGet.CommandText = "SELECT BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO='" + jo + "' and LAY_NO=" + layno + " AND LEN(BARCODE)=14 GROUP BY BUNDLE_NO ORDER BY BUNDLE_NO";
        return sqlComGet.ExecuteReader();
    }
    public SqlDataReader getbarcode(SqlConnection sqlConn, string jo, string bundle)
    {
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlConn;
        sqlComGet.CommandText = "select BARCODE from CIPMS_BUNDLE_FOR_SCANNING where JOB_ORDER_NO='" + jo + "' and BUNDLE_NO=" + bundle + " GROUP BY BARCODE";
        return sqlComGet.ExecuteReader();
    }
}