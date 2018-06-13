using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;
using NameSpace;
using System.Data;

/// <summary>
///CheckBundleStatus 的摘要说明
/// </summary>
public class CheckBundleStatus
{
    private string isneedmatching = "N";
    private string isneedcarton = "N";
    private string isneedfullbundle = "N";
    private string isneedfullcarton = "N";
    private string isneedbundleprocess = "N";
    private string isneedcartonprocess = "N";
    private string isneedsubmit = "N";
    private string isneedonereceive = "N";
    private string isneedonesend = "N";
    private string isneedonecutgarmenttype = "N";
    private string isneedonesewgarmenttype = "N";

    public string ismatching = "true";
    public string iscarton = "true";
    public string isfullbundle = "true";
    public string isfullcarton = "true";
    public string isbundleprocess = "true";
    public string iscartonprocess = "true";
    public string issubmit = "true";
    public string isonereceive = "true";
    public string isonesend = "true";
    public string isonecutgarmenttype = "true";
    public string isonesewgarmenttype = "true";
    public string receivefactory;
    public string receiveprocess;
    public string receiveproduction;
    public string sendfactory;
    public string sendprocess;
    public string sendproduction;
    public string cutgarmenttype;
    public string sewgarmenttype;
    public string cartonbarcode;
    public void setisneedmatching(Boolean matching)
    {
        if (matching == true)
        {
            isneedmatching = "Y";
        }
        else
        {
            isneedmatching = "N";
        }
    }
    public void setisneedcarton(Boolean carton)
    {
        if (carton == true)
        {
            isneedcarton = "Y";
        }
        else
        {
            isneedcarton = "N";
        }
    }
    public void setisneedfullbundle(Boolean fullbundle)
    {
        if (fullbundle == true)
        {
            isneedfullbundle = "Y";
        }
        else
        {
            isneedfullbundle = "N";
        }
    }
    public void setisneedfullcarton(Boolean fullcarton)
    {
        if (fullcarton == true)
        {
            isneedfullcarton = "Y";
        }
        else
        {
            isneedfullcarton = "N";
        }
    }
    public void setisneedbundleprocess(Boolean bundleprocess)
    {
        if (bundleprocess == true)
        {
            isneedbundleprocess = "Y";
        }
        else
        {
            isneedbundleprocess = "N";
        }
    }
    public void setisneedcartonprocess(Boolean cartonprocess)
    {
        if (cartonprocess == true)
        {
            isneedcartonprocess = "Y";
        }
        else
        {
            isneedcartonprocess = "N";
        }
    }
    public void setisneedsubmit(Boolean submit)
    {
        if (submit == true)
        {
            isneedsubmit = "Y";
        }
        else
        {
            isneedsubmit = "N";
        }
    }
    public void setisneedonereceive(Boolean onereceive)
    {
        if (onereceive == true)
        {
            isneedonereceive = "Y";
        }
        else
        {
            isneedonereceive = "N";
        }
    }
    public void setisneedonesend(Boolean onesend)
    {
        if (onesend == true)
        {
            isneedonesend = "Y";
        }
        else
        {
            isneedonesend = "N";
        }
    }
    public void setisneedonecutgarmenttype(Boolean onecutgarmenttype)
    {
        if (onecutgarmenttype == true)
        {
            isneedonecutgarmenttype = "Y";
        }
        else
        {
            isneedonecutgarmenttype = "N";
        }
    }
    public void setisneedonesewgarmenttype(Boolean onesewgarmenttype)
    {
        if (onesewgarmenttype == true)
        {
            isneedonesewgarmenttype = "Y";
        }
        else
        {
            isneedonesewgarmenttype = "N";
        }
    }

    public Boolean checkfunc(SqlConnection sqlCon, string docno, string userbarcode, string factory, string process)
    {
        SqlCommand cmd;
        try
        {
            cmd = new SqlCommand("CIPMS_SP_CHECK_BUNDLE_STATUS", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@DOCNO", SqlDbType.VarChar);
            cmd.Parameters.Add("@USERBARCODE", SqlDbType.NVarChar);
            cmd.Parameters.Add("@CIPMSFACTORY", SqlDbType.NVarChar);
            cmd.Parameters.Add("@CIPMSPROCESS", SqlDbType.NVarChar);
            cmd.Parameters.Add("@MATCHING", SqlDbType.NChar);
            cmd.Parameters.Add("@CARTON", SqlDbType.NChar);
            cmd.Parameters.Add("@SUBMIT", SqlDbType.NChar);
            cmd.Parameters.Add("@FULLBUNDLE", SqlDbType.NChar);
            cmd.Parameters.Add("@FULLCARTON", SqlDbType.NChar);
            cmd.Parameters.Add("@BUNDLEPROCESS", SqlDbType.NChar);
            cmd.Parameters.Add("@CARTONPROCESS", SqlDbType.NChar);
            cmd.Parameters.Add("@ONESEND", SqlDbType.NChar);
            cmd.Parameters.Add("@ONERECEIVE", SqlDbType.NChar);
            cmd.Parameters.Add("@CUTGARMENTTYPE", SqlDbType.NChar);
            cmd.Parameters.Add("@SEWGARMENTTYPE", SqlDbType.NChar);
            cmd.Parameters.Add("@ISMATCHING", SqlDbType.NChar, 1);
            cmd.Parameters.Add("@ISCARTON", SqlDbType.NChar, 1);
            cmd.Parameters.Add("@ISSUBMIT", SqlDbType.NChar, 1);
            cmd.Parameters.Add("@ISFULLBUNDLE", SqlDbType.NChar, 1);
            cmd.Parameters.Add("@ISFULLCARTON", SqlDbType.NChar, 1);
            cmd.Parameters.Add("@ISBUNDLEPROCESS", SqlDbType.NChar, 1);
            cmd.Parameters.Add("@ISCARTONPROCESS", SqlDbType.NChar, 1);
            cmd.Parameters.Add("@ISONERECEIVE", SqlDbType.NChar, 1);
            cmd.Parameters.Add("@ISONESEND", SqlDbType.NChar, 1);
            cmd.Parameters.Add("@ISCUTGARMENTTYPE", SqlDbType.NChar, 1);
            cmd.Parameters.Add("@ISSEWGARMENTTYPE", SqlDbType.NChar, 1);
            cmd.Parameters.Add("@RECEIVEFACTORY", SqlDbType.NVarChar, 10);
            cmd.Parameters.Add("@RECEIVEPROCESS", SqlDbType.NVarChar, 10);
            cmd.Parameters.Add("@RECEIVEPRODUCTION", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@SENDFACTORY", SqlDbType.NVarChar, 10);
            cmd.Parameters.Add("@SENDPROCESS", SqlDbType.NVarChar, 10);
            cmd.Parameters.Add("@SENDPRODUCTION", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@CUTGARMENTOUTPUT", SqlDbType.NChar, 1);
            cmd.Parameters.Add("@SEWGARMENTOUTPUT", SqlDbType.NChar, 1);
            cmd.Parameters.Add("@CARTONBARCODE", SqlDbType.VarChar, 20);
            cmd.Parameters["@ISMATCHING"].Direction = ParameterDirection.Output;
            cmd.Parameters["@ISCARTON"].Direction = ParameterDirection.Output;
            cmd.Parameters["@ISSUBMIT"].Direction = ParameterDirection.Output;
            cmd.Parameters["@ISFULLBUNDLE"].Direction = ParameterDirection.Output;
            cmd.Parameters["@ISFULLCARTON"].Direction = ParameterDirection.Output;
            cmd.Parameters["@ISBUNDLEPROCESS"].Direction = ParameterDirection.Output;
            cmd.Parameters["@ISCARTONPROCESS"].Direction = ParameterDirection.Output;
            cmd.Parameters["@ISONERECEIVE"].Direction = ParameterDirection.Output;
            cmd.Parameters["@ISONESEND"].Direction = ParameterDirection.Output;
            cmd.Parameters["@ISCUTGARMENTTYPE"].Direction = ParameterDirection.Output;
            cmd.Parameters["@ISSEWGARMENTTYPE"].Direction = ParameterDirection.Output;
            cmd.Parameters["@RECEIVEFACTORY"].Direction = ParameterDirection.Output;
            cmd.Parameters["@RECEIVEPROCESS"].Direction = ParameterDirection.Output;
            cmd.Parameters["@RECEIVEPRODUCTION"].Direction = ParameterDirection.Output;
            cmd.Parameters["@SENDFACTORY"].Direction = ParameterDirection.Output;
            cmd.Parameters["@SENDPROCESS"].Direction = ParameterDirection.Output;
            cmd.Parameters["@SENDPRODUCTION"].Direction = ParameterDirection.Output;
            cmd.Parameters["@CUTGARMENTOUTPUT"].Direction = ParameterDirection.Output;
            cmd.Parameters["@SEWGARMENTOUTPUT"].Direction = ParameterDirection.Output;
            cmd.Parameters["@CARTONBARCODE"].Direction = ParameterDirection.Output;
            cmd.Parameters["@DOCNO"].Value = docno;
            cmd.Parameters["@USERBARCODE"].Value = userbarcode;
            cmd.Parameters["@CIPMSFACTORY"].Value = factory;
            cmd.Parameters["@CIPMSPROCESS"].Value = process;
            //判断是否已经配套
            cmd.Parameters["@MATCHING"].Value = isneedmatching;
            //判断是否已经装箱
            cmd.Parameters["@CARTON"].Value = isneedcarton;
            //扫描的裁片是否是提交状态
            cmd.Parameters["@SUBMIT"].Value = isneedsubmit;
            //如果是车缝，判断扫描的裁片是否完整
            cmd.Parameters["@FULLBUNDLE"].Value = isneedfullbundle;
            //如果是车缝，判断扫描的裁片是否完整
            cmd.Parameters["@FULLCARTON"].Value = isneedfullcarton;
            //判断扫描的裁片是否都在本部门
            cmd.Parameters["@BUNDLEPROCESS"].Value = isneedbundleprocess;
            //判断扫描的裁片是否都在本部门
            cmd.Parameters["@CARTONPROCESS"].Value = isneedcartonprocess;
            //接收部门组别是否是同一个
            cmd.Parameters["@ONERECEIVE"].Value = isneedonereceive;
            //发送部门组别是否是同一个
            cmd.Parameters["@ONESEND"].Value = isneedonesend;
            //获取MES CUT的GARMENT_TYPE
            cmd.Parameters["@CUTGARMENTTYPE"].Value = isneedonecutgarmenttype;
            //获取MES SEW的GARMENT_TYPE
            cmd.Parameters["@SEWGARMENTTYPE"].Value = isneedonesewgarmenttype;

            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            sqlCon.Close();
            return false;
        }

        if (cmd.Parameters["@ISMATCHING"].Value.ToString() != "Y")
            ismatching = "false";
        if (cmd.Parameters["@ISCARTON"].Value.ToString() == "Y")
        {
            cartonbarcode = cmd.Parameters["@CARTONBARCODE"].Value.ToString();
            iscarton = "false";
        }
        if (cmd.Parameters["@ISSUBMIT"].Value.ToString() == "Y")
            issubmit = "false";
        if (process == "SEW")
        {
            if (cmd.Parameters["@ISFULLBUNDLE"].Value.ToString() != "Y")
                isfullbundle = "false";
        }
        if (cmd.Parameters["@ISFULLCARTON"].Value.ToString() != "Y")
            isfullcarton = "false";
        if (cmd.Parameters["@ISBUNDLEPROCESS"].Value.ToString() != "Y")
            isbundleprocess = "false";
        if (cmd.Parameters["@ISCARTONPROCESS"].Value.ToString() != "Y")
            iscartonprocess = "false";
        if (cmd.Parameters["@ISONERECEIVE"].Value.ToString() == "Y")
        {
            receivefactory = cmd.Parameters["@RECEIVEFACTORY"].Value.ToString();
            receiveprocess = cmd.Parameters["@RECEIVEPROCESS"].Value.ToString();
            receiveproduction = cmd.Parameters["@RECEIVEPRODUCTION"].Value.ToString();
        }
        else
            isonereceive = "false";
        if (cmd.Parameters["@ISONESEND"].Value.ToString() == "Y")
        {
            sendfactory = cmd.Parameters["@SENDFACTORY"].Value.ToString();
            sendprocess = cmd.Parameters["@SENDPROCESS"].Value.ToString();
            sendproduction = cmd.Parameters["@SENDPRODUCTION"].Value.ToString();
        }
        else
            isonesend = "false";
        if (cmd.Parameters["@ISCUTGARMENTTYPE"].Value.ToString() == "Y")
            cutgarmenttype = cmd.Parameters["@CUTGARMENTOUTPUT"].Value.ToString();
        else
            isonecutgarmenttype = "false";
        if (cmd.Parameters["@ISSEWGARMENTTYPE"].Value.ToString() == "Y")
        {
            sewgarmenttype = cmd.Parameters["@SEWGARMENTOUTPUT"].Value.ToString();
        }
        else
            isonesewgarmenttype = "false";

        return true;
    }
}