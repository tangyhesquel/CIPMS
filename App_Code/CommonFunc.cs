using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

/// <summary>
///Common 的摘要说明
/// </summary>
namespace commonfunction
{
    public class CommonFunc
    {
        public CommonFunc()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        //获取条码的类型：扎码B、箱码C或者流水单号D
        public string GetBarcodeType(string barcode)
        {
            if (barcode != null && barcode != "")
            {
                int index = barcode.IndexOf("-");
                if (index > -1)//存在符号“-”，说明是箱码或者流水单号
                {
                    //获取字符串第四个到第九个字符
                    string keyword = barcode.Substring(3, 6);
                    if (Regex.Matches(keyword, "[a-zA-Z]").Count > 0)//如果获取的字母的捕获数大于零
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
                {
                    return "B";
                }
            }
            else
                return null;
        }


        //判断主次箱码
        public int JudgeCartonType(string cartonbarcode)
        {
            if (cartonbarcode != null && cartonbarcode != "")
            {
                return Regex.Matches(cartonbarcode, "-").Count;
            }
            else
                return 0;
        }



        //获取主箱码
        public string GetMainCarton(string cartonbarcode)
        {
            if (Regex.Matches(cartonbarcode, "-").Count > 1)//是次箱码
            {
                int a = cartonbarcode.LastIndexOf("-");
                return cartonbarcode.Substring(0, cartonbarcode.LastIndexOf("-"));
            }
            else
                return cartonbarcode;
        }



        //获取箱码的seq
        public string GetCartonSeq(string cartonbarcode)
        {
            if (Regex.Matches(cartonbarcode, "-").Count > 0)//是主箱码
            {
                return cartonbarcode.Substring(cartonbarcode.LastIndexOf("-")+1);
            }
            else
                return null;
        }
    }
}