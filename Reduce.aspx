<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Reduce.aspx.cs" Inherits="Reduce" %>

<!DOCTYPE html>
<html>
<head>
    <link href="css/JQM/jquery.mobile-1.4.5.min.css" rel="stylesheet" type="text/css" />
    <script src="js/JQM/jquery.js" type="text/javascript"></script>
    <script src="js/JQM/jquery.mobile-1.4.5.min.js" type="text/javascript"></script>
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
        var globeltranslate; //保存json字符串
        var environment = "";

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
                beforeSend: function () { $.mobile.loading('show'); },
                error: function () { $.mobile.loading('hide'); alert("Sorry! Pls check your network and refresh your brower! Wrong Code::001") },
                success: function (json) {
                    $.mobile.loading('hide');
                    globeltranslate = eval(json);
                    $('#headertitle').html(globeltranslate.headertitle);
                    $('#cancelbutton').html(globeltranslate.cancelbutton);
                    $('#functionmenubutton').html(globeltranslate.functionmenubutton);
                    $('#jotextlabel').html(globeltranslate.jotextlabel);
                    $('#laynoselectlabel').html(globeltranslate.laynoselectlabel);
                    $('#bundlenoselectlabel').html(globeltranslate.bundlenoselectlabel);
                    $('#barcodelabel').html(globeltranslate.barcodelabel);
                    $('#confirmbutton').val(globeltranslate.confirmbutton).button('refresh');
                    $('#cleanbutton').val(globeltranslate.cleanbutton).button('refresh');
                    $('#reasonlabel').html(globeltranslate.reasonlabel);
                    $('#reduceqtylabel').html(globeltranslate.reduceqtylabel);
                    $('#reducetablelabel').html(globeltranslate.reducetablelabel);
                    $('#defecttablelabel').html(globeltranslate.defecttablelabel);
                    $('#outqtylabel').html(globeltranslate.outqtylabel);
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
        };

        function reasonselecttrans() {
            $('#reasonselect').empty();
            $('#reasonselect').append("<option value='select'>" + globeltranslate.reasonselect + "</option>");
            $('#reasonselect').selectmenu('refresh');
        }
        function laynoselecttrans() {
            $('#laynoselect').empty();
            $('#laynoselect').append("<option>" + globeltranslate.reasonselect + "</option>");
            $('#laynoselect').selectmenu('refresh');
        }
        function bundlenoselecttrans() {
            $('#bundlenoselect').empty();
            $('#bundlenoselect').append("<option>" + globeltranslate.reasonselect + "</option>");
            $('#bundlenoselect').selectmenu('refresh');
        }

        function btnstatus() {
            if (isprocess == '1' && reasonselected == '1' && reduceqtyinput == '1')
                $('#confirmbutton').removeAttr('disabled').button('refresh');
            else
                $('#confirmbutton').attr('disabled', 'true').button('refresh');
        }

        function clean() {
            $('#barcodetext').val('');
            $('#reduceqtytext').val('');
            $('#outqtytext').val('');
            $("#reasonselect option[value='select']").attr('selected', 'selected');
            $('#reasonselect').selectmenu('refresh');
            reasonselected = '0';
            reduceqtyinput = '0';
            btnstatus();
        }

        var isprocess = '0';
        var reasonselected = '0';
        var reduceqtyinput = '0';

        $(function () {
            fty = getUrlParam('FTY');
            environment = getUrlParam('svTYPE');

            //20170615-tangyh
            queryfactory("T", fty,"li");
            
            if (fty == null || environment == null) {
                alert('The url is wrong! Pls input like 192.168.x.x/CIPMS/Login.aspx?FTY=GEG&svTYPE=PROD');
            } else {
                //调用语言翻译函数
                if (window.localStorage.languageid == "1") {
                    window.language = "Reduce/Reduce-zh-CN.js";
                } else if (window.localStorage.languageid == "2") {
                    window.language = "Reduce/Reduce-en-US.js";
                }
                translate();

                //左侧栏
                $("#my-menu").mmenu({
                    "footer": {
                        "add": true,
                        "title": globeltranslate.footertitle
                    },
                    "header": {
                        "title": globeltranslate.reasonselect,
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
                    $('#barcodetext').focus();
                    reasonselecttrans();
                    laynoselecttrans();
                    bundlenoselecttrans();
                    $('#confirmbutton').attr('disabled', 'true').button('refresh');
                    //function权限控制
                    Accessmodulehide();
                    $('#messagetitle').html(sessionStorage.getItem("name") + " , " + sessionStorage.getItem("factory") + "/" + sessionStorage.getItem("process") + "/" + sessionStorage.getItem("productionline"));
                    var employeeno = sessionStorage.getItem("employeeno");
                    var userbarcode = sessionStorage.getItem("userbarcode");
                    var factory = sessionStorage.getItem("factory");
                    $.ajax({
                        type: "POST",
                        url: "Package.aspx/Accessfunction",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + userbarcode + "' }",
                        success: function (data) {
                            $.mobile.loading('hide');
                            Accessmoduleshow(data.d);
                        },
                        error: function (err) {
                            $.mobile.loading('hide');
                            alert(globeltranslate.errormessage);
                        }
                    });
                    //加载下数原因
                    $.ajax({
                        type: "POST",
                        url: "Transaction.aspx/Defectreason",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "' }",
                        success: function (data) {
                            $('#reasonselect').append(data.d);
                            $('#reasonselect').selectmenu('refresh');
                        },
                        error: function (err) {
                            alert(globeltranslate.errormessage + "512");
                        }
                    });
                }

                $("#reducetabletitle thead tr").empty();
                $('#reducetabletitle thead').append("<tr><th>" + globeltranslate.title1 + "</th><th>" + globeltranslate.title2 + "</th><th>" + globeltranslate.title3 + "</th><th>" + globeltranslate.title4 + "</th><th>" + globeltranslate.title5 + "</th><th>" + globeltranslate.title6 + "</th><th></th></tr>");
                $('#reducetabletitle').trigger('create');

                $("#defecttabletitle thead tr").empty();
                $('#defecttabletitle thead').append("<tr><th>" + globeltranslate.title8 + "</th><th>" + globeltranslate.title1 + "</th><th>" + globeltranslate.title4 + "</th><th>" + globeltranslate.title7 + "</th><th>" + globeltranslate.title5 + "</th><th>" + globeltranslate.title6 + "</th><th></th></tr>");
                $('#defecttabletitle').trigger('create');

                //表格样式
                var screenwidth = document.body.clientWidth;
                var screenheight = document.body.clientHeight;
                $('#defecttable').chromatable({
                    width: screenwidth * 0.47,
                    height: screenheight * 0.4,
                    scrolling: "yes"
                });
                $('#reducetable').chromatable({
                    width: screenwidth * 0.47,
                    height: screenheight * 0.4,
                    scrolling: "yes"
                });
                //表格高亮行
                $('.stripe tr').mouseover(function () {
                    $(this).addClass("over")
                }).mouseout(function () {
                    $(this).removeClass("over");
                });

                //注销按钮：返回登录页面
                $('#cancelbutton').click(function () {
                    Loginout(fty, environment);
                });

                //监控reduce_qty的输入的值，不能大于output的值
                $('#reduceqtytext').bind('input propertychange', function () {
                    if ($('#outqtytext').val() == '' || parseInt($('#reduceqtytext').val()) > parseInt($('#outqtytext').val())) {
                        $('#reduceqtytext').val("");

                    } else {
                        if ($(this).val().substr(0, 1) == '-') {
                            if ($(this).val().length > 1 && isNaN($(this).val().substr(1)))
                                $('#reduceqtytext').val("");
                        }
                        else if (isNaN($(this).val()))
                            $('#reduceqtytext').val("");
                    }
                    if ($('#reduceqtytext').val() != "") {
                        reduceqtyinput = '1';
                        btnstatus();
                    } else {
                        reduceqtyinput = '0';
                        btnstatus();
                    }
                });

                //下数原因选择监控
                $('#reasonselect').change(function () {
                    reasonselected = '1';
                    btnstatus();
                });

                //barcodetext的确认监控
                $('#barcodetext').bind('keypress', function (event) {
                    var barcode = $('#barcodetext').val();
                    if (event.keyCode == '13') {
                        if (barcode.length == '14') {
                            //加载该bundle barcode的最小WIP以及判断能否够reduce
                            $.ajax({
                                type: "POST",
                                url: "Reduce.aspx/Minwip",
                                contentType: "application/json;charset=utf-8",
                                dataType: "json",
                                data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'barcode': '" + barcode + "'}",
                                success: function (data) {
                                    var result = eval(data.d);
                                    if (result[0].ISPROCESS == "true" && result[0].ISSUBMIT == "false")
                                        isprocess = '1';
                                    else
                                        isprocess = '0';
                                    if (result[0].ISSUBMIT == 'true') {
                                        alert(globeltranslate.barcodescanmessage2);
                                    }
                                    clean();
                                    $('#barcodetext').val(barcode);
                                    $('#outqtytext').val(result[0].WIP);
                                    btnstatus();
                                },
                                error: function (err) {
                                    alert(err);
                                    alert(globeltranslate.errormessage + "005");
                                }
                            });
                            //加载该bundle barcode的defect信息
                            $.ajax({
                                type: "POST",
                                url: "Reduce.aspx/Defectinformation",
                                contentType: "application/json;charset=utf-8",
                                dataType: "json",
                                data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'barcode': '" + barcode + "'}",
                                success: function (data) {
                                    $('#defecttable tr').empty();
                                    if (data.d != 'null') {
                                        $('#defecttable tbody').append(data.d);
                                        $('#defecttable').trigger('create');
                                        $('#defecttabletitle thead').find('tr th').each(function (i) {
                                            if (i <= 5) {
                                                $(this).width($('#defecttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                            }
                                        });
                                    }
                                },
                                error: function (err) {
                                    alert(globeltranslate.errormessage + "002");
                                }
                            });
                            //加载该bundle barcode的reduce信息
                            $.ajax({
                                type: "POST",
                                url: "Reduce.aspx/Reduceinformation",
                                contentType: "application/json;charset=utf-8",
                                dataType: "json",
                                data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'barcode': '" + barcode + "'}",
                                success: function (data) {
                                    $('#reducetable tr').empty();
                                    if (data.d != 'null') {
                                        $('#reducetable tbody').append(data.d);
                                        $('#reducetable').trigger('create');
                                        $('#reducetabletitle thead').find('tr th').each(function (i) {
                                            if (i <= 5) {
                                                $(this).width($('#reducetable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                            }
                                        });
                                    }
                                },
                                error: function (err) {
                                    alert(globeltranslate.errormessage + "003");
                                }
                            });
                        } else {
                            alert(globeltranslate.barcodescanmessage1);
                        }
                    }
                });

                //confirm按钮监控
                $('#confirmbutton').click(function () {
                    var barcode = $('#barcodetext').val();
                    var reduceqty = $('#reduceqtytext').val();
                    var reducereason = $('#reasonselect').find("option:selected").val();
                    var temp = "false";
                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                    setTimeout(function () {
                        $.ajax({
                            type: "POST",
                            url: "Reduce.aspx/Confirm",
                            contentType: "application/json;charset=utf-8",
                            //async: false,
                            dataType: "json",
                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "','cipmsprocess': '" + sessionStorage.getItem("process") + "' ,'barcode': '" + barcode + "', 'reduceqty': '" + reduceqty + "', 'reducereason': '" + reducereason + "'}",
                            success: function (data) {
                                $.mobile.loading('hide');
                                //重新加载reduce list、out_qty
                                if (data.d.substr(0, 5) != 'error' && data.d.substr(0, 5) != 'false') {
                                    alert(globeltranslate.confirmmessage2);
                                    $.ajax({
                                        type: "POST",
                                        url: "Reduce.aspx/Minwip",
                                        contentType: "application/json;charset=utf-8",
                                        dataType: "json",
                                        data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'barcode': '" + barcode + "'}",
                                        success: function (data) {
                                            var result = eval(data.d);
                                            if (result[0].ISPROCESS == "true")
                                                isprocess = '1';
                                            else
                                                isprocess = '0';
                                            $('#reduceqtytext').val("");
                                            reduceqtyinput = '0';
                                            $('#outqtytext').val(result[0].WIP);
                                            btnstatus();
                                        },
                                        error: function (err) {
                                            alert(globeltranslate.errormessage + "005");
                                        }
                                    });
                                    //重新加载reduce list
                                    $.ajax({
                                        type: "POST",
                                        url: "Reduce.aspx/Reduceinformation",
                                        contentType: "application/json;charset=utf-8",
                                        dataType: "json",
                                        data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'barcode': '" + barcode + "'}",
                                        success: function (data) {
                                            if (data.d != 'null') {
                                                $('#reducetable tr').empty();
                                                $('#reducetable tbody').append(data.d);
                                                $('#reducetable').trigger('create');
                                                $('#reducetabletitle thead').find('tr th').each(function (i) {
                                                    if (i <= 5) {
                                                        $(this).width($('#reducetable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    }
                                                });
                                            }
                                        },
                                        error: function (err) {
                                            alert(globeltranslate.errormessage + "010");
                                        }
                                    });
                                }
                                else if (data.d.substr(0, 5) == 'false') {
                                    if (data.d.substr(5) == 'Y') {
                                        alert(globeltranslate.confirmmessage3 + 'Y');
                                    }
                                    else if (data.d.substr(5) == 'C') {
                                        alert(globeltranslate.confirmmessage4 + 'C');
                                    }
                                    else if (data.d.substr(5) == 'M') {
                                        alert(globeltranslate.confirmmessage5 + 'M');
                                    }
                                    else if (data.d.substr(5) == 'K') {
                                        alert(globeltranslate.confirmmessage6 + 'K');
                                    }
                                }
                            },
                            error: function (err) {
                                $.mobile.loading('hide');
                                alert(globeltranslate.errormessage + "012");
                            }
                        });
                    }, 10);
                });

                $('#jotext').bind('keypress', function (event) {
                    if (event.keyCode == '13') {
                        //加载床次
                        laynoselecttrans();
                        bundlenoselecttrans();
                        $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                        $.ajax({
                            type: "POST",
                            url: "Reduce.aspx/layno",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'joborderno': '" + $('#jotext').val() + "' }",
                            success: function (data) {
                                $.mobile.loading('hide');
                                if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                    var result = eval(data.d);
                                    $('#laynoselect').append(result[0].laynohtml);
                                    $('#laynoselect').selectmenu('refresh');
                                    $('#bundlenoselect').append(result[0].bundlehtml);
                                    $('#bundlenoselect').selectmenu('refresh');
                                    clean();
                                }
                                else if (data.d.substr(0, 5) == 'false') {
                                    alert(globeltranslate.barcodescanmessage3);
                                }
                            },
                            error: function (err) {
                                $.mobile.loading('hide');
                                alert(globeltranslate.errormessage + "42");
                            }
                        });
                    }
                });

                $('#laynoselect').change(function () {
                    //加载扎号
                    bundlenoselecttrans();
                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                    $.ajax({
                        type: "POST",
                        url: "Reduce.aspx/bundleno",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'joborderno': '" + $('#jotext').val() + "', 'layno': '" + $('#laynoselect').val() + "' }",
                        success: function (data) {
                            $.mobile.loading('hide');
                            if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                var result = eval(data.d);
                                $('#bundlenoselect').append(result[0].bundlehtml);
                                $('#bundlenoselect').selectmenu('refresh');
                                clean();
                            } else if (data.d.substr(0, 5) == 'false') {
                                alert(globeltranslate.barcodescanmessage3);
                            }
                        },
                        error: function (err) {
                            $.mobile.loading('hide');
                            alert(globeltranslate.errormessage + "564");
                        }
                    });
                });

                $('#bundlenoselect').change(function () {
                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                    $.ajax({
                        type: "POST",
                        url: "Reduce.aspx/Minwip2",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'joborderno': '" + $('#jotext').val() + "', 'bundleno': '" + $('#bundlenoselect').val() + "' }",
                        success: function (data) {
                            $.mobile.loading('hide');
                            if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                var result = eval(data.d);
                                if (result[0].ISPROCESS == "true" && result[0].ISSUBMIT == "false")
                                    isprocess = '1';
                                else
                                    isprocess = '0';
                                if (result[0].ISSUBMIT == 'true') {
                                    alert(globeltranslate.barcodescanmessage2);
                                }
                                clean();
                                $('#reduceqtytext').val('');
                                $('#barcodetext').val(result[0].BARCODE);
                                $('#outqtytext').val(result[0].WIP);
                                btnstatus();

                                //加载该bundle barcode的defect信息
                                $.ajax({
                                    type: "POST",
                                    url: "Reduce.aspx/Defectinformation",
                                    contentType: "application/json;charset=utf-8",
                                    dataType: "json",
                                    data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'barcode': '" + $('#barcodetext').val() + "'}",
                                    success: function (data) {
                                        $('#defecttable tr').empty();
                                        if (data.d != 'null') {
                                            $('#defecttable tbody').append(data.d);
                                            $('#defecttable').trigger('create');
                                            $('#defecttabletitle thead').find('tr th').each(function (i) {
                                                if (i <= 5) {
                                                    $(this).width($('#defecttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                }
                                            });
                                        }
                                    },
                                    error: function (err) {
                                        alert(globeltranslate.errormessage + "002");
                                    }
                                });
                                //加载该bundle barcode的reduce信息
                                $.ajax({
                                    type: "POST",
                                    url: "Reduce.aspx/Reduceinformation",
                                    contentType: "application/json;charset=utf-8",
                                    dataType: "json",
                                    data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'barcode': '" + $('#barcodetext').val() + "'}",
                                    success: function (data) {
                                        $('#reducetable tr').empty();
                                        if (data.d != 'null') {
                                            $('#reducetable tbody').append(data.d);
                                            $('#reducetable').trigger('create');
                                            $('#reducetabletitle thead').find('tr th').each(function (i) {
                                                if (i <= 5) {
                                                    $(this).width($('#reducetable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                }
                                            });
                                        }
                                    },
                                    error: function (err) {
                                        alert(globeltranslate.errormessage + "003");
                                    }
                                });
                            }
                            else if (data.d.substr(0, 5) == 'false') {
                                if (data.d == 'false1') {
                                    alert(globeltranslate.barcodescanmessage5);
                                }
                                else if (data.d == 'false2') {
                                    alert(globeltranslate.barcodescanmessage5);
                                }
                            }
                        },
                        error: function (err) {
                            alert(globeltranslate.errormessage + "005");
                        }
                    });
                });

                $('#cleanbutton').click(function () {
                    reasonselecttrans();
                    reasonselected = '0';
                    btnstatus();
                    $('#jotext').val('');
                    laynoselecttrans();
                    bundlenoselecttrans();
                    $('#barcodetext').val('');
                    $('#barcodetext').focus();
                    $('#outqtytext').val('');
                    $('#reduceqtytext').val('');
                    $('#reducetable tr').empty();
                    $('#reducetable').trigger('create');
                    $('#defecttable tr').empty();
                    $('#defecttable').trigger('create');
                    $.ajax({
                        type: "POST",
                        url: "Transaction.aspx/Defectreason",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "' }",
                        success: function (data) {
                            $('#reasonselect').append(data.d);
                            $('#reasonselect').selectmenu('refresh');
                        },
                        error: function (err) {
                            alert(globeltranslate.errormessage + "512");
                        }
                    });
                });
            }
        });
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
            <div data-role="navbar">
                <div data-role="navbar">
                    <table style="width:100%">
                        <tr>
                            <td style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="confirmbutton" name="confirmbutton" /></li>
                                </ul>
                            </td>
                            <td style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="cleanbutton" name="cleanbutton" /></li>
                                </ul>
                            </td>
                            <td style="width:60%">
                            </td>

                            <%--//20170615-tangyh--%>
                            <td style="width:20%" hidden="hidden" id="queryfactory_td">
                                <ul>
                                    <li>
                                        <select data-native-menu="false" id="queryfactory" name="queryfactory"></select>
                                    </li>
                                </ul>
                            </td>
                            <script>  //20170615-tangyh
                                if (sessionStorage.getItem("process") == 'PRT') {
                                    $('#queryfactory_td').show();
                                }

                                $('#queryfactory').change(function () {
                                    if (($('#queryfactory').val() == "undefined") || ($('#queryfactory').val() == globeltranslate.queryfactory)) {
                                        return;
                                    }

                                    var int1 = $("#queryfactory").get(0).selectedIndex;
                                    var value1 = $('#queryfactory').get(0)[int1].innerText;

                                    $('#queryfactory').selectmenu('refresh');
                                    if (value1 != fty) {
                                        var isokaccountid = checkaccountid("F", value1, sessionStorage.getItem("userbarcode"), environment);
                                        if (isokaccountid == "F") {
                                            return;
                                        }
                                        fty = value1;
                                        $('#cleanbutton').click();
                                        
                                    }
                                });
                            </script>     
                            

                        </tr>
                    </table>
                </div>
            </div>
        </div>

        <div data-role="content" style="position: absolute; width:100%; top: 17%; bottom: 8%">
            <div class="ui-grid-c">
                <div class="ui-block-a">
                    <table style="width:100%">
                        <tr>
                            <td style="width: 50%; text-align: center"><label for="jotext" id="jotextlabel"></label></td>
                            <td style="width: 50%; text-align: right;"><input id="jotext" type="text" name="jotext" /></td>
                        </tr>
                    </table>
                    <table style="width:100%">
                        <tr>
                            <td style="width: 50%; text-align: center"><label for="laynoselect" id="laynoselectlabel"></label></td>
                            <td style="width: 50%; text-align: right;">
                                <select id="laynoselect" name="laynoselect" >
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table style="width:100%">
                        <tr>
                            <td style="width: 50%; text-align: center"><label for="bundlenoselect" id="bundlenoselectlabel"></label></td>
                            <td style="width: 50%; text-align: right;">
                                <select id="bundlenoselect" name="bundlenoselect" >
                                </select>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-b">
                    <table style="width:100%">
                        <tr>
                            <td style="width: 50%; text-align: center"><label for="barcodetext" id="barcodelabel"></label></td>
                            <td style="width: 50%; text-align: right;"><input id="barcodetext" type="text" name="barcodetext" /></td>
                        </tr>
                    </table>
                    <table style="width:100%">
                        <tr>
                            <td style="width: 50%; text-align: center"><label for="reduceqtytext" id="reduceqtylabel"></label></td>
                            <td style="width: 50%; text-align: right;"><input id="reduceqtytext" type="text" name="reduceqtytext" /></td>
                        </tr>
                    </table>
                    <table style="width:100%">
                        <tr>
                            <td style="width: 50%; text-align: center"><label for="reasonselect" id="reasonlabel"></label></td>
                            <td style="width: 50%; text-align: right;">
                                <select id="reasonselect" name="reasonselect" >
                                </select>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-c">
                    <table style="width:100%">
                        <tr>
                            <td style="width: 50%; text-align: center"><label for="outqtytext" id="outqtylabel"></label></td>
                            <td style="width: 50%; text-align: right;"><input readonly="readonly" type="text" id="outqtytext" name="outqtytext" style="color:Red" /></td>
                        </tr>
                    </table>
                    <table style="width:100%">
                        <tr>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-d">
                </div>
            </div>
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <table style="width:100%">
                        <tr>
                            <td style="width:3%"></td>
                            <td style="width: 94%; text-align: left">
                                <p id="reducetablelabel"></p>
                                <table class="stripe" id="reducetabletitle" style="width:100%" border="0" cellspacing="0" cellpadding="0">
                                    <thead> 
                                    </thead> 
                                    <tbody>
                                    </tbody>
                                </table>
                                <table class="stripe" id="reducetable" style="width:100%" border="0" cellspacing="0" cellpadding="0">
                                    <thead> 
                                    </thead> 
                                    <tbody>
                                    </tbody>
                                </table>
                            </td>
                            <td style="width:3%"></td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-b">
                    <table style="width:100%">
                        <tr>
                            <td style="width:3%"></td>
                            <td style="width: 94%; text-align: left">
                                <p id="defecttablelabel"></p>
                                <table class="stripe" id="defecttabletitle" style="width:100%" border="0" cellspacing="0" cellpadding="0">
                                    <thead> 
                                    </thead> 
                                    <tbody>
                                    </tbody>
                                </table>
                                <table class="stripe" id="defecttable" style="width:100%" border="0" cellspacing="0" cellpadding="0">
                                    <thead> 
                                    </thead> 
                                    <tbody>
                                    </tbody>
                                </table>
                            </td>
                            <td style="width:3%"></td>
                        </tr>
                    </table>
                </div>
            </div>
            
        </div>

        <div data-role="footer" data-position="fixed" data-fullscreen="true">
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
                <a id="bundlereducea" href=" " data-ajax="false"></a>
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
</body>
</html>
