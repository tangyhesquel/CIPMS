using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Model;
using Newtonsoft.Json;

/// <summary>
///BARCODEINQUIRYBLL 的摘要说明
/// </summary>
public class BARCODEINQUIRYBLL
{
    public BARCODEINQUIRYDAL barcodeinquirydal;

    public BARCODEINQUIRYBLL(string factory, string svTYPE)
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
        barcodeinquirydal = new BARCODEINQUIRYDAL(factory, svTYPE);
	}

    public BARCODEINQUIRYBLL()
    {
    }

    public string GetDOCNOList(string DOC_NO, string JOB_ORDER_NO, string STATUS, string SENDPROCESS, string RECEIVEPROCESS, string CREATEDATEFROM, string CREATEDATETO)
    {
        if (CREATEDATEFROM != "" && CREATEDATEFROM != null && CREATEDATEFROM != "null")
            CREATEDATEFROM = DateTime.ParseExact(CREATEDATEFROM, "yyyy/MM/dd", System.Globalization.CultureInfo.GetCultureInfo("en-US")).ToString("yyyy-MM-dd");
        if (CREATEDATETO != "" && CREATEDATETO != null && CREATEDATETO != "null")
            CREATEDATETO = DateTime.ParseExact(CREATEDATETO, "yyyy/MM/dd", System.Globalization.CultureInfo.GetCultureInfo("en-US")).ToString("yyyy-MM-dd");

        if (DOC_NO != "" && DOC_NO != null && DOC_NO != "null")
        {
            int i = 1;
            int letterindex = 0;
            foreach (char a in DOC_NO)
            {
                if (Char.IsLetter(a))
                    letterindex = i;
                i++;
            }
            if (letterindex != 0)
                DOC_NO = DOC_NO.Substring(letterindex);
            DOC_NO = "%" + DOC_NO + "%";
        }
        if (JOB_ORDER_NO != "" && JOB_ORDER_NO != null && JOB_ORDER_NO != "null")
            JOB_ORDER_NO = "%" + JOB_ORDER_NO + "%";

        DataSet ds = barcodeinquirydal.GetDOCNOList(DOC_NO, JOB_ORDER_NO, STATUS, SENDPROCESS, RECEIVEPROCESS, CREATEDATEFROM, CREATEDATETO);
        List<DOCNOLIST_INQUIRY> docnolist = new List<DOCNOLIST_INQUIRY>();
        if (ds.Tables.Count > 0)
        {
            foreach (DataTable dt in ds.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DOCNOLIST_INQUIRY docnotemp = new DOCNOLIST_INQUIRY();
                    try
                    {
                        docnotemp.DOC_NO = dr[4].ToString()+dr[0].ToString();
                        docnotemp.CREATE_DATE = dr[1].ToString();
                        docnotemp.SUBMIT_DATE = dr[2].ToString();
                        docnotemp.CONFIRM_DATE = dr[3].ToString();
                        docnotemp.PROCESS_PRODUCTION_FACTORY = dr[4].ToString();
                        docnotemp.PROCESS_CD = dr[5].ToString();
                        docnotemp.PRODUCTION_LINE_CD = dr[6].ToString();
                        docnotemp.NEXT_PROCESS_PRODUCTION_FACTORY = dr[7].ToString();
                        docnotemp.NEXT_PROCESS_CD = dr[8].ToString();
                        docnotemp.NEXT_PRODUCTION_LINE_CD = dr[9].ToString();
                        docnotemp.STATUS = dr[10].ToString();
                        //tangyh 2017.03.29
                        docnotemp.QTY = int.Parse(dr[11].ToString());
                        docnotemp.SELECTED = dr[12].ToString();
                        docnolist.Add(docnotemp);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        string JsonResponse = "";
        JsonResponse = "[{\"SUCCESS\":true, \"data\":";
        JsonResponse += JsonConvert.SerializeObject(docnolist);
        JsonResponse += "}]";
        return JsonResponse;
    }

    public string GetDOCNODetailList(string DOC_NO, string BYPART)
    {

        if (DOC_NO.Length==16)
            DOC_NO = DOC_NO.Substring(3); 
        DataSet ds = barcodeinquirydal.GetDOCNODetailList(DOC_NO, BYPART);
        List<DOCNOLISTDETAIL_INQUIRY> docnodetaillist = new List<DOCNOLISTDETAIL_INQUIRY>();
        if (ds.Tables.Count > 0)
        {
            foreach (DataTable dt in ds.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DOCNOLISTDETAIL_INQUIRY docnodetailtemp = new DOCNOLISTDETAIL_INQUIRY();
                    try
                    {
                        docnodetailtemp.JOB_ORDER_NO = dr[0].ToString();
                        docnodetailtemp.COLOR_CD = dr[1].ToString();
                        docnodetailtemp.SIZE_CD = dr[2].ToString();
                        docnodetailtemp.LAY_NO = dr[3].ToString();
                        docnodetailtemp.BUNDLE_NO = dr[4].ToString();
                        docnodetailtemp.PART_DESC = dr[5].ToString();
                        docnodetailtemp.QTY = dr[6].ToString();
                        docnodetailtemp.BARCODE = dr[7].ToString();
                        docnodetailtemp.CARTON_BARCODE = dr[8].ToString();
                        docnodetailtemp.CARTON_STATUS = dr[9].ToString();
                        docnodetaillist.Add(docnodetailtemp);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        string JsonResponse = "";
        JsonResponse = "[{\"SUCCESS\":true, \"data\":";
        JsonResponse += JsonConvert.SerializeObject(docnodetaillist);
        JsonResponse += "}]";
        return JsonResponse;
    }


    public string GetBarcodeInformationDetail(string GO, string JO, string COLOR, string LAYNO, string BUNDLENO, string BARCODE)
    {
        COMMONBLL commonbll = new COMMONBLL();
        string BARCODETYPE = commonbll.GetBarcodeType(BARCODE);
        
        if (BARCODETYPE=="D")
        {
            BARCODE = BARCODE.Substring(3);
        }
        if (GO != "" && GO != null && GO != "null")
            GO = "%" + GO + "%";
        if (JO != "" && JO != null && JO != "null")
            JO = "%" + JO + "%";

        DataSet ds = barcodeinquirydal.GetBarcodeInformationDetail(GO, JO, COLOR, LAYNO, BUNDLENO, BARCODE, BARCODETYPE);
        List<BARCODEINFORMATION_INQUIRY> barcodedetaillist = new List<BARCODEINFORMATION_INQUIRY>();
        if (ds.Tables.Count > 0)
        {
            foreach (DataTable dt in ds.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    BARCODEINFORMATION_INQUIRY barcodedetail = new BARCODEINFORMATION_INQUIRY();
                    try
                    {
                        barcodedetail.FACTORY_CD = dr[0].ToString();
                        barcodedetail.PROCESS_CD = dr[1].ToString();
                        barcodedetail.PRODUCTION_LINE_CD = dr[2].ToString();
                        barcodedetail.CUT_LINE = dr[3].ToString();
                        barcodedetail.GARMENT_ORDER_NO = dr[4].ToString();
                        barcodedetail.JOB_ORDER_NO = dr[5].ToString();
                        barcodedetail.COLOR_CD = dr[6].ToString();
                        barcodedetail.SIZE_CD = dr[7].ToString();
                        barcodedetail.LAY_NO = dr[8].ToString();
                        barcodedetail.BUNDLE_NO = dr[9].ToString();
                        barcodedetail.BARCODE = dr[10].ToString();
                        barcodedetail.PART_CD = dr[11].ToString();
                        barcodedetail.PART_DESC = dr[12].ToString();
                        barcodedetail.QTY = dr[13].ToString();
                        barcodedetail.DISCREPANCY_QTY = dr[14].ToString();
                        barcodedetail.CARTON_BARCODE = dr[15].ToString();
                        barcodedetail.CARTON_STATUS = dr[16].ToString();
                        if (dr[15].ToString() != "" && dr[17].ToString() != "0")
                            barcodedetail.DOC_NO = barcodedetail.FACTORY_CD + dr[17].ToString();
                        if (barcodedetail.DISCREPANCY_QTY != "" && barcodedetail.DISCREPANCY_QTY != null)
                            barcodedetail.WIP = (int.Parse(barcodedetail.QTY) - int.Parse(barcodedetail.DISCREPANCY_QTY)).ToString();
                        else
                            barcodedetail.WIP = barcodedetail.QTY;
                        barcodedetail.IS_MATCHING = dr[18].ToString();
                        barcodedetail.SEW_LINE = dr[19].ToString();
                        barcodedetaillist.Add(barcodedetail);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        string JsonResponse = "";
        JsonResponse = "[{\"SUCCESS\":true, \"data\":";
        JsonResponse += JsonConvert.SerializeObject(barcodedetaillist);
        JsonResponse += "}]";
        return JsonResponse;
    }

    public string GetColorLaynoBundleByJO(string factory, string svTYPE, string JO)
    {
        COMMONDAL commondal = new COMMONDAL(factory, svTYPE);
        string JsonResponse = "";
        JsonResponse = "[{\"SUCCESS\":true, \"COLOR\":" + JsonConvert.SerializeObject(commondal.GetColorByJO(JO)) + ", \"LAYNO\":" + JsonConvert.SerializeObject(commondal.GetLaynoByJO(JO)) + ", \"BUNDLENO\":" + JsonConvert.SerializeObject(commondal.GetBundlenoByJO(JO)) + "}]";
        return JsonResponse;
    }

    public string GetJO(string factory, string svTYPE)
    {
        COMMONDAL commondal = new COMMONDAL(factory, svTYPE);
        string JsonResponse = "";
        JsonResponse = "[{\"SUCCESS\":true, \"data\":";
        JsonResponse += JsonConvert.SerializeObject(commondal.GetJO(factory));
        JsonResponse += "}]";
        return JsonResponse;
    }

    public string GetGO(string factory, string svTYPE)
    {
        COMMONDAL commondal = new COMMONDAL(factory, svTYPE);
        string JsonResponse = "";
        JsonResponse = "[{\"SUCCESS\":true, \"data\":";
        JsonResponse += JsonConvert.SerializeObject(commondal.GetGO(factory));
        JsonResponse += "}]";
        return JsonResponse;
    }

    public string GetJO(string factory, string svTYPE, string GO)
    {
        COMMONDAL commondal = new COMMONDAL(factory, svTYPE);
        string JsonResponse = "";

        if (GO != "" && GO != null && GO != "null")
        {
            JsonResponse = "[{\"SUCCESS\":true, \"data\":";
            JsonResponse += JsonConvert.SerializeObject(commondal.GetJO(factory, GO));
            JsonResponse += "}]";
        }
        else
        {
            JsonResponse = "[{\"SUCCESS\":true, \"data\":";
            JsonResponse += JsonConvert.SerializeObject(commondal.GetJO(factory));
            JsonResponse += "}]";
        }
        return JsonResponse;
    }

    public string GetBundleByJOColorLayno(string factory, string svTYPE, string JO, string COLOR, string LAYNO)
    {
        COMMONDAL commondal = new COMMONDAL(factory, svTYPE);

        string JsonResponse = "";

        if (COLOR != "" && COLOR != null && COLOR != "null" && (LAYNO == "" || LAYNO == null || LAYNO == "null"))
        {
            JsonResponse = "[{\"SUCCESS\":true, \"BUNDLENO\":" + JsonConvert.SerializeObject(commondal.GetBundlenoByJOColor(JO, COLOR)) + "}]";
        }
        else if (LAYNO != "" && LAYNO != null && LAYNO != "null" && (COLOR == "" || COLOR == null || COLOR == "null"))
        {
            JsonResponse = "[{\"SUCCESS\":true, \"BUNDLENO\":" + JsonConvert.SerializeObject(commondal.GetBundlenoByJOLayno(JO, LAYNO)) + "}]";
        }
        else if (LAYNO != "" && LAYNO != null && LAYNO != "null" && COLOR != "" && COLOR != null && COLOR != "null")
        {
            JsonResponse = "[{\"SUCCESS\":true, \"BUNDLENO\":" + JsonConvert.SerializeObject(commondal.GetBundlenoByJOColorLayno(JO, COLOR, LAYNO)) + "}]";
        }
        else
        {
            JsonResponse = "[{\"SUCCESS\":false}]";
        }

        return JsonResponse;
    }
}