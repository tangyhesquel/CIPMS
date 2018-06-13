using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;

/// <summary>
/// Summary description for Select
/// </summary>
///
namespace NameSpace
{

    public class Sqlstatement
    {
        public Sqlstatement()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        //add by Jacob 2015-11-28
        public SqlDataReader getdocnobydocno(SqlConnection sqlConn, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT DOC_NO FROM CIPMS_TRANSFER_HD  with (nolock) WHERE DOC_NO='" + docno + "' AND STATUS IN ('S','M','N','X','Y')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getdocnobybundle(SqlConnection sqlConn, string barcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT A.DOC_NO FROM CIPMS_BUNDLE_FOR_SCANNING AS A  with (nolock) INNER JOIN CIPMS_TRANSFER_HD AS B ON A.DOC_NO=B.DOC_NO WHERE A.BARCODE='" + barcode + "' AND B.STATUS IN ('S','M','N','X','Y') GROUP BY A.DOC_NO";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getdocnobycarton(SqlConnection sqlConn, string barcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT A.DOC_NO FROM CIPMS_BUNDLE_FOR_SCANNING AS A INNER JOIN CIPMS_TRANSFER_HD AS B ON A.DOC_NO=B.DOC_NO WHERE A.CARTON_BARCODE='"+barcode+"' AND A.CARTON_STATUS='C' AND B.STATUS IN ('S','M','N','X','Y') GROUP BY A.DOC_NO";
            return sqlComGet.ExecuteReader();
        }

        //by bundle scann ------add by Jacob 201511/26
        public void unscandocnoinsert(SqlConnection sqlConn, string docno, string userbarcode, string doc_no, string functioncd)
        {
            string sql = "";
            sql = sql + " insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE)";
            sql = sql + " select BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where DOC_NO='" + doc_no + "' and BUNDLE_ID not in (";
            sql = sql + " select BUNDLE_ID from CIPMS_USER_SCANNING_DFT with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }

        //add by Jacob 201511/29
        public void unscanqueryinsert(SqlConnection sqlConn, string docno, string userbarcode, string date, string functioncd)
        {
            string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) select BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING where BUNDLE_ID in (SELECT BUNDLE_ID from CIPMS_SAVE_AND_QUERY WHERE CREATE_DATE='" + date + "' AND CREATE_USER_ID='" + userbarcode + "' AND FUNCTION_CD='" + functioncd + "' EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }

        //by bundle scann ------add by Jacob 201511/13
        public SqlDataReader bundlescannedbybundle(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT A.JOB_ORDER_NO,A.LAY_NO,A.COLOR_CD,A.SIZE_CD,A.BUNDLE_NO,A.BARCODE,A.FACTORY_CD,A.PROCESS_CD,A.PRODUCTION_LINE_CD,A.GARMENT_TYPE,A.PROCESS_TYPE,A.CARTON_BARCODE,A.CARTON_STATUS,A.DOC_NO,MIN(A.QTY)AS QTY,SUM(A.DEFECT)AS DEFECT,MAX(A.DISCREPANCY_QTY)AS DISCREPANCY_QTY FROM (SELECT TOP 100 PERCENT A.BUNDLE_ID,A.JOB_ORDER_NO,A.LAY_NO,A.COLOR_CD,A.SIZE_CD,A.BUNDLE_NO,A.BARCODE,A.PART_CD,B.PART_DESC,A.FACTORY_CD,A.PROCESS_CD,A.PRODUCTION_LINE_CD,A.GARMENT_TYPE,A.PROCESS_TYPE,A.CARTON_BARCODE,A.CARTON_STATUS,A.DOC_NO,A.QTY,A.DEFECT,A.DISCREPANCY_QTY FROM CIPMS_BUNDLE_FOR_SCANNING as A INNER JOIN CIPMS_PART_MASTER as B on A.PART_CD=B.PART_CD INNER JOIN CIPMS_USER_SCANNING_DFT as C on A.BUNDLE_ID=C.BUNDLE_ID WHERE C.DOC_NO='"+docno+"' and C.USER_BARCODE='"+userbarcode+"' ORDER BY C.CREATE_DATE DESC)AS A GROUP BY A.JOB_ORDER_NO,A.LAY_NO,A.COLOR_CD,A.SIZE_CD,A.BUNDLE_NO,A.BARCODE,A.FACTORY_CD,A.PROCESS_CD,A.PRODUCTION_LINE_CD,A.GARMENT_TYPE,A.PROCESS_TYPE,A.CARTON_BARCODE,A.CARTON_STATUS,A.DOC_NO";
            return sqlComGet.ExecuteReader();
        }
        //by part scann ------add by Jacob 201511/13
        public SqlDataReader bundlescannedbypart(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT TOP 100 PERCENT A.BUNDLE_ID,A.JOB_ORDER_NO,A.LAY_NO,A.COLOR_CD,A.SIZE_CD,A.BUNDLE_NO,A.BARCODE,A.PART_CD,B.PART_DESC,A.FACTORY_CD,A.PROCESS_CD,A.PRODUCTION_LINE_CD,A.GARMENT_TYPE,A.PROCESS_TYPE,A.CARTON_BARCODE,A.CARTON_STATUS,A.DOC_NO,A.QTY,A.DEFECT,A.DISCREPANCY_QTY,A.CAR_NO  FROM CIPMS_BUNDLE_FOR_SCANNING as A INNER JOIN CIPMS_PART_MASTER as B on A.PART_CD=B.PART_CD INNER JOIN CIPMS_USER_SCANNING_DFT as C on A.BUNDLE_ID=C.BUNDLE_ID WHERE C.DOC_NO='" + docno + "' and C.USER_BARCODE='" + userbarcode + "' ORDER BY C.CREATE_DATE DESC";
            return sqlComGet.ExecuteReader();
        }
        //delete scanned bundle -----add by Jacob 2015/11/13
        public void deletescanned(SqlConnection sqlConn, string docno, string userbarcode, string barcode)
        {
            string sql = "";
            if (barcode.Length == 14)
                sql = "delete from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "' AND BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE BARCODE='" + barcode + "')";
            else if (barcode.Length == 13 || barcode.Length == 15)
                sql = "delete from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "' AND BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE CARTON_BARCODE='" + barcode + "' AND CARTON_STATUS='C')";
            else if (barcode.Length == 16)
                sql = "delete from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "' AND BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE DOC_NO='" + barcode.Substring(3) + "')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }

        //add by Jacob 2015/11/29
        public void deletescanned(SqlConnection sqlConn, string docno, string userbarcode, string date, string functioncd)
        {
            string sql = "delete from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "' AND BUNDLE_ID IN (SELECT BUNDLE_ID from CIPMS_SAVE_AND_QUERY WHERE CREATE_DATE='" + date + "' AND CREATE_USER_ID='" + userbarcode + "' AND FUNCTION_CD='" + functioncd + "')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }

        public void cleanscan(SqlConnection sqlConn, string docno, string userbarcode)
        {
            string sql = "delete from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //登录用户条码检查
        public SqlDataReader checkuserdr(SqlConnection sqlConn, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select USER_BARCODE,EMPLOYEE_NO,NAME,FACTORY_CD,PRC_CD,PRODUCTION_LINE_CD,B.MODULE_CD AS DEFAULTFUNC from CIPMS_USER AS A INNER JOIN CIPMS_MODULE_MASTER AS B ON A.DEFAULTFUNC=B.MODULE_ID where USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkaccessfunc(SqlConnection sqlConn, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select ID from CIPMS_USER_FUNC where USER_BARCODE='"+userbarcode+"' and FUNC_ID<>1";
            return sqlComGet.ExecuteReader();
        }
        //用户注册权限检查
        public SqlDataReader permissiondr(SqlConnection sqlConn, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT a.FACTORY_CD FROM CIPMS_USER as a inner join CIPMS_USER_FUNC as b on a.USER_BARCODE=b.USER_BARCODE inner join CIPMS_FUNC_MASTER as c on b.FUNC_ID=c.ID WHERE a.USER_BARCODE='" + userbarcode + "' and c.FUNCTION_ENG='Register'";
            return sqlComGet.ExecuteReader();
        }
        //获取工厂
        public SqlDataReader factorydr(SqlConnection sqlConn)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select FACTORY_ID,PROCESS_TYPE from GEN_FACTORY";
            return sqlComGet.ExecuteReader();
        }//获取PEER工厂
        public SqlDataReader peerfactorydr(SqlConnection sqlConn)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select FACTORY_ID from GEN_FACTORY where PROCESS_TYPE='P'";
            return sqlComGet.ExecuteReader();
        }
        //获取部门
        public SqlDataReader processdr(SqlConnection sqlConn, string factory)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PRC_CD,NM,CIPMS_CHS from GEN_PRC_CD_MST where FACTORY_CD='" + factory + "'and GARMENT_TYPE='K' and DISPLAY_SEQ < 40 AND CIPMS_FLAG='Y' group by PRC_CD,NM,DISPLAY_SEQ,CIPMS_CHS order by DISPLAY_SEQ";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader processdr1(SqlConnection sqlConn, string factory, string process, string flowtype)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.PRC_CD,b.NM  from CIPMS_FTY_PROCESS_FLOW as a inner join GEN_PRC_CD_MST as b on a.FACTORY_CD=b.FACTORY_CD and a.NEXT_PROCESS_CD=b.PRC_CD where a.FACTORY_CD='" + factory + "' and a.PROCESS_CD='" + process + "' and a.Flow_Type='" + flowtype + "' GROUP BY b.PRC_CD,b.NM";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader layno1(SqlConnection sqlConn, string joborderno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select LAY_NO from CIPMS_BUNDLE_FOR_SCANNING where JOB_ORDER_NO='" + joborderno + "'";
            return sqlComGet.ExecuteReader();
        }
        //获取组别
        public SqlDataReader productiondr(SqlConnection sqlConn, string factory, string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PRODUCTION_LINE_CD,PRODUCTION_LINE_NAME from GEN_PRODUCTION_LINE where FACTORY_CD='" + factory + "' and CIPMS_PROCESS='" + process + "' and GARMENT_TYPE_CD='K'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader peerproduction(SqlConnection sqlConn, string factory, string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PRODUCTION_LINE_CD,PRODUCTION_LINE_NAME from GEN_PRODUCTION_LINE where FACTORY_CD='" + factory + "' and CIPMS_PROCESS='" + process + "' and GARMENT_TYPE_CD='K'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader nextprocessneedtorec(SqlConnection sqlConn, string factory, string process, string nextprocess)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select * from GEN_PROCESS_SUBMIT_CONTROL where FACTORY_CD='" + factory + "' and PROCESS_CD='" + process + "' and TO_PROCESS_CD='" + nextprocess + "' and CIPMS_FLAG='Y' and ACTIVE='Y' and PROCESS_GARMENT_TYPE='K' and TO_PROCESS_GARMENT_TYPE='K'";
            return sqlComGet.ExecuteReader();
        }
        //获取系统功能
        public SqlDataReader accessfunctiondr(SqlConnection sqlConn)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select A.ID,A.FUNCTION_ENG,B.MODULE_CD,B.MODULE_ID from CIPMS_FUNC_MASTER AS A INNER JOIN CIPMS_MODULE_MASTER AS B ON A.MODULE=B.MODULE_ID where A.IS_ACTIVE=1 AND B.ACTIVE=1 ORDER BY B.MODULE_ID";
            return sqlComGet.ExecuteReader();
        }
        //获取用户旧条码
        public SqlDataReader reprintdr(SqlConnection sqlConn, string employeeno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select USER_BARCODE,NAME,PRC_CD,EMPLOYEE_NO from CIPMS_USER where EMPLOYEE_NO='" + employeeno + "'";
            return sqlComGet.ExecuteReader();
        }
        //获取userbarcode的sequence
        public SqlDataReader userbarcodeseqdr(SqlConnection sqlConn)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select FACTORY_FLAG,SEQUENCE_NO from CIPMS_BARCODE_SEQUENCENO where BARCODE_TYPE='userbarcode' and PRINT_BARCODE='1'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader isemployeenoexist(SqlConnection sqlConn, string employeeno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select USER_BARCODE from CIPMS_USER where EMPLOYEE_NO='" + employeeno + "'";
            return sqlComGet.ExecuteReader();
        }
        //更新userbarcode的sequence
        public void userbarcodesequpdate(SqlConnection sqlConn, int temp)
        {
            string sql = "update CIPMS_BARCODE_SEQUENCENO set SEQUENCE_NO='" + temp + "' where BARCODE_TYPE='userbarcode'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //插入用户信息
        public void userinsert(SqlConnection sqlConn, string newbarcode, string employeeno, string username, string factory, string process, string production, string shift, string defaultfunction)
        {
            string sql = "INSERT INTO CIPMS_USER (USER_BARCODE,EMPLOYEE_NO,NAME,FACTORY_CD,PRC_CD,PRODUCTION_LINE_CD,SHIFT,DEFAULTFUNC) SELECT '"+newbarcode+"','"+employeeno+"','"+username+"','"+factory+"','"+process+"','"+production+"','"+shift+"',MODULE_ID FROM CIPMS_MODULE_MASTER WHERE MODULE_CD='"+defaultfunction+"'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //插入用户功能权限
        public void userfuncinsert(SqlConnection sqlConn, string temp)
        {
            string sql = "insert into CIPMS_USER_FUNC values " + temp;
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //获取用户功能权限
        public SqlDataReader useraccessfunc(SqlConnection sqlConn, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.FUNCTION_ENG,C.MODULE_CD AS MODULE from CIPMS_FUNC_MASTER as a inner join CIPMS_USER_FUNC as b on a.ID = b.FUNC_ID INNER JOIN CIPMS_MODULE_MASTER AS C ON A.MODULE=C.MODULE_ID where a.IS_ACTIVE=1 AND C.ACTIVE=1 and b.USER_BARCODE='"+userbarcode+"'";
            return sqlComGet.ExecuteReader();
        }

        //empty list
        public void emptylist(SqlConnection sqlConn, string userbarcode)
        {
            string sql = "delete from CIPMS_USER_SCANNING_DFT WHERE USER_BARCODE='" + userbarcode + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void emptylist(SqlConnection sqlConn, string docno, string userbarcode)
        {
            string sql = "delete from CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }

        //part
        public SqlDataReader part(SqlConnection sqlConn, string factory)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PART_CD, PART_DESC from CIPMS_PART_MASTER WHERE FACTORY_CD = '" + factory + "' AND ACTIVE='Y' ORDER BY SEQ_NO";
            return sqlComGet.ExecuteReader();
        }

        public SqlDataReader isbundleexist(SqlConnection sqlConn, string bundlebarcode, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(BUNDLE_ID)as QTY from CIPMS_BUNDLE_FOR_SCANNING where BARCODE='" + bundlebarcode + "' and PART_CD in" + part;
            return sqlComGet.ExecuteReader();
        }

        //Savebutton
        public void save(SqlConnection sqlConn, string docno, string userbarcode, string module,string carno="")
        {
            string sql = "insert into CIPMS_SAVE_AND_QUERY (BUNDLE_ID,CREATE_DATE,CREATE_USER_ID,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,DOC_NO) select BUNDLE_ID,GETDATE(),'" + userbarcode + "',BUNDLE_BARCODE,PART_CD,'" + module + "',DOC_NO from CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "';";
            if (carno != "")
            {
                sql = sql + "UPDATE  A SET  A.CAR_NO='" + carno + "'  FROM CIPMS_USER_SCANNING_DFT A  WHERE  A.DOC_NO='" + docno +
                      "'  AND A.USER_BARCODE='" + userbarcode + "';";
                sql = sql +
                      @"UPDATE B SET B.CAR_NO='"+carno+@"' FROM   CIPMS_USER_SCANNING_DFT A INNER JOIN dbo.CIPMS_BUNDLE_FOR_SCANNING B ON B.BUNDLE_ID = A.BUNDLE_ID
            WHERE A.DOC_NO='" + docno + @"' AND A.USER_BARCODE='" + userbarcode + "';";
            }

            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader savemaxdate(SqlConnection sqlConn, string userbarcode, string module)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select MAX(CREATE_DATE)as CREATE_DATE from CIPMS_SAVE_AND_QUERY where CREATE_USER_ID='" + userbarcode + "' and FUNCTION_CD='" + module + "'";
            return sqlComGet.ExecuteReader();
        }
        
        //Package
        public SqlDataReader adjustmentbundlescan(SqlConnection sqlConn, string bundlebarcode, string part, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            //sqlComGet.CommandText = "SELECT c.FACTORY_CD as LAST_FACTORY,c.PROCESS_CD AS LAST_PROCESS,c.PRODUCTION_LINE_CD AS LAST_PRODUCTION,a.DOC_NO,a.PROCESS_TYPE,BUNDLE_ID,CARTON_BARCODE,a.FACTORY_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,a.PROCESS_TYPE,BARCODE,a.PART_CD,b.PART_DESC,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD inner join CIPMS_TRANSFER_HD as c on a.DOC_NO=c.DOC_NO where a.BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE BARCODE='" + bundlebarcode + "' AND PART_CD IN" + part + " EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') and c.STATUS='C'";
            sqlComGet.CommandText = "SELECT c.FACTORY_CD as LAST_FACTORY,c.PROCESS_CD AS LAST_PROCESS,c.PRODUCTION_LINE_CD AS LAST_PRODUCTION,a.DOC_NO,a.PROCESS_TYPE,a.BUNDLE_ID,CARTON_BARCODE,a.FACTORY_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,a.PROCESS_TYPE,BARCODE,a.PART_CD,b.PART_DESC,a.JOB_ORDER_NO,a.BUNDLE_NO,a.LAY_NO,a.SIZE_CD,a.COLOR_CD,a.QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD inner join CIPMS_TRANSFER_HD as c on a.DOC_NO=c.DOC_NO inner join CIPMS_USER_SCANNING_DFT as d on a.BUNDLE_ID=d.BUNDLE_ID WHERE d.DOC_NO='" + docno + "' and d.USER_BARCODE='" + userbarcode + "' and c.STATUS='C' order by d.CREATE_DATE desc";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader adjustmentcartonscan(SqlConnection sqlConn, string cartonbarcode, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            //sqlComGet.CommandText = "SELECT c.FACTORY_CD as LAST_FACTORY,c.PROCESS_CD AS LAST_PROCESS,c.PRODUCTION_LINE_CD AS LAST_PRODUCTION,a.DOC_NO,a.PROCESS_TYPE,BUNDLE_ID,CARTON_BARCODE,a.FACTORY_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,a.PROCESS_TYPE,BARCODE,a.PART_CD,b.PART_DESC,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD inner join CIPMS_TRANSFER_HD as c on a.DOC_NO=c.DOC_NO where a.BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C' EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') and c.STATUS='C'";
            sqlComGet.CommandText = "SELECT c.FACTORY_CD as LAST_FACTORY,c.PROCESS_CD AS LAST_PROCESS,c.PRODUCTION_LINE_CD AS LAST_PRODUCTION,a.DOC_NO,a.PROCESS_TYPE,a.BUNDLE_ID,CARTON_BARCODE,a.FACTORY_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,a.PROCESS_TYPE,BARCODE,a.PART_CD,b.PART_DESC,a.JOB_ORDER_NO,a.BUNDLE_NO,a.LAY_NO,a.SIZE_CD,a.COLOR_CD,a.QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD inner join CIPMS_TRANSFER_HD as c on a.DOC_NO=c.DOC_NO inner join CIPMS_USER_SCANNING_DFT as d on a.BUNDLE_ID=d.BUNDLE_ID WHERE d.DOC_NO='" + docno + "' and d.USER_BARCODE='" + userbarcode + "' and c.STATUS='C' order by d.CREATE_DATE desc";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader adjustmentdocnoscan(SqlConnection sqlConn, string docnobarcode, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            //sqlComGet.CommandText = "SELECT c.FACTORY_CD as LAST_FACTORY,c.PROCESS_CD AS LAST_PROCESS,c.PRODUCTION_LINE_CD AS LAST_PRODUCTION,a.DOC_NO,a.PROCESS_TYPE,BUNDLE_ID,CARTON_BARCODE,a.FACTORY_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,a.PROCESS_TYPE,BARCODE,a.PART_CD,b.PART_DESC,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD inner join CIPMS_TRANSFER_HD as c on a.DOC_NO=c.DOC_NO where a.BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE DOC_NO='" + docnobarcode + "' EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') and c.STATUS='C'";
            sqlComGet.CommandText = "SELECT c.FACTORY_CD as LAST_FACTORY,c.PROCESS_CD AS LAST_PROCESS,c.PRODUCTION_LINE_CD AS LAST_PRODUCTION,a.DOC_NO,a.PROCESS_TYPE,a.BUNDLE_ID,CARTON_BARCODE,a.FACTORY_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,a.PROCESS_TYPE,BARCODE,a.PART_CD,b.PART_DESC,a.JOB_ORDER_NO,a.BUNDLE_NO,a.LAY_NO,a.SIZE_CD,a.COLOR_CD,a.QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD inner join CIPMS_TRANSFER_HD as c on a.DOC_NO=c.DOC_NO inner join CIPMS_USER_SCANNING_DFT as d on a.BUNDLE_ID=d.BUNDLE_ID WHERE d.DOC_NO='" + docno + "' and d.USER_BARCODE='" + userbarcode + "' order by d.CREATE_DATE desc";
            return sqlComGet.ExecuteReader();
        }
        public void adjustmentbundlescaninsert(SqlConnection sqlConn, string docno, string userbarcode, string bundlebarcode, string part, string functioncd)
        {
            string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE BARCODE='" + bundlebarcode + "' AND PART_CD IN " + part + " EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') order by BARCODE";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void adjustmentcartonscaninsert(SqlConnection sqlConn, string docno, string userbarcode, string cartonbarcode, string functioncd)
        {
            string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE)SELECT BUNDLE_ID,'"+docno+"','"+userbarcode+"',BARCODE,PART_CD,'"+functioncd+"',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C' EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') order by BARCODE";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void adjustmentdocnoscaninsert(SqlConnection sqlConn, string docno, string userbarcode, string docnobarcode, string functioncd)
        {
            string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) SELECT BUNDLE_ID,'"+docno+"','"+userbarcode+"',BARCODE,PART_CD,'"+functioncd+"',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE DOC_NO='" + docnobarcode + "' EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') order by BARCODE";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader unscan(SqlConnection sqlConn, string bundlebarcode, string part, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT DOC_NO,PROCESS_TYPE,BUNDLE_ID,CARTON_BARCODE,a.FACTORY_CD,PROCESS_CD,PRODUCTION_LINE_CD,PROCESS_TYPE,BARCODE,a.PART_CD,b.PART_DESC,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE BARCODE='" + bundlebarcode + "' AND PART_CD IN" + part + " EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader scanned(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
           // sqlComGet.CommandText = "SELECT A.CREATE_DATE,A.BUNDLE_ID,C.DOC_NO,C.CARTON_BARCODE,C.FACTORY_CD,C.PROCESS_CD,C.PRODUCTION_LINE_CD,C.PROCESS_TYPE,C.BARCODE,B.PART_CD,B.PART_DESC,C.JOB_ORDER_NO,C.BUNDLE_NO,C.LAY_NO,C.SIZE_CD,C.COLOR_CD,C.QTY,E.WIP,C.DEFECT,C.DISCREPANCY_QTY,C.CARTON_STATUS FROM CIPMS_USER_SCANNING_DFT AS A INNER JOIN CIPMS_PART_MASTER AS B ON A.PART_CD=B.PART_CD INNER JOIN CIPMS_BUNDLE_FOR_SCANNING AS C ON A.BUNDLE_ID=C.BUNDLE_ID INNER JOIN CIPMS_JO_WIP_HD as D ON C.JOB_ORDER_NO=D.JOB_ORDER_NO AND C.COLOR_CD=D.COLOR_CODE AND C.SIZE_CD=D.SIZE_CODE AND C.PART_CD=D.PART_CD AND C.FACTORY_CD=D.FACTORY_CD AND C.PROCESS_CD=D.PROCESS_CD AND C.PRODUCTION_LINE_CD=D.PRODUCTION_LINE_CD AND C.GARMENT_TYPE=D.GARMENT_TYPE AND C.PROCESS_TYPE=D.PROCESS_TYPE INNER JOIN CIPMS_JO_WIP_BUNDLE AS E ON D.STOCK_ID=E.STOCK_ID AND C.BUNDLE_NO=E.BUNDLE_NO WHERE A.DOC_NO='" + docno + "' AND A.USER_BARCODE='" + userbarcode + "' ORDER BY A.CREATE_DATE DESC";
            // 去除了下面关系 tangyh 2017.03.26 
            // AND C.PRODUCTION_LINE_CD=D.PRODUCTION_LINE_CD
            //change by 2018-02-26
            sqlComGet.CommandText = "SELECT A.CREATE_DATE,A.BUNDLE_ID,C.DOC_NO,C.CARTON_BARCODE,C.FACTORY_CD,C.PROCESS_CD,C.PRODUCTION_LINE_CD,C.PROCESS_TYPE,C.BARCODE,B.PART_CD,B.PART_DESC,C.JOB_ORDER_NO,C.BUNDLE_NO,C.LAY_NO,C.SIZE_CD,C.COLOR_CD,C.QTY,(C.QTY-C.DEFECT) as WIP,C.DEFECT,C.DISCREPANCY_QTY,C.CARTON_STATUS,ISNULL(C.CAR_NO,'') AS CAR_NO  FROM CIPMS_USER_SCANNING_DFT AS A INNER JOIN CIPMS_PART_MASTER AS B ON A.PART_CD=B.PART_CD INNER JOIN CIPMS_BUNDLE_FOR_SCANNING AS C ON A.BUNDLE_ID=C.BUNDLE_ID INNER JOIN CIPMS_JO_WIP_HD as D ON C.JOB_ORDER_NO=D.JOB_ORDER_NO AND C.COLOR_CD=D.COLOR_CODE AND C.SIZE_CD=D.SIZE_CODE AND C.PART_CD=D.PART_CD AND C.FACTORY_CD=D.FACTORY_CD AND C.PROCESS_CD=D.PROCESS_CD AND C.GARMENT_TYPE=D.GARMENT_TYPE AND C.PROCESS_TYPE=D.PROCESS_TYPE INNER JOIN CIPMS_JO_WIP_BUNDLE AS E ON D.STOCK_ID=E.STOCK_ID AND C.BUNDLE_NO=E.BUNDLE_NO WHERE A.DOC_NO='" + docno + "' AND A.USER_BARCODE='" + userbarcode + "' ORDER BY A.CREATE_DATE DESC";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader findcarton(SqlConnection sqlConn, string bundlebarcode, string part, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT BUNDLE_ID,CARTON_BARCODE,PROCESS_CD,PROCESS_TYPE,BARCODE,PART_CD,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE CARTON_BARCODE IN(SELECT CARTON_BARCODE FROM CIPMS_BUNDLE_FOR_SCANNING WHERE BARCODE='" + bundlebarcode + "' AND PART_CD IN" + part + " AND CARTON_STATUS='C' GROUP BY CARTON_BARCODE) AND CARTON_STATUS='C' EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            return sqlComGet.ExecuteReader();
        }

        public SqlDataReader partdesc(SqlConnection sqlConn, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PART_DESC from CIPMS_PART_MASTER where PART_CD in " + part;
            return sqlComGet.ExecuteReader();
        }
        public void unscaninsert(SqlConnection sqlConn, string sql)
        {
            sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) VALUES " + sql;
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void unscaninsert(SqlConnection sqlConn, string docno, string userbarcode, string functioncd, string cartonbarcode, string get_othercarton, bool nouse, string part)

        {
            string sql="";
            if (part=="null")
               part="";

            if (get_othercarton == "F")
            {
                sql = " insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE CARTON_BARCODE= '" + cartonbarcode + "' AND CARTON_STATUS='C' EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            }
            else
            {  //tangyh 2017.03.22 
                sql = sql + " insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE)";
                sql = sql + " SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING  a with (nolock) ";
                sql = sql + " inner join ";
                sql = sql + " (SELECT distinct JOB_ORDER_NO,BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) WHERE CARTON_BARCODE='" + cartonbarcode + "') b ";
                sql = sql + " on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO";
                sql = sql + " and BUNDLE_ID not in (SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT with (nolock) WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') ";
                sql = sql + " and ('" + part + "'='' or a.PART_CD in ( select FNField from FN_SPLIT_STRING_TB('" + part + "',',')))";
                sql = sql + " and CARTON_BARCODE<>'" + cartonbarcode + "'";

            }
            
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }


        public void unscaninsert(SqlConnection sqlConn, string functioncd, string docno, string userbarcode, string bundlebarcode, string part, string get_othercarton)
        {
            string sql="";
            if (part == "null")
                part = "";
            if (get_othercarton == "F")
            {
                sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE BARCODE='" + bundlebarcode + "' AND PART_CD IN " + part + " EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            }
            else
            {
                sql = sql + " insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE)";
                sql = sql + " SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING  a with (nolock) ";
                sql = sql + " inner join ";
                sql = sql + " (SELECT distinct JOB_ORDER_NO,BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING with (nolock) WHERE BARCODE='" + bundlebarcode + "') b ";
                sql = sql + " on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO";
                sql = sql + " and BUNDLE_ID not in (SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT with (nolock) WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') ";
                sql = sql + " and ('" + part + "'='' or a.PART_CD in ( select FNField from FN_SPLIT_STRING_TB('" + part + "',',')))";
                sql = sql + " and DOC_NO<>'" + docno + "'";
            }
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void unscaninsertbybundle(SqlConnection sqlConn, string functioncd, string docno, string userbarcode, string bundlebarcode)
        {
            string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE BARCODE='" + bundlebarcode + "' EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader processmatch(SqlConnection sqlConn, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.JOB_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING AS a INNER JOIN CIPMS_USER_SCANNING_DFT AS b ON a.BARCODE = b.BUNDLE_BARCODE and a.PART_CD = b.PART_CD where a.PROCESS_CD <> 'CUT2' and b.DOC_NO='" + docno + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader partcount(SqlConnection sqlConn, string factory)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(PART_CD) as count FROM CIPMS_PART_MASTER WHERE FACTORY_CD='" + factory + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader scanbundlepart(SqlConnection sqlConn, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(DOC_NO) AS COUNT FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader scanbundles(SqlConnection sqlConn, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select count(distinct convert(nvarchar(20),a.JOB_ORDER_NO)+','+convert(nvarchar(20),a.BUNDLE_NO)) as count from CIPMS_BUNDLE_FOR_SCANNING AS a inner join CIPMS_USER_SCANNING_DFT AS b on a.BARCODE = b.BUNDLE_BARCODE and a.PART_CD = b.PART_CD where b.DOC_NO='" + docno + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader bundlepart(SqlConnection sqlConn, string bundlebarcode, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT BUNDLE_ID,CARTON_BARCODE,PROCESS_CD,BARCODE,PART_CD,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS FROM CIPMS_BUNDLE_FOR_SCANNING where BARCODE='" + bundlebarcode + "' and PART_CD in (" + part + ")";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader layno(SqlConnection sqlConn, string JO)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select DISTINCT LAY_NO from CIPMS_BUNDLE_FOR_SCANNING where JOB_ORDER_NO='" + JO + "'";
            return sqlComGet.ExecuteReader();
        }

        public SqlDataReader cartonstatus(SqlConnection sqlConn, string barcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
      
            sqlComGet.CommandText = "select CARTON_STATUS,PROCESS_CD,PROCESS_TYPE FROM CIPMS_CARTON WHERE CARTON_BARCODE='" + barcode + "'";
            
            return sqlComGet.ExecuteReader();
        }

        public SqlDataReader cartonstatus(SqlConnection sqlConn, string barcode, string get_othercarton)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            if (get_othercarton == "F")
                sqlComGet.CommandText = "select CARTON_STATUS,PROCESS_CD,PROCESS_TYPE FROM CIPMS_CARTON WHERE CARTON_BARCODE='" + barcode + "'";
            else
            {   //tangyh 2017.03.22
                sqlComGet.CommandText = "select CARTON_STATUS,PROCESS_CD,PROCESS_TYPE FROM CIPMS_CARTON WHERE CARTON_BARCODE<>'" + barcode + "'  and CARTON_BARCODE like '" + barcode + "%' and SUBSTRING(CARTON_BARCODE,10,1)='-'";
            }
            return sqlComGet.ExecuteReader();
        }

        //package更改 carton扫描
        public SqlDataReader unscanbundlepart(SqlConnection sqlConn, string cartonbarcode, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.FACTORY_CD,PRODUCTION_LINE_CD,BUNDLE_ID,CARTON_BARCODE,PROCESS_CD,PROCESS_TYPE,BARCODE,a.PART_CD,b.PART_DESC,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD WHERE BUNDLE_ID IN (select BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING where CARTON_BARCODE ='" + cartonbarcode + "' and CARTON_STATUS = 'C' except select BUNDLE_ID from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader unscancartondctosew(SqlConnection sqlConn, string cartonbarcode, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT a.PROCESS_TYPE,CARTON_BARCODE,a.FACTORY_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,BARCODE,a.JOB_ORDER_NO,a.BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,MIN(a.QTY)AS CUT_QTY,MIN(c.MATCHING-c.OUT_QTY)as OUTPUT,MAX(a.DISCREPANCY_QTY)AS DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_JO_WIP_HD as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CODE and a.SIZE_CD=b.SIZE_CODE and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD and a.PROCESS_TYPE=b.PROCESS_TYPE inner join CIPMS_JO_WIP_BUNDLE as c on b.STOCK_ID=c.STOCK_ID and a.BUNDLE_NO=c.BUNDLE_NO where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C' EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') group by a.PROCESS_TYPE,CARTON_BARCODE,a.FACTORY_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,BARCODE,a.JOB_ORDER_NO,a.BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,CARTON_STATUS";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader cartonbundle(SqlConnection sqlConn, string barcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT BUNDLE_ID,CARTON_BARCODE,PROCESS_CD,PROCESS_TYPE,BARCODE,PART_CD,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS FROM CIPMS_BUNDLE_FOR_SCANNING where CARTON_BARCODE='" + barcode + "' and CARTON_STATUS='C' and GARMENT_TYPE='K'";
            return sqlComGet.ExecuteReader();
        }
        public void insertnewclosecarton(SqlConnection sqlConn, string cartonstatus, string cartonbarcode, string userbarcode, string process, string processtype)
        {
            string sql = "insert into CIPMS_CARTON values ('" + cartonbarcode + "','"+cartonstatus+"','" + userbarcode + "',GETDATE(),'" + userbarcode + "',getdate(),'" + process + "','" + processtype + "')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        
        //获取layno里bundle part的信息
        public SqlDataReader laynobundle(SqlConnection sqlConn, string jo, string part, string layno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT BUNDLE_ID,CARTON_BARCODE,PROCESS_CD,BARCODE,PART_CD,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO='" + jo + "' AND PART_CD IN (" + part + ") AND LAY_NO IN (" + layno + ")";
            return sqlComGet.ExecuteReader();
        }
        
        //save
        
        public SqlDataReader savejudge(SqlConnection sqlConn, string userbarcode, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public void savedefect(SqlConnection sqlConn, string docno, string userbarcode)
        {
            string sql = "INSERT INTO CIPMS_SAVE_AND_QUERY_DEFECT_DFT (DOC_NO,BUNDLE_BARCODE,PART_CD,DEFECT_REASON_CD,DEFECT_QTY,USER_BARCODE,CREATE_DATE) SELECT a.DOC_NO,a.BUNDLE_BARCODE,a.PART_CD,a.DEFECT_REASON_CD,a.DEFECT_QTY,a.USER_BARCODE,b.CREATE_DATE FROM CIPMS_BUNDLE_DEFECT_DFT as a inner join CIPMS_SAVE_AND_QUERY as b on a.DOC_NO=b.DOC_NO and a.USER_BARCODE=b.CREATE_USER_ID where b.CREATE_DATE=(SELECT MAX(CREATE_DATE) FROM CIPMS_SAVE_AND_QUERY WHERE DOC_NO='" + docno + "' AND CREATE_USER_ID='" + userbarcode + "') group by a.DOC_NO,b.CREATE_DATE,a.BUNDLE_BARCODE,a.PART_CD,a.DEFECT_REASON_CD,a.DEFECT_QTY,a.USER_BARCODE,a.BUNDLE_ID";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void saveinsert(SqlConnection sqlConn, string sql)
        {
            sql = "insert into CIPMS_SAVE_AND_QUERY VALUES " + sql;
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        
        //querychange
        public SqlDataReader querychange(SqlConnection sqlConn, string date, string userbarcode, string docno, string functioncd)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT BUNDLE_ID,CARTON_BARCODE,PROCESS_CD,PROCESS_TYPE,BARCODE,a.PART_CD,b.PART_DESC,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS FROM CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD where BUNDLE_ID IN (SELECT BUNDLE_ID from CIPMS_SAVE_AND_QUERY WHERE CREATE_DATE='" + date + "' AND CREATE_USER_ID='" + userbarcode + "' AND FUNCTION_CD='" + functioncd + "' EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "')";
            return sqlComGet.ExecuteReader();
        }
        public void unscannedinsert(SqlConnection sqlConn, string date, string userbarcode, string docno, string functioncd)
        {
            string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) SELECT BUNDLE_ID,'"+docno+"','"+userbarcode+"',BARCODE,PART_CD,'"+functioncd+"',GETDATE() FROM CIPMS_BUNDLE_FOR_SCANNING where BUNDLE_ID IN (SELECT BUNDLE_ID from CIPMS_SAVE_AND_QUERY WHERE CREATE_DATE='" + date + "' AND CREATE_USER_ID='" + userbarcode + "' AND FUNCTION_CD='" + functioncd + "' EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "') ORDER BY BARCODE";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void querydefect(SqlConnection sqlConn, string date, string userbarcode, string docno)
        {
            string sql = "insert into CIPMS_BUNDLE_DEFECT_DFT (DOC_NO,BUNDLE_BARCODE,PART_CD,DEFECT_REASON_CD,DEFECT_QTY,USER_BARCODE,CREATE_DATE) select '"+docno+"',BUNDLE_BARCODE,PART_CD,DEFECT_REASON_CD,DEFECT_QTY,USER_BARCODE,GETDATE() from CIPMS_SAVE_AND_QUERY_DEFECT_DFT where CREATE_DATE='" + date + "' and USER_BARCODE='" + userbarcode + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader querybundlepart(SqlConnection sqlConn, string date, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT ID,CARTON_BARCODE,PROCESS_CD,PROCESS_TYPE,BARCODE,PART_CD,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS FROM CIPMS_BUNDLE_FOR_SCANNING where ID IN (SELECT ID FROM CIPMS_SAVE_AND_QUERY WHERE CREATE_DATE='" + date + "' AND CREATE_USER_ID='" + userbarcode + "' AND FUNCTION_CD='Package')";
            return sqlComGet.ExecuteReader();
        }
        //reprint
        public SqlDataReader reprint(SqlConnection sqlConn, string barcode, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select distinct CARTON_BARCODE FROM CIPMS_BUNDLE_FOR_SCANNING WHERE BARCODE='" + barcode + "' and PART_CD IN " + part + " and CARTON_STATUS='C'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkcarton(SqlConnection sqlConn, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select CARTON_BARCODE from CIPMS_CARTON where CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getcartonprocprod(SqlConnection sqlConn, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT PROCESS_CD,PRODUCTION_LINE_CD FROM CIPMS_CARTON where CARTON_BARCODE='"+cartonbarcode+"'";
            return sqlComGet.ExecuteReader();
        }
        //print
        public SqlDataReader printbundleinformation(SqlConnection sqlConn, string joborderno, string layno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BARCODE,JOB_ORDER_NO,COLOR_CD,LAY_NO,BUNDLE_NO,SIZE_CD,QTY,PART_CD,PRODUCTION_LINE_CD from CIPMS_BUNDLE_FOR_SCANNING where JOB_ORDER_NO='" + joborderno + "' and LAY_NO='" + layno + "'";
            return sqlComGet.ExecuteReader();
        }
        //printcarton
        public SqlDataReader printnewcarton(SqlConnection sqlConn)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select FACTORY_FLAG, SEQUENCE_NO from CIPMS_BARCODE_SEQUENCENO WHERE BARCODE_TYPE='cartonbarcode' and PRINT_BARCODE='1'";
            return sqlComGet.ExecuteReader();
        }
        public void updatecartonseq(SqlConnection sqlConn, int temp)
        {
            string sql = "update CIPMS_BARCODE_SEQUENCENO set SEQUENCE_NO='" + temp + "' where BARCODE_TYPE='cartonbarcode'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void cartoninsert(SqlConnection sqlConn, string newbarcode, string userbarcode, string process)
        {
            string sql = "insert into CIPMS_CARTON VALUES('" + newbarcode + "','O','" + userbarcode + "',getdate(),'" + userbarcode + "',getdate(),'" + process + "','I')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //open carton
        //更新bundle的箱的状态
        public void cartonstatusupdate(SqlConnection sqlConn, string docno)
        {
            string sql = "select distinct a.CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING AS a INNER JOIN CIPMS_USER_SCANNING_DFT AS b ON a.BARCODE = b.BUNDLE_BARCODE and a.PART_CD = b.PART_CD where b.DOC_NO='" + docno + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //更新箱的状态
        //获取要更新的箱码
        public SqlDataReader cartonget(SqlConnection sqlConn, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select distinct a.CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING AS a INNER JOIN CIPMS_USER_SCANNING_DFT AS b ON a.BARCODE = b.BUNDLE_BARCODE and a.PART_CD = b.PART_CD where b.DOC_NO='" + docno + "'";
            return sqlComGet.ExecuteReader();
        }
        //更新箱的状态
        public void cartonsupdate(SqlConnection sqlConn, string userbarcode, string cartonlist)
        {
            string sql = "update CIPMS_CARTON SET CARTON_STATUS='O',LAST_MODI_USER_ID='" + userbarcode + "', LAST_MODI_DATE=getdate() WHERE CARTON_BARCODE IN (" + cartonlist + ")";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //添加到箱的历史表1
        public void cartonhistory(SqlConnection sqlConn, string sql)
        {
            sql = "insert into CIPMS_CARTON_HISTORY_HD (CARTON_BARCODE,OPERATION,FACTORY_CD,PROCESS_CD,MODI_USER_ID,MODI_DATE) values " + sql;
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //closecarton
        //查看箱码是否为开并且是否在本部门
        public SqlDataReader closecartonstatus(SqlConnection sqlConn, string cartonbarcode, string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select CARTON_BARCODE FROM CIPMS_CARTON where CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='O' and PROCESS_CD='" + process + "'";
            return sqlComGet.ExecuteReader();
        }
        
        //更新箱码的状态1
        public void updatecartonstatus(SqlConnection sqlConn, string userbarcode, string cartonbarcode, string cartonstatus)
        {
            string sql = "update CIPMS_CARTON SET CARTON_STATUS='" + cartonstatus + "',LAST_MODI_USER_ID='" + userbarcode + "',LAST_MODI_DATE=getdate() where CARTON_BARCODE='" + cartonbarcode + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }

        //check
        //查找多出的bundle part
        public SqlDataReader checkmore(SqlConnection sqlConn, string cartonbarcode, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BARCODE,BUNDLE_NO,a.PART_CD,b.PART_DESC,QTY from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD where BUNDLE_ID in(select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING WHERE CARTON_BARCODE = '" + cartonbarcode + "' AND CARTON_STATUS='C' EXCEPT select BUNDLE_ID from CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' and USER_BARCODE='"+userbarcode+"')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader receivecheckmore(SqlConnection sqlConn, string cartonbarcode, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BARCODE,BUNDLE_NO,a.PART_CD,b.PART_DESC,QTY from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD where BUNDLE_ID in(select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING WHERE DOC_NO = '" + cartonbarcode + "' EXCEPT select BUNDLE_ID from CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            return sqlComGet.ExecuteReader();
        }
        //查找缺少的bundle part
        public SqlDataReader checkless(SqlConnection sqlConn, string cartonbarcode, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BARCODE,BUNDLE_NO,a.PART_CD,b.PART_DESC,QTY from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD where BUNDLE_ID in(select BUNDLE_ID from CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "' except select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING WHERE CARTON_BARCODE = '" + cartonbarcode + "' AND CARTON_STATUS='C')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader redundant(SqlConnection sqlConn, string bundlebarcode, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT JOB_ORDER_NO,BUNDLE_NO,PART_CD,BARCODE FROM CIPMS_BUNDLE_FOR_SCANNING WHERE BARCODE='" + bundlebarcode + "' AND PART_CD='" + part + "'";
            return sqlComGet.ExecuteReader();
        }
        //match
        public SqlDataReader match1(SqlConnection sqlConn, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.JOB_ORDER_NO,b.BUNDLE_NO from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' group by b.JOB_ORDER_NO,b.BUNDLE_NO";
            return sqlComGet.ExecuteReader();
        }
        public void match2(SqlConnection sqlConn, string jo, string bundle)
        {
            string sql = "update CIPMS_JO_WIP_BUNDLE set MATCHING=(select min(b.IN_QTY-b.DISCREPANCY_QTY) from CIPMS_JO_WIP_HD as a inner join CIPMS_JO_WIP_BUNDLE as b on a.STOCK_ID=b.STOCK_ID where a.JOB_ORDER_NO='" + jo + "' and b.BUNDLE_NO=" + bundle + ") where (str(STOCK_ID) + str(BUNDLE_NO)) in (select str(b.STOCK_ID)+str(b.BUNDLE_NO) from CIPMS_JO_WIP_HD as a inner join CIPMS_JO_WIP_BUNDLE as b on a.STOCK_ID=b.STOCK_ID where a.JOB_ORDER_NO='" + jo + "' and b.BUNDLE_NO=" + bundle + " group by b.STOCK_ID, b.BUNDLE_NO)";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader match3(SqlConnection sqlConn, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.JOB_ORDER_NO,b.BUNDLE_NO from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' group by b.JOB_ORDER_NO,b.BUNDLE_NO";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader match4(SqlConnection sqlConn, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.STOCK_ID from CIPMS_BUNDLE_FOR_SCANNING as c inner join CIPMS_JO_WIP_HD as a on a.JOB_ORDER_NO=c.JOB_ORDER_NO inner join CIPMS_JO_WIP_BUNDLE as b on b.BUNDLE_NO=c.BUNDLE_NO AND a.STOCK_ID=b.STOCK_ID where c.BUNDLE_ID in (select BUNDLE_ID from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "') group by b.STOCK_ID";
            return sqlComGet.ExecuteReader();
        }
        public void match5(SqlConnection sqlConn, string stockid)
        {
            string sql = "update CIPMS_JO_WIP_BUNDLE set MATCHING=(select SUM(MATCHING) from CIPMS_JO_WIP_BUNDLE WHERE STOCK_ID='" + stockid + "')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void match6(SqlConnection sqlConn, string userbarcode, string docno)
        {
            string sql = "update CIPMS_CARTON set CARTON_STATUS='O', LAST_MODI_USER_ID='" + userbarcode + "', LAST_MODI_DATE=GETDATE() where CARTON_BARCODE IN (select distinct a.CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_USER_SCANNING_DFT as b on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "' and a.CARTON_STATUS='C')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void match7(SqlConnection sqlConn, string userbarcode, string docno, string factory, string process)
        {
            string sql = "insert into CIPMS_CARTON_HISTORY_HD (CARTON_BARCODE,OPERATION,FACTORY_CD,PROCESS_CD,MODI_USER_ID,MODI_DATE) select distinct a.CARTON_BARCODE, 'Opencarton','" + factory + "','" + process + "','" + userbarcode + "',getdate() from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_USER_SCANNING_DFT as b on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "' and a.CARTON_STATUS='C')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void match8(SqlConnection sqlConn, string cartonbarcode, string userbarcode)
        {
            string sql = "update CIPMS_CARTON set CARTON_STATUS='C', LAST_MODI_USER_ID='" + userbarcode + "', LAST_MODI_DATE=GETDATE() where CARTON_BARCODE='" + cartonbarcode + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //package
        //Delete
        public SqlDataReader isprocess(SqlConnection sqlConn, string docno, string userbarcode, string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.BUNDLE_ID from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' and (b.PROCESS_CD<>'" + process + "' or b.PROCESS_TYPE<>'I')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader isempty(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader closecarton(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.BUNDLE_ID from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' and b.CARTON_STATUS='C'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader opencarton1(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "' except select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING where CARTON_STATUS='C' and CARTON_BARCODE in (select distinct b.CARTON_BARCODE from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.BUNDLE_ID=b.BUNDLE_ID where b.CARTON_STATUS='C' and a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader opencarton2(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING where CARTON_STATUS='C' and CARTON_BARCODE in (select distinct b.CARTON_BARCODE from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.BUNDLE_ID=b.BUNDLE_ID where b.CARTON_STATUS='C' and a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "') except select BUNDLE_ID from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader deletebundlepart(SqlConnection sqlConn, string id)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.BUNDLE_BARCODE,b.PART_CD from CIPMS_BUNDLE_FOR_SCANNING AS a INNER JOIN CIPMS_USER_SCANNING_DFT AS b ON a.BARCODE = b.BUNDLE_BARCODE and a.PART_CD = b.PART_CD where a.BUNDLE_ID = '" + id + "'";
            return sqlComGet.ExecuteReader();
        }
        public void deletebundle(SqlConnection sqlConn, string docno, string userbarcode, string bundle)
        {
            string sql = "delete from CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "' AND BUNDLE_BARCODE='" + bundle + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void deletepart(SqlConnection sqlConn, string docno, string userbarcode, string id)
        {
            string sql = "delete from CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "' AND BUNDLE_ID='" + id + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader deleteinformation(SqlConnection sqlConn, string id)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PROCESS_CD,PROCESS_TYPE from CIPMS_BUNDLE_FOR_SCANNING where BUNDLE_ID=" + id;
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader deletebundlescan(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.PROCESS_CD, a.CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING AS a INNER JOIN CIPMS_USER_SCANNING_DFT AS b ON a.BUNDLE_ID = b.BUNDLE_ID where b.USER_BARCODE='" + userbarcode + "' and b.DOC_NO='" + docno + "'";
            return sqlComGet.ExecuteReader();
        }
        public void deletecarton(SqlConnection sqlConn, string docno, string userbarcode, string cartonbarcode)
        {
            string sql = "delete from CIPMS_USER_SCANNING_DFT from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='"+docno+"' and a.USER_BARCODE='"+userbarcode+"' and b.CARTON_BARCODE='"+cartonbarcode+"' and b.CARTON_STATUS='C'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader totalpcs(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select SUM(a.QTY) as qty,SUM(a.DISCREPANCY_QTY) as reduce from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock)  inner join CIPMS_USER_SCANNING_DFT as b with (nolock)  on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader totalbundles(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(BUNDLE_BARCODE) as count,COUNT(distinct BUNDLE_BARCODE) as distinctcount from CIPMS_USER_SCANNING_DFT with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader totalbundlesbybundle(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT COUNT(1)AS count FROM (select A.JOB_ORDER_NO,A.BUNDLE_NO from CIPMS_BUNDLE_FOR_SCANNING AS A INNER JOIN CIPMS_USER_SCANNING_DFT AS B ON A.BUNDLE_ID=B.BUNDLE_ID where B.DOC_NO='"+docno+"' and B.USER_BARCODE='"+userbarcode+"' GROUP BY A.JOB_ORDER_NO,A.BUNDLE_NO)AS A ";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader totalgarment(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select SUM(QTY)as QTY FROM (select MIN(a.QTY-a.DISCREPANCY_QTY)as QTY from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_USER_SCANNING_DFT as b with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "' group by a.JOB_ORDER_NO,a.BUNDLE_NO) AS c";
            return sqlComGet.ExecuteReader();
        }

        //transaction
        public SqlDataReader confirmflag(SqlConnection sqlConn, string factory, string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select Confirm_flag from GEN_PRC_CD_MST where FACTORY_CD='" + factory + "' and PRC_CD='" + process + "'";
            return sqlComGet.ExecuteReader();
        }
        //defect
        public SqlDataReader defectreason(SqlConnection sqlConn, string factory)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select REASON_CD,REASON_DESC from PRD_REASON_CODE where FACTORY_CD='" + factory + "' and ACTIVE = 'Y' and IS_BUNDLE_REDUCE='Y'";
            return sqlComGet.ExecuteReader();
        }
        //defectlist
        public SqlDataReader defectlist(SqlConnection sqlConn, string docno, string bundlebarcode, string partcd, string factory,string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.DEFECT_QTY, b.REASON_DESC from CIPMS_BUNDLE_DEFECT_DFT as a inner join PRD_REASON_CODE as b on a.DEFECT_REASON_CD=b.REASON_CD where a.DOC_NO='" + docno + "' and a.BUNDLE_BARCODE='" + bundlebarcode + "' and a.PART_CD='" + partcd + "' and b.FACTORY_CD='" + factory + "' and a.PROCESS_CD='" + process + "'";
            return sqlComGet.ExecuteReader();
        }


        //defectDelete(sqlCon, docno, bundlebarcode, partcd, reason, qty, userbarcode, bundleid, process);
        public Boolean defectDelete(SqlConnection sqlConn, string docno, string bundlebarcode, string partcd, string reason, string qty,string userbarcode, string bundleid, string process)
        {
            try
            {
                string SQL = "";
                SQL += "Update CIPMS_BUNDLE_DEFECT_DFT set Qty=QTY-" + Convert.ToInt32(qty) + " Where Doc_No='" + docno + "' and BUNDLE_BARCODE='" + bundlebarcode + "' and PART_CD='" + partcd + "' and REASON_CD='" + reason + "' and BUNDLE_ID='" + bundleid + "' and PROCESS_CD='" + process + "';";

                SQL += "Delete from CIPMS_BUNDLE_DEFECT_DFT Where Doc_No='" + docno + "' and BUNDLE_BARCODE='" + bundlebarcode + "' and PART_CD='" + partcd + "' and REASON_CD='" + reason + "' and BUNDLE_ID='" + bundleid + "' and PROCESS_CD='" + process + "' and qty=0;";

                SqlCommand cmd = new SqlCommand(SQL, sqlConn);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        //本次流转的defect和
        public SqlDataReader defectsumqty(SqlConnection sqlConn, string docno, string bundlebarcode, string partcd)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select SUM(DEFECT_QTY)as QTY from CIPMS_BUNDLE_DEFECT_DFT where BUNDLE_BARCODE='"+bundlebarcode+"' and PART_CD='"+partcd+"' and DOC_NO='"+docno+"' group by BUNDLE_BARCODE,PART_CD";
            return sqlComGet.ExecuteReader();
        }
        //added by Jacob 20151117
        public SqlDataReader getcutgarmenttype(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT A.GARMENT_TYPE FROM CIPMS_BUNDLE_FOR_SCANNING AS A INNER JOIN CIPMS_USER_SCANNING_DFT AS B ON A.BUNDLE_ID=B.BUNDLE_ID WHERE B.DOC_NO='"+docno+"' AND B.USER_BARCODE='"+userbarcode+"' GROUP BY A.GARMENT_TYPE";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getsewgarmenttype(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "";
            return sqlComGet.ExecuteReader();
        }
        //CUT->SEW confirm
        public void deleteprdgarmenttransferhd(SqlConnection sqlConn, string userbarcode, string newdocno, string factorycd, string processcd, string nextprocesscd, string productionlinecd, string nextproductionlinecd, string status, string processgarmenttype, string nextprocessgarmenttype, string processtype, string nextprocesstype, string processproductionfactory, string nextprocessproductionfactory, string startseq)
        {
            string sql = "DELETE FROM PRD_GARMENT_TRANSFER_DFT WHERE DOC_NO='" + newdocno + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
            sql = "DELETE FROM PRD_GARMENT_TRANSFER_HD WHERE DOC_NO='" + newdocno + "'";
            cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
            sql = "INSERT INTO PRD_GARMENT_TRANSFER_HD (DOC_NO,FACTORY_CD,PROCESS_CD,NEXT_PROCESS_CD,PRODUCTION_LINE_CD,NEXT_PRODUCTION_LINE_CD,status,CREATE_USER_ID,CREATE_DATE,PROCESS_GARMENT_TYPE,NEXT_PROCESS_GARMENT_TYPE,PROCESS_TYPE,NEXT_PROCESS_TYPE,PROCESS_PRODUCTION_FACTORY,NEXT_PROCESS_PRODUCTION_FACTORY,START_SEQ) VALUES ('" + newdocno + "','" + factorycd + "','" + processcd + "','" + nextprocesscd + "','" + productionlinecd + "','" + nextproductionlinecd + "','" + status + "','" + userbarcode + "',GETDATE(),'" + processgarmenttype + "','" + nextprocessgarmenttype + "','" + processtype + "','" + nextprocesstype + "','" + processproductionfactory + "','" + nextprocessproductionfactory + "','" + startseq + "')";
            cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void insertprdgarmenttransferdft(SqlConnection sqlConn, string newdocno, string docno, string userbarcode, string cutline)
        {
            string sql = "INSERT INTO PRD_GARMENT_TRANSFER_DFT (DOC_NO,JOB_ORDER_NO,COLOR_CD,SIZE_CD,QTY,CUT_LAY_NO,BUNDLE_NO,USER_CARD_ID,SID)select '" + newdocno + "',A.JOB_ORDER_NO,A.COLOR_CD,A.SIZE_CD,MIN(C.WIP),A.LAY_NO,A.BUNDLE_NO,'" + userbarcode + "',NEWID() from CIPMS_BUNDLE_FOR_SCANNING AS A INNER JOIN CIPMS_JO_WIP_HD AS B ON A.JOB_ORDER_NO=B.JOB_ORDER_NO AND A.COLOR_CD=B.COLOR_CODE AND A.SIZE_CD=B.SIZE_CODE AND A.PART_CD=B.PART_CD AND A.FACTORY_CD=B.FACTORY_CD AND A.PROCESS_CD=B.PROCESS_CD AND A.PRODUCTION_LINE_CD=B.PRODUCTION_LINE_CD AND A.GARMENT_TYPE=B.GARMENT_TYPE AND A.PROCESS_TYPE=B.PROCESS_TYPE INNER JOIN CIPMS_JO_WIP_BUNDLE AS C ON B.STOCK_ID=C.STOCK_ID AND A.BUNDLE_NO=C.BUNDLE_NO AND A.PART_CD=C.PART_CD INNER JOIN CIPMS_USER_SCANNING_DFT AS D ON A.BUNDLE_ID=D.BUNDLE_ID WHERE D.DOC_NO='" + docno + "' AND D.USER_BARCODE='" + userbarcode + "' and a.CUT_LINE='"+cutline+"' GROUP BY A.JOB_ORDER_NO,A.COLOR_CD,A.SIZE_CD,A.LAY_NO,A.BUNDLE_NO";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //过去流转的defect和
        public SqlDataReader defectsumqty(SqlConnection sqlConn, string bundlebarcode, string partcd)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT DEFECT FROM CIPMS_BUNDLE_FOR_SCANNING WHERE BARCODE='" + bundlebarcode + "' AND PART_CD='" + partcd + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader parttransaction(SqlConnection sqlConn, string bundlepart)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PART_CD from CIPMS_PART_MASTER where PART_DESC='" + bundlepart + "'";
            return sqlComGet.ExecuteReader();
        }
        public void defectconfirm(SqlConnection sqlConn, string docno, string bundlebarcode, string partcd, string reason, string qty, string userbarcode, string bundleid,string process)
        {
            string sql = "insert into CIPMS_BUNDLE_DEFECT_DFT (DOC_NO,BUNDLE_BARCODE,PART_CD,DEFECT_REASON_CD,DEFECT_QTY,USER_BARCODE,CREATE_DATE,BUNDLE_ID,PROCESS_CD) values ('" + docno + "', '" + bundlebarcode + "', '" + partcd + "', '" + reason + "', '" + qty + "', '" + userbarcode + "', getdate(), '" + bundleid + "', '" + process + "')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //transaction confirm
        public SqlDataReader tranconfirm1(SqlConnection sqlConn, string factory, string barcodetype)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select SEQUENCE_NO from CIPMS_BARCODE_SEQUENCENO where FACTORY_CD='" + factory + "' and BARCODE_TYPE='" + barcodetype + "' and PRINT_BARCODE='1'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getname(SqlConnection sqlConn, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select NAME from CIPMS_USER where USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getdocnopart(SqlConnection sqlConn, string doc_no)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.PART_CD,b.PART_DESC from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD where a.DOC_NO='" + doc_no + "' group by a.PART_CD,b.PART_DESC";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getcartonnum(SqlConnection sqlConn, string doc_no)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING where DOC_NO='" + doc_no + "' and CARTON_STATUS='C' group by CARTON_BARCODE";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkgtn(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.MATCHING,b.OUT_QTY from CIPMS_JO_WIP_HD as a inner join CIPMS_JO_WIP_BUNDLE as b on a.STOCK_ID=b.STOCK_ID inner join CIPMS_BUNDLE_FOR_SCANNING as c on a.JOB_ORDER_NO=c.JOB_ORDER_NO and b.BUNDLE_NO=c.BUNDLE_NO and a.PART_CD=c.PART_CD and a.PROCESS_CD=c.PROCESS_CD and a.PROCESS_TYPE=c.PROCESS_TYPE inner join CIPMS_USER_SCANNING_DFT as d on c.BUNDLE_ID=d.BUNDLE_ID where d.DOC_NO='" + docno + "' and d.USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkallmatching(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT C.BUNDLE_NO FROM CIPMS_JO_WIP_HD AS A INNER JOIN (select B.JOB_ORDER_NO,B.COLOR_CD,B.SIZE_CD,B.BUNDLE_NO,B.FACTORY_CD,B.PROCESS_CD,B.PRODUCTION_LINE_CD,B.GARMENT_TYPE,B.PROCESS_TYPE from CIPMS_USER_SCANNING_DFT AS A INNER JOIN CIPMS_BUNDLE_FOR_SCANNING AS B ON A.BUNDLE_ID=B.BUNDLE_ID WHERE A.DOC_NO='" + docno + "' AND A.USER_BARCODE='" + userbarcode + "' GROUP BY B.JOB_ORDER_NO,B.COLOR_CD,B.SIZE_CD,B.BUNDLE_NO,B.FACTORY_CD,B.PROCESS_CD,B.PRODUCTION_LINE_CD,B.GARMENT_TYPE,B.PROCESS_TYPE)AS B ON A.JOB_ORDER_NO=B.JOB_ORDER_NO AND A.COLOR_CODE=B.COLOR_CD AND A.SIZE_CODE=B.SIZE_CD AND A.PROCESS_CD=B.PROCESS_CD AND A.PRODUCTION_LINE_CD=B.PRODUCTION_LINE_CD AND A.GARMENT_TYPE=B.GARMENT_TYPE AND A.PROCESS_TYPE=B.PROCESS_TYPE INNER JOIN CIPMS_JO_WIP_BUNDLE AS C ON A.STOCK_ID=C.STOCK_ID AND B.BUNDLE_NO=C.BUNDLE_NO WHERE C.STATUS<>'M'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getttlbundle(SqlConnection sqlConn, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select JOB_ORDER_NO,BUNDLE_NO from CIPMS_BUNDLE_FOR_SCANNING where CARTON_BARCODE='"+cartonbarcode+"' AND CARTON_STATUS='C' GROUP BY JOB_ORDER_NO,BUNDLE_NO";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getcutline(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.CUT_LINE from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_USER_SCANNING_DFT as b on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "' group by a.CUT_LINE";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getsewline(SqlConnection sqlConn, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.PRODUCTION_LINE_CD from CIPMS_BUNDLE_FOR_SCANNING as a inner join CUT_BUNDLE_HD as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO where a.CARTON_BARCODE='"+cartonbarcode+"' group by b.PRODUCTION_LINE_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getcutline(SqlConnection sqlConn, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.CUT_LINE from CIPMS_BUNDLE_FOR_SCANNING as a inner join CUT_BUNDLE_HD as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO where a.CARTON_BARCODE='"+cartonbarcode+"' group by b.CUT_LINE";
            return sqlComGet.ExecuteReader();
        }
        public void tranconfirm2(SqlConnection sqlConn, string factory, string seq, string barcodetype)
        {
            string sql = "update CIPMS_BARCODE_SEQUENCENO set SEQUENCE_NO=" + seq + " where FACTORY_CD='" + factory + "' and BARCODE_TYPE='" + barcodetype + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //public SqlDataReader tranconfirm3(SqlConnection sqlConn, string docno, string userbarcode)
        //{
        //    SqlCommand sqlComGet = new SqlCommand();
        //    sqlComGet.Connection = sqlConn;
        //    sqlComGet.CommandText = "select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.QTY,c.LAY_NO,c.BUNDLE_NO,c.PART_CD  from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_DEFECT_DFT as b on a.DOC_NO=b.DOC_NO and a.USER_BARCODE=b.USER_BARCODE inner join CIPMS_BUNDLE_FOR_SCANNING as c on a.ID=c.ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "'";
        //    return sqlComGet.ExecuteReader();
        //}
        public void inserttransferdt(SqlConnection sqlConn, string docno, string userbarcode, string trandocno, string productionline)
        {
            string sql = "insert into CIPMS_TRANSFER_DT (DOC_NO,JOB_ORDER_NO,COLOR_CD,SIZE_CD,QTY,CUT_LAY_NO,BUNDLE_NO,PART_CD,USER_CARD_ID) select '" + trandocno + "',c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.QTY,c.LAY_NO,c.BUNDLE_NO,c.PART_CD,'" + userbarcode + "' from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as c on a.BUNDLE_ID=c.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' and c.PRODUCTION_LINE_CD='" + productionline + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void inserttransferdt1(SqlConnection sqlConn, string docno, string userbarcode, string trandocno, string productionline, string nextprocess, string processtype, string productionlinecontrol)
        {
            string sql = "insert into CIPMS_TRANSFER_DT (DOC_NO,JOB_ORDER_NO,COLOR_CD,SIZE_CD,QTY,CUT_LAY_NO,BUNDLE_NO,PART_CD,USER_CARD_ID) select '" + trandocno + "',c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.QTY,c.LAY_NO,c.BUNDLE_NO,c.PART_CD,'" + userbarcode + "' from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as c on a.BUNDLE_ID=c.BUNDLE_ID inner join PRD_JO_PROCESS_WIP_CONTROL as b on b.JOB_ORDER_NO=c.JOB_ORDER_NO where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' and c.PRODUCTION_LINE_CD='" + productionline + "' and b.PROCESS_CD='" + nextprocess + "' and b.PROCESS_TYPE='" + processtype + "' and b.WIP_CONTROL_BY_PRD_LINE='" + productionlinecontrol + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void tranconfirm5(SqlConnection sqlConn, string trandocno, string factory, string nextfactory, string process, string nextprocess, string production, string nextproduction, string status, string userbarcode, string garmenttype, string nextgarmenttype, string processtype, string nextprocesstype, string processproductionfactory, string nextprocessproductionfactory)
        {
            string sql = "insert into CIPMS_TRANSFER_HD (DOC_NO,FACTORY_CD,PROCESS_CD,NEXT_PROCESS_CD,PRODUCTION_LINE_CD,NEXT_PRODUCTION_LINE_CD,STATUS,CREATE_USER_ID,CREATE_DATE,LAST_MODI_USER_ID,LAST_MODI_DATE,CONFIRM_USER_ID,CONFIRM_DATE,PROCESS_GARMENT_TYPE,NEXT_PROCESS_GARMENT_TYPE,PROCESS_TYPE,NEXT_PROCESS_TYPE,PROCESS_PRODUCTION_FACTORY,NEXT_PROCESS_PRODUCTION_FACTORY) values ('" + trandocno + "','" + factory + "','" + process + "','" + nextprocess + "','" + production + "','" + nextproduction + "','" + status + "','" + userbarcode + "',getdate(),'','','" + userbarcode + "',getdate(),'" + garmenttype + "','" + nextgarmenttype + "','" + processtype + "','" + nextprocesstype + "','" + processproductionfactory + "','" + nextprocessproductionfactory + "')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void inserttrandefect(SqlConnection sqlConn, string docno, string userbarcode, string process, string sql)
        {
            sql = "insert into CIPMS_BUNDLE_DEFECT (TRX_ID,DEFECT_REASON_CD,QTY,PROCESS_CD) select e.TRX_ID,b.DEFECT_REASON_CD,b.DEFECT_QTY,'" + process + "' from CIPMS_BUNDLE_DEFECT_DFT as b inner join CIPMS_BUNDLE_FOR_SCANNING as d on b.BUNDLE_ID=d.BUNDLE_ID inner join CIPMS_TRANSFER_DT as e ON e.JOB_ORDER_NO=d.JOB_ORDER_NO and e.BUNDLE_NO=d.BUNDLE_NO and e.PART_CD=d.PART_CD where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "' and e.DOC_NO in " + sql + "";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatetranwipbundledefect(SqlConnection sqlConn, string sql)
        {
            sql = "update CIPMS_JO_WIP_BUNDLE set DEFECT=DEFECT+e.QTY from CIPMS_JO_WIP_BUNDLE as a, (select c.STOCK_ID,d.QTY from CIPMS_TRANSFER_DT as b inner join CIPMS_BUNDLE_FOR_SCANNING as f on b.JOB_ORDER_NO=f.JOB_ORDER_NO and b.BUNDLE_NO=f.BUNDLE_NO and b.PART_CD=f.PART_CD inner join CIPMS_JO_WIP_HD as c on c.JOB_ORDER_NO=f.JOB_ORDER_NO and c.COLOR_CODE=f.COLOR_CD and c.SIZE_CODE=f.SIZE_CD and c.PART_CD=f.PART_CD and c.PROCESS_CD=f.PROCESS_CD and c.PRODUCTION_LINE_CD=f.PRODUCTION_LINE_CD inner join CIPMS_BUNDLE_DEFECT as d on d.TRX_ID=b.TRX_ID where b.DOC_NO in" + sql + ")as e where a.STOCK_ID=e.STOCK_ID";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //更改，本厂submit
        public void updatewipbundleqty(SqlConnection sqlConn, string sql)
        {
            sql = "update CIPMS_JO_WIP_BUNDLE set INTRANS_QTY=a.INTRANS_QTY+e.QTY from CIPMS_JO_WIP_BUNDLE as a, (select d.STOCK_ID,c.BUNDLE_NO,(c.QTY-c.DISCREPANCY_QTY)as QTY from CIPMS_TRANSFER_DT as b inner join CIPMS_BUNDLE_FOR_SCANNING as c on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.BUNDLE_NO=c.BUNDLE_NO and b.PART_CD=c.PART_CD inner join CIPMS_JO_WIP_HD as d on d.JOB_ORDER_NO=c.JOB_ORDER_NO and d.COLOR_CODE=c.COLOR_CD and d.SIZE_CODE=c.SIZE_CD and d.PART_CD=c.PART_CD and d.PROCESS_CD=c.PROCESS_CD and d.PRODUCTION_LINE_CD=c.PRODUCTION_LINE_CD where b.DOC_NO in" + sql + ")as e where a.STOCK_ID=e.STOCK_ID and a.BUNDLE_NO=e.BUNDLE_NO";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //更改，本厂submit
        public void updatewiphdqty(SqlConnection sqlConn, string sql)
        {
            sql = "update CIPMS_JO_WIP_HD set INTRANS_QTY=a.INTRANS_QTY+d.QTY from CIPMS_JO_WIP_HD as a, (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD,(SUM(c.QTY)-SUM(c.DISCREPANCY_QTY))as QTY from CIPMS_TRANSFER_DT as b inner join CIPMS_BUNDLE_FOR_SCANNING as c on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.BUNDLE_NO=c.BUNDLE_NO and b.PART_CD=c.PART_CD where b.DOC_NO IN" + sql + " group by c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD)as d where a.JOB_ORDER_NO=d.JOB_ORDER_NO and a.COLOR_CODE=d.COLOR_CD and a.SIZE_CODE=d.SIZE_CD and a.PART_CD=d.PART_CD and a.PROCESS_CD=d.PROCESS_CD and a.PRODUCTION_LINE_CD=d.PRODUCTION_LINE_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //更改，本厂submit
        public void updatebfsdocno(SqlConnection sqlConn, string sql, string userbarcode)
        {
            sql = "update CIPMS_BUNDLE_FOR_SCANNING set DOC_NO=b.DOC_NO,LAST_MODI_USER_ID='"+userbarcode+"',LAST_MODI_DATE=GETDATE() from CIPMS_BUNDLE_FOR_SCANNING as c, (select JOB_ORDER_NO,BUNDLE_NO,PART_CD,DOC_NO from CIPMS_TRANSFER_DT where DOC_NO in"+sql+") as b where c.JOB_ORDER_NO=b.JOB_ORDER_NO and c.BUNDLE_NO=b.BUNDLE_NO and c.PART_CD=b.PART_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void tranconfirm3(SqlConnection sqlConn, string docno, string userbarcode, string nextprocess)
        {
            string sql = "update CIPMS_BUNDLE_FOR_SCANNING set PROCESS_CD='" + nextprocess + "' where BUNDLE_ID IN (select BUNDLE_ID from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void tranconfirm8(SqlConnection sqlConn, string docno, string userbarcode)
        {
            string sql = "update CIPMS_BUNDLE_FOR_SCANNING set DEFECT=a.QTY FROM (select BUNDLE_BARCODE,PART_CD,sum(DEFECT_QTY) as QTY from CIPMS_BUNDLE_DEFECT_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "' group by BUNDLE_BARCODE,PART_CD) as a where a.BUNDLE_BARCODE=CIPMS_BUNDLE_FOR_SCANNING.BARCODE and a.PART_CD=CIPMS_BUNDLE_FOR_SCANNING.PART_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void tranemptylist(SqlConnection sqlConn, string userbarcode)
        {
            string sql = "delete from CIPMS_USER_SCANNING_DFT WHERE USER_BARCODE='" + userbarcode + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
            sql = "delete from CIPMS_BUNDLE_DEFECT_DFT WHERE USER_BARCODE='" + userbarcode + "'";
            cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void trandelete(SqlConnection sqlConn, string docno, string userbarcode, string id)
        {
            string sql = "delete from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "' and BUNDLE_ID='" + id + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void trandeletecarton(SqlConnection sqlConn, string docno, string userbarcode, string cartonbarcode)
        {
            string sql = "delete from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "' and BUNDLE_ID in (select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING where CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader trandeletejudge1(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING where CARTON_STATUS='C' and CARTON_BARCODE IN (select distinct CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING where CARTON_STATUS='C' and BUNDLE_ID IN (select BUNDLE_ID from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')) except select BUNDLE_ID from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader trandeletejudge2(SqlConnection sqlConn, string docno, string userbarcode, string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.BUNDLE_ID as NUMBER from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_USER_SCANNING_DFT as b on a.BUNDLE_ID=b.BUNDLE_ID where a.PROCESS_CD<>'" + process + "' and b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public void updatetransferbundle(SqlConnection sqlConn, string sql)
        {
            sql = "update CIPMS_JO_WIP_BUNDLE set WIP=a.WIP-e.QTY,TRANSFER_OUT=a.TRANSFER_OUT+e.QTY from CIPMS_JO_WIP_BUNDLE as a, (select d.STOCK_ID,c.BUNDLE_NO,(c.QTY-c.DISCREPANCY_QTY)as QTY from CIPMS_TRANSFER_DT as b inner join CIPMS_BUNDLE_FOR_SCANNING as c on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.BUNDLE_NO=c.BUNDLE_NO and b.PART_CD=c.PART_CD inner join CIPMS_JO_WIP_HD as d on d.JOB_ORDER_NO=c.JOB_ORDER_NO and d.COLOR_CODE=c.COLOR_CD and d.SIZE_CODE=c.SIZE_CD and d.PART_CD=c.PART_CD and d.PROCESS_CD=c.PROCESS_CD and d.PRODUCTION_LINE_CD=c.PRODUCTION_LINE_CD where b.DOC_NO in" + sql + ")as e where a.STOCK_ID=e.STOCK_ID and a.BUNDLE_NO=e.BUNDLE_NO";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatetransferbundle1(SqlConnection sqlConn, string sql)
        {
            sql = "update CIPMS_JO_WIP_BUNDLE set WIP=a.WIP-e.QTY,OUT_QTY=a.OUT_QTY+e.QTY from CIPMS_JO_WIP_BUNDLE as a, (select d.STOCK_ID,c.BUNDLE_NO,(c.QTY-c.DISCREPANCY_QTY)as QTY from CIPMS_TRANSFER_DT as b inner join CIPMS_BUNDLE_FOR_SCANNING as c on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.BUNDLE_NO=c.BUNDLE_NO and b.PART_CD=c.PART_CD inner join CIPMS_JO_WIP_HD as d on d.JOB_ORDER_NO=c.JOB_ORDER_NO and d.COLOR_CODE=c.COLOR_CD and d.SIZE_CODE=c.SIZE_CD and d.PART_CD=c.PART_CD and d.PROCESS_CD=c.PROCESS_CD and d.PRODUCTION_LINE_CD=c.PRODUCTION_LINE_CD where b.DOC_NO in" + sql + ")as e where a.STOCK_ID=e.STOCK_ID and a.BUNDLE_NO=e.BUNDLE_NO";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //更改，本厂rework上道部门流转
        public void updatetransferbundle3(SqlConnection sqlConn, string sql)
        {
            sql = "update CIPMS_JO_WIP_BUNDLE set WIP=a.WIP-e.QTY from CIPMS_JO_WIP_BUNDLE as a, (select d.STOCK_ID,c.BUNDLE_NO,(c.QTY-c.DISCREPANCY_QTY)as QTY from CIPMS_TRANSFER_DT as b inner join CIPMS_BUNDLE_FOR_SCANNING as c on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.BUNDLE_NO=c.BUNDLE_NO and b.PART_CD=c.PART_CD inner join CIPMS_JO_WIP_HD as d on d.JOB_ORDER_NO=c.JOB_ORDER_NO and d.COLOR_CODE=c.COLOR_CD and d.SIZE_CODE=c.SIZE_CD and d.PART_CD=c.PART_CD and d.PROCESS_CD=c.PROCESS_CD and d.PRODUCTION_LINE_CD=c.PRODUCTION_LINE_CD where b.DOC_NO in" + sql + ")as e where a.STOCK_ID=e.STOCK_ID and a.BUNDLE_NO=e.BUNDLE_NO";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatetransferbundle2(SqlConnection sqlConn, string sql)
        {
            sql = "update CIPMS_JO_WIP_BUNDLE set WIP=0 from (select a.STOCK_ID, b.BUNDLE_NO, c.QTY from CIPMS_JO_WIP_HD as a inner join CIPMS_JO_WIP_BUNDLE as b on a.STOCK_ID=b.STOCK_ID inner join CIPMS_TRANSFER_DT as c on c.JOB_ORDER_NO=a.JOB_ORDER_NO and c.BUNDLE_NO=b.BUNDLE_NO and c.PART_CD=b.PART_CD where c.DOC_NO in" + sql + ") as d where CIPMS_JO_WIP_BUNDLE.BUNDLE_NO=d.BUNDLE_NO and CIPMS_JO_WIP_BUNDLE.STOCK_ID=d.STOCK_ID";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updaterecbundle(SqlConnection sqlConn, string barcode)
        {
            string sql = "update CIPMS_JO_WIP_BUNDLE set WIP=WIP-INTRANS_QTY, INTRANS_QTY=0, OUT_QTY=OUT_QTY+d.QTY from CIPMS_JO_WIP_BUNDLE as e,(select a.STOCK_ID, b.BUNDLE_NO, c.QTY from CIPMS_JO_WIP_HD as a inner join CIPMS_JO_WIP_BUNDLE as b on a.STOCK_ID=b.STOCK_ID inner join CIPMS_TRANSFER_DT as c on c.JOB_ORDER_NO=a.JOB_ORDER_NO and c.BUNDLE_NO=b.BUNDLE_NO and c.PART_CD=b.PART_CD where b.WIP<>0 and c.DOC_NO in" + barcode + ") as d where e.BUNDLE_NO=d.BUNDLE_NO and e.STOCK_ID=d.STOCK_ID";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updaterecbundlerework(SqlConnection sqlConn, string barcode)
        {
            string sql = "update CIPMS_JO_WIP_BUNDLE set WIP=WIP-INTRANS_QTY, INTRANS_QTY=0 from CIPMS_JO_WIP_BUNDLE as e,(select a.STOCK_ID, b.BUNDLE_NO, c.QTY from CIPMS_JO_WIP_HD as a inner join CIPMS_JO_WIP_BUNDLE as b on a.STOCK_ID=b.STOCK_ID inner join CIPMS_TRANSFER_DT as c on c.JOB_ORDER_NO=a.JOB_ORDER_NO and c.BUNDLE_NO=b.BUNDLE_NO and c.PART_CD=b.PART_CD where b.WIP<>0 and c.DOC_NO in" + barcode + ") as d where e.BUNDLE_NO=d.BUNDLE_NO and e.STOCK_ID=d.STOCK_ID";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatetransferhd(SqlConnection sqlConn, string sql)
        {
            sql = "update CIPMS_JO_WIP_HD set WIP=a.WIP-d.QTY,INTRANS_OUT=a.INTRANS_OUT+d.QTY from CIPMS_JO_WIP_HD as a, (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD,(c.QTY-c.DISCREPANCY_QTY)as QTY from CIPMS_TRANSFER_DT as b inner join CIPMS_BUNDLE_FOR_SCANNING as c on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.BUNDLE_NO=c.BUNDLE_NO and b.PART_CD=c.PART_CD where b.DOC_NO in" + sql + ")as d where a.JOB_ORDER_NO=d.JOB_ORDER_NO and a.COLOR_CODE=d.COLOR_CD and a.SIZE_CODE=d.SIZE_CD and a.PART_CD=d.PART_CD and a.PROCESS_CD=d.PROCESS_CD and a.PRODUCTION_LINE_CD=d.PRODUCTION_LINE_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatetransferhd1(SqlConnection sqlConn, string sql)
        {
            sql = "update CIPMS_JO_WIP_HD set WIP=a.WIP-d.QTY,OUT_QTY=a.OUT_QTY+d.QTY from CIPMS_JO_WIP_HD as a, (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD,(SUM(c.QTY)-SUM(c.DISCREPANCY_QTY))as QTY from CIPMS_TRANSFER_DT as b inner join CIPMS_BUNDLE_FOR_SCANNING as c on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.BUNDLE_NO=c.BUNDLE_NO and b.PART_CD=c.PART_CD where b.DOC_NO IN"+sql+" GROUP BY c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD)as d where a.JOB_ORDER_NO=d.JOB_ORDER_NO and a.COLOR_CODE=d.COLOR_CD and a.SIZE_CODE=d.SIZE_CD and a.PART_CD=d.PART_CD and a.PROCESS_CD=d.PROCESS_CD and a.PRODUCTION_LINE_CD=d.PRODUCTION_LINE_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //更改 本厂rework上道部门流转
        public void updatetransferhd3(SqlConnection sqlConn, string sql)
        {
            sql = "update CIPMS_JO_WIP_HD set WIP=a.WIP-d.QTY from CIPMS_JO_WIP_HD as a, (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD,(SUM(c.QTY)-SUM(c.DISCREPANCY_QTY))as QTY from CIPMS_TRANSFER_DT as b inner join CIPMS_BUNDLE_FOR_SCANNING as c on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.BUNDLE_NO=c.BUNDLE_NO and b.PART_CD=c.PART_CD where b.DOC_NO IN"+sql+" group by c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD)as d where a.JOB_ORDER_NO=d.JOB_ORDER_NO and a.COLOR_CODE=d.COLOR_CD and a.SIZE_CODE=d.SIZE_CD and a.PART_CD=d.PART_CD and a.PROCESS_CD=d.PROCESS_CD and a.PRODUCTION_LINE_CD=d.PRODUCTION_LINE_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatetransferhd2(SqlConnection sqlConn, string sql)
        {
            sql = "update CIPMS_JO_WIP_HD set WIP=WIP-b.QTY from (select JOB_ORDER_NO,COLOR_CD,SIZE_CD,PART_CD,SUM(QTY)as QTY from CIPMS_TRANSFER_DT where DOC_NO in" + sql + " group by JOB_ORDER_NO,COLOR_CD,SIZE_CD,PART_CD) as b where CIPMS_JO_WIP_HD.JOB_ORDER_NO=b.JOB_ORDER_NO and CIPMS_JO_WIP_HD.COLOR_CODE=b.COLOR_CD and CIPMS_JO_WIP_HD.SIZE_CODE=b.SIZE_CD and CIPMS_JO_WIP_HD.PART_CD=b.PART_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader istransfer(SqlConnection sqlConn, string trandocno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.STOCK_ID from CIPMS_JO_WIP_HD as a inner join (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,d.NEXT_PROCESS_CD,d.NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_DT as c inner join CIPMS_TRANSFER_HD as d on c.DOC_NO=d.DOC_NO where c.DOC_NO='"+trandocno+"') as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CODE=b.COLOR_CD and a.SIZE_CODE=b.SIZE_CD and a.PROCESS_CD=b.NEXT_PROCESS_CD and a.PRODUCTION_LINE_CD=b.NEXT_PRODUCTION_LINE_CD";
            return sqlComGet.ExecuteReader();
        }
        //public SqlDataReader istransferhd(SqlConnection sqlConn, string sql, string process, string productionline)
        //{
        //    SqlCommand sqlComGet = new SqlCommand();
        //    sqlComGet.Connection = sqlConn;
        //    sqlComGet.CommandText = "select a.STOCK_ID from CIPMS_JO_WIP_HD as a inner join (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,d.NEXT_PROCESS_CD,d.NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_DT as c inner join CIPMS_TRANSFER_HD as d on c.DOC_NO=d.DOC_NO where c.DOC_NO='" + trandocno + "') as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CODE=b.COLOR_CD and a.SIZE_CODE=b.SIZE_CD and a.PROCESS_CD=b.NEXT_PROCESS_CD and a.PRODUCTION_LINE_CD=b.NEXT_PRODUCTION_LINE_CD";
        //    return sqlComGet.ExecuteReader();
        //}
        public void tranwiphdadd(SqlConnection sqlConn, string trandocno, string factory, string processtype, string garmenttype, string sql)
        {
            sql = "update CIPMS_JO_WIP_HD set WIP=WIP+c.QTY, INTRANS_IN=INTRANS_IN+c.QTY from (select a.JOB_ORDER_NO,a.COLOR_CD,a.SIZE_CD,a.PART_CD,SUM(a.QTY)as QTY,b.NEXT_PROCESS_CD,b.NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_DT as a inner join CIPMS_TRANSFER_HD as b on a.DOC_NO=b.DOC_NO where a.DOC_NO='" + trandocno + "' group by a.JOB_ORDER_NO,a.COLOR_CD,a.SIZE_CD,a.PART_CD,b.NEXT_PROCESS_CD,b.NEXT_PRODUCTION_LINE_CD) as c where CIPMS_JO_WIP_HD.JOB_ORDER_NO=c.JOB_ORDER_NO and CIPMS_JO_WIP_HD.COLOR_CODE=c.COLOR_CD and CIPMS_JO_WIP_HD.SIZE_CODE=c.SIZE_CD and CIPMS_JO_WIP_HD.PART_CD=c.PART_CD and CIPMS_JO_WIP_HD.PROCESS_CD=c.NEXT_PROCESS_CD and CIPMS_JO_WIP_HD.PRODUCTION_LINE_CD=c.NEXT_PRODUCTION_LINE_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void tranwiphdadd(SqlConnection sqlConn, string factory, string processtype, string garmenttype, string sql)
        {
            string sql1 = "";
            sql1 = "insert into CIPMS_JO_WIP_HD (FACTORY_CD,JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PROCESS_CD,PRODUCTION_LINE_CD,PROCESS_TYPE,PART_CD,IN_QTY,WIP,GARMENT_TYPE,PRODUCTION_FACTORY)select '" + factory + "',b.JOB_ORDER_NO,b.COLOR_CD,b.SIZE_CD,b.NEXT_PROCESS_CD,b.NEXT_PRODUCTION_LINE_CD,'" + processtype + "',b.PART_CD,0,0,'" + garmenttype + "','" + factory + "' from (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,e.NEXT_PROCESS_CD,e.NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_DT as c inner join CIPMS_TRANSFER_HD as e on c.DOC_NO=e.DOC_NO where c.DOC_NO in"+sql+" group by c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,e.NEXT_PROCESS_CD,e.NEXT_PRODUCTION_LINE_CD)as b left join CIPMS_JO_WIP_HD as a on b.JOB_ORDER_NO=a.JOB_ORDER_NO and b.COLOR_CD=a.COLOR_CODE and b.SIZE_CD=a.SIZE_CODE and b.PART_CD=a.PART_CD and b.NEXT_PROCESS_CD=a.PROCESS_CD and b.NEXT_PRODUCTION_LINE_CD=a.PRODUCTION_LINE_CD";
            SqlCommand cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
            sql1 = "update CIPMS_JO_WIP_HD set WIP=a.WIP+b.QTY,INTRANS_IN=a.INTRANS_IN+b.QTY from CIPMS_JO_WIP_HD as a, (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,e.NEXT_PROCESS_CD,e.NEXT_PRODUCTION_LINE_CD,(d.QTY-d.DISCREPANCY_QTY)as QTY from CIPMS_TRANSFER_DT as c inner join CIPMS_BUNDLE_FOR_SCANNING as d on c.JOB_ORDER_NO=d.JOB_ORDER_NO and c.BUNDLE_NO=d.BUNDLE_NO and c.PART_CD=d.PART_CD inner join CIPMS_TRANSFER_HD as e on c.DOC_NO=e.DOC_NO where c.DOC_NO in" + sql + ")as b where a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CODE=b.COLOR_CD and a.SIZE_CODE=b.SIZE_CD and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.NEXT_PROCESS_CD and a.PRODUCTION_LINE_CD=b.NEXT_PRODUCTION_LINE_CD";
            cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //更改 本厂下道流转
        public void tranwiphdadd2(SqlConnection sqlConn, string factory,string processtype, string garmenttype, string sql)
        {
            string sql1 = "";
            sql1 = "insert into CIPMS_JO_WIP_HD (FACTORY_CD,JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PROCESS_CD,PRODUCTION_LINE_CD,PROCESS_TYPE,PART_CD,IN_QTY,OUT_QTY,DISCREPANCY_QTY,WIP,INTRANS_QTY,INTRANS_IN,INTRANS_OUT,BUNDLE_REDUCE,MATCHING,GARMENT_TYPE,PRODUCTION_FACTORY)select '" + factory + "',a.JOB_ORDER_NO,a.COLOR_CD,a.SIZE_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD,'" + processtype + "',a.PART_CD,0,0,0,0,0,0,0,0,0,'" + garmenttype + "','" + factory + "' from (select JOB_ORDER_NO,COLOR_CD,SIZE_CD,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD from CIPMS_BUNDLE_FOR_SCANNING where DOC_NO in" + sql + " group by JOB_ORDER_NO,COLOR_CD,SIZE_CD,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD except select JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD from CIPMS_JO_WIP_HD)as a";
            SqlCommand cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
            sql1 = "update CIPMS_JO_WIP_HD set WIP=a.WIP+b.QTY,IN_QTY=a.IN_QTY+b.QTY from CIPMS_JO_WIP_HD as a, (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD,SUM(c.QTY)as QTY from CIPMS_BUNDLE_FOR_SCANNING as c where c.DOC_NO IN" + sql + " group by c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,c.PROCESS_CD,c.PRODUCTION_LINE_CD)as b where a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CODE=b.COLOR_CD and a.SIZE_CODE=b.SIZE_CD and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD";
            cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void tranwiphdadd1(SqlConnection sqlConn, string factory, string processtype, string garmenttype, string sql)
        {
            string sql1 = "";
            sql1 = "update CIPMS_JO_WIP_HD set WIP=WIP+z.QTY, IN_QTY=IN_QTY+z.QTY from (select b.JOB_ORDER_NO,b.COLOR_CD,b.SIZE_CD,b.NEXT_PROCESS_CD,b.NEXT_PRODUCTION_LINE_CD,b.PART_CD,SUM(a.QTY)as QTY from CIPMS_TRANSFER_DT as a inner join (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,d.NEXT_PROCESS_CD,d.NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_DT as c inner join CIPMS_TRANSFER_HD as d on c.DOC_NO=d.DOC_NO where c.DOC_NO in" + sql + " intersect select JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD from CIPMS_JO_WIP_HD group by JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD) as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CD and a.SIZE_CD=b.SIZE_CD and a.PART_CD=b.PART_CD where a.DOC_NO in" + sql + " group by b.JOB_ORDER_NO,b.COLOR_CD,b.SIZE_CD,b.PART_CD,b.NEXT_PROCESS_CD,b.NEXT_PRODUCTION_LINE_CD ) as z where CIPMS_JO_WIP_HD.JOB_ORDER_NO=z.JOB_ORDER_NO and CIPMS_JO_WIP_HD.COLOR_CODE=z.COLOR_CD and CIPMS_JO_WIP_HD.SIZE_CODE=z.SIZE_CD and CIPMS_JO_WIP_HD.PART_CD=z.PART_CD and CIPMS_JO_WIP_HD.PROCESS_CD=z.NEXT_PROCESS_CD and CIPMS_JO_WIP_HD.PRODUCTION_LINE_CD=z.NEXT_PRODUCTION_LINE_CD";
            SqlCommand cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
            sql1 = "insert into CIPMS_JO_WIP_HD (FACTORY_CD, JOB_ORDER_NO, COLOR_CODE, SIZE_CODE, PROCESS_CD, PRODUCTION_LINE_CD,PROCESS_TYPE, PART_CD, WIP, IN_QTY, GARMENT_TYPE, PRODUCTION_FACTORY) select '" + factory + "',b.JOB_ORDER_NO,b.COLOR_CD,b.SIZE_CD,b.NEXT_PROCESS_CD,b.NEXT_PRODUCTION_LINE_CD,'" + processtype + "',b.PART_CD,SUM(a.QTY),SUM(a.QTY),'" + garmenttype + "','" + factory + "' from CIPMS_TRANSFER_DT as a inner join (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,d.NEXT_PROCESS_CD,d.NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_DT as c inner join CIPMS_TRANSFER_HD as d on c.DOC_NO=d.DOC_NO where c.DOC_NO in" + sql + " except select JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD from CIPMS_JO_WIP_HD group by JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD) as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CD and a.SIZE_CD=b.SIZE_CD and a.PART_CD=b.PART_CD where a.DOC_NO in" + sql + "group by b.JOB_ORDER_NO,b.COLOR_CD,b.SIZE_CD,b.PART_CD,b.NEXT_PROCESS_CD,b.NEXT_PRODUCTION_LINE_CD";
            cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void insertwiphdadd(SqlConnection sqlConn, string trandocno, string factory, string processtype, string garmenttype)
        {
            string sql = "insert into CIPMS_JO_WIP_HD (FACTORY_CD, JOB_ORDER_NO, COLOR_CODE, SIZE_CODE, PROCESS_CD, PRODUCTION_LINE_CD,PROCESS_TYPE, PART_CD, WIP, INTRANS_IN, GARMENT_TYPE, PRODUCTION_FACTORY) select '"+factory+"',a.JOB_ORDER_NO,a.COLOR_CD,a.SIZE_CD,b.NEXT_PROCESS_CD,b.NEXT_PRODUCTION_LINE_CD,'"+processtype+"',a.PART_CD,SUM(a.QTY),SUM(a.QTY),'"+garmenttype+"','"+factory+"' from CIPMS_TRANSFER_DT as a inner join CIPMS_TRANSFER_HD as b on a.DOC_NO=b.DOC_NO where a.DOC_NO='"+trandocno+"' group by a.JOB_ORDER_NO,a.COLOR_CD,a.SIZE_CD,a.PART_CD,b.NEXT_PROCESS_CD,b.NEXT_PRODUCTION_LINE_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void insertwipbundle(SqlConnection sqlConn, string sql)
        {
            sql = "insert into CIPMS_JO_WIP_BUNDLE (STOCK_ID,BUNDLE_NO,WIP,TRANSFER_IN,PART_CD) select a.STOCK_ID,b.BUNDLE_NO,b.QTY,b.QTY,b.PART_CD from CIPMS_JO_WIP_HD as a inner join CIPMS_TRANSFER_DT as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CODE=b.COLOR_CD and a.SIZE_CODE=b.SIZE_CD and a.PART_CD=b.PART_CD inner join CIPMS_TRANSFER_HD as c on b.DOC_NO=c.DOC_NO and a.PROCESS_CD=c.NEXT_PROCESS_CD and a.PRODUCTION_LINE_CD=c.NEXT_PRODUCTION_LINE_CD where b.DOC_NO in"+sql+"";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void peerwipcontrol(SqlConnection sqlConn, string sql, string nextfactory, string peerfactorydblink)
        {
            sql = "delete from "+peerfactorydblink+" where FACTORY_CD='" + nextfactory + "' and JOB_ORDER_NO in(select JOB_ORDER_NO from CIPMS_TRANSFER_DT where DOC_NO in" + sql + " group by JOB_ORDER_NO) and PROCESS_CD in('CUT','PRT','EMB','DC') and GARMENT_TYPE='K'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader querywipcontrol(SqlConnection sqlConn, string sql)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT FACTORY_CD,JOB_ORDER_NO,PROCESS_CD,WIP_CONTROL_BY_BUNDLE,WIP_CONTROL_BY_COLOR,WIP_CONTROL_BY_SIZE,WIP_CONTROL_BY_PRD_LINE,STAGE,GARMENT_TYPE,PROCESS_TYPE,PRODUCTION_FACTORY from PRD_JO_PROCESS_WIP_CONTROL where JOB_ORDER_NO in(select JOB_ORDER_NO from CIPMS_TRANSFER_DT where DOC_NO in" + sql + " group by JOB_ORDER_NO) and PROCESS_CD in('CUT','PRT','EMB','DC') and GARMENT_TYPE='K'";
            return sqlComGet.ExecuteReader();
        }
        public void insertwipcontrol(SqlConnection sqlConn, string temp, string peerfactorydblink)
        {
            string sql = "insert into " + peerfactorydblink + " values " + temp;
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //更改，本厂下道部门流转
        public void insertwipbundle1(SqlConnection sqlConn, string sql)
        {
            sql = "insert into CIPMS_JO_WIP_BUNDLE (STOCK_ID,BUNDLE_NO,IN_QTY,OUT_QTY,WIP,INTRANS_QTY,DISCREPANCY_QTY,MATCHING,DEFECT,TRANSFER_IN,TRANSFER_OUT,PART_CD,EMPLOYEE_OUTPUT) select a.STOCK_ID,b.BUNDLE_NO,b.QTY-b.DISCREPANCY_QTY,0,b.QTY-b.DISCREPANCY_QTY,0,0,0,0,0,0,b.PART_CD,'0' from CIPMS_JO_WIP_HD as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CODE=b.COLOR_CD and a.SIZE_CODE=b.SIZE_CD and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD where b.DOC_NO in" + sql;
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void insertwipbundle2(SqlConnection sqlConn, string peerfactorydblink, string userbarcode, string sql)
        {
            sql = "insert into " + peerfactorydblink + " (STOCK_ID,BUNDLE_NO,WIP,IN_QTY,PART_CD) select b.STOCK_ID,a.BUNDLE_NO,a.QTY,a.QTY,a.PART_CD from CIPMS_TRANSFER_DT as a inner join CIPMS_TRANSFER_HD as c on a.DOC_NO=c.DOC_NO inner join #" + userbarcode + " as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CODE and a.SIZE_CD=b.SIZE_CODE and a.PART_CD=b.PART_CD and c.NEXT_PROCESS_CD=b.PROCESS_CD and c.NEXT_PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD where c.DOC_NO in" + sql + "";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatetrandefect(SqlConnection sqlConn, string sql)
        {
            sql = "update CIPMS_BUNDLE_FOR_SCANNING set DEFECT=DEFECT+d.QTY from CIPMS_BUNDLE_FOR_SCANNING as c, (select a.JOB_ORDER_NO,a.BUNDLE_NO,a.PART_CD,b.QTY from CIPMS_TRANSFER_DT as a inner join CIPMS_BUNDLE_DEFECT as b on a.TRX_ID=b.TRX_ID where a.DOC_NO in"+sql+")as d where c.JOB_ORDER_NO=d.JOB_ORDER_NO and c.BUNDLE_NO=d.BUNDLE_NO and c.PART_CD=d.PART_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void insertpeerbundleforscan(SqlConnection sqlConn, string sql, string nextfactory, string nextprocess, string nextproductionline, string userbarcode, string peerfactorydblink)
        {
            string sql1 = "";
            sql1 = "insert into " + peerfactorydblink + " (FACTORY_CD,PROCESS_CD,PRODUCTION_LINE_CD,JOB_ORDER_NO,SIZE_CD,COLOR_CD,LAY_NO,CUT_LINE,BUNDLE_NO,BARCODE,PART_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_BARCODE,CARTON_STATUS,DOC_NO,GARMENT_TYPE,PROCESS_TYPE,USER_CREATE_ID,CREATE_DATE,LAST_MODI_USER_ID,LAST_MODI_DATE,GARMENT_ORDER_NO)select '" + nextfactory + "','" + nextprocess + "','" + nextproductionline + "',c.JOB_ORDER_NO,c.SIZE_CD,c.COLOR_CD,c.LAY_NO,c.CUT_LINE,c.BUNDLE_NO,c.BARCODE,c.PART_CD,c.QTY,c.DEFECT,c.DISCREPANCY_QTY,c.CARTON_BARCODE,c.CARTON_STATUS,c.DOC_NO,c.GARMENT_TYPE,c.PROCESS_TYPE,c.USER_CREATE_ID,c.CREATE_DATE,'" + userbarcode + "',GETDATE(),c.GARMENT_ORDER_NO from (select a.JOB_ORDER_NO,a.BUNDLE_NO,a.PART_CD,a.QTY from CIPMS_TRANSFER_DT as a left join #" + userbarcode + "_BUNDLE_FOR_SCANNING as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO and a.PART_CD=b.PART_CD where a.DOC_NO in" + sql + ")as d inner join CIPMS_BUNDLE_FOR_SCANNING as c on d.JOB_ORDER_NO=c.JOB_ORDER_NO and d.BUNDLE_NO=c.BUNDLE_NO and d.PART_CD=c.PART_CD";
            SqlCommand cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
            sql1 = "update " + peerfactorydblink + " set FACTORY_CD='" + nextfactory + "',PROCESS_CD='" + nextprocess + "',PRODUCTION_LINE_CD='" + nextproductionline + "',LAST_MODI_USER_ID='" + userbarcode + "',LAST_MODI_DATE=GETDATE(),QTY=d.QTY,DEFECT=d.DEFECT,DISCREPANCY_QTY=d.DISCREPANCY_QTY,CARTON_BARCODE=d.CARTON_BARCODE,CARTON_STATUS=d.CARTON_STATUS,DOC_NO=d.DOC_NO from " + peerfactorydblink + " as a, (select c.JOB_ORDER_NO,c.BUNDLE_NO,c.PART_CD,c.QTY,c.DEFECT,c.DISCREPANCY_QTY,c.CARTON_BARCODE,c.CARTON_STATUS,c.DOC_NO from CIPMS_TRANSFER_DT as b inner join CIPMS_BUNDLE_FOR_SCANNING as c on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.BUNDLE_NO=c.BUNDLE_NO and b.PART_CD=c.PART_CD where b.DOC_NO in" + sql + ")as d where a.JOB_ORDER_NO=d.JOB_ORDER_NO and a.BUNDLE_NO=d.BUNDLE_NO and a.PART_CD=d.PART_CD";
            cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void createbundleforscantemptable(SqlConnection sqlConn, string userbarcode, string peerfactorydblink)
        {
            string sql = "CREATE TABLE #" + userbarcode + "_BUNDLE_FOR_SCANNING(JOB_ORDER_NO NVARCHAR(20),BUNDLE_NO NUMERIC(38,0),PART_CD NVARCHAR(20));CREATE INDEX IDX_" + userbarcode + "_BUNDLE_FOR_SCANNING ON #" + userbarcode + "_BUNDLE_FOR_SCANNING(JOB_ORDER_NO,BUNDLE_NO,PART_CD);INSERT INTO #" + userbarcode + "_BUNDLE_FOR_SCANNING select JOB_ORDER_NO,BUNDLE_NO,PART_CD from " + peerfactorydblink + "";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void createtemptable(SqlConnection sqlConn, string userbarcode)
        {
            string sql = "CREATE TABLE #" + userbarcode + "(STOCK_ID BIGINT,JOB_ORDER_NO NVARCHAR(20),COLOR_CD NVARCHAR(20),SIZE_CD NVARCHAR(20),PART_CD NVARCHAR(20),PROCESS_CD NVARCHAR(10),PRODUCTION_LINE_CD NVARCHAR(10));CREATE INDEX IDX_" + userbarcode + "_ALL ON #" + userbarcode + "(JOB_ORDER_NO,COLOR_CD,SIZE_CD,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD)";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void inserttemptable(SqlConnection sqlConn, string userbarcode, string peerfactorydblink)
        {
            string sql = "INSERT INTO #" + userbarcode + " (STOCK_ID,JOB_ORDER_NO,COLOR_CD,SIZE_CD,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD) SELECT STOCK_ID,JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD FROM " + peerfactorydblink + "";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void clearandinserttemptable(SqlConnection sqlConn, string userbarcode, string peerfactorydblink)
        {
            string sql = "delete from #"+userbarcode+";INSERT INTO #" + userbarcode + " (STOCK_ID,JOB_ORDER_NO,COLOR_CD,SIZE_CD,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD) SELECT STOCK_ID,JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD FROM " + peerfactorydblink + "";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void insertorupdatewiphd(SqlConnection sqlConn, string userbarcode, string peerfactorydblink, string factory, string processtype, string garmenttype, string sql)
        {
            string sql1 = "";
            sql1 = "insert into " + peerfactorydblink + " (FACTORY_CD,JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PROCESS_CD,PRODUCTION_LINE_CD,PROCESS_TYPE,PART_CD,IN_QTY,WIP,GARMENT_TYPE,PRODUCTION_FACTORY) select '" + factory + "',c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.NEXT_PROCESS_CD,c.NEXT_PRODUCTION_LINE_CD,'" + processtype + "',c.PART_CD,0,0,'" + garmenttype + "','" + factory + "' from (select b.JOB_ORDER_NO,b.COLOR_CD,b.SIZE_CD,b.PART_CD,a.NEXT_PROCESS_CD,a.NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_HD as a inner join CIPMS_TRANSFER_DT as b on a.DOC_NO=b.DOC_NO where a.DOC_NO in" + sql + " group by b.JOB_ORDER_NO,b.COLOR_CD,b.SIZE_CD,b.PART_CD,a.NEXT_PROCESS_CD,a.NEXT_PRODUCTION_LINE_CD)as c left join #" + userbarcode + " as d on c.JOB_ORDER_NO=d.JOB_ORDER_NO and c.COLOR_CD=d.COLOR_CD and c.SIZE_CD=d.SIZE_CD and c.PART_CD=d.PART_CD and c.NEXT_PROCESS_CD=d.PROCESS_CD and c.NEXT_PRODUCTION_LINE_CD=d.PRODUCTION_LINE_CD";
            SqlCommand cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
            sql1 = "update " + peerfactorydblink + " set IN_QTY=IN_QTY+b.QTY,WIP=WIP+b.QTY from #" + userbarcode + " as a, (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,d.NEXT_PROCESS_CD,d.NEXT_PRODUCTION_LINE_CD,c.QTY from CIPMS_TRANSFER_DT as c inner join CIPMS_TRANSFER_HD as d on c.DOC_NO=d.DOC_NO where c.DOC_NO in" + sql + ")as b where a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CD and a.SIZE_CD=b.SIZE_CD and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.NEXT_PROCESS_CD and a.PRODUCTION_LINE_CD=b.NEXT_PRODUCTION_LINE_CD";
            cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void tranadjustupdatewipbundle(SqlConnection sqlConn, string docno, string userbarcode, string process, string productionline)
        {
            string sql = "update CIPMS_JO_WIP_BUNDLE set WIP=0,IN_QTY=0 from CIPMS_JO_WIP_BUNDLE as a inner join CIPMS_JO_WIP_HD as b on a.STOCK_ID=b.STOCK_ID inner join CIPMS_BUNDLE_FOR_SCANNING as c on c.JOB_ORDER_NO=b.JOB_ORDER_NO and c.BUNDLE_NO=a.BUNDLE_NO and c.PART_CD=b.PART_CD inner join CIPMS_USER_SCANNING_DFT as d on d.BUNDLE_ID=c.BUNDLE_ID where d.DOC_NO='" + docno + "' and d.USER_BARCODE='" + userbarcode + "' and b.PROCESS_CD='" + process + "' and b.PRODUCTION_LINE_CD='" + productionline + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void tranadjustupdatewiphd(SqlConnection sqlConn, string docno, string userbarcode, string process, string productionline)
        {
            string sql = "update CIPMS_JO_WIP_HD set WIP=WIP-d.QTY,IN_QTY=IN_QTY-d.QTY from CIPMS_JO_WIP_HD as a,(select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,SUM(c.QTY)as QTY from CIPMS_USER_SCANNING_DFT as b inner join CIPMS_BUNDLE_FOR_SCANNING as c on b.BUNDLE_ID=c.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "' group by c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD) as d where a.JOB_ORDER_NO=d.JOB_ORDER_NO and a.COLOR_CODE=d.COLOR_CD and a.SIZE_CODE=d.SIZE_CD and a.PROCESS_CD='" + process + "' and a.PRODUCTION_LINE_CD='" + productionline + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updateadjustwipbundle(SqlConnection sqlConn, string docno, string userbarcode)
        {
            string sql = "update CIPMS_JO_WIP_BUNDLE set WIP=WIP+f.QTY,OUT_QTY=OUT_QTY-f.QTY from CIPMS_JO_WIP_BUNDLE as e, (select b.STOCK_ID,c.BUNDLE_NO,g.QTY from CIPMS_BUNDLE_FOR_SCANNING as c inner join CIPMS_USER_SCANNING_DFT as d on c.BUNDLE_ID=d.BUNDLE_ID inner join CIPMS_TRANSFER_HD as a on a.DOC_NO=c.DOC_NO inner join CIPMS_TRANSFER_DT as g on a.DOC_NO=g.DOC_NO and g.JOB_ORDER_NO=c.JOB_ORDER_NO and g.BUNDLE_NO=c.BUNDLE_NO and g.PART_CD=c.PART_CD inner join CIPMS_JO_WIP_HD as b on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.COLOR_CODE=c.COLOR_CD and b.SIZE_CODE=c.SIZE_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD where d.DOC_NO='" + docno + "' and d.USER_BARCODE='" + userbarcode + "') as f where e.STOCK_ID=f.STOCK_ID and e.BUNDLE_NO=f.BUNDLE_NO";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updateadjustwiphd(SqlConnection sqlConn, string docno, string userbarcode)
        {
            string sql = "update CIPMS_JO_WIP_HD set WIP=WIP+f.QTY,OUT_QTY=OUT_QTY-f.QTY from CIPMS_JO_WIP_HD as e, (select a.PROCESS_CD,a.PRODUCTION_LINE_CD,g.JOB_ORDER_NO,g.COLOR_CD,g.SIZE_CD,SUM(g.QTY)as QTY from CIPMS_BUNDLE_FOR_SCANNING as c inner join CIPMS_USER_SCANNING_DFT as d on c.BUNDLE_ID=d.BUNDLE_ID inner join CIPMS_TRANSFER_HD as a on a.DOC_NO=c.DOC_NO inner join CIPMS_TRANSFER_DT as g on a.DOC_NO=g.DOC_NO and g.JOB_ORDER_NO=c.JOB_ORDER_NO and g.BUNDLE_NO=c.BUNDLE_NO and g.PART_CD=c.PART_CD inner join CIPMS_JO_WIP_HD as b on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.COLOR_CODE=c.COLOR_CD and b.SIZE_CODE=c.SIZE_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD where d.DOC_NO='" + docno + "' and d.USER_BARCODE='" + userbarcode + "' group by g.JOB_ORDER_NO,g.COLOR_CD,g.SIZE_CD,a.PROCESS_CD,a.PRODUCTION_LINE_CD) as f where e.JOB_ORDER_NO=f.JOB_ORDER_NO and e.COLOR_CODE=f.COLOR_CD and e.SIZE_CODE=f.SIZE_CD and e.PROCESS_CD=f.PROCESS_CD and e.PRODUCTION_LINE_CD=f.PRODUCTION_LINE_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void tranadjustupdatebundleforscan(SqlConnection sqlConn, string docno, string userbarcode)
        {
            string sql = "update CIPMS_BUNDLE_FOR_SCANNING set PROCESS_CD=e.PROCESS_CD,PRODUCTION_LINE_CD=e.PRODUCTION_LINE_CD,LAST_MODI_USER_ID='" + userbarcode + "',LAST_MODI_DATE=GETDATE() from CIPMS_BUNDLE_FOR_SCANNING as f, (select d.PROCESS_CD,d.PRODUCTION_LINE_CD,b.JOB_ORDER_NO,b.BUNDLE_NO,b.PART_CD from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.BUNDLE_ID=b.BUNDLE_ID inner join CIPMS_TRANSFER_DT as c on b.JOB_ORDER_NO=c.JOB_ORDER_NO and b.COLOR_CD=c.COLOR_CD and b.SIZE_CD=c.SIZE_CD inner join CIPMS_TRANSFER_HD as d on c.DOC_NO=d.DOC_NO where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "') as e where f.JOB_ORDER_NO=e.JOB_ORDER_NO and f.BUNDLE_NO=e.BUNDLE_NO and f.PART_CD=e.PART_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //更改：本道部门下道流转
        public void updatetranbundleforscanning(SqlConnection sqlConn, string sql, string userbarcode, string nextproduction, string factory, string nextprocess, string garmenttype, string processtype)
        {
            string sql1 = "";
            sql1 = "update CIPMS_BUNDLE_FOR_SCANNING set QTY=b.QTY,DOC_NO=b.DOC_NO, PROCESS_CD=b.NEXT_PROCESS_CD, PRODUCTION_LINE_CD=b.NEXT_PRODUCTION_LINE_CD,LAST_MODI_USER_ID='" + userbarcode + "',LAST_MODI_DATE=GETDATE() from CIPMS_BUNDLE_FOR_SCANNING as a, (select c.DOC_NO,c.JOB_ORDER_NO,c.BUNDLE_NO,c.PART_CD,c.QTY,d.NEXT_PROCESS_CD,'NA' as NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_DT as c inner join CIPMS_TRANSFER_HD as d on c.DOC_NO=d.DOC_NO where c.DOC_NO IN" + sql + ") as b where a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO and a.PART_CD=b.PART_CD";
            SqlCommand cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
            sql1 = "update CIPMS_BUNDLE_FOR_SCANNING set PRODUCTION_LINE_CD='"+nextproduction+"' from CIPMS_BUNDLE_FOR_SCANNING as c, (select JOB_ORDER_NO,BUNDLE_NO,PART_CD from CIPMS_TRANSFER_DT where DOC_NO in"+sql+" and JOB_ORDER_NO in (select b.JOB_ORDER_NO from PRD_JO_PROCESS_WIP_CONTROL as a inner join CIPMS_TRANSFER_DT as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO where b.DOC_NO in"+sql+" and a.FACTORY_CD='"+factory+"' and a.PROCESS_CD='"+nextprocess+"' and a.WIP_CONTROL_BY_PRD_LINE='Y' and GARMENT_TYPE='"+garmenttype+"' and PROCESS_TYPE='"+processtype+"' group by b.JOB_ORDER_NO))as d where c.JOB_ORDER_NO=d.JOB_ORDER_NO and c.BUNDLE_NO=d.BUNDLE_NO and c.PART_CD=d.PART_CD";
            cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatecarton(SqlConnection sqlConn, string userbarcode, string process, string processtype, string docno)
        {
            string sql = "update CIPMS_CARTON set LAST_MODI_USER_ID='" + userbarcode + "',LAST_MODI_DATE=GETDATE(),PROCESS_CD='" + process + "',PROCESS_TYPE='" + processtype + "' where CARTON_BARCODE in (select CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING as a where DOC_NO in" + docno + " and CARTON_STATUS='C' group by CARTON_BARCODE)";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatebundleforscanning(SqlConnection sqlConn, string sql, string userbarcode)
        {
            sql = "update CIPMS_BUNDLE_FOR_SCANNING set DOC_NO=b.DOC_NO,FACTORY_CD=b.NEXT_PROCESS_PRODUCTION_FACTORY,PROCESS_CD=b.NEXT_PROCESS_CD,PRODUCTION_LINE_CD=b.NEXT_PRODUCTION_LINE_CD,LAST_MODI_USER_ID=" + userbarcode + ",LAST_MODI_DATE=GETDATE() from CIPMS_BUNDLE_FOR_SCANNING as a, (select c.DOC_NO,d.JOB_ORDER_NO,d.BUNDLE_NO,d.PART_CD,c.NEXT_PROCESS_PRODUCTION_FACTORY,c.NEXT_PROCESS_CD,c.NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_HD as c inner join CIPMS_TRANSFER_DT as d on c.DOC_NO=d.DOC_NO where c.DOC_NO in" + sql + ")as b where a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO and a.PART_CD=b.PART_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatedocnobundleforscanning(SqlConnection sqlConn, string sql, string userbarcode)
        {
            sql = "update CIPMS_BUNDLE_FOR_SCANNING set DOC_NO=b.DOC_NODUCTION_LINE_CD,LAST_MODI_USER_ID=" + userbarcode + ",LAST_MODI_DATE=GETDATE() from CIPMS_BUNDLE_FOR_SCANNING as a, (select c.DOC_NO,d.JOB_ORDER_NO,d.BUNDLE_NO,d.PART_CD from CIPMS_TRANSFER_HD as c inner join CIPMS_TRANSFER_DT as d on c.DOC_NO=d.DOC_NO where c.DOC_NO in" + sql + ")as b where a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO and a.PART_CD=b.PART_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updaterecbundleforscanning(SqlConnection sqlConn, string trandocno, string nextproductionline, string userbarcode)
        {
            string sql = "update CIPMS_BUNDLE_FOR_SCANNING set QTY=b.QTY,DOC_NO='" + trandocno + "',PRODUCTION_LINE_CD='" + nextproductionline + "',LAST_MODI_USER_ID='" + userbarcode + "',LAST_MODI_DATE=GETDATE() from (select JOB_ORDER_NO,BUNDLE_NO,PART_CD,QTY from CIPMS_TRANSFER_DT where DOC_NO='" + trandocno + "') as b where CIPMS_BUNDLE_FOR_SCANNING.JOB_ORDER_NO=b.JOB_ORDER_NO and CIPMS_BUNDLE_FOR_SCANNING.BUNDLE_NO=b.BUNDLE_NO and CIPMS_BUNDLE_FOR_SCANNING.PART_CD=b.PART_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void inserttransferhd(SqlConnection sqlConn, string sql)
        {
            sql = "insert into CIPMS_TRANSFER_HD (DOC_NO,FACTORY_CD,PROCESS_CD,NEXT_PROCESS_CD,PRODUCTION_LINE_CD,NEXT_PRODUCTION_LINE_CD,STATUS,CREATE_USER_ID,CREATE_DATE,LAST_MODI_USER_ID,LAST_MODI_DATE,CONFIRM_USER_ID,CONFIRM_DATE,PROCESS_GARMENT_TYPE,NEXT_PROCESS_GARMENT_TYPE,PROCESS_TYPE,NEXT_PROCESS_TYPE,PROCESS_PRODUCTION_FACTORY,NEXT_PROCESS_PRODUCTION_FACTORY) values " + sql;
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void inserttransferhd1(SqlConnection sqlConn, string sql)
        {
            sql = "insert into CIPMS_TRANSFER_HD (DOC_NO,FACTORY_CD,PROCESS_CD,NEXT_PROCESS_CD,PRODUCTION_LINE_CD,NEXT_PRODUCTION_LINE_CD,STATUS,CREATE_USER_ID,CREATE_DATE,LAST_MODI_USER_ID,LAST_MODI_DATE,SUBMIT_USER_ID,SUBMIT_DATE,PROCESS_GARMENT_TYPE,NEXT_PROCESS_GARMENT_TYPE,PROCESS_TYPE,NEXT_PROCESS_TYPE,PROCESS_PRODUCTION_FACTORY,NEXT_PROCESS_PRODUCTION_FACTORY) values " + sql;
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader selecttranproduction(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.PRODUCTION_LINE_CD from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='"+docno+"' and a.USER_BARCODE='"+userbarcode+"' group by b.PRODUCTION_LINE_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader selecttranproduction1(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.PRODUCTION_LINE_CD from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' group by b.PRODUCTION_LINE_CD";
            return sqlComGet.ExecuteReader();
        }
        //receive
        public SqlDataReader partselect(SqlConnection sqlConn, string factory)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PART_CD,PART_DESC from CIPMS_PART_MASTER where FACTORY_CD='" + factory + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getdocnoinformation(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT a.FACTORY_CD,a.NEXT_PROCESS_PRODUCTION_FACTORY,a.NEXT_PROCESS_CD,a.NEXT_PRODUCTION_LINE_CD FROM CIPMS_TRANSFER_HD as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.DOC_NO=b.DOC_NO inner join CIPMS_USER_SCANNING_DFT as c on b.BUNDLE_ID=c.BUNDLE_ID where c.DOC_NO='"+docno+"' and c.USER_BARCODE='"+userbarcode+"' group by a.FACTORY_CD,a.NEXT_PROCESS_PRODUCTION_FACTORY,a.NEXT_PROCESS_CD,a.NEXT_PRODUCTION_LINE_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getreceiveprocess(SqlConnection sqlConn, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select NEXT_PROCESS_CD from CIPMS_TRANSFER_HD where DOC_NO='" + docno + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader production(SqlConnection sqlConn, string proess)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader reccheckdocno(SqlConnection sqlConn, string barcode, string status)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select NEXT_PROCESS_PRODUCTION_FACTORY,NEXT_PROCESS_CD,NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_HD where DOC_NO='" + barcode + "' and STATUS='" + status + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader recdocno(SqlConnection sqlConn, string barcode, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select DOC_NO,BUNDLE_ID,CARTON_BARCODE,CARTON_STATUS,PROCESS_CD,PROCESS_TYPE,BARCODE,PART_CD,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING where DOC_NO='" + barcode + "' and BUNDLE_ID not in (select BUNDLE_ID from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader reccarton(SqlConnection sqlConn, string barcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select distinct a.DOC_NO from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_TRANSFER_HD as b on a.DOC_NO=b.DOC_NO where a.CARTON_BARCODE='"+barcode+"' and a.CARTON_STATUS='C' and (b.STATUS='S' or b.STATUS='M' or b.STATUS='N' or b.STATUS='Y' or b.STATUS='X')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader reccartondocno(SqlConnection sqlConn, string barcode, string status)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select NEXT_PROCESS_PRODUCTION_FACTORY,NEXT_PROCESS_CD,NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_HD where DOC_NO in'" + barcode + "' and STATUS='" + status + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader reccartondocno2(SqlConnection sqlConn, string barcode, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select DOC_NO,BUNDLE_ID,CARTON_BARCODE,CARTON_STATUS,PROCESS_CD,PROCESS_TYPE,BARCODE,PART_CD,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING where DOC_NO in'" + barcode + "' and BUNDLE_ID not in (select BUNDLE_ID from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            return sqlComGet.ExecuteReader();
        }
        public void reccartonforscan(SqlConnection sqlConn, string docno, string barcode, string userbarcode, string functioncd)
        {
            string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID, DOC_NO, USER_BARCODE,BUNDLE_BARCODE, PART_CD, FUNCTION_CD, CREATE_DATE) select BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING where DOC_NO in'" + barcode + "' and BUNDLE_ID not in (select BUNDLE_ID from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void recbundleforscan(SqlConnection sqlConn, string docno, string barcode, string userbarcode, string functioncd)
        {
            string sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID, DOC_NO, USER_BARCODE,BUNDLE_BARCODE, PART_CD, FUNCTION_CD, CREATE_DATE) select BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING where DOC_NO='" + barcode + "' and BUNDLE_ID not in (select BUNDLE_ID from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader recbundle(SqlConnection sqlConn, string barcode, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select distinct a.DOC_NO from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_TRANSFER_HD as b on a.DOC_NO=b.DOC_NO where a.BARCODE='" + barcode + "' and a.PART_CD in " + part + " and (b.STATUS='S' or b.STATUS='M' or b.STATUS='N' or b.STATUS='X' or b.STATUS='Y')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader receivecheck(SqlConnection sqlConn, string trandocno, string factory, string process, string productionline, string status)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select DOC_NO from CIPMS_TRANSFER_HD where  NEXT_PROCESS_PRODUCTION_FACTORY='" + factory + "' and NEXT_PROCESS_CD='" + process + "' and NEXT_PRODUCTION_LINE_CD='" + productionline + "' and STATUS='" + status + "'";
            return sqlComGet.ExecuteReader();
        }
        public void updatereceivetranhd(SqlConnection sqlConn, string barcode, string status, string userbarcode)
        {
            string sql = "update CIPMS_TRANSFER_HD set STATUS='" + status + "', LAST_MODI_USER_ID='" + userbarcode + "',LAST_MODI_DATE=GETDATE(),CONFIRM_USER_ID='" + userbarcode + "',CONFIRM_DATE=GETDATE() where DOC_NO in" + barcode + "";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updaterechd(SqlConnection sqlConn, string barcode)
        {
            string sql = "update CIPMS_JO_WIP_HD set WIP=WIP-y.QTY,INTRANS_QTY=INTRANS_QTY-y.QTY from CIPMS_JO_WIP_HD as z, (select a.STOCK_ID,a.JOB_ORDER_NO,b.BUNDLE_NO,a.PART_CD,c.QTY from CIPMS_JO_WIP_HD as a inner join CIPMS_JO_WIP_BUNDLE as b on a.STOCK_ID=b.STOCK_ID inner join CIPMS_TRANSFER_DT as c on c.JOB_ORDER_NO=a.JOB_ORDER_NO and c.BUNDLE_NO=b.BUNDLE_NO and c.PART_CD=b.PART_CD where b.WIP<>0 and c.DOC_NO in" + barcode + ")as y where z.STOCK_ID=y.STOCK_ID";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updaterechdrework(SqlConnection sqlConn, string barcode)
        {
            string sql = "update CIPMS_JO_WIP_HD set WIP=WIP-b.QTY, INTRANS_QTY=INTRANS_QTY-b.QTY from CIPMS_JO_WIP_HD as d,(select JOB_ORDER_NO,COLOR_CD,SIZE_CD,PART_CD,SUM(QTY)as QTY from CIPMS_TRANSFER_DT where DOC_NO in " + barcode + " group by JOB_ORDER_NO,COLOR_CD,SIZE_CD,PART_CD) as b where d.JOB_ORDER_NO=b.JOB_ORDER_NO and d.COLOR_CODE=b.COLOR_CD and d.SIZE_CODE=b.SIZE_CD and d.PART_CD=b.PART_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void recwiphdadd(SqlConnection sqlConn, string factory, string processtype, string garmenttype, string barcode)
        {
            string sql1 = "update CIPMS_JO_WIP_HD set WIP=WIP+z.QTY, IN_QTY=IN_QTY+z.QTY from CIPMS_JO_WIP_HD as m,(select b.JOB_ORDER_NO,b.COLOR_CD,b.SIZE_CD,b.NEXT_PROCESS_CD,b.NEXT_PRODUCTION_LINE_CD,b.PART_CD,SUM(a.QTY)as QTY from CIPMS_TRANSFER_DT as a inner join (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,d.NEXT_PROCESS_CD,d.NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_DT as c inner join CIPMS_TRANSFER_HD as d on c.DOC_NO=d.DOC_NO where c.DOC_NO in"+barcode+" intersect select JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD from CIPMS_JO_WIP_HD group by JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD) as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CD and a.SIZE_CD=b.SIZE_CD and a.PART_CD=b.PART_CD where a.DOC_NO in"+barcode+" group by b.JOB_ORDER_NO,b.COLOR_CD,b.SIZE_CD,b.PART_CD,b.NEXT_PROCESS_CD,b.NEXT_PRODUCTION_LINE_CD ) as z where m.JOB_ORDER_NO=z.JOB_ORDER_NO and m.COLOR_CODE=z.COLOR_CD and m.SIZE_CODE=z.SIZE_CD and m.PART_CD=z.PART_CD and m.PROCESS_CD=z.NEXT_PROCESS_CD and m.PRODUCTION_LINE_CD=z.NEXT_PRODUCTION_LINE_CD";
            SqlCommand cmd = new SqlCommand(sql1, sqlConn);
            cmd.ExecuteNonQuery();
            string sql2 = "insert into CIPMS_JO_WIP_HD (FACTORY_CD, JOB_ORDER_NO, COLOR_CODE, SIZE_CODE, PROCESS_CD, PRODUCTION_LINE_CD,PROCESS_TYPE, PART_CD, WIP, IN_QTY, GARMENT_TYPE, PRODUCTION_FACTORY) select '" + factory + "',b.JOB_ORDER_NO,b.COLOR_CD,b.SIZE_CD,b.NEXT_PROCESS_CD,b.NEXT_PRODUCTION_LINE_CD,'" + processtype + "',b.PART_CD,SUM(a.QTY),SUM(a.QTY),'" + garmenttype + "','" + factory + "' from CIPMS_TRANSFER_DT as a inner join (select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.PART_CD,d.NEXT_PROCESS_CD,d.NEXT_PRODUCTION_LINE_CD from CIPMS_TRANSFER_DT as c inner join CIPMS_TRANSFER_HD as d on c.DOC_NO=d.DOC_NO where c.DOC_NO in"+barcode+" except select JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD from CIPMS_JO_WIP_HD group by JOB_ORDER_NO,COLOR_CODE,SIZE_CODE,PART_CD,PROCESS_CD,PRODUCTION_LINE_CD) as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CD and a.SIZE_CD=b.SIZE_CD and a.PART_CD=b.PART_CD where a.DOC_NO in"+barcode+" group by b.JOB_ORDER_NO,b.COLOR_CD,b.SIZE_CD,b.PART_CD,b.NEXT_PROCESS_CD,b.NEXT_PRODUCTION_LINE_CD";
            cmd = new SqlCommand(sql2, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void recwipbundleadd(SqlConnection sqlConn, string trandocno)
        {
            string sql = "insert into CIPMS_JO_WIP_BUNDLE (STOCK_ID,BUNDLE_NO,WIP,IN_QTY,PART_CD) select a.STOCK_ID,b.BUNDLE_NO,b.QTY,b.QTY,b.PART_CD from CIPMS_JO_WIP_HD as a inner join CIPMS_TRANSFER_HD as c on c.NEXT_PROCESS_CD=a.PROCESS_CD and c.NEXT_PRODUCTION_LINE_CD=a.PRODUCTION_LINE_CD inner join CIPMS_TRANSFER_DT as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CODE=b.COLOR_CD and a.SIZE_CODE=b.SIZE_CD and a.PART_CD=b.PART_CD and b.DOC_NO=c.DOC_NO where b.DOC_NO='"+trandocno+"'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void recsave(SqlConnection sqlConn, string docno, string userbarcode, string module)
        {
            string sql = "insert into CIPMS_SAVE_AND_QUERY_DOC_NO (CREATE_DATE,CREATE_USER_ID,DOC_NO,FUNCTION_CD) select getdate(),'" + userbarcode + "',b.DOC_NO,'" + module + "' from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' group by b.DOC_NO";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader recsave(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.DOC_NO from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' group by b.DOC_NO";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader recquery(SqlConnection sqlConn, string module, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select distinct top 10 DOC_NO,max(CREATE_DATE)CREATE_DATE from CIPMS_SAVE_AND_QUERY_DOC_NO where CREATE_USER_ID='"+userbarcode+"' and FUNCTION_CD='"+module+"' group by DOC_NO order by CREATE_DATE DESC";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader reclastsubmit(SqlConnection sqlConn, string barcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select FACTORY_CD, PROCESS_CD, PRODUCTION_LINE_CD, STATUS from CIPMS_TRANSFER_HD where DOC_NO='" + barcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader recquerychangeunscan(SqlConnection sqlConn, string date)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.BUNDLE_ID,a.DOC_NO,a.CARTON_BARCODE,a.PROCESS_CD,a.PROCESS_TYPE,a.BARCODE,a.PART_CD,a.JOB_ORDER_NO,a.BUNDLE_NO,a.LAY_NO,a.SIZE_CD,a.COLOR_CD,a.QTY,a.DEFECT,a.DISCREPANCY_QTY,a.CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING as a where a.DOC_NO='" + date + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader recquerychangeunscan(SqlConnection sqlConn, string docno, string userbarcode, string date)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.BUNDLE_ID,a.DOC_NO,a.CARTON_BARCODE,a.PROCESS_CD,a.PROCESS_TYPE,a.BARCODE,a.PART_CD,b.PART_DESC,a.JOB_ORDER_NO,a.BUNDLE_NO,a.LAY_NO,a.SIZE_CD,a.COLOR_CD,a.QTY,a.DEFECT,a.DISCREPANCY_QTY,a.CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD where a.DOC_NO='" + date + "' and a.BUNDLE_ID not in (select BUNDLE_ID from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader recquerychangebelongs(SqlConnection sqlConn, string docno, string userbarcode, string factory, string process, string productionline, string status)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select DOC_NO from CIPMS_TRANSFER_HD where (NEXT_PROCESS_PRODUCTION_FACTORY<>'" + factory + "' or NEXT_PROCESS_CD<>'" + process + "' or NEXT_PRODUCTION_LINE_CD<>'" + productionline + "' or STATUS<>'" + status + "') and DOC_NO in (select distinct a.DOC_NO from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_USER_SCANNING_DFT as b on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader recisalldocno1(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(BUNDLE_ID) as NUM from CIPMS_USER_SCANNING_DFT where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader recisalldocno2(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(BUNDLE_ID) as NUM from CIPMS_BUNDLE_FOR_SCANNING where DOC_NO in (select distinct a.DOC_NO from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_USER_SCANNING_DFT as b on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader recconfirmdocno(SqlConnection sqlConn, string userbarcode, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select distinct b.DOC_NO from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public void updaterecrejecttranhd(SqlConnection sqlConn, string barcode, string status, string userbarcode)
        {
            string sql = "update CIPMS_TRANSFER_HD set STATUS='" + status + "', LAST_MODI_USER_ID='" + userbarcode + "',LAST_MODI_DATE=GETDATE(),REJECT_USER_ID='" + userbarcode + "',REJECT_DATE=GETDATE() where DOC_NO in" + barcode + "";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updaterecrejectwipbundle(SqlConnection sqlConn, string barcode)
        {
            string sql = "update CIPMS_JO_WIP_BUNDLE set INTRANS_QTY=0 from CIPMS_JO_WIP_BUNDLE as e, (select a.STOCK_ID, b.BUNDLE_NO from CIPMS_JO_WIP_HD as a inner join CIPMS_JO_WIP_BUNDLE as b on a.STOCK_ID=b.STOCK_ID inner join CIPMS_TRANSFER_DT as c on c.JOB_ORDER_NO=a.JOB_ORDER_NO and c.BUNDLE_NO=b.BUNDLE_NO and c.PART_CD=b.PART_CD where b.WIP<>0 and c.DOC_NO in" + barcode + ") as d where e.BUNDLE_NO=d.BUNDLE_NO and e.STOCK_ID=d.STOCK_ID";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updaterecrejectwiphd(SqlConnection sqlConn, string barcode)
        {
            string sql = "update CIPMS_JO_WIP_HD set INTRANS_QTY=INTRANS_QTY-y.QTY from CIPMS_JO_WIP_HD as z, (select a.STOCK_ID,a.JOB_ORDER_NO,b.BUNDLE_NO,a.PART_CD,c.QTY from CIPMS_JO_WIP_HD as a inner join CIPMS_JO_WIP_BUNDLE as b on a.STOCK_ID=b.STOCK_ID inner join CIPMS_TRANSFER_DT as c on c.JOB_ORDER_NO=a.JOB_ORDER_NO and c.BUNDLE_NO=b.BUNDLE_NO and c.PART_CD=b.PART_CD where b.WIP<>0 and c.DOC_NO in" + barcode + ")as y where z.STOCK_ID=y.STOCK_ID";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //Reduce
        public SqlDataReader defectinformation(SqlConnection sqlConn, string barcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.PROCESS_CD,c.JOB_ORDER_NO,c.BUNDLE_NO,e.PART_DESC,d.REASON_DESC,a.QTY as DEFECT_QTY from CIPMS_BUNDLE_DEFECT as a inner join PRD_REASON_CODE as d on a.DEFECT_REASON_CD=d.REASON_CD inner join CIPMS_TRANSFER_DT as b on a.TRX_ID=b.TRX_ID inner join CIPMS_BUNDLE_FOR_SCANNING as c on c.JOB_ORDER_NO=b.JOB_ORDER_NO and c.BUNDLE_NO=b.BUNDLE_NO and c.PART_CD=b.PART_CD inner join CIPMS_PART_MASTER as e on c.PART_CD=e.PART_CD where c.BARCODE='" + barcode + "' order by a.PROCESS_CD";
            return sqlComGet.ExecuteReader();
        }
        //transaction defect list
        public SqlDataReader defectinformation(SqlConnection sqlConn, string barcode, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select c.BARCODE,e.PART_DESC,c.QTY as CUT_QTY,d.REASON_DESC,a.QTY as DEFECT_QTY,a.PROCESS_CD from CIPMS_BUNDLE_DEFECT as a inner join PRD_REASON_CODE as d on a.DEFECT_REASON_CD=d.REASON_CD inner join CIPMS_TRANSFER_DT as b on a.TRX_ID=b.TRX_ID inner join CIPMS_BUNDLE_FOR_SCANNING as c on c.JOB_ORDER_NO=b.JOB_ORDER_NO and c.BUNDLE_NO=b.BUNDLE_NO and c.PART_CD=b.PART_CD inner join CIPMS_PART_MASTER as e on c.PART_CD=e.PART_CD where c.BARCODE='" + barcode + "' and c.PART_CD='" + part + "' order by a.PROCESS_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader reduceinformation(SqlConnection sqlConn, string barcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select c.JOB_ORDER_NO,c.COLOR_CD,c.SIZE_CD,c.BUNDLE_NO,d.REASON_DESC,b.REDUCE_QTY from CIPMS_REDUCE_HD as a inner join CIPMS_REDUCE_DT as b on a.ID=b.ID inner join PRD_REASON_CODE as d on b.REDUCE_REASON_CD=d.REASON_CD inner join (select JOB_ORDER_NO,COLOR_CD,SIZE_CD,BUNDLE_NO,BARCODE from CIPMS_BUNDLE_FOR_SCANNING where BARCODE='" + barcode + "' group by JOB_ORDER_NO,COLOR_CD,SIZE_CD,BUNDLE_NO,BARCODE)as c on c.JOB_ORDER_NO=a.JOB_ORDER_NO and c.BUNDLE_NO=b.BUNDLE_NO";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getjobundle(SqlConnection sqlConn, string bundlebarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select JOB_ORDER_NO,BUNDLE_NO from CIPMS_BUNDLE_FOR_SCANNING where BARCODE='" + bundlebarcode + "' group by JOB_ORDER_NO,BUNDLE_NO";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getwipdatanew(SqlConnection sqlConn, string jo, string bundle)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT GARMENT_TYPE,PROCESS_TYPE,PRODUCTION_FACTORY,COLOR_CD,SIZE_CD,CUT_LINE,QTY FROM CUT_BUNDLE_HD WHERE JOB_ORDER_NO='" + jo + "' AND BUNDLE_NO='" + bundle + "' AND TRX_TYPE='NM'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader minqty(SqlConnection sqlConn, string barcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.PROCESS_CD,MIN(c.WIP)as MINQTY from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_JO_WIP_HD as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CODE and a.SIZE_CD=b.SIZE_CODE and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD inner join CIPMS_JO_WIP_BUNDLE as c on c.STOCK_ID=b.STOCK_ID and a.BUNDLE_NO=c.BUNDLE_NO where a.BARCODE='" + barcode + "' group by a.PROCESS_CD";
            return sqlComGet.ExecuteReader();
        }
        public void reduceupdatebfs(SqlConnection sqlConn, string barcode, string reduceqty, string userbarcode)
        {
            string sql = "update CIPMS_BUNDLE_FOR_SCANNING set DISCREPANCY_QTY=DISCREPANCY_QTY+(" + reduceqty + "), LAST_MODI_USER_ID='" + userbarcode + "', LAST_MODI_DATE=GETDATE() where BARCODE='" + barcode + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void reduceupdatewiphd(SqlConnection sqlConn, string barcode, string reduceqty)
        {
            string sql = "update CIPMS_JO_WIP_HD set MATCHING=A.MATCHING-" + reduceqty + " FROM CIPMS_JO_WIP_HD AS A, (SELECT A.JOB_ORDER_NO,A.COLOR_CD,A.SIZE_CD,A.PART_CD,A.PROCESS_CD,A.PRODUCTION_LINE_CD,A.GARMENT_TYPE,A.PROCESS_TYPE FROM CIPMS_BUNDLE_FOR_SCANNING AS A INNER JOIN CIPMS_JO_WIP_HD AS B ON A.JOB_ORDER_NO=B.JOB_ORDER_NO AND A.COLOR_CD=B.COLOR_CODE AND A.SIZE_CD=B.SIZE_CODE AND A.PART_CD=B.PART_CD AND A.FACTORY_CD=B.FACTORY_CD AND A.PROCESS_CD=B.PROCESS_CD AND A.PRODUCTION_LINE_CD=B.PRODUCTION_LINE_CD AND A.GARMENT_TYPE=B.GARMENT_TYPE AND A.PROCESS_TYPE=B.PROCESS_TYPE INNER JOIN CIPMS_JO_WIP_BUNDLE AS C ON B.STOCK_ID=C.STOCK_ID AND A.BUNDLE_NO=C.BUNDLE_NO WHERE A.BARCODE='" + barcode + "' AND C.STATUS='M' AND A.GARMENT_TYPE='K' AND A.PROCESS_TYPE='I')AS B WHERE A.JOB_ORDER_NO=B.JOB_ORDER_NO AND A.COLOR_CODE=B.COLOR_CD AND A.SIZE_CODE=B.SIZE_CD AND A.PROCESS_CD=B.PROCESS_CD AND A.PRODUCTION_LINE_CD=B.PRODUCTION_LINE_CD AND A.GARMENT_TYPE=B.GARMENT_TYPE AND A.PROCESS_TYPE=B.PROCESS_TYPE";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
            sql = "update CIPMS_JO_WIP_HD set WIP=a.WIP-(" + reduceqty + "),DISCREPANCY_QTY=a.DISCREPANCY_QTY+(" + reduceqty + ") from CIPMS_JO_WIP_HD as a, (select JOB_ORDER_NO,COLOR_CD,SIZE_CD,PROCESS_CD,PRODUCTION_LINE_CD,PART_CD from CIPMS_BUNDLE_FOR_SCANNING where BARCODE='" + barcode + "') as b where b.JOB_ORDER_NO=a.JOB_ORDER_NO and b.COLOR_CD=a.COLOR_CODE and b.SIZE_CD=a.SIZE_CODE and b.PART_CD=a.PART_CD and b.PROCESS_CD=a.PROCESS_CD and b.PRODUCTION_LINE_CD=a.PRODUCTION_LINE_CD";
            cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void reduceupdatewipbundle(SqlConnection sqlConn, string barcode, string reduceqty)
        {
            string sql = "UPDATE CIPMS_JO_WIP_BUNDLE SET MATCHING=A.MATCHING-" + reduceqty + " FROM CIPMS_JO_WIP_BUNDLE AS A, (SELECT C.STOCK_ID,C.BUNDLE_NO,C.PART_CD FROM CIPMS_BUNDLE_FOR_SCANNING AS A INNER JOIN CIPMS_JO_WIP_HD AS B ON A.JOB_ORDER_NO=B.JOB_ORDER_NO AND A.COLOR_CD=B.COLOR_CODE AND A.SIZE_CD=B.SIZE_CODE AND A.PART_CD=B.PART_CD AND A.FACTORY_CD=B.FACTORY_CD AND A.PROCESS_CD=B.PROCESS_CD AND A.PRODUCTION_LINE_CD=B.PRODUCTION_LINE_CD AND A.GARMENT_TYPE=B.GARMENT_TYPE AND A.PROCESS_TYPE=B.PROCESS_TYPE INNER JOIN CIPMS_JO_WIP_BUNDLE AS C ON B.STOCK_ID=C.STOCK_ID AND A.BUNDLE_NO=C.BUNDLE_NO WHERE A.BARCODE='" + barcode + "' AND C.STATUS='M')AS B WHERE A.STOCK_ID=B.STOCK_ID AND A.BUNDLE_NO=B.BUNDLE_NO AND A.PART_CD=B.PART_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
            sql = "update CIPMS_JO_WIP_BUNDLE set WIP=WIP-(" + reduceqty + "),DISCREPANCY_QTY=DISCREPANCY_QTY+(" + reduceqty + ") from CIPMS_JO_WIP_BUNDLE as a, (select e.STOCK_ID,c.BUNDLE_NO from CIPMS_BUNDLE_FOR_SCANNING as c inner join CIPMS_JO_WIP_HD as d on c.JOB_ORDER_NO=d.JOB_ORDER_NO and c.COLOR_CD=d.COLOR_CODE and c.SIZE_CD=d.SIZE_CODE and c.PART_CD=d.PART_CD and c.PROCESS_CD=d.PROCESS_CD and c.PRODUCTION_LINE_CD=d.PRODUCTION_LINE_CD inner join CIPMS_JO_WIP_BUNDLE as e on e.STOCK_ID=d.STOCK_ID and c.BUNDLE_NO=e.BUNDLE_NO where c.BARCODE='" + barcode + "')as b where a.STOCK_ID=b.STOCK_ID and a.BUNDLE_NO=b.BUNDLE_NO";
            cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void insertcutbundlereducehd(SqlConnection sqlConn, string docno, string factorycd, string trxdate, string trxtype, string processcd, string joborderno, string garmenttype, string processtype, string productionfactory, string createuserid, string createusergrpid)
        {
            string sql = "insert into CUT_BUNDLE_REDUCE_HD(DOC_NO,FACTORY_CD,TRX_DATE,TRX_TYPE,PROCESS_CD,JOB_ORDER_NO,GARMENT_TYPE,PROCESS_TYPE,PRODUCTION_FACTORY,CREATE_DATE,CREATE_USER_ID,CREATE_USER_GRP_ID,STATUS) VALUES ('"+docno+"','"+factorycd+"','"+trxdate+"','"+trxtype+"','"+processcd+"','"+joborderno+"','"+garmenttype+"','"+processtype+"','"+productionfactory+"',GETDATE(),'"+createuserid+"','"+factorycd+"','C')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatecutbundlereducehd(SqlConnection sqlConn, string docno, string trxdate, string trxtype, string processcd, string joborderno, string garmenttype, string processtype, string productionfactory)
        {
            string sql = "update CUT_BUNDLE_REDUCE_HD SET TRX_DATE='" + trxdate + "',TRX_TYPE='" + trxtype + "',PROCESS_CD='" + processcd + "',JOB_ORDER_NO='" + joborderno + "',GARMENT_TYPE='" + garmenttype + "',PROCESS_TYPE='" + processtype + "',PRODUCTION_FACTORY='" + productionfactory + "' WHERE DOC_NO='" + docno + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void deletecutbundlereducedt(SqlConnection sqlConn, string docno)
        {
            string sql = "delete from CUT_BUNDLE_REDUCE_DT where DOC_NO='" + docno + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void deletecutbundlereducedt2(SqlConnection sqlConn, string trxid, string newdocno)
        {
            string sql = "delete from CUT_BUNDLE_REDUCE_DT where TRX_ID<>'" + trxid + "' AND DOC_NO='" + newdocno + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void insertcutbundlereducetrx(SqlConnection sqlConn, string docno, string trxid, string trxdate, string trxtype, string factorycd, string processcd, string joborderno, string bundleno, string reduceqty, string reasoncd, string previousqty)
        {
            string sql = "insert into CUT_BUNDLE_REDUCE_TRX(TRX_ID,DOC_NO,FACTORY_CD,TRX_DATE,TRX_TYPE,PROCESS_CD,JOB_ORDER_NO,BUNDLE_NO,REDUCE_QTY,REASON_CD,CREATE_DATE,PREVIOUS_QTY) VALUES ('" + trxid + "','" + docno + "','" + factorycd + "','" + trxdate + "','" + trxtype + "','" + processcd + "','" + joborderno + "','" + bundleno + "','" + reduceqty + "','" + reasoncd + "',GETDATE(),'" + previousqty + "')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //update by Jacob 20151114 ----add column CIPMS_PRINT_BUNDLE='C'
        public void insertcutbundlehd(SqlConnection sqlConn, string joborderno, string bundleno, string factorycd, string trxtype, string qty, string actualprintdate, string userbarcode)
        {
            string sql = "insert into CUT_BUNDLE_HD (JOB_ORDER_NO,BUNDLE_NO,FACTORY_CD,TRX_TYPE,QTY,ACTUAL_PRINT_DATE,CREATE_DATE,CREATE_USER_ID,CREATE_USER_GRP_ID,CIPMS_PRINT_BUNDLE) values ('" + joborderno + "','" + bundleno + "','" + factorycd + "','" + trxtype + "','" + qty + "','" + actualprintdate + "',GETDATE(),'" + userbarcode + "','" + factorycd + "','C')";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatecutbundlehd(SqlConnection sqlConn, string ctrxtypenormal, string joborderno, string bundleno, string factorycd, string ctrxtypereduce, string trxdate)
        {
            string sql = "update CUT_BUNDLE_HD set LAY_DT_ID=a.LAY_DT_ID,PRODUCTION_LINE_CD=a.PRODUCTION_LINE_CD,GARMENT_TYPE=a.GARMENT_TYPE,PROCESS_TYPE=a.PROCESS_TYPE,PRODUCTION_FACTORY=a.PRODUCTION_FACTORY,LAY_NO=a.LAY_NO,CUT_LINE=a.CUT_LINE,COLOR_CD=a.COLOR_CD,SIZE_CD=a.SIZE_CD,RUNNING_NO=a.RUNNING_NO,LAY_TRANS_ID=a.LAY_TRANS_ID from CUT_BUNDLE_HD inner join CUT_BUNDLE_HD as a on CUT_BUNDLE_HD.JOB_ORDER_NO=a.JOB_ORDER_NO and CUT_BUNDLE_HD.BUNDLE_NO=a.BUNDLE_NO and CUT_BUNDLE_HD.FACTORY_CD=a.FACTORY_CD where a.STATUS<>'N' AND a.TRX_TYPE='" + ctrxtypenormal + "' and CUT_BUNDLE_HD.JOB_ORDER_NO='" + joborderno + "' and CUT_BUNDLE_HD.BUNDLE_NO='" + bundleno + "' and CUT_BUNDLE_HD.FACTORY_CD='" + factorycd + "' and CUT_BUNDLE_HD.STATUS<>'N' and CUT_BUNDLE_HD.TRX_TYPE='" + ctrxtypereduce + "' and CUT_BUNDLE_HD.ACTUAL_PRINT_DATE='" + trxdate + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader checknewdocno(SqlConnection sqlConn, string newrunningno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select DOC_NO from CUT_BUNDLE_REDUCE_HD where DOC_NO='"+newrunningno+"'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader reducejo(SqlConnection sqlConn, string barcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.JOB_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_REDUCE_HD as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO where a.BARCODE='" + barcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public void reduceinsertjo(SqlConnection sqlConn, string barcode, string factory, string process, string garmenttype, string userbarcode)
        {
            string sql = "insert into CIPMS_REDUCE_HD (FACTORY_CD,PROCESS_CD,JOB_ORDER_NO,GARMENT_TYPE,CREATE_USER_ID,CREATE_DATE) select '" + factory + "','" + process + "',JOB_ORDER_NO,'" + garmenttype + "','" + userbarcode + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING where BARCODE='" + userbarcode + "' group by JOB_ORDER_NO";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void reducehddt(SqlConnection sqlConn, string barcode, string reduceqty, string reducereason, string userbarcode)
        {
            string sql = "insert into CIPMS_REDUCE_DT (ID,BUNDLE_NO,REDUCE_QTY,REDUCE_REASON_CD,CREATE_USER_ID,CREATE_DATE)select b.ID,a.BUNDLE_NO," + reduceqty + ",'" + reducereason + "','" + userbarcode + "',GETDATE() from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_REDUCE_HD as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO where a.BARCODE='" + barcode + "' group by b.ID,a.BUNDLE_NO";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        //employee output
        public SqlDataReader userinformation(SqlConnection sqlConn, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select NAME,PRC_CD,PRODUCTION_LINE_CD from CIPMS_USER where USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader bundleishere(SqlConnection sqlConn, string barcode, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PROCESS_CD from CIPMS_BUNDLE_FOR_SCANNING where BARCODE='" + barcode + "' and PART_CD='" + part + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader bundleishere(SqlConnection sqlConn, string barcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PROCESS_CD,CARTON_STATUS from CIPMS_CARTON where CARTON_BARCODE='" + barcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader havebeenscanned(SqlConnection sqlConn, string barcode, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select c.EMPLOYEE_OUTPUT from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_JO_WIP_HD as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CODE and a.SIZE_CD=b.SIZE_CODE and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD inner join CIPMS_JO_WIP_BUNDLE as c on b.STOCK_ID=c.STOCK_ID and a.BUNDLE_NO=c.BUNDLE_NO where a.BARCODE='" + barcode + "' and a.PART_CD='" + part + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader whoscanned(SqlConnection sqlConn, string who)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select NAME from CIPMS_USER where USER_BARCODE='" + who + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader havebeenscanned(SqlConnection sqlConn, string barcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select c.EMPLOYEE_OUTPUT from CIPMS_BUNDLE_FOR_SCANNING as a with (nolock) inner join CIPMS_JO_WIP_HD as b with (nolock) on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CODE and a.SIZE_CD=b.SIZE_CODE and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD inner join CIPMS_JO_WIP_BUNDLE as c with (nolock) on b.STOCK_ID=c.STOCK_ID and a.BUNDLE_NO=c.BUNDLE_NO where a.CARTON_BARCODE='" + barcode + "' and a.CARTON_STATUS='C' and c.EMPLOYEE_OUTPUT<>'0'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader bundleinformation(SqlConnection sqlConn, string barcode, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select CARTON_BARCODE,CARTON_STATUS,BARCODE,PART_CD,JOB_ORDER_NO,BUNDLE_NO,SIZE_CD,COLOR_CD,LAY_NO,QTY,DISCREPANCY_QTY from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where BARCODE='" + barcode + "' and PART_CD='" + part + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader bundleinformation(SqlConnection sqlConn, string barcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select CARTON_BARCODE,BARCODE,PART_CD,JOB_ORDER_NO,BUNDLE_NO,SIZE_CD,COLOR_CD,LAY_NO,QTY,DISCREPANCY_QTY from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where CARTON_BARCODE='" + barcode + "' and CARTON_STATUS='C'";
            return sqlComGet.ExecuteReader();
        }
        public void updatewipbundleuser(SqlConnection sqlConn, string barcode, string part, string userbarcode)
        {
            string sql = "update CIPMS_JO_WIP_BUNDLE set EMPLOYEE_OUTPUT='" + userbarcode + "' from CIPMS_JO_WIP_BUNDLE as d, (select b.STOCK_ID,c.BUNDLE_NO from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_JO_WIP_HD as b with (nolock) on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CODE and a.SIZE_CD=b.SIZE_CODE and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD inner join CIPMS_JO_WIP_BUNDLE as c with (nolock) on b.STOCK_ID=c.STOCK_ID and a.BUNDLE_NO=c.BUNDLE_NO where a.BARCODE='" + barcode + "' and a.PART_CD='" + part + "')as e where d.STOCK_ID=e.STOCK_ID and d.BUNDLE_NO=e.BUNDLE_NO";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatewipbundleuser(SqlConnection sqlConn, string barcode, string userbarcode)
        {
            string sql = "update CIPMS_JO_WIP_BUNDLE set EMPLOYEE_OUTPUT='" + userbarcode + "' from CIPMS_JO_WIP_BUNDLE as d, (select b.STOCK_ID,c.BUNDLE_NO from CIPMS_BUNDLE_FOR_SCANNING as a inner join CIPMS_JO_WIP_HD as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CODE and a.SIZE_CD=b.SIZE_CODE and a.PART_CD=b.PART_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD inner join CIPMS_JO_WIP_BUNDLE as c on b.STOCK_ID=c.STOCK_ID and a.BUNDLE_NO=c.BUNDLE_NO where a.CARTON_BARCODE='" + barcode + "' and a.CARTON_STATUS='C')as e where d.STOCK_ID=e.STOCK_ID and d.BUNDLE_NO=e.BUNDLE_NO";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader checkdocno(SqlConnection sqlConn, string barcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select DOC_NO from CIPMS_TRANSFER_HD with (nolock) where DOC_NO='" + barcode + "' and (STATUS='S' or STATUS='M' or STATUS='N' or STATUS='X' or STATUS='Y' or STATUS='A')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getsubmituser(SqlConnection sqlConn, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select CREATE_USER_ID from CIPMS_TRANSFER_HD with (nolock) where DOC_NO='"+docno+"'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader ifincarton(SqlConnection sqlConn, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where DOC_NO='" + docno + "' and CARTON_STATUS='C'";
            return sqlComGet.ExecuteReader();
        }
    }

}
