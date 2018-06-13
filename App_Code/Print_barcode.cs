using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;

/// <summary>
///Printbarcode 的摘要说明
/// </summary>
/// 
namespace PrintBarcode
{
    public class Print_barcode
    {
        public Print_barcode()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        //查找该JO的制单数
        public SqlDataReader joqty(SqlConnection sqlConn, string joborderno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select SUM(QTY)as JOQTY from CUT_BUNDLE_HD with (nolock) where JOB_ORDER_NO='" + joborderno + "' AND STATUS='Y'";
            return sqlComGet.ExecuteReader();
        }
        //查询JO的详细信息
        public SqlDataReader jolaynosku(SqlConnection sqlConn, string joborderno, string layno, string bundlelist1)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
       
           //sqlComGet.CommandText = @"SELECT E.JOB_ORDER_NO,E.BUNDLE_NO,E.BARCODE,E.QTY,E.CUT_LINE,E.SIZE_CD,E.COLOR_CD,E.LAY_NO,B.BATCH_NO,D.SHORT_NAME,C.STYLE_NO,A.LAY_TRANS_ID,B.SHADE_LOT,F.PATTERN_NO FROM CUT_BUNDLE_HD AS A INNER JOIN CUT_LAY_DT AS B ON A.LAY_TRANS_ID = B.LAY_TRANS_ID AND A.lay_dt_id = B.lay_dt_id INNER JOIN JO_HD AS C ON C.JO_NO=A.JOB_ORDER_NO INNER JOIN GEN_CUSTOMER AS D ON C.CUSTOMER_CD=D.CUSTOMER_CD INNER JOIN CIPMS_BUNDLE_FOR_SCANNING AS E on E.JOB_ORDER_NO=A.JOB_ORDER_NO AND E.BUNDLE_NO=A.BUNDLE_NO INNER JOIN CUT_LAY AS F ON A.JOB_ORDER_NO=F.JOB_ORDER_NO AND A.LAY_NO=F.LAY_NO WHERE A.JOB_ORDER_NO='" + joborderno + "' AND A.LAY_NO=" + layno + " AND A.TRX_TYPE='NM' and A.STATUS='Y' GROUP BY E.JOB_ORDER_NO,E.BUNDLE_NO,E.BARCODE,E.QTY,E.CUT_LINE,E.SIZE_CD,E.COLOR_CD,E.LAY_NO,B.BATCH_NO,D.SHORT_NAME,C.STYLE_NO,A.LAY_TRANS_ID,B.SHADE_LOT,F.PATTERN_NO ORDER BY A.LAY_TRANS_ID";

            sqlComGet.CommandText = @"
                        DECLARE @SQL AS NVARCHAR(MAX)
                        SET @SQL='SELECT  E.JOB_ORDER_NO , E.BUNDLE_NO , E.BARCODE , E.QTY , E.CUT_LINE , E.SIZE_CD , E.COLOR_CD , E.LAY_NO ,'
                        IF EXISTS(SELECT   1   FROM  dbo.GEN_SYSTEM_SETTING WHERE SYSTEM_KEY='CIPMS_PRINT_BARCODE_DISPLAY_CUTLOT' AND SYSTEM_VALUE='Y')
                        BEGIN
	                        SET @SQL+=' B.BATCH_NO+''/''+CAST(F.CUT_TABLE_NO AS NVARCHAR(10)) AS BATCH_NO,'   
                        END;
                        ELSE
                        BEGIN
	                        SET @SQL+=' B.BATCH_NO,'
                        END;
                        SET @SQL+=' D.SHORT_NAME , C.STYLE_NO , A.LAY_TRANS_ID , B.SHADE_LOT , F.PATTERN_NO 
                                FROM    CUT_BUNDLE_HD AS A
                                INNER JOIN CUT_LAY_DT AS B ON A.LAY_TRANS_ID = B.LAY_TRANS_ID AND A.LAY_DT_ID = B.LAY_DT_ID INNER JOIN JO_HD AS C ON C.JO_NO = A.JOB_ORDER_NO
                                INNER JOIN GEN_CUSTOMER AS D ON C.CUSTOMER_CD = D.CUSTOMER_CD
                                INNER JOIN CIPMS_BUNDLE_FOR_SCANNING AS E ON E.JOB_ORDER_NO = A.JOB_ORDER_NO AND E.BUNDLE_NO = A.BUNDLE_NO 
                                INNER JOIN CUT_LAY AS F ON A.JOB_ORDER_NO = F.JOB_ORDER_NO AND A.LAY_NO = F.LAY_NO'
                        SET @SQL+=' WHERE  (    ''" + bundlelist1 + "''    ='','' or a.bundle_no in (select FNField from FN_SPLIT_STRING_TB(''" + bundlelist1 + "'' ,'',''))) and A.JOB_ORDER_NO = ''" + joborderno + "''  AND A.LAY_NO =" + layno + @" '
                        SET @SQL+=' AND A.TRX_TYPE = ''NM''  AND A.STATUS = ''Y'''
                        SET @SQL+=' GROUP BY E.JOB_ORDER_NO , E.BUNDLE_NO , E.BARCODE , E.QTY , E.CUT_LINE , E.SIZE_CD , E.COLOR_CD , E.LAY_NO , B.BATCH_NO , D.SHORT_NAME , C.STYLE_NO , A.LAY_TRANS_ID , B.SHADE_LOT , F.PATTERN_NO'
                        IF EXISTS(SELECT   1   FROM  dbo.GEN_SYSTEM_SETTING WHERE SYSTEM_KEY='CIPMS_PRINT_BARCODE_DISPLAY_CUTLOT' AND SYSTEM_VALUE='Y')
                        BEGIN
                        SET @SQL+=',F.CUT_TABLE_NO '
                        END;
                        SET @SQL+=' ORDER BY A.LAY_TRANS_ID'
                        EXECUTE sp_executeSql @SQL ";

            return sqlComGet.ExecuteReader();
        }

        public SqlDataReader jolayisexist(SqlConnection sqlConn, string joborderno, string layno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select distinct JOB_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING where JOB_ORDER_NO='" + joborderno + "' and LAY_NO=" + layno;
            return sqlComGet.ExecuteReader();
        }
        public void insertbundleinformation(SqlConnection sqlConn, string joborderno, string layno, string userbarcode)
        {
            string sql = "insert into CIPMS_BUNDLE_FOR_SCANNING (FACTORY_CD,PROCESS_CD,PRODUCTION_LINE_CD,JOB_ORDER_NO,SIZE_CD,COLOR_CD,LAY_NO,CUT_LINE,BUNDLE_NO,BARCODE,PART_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_BARCODE,DOC_NO,GARMENT_TYPE,PROCESS_TYPE,USER_CREATE_ID,CREATE_DATE,GARMENT_ORDER_NO)select a.FACTORY_CD,'CUT',a.CUT_LINE,a.JOB_ORDER_NO,a.SIZE_CD,a.COLOR_CD,a.LAY_NO,a.CUT_LINE,a.BUNDLE_NO,0,g.PART_CD,a.QTY,0,0,0,0,a.GARMENT_TYPE,a.PROCESS_TYPE,'" + userbarcode + "',GETDATE(),c.SC_NO from CUT_BUNDLE_HD as ainner join cut_lay_dt as b on a.LAY_TRANS_ID = b.LAY_TRANS_ID and a.lay_dt_id = b.lay_dt_id inner join JO_HD as c on c.JO_NO=a.JOB_ORDER_NO inner join GEN_CUSTOMER as d on c.CUSTOMER_CD=d.CUSTOMER_CD inner join PRD_GO_ROUTE_LIST_HD as e on e.GO_NO=c.SC_NO inner join PRD_GO_ROUTE_LIST_DT as f on e.ROUTE_HD_ID = f.ROUTE_HD_ID inner join CIPMS_PART_MASTER as g on f.PART_DESC=g.PART_DESC where a.JOB_ORDER_NO='" + joborderno + "' and a.LAY_NO=" + layno + " group by g.PART_CD,c.SC_NO,a.JOB_ORDER_NO,a.BUNDLE_NO,a.FACTORY_CD,a.QTY,a.CUT_LINE,a.SIZE_CD,a.COLOR_CD,a.LAY_NO,a.GARMENT_TYPE,a.PROCESS_TYPE,a.PRODUCTION_FACTORY,b.BATCH_NO,d.SHORT_NAME,c.STYLE_NO";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void insertupdatewiphd(SqlConnection sqlConn, string factory, string processtype, string garmenttype, string joborderno, string layno)
        {
            string sql = "insert into CIPMS_JO_WIP_HD (FACTORY_CD,JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PROCESS_CD,PRODUCTION_LINE_CD,PROCESS_TYPE,PART_CD,IN_QTY,OUT_QTY,DISCREPANCY_QTY,WIP,INTRANS_QTY,INTRANS_IN,INTRANS_OUT,BUNDLE_REDUCE,MATCHING,GARMENT_TYPE,PRODUCTION_FACTORY)select '" + factory + "',a.JOB_ORDER_NO,a.COLOR_CD,a.SIZE_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,'" + processtype + "',a.PART_CD,0,0,0,0,0,0,0,0,0,'" + garmenttype + "','" + factory + "' from (select JOB_ORDER_NO,COLOR_CD,SIZE_CD,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD from CIPMS_BUNDLE_FOR_SCANNING where JOB_ORDER_NO='" + joborderno + "' and LAY_NO=" + layno + " group by JOB_ORDER_NO,COLOR_CD,SIZE_CD,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD except select JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD from CIPMS_JO_WIP_HD)as a";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
            sql = "update CIPMS_JO_WIP_HD set IN_QTY=a.IN_QTY+b.QTY,WIP=a.WIP+b.QTY from CIPMS_JO_WIP_HD as a, (select JOB_ORDER_NO,COLOR_CD,SIZE_CD,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD,SUM(QTY)as QTY from CIPMS_BUNDLE_FOR_SCANNING where JOB_ORDER_NO='" + joborderno + "' and LAY_NO=" + layno + " group by JOB_ORDER_NO,COLOR_CD,SIZE_CD,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD) as b where a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CODE=b.COLOR_CD and a.SIZE_CODE=b.SIZE_CD and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD";
            cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void insertwipbundle(SqlConnection sqlConn, string joborderno, string layno)
        {
            string sql = "insert into CIPMS_JO_WIP_BUNDLE (STOCK_ID,BUNDLE_NO,IN_QTY,OUT_QTY,WIP,INTRANS_QTY,DISCREPANCY_QTY,MATCHING,DEFECT,TRANSFER_IN,TRANSFER_OUT,PART_CD,EMPLOYEE_OUTPUT)select b.STOCK_ID,a.BUNDLE_NO,a.QTY,0,a.QTY,0,0,0,0,0,0,a.PART_CD,'0' from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_JO_WIP_HD as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CODE and a.SIZE_CD=b.SIZE_CODE and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD where a.JOB_ORDER_NO='" + joborderno + "' and a.LAY_NO=" + layno;
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader bundleseq(SqlConnection sqlConn, string factory, string barcodetype)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select SEQUENCE_NO from CIPMS_BARCODE_SEQUENCENO where FACTORY_CD='" + factory + "' and BARCODE_TYPE='" + barcodetype + "' and PRINT_BARCODE='1'";
            return sqlComGet.ExecuteReader();
        }
    }
}