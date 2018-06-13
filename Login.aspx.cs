using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;
using NameSpace;

public partial class Login : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    [WebMethod]
    public static String CheckUser(string factory, string svTYPE, string password)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement commonsql = new Sqlstatement();
        SqlDataReader sqlDr = commonsql.checkuserdr(sqlCon, password);
        if (sqlDr.Read())
        {
            result = sqlDr["EMPLOYEE_NO"].ToString() + "," + sqlDr["NAME"].ToString() + "," + sqlDr["FACTORY_CD"].ToString() + "," + sqlDr["PRC_CD"].ToString() + "," + sqlDr["PRODUCTION_LINE_CD"].ToString() + "," + sqlDr["DEFAULTFUNC"].ToString() + "," + sqlDr["USER_BARCODE"].ToString();
        }
        else
            result = "false";
        sqlDr.Close();
        if (result != "false")
        {
            sqlDr = commonsql.checkaccessfunc(sqlCon, password);
            if (!sqlDr.Read())
                result = "false1";
            sqlDr.Close();
        }
        sqlCon.Close();
        return result;
    }
}
