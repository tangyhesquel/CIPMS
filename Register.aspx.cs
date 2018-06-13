using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Data;
using NameSpace;
using Code;
using MSCL;

public partial class Register : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    [WebMethod]
    public static String Permission(string factory, string svTYPE, string barcode)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement permission = new Sqlstatement();
        SqlDataReader sqlDr = permission.permissiondr(sqlCon, barcode);
        if (sqlDr.Read())
            result = sqlDr["FACTORY_CD"].ToString();
        else
            result = "false";
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Factory(string factory, string svTYPE, string barcode)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connectstring = new Connect();
        sqlCon.ConnectionString = connectstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr = sqlstatement.factorydr(sqlCon);
        while (sqlDr.Read())
        {
            if (sqlDr["PROCESS_TYPE"].ToString() == "I")
                result += "<option selected='selected' value='" + sqlDr["FACTORY_ID"] + "'>" + sqlDr["FACTORY_ID"] + "</option>";
            else
                result += "<option value='" + sqlDr["FACTORY_ID"] + "'>" + sqlDr["FACTORY_ID"] + "</option>";
        }
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Process(string factory, string svTYPE)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr = sqlstatement.processdr(sqlCon, factory);
        while (sqlDr.Read())
        {
            result += "<option value='" + sqlDr["PRC_CD"] + "'>" + sqlDr["NM"] + "</option>";
        }
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Production(string factory, string svTYPE, string process)
    {
        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr = sqlstatement.productiondr(sqlCon, factory, process);
        while (sqlDr.Read())
        {
            result += "<option value='" + sqlDr["PRODUCTION_LINE_CD"] + "'>" + sqlDr["PRODUCTION_LINE_NAME"] + "</option>";
        }
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Accessfunction(string factory, string svTYPE)
    {
        string result = "";
        int i = 0;
        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr = sqlstatement.accessfunctiondr(sqlCon);
        while (sqlDr.Read())
        {
            if (i == 0)
                result += "{\"ID\":\"" + sqlDr["ID"] + "\",\"FUNCTION_ENG\":\"" + sqlDr["FUNCTION_ENG"] + "\",\"MODULE\":\"" + sqlDr["MODULE_CD"] + "\"}";
            else result += ",{\"ID\":\"" + sqlDr["ID"] + "\",\"FUNCTION_ENG\":\"" + sqlDr["FUNCTION_ENG"] + "\",\"MODULE\":\"" + sqlDr["MODULE_CD"] + "\"}";
            i++;
        }
        result = "[" + result + "]";
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String LoadUser(string factory, string svTYPE, string userbarcode)
    {
        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        SqlDataReader sqlDr;
        Registersql register = new Registersql();
        string funcid = "";
        sqlDr = register.getfuncid(sqlCon, userbarcode);
        int i = 0;
        while (sqlDr.Read())
        {
            if (i == 0)
            {
                funcid += sqlDr["FUNC_ID"].ToString();
            }
            else
            {
                funcid += "/" + sqlDr["FUNC_ID"].ToString();
            }
            i++;
        }
        sqlDr.Close();

        if (i == 0)
        {
            sqlDr.Close();
            sqlCon.Close();
            return "nothisuser";
        }

        string name = "";
        string employeeno = "";
        string factorycd = "";
        string prc = "";
        string production = "";
        string shift = "";
        string defaultfunc = "";
        string module = "";
        sqlDr = register.getuserinfo(sqlCon, userbarcode);
        i = 0;
        while (sqlDr.Read())
        {
            name = sqlDr["NAME"].ToString();
            employeeno = sqlDr["NAME"].ToString();
            factorycd = sqlDr["FACTORY_CD"].ToString();
            prc = sqlDr["PRC_CD"].ToString();
            production = sqlDr["PRODUCTION_LINE_CD"].ToString();
            shift = sqlDr["SHIFT"].ToString();
            //defaultfunc = sqlDr["MODULE_CD"].ToString();
            if (i == 0)
            {
                module += sqlDr["MODULE_CD"].ToString();
            }
            else
            {
                module += "/" + sqlDr["MODULE_CD"].ToString();
            }
            i++;
        }
        sqlDr.Close();

        sqlDr = register.getdefaultfunc(sqlCon, userbarcode);
        if (sqlDr.Read())
        {
            defaultfunc = sqlDr["MODULE_CD"].ToString();
        }
        sqlDr.Close();

        //获取process获取production
        string processlist = "";
        string productionlist = "";
        Sqlstatement sqlstatement = new Sqlstatement();
        sqlDr = sqlstatement.processdr(sqlCon, factory);
        while (sqlDr.Read())
        {
            if (sqlDr["PRC_CD"].ToString() == prc)
                processlist += "<option selected='selected' value='" + sqlDr["PRC_CD"] + "'>" + sqlDr["NM"] + "</option>";
            else
                processlist += "<option value='" + sqlDr["PRC_CD"] + "'>" + sqlDr["NM"] + "</option>";
        }
        sqlDr.Close();

        sqlDr = sqlstatement.productiondr(sqlCon, factory, prc);
        while (sqlDr.Read())
        {
            if (sqlDr["PRODUCTION_LINE_CD"].ToString() == production)
                productionlist += "<option selected='selected' value='" + sqlDr["PRODUCTION_LINE_CD"] + "'>" + sqlDr["PRODUCTION_LINE_NAME"] + "</option>";
            else
                productionlist += "<option value='" + sqlDr["PRODUCTION_LINE_CD"] + "'>" + sqlDr["PRODUCTION_LINE_NAME"] + "</option>";
        }
        sqlDr.Close();
        sqlCon.Close();

        string result = "[{ \"NAME\":\""+name+"\", \"EMPLOYEENO\":\""+employeeno+"\", \"FACTORY\":\""+factorycd+"\", \"PROCESS\":\""+prc+"\", \"PROCESSLIST\":\""+processlist+"\", \"PRODUCTION\":\""+production+"\", \"PRODUCTIONLIST\":\""+productionlist+"\", \"SHIFT\":\""+shift+"\", \"FUNCID\":\""+funcid+"\", \"MODULE\":\""+module+"\", \"DEFAULTFUNC\":\""+defaultfunc+"\" }]";
        return result;
    }
    
    [WebMethod]
    public static String Reprint(string factory, string svTYPE, string employeeno, string processlabel, string usernamelabel, string employeenolabel)
    {

        string result = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        Barcode39 Code = new Barcode39();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        sqlCon.Open();
        Sqlstatement sqlstatement = new Sqlstatement();
        SqlDataReader sqlDr = sqlstatement.reprintdr(sqlCon, employeeno);
        string userbarcode = "";
        string username = "";
        employeeno = "";
        string process = "";

        if (sqlDr.Read())
        {
            userbarcode = sqlDr["USER_BARCODE"].ToString();
            username = sqlDr["NAME"].ToString();
            employeeno = sqlDr["EMPLOYEE_NO"].ToString();
            process = sqlDr["PRC_CD"].ToString();
            //string html = "<div id='barcodediv'></div><div id='processdiv'>" + processlabel + ":" + process + "</div><div id='usernamediv'>" + usernamelabel + ":" + username + "</div><div id='employeediv'>" + employeenolabel + ":" + employeeno + "</div>";
            string html = "<div id='barcodediv'></div><div style='font-size:12px' class='ui-grid-a'><div class='ui-block-a'><div class='ui-grid-a'><div class='ui-block-a' id='processdiv'>" + processlabel + ":" + process + "</div><div class='ui-block-b' id='usernamediv'>" + usernamelabel + ":" + username + "</div></div></div><div class='ui-block-b'><div class='ui-block-c' id='employeediv'>" + employeenolabel + ":" + employeeno + "</div></div></div>";
            result = "[{ \"BARCODE\": \"" + userbarcode + "\", \"HTML\": \""+html+"\" }]";
        }
        else
            result = "false1";
        sqlDr.Close();
        sqlCon.Close();
        return result;
    }

    [WebMethod]
    public static String Registerprint(string username, string employeeno, string factory, string svTYPE, string process, string production, string shift, string selectfunction, string defaultfunction, string processlabel, string usernamelabel, string employeenolabel, string select)
    {
        string[] functions = selectfunction.Split(',');
        if (production == select)
            production = "";
        if (shift == select)
            shift = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        SqlCommand cmd = new SqlCommand();
        sqlCon.Open();
        factory = factory.Substring(0, 3);
        Sqlstatement commonsql = new Sqlstatement();
        Registersql register = new Registersql();
        SqlDataReader sqlDr;
        string result = "";

        try
        {
            //判断工号是否已经存在
            sqlDr = register.isemployeenoexist(sqlCon, employeeno);
            if (sqlDr.Read())
            {
                sqlDr.Close();
                sqlCon.Close();
                return "false";
            }
            sqlDr.Close();

            string newbarcode = "";
            newbarcode = employeeno;

            string html = "<div id='barcodediv'></div><div style='font-size:12px' class='ui-grid-a'><div class='ui-block-a'><div class='ui-grid-a'><div class='ui-block-a' id='processdiv'>" + processlabel + ":" + process + "</div><div class='ui-block-b' id='usernamediv'>" + usernamelabel + ":" + username + "</div></div></div><div class='ui-block-b'><div class='ui-block-c' id='employeediv'>" + employeenolabel + ":" + employeeno + "</div></div></div>";
            result = "[{ \"BARCODE\": \"" + newbarcode + "\", \"HTML\": \"" + html + "\" }]";

            register.userinsert(sqlCon, cmd, newbarcode, employeeno, username, factory, process, production, shift, defaultfunction);

            string temp1 = "";
            for (int i = 0; i < functions.Length; i++)
            {
                if (i == 0)
                    temp1 += "('" + newbarcode + "','" + functions[i] + "')";
                else
                    temp1 += ",('" + newbarcode + "','" + functions[i] + "')";
            }
            register.userfuncinsert(sqlCon, cmd, temp1);
        }
        catch (Exception ex)
        {
            return "error1";
        }
        finally
        {
            sqlCon.Close();
        }
        return result;
    }

    [WebMethod]
    public static String Modify(string userbarcode, string username, string employeeno, string factory, string svTYPE, string process, string production, string shift, string selectfunction, string defaultfunction, string processlabel, string usernamelabel, string employeenolabel, string select)
    {
        string[] functions = selectfunction.Split(',');
        if (production == select)
            production = "";
        if (shift == select)
            shift = "";
        SqlConnection sqlCon = new SqlConnection();
        Connect connectionstring = new Connect();
        sqlCon.ConnectionString = connectionstring.Connectstring(factory, svTYPE);
        SqlCommand cmd = new SqlCommand();
        sqlCon.Open();

        factory = factory.Substring(0, 3);
        Sqlstatement commonsql = new Sqlstatement();
        Registersql register = new Registersql();
        string result = "";

        try
        {
            //先删除用户信息
            register.deleteuser(sqlCon, cmd, userbarcode);

            string newbarcode = "";
            newbarcode = userbarcode;

            string html = "<div id='barcodediv'></div><div style='font-size:12px' class='ui-grid-a'><div class='ui-block-a'><div class='ui-grid-a'><div class='ui-block-a' id='processdiv'>" + processlabel + ":" + process + "</div><div class='ui-block-b' id='usernamediv'>" + usernamelabel + ":" + username + "</div></div></div><div class='ui-block-b'><div class='ui-block-c' id='employeediv'>" + employeenolabel + ":" + employeeno + "</div></div></div>";
            result = "[{ \"BARCODE\": \"" + newbarcode + "\", \"HTML\": \"" + html + "\" }]";

            register.userinsert(sqlCon, cmd, newbarcode, employeeno, username, factory, process, production, shift, defaultfunction);

            string temp1 = "";
            for (int i = 0; i < functions.Length; i++)
            {
                if (i == 0)
                    temp1 += "('" + newbarcode + "','" + functions[i] + "')";
                else
                    temp1 += ",('" + newbarcode + "','" + functions[i] + "')";
            }
            register.userfuncinsert(sqlCon, cmd, temp1);
        }
        catch (Exception ex)
        {
            return "error1";
        }
        finally
        {
            sqlCon.Close();
            sqlCon.Dispose();
        }
        return result;
    }
}
