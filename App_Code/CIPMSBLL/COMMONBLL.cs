using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Model;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Newtonsoft.Json.Converters;
using System.IO;

/// <summary>
///COMMONBLL 的摘要说明
/// </summary>
public class COMMONBLL
{
    public COMMONDAL commondal;

    public COMMONBLL(string factory, string svTYPE)
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
        commondal = new COMMONDAL(factory, svTYPE);
	}

    public COMMONBLL()
    {
    }

    //获取工序列表
    public string GetProcessList(string factory, string garmenttype)
    {
        SqlDataReader sqlDr = commondal.GetProcessList(factory, garmenttype);
        List<PROCESS> processlist = new List<PROCESS>();
        PROCESS lastprocess = new PROCESS();
        while (sqlDr.Read())
        {
            PROCESS process = new PROCESS();
            process.PRC_CD = sqlDr["PRC_CD"].ToString();
            if (process.PRC_CD == "SEW")
            {
                lastprocess.PRC_CD = sqlDr["PRC_CD"].ToString();
                lastprocess.NM = sqlDr["NM"].ToString();
                lastprocess.CIPMS_CHS = sqlDr["CIPMS_CHS"].ToString();
                continue;
            }
            process.NM = sqlDr["NM"].ToString();
            process.CIPMS_CHS = sqlDr["CIPMS_CHS"].ToString();
            if (process.CIPMS_CHS.Length >= 3)
            {
                process.CIPMS_CHS = process.CIPMS_CHS.Substring(1);
            }
            processlist.Add(process);
        }
        sqlDr.Close();

        if (lastprocess != null)
            processlist.Add(lastprocess);

        string JsonResponse = "";

        JsonResponse = "[{\"SUCCESS\":true, \"data\":";
        JsonResponse += JsonConvert.SerializeObject(processlist);
        JsonResponse += "}]";
        return JsonResponse;
    }

    //获取用户信息
    public string GetUserProfile(string userbarcode)
    {
        DataSet ds = commondal.GetUserProfile(userbarcode);
        List<USERPROFILE> userprofile = new List<USERPROFILE>();
        if (ds.Tables.Count > 0)
        {
            foreach (DataTable dt in ds.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    USERPROFILE userprofiletemp = new USERPROFILE();
                    try
                    {
                        userprofiletemp.USER_BARCODE = dr[0].ToString();
                        userprofiletemp.EMPLOYEE_NO = dr[1].ToString();
                        userprofiletemp.NAME = dr[2].ToString();
                        userprofiletemp.FACTORY_CD = dr[3].ToString();
                        userprofiletemp.PRC_CD = dr[4].ToString();
                        userprofiletemp.PRODUCTION_LINE_CD = dr[5].ToString();
                        userprofiletemp.SHIFT = dr[6].ToString();
                        userprofiletemp.DEFAULTMODULE = dr[7].ToString();
                        userprofiletemp.FUNCTION_ENG = dr[8].ToString();
                        userprofiletemp.FUNCTION_CHS = dr[9].ToString();
                        userprofiletemp.MODULE_CD = dr[10].ToString();
                        userprofile.Add(userprofiletemp);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        string JsonResponse = "";

        if (userprofile == null)
        {
            JsonResponse = "[{\"SUCCESS\":false}]";
        }
        else
        {
            JsonResponse = "[{\"SUCCESS\":true, \"data\":";
            JsonResponse += JsonConvert.SerializeObject(userprofile);
            JsonResponse += "}]";
        }
        return JsonResponse;
    }

    //判断条码类型
    public string GetBarcodeType(string barcode)
    {
        if (barcode != null && barcode != "")
        {
            var index = barcode.IndexOf("-");
            if (index > -1)//存在符号“-”，说明是箱码或者流水单号
            {
                //获取字符串第四个到第九个字符
                var keyword = barcode.Substring(3, 6);
                //正则表达式判断字符串是否包含字母
                if (!isNumberic(keyword))
                {
                    //说明字符串中存在英文字母，则说明是箱码
                    return "C";
                }
                else
                {
                    //不存在则说明是流水单号
                    return "D";
                }
            }
            else//不存在，说明是扎码B
            {//获取字符串第四个到第九个字符
                var keyword = barcode.Substring(3, 6);
                //正则表达式判断字符串是否包含字母
                if (!isNumberic(keyword))
                {
                    //说明字符串中存在英文字母，则说明是箱码
                    return "C";
                }
                else
                {
                    return "B";
                }
            }
        }
        else
            return "";
    }

    public bool isNumberic(string keyword)
    {
        //判断是否为整数字符串
        //是的话则将其转换为数字并将其设为out类型的输出值、返回true, 否则为false
        int result = -1;   //result 定义为out 用来输出值
        try
        {
            //当数字字符串的为是少于4时，以下三种都可以转换，任选一种
            //如果位数超过4的话，请选用Convert.ToInt32() 和int.Parse()
            result = Convert.ToInt32(keyword);
            return true;
        }
        catch
        {
            return false;
        }
    }

    //打印扎码或者箱码或者流水单
    public string PrintBarcode(string BARCODE, string LANGUAGE)
    {
        string JsonResponse = "";

        string barcodetype = GetBarcodeType(BARCODE);

        if (barcodetype == "B")
        {
            JsonResponse = "[{\"SUCCESS\":false, \"data\":\"暂不支持打菲功能\"}]";
        }
        else if (barcodetype == "C")
        {
            string filePath = "";
            if (LANGUAGE == "2")
                filePath = System.Web.HttpContext.Current.Server.MapPath("~/Language/en_US/CartonBarcode_Print.js");
            else
                filePath = System.Web.HttpContext.Current.Server.MapPath("~/Language/zh_CN/CartonBarcode_Print.js");

            CartonBarcode_Print cartonbarcodecolumn = GetCartonTransactionFromJson(filePath);

            JsonResponse = "[{\"SUCCESS\":true, \"data\":\"" + commondal.PrintCartonBarcode(BARCODE, cartonbarcodecolumn) + "\"}]";
        }
        else if (barcodetype == "D")
        {
            string filePath = "";
            if (LANGUAGE == "2")
                filePath = System.Web.HttpContext.Current.Server.MapPath("~/Language/en_US/Docno_Print.js");
            else
                filePath = System.Web.HttpContext.Current.Server.MapPath("~/Language/zh_CN/Docno_Print.js");

            Docno_Print docnocolumn = GetDocnoTransactionFromJson(filePath);

            JsonResponse = "[{\"SUCCESS\":true, \"data\":\"" + commondal.PrintDocno(BARCODE.Substring(3), docnocolumn) + "\"}]";
        }
        else
        {
        }

        return JsonResponse;
    }


    public CartonBarcode_Print GetCartonTransactionFromJson(string con_file_path)
    {
        using (StreamReader sr = new StreamReader(con_file_path))
        {
            try
            {
                CartonBarcode_Print cartonbarcodeprinttransaction = new CartonBarcode_Print();

                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new JavaScriptDateTimeConverter());
                serializer.NullValueHandling = NullValueHandling.Ignore;

                //构建Json.net的读取流  
                JsonReader reader = new JsonTextReader(sr);
                //对读取出的Json.net的reader流进行反序列化，并装载到模型中  
                cartonbarcodeprinttransaction = serializer.Deserialize<CartonBarcode_Print>(reader);
                return cartonbarcodeprinttransaction;
            }
            catch
            {
                return null;
            }
        }
    }


    public Docno_Print GetDocnoTransactionFromJson(string con_file_path)
    {
        using (StreamReader sr = new StreamReader(con_file_path))
        {
            try
            {
                Docno_Print docnoprinttransaction = new Docno_Print();

                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new JavaScriptDateTimeConverter());
                serializer.NullValueHandling = NullValueHandling.Ignore;

                //构建Json.net的读取流  
                JsonReader reader = new JsonTextReader(sr);
                //对读取出的Json.net的reader流进行反序列化，并装载到模型中  
                docnoprinttransaction = serializer.Deserialize<Docno_Print>(reader);
                return docnoprinttransaction;
            }
            catch
            {
                return null;
            }
        }
    } 
}