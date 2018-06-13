using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;

/// <summary>
/// Summary description for Connect
/// </summary>
/// 
namespace NameSpace
{
    public class Connect
    {
        public Connect()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public String Connectstring(string factory, string svTYPE)
        {
            if (factory == "GEG" && svTYPE=="DEVTEST")
                return ConfigurationManager.ConnectionStrings["connTest"].ConnectionString;
            else if (factory == "YMG" && svTYPE == "TEST")
                return ConfigurationManager.ConnectionStrings["YMGTEST"].ConnectionString;
            else if (factory == "GEG" && svTYPE == "TEST")
                return ConfigurationManager.ConnectionStrings["GEGTEST"].ConnectionString;
            else if (factory == "YMG" && svTYPE == "OLDTEST")
                return ConfigurationManager.ConnectionStrings["OLDYMGTEST"].ConnectionString;
            else if (factory == "GEG" && svTYPE == "OLDTEST")
                return ConfigurationManager.ConnectionStrings["OLDGEGTEST"].ConnectionString;
            else if (factory == "YMG" && svTYPE == "PROD")
                return ConfigurationManager.ConnectionStrings["YMGPROD"].ConnectionString;
            else if (factory == "GEG" && svTYPE == "PROD")
                return ConfigurationManager.ConnectionStrings["GEGPROD"].ConnectionString;
            return null;
        }
    }
}
