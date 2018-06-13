using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;

/// <summary>
///Package 的摘要说明
/// </summary>
/// 
namespace NameSpace
{
    public class Packagesql
    {
        public Packagesql()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }
        public SqlDataReader part(SqlConnection sqlConn, string jo)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            //sqlComGet.CommandText = "select c.PART_CD,c.PART_DESC from PRD_GO_ROUTE_LIST_HD as a inner join PRD_GO_ROUTE_LIST_DT as b on a.ROUTE_HD_ID = b.ROUTE_HD_ID inner join CIPMS_PART_MASTER as c on b.PART_DESC=c.PART_DESC inner join JO_HD as d on a.GO_NO=d.SC_NO where d.JO_NO='" + jo + "' group by c.PART_CD,c.PART_DESC,c.ID order by c.ID";
            sqlComGet.CommandText = "select c.PART_CD,c.PART_DESC from PRD_GO_ROUTE_LIST_HD as a  with (nolock)  inner join PRD_GO_ROUTE_LIST_DT as b  with (nolock) on a.ROUTE_HD_ID = b.ROUTE_HD_ID inner join GEN_PART_MASTER as e  with (nolock) on a.FACTORY_CD=e.FACTORY_CD and b.PART_CD=e.PART_CD inner join CIPMS_PART_MASTER as c  with (nolock) on c.PART_DESC=e.PART_DESC inner join JO_HD as d on a.GO_NO=d.SC_NO where d.JO_NO='" + jo + "' group by c.PART_CD,c.PART_DESC,c.SEQ_NO order by c.SEQ_NO";
            return sqlComGet.ExecuteReader();
        }

        //query
        public SqlDataReader query(SqlConnection sqlConn, string userbarcode, string functioncd)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT DISTINCT top 5 CONVERT(VARCHAR(30),CREATE_DATE,121) AS DATE_TIME FROM CIPMS_SAVE_AND_QUERY  with (nolock) WHERE CREATE_USER_ID='" + userbarcode + "' AND FUNCTION_CD='" + functioncd + "' ORDER BY CONVERT(VARCHAR(30),CREATE_DATE,121) DESC";
            return sqlComGet.ExecuteReader();
        }

        //empty list
        public void emptylist(SqlConnection sqlConn, string docno, string userbarcode)
        {
            string sql = "delete from CIPMS_USER_SCANNING_DFT  with (nolock) WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }

        //layno
        public SqlDataReader querylayno(SqlConnection sqlConn, string bundlebarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select DISTINCT LAY_NO from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where BARCODE='" + bundlebarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader layno(SqlConnection sqlConn, string jo)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select LAY_NO from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where JOB_ORDER_NO='" + jo + "' GROUP BY LAY_NO ORDER BY LAY_NO";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader colorcode(SqlConnection sqlConn, string jo)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select  distinct COLOR_CODE from JO_DT with (nolock) where JO_NO='"+jo+"'";
            return sqlComGet.ExecuteReader();
        }

        //cartonbarcodecheck
        public SqlDataReader cartonbarcodecheck(SqlConnection sqlConn, string barcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select ID from CIPMS_CARTON  with (nolock) where CARTON_BARCODE='" + barcode + "'";
            return sqlComGet.ExecuteReader();
        }

        public SqlDataReader getparttranslation(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.PART_DESC,b.PART_CD from CIPMS_USER_SCANNING_DFT as a  with (nolock) inner join CIPMS_PART_MASTER as b  with (nolock) on a.PART_CD=b.PART_CD where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "' group by b.PART_DESC,b.PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getremainpart(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.PART_DESC from CIPMS_PART_MASTER as a  with (nolock)  inner join (select a.PART_CD from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock)  inner join (select a.JOB_ORDER_NO,a.BUNDLE_NO from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_USER_SCANNING_DFT as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "') as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO group by a.PART_CD except select PART_CD from CIPMS_USER_SCANNING_DFT  with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "' group by PART_CD)as b on a.PART_CD=b.PART_CD";
            return sqlComGet.ExecuteReader();
        }

        //bundle scan
        public SqlDataReader checkbundleexist1(SqlConnection sqlConn, string bundlebarcode, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where BARCODE='" + bundlebarcode + "' and PART_CD in " + part;
            return sqlComGet.ExecuteReader();
        }


        //tangyh 2017.03.27 
        public SqlDataReader checkbundlecolor(SqlConnection sqlConn, string bundlebarcode, string colorcode,string barcodetype)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            switch (barcodetype)
            {
                case "B":
                    sqlComGet.CommandText = "select top 1 1 from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where BARCODE='" + bundlebarcode + "' and COLOR_CD<> '" + colorcode+"'";
                    break;
                case "C":
                    sqlComGet.CommandText = "select top 1 1 from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where CARTON_BARCODE='" + bundlebarcode + "' and COLOR_CD<> '" + colorcode+"'";
                    break;
                      
            }
            return sqlComGet.ExecuteReader();
        }


        public SqlDataReader checkbundleexist2(SqlConnection sqlConn, string bundlebarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where BARCODE='" + bundlebarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkbundleinjo(SqlConnection sqlConn, string bundlebarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select JOB_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where BARCODE='" + bundlebarcode + "' group by JOB_ORDER_NO";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getmaincartonpart(SqlConnection sqlConn, string factory, string bundlebarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            //sqlComGet.CommandText = "select PART_CD from CIPMS_BUNDLE_FOR_SCANNING where GARMENT_ORDER_NO in (select GARMENT_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING where BARCODE='" + bundlebarcode + "' group by GARMENT_ORDER_NO) AND LEN(CARTON_BARCODE)=13 and SUBSTRING(CARTON_BARCODE,10,1)='-' and CARTON_STATUS='C' group by PART_CD";
            sqlComGet.CommandText = "SELECT PART_CD FROM (select A.CARTON_BARCODE,A.PART_CD from CIPMS_BUNDLE_FOR_SCANNING AS A  with (nolock) INNER JOIN CIPMS_CARTON_DT AS B  with (nolock) ON A.JOB_ORDER_NO=B.JOB_ORDER_NO AND A.BUNDLE_NO=B.BUNDLE_NO where A.FACTORY_CD='" + factory + "' and A.GARMENT_ORDER_NO in (select GARMENT_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where BARCODE='" + bundlebarcode + "' group by GARMENT_ORDER_NO) AND B.STATUS='C')AS A WHERE LEN(CARTON_BARCODE)=13 AND SUBSTRING(CARTON_BARCODE,10,1)='-' GROUP BY PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getmaincartonpart(SqlConnection sqlConn, string joborderno, int layno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.PART_CD from CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) inner join (select CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING with (nolock)  where JOB_ORDER_NO='" + joborderno + "' and LAY_NO=" + layno + " and CARTON_STATUS='C' group by CARTON_BARCODE)as a on a.CARTON_BARCODE=b.CARTON_BARCODE where LEN(a.CARTON_BARCODE)=13 and SUBSTRING(a.CARTON_BARCODE,10,1)='-' and b.CARTON_STATUS='C' group by b.PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkifallinmaincarton(SqlConnection sqlConn, string joborderno, int layno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_NO from CIPMS_BUNDLE_FOR_SCANNING  with (nolock)  where JOB_ORDER_NO='" + joborderno + "' and LAY_NO=" + layno + " and BUNDLE_NO not in (select b.BUNDLE_NO from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_CARTON_DT as b  with (nolock) on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO where a.JOB_ORDER_NO='" + joborderno + "' and a.LAY_NO=" + layno + " and b.STATUS='C' group by b.BUNDLE_NO) group by BUNDLE_NO";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getwholepart(SqlConnection sqlConn, string bundlebarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PART_CD from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where BARCODE='" + bundlebarcode + "' group by PART_CD";
            return sqlComGet.ExecuteReader();
        }
        //isprocess
        public SqlDataReader isprocess(SqlConnection sqlConn, string bundlebarcode, string part, string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where BARCODE='" + bundlebarcode + "' and PART_CD in" + part + " and (PROCESS_CD<>'" + process + "' or PROCESS_TYPE<>'I')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader closecarton(SqlConnection sqlConn, string bundlebarcode, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where BARCODE='" + bundlebarcode + "' and PART_CD in" + part + " and CARTON_STATUS='C'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader closecartonlayno(SqlConnection sqlConn, string joborderno, string layno, string part)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where JOB_ORDER_NO='" + joborderno + "' and LAY_NO='" + layno + "' and PART_CD in" + part + " and CARTON_STATUS='C'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getpart(SqlConnection sqlConn, string joborderno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PART_CD from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) WHERE JOB_ORDER_NO='" + joborderno + "' group by PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader closecartonall(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_USER_SCANNING_DFT as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "' and a.CARTON_STATUS='C'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader closecartonproduction(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(DISTINCT PRODUCTION_LINE_CD) AS QTY from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock)  inner join CIPMS_USER_SCANNING_DFT as b  with (nolock)  on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader closecartonquery(SqlConnection sqlConn, string date, string userbarcode, string functioncd)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING  with (nolock)  WHERE BUNDLE_ID IN (SELECT BUNDLE_ID from CIPMS_SAVE_AND_QUERY  with (nolock) WHERE CREATE_DATE='" + date + "' AND CREATE_USER_ID='" + userbarcode + "' AND FUNCTION_CD='" + functioncd + "') AND CARTON_STATUS='C'";
            return sqlComGet.ExecuteReader();
        }

        //close carton
        public SqlDataReader getonejobundle(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.JOB_ORDER_NO,b.BUNDLE_NO from CIPMS_USER_SCANNING_DFT as a  with (nolock)  inner join CIPMS_BUNDLE_FOR_SCANNING as b with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getpartnum(SqlConnection sqlConn, string jo, string bundle, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(PART_CD)AS QTY from (select b.PART_CD from CIPMS_USER_SCANNING_DFT as a  with (nolock)  inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock)  on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' and b.JOB_ORDER_NO='" + jo + "' and b.BUNDLE_NO='" + bundle + "')as c";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getbundlenum(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(BUNDLE_NO)as QTY from (select b.JOB_ORDER_NO,b.BUNDLE_NO from CIPMS_USER_SCANNING_DFT as a  with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' GROUP BY b.JOB_ORDER_NO,b.BUNDLE_NO)as c";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getscanrownum(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT COUNT(BUNDLE_ID)AS QTY FROM CIPMS_USER_SCANNING_DFT  with (nolock) WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getscanpartnum(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(PART_CD)as QTY from (SELECT PART_CD FROM CIPMS_USER_SCANNING_DFT  with (nolock) WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "' group by PART_CD)as c";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader cartonstatus(SqlConnection sqlConn, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select CARTON_STATUS,PROCESS_CD,PROCESS_TYPE from CIPMS_CARTON  with (nolock) where CARTON_BARCODE='" + cartonbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader closecheckbundle(SqlConnection sqlConn, string docno, string userbarcode, string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING with (nolock) where BUNDLE_ID in(select BUNDLE_ID from CIPMS_USER_SCANNING_DFT  with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') and (PROCESS_CD<>'" + process + "' OR PROCESS_TYPE<>'I' OR CARTON_STATUS='C')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkmaincartonclose(SqlConnection sqlConn, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select ID from CIPMS_CARTON  with (nolock) where CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkgo(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(1) as QTY from (select b.GARMENT_ORDER_NO from CIPMS_USER_SCANNING_DFT as a  with (nolock)  inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' group by b.GARMENT_ORDER_NO)as c";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkgoclosed(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select c.JOB_ORDER_NO from CIPMS_CARTON_DT as c  with (nolock) inner join (select JOB_ORDER_NO,BUNDLE_NO from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where GARMENT_ORDER_NO in (select b.GARMENT_ORDER_NO from CIPMS_USER_SCANNING_DFT as a  with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' group by b.GARMENT_ORDER_NO))as d on c.JOB_ORDER_NO=d.JOB_ORDER_NO and c.BUNDLE_NO=d.BUNDLE_NO where c.STATUS='C'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkpartsame1(SqlConnection sqlConn, string factory, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PART_CD from CIPMS_USER_SCANNING_DFT  with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "' group by PART_CD except select PART_CD from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where FACTORY_CD='" + factory + "' AND GARMENT_ORDER_NO in (SELECT b.GARMENT_ORDER_NO from CIPMS_USER_SCANNING_DFT as a  with (nolock)  inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock)  on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' group by b.GARMENT_ORDER_NO) and SUBSTRING(CARTON_BARCODE,10,1)='-' and LEN(CARTON_BARCODE)=13 and CARTON_STATUS='C' group by PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkpartsame2(SqlConnection sqlConn, string factory, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PART_CD from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where FACTORY_CD='" + factory + "' AND GARMENT_ORDER_NO in (SELECT b.GARMENT_ORDER_NO from CIPMS_USER_SCANNING_DFT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' group by b.GARMENT_ORDER_NO) and SUBSTRING(CARTON_BARCODE,10,1)='-' and LEN(CARTON_BARCODE)=13 and CARTON_STATUS='C' group by PART_CD except select PART_CD from CIPMS_USER_SCANNING_DFT  with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "' group by PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkpartsame3(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PART_CD from CIPMS_USER_SCANNING_DFT  with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "' group by PART_CD except select d.PART_CD from CIPMS_BUNDLE_FOR_SCANNING as d  with (nolock) inner join (select b.JOB_ORDER_NO,b.BUNDLE_NO from CIPMS_USER_SCANNING_DFT as a  with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' group by b.JOB_ORDER_NO,b.BUNDLE_NO)as c on d.JOB_ORDER_NO=c.JOB_ORDER_NO and d.BUNDLE_NO=c.BUNDLE_NO where CARTON_STATUS<>'C' group by d.PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkpartsame4(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select d.PART_CD from CIPMS_BUNDLE_FOR_SCANNING as d  with (nolock) inner join (select b.JOB_ORDER_NO,b.BUNDLE_NO from CIPMS_USER_SCANNING_DFT as a   with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' group by b.JOB_ORDER_NO,b.BUNDLE_NO)as c on d.JOB_ORDER_NO=c.JOB_ORDER_NO and d.BUNDLE_NO=c.BUNDLE_NO where CARTON_STATUS<>'C' group by d.PART_CD except select PART_CD from CIPMS_USER_SCANNING_DFT  with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "' group by PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checksamemaincarton(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(1)as QTY from (select e.CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING as e  with (nolock) inner join (select a.JOB_ORDER_NO,a.BUNDLE_NO from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock)  inner join CIPMS_USER_SCANNING_DFT as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "')as d on e.JOB_ORDER_NO=d.JOB_ORDER_NO and e.BUNDLE_NO=d.BUNDLE_NO where len(e.CARTON_BARCODE)=13 and e.CARTON_STATUS='C' group by e.CARTON_BARCODE)as c";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getsize(SqlConnection sqlConn, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select SIZE_CD from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C' group by SIZE_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getjocolor(SqlConnection sqlConn, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select JOB_ORDER_NO,COLOR_CD,SUM(QTY)as QTY from (select JOB_ORDER_NO,COLOR_CD,SIZE_CD,MIN(QTY)as QTY from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C' group by JOB_ORDER_NO,BUNDLE_NO)as a group by JOB_ORDER_NO,COLOR_CD order by COLOR_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getlayno(SqlConnection sqlConn, string jo, string color, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select LAY_NO,COUNT(BUNDLE_ID)as QTY from CIPMS_BUNDLE_FOR_SCANNING  with (nolock)  where JOB_ORDER_NO='" + jo + "' and COLOR_CD='" + color + "' and CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader gettotalbundle(SqlConnection sqlConn, string jo, string color, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(a.BUNDLE_NO)as QTY from (select BUNDLE_NO from CIPMS_BUNDLE_FOR_SCANNING  with (nolock)  where JOB_ORDER_NO='" + jo + "' and COLOR_CD='" + color + "' and CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C' group by JOB_ORDER_NO,BUNDLE_NO) as a";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getcolorqty(SqlConnection sqlConn, string color, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select SUM(QTY)as QTY from (select JOB_ORDER_NO,COLOR_CD,SIZE_CD,MIN(QTY)as QTY from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C' group by JOB_ORDER_NO,BUNDLE_NO)as a where COLOR_CD='" + color + "' group by COLOR_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getcolornum(SqlConnection sqlConn, string color, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(JOB_ORDER_NO)as QTY from (SELECT JOB_ORDER_NO,COLOR_CD FROM CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where COLOR_CD='" + color + "' and CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C' GROUP BY JOB_ORDER_NO,COLOR_CD)as a";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getttlbygocolor(SqlConnection sqlConn, string color, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT SUM(QTY)AS QTY FROM (SELECT MIN(D.WIP)AS QTY FROM CIPMS_BUNDLE_FOR_SCANNING AS B  with (nolock) INNER JOIN CIPMS_JO_WIP_HD AS C ON B.JOB_ORDER_NO=C.JOB_ORDER_NO AND B.COLOR_CD=C.COLOR_CODE AND B.SIZE_CD=C.SIZE_CODE AND B.PART_CD=C.PART_CD AND B.FACTORY_CD=C.FACTORY_CD AND B.PROCESS_CD=C.PROCESS_CD AND B.PRODUCTION_LINE_CD=C.PRODUCTION_LINE_CD AND B.GARMENT_TYPE=C.GARMENT_TYPE AND B.PROCESS_TYPE=C.PROCESS_TYPE INNER JOIN CIPMS_JO_WIP_BUNDLE AS D ON C.STOCK_ID=D.STOCK_ID AND D.BUNDLE_NO=B.BUNDLE_NO WHERE B.COLOR_CD='" + color + "' AND B.CARTON_BARCODE='" + cartonbarcode + "' AND B.CARTON_STATUS='C' GROUP BY B.JOB_ORDER_NO,B.BUNDLE_NO)AS B";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getpackagepart(SqlConnection sqlConn, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.PART_DESC from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock)  inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD where CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C' group by b.PART_DESC";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getshortpackagepart(SqlConnection sqlConn, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT B.PART_DESC FROM (SELECT A.PART_CD FROM CIPMS_BUNDLE_FOR_SCANNING AS A  with (nolock) INNER JOIN (SELECT JOB_ORDER_NO,BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING WHERE CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C' GROUP BY JOB_ORDER_NO,BUNDLE_NO)AS B ON A.JOB_ORDER_NO=B.JOB_ORDER_NO AND A.BUNDLE_NO=B.BUNDLE_NO GROUP BY A.PART_CD EXCEPT select PART_CD from CIPMS_BUNDLE_FOR_SCANNING where CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C' group by PART_CD)AS A INNER JOIN CIPMS_PART_MASTER AS B ON A.PART_CD=B.PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlCommand getjocolordb(SqlConnection sqlConn, string cartonbarcode)
        {
            string sql = "select JOB_ORDER_NO,COLOR_CD,SIZE_CD,SUM(QTY-DISCREPANCY_QTY)as QTY from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C' group by JOB_ORDER_NO,COLOR_CD,SIZE_CD order by COLOR_CD";
            SqlCommand sqlComGet = new SqlCommand(sql, sqlConn);
            return sqlComGet;
        }
        public SqlCommand getjocolordb(SqlConnection sqlConn, string sql, string cartonbarcode)
        {
            //sql = "select JOB_ORDER_NO,COLOR_CD,LAY_NO=stuff((select '，'+CAST(LAY_NO AS VARCHAR(10)) from CIPMS_BUNDLE_FOR_SCANNING where JOB_ORDER_NO=a.JOB_ORDER_NO and COLOR_CD=a.COLOR_CD AND CARTON_BARCODE='" + cartonbarcode + "' AND CARTON_STATUS='C' GROUP BY LAY_NO for XML path('')),1,1,''), (SELECT COUNT(BUNDLE_NO) FROM (SELECT BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO=a.JOB_ORDER_NO AND COLOR_CD=a.COLOR_CD AND CARTON_BARCODE='" + cartonbarcode + "' AND CARTON_STATUS='C' GROUP BY JOB_ORDER_NO,COLOR_CD,BUNDLE_NO)AS B)AS BUNDLE, (SELECT SUM(QTY) FROM (SELECT BUNDLE_NO,MIN(QTY-DISCREPANCY_QTY)AS QTY FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO=a.JOB_ORDER_NO AND COLOR_CD=a.COLOR_CD AND CARTON_BARCODE='" + cartonbarcode + "' AND CARTON_STATUS='C' GROUP BY JOB_ORDER_NO,COLOR_CD,BUNDLE_NO)AS B)as TOTAL_QTY, (SELECT SUM(QTY) FROM (SELECT BUNDLE_NO,MIN(QTY)AS QTY FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO=a.JOB_ORDER_NO AND COLOR_CD=a.COLOR_CD GROUP BY JOB_ORDER_NO,COLOR_CD,BUNDLE_NO)AS B)as CUT_QTY, (SELECT SUM(QTY) FROM (SELECT BUNDLE_NO,MIN(QTY)AS QTY FROM CIPMS_BUNDLE_FOR_SCANNING WHERE COLOR_CD=a.COLOR_CD AND CARTON_BARCODE='" + cartonbarcode + "' AND CARTON_STATUS='C' GROUP BY JOB_ORDER_NO,COLOR_CD,BUNDLE_NO)AS B)AS TTL" + sql + " from CIPMS_BUNDLE_FOR_SCANNING AS a WHERE CARTON_BARCODE='" + cartonbarcode + "' AND CARTON_STATUS='C' GROUP BY JOB_ORDER_NO,COLOR_CD ORDER BY COLOR_CD";
            //sql = "select JOB_ORDER_NO,COLOR_CD,LAY_NO=stuff((select '，'+CAST(LAY_NO AS VARCHAR(10)) from CIPMS_BUNDLE_FOR_SCANNING where JOB_ORDER_NO=a.JOB_ORDER_NO and COLOR_CD=a.COLOR_CD AND CARTON_BARCODE='"+cartonbarcode+"' AND CARTON_STATUS='C' GROUP BY LAY_NO for XML path('')),1,1,''), (SELECT COUNT(BUNDLE_NO) FROM (SELECT BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO=a.JOB_ORDER_NO AND COLOR_CD=a.COLOR_CD AND CARTON_BARCODE='"+cartonbarcode+"' AND CARTON_STATUS='C' GROUP BY JOB_ORDER_NO,COLOR_CD,BUNDLE_NO)AS B)AS BUNDLE, (SELECT SUM(QTY) FROM (SELECT B.BUNDLE_NO,MIN(D.WIP)AS QTY FROM CIPMS_BUNDLE_FOR_SCANNING AS B INNER JOIN CIPMS_JO_WIP_HD AS C ON B.JOB_ORDER_NO=C.JOB_ORDER_NO AND B.COLOR_CD=C.COLOR_CODE AND B.SIZE_CD=C.SIZE_CODE AND B.PART_CD=C.PART_CD AND B.PROCESS_CD=C.PROCESS_CD AND B.PRODUCTION_LINE_CD=C.PRODUCTION_LINE_CD AND B.GARMENT_TYPE=C.GARMENT_TYPE AND B.PROCESS_TYPE=C.PROCESS_TYPE INNER JOIN CIPMS_JO_WIP_BUNDLE AS D ON C.STOCK_ID=D.STOCK_ID AND D.BUNDLE_NO=B.BUNDLE_NO WHERE B.JOB_ORDER_NO=a.JOB_ORDER_NO AND B.COLOR_CD=a.COLOR_CD AND B.CARTON_BARCODE='"+cartonbarcode+"' AND B.CARTON_STATUS='C' GROUP BY B.JOB_ORDER_NO,B.COLOR_CD,B.BUNDLE_NO)AS B)as TOTAL_QTY, (SELECT SUM(QTY) FROM CUT_BUNDLE_HD WHERE JOB_ORDER_NO=a.JOB_ORDER_NO AND COLOR_CD=a.COLOR_CD GROUP BY JOB_ORDER_NO,COLOR_CD)as CUT_QTY"+sql+" from (select B.JOB_ORDER_NO,B.COLOR_CD,B.SIZE_CD,B.BUNDLE_NO,MIN(D.WIP)AS QTY from CIPMS_BUNDLE_FOR_SCANNING AS B INNER JOIN CIPMS_JO_WIP_HD AS C ON B.JOB_ORDER_NO=C.JOB_ORDER_NO AND B.COLOR_CD=C.COLOR_CODE AND B.SIZE_CD=C.SIZE_CODE AND B.PART_CD=C.PART_CD AND B.PROCESS_CD=C.PROCESS_CD AND B.PRODUCTION_LINE_CD=C.PRODUCTION_LINE_CD AND B.GARMENT_TYPE=C.GARMENT_TYPE AND B.PROCESS_TYPE=C.PROCESS_TYPE INNER JOIN CIPMS_JO_WIP_BUNDLE AS D ON C.STOCK_ID=D.STOCK_ID AND B.BUNDLE_NO=D.BUNDLE_NO WHERE B.CARTON_BARCODE='"+cartonbarcode+"' AND CARTON_STATUS='C' GROUP BY B.JOB_ORDER_NO,B.COLOR_CD,B.SIZE_CD,B.BUNDLE_NO) AS a GROUP BY a.JOB_ORDER_NO,a.COLOR_CD ORDER BY a.COLOR_CD";
            sql = "select JOB_ORDER_NO,COLOR_CD,LAY_NO=stuff((select '，'+CAST(LAY_NO AS VARCHAR(10)) from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where JOB_ORDER_NO=a.JOB_ORDER_NO and COLOR_CD=a.COLOR_CD AND CARTON_BARCODE='" + cartonbarcode + "' AND CARTON_STATUS='C' GROUP BY LAY_NO for XML path('')),1,1,''), (SELECT COUNT(BUNDLE_NO) FROM (SELECT BUNDLE_NO FROM CIPMS_BUNDLE_FOR_SCANNING WHERE JOB_ORDER_NO=a.JOB_ORDER_NO AND COLOR_CD=a.COLOR_CD AND CARTON_BARCODE='" + cartonbarcode + "' AND CARTON_STATUS='C' GROUP BY JOB_ORDER_NO,COLOR_CD,BUNDLE_NO)AS B)AS BUNDLE, (SELECT SUM(QTY) FROM CUT_BUNDLE_HD WHERE JOB_ORDER_NO=a.JOB_ORDER_NO AND COLOR_CD=a.COLOR_CD GROUP BY JOB_ORDER_NO,COLOR_CD)as CUT_QTY" + sql + " from (select B.JOB_ORDER_NO,B.COLOR_CD,B.SIZE_CD,B.BUNDLE_NO,MIN(D.WIP)AS QTY from CIPMS_BUNDLE_FOR_SCANNING AS B INNER JOIN CIPMS_JO_WIP_HD AS C ON B.JOB_ORDER_NO=C.JOB_ORDER_NO AND B.COLOR_CD=C.COLOR_CODE AND B.SIZE_CD=C.SIZE_CODE AND B.PART_CD=C.PART_CD AND B.FACTORY_CD=C.FACTORY_CD AND B.PROCESS_CD=C.PROCESS_CD AND B.PRODUCTION_LINE_CD=C.PRODUCTION_LINE_CD AND B.GARMENT_TYPE=C.GARMENT_TYPE AND B.PROCESS_TYPE=C.PROCESS_TYPE INNER JOIN CIPMS_JO_WIP_BUNDLE AS D ON C.STOCK_ID=D.STOCK_ID AND B.BUNDLE_NO=D.BUNDLE_NO WHERE B.CARTON_BARCODE='" + cartonbarcode + "' AND CARTON_STATUS='C' GROUP BY B.JOB_ORDER_NO,B.COLOR_CD,B.SIZE_CD,B.BUNDLE_NO) AS a GROUP BY a.JOB_ORDER_NO,a.COLOR_CD ORDER BY a.COLOR_CD";
            SqlCommand sqlComGet = new SqlCommand(sql, sqlConn);
            return sqlComGet;
        }
        public SqlDataReader gettotalqty(SqlConnection sqlConn, string joborderno, string color, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT SUM(QTY)AS TOTAL_QTY FROM (SELECT B.BUNDLE_NO,MIN(D.WIP)AS QTY FROM CIPMS_BUNDLE_FOR_SCANNING AS B  with (nolock) INNER JOIN CIPMS_JO_WIP_HD AS C  with (nolock) ON B.JOB_ORDER_NO=C.JOB_ORDER_NO AND B.COLOR_CD=C.COLOR_CODE AND B.SIZE_CD=C.SIZE_CODE AND B.PART_CD=C.PART_CD AND B.FACTORY_CD=C.FACTORY_CD AND B.PROCESS_CD=C.PROCESS_CD AND B.PRODUCTION_LINE_CD=C.PRODUCTION_LINE_CD AND B.GARMENT_TYPE=C.GARMENT_TYPE AND B.PROCESS_TYPE=C.PROCESS_TYPE INNER JOIN CIPMS_JO_WIP_BUNDLE AS D  with (nolock) ON C.STOCK_ID=D.STOCK_ID AND D.BUNDLE_NO=B.BUNDLE_NO WHERE B.JOB_ORDER_NO='" + joborderno + "' AND B.COLOR_CD='" + color + "' AND B.CARTON_BARCODE='" + cartonbarcode + "' AND B.CARTON_STATUS='C' GROUP BY B.JOB_ORDER_NO,B.COLOR_CD,B.BUNDLE_NO)AS B";
            sqlComGet.CommandTimeout = 300;
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader gettotalqtydocno(SqlConnection sqlConn, string joborderno, string color, string docno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT SUM(QTY) AS TOTAL_QTY FROM (SELECT B.BUNDLE_NO,MIN(D.WIP)AS QTY FROM CIPMS_BUNDLE_FOR_SCANNING AS B  with (nolock)  INNER JOIN CIPMS_JO_WIP_HD AS C  with (nolock) ON B.JOB_ORDER_NO=C.JOB_ORDER_NO AND B.COLOR_CD=C.COLOR_CODE AND B.SIZE_CD=C.SIZE_CODE AND B.PART_CD=C.PART_CD AND B.FACTORY_CD=C.FACTORY_CD AND B.PROCESS_CD=C.PROCESS_CD AND B.PRODUCTION_LINE_CD=C.PRODUCTION_LINE_CD AND B.GARMENT_TYPE=C.GARMENT_TYPE AND B.PROCESS_TYPE=C.PROCESS_TYPE INNER JOIN CIPMS_JO_WIP_BUNDLE AS D  with (nolock) ON C.STOCK_ID=D.STOCK_ID AND D.BUNDLE_NO=B.BUNDLE_NO WHERE B.JOB_ORDER_NO='" + joborderno + "' AND B.COLOR_CD='" + color + "' AND B.DOC_NO='" + docno + "' GROUP BY B.JOB_ORDER_NO,B.COLOR_CD,B.BUNDLE_NO)AS B ";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getgocus(SqlConnection sqlConn, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.GARMENT_ORDER_NO,c.SHORT_NAME,a.PRODUCTION_LINE_CD from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join JO_HD as b on a.JOB_ORDER_NO=b.JO_NO inner join GEN_CUSTOMER as c on c.CUSTOMER_CD=b.CUSTOMER_CD where a.CARTON_BARCODE='" + cartonbarcode + "' and a.CARTON_STATUS='C' group by a.GARMENT_ORDER_NO,c.SHORT_NAME,a.PRODUCTION_LINE_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkmaincarton(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_USER_SCANNING_DFT as b   with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "' and a.CARTON_STATUS='C' GROUP BY a.CARTON_BARCODE";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkmaincarton(SqlConnection sqlConn, string bundlebarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            //sqlComGet.CommandText = "select CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING where BARCODE='" + bundlebarcode + "' and LEN(CARTON_BARCODE)=13 and SUBSTRING(CARTON_BARCODE,10,1)='-' and CARTON_STATUS='C'";
            sqlComGet.CommandText = "select A.JOB_ORDER_NO from CIPMS_CARTON_DT AS A  with (nolock) INNER JOIN CIPMS_BUNDLE_FOR_SCANNING AS B  with (nolock) ON A.JOB_ORDER_NO=B.JOB_ORDER_NO AND A.BUNDLE_NO=B.BUNDLE_NO WHERE B.BARCODE='" + bundlebarcode + "' AND A.STATUS='C'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checksecondcarton(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select d.CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING as d  with (nolock)  inner join (select b.JOB_ORDER_NO,b.BUNDLE_NO from CIPMS_USER_SCANNING_DFT as a  with (nolock)  inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' group by b.JOB_ORDER_NO,b.BUNDLE_NO)as c on d.JOB_ORDER_NO=c.JOB_ORDER_NO and d.BUNDLE_NO=c.BUNDLE_NO where len(d.CARTON_BARCODE)=15 and d.CARTON_STATUS='C'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checksecondcarton(SqlConnection sqlConn, string bundlebarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            //sqlComGet.CommandText = "select CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING where BARCODE='" + bundlebarcode + "' and LEN(CARTON_BARCODE)=15 and CARTON_STATUS='C'";
            sqlComGet.CommandText = "select 1 from (select CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING  with (nolock)  where BARCODE='" + bundlebarcode + "' and CARTON_STATUS='C')AS A WHERE LEN(A.CARTON_BARCODE)=15";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getremainpart(SqlConnection sqlConn, string joborderno, int layno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PART_CD from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where JOB_ORDER_NO='" + joborderno + "' and LAY_NO=" + layno + " group by PART_CD except select b.PART_CD from CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) inner join (select CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where JOB_ORDER_NO='" + joborderno + "' and LAY_NO=" + layno + " and CARTON_STATUS='C' group by CARTON_BARCODE)as a on a.CARTON_BARCODE=b.CARTON_BARCODE where LEN(a.CARTON_BARCODE)=13 and SUBSTRING(a.CARTON_BARCODE,10,1)='-' and b.CARTON_STATUS='C' group by b.PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getremainpart(SqlConnection sqlConn, string bundlebarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PART_CD from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where BARCODE='" + bundlebarcode + "' and CARTON_STATUS<>'C'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader opencheckbundle(SqlConnection sqlConn, string docno, string userbarcode, string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where BUNDLE_ID in(select BUNDLE_ID from CIPMS_USER_SCANNING_DFT  with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') and (PROCESS_CD<>'" + process + "' OR PROCESS_TYPE<>'I' OR CARTON_STATUS<>'C')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader matchingcheckbundle(SqlConnection sqlConn, string docno, string userbarcode, string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where BUNDLE_ID in(select BUNDLE_ID from CIPMS_USER_SCANNING_DFT  with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "') and (PROCESS_CD<>'" + process + "' OR PROCESS_TYPE<>'I')";
            return sqlComGet.ExecuteReader();
        }
        public void bundleclose(SqlConnection sqlConn, string cartonbarcode, string userbarcode, string docno)
        {
            string sql = "update CIPMS_BUNDLE_FOR_SCANNING SET CARTON_BARCODE='" + cartonbarcode + "', CARTON_STATUS='C',LAST_MODI_USER_ID='" + userbarcode + "',LAST_MODI_DATE=getdate() from CIPMS_BUNDLE_FOR_SCANNING as a,(select BUNDLE_ID from CIPMS_USER_SCANNING_DFT  with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')as b where a.BUNDLE_ID=b.BUNDLE_ID";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader cartonlist(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_USER_SCANNING_DFT as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "' and a.CARTON_STATUS='C' group by CARTON_BARCODE";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkmainhassecond(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select e.CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING as e  with (nolock) inner join (select JOB_ORDER_NO,BUNDLE_NO from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where CARTON_BARCODE in (select b.CARTON_BARCODE from CIPMS_USER_SCANNING_DFT as a  with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' and LEN(b.CARTON_BARCODE)=13 and b.CARTON_STATUS='C' group by b.CARTON_BARCODE) and CARTON_STATUS='C' group by JOB_ORDER_NO,BUNDLE_NO)as f on e.JOB_ORDER_NO=f.JOB_ORDER_NO and e.BUNDLE_NO=f.BUNDLE_NO where LEN(e.CARTON_BARCODE)=15 and e.CARTON_STATUS='C' group by e.CARTON_BARCODE";
            return sqlComGet.ExecuteReader();
        }
        public void insertcartonhistory(SqlConnection sqlConn, string factory, string process, string userbarcode, string cartonlist)
        {
            string sql = "insert into CIPMS_CARTON_HISTORY_HD (CARTON_BARCODE,OPERATION,FACTORY_CD,PROCESS_CD,MODI_USER_ID,MODI_DATE) select CARTON_BARCODE,'Open Carton','" + factory + "','" + process + "','" + userbarcode + "',GETDATE() from CIPMS_CARTON  with (nolock) where CARTON_BARCODE='" + cartonlist + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader checkhavesecondcarton(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select d.CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING as d  with (nolock) inner join (select b.JOB_ORDER_NO,b.BUNDLE_NO from CIPMS_USER_SCANNING_DFT as a  with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' group by b.JOB_ORDER_NO,b.BUNDLE_NO)as c on d.JOB_ORDER_NO=c.JOB_ORDER_NO and d.BUNDLE_NO=c.BUNDLE_NO where LEN(d.CARTON_BARCODE)=15 and d.CARTON_STATUS='C'";
            return sqlComGet.ExecuteReader();
        }
        public void opencartonbundlestatus(SqlConnection sqlConn, string userbarcode, string cartonlist)
        {
            string sql = "update CIPMS_BUNDLE_FOR_SCANNING set CARTON_STATUS='O',LAST_MODI_USER_ID='" + userbarcode + "',LAST_MODI_DATE=getdate(),IS_MATCHING=(case process_cd when 'SEW' then IS_MATCHING else 'N' end) where CARTON_BARCODE='" + cartonlist + "' and CARTON_STATUS='C'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatejobundlestatus(SqlConnection sqlConn, string cartonbarcode)
        {
            string sql = "update CIPMS_JO_WIP_BUNDLE set STATUS='C',MATCHING=(case b.process_cd when 'SEW' then MATCHING else 0 end) from CIPMS_JO_WIP_BUNDLE as a inner join (select c.STOCK_ID,c.BUNDLE_NO,c.PART_CD,a.PROCESS_CD from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_JO_WIP_HD as b on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.COLOR_CD=b.COLOR_CODE and a.SIZE_CD=b.SIZE_CODE and a.PART_CD=b.PART_CD and a.FACTORY_CD=b.FACTORY_CD and a.PROCESS_CD=b.PROCESS_CD and a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD and a.GARMENT_TYPE=b.GARMENT_TYPE and a.PROCESS_TYPE=b.PROCESS_TYPE inner join CIPMS_JO_WIP_BUNDLE as c on b.STOCK_ID=c.STOCK_ID and a.BUNDLE_NO=c.BUNDLE_NO where a.CARTON_BARCODE='" + cartonbarcode + "' and a.CARTON_STATUS='C') as b on a.STOCK_ID=b.STOCK_ID and a.BUNDLE_NO=b.BUNDLE_NO and a.PART_CD=b.PART_CD";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void opencartonstatus(SqlConnection sqlConn, string userbarcode, string cartonlist)
        {
            string sql = "update CIPMS_CARTON SET CARTON_STATUS='O',LAST_MODI_USER_ID='" + userbarcode + "', LAST_MODI_DATE=getdate() where CARTON_BARCODE='" + cartonlist+"'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatecartondt(SqlConnection sqlConn, string userbarcode, string cartonbarcode)
        {
            string sql = "update CIPMS_CARTON_DT set LAST_MODI_DATE=GETDATE(),LAST_MODI_USER='" + userbarcode + "' from CIPMS_CARTON_DT as a inner join CIPMS_BUNDLE_FOR_SCANNING as d  with (nolock) on a.JOB_ORDER_NO=d.JOB_ORDER_NO and a.BUNDLE_NO=d.BUNDLE_NO where d.CARTON_BARCODE='" + cartonbarcode + "' and d.CARTON_STATUS='C'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatecartonfirstdt(SqlConnection sqlConn, string docno, string userbarcode, int firstseq, string cartonbarcode)
        {
            string sql = "UPDATE CIPMS_CARTON_DT SET FIRST_SEQ=" + firstseq + ",SECOND_SEQ=NULL,STATUS='C',LAST_MODI_DATE=GETDATE(),LAST_MODI_USER='" + userbarcode + "' FROM CIPMS_CARTON_DT AS a  with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO inner join CIPMS_USER_SCANNING_DFT as c on b.BUNDLE_ID=c.BUNDLE_ID where c.DOC_NO='" + docno + "'  and c.USER_BARCODE='" + userbarcode + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
            sql = "insert into CIPMS_CARTON_DT (JOB_ORDER_NO,BUNDLE_NO,FIRST_SEQ,STATUS,CREATE_DATE,CREATE_USER,LAST_MODI_DATE,LAST_MODI_USER) select JOB_ORDER_NO,BUNDLE_NO," + firstseq + ",'C',GETDATE(),'" + userbarcode + "' ,GETDATE(),'" + userbarcode + "' from (select a.JOB_ORDER_NO,a.BUNDLE_NO from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_USER_SCANNING_DFT as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "'  and b.USER_BARCODE='" + userbarcode + "' group by a.JOB_ORDER_NO,a.BUNDLE_NO except select JOB_ORDER_NO,BUNDLE_NO from CIPMS_CARTON_DT)as d";
            cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatecartondt2(SqlConnection sqlConn, string userbarcode, string cartonbarcode)
        {
            string sql = "update CIPMS_CARTON_DT set LAST_MODI_DATE=GETDATE(),LAST_MODI_USER='" + userbarcode + "',STATUS='O' from CIPMS_CARTON_DT as a  with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as d  with (nolock) on a.JOB_ORDER_NO=d.JOB_ORDER_NO and a.BUNDLE_NO=d.BUNDLE_NO where d.CARTON_BARCODE='" + cartonbarcode + "' and d.CARTON_STATUS='C'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void deletesecond(SqlConnection sqlConn, string go, int firstseq)
        {
            string sql = "delete from CIPMS_CARTON_SECOND_SEQ where GARMENT_ORDER_NO='" + go + "' and FIRST_SEQ=" + firstseq;
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatecartonfirst1(SqlConnection sqlConn, string go, int firstseq, string userbarcode)
        {
            string sql = "update CIPMS_CARTON_FIRST_SEQ set FIRST_SEQ=" + firstseq + ",LAST_MODI_USER='" + userbarcode + "',LAST_MODI_DATE=GETDATE() where GARMENT_ORDER_NO='" + go + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatecartonfirst1(SqlConnection sqlConn, string go, string userbarcode)
        {
            string sql = "update CIPMS_CARTON_FIRST_SEQ set FIRST_SEQ=1,LAST_MODI_USER='" + userbarcode + "',LAST_MODI_DATE=GETDATE() where GARMENT_ORDER_NO='" + go + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatecartonfirst2(SqlConnection sqlConn, string go, int firstseq, string userbarcode)
        {
            string sql = "update CIPMS_CARTON_FIRST_SEQ set FIRST_SEQ=" + firstseq + ",LAST_MODI_USER='" + userbarcode + "',LAST_MODI_DATE=GETDATE() where GARMENT_ORDER_NO='" + go + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatecartonsecondseq(SqlConnection sqlConn, string go, int firstseq, int secondseq, string userbarcode)
        {
            string sql = "update CIPMS_CARTON_SECOND_SEQ set SECOND_SEQ=" + secondseq + ",LAST_MODI_USER='" + userbarcode + "',LAST_MODI_DATE=GETDATE() where GARMENT_ORDER_NO='" + go + "' and FIRST_SEQ=" + firstseq;
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void updatecartonsecondseq(SqlConnection sqlConn, string go, int firstseq, string userbarcode)
        {
            string sql = "update CIPMS_CARTON_SECOND_SEQ set SECOND_SEQ=65,LAST_MODI_USER='" + userbarcode + "',LAST_MODI_DATE=GETDATE() where GARMENT_ORDER_NO='" + go + "' and FIRST_SEQ=" + firstseq;
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader getcartonfirstseq(SqlConnection sqlConn, string go)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select FIRST_SEQ from CIPMS_CARTON_FIRST_SEQ where GARMENT_ORDER_NO='" + go + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getcartonsecondseq2(SqlConnection sqlConn, string go, int firstseq)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select CHAR(SECOND_SEQ)as SECOND_SEQ from CIPMS_CARTON_SECOND_SEQ where GARMENT_ORDER_NO='" + go + "' and FIRST_SEQ=" + firstseq;
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getcartonsecondseq(SqlConnection sqlConn, string go, int firstseq)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select CHAR(SECOND_SEQ-1)AS SECOND_SEQ from CIPMS_CARTON_SECOND_SEQ where GARMENT_ORDER_NO='"+go+"' and FIRST_SEQ=" + firstseq;
            return sqlComGet.ExecuteReader();
        }
        //UPDATE 20151112 19:14 BY JACOB
        //-----CHANGE THE TABLE AND COLUMN CHECK
        public SqlDataReader getmaxcartonseq(SqlConnection sqlConn, string go)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT MAX(CONVERT(int, SUBSTRING(a.CARTON_BARCODE,11,3)))AS MAX_CARTON_SEQ FROM (SELECT CARTON_BARCODE FROM CIPMS_CARTON  with (nolock) WHERE GARMENT_ORDER_NO='" + go + "' AND CARTON_STATUS<>'O')AS A WHERE LEN(A.CARTON_BARCODE)=13 AND SUBSTRING(A.CARTON_BARCODE,10,1)='-'";
            //sqlComGet.CommandText = "select MAX(CONVERT(int, SUBSTRING(a.CARTON_BARCODE,11,3)))AS MAX_CARTON_SEQ from (select CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING where GARMENT_ORDER_NO='" + go + "' and CARTON_STATUS='C' group by CARTON_BARCODE)as a where LEN(a.CARTON_BARCODE)=13 and SUBSTRING(a.CARTON_BARCODE,10,1)='-'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getmaxcartonsecondseq(SqlConnection sqlConn, string go)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select MAX(ASCII(SUBSTRING(a.CARTON_BARCODE,15,1)))AS MAX_CARTON_SEQ from (select CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where GARMENT_ORDER_NO='" + go + "' and CARTON_STATUS='C' group by CARTON_BARCODE)as a where LEN(a.CARTON_BARCODE)=15";
            return sqlComGet.ExecuteReader();
        }
        public void updatecartondtfirstseq(SqlConnection sqlConn, string go)
        {
            string sql = "update CIPMS_CARTON_SECOND_SEQ set SECOND_SEQ=65 where GARMENT_ORDER_NO='" + go + "' and FIRST_SEQ=2";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
            sql = "update CIPMS_CARTON_FIRST_SEQ set FIRST_SEQ=1 where GARMENT_ORDER_NO='" + go + "' and FIRST_SEQ=2";
            cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public SqlDataReader processmatch(SqlConnection sqlConn, string docno, string userbarcode, string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.JOB_ORDER_NO from CIPMS_BUNDLE_FOR_SCANNING AS a  with (nolock) INNER JOIN CIPMS_USER_SCANNING_DFT AS b  with (nolock) ON a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "' and a.PROCESS_CD <> '" + process + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader bundlepartnum(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(BUNDLE_ID)as QTY from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where BARCODE in(select BUNDLE_BARCODE from CIPMS_USER_SCANNING_DFT  with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "' group by BUNDLE_BARCODE)";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader scanqty(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(BUNDLE_ID)as QTY from CIPMS_USER_SCANNING_DFT  with (nolock) where DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader cartonbundleqty1(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(a.BUNDLE_ID)as QTY from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_USER_SCANNING_DFT as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "' and a.CARTON_STATUS='C'";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader cartonbundleqty2(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(BUNDLE_ID)as QTY from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where CARTON_BARCODE in(select distinct a.CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_USER_SCANNING_DFT as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "' and a.CARTON_STATUS='C') and CARTON_STATUS='C'";
            return sqlComGet.ExecuteReader();
        }

        public SqlDataReader getcartoninformation(SqlConnection sqlConn, string cartonbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PROCESS_CD,PRODUCTION_LINE_CD from CIPMS_BUNDLE_FOR_SCANNING where CARTON_BARCODE='" + cartonbarcode + "' AND CARTON_STATUS='C' GROUP BY PROCESS_CD,PRODUCTION_LINE_CD";
            return sqlComGet.ExecuteReader();
        }

        //matching
        public SqlDataReader matchingselect(SqlConnection sqlConn, string bundlebarcode, string process, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT PROCESS_TYPE,BUNDLE_ID,CARTON_BARCODE,a.FACTORY_CD,PROCESS_CD,PRODUCTION_LINE_CD,PROCESS_TYPE,BARCODE,a.PART_CD,b.PART_DESC,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING as a   with (nolock) inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE BARCODE='" + bundlebarcode + "' and PROCESS_CD='" + process + "' EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader matchingselectcarton(SqlConnection sqlConn, string cartonbarcode, string process, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT PROCESS_TYPE,BUNDLE_ID,CARTON_BARCODE,a.FACTORY_CD,PROCESS_CD,PRODUCTION_LINE_CD,PROCESS_TYPE,BARCODE,a.PART_CD,b.PART_DESC,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD where BUNDLE_ID IN (SELECT BUNDLE_ID FROM CIPMS_BUNDLE_FOR_SCANNING WHERE CARTON_BARCODE='" + cartonbarcode + "' and CARTON_STATUS='C' and PROCESS_CD='" + process + "' EXCEPT SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' and USER_BARCODE='" + userbarcode + "')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader matchingcheck(SqlConnection sqlConn, string docno, string userbarcode, string process)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.BUNDLE_ID from CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_USER_SCANNING_DFT as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where b.DOC_NO='" + docno + "' and b.USER_BARCODE='" + userbarcode + "' and (a.PROCESS_CD<>'" + process + "' or a.PROCESS_TYPE<>'I')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkoas1(SqlConnection sqlConn, string bundlebarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.BUNDLE_ID from CIPMS_OAS_INTERFACE as a  with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO where b.BARCODE='" + bundlebarcode + "' and a.FLAG=0";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkoas2(SqlConnection sqlConn, string bundlebarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select b.BUNDLE_ID from CIPMS_OAS_INTERFACE as a  with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock)  on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO where b.BARCODE='" + bundlebarcode + "' and a.FLAG=1";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkjobundlepcs(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT COUNT(c.QTY)AS QTY FROM (select COUNT(b.JOB_ORDER_NO)as QTY from CIPMS_USER_SCANNING_DFT as a  with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' group by b.JOB_ORDER_NO,b.BUNDLE_NO) AS c";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checkjobundleclosepcs(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(e.QTY)AS QTY from (SELECT COUNT(d.JOB_ORDER_NO)AS QTY from CIPMS_BUNDLE_FOR_SCANNING as d  with (nolock)  INNER JOIN (select b.JOB_ORDER_NO,b.BUNDLE_NO from CIPMS_USER_SCANNING_DFT as a  with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.BUNDLE_ID=b.BUNDLE_ID where a.DOC_NO='" + docno + "' and a.USER_BARCODE='" + userbarcode + "' group by b.JOB_ORDER_NO,b.BUNDLE_NO) as c on d.JOB_ORDER_NO=c.JOB_ORDER_NO and d.BUNDLE_NO=c.BUNDLE_NO and d.CARTON_STATUS='C' GROUP BY d.JOB_ORDER_NO,d.BUNDLE_NO)as e";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader getfirstseqqty(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select COUNT(c.FIRST_SEQ) as QTY FROM (select a.FIRST_SEQ from CIPMS_CARTON_DT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO inner join CIPMS_USER_SCANNING_DFT as c  with (nolock) on b.BUNDLE_ID=c.BUNDLE_ID where c.DOC_NO='" + docno + "' and c.USER_BARCODE='" + userbarcode + "' group by a.FIRST_SEQ) AS c";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader checksecondempty(SqlConnection sqlConn, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select a.SECOND_SEQ from CIPMS_CARTON_DT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO inner join CIPMS_USER_SCANNING_DFT as c  with (nolock) on b.BUNDLE_ID=c.BUNDLE_ID where c.DOC_NO='" + docno + "' and c.USER_BARCODE='" + userbarcode + "' and a.SECOND_SEQ<>NULL";
            return sqlComGet.ExecuteReader();
        }

        //床次批量选择未扫描bundle part
        public SqlDataReader queryway1(SqlConnection sqlConn, string joborderno, int layno, string part, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT PROCESS_TYPE,BUNDLE_ID,CARTON_BARCODE,PROCESS_CD,BARCODE,a.PART_CD,b.PART_DESC,JOB_ORDER_NO,BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS FROM CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_PART_MASTER as b  with (nolock) on a.PART_CD=b.PART_CD WHERE a.JOB_ORDER_NO='" + joborderno + "' AND a.LAY_NO='" + layno + "' AND a.PART_CD in" + part + " and a.BUNDLE_ID NOT IN (SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT  with (nolock) WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "')";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader queryway2(SqlConnection sqlConn, string joborderno, int layno, string part, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT PROCESS_TYPE,BUNDLE_ID,CARTON_BARCODE,PROCESS_CD,BARCODE,a.PART_CD,b.PART_DESC,a.JOB_ORDER_NO,a.BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS FROM CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_PART_MASTER as b on a.PART_CD=b.PART_CD WHERE a.JOB_ORDER_NO='" + joborderno + "' AND a.LAY_NO=" + layno + " AND a.PART_CD in " + part + " and a.BUNDLE_ID NOT IN (SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT  with (nolock) WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "' union select b.BUNDLE_ID from CIPMS_CARTON_DT as a inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO where b.JOB_ORDER_NO='" + joborderno + "' and b.LAY_NO=" + layno + " and a.STATUS='C' group by b.BUNDLE_ID) ORDER BY a.BUNDLE_NO,a.PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public SqlDataReader queryway6(SqlConnection sqlConn, string joborderno, int layno, string part, string docno, string userbarcode)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "SELECT PROCESS_TYPE,BUNDLE_ID,CARTON_BARCODE,PROCESS_CD,BARCODE,a.PART_CD,b.PART_DESC,a.JOB_ORDER_NO,a.BUNDLE_NO,LAY_NO,SIZE_CD,COLOR_CD,QTY,DEFECT,DISCREPANCY_QTY,CARTON_STATUS FROM CIPMS_BUNDLE_FOR_SCANNING as a  with (nolock) inner join CIPMS_PART_MASTER as b  with (nolock) on a.PART_CD=b.PART_CD WHERE a.JOB_ORDER_NO='" + joborderno + "' AND a.LAY_NO='" + layno + "' AND a.PART_CD in" + part + " and a.BUNDLE_ID NOT IN (SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT  with (nolock) WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "' union select a.BUNDLE_ID from (select BUNDLE_ID,CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING  with (nolock) where JOB_ORDER_NO='" + joborderno + "' and LAY_NO=" + layno + " and CARTON_STATUS='C' group by BUNDLE_ID,CARTON_BARCODE)as a where LEN(a.CARTON_BARCODE)=15) ORDER BY a.BUNDLE_NO,a.PART_CD";
            return sqlComGet.ExecuteReader();
        }
        public void unscannedinsert3(SqlConnection sqlConn, string docno, string userbarcode, string joborderno, string layno, string part, string functioncd, string colorcd)
        {
            string sql = "";
            if (colorcd.Equals("--选择--"))
                sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() FROM CIPMS_BUNDLE_FOR_SCANNING  with (nolock) WHERE BUNDLE_ID NOT IN (SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "' union select a.BUNDLE_ID from (select BUNDLE_ID,CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING where JOB_ORDER_NO='" + joborderno + "' and LAY_NO=" + layno + " and CARTON_STATUS='C')as a where LEN(a.CARTON_BARCODE)=15) AND JOB_ORDER_NO='" + joborderno + "' AND LAY_NO='" + layno + "' AND LEN(BARCODE)=14 AND PART_CD in " + part + " ORDER BY BARCODE";
            else
                sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() FROM CIPMS_BUNDLE_FOR_SCANNING  with (nolock) WHERE BUNDLE_ID NOT IN (SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "' union select a.BUNDLE_ID from (select BUNDLE_ID,CARTON_BARCODE from CIPMS_BUNDLE_FOR_SCANNING where JOB_ORDER_NO='" + joborderno + "' and LAY_NO=" + layno + " and CARTON_STATUS='C')as a where LEN(a.CARTON_BARCODE)=15) AND JOB_ORDER_NO='" + joborderno + "' AND LAY_NO='" + layno + "' AND COLOR_CD='" + colorcd + "' AND LEN(BARCODE)=14 AND PART_CD in " + part + " ORDER BY BARCODE";

            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void unscannedinsert2(SqlConnection sqlConn, string docno, string userbarcode, string joborderno, string layno, string part, string functioncd, string colorcd)
        {
            string sql = "";
            if (colorcd.Equals("--选择--"))
                sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() FROM CIPMS_BUNDLE_FOR_SCANNING  with (nolock) WHERE BUNDLE_ID NOT IN (SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "' union select b.BUNDLE_ID from CIPMS_CARTON_DT as a  with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO where b.JOB_ORDER_NO='" + joborderno + "' and b.LAY_NO=" + layno + " and a.STATUS='C' group by b.BUNDLE_ID) AND JOB_ORDER_NO='" + joborderno + "' AND LAY_NO=" + layno + " AND LEN(BARCODE)=14 AND PART_CD in " + part + " ORDER BY BARCODE";
            else
                sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() FROM CIPMS_BUNDLE_FOR_SCANNING  with (nolock) WHERE BUNDLE_ID NOT IN (SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "' union select b.BUNDLE_ID from CIPMS_CARTON_DT as a  with (nolock) inner join CIPMS_BUNDLE_FOR_SCANNING as b  with (nolock) on a.JOB_ORDER_NO=b.JOB_ORDER_NO and a.BUNDLE_NO=b.BUNDLE_NO where b.JOB_ORDER_NO='" + joborderno + "' and b.LAY_NO=" + layno + " and a.STATUS='C' group by b.BUNDLE_ID) AND JOB_ORDER_NO='" + joborderno + "' AND LAY_NO=" + layno + " AND COLOR_CD='" + colorcd + "' AND LEN(BARCODE)=14 AND PART_CD in " + part + " ORDER BY BARCODE";
            
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }
        public void unscannedinsert1(SqlConnection sqlConn, string docno, string userbarcode, string joborderno, string layno, string part, string functioncd,string colorcd)
        {
            string sql = "";
            if (colorcd.Equals("--选择--"))
                sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() FROM CIPMS_BUNDLE_FOR_SCANNING  with (nolock) WHERE BUNDLE_ID NOT IN (SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT  with (nolock) WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "') AND JOB_ORDER_NO='" + joborderno + "' AND LAY_NO=" + layno + " AND LEN(BARCODE)=14 AND PART_CD in " + part + " ORDER BY BARCODE";
            else
                sql = "insert into CIPMS_USER_SCANNING_DFT (BUNDLE_ID,DOC_NO,USER_BARCODE,BUNDLE_BARCODE,PART_CD,FUNCTION_CD,CREATE_DATE) SELECT BUNDLE_ID,'" + docno + "','" + userbarcode + "',BARCODE,PART_CD,'" + functioncd + "',GETDATE() FROM CIPMS_BUNDLE_FOR_SCANNING  with (nolock) WHERE BUNDLE_ID NOT IN (SELECT BUNDLE_ID FROM CIPMS_USER_SCANNING_DFT  with (nolock) WHERE DOC_NO='" + docno + "' AND USER_BARCODE='" + userbarcode + "') AND JOB_ORDER_NO='" + joborderno + "' AND COLOR_CD='" + colorcd + "' AND LAY_NO=" + layno + " AND LEN(BARCODE)=14 AND PART_CD in " + part + " ORDER BY BARCODE";

            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.ExecuteNonQuery();
        }

        //tangyh 2017.03.27 
        //select PRINTING_REQUEST,EMBROIDERY_REQUEST from jo_hd where JO_NO='16E19341CB01'
        // select PRINTING,EMBROIDERY from sc_hd where sc_NO='S16E19341'
        public SqlDataReader queryEmbAndPrt(SqlConnection sqlConn, string scno)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select PRINTING,EMBROIDERY from SC_HD with (nolock) where SC_NO='" + scno + "'";
            return sqlComGet.ExecuteReader();
        }

        //20170615-tangyh
        public SqlDataReader queryfactory(SqlConnection sqlConn)
        {
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlConn;
            sqlComGet.CommandText = "select  FACTORY_CD from CIPMS_V_FACTORY";
            return sqlComGet.ExecuteReader();
        }
    }
}