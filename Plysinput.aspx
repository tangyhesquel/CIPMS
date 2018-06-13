<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Plysinput.aspx.cs" Inherits="Input2" %>

<!DOCTYPE html>
<html>
<head>
    <link href="css/JQM/jquery.mobile-1.4.5.min.css" rel="stylesheet" type="text/css" />
    <script src="js/JQM/jquery.js" type="text/javascript"></script>
    <script src="js/JQM/jquery.mobile-1.4.5.min.js" type="text/javascript"></script>
    <link href="css/jquery.mmenu.all.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.mmenu.min.all.js" type="text/javascript"></script>
</head>
<body>   
    <div data-role="page">
    <script type="text/javascript">

        //语言翻译函数
        window.language; //全局变量，保存用户选择了哪个json语言文件
        function translate() {
            $.ajax({
                type: "GET",
                url: window.language,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                async: false,
                cache: false,
                beforeSend: function() { $.mobile.loading('show'); },
                error: function() { $.mobile.loading('hide'); alert("Failure! Please check your network!") },
                success: function(json) {
                    $.mobile.loading('hide');
                    globeltranslate = eval(json);
                    $('#headertitle').html(globeltranslate.headertitle);
                    $('#cancelbutton').html(globeltranslate.cancelbutton);
                    $('#functionmenubutton').html(globeltranslate.functionmenubutton);
                    $('#contenttitle').html(globeltranslate.contenttitle);
                    $('#processlabel').html(globeltranslate.processlabel);
                    $('#productionlinelabel').html(globeltranslate.productionlinelabel);
                    $('#jolabel').html(globeltranslate.jolabel);
                    $('#jotitle').html(globeltranslate.jotitle);
                    $('#laynolabel').html(globeltranslate.laynolabel);
                    $('#colorlabel').html(globeltranslate.colorlabel);
                    $('#sizeratiolabel').html(globeltranslate.sizeratiolabel);
                    $('#plyslabel').html(globeltranslate.plyslabel);
                    $('#partlabe').html(globeltranslate.partlabe);
                    $('#confirmbutton').html(globeltranslate.confirmbutton);
                    $('#dctreporta').html(globeltranslate.dctreporta);
                    $('#docnoinquirya').html(globeltranslate.docnoinquirya);
                    $('#barcodeinquirya').html(globeltranslate.barcodeinquirya);
                    $('#wipandoutputlia').html(globeltranslate.wipandoutputlia);
                }
            });
        };

        $(function() {
            //调用语言翻译函数
            var globeltranslate; //保存json字符串
            if (window.localStorage.languageid == "1") {
                window.language = "Externalreceive/Plysinput-zh-CN.js";
            } else if (window.localStorage.languageid == "2") {
                window.language = "Externalreceive/Plysinput-en-US.js";
            }
            translate();

            //左侧栏
            $("#my-menu").mmenu({
                "footer": {
                    "add": true,
                    "title": "CPMIS"
                },
                "header": {
                    "title": "请选择",
                    "add": true,
                    "update": true
                },
                slidingSubmenus: false
            });
            $('#functionmenubutton').click(function() {
                $('#my-menu').trigger('open.mm');
            });

            //用户登录之后才能进入系统模块
            if (sessionStorage.getItem("name") == null || sessionStorage.getItem("name") == "") {
                window.location.href = "Login.aspx";
            }
            else {
                //function权限控制
                $('#packageli').hide();
                $('#transactionli').hide();
                $('#receiveli').hide();
                $('#bundlereduceli').hide();
                $('#employeeoutputli').hide();
                $('#externalreceiveli').hide();
                $('#reportli').hide();
                $('#messagetitle').html(sessionStorage.getItem("name") + " / " + sessionStorage.getItem("factory") + sessionStorage.getItem("process") + sessionStorage.getItem("productionline"));
                var employeeno = sessionStorage.getItem("employeeno");
                var factory = sessionStorage.getItem("factory");
                $.ajax({
                    type: 'POST',
                    url: 'Package/Accessfunction.ashx',
                    dataType: 'json',
                    async: false,
                    data: { factory: factory, employeeno: employeeno },
                    beforeSend: function() { $.mobile.loading('show'); },
                    success: function(data) {
                        $.mobile.loading('hide');
                        //判断用户的权限模块
                        $(data).each(function(i) {
                            if (data[i].MODULE == 'Package') $('#packageli').show();
                            else if (data[i].MODULE == 'Transaction') $('#transactionli').show();
                            else if (data[i].MODULE == 'Receive') $('#receiveli').show();
                            else if (data[i].MODULE == 'BundleReduce') $('#bundlereduceli').show();
                            else if (data[i].MODULE == 'EmployeeOutput') $('#employeeoutputli').show();
                            else if (data[i].MODULE == 'ExternalReceive') $('#externalreceiveli').show();
                            else if (data[i].MODULE == 'Report') $('#reportli').show();
                        });
                    },
                    error: function(err) {
                        $.mobile.loading('hide');
                        alert(err);
                    }
                });
            }

            //注销按钮：返回登录页面
            $('#cancelbutton').click(function() {
                sessionStorage.clear();
                window.location.href = "Login.aspx";
            });
        });
    </script> 
        <div data-role="header" data-theme="b">
            <table style="width: 100%">
                <tr>
                    <td style="width: 8%">
                        <button id="functionmenubutton" data-icon="grid" data-iconpos="left"></button>
                    </td>
                    <td style="width: 30%">
                        <label id="messagetitle" style="text-align: left">
                        </label>
                    </td>
                    <td style="width: 24%; text-align: center">
                        <label id="headertitle"></label>
                    </td>
                    <td style="width: 30%">
                    </td>
                    <td style="width: 8%; text-align: right">
                        <button id="cancelbutton" data-icon="back" data-iconpos="right"></button>
                    </td>
                </tr>
            </table>
        </div>

        <div data-role="content" style="position: absolute; width: 50%; top: 14%; left: 25%; bottom: 8%">
            <ul data-role="listview" data-inset="true" data-split-theme="e">
                <li>
                    <h1 id="contenttitle" style="text-align: center"></h1>
                </li>
                <li style = "padding : 0px 0px">
                    <div style="margin: 0px 0px; padding : 0px 0px">
                        <table width="100%">
                            <tr>
                                <td style="width: 20%; text-align: center">
                                    <label for="processselect" id="processlabel"></label>
                                </td>
                                <td style="width: 30%; text-align: right;">
                                    <select name="processselect" id="processselect">
                                        <option>--Select--</option>
                                    </select>
                                </td>
                                <td style="width: 20%; text-align: center">
                                    <label for="productionlineselect" id="productionlinelabel">Production Line</label>
                                </td>
                                <td style="width: 30%; text-align: right;">
                                    <select id="productionlineselect" name="productionlineselect">
                                        <option>--Select--</option>
                                    </select>
                                </td>
                            </tr>
                        </table>
                    </div>
                </li>
                <li style = "padding : 0px 0px">
                    <div style="margin: 0px 0px; padding : 0px 0px">
                        <table width="100%">
                            <tr>
                                <td style="width: 20%; text-align: center">
                                    <label for="jotext" id="jolabel"></label>
                                </td>
                                <td style="width: 30%; text-align: right;">
                                    <input type="text" name="jotext" id="jotext"/>
                                </td>
                                <td style="width: 20%; text-align: center">
                                    <label for="laynotext" id="laynolabel"></label>
                                </td>
                                <td style="width: 30%; text-align: right;">
                                    <input type="text" name="laynotext" id="laynotext"/>
                                </td>
                            </tr>
                        </table>
                    </div>
                </li>
                <li style = "padding : 0px 0px">
                    <div style="margin: 0px 0px; padding : 0px 0px">
                        <table width="100%">
                            <tr>
                                <td style="width: 20%; text-align: center">
                                    <label for="colortext" id="colorlabel"></label>
                                </td>
                                <td style="width: 30%; text-align: right;">
                                    <input type="text" name="colortext" id="colortext"/>
                                </td>
                                <td style="width: 20%; text-align: center">
                                    <label for="plystext" id="plyslabel"></label>
                                </td>
                                <td style="width: 30%; text-align: right;">
                                    <input type="text" name="plystext" id="plystext"/>
                                </td>
                            </tr>
                        </table>
                    </div>
                </li>
                <li style = "padding : 0px 0px">
                    <div style="margin: 0px 0px; padding : 0px 0px">
                        <table width="100%">
                            <tr>
                                <td style="width: 20%; text-align: center">
                                    <label for="sizeratiotext" id="sizeratiolabel"></label>
                                </td>
                                <td style="width: 30%; text-align: right;">
                                    <select multiple name="sizeratiotext" id="sizeratiotext" data-native-menu="false">
                                        <option>--Select--</option>
                                        <option>S</option>
                                        <option>M</option>
                                        <option>L</option>
                                        <option>XL</option>
                                        <option>XXL</option>
                                    </select>
                                </td>
                                <td style="width:50%"></td>
                            </tr>
                        </table>
                    </div>
                </li>
                <li style = "padding : 0px 0px">
                    <div style="margin: 0px 0px; padding : 0px 0px">
                        <table width="100%">
                            <tr>
                                <td style="width: 20%; text-align:center">
                                    <label for="partselect" id="partlabel">Part</label>
                                </td>
                                <td style="width:30%">
                                    <select id="partselect" name="partselect" data-native-menu="false">
                                        <option>--Select--</option>
                                        <option>Front</option>
                                        <option>Back</option>
                                        <option>Collar</option>
                                        <option>Sleeve</option>
                                    </select>
                                </td>
                                <td style="width: 50%; text-align: center">
                                </td>
                            </tr>
                        </table>
                    </div>
                </li>
                <li style = "padding : 0px 0px">
                    <div style="margin: 0px 0px; padding : 0px 0px">
                        <table width="100%">
                            <tr>
                                <td style="width: 30%">
                                </td>
                                <td style="width: 40%; text-align:center;">
                                    <button id="confirmbutton">Confirm</button>
                                </td>
                                <td style="width: 30%">
                                </td>
                            </tr>
                        </table>
                    </div>
                </li>
            </ul>
        </div>

        <div data-role="footer" data-theme="a" >
        </div>
    </div>
    <nav id="my-menu">
        <ul>
            <li id="packageli">
                <a id="packagea" href="Package.aspx" onclick="packagejump(fty);return false;" data-ajax="false"></a>
            </li>
            <li id="matchingli">
                <a id="matchinga" href="Matching.aspx" onclick="matchingjump(fty);return false;" data-ajax="false"></a>
            </li>
            <li id="transactionli">
                <a id="transactiona" href="Transaction.aspx" onclick="transactionjump(fty);return false;" data-ajax="false"></a>
            </li>
            <li id="receiveli">
                <a id="receivea" href="Receive.aspx" onclick="receivejump(fty);return false;" data-ajax="false"></a>
            </li>
            <li id="bundlereduceli">
                <a id="bundlereducea" href="Reduce.aspx" onclick="reducejump(fty);return false;" data-ajax="false"></a>
            </li>
            <li id="printli">
                <a id="printbarcodea" href="Printbarcode.aspx" onclick="printbarcodejump(fty);return false;" data-ajax="false"></a>
            </li>
            <li id="employeeoutputli">
                <a id="employeeoutputa" href="Employeeoutput.aspx" onclick="employeeoutputjump(fty);return false;" data-ajax="false"></a>
            </li>
            <li id="externalreceiveli">
                <a id="externalreceivea" href="#mm-1"></a>
                <ul>
                    <li><a id="bundleinputa" href="" data-ajax="false"></a></li>
                </ul>
            </li>
            <li id="reportli">
                <a id="reporta" href="#mm-2"></a>
                <ul>
                    <li id="wipreportli"><a id="wipreporta" href="Wipreport.aspx" onclick="wipreportjump(fty);return false;" data-ajax="false"></a></li>
                    <li id="outputreportli"><a id="outputreporta" href="Outputreport.aspx" onclick="outputreportjump(fty);return false;" data-ajax="false"></a></li>
                    <li id="dctreportli"><a id="dctreporta" href="Outputreport.aspx" onclick="dctreportjump(fty);return false;" data-ajax="false"></a></li>
                    <li id="docnoli"><a id="docnoinquirya" href="Barcode_Inquiry.aspx" onclick="docnoinquiryjump(fty, environment);return false;" data-ajax="false"></a></li>
                    <li id="bundleorcartonli"><a id="barcodeinquirya" href="Barcode_Information_Inquiry.aspx" onclick="bundleorcartoninquiryjump(fty, environment);return false;" data-ajax="false"></a></li>
                    <li id="wipandoutputli"><a id="wipandoutputlia" href="WipAndOutputNEW.aspx" onclick="WipAndOutputJump(fty, environment);return false;" data-ajax="false"></a></li>
                </ul>
            </li>
        </ul>
    </nav>
</body>
</html>