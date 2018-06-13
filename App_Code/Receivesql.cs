using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;

/// <summary>
///Receivesql 的摘要说明
/// </summary>
/// 
namespace NameSpace
{
    public class Receivesql
    {
        public Receivesql()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        public SqlDataReader shipperdocnostatus(SqlConnection sqlConn, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select STATUS from CIPMS_TRANSFER_HD with (nolock) where DOC_NO='" + docno + "'";
            return sqlComGet.ExecuteReader();
        }
        public void shippertransferhd(SqlConnection sqlConn, string barcode, string status, string userbarcode)
        {
            string sql = "update CIPMS_TRANSFER_HD set STATUS='" + status + "', LAST_MODI_USER_ID='" + userbarcode + "',LAST_MODI_DATE=GETDATE(),CONFIRM_USER_ID='" + userbarcode + "',CONFIRM_DATE=GETDATE() where DOC_NO='" + barcode + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void shipperupdatewiphd(SqlConnection sqlConn, string barcode)
        {
            string sql = "update CIPMS_JO_WIP_HD set WIP=a.WIP-d.QTY,OUT_QTY=a.OUT_QTY+d.QTY,INTRANS_QTY=a.INTRANS_QTY-d.QTY from CIPMS_JO_WIP_HD as a with (nolock) , (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD,(SUM(c.QTY)-SUM(c.DISCREPANCY_QTY))as QTY from CIPMS_TRANSFER_DT as b with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as c with (nolock) on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.BUNDLE_NO=c.BUNDLE_NO and b.PART_CD=c.PART_CD where b.DOC_NO='" + barcode + "' group by c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD)as d where a.JOB_ORDER_NO=d.JOB_ORDER_NO and a.COLOR_CODE=d.COLOR_CD and a.SIZE_CODE=d.SIZE_CD and a.PART_CD=d.PART_CD and a.PROCESS_CD=d.PROCESS_CD and a.PRODUCTION_LINE_CD=d.PRODUCTION_LINE_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void shipperupdatewiphdrework(SqlConnection sqlConn, string docno)
        {
            string sql = "update CIPMS_JO_WIP_HD set WIP=a.WIP-d.QTY,INTRANS_QTY=a.INTRANS_QTY-d.QTY from CIPMS_JO_WIP_HD as a, (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD,(SUM(c.QTY)-SUM(c.DISCREPANCY_QTY))as QTY from CIPMS_TRANSFER_DT as b with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as c with (nolock) on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.BUNDLE_NO=c.BUNDLE_NO and b.PART_CD=c.PART_CD where b.DOC_NO='" + docno + "' group by c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD)as d where a.JOB_ORDER_NO=d.JOB_ORDER_NO and a.COLOR_CODE=d.COLOR_CD and a.SIZE_CODE=d.SIZE_CD and a.PART_CD=d.PART_CD and a.PROCESS_CD=d.PROCESS_CD and a.PRODUCTION_LINE_CD=d.PRODUCTION_LINE_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void shipperinsertwiphd(SqlConnection sqlConn, string factory, string processtype, string garmenttype, string docno)
        {
            string sql = "insert into CIPMS_JO_WIP_HD (FACTORY_CD,JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PROCESS_CD,PRODUCTION_LINE_CD,PROCESS_TYPE,PART_CD,IN_QTY,OUT_QTY,DISCREPANCY_QTY,WIP,INTRANS_QTY,INTRANS_IN,INTRANS_OUT,BUNDLE_REDUCE,MATCHING,GARMENT_TYPE,PRODUCTION_FACTORY)select '" + factory + "',a.JOB_ORDER_NO,a.COLOR_CD,a.SIZE_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,'" + processtype + "',a.PART_CD,0,0,0,0,0,0,0,0,0,'" + garmenttype + "','" + factory + "' from ((select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,d.NEXT_PROCESS_CD,d.NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_DT as c with (nolock) inner join CIPMS_TRANSFER_HD as d with (nolock)  on c.DOC_NO=d.DOC_NO where c.DOC_NO='" + docno + "' group by c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,d.NEXT_PROCESS_CD,d.NEXT_PRODUCTION_LINE_CD)as b except select JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD from CIPMS_JO_WIP_HD with (nolock) )as a";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
            sql = "update CIPMS_JO_WIP_HD set WIP=a.WIP+b.QTY,IN_QTY=a.IN_QTY+b.QTY from CIPMS_JO_WIP_HD as a, (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,d.NEXT_PROCESS_CD,d.NEXT_PRODUCTION_LINE_CD,SUM(c.QTY)as QTY from CIPMS_TRANSFER_DT as c inner join CIPMS_TRANSFER_HD as d on c.DOC_NO=d.DOC_NO where c.DOC_NO='" + docno + "' group by c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,d.NEXT_PROCESS_CD,d.NEXT_PRODUCTION_LINE_CD)as b where a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CODE=b.COLOR_CD and a.SIZE_CODE=b.SIZE_CD and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.NEXT_PROCESS_CD and a.PRODUCTION_LINE_CD=b.NEXT_PRODUCTION_LINE_CD";
            cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void shipperupdatewipbundle(SqlConnection sqlConn, string docno)
        {
            string sql = "update CIPMS_JO_WIP_BUNDLE set WIP=a.WIP-e.QTY,OUT_QTY=a.OUT_QTY+e.QTY,INTRANS_QTY=a.INTRANS_QTY-e.QTY from CIPMS_JO_WIP_BUNDLE as a with (nolock) , (select d.STOCK_ID,b.BUNDLE_NO,b.QTY from CIPMS_TRANSFER_DT as b with (nolock) inner join CIPMS_TRANSFER_HD as c  with (nolock) on b.DOC_NO=c.DOC_NO inner join CIPMS_JO_WIP_HD as d with (nolock) on d.JOB_ORDER_NO=b.JOB_ORDER_NO and d.COLOR_CODE=b.COLOR_CD and d.SIZE_CODE=b.SIZE_CD and d.PART_CD=b.PART_CD and d.PROCESS_CD=c.PROCESS_CD and d.PRODUCTION_LINE_CD=c.PRODUCTION_LINE_CD where b.DOC_NO=" + docno + ")as e where a.STOCK_ID=e.STOCK_ID and a.BUNDLE_NO=e.BUNDLE_NO";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void shipperupdatewipbundlerework(SqlConnection sqlConn, string docno)
        {
            string sql = "update CIPMS_JO_WIP_BUNDLE set WIP=a.WIP-e.QTY,INTRANS_QTY=a.INTRANS_QTY-e.QTY from CIPMS_JO_WIP_BUNDLE as a with (nolock) , (select d.STOCK_ID,b.BUNDLE_NO,b.QTY from CIPMS_TRANSFER_DT as b with (nolock) inner join CIPMS_TRANSFER_HD as c with (nolock)  on b.DOC_NO=c.DOC_NO inner join CIPMS_JO_WIP_HD as d with (nolock)  on d.JOB_ORDER_NO=b.JOB_ORDER_NO and d.COLOR_CODE=b.COLOR_CD and d.SIZE_CODE=b.SIZE_CD and d.PART_CD=b.PART_CD and d.PROCESS_CD=c.PROCESS_CD and d.PRODUCTION_LINE_CD=c.PRODUCTION_LINE_CD where b.DOC_NO='" + docno + "')as e where a.STOCK_ID=e.STOCK_ID and a.BUNDLE_NO=e.BUNDLE_NO";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void shipperinsertwipbundle(SqlConnection sqlConn, string docno)
        {
            string sql = "insert into CIPMS_JO_WIP_BUNDLE (STOCK_ID,BUNDLE_NO,IN_QTY,OUT_QTY,WIP,INTRANS_QTY,DISCREPANCY_QTY,MATCHING,DEFECT,TRANSFER_IN,TRANSFER_OUT,PART_CD,EMPLOYEE_OUTPUT) select a.STOCK_ID,b.BUNDLE_NO,b.QTY,0,b.QTY,0,0,0,0,0,0,b.PART_CD,'0' from CIPMS_JO_WIP_HD as a with (nolock) inner join CIPMS_TRANSFER_DT as b with (nolock) on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CODE=b.COLOR_CD and a.SIZE_CODE=b.SIZE_CD and a.PART_CD=b.PART_CD inner join CIPMS_TRANSFER_HD as c with (nolock) on b.DOC_NO=c.DOC_NO and a.PROCESS_CD=c.NEXT_PROCESS_CD and a.PRODUCTION_LINE_CD=c.NEXT_PRODUCTION_LINE_CD where b.DOC_NO='" + docno + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void shipperupdatebfs(SqlConnection sqlConn, string docno, string userbarcode)
        {
            string sql = "update CIPMS_BUNDLE_FOR_SCANNING set PROCESS_CD=b.NEXT_PROCESS_CD, PRODUCTION_LINE_CD=b.NEXT_PRODUCTION_LINE_CD,LAST_MODI_USER_ID='" + userbarcode + "',LAST_MODI_DATE=GETDATE() from CIPMS_BUNDLE_FOR_SCANNING as a with (nolock) , (select c.JOB_ORDER_NO,c.BUNDLE_NO,c.PART_CD,d.NEXT_PROCESS_CD,d.NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_DT as c with (nolock)  inner join CIPMS_TRANSFER_HD as d with (nolock)  on c.DOC_NO=d.DOC_NO where c.DOC_NO='" + docno + "') as b where a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO and a.PART_CD=b.PART_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatecarton(SqlConnection sqlConn, string userbarcode, string process, string processtype, string docno)
        {
            string sql = "update CIPMS_CARTON set LAST_MODI_USER_ID='" + userbarcode + "',LAST_MODI_USER_ID=GETDATE(),PROCESS_CD='" + process + "',PROCESS_TYPE='" + processtype + "' where CARTON_BARCODE in (select CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING as a with (nolock) where DOC_NO='" + docno + "' and CARTON_STATUS='C' group by CARTON_BARCODE)";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }

        public SqlDataReader peerdocnostatus(SqlConnection sqlConn, string docno, string peerfactorydblink)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select STATUS from " + peerfactorydblink + " where DOC_NO='+docno+'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader peergetjo(SqlConnection sqlConn, string factory, string process, string barcode, string peerfactorydblink)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select JOB_ORDER_NO from PRD_JO_PROCESS_WIP_CONTROL with (nolock) where JOB_ORDER_NO in(select JOB_ORDER_NO from " + peerfactorydblink + " where FACTORY_CD='" + factory + "' and DOC_NO='" + barcode + "' group by JOB_ORDER_NO) and PROCESS_CD='" + process + "' and WIP_CONTROL_BY_PRD_LINE='Y' and GARMENT_TYPE='K' and PROCESS_TYPE='I'";
            return sqlComGet.ExecuteReader();
        }
        public void createbundleforscantemptable(SqlConnection sqlConn, string factory, string userbarcode, string barcode, string peerfactorydblink)
        {
            string sql = "insert into CIPMS_BUNDLE_FOR_SCANNING (FACTORY_CD,PROCESS_CD,PRODUCTION_LINE_CD,JOB_ORDER_NO,SIZE_CD,COLOR_CD,LAY_NO,CUT_LINE,BUNDLE_NO,BARCODE,PART_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_BARCODE,CARTON_STATUS,DOC_NO,GARMENT_TYPE,PROCESS_TYPE,USER_CREATE_ID,CREATE_DATE,LAST_MODI_USER_ID,LAST_MODI_DATE,GARMENT_ORDER_NO)select '"+factory+"',PROCESS_CD,PRODUCTION_LINE_CD,JOB_ORDER_NO,SIZE_CD,COLOR_CD,LAY_NO,CUT_LINE,BUNDLE_NO,BARCODE,PART_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_BARCODE,CARTON_STATUS,DOC_NO,GARMENT_TYPE,PROCESS_TYPE,USER_CREATE_ID,CREATE_DATE,'"+userbarcode+"',GETDATE(),GARMENT_ORDER_NO from "+peerfactorydblink+" where DOC_NO='"+barcode+"'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void insertpeerbundleforscan(SqlConnection sqlConn, string sql, string nextfactory, string nextprocess, string nextproductionline, string userbarcode, string peerfactorydblink)
        {
            string sql1 = "";
            sql1 = "insert into " + peerfactorydblink + " (FACTORY_CD,PROCESS_CD,PRODUCTION_LINE_CD,JOB_ORDER_NO,SIZE_CD,COLOR_CD,LAY_NO,CUT_LINE,BUNDLE_NO,BARCODE,PART_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_BARCODE,CARTON_STATUS,DOC_NO,GARMENT_TYPE,PROCESS_TYPE,USER_CREATE_ID,CREATE_DATE,LAST_MODI_USER_ID,LAST_MODI_DATE,GARMENT_ORDER_NO)select '" + nextfactory + "','" + nextprocess + "','" + nextproductionline + "',c.JOB_ORDER_NO,c.SIZE_CD,c.COLOR_CD,c.LAY_NO,c.CUT_LINE,c.BUNDLE_NO,c.BARCODE,c.PART_CD,c.QTY,c.DEFECT,c.DISCREPANCY_QTY,c.CARTON_BARCODE,c.CARTON_STATUS,c.DOC_NO,c.GARMENT_TYPE,c.PROCESS_TYPE,c.USER_CREATE_ID,c.CREATE_DATE,'" + userbarcode + "',GETDATE(),c.GARMENT_ORDER_NO from (select a.JOB_ORDER_NO,a.BUNDLE_NO,a.PART_CD,a.QTY from CIPMS_TRANSFER_DT as a left join #" + userbarcode + "_BUNDLE_FOR_SCANNING as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO and a.PART_CD=b.PART_CD where a.DOC_NO='' and b.JOB_ORDER_NO is null)as d inner join CIPMS_BUNDLE_FOR_SCANNING as c on d.JOB_ORDER_NO=c.JOB_ORDER_NO and d.BUNDLE_NO=c.BUNDLE_NO and d.PART_CD=c.PART_CD";
            SqlCommand cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
            sql1 = "update " + peerfactorydblink + " set FACTORY_CD='" + nextfactory + "',PROCESS_CD='" + nextprocess + "',PRODUCTION_LINE_CD='" + nextproductionline + "',LAST_MODI_USER_ID='" + userbarcode + "',LAST_MODI_DATE=GETDATE(),QTY=d.QTY,DEFECT=d.DEFECT,DISCREPANCY_QTY=d.DISCREPANCY_QTY,CARTON_BARCODE=d.CARTON_BARCODE,CARTON_STATUS=d.CARTON_STATUS,DOC_NO=d.DOC_NO from " + peerfactorydblink + " as a, (select c.JOB_ORDER_NO,c.BUNDLE_NO,c.PART_CD,c.QTY,c.DEFECT,c.DISCREPANCY_QTY,c.CARTON_BARCODE,c.CARTON_STATUS,c.DOC_NO from CIPMS_TRANSFER_DT as b inner join CIPMS_BUNDLE_FOR_SCANNING as c on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.BUNDLE_NO=c.BUNDLE_NO and b.PART_CD=c.PART_CD where b.DOC_NO in" + sql + ")as d where a.JOB_ORDER_NO=d.JOB_ORDER_NO and a.BUNDLE_NO=d.BUNDLE_NO and a.PART_CD=d.PART_CD";
            cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader needchooseproduction(SqlConnection sqlConn, string factory, string process, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.JOB_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING as a with (nolock)  inner join PRD_JO_PROCESS_WIP_CONTROL as b with (nolock)  on a.JOB_ORDER_NO=b.JOB_ORDER_NO where a.DOC_NO='" + docno + "' and b.FACTORY_CD='" + factory + "' and b.PROCESS_CD='" + process + "' and b.WIP_CONTROL_BY_PRD_LINE='Y' and b.GARMENT_TYPE='K' and b.PROCESS_TYPE='I' group by a.JOB_ORDER_NO";
            return sqlComGet.ExecuteReader();
        }

        //接收
        public SqlDataReader chooseproduction(SqlConnection sqlConn, string factory, string process, string processtype, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.JOB_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING as a with (nolock) inner join PRD_JO_PROCESS_WIP_CONTROL as b with (nolock)  on a.JOB_ORDER_NO=b.JOB_ORDER_NO where a.DOC_NO='" + docno + "' and b.PROCESS_CD='" + process + "' and b.WIP_CONTROL_BY_PRD_LINE='Y' and b.GARMENT_TYPE='K' and b.PROCESS_TYPE='" + processtype + "' and b.PRODUCTION_FACTORY='" + factory + "' group by a.JOB_ORDER_NO";
            return sqlComGet.ExecuteReader();
        }

        public SqlDataReader checkcarton(SqlConnection sqlConn, string barcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select ID from CIPMS_CARTON with (nolock) where CARTON_BARCODE='" + barcode + "' and  CARTON_STATUS='C' and PROCESS_TYPE='I'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checksameuserconfirm(SqlConnection sqlConn, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select * from CIPMS_TRANSFER_HD as a  with (nolock) inner join GEN_PROCESS_SUBMIT_CONTROL as b  with (nolock) on a.PROCESS_CD=b.PROCESS_CD and a.NEXT_PROCESS_CD=b.TO_PROCESS_CD where a.DOC_NO='" + docno + "' and b.SAME_USER='Y'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkuserreceive(SqlConnection sqlConn, string userbarcode, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.USER_BARCODE from CIPMS_USER as a inner join CIPMS_TRANSFER_HD as b with (nolock) on a.PRC_CD=b.NEXT_PROCESS_CD where a.USER_BARCODE='" + userbarcode + "' and b.DOC_NO='" + docno + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkbarcode(SqlConnection sqlConn, string barcode, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where BARCODE='" + barcode + "' and PART_CD in " + part + " and PROCESS_TYPE='I'";
            return sqlComGet.ExecuteReader();
        }

        //OAS
        public SqlDataReader bundleinformation(SqlConnection sqlConn, string barcode, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID,DOC_NO,CARTON_BARCODE,PROCESS_CD,PROCESS_TYPE,BARCODE,PART_CD,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING where BARCODE='" + barcode + "' and PART_CD in " + part;
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getcontractno(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.CONTRACT_NO from CIPMS_OAS_INTERFACE as a with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b with (nolock) on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO inner join CIPMS_USER_SCANNING_DFT as c with (nolock) on b.BUNDLE_ID=c.BUNDLE_ID where c.DOC_NO='" + docno + "' and c.USER_BARCODE='" + userbarcode + "' and isnull(c.STATUS,'')<>'N' and a.FLAG=0 group by a.CONTRACT_NO";
            return sqlComGet.ExecuteReader();
        }
        public void updatebundleforscanning(SqlConnection sqlConn, string newdocno, string userbarcode, string process, string production)
        {
            string sql = "update CIPMS_BUNDLE_FOR_SCANNING set PROCESS_CD='" + process + "',PRODUCTION_LINE_CD='" + production + "',PROCESS_TYPE='I',LAST_MODI_USER_ID='" + userbarcode + "',LAST_MODI_DATE=GETDATE() from CIPMS_BUNDLE_FOR_SCANNING as a with (nolock) inner join CIPMS_USER_SCANNING_DFT as b with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + newdocno + "' and b.USER_BARCODE='" + userbarcode + "' and isnull(b.STATUS,'')<>'N'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void reducejowiphd(SqlConnection sqlConn, string newdocno, string userbarcode)
        {
            string sql = "update CIPMS_JO_WIP_HD set WIP=a.WIP-d.QTY from CIPMS_JO_WIP_HD as a, (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD,C.PROCESS_TYPE,(SUM(c.QTY)-SUM(c.DISCREPANCY_QTY))as QTY from CIPMS_BUNDLE_FOR_SCANNING as c  with (nolock) inner join CIPMS_USER_SCANNING_DFT as b with (nolock)  on c.BUNDLE_ID=b.BUNDLE_ID  where b.DOC_NO='" + newdocno + "' and b.USER_BARCODE='" + userbarcode + "' and isnull(b.STATUS,'')<>'N' group by c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD)as d where a.JOB_ORDER_NO=d.JOB_ORDER_NO and a.COLOR_CODE=d.COLOR_CD and a.SIZE_CODE=d.SIZE_CD and a.PART_CD=d.PART_CD and a.PROCESS_CD=d.PROCESS_CD and a.PRODUCTION_LINE_CD=d.PRODUCTION_LINE_CD and a.PROCESS_TYPE=d.PROCESS_TYPE";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void reducejowipbundle(SqlConnection sqlConn, string newdocno, string userbarcode)
        {
            string sql = "update CIPMS_JO_WIP_BUNDLE set WIP=a.WIP-e.QTY,OUT_QTY=a.OUT_QTY+e.QTY from CIPMS_JO_WIP_BUNDLE as a with (nolock) , (select d.STOCK_ID,c.BUNDLE_NO,(c.QTY-c.DISCREPANCY_QTY)as QTY from CIPMS_BUNDLE_FOR_SCANNING as c with (nolock) inner join CIPMS_USER_SCANNING_DFT as b with (nolock) on c.BUNDLE_ID=b.BUNDLE_ID inner join CIPMS_JO_WIP_HD as d with (nolock) on d.JOB_ORDER_NO=c.JOB_ORDER_NO and d.COLOR_CODE=c.COLOR_CD and d.SIZE_CODE=c.SIZE_CD and d.PART_CD=c.PART_CD and d.PROCESS_CD=c.PROCESS_CD and d.PRODUCTION_LINE_CD=c.PRODUCTION_LINE_CD and d.PROCESS_TYPE=c.PROCESS_TYPE where b.DOC_NO='" + newdocno + "' and b.USER_BARCODE='" + userbarcode + "' and isnull(b.STATUS,'')<>'N')as e where a.STOCK_ID=e.STOCK_ID and a.BUNDLE_NO=e.BUNDLE_NO";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updateorinsertjowiphd(SqlConnection sqlConn, string newdocno, string userbarcode, string factory, string processtype, string garmenttype)
        {
            string sql1 = "";
            sql1 = "insert into CIPMS_JO_WIP_HD (FACTORY_CD,JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PROCESS_CD,PRODUCTION_LINE_CD,PROCESS_TYPE,PART_CD,IN_QTY,OUT_QTY,DISCREPANCY_QTY,WIP,INTRANS_QTY,INTRANS_IN,INTRANS_OUT,BUNDLE_REDUCE,MATCHING,GARMENT_TYPE,PRODUCTION_FACTORY) select '" + factory + "',a.JOB_ORDER_NO,a.COLOR_CD,a.SIZE_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,'" + processtype + "',a.PART_CD,0,0,0,0,0,0,0,0,0,'" + garmenttype + "','" + factory + "' from (select JOB_ORDER_NO,COLOR_CD,SIZE_CD,b.PART_CD,PROCESS_CD,PRODUCTION_LINE_CD from CIPMS_BUNDLE_FOR_SCANNING as b with (nolock) inner join CIPMS_USER_SCANNING_DFT as c with (nolock)  on b.BUNDLE_ID=c.BUNDLE_ID where c.DOC_NO='" + newdocno + "' and c.USER_BARCODE='" + userbarcode + "' and isnull(c.STATUS,'')<>'N' group by JOB_ORDER_NO,COLOR_CD,SIZE_CD,c.PART_CD,PROCESS_CD,PRODUCTION_LINE_CD except select JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD from CIPMS_JO_WIP_HD with (nolock) )as a";
            SqlCommand cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
            sql1 = "update CIPMS_JO_WIP_HD set WIP=a.WIP+b.QTY,IN_QTY=a.IN_QTY+b.QTY from CIPMS_JO_WIP_HD as a with (nolock) , (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD,c.PROCESS_TYPE,SUM(c.QTY)as QTY from CIPMS_BUNDLE_FOR_SCANNING as c inner join CIPMS_USER_SCANNING_DFT as d with (nolock) on c.BUNDLE_ID=d.BUNDLE_ID where d.DOC_NO='" + newdocno + "' and d.USER_BARCODE='" + userbarcode + "' and isnull(d.STATUS,'')<>'N' group by c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD)as b where a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CODE=b.COLOR_CD and a.SIZE_CODE=b.SIZE_CD and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD and a.PROCESS_TYPE=b.PROCESS_TYPE";
            cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void insertjowipbundle(SqlConnection sqlConn, string newdocno, string userbarcode)
        {
            string sql = "insert into CIPMS_JO_WIP_BUNDLE (STOCK_ID,BUNDLE_NO,IN_QTY,OUT_QTY,WIP,INTRANS_QTY,DISCREPANCY_QTY,MATCHING,DEFECT,TRANSFER_IN,TRANSFER_OUT,PART_CD,EMPLOYEE_OUTPUT) select a.STOCK_ID,b.BUNDLE_NO,b.QTY-b.DISCREPANCY_QTY,0,b.QTY-b.DISCREPANCY_QTY,0,0,0,0,0,0,b.PART_CD,'0' from CIPMS_JO_WIP_HD as a inner join CIPMS_BUNDLE_FOR_SCANNING as b with (nolock) on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CODE=b.COLOR_CD and a.SIZE_CODE=b.SIZE_CD and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD inner join CIPMS_USER_SCANNING_DFT as c with (nolock)  on b.BUNDLE_ID=c.BUNDLE_ID where c.DOC_NO='" + newdocno + "' and c.USER_BARCODE='" + userbarcode + "' and isnull(c.STATUS,'')<>'N'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updateoasinterface(SqlConnection sqlConn, string factory, string process, string doc_no, string docno, string userbarcode, string contractno)
        {
            string sql = "insert into CIPMS_OAS_INTERFACE (SEND_ID,FACTORY_CD,CIPMS_DOC_NO,CONTRACT_NO,PROCESS_CD,JOB_ORDER_NO,BUNDLE_NO,QTY,COLOR_CD,SIZE_CD,LAY_NO,CUT_LINE,CREATE_USER_ID,CREATE_TIME,FLAG,Status) select c.SEND_ID,'" + factory + "','" + doc_no + "',c.CONTRACT_NO,'" + process + "',b.JOB_ORDER_NO,b.BUNDLE_NO,(MIN(b.QTY)-MAX(b.DISCREPANCY_QTY))as QTY,b.COLOR_CD,b.SIZE_CD,b.LAY_NO,b.CUT_LINE,'" + userbarcode + "',GETDATE(),1,'N' from CIPMS_USER_SCANNING_DFT as a with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID inner join PRD_OUTSOURCE_CONTRACT_DT as c on b.JOB_ORDER_NO=c.JOB_ORDER_NO where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' and isnull(a.STATUS,'')<>'N' and c.CONTRACT_NO='" + contractno + "' group by c.SEND_ID,c.CONTRACT_NO,b.JOB_ORDER_NO,b.BUNDLE_NO,b.COLOR_CD,b.SIZE_CD,b.LAY_NO,b.CUT_LINE";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader checkneedproduction(SqlConnection sqlConn, string factory, string lastprocess, string process, string lastprocesstype, string processtype)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT NEED_PRODUCTION FROM CIPMS_FTY_PROCESS_FLOW with (nolock) where FACTORY_CD='" + factory + "' and PROCESS_CD='" + lastprocess + "' and NEXT_PROCESS_CD='" + process + "' and PROCESS_TYPE='" + lastprocesstype + "' and NEXT_PROCESS_TYPE='" + processtype + "'";
            return sqlComGet.ExecuteReader();
        }
        public void unscannedinsert(SqlConnection sqlConn, string docno, string userbarcode, string functioncd, string date)
        {
            string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) select BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING with (nolock)  where DOC_NO='" + date + "' and BUNDLE_ID not in (select BUNDLE_ID from CIPMS_USER_SCANNING_DFT  with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void unscannedinsert(SqlConnection sqlConn, string docno, string userbarcode, string functioncd, string bundlebarcode, string part)
        {
            string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) select BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) WHERE BARCODE='" + bundlebarcode + "' AND PART_CD IN " + part + " EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT with (nolock) WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
    }
}