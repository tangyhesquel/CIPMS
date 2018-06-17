using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

//public class ApplicationUser : IdentityUser
//{
//    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
//    {
//        // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
//        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
//        // 在此处添加自定义用户声明
//        return userIdentity;
//    }
//}

//public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
//{

//    public ApplicationDbContext()
//        : base("GEGPRD", throwIfV1Schema: false)
//    {
//    }

//    public static ApplicationDbContext Create()
//    {
//        return new ApplicationDbContext();
//    }

//    public System.Data.Entity.DbSet<WipAndOutputData> WipAndOutputData { get; set; }

//}

//public class WipAndOutputSql 
//{
    
//    static string DBStr;
//    public WipAndOutputSql(string Str)
//    {
//        DBStr = Str;
//        db = new ApplicationDbContext();
//    }
//    public ApplicationDbContext db
//    {
//        get;
//        set;
//    }

//    public SqlDataReader WipAndOutput(SqlConnection sqlConn, string fty, string jo, string go, string fromdate, string todate)
//    {
//        SqlCommand sqlComGet = new SqlCommand();
//        sqlComGet.Connection = sqlConn;
//        sqlComGet.CommandText = " exec CIPMS_OUTPUT_AND_WIP '" + fty + "','" + jo + "','" + go + "','" + fromdate + "','" + todate + "'";
//        return sqlComGet.ExecuteReader();
//    }



//    public List<WipAndOutputData> WipAndOutputList(string fty, string svTYPE, string jo, string go, string fromdate, string todate)
//    {
//        try
//        {
//            string query = " exec CIPMS_OUTPUT_AND_WIP '" + fty + "','" + jo + "','" + go + "','" + fromdate + "','" + todate + "'";
//            return db.Database.SqlQuery<WipAndOutputData>(query).ToList();
//        }
//        catch (Exception ex)
//        {
//            return null;
//        }
//        finally
//        {
//        }
//    }


//}


public class ApplicationUser : IdentityUser
{
}

public class CIPMSDbContext :IdentityDbContext<ApplicationUser>
{
    public CIPMSDbContext(string DBStr)
        : base(DBStr)
    {
        Database.SetInitializer<CIPMSDbContext>(null);
    }
    public static CIPMSDbContext Create(string DBStr)
    {
        return new CIPMSDbContext(DBStr);
    }
    //public System.Data.Entity.DbSet<WipAndOutputData> WipAndOutputData { get; set; }
}

public class WipAndOutputSql 
{
    public CIPMSDbContext db
    {
        get;
        set;
    }

    string DBStr;
    public WipAndOutputSql(string Str)
    {        
        DBStr=Str;
        db = new CIPMSDbContext(DBStr);
    }


    public SqlDataReader WipAndOutput(SqlConnection sqlConn, string fty, string jo, string go, string fromdate, string todate)
    {
        SqlCommand sqlComGet = new SqlCommand();
        sqlComGet.Connection = sqlConn;
        sqlComGet.CommandText = " exec CIPMS_OUTPUT_AND_WIP '" + fty + "','" + jo + "','" + go + "','" + fromdate + "','" + todate + "'";
        return sqlComGet.ExecuteReader();
    }

    
    public List<WipAndOutputData> WipAndOutputList(string fty, string svTYPE, string jo, string go, string fromdate, string todate)
    {
        try
        {
            string query = " exec CIPMS_OUTPUT_AND_WIP '" + fty + "','" + jo + "','" + go + "','" + fromdate + "','" + todate + "'";
            //string query = " select  CUT_LINE,PRODUCTION_LINE_CD,JOB_ORDER_NO,COLOR_CD,ORDER_QTY,CUT_QTY_TODAY,CUT_QTY_TOTAL,REDUCE_QTY_TODAY,REDUCE_QTY_TOTAL,";
            //query=query+" CUT_OUT_TODAY,CUT_OUT_TOTAL,CUT_OUT_WIP,	 PRT_IN_TODAY,PRT_IN_TOTAL,PRT_OUT_TODAY,PRT_OUT_TOTAL,PRT_IN_OUT_WIP,EMB_OUT_TODAY,";
            //query=query+" EMB_OUT_TOTAL,EMB_OUT_WIP,FUSING_OUT_TODAY,FUSING_OUT_TOTAL,FUSING_OUT_WIP,MATCHING_OUT_TODAY,MATCHING_OUT_TOTAL,MATCHING_OUT_WIP,DC_BEFORE_WIP,";
            //query=query+" DC_SEW_S_TOTAL,DC_SEW_C,DC_SEW_C_TOTAL,TWIP,DCT,ORDER_QTY,CUT_QTY,SEQ from DATAPROCESS ";
            db.Database.CommandTimeout = 3000;
            return db.Database.SqlQuery<WipAndOutputData>(query).ToList();
        }
        catch (Exception ex)
        {
            return null;
        }
        finally
        {
        }
    }


}