<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PrintSparebarcode.aspx.cs" Inherits="PrintSparebarcode" %>
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
    <script type="text/javascript">
        var fty = "";
        var environment = "";
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
                    debugger;
                    $('#headertitle').html(globeltranslate.headertitle);
                    $('#footertitle').html(globeltranslate.footertitle);
                    $('#cancelbutton').html(globeltranslate.cancelbutton);
                    $('#functionmenubutton').html(globeltranslate.functionmenubutton);
                    $('#gonolabel').html(globeltranslate.gonolabel);
                    $('#colorlabel').html(globeltranslate.colorlabel);
                    $('#patternlabel').html(globeltranslate.patternlabel);
                    $('#batchnolabel').html(globeltranslate.batchnolabel);
                    $('#shadelotlabel').html(globeltranslate.shadelotlabel);
                    $('#querygobutton').val(globeltranslate.querygobutton).button('refresh');
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
                    $('#printsparebarcodea').html(globeltranslate.printsparebarcodea);
                    $('#laynolabel').html(globeltranslate.laynolabel);
                    $('#overprintbutton').val(globeltranslate.overprint).button('refresh');

                }
            });
        };

        $(function () {

            fty = getUrlParam('FTY');
            environment = getUrlParam('svTYPE');
            if (fty == null || environment == null) {
                alert('The url is wrong! Pls input like 192.168.x.x/CIPMS/Login.aspx?FTY=GEG&svTYPE=PROD');
            } else {

                //调用语言翻译函数
                if (window.localStorage.languageid == "1") {
                    window.language = "PrintSparebarcode/PrintSparebarcode-zh-CN.js";
                } else if (window.localStorage.languageid == "2") {
                    window.language = "PrintSparebarcode/PrintSparebarcode-en-US.js";
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

                    $('#gono').focus();
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
                    //--------------------------------------------------------------------------
                    //查询制单号
                    $('#querygobutton').click(function () {
                        if ($('#gono').val() == "") {
                            $('#gono').focus();
                        }
                        else {
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            $.ajax({
                                type: "POST",
                                url: "PrintSparebarcode.aspx/GonoInfo",
                                contentType: "application/json;charset=utf-8",
                                dataType: "json",
                                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'gono': '" + $('#gono').val() + "' }",
                                success: function (data) {
                                    $.mobile.loading('hide');
                                    var result = eval(data.d);
                                    if (result[0].PROCESS == "N") {
                                        alert(globeltranslate.processnotopen); //提示MES系统没有开工序
                                    } else {
                                        $('#layno').empty();
                                        $('#layno').append("<option>&nbsp;</option>");
                                        $('#layno').append(result[0].LAYNO);
                                        $('#layno').selectmenu('refresh');
                                        $('#newlayno').val(result[0].MAXLAYNO);
                                        $('#color').empty();
                                        $('#color').append("<option>&nbsp;</option>");
                                        $('#color').append(result[0].COLORCD);
                                        $('#color').selectmenu('refresh');
                                        $('#pattern').empty();
                                        $('#pattern').append("<option>&nbsp;</option>");
                                        $('#pattern').append(result[0].PATTERNNO);
                                        $('#pattern').selectmenu('refresh');
                                        $('#batchno').empty();
                                        $('#batchno').append("<option>&nbsp;</option>");
                                        $('#batchno').append(result[0].BATCHNO);
                                        $('#batchno').selectmenu('refresh');
                                        $('#shadelot').empty();
                                        $('#shadelot').append("<option>&nbsp;</option>");
                                        $('#shadelot').append(result[0].SHADELOT);
                                        $('#shadelot').selectmenu('refresh');
                                        $("input[name='part']").remove();
                                        $("label[name='part']").remove();
                                        $('#partgroup').append(result[0].PART);
                                        $("#partgroup").trigger("create");
                                        $("input[name='part']").each(function (i) {
                                            if (result[0].LASTLAYNO_SELECT_PART.indexOf($(this).val()) > -1) { $(this).attr("checked", true).checkboxradio("refresh"); }
                                            //不是第一床就禁用可以选中部位
                                            debugger;
                                            if (result[0].ISFIRSTLAY == "Y") {
                                                $(this).attr("disabled", false).checkboxradio("refresh");
                                            } else {
                                                $(this).attr("disabled", true).checkboxradio("refresh");
                                            }
                                        });
                                        $("input[name='SIZE_NUM']").remove();
                                        $("label[name='sizelabel']").remove();
                                        $('#sizegroup').append(result[0].SIZE);
                                        $("#sizegroup").trigger("create");
                                        $('#overprintbutton').attr('disabled', true).button('refresh');
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
                    $('#gono').bind('keypress', function (event) {
                        if (event.keyCode == '13') {
                            $('#querygobutton').click();
                        }
                    });

                    //注销按钮：返回登录页面
                    $('#cancelbutton').click(function () {
                        Loginout(fty, environment);
                    });

                    //打印按钮
                    $('#printbutton').click(function () {
                        publicPrintfunction("Y", userbarcode);
                    });

                    //覆盖打印
                    $('#overprintbutton').click(function () {
                        publicPrintfunction("N", userbarcode);
                    });


                }
            }
        });



        //--------------------------------------------------------
        //变化Color时触发
        function colorchange() {
             debugger;
            if (($('#gono').val().trim() == "") || ($('#gono').val() == null)) {
                $('#gono').focus();
                return;
            }
            if (($('#color').val().trim() == "") || ($('#color').val() == null)) {
                $('#color').focus();
                return;
            }

            $.ajax({
                type: "POST",
                url: "PrintSparebarcode.aspx/GonoColorInfo",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'gono': '" + $('#gono').val() + "', 'colorcd': '" + $('#color').val() + "' }",
                success: function (data) {
                    $.mobile.loading('hide');
                    var result = eval(data.d);
                    $('#pattern').empty();
                    $('#batchno').empty();
                    $('#shadelot').empty();
                    $('#pattern').append("<option>&nbsp;</option>");
                    $('#pattern').append(result[0].PATTERNNO);
                    $('#pattern').selectmenu('refresh');
                    $('#batchno').append("<option>&nbsp;</option>");
                    $('#batchno').append(result[0].BATCHNO);
                    $('#batchno').selectmenu('refresh');
                    $('#shadelot').append("<option>&nbsp;</option>");
                    $('#shadelot').append(result[0].SHADELOT);
                    $('#shadelot').selectmenu('refresh');
                },
                error: function (err) {
                    $.mobile.loading('hide');
                    alert(globeltranslate.errormessage + ":013");
                }
            });

        }


       //覆盖打印时触发
        function recoverprint() {
            $('#printbutton').attr('disabled', true).button('refresh');
            $('#overprintbutton').attr('disabled', false).button('refresh');
            $('#newlayno').val('');
            //把第一床选择部位带出来
            if (!($('#layno').val().trim() == "" || $('#layno').val() == null) && !($('#gono').val().trim() == "") || ($('#gono').val() == null)) {
                $.ajax({
                    type: "POST",
                    url: "PrintSparebarcode.aspx/GetGoFirstLayInfo",
                    contentType: "application/json;charset=utf-8",
                    dataType: "json",
                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'gono': '" + $('#gono').val() + "', 'layno': '" + $('#layno').val().trim() + "' }",
                    success: function(data) {
                        $.mobile.loading('hide');
                        var result = eval(data.d);

                        $('#color').val(result[0].COLORCD);
                        $('#color').selectmenu('refresh');
                        $('#pattern').val(result[0].PATTERNNO);
                        $('#pattern').selectmenu('refresh');
                        $('#batchno').val(result[0].BATCHNO);
                        $('#batchno').selectmenu('refresh');
                        $('#shadelot').val(result[0].SHADELOT);
                        $('#shadelot').selectmenu('refresh');
                        $("input[name='part']").each(function (i) {
                            if (result[0].LASTLAYNO_SELECT_PART.indexOf($(this).val()) > -1) { $(this).attr("checked", true).checkboxradio("refresh"); }
                            //不是第一床就禁用可以选中部位
                            debugger;
                            if (result[0].ISFIRSTLAY == "Y") {
                                $(this).attr("disabled", false).checkboxradio("refresh");
                            } else {
                                $(this).attr("disabled", true).checkboxradio("refresh");
                            }
                        });
                        //SIZE 赋值
                        debugger;
                        $("input[name='SIZE_NUM']").val('');
                        var sizenum= result[0].SIZE.split('$');
                        for (var i = 0; i < sizenum.length; i++) {
                            var domid = "#"+escapeJquery(sizenum[i].split('/')[0]);
                            var domobj = $(domid);
                            var domvalue = sizenum[i].split('/')[1];
                            domobj.val(domvalue);
                        }
                    },
                    error: function(err) {
                        $.mobile.loading('hide');
                        alert(globeltranslate.errormessage + ":013");
                    }
                });
            }

        }

        //新打印或重新打印时触发
        function printnew() {
            $('#printbutton').attr('disabled', false).button('refresh');
            $('#overprintbutton').attr('disabled', true).button('refresh');
            $('#layno').val('');
            $('#layno').selectmenu('refresh');
            //及时清空相关控件
            $('#color').val('');
            $('#color').selectmenu('refresh');
            $('#pattern').val('');
            $('#pattern').selectmenu('refresh');
            $('#batchno').val('');
            $('#batchno').selectmenu('refresh');
            $('#shadelot').val('');
            $('#shadelot').selectmenu('refresh');
            //$("input[name='part']").each(function(i) {
            //    $(this).attr("checked", false).checkboxradio("refresh");
            //});
            $("input[name='SIZE_NUM']").val('');

            if (!($('#newlayno').val().trim() == "" || $('#newlayno').val() == null) && !($('#gono').val().trim() == "") || ($('#gono').val() == null)) {
                $.ajax({
                    type: "POST",
                    url: "PrintSparebarcode.aspx/GetGoFirstLayInfo",
                    contentType: "application/json;charset=utf-8",
                    dataType: "json",
                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'gono': '" + $('#gono').val() + "', 'layno': '" + $('#newlayno').val().trim() + "' }",
                    success: function (data) {
                        $.mobile.loading('hide');
                        var result = eval(data.d);
                        $('#color').val(result[0].COLORCD);
                        $('#color').selectmenu('refresh');
                        $('#pattern').val(result[0].PATTERNNO);
                        $('#pattern').selectmenu('refresh');
                        $('#batchno').val(result[0].BATCHNO);
                        $('#batchno').selectmenu('refresh');
                        $('#shadelot').val(result[0].SHADELOT);
                        $('#shadelot').selectmenu('refresh');
                        $("input[name='part']").each(function (i) {
                            if (result[0].LASTLAYNO_SELECT_PART.indexOf($(this).val()) > -1) { $(this).attr("checked", true).checkboxradio("refresh"); }
                            //不是第一床就禁用可以选中部位
                            debugger;
                            if (result[0].ISFIRSTLAY == "Y") {
                                $(this).attr("disabled", true).checkboxradio("refresh");
                            } else {
                                $(this).attr("disabled", true).checkboxradio("refresh");
                            }
                        });
                        //SIZE 赋值
                        debugger;
                        $("input[name='SIZE_NUM']").val('');
                        var sizenum = result[0].SIZE.split('$');
                        for (var i = 0; i < sizenum.length; i++) {
                            var domid = "#" + escapeJquery(sizenum[i].split('/')[0]);
                            var domobj = $(domid);
                            var domvalue = sizenum[i].split('/')[1];
                            domobj.val(domvalue);
                        }
                    },
                    error: function (err) {
                        $.mobile.loading('hide');
                        alert(globeltranslate.errormessage + ":013");
                    }
                });
            }



        }

        //打印公共函数
        function publicPrintfunction(flag, userbarcode) {
            debugger;
            if ($.trim($('#color').val()) == '' || $('#color').val() == null) {
                alert(globeltranslate.confirmmessage1);
                return;
            }
            debugger;
            var selectPart = "";
            $("input[name='part']:checked").each(function (i) {

                selectPart = selectPart + $.trim($(this).val()) + ",";
            });

            if (selectPart.length > 1) {
                selectPart = selectPart.substr(0, selectPart.length - 1);
            }

            if (selectPart == "" && ($.trim($('#layno').val()) == "" || $('#layno').val() == null)) {
                alert(globeltranslate.confirmmessage6);//选择印花部位
                return;
            }

            var sizeInput = "";
            $("input[name='SIZE_NUM']").each(function () {
                if ($.trim($(this).val()) != "") {
                    sizeInput = sizeInput + $(this).attr("id") + "$" + $.trim($(this).val()) + ",";
                }
            });

            if (sizeInput.length > 1) {
                sizeInput = sizeInput.substr(0, sizeInput.length - 1);
            }
            if (sizeInput == "" && ($.trim($('#layno').val()) == "" || $('#layno').val() == null)) {
                alert(globeltranslate.confirmmessage7); //输入size 数量
                return;
            }


            debugger;
            var layno = "0";
            if (flag == "Y") {
                if ($.trim($('#newlayno').val()) != "" && ($.trim($('#layno').val()) == "" || $('#layno').val() == null)) {
                    layno = $.trim($('#newlayno').val());
                    var arrMaxLay = checkifmaxlayno(fty, environment, $('#gono').val(), userbarcode, layno);
                    if (arrMaxLay[0] == "N" && arrMaxLay[1] !="0")
                    {
                        layno = '0';
                        $('#newlayno').val('');
                        alert("GO:" + $.trim($('#gono').val()) + globeltranslate.confirmmessage8 + arrMaxLay[1] + globeltranslate.confirmmessage9);
                        return ;
                    }
                }
            } else if (flag == "N") {
                if ($.trim($('#layno').val()) != "" && ($.trim($('#newlayno').val()) == "" || $('#newlayno').val() == null)) {
                    layno = $.trim($('#layno').val());
                } else {
                   alert(globeltranslate.confirmmessage10); //请不要同时输入床次和选择床次打印!
                    return;
                }
            } else {
                alert("print error! please check the print data, thanks.");
                return;
            }
            debugger;

            if ($.trim($('#color').val()) != '' && $.trim($('#gono').val()) != '' && layno != '0') {
                debugger;
                $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                $.ajax({
                    type: "POST",
                    url: "PrintSparebarcode.aspx/Print",
                    contentType: "application/json;charset=utf-8",
                    //async: false,//同步
                    dataType: "json",
                    data: "{ 'process': '" + sessionStorage.getItem("process") + "', 'userbarcode': '" + userbarcode + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'gono': '" + $.trim($('#gono').val()) + "','layno':'" + layno + "', 'colorcd': '" + $.trim($('#color').val()) + "' ,'part': '" + selectPart + "', 'sizenum':'" + sizeInput + "', 'patternno':'" + $.trim($('#pattern').val()) + "', 'batchno': '" + $.trim($('#batchno').val()) + "','shadelot': '" + $.trim($('#shadelot').val()) + "','flag':'" + flag + "'}",
                    success: function (data) {

                        $.mobile.loading('hide');
                        debugger;
                        // var result = eval(data.d);
                        var result = $.parseJSON(data.d);
                        debugger;
                        $('#myprinttest').empty();
                        $.each(result.BUNDLEINFO, function (i, o) {
                            var html = "" +
                                 "<div style='height:3.9cm;line-height:1.2;'>" +
                                 "<table style='width:100%'>" +
                                 "<tr style='width:100%'>" +
                                 "<td width='65%'>" +
                                     "<table style='width:100%'>" +
                                         "<tr style='width:100%'><td width='50%'><div style='font-size:13px;line-height:1.2;'>" + o.DATE + "</div></td></tr>" +
                                         "<tr style='width:100%'><td width='50%'><div style='font-size:13px;line-height:1.2;'>客户:" + o.CLIENT + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.2;'>款式:" + o.STYLE_NO + "</div></td></tr>" +
                                         "<tr style='width:100%'><td width='50%'><div style='font-size:14px;line-height:1.2;'>制单:" + o.JOB_ORDER_NO + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.2;'>制单数:" + o.CUT_QTY + "</div></td></tr>" +
                                         "<tr style='width:100%'><td width='50%'><div style='font-size:13px;line-height:1.2;'>颜色:" + o.COLOR + "</div></td><td width='50%'><div style='font-size:13px;line-height:1.2;'>扎号:</div></td></tr>" +
                                     "</table>  " +
                                     "<table style='width:100%'>" +
                                         "<tr style='width:100%'>" +
                                                 "<td width='50%'>" +
                                                             "<table style='margin-left:-4px; margin-top: -5px;'>" +
                                                                 "<tr><td><div style='font-size:13px;line-height:1.2;'>床次:" + o.LAY_NO + "</div></td></tr>" +
                                                                 "<tr><td><div style='font-size:15px;line-height:1.2;'>尺码:<span style='font-size:20px;line-height:1.2;'>" + o.SIZE + "</span></div></td></tr>" +
                                                                 "<tr><td><div style='font-size:13px;line-height:1.2;'>数量:<span style='font-size:13px;line-height:1.2;'>" + o.QTY + "</span></div></td></tr>" +
                                                             "</table>" +
                                                  "</td>" +
                                                 "<td width='50%'><table><tr><td><div style='font-size:50px;line-height:1;text-align:left'>" + o.BUNDLE_NO + "</div></td></tr></table></td>" +
                                                 "<td width='34%'></td>" +
                                      "</tr>" +
                                 "</table>" +
                                 "</td>" +

                                 "<td width='35%'><div id='bundleno" + i.toString() + "' style='font-size:30px;line-height:1;text-align:left;color:red;'>印花士啤</div></td>" +

                                 "</tr>" +
                                 "</table>" +
                                 "</div>" +

                                 "<div style='height:1.180cm;line-height:1.2;'>" +
                                     "<table style='width:100%'>" +
                                         "<tr style='width:100%'>" +
                                             "<td width='50%'><div style='font-size:13px;line-height:1.2;'>组别:" + o.PRODUCTION_LINE + "</div></td>" +
                                             "<td width='50%'><div style='font-size:13px;line-height:1.2;'>印花幅位:" + o.PART + "</div></td>" +
                                         "</tr>" +
                                     "</table> " +
                                     " <table style='width:100%; margin-top: -6px;'>" +
                                         "<tr style='width:100%'>" +
                                             "<td width='16%'><div style='font-size:13px;line-height:1.2;'>纸样:" + o.PATTERN_NO + "</div></td>" +
                                             "<td width='17%'><div style='font-size:13px;line-height:1.2;'>色级:" + o.SHADE_LOT + "</div></td>" +
                                             "<td width='67%'><div style='font-size:13px;line-height:1.2;'>缸号:" + o.BATCH_NO + "</div></td>" +
                                         "</tr>" +
                                     "</table>" +
                                 "</div>";

                            $('#myprinttest').append(html);

                        });

                        setTimeout(function () {
                            //打印div标签里的内容
                            $('#myprinttest').jqprint();
                        }, 2000);

                        $('#gono').val('');
                        $('#gono').focus();
                        $('#newlayno').val('');
                        $('#layno').empty();
                        $('#layno').selectmenu('refresh');
                        $('#color').empty();
                        $('#color').selectmenu('refresh');
                        $('#pattern').empty();
                        $('#pattern').selectmenu('refresh');
                        $('#batchno').empty();
                        $('#batchno').selectmenu('refresh');
                        $('#shadelot').empty();
                        $('#shadelot').selectmenu('refresh');
                        $("input[name='part']").remove();
                        $("label[name='part']").remove();
                        $("#partgroup").trigger("create");
                        $("#sizegroup").empty();
                        $('#gono').focus();

                    },
                    error: function (err) {
                        $.mobile.loading('hide');
                        alert(globeltranslate.errormessage + "033");
                    }
                });
            } else {
                alert(globeltranslate.confirmmessage1);
            }
        }

        //查询最大的床号
        var checkifmaxlayno = function (fty, environment, gono, userbarcode, layno) {
            var arr = [];
            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
            $.ajax({
                type: "POST",
                url: "PrintSparebarcode.aspx/GetGoMaxLayno",
                contentType: "application/json;charset=utf-8",
                async: false,
                dataType: "json",
                data: "{ 'process': '" + sessionStorage.getItem("process") + "', 'userbarcode': '" + userbarcode + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'gono': '" + gono + "','currentlayno':'" + layno + "'}",
                success: function (data) {
                    $.mobile.loading('hide');
                    var result = eval(data.d);
                    if (result[0].FLAG == "Y") {
                        debugger;
                        arr[0] = "Y";
                        arr[1] = result[0].LAYNO;
                      
                    } else {
                       debugger;
                        arr[0] = "N";
                        arr[1] = result[0].LAYNO;
                    }
                },
                error: function (err) {
                    $.mobile.loading('hide');
                    //alert(globeltranslate.errormessage + err + "033");
                    arr[0] = "N";
                    arr[1] = "";
                }
            });
            return arr;
        }

        function escapeJquery(srcString) {
            // 转义之后的结果  
            var escapseResult = srcString;
            // javascript正则表达式中的特殊字符  
            var jsSpecialChars = ["\\", "^", "$", "*", "?", ".", "+", "(", ")", "[",
                    "]", "|", "{", "}"];
            // jquery中的特殊字符,不是正则表达式中的特殊字符  
            var jquerySpecialChars = ["~", "`", "@", "#", "%", "&", "=", "'", "\"",
                    ":", ";", "<", ">", ",", "/"];
            for (var i = 0; i < jsSpecialChars.length; i++) {
                escapseResult = escapseResult.replace(new RegExp("\\"
                                        + jsSpecialChars[i], "g"), "\\"
                                + jsSpecialChars[i]);
            }
            for (var i = 0; i < jquerySpecialChars.length; i++) {
                escapseResult = escapseResult.replace(new RegExp(jquerySpecialChars[i],
                                "g"), "\\" + jquerySpecialChars[i]);
            }
            return escapseResult;
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
                                        <input type="button" style="text-align:center" id="querygobutton" name="querygobutton" />
                                    </li>
                                </ul>
                            </td>
                            <td style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="printbutton" name="printbutton" /></li>
                                </ul>
                            </td>
                             <td style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="overprintbutton" name="overprintbutton"    disabled="true" /></li>
                                </ul>
                            </td>
                            <td style="width:70%">
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
    </div>

    <div data-role="content" style="position: absolute; top: 17%; width: 100%; bottom: 2%;">
        <div class="ui-grid-d">
            <div class="ui-block-a" style="min-width: 500px;">
                <table style="width:100%">
                    <tr>
                        <td style="width:30%; text-align:center"><label for="gono" id="gonolabel"></label></td>
                        <td style="width:65%">
                            <input type="text" name="gono" id="gono" placeholder="GO NO" value="" />
                        </td>
                        <td style="width:5%"></td>
                    </tr>
                </table>
                <!--床次-->
                <table style="width:100%">
                    <tr>
                        <td style="width: 30%; text-align: center"><label for="layno" id="laynolabel"></label></td>
                        <td style="width: 65%; text-align: left;">
                           <table>
                               <tr>
                               <td style="width: 50%"><input type="text" name="newlayno" id="newlayno" placeholder="LAY NO" value=""  onchange="printnew()" /></td>
                               <td style="width: 50%">
                                        <select data-native-menu="false" name="layno" id="layno"  onchange="recoverprint()"   > <%--multiple="multiple" --%>
                                        </select>
                               </td>
                           </tr>
                           </table> 

                        </td>
                        <td style="width:5%"></td>
                    </tr>
                </table>
                <!--颜色-->
                <table style="width:100%">
                    <tr>
                        <td style="width: 30%; text-align: center"><label for="color" id="colorlabel"></label></td>
                        <td style="width: 65%; text-align: right;">
                            <select data-native-menu="false" name="color" id="color"    onchange="colorchange()" date-role="none">
                            </select>
                        </td>
                        <td style="width:5%"></td>
                    </tr>
                </table>
                <!--纸样-->
                <table style="width:100%">
                    <tr>
                        <td style="width: 30%; text-align: center"><label for="pattern" id="patternlabel"></label></td>
                        <td style="width: 65%; text-align: right;"  >
                            <select data-native-menu="false" name="pattern" id="pattern" >
                            </select>
                        </td>
                        <td style="width:5%"></td>
                    </tr>
                </table>
                 <!--缸号-->
                <table style="width:100%">
                    <tr>
                        <td style="width: 30%; text-align: center"><label for="batchno" id="batchnolabel"></label></td>
                        <td style="width: 65%; text-align: right;" >
                            <select data-native-menu="false" name="batchno" id="batchno" >
                            </select>
                        </td>
                        <td style="width:5%"></td>
                    </tr>
                </table>
                <!--色级-->
                <table style="width:100%">
                    <tr>
                        <td style="width: 30%; text-align: center"><label for="shadelot" id="shadelotlabel"></label></td>
                        <td style="width: 65%; text-align: right;" >
                            <select data-native-menu="false" name="shadelot" id="shadelot"  >
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
                  <fieldset data-role="controlinputgroup" >
                    <legend id="isprintpart"></legend>
                      <table id="sizegroup" style="width: 100% "></table>
                </fieldset>
            </div>  
            <div class="ui-block-d" >

            </div>
            <div class="ui-block-e">
            </div>
        </div>
        <div id="myprinttest" style="width:12.4cm;">

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
                <a id="printbarcodea" href="Printbarcode.aspx"  onclick="printbarcodejump(fty, environment);return false;" data-ajax="false"></a>
            </li>
             <!--added by lijer-->
            <li id="printsparebarcodeli">
                <a id="printsparebarcodea"  href="" data-ajax="false"></a>
            
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