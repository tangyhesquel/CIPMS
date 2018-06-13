using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;

/// <summary>
///Peertransaction 的摘要说明
/// </summary>
/// 
namespace NameSpace
{
    public class Peertransaction
    {
        public Peertransaction()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        //查询JO的详细信息
        public SqlDataReader jolaynosku(SqlConnection sqlConn, string joborderno, string layno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.JOB_ORDER_NO,a.BUNDLE_NO,a.COLOR_CD,a.SIZE_CD,a.CUT_LINE,a.LAY_NO,a.GARMENT_TYPE,a.QTY,c.STYLE_NO,d.SHORT_NAME,b.BATCH_NO from CUT_BUNDLE_HD a inner join cut_lay_dt b on a.LAY_TRANS_ID = b.LAY_TRANS_ID and a.lay_dt_id = b.lay_dt_id inner join JO_HD as c on a.JOB_ORDER_NO = c.JO_NO inner join GEN_CUSTOMER as d on c.CUSTOMER_CD=d.CUSTOMER_CD where a.JOB_ORDER_NO = '" + joborderno + "' and a.LAY_NO='" + layno + "'";
            return sqlComGet.ExecuteReader();
        }

        public void unscaninsert1(SqlConnection sqlConn, string sql)
        {
            sql = "insert into CIPMS_USER_SCANNING_DFT VALUES " + sql;
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
    }
}