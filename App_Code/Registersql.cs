using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;

/// <summary>
///Register 的摘要说明
/// </summary>
namespace NameSpace
{
    public class Registersql
    {
        public Registersql()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        public SqlDataReader getfuncid(SqlConnection sqlConn, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT B.FUNC_ID FROM CIPMS_USER AS A INNER JOIN CIPMS_USER_FUNC AS B ON A.USER_BARCODE=B.USER_BARCODE INNER JOIN CIPMS_FUNC_MASTER AS C ON B.FUNC_ID=C.ID WHERE A.USER_BARCODE='" + userbarcode + "' ORDER BY B.FUNC_ID";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getuserinfo(SqlConnection sqlConn, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT A.EMPLOYEE_NO,A.NAME,A.FACTORY_CD,A.PRC_CD,A.PRODUCTION_LINE_CD,A.SHIFT,D.MODULE_CD,D.MODULE_ID FROM CIPMS_USER AS A INNER JOIN CIPMS_USER_FUNC AS B ON A.USER_BARCODE=B.USER_BARCODE INNER JOIN CIPMS_FUNC_MASTER AS C ON B.FUNC_ID=C.ID INNER JOIN CIPMS_MODULE_MASTER AS D ON C.MODULE=D.MODULE_ID WHERE A.USER_BARCODE='"+userbarcode+"' AND C.IS_ACTIVE=1 AND D.ACTIVE=1 GROUP BY A.EMPLOYEE_NO,A.NAME,A.FACTORY_CD,A.PRC_CD,A.PRODUCTION_LINE_CD,A.SHIFT,D.MODULE_CD,D.MODULE_ID ORDER BY D.MODULE_ID";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getdefaultfunc(SqlConnection sqlConn, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select B.MODULE_CD from CIPMS_USER AS A INNER JOIN CIPMS_MODULE_MASTER AS B ON A.DEFAULTFUNC=B.MODULE_ID WHERE A.USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }

        public void deleteuser(SqlConnection sqlConn, SqlCommand cmd, string userbarcode)
        {
            string sql = "DELETE FROM CIPMS_USER_FUNC WHERE USER_BARCODE='" + userbarcode + "'";
            cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery(); 
            sql = "DELETE FROM CIPMS_USER WHERE USER_BARCODE='" + userbarcode + "'";
            cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }

        //插入用户信息
        public void userinsert(SqlConnection sqlConn, SqlCommand cmd, string newbarcode, string employeeno, string username, string factory, string process, string production, string shift, string defaultfunction)
        {
            string sql = "INSERT INTO CIPMS_USER (USER_BARCODE,EMPLOYEE_NO,NAME,FACTORY_CD,PRC_CD,PRODUCTION_LINE_CD,SHIFT,DEFAULTFUNC) SELECT '" + newbarcode + "','" + employeeno + "','" + username + "','" + factory + "','" + process + "','" + production + "','" + shift + "',MODULE_ID FROM CIPMS_MODULE_MASTER WHERE MODULE_CD='" + defaultfunction + "'";
            cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }

        //插入用户功能权限
        public void userfuncinsert(SqlConnection sqlConn, SqlCommand cmd, string temp)
        {
            string sql = "insert into CIPMS_USER_FUNC values " + temp;
            cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }

        public SqlDataReader isemployeenoexist(SqlConnection sqlConn, string employeeno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select USER_BARCODE from CIPMS_USER where EMPLOYEE_NO='" + employeeno + "'";
            return sqlComGet.ExecuteReader();
        }
    }
}