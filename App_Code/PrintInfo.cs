using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///PrintInfo 的摘要说明
/// </summary>
public class PrintInfo
{
	public PrintInfo()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}

    private static string _jo;
    private string _barcode;
    private string _color;
    private string _size;
    private string _layno;
    private string _bundleno;
    private string _qty;
    private string _partcd;
    private string _productionlinecd;

    public string jo
    {
        get { return _jo; }
        set { _jo = value; }
    }
    public string barcode
    {
        get { return _barcode; }
        set { _barcode = value; }
    }
    public string color
    {
        get { return _color; }
        set { _color = value; }
    }
    public string size
    {
        get { return _size; }
        set { _size = value; }
    }
    public string layno
    {
        get { return _layno; }
        set { _layno = value; }
    }
    public string bundleno
    {
        get { return _bundleno; }
        set { _bundleno = value; }
    }
    public string qty
    {
        get { return _qty; }
        set { _qty = value; }
    }
    public string partcd
    {
        get { return _partcd; }
        set { _partcd = value; }
    }
    public string productionlinecd
    {
        get { return _productionlinecd; }
        set { _productionlinecd = value; }
    }
}