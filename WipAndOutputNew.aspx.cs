using Model;
using NameSpace;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class WipAndOutputNew : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod]
    public static String Getoutput(string factory, string svTYPE, string jo, string go, string fromdate, string todate)
    {
        string bydate = "false";
        if (fromdate != "" && todate != "")
            bydate = "true";
        WipAndOutputSql outputreportsql = new WipAndOutputSql(factory.Trim()+svTYPE.Trim());

        if (jo == "" && go == "" && bydate == "false")
            {
                return "nodata";
            }

        string JsonResponse;
        try
        {
            List<WipAndOutputData> WipAndOutputdata = outputreportsql.WipAndOutputList(factory,svTYPE, go, jo, fromdate, todate);
            JsonResponse = "[{\"SUCCESS\":true, \"Data\": " + JsonConvert.SerializeObject(WipAndOutputdata);
            JsonResponse += "}]";
        }
        catch (Exception ex)
        {
            JsonResponse = "[{\"SUCCESS\":false, \"Data\": " + JsonConvert.SerializeObject(null);
            JsonResponse += "}]";
            //Jsonresponse = "[{ \"SUCCESS\": false }]";
        }
        finally { }

        return JsonResponse;
    }
}

