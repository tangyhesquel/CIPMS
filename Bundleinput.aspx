<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Bundleinput.aspx.cs" Inherits="Input" %>

<!DOCTYPE html>
<html>
<head>
    <link href="css/JQM/jquery.mobile-1.4.5.min.css" rel="stylesheet" type="text/css" />
    <script src="js/JQM/jquery.js" type="text/javascript"></script>
    <script src="js/JQM/jquery.mobile-1.4.5.min.js" type="text/javascript"></script>
    <link href="css/mobiscroll.custom-2.5.0.min.css" rel="stylesheet" type="text/css" />
    <script src="js/mobiscroll.custom-2.5.0.min.js" type="text/javascript"></script>
    <link href="css/jquery.mmenu.all.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.mmenu.min.all.js" type="text/javascript"></script>
    <script src="js/jquery.chromatable.js" type="text/javascript"></script>
    <script src="js/Common.js?v1.0" type="text/javascript"></script>
</head>
<body> 
    <div data-role="page">
    <style type="text/css">
        th
            {
                background: #dde6f5;
                border-top: 1px solid #d6d6d6;
                border-bottom: 1px solid #d6d6d6;
            }
            tr
            {
                border-left: 1px solid #d6d6d6;
                border-right: 1px solid #d6d6d6;
            }
            tr:nth-child(even)
            {
                background: #e9e9e9;
            }
            tr.over td
            {
                background: #bcd4ec;
            }
    </style>
    <script type="text/javascript">
        var fty = "";
        var environment = "";
        var productionselected = "";
        var globeltranslate; //保存json字符串
        var docno;

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
                error: function () { alert("Sorry, something wrong! Pls try again! Wrong Code:001") },
                success: function(json) {
                    globeltranslate = eval(json);
                    $('#functionmenubutton').html(globeltranslate.functionmenubutton);
                    $('#headertitle').html(globeltranslate.headertitle);
                    $('#cancelbutton').html(globeltranslate.cancelbutton);
                    $('#contenttitle').html(globeltranslate.contenttitle);
                    $('#processselectlabel').html(globeltranslate.processselectlabel);
                    $('#jotextlabel').html(globeltranslate.jotextlabel);
                    $('#bundletextlabel').html(globeltranslate.bundletextlabel);
                    $('#sizetextlabel').html(globeltranslate.sizetextlabel);
                    $('#partselectlabel').html(globeltranslate.partselectlabel);
                    $('#laynotextlabel').html(globeltranslate.laynotextlabel);
                    $('#colortextlabel').html(globeltranslate.colortextlabel);
                    $('#qtytextlabel').html(globeltranslate.qtytextlabel);
                    $('#importbutton').val(globeltranslate.importbutton).button('refresh');
                    $('#clearbutton').val(globeltranslate.clearbutton).button('refresh');
                    $('#surebutton').val(globeltranslate.surebutton).button('refresh');
                    $('#printbutton').val(globeltranslate.printbutton).button('refresh');
                    $('#packagea').html(globeltranslate.packagea);
                    $('#matchinga').html(globeltranslate.matchinga);
                    $('#transactiona').html(globeltranslate.transactiona);
                    $('#receivea').html(globeltranslate.receivea);
                    $('#bundlereducea').html(globeltranslate.bundlereducea);
                    $('#printbarcodea').html(globeltranslate.printbarcodea);
                    $('#employeeoutputa').html(globeltranslate.employeeoutputa);
                    $('#externalreceivea').html(globeltranslate.externalreceivea);
                    $('#bundleinputa').html(globeltranslate.bundleinputa);
                    $('#reporta').html(globeltranslate.reporta);
                    $('#wipreporta').html(globeltranslate.wipreporta);
                    $('#outputreporta').html(globeltranslate.outputreporta);
                    $('#dctreporta').html(globeltranslate.dctreporta);
                    $('#docnoinquirya').html(globeltranslate.docnoinquirya);
                    $('#barcodeinquirya').html(globeltranslate.barcodeinquirya);
                    $('#wipandoutputlia').html(globeltranslate.wipandoutputlia);
                    $('#printsparebarcodea').html(globeltranslate.printsparebarcodea);
                }
            });
        }

        function readThis() {
            if ($('#partselect').val() == globeltranslate.select) {
                alert(globeltranslate.importmessage4);
                return;
            }
            $.mobile.loading("show");
            var tempStr = "";
            var filePath = $('#file').val();
            var oXL = new ActiveXObject("Excel.application");
            var oWB = oXL.Workbooks.open(filePath);
            oWB.worksheets(1).select();
            var oSheet = oWB.ActiveSheet;
            try {
                //构造字符串
                var jsondata = ""; ;
                var j = 0;
                for (var i = 2; i <= oSheet.usedrange.rows.count; i++) {
                    if (oSheet.Cells(i, 1).value == null || oSheet.Cells(i, 2).value == null || oSheet.Cells(i, 3).value == null || oSheet.Cells(i, 4).value == null || oSheet.Cells(i, 5).value == null || oSheet.Cells(i, 6).value == null)
                        alert(globeltranslate.importmessage1 + i + globeltranslate.importmessage2);
                    else {
                        if (j == 0)
                            jsondata += oSheet.Cells(i, 1) + "," + oSheet.Cells(i, 2) + "," + oSheet.Cells(i, 3) + "," + oSheet.Cells(i, 4) + "," + oSheet.Cells(i, 5) + "," + oSheet.Cells(i, 6);
                        else
                            jsondata += "////" + oSheet.Cells(i, 1) + "," + oSheet.Cells(i, 2) + "," + oSheet.Cells(i, 3) + "," + oSheet.Cells(i, 4) + "," + oSheet.Cells(i, 5) + "," + oSheet.Cells(i, 6);
                        j++;
                    }
                }
                //表内数据全部保存到json字符串，传到后台和服务器端进行比较，判断是否已经生成了条码。返回处理完的带有barcode的json字符串
                var seq = "0";
                if ($('#bundleinputtable tr:last').find('td').parents('tr').children('td').eq(0).text() != "") {
                    seq = $('#bundleinputtable tr:last').find('td').parents('tr').children('td').eq(0).text();
                }
                $.mobile.loading('show');
                setTimeout(function () {
                    $.ajax({
                        type: "POST",
                        url: "Bundleinput.aspx/Checkdata",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        async: false,
                        beforeSend: function () { $.mobile.loading('show'); },
                        data: "{ 'seq': '" + seq + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno':'" + docno + "', 'jsondata': '" + jsondata + "', 'part': '" + $('#partselect').val() + "', 'dbtrans': '" + globeltranslate.deletebutton + "' }",
                        success: function (data) {
                            $.mobile.loading('hide');
                            if (data.d == 'false') {
                                alert(globeltranslate.importmessage3);
                            } else {
                                $('#bundleinputtable tbody').append(data.d);
                                $('#bundleinputtable').trigger('create');
                            }
                        },
                        error: function (err) {
                            $.mobile.loading('hide');
                            alert(globeltranslate.errormessage + "054");
                        }
                    });
                }, 10);
                $.mobile.loading("hide");
            } catch (e) {
                alert(e);
            }
            oXL.Quit();
            CollectGarbage();

        }

        function partselecttrans() {
            $('#partselect').empty();
            $('#partselect').append("<option>" + globeltranslate.select + "</option>");
            $('#partselect').selectmenu('refresh');
            part();
        }

        function processselecttrans() {
            $('#processselect').empty();
            $('#processselect').append("<option selected='selected' value='" + sessionStorage.getItem("process") + "'>" + sessionStorage.getItem("process") + "</option>");
            $('#processselect').selectmenu('refresh');
        }

        //表格里的删除按钮监控
        function deletebtn(obj) {
            var jo = $(obj).parents('tr').children('td').eq(1).text();
            var bundle = $(obj).parents('tr').children('td').eq(2).text();
            var part = $(obj).parents('tr').children('td').eq(3).text();
            $.mobile.loading('show');
            setTimeout(function () {
                $.ajax({
                    type: "POST",
                    url: "Bundleinput.aspx/Delete",
                    contentType: "application/json;charset=utf-8",
                    dataType: "json",
                    async: false,
                    beforeSend: function () { $.mobile.loading('show'); },
                    data: "{ 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'jo': '"+jo+"', 'bundle': '"+bundle+"', 'part': '"+part+"' }",
                    success: function (data) {
                        $(obj).closest('tr').remove();
                        $('#bundleinputtable').trigger('create');
                        alert(globeltranslate.deletemessage);
                        $.mobile.loading('hide');
                    },
                    error: function (err) {
                        $.mobile.loading('hide');
                        alert(globeltranslate.errormessage + "002");
                    }
                });
            }, 10);
        }

        function part() {
            //加载part select
            $.ajax({
                type: "POST",
                url: "Bundleinput.aspx/Part",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "' }",
                success: function (data) {
                    $('#partselect').append(data.d);
                    $('#partselect').selectmenu('refresh');
                },
                error: function (err) {
                    alert(globeltranslate.errormessage + "005");
                }
            });
        }

        function empty() {
            //清空界面
            partselecttrans();
            $('#jotext').val("");
            $('#bundletext').val("");
            $('#colortext').val("");
            $('#sizetext').val("");
            $('#qtytext').val("");
            $('#laynotext').val("");
            $('#file').val("");
            //清空扫描表
            $.mobile.loading('show');
            setTimeout(function () {
                $.ajax({
                    type: "POST",
                    url: "Bundleinput.aspx/Empty",
                    contentType: "application/json;charset=utf-8",
                    dataType: "json",
                    beforeSend: function () { $.mobile.loading('show'); },
                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "' }",
                    success: function (data) {
                        $.mobile.loading('hide');
                        $('#bundleinputtable tr:not(:first)').empty();
                        $('#bundleinputtable').trigger('create');
                    },
                    error: function (err) {
                        $.mobile.loading('hide');
                        alert(globeltranslate.errormessage + "058");
                    }
                });
            }, 10);
        }

        $(function () {
            fty = getUrlParam('FTY');
            environment = getUrlParam('svTYPE');
            if (fty == null || environment == null) {
                alert('The url is wrong! Pls input like 192.168.x.x/CIPMS/Login.aspx?FTY=GEG&svTYPE=PROD');
            } else {
                //调用语言翻译函数
                if (window.localStorage.languageid == "1") {
                    window.language = "Externalreceive/Bundleinput-zh-CN.js";
                } else if (window.localStorage.languageid == "2") {
                    window.language = "Externalreceive/Bundleinput-en-US.js";
                }
                translate();

                var d = new Date();
                var vYear = d.getFullYear()
                var vMon = d.getMonth() + 1;
                var vDay = d.getDate();
                var h = d.getHours();
                var m = d.getMinutes();
                var se = d.getSeconds();
                docno = sessionStorage.getItem("userbarcode") + vYear + vMon + vDay + h + m + se;

                //左侧栏
                $("#my-menu").mmenu({
                    "footer": {
                        "add": true,
                        "title": "CPMIS"
                    },
                    "header": {
                        "title": globeltranslate.select,
                        "add": true,
                        "update": true
                    },
                    slidingSubmenus: false
                });
                $('#functionmenubutton').click(function () {
                    $('#my-menu').trigger('open.mm');
                });

                //用户登录之后才能进入系统模块
                if (sessionStorage.getItem("name") == null || sessionStorage.getItem("name") == "") {
                    Loginout(fty, environment);
                }
                else {
                    partselecttrans();
                    Accessmodulehide();
                    processselecttrans();
                    $('#messagetitle').html(sessionStorage.getItem("name") + " , " + sessionStorage.getItem("factory") + "/" + sessionStorage.getItem("process") + "/" + sessionStorage.getItem("productionline"));
                    var userbarcode = sessionStorage.getItem("userbarcode");
                    var factory = sessionStorage.getItem("factory");
                    var process = sessionStorage.getItem("process");
                    $.ajax({
                        type: "POST",
                        url: "Package.aspx/Accessfunction",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        async: false,
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + userbarcode + "' }",
                        success: function (data) {
                            Accessmoduleshow(data.d);
                        },
                        error: function (err) {
                            alert(globeltranslate.errormessage + "007");
                        }
                    });

                    //注销按钮：返回登录页面
                    $('#cancelbutton').click(function () {
                        Loginout(fty, environment);
                    });

                    //表格样式
                    var screenwidth = document.body.clientWidth;
                    var screenheight = document.body.clientHeight;
                    $('#bundleinputtable').chromatable({
                        width: screenwidth * 0.96,
                        height: screenheight * 0.7,
                        scrolling: "yes"
                    });

                    //加载表头
                    $("#bundleinputtable thead tr").empty();
                    $('#bundleinputtable thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table3 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table4 + "</th><th>" + globeltranslate.table5 + "</th><th>" + globeltranslate.table6 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th></tr>");
                    $('#bundleinputtable').trigger('create');

                    //确认按钮监控
                    $('#surebutton').click(function () {
                        if ($('#jotext').val() == "" || $('#bundletext').val() == "" || $('#laynotext').val() == '' || $('#sizetext').val() == "" || $('#colortext').val() == "" || $('#qtytext').val() == "" || $('#partselect').val() == globeltranslate.select) {
                            alert(globeltranslate.confirmmessage1);
                            return;
                        } else {//生成新的条码
                            var seq = "0";
                            if ($('#bundleinputtable tr:last').find('td').parents('tr').children('td').eq(0).text() != "") {
                                seq = $('#bundleinputtable tr:last').find('td').parents('tr').children('td').eq(0).text();
                            }
                            $.mobile.loading("show");
                            setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: "Bundleinput.aspx/Confirm",
                                    contentType: "application/json;charset=utf-8",
                                    dataType: "json",
                                    beforeSend: function () { $.mobile.loading('show'); },
                                    data: "{ 'seq': '" + seq + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + userbarcode + "', 'docno': '" + docno + "', 'jo': '" + $('#jotext').val() + "', 'bundle': '" + $('#bundletext').val() + "', 'color': '" + $('#colortext').val() + "', 'size': '" + $('#sizetext').val() + "', 'part': '" + $('#partselect').val() + "', 'qty': '" + $('#qtytext').val() + "', 'layno': '" + $('#laynotext').val() + "', 'dbtrans': '" + globeltranslate.deletebutton + "' }",
                                    success: function (data) {
                                        $.mobile.loading('hide');
                                        if (data.d == 'false')
                                            alert(globeltranslate.confirmmessage2);
                                        else {
                                            $('#bundleinputtable tbody').append(data.d);
                                            $('#bundleinputtable').trigger('create');
                                        }
                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                        alert(globeltranslate.errormessage + "009");
                                    }
                                });
                            }, 10);
                        }
                    });

                    //清空按钮监控
                    $('#clearbutton').click(function () {
                        empty();
                    });

                    //打印按钮监控
                    $('#printbutton').click(function () {
                        $.mobile.loading('show');
                        setTimeout(function () {
                            $.ajax({
                                type: "POST",
                                url: "Bundleinput.aspx/Print",
                                contentType: "application/json;charset=utf-8",
                                dataType: "json",
                                beforeSend: function () { $.mobile.loading('show'); },
                                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'docno': '" + docno + "', 'userbarcode': '" + userbarcode + "' }",
                                success: function (data) {
                                    $.mobile.loading('hide');
                                    if (data.d == "success") {
                                        alert(globeltranslate.printmessage1);
                                        empty();
                                    } else {
                                        var result = eval(data.d);
                                        if (result.MESSAGE1 == "null") {
                                            alert(globeltranslate.printmessage2 + result.MESSAGE2 + globeltranslate.printmessage4);
                                        } else if (result.MESSAGE2 == "null") {
                                            alert(globeltranslate.printmessage2 + result.MESSAGE1 + globeltranslate.printmessage3);
                                        } else {
                                            alert(globeltranslate.printmessage2 + result.MESSAGE1 + globeltranslate.printmessage3 + " \n " + globeltranslate.printmessage2 + result.MESSAGE2 + globeltranslate.printmessage4);
                                        }
                                    }
                                },
                                error: function (err) {
                                    $.mobile.loading('hide');
                                    alert(globeltranslate.errormessage + "049");
                                }
                            });
                        }, 10);
                    });
                }
            }
        });
    </script> 
        <div data-role="header" data-theme="b">
            <table style="width: 100%">
                <tr>
                    <td style="width: 8%">
                        <button id="functionmenubutton" data-icon="grid" data-iconpos="left">
                        </button>
                    </td>
                    <td style="width: 30%">
                        <table id="messagetitle" style="text-align: center">
                        </table>
                    </td>
                    <td style="width: 24%; text-align: center">
                        <label id="headertitle" style="text-align: center">
                        </label>
                    </td>
                    <td style="width: 30%">
                    </td>
                    <td style="width: 8%; text-align: right">
                        <button id="cancelbutton" data-icon="back" data-iconpos="right">
                        </button>
                    </td>
                </tr>
            </table>
        </div>

        <div data-role="content" style="position: absolute; top: 10%; width: 100%; bottom: 2%;">
            <div class="ui-grid-c">
                <div class="ui-block-a">
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="processselect" id="processselectlabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <select data-native-menu="false" name="processselect" id="processselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="jotext" id="jotextlabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <input id="jotext" name="jotext" type="text" />
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="bundletext" id="bundletextlabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <input id="bundletext" name="bundletext" type="text" />
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="sizetext" id="sizetextlabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <input id="sizetext" name="sizetext" type="text" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-b">
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="partselect" id="partselectlabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <select data-native-menu="false" multiple="multiple" name="partselect" id="partselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="laynotext" id="laynotextlabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <input id="laynotext" name="laynotext" type="text" />
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="colortext" id="colortextlabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <input id="colortext" name="colortext" type="text" />
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="qtytext" id="qtytextlabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <input id="qtytext" name="qtytext" type="text" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-c">
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"></td>
                            <td style="width: 70%; text-align: center">
                                <input id="file" name="file" type="file"/>
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"></td>
                            <td style="width: 70%; text-align: center">
                                <input type="button" onclick="readThis()" style="text-align:center" id="importbutton" name="importbutton" />
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"></td>
                            <td style="width: 70%; text-align: right;">
                                <input type="button" style="text-align:center" id="clearbutton" name="clearbutton" />
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 50%; text-align: center">
                                <input type="button" style="text-align:center" id="surebutton" name="surebutton" />
                            </td>
                            <td style="width: 50%; text-align: right;">
                                <input type="button" style="text-align:center" id="printbutton" name="printbutton" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-d">
                </div>
            </div>
            
            <table width="100%">
                <tr>
                    <td style="width: 3%">
                    </td>
                    <td style="width: 94%; text-align: left">
                        <table width="100%">
                            <tr>
                                <td style="width: 10%"><label id="wiplabel"></label></td>
                                <td style="width: 90%"></td>
                            </tr>
                        </table>
                        <table class="stripe" id="bundleinputtable" width="100%" border="0" cellspacing="0" cellpadding="0">
                            <thead id="testthead">
                            </thead>
                            <tbody id="testtbody">
                            </tbody>
                        </table>
                    </td>
                    <td style="width: 3%">
                    </td>
                </tr>
            </table>

        </div>

        <div data-role="footer" data-theme="a" data-position="fixed" data-fullscreen="true">
        </div>
    </div>
    <nav id="my-menu">
        <ul>
            <li id="packageli">
                <a id="packagea" href="Package.aspx" onclick="packagejump(fty, environment);return false;" data-ajax="false"></a>
            </li>
            <li id="matchingli">
                <a id="matchinga" href="Matching.aspx" onclick="matchingjump(fty, environment);return false;" data-ajax="false"></a>
            </li>
            <li id="transactionli">
                <a id="transactiona" href="Transaction.aspx" onclick="transactionjump(fty, environment);return false;" data-ajax="false"></a>
            </li>
            <li id="receiveli">
                <a id="receivea" href="Receive.aspx" onclick="receivejump(fty, environment);return false;" data-ajax="false"></a>
            </li>
            <li id="bundlereduceli">
                <a id="bundlereducea" href="Reduce.aspx" onclick="reducejump(fty, environment);return false;" data-ajax="false"></a>
            </li>
            <li id="printli">
                <a id="printbarcodea" href="Printbarcode.aspx" onclick="printbarcodejump(fty, environment);return false;" data-ajax="false"></a>
            </li>
            <!--added by lijer-->          
            <li id="printsparebarcodeli">
               <a id="printsparebarcodea" href="PrintSparebarcode.aspx"   onclick="printsparebarcodejump(fty, environment);return false;" data-ajax="false"></a>
            </li>
            <li id="employeeoutputli">
                <a id="employeeoutputa" href="Employeeoutput.aspx" onclick="employeeoutputjump(fty, environment);return false;" data-ajax="false"></a>
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
                    <li id="wipreportli"><a id="wipreporta" href="Wipreport.aspx" onclick="wipreportjump(fty, environment);return false;" data-ajax="false"></a></li>
                    <li id="outputreportli"><a id="outputreporta" href="Outputreport.aspx" onclick="outputreportjump(fty, environment);return false;" data-ajax="false"></a></li>
                    <li id="dctreportli"><a id="dctreporta" href="Outputreport.aspx" onclick="dctreportjump(fty, environment);return false;" data-ajax="false"></a></li>
                    <li id="docnoli"><a id="docnoinquirya" href="Barcode_Inquiry.aspx" onclick="docnoinquiryjump(fty, environment);return false;" data-ajax="false"></a></li>
                    <li id="bundleorcartonli"><a id="barcodeinquirya" href="Barcode_Information_Inquiry.aspx" onclick="bundleorcartoninquiryjump(fty, environment);return false;" data-ajax="false"></a></li>
                    <li id="wipandoutputli"><a id="wipandoutputlia" href="WipAndOutputNEW.aspx" onclick="WipAndOutputJump(fty, environment);return false;" data-ajax="false"></a></li>
                </ul>
            </li>
        </ul>
    </nav>
</body>
</html>
