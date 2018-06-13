using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;
using System.Data;

/// <summary>
///Transactionsql 的摘要说明
/// </summary>
/// 
namespace NameSpace
{
    public class Transactionsql
    {
        public Transactionsql()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }
        public SqlDataReader checkifbybundle(SqlConnection sqlConn, string process, string nextprocess)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select NEXT_PROCESS_CD from CIPMS_FTY_PROCESS_FLOW with (nolock) where PROCESS_CD='" + process + "' AND  NEXT_PROCESS_CD='" + nextprocess + "' and NEXT_PROCESS_GARMENT_TYPE='K' and NEXT_PROCESS_TYPE='I' and CIPMS_PART='Y'";//CIPMS_PART='Y 表示by bundle
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader unscan(SqlConnection sqlConn, string bundlebarcode, string userbarcode, string part, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT PROCESS_TYPE,BUNDLE_ID,CARTON_BARCODE,a.FACTORY_CD,PROCESS_CD,PRODUCTION_LINE_CD,PROCESS_TYPE,BARCODE,a.PART_CD,b.PART_DESC,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_PART_MASTER as b with (nolock) on a.PART_CD=b.PART_CD where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING  with (nolock) WHERE BARCODE='" + bundlebarcode + "' AND PART_CD IN" + part + " EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT with (nolock) WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader querycontract(SqlConnection sqlConn, string bundlebarcode, string nextprocess)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select CONTRACT_NO from PRD_OUTSOURCE_CONTRACT_DT with (nolock) where JOB_ORDER_NO=(select JOB_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where BARCODE='" + bundlebarcode + "' group by JOB_ORDER_NO) and PROCESS_CD like '%" + nextprocess + "%'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader querycontractcarton(SqlConnection sqlConn, string cartonbarcode, string nextprocess)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select distinct CONTRACT_NO from PRD_OUTSOURCE_CONTRACT_DT with (nolock) where JOB_ORDER_NO in(select JOB_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING with (nolock)  where CARTON_BARCODE='" + cartonbarcode + "' group by JOB_ORDER_NO) and PROCESS_CD like '%" + nextprocess + "%'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader howmanyjo(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(a.JOB_ORDER_NO)as QTY from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock)  inner join CIPMS_USER_SCANNING_DFT as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "' group by a.JOB_ORDER_NO";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader howmanycon(SqlConnection sqlConn, string docno, string userbarcode, string contractno, string nextprocess)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select count(SEND_ID)as QTY from PRD_OUTSOURCE_CONTRACT_DT with (nolock) where JOB_ORDER_NO in(select b.JOB_ORDER_NO from CIPMS_USER_SCANNING_DFT as a with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' group by b.JOB_ORDER_NO) and CONTRACT_NO='" + contractno + "' and PROCESS_CD like '%" + nextprocess + "%'";
            return sqlComGet.ExecuteReader();
        }
        public void updatetranbundleforscanning1(SqlConnection sqlConn, string processtype, string sql, string userbarcode)
        {
            sql = "update CIPMS_BUNDLE_FOR_SCANNING set DOC_NO=b.DOC_NO, PROCESS_CD=b.NEXT_PROCESS_CD, PRODUCTION_LINE_CD=b.NEXT_PRODUCTION_LINE_CD,PROCESS_TYPE='" + processtype + "',LAST_MODI_USER_ID='" + userbarcode + "',LAST_MODI_DATE=GETDATE() from CIPMS_BUNDLE_FOR_SCANNING as a with (nolock) , (select c.DOC_NO,c.JOB_ORDER_NO,c.BUNDLE_NO,c.PART_CD,c.QTY,d.NEXT_PROCESS_CD,d.NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_DT as c with (nolock)  inner join CIPMS_TRANSFER_HD as d with (nolock) on c.DOC_NO=d.DOC_NO where c.DOC_NO in" + sql + ") as b where a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO and a.PART_CD=b.PART_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void insertoas(SqlConnection sqlConn, string docno, string userbarcode, string contractno)
        {
            string sql = "insert into CIPMS_OAS_TRANSFER_DFT (SEND_ID,CONTRACT_NO,JOB_ORDER_NO,BUNDLE_NO,QTY,COLOR_CD,SIZE_CD,LAY_NO,CUT_LINE) select c.SEND_ID,c.CONTRACT_NO,b.JOB_ORDER_NO,b.BUNDLE_NO,MIN(b.QTY-b.DISCREPANCY_QTY)as QTY,b.COLOR_CD,b.SIZE_CD,b.LAY_NO,b.CUT_LINE,GETDATE(),'" + userbarcode + "' from CIPMS_USER_SCANNING_DFT as a  with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID inner join PRD_OUTSOURCE_CONTRACT_DT as c  with (nolock) on b.JOB_ORDER_NO=c.JOB_ORDER_NO where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' and c.CONTRACT_NO='" + contractno + "' group by c.SEND_ID,c.CONTRACT_NO,b.JOB_ORDER_NO,b.BUNDLE_NO,b.COLOR_CD,b.SIZE_CD,b.LAY_NO,b.CUT_LINE";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }

        //bundle scan
        public SqlDataReader getrow1(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(*)AS ROW1 from CIPMS_USER_SCANNING_DFT with (nolock)  where DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getrow2(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT COUNT(*)AS ROW2 FROM CIPMS_BUNDLE_FOR_SCANNING AS A with (nolock) INNER JOIN (SELECT A.JOB_ORDER_NO,A.BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING AS A  with (nolock) INNER JOIN CIPMS_USER_SCANNING_DFT AS B with (nolock)  ON A.BUNDLE_ID=B.BUNDLE_ID WHERE B.DOC_NO='" + docno + "' AND B.USER_BARCODE='" + userbarcode + "' GROUP BY A.JOB_ORDER_NO,A.BUNDLE_NO)AS B ON A.JOB_ORDER_NO=B.JOB_ORDER_NO AND A.BUNDLE_NO=B.BUNDLE_NO";
            return sqlComGet.ExecuteReader();
        }
        public void unscannedinsert(SqlConnection sqlConn, string docno, string userbarcode, string bundlebarcode, string part, string functioncd)
        {
            string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE)SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING with (nolock)  where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) WHERE BARCODE='" + bundlebarcode + "' AND PART_CD IN " + part + " EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT with (nolock) WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') order by BARCODE";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void unscanneddocnoinsert(SqlConnection sqlConn, string docno, string userbarcode, string doc_no, string functioncd, string get_othercarton,string part)
        {
            string sql = "";
            if (part=="null")
                part="";
            if (get_othercarton == "F")
                sql = sql + " insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE,CAR_NO) select BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE(), ISNULL(CAR_NO,'') AS CAR_NO from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where DOC_NO='" + doc_no + "' and BUNDLE_ID not in (select BUNDLE_ID from CIPMS_USER_SCANNING_DFT with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') order by BARCODE";
            //下面未变
            else
            {
                sql = sql + " insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE,CAR_NO)";
                sql = sql + " select BUNDLE_ID,'" + docno + "','" + userbarcode + "',a.BARCODE,a.PART_CD,'" + functioncd + "',GETDATE(), ISNULL(CAR_NO,'') from CIPMS_BUNDLE_FOR_SCANNING a with (nolock)  ";
                sql = sql + " inner join (SELECT distinct JOB_ORDER_NO,BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) WHERE DOC_NO='"+doc_no+"') b ";
                sql = sql + " on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO ";
                sql = sql + " and BUNDLE_ID not in (select BUNDLE_ID from CIPMS_USER_SCANNING_DFT with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') ";

                sql = sql + " and ('" + part + "'='' or a.PART_CD in ( select FNField from FN_SPLIT_STRING_TB('" + part + "',',')))";

                sql = sql + " where DOC_NO='0' order by BARCODE";

            }
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void unscanneddocnoinsertbybundle(SqlConnection sqlConn, string docno, string userbarcode, string doc_no, string functioncd)
        {
            string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE,CAR_NO) select BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE(), ISNULL(CAR_NO,'')  from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where DOC_NO='" + doc_no + "' and BUNDLE_ID not in (select BUNDLE_ID from CIPMS_USER_SCANNING_DFT with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') order by BARCODE";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void unscannedinsert(SqlConnection sqlConn, string docno, string userbarcode, string cartonbarcode, string functioncd)
        {
            string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE,CAR_NO) select BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE(),ISNULL(CAR_NO,'')  from CIPMS_BUNDLE_FOR_SCANNING with (nolock) WHERE BUNDLE_ID IN (select BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) where CARTON_BARCODE ='" + cartonbarcode + "' and CARTON_STATUS = 'C' except select BUNDLE_ID from CIPMS_USER_SCANNING_DFT with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') order by BARCODE";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader unscanbundle(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            //sqlComGet.CommandText = "SELECT a.PROCESS_TYPE,CARTON_BARCODE,a.FACTORY_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,BARCODE,a.JOB_ORDER_NO,a.BUNDLE_NO,a.LAY_NO,a.SIZE_CD,a.COLOR_CD,MIN(a.QTY)AS CUT_QTY,MIN(c.MATCHING-c.OUT_QTY)as OUTPUT,MAX(a.DISCREPANCY_QTY)AS DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_JO_WIP_HD as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CODE and a.SIZE_CD=b.SIZE_CODE and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD and a.PROCESS_TYPE=b.PROCESS_TYPE inner join CIPMS_JO_WIP_BUNDLE as c on b.STOCK_ID=c.STOCK_ID and a.BUNDLE_NO=c.BUNDLE_NO INNER JOIN CIPMS_USER_SCANNING_DFT AS D ON A.BUNDLE_ID=D.BUNDLE_ID WHERE D.DOC_NO='" + docno + "' and D.USER_BARCODE='" + userbarcode + "' group by a.PROCESS_TYPE,CARTON_BARCODE,a.FACTORY_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,BARCODE,a.JOB_ORDER_NO,a.BUNDLE_NO,a.LAY_NO,a.SIZE_CD,a.COLOR_CD,CARTON_STATUS";
            sqlComGet.CommandText = "SELECT a.PROCESS_TYPE,a.FACTORY_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,BARCODE,a.JOB_ORDER_NO,a.BUNDLE_NO,a.LAY_NO,a.SIZE_CD,a.COLOR_CD,MIN(a.QTY)AS CUT_QTY,MIN(a.MATCHING-a.OUT_QTY)as OUTPUT,MAX(a.DISCREPANCY_QTY)AS DISCREPANCY_QTY,CARTON_STATUS FROM (SELECT top 100 percent a.PROCESS_TYPE,a.CARTON_BARCODE,a.FACTORY_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,a.BARCODE,a.JOB_ORDER_NO,a.BUNDLE_NO,a.PART_CD,a.LAY_NO,a.COLOR_CD,a.SIZE_CD,a.CARTON_STATUS,a.QTY,c.MATCHING,c.OUT_QTY,a.DISCREPANCY_QTY from CIPMS_BUNDLE_FOR_SCANNING as a with (nolock) inner join CIPMS_JO_WIP_HD as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CODE and a.SIZE_CD=b.SIZE_CODE and a.PART_CD=b.PART_CD and a.FACTORY_CD=b.FACTORY_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD and a.GARMENT_TYPE=b.GARMENT_TYPE and a.PROCESS_TYPE=b.PROCESS_TYPE inner join CIPMS_JO_WIP_BUNDLE as c with (nolock) on b.STOCK_ID=c.STOCK_ID and a.BUNDLE_NO=c.BUNDLE_NO INNER JOIN CIPMS_USER_SCANNING_DFT AS D with (nolock) ON A.BUNDLE_ID=D.BUNDLE_ID WHERE D.DOC_NO='" + docno + "' and D.USER_BARCODE='" + userbarcode + "' order by D.CREATE_DATE desc)AS a group by a.PROCESS_TYPE,a.FACTORY_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,BARCODE,a.JOB_ORDER_NO,a.BUNDLE_NO,a.LAY_NO,a.SIZE_CD,a.COLOR_CD,CARTON_STATUS";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader totalbundles1(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT COUNT(BUNDLE_BARCODE)AS BUNDLE FROM(select BUNDLE_BARCODE from CIPMS_USER_SCANNING_DFT with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "' group by BUNDLE_BARCODE)AS A";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader totalpcs(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select MIN(MATCHING-OUT_QTY)as QTY from (select a.BARCODE,a.PART_CD,c.MATCHING,c.OUT_QTY from CIPMS_BUNDLE_FOR_SCANNING as a with (nolock)  inner join CIPMS_JO_WIP_HD as b with (nolock) on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CODE and a.SIZE_CD=b.SIZE_CODE and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD and a.PROCESS_TYPE=b.PROCESS_TYPE inner join CIPMS_JO_WIP_BUNDLE as c with (nolock) on b.STOCK_ID=c.STOCK_ID and a.BUNDLE_NO=c.BUNDLE_NO inner join CIPMS_USER_SCANNING_DFT as d with (nolock) on a.BUNDLE_ID=d.BUNDLE_ID where d.DOC_NO='" + docno + "' and d.USER_BARCODE='" + userbarcode + "')as e group by BARCODE";
            return sqlComGet.ExecuteReader();
        }
        public void unscaninsert(SqlConnection sqlConn, string bundlebarcode, string docno, string userbarcode, string functioncd)
        {
            string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) select BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING where BARCODE='" + bundlebarcode + "' and BUNDLE_ID not in (select BUNDLE_ID from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') ORDER BY BARCODE";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void unscaninsertcarton(SqlConnection sqlConn, string cartonbarcode, string docno, string userbarcode, string functioncd)
        {
            string sql = "INSERT INTO CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() FROM CIPMS_BUNDLE_FOR_SCANNING AS A with (nolock) INNER JOIN (SELECT JOB_ORDER_NO,BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) WHERE CARTON_BARCODE='" + cartonbarcode + "' AND CARTON_STATUS='C' GROUP BY JOB_ORDER_NO,BUNDLE_NO)AS B ON A.JOB_ORDER_NO=B.JOB_ORDER_NO AND A.BUNDLE_NO=B.BUNDLE_NO AND BUNDLE_ID NOT IN (select BUNDLE_ID from CIPMS_USER_SCANNING_DFT with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //tangyh 2017.04.05
        public void unDOCinsertDOC(SqlConnection sqlConn, string cartonbarcode, string docno, string userbarcode, string functioncd)
        {
            string sql = "INSERT INTO CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() FROM CIPMS_BUNDLE_FOR_SCANNING AS A with (nolock) INNER JOIN (SELECT JOB_ORDER_NO,BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) WHERE DOC_NO='" + cartonbarcode + "'  GROUP BY JOB_ORDER_NO,BUNDLE_NO)AS B ON A.JOB_ORDER_NO=B.JOB_ORDER_NO AND A.BUNDLE_NO=B.BUNDLE_NO AND BUNDLE_ID NOT IN (select BUNDLE_ID from CIPMS_USER_SCANNING_DFT with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void unscanedinsert(SqlConnection sqlConn, string docno, string userbarcode, string cartonbarcode, string functioncd)
        {
            string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) WHERE CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C' EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT with (nolock) WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') order by BARCODE";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader needtoselectproduction(SqlConnection sqlConn, string bundlebarcode, string nextprocess)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select WIP_CONTROL_BY_PRD_LINE from PRD_JO_PROCESS_WIP_CONTROL with (nolock) where JOB_ORDER_NO in(select JOB_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING where BARCODE='" + bundlebarcode + "') and PROCESS_CD='" + nextprocess + "' and GARMENT_TYPE='K' and PROCESS_TYPE='I'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader isinmyprocess(SqlConnection sqlConn, string bundlebarcode, string part, string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where BARCODE='" + bundlebarcode + "' and PART_CD in" + part + " and (PROCESS_CD<>'" + process + "' or PROCESS_TYPE<>'I')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader isinmyprocesscarton(SqlConnection sqlConn, string bundlebarcode, string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where CARTON_BARCODE='" + bundlebarcode + "' and CARTON_STATUS='C' and (PROCESS_CD<>'" + process + "' or PROCESS_TYPE<>'I')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader isinmyprocessdocno(SqlConnection sqlConn, string bundlebarcode, string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where DOC_NO='" + bundlebarcode + "' and (PROCESS_CD<>'" + process + "' or PROCESS_TYPE<>'I')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader isinmyprocessnopart(SqlConnection sqlConn, string bundlebarcode, string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where BARCODE='" + bundlebarcode + "' and (PROCESS_CD<>'" + process + "' or PROCESS_TYPE<>'I')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader isincarton(SqlConnection sqlConn, string bundlebarcode, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where BARCODE='" + bundlebarcode + "' and PART_CD in" + part + " group by CARTON_STATUS";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader isincartonnopart(SqlConnection sqlConn, string bundlebarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where BARCODE='" + bundlebarcode + "' group by CARTON_STATUS";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkifmatching(SqlConnection sqlConn, string bundlebarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_JO_WIP_HD as b  with (nolock) on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CODE and a.SIZE_CD=b.SIZE_CODE and a.PROCESS_CD=b.PROCESS_CD and a.PART_CD=b.PART_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD and a.PROCESS_TYPE=b.PROCESS_TYPE inner join CIPMS_JO_WIP_BUNDLE as c  with (nolock) on b.STOCK_ID=c.STOCK_ID and a.BUNDLE_NO=c.BUNDLE_NO where a.BARCODE='" + bundlebarcode + "' and (isnull(c.STATUS,'N')<>'M' OR c.MATCHING<=c.OUT_QTY)";
            return sqlComGet.ExecuteReader();
        }
        //UPDATE 20151112 16:39 BY JACOB
        //REMOVE = 
        public SqlDataReader checkifcartonmatching(SqlConnection sqlConn, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING as a with (nolock) inner join CIPMS_JO_WIP_HD as b with (nolock) on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CODE and a.SIZE_CD=b.SIZE_CODE and a.PROCESS_CD=b.PROCESS_CD and a.PART_CD=b.PART_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD and a.PROCESS_TYPE=b.PROCESS_TYPE inner join CIPMS_JO_WIP_BUNDLE as c with (nolock) on b.STOCK_ID=c.STOCK_ID and a.BUNDLE_NO=c.BUNDLE_NO INNER JOIN (SELECT JOB_ORDER_NO,BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) WHERE CARTON_BARCODE='" + cartonbarcode + "' AND CARTON_STATUS='C' GROUP BY JOB_ORDER_NO,BUNDLE_NO)AS D ON A.JOB_ORDER_NO=D.JOB_ORDER_NO AND A.BUNDLE_NO=D.BUNDLE_NO where isnull(c.STATUS,'N')<>'M' OR c.MATCHING<c.OUT_QTY";
            return sqlComGet.ExecuteReader();
        }
        //更改 carton barcode
        public SqlDataReader needtoselectproductioncarton(SqlConnection sqlConn, string factory, string cartonbarcode, string nextprocess, string garmenttype, string processtype, string nextfactory)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select WIP_CONTROL_BY_PRD_LINE from PRD_JO_PROCESS_WIP_CONTROL  with (nolock) where FACTORY_CD='" + factory + "' and JOB_ORDER_NO in(select JOB_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C' group by JOB_ORDER_NO) and PROCESS_CD='" + nextprocess + "' and GARMENT_TYPE='" + garmenttype + "' and PROCESS_TYPE='" + processtype + "' and PRODUCTION_FACTORY='" + nextfactory + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkcartonsubmit(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            //sqlComGet.CommandText = "select d.STOCK_ID from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.BUNDLE_ID=b.BUNDLE_ID inner join CIPMS_JO_WIP_HD as c on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.FACTORY_CD=c.FACTORY_CD and b.PROCESS_CD=c.PROCESS_CD and b.PROCESS_TYPE=c.PROCESS_TYPE inner join CIPMS_JO_WIP_BUNDLE as d on c.STOCK_ID=d.STOCK_ID and b.BUNDLE_NO=d.BUNDLE_NO and b.PART_CD=d.PART_CD where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' and d.STATUS is not NULL and d.STATUS<>'C' and d.STATUS<>'M'";
            //tangyh 2017.04.05 只检查是否是S状态
            sqlComGet.CommandText = "select d.STOCK_ID from CIPMS_USER_SCANNING_DFT as a with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b with (nolock)  on a.BUNDLE_ID=b.BUNDLE_ID inner join CIPMS_JO_WIP_HD as c with (nolock) on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.FACTORY_CD=c.FACTORY_CD and b.PROCESS_CD=c.PROCESS_CD and b.PROCESS_TYPE=c.PROCESS_TYPE inner join CIPMS_JO_WIP_BUNDLE as d with (nolock) on c.STOCK_ID=d.STOCK_ID and b.BUNDLE_NO=d.BUNDLE_NO and b.PART_CD=d.PART_CD where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' and d.STATUS='S'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkcartonsubmit(SqlConnection sqlConn, string bundlebarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select d.STOCK_ID from CIPMS_USER_SCANNING_DFT as a with (nolock)  inner join CIPMS_BUNDLE_FOR_SCANNING as b with (nolock)  on a.BUNDLE_ID=b.BUNDLE_ID inner join CIPMS_JO_WIP_HD as c on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.FACTORY_CD=c.FACTORY_CD and b.PROCESS_CD=c.PROCESS_CD and b.PROCESS_TYPE=c.PROCESS_TYPE inner join CIPMS_JO_WIP_BUNDLE as d on c.STOCK_ID=d.STOCK_ID and b.BUNDLE_NO=d.BUNDLE_NO and b.PART_CD=d.PART_CD where a.BUNDLE_BARCODE='" + bundlebarcode + "' and d.STATUS is not NULL and d.STATUS<>'C' and isnull(d.STATUS,'N')<>'M'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkbundlesubmit(SqlConnection sqlConn, string bundlebarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select d.STOCK_ID from CIPMS_BUNDLE_FOR_SCANNING as b with (nolock)  inner join CIPMS_JO_WIP_HD as c with (nolock)  on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.COLOR_CD=c.COLOR_CODE and b.SIZE_CD=c.SIZE_CODE and b.PART_CD=c.PART_CD and b.PROCESS_CD=c.PROCESS_CD and b.PRODUCTION_LINE_CD=c.PRODUCTION_LINE_CD and b.GARMENT_TYPE=c.GARMENT_TYPE and b.PROCESS_TYPE=c.PROCESS_TYPE inner join CIPMS_JO_WIP_BUNDLE as d on c.STOCK_ID=d.STOCK_ID and b.BUNDLE_NO=d.BUNDLE_NO where b.BARCODE='" + bundlebarcode + "' and d.STATUS is not NULL and d.STATUS<>'C' and isnull(d.STATUS,'N')<>'M'";
            return sqlComGet.ExecuteReader();
        }

        //OASbundle scan
        public SqlDataReader numofscan(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(BUNDLE_ID)as QTY from CIPMS_USER_SCANNING_DFT with (nolock)  where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkreceived(SqlConnection sqlConn, string bundlebarcode, string nextprocess)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT b.BUNDLE_ID FROM CIPMS_OAS_INTERFACE AS a with (nolock)  inner join CIPMS_BUNDLE_FOR_SCANNING as b with (nolock)  on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO where b.BARCODE='" + bundlebarcode + "' and a.PROCESS_CD='" + nextprocess + "' and a.FLAG=0";
            return sqlComGet.ExecuteReader();
        }

        public SqlDataReader hassamecontract(SqlConnection sqlConn, string contract, string bundlebarcode, string nextprocess)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select SEND_ID from PRD_OUTSOURCE_CONTRACT_DT with (nolock)  where CONTRACT_NO='" + contract + "' and JOB_ORDER_NO=(select JOB_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING where BARCODE='" + bundlebarcode + "' group by JOB_ORDER_NO) and PROCESS_CD like '%" + nextprocess + "%'";
            return sqlComGet.ExecuteReader();
        }

        //OAScarton scan
        public SqlDataReader joqty(SqlConnection sqlConn, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(JOB_ORDER_NO)as QTY from CIPMS_BUNDLE_FOR_SCANNING with (nolock)  where CARTON_BARCODE='" + cartonbarcode + "' group by JOB_ORDER_NO";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader ctnqty(SqlConnection sqlConn, string contract, string cartonbarcode, string nextprocess)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(SEND_ID)as QTY from PRD_OUTSOURCE_CONTRACT_DT with (nolock) where CONTRACT_NO='" + contract + "' and JOB_ORDER_NO in(select JOB_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING where CARTON_BARCODE='" + cartonbarcode + "' group by JOB_ORDER_NO) and PROCESS_CD like '%" + nextprocess + "%'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader jo(SqlConnection sqlConn, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select JOB_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where CARTON_BARCODE='" + cartonbarcode + "' group by JOB_ORDER_NO";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader onecn(SqlConnection sqlConn, string onejo, string nextprocess)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select distinct CONTRACT_NO from PRD_OUTSOURCE_CONTRACT_DT with (nolock)  where JOB_ORDER_NO='" + onejo + "' and PROCESS_CD like '%" + nextprocess + "%'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader cnjoqty(SqlConnection sqlConn, string contract, string jolist)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(SEND_ID)as QTY from PRD_OUTSOURCE_CONTRACT_DT with (nolock)  where CONTRACT_NO='" + contract + "' and JOB_ORDER_NO in" + jolist + " group by SEND_ID";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getpackagepart(SqlConnection sqlConn, string newdocno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PART_CD from CIPMS_BUNDLE_FOR_SCANNING with (nolock)  where DOC_NO='" + newdocno + "' group by PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getgo(SqlConnection sqlConn, string newdocno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select GARMENT_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where DOC_NO='" + newdocno + "' GROUP BY GARMENT_ORDER_NO";
            return sqlComGet.ExecuteReader();
        }

        //Process type为I的平级调动sql
        public SqlDataReader selecttranproduction(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.PRODUCTION_LINE_CD from CIPMS_USER_SCANNING_DFT as a with (nolock)  inner join CIPMS_BUNDLE_FOR_SCANNING as b with (nolock)  on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' group by b.PRODUCTION_LINE_CD";
            return sqlComGet.ExecuteReader();
        }
        public void inserttransferhd(SqlConnection sqlConn, string sql)
        {
            sql = "insert into CIPMS_TRANSFER_HD (DOC_NO,FACTORY_CD,PROCESS_CD,NEXT_PROCESS_CD,PRODUCTION_LINE_CD,NEXT_PRODUCTION_LINE_CD,STATUS,CREATE_USER_ID,CREATE_DATE,LAST_MODI_USER_ID,LAST_MODI_DATE,CONFIRM_USER_ID,CONFIRM_DATE,PROCESS_GARMENT_TYPE,NEXT_PROCESS_GARMENT_TYPE,PROCESS_TYPE,NEXT_PROCESS_TYPE,PROCESS_PRODUCTION_FACTORY,NEXT_PROCESS_PRODUCTION_FACTORY) values " + sql;
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void inserttransferdt(SqlConnection sqlConn, string docno, string userbarcode, string trandocno, string productionline)
        {
            string sql = "insert into CIPMS_TRANSFER_DT (DOC_NO,JOB_ORDER_NO,COLOR_CD,SIZE_CD,QTY,CUT_LAY_NO,BUNDLE_NO,PART_CD,USER_CARD_ID) select '" + trandocno + "',c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.QTY-c.DISCREPANCY_QTY,c.LAY_NO,c.BUNDLE_NO,c.PART_CD,'" + userbarcode + "' from CIPMS_USER_SCANNING_DFT as a with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as c with (nolock) on a.BUNDLE_ID=c.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' and c.PRODUCTION_LINE_CD='" + productionline + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void inserttrandefect(SqlConnection sqlConn, string docno, string userbarcode, string process, string sql)
        {
            sql = "insert into CIPMS_BUNDLE_DEFECT (TRX_ID,DEFECT_REASON_CD,QTY,PROCESS_CD) select e.TRX_ID,b.DEFECT_REASON_CD,b.DEFECT_QTY,'" + process + "' from CIPMS_BUNDLE_DEFECT_DFT as b with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as d with (nolock) on b.BUNDLE_ID=d.BUNDLE_ID inner join CIPMS_TRANSFER_DT as e with (nolock) ON e.JOB_ORDER_NO=d.JOB_ORDER_NO and e.BUNDLE_NO=d.BUNDLE_NO and e.PART_CD=d.PART_CD where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "' and e.DOC_NO in " + sql + "";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatetrandefect(SqlConnection sqlConn, string sql)
        {
            sql = "update CIPMS_BUNDLE_FOR_SCANNING set DEFECT=DEFECT+d.QTY from CIPMS_BUNDLE_FOR_SCANNING as c, (select a.JOB_ORDER_NO,a.BUNDLE_NO,a.PART_CD,b.QTY from CIPMS_TRANSFER_DT as a with (nolock)  inner join CIPMS_BUNDLE_DEFECT as b with (nolock)  on a.TRX_ID=b.TRX_ID where a.DOC_NO in" + sql + ")as d where c.JOB_ORDER_NO=d.JOB_ORDER_NO and c.BUNDLE_NO=d.BUNDLE_NO and c.PART_CD=d.PART_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatetransferhd(SqlConnection sqlConn, string sql)
        {
            sql = "update CIPMS_JO_WIP_HD set WIP=a.WIP-d.QTY,INTRANS_OUT=a.INTRANS_OUT+d.QTY from CIPMS_JO_WIP_HD as a with (nolock) , (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD,(SUM(c.QTY)-SUM(c.DISCREPANCY_QTY))as QTY from CIPMS_TRANSFER_DT as b with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as c with (nolock) on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.BUNDLE_NO=c.BUNDLE_NO and b.PART_CD=c.PART_CD where b.DOC_NO in" + sql + " group by c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD)as d where a.JOB_ORDER_NO=d.JOB_ORDER_NO and a.COLOR_CODE=d.COLOR_CD and a.SIZE_CODE=d.SIZE_CD and a.PART_CD=d.PART_CD and a.PROCESS_CD=d.PROCESS_CD and a.PRODUCTION_LINE_CD=d.PRODUCTION_LINE_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void tranwiphdadd(SqlConnection sqlConn, string factory, string processtype, string garmenttype, string sql)
        {
            string sql1 = "";
            sql1 = "insert into CIPMS_JO_WIP_HD (FACTORY_CD,JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PROCESS_CD,PRODUCTION_LINE_CD,PROCESS_TYPE,PART_CD,IN_QTY,WIP,GARMENT_TYPE,PRODUCTION_FACTORY)select '" + factory + "',b.JOB_ORDER_NO,b.COLOR_CD,b.SIZE_CD,b.NEXT_PROCESS_CD,b.NEXT_PRODUCTION_LINE_CD,'" + processtype + "',b.PART_CD,0,0,'" + garmenttype + "','" + factory + "' from (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,e.NEXT_PROCESS_CD,e.NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_DT as c with (nolock) inner join CIPMS_TRANSFER_HD as e with (nolock) on c.DOC_NO=e.DOC_NO where c.DOC_NO in" + sql + " group by c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,e.NEXT_PROCESS_CD,e.NEXT_PRODUCTION_LINE_CD except select JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD from CIPMS_JO_WIP_HD with (nolock) )as b";
            SqlCommand cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
            sql1 = "update CIPMS_JO_WIP_HD set WIP=a.WIP+b.QTY,INTRANS_IN=a.INTRANS_IN+b.QTY from CIPMS_JO_WIP_HD as a, (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,e.NEXT_PROCESS_CD,e.NEXT_PRODUCTION_LINE_CD,(d.QTY-d.DISCREPANCY_QTY)as QTY from CIPMS_TRANSFER_DT as c with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as d with (nolock) on c.JOB_ORDER_NO=d.JOB_ORDER_NO and c.BUNDLE_NO=d.BUNDLE_NO and c.PART_CD=d.PART_CD inner join CIPMS_TRANSFER_HD as e with (nolock)  on c.DOC_NO=e.DOC_NO where c.DOC_NO in" + sql + ")as b where a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CODE=b.COLOR_CD and a.SIZE_CODE=b.SIZE_CD and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.NEXT_PROCESS_CD and a.PRODUCTION_LINE_CD=b.NEXT_PRODUCTION_LINE_CD";
            cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatetransferbundle(SqlConnection sqlConn, string sql)
        {
            sql = "update CIPMS_JO_WIP_BUNDLE set WIP=a.WIP-e.QTY,TRANSFER_OUT=a.TRANSFER_OUT+e.QTY from CIPMS_JO_WIP_BUNDLE as a with (nolock) , (select d.STOCK_ID,c.BUNDLE_NO,(c.QTY-c.DISCREPANCY_QTY)as QTY from CIPMS_TRANSFER_DT as b inner join CIPMS_BUNDLE_FOR_SCANNING as c with (nolock) on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.BUNDLE_NO=c.BUNDLE_NO and b.PART_CD=c.PART_CD inner join CIPMS_JO_WIP_HD as d with (nolock) on d.JOB_ORDER_NO=c.JOB_ORDER_NO and d.COLOR_CODE=c.COLOR_CD and d.SIZE_CODE=c.SIZE_CD and d.PART_CD=c.PART_CD and d.PROCESS_CD=c.PROCESS_CD and d.PRODUCTION_LINE_CD=c.PRODUCTION_LINE_CD where b.DOC_NO in" + sql + ")as e where a.STOCK_ID=e.STOCK_ID and a.BUNDLE_NO=e.BUNDLE_NO";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void insertwipbundle(SqlConnection sqlConn, string sql)
        {
            sql = "insert into CIPMS_JO_WIP_BUNDLE (STOCK_ID,BUNDLE_NO,WIP,TRANSFER_IN,PART_CD) select a.STOCK_ID,b.BUNDLE_NO,b.QTY,b.QTY,b.PART_CD from CIPMS_JO_WIP_HD as a with (nolock) inner join CIPMS_TRANSFER_DT as b with (nolock)  on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CODE=b.COLOR_CD and a.SIZE_CODE=b.SIZE_CD and a.PART_CD=b.PART_CD inner join CIPMS_TRANSFER_HD as c with (nolock)  on b.DOC_NO=c.DOC_NO and a.PROCESS_CD=c.NEXT_PROCESS_CD and a.PRODUCTION_LINE_CD=c.NEXT_PRODUCTION_LINE_CD where b.DOC_NO in" + sql;
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatetranbundleforscanning(SqlConnection sqlConn, string sql, string userbarcode)
        {
            sql = "update CIPMS_BUNDLE_FOR_SCANNING set QTY=b.QTY,DOC_NO=b.DOC_NO, PROCESS_CD=b.NEXT_PROCESS_CD, PRODUCTION_LINE_CD=b.NEXT_PRODUCTION_LINE_CD,LAST_MODI_USER_ID='" + userbarcode + "',LAST_MODI_DATE=GETDATE() from CIPMS_BUNDLE_FOR_SCANNING as a with (nolock) , (select c.DOC_NO,c.JOB_ORDER_NO,c.BUNDLE_NO,c.PART_CD,c.QTY,d.NEXT_PROCESS_CD,d.NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_DT as c with (nolock) inner join CIPMS_TRANSFER_HD as d with (nolock)  on c.DOC_NO=d.DOC_NO where c.DOC_NO in" + sql + ") as b where a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO and a.PART_CD=b.PART_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }

        //查找出该JO所有的part
        public SqlDataReader jopart(SqlConnection sqlConn, string barcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.PART_CD,b.PART_DESC from CIPMS_BUNDLE_FOR_SCANNING as a with (nolock)  inner join CIPMS_PART_MASTER as b with (nolock)  on a.PART_CD=b.PART_CD where a.BARCODE='" + barcode + "' group by a.PART_CD,b.PART_DESC";
            return sqlComGet.ExecuteReader();
        }

        //查询允许流转的部门
        public SqlDataReader nextprocess(SqlConnection sqlConn, string factory, string process, string flowtype)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.PRC_CD,b.NM,a.CIPMS_PART from CIPMS_FTY_PROCESS_FLOW as a with (nolock)  inner join GEN_PRC_CD_MST as b with (nolock)  on a.FACTORY_CD=b.FACTORY_CD and a.NEXT_PROCESS_CD=b.PRC_CD where a.FACTORY_CD='" + factory + "' and a.PROCESS_CD='" + process + "' and a.Flow_Type='" + flowtype + "' and b.CIPMS_FLAG='Y' GROUP BY b.PRC_CD,b.NM,a.CIPMS_PART,b.DISPLAY_SEQ order by b.DISPLAY_SEQ";
            //sqlComGet.CommandText = "select b.PRC_CD,b.NM,a.CIPMS_PART,b.CIPMS_CHS from CIPMS_FTY_PROCESS_FLOW as a inner join GEN_PRC_CD_MST as b on a.FACTORY_CD=b.FACTORY_CD and a.NEXT_PROCESS_CD=b.PRC_CD where a.FACTORY_CD='" + factory + "' and a.PROCESS_CD='" + process + "' and a.Flow_Type='" + flowtype + "' and b.CIPMS_FLAG='Y' GROUP BY b.PRC_CD,b.NM,a.CIPMS_PART,b.DISPLAY_SEQ,b.CIPMS_CHS order by b.DISPLAY_SEQ";
            return sqlComGet.ExecuteReader();
        }

        public SqlCommand getjocolordb(SqlConnection sqlConn, string sql, string doc_no)
        {
            //sql = "select JOB_ORDER_NO,COLOR_CD,LAY_NO=stuff((select '，'+CAST(LAY_NO AS VARCHAR(10)) from CIPMS_BUNDLE_FOR_SCANNING where JOB_ORDER_NO=a.JOB_ORDER_NO and COLOR_CD=a.COLOR_CD AND DOC_NO='" + doc_no + "' GROUP BY LAY_NO for XML path('')),1,1,''), (SELECT COUNT(BUNDLE_NO) FROM (SELECT BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO=a.JOB_ORDER_NO AND COLOR_CD=a.COLOR_CD AND DOC_NO='" + doc_no + "' GROUP BY JOB_ORDER_NO,COLOR_CD,BUNDLE_NO)AS B)AS BUNDLE, (SELECT SUM(QTY) FROM (SELECT B.BUNDLE_NO,MIN(D.WIP)AS QTY FROM CIPMS_BUNDLE_FOR_SCANNING AS B INNER JOIN CIPMS_JO_WIP_HD AS C ON B.JOB_ORDER_NO=C.JOB_ORDER_NO AND B.COLOR_CD=C.COLOR_CODE AND B.SIZE_CD=C.SIZE_CODE AND B.PART_CD=C.PART_CD AND B.PROCESS_CD=C.PROCESS_CD AND B.PRODUCTION_LINE_CD=C.PRODUCTION_LINE_CD AND B.GARMENT_TYPE=C.GARMENT_TYPE AND B.PROCESS_TYPE=C.PROCESS_TYPE INNER JOIN CIPMS_JO_WIP_BUNDLE AS D ON C.STOCK_ID=D.STOCK_ID AND D.BUNDLE_NO=B.BUNDLE_NO WHERE B.JOB_ORDER_NO=a.JOB_ORDER_NO AND B.COLOR_CD=a.COLOR_CD AND B.DOC_NO='" + doc_no + "' GROUP BY B.JOB_ORDER_NO,B.COLOR_CD,B.BUNDLE_NO)AS B)as TOTAL_QTY, (SELECT SUM(QTY) FROM CUT_BUNDLE_HD WHERE JOB_ORDER_NO=a.JOB_ORDER_NO AND COLOR_CD=a.COLOR_CD GROUP BY JOB_ORDER_NO,COLOR_CD)as CUT_QTY" + sql + " from (select B.JOB_ORDER_NO,B.COLOR_CD,B.SIZE_CD,B.BUNDLE_NO,MIN(D.WIP)AS QTY from CIPMS_BUNDLE_FOR_SCANNING AS B INNER JOIN CIPMS_JO_WIP_HD AS C ON B.JOB_ORDER_NO=C.JOB_ORDER_NO AND B.COLOR_CD=C.COLOR_CODE AND B.SIZE_CD=C.SIZE_CODE AND B.PART_CD=C.PART_CD AND B.PROCESS_CD=C.PROCESS_CD AND B.PRODUCTION_LINE_CD=C.PRODUCTION_LINE_CD AND B.GARMENT_TYPE=C.GARMENT_TYPE AND B.PROCESS_TYPE=C.PROCESS_TYPE INNER JOIN CIPMS_JO_WIP_BUNDLE AS D ON C.STOCK_ID=D.STOCK_ID AND B.BUNDLE_NO=D.BUNDLE_NO WHERE B.DOC_NO='" + doc_no + "' GROUP BY B.JOB_ORDER_NO,B.COLOR_CD,B.SIZE_CD,B.BUNDLE_NO) AS a GROUP BY JOB_ORDER_NO,COLOR_CD ORDER BY COLOR_CD";
            sql = "select JOB_ORDER_NO,COLOR_CD,LAY_NO=stuff((select '，'+CAST(LAY_NO AS VARCHAR(10)) from CIPMS_BUNDLE_FOR_SCANNING with (nolock)  where JOB_ORDER_NO=a.JOB_ORDER_NO and COLOR_CD=a.COLOR_CD AND DOC_NO='" + doc_no + "' GROUP BY LAY_NO for XML path('')),1,1,''), (SELECT COUNT(BUNDLE_NO) FROM (SELECT BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) WHERE JOB_ORDER_NO=a.JOB_ORDER_NO AND COLOR_CD=a.COLOR_CD AND DOC_NO='" + doc_no + "' GROUP BY JOB_ORDER_NO,COLOR_CD,BUNDLE_NO)AS B)AS BUNDLE, (SELECT SUM(QTY) FROM CUT_BUNDLE_HD with (nolock)  WHERE JOB_ORDER_NO=a.JOB_ORDER_NO AND COLOR_CD=a.COLOR_CD GROUP BY JOB_ORDER_NO,COLOR_CD)as CUT_QTY" + sql + " from (select B.JOB_ORDER_NO,B.COLOR_CD,B.SIZE_CD,B.BUNDLE_NO,MIN(D.WIP)AS QTY from CIPMS_BUNDLE_FOR_SCANNING AS B with (nolock) INNER JOIN CIPMS_JO_WIP_HD AS C with (nolock)  ON B.JOB_ORDER_NO=C.JOB_ORDER_NO AND B.COLOR_CD=C.COLOR_CODE AND B.SIZE_CD=C.SIZE_CODE AND B.PART_CD=C.PART_CD AND B.FACTORY_CD=C.FACTORY_CD AND B.PROCESS_CD=C.PROCESS_CD AND B.PRODUCTION_LINE_CD=C.PRODUCTION_LINE_CD AND B.GARMENT_TYPE=C.GARMENT_TYPE AND B.PROCESS_TYPE=C.PROCESS_TYPE INNER JOIN CIPMS_JO_WIP_BUNDLE AS D with (nolock) ON C.STOCK_ID=D.STOCK_ID AND B.BUNDLE_NO=D.BUNDLE_NO WHERE B.DOC_NO='" + doc_no + "' GROUP BY B.JOB_ORDER_NO,B.COLOR_CD,B.SIZE_CD,B.BUNDLE_NO) AS a GROUP BY JOB_ORDER_NO,COLOR_CD ORDER BY COLOR_CD";
            SqlCommand sqlComGet = new SqlCommand(sql, sqlConn);
            sqlComGet.CommandTimeout = 300;
            return sqlComGet;
        }
        public void unscaninsert(SqlConnection sqlConn, string sql)
        {
            sql = "insert into CIPMS_USER_SCANNING_DFT VALUES " + sql;
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }

        public SqlDataReader getcustomer(SqlConnection sqlConn, string go)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.SHORT_NAME from JO_HD as a with (nolock) inner join GEN_CUSTOMER as b with (nolock)  on a.CUSTOMER_CD=b.CUSTOMER_CD where a.SC_NO='" + go + "' group by b.SHORT_NAME";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getsize(SqlConnection sqlConn, string newdocno, string go)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select SIZE_CD from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where DOC_NO='" + newdocno + "' and GARMENT_ORDER_NO='" + go + "' group by SIZE_CD";
            return sqlComGet.ExecuteReader();
        }
        //added on 2018-02-28
        public SqlCommand Getprintpartdetail(SqlConnection sqlConn, string docno, string go)
        {
            string sql = @"select JOB_ORDER_NO,(CASE WHEN ISNULL(CARTON_BARCODE,'')='' THEN '-'ELSE CARTON_BARCODE END) AS CARTON_BARCODE,BARCODE,B.PART_DESC,COLOR_CD,LAY_NO,BUNDLE_NO,SIZE_CD,QTY,CAR_NO 
                                                            FROM CIPMS_BUNDLE_FOR_SCANNING  A with (nolock)  INNER JOIN  CIPMS_PART_MASTER B ON B.PART_CD = A.PART_CD
                                                            WHERE DOC_NO='" + docno + "' AND GARMENT_ORDER_NO='" + go + "' AND PRINT_PART='Y'  ORDER BY JOB_ORDER_NO,LAY_NO,BUNDLE_NO";
            SqlCommand sqlComGet = new SqlCommand(sql, sqlConn);
            sqlComGet.CommandTimeout = 300;
            return sqlComGet;
        }
        //added on 2018-02-28
        public SqlDataReader Getprintpartsummary(SqlConnection sqlConn, string docno, string go)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = @"SELECT STUFF(' '+AA.PART_DESC+CAST(AA.QTY AS NVARCHAR(10))+'/',1,1,'')  
                                                                FROM ( 
                                                                SELECT B.PART_DESC+':' AS PART_DESC,SUM(QTY) AS QTY
                                                                FROM CIPMS_BUNDLE_FOR_SCANNING A with (nolock)  INNER JOIN  CIPMS_PART_MASTER B ON B.PART_CD = A.PART_CD
                                                                WHERE DOC_NO='"+docno+"' AND GARMENT_ORDER_NO='"+go+"'  AND PRINT_PART='Y'  GROUP BY B.PART_DESC ) AA for XML path('') ";                                   
            return sqlComGet.ExecuteReader();
        }


        //public SqlCommand getjocolordb(SqlConnection sqlConn, string sql, string docno, string go)
        //{
        //    sql = "select JOB_ORDER_NO,COLOR_CD,LAY_NO=stuff((select '，'+CAST(LAY_NO AS VARCHAR(10)) from CIPMS_BUNDLE_FOR_SCANNING where JOB_ORDER_NO=a.JOB_ORDER_NO and COLOR_CD=a.COLOR_CD AND DOC_NO='" + docno + "' and GARMENT_ORDER_NO='" + go + "' GROUP BY LAY_NO for XML path('')),1,1,''), (SELECT COUNT(BUNDLE_NO) FROM (SELECT BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO=a.JOB_ORDER_NO AND COLOR_CD=a.COLOR_CD AND DOC_NO='" + docno + "' and GARMENT_ORDER_NO='" + go + "' GROUP BY JOB_ORDER_NO,COLOR_CD,BUNDLE_NO)AS B)AS BUNDLE, (SELECT SUM(QTY) FROM (SELECT BUNDLE_NO,MIN(QTY-DISCREPANCY_QTY)AS QTY FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO=a.JOB_ORDER_NO AND COLOR_CD=a.COLOR_CD AND DOC_NO='" + docno + "' and GARMENT_ORDER_NO='" + go + "' GROUP BY JOB_ORDER_NO,COLOR_CD,BUNDLE_NO)AS B)as TOTAL_QTY, (SELECT SUM(QTY) FROM (SELECT BUNDLE_NO,MIN(QTY)AS QTY FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO=a.JOB_ORDER_NO AND COLOR_CD=a.COLOR_CD GROUP BY JOB_ORDER_NO,COLOR_CD,BUNDLE_NO)AS B)as CUT_QTY, (SELECT SUM(QTY) FROM (SELECT BUNDLE_NO,MIN(QTY)AS QTY FROM CIPMS_BUNDLE_FOR_SCANNING WHERE COLOR_CD=a.COLOR_CD AND DOC_NO='" + docno + "' and GARMENT_ORDER_NO='" + go + "' GROUP BY JOB_ORDER_NO,COLOR_CD,BUNDLE_NO)AS B)AS TTL" + sql + " from CIPMS_BUNDLE_FOR_SCANNING AS a WHERE DOC_NO='" + docno + "' and GARMENT_ORDER_NO='" + go + "' GROUP BY JOB_ORDER_NO,COLOR_CD ORDER BY COLOR_CD";
        //    SqlCommand sqlComGet = new SqlCommand(sql, sqlConn);
        //    return sqlComGet;
        //}
        public SqlDataReader getttlbygocolor(SqlConnection sqlConn, string color, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT SUM(QTY)AS QTY FROM (SELECT MIN(D.WIP)AS QTY FROM CIPMS_BUNDLE_FOR_SCANNING AS B with (nolock)  INNER JOIN CIPMS_JO_WIP_HD AS C with (nolock)  ON B.JOB_ORDER_NO=C.JOB_ORDER_NO AND B.COLOR_CD=C.COLOR_CODE AND B.SIZE_CD=C.SIZE_CODE AND B.PART_CD=C.PART_CD AND B.FACTORY_CD=C.FACTORY_CD AND B.PROCESS_CD=C.PROCESS_CD AND B.PRODUCTION_LINE_CD=C.PRODUCTION_LINE_CD AND B.GARMENT_TYPE=C.GARMENT_TYPE AND B.PROCESS_TYPE=C.PROCESS_TYPE INNER JOIN CIPMS_JO_WIP_BUNDLE AS D ON C.STOCK_ID=D.STOCK_ID AND D.BUNDLE_NO=B.BUNDLE_NO WHERE B.COLOR_CD='" + color + "' AND B.DOC_NO='" + docno + "' GROUP BY B.JOB_ORDER_NO,B.BUNDLE_NO)AS B";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getcolornum(SqlConnection sqlConn, string color, string docno, string go)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(JOB_ORDER_NO)as QTY from (SELECT JOB_ORDER_NO,COLOR_CD FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) where COLOR_CD='" + color + "' and DOC_NO='" + docno + "' and GARMENT_ORDER_NO='" + go + "' GROUP BY JOB_ORDER_NO,COLOR_CD)as a";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader gettransfergo(SqlConnection sqlConn, string doc_no)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select GARMENT_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING with (nolock)  where DOC_NO='" + doc_no + "' group by GARMENT_ORDER_NO";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getjopart(SqlConnection sqlConn, string doc_no, string go)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT JOB_ORDER_NO,a.PART_CD,b.PART_DESC FROM CIPMS_BUNDLE_FOR_SCANNING as a with (nolock) INNER JOIN CIPMS_PART_MASTER AS b with (nolock) on a.PART_CD=b.PART_CD WHERE DOC_NO='" + doc_no + "' AND GARMENT_ORDER_NO='" + go + "' GROUP BY JOB_ORDER_NO,a.PART_CD,b.PART_DESC ORDER BY JOB_ORDER_NO,a.PART_CD,b.PART_DESC";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getcutqty(SqlConnection sqlConn, string jo, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT SUM(QTY)as QTY FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) where JOB_ORDER_NO='" + jo + "' and PART_CD='" + part + "' GROUP BY JOB_ORDER_NO,PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getnowqty(SqlConnection sqlConn, string doc_no, string jo, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT SUM(QTY)as QTY FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock)  WHERE DOC_NO='" + doc_no + "' and JOB_ORDER_NO='" + jo + "' and PART_CD='" + part + "' GROUP BY JOB_ORDER_NO,PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getoutput(SqlConnection sqlConn, string factory, string process, string doc_no, string jo, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT SUM(QTY)as QTY FROM CIPMS_TRANSFER_HD as a with (nolock)  inner join CIPMS_TRANSFER_DT as b with (nolock) on a.DOC_NO=b.DOC_NO where a.FACTORY_CD='" + factory + "' and a.PROCESS_CD='" + process + "' and a.STATUS='C' and a.PROCESS_TYPE='I' and b.JOB_ORDER_NO='" + jo + "' and b.PART_CD='" + part + "' group by b.JOB_ORDER_NO,b.PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getjocolorpart(SqlConnection sqlConn, string doc_no, string go)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT JOB_ORDER_NO,COLOR_CD,a.PART_CD,b.PART_DESC FROM CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_PART_MASTER as b with (nolock) on a.PART_CD=b.PART_CD WHERE DOC_NO='" + doc_no + "' AND GARMENT_ORDER_NO='" + go + "' GROUP BY JOB_ORDER_NO,COLOR_CD,a.PART_CD,b.PART_DESC ORDER BY COLOR_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getcutqty2(SqlConnection sqlConn, string jo, string color, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select SUM(QTY)as QTY from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where JOB_ORDER_NO='" + jo + "' and COLOR_CD='" + color + "' and PART_CD='" + part + "' group by JOB_ORDER_NO,COLOR_CD,PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getcutqty1(SqlConnection sqlConn, string jo, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select SUM(QTY)as QTY from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where JOB_ORDER_NO='" + jo + "' and PART_CD='" + part + "' group by JOB_ORDER_NO,PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getnowqty2(SqlConnection sqlConn, string doc_no, string go, string jo, string color, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT SUM(QTY-DISCREPANCY_QTY)as QTY FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) WHERE DOC_NO='" + doc_no + "' and GARMENT_ORDER_NO='" + go + "' and JOB_ORDER_NO='" + jo + "' and COLOR_CD='" + color + "' and PART_CD='" + part + "' GROUP BY JOB_ORDER_NO,COLOR_CD,PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getnowqty1(SqlConnection sqlConn, string doc_no, string go, string jo, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT SUM(QTY-DISCREPANCY_QTY)as QTY FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) WHERE DOC_NO='" + doc_no + "' and GARMENT_ORDER_NO='" + go + "' and JOB_ORDER_NO='" + jo + "' and PART_CD='" + part + "' GROUP BY JOB_ORDER_NO,PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getoutput2(SqlConnection sqlConn, string factory, string process, string doc_no, string jo, string color, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT SUM(QTY)as QTY FROM CIPMS_TRANSFER_HD as a  with (nolock) inner join CIPMS_TRANSFER_DT as b  with (nolock) on a.DOC_NO=b.DOC_NO where a.FACTORY_CD='" + factory + "' and a.PROCESS_CD='" + process + "' and a.NEXT_PROCESS_CD<>'" + process + "' and a.STATUS='C' and a.PROCESS_TYPE='I' and b.JOB_ORDER_NO='" + jo + "' and b.COLOR_CD='" + color + "' and b.PART_CD='" + part + "' group by b.JOB_ORDER_NO,b.COLOR_CD,b.PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getoutput1(SqlConnection sqlConn, string factory, string process, string doc_no, string jo, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT SUM(QTY)as QTY FROM CIPMS_TRANSFER_HD as a with (nolock) inner join CIPMS_TRANSFER_DT as b with (nolock) on a.DOC_NO=b.DOC_NO where a.FACTORY_CD='" + factory + "' and a.PROCESS_CD='" + process + "' and a.NEXT_PROCESS_CD<>'" + process + "' and a.STATUS='C' and a.PROCESS_TYPE='I' and b.JOB_ORDER_NO='" + jo + "' and b.PART_CD='" + part + "' group by b.JOB_ORDER_NO,b.PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getresidualqty(SqlConnection sqlConn, string factory, string process, string jo, string color, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select SUM(IN_QTY-OUT_QTY)AS QTY from CIPMS_JO_WIP_HD with (nolock)  where FACTORY_CD='" + factory + "' and JOB_ORDER_NO='" + jo + "' and COLOR_CODE='" + color + "' and PART_CD='" + part + "' and PROCESS_CD='" + process + "' and PROCESS_TYPE='I' group by JOB_ORDER_NO,COLOR_CODE,PART_CD,PROCESS_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getresidualqty1(SqlConnection sqlConn, string factory, string process, string jo, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select SUM(IN_QTY-OUT_QTY)AS QTY from CIPMS_JO_WIP_HD with (nolock) where FACTORY_CD='" + factory + "' and JOB_ORDER_NO='" + jo + "' and PART_CD='" + part + "' and PROCESS_CD='" + process + "' and PROCESS_TYPE='I' group by JOB_ORDER_NO,PART_CD,PROCESS_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkdocnostatus(SqlConnection sqlConn, string doc_no)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            //sqlComGet.CommandText = "select DOC_NO from CIPMS_TRANSFER_HD where DOC_NO='" + doc_no + "' and (STATUS<>'C' OR STATUS<>'R' OR STATUS<>'U')";
            sqlComGet.CommandText = "select DOC_NO from CIPMS_TRANSFER_HD with (nolock) where DOC_NO='" + doc_no + "' and STATUS='S'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkdocnobundles1(SqlConnection sqlConn, string doc_no, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select JOB_ORDER_NO,BUNDLE_NO,PART_CD from CIPMS_TRANSFER_DT with (nolock)  where DOC_NO='" + doc_no + "' except select b.JOB_ORDER_NO,b.BUNDLE_NO,b.PART_CD from CIPMS_USER_SCANNING_DFT as a  with (nolock)  inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkdocnobundles2(SqlConnection sqlConn, string doc_no, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.JOB_ORDER_NO,b.BUNDLE_NO,b.PART_CD from CIPMS_USER_SCANNING_DFT as a with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b with (nolock)  on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' except select JOB_ORDER_NO,BUNDLE_NO,PART_CD from CIPMS_TRANSFER_DT with (nolock) where DOC_NO='" + doc_no + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkdocnoinprocess(SqlConnection sqlConn, string doc_no, string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select DOC_NO from CIPMS_TRANSFER_HD with (nolock) where DOC_NO='" + doc_no + "' and PROCESS_CD='" + process + "'";
            return sqlComGet.ExecuteReader();
        }
        public void updatedocnostatus(SqlConnection sqlConn, string doc_no)
        {
            string sql = "update CIPMS_TRANSFER_HD set STATUS='U' where DOC_NO='" + doc_no + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
            sql = "UPDATE CIPMS_JO_WIP_BUNDLE SET INTRANS_QTY=A.INTRANS_QTY-B.WIP-(CASE WHEN B.STATUS='A' OR B.STATUS='M' THEN B.DISCREPANCY_QTY ELSE 0 END),STATUS='C' FROM CIPMS_JO_WIP_BUNDLE AS A,(select D.STOCK_ID,D.BUNDLE_NO,D.PART_CD,D.WIP,D.DISCREPANCY_QTY,A.STATUS from CIPMS_TRANSFER_HD AS A with (nolock)  INNER JOIN CIPMS_TRANSFER_DT AS B with (nolock)  ON A.DOC_NO=B.DOC_NO INNER JOIN CIPMS_JO_WIP_HD AS C with (nolock) ON C.JOB_ORDER_NO=B.JOB_ORDER_NO AND C.COLOR_CODE=B.COLOR_CD AND C.SIZE_CODE=B.SIZE_CD AND C.PART_CD=B.PART_CD AND C.PROCESS_CD=A.PROCESS_CD AND C.PRODUCTION_LINE_CD=A.PRODUCTION_LINE_CD AND C.GARMENT_TYPE='K' AND C.PROCESS_TYPE='I' INNER JOIN CIPMS_JO_WIP_BUNDLE AS D with (nolock) ON C.STOCK_ID=D.STOCK_ID AND D.BUNDLE_NO=B.BUNDLE_NO WHERE A.DOC_NO='" + doc_no + "')AS B WHERE A.STOCK_ID=B.STOCK_ID AND A.BUNDLE_NO=B.BUNDLE_NO AND A.PART_CD=B.PART_CD";
            cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
            sql = "UPDATE CIPMS_JO_WIP_HD SET INTRANS_QTY=A.INTRANS_QTY-B.WIP-(CASE WHEN B.STATUS='A' OR B.STATUS='M' THEN B.DISCREPANCY_QTY ELSE 0 END) FROM CIPMS_JO_WIP_HD AS A,(select B.JOB_ORDER_NO,B.COLOR_CD,B.SIZE_CD,B.PART_CD,A.PROCESS_CD,A.PRODUCTION_LINE_CD,SUM(D.WIP)AS WIP,SUM(D.DISCREPANCY_QTY)AS DISCREPANCY_QTY,A.STATUS from CIPMS_TRANSFER_HD AS A with (nolock) INNER JOIN CIPMS_TRANSFER_DT AS B with (nolock) ON A.DOC_NO=B.DOC_NO INNER JOIN CIPMS_JO_WIP_HD AS C ON C.JOB_ORDER_NO=B.JOB_ORDER_NO AND C.COLOR_CODE=B.COLOR_CD AND C.SIZE_CODE=B.SIZE_CD AND C.PART_CD=B.PART_CD AND C.PROCESS_CD=A.PROCESS_CD AND C.PRODUCTION_LINE_CD=A.PRODUCTION_LINE_CD AND C.GARMENT_TYPE='K' AND C.PROCESS_TYPE='I' INNER JOIN CIPMS_JO_WIP_BUNDLE AS D with (nolock) ON C.STOCK_ID=D.STOCK_ID AND D.BUNDLE_NO=B.BUNDLE_NO WHERE A.DOC_NO='" + doc_no + "' GROUP BY B.JOB_ORDER_NO,B.COLOR_CD,B.SIZE_CD,B.PART_CD,A.PROCESS_CD,A.PRODUCTION_LINE_CD,A.STATUS)AS B WHERE A.JOB_ORDER_NO=B.JOB_ORDER_NO AND A.COLOR_CODE=B.COLOR_CD AND A.SIZE_CODE=B.SIZE_CD AND A.PART_CD=B.PART_CD AND A.PROCESS_CD=B.PROCESS_CD AND A.PRODUCTION_LINE_CD=B.PRODUCTION_LINE_CD AND A.GARMENT_TYPE='K' AND A.PROCESS_TYPE='I'";
            cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader querybundlepart(SqlConnection sqlConn, string doc_no, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.FACTORY_CD,PROCESS_CD,PRODUCTION_LINE_CD,BUNDLE_ID,BARCODE,a.PART_CD,b.PART_DESC,CARTON_BARCODE,CARTON_STATUS,QTY,DISCREPANCY_QTY,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,COLOR_CD,SIZE_CD,DEFECT from CIPMS_BUNDLE_FOR_SCANNING as a with (nolock) inner join CIPMS_PART_MASTER as b with (nolock)  on a.PART_CD=b.PART_CD where DOC_NO='" + doc_no + "' and BUNDLE_ID not in (select BUNDLE_ID from CIPMS_USER_SCANNING_DFT with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checksubmit(SqlConnection sqlConn, string doc_no)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT DOC_NO FROM CIPMS_TRANSFER_HD with (nolock)  where DOC_NO='" + doc_no + "' and STATUS='S'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checksubmit(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getdocno(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.DOC_NO from CIPMS_USER_SCANNING_DFT as a with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b with (nolock)  on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' group by b.DOC_NO";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getdocnoinformation(SqlConnection sqlConn, string doc_no)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.FACTORY_CD,a.PROCESS_CD,a.NEXT_PROCESS_CD,a.PRODUCTION_LINE_CD,a.NEXT_PRODUCTION_LINE_CD,b.NAME,a.CREATE_DATE from CIPMS_TRANSFER_HD as a with (nolock) inner join CIPMS_USER as b on a.CREATE_USER_ID=b.USER_BARCODE where a.DOC_NO='" + doc_no + "'";
            return sqlComGet.ExecuteReader();
        }

        public SqlDataReader checkreprint(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select distinct A.DOC_NO from CIPMS_BUNDLE_FOR_SCANNING AS A with (nolock) INNER JOIN CIPMS_USER_SCANNING_DFT AS B with (nolock) ON A.BUNDLE_ID=B.BUNDLE_ID WHERE B.DOC_NO='" + docno + "' AND B.USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
    }
}