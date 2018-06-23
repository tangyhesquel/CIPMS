<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Printbarcode.aspx.cs" Inherits="Printbarcode" %>
<!DOCTYPE html>
<meta http-equiv="X-UA-Compatible" content="IE=Edge"> 
<html>
<head>
    <link href="css/JQM/jquery.mobile-1.4.5.min.css" rel="stylesheet" type="text/css" />
    <script src="js/JQM/jquery.js" type="text/javascript"></script>
    <script src="js/jquery.jqprint-0.3.js" type="text/javascript"></script>
    <script src="js/JQM/jquery.mobile-1.4.5.min.js" type="text/javascript"></script>
    <link href="css/jquery.mmenu.all.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.mmenu.min.all.js" type="text/javascript"></script>
    <script src="js/Common.js?v1.0" type="text/javascript"></script>
    <script src="js/jquery-barcode.js" type="text/javascript"></script>
    <script src="js/qrcode.js" type="text/javascript"></script>
</head>
<body>

    <div data-role="page">
    <script type="text/javascript" >
        var fty = "";
        var environment = "";
        var laynoselect = "";
        //layno 下拉框监控
        var oldjo = "";
        var oldlayno = "";

        function laynoselecttran() {
            $('#layno').empty();
            $('#layno').append("<option'>" + globeltranslate.layno + "</option>");
            $('#layno').selectmenu('refresh');
            laynoselect = globeltranslate.layno;
        }
        //tangyh 2017.03.24
        var bundleselect = "";
        function bundleselecttran() {
            $('#bundle').empty();
            $('#bundle').append("<option>" + globeltranslate.bundle + "</option>");
            $('#bundle').selectmenu('refresh');
            bundleselect = globeltranslate.bundle;
        }

        //语言翻译函数
        window.language; //全局变量，保存用户选择了哪个json语言文件
        var globeltranslate; //保存json字符串
        function translate() {
            $.ajax({
                type: "GET",
                url: window.language,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                async: false,
                cache: false,
                error: function () { $.mobile.loading('hide'); alert("Sorry! Pls check your network and refresh your brower! Wrong Code::001") },
                success: function (json) {
                    globeltranslate = eval(json);
                    $('#headertitle').html(globeltranslate.headertitle);
                    $('#footertitle').html(globeltranslate.footertitle);
                    $('#cancelbutton').html(globeltranslate.cancelbutton);
                    $('#functionmenubutton').html(globeltranslate.functionmenubutton);
                    $('#laynolabel').html(globeltranslate.laynolabel);
                    $('#bundlelabel').html(globeltranslate.bundlelabel);
                    $('#jobordernolabel').html(globeltranslate.jobordernolabel);
                    $('#queryjobutton').val(globeltranslate.queryjobutton).button('refresh');
                    $('#printbutton').val(globeltranslate.printbutton).button('refresh');
                    $('#combinepart').html(globeltranslate.combinepart);
                    $('#isprintpart').html(globeltranslate.isprintpart);//added on 2018-02-26
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
                    $('#printsparebarcodea').html(globeltranslate.printsparebarcodea); //add by lijer on 2018-05-18
                    
                }
            });
        };

        function bundledata() {

            if (($('#joborderno').val() == "") || ($('#joborderno').val() == null)) {
                $('#joborderno').focus();
                return;
            }

            if (($('#layno').val() == "") || ($('#layno').val() == null)) {
                $('#layno').focus();
                return;
            }
            if ((oldjo == $('#joborderno').val()) || (oldlayno == $('#layno').val())) {

                return;
            }

            oldjo = $('#joborderno').val();
            oldlayno = $('#layno').val();

            $('#layno').removeAttr("disabled").selectmenu('refresh');
            $('#bundle').removeAttr("disabled").selectmenu('refresh');
            $('#bundle').empty();
            bundleselecttran();


            $.ajax({
                type: "POST",
                url: "Printbarcode.aspx/JobordernoBundle",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'joborderno': '" + $('#joborderno').val() + "', 'layno': '" + $('#layno').val() + "' }",
                success: function (data) {
                    $.mobile.loading('hide');
                    var result = eval(data.d);

                    bundleselecttran();
                    $('#bundle').append(result[0].BUNDLE);
                    $('#bundle').selectmenu('refresh');
                },
                error: function (err) {
                    $.mobile.loading('hide');
                    alert(globeltranslate.errormessage + ":013");
                    oldjo = "";
                    oldlayno = "";
                }
            });

        }

        $(function () {
            fty = getUrlParam('FTY');
            environment = getUrlParam('svTYPE');
            if (fty == null || environment == null) {
                alert('The url is wrong! Pls input like 192.168.x.x/CIPMS/Login.aspx?FTY=GEG&svTYPE=PROD');
            } else {

                //调用语言翻译函数
                if (window.localStorage.languageid == "1") {
                    window.language = "Printbarcode/Printbarcode-zh-CN.js";
                } else if (window.localStorage.languageid == "2") {
                    window.language = "Printbarcode/Printbarcode-en-US.js";
                }
                translate();

                //用户登录之后才能进入系统模块
                if (sessionStorage.getItem("name") == null || sessionStorage.getItem("name") == "") {
                    Loginout(fty, environment);
                }
                else {
                    //左侧栏
                    $("#my-menu").mmenu({
                        "footer": {
                            "add": true,
                            "title": globeltranslate.footertitle
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

                    $('#joborderno').focus();
                    laynoselecttran();
                    bundleselecttran();
                    //function权限控制
                    Accessmodulehide();
                    $('#messagetitle').html(sessionStorage.getItem("name") + " , " + sessionStorage.getItem("factory") + "/" + sessionStorage.getItem("process") + "/" + sessionStorage.getItem("productionline"));
                    var userbarcode = sessionStorage.getItem("userbarcode");
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

                    $('#queryjobutton').click(function () {
                        oldlayno = "";
                        oldjo = "";
                        if ($('#joborderno').val() == "") {
                            $('#joborderno').focus();
                        }
                        else {
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            $.ajax({
                                type: "POST",
                                url: "Printbarcode.aspx/Joborderno",
                                contentType: "application/json;charset=utf-8",
                                dataType: "json",
                                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'joborderno': '" + $('#joborderno').val() + "' }",
                                success: function (data) {
                                    $.mobile.loading('hide');
                                    var result = eval(data.d);
                                    if (result[0].PROCESS == "N") {
                                        alert(globeltranslate.processnotopen); //提示MES系统没有开工序
                                    } else {
                                        laynoselecttran();
                                        bundleselecttran();
                                        $('#layno').append(result[0].LAYNO);
                                        $('#layno').selectmenu('refresh');
                                        $("input[name='part']").remove();
                                        $("label[name='part']").remove();
                                        $('#partgroup').append(result[0].PART);
                                        $("#partgroup").trigger("create");
                                        //added on 2018-02-26--start
                                        $("input[name='printpart']").remove();
                                        $("label[name='printpart']").remove();
                                        $('#isprintpartgroup').append(result[0].PRINTPART); 
                                        $("#isprintpartgroup").trigger("create");
                                        //added on 2018-02-26--end
                                    }
                                },
                                error: function (err) {
                                    $.mobile.loading('hide');
                                    alert(globeltranslate.errormessage + ":013");
                                }
                            });
                        }
                    });

                    //JO输入框回车按钮事件
                    $('#joborderno').bind('keypress', function (event) {
                        oldlayno = "";
                        oldjo = "";
                        if (event.keyCode == '13') {
                            if ($('#joborderno').val() == "") {
                                $('#joborderno').focus();
                            }
                            else {
                                $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                $.ajax({
                                    type: "POST",
                                    url: "Printbarcode.aspx/Joborderno",
                                    contentType: "application/json;charset=utf-8",
                                    dataType: "json",
                                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'joborderno': '" + $('#joborderno').val() + "' }",
                                    success: function (data) {
              
                                        $.mobile.loading('hide');
                                        var result = eval(data.d);
                                        if (result[0].PROCESS == "N") {
                                            alert(globeltranslate.processnotopen); //提示MES系统没有开工序
                                        } else {
                                            laynoselecttran();
                                            bundleselecttran();
                                            $('#layno').append(result[0].LAYNO);
                                            $('#layno').selectmenu('refresh');
                                            $("input[name='part']").remove();
                                            $("label[name='part']").remove();
                                            $('#partgroup').append(result[0].PART);
                                            $("#partgroup").trigger("create");
                                            //added on 2018-02-26--start
                                            $("input[name='printpart']").remove();
                                            $("label[name='printpart']").remove();
                                            $('#isprintpartgroup').append(result[0].PRINTPART); 
                                            $("#isprintpartgroup").trigger("create");
                                            //added on 2018-02-26--end
                                        }



                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                        alert(globeltranslate.errormessage + ":013");
                                    }
                                });
                            }
                        }
                    });

                    //注销按钮：返回登录页面
                    $('#cancelbutton').click(function () {
                        Loginout(fty, environment);
                    });
                    //打印按钮
                    $('#printbutton').click(function () {
                        var partselected = "";
                        var partcdselected = "";
                        var unpartseelcted = "";
                        var partnum = 0;
                        var laynoselected = "";

                        laynoselected = $('#layno').val();
                        if (laynoselected == null) {
                            alert(globeltranslate.confirmmessage1);
                            return;
                        }
                        $("input[name='part']:checked").each(function (i) {
                            if (i == 0) {
                                partselected += $(this).prev().text();
                                partcdselected += $(this).val();
                            }
                            else {
                                partselected += "/" + $(this).prev().text();
                                partcdselected += "," + $(this).val();
                            }
                            partnum++;
                        });
                        $("input[name='part']").not("input:checked").each(function (i) {
                            if (i == 0)
                                unpartseelcted += $(this).prev().text();
                            else
                                unpartseelcted += "/" + $(this).prev().text();
                        });
                        //added on 2018-02-26--start
                        var printpartselected = "";
                        var printpartcdselected = "";
                        var printpartnum = 0;
                        $("input[name='printpart']:checked").each(function (i) {
                            if (i == 0) {
                                printpartselected += $(this).prev().text();
                                printpartcdselected += $(this).val();
                            }
                            else {
                                printpartselected += "/" + $(this).prev().text();
                                printpartcdselected += "," + $(this).val();
                            }
                            printpartnum++;
                        });
                        //added on 2018-02-26--end
                        var filename = "";
                        if ($('#layno').val() != laynoselect && $('#joborderno').val() != '') {
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            $.ajax({
                                type: "POST",
                                url: "Printbarcode.aspx/Print",
                                contentType: "application/json;charset=utf-8",
                                //async: false,
                                dataType: "json",
                                data: "{ 'process': '" + sessionStorage.getItem("process") + "', 'userbarcode': '" + userbarcode + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'joborderno': '" + $('#joborderno').val() + "', 'lay_no': '" + laynoselected + "' ,'part': '" + unpartseelcted + "', 'combinepart':'" + partselected + "', 'combinepartcd':'" + partcdselected + "', 'partnum': '" + partnum.toString() + "','printpart': '" + printpartselected + "','printpartcd': '" + printpartcdselected + "', 'printpartnum': '" + printpartnum.toString() + "', 'bundle': '" + $('#bundle').val() + "' }",
                                success: function (data) {
                                    $.mobile.loading('hide');
                                    //var result = eval(data.d);
                                    //var result = eval('(' + data + ')');
                                    var result = $.parseJSON(data.d);
                                    //alert(JSON.stringify(result.BUNDLEINFO));
                                    if (result.OTHER[0].FAILLAYNO == laynoselected) {//全部打印失败
                                        alert(globeltranslate.confirmmessage4 + globeltranslate.confirmmessage5 + result.OTHER[0].FAILREASON);
                                    }
                                    else {
                                        if (result.OTHER[0].FAILLAYNO == 'null') {//全部打印成功
                                            alert(globeltranslate.confirmmessage2);
                                        }
                                        else {//部分打印成功
                                            alert(globeltranslate.confirmmessage3 + globeltranslate.confirmmessage5);
                                        }
                                        $('#myprinttest').empty();

                                        $.each(result.BUNDLEINFO, function (i, o) {
                                            //var html = "<div class='ui-grid-a' style='height:3.5cm'><div class='ui-block-a'><table style='font-size:10px;line-height:1.4;'><tr><td style='font-size:15px; font-weight:bolder;'>" + o.BUNDLE_BARCODE + "</td></tr><tr><td>客户:" + o.CLIENT + "</td></tr><tr><td>制单:" + o.JOB_ORDER_NO + "</td></tr></table><div class='ui-grid-1'><div class='ui-block-a'><table style='font-size:10px;line-height:1.4;'><tr><td>颜色:" + o.COLOR + "</td></tr><tr><td>床号:" + o.LAY_NO + "</td></tr><tr><td>扎号:</td></tr></table></div><div class='ui-block-b'><div style='font-size:55px;line-height:1.4cm;ltext-align:left'>" + o.BUNDLE_NO + "</div></div></div></div><div class='ui-block-b'><table style='font-size:10px;line-height:1.2;margin-left:0.7cm;'><tr><td>" + o.DATE + "</td></tr><tr><td>制单数:" + o.JOB_ORDER_NO + "</td></tr><tr><td>缸号:" + o.BATCH_NO + "</td></tr><tr><td>幅位:" + o.PART + "</td></tr><tr><td>唛架:" + o.MARKERS + "</td></tr><tr><td>款式:" + o.STYLE_NO + "</td></tr><tr><td>组别:" + o.PRODUCTION_LINE + "</td></tr></table></div></div></div></div><div class='ui-grid-3' style='height:1.580cm'><div class='ui-block-a'><table style='font-size:10px;line-height:1.1;'><tr><td>尺码:" + o.SIZE + "</td></tr><tr><td>数量:" + o.QTY + "</td></tr></table></div><div class='ui-block-b'><div id='test" + i.toString() + "'></div></div></div>";
                                            //$('#myprinttest').append(html);
                                            //$("#test" + i.toString()).barcode(o.BUNDLE_BARCODE, "code128", { barWidth: 2, barHeight: 40, showHRI: false });
                                            //var html = "<div style='height:3.9cm;line-height:1.4;'><table style='width:100%'><tr style='width:100%'><td width='65%'><table style='width:100%'><tr style='width:100%'><td width='50%'><div style='font-size:15px; font-weight:bolder;line-height:1.4;'>" + o.BUNDLE_BARCODE + "</div></td><td width='50%'><div style='font-size:11px;line-height:1.4;'>" + o.DATE + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:11px;line-height:1.4;'>客户:" + o.CLIENT + "</div></td><td width='50%'><div style='font-size:11px;line-height:1.4;'>款式:" + o.STYLE_NO + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:11px;line-height:1.4;'>制单:" + o.JOB_ORDER_NO + "</div></td><td width='50%'><div style='font-size:11px;line-height:1.4;'>制单数:" + o.CUT_QTY + "</div></td></tr></table><table style='width:100%'><tr style='width:100%'><td width='33%'><table><tr><td><div style='font-size:11px;line-height:1.4;'>颜色:" + o.COLOR + "</div></td></tr><tr><td><div style='font-size:11px;line-height:1.4;'>床次:" + o.LAY_NO + "</div></td></tr><tr><td><div style='font-size:11px;line-height:1.4;'>尺码:" + o.SIZE + "</div></td></tr><tr><td><div style='font-size:11px;line-height:1.4;'>数量:" + o.QTY + "</div></td></tr></table></td><td width='33%'><table><tr><td><div style='font-size:11px;line-height:1.4;'>扎号:</div></td></tr><tr><td><div style='font-size:50px;line-height:1;text-align:left'>" + o.BUNDLE_NO + "</div></td></tr></table></td><td width='34%'></td></tr></table></td><td width='35%'><div id='bundleno" + i.toString() + "'></div></td></tr></table></div><div style='height:1.180cm;line-height:1.4;'><table style='width:100%'><tr style='width:100%'><td width='20%'><div style='font-size:11px;line-height:1.4;'>组别:" + o.PRODUCTION_LINE + "</div></td><td width='80%'><div style='font-size:11px;line-height:1.4;'>幅位:" + o.PART + "</div></td></tr></table><table style='width:100%; margin-top: -3px;'><tr style='width:100%'><td width='20%'><div style='font-size:11px;line-height:1.4;'>唛架:" + o.MARKERS + "</div></td><td width='20%'><div style='font-size:11px;line-height:1.4;'>纸样:" + o.PATTERN_NO + "</div></td><td width='20%'><div style='font-size:11px;line-height:1.4;'>色级:" + o.SHADE_LOT + "</div></td><td width='40%'><div style='font-size:11px;line-height:1.4;'>缸号:" + o.BATCH_NO + "</div></td></tr></table></div>";
                                            // var html = "<div style='height:3.9cm;line-height:1.4;'><table style='width:100%'><tr style='width:100%'><td width='65%'><table style='width:100%'><tr style='width:100%'><td width='50%'><div style='font-size:15px; font-weight:bolder;line-height:1.4;'>" + o.BUNDLE_BARCODE + "</div></td><td width='50%'><div style='font-size:11px;line-height:1.4;'>" + o.DATE + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:11px;line-height:1.4;'>客户:" + o.CLIENT + "</div></td><td width='50%'><div style='font-size:11px;line-height:1.4;'>款式:" + o.STYLE_NO + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:11px;line-height:1.4;'>制单:" + o.JOB_ORDER_NO + "</div></td><td width='50%'><div style='font-size:11px;line-height:1.4;'>制单数:" + o.CUT_QTY + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:11px;line-height:1.4;'>颜色:" + o.COLOR + "</div></td><td width='50%'><div style='font-size:11px;line-height:1.4;'>扎号:</div></td></tr></table><table style='width:100%'><tr style='width:100%'><td width='33%'><table style='margin-left:-2px'><tr><td><div style='font-size:11px;line-height:1.4;'>床次:" + o.LAY_NO + "</div></td></tr><tr><td><div style='font-size:11px;line-height:1.4;'>尺码:" + o.SIZE + "</div></td></tr><tr><td><div style='font-size:11px;line-height:1.4;'>数量:" + o.QTY + "</div></td></tr></table></td><td width='33%'><table><tr><td><div style='font-size:50px;line-height:1;text-align:left'>" + o.BUNDLE_NO + "</div></td></tr></table></td><td width='34%'></td></tr></table></td><td width='35%'><div id='bundleno" + i.toString() + "'></div></td></tr></table></div><div style='height:1.180cm;line-height:1.4;'><table style='width:100%'><tr style='width:100%'><td width='20%'><div style='font-size:11px;line-height:1.4;'>组别:" + o.PRODUCTION_LINE + "</div></td><td width='80%'><div style='font-size:11px;line-height:1.4;'>幅位:" + o.PART + "</div></td></tr></table><table style='width:100%; margin-top: -6px;'><tr style='width:100%'><td width='20%'><div style='font-size:11px;line-height:1.4;'>唛架:" + o.MARKERS + "</div></td><td width='20%'><div style='font-size:11px;line-height:1.4;'>纸样:" + o.PATTERN_NO + "</div></td><td width='20%'><div style='font-size:11px;line-height:1.4;'>色级:" + o.SHADE_LOT + "</div></td><td width='40%'><div style='font-size:11px;line-height:1.4;'>缸号:" + o.BATCH_NO + "</div></td></tr></table></div>";
                                            //20161213 //var html = "<div style='height:3.9cm;line-height:1.4;'><table style='width:100%'><tr style='width:100%'><td width='65%'><table style='width:100%'><tr style='width:100%'><td width='50%'><div style='font-size:15px; font-weight:bolder;line-height:1.4;'>" + o.BUNDLE_BARCODE + "</div></td><td width='50%'><div style='font-size:11px;line-height:1.4;'>" + o.DATE + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:11px;line-height:1.4;'>客户:" + o.CLIENT + "</div></td><td width='50%'><div style='font-size:11px;line-height:1.4;'>款式:" + o.STYLE_NO + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:11px;line-height:1.4;'>制单:" + o.JOB_ORDER_NO + "</div></td><td width='50%'><div style='font-size:11px;line-height:1.4;'>制单数:" + o.CUT_QTY + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:11px;line-height:1.4;'>颜色:" + o.COLOR + "</div></td><td width='50%'><div style='font-size:11px;line-height:1.4;'>扎号:</div></td></tr></table><table style='width:100%'><tr style='width:100%'><td width='33%'><table style='margin-left:-2px'><tr><td><div style='font-size:11px;line-height:1.4;'>床次:" + o.LAY_NO + "</div></td></tr><tr><td><div style='font-size:11px;line-height:1.4;'>尺码:" + o.SIZE + "</div></td></tr><tr><td><div style='font-size:11px;line-height:1.4;'>数量:" + o.QTY + "</div></td></tr></table></td><td width='33%'><table><tr><td><div style='font-size:50px;line-height:1;text-align:left'>" + o.BUNDLE_NO + "</div></td></tr></table></td><td width='34%'></td></tr></table></td><td width='35%'><div id='bundleno" + i.toString() + "'></div></td></tr></table></div><div style='height:1.180cm;line-height:1.4;'><table style='width:100%'><tr style='width:100%'><td width='20%'><div style='font-size:11px;line-height:1.4;'>组别:" + o.PRODUCTION_LINE + "</div></td><td width='80%'><div style='font-size:11px;line-height:1.4;'>幅位:" + o.PART + "</div></td></tr></table><table style='width:100%; margin-top: -6px;'><tr style='width:100%'><td width='20%'><div style='font-size:11px;line-height:1.4;'>唛架:" + o.MARKERS + "</div></td><td width='20%'><div style='font-size:11px;line-height:1.4;'>纸样:" + o.PATTERN_NO + "</div></td><td width='20%'><div style='font-size:11px;line-height:1.4;'>色级:" + o.SHADE_LOT + "</div></td><td width='40%'><div style='font-size:11px;line-height:1.4;'>缸号:" + o.BATCH_NO + "</div></td></tr></table></div>";
                                            //20161213调大尺码和数量数据的字体
                                            //var html = "<div style='height:3.9cm;line-height:1.4;'><table style='width:100%'><tr style='width:100%'><td width='65%'><table style='width:100%'><tr style='width:100%'><td width='50%'><div style='font-size:15px; font-weight:bolder;line-height:1.4;'>" + o.BUNDLE_BARCODE + "</div></td><td width='50%'><div style='font-size:11px;line-height:1.4;'>" + o.DATE + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:11px;line-height:1.4;'>客户:" + o.CLIENT + "</div></td><td width='50%'><div style='font-size:11px;line-height:1.4;'>款式:" + o.STYLE_NO + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:11px;line-height:1.4;'>制单:" + o.JOB_ORDER_NO + "</div></td><td width='50%'><div style='font-size:11px;line-height:1.4;'>制单数:" + o.CUT_QTY + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:11px;line-height:1.4;'>颜色:" + o.COLOR + "</div></td><td width='50%'><div style='font-size:11px;line-height:1.4;'>扎号:</div></td></tr></table><table style='width:100%'><tr style='width:100%'><td width='33%'><table style='margin-left:-4px; margin-top: -5px;'><tr><td><div style='font-size:11px;line-height:1.4;'>床次:" + o.LAY_NO + "</div></td></tr><tr><td><div style='font-size:11px;line-height:1.4;'>尺码:<span style='font-size:13px;line-height:1.4;'>" + o.SIZE + "</span></div></td></tr><tr><td><div style='font-size:11px;line-height:1.4;'>数量:<span style='font-size:13px;line-height:1.4;'>" + o.QTY + "</span></div></td></tr></table></td><td width='33%'><table><tr><td><div style='font-size:50px;line-height:1;text-align:left'>" + o.BUNDLE_NO + "</div></td></tr></table></td><td width='34%'></td></tr></table></td><td width='35%'><div id='bundleno" + i.toString() + "'></div></td></tr></table></div><div style='height:1.180cm;line-height:1.4;'><table style='width:100%'><tr style='width:100%'><td width='20%'><div style='font-size:11px;line-height:1.4;'>组别:" + o.PRODUCTION_LINE + "</div></td><td width='80%'><div style='font-size:11px;line-height:1.4;'>幅位:" + o.PART + "</div></td></tr></table><table style='width:100%; margin-top: -6px;'><tr style='width:100%'><td width='20%'><div style='font-size:11px;line-height:1.4;'>唛架:" + o.MARKERS + "</div></td><td width='20%'><div style='font-size:11px;line-height:1.4;'>纸样:" + o.PATTERN_NO + "</div></td><td width='20%'><div style='font-size:11px;line-height:1.4;'>色级:" + o.SHADE_LOT + "</div></td><td width='40%'><div style='font-size:11px;line-height:1.4;'>缸号:" + o.BATCH_NO + "</div></td></tr></table></div>";
                                            fty_cd = getUrlParam('FTY');
                                            var print_part_flag = "";
                                            if (o.PRINTPART_FLAG == "Y") {
                                                print_part_flag = "(印花)";
                                            }
                                            var html;

                                            //if (fty_cd.toUpperCase() == "GEG") {
                                            //    //html = "<div style='height:3.9cm;line-height:1.3;'><table style='width:100%'><tr style='width:100%'><td width='65%'><table style='width:100%'><tr style='width:100%'><td width='50%'><div style='font-size:15px; font-weight:bolder;line-height:1.3;'>" + o.BUNDLE_BARCODE + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.3;'>" + o.DATE + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:13px;line-height:1.3;'>客户:" + o.CLIENT + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.3;'>款式:" + o.STYLE_NO + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:13px;line-height:1.3;'>制单:" + o.JOB_ORDER_NO + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.3;'>制单数:" + o.CUT_QTY + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:13px;line-height:1.3;'>颜色:" + o.COLOR + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.3;'>扎号:</div></td></tr></table><table style='width:100%'><tr style='width:100%'><td width='33%'><table style='margin-left:-4px; margin-top: -5px;'><tr><td><div style='font-size:13px;line-height:1.3;'>床次:" + o.LAY_NO + "</div></td></tr><tr><td><div style='font-size:15px;line-height:1.3;'>尺码:<span style='font-size:15px;line-height:1.3;'>" + o.SIZE + "</span></div></td></tr><tr><td><div style='font-size:13px;line-height:1.3;'>数量:<span style='font-size:13px;line-height:1.3;'>" + o.QTY + "</span></div></td></tr></table></td><td width='33%'><table><tr><td><div style='font-size:50px;line-height:1;text-align:left'>" + o.BUNDLE_NO + "</div></td></tr></table></td><td width='34%'></td></tr></table></td><td width='35%'><div id='bundleno" + i.toString() + "'></div></td></tr></table></div><div style='height:1.180cm;line-height:1.3;'><table style='width:100%'><tr style='width:100%'><td width='20%'><div style='font-size:13px;line-height:1.3;'>组别:" + o.PRODUCTION_LINE + "</div></td><td width='80%'><div style='font-size:13px;line-height:1.3;'>幅位:" + o.PART + "</div></td></tr></table>  <table style='width:100%; margin-top: -6px;'><tr style='width:100%'><td width='10%'><div style='font-size:13px;line-height:1.3;'>唛架:" + o.MARKERS + "</div></td><td width='10%'><div style='font-size:13px;line-height:1.3;'>纸样:" + o.PATTERN_NO + "</div></td><td width='10%'><div style='font-size:13px;line-height:1.3;'>色级:" + o.SHADE_LOT + "</div></td><td width='70%'><div style='font-size:13px;line-height:1.3;'>缸号:" + o.BATCH_NO + "</div></td></tr></table></div>";
                                            //    html = "<div style='height:3.9cm;line-height:1.2;'><table style='width:100%'><tr style='width:100%'><td width='65%'><table style='width:100%'><tr style='width:100%'><td width='50%'><div style='font-size:15px; font-weight:bolder;line-height:1.2;'>" + o.BUNDLE_BARCODE + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.2;'>" + o.DATE + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:13px;line-height:1.2;'>客户:" + o.CLIENT + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.2;'>款式:" + o.STYLE_NO + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:14px;line-height:1.2;'>制单:" + o.JOB_ORDER_NO + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.2;'>制单数:" + o.CUT_QTY + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:13px;line-height:1.2;'>颜色:" + o.COLOR + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.2;'>扎号:</div></td></tr></table>  <table style='width:100%'><tr style='width:100%'><td width='50%'><table style='margin-left:-4px; margin-top: -5px;'><tr><td><div style='font-size:13px;line-height:1.2;'>床次:" + o.LAY_NO + "</div></td></tr><tr><td><div style='font-size:15px;line-height:1.2;'>尺码:<span style='font-size:20px;line-height:1.2;'>" + o.SIZE + "</span></div></td></tr><tr><td><div style='font-size:13px;line-height:1.2;'>数量:<span style='font-size:13px;line-height:1.2;'>" + o.QTY + "</span></div></td></tr></table></td><td width='50%'><table><tr><td><div style='font-size:50px;line-height:1;text-align:left'>" + o.BUNDLE_NO + "</div></td></tr></table></td><td width='34%'></td></tr></table></td><td width='35%'><div id='bundleno" + i.toString() + "'></div></td></tr></table></div><div style='height:1.180cm;line-height:1.2;'><table style='width:100%'><tr style='width:100%'><td width='20%'><div style='font-size:13px;line-height:1.2;'>组别:" + o.PRODUCTION_LINE + "</div></td><td width='80%'><div style='font-size:13px;line-height:1.2;'>幅位:" + o.PART + print_part_flag + "</div></td></tr></table>  <table style='width:100%; margin-top: -6px;'><tr style='width:100%'><td width='11%'><div style='font-size:13px;line-height:1.2;'>唛架" + o.MARKERS + "</div></td><td width='11%'><div style='font-size:13px;line-height:1.2;'>纸样" + o.PATTERN_NO + "</div></td><td width='11%'><div style='font-size:13px;line-height:1.2;'>色级" + o.SHADE_LOT + "</div></td><td width='67%'><div style='font-size:13px;line-height:1.2;'>缸号:" + o.BATCH_NO + "</div></td></tr></table></div>";
                                            //} else {
                                            //    html = "<div style='height:3.9cm;line-height:1.3;'><table style='width:100%'><tr style='width:100%'><td width='65%'><table style='width:100%'><tr style='width:100%'><td width='50%'><div style='font-size:15px; font-weight:bolder;line-height:1.3;'>" + o.BUNDLE_BARCODE + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.3;'>" + o.DATE + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:13px;line-height:1.3;'>客户:" + o.CLIENT + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.3;'>款式:" + o.STYLE_NO + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:13px;line-height:1.3;'>制单:" + o.JOB_ORDER_NO + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.3;'>制单数:" + o.CUT_QTY + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:13px;line-height:1.3;'>颜色:" + o.COLOR + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.3;'>扎号:</div></td></tr></table><table style='width:100%'><tr style='width:100%'><td width='33%'><table style='margin-left:-4px; margin-top: -5px;'><tr><td><div style='font-size:13px;line-height:1.3;'>床次:" + o.LAY_NO + "</div></td></tr><tr><td><div style='font-size:13px;line-height:1.3;'>尺码:<span style='font-size:13px;line-height:1.3;'>" + o.SIZE + "</span></div></td></tr><tr><td><div style='font-size:13px;line-height:1.3;'>数量:<span style='font-size:13px;line-height:1.3;'>" + o.QTY + "</span></div></td></tr></table></td><td width='33%'><table><tr><td><div style='font-size:50px;line-height:1;text-align:left'>" + o.BUNDLE_NO + "</div></td></tr></table></td><td width='34%'></td></tr></table></td><td width='35%'><div id='bundleno" + i.toString() + "'></div></td></tr></table></div><div style='height:1.180cm;line-height:1.3;'><table style='width:100%'><tr style='width:100%'><td width='20%'><div style='font-size:13px;line-height:1.3;'>组别:" + o.PRODUCTION_LINE + "</div></td><td width='80%'><div style='font-size:13px;line-height:1.3;'>幅位:" + o.PART + print_part_flag + "</div></td></tr></table>  <table style='width:100%; margin-top: -8px;'><tr style='width:100%'><td width='20%'><div style='font-size:13px;line-height:1.3;'>唛架:" + o.MARKERS + "</div></td><td width='20%'><div style='font-size:13px;line-height:1.3;'>纸样" + o.PATTERN_NO + "</div></td><td width='20%'><div style='font-size:13px;line-height:1.3;'>色级:" + o.SHADE_LOT + "</div></td><td width='40%'><div style='font-size:13px;line-height:1.3;'>缸号:" + o.BATCH_NO + "</div></td></tr></table></div>";
                                            //}

                                            if (fty_cd.toUpperCase() == "GEG") {
                                                //html = "<div style='height:3.9cm;line-height:1.2;'><table style='width:100%'><tr style='width:100%'><td width='65%'><table style='width:100%'><tr style='width:100%'><td width='50%'><div style='font-size:15px; font-weight:bolder;line-height:1.2;'>" + o.BUNDLE_BARCODE + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.2;'>" + o.DATE + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:13px;line-height:1.2;'>客户:" + o.CLIENT + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.2;'>款式:" + o.STYLE_NO + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:14px;line-height:1.2;'>制单:" + o.JOB_ORDER_NO + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.2;'>制单数:" + o.CUT_QTY + "</div></td></tr><tr style='width:100%'><td width='35%'><div style='font-size:13px;line-height:1.2;'>颜色:" + o.COLOR + "</div></td><td width='75%'><div style='font-size:13px;line-height:1.2;'>扎号:</div></td></tr></table>  <table style='width:100%'><tr style='width:100%'><td width='35%'><table style='margin-left:-4px; margin-top: -5px;'><tr><td><div style='font-size:13px;line-height:1.2;'>床次:" + o.LAY_NO + "</div></td></tr><tr><td><div style='font-size:15px;line-height:1.2;'>尺码:<span style='font-size:20px;line-height:1.2;'>" + o.SIZE + "</span></div></td></tr><tr><td><div style='font-size:13px;line-height:1.2;'>数量:<span style='font-size:13px;line-height:1.2;'>" + o.QTY + "</span></div></td></tr></table></td><td width='75%'><table><tr><td><div style='font-size:50px;line-height:1;text-align:left'>" + o.BUNDLE_NO + "</div></td></tr></table></td><td width='34%'></td></tr></table></td><td width='35%'><div id='bundleno" + i.toString() + "'></div></td></tr></table></div><div style='height:1.180cm;line-height:1.2;'><table style='width:100%'><tr style='width:100%'><td width='20%'><div style='font-size:13px;line-height:1.2;'>组别:" + o.PRODUCTION_LINE + "</div></td><td width='80%'><div style='font-size:13px;line-height:1.2;'>幅位:" + o.PART + "</div></td></tr></table>  <table style='width:100%; margin-top: -6px;'><tr style='width:100%'><td width='11%'><div style='font-size:13px;line-height:1.2;'>唛架" + o.MARKERS + "</div></td><td width='11%'><div style='font-size:13px;line-height:1.2;'>纸样" + o.PATTERN_NO + "</div></td><td width='11%'><div style='font-size:13px;line-height:1.2;'>色级" + o.SHADE_LOT + "</div></td><td width='67%'><div style='font-size:13px;line-height:1.2;'>缸号:" + o.BATCH_NO + "</div></td></tr></table></div>";
                                                //tangyh 20170921
                                                html = Print_GEG_HTML(i, o, "");
                                            } else {

                                                //html = "<div style='height:3.9cm;line-height:1.3;'><table style='width:100%'><tr style='width:100%'><td width='65%'><table style='width:100%'><tr style='width:100%'><td width='50%'><div style='font-size:15px; font-weight:bolder;line-height:1.3;'>" + o.BUNDLE_BARCODE + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.3;'>" + o.DATE + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:13px;line-height:1.3;'>客户:" + o.CLIENT + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.3;'>款式:" + o.STYLE_NO + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:15px;line-height:1.3;'>制单:" + o.JOB_ORDER_NO + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.3;'>制单数:" + o.CUT_QTY + "</div></td></tr><tr style='width:100%'><td width='50%'><div style='font-size:13px;line-height:1.3;'>颜色:" + o.COLOR + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.3;'>扎号:</div></td></tr></table><table style='width:100%'><tr style='width:100%'><td width='33%'><table style='margin-left:-4px; margin-top: -5px;'><tr><td><div style='font-size:13px;line-height:1.3;'>床次:" + o.LAY_NO + "</div></td></tr><tr><td><div style='font-size:13px;line-height:1.3;'>尺码:<span style='font-size:13px;line-height:1.3;'>" + o.SIZE + "</span></div></td></tr><tr><td><div style='font-size:13px;line-height:1.3;'>数量:<span style='font-size:13px;line-height:1.3;'>" + o.QTY + "</span></div></td></tr></table></td><td width='33%'><table><tr><td><div style='font-size:50px;line-height:1;text-align:left'>" + o.BUNDLE_NO + "</div></td></tr></table></td><td width='34%'></td></tr></table></td><td width='35%'><div id='bundleno" + i.toString() + "'></div></td></tr></table></div><div style='height:1.180cm;line-height:1.3;'><table style='width:100%'><tr style='width:100%'><td width='35%'><div style='font-size:13px;line-height:1.3;'>组别:" + o.PRODUCTION_LINE + "</div></td><td width='65%'><div style='font-size:13px;line-height:1.3;'>幅位:" + o.PART + "</div></td></tr></table>  <table style='width:100%; margin-top: -8px;'><tr style='width:100%'><td width='20%'><div style='font-size:13px;line-height:1.3;'>唛架:" + o.MARKERS + "</div></td><td width='20%'><div style='font-size:13px;line-height:1.3;'>纸样" + o.PATTERN_NO + "</div></td><td width='20%'><div style='font-size:13px;line-height:1.3;'>色级:" + o.SHADE_LOT + "</div></td><td width='40%'><div style='font-size:13px;line-height:1.3;'>缸号:" + o.BATCH_NO + "</div></td></tr></table></div>";
                                                //tangyh 20170921
                                                html = Print_YMG_HTML(i, o, "");
                                            }


                                            $('#myprinttest').append(html);
                                            var qrcode = new QRCode("bundleno" + i, {
                                                width: 135,
                                                height: 135,
                                                text: o.BUNDLE_BARCODE
                                            });
                                        });
                                        setTimeout(function () {
                                            //打印div标签里的内容
                                            $('#myprinttest').jqprint();
                                        }, 2000);

                                        //                                        for (var i = i; i < result.BUNDLEINFO.count; i++) {
                                        //                                            alert(result.BUNDLEINFO[i].PART);
                                        //                                        }

                                        //                                        var laynoarray = new Array();
                                        //                                        var bundlearray = new Array();
                                        //                                        laynoarray = result[0].HTML.split('@@');
                                        //                                        for (var i = 0; i < laynoarray.length; i++) {
                                        //                                            bundlearray = laynoarray[i].split('*****');
                                        //                                            for (var j = 0; j < bundlearray.length; j++) {
                                        //                                                var temp = new Array();
                                        //                                                temp = bundlearray[j].split('|');
                                        //                                                var html = "<div class='ui-grid-a' style='height:3.5cm'><div class='ui-block-a'><table style='font-size:10px;line-height:1.4;'><tr><td style='font-size:15px; font-weight:bolder;'>" + temp[0] + "</td></tr><tr><td>客户:" + temp[1] + "</td></tr><tr><td>制单:" + temp[2] + "</td></tr></table><div class='ui-grid-1'><div class='ui-block-a'><table style='font-size:10px;line-height:1.4;'><tr><td>颜色:" + temp[3] + "</td></tr><tr><td>床号:" + temp[4] + "</td></tr><tr><td>扎号:</td></tr></table></div><div class='ui-block-b'><div style='font-size:55px;line-height:1.4cm;ltext-align:left'>" + temp[5] + "</div></div></div></div><div class='ui-block-b'><table style='font-size:10px;line-height:1.2;margin-left:0.7cm;'><tr><td>" + temp[6] + "</td></tr><tr><td>制单数:" + temp[7] + "</td></tr><tr><td>缸号:" + temp[8] + "</td></tr><tr><td>幅位:" + temp[9] + "</td></tr><tr><td>唛架:" + temp[10] + "</td></tr><tr><td>款式:" + temp[11] + "</td></tr><tr><td>组别:" + temp[12] + "</td></tr></table></div></div></div></div><div class='ui-grid-3' style='height:1.580cm'><div class='ui-block-a'><table style='font-size:10px;line-height:1.1;'><tr><td>尺码:" + temp[13] + "</td></tr><tr><td>数量:" + temp[14] + "</td></tr></table></div><div class='ui-block-b'><div id='test" + i.toString() + j.toString() + "'></div></div></div>";
                                        //                                                $('#myprinttest').append(html);
                                        //                                                $("#test" + i.toString() + j.toString()).barcode(temp[0], "code128", { barWidth: 2, barHeight: 40, showHRI: false });
                                        //                                            }
                                        //                                        }
                                        //                                        setTimeout(function () {
                                        //                                            //打印div标签里的内容
                                        //                                            $('#myprinttest').jqprint();
                                        //                                        }, 2000);


                                        laynoselecttran();
                                        bundleselecttran();
                                        $('#joborderno').val('');
                                        $('#joborderno').focus();
                                        $("input[name='part']").remove();
                                        $("label[name='part']").remove();
                                        $("#partgroup").trigger("create");
                                        //added on 2018-02-26--start
                                        $("input[name='printpart']").remove();
                                        $("label[name='printpart']").remove();
                                        $("#isprintpartgroup").trigger("create");
                                        //added on 2018-02-26--end
                                        $('#joborderno').focus();
                                    }
                                },
                                error: function (err) {
                                    $.mobile.loading('hide');
                                    alert(globeltranslate.errormessage + "033");
                                }
                            });
                        } else {
                            alert(globeltranslate.confirmmessage1);
                        }
                    });


                    $('#layno').change(function () {
                        oldlayno = "";
                        oldjo = "";
                        //add on 2018-02-26
                        loadselectPrintpart($('#joborderno').val(), $('#layno').val());
                    });

                }
            }
        });

        function loadselectPrintpart(jo, layno) {
            if(jo !="" && layno !=""){
                $.ajax({
                    type: "POST",
                    url: "Printbarcode.aspx/printpartselect",
                    contentType: "application/json;charset=utf-8",
                    dataType: "json",
                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'jo': '" + jo + "' ,'layno': '" + layno + "'}",
                    success: function (data) {
                        var result = eval(data.d);
                        $("input[name='printpart']").each(function (i) {
                            if (result[0].PRINT_PART.indexOf($(this).val()) > -1) { $(this).attr("checked", true).checkboxradio("refresh"); }
                        });

                    },
                    error: function (err) {
                        alert(globeltranslate.errormessage + ":013");
                    }
                });
            }
        }
    </script>

    <div data-role="header" data-theme="b" >
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
            <div data-role="navbar" id="navbar">
                <div data-role="navbar">
                    <table width="100%">
                        <tr>
                            <td style="width:10%">
                                <ul>
                                    <li>
                                        <input type="button" style="text-align:center" id="queryjobutton" name="queryjobutton" />
                                    </li>
                                </ul>
                            </td>
                            <td style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="printbutton" name="printbutton" /></li>
                                </ul>
                            </td>
                            <td style="width:80%">
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
    </div>

    <div data-role="content" style="position: absolute; top: 17%; width: 100%; bottom: 2%;">
        <div class="ui-grid-d">
            <div class="ui-block-a">
                <table style="width:100%">
                    <tr>
                        <td style="width:30%; text-align:center"><label for="joborderno" id="jobordernolabel"></label></td>
                        <td style="width:65%">
                            <input type="text" name="joborderno" id="joborderno" placeholder="JOB_ORDER_NO" />
                        </td>
                        <td style="width:5%"></td>
                    </tr>
                </table>
                <table style="width:100%">
                    <tr>
                        <td style="width: 30%; text-align: center"><label for="layno" id="laynolabel"></label></td>
                        <td style="width: 65%; text-align: right;">
                            <select data-native-menu="false" name="layno" id="layno" multiple="multiple"  >
                            </select>
                        </td>
                        <td style="width:5%"></td>
                    </tr>
                </table>
                <table style="width:100%">
                    <tr>
                        <td style="width: 30%; text-align: center"><label for="bundle" id="bundlelabel"></label></td>
                        <td style="width: 65%; text-align: right;" onclick="bundledata()">
                            <select data-native-menu="false" name="bundle" id="bundle" multiple="multiple">
                            </select>
                        </td>
                        <td style="width:5%"></td>
                    </tr>
                </table>
            </div>
            <div class="ui-block-b">
                <fieldset data-role="controlgroup" id="partgroup">
                    <legend id="combinepart"></legend>
                </fieldset>
            </div>
            <div class="ui-block-c" style="margin-left: 15px;">
                  <fieldset data-role="controlgroup" id="isprintpartgroup">
                    <legend id="isprintpart"></legend>
                </fieldset>
            </div>  
            <div class="ui-block-d">

            </div>
            <div class="ui-block-e">
            </div>
        </div>
        <div id="myprinttest" style="width:12.4cm;">
            <%--<div style='height:3.9cm;line-height:1.4;'>
                <table style='width:100%'>
                    <tr style='width:100%'>
                        <td width='65%'>
                            <table style='width:100%'>
                                <tr style='width:100%'>
                                    <td width='50%'><div style='font-size:15px; font-weight:bolder;line-height:1.4;'>" + o.BUNDLE_BARCODE + "</div></td>
                                    <td width='50%'><div style='font-size:11px;line-height:1.4;'>" + o.DATE + "</div></td>
                                </tr>
                                <tr style='width:100%'>
                                    <td width='50%'><div style='font-size:11px;line-height:1.4;'>客户:" + o.CLIENT + "</div></td>
                                    <td width='50%'><div style='font-size:11px;line-height:1.4;'>款式:" + o.STYLE_NO + "</div></td>
                                </tr>
                                <tr style='width:100%'>
                                    <td width='50%'><div style='font-size:11px;line-height:1.4;'>制单:" + o.JOB_ORDER_NO + "</div></td>
                                    <td width='50%'><div style='font-size:11px;line-height:1.4;'>制单数:" + o.CUT_QTY + "</div></td>
                                </tr>
                                <tr style='width:100%'>
                                    <td width='50%'><div style='font-size:11px;line-height:1.4;'>颜色:" + o.COLOR + "</div></td>
                                    <td width='50%'><div style='font-size:11px;line-height:1.4;'>扎号:</div></td>
                                </tr>
                            </table>
                            <table style='width:100%'>
                                <tr style='width:100%'>
                                    <td width='33%'>
                                        <table style='margin-left:-2px'>
                                            <tr>
                                                <td><div style='font-size:11px;line-height:1.4;'>床次:" + o.LAY_NO + "</div></td>
                                            </tr>
                                            <tr>
                                                <td><div style='font-size:11px;line-height:1.4;'>尺码:" + o.SIZE + "</div></td>
                                            </tr>
                                            <tr>
                                                <td><div style='font-size:11px;line-height:1.4;'>数量:" + o.QTY + "</div></td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td width='33%'>
                                        <table>
                                            <tr>
                                                <td><div style='font-size:50px;line-height:1;text-align:left'>" + o.BUNDLE_NO + "</div></td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td width='34%'></td>
                                </tr>
                            </table>
                        </td>
                        <td width='35%'><div id='bundleno" + i.toString() + "'></div></td>
                    </tr>
                </table>
            </div>
            <div style='height:1.180cm;line-height:1.4;'>
                <table style='width:100%'>
                    <tr style='width:100%'>
                        <td width='20%'><div style='font-size:11px;line-height:1.4;'>组别:" + o.PRODUCTION_LINE + "</div></td>
                        <td width='80%'><div style='font-size:11px;line-height:1.4;'>幅位:" + o.PART + "</div></td>
                    </tr>
                </table>
                <table style='width:100%; margin-top: -3px;'>
                    <tr style='width:100%'>
                        <td width='20%'><div style='font-size:11px;line-height:1.4;'>唛架:" + o.MARKERS + "</div></td>
                        <td width='20%'><div style='font-size:11px;line-height:1.4;'>纸样:" + o.PATTERN_NO + "</div></td>
                        <td width='20%'><div style='font-size:11px;line-height:1.4;'>色级:" + o.SHADE_LOT + "</div></td>
                        <td width='40%'><div style='font-size:11px;line-height:1.4;'>缸号:" + o.BATCH_NO + "</div></td>
                    </tr>
                </table>
            </div>--%>
        </div>
    </div>

    <div data-role="footer" data-theme="b" data-position="fixed" data-fullscreen="true">
        <h1 id="footertitle"></h1>
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
                <a id="printbarcodea" href="" data-ajax="false"></a>
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
                    <li><a id="bundleinputa" href="Bundleinput.aspx" onclick="bundleinputjump(fty, environment);return false;" data-ajax="false"></a></li>
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
    </div>
</body>
</html>






<script>

    //tangyh 20170921
    function Print_YMG_HTML(i, o, printPartFlag) {
        var html = "";
        html = html + "   <div style='height:3.9cm;line-height:1.3;'>";
        html = html + "   <table style='width:100%'>";
        html = html + "   <tr style='width:100%'>";
        html = html + "   <td width='65%'>";

        html = html + "   <table style='width:100%'>";
        html = html + "   <tr style='width:100%'>";
        html = html + "   <td width='50%'><div style='font-size:15px; font-weight:bolder;line-height:1.3;'>" + o.BUNDLE_BARCODE + "</div></td>";
        html = html + "   <td width='50%'><div style='font-size:13px;line-height:1.3;'>" + o.DATE + "</div></td>";
        html = html + "   </tr>";
        html = html + "   <tr style='width:100%'>";
        html = html + "   <td width='50%'><div style='font-size:13px;line-height:1.3;'>客户:" + o.CLIENT + "</div></td>";
        html = html + "   <td width='50%'><div style='font-size:13px;line-height:1.3;'>款式:" + o.STYLE_NO + "</div></td>";
        html = html + "   </tr>";
        html = html + "   <tr style='width:100%'>";
        html = html + "   <td width='50%'><div style='font-size:15px;line-height:1.3;'>制单:" + o.JOB_ORDER_NO + "</div></td>";
        html = html + "   <td width='50%'><div style='font-size:13px;line-height:1.3;'>制单数:" + o.CUT_QTY + "</div></td>";
        html = html + "   </tr>";
        html = html + "   <tr style='width:100%'>";
        //0925 13-15
        html = html + "   <td width='50%'><div style='font-size:13px;line-height:1.3;'>颜色:<span style='font-size:14px;line-height:1.3;'>" + o.COLOR + "</span></div></td>";
        html = html + "   <td width='50%'><div style='font-size:13px;line-height:1.3;'>扎号:</div></td>";
        html = html + "   </tr>";
        html = html + "   </table>";

        html = html + "   <table style='width:100%'>";
        html = html + "   <tr style='width:100%'>";

        html = html + "   <td width='50%'><table style='margin-left:-4px; margin-top: -5px;'>";
        html = html + "   <tr>";
        html = html + "   <td><div style='font-size:13px;line-height:1.3;'>床次:<span style='font-size:13px;line-height:1.3;'>" + o.LAY_NO + "</span></div></td>";
        html = html + "   </tr>";
        html = html + "   <tr>";
        html = html + "   <td><div style='font-size:13px;line-height:1.3;'>尺码:<span style='font-size:13px;line-height:1.4;'>" + o.SIZE + "</span></div></td>";
        html = html + "   </tr>";

        html = html + "   <tr>";
        html = html + "   <td><div style='font-size:13px;line-height:1.3;'>数量:<span style='font-size:14px;line-height:1.3;'>" + o.QTY + "</span></div></td>";
        html = html + "   </tr>";
        html = html + "   </table>";
        html = html + "   </td>";

        html = html + "   <td width='24%'>";
        html = html + "   <table>";
        html = html + "   <tr>";
        //45--0611
        html = html + "   <td><div style='font-size:45px;line-height:1;text-align:left'>" + o.BUNDLE_NO + "</div></td>";
        html = html + "   </tr>";
        html = html + "   </table>";
        html = html + "   </td>";

        html = html + "   <td width='26%'></td>";
        html = html + "   </tr>";
        html = html + "   </table>";

        html = html + "   </td>";
        html = html + "   <td width='35%'>";
        html = html + "   <div id='bundleno" + i.toString() + "'></div>";
        html = html + "   </td>";
        html = html + "   </tr>";
        html = html + "   </table>";
        html = html + "   </div>";
        html = html + "   <div style='height:1.180cm;line-height:1.3;'>";
        html = html + "   <table style='width:100%'>";
        html = html + "   <tr style='width:100%'>";
        html = html + "   <td width='35%'><div style='font-size:13px;line-height:1.3;'>组别:" + o.PRODUCTION_LINE + "</div></td>";
        html = html + "   <td width='65%'><div style='font-size:13px;line-height:1.3;'>幅位:" + o.PART + printPartFlag + "</div></td>";
        html = html + "   </tr>";
        html = html + "   </table>  ";
        html = html + "   <table style='width:100%; margin-top: -8px;'>";
        html = html + "   <tr style='width:100%'>";
        html = html + "   <td width='20%'><div style='font-size:13px;line-height:1.3;'>唛架" + o.MARKERS + "</div></td>";
        html = html + "   <td width='20%'><div style='font-size:13px;line-height:1.3;'>纸样" + o.PATTERN_NO + "</div></td>";
        html = html + "   <td width='20%'><div style='font-size:13px;line-height:1.3;'>色级" + o.SHADE_LOT + "</div></td>";
        html = html + "   <td width='40%'><div style='font-size:14px;line-height:1.3;'>缸号:" + o.BATCH_NO + "</div></td>";
        html = html + "   </tr>";
        html = html + "   </table>";
        html = html + "   </div>";
        return html;
    }


    function Print_GEG_HTML(i, o, printPartFlag) {
        var html = "";
        html = html + "   <div style='height:3.9cm;line-height:1.2;'>";
        html = html + "   <table style='width:100%'>";
        html = html + "   <tr style='width:100%'>";
        html = html + "   <td width='65%'>";
        html = html + "   <table style='width:100%'>";
        html = html + "   <tr style='width:100%'>";
        html = html + "   <td width='50%'><div style='font-size:15px; font-weight:bolder;line-height:1.2;'>" + o.BUNDLE_BARCODE + "</div></td>";
        html = html + "   <td width='50%'><div style='font-size:13px;line-height:1.2;'>" + o.DATE + "</div></td>";
        html = html + "   </tr>";
        html = html + "   <tr style='width:100%'>";
        html = html + "   <td width='50%'><div style='font-size:13px;line-height:1.2;'>客户:" + o.CLIENT + "</div></td>";
        html = html + "   <td width='50%'><div style='font-size:13px;line-height:1.2;'>款式:" + o.STYLE_NO + "</div></td>";
        html = html + "   </tr>";
        html = html + "   <tr style='width:100%'>";
        html = html + "   <td width='50%'><div style='font-size:13px;line-height:1.2;'>制单:" + o.JOB_ORDER_NO + "</div></td>";
        html = html + "   <td width='50%'><div style='font-size:13px;line-height:1.2;'>制单数:" + o.CUT_QTY + "</div></td>";
        html = html + "   </tr>";
        html = html + "   <tr style='width:100%'>";
        html = html + "   <td width='50%'><div style='font-size:13px;line-height:1.2;'>颜色:" + o.COLOR + "</div></td>";
        html = html + "   <td width='50%'><div style='font-size:13px;line-height:1.2;'>扎号:</div></td>";
        html = html + "   </tr>";
        html = html + "   </table>";
        html = html + "   <table style='width:100%'>";
        html = html + "   <tr style='width:100%'>";
        html = html + "   <td width='50%'><table style='margin-left:-4px; margin-top: -5px;'>";
        html = html + "   <tr>";
        html = html + "   <td><div style='font-size:13px;line-height:1.2;'>床次:" + o.LAY_NO + "</div></td>";
        html = html + "   </tr>";
        html = html + "   <tr>";
        html = html + "   <td><div style='font-size:15px;line-height:1.2;'>尺码:<span style='font-size:20px;line-height:1.2;'>" + o.SIZE + "</span></div></td>";
        html = html + "   </tr>";
        html = html + "   <tr>";
        html = html + "   <td><div style='font-size:13px;line-height:1.2;'>数量:<span style='font-size:13px;line-height:1.2;'>" + o.QTY + "</span></div></td>";
        html = html + "   </tr>";
        html = html + "   </table>";
        html = html + "   </td>";
        html = html + "   <td width='50%'>";
        html = html + "   <table>";
        html = html + "   <tr>";
        html = html + "   <td><div style='font-size:50px;line-height:1;text-align:left'>" + o.BUNDLE_NO + "</div></td>";
        html = html + "   </tr>";
        html = html + "   </table>";
        html = html + "   </td>";
        html = html + "   <td width='34%'></td>";
        html = html + "   </tr>";
        html = html + "   </table>";
        html = html + "   </td>";
        html = html + "   <td width='35%'>";
        html = html + "   <div id='bundleno" + i.toString() + "'></div>";
        html = html + "   </td>";
        html = html + "   </tr>";
        html = html + "   </table>";
        html = html + "   </div>";
        html = html + "   <div style='height:1.180cm;line-height:1.2;'>";
        html = html + "   <table style='width:100%'>";
        html = html + "   <tr style='width:100%'>";
        html = html + "   <td width='20%'><div style='font-size:13px;line-height:1.2;'>组别:" + o.PRODUCTION_LINE + "</div></td>";
        html = html + "   <td width='80%'><div style='font-size:13px;line-height:1.2;'>幅位:" + o.PART + printPartFlag + "</div></td>";
        html = html + "   </tr>";
        html = html + "   </table>  ";
        html = html + "   <table style='width:100%; margin-top: -6px;'>";
        html = html + "   <tr style='width:100%'>";
        html = html + "   <td width='11%'><div style='font-size:13px;line-height:1.2;'>唛架" + o.MARKERS + "</div></td>";
        html = html + "   <td width='11%'><div style='font-size:13px;line-height:1.2;'>纸样" + o.PATTERN_NO + "</div></td>";
        html = html + "   <td width='11%'><div style='font-size:13px;line-height:1.2;'>色级" + o.SHADE_LOT + "</div></td>";
        html = html + "   <td width='67%'><div style='font-size:13px;line-height:1.2;'>缸号:" + o.BATCH_NO + "</div></td>";
        html = html + "   </tr>";
        html = html + "   </table>";
        html = html + "   </div>";
        return html;
    }
    </script>


