using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Model;
using NameSpace;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Converters;
using System.Data;

/// <summary>
///CIPMSWebService 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
//若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
// [System.Web.Script.Services.ScriptService]
public class CIPMSWebService : System.Web.Services.WebService {

    public CIPMSWebService () {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod(Description = "获取STATUS为Y的data")]
    public List<CIPMS_MIS_INTERFACE> GetCUTtoPRTdata(string factory, string svTYPE, string username, string TRANSFER_TYPE)
    {
        List<CIPMS_MIS_INTERFACE> interfaceModellist = new List<CIPMS_MIS_INTERFACE>();
        CIPMS_MIS_INTERFACE interfaceModel;
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlCon;
        //获取状态为P的数据
        //sqlComGet.CommandText = "SELECT ID,CUSTOMER,FACTORY_CODE,GARMENT_ORDER_NO,JOB_ORDER_NO,LAY_NO,BUNDLE_NO,CARTON_BARCODE,BUNDLE_BARCODE,PART_CODE,COLOR_CODE,SIZE_CODE,OUTPUT_QTY,REDUCE_QTY,TYPE,TRANSFER_DESC,STATUS FROM CIPMS_MIS_INTERFACE WHERE FACTORY_CODE='" + factory + "' AND STATUS='Y' AND TYPE IN " + TRANSFER_TYPE;
        //change by lijer on 20160713
        sqlComGet.CommandText =string.Format(@"SELECT  A.ID ,
                                    A.CUSTOMER ,
                                    A.FACTORY_CODE ,
                                    A.GARMENT_ORDER_NO ,
                                    A.JOB_ORDER_NO ,
                                    A.LAY_NO ,
                                    A.BUNDLE_NO ,
                                    A.CARTON_BARCODE ,
                                    A.BUNDLE_BARCODE ,
		                            B.PART_DESC AS PART_CODE,
                                    A.COLOR_CODE ,
                                    A.SIZE_CODE ,
                                    A.OUTPUT_QTY ,
                                    A.REDUCE_QTY ,
                                    A.[TYPE] ,
                                    A.TRANSFER_DESC ,
                                    A.[STATUS],
		                            A.SEND_ON,
                                   A.FACTORY_CODE+ ISNULL(A.DOC_NO,'') AS DOC_NO,
                                   ISNULL(A.PRINT_PART,'') AS PRINT_PART ,
                                   ISNULL(A.CAR_NO,'') AS CAR_NO
                            FROM    CIPMS_MIS_INTERFACE A WITH(NOLOCK) INNER JOIN  dbo.CIPMS_PART_MASTER B  WITH(NOLOCK)  ON A.PART_CODE=B.PART_CD
                            WHERE   A.FACTORY_CODE = '{0} '
                                    AND A.[STATUS] = 'Y'
                                    AND A.DOC_NO IS NOT NULL
                                    AND A.[TYPE] IN {1}   ORDER BY  SEND_ON ", factory, TRANSFER_TYPE);
        SqlDataReader sqlDr = sqlComGet.ExecuteReader();
        while (sqlDr.Read())
        {
            interfaceModel = new CIPMS_MIS_INTERFACE();
            interfaceModel.ID = decimal.Parse(sqlDr["ID"].ToString());
            interfaceModel.CUSTOMER = sqlDr["CUSTOMER"].ToString();
            interfaceModel.FACTORY_CODE = sqlDr["FACTORY_CODE"].ToString();
            interfaceModel.GARMENT_ORDER_NO = sqlDr["GARMENT_ORDER_NO"].ToString();
            interfaceModel.JOB_ORDER_NO = sqlDr["JOB_ORDER_NO"].ToString();
            interfaceModel.LAY_NO = decimal.Parse(sqlDr["LAY_NO"].ToString());
            interfaceModel.BUNDLE_NO = decimal.Parse(sqlDr["BUNDLE_NO"].ToString());
            interfaceModel.CARTON_BARCODE = sqlDr["CARTON_BARCODE"].ToString();
            interfaceModel.BUNDLE_BARCODE = sqlDr["BUNDLE_BARCODE"].ToString();
            interfaceModel.PART_CODE = sqlDr["PART_CODE"].ToString();
            interfaceModel.COLOR_CODE = sqlDr["COLOR_CODE"].ToString();
            interfaceModel.SIZE_CODE = sqlDr["SIZE_CODE"].ToString();
            interfaceModel.OUTPUT_QTY = decimal.Parse(sqlDr["OUTPUT_QTY"].ToString());
            interfaceModel.REDUCE_QTY = decimal.Parse(sqlDr["REDUCE_QTY"].ToString());
            interfaceModel.TYPE = sqlDr["TYPE"].ToString();
            interfaceModel.TRANSFER_DESC = sqlDr["TRANSFER_DESC"].ToString();
            interfaceModel.STATUS = sqlDr["STATUS"].ToString();
            interfaceModel.SEND_ON = Convert.ToDateTime(sqlDr["SEND_ON"].ToString());//added by lijer on 20160713
            interfaceModel.DOC_NO= sqlDr["DOC_NO"].ToString();//added on 2018-02-28
            interfaceModel.PRINT_PART = sqlDr["PRINT_PART"].ToString();//added on 2018-02-28
            interfaceModel.CAR_NO = sqlDr["CAR_NO"].ToString();//added on 2018-02-28
            interfaceModellist.Add(interfaceModel);
        }
        sqlDr.Close();

        //更新操作数据的接收人以及接收时间
        string idlist = "";
        int i = 0;
        foreach (CIPMS_MIS_INTERFACE interfacetemp in interfaceModellist)
        {
            if (i == 0)
                idlist += interfacetemp.ID;
            else
                idlist += "," + interfacetemp.ID;
            i++;
        }
        if (idlist == "")
            idlist = "(NULL)";
        else
            idlist = "(" + idlist + ")";
        string sql = "UPDATE CIPMS_MIS_INTERFACE SET RECEIVE_BY='" + username + "',RECEIVE_ON=GETDATE() WHERE STATUS='Y' AND ID IN " + idlist;
        SqlCommand cmd = new SqlCommand(sql, sqlCon);
        cmd.ExecuteNonQuery();
        sqlCon.Close();

        return interfaceModellist;
    }

    [WebMethod(Description = "更新相应数据的STATUS为N")]
    public void UpdateCutToPrtDataFlag(string factory, string svTYPE, string idlist, string username)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlCon;
        string sql = "UPDATE CIPMS_MIS_INTERFACE SET STATUS='N' WHERE STATUS='Y' AND ID IN " + idlist;
        SqlCommand cmd = new SqlCommand(sql, sqlCon);
        cmd.ExecuteNonQuery();
        sqlCon.Close();
    }

    [WebMethod(Description = "获取印花数据")]
    //public string POSTPRTTOCIPMS(List<CIPMS_PRT_TO_CIPMS> PRTDATA)
    public string POST_PRT_TO_CIPMS(string svTYPE,string PRTDATA)
    {
        string factory;
        string BATCH_NO;

        BATCH_NO = DateTime.Now.ToString();
        svTYPE = "TEST";
        PRTDATA = "";
        PRTDATA += "[";
        PRTDATA += "{'BARCODE':'GEG17060900025','TO_PROCESS':'DC','PART_LIST':'前幅/后幅','REMAIN_PART_LIST':'介英','DISCREPANCY_QTY':'1/0'},";
        PRTDATA += "{'BARCODE':'GEG17060900026','TO_PROCESS':'DC','PART_LIST':'前幅/后幅','REMAIN_PART_LIST':'介英','DISCREPANCY_QTY':'2/1'},";
        PRTDATA += "{'BARCODE':'GEG17060900027','TO_PROCESS':'DC','PART_LIST':'前幅/后幅','REMAIN_PART_LIST':'介英','DISCREPANCY_QTY':''}";
        PRTDATA += "]";
        List<CIPMS_PRT_TO_CIPMS> prt_data_list = JsonConvert.DeserializeObject<List<CIPMS_PRT_TO_CIPMS>>(PRTDATA);
        if (prt_data_list.Count <1)
        {
            return "No Data!";
        }
        factory=prt_data_list[0].BARCODE.Substring(0,3);

         string SQL="";
        //SQL += " IF OBJECT_ID('tempdb..#TEMP') IS NOT NULL DROP TABLE #TEMP;";
         //SQL += " select BARCODE,TO_PROCESS,PART_LIST,REMAIN_PART_LIST,DISCREPANCY_QTY,BATCH_NO,FACTORY_CD,GO_NO,JO_NO,CREATE_DATE,STATUS,REMARK into #TEMP from CIPMS_PRT_TO_CIPMS where 1=2;";
        foreach (CIPMS_PRT_TO_CIPMS prt_data in prt_data_list)
        {
            SQL += " insert into #TEMP (BARCODE,TO_PROCESS,PART_LIST,REMAIN_PART_LIST,DISCREPANCY_QTY,BATCH_NO,FACTORY_CD,GO_NO,JO_NO,CREATE_DATE,STATUS,REMARK) values (";       
            SQL += "'" + prt_data.BARCODE + "',";
            SQL += "'" + prt_data.TO_PROCESS + "',";
            SQL += "'" + prt_data.PART_LIST + "',";
            SQL += "'" + prt_data.REMAIN_PART_LIST + "',";
            SQL += "'" + prt_data.DISCREPANCY_QTY + "',";
            SQL += "'" + BATCH_NO + "',";
            SQL += "'" + prt_data.BARCODE.Substring(0, 3) + "',";
            SQL += "null,null,getdate(),'N',null);";
        }
        string returnstr = GET_REMARK(factory, svTYPE,SQL);
        return returnstr;
    }

    public static String GET_REMARK(string factory, string svTYPE,string SQL)
    {
        string result;
        try
        {
            SqlConnection sqlCon = new SqlConnection();
            Connect connectstring = new Connect();
            sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
            sqlCon.Open();
            SqlCommand sqlComGet = new SqlCommand();
            sqlComGet.Connection = sqlCon;
            SqlCommand cmd = new SqlCommand("CIPMS_PROCESS_PRT_INTERFACE_DATA", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300;
            cmd.Parameters.Add("@FACTORY_CD", SqlDbType.NVarChar);
            cmd.Parameters.Add("@SQL", SqlDbType.NVarChar);
            cmd.Parameters.Add("@ReturnStr", SqlDbType.NVarChar,300);
            cmd.Parameters["@ReturnStr"].Direction = ParameterDirection.Output;
            cmd.Parameters["@FACTORY_CD"].Value = factory;
            cmd.Parameters["@SQL"].Value = SQL;
            cmd.ExecuteNonQuery();
            result = cmd.Parameters["@ReturnStr"].Value.ToString();
        }
        catch (Exception ex)
        {
            result = " *****处理出错!请联系IT!";
        }
        return result;
    }
}
