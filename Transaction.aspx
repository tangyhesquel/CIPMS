<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Transaction.aspx.cs" Inherits="Flow_test" %>

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
    <script src="js/jquery.jqprint-0.3.js" type="text/javascript"></script>
    <script src="js/jquery-barcode.js" type="text/javascript"></script>
    <script src="js/jquery.simplePagination.js" type="text/javascript"></script>
    <link href="css/simplePagination.css" rel="stylesheet" type="text/css" />
    <script src="js/qrcode.js" type="text/javascript"></script>
    <script src="js/Dialog.js" type="text/javascript"></script>

    <title></title>
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
        .d1 
        {
            border-style:dotted;
        }
        .borderTable, .borderTable td
        {
            border-width: 1px;
            border-style: dotted;
            border-collapse: collapse;
        }
        .NoBottomBorder
        {
            border-bottom-style:none;
        }
    </style>

    <script type="text/javascript">

        var fromfactoryselected = "";
        var fromprocessselected = "";
        var fromproductionselcted = "";
        var fty = "";
        var globeltranslate; //保存json翻译的字符串
        var select;
        var factoryselect;
        var productionselect;
        var processselect;
        var contractnoselect;
        var contractselect = '0';//contract未选
        var processselected = '2'; //process未选
        var processtypeselected = '0'; //process_type为I
        var bundlescanstatus = '2'; //bundle list为空
        var productionlineselected = '0'; //production未选
        var radioselected = '0'; //0为初始值，123分别为transaction、rework、adjustment
        var reprintstatus = '0';
        var doc_no = '';
        var environment = "";
        var surebutton = "";
        var arr; //全局定义查询结果数组
        var pagenum;
        var currentpagenum = 1;

        //打印参数
        var printhtml = "";
        var cartonbarcode = "";
        var pdbutton = "";
        var pdtitle = "";
        var printwidth = 1;
        var printheight = 40;

        var barcodetype;
        var tosew = "false";
        var employeeno = sessionStorage.getItem("employeeno");
        var factory = sessionStorage.getItem("factory");
        var userbarcode = sessionStorage.getItem("userbarcode");
        var firstflag = 'T';//20170615-tangyh
    
        //语言翻译函数
        window.language;//全局变量，保存用户选择了哪个json语言文件

        function translate() {
           // alert(window.language);
            $.ajax({
                type: "GET",
                url: window.language,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                async: false,
                cache: false,
                error: function () {
                    alert("Sorry! Pls check your network and refresh your brower! Wrong Code::001");
                },
                success: function (json) {
                    globeltranslate = eval(json);
                    pdbutton = globeltranslate.pdbutton;
                    pdtitle = globeltranslate.pdtitle;
                    $('#headertitle').html(globeltranslate.headertitle);
                    $('#cancelbutton').html(globeltranslate.cancelbutton);
                    $('#functionmenubutton').html(globeltranslate.functionmenubutton);
                    $('#barcodelabel').html(globeltranslate.barcodelabel);
                    $('#processtypelabel').html(globeltranslate.processtypelabel);
                    $('#contractnolabel').html(globeltranslate.contractnolabel);
                    $('#fromfactoryselectlabel').html(globeltranslate.fromfactoryselectlabel);
                    $('#nextfactorylabel').html(globeltranslate.nextfactorylabel);
                    $('#nextprocesslabel').html(globeltranslate.nextfactorylabel);
                    $('#partlabel').html(globeltranslate.partlabel);
                   
                    $('#totalbundlelabel').html(globeltranslate.totalbundlelabel);
                    $('#totalpcslabel').html(globeltranslate.totalpcslabel);
                    $('#totalgarmentpcslabel').html(globeltranslate.totalgarmentpcslabel);
                    $('#totalpartlabel').html(globeltranslate.totalpartlabel);

                    $('#messageth').html(globeltranslate.messageth);
                    $('#barcodelistlabel').html(globeltranslate.barcodelistlabel);
                    $('#adjustmentradiolabel').html(globeltranslate.adjustmentradiolabel);
                    $('#reworkradiolabel').html(globeltranslate.reworkradiolabel);
                    $('#transactionradiolabel').html(globeltranslate.transactionradiolabel);

                    $('#confirmbutton').val(globeltranslate.confirmbutton).button('refresh');
                    $('#submitbutton').val(globeltranslate.submitbutton).button('refresh');
                    $('#unsubmitbutton').val(globeltranslate.unsubmitbutton).button('refresh');
                    $('#reprintbutton').val(globeltranslate.reprintbutton).button('refresh');
                    $('#savebutton').val(globeltranslate.savebutton).button('refresh');
                    $('#emptylistbutton').val(globeltranslate.emptylistbutton).button('refresh');

                    //$('#confirmbutton').html(globeltranslate.confirmbutton);
                    //$('#submitbutton').html(globeltranslate.submitbutton);
                    //$('#unsubmitbutton').html(globeltranslate.unsubmitbutton);
                    //$('#reprintbutton').html(globeltranslate.reprintbutton);
                    //$('#savebutton').html(globeltranslate.savebutton);
                    //$('#emptylistbutton').html(globeltranslate.emptylistbutton);


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
                    $('#carnolabel').html(globeltranslate.carnolabel);
                    select = globeltranslate.select;
                    $('#printsparebarcodea').html(globeltranslate.printsparebarcodea);
                }
            });
        };

        function defecttrans() {
            $('#headertitle1').html(globeltranslate.headertitle1);
            $('#reasonselect').append("<option>" + globeltranslate.reasonselect + "</option>");
            $('#barcodelabel1').html(globeltranslate.barcodelabel1);
            $('#partlabel1').html(globeltranslate.partlabel);
            $('#existingqytlabel').html(globeltranslate.existingqytlabel);
            $('#qtylabel').html(globeltranslate.qtylabel);
            $('#reasonlabel').html(globeltranslate.reasonlabel);
            $('#reducetablelabel').html(globeltranslate.reducetablelabel);
            //加载表头
            $("#reducetable thead tr").empty();
            $('#reducetable thead').append("<tr><th>" + globeltranslate.defecttabletitle1 + "</th><th>" + globeltranslate.defecttabletitle2 + "</th><th>" + globeltranslate.defecttabletitle3 + "</th><th>" + globeltranslate.defecttabletitle4 + "</th><th>" + globeltranslate.defecttabletitle5 + "</th></tr>");
            $('#reducetable').trigger('create');
        }

        function partselecttrans() {
            $('#partselect').empty();
            $('#partselect').append("<option>" + globeltranslate.select + "</option>");
            $('#partselect').selectmenu('refresh');
        }
        function queryselecttrans() {
            $('#querybutton').empty();
            $('#querybutton').append("<option value='selectdefault'>" + globeltranslate.querybutton + "</option>");
            $('#querybutton').selectmenu('refresh');
        }
        
        function contractselecttrans() {
            $('#contractnoselect').empty();
            $('#contractnoselect').append("<option>" + globeltranslate.select + "</option>");
            $('#contractnoselect').selectmenu('refresh');
            contractnoselect = globeltranslate.reasonselect;
        }
        function processtypetrans() {
            $('#processtypeselect').empty();
            $('#processtypeselect').append("<option>" + globeltranslate.select + "</option>");

            $('#processtypeselect').append("<option value='I' selected='selected'>I</option>");
            if (sessionStorage.getItem("process") == "DC" || sessionStorage.getItem("process") == "SEW") {
                $('#processtypeselect').append("<option value='P'>P</option>");
                $('#processtypeselect').append("<option value='E'>E</option>");
                $('#processtypeselect').append("<option value='O'>O</option>");
                //tangyh 2017.03.31
                $('#processtypeselect').append("<option value='T'>T</option>");
            }
            $('#processtypeselect').selectmenu('refresh');
        }
        function factoryselecttrans() {
            $('#nextfactoryselect').empty();
            $('#nextfactoryselect').append("<option>" + globeltranslate.nextfactoryselect + "</option>");
            $('#nextfactoryselect').selectmenu('refresh');
            factoryselect = globeltranslate.nextfactoryselect;
        }
        function processselecttrans() {
            $('#nextprocessselect').empty();
            $('#nextprocessselect').append("<option>" + globeltranslate.nextprocessselect + "</option>");
            $('#nextprocessselect').selectmenu('refresh');
            processselect = globeltranslate.nextprocessselect;
        }
        function productionselecttrans() {
            $('#nextproductionlineselect').empty();
            $('#nextproductionlineselect').append("<option>" + globeltranslate.nextproductionlineselect + "</option>");
            $('#nextproductionlineselect').selectmenu('refresh');
            productionselect = globeltranslate.nextproductionlineselect;
        }
        function fromfactoryselecttrans() {
            $('#fromfactoryselect').empty();
            $('#fromfactoryselect').append("<option>" + globeltranslate.nextfactoryselect + "</option>");
            $('#fromfactoryselect').selectmenu('refresh');
            fromfactoryselected = globeltranslate.nextfactoryselect;
        }
        function fromprocessselecttrans() {
            $('#fromprocessselect').empty();
            $('#fromprocessselect').append("<option>" + globeltranslate.nextprocessselect + "</option>");
            $('#fromprocessselect').selectmenu('refresh');
            fromprocessselected = globeltranslate.nextprocessselect;
        }
        function fromproductionselecttrans() {
            $('#fromproductionselect').empty();
            $('#fromproductionselect').append("<option>" + globeltranslate.nextproductionlineselect + "</option>");
            $('#fromproductionselect').selectmenu('refresh');
            fromproductionselcted = globeltranslate.nextproductionlineselect;
        }

        function query() {
            $.ajax({
                type: "POST",
                url: "Package.aspx/Query",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + userbarcode + "', 'functioncd': 'Transaction' }",
                success: function (data) {
                    queryselecttrans();
                    $('#querybutton').append(data.d);
                    $('#querybutton').selectmenu('refresh');
                },
                error: function (err) {
                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "009</td></tr>");
                    $('#messagetable').trigger('create');
                    alert(globeltranslate.errormessage + "009");
                }
            });
        }

        

        function empty() {
            arr = [];
            $('#selector').pagination({
                cssStyle: 'light-theme',
                prevText: globeltranslate.prevbutton,
                nextText: globeltranslate.nextbutton
            });
            doc_no = '';
            $('#parttr').show();
            $("#barcodetabletitle thead").empty();
            $('#barcodetabletitle thead').append("<tr><th>" + globeltranslate.tabletranslation1 + "</th><th>" + globeltranslate.tabletranslation2 + "</th><th>" + globeltranslate.tabletranslation3 + "</th><th>" + globeltranslate.tabletranslation4 + "</th><th>" + globeltranslate.tabletranslation5 + "</th><th>" + globeltranslate.tabletranslation6 + "</th><th>" + globeltranslate.tabletranslation7 + "</th><th>" + globeltranslate.tabletranslation8 + "</th><th>" + globeltranslate.tabletranslation9 + "</th><th>" + globeltranslate.tabletranslation10 + "</th><th>" + globeltranslate.tabletranslation11 + "</th><th>" + globeltranslate.tabletranslation12 + "</th><th>" + globeltranslate.tabletranslation13 + "</th><th></th><th></th></tr>");
            $('#barcodetabletitle').trigger('create');
            if ($("input[name='radio']:checked").val() == 'adjustmentradio') {
                factoryselecttrans();
                processselecttrans();
                productionselecttrans();
                $('#processtypeselect').empty();
                $('#processtypeselect').append("<option select='selected'>" + globeltranslate.select + "</option>");
                $('#processtypeselect').selectmenu('refresh');
                selectcontrol('1', '0', '1');
            } else {
                processtypetrans();
                contractselecttrans();
                $("#nextprocessselect").removeAttr("disabled").selectmenu('refresh');
                if (radioselected == '1') {
                    iprocess('true');
                } else if (radioselected == '2') {
                    iprocess('false');
                }
                selectcontrol('0', '0', '0');
            }
            fromfactoryselecttrans();
            fromprocessselecttrans();
            fromproductionselecttrans();
            $("input[name='part_list']").prop("checked", false).checkboxradio('refresh');
            $('#barcodetext').val("");
            $('#barcodetext').focus();
            barcodescan = "";
            btnstatuscontrol(1);
            $('#totalbundlediv').html("0");
            $('#totalpcsdiv').html("0");
            $('#totalgarmentpcs').html("0");
            processselect = "";
            productionselect = "";
            contractselect = '0';
            bundlescanstatus = '2';
            btninit();
            $('#barcodelisttable tbody').empty();
            $('#barcodelisttable').trigger('create');
            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.emptymessage + "</td></tr>");
            $('#messagetable').trigger('create');
            $('#carno').val("");
        }

        //20170615-tangyh        
        function iprocess(flowtype) {
            $('#nextfactoryselect').empty();
            factoryselecttrans();
            //20170615-tangyh
            //$('#nextfactoryselect').append("<option selected='selected' value='" + sessionStorage.getItem("factory") + "'>" + sessionStorage.getItem("factory") + "</option>");
            $('#nextfactoryselect').append("<option selected='selected' value='" + fty + "'>" + fty + "</option>");

            $('#nextfactoryselect').selectmenu('refresh');
            $.ajax({
                type: "POST",
                url: "Transaction.aspx/Process",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '"+sessionStorage.getItem("process")+"', 'flowtype': '" + flowtype + "' }",
                success: function(data) {
                    $('#nextprocessselect').empty();
                    processselecttrans();
                    $('#nextprocessselect').selectmenu('refresh');
                    $('#nextproductionlineselect').empty();
                    productionselecttrans();
                    $('#nextproductionlineselect').selectmenu('refresh');
                    $('#nextprocessselect').append(data.d);
                    $('#nextprocessselect').selectmenu('refresh');
                    processselected = '2';
                },
                error: function(err) {
                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "002</td></tr>");
                    $('#messagetable').trigger('create');
                    alert(globeltranslate.errormessage + "002");
                }
            });
        }

        function pprocess() {
            $('#nextfactoryselect').empty();
            factoryselecttrans();
            $('#nextfactoryselect').selectmenu('refresh');
            $('#nextprocessselect').empty();
            processselecttrans();
            $('#nextprocessselect').selectmenu('refresh');
            $('#nextproductionlineselect').empty();
            productionselecttrans();
            $('#nextproductionlineselect').selectmenu('refresh');
            $.ajax({
                type: "POST",
                url: "Transaction.aspx/Peerfactory",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "' }",
                success: function(data) {
                    $('#nextfactoryselect').append(data.d);
                    $('#nextfactoryselect').selectmenu('refresh');
                    processselected = '2';
                },
                error: function(err) {
                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "003</td></tr>");
                    $('#messagetable').trigger('create');
                    alert(globeltranslate.errormessage + "003");
                }
            });
        }

        function partselect() {
            //加载part select
            $.ajax({
                type: "POST",
                url: "Package.aspx/Part",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "' }",
                success: function (data) {
                    partselecttrans();
                    $('#partselect').append(data.d);
                    $('#partselect').selectmenu('refresh');
                },
                error: function (err) {
                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "008</td></tr>");
                    $('#messagetable').trigger('create');
                    alert(globeltranslate.errormessage + "008");
                }
            });
        }

        //confirm和submit和unsubmit按钮的控制函数
        function btnstatus() {
//            if (radioselected == '3') {
//                if (sessionStorage.getItem("process") != 'CUT' && bundlescanstatus == '1')
//                    btnstatuscontrol(4);
//                else
//                    btnstatuscontrol(1);
//            } else {
            if (processtypeselected == '1') {//0为I，1为P，2为O/E
                if (processselected == '2') {//0为下道部门不用接收，1为下道部门需要接收，2为未选下道部门
                    btnstatuscontrol(1);
                } else {
                    if (bundlescanstatus == '0') {//0为bundle part不在本道部门或者箱内bundle扫描不全，1满足流转条件，2为bundle list扫描表为空
                        btnstatuscontrol(1);
                    } else if (bundlescanstatus == '1') {
                        btnstatuscontrol(3);
                    } else {
                        btnstatuscontrol(1);
                    }
                }
            } else if (processtypeselected == '0') {
                if (processselected == '2') {
                    btnstatuscontrol(1);
                } else if (processselected == '0') {
                    if (bundlescanstatus == '0') {
                        btnstatuscontrol(1);
                    } else if (bundlescanstatus == '1') {
                        btnstatuscontrol(4);
                    } else {
                        btnstatuscontrol(1);
                    }
                } else {
                    if (bundlescanstatus == '0') {
                        btnstatuscontrol(1);
                    } else if (bundlescanstatus == '1') {
                        btnstatuscontrol(3);
                    } else {
                        btnstatuscontrol(1);
                    }
                }
            } else {
                if (bundlescanstatus == '0') {
                    btnstatuscontrol(1);
                } else if (bundlescanstatus == '1' && contractselect == '1') {
                    btnstatuscontrol(4);
                } else {
                    btnstatuscontrol(1);
                }
            }
            //}
        }

        function btnstatuscontrol(temp) {
            if (reprintstatus == '0') {
                $('#reprintbutton').attr('disabled', 'true').button('refresh');
            } else {
                $('#reprintbutton').removeAttr('disabled').button('refresh');
            }
            if (doc_no == '') {
                $('#unsubmitbutton').attr('disabled', 'true').button('refresh');
            } else { //tangyh 2017.04.06
                if (barcodetype == 'D' && tosew == 'true') {
                    $('#unsubmitbutton').removeAttr('disabled').button('refresh');
                }
            }
            if (temp == '1') {//全部禁用
                $('#confirmbutton').attr('disabled', 'true').button('refresh');
                $('#submitbutton').attr('disabled', 'true').button('refresh');
            } else if (temp == '2') {//全部激活
                $('#confirmbutton').removeAttr('disabled').button('refresh');
                $('#submitbutton').removeAttr('disabled').button('refresh');
            } else if (temp == '3') {//激活submit，禁用confirm
                $('#submitbutton').removeAttr('disabled').button('refresh');
                $('#confirmbutton').attr('disabled', 'true').button('refresh');
            } else if (temp == '4') {//激活confirm，禁用submit
                $('#confirmbutton').removeAttr('disabled').button('refresh');
                $('#submitbutton').attr('disabled', 'true').button('refresh');
            }
        }

        function btninit() {
            contractselect = '0'; //contract未选
            processselected = '2'; //process未选
            processtypeselected = '0'; //process_type为I
            bundlescanstatus = '2'; //bundle list为空
            productionlineselected = '0'; //production未选
            reprintstatus = '0';
            btnstatus();
            bundlescanstatus = '2';
        }

        function selectcontrol(temp1, temp2, temp3) {
            $('#fromfactoryselect').attr('disabled', 'disabled');
            $('#fromprocessselect').attr('disabled', 'disabled');
            $('#fromproductionselect').attr('disabled', 'disabled');
            if (temp1 == '1') {//已扫描，禁用
                $('#processtypeselect').attr("disabled", "disabled");
                $('#nextfactoryselect').attr("disabled", "disabled");
                $('#nextprocessselect').attr("disabled", "disabled");
            } else if (temp1 == '0') {
                $('#processtypeselect').removeAttr("disabled");
                $('#nextfactoryselect').removeAttr("disabled");
                $('#nextprocessselect').removeAttr("disabled");
            }
            if (temp2 == '1') {
                $("input[name='radio']").attr("disabled", "disabled");
            } else if (temp2 == '0') {
                $("input[name='radio']").removeAttr("disabled");
            }
            if (temp3 == '1') {
                $('#nextproductionlineselect').attr("disabled", "disabled");
            } else if (temp3 == '0') {
                $('#nextproductionlineselect').removeAttr("disabled");
            }
        }

        //表格里的defect按钮
        var bundlebarcode = "";
        var bundlepart = "";
        var bundleqty = "";
        var bundleid = "";
        function defectbtn(obj) {
            bundlebarcode =  $(obj).parents('tr').children('td').eq(2).text();
            bundlepart = $(obj).parents('tr').children('td').eq(3).text();
            bundleqty = $(obj).parents('tr').children('td').eq(10).text();
            bundleid = $(obj).closest('tr').attr('id');
        }

        //表格里的删除按钮监控
        function deletebtn(obj) {
            var carton = $(obj).parents('tr').children('td').eq(0).text();
            bundlebarcode = $(obj).parents('tr').children('td').eq(2).text();
            if (carton == '') {
                $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                setTimeout(function () {
                    $.ajax({
                        type: "POST",
                        url: "Transaction.aspx/Delete",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        //async: false,
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'nextprocess': '" + $('#nextprocessselect').val() + "', 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'id': '" + $(obj).closest('tr').attr('id') + "', 'bundlebarcode': '" + bundlebarcode + "' }",
                        success: function (data) {
                            $.mobile.loading('hide');
                            if (data.d != "false1") {
                                var result = $.parseJSON(data.d);

                                $('#totalbundlediv').html(result.bundlestatus[0].TOTALBUNDLES);
                                $('#totalpcsdiv').html(result.bundlestatus[0].TOTALPCS);
                                $('#totalgarmentpcs').html(result.bundlestatus[0].TOTALGARMENTPCS);
                                $('#totalpartdiv').html(result.bundlestatus[0].TOTALPART);

                                if (result.bundlestatus[0].INPROCESS == 'true' && result.bundlestatus[0].ALL == 'true' && result.bundlestatus[0].ISSUBMIT == 'false') {
                                    bundlescanstatus = '1';
                                } else {
                                    bundlescanstatus = '0';
                                }
                                if (result.bundlestatus[0].CANREPRINT == 'true')
                                    reprintstatus = '1';
                                else
                                    reprintstatus = '0';
                                if (result.bundlestatus[0].BYBUNDLE == 'false') {
                                    //$(obj).closest('tr').remove();
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.deletemessage1 + $(obj).parents('tr').children('td').eq(2).text() + globeltranslate.s + $(obj).parents('tr').children('td').eq(3).text() + "</td></tr>");
                                    $('#messagetable').trigger('create');
                                    var html = result.bundlestatus[0].HTML;
                                    arr = html.split('@');
                                    pagenum = Math.ceil(arr.length / 10);
                                    if (currentpagenum > pagenum) {
                                        currentpagenum = pagenum;
                                    }
                                    html = "";
                                    $('#barcodelisttable tr').empty();
                                    html += tablelogic(arr, currentpagenum, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'false');
                                    $('#barcodelisttable tbody').append(html);
                                    $('#barcodelisttable').trigger('create');

                                    $('#selector').pagination({
                                        items: arr.length,
                                        itemsOnPage: 10,
                                        cssStyle: 'light-theme',
                                        currentPage: currentpagenum,
                                        prevText: globeltranslate.prevbutton,
                                        nextText: globeltranslate.nextbutton,
                                        onPageClick: function (pagenumber, event) {
                                            currentpagenum = pagenumber;
                                            $('#barcodelisttable tbody').empty();
                                            html = "";
                                            html += tablelogic(arr, currentpagenum, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'false');
                                            $('#barcodelisttable tbody').append(html);
                                            $('#barcodelisttable').trigger('create');
                                        }
                                    });

                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                        if (i <= 12)
                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                    });
                                } else {
                                    //$('#barcodelisttable tr').each(function () {
                                    //if ($(this).children('td').eq(2).text() == bundlebarcode)
                                    //$(this).remove();
                                    //$('#barcodelisttable').trigger('create');
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.deletemessage1 + "</td></tr>");
                                    $('#messagetable').trigger('create');
                                    //});

                                    var html = result.bundlestatus[0].HTML;
                                    arr = html.split('@');
                                    pagenum = Math.ceil(arr.length / 10);
                                    if (currentpagenum > pagenum) {
                                        currentpagenum = pagenum;
                                    }
                                    html = "";
                                    $('#barcodelisttable tr').empty();
                                    html += tablelogic(arr, currentpagenum, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'true');
                                    $('#barcodelisttable tbody').append(html);
                                    $('#barcodelisttable').trigger('create');

                                    $('#selector').pagination({
                                        items: arr.length,
                                        itemsOnPage: 10,
                                        cssStyle: 'light-theme',
                                        currentPage: currentpagenum,
                                        prevText: globeltranslate.prevbutton,
                                        nextText: globeltranslate.nextbutton,
                                        onPageClick: function (pagenumber, event) {
                                            currentpagenum = pagenumber;
                                            $('#barcodelisttable tbody').empty();
                                            html = "";
                                            html += tablelogic(arr, currentpagenum, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'true');
                                            $('#barcodelisttable tbody').append(html);
                                            $('#barcodelisttable').trigger('create');
                                        }
                                    });

                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                        if (i <= 10)
                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                    });
                                }

                            } else {//bundle list已删空
                                arr = [];
                                $('#selector').pagination({
                                    cssStyle: 'light-theme',
                                    prevText: globeltranslate.prevbutton,
                                    nextText: globeltranslate.nextbutton
                                });
                                bundlescanstatus = '2';
                                $('#barcodelisttable tbody').empty();
                                $('#totalbundlediv').html("0");
                                $('#totalpcsdiv').html("0");
                                $('#totalgarmentpcs').html("0");
                                fromfactoryselecttrans();
                                fromprocessselecttrans();
                                fromproductionselecttrans();
                                if (radioselected == '3') {
                                    factoryselecttrans();
                                    processselecttrans();
                                    productionselecttrans();
                                    $('#processtypeselect').empty();
                                    $('#processtypeselect').append("<option select='selected'>" + globeltranslate.select + "</option>");
                                    $('#processtypeselect').selectmenu('refresh');
                                    selectcontrol('1', '0', '1');
                                } else {
                                    selectcontrol('0', '0', '0');
                                }
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.deletemessage3 + "</td></tr>");
                                $('#messagetable').trigger('create');
                            }
                            btnstatus();
                        },
                        error: function (err) {
                            $.mobile.loading('hide');
                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "004</td></tr>");
                            $('#messagetable').trigger('create');
                            alert(globeltranslate.errormessage + "004");
                        }
                    });
                }, 10);
            }
            else {
                $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                setTimeout(function () {
                    $.ajax({
                        type: "POST",
                        url: "Transaction.aspx/Deletecarton",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        //async: false,
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'nextprocess': '" + $('#nextprocessselect').val() + "', 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'cartonbarcode': '" + carton + "' }",
                        success: function (data) {
                            $.mobile.loading('hide');
                            if (data.d != "false1") {
                                var result = $.parseJSON(data.d);
                                $('#totalbundlediv').html(result.bundlestatus[0].TOTALBUNDLES);
                                $('#totalpcsdiv').html(result.bundlestatus[0].TOTALPCS);
                                $('#totalgarmentpcs').html(result.bundlestatus[0].TOTALGARMENTPCS);
                                $('#totalpartdiv').html(result.bundlestatus[0].TOTALPART);

                                if (result.bundlestatus[0].INPROCESS == 'true' && result.bundlestatus[0].ALL == 'true' && result.bundlestatus[0].ISSUBMIT == 'false') {
                                    bundlescanstatus = '1';
                                } else {
                                    bundlescanstatus = '0';
                                }
                                if (result.bundlestatus[0].CANREPRINT == 'true')
                                    reprintstatus = '1';
                                else
                                    reprintstatus = '0';

                                if (result.bundlestatus[0].BYBUNDLE == 'false') {
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.deletemessage1 + $(obj).parents('tr').children('td').eq(2).text() + globeltranslate.s + $(obj).parents('tr').children('td').eq(3).text() + "</td></tr>");
                                    $('#messagetable').trigger('create');
                                    var html = result.bundlestatus[0].HTML;
                                    arr = html.split('@');
                                    pagenum = Math.ceil(arr.length / 10);
                                    if (currentpagenum > pagenum) {
                                        currentpagenum = pagenum;
                                    }
                                    html = "";
                                    $('#barcodelisttable tr').empty();
                                    html += tablelogic(arr, currentpagenum, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'false');
                                    $('#barcodelisttable tbody').append(html);
                                    $('#barcodelisttable').trigger('create');

                                    $('#selector').pagination({
                                        items: arr.length,
                                        itemsOnPage: 10,
                                        cssStyle: 'light-theme',
                                        currentPage: currentpagenum,
                                        prevText: globeltranslate.prevbutton,
                                        nextText: globeltranslate.nextbutton,
                                        onPageClick: function (pagenumber, event) {
                                            currentpagenum = pagenumber;
                                            $('#barcodelisttable tbody').empty();
                                            html = "";
                                            html += tablelogic(arr, currentpagenum, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'false');
                                            $('#barcodelisttable tbody').append(html);
                                            $('#barcodelisttable').trigger('create');
                                        }
                                    });

                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                        if (i <= 12)
                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                    });
                                } else {
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.deletemessage1 + "</td></tr>");
                                    $('#messagetable').trigger('create');

                                    var html = result.bundlestatus[0].HTML;
                                    arr = html.split('@');
                                    pagenum = Math.ceil(arr.length / 10);
                                    if (currentpagenum > pagenum) {
                                        currentpagenum = pagenum;
                                    }
                                    html = "";
                                    $('#barcodelisttable tr').empty();
                                    html += tablelogic(arr, currentpagenum, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'true');
                                    $('#barcodelisttable tbody').append(html);
                                    $('#barcodelisttable').trigger('create');

                                    $('#selector').pagination({
                                        items: arr.length,
                                        itemsOnPage: 10,
                                        cssStyle: 'light-theme',
                                        currentPage: currentpagenum,
                                        prevText: globeltranslate.prevbutton,
                                        nextText: globeltranslate.nextbutton,
                                        onPageClick: function (pagenumber, event) {
                                            currentpagenum = pagenumber;
                                            $('#barcodelisttable tbody').empty();
                                            html = "";
                                            html += tablelogic(arr, currentpagenum, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'true');
                                            $('#barcodelisttable tbody').append(html);
                                            $('#barcodelisttable').trigger('create');
                                        }
                                    });

                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                        if (i <= 10)
                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                    });
                                }

                            } else {//bundle list已删空
                                arr = [];
                                $('#selector').pagination({
                                    cssStyle: 'light-theme',
                                    prevText: globeltranslate.prevbutton,
                                    nextText: globeltranslate.nextbutton
                                });
                                bundlescanstatus = '2';
                                fromfactoryselecttrans();
                                fromprocessselecttrans();
                                fromproductionselecttrans();
                                if (radioselected == '3') {
                                    factoryselecttrans();
                                    processselecttrans();
                                    productionselecttrans();
                                    $('#processtypeselect').empty();
                                    $('#processtypeselect').append("<option select='selected'>" + globeltranslate.select + "</option>");
                                    $('#processtypeselect').selectmenu('refresh');
                                    selectcontrol('1', '0', '1');
                                } else {
                                    selectcontrol('0', '0', '0');
                                }
                                $('#totalbundlediv').html("0");
                                $('#totalpcsdiv').html("0");
                                $('#totalgarmentpcs').html("0");
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.deletemessage3 + "</td></tr>");
                                $('#messagetable').trigger('create');
                            }
                            btnstatus();
                            $('#barcodelisttable tr').each(function () {
                                if ($(this).children('td').eq(0).text() == carton)
                                    $(this).remove();
                                $('#barcodelisttable').trigger('create');
                            });
                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.deletemessage2 + $(obj).parents('tr').children('td').eq(0).text() + "</td></tr>");
                            $('#messagetable').trigger('create');
                        },
                        error: function (err) {
                            $.mobile.loading('hide');
                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "006</td></tr>");
                            $('#messagetable').trigger('create');
                            alert(globeltranslate.errormessage + "006");
                        }
                    });
                }, 10);
            }
        }

        

        $(function () {
            //20170615-tangyh
            fty = getUrlParam('FTY');
            environment = getUrlParam('svTYPE');
            firstflag = 'T';
            start(firstflag,fty);
        });


        function start(firstflag,fty4)
        {
            var isokaccountid = checkaccountid(firstflag, fty4, userbarcode, environment);
            if (isokaccountid == "F") {
                return;
            }

            if (fty4 == null || environment == null) {
                alert('The url is wrong! Pls input like 192.168.x.x/CIPMS/Login.aspx?FTY=GEG&svTYPE=PROD');
            } else {
                var d = new Date();
                var vYear = d.getFullYear()
                var vMon = d.getMonth() + 1;
                var vDay = d.getDate();
                var h = d.getHours();
                var m = d.getMinutes();
                var se = d.getSeconds();
                docno = sessionStorage.getItem("userbarcode") + vYear + vMon + vDay + h + m + se;

                //调用语言翻译函数
                if (window.localStorage.languageid == "1") {
                    window.language = "Transaction/Transaction-zh-CN.js";
                } else if (window.localStorage.languageid == "2") {
                    window.language = "Transaction/Transaction-en-US.js";
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
                    Loginout(fty4, environment);
                }
                else {
                    contractselecttrans();
                    factoryselecttrans();
                    processselecttrans();
                    productionselecttrans();
                    processtypetrans();
                    fromfactoryselecttrans();
                    fromprocessselecttrans();
                    fromproductionselecttrans();
                    $('#nextprocesslabel').hide();
                    $('#contractnoselect').selectmenu('refresh');
                    $('#querybutton').selectmenu('refresh');
                    $('#nextfactoryselect').selectmenu('refresh');
                    $('#nextprocessselect').selectmenu('refresh');
                    $('#nextproductionlineselect').selectmenu('refresh');
                    //function权限控制
                    Accessmodulehide();
                    $('#transactiondiv').hide();
                    $('#reworkdiv').hide();
                    $('#adjustmentdiv').hide();
                    $('#contractnotr').hide();
                    $('#totalbundlediv').html("0");
                    $('#totalpcsdiv').html("0");
                    $('#totalgarmentpcs').html("0");
                    $('#printtest').hide();
                    $('#messagetitle').html(sessionStorage.getItem("name") + " , " + sessionStorage.getItem("factory") + "/" + sessionStorage.getItem("process") + "/" + sessionStorage.getItem("productionline"));
                    //var employeeno = sessionStorage.getItem("employeeno");
                    //var factory = sessionStorage.getItem("factory");
                    //var userbarcode = sessionStorage.getItem("userbarcode");

                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                    $.ajax({
                        type: "POST",
                        url: "Package.aspx/Accessfunction",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        async: false,
                        data: "{ 'factory': '" + fty4 + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + userbarcode + "' }",
                        success: function (data) {
                            $.mobile.loading('hide');
                            //判断用户的权限按钮
                            var result = eval(data.d);
                             $(result).each(function (i) {
                                    if (result[i].FUNCTION_ENG == 'Transaction') {
                                        $('#transactiondiv').show();
                                        radioselected = '1';
                                        $("input[name='radio'][value='transaction']").attr('checked', true).checkboxradio('refresh');
                                    }
                                    else if (result[i].FUNCTION_ENG == 'Rework') {
                                        $('#reworkdiv').show();
                                        if (radioselected == '0') {
                                            radioselected = '2';
                                            $("input[name='radio'][value='reworkradio']").attr('checked', true).checkboxradio('refresh');
                                        }
                                    }
                                    else if (result[i].FUNCTION_ENG == 'Adjustment') {
                                        $('#adjustmentdiv').show();
                                        if (radioselected == '0') {
                                            radioselected = '3';
                                            $("input[name='radio'][value='adjustmentradio']").attr('checked', true).checkboxradio('refresh');
                                        }
                                    }
                                    btninit();
                                });
                      
                            //判断用户的权限模块
                            Accessmoduleshow(data.d);
                        },
                        error: function (err) {
                            $.mobile.loading('hide');
                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "007</td></tr>");
                            $('#messagetable').trigger('create');
                            alert(globeltranslate.errormessage + "007");
                        }
                    });

                    //加载processtype="I"时的process
                    if (radioselected == '1') {
                        iprocess('true');
                    } else if (radioselected == '2') {
                        //加载上道部门
                        iprocess('false');
                    }

                    partselect();
                    //query加载
                    query();

                    //20170615-tangyh
                    queryfactory(firstflag,fty,'li');

                }

                //表格样式
                var screenwidth = $(document.body).width();
                var screenheight = $(document.body).height();
                $('#barcodelisttable').chromatable({
                    width: screenwidth * 0.96,
                    height: screenheight * 0.7,
                    scrolling: "yes"
                });
                $('#messagetable').chromatable({
                    width: screenwidth * 0.17,
                    height: screenheight * 0.23,
                    scrolling: "yes"
                });
                $('#reducetable').chromatable({
                    width: screenwidth,
                    height: screenheight * 0.5,
                    scrolling: "yes"
                });

                //加载表头
                $("#barcodetabletitle thead tr").empty();
                $('#barcodetabletitle thead').append("<tr><th>" + globeltranslate.tabletranslation1 + "</th><th>" + globeltranslate.tabletranslation2 + "</th><th>" + globeltranslate.tabletranslation3 + "</th><th>" + globeltranslate.tabletranslation4 + "</th><th>" + globeltranslate.tabletranslation5 + "</th><th>" + globeltranslate.tabletranslation6 + "</th><th>" + globeltranslate.tabletranslation7 + "</th><th>" + globeltranslate.tabletranslation8 + "</th><th>" + globeltranslate.tabletranslation9 + "</th><th>" + globeltranslate.tabletranslation10 + "</th><th>" + globeltranslate.tabletranslation11 + "</th><th>" + globeltranslate.tabletranslation12 + "</th><th>" + globeltranslate.tabletranslation13 + "</th><th></th><th></th></tr>");
                $('#barcodetabletitle').trigger('create');

                

                //Process type下拉框监控
                $('#processtypeselect').change(function () {
                    $("#nextprocessselect").removeAttr("disabled").selectmenu('refresh');
                    if ($(this).val() == 'I') {
                        $("#nextfactoryselect").removeAttr("disabled");
                        $('#nextfactorytr').show();
                        $('#transtypetr').show();
                        $('#productiontr').show();
                        $('#contractnotr').hide();
                        $('#nextprocesslabel').hide();
                        $('#nextfactoryselect').empty();
                        factoryselecttrans();
                        if (radioselected == '1') {
                            iprocess('true');
                        } else if (radioselected == '2') {
                            iprocess('false');
                        }
                        processtypeselected = '0';
                    } else if ($(this).val() == 'P') {
                        $("#nextfactoryselect").removeAttr("disabled");
                        $('#nextfactorytr').show();
                        $('#transtypetr').show();
                        $('#productiontr').hide();
                        $('#productionlineselect').hide();
                        $('#nextprocesslabel').hide();
                        $('#contractnotr').hide();
                        processtypeselected = '1';
                        pprocess();
                    } else if ($(this).val() == 'O' || $(this).val() == 'E') {
                        $('#nextfactorytr').hide();
                        $('#transtypetr').hide();
                        $('#productiontr').hide();
                        $('#contractnotr').show();
                        $('#nextprocesslabel').show();
                        //$('#nextprocessselect').empty();
                        //processselecttrans();
                        //$('#nextprocessselect').selectmenu('refresh');
                        processtypeselected = '2';
                    }
                    btnstatus();
                });

                //工厂下拉框改变选择
                $('#nextfactoryselect').change(function () {
                    $('#nextprocessselect').removeAttr("disabled").selectmenu('refresh');
                    $('#nextprocessselect').empty();
                    processselecttrans();
                    $('#nextprocessselect').selectmenu('refresh');
                    $('#nextproductionlineselect').empty();
                    productionselecttrans();
                    $('#nextproductionlineselect').selectmenu('refresh');
                    var temp = '';
                    if (radioselected == '1') {
                        temp = 'true';
                    } else if (radioselected == '2') {
                        temp = 'false';
                    }
                    $.ajax({
                        type: "POST",
                        url: "Transaction.aspx/Process",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'flowtype': '" + temp + "' }",
                        success: function (data) {
                            $('#nextprocessselect').empty();
                            processselecttrans();
                            $('#nextprocessselect').selectmenu('refresh');
                            $('#nextproductionlineselect').empty();
                            productionselecttrans();
                            $('#nextproductionlineselect').selectmenu('refresh');
                            $('#nextprocessselect').append(data.d);
                            $('#nextprocessselect').selectmenu('refresh');
                            processselected = '2';
                        },
                        error: function (err) {
                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "011</td></tr>");
                            $('#messagetable').trigger('create');
                            alert(globeltranslate.errormessage + "011");
                        }
                    });
                });

                //部门下拉框改变选择
                $('#nextprocessselect').change(function () {
                    //如果下道部门选择SEW，则隐藏部件选择
                    if ($('#nextprocessselect').find("option:selected").attr("title") == 'Y') {
                        $('#parttr').hide();
                        $("#barcodetabletitle thead tr").empty();
                        $('#barcodetabletitle thead').append("<tr><th>" + globeltranslate.tabletranslation1 + "</th><th>" + globeltranslate.tabletranslation2 + "</th><th>" + globeltranslate.tabletranslation3 + "</th><th>" + globeltranslate.tabletranslation5 + "</th><th>" + globeltranslate.tabletranslation6 + "</th><th>" + globeltranslate.tabletranslation7 + "</th><th>" + globeltranslate.tabletranslation8 + "</th><th>" + globeltranslate.tabletranslation9 + "</th><th>" + globeltranslate.tabletranslation10 + "</th><th>" + globeltranslate.tabletranslation11 + "</th><th>" + globeltranslate.tabletranslation12 + "</th><th></th><th></th></tr>");
                        $('#barcodetabletitle').trigger('create');
                        $('#totalpcslabel').hide();
                        $('#totalpcsdiv').hide();
                        //$('#totalpcslabel').html(globeltranslate.totalpcslabel);
                    } else {
                        $('#parttr').show();
                        $("#barcodetabletitle thead tr").empty();
                        $('#barcodetabletitle thead').append("<tr><th>" + globeltranslate.tabletranslation1 + "</th><th>" + globeltranslate.tabletranslation2 + "</th><th>" + globeltranslate.tabletranslation3 + "</th><th>" + globeltranslate.tabletranslation4 + "</th><th>" + globeltranslate.tabletranslation5 + "</th><th>" + globeltranslate.tabletranslation6 + "</th><th>" + globeltranslate.tabletranslation7 + "</th><th>" + globeltranslate.tabletranslation8 + "</th><th>" + globeltranslate.tabletranslation9 + "</th><th>" + globeltranslate.tabletranslation10 + "</th><th>" + globeltranslate.tabletranslation11 + "</th><th>" + globeltranslate.tabletranslation12 + "</th><th>" + globeltranslate.tabletranslation13 + "</th><th></th><th></th></tr>");
                        $('#barcodetabletitle').trigger('create');
                        $('#totalpcslabel').show();
                        $('#totalpcsdiv').show();
                        //$('#totalpcslabel').html(globeltranslate.totalpcslabel1);
                    }
                    $('#nextproductionlineselect').empty();
                    productionselecttrans();
                    $('#nextproductionlineselect').selectmenu('refresh');
                    var factorycd = $('#nextfactoryselect option:selected').val();
                    var processcd = $('#nextprocessselect option:selected').val();
                    $.ajax({
                        type: "POST",
                        url: "Transaction.aspx/Production",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'nextfactory': '" + factorycd + "', 'nextprocess': '" + processcd + "' }",
                        success: function (data) {
                            var result = eval(data.d);
                            if (result[0].FLAG == 'True') {
                                processselected = '1';
                            } else {
                                processselected = '0';
                            }
                            $('#nextproductionlineselect').append(result[0].HTML);
                            $('#nextproductionlineselect').selectmenu('refresh');
                        },
                        error: function (err) {
                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "022</td></tr>");
                            $('#messagetable').trigger('create');
                            alert(globeltranslate.errormessage + "022");
                        }
                    });
                });

                //组别下拉框选择改变
                $('#nextproductionlineselect').change(function () {
                    if ($('#nextproductionlineselect option:selected').val() != sessionStorage.getItem("productionline")) {
                        productionlineselected = '1';
                    } else {
                        productionlineselected = '0';
                    }
                });

                //注销按钮：返回登录页面
                $('#cancelbutton').click(function () {
                    Loginout(fty, environment);
                });

                //清空按钮
                $('#emptylistbutton').click(function () {
                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                    setTimeout(function () {
                        $.ajax({
                            type: "POST",
                            url: 'Transaction.aspx/Emptylist',
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            async: false,
                            data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "' }",
                            success: function (data) {
                                $.mobile.loading("hide");
                                empty();
                                partselecttrans();
                                partselect();
                                factoryselecttrans();
                            },
                            error: function (err) {
                                $.mobile.loading("hide");
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "044</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.errormessage + "044");
                            }
                        });
                    }, 10);
                });

                //Save按钮监控
                $('#savebutton').click(function () {
                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                    setTimeout(function () {
                        $.ajax({
                            type: "POST",
                            url: "Package.aspx/Save",
                            contentType: "application/json;charset=utf-8",
                            async: false,
                            dataType: "json",
                            data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'module': 'Transaction' ,'carno':'"+$('#carno').val()+"'}",
                            success: function (data) {
                                $.mobile.loading('hide');
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.savemessage1 + data.d + "</td></tr>");
                                $('#messagetable').trigger('create');
                                query();
                                alert(globeltranslate.savemessage1 + data.d);
                            },
                            error: function (err) {
                                $.mobile.loading('hide');
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "044</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.errormessage + "044");
                            }
                        });
                    }, 10);
                });

                //querybutton change
                $('#querybutton').change(function () {
                    if (($('#nextfactoryselect').val() == factoryselect || $('#nextprocessselect').val() == processselect) && ($('#processtypeselect').val() == 'I' || $('#processtypeselect').val() == 'P')) {
                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage5 + "</td></tr>");
                        $('#messagetable').trigger('create');
                        alert(globeltranslate.bundlescanmessage5);
                        $("#querybutton option[value='selectdefault']").attr("selected", "selected");
                        $('#querybutton').selectmenu('refresh');
                        return;
                    }
                    var date = $(this).find('option:selected').text();
                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                    setTimeout(function () {
                        $.ajax({
                            type: "POST",
                            url: "Transaction.aspx/Querychange",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            async: false,
                            data: "{ 'docno': '" + docno + "', 'date': '" + date + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'functioncd': 'Transaction', 'dbtransdefect': '" + globeltranslate.tabletranslation13 + "', 'dbtransdelete': '" + globeltranslate.tabletranslation14 + "' }",
                            success: function (data) {
                                $.mobile.loading('hide');
                                if (data.d != "false1") {
                                    var result = $.parseJSON(data.d);
                                    if (result.bundles[0].ISFIRST == 'true') {//第一次扫描
                                        $('#fromfactoryselect').append("<option selected='selected' value='" + result.bundles[0].FACTORY + "'>" + result.bundles[0].FACTORY + "</option>");
                                        $('#fromprocessselect').append("<option selected='selected' value='" + result.bundles[0].PROCESS + "'>" + result.bundles[0].PROCESS + "</option>");
                                        $('#fromproductionselect').append("<option selected='selected' value='" + result.bundles[0].PRODUCTION + "'>" + result.bundles[0].PRODUCTION + "</option>");
                                        $('#fromfactoryselect').selectmenu('refresh');
                                        $('#fromprocessselect').selectmenu('refresh');
                                        $('#fromproductionselect').selectmenu('refresh');
                                    }
                                    if (result.bundles[0].ISPRODUCTION == 'false') {//裁片有不同组别
                                        bundlescanstatus = "0";
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage8 + $('#fromproductionselect').val() + globeltranslate.bundlescanmessage9 + result.bundles[0].WRONGPRODUCTION + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.bundlescanmessage8 + $('#fromproductionselect').val() + globeltranslate.bundlescanmessage9 + result.bundles[0].WRONGPRODUCTION);
                                        return;
                                    }
                                    var html = result.bundles[0].HTML;

                                    if ($('#totalbundlediv').html() == "0") {
                                        $('#barcodelisttable tbody').append(html);
                                    } else {
                                        $('#barcodelisttable tr:first').before(html);
                                    }
                                    $('#barcodelisttable1').trigger('create');

                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                        if (i <= 12)
                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                    });
                                    $('#totalbundlediv').html(result.bundlestatus[0].TOTALBUNDLES);
                                    $('#totalpcsdiv').html(result.bundlestatus[0].TOTALPCS);
                                    $('#totalgarmentpcs').html(result.bundlestatus[0].TOTALGARMENTPCS);
                                    $('#totalpartdiv').html(result.bundlestatus[0].TOTALPART);

                                    if (result.bundlestatus[0].ISPROCESS == "false")
                                        bundlescanstatus = "0";
                                    if (bundlescanstatus == '1' && result.bundlestatus[0].ISSUBMIT == "true")
                                        bundlescanstatus = "0";
                                    if (bundlescanstatus == '2' && result.bundlestatus[0].ISPROCESS == 'true' && result.bundlestatus[0].ISSUBMIT == 'false')
                                        bundlescanstatus = '1';
                                    if (result.bundlestatus[0].CANREPRINT == 'true')
                                        reprintstatus = '1';
                                    else
                                        reprintstatus = '0';
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.querymessage1 + date + "</td></tr>");
                                    $('#messagetable').trigger('create');
                                    selectcontrol('1', '1', '1');
                                } else {
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + date + globeltranslate.querymessage2 + "</td></tr>");
                                    $('#messagetable').trigger('create');
                                }
                                btnstatus();
                            },
                            error: function (err) {
                                $.mobile.loading('hide');
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "123</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.errormessage + "123");
                            }
                        });
                    }, 10);
                });

                
                //条码输入框监控
                //var barcodetype;  
                //var tosew = "false";
                $('#barcodetext').bind('keypress', function (event) {
                    if (event.keyCode == '13') {
                        var barcode = $(this).val();
                        barcodetype = GetBarcodeType(barcode);

                        //alert(barcodetype);
                        //alert(tosew);
                        //tangyh 2017.03.22
                        var get_othercarton;

                        if ($("input[id='bundlebarcodeother']").is(':checked')) {
                            get_othercarton = "T";
                        }
                        else {
                            get_othercarton = "F";
                        }
                        //调数扫描
                        if ($("input[name='radio']:checked").val() == 'adjustmentradio') {
                            if (barcodetype == 'B' && $('#partselect').val() == null && get_othercarton == "F") {
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage4 + "</td></tr>");
                                $('#messagetable').trigger('create');
                                $('#barcodetext').val("");
                                $('#barcodetext').focus();
                                alert(globeltranslate.bundlescanmessage4);
                                return;
                            }

                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: "Transaction.aspx/Adjustmentscan",
                                    contentType: "application/json;charset=utf-8",
                                    //async: false,
                                    dataType: "json",
                                    data: "{ 'lastfactory': '" + $('#nextfactoryselect').val() + "', 'lastprocess': '" + $('#nextprocessselect').val() + "', 'lastproduction': '" + $('#nextproductionlineselect').val() + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'nextprocess': '" + $('#nextprocessselect').val() + "', 'barcode': '" + barcode + "', 'part': '" + $('#partselect').val() + "', 'nextproduction': '" + $('#nextproductionlineselect').val() + "', 'productionselect': '" + productionselect + "', 'fromfactoryselected': '" + $('#fromfactoryselect').val() + "', 'fromprocessselected': '" + $('#fromprocessselect').val() + "', 'fromproductionselected': '" + $('#fromproductionselect').val() + "', 'fromfactory': '" + fromfactoryselected + "', 'dbtransdefect': '" + globeltranslate.tabletranslation13 + "', 'dbtransdelete': '" + globeltranslate.tabletranslation14 + "', 'get_othercarton': '" + get_othercarton + "' }",
                                    success: function (data) {
                                        $.mobile.loading('hide');
                                        $('#barcodetext').val("");
                                        $('#barcodetext').focus();
                                        if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                            var result = eval(data.d);
                                            if (result[0].STATUS == 'N') {
                                                if (result[0].ISSUBMIT == 'false') {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.statusmessage1 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.statusmessage1);
                                                }
                                                if (sessionStorage.getItem("process") == 'SEW') {
                                                    if (result[0].ISFULLBUNDLE == 'false') {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.statusmessage3 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.statusmessage3);
                                                    }
                                                }
                                                if (result[0].ISPROCESS == 'false') {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.statusmessage2 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.statusmessage2);
                                                }
                                                if (result[0].ISONERECEIVE == 'false') {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.statusmessage4 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.statusmessage4);
                                                }
                                                if (result[0].ISONESEND == 'false') {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.statusmessage5 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.statusmessage5);
                                                }
                                                if (result[0].ISONECUTGARMENTTYPE == 'false') {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.statusmessage7 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.statusmessage7);
                                                }
                                                if (result[0].ISONESEWGARMENTTYPE == 'false') {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.statusmessage8 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.statusmessage8);
                                                }
                                            }
                                            else {
                                                //激活确认或者提交的按钮标志
                                                bundlescanstatus = '1';

                                                //获取bundle信息
                                                var html = result[0].HTML;
                                                if (html == '') {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage15 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    return;
                                                }

                                                //更新界面的接收部门和发送部门以及发送类型以及table的title
                                                $('#fromfactoryselect').append("<option selected='selected' value='" + result[0].SENDFACTORY + "'>" + result[0].SENDFACTORY + "</option>");
                                                $('#fromprocessselect').append("<option selected='selected' value='" + result[0].SENDPROCESS + "'>" + result[0].SENDPROCESS + "</option>");
                                                $('#fromproductionselect').append("<option selected='selected' value='" + result[0].SENDPRODUCTION + "'>" + result[0].SENDPRODUCTION + "</option>");
                                                $('#nextfactoryselect').append("<option selected='selected' value='" + result[0].RECEIVEFACTORY + "'>" + result[0].RECEIVEFACTORY + "</option>");
                                                $('#nextprocessselect').append("<option selected='selected' value='" + result[0].RECEIVEPROCESS + "'>" + result[0].RECEIVEPROCESS + "</option>");
                                                $('#nextproductionlineselect').append("<option selected='selected' value='" + result[0].RECEIVEPRODUCTION + "'>" + result[0].RECEIVEPRODUCTION + "</option>");
                                                $('#fromfactoryselect').selectmenu('refresh');
                                                $('#fromprocessselect').selectmenu('refresh');
                                                $('#fromproductionselect').selectmenu('refresh');
                                                $('#nextfactoryselect').selectmenu('refresh');
                                                $('#nextprocessselect').selectmenu('refresh');
                                                $('#nextproductionlineselect').selectmenu('refresh');
                                                $('#carno').val( result[0].CARNO);
                                                if (result[0].RECEIVEFACTORY == result[0].SENDFACTORY)
                                                    $('#processtypeselect').append("<option selected='selected' value='I'>I</option>");
                                                else
                                                    $('#processtypeselect').append("<option selected='selected' value='P'>P</option>");
                                                $('#processtypeselect').selectmenu('refresh');
                                                if ($('#fromprocessselect').val() == 'SEW') {
                                                    $("#barcodetabletitle thead tr").empty();
                                                    $('#barcodetabletitle thead').append("<tr><th>" + globeltranslate.tabletranslation1 + "</th><th>" + globeltranslate.tabletranslation2 + "</th><th>" + globeltranslate.tabletranslation3 + "</th><th>" + globeltranslate.tabletranslation5 + "</th><th>" + globeltranslate.tabletranslation6 + "</th><th>" + globeltranslate.tabletranslation7 + "</th><th>" + globeltranslate.tabletranslation8 + "</th><th>" + globeltranslate.tabletranslation9 + "</th><th>" + globeltranslate.tabletranslation10 + "</th><th>" + globeltranslate.tabletranslation11 + "</th><th>" + globeltranslate.tabletranslation12 + "</th><th></th><th></th></tr>");
                                                    $('#barcodetabletitle').trigger('create');
                                                }
                                                else {
                                                    $("#barcodetabletitle thead tr").empty();
                                                    $('#barcodetabletitle thead').append("<tr><th>" + globeltranslate.tabletranslation1 + "</th><th>" + globeltranslate.tabletranslation2 + "</th><th>" + globeltranslate.tabletranslation3 + "</th><th>" + globeltranslate.tabletranslation4 + "</th><th>" + globeltranslate.tabletranslation5 + "</th><th>" + globeltranslate.tabletranslation6 + "</th><th>" + globeltranslate.tabletranslation7 + "</th><th>" + globeltranslate.tabletranslation8 + "</th><th>" + globeltranslate.tabletranslation9 + "</th><th>" + globeltranslate.tabletranslation10 + "</th><th>" + globeltranslate.tabletranslation11 + "</th><th>" + globeltranslate.tabletranslation12 + "</th><th>" + globeltranslate.tabletranslation13 + "</th><th></th><th></th></tr>");
                                                    $('#barcodetabletitle').trigger('create');
                                                }

                                                //判断接收部门是否需要提交
                                                $.ajax({
                                                    type: "POST",
                                                    url: "Transaction.aspx/Checkifneedtoreceive",
                                                    contentType: "application/json;charset=utf-8",
                                                    async: false,
                                                    dataType: "json",
                                                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + $('#fromprocessselect').val() + "', 'nextprocess': '" + $('#nextprocessselect').val() + "' }",
                                                    success: function (data) {
                                                        if (data.d == 'false')
                                                            processselected = 0;
                                                        else
                                                            processselected = 1;
                                                        btnstatus();
                                                    },
                                                    error: function (err) {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "039</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.errormessage + "039");
                                                    }
                                                });

                                                //刷新table的title
                                                if (sessionStorage.getItem("process") == 'SEW') {
                                                    //把bundle信息添加到table里
                                                    arr = html.split('@');
                                                    pagenum = Math.ceil(arr.length / 10);
                                                    if (currentpagenum > pagenum) {
                                                        currentpagenum = pagenum;
                                                    }
                                                    html = "";
                                                    $('#barcodelisttable tr').empty();
                                                    html += tablelogic1(arr, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'true');
                                                    $('#barcodelisttable tbody').append(html);
                                                    $('#barcodelisttable').trigger('create');

                                                    $('#selector').pagination({
                                                        items: arr.length,
                                                        itemsOnPage: 10,
                                                        cssStyle: 'light-theme',
                                                        currentPage: currentpagenum,
                                                        prevText: globeltranslate.prevbutton,
                                                        nextText: globeltranslate.nextbutton,
                                                        onPageClick: function (pagenumber, event) {
                                                            currentpagenum = pagenumber;
                                                            $('#barcodelisttable tbody').empty();
                                                            html = "";
                                                            html += tablelogic(arr, currentpagenum, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'true');
                                                            $('#barcodelisttable tbody').append(html);
                                                            $('#barcodelisttable').trigger('create');
                                                        }
                                                    });
                                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                                        if (i <= 10)
                                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    });
                                                }
                                                else {
                                                    arr = html.split('@');
                                                    pagenum = Math.ceil(arr.length / 10);
                                                    if (currentpagenum > pagenum) {
                                                        currentpagenum = pagenum;
                                                    }
                                                    html = "";
                                                    $('#barcodelisttable tr').empty();
                                                    html += tablelogic1(arr, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'false');
                                                    $('#barcodelisttable tbody').append(html);
                                                    $('#barcodelisttable').trigger('create');

                                                    $('#selector').pagination({
                                                        items: arr.length,
                                                        itemsOnPage: 10,
                                                        cssStyle: 'light-theme',
                                                        currentPage: currentpagenum,
                                                        prevText: globeltranslate.prevbutton,
                                                        nextText: globeltranslate.nextbutton,
                                                        onPageClick: function (pagenumber, event) {
                                                            currentpagenum = pagenumber;
                                                            $('#barcodelisttable tbody').empty();
                                                            html = "";
                                                            html += tablelogic(arr, currentpagenum, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'false');
                                                            $('#barcodelisttable tbody').append(html);
                                                            $('#barcodelisttable').trigger('create');
                                                        }
                                                    });
                                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                                        if (i <= 12)
                                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    });
                                                }

                                                //更新界面的扎数等数量信息
                                                $('#totalbundlediv').html(result[0].TOTALBUNDLES);
                                                $('#totalpcsdiv').html(result[0].TOTALPCS);
                                                $('#totalgarmentpcs').html(result[0].TOTALGARMENTPCS);
                                                $('#totalpartdiv').html(result[0].TOTALPART);

                                                //激活重新打印按钮
                                                if (result[0].CANREPRINT == 'true')
                                                    reprintstatus = '1';
                                                else
                                                    reprintstatus = '0';

                                                //扫描成功信息提示
                                                if (barcode.length == 14) {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage1 + barcode + globeltranslate.s + result[0].PART + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                }
                                                else if (barcode.length == 13 || barcode.length == 15) {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage10 + barcode + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                }
                                                else if (barcode.length == 16) {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage14 + barcode + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                }
                                            }
                                        }
                                        else if (data.d.substr(0, 5) == "false") {
                                            if (data.d == 'false1') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage6 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.bundlescanmessage6);
                                            }
                                            else if (data.d == "false2") {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + barcode + globeltranslate.s + $('#partselect').val() + globeltranslate.bundlescanmessage2 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                            }
                                            else if (data.d == "false3") {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage7 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.bundlescanmessage7);
                                            }
                                        }
                                        else if (data.d.substr(0, 5) == "error") {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.statusmessage6 + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.statusmessage6);
                                        }
                                        $('#barcodetext').val("");
                                        btnstatus();
                                        selectcontrol('1', '1', '1');
                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "125</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage + "125");
                                    }
                                });
                            }, 10);
                        }
                        else {
                            //外发扫描
                            if ($('#processtypeselect').val() == 'O' || $('#processtypeselect').val() == 'E') {
                                if ($('#nextprocessselect').val() == processselect) {
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.processmessage + "</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.processmessage);
                                    return;
                                }
                                if (barcode.length == '14') {
                                    if ($('#partselect').val() == null && (get_othercarton == "F")) {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.partselectmessage1 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.partselectmessage1);
                                        return;
                                    }
                                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                    setTimeout(function () {
                                        $.ajax({
                                            type: "POST",
                                            url: "Transaction.aspx/OASBundlescan",
                                            contentType: "application/json;charset=utf-8",
                                            //async: false,
                                            dataType: "json",
                                            ata: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'nextprocess': '" + $('#nextprocessselect').val() + "', 'barcode': '" + barcode + "', 'part': '" + $('#partselect').val() + "', 'contract': '" + $('#contractnoselect').val() + "', 'contractnoselect': '" + contractnoselect + "', 'fromfactoryselected': '" + $('#fromfactoryselect').val() + "', 'fromprocessselected': '" + $('#fromprocessselect').val() + "', 'fromproductionselected': '" + $('#fromproductionselect').val() + "', 'fromfactory': '" + fromfactoryselected + "' }",
                                            success: function (data) {
                                                $.mobile.loading('hide');
                                                $('#barcodetext').val("");
                                                $('#barcodetext').focus();
                                                if (data.d == "false11") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage11 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.bundlescanmessage11);
                                                } else if (data.d == "false1") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + barcode + "'s " + $('#partselect').val() + bundlescanmessage2() + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                } else if (data.d == "false2") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.oasbundlemessage1 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.oasbundlemessage1);
                                                } else if (data.d == "false3") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.oasbundlemessage2 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.oasbundlemessage2);
                                                } else if (data.d == "false4") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.oasbundlemessage3 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.oasbundlemessage3);
                                                } else {
                                                    var result = $.parseJSON(data.d);
                                                    var html = result.bundles[0].HTML;
                                                    if (result.bundles[0].ISFIRST == 'true') {//第一次扫描
                                                        $('#fromfactoryselect').append("<option selected='selected' value='" + result.bundles[0].FACTORY + "'>" + result.bundles[0].FACTORY + "</option>");
                                                        $('#fromprocessselect').append("<option selected='selected' value='" + result.bundles[0].PROCESS + "'>" + result.bundles[0].PROCESS + "</option>");
                                                        $('#fromproductionselect').append("<option selected='selected' value='" + result.bundles[0].PRODUCTION + "'>" + result.bundles[0].PRODUCTION + "</option>");
                                                        $('#fromfactoryselect').selectmenu('refresh');
                                                        $('#fromprocessselect').selectmenu('refresh');
                                                        $('#fromproductionselect').selectmenu('refresh');
                                                    }
                                                    if (result.bundles[0].ISPRODUCTION == 'false') {//裁片有不同组别
                                                        bundlescanstatus = "0";
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage8 + $('#fromproductionselect').val() + globeltranslate.bundlescanmessage9 + result.bundles[0].WRONGPRODUCTION + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.bundlescanmessage8 + $('#fromproductionselect').val() + globeltranslate.bundlescanmessage9 + result.bundles[0].WRONGPRODUCTION);
                                                        return;
                                                    }
                                                    if (result.bundlestatus[0].CONTRACTNO != "") {
                                                        $('#contractnoselect').empty();
                                                        contractselecttrans();
                                                        $('#contractnoselect').append(result.bundlestatus[0].CONTRACTNO);
                                                        $('#contractnoselect').selectmenu('refresh');
                                                    }
                                                    $('#barcodelisttable tbody').after(html);
                                                    $('#barcodelisttable').trigger('create');
                                                    $('#totalbundlediv').html(result.bundlestatus[0].TOTALBUNDLES);
                                                    $('#totalpcsdiv').html(result.bundlestatus[0].TOTALPCS);
                                                    $('#totalgarmentpcs').html(result.bundlestatus[0].TOTALGARMENTPCS);
                                                    $('#totalpartdiv').html(result.bundlestatus[0].TOTALPART);
                                                    if (result.bundlestatus[0].ISPROCESS == "true") {
                                                        bundlescanstatus = "1"; //都在本部门，满足流转条件
                                                    } else {
                                                        bundlescanstatus = "0";
                                                    }
                                                    if (result.bundlestatus[0].CANREPRINT == 'true')
                                                        reprintstatus = '1';
                                                    else
                                                        reprintstatus = '0';
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + bundlescanmessage1() + barcode + globeltranslate.s + result.bundle[0].PART + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                }
                                                btnstatus();
                                                $('#barcodetext').val("");
                                            },
                                            error: function (err) {
                                                $.mobile.loading('hide');
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "142</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage + "142");
                                            }
                                        });
                                    }, 10);
                                } else if (barcode.length == 13 || barcode.length == 15) {
                                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                    setTimeout(function () {
                                        $.ajax({
                                            type: "POST",
                                            url: "Transaction.aspx/OASCartonscan",
                                            contentType: "application/json;charset=utf-8",
                                            //async: false,
                                            dataType: "json",
                                            data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'nextprocess': '" + process + "', 'barcode': '" + barcode + "', 'contract': '" + $('#contractnoselect').val() + "', 'contractnoselect': '" + contractnoselect + "', 'get_othercarton': '" + get_othercarton + "' }",
                                            success: function (data) {
                                                $.mobile.loading('hide');
                                                $('#barcodetext').val("");
                                                $('#barcodetext').focus();
                                                if (data.d == "false1") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage1 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.cartonscanmessage1);
                                                } else if (data.d == "false2") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage6 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.cartonscanmessage6);
                                                } else if (data.d == "false3") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.oasbundlemessage1 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.oasbundlemessage1);
                                                } else if (data.d == "false4") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage7 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.cartonscanmessage7);
                                                } else if (data.d == "false5") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage3 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.cartonscanmessage3);
                                                } else {
                                                    var result = $.parseJSON(data.d);
                                                    var html = result.bundles[0].HTML;
                                                    if (result.bundlestatus[0].CONTRACTNO != "") {
                                                        $('#contractnoselect').empty();
                                                        contractselecttrans();
                                                        $('#contractnoselect').append(result.bundlestatus[0].CONTRACTNO);
                                                        $('#contractnoselect').selectmenu('refresh');
                                                    }
                                                    $('#barcodelisttable tbody').after(html);
                                                    $('#barcodelisttable').trigger('create');
                                                    $('#totalbundlediv').html(result.bundlestatus[0].TOTALBUNDLES);
                                                    $('#totalpcsdiv').html(result.bundlestatus[0].TOTALPCS);
                                                    $('#totalgarmentpcs').html(result.bundlestatus[0].TOTALGARMENTPCS);
                                                    $('#totalpartdiv').html(result.bundlestatus[0].TOTALPART);
                                                    if (result.bundlestatus[0].ISPROCESS == "true") {
                                                        bundlescanstatus = "1"; //都在本部门
                                                    } else {
                                                        bundlescanstatus = "0";
                                                    }
                                                    if (result.bundlestatus[0].CANREPRINT == 'true')
                                                        reprintstatus = '1';
                                                    else
                                                        reprintstatus = '0';
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage10 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                }
                                                $('#barcodetext').val("");
                                                btnstatus();
                                            },
                                            error: function (err) {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "121</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage + "121");
                                            }
                                        });
                                    }, 10);
                                } else {
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage3 + "</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.bundlescanmessage3);
                                    return;
                                }
                            }
                            //本厂或者Peer厂扫描
                            else {
                                //var tosew = "false";
                                tosew = "false";
                                if ($('#nextprocessselect').val().substring(0, 3) == 'SEW') {
                                    tosew = "true";
                                    if ($('#nextproductionlineselect').val() == globeltranslate.nextproductionlineselect) {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage5 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.bundlescanmessage5);
                                        return;
                                    }
                                }
                                //如果下道部门是sew，不需要选择part。系统把配套好的bundle全部查询出来。如果还没有配套好，则做出错误提示

                                //裁片条码扫描
                                if (barcodetype == 'B' && tosew == 'false') {
                                    if (($('#nextfactoryselect').val() == factoryselect || $('#nextprocessselect').val() == processselect) && ($('#processtypeselect').val() == 'I' || $('#processtypeselect').val() == 'P')) {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage5 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.bundlescanmessage5);
                                        return;
                                    }
                                    if ($('#partselect').val() == null && get_othercarton == "F") {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage4 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.bundlescanmessage4);
                                        return;
                                    }
                                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                    //setTimeout(function () {
                                        $.ajax({
                                            type: "POST",
                                            url: "Transaction.aspx/Bundlescan",
                                            contentType: "application/json;charset=utf-8",
                                            //async: false,
                                            dataType: "json",
                                            data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'nextprocess': '" + $('#nextprocessselect').val() + "', 'barcode': '" + barcode + "', 'part': '" + $('#partselect').val() + "', 'nextproduction': '" + $('#nextproductionlineselect').val() + "', 'productionselect': '" + productionselect + "', 'fromfactoryselected': '" + $('#fromfactoryselect').val() + "', 'fromprocessselected': '" + $('#fromprocessselect').val() + "', 'fromproductionselected': '" + $('#fromproductionselect').val() + "', 'fromfactory': '" + fromfactoryselected + "', 'dbtransdefect': '" + globeltranslate.tabletranslation13 + "', 'dbtransdelete': '" + globeltranslate.tabletranslation14 + "', 'get_othercarton': '" + get_othercarton + "' }",
                                            success: function (data) {
                                                $.mobile.loading('hide');
                                                $('#barcodetext').val("");
                                                $('#barcodetext').focus();
                                                if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                                    var result = $.parseJSON(data.d);
                                                    if (result.bundles[0].ISFIRST == 'true') {//第一次扫描
                                                        $('#fromfactoryselect').append("<option selected='selected' value='" + result.bundles[0].FACTORY + "'>" + result.bundles[0].FACTORY + "</option>");
                                                        $('#fromprocessselect').append("<option selected='selected' value='" + result.bundles[0].PROCESS + "'>" + result.bundles[0].PROCESS + "</option>");
                                                        $('#fromproductionselect').append("<option selected='selected' value='" + result.bundles[0].PRODUCTION + "'>" + result.bundles[0].PRODUCTION + "</option>");
                                                        $('#fromfactoryselect').selectmenu('refresh');
                                                        $('#fromprocessselect').selectmenu('refresh');
                                                        $('#fromproductionselect').selectmenu('refresh');
                                                    }
                                                    if (result.bundles[0].ISPRODUCTION == 'false') {//裁片有不同组别
                                                        bundlescanstatus = "0";
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage8 + $('#fromproductionselect').val() + globeltranslate.bundlescanmessage9 + result.bundles[0].WRONGPRODUCTION + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.bundlescanmessage8 + $('#fromproductionselect').val() + globeltranslate.bundlescanmessage9 + result.bundles[0].WRONGPRODUCTION);
                                                        return;
                                                    }

                                                    var html = result.bundles[0].HTML;
                                                    if (html == '') {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage15 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        return;
                                                    }
                                                    arr = html.split('@');
                                                    pagenum = Math.ceil(arr.length / 10);
                                                    if (currentpagenum > pagenum) {
                                                        currentpagenum = pagenum;
                                                    }
                                                    html = "";
                                                    $('#barcodelisttable tr').empty();
                                                    html += tablelogic1(arr, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'false');
                                                    $('#barcodelisttable tbody').append(html);
                                                    $('#barcodelisttable').trigger('create');

                                                    $('#selector').pagination({
                                                        items: arr.length,
                                                        itemsOnPage: 10,
                                                        cssStyle: 'light-theme',
                                                        currentPage: currentpagenum,
                                                        prevText: globeltranslate.prevbutton,
                                                        nextText: globeltranslate.nextbutton,
                                                        onPageClick: function (pagenumber, event) {
                                                            currentpagenum = pagenumber;
                                                            $('#barcodelisttable tbody').empty();
                                                            html = "";
                                                            html += tablelogic(arr, currentpagenum, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'false');
                                                            $('#barcodelisttable tbody').append(html);
                                                            $('#barcodelisttable').trigger('create');
                                                        }
                                                    });

                                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                                        if (i <= 12)
                                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    });

                                                    $('#totalbundlediv').html(result.bundlestatus[0].TOTALBUNDLES);
                                                    $('#totalpcsdiv').html(result.bundlestatus[0].TOTALPCS);
                                                    $('#totalgarmentpcs').html(result.bundlestatus[0].TOTALGARMENTPCS);
                                                    $('#totalpartdiv').html(result.bundlestatus[0].TOTALPART);
                                                    if (result.bundlestatus[0].ISPROCESS == "false")
                                                        bundlescanstatus = "0";
                                                    if (bundlescanstatus == '1' && result.bundlestatus[0].ISSUBMIT == "true")
                                                        bundlescanstatus = "0";
                                                    if (bundlescanstatus == '2' && result.bundlestatus[0].ISPROCESS == 'true' && result.bundlestatus[0].ISSUBMIT == 'false')
                                                        bundlescanstatus = '1';
                                                    if (result.bundlestatus[0].CANREPRINT == 'true')
                                                        reprintstatus = '1';
                                                    else
                                                        reprintstatus = '0';
                                                    if (result.bundlestatus[0].ISSUBMIT == "true")
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage8 + "</td></tr>");
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage1 + barcode + globeltranslate.s + result.bundles[0].PART + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                } else if (data.d == "false1") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage6 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.bundlescanmessage6);
                                                } else if (data.d == "false2") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + barcode + globeltranslate.s + $('#partselect').val() + globeltranslate.bundlescanmessage2 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                } else if (data.d == "false3") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage7 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.bundlescanmessage7);
                                                }
                                                $('#barcodetext').val("");
                                                btnstatus();
                                                selectcontrol('1', '1', '1');
                                            },
                                            error: function (err) {
                                                $.mobile.loading('hide');
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "143</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage + "143");
                                            }
                                        });
                                    //}, 10);
                                }
                                else if (barcodetype == 'B' && tosew == 'true') {
                                    if (($('#nextfactoryselect').val() == factoryselect || $('#nextprocessselect').val() == processselect) && ($('#processtypeselect').val() == 'I' || $('#processtypeselect').val() == 'P')) {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage5 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.bundlescanmessage5);
                                        return;
                                    }
                                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                    //setTimeout(function () {
                                        $.ajax({
                                            type: "POST",
                                            url: "Transaction.aspx/DCtoSEWscan",
                                            contentType: "application/json;charset=utf-8",
                                            //async: false,
                                            dataType: "json",
                                            data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'nextprocess': '" + $('#nextprocessselect').val() + "', 'barcode': '" + barcode + "', 'part': '" + $('#partselect').val() + "', 'nextproduction': '" + $('#nextproductionlineselect').val() + "', 'productionselect': '" + productionselect + "', 'fromfactoryselected': '" + $('#fromfactoryselect').val() + "', 'fromprocessselected': '" + $('#fromprocessselect').val() + "', 'fromproductionselected': '" + $('#fromproductionselect').val() + "', 'fromfactory': '" + fromfactoryselected + "', 'dbtransdelete': '" + globeltranslate.tabletranslation14 + "' }",
                                            success: function (data) {
                                                $.mobile.loading('hide');
                                                $('#barcodetext').val("");
                                                $('#barcodetext').focus();
                                                if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                                    var result = $.parseJSON(data.d);
                                                    if (result.bundles[0].ISFIRST == 'true') {//第一次扫描
                                                        $('#fromfactoryselect').append("<option selected='selected' value='" + result.bundles[0].FACTORY + "'>" + result.bundles[0].FACTORY + "</option>");
                                                        $('#fromprocessselect').append("<option selected='selected' value='" + result.bundles[0].PROCESS + "'>" + result.bundles[0].PROCESS + "</option>");
                                                        $('#fromproductionselect').append("<option selected='selected' value='" + result.bundles[0].PRODUCTION + "'>" + result.bundles[0].PRODUCTION + "</option>");
                                                        $('#fromfactoryselect').selectmenu('refresh');
                                                        $('#fromprocessselect').selectmenu('refresh');
                                                        $('#fromproductionselect').selectmenu('refresh');
                                                    }
                                                    if (result.bundles[0].ISPRODUCTION == 'false') {//裁片有不同组别
                                                        bundlescanstatus = "0";
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage8 + $('#fromproductionselect').val() + globeltranslate.bundlescanmessage9 + result.bundles[0].WRONGPRODUCTION + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.bundlescanmessage8 + $('#fromproductionselect').val() + globeltranslate.bundlescanmessage9 + result.bundles[0].WRONGPRODUCTION);
                                                        return;
                                                    }

                                                    var html = result.bundles[0].HTML;
                                                    if (html == '') {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage15 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        return;
                                                    }
                                                    arr = html.split('@');
                                                    pagenum = Math.ceil(arr.length / 10);
                                                    if (currentpagenum > pagenum) {
                                                        currentpagenum = pagenum;
                                                    }
                                                    html = "";
                                                    $('#barcodelisttable tr').empty();
                                                    html += tablelogic1(arr, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'true');
                                                    $('#barcodelisttable tbody').append(html);
                                                    $('#barcodelisttable').trigger('create');

                                                    $('#selector').pagination({
                                                        items: arr.length,
                                                        itemsOnPage: 10,
                                                        cssStyle: 'light-theme',
                                                        currentPage: currentpagenum,
                                                        prevText: globeltranslate.prevbutton,
                                                        nextText: globeltranslate.nextbutton,
                                                        onPageClick: function (pagenumber, event) {
                                                            currentpagenum = pagenumber;
                                                            $('#barcodelisttable tbody').empty();
                                                            html = "";
                                                            html += tablelogic(arr, currentpagenum, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'true');
                                                            $('#barcodelisttable tbody').append(html);
                                                            $('#barcodelisttable').trigger('create');
                                                        }
                                                    });

                                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                                        if (i <= 10)
                                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    });

                                                    $('#totalbundlediv').html(result.bundlestatus[0].TOTALBUNDLES);
                                                    $('#totalpcsdiv').html(result.bundlestatus[0].TOTALPCS);
                                                    $('#totalgarmentpcs').html(result.bundlestatus[0].TOTALGARMENTPCS);
                                                    $('#totalpartdiv').html(result.bundlestatus[0].TOTALPART);

                                                    if (result.bundlestatus[0].ISPROCESS == "false")
                                                        bundlescanstatus = "0";
                                                    if (bundlescanstatus == '1' && result.bundlestatus[0].ISSUBMIT == "true")
                                                        bundlescanstatus = "0";
                                                    if (bundlescanstatus == '2' && result.bundlestatus[0].ISPROCESS == 'true' && result.bundlestatus[0].ISSUBMIT == 'false')
                                                        bundlescanstatus = '1';
                                                    if (result.bundlestatus[0].CANREPRINT == 'true')
                                                        reprintstatus = '1';
                                                    else
                                                        reprintstatus = '0';
                                                    if (result.bundlestatus[0].ISSUBMIT == "true")
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage8 + "</td></tr>");
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage1 + barcode + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                } else if (data.d == 'false9') {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage12 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.bundlescanmessage12);
                                                } else if (data.d == "false1") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage6 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.bundlescanmessage6);
                                                } else if (data.d == "false2") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + barcode + globeltranslate.s + $('#partselect').val() + globeltranslate.bundlescanmessage2 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                } else if (data.d == "false3") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage7 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.bundlescanmessage7);
                                                }
                                                $('#barcodetext').val("");
                                                btnstatus();
                                                selectcontrol('1', '1', '1');
                                            },
                                            error: function (err) {
                                                $.mobile.loading('hide');
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "143</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage + "143");
                                            }
                                        });
                                    //}, 10);
                                }
                                else if ((barcodetype == 'C') && tosew == 'false') {
                                    //箱码扫描
                                    if (($('#nextfactoryselect').val() == factoryselect || $('#nextprocessselect').val() == processselect) && ($('#processtypeselect').val() == 'I' || $('#processtypeselect').val() == 'P')) {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage5 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.bundlescanmessage5);
                                        return;
                                    }
                                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                    //setTimeout(function () {
                                        $.ajax({
                                            type: "POST",
                                            url: "Transaction.aspx/Cartonscan",
                                            contentType: "application/json;charset=utf-8",
                                            //async: false,
                                            dataType: "json",
                                            data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'nextprocess': '" + $('#nextprocessselect').val() + "', 'barcode': '" + barcode + "', 'nextproduction': '" + $('#nextproductionlineselect').val() + "', 'productionselect': '" + productionselect + "', 'fromfactoryselected': '" + $('#fromfactoryselect').val() + "', 'fromprocessselected': '" + $('#fromprocessselect').val() + "', 'fromproductionselected': '" + $('#fromproductionselect').val() + "', 'fromfactory': '" + fromfactoryselected + "', 'dbtransdefect': '" + globeltranslate.tabletranslation13 + "', 'dbtransdelete': '" + globeltranslate.tabletranslation14 + "', 'get_othercarton': '" + get_othercarton + "', 'part': '" + $('#partselect').val() + "' }",
                                            success: function (data) {
                                                $.mobile.loading('hide');
                                                $('#barcodetext').val("");
                                                $('#barcodetext').focus();
                                                if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                                    var result = $.parseJSON(data.d);
                                                    if (result.bundles[0].ISFIRST == 'true') {//第一次扫描
                                                        $('#fromfactoryselect').append("<option selected='selected' value='" + result.bundles[0].FACTORY + "'>" + result.bundles[0].FACTORY + "</option>");
                                                        $('#fromprocessselect').append("<option selected='selected' value='" + result.bundles[0].PROCESS + "'>" + result.bundles[0].PROCESS + "</option>");
                                                        $('#fromproductionselect').append("<option selected='selected' value='" + result.bundles[0].PRODUCTION + "'>" + result.bundles[0].PRODUCTION + "</option>");
                                                        $('#fromfactoryselect').selectmenu('refresh');
                                                        $('#fromprocessselect').selectmenu('refresh');
                                                        $('#fromproductionselect').selectmenu('refresh');
                                                    }

                                                    var html = result.bundles[0].HTML;
                                                    if (html == '') {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage15 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        return;
                                                    }
                                                    arr = html.split('@');
                                                    pagenum = Math.ceil(arr.length / 10);
                                                    if (currentpagenum > pagenum) {
                                                        currentpagenum = pagenum;
                                                    }
                                                    html = "";
                                                    $('#barcodelisttable tr').empty();
                                                    html += tablelogic1(arr, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'false');
                                                    $('#barcodelisttable tbody').append(html);
                                                    $('#barcodelisttable').trigger('create');

                                                    $('#selector').pagination({
                                                        items: arr.length,
                                                        itemsOnPage: 10,
                                                        cssStyle: 'light-theme',
                                                        currentPage: currentpagenum,
                                                        prevText: globeltranslate.prevbutton,
                                                        nextText: globeltranslate.nextbutton,
                                                        onPageClick: function (pagenumber, event) {
                                                            currentpagenum = pagenumber;
                                                            $('#barcodelisttable tbody').empty();
                                                            html = "";
                                                            html += tablelogic(arr, currentpagenum, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'false');
                                                            $('#barcodelisttable tbody').append(html);
                                                            $('#barcodelisttable').trigger('create');
                                                        }
                                                    });

                                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                                        if (i <= 12) {
                                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                        }
                                                    });

                                                    $('#totalbundlediv').html(result.cartonstatus[0].TOTALBUNDLES);
                                                    $('#totalpcsdiv').html(result.cartonstatus[0].TOTALPCS);
                                                    $('#totalgarmentpcs').html(result.cartonstatus[0].TOTALGARMENTPCS);
                                                    $('#totalpartdiv').html(result.cartonstatus[0].TOTALPART);

                                                    if (result.cartonstatus[0].ISPROCESS == "false")
                                                        bundlescanstatus = "0";
                                                    if (bundlescanstatus == '1' && result.cartonstatus[0].ISSUBMIT == "true")
                                                        bundlescanstatus = "0";
                                                    if (get_othercarton == "F") {
                                                        if (bundlescanstatus == '2' && result.cartonstatus[0].ISPROCESS == 'true' && result.cartonstatus[0].ISSUBMIT == 'false')
                                                            bundlescanstatus = '1';
                                                    }
                                                    else
                                                        bundlescanstatus = 1
                                                    if (result.cartonstatus[0].CANREPRINT == 'true')
                                                        reprintstatus = '1';
                                                    else
                                                        reprintstatus = '0';
                                                    if (result.cartonstatus[0].ISSUBMIT == "true")
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage8 + "</td></tr>");
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage5 + barcode + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                } else if (data.d == "false1") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage1 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.cartonscanmessage1);
                                                } else if (data.d == "false2") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage2 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.cartonscanmessage2);
                                                } else if (data.d == "false3") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage3 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                } else if (data.d == "false4") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage4 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.cartonscanmessage4);
                                                }
                                                btnstatus();
                                                selectcontrol('1', '1', '1');
                                            },
                                            error: function (err) {
                                                $.mobile.loading('hide');
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "013</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage + "013");
                                            }
                                        });
                                    //}, 10);
                                }
                                else if ((barcodetype == 'C') && tosew == 'true') {
                                    //箱码扫描
                                    if (($('#nextfactoryselect').val() == factoryselect || $('#nextprocessselect').val() == processselect) && ($('#processtypeselect').val() == 'I' || $('#processtypeselect').val() == 'P')) {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage5 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.bundlescanmessage5);
                                        return;
                                    }
                                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                    //setTimeout(function () {
                                        $.ajax({
                                            type: "POST",
                                            url: "Transaction.aspx/DCtoSEWcartonscan",
                                            contentType: "application/json;charset=utf-8",
                                            //async: false,
                                            dataType: "json",
                                            data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'nextprocess': '" + $('#nextprocessselect').val() + "', 'barcode': '" + barcode + "', 'nextproduction': '" + $('#nextproductionlineselect').val() + "', 'productionselect': '" + productionselect + "', 'fromfactoryselected': '" + $('#fromfactoryselect').val() + "', 'fromprocessselected': '" + $('#fromprocessselect').val() + "', 'fromproductionselected': '" + $('#fromproductionselect').val() + "', 'fromfactory': '" + fromfactoryselected + "', 'dbtransdefect': '" + globeltranslate.tabletranslation13 + "', 'dbtransdelete': '" + globeltranslate.tabletranslation14 + "', 'get_othercarton': '" + get_othercarton + "' }",
                                            success: function (data) {
                                                $.mobile.loading('hide');
                                                $('#barcodetext').val("");
                                                $('#barcodetext').focus();
                                                if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                                    var result = $.parseJSON(data.d);
                                                    if (result.bundles[0].ISFIRST == 'true') {//第一次扫描
                                                        $('#fromfactoryselect').append("<option selected='selected' value='" + result.bundles[0].FACTORY + "'>" + result.bundles[0].FACTORY + "</option>");
                                                        $('#fromprocessselect').append("<option selected='selected' value='" + result.bundles[0].PROCESS + "'>" + result.bundles[0].PROCESS + "</option>");
                                                        $('#fromproductionselect').append("<option selected='selected' value='" + result.bundles[0].PRODUCTION + "'>" + result.bundles[0].PRODUCTION + "</option>");
                                                        $('#fromfactoryselect').selectmenu('refresh');
                                                        $('#fromprocessselect').selectmenu('refresh');
                                                        $('#fromproductionselect').selectmenu('refresh');
                                                    }

                                                    var html = result.bundles[0].HTML;
                                                    if (html == '') {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage15 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        return;
                                                    }
                                                    arr = html.split('@');
                                                    pagenum = Math.ceil(arr.length / 10);
                                                    if (currentpagenum > pagenum) {
                                                        currentpagenum = pagenum;
                                                    }
                                                    html = "";
                                                    $('#barcodelisttable tr').empty();
                                                    html += tablelogic1(arr, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'true');
                                                    $('#barcodelisttable tbody').append(html);
                                                    $('#barcodelisttable').trigger('create');

                                                    $('#selector').pagination({
                                                        items: arr.length,
                                                        itemsOnPage: 10,
                                                        cssStyle: 'light-theme',
                                                        currentPage: currentpagenum,
                                                        prevText: globeltranslate.prevbutton,
                                                        nextText: globeltranslate.nextbutton,
                                                        onPageClick: function (pagenumber, event) {
                                                            currentpagenum = pagenumber;
                                                            $('#barcodelisttable tbody').empty();
                                                            html = "";
                                                            html += tablelogic(arr, currentpagenum, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'true');
                                                            $('#barcodelisttable tbody').append(html);
                                                            $('#barcodelisttable').trigger('create');
                                                        }
                                                    });

                                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                                        if (i <= 11) {
                                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                        }
                                                    });

                                                    $('#totalbundlediv').html(result.cartonstatus[0].TOTALBUNDLES);
                                                    $('#totalpcsdiv').html(result.cartonstatus[0].TOTALPCS);
                                                    $('#totalgarmentpcs').html(result.cartonstatus[0].TOTALGARMENTPCS);
                                                    $('#totalpartdiv').html(result.cartonstatus[0].TOTALPART);

                                                    if (result.cartonstatus[0].ISPROCESS == "false")
                                                        bundlescanstatus = "0";
                                                    if (bundlescanstatus == '1' && result.cartonstatus[0].ISSUBMIT == "true")
                                                        bundlescanstatus = "0";
                                                    if (bundlescanstatus == '2' && result.cartonstatus[0].ISPROCESS == 'true' && result.cartonstatus[0].ISSUBMIT == 'false')
                                                        bundlescanstatus = '1';
                                                    if (result.cartonstatus[0].CANREPRINT == 'true')
                                                        reprintstatus = '1';
                                                    else
                                                        reprintstatus = '0';
                                                    if (result.cartonstatus[0].ISSUBMIT == "true")
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage8 + "</td></tr>");
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage5 + barcode + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                } else if (data.d.substr(0, 5) == 'false') {
                                                    if (data.d == "false1") {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage1 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.cartonscanmessage1);
                                                    } else if (data.d == "false2") {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage2 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.cartonscanmessage2);
                                                    } else if (data.d == "false3") {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage3 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                    } else if (data.d == "false4") {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage4 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.cartonscanmessage4);
                                                    } else if (data.d == "false9") {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage9 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.cartonscanmessage9);
                                                    }
                                                }
                                                btnstatus();
                                                selectcontrol('1', '1', '1');
                                            },
                                            error: function (err) {
                                                $.mobile.loading('hide');
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "013</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage + "013");
                                            }
                                        });
                                    //}, 10);
                                }
                                else if (barcodetype == 'D' && tosew == 'false') {
                                    //加载流水单的裁片信息
                                    if (($('#nextfactoryselect').val() == factoryselect || $('#nextprocessselect').val() == processselect) && ($('#processtypeselect').val() == 'I' || $('#processtypeselect').val() == 'P')) {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage5 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.bundlescanmessage5);
                                        return;
                                    }
                                    //var s;
                                    //s = "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'doc_no': '" + barcode + "', 'fromfactoryselected': '" + $('#fromfactoryselect').val() + "', 'fromprocessselected': '" + $('#fromprocessselect').val() + "', 'fromproductionselected': '" + $('#fromproductionselect').val() + "', 'fromfactory': '" + fromfactoryselected + "', 'dbtransdefect': '" + globeltranslate.tabletranslation13 + "', 'dbtransdelete': '" + globeltranslate.tabletranslation14 + "', 'get_othercarton': '" + get_othercarton + "' }";
                                    //alert(s);
                                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                    //setTimeout(function () {
                                        $.ajax({
                                            type: "POST",
                                            url: "Transaction.aspx/DOCNOscan",
                                            contentType: "application/json;charset=utf-8",
                                            //async: false,
                                            dataType: "json",
                                            data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'doc_no': '" + barcode + "', 'fromfactoryselected': '" + $('#fromfactoryselect').val() + "', 'fromprocessselected': '" + $('#fromprocessselect').val() + "', 'fromproductionselected': '" + $('#fromproductionselect').val() + "', 'fromfactory': '" + fromfactoryselected + "', 'dbtransdefect': '" + globeltranslate.tabletranslation13 + "', 'dbtransdelete': '" + globeltranslate.tabletranslation14 + "', 'get_othercarton': '" + get_othercarton + "', 'part': '" + $('#partselect').val() + "' }",
                                            success: function (data) {
                                                $.mobile.loading('hide');
                                                $('#barcodetext').val("");
                                                $('#barcodetext').focus();
                                                if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                                    var result = $.parseJSON(data.d);
                                                    doc_no = barcode;
                                                    $('#carno').val(result.bundles[0].CARNO);//added on 201-02-26
                                                    if (result.bundles[0].ISFIRST == 'true') {//第一次扫描
                                                        $('#fromfactoryselect').append("<option selected='selected' value='" + result.bundles[0].FACTORY + "'>" + result.bundles[0].FACTORY + "</option>");
                                                        $('#fromprocessselect').append("<option selected='selected' value='" + result.bundles[0].PROCESS + "'>" + result.bundles[0].PROCESS + "</option>");
                                                        $('#fromproductionselect').append("<option selected='selected' value='" + result.bundles[0].PRODUCTION + "'>" + result.bundles[0].PRODUCTION + "</option>");
                                                        $('#fromfactoryselect').selectmenu('refresh');
                                                        $('#fromprocessselect').selectmenu('refresh');
                                                        $('#fromproductionselect').selectmenu('refresh');
                                                    }
                                                    if (result.bundles[0].ISPRODUCTION == 'false') {//裁片有不同组别
                                                        bundlescanstatus = "0";
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage8 + $('#fromproductionselect').val() + globeltranslate.bundlescanmessage9 + result.bundles[0].WRONGPRODUCTION + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.bundlescanmessage8 + $('#fromproductionselect').val() + globeltranslate.bundlescanmessage9 + result.bundles[0].WRONGPRODUCTION);
                                                        return;
                                                    }

                                                    var html = result.bundles[0].HTML;
                                                    if (html == '') {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage15 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        return;
                                                    }
                                                    arr = html.split('@');
                                                    pagenum = Math.ceil(arr.length / 10);
                                                    if (currentpagenum > pagenum) {
                                                        currentpagenum = pagenum;
                                                    }
                                                    html = "";
                                                    $('#barcodelisttable tr').empty();
                                                    html += tablelogic1(arr, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'false');
                                                    $('#barcodelisttable tbody').append(html);
                                                    $('#barcodelisttable').trigger('create');

                                                    $('#selector').pagination({
                                                        items: arr.length,
                                                        itemsOnPage: 10,
                                                        cssStyle: 'light-theme',
                                                        currentPage: currentpagenum,
                                                        prevText: globeltranslate.prevbutton,
                                                        nextText: globeltranslate.nextbutton,
                                                        onPageClick: function (pagenumber, event) {
                                                            currentpagenum = pagenumber;
                                                            $('#barcodelisttable tbody').empty();
                                                            html = "";
                                                            html += tablelogic(arr, currentpagenum, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'false');
                                                            $('#barcodelisttable tbody').append(html);
                                                            $('#barcodelisttable').trigger('create');
                                                        }
                                                    });

                                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                                        if (i <= 12)
                                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    });

                                                    $('#totalbundlediv').html(result.bundlestatus[0].TOTALBUNDLES);
                                                    $('#totalpcsdiv').html(result.bundlestatus[0].TOTALPCS);
                                                    $('#totalgarmentpcs').html(result.bundlestatus[0].TOTALGARMENTPCS);
                                                    $('#totalpartdiv').html(result.bundlestatus[0].TOTALPART);

                                                    if (result.bundlestatus[0].ISPROCESS == "false")
                                                        bundlescanstatus = "0";
                                                    if (bundlescanstatus == '1' && result.bundlestatus[0].ISSUBMIT == "true")
                                                        bundlescanstatus = "0";
                                                    if (bundlescanstatus == '2' && result.bundlestatus[0].ISPROCESS == 'true' && result.bundlestatus[0].ISSUBMIT == 'false')
                                                        bundlescanstatus = '1';
                                                    if (result.bundlestatus[0].CANREPRINT == 'true')
                                                        reprintstatus = '1';
                                                    else
                                                        reprintstatus = '0';
                                                    if (result.bundlestatus[0].ISSUBMIT == "true")
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage8 + "</td></tr>");
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage14 + barcode + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                } else if (data.d == "false1") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage6 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.bundlescanmessage6);
                                                } else if (data.d == "false2") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + barcode + globeltranslate.s + $('#partselect').val() + globeltranslate.bundlescanmessage2 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                } else if (data.d == "false3") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage7 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.bundlescanmessage7);
                                                }
                                                $('#barcodetext').val("");
                                                btnstatus();
                                                selectcontrol('1', '1', '1');
                                            },
                                            error: function (err) {
                                                $.mobile.loading('hide');
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "054</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage + "054");
                                            }
                                        });
                                    //}, 10);
                                }
                                else if (barcodetype == 'D' && tosew == 'true') {
                                    //加载流水单的裁片信息
                                    if (($('#nextfactoryselect').val() == factoryselect || $('#nextprocessselect').val() == processselect) && ($('#processtypeselect').val() == 'I' || $('#processtypeselect').val() == 'P')) {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage5 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.bundlescanmessage5);
                                        return;
                                    }
                                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                    //setTimeout(function () {
                                        $.ajax({
                                            type: "POST",
                                            url: "Transaction.aspx/DCtoSEWdocnoscan",
                                            contentType: "application/json;charset=utf-8",
                                            //async: false,
                                            dataType: "json",
                                            data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'nextprocess': '" + $('#nextprocessselect').val() + "', 'barcode': '" + barcode + "', 'nextproduction': '" + $('#nextproductionlineselect').val() + "', 'productionselect': '" + productionselect + "', 'fromfactoryselected': '" + $('#fromfactoryselect').val() + "', 'fromprocessselected': '" + $('#fromprocessselect').val() + "', 'fromproductionselected': '" + $('#fromproductionselect').val() + "', 'fromfactory': '" + fromfactoryselected + "', 'dbtransdefect': '" + globeltranslate.tabletranslation13 + "', 'dbtransdelete': '" + globeltranslate.tabletranslation14 + "', 'get_othercarton': '" + get_othercarton + "' }",
                                            success: function (data) {
                                                $.mobile.loading('hide');
                                                $('#barcodetext').val("");
                                                $('#barcodetext').focus();
                                                if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                                    var result = $.parseJSON(data.d);
                                                    if (result.bundles[0].ISFIRST == 'true') {//第一次扫描
                                                        $('#fromfactoryselect').append("<option selected='selected' value='" + result.bundles[0].FACTORY + "'>" + result.bundles[0].FACTORY + "</option>");
                                                        $('#fromprocessselect').append("<option selected='selected' value='" + result.bundles[0].PROCESS + "'>" + result.bundles[0].PROCESS + "</option>");
                                                        $('#fromproductionselect').append("<option selected='selected' value='" + result.bundles[0].PRODUCTION + "'>" + result.bundles[0].PRODUCTION + "</option>");
                                                        $('#fromfactoryselect').selectmenu('refresh');
                                                        $('#fromprocessselect').selectmenu('refresh');
                                                        $('#fromproductionselect').selectmenu('refresh');
                                                    }

                                                    var html = result.bundles[0].HTML;
                                                    if (html == '') {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage15 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                         return;
                                                    }
                                                  
                                                    //tangyh 2017.04.05 
                                                     
                                                        if (result.cartonstatus[0].ISSUBMIT == "true") {
                                                            $('#unsubmitbutton').removeAttr('disabled').button('refresh');
                                                            doc_no = barcode;
                                                        }
                                                        else
                                                            $('#unsubmitbutton').attr('disabled', 'true').button('refresh'); //tangyh 2017.04.05
                                               
                                                    arr = html.split('@');
                                                    pagenum = Math.ceil(arr.length / 10);
                                                    if (currentpagenum > pagenum) {
                                                        currentpagenum = pagenum;
                                                    }
                                                    html = "";
                                                    $('#barcodelisttable tr').empty();
                                                    html += tablelogic1(arr, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'true');
                                                    $('#barcodelisttable tbody').append(html);
                                                    $('#barcodelisttable').trigger('create');

                                                    $('#selector').pagination({
                                                        items: arr.length,
                                                        itemsOnPage: 10,
                                                        cssStyle: 'light-theme',
                                                        currentPage: currentpagenum,
                                                        prevText: globeltranslate.prevbutton,
                                                        nextText: globeltranslate.nextbutton,
                                                        onPageClick: function (pagenumber, event) {
                                                            currentpagenum = pagenumber;
                                                            $('#barcodelisttable tbody').empty();
                                                            html = "";
                                                            html += tablelogic(arr, currentpagenum, globeltranslate.tabletranslation14, globeltranslate.tabletranslation13, 'true');
                                                            $('#barcodelisttable tbody').append(html);
                                                            $('#barcodelisttable').trigger('create');
                                                        }
                                                    });

                                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                                        if (i <= 11) {
                                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                        }
                                                    });

                                                    $('#totalbundlediv').html(result.cartonstatus[0].TOTALBUNDLES);
                                                    $('#totalpcsdiv').html(result.cartonstatus[0].TOTALPCS);
                                                    $('#totalgarmentpcs').html(result.cartonstatus[0].TOTALGARMENTPCS);
                                                    $('#totalpartdiv').html(result.cartonstatus[0].TOTALPART);

                                                    if (result.cartonstatus[0].ISPROCESS == "false")
                                                        bundlescanstatus = "0";
                                                    if (bundlescanstatus == '1' && result.cartonstatus[0].ISSUBMIT == "true")
                                                        bundlescanstatus = "0";
                                                    if (bundlescanstatus == '2' && result.cartonstatus[0].ISPROCESS == 'true' && result.cartonstatus[0].ISSUBMIT == 'false')
                                                        bundlescanstatus = '1';
                                                    if (result.cartonstatus[0].CANREPRINT == 'true')
                                                        reprintstatus = '1';
                                                    else
                                                        reprintstatus = '0';
                                                    if (result.cartonstatus[0].ISSUBMIT == "true")
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage8 + "</td></tr>");
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage5 + barcode + "</td></tr>");
                                                       $('#messagetable').trigger('create');
                                                } else if (data.d.substr(0, 5) == 'false') {
                                                    if (data.d == "false1") {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage1 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.cartonscanmessage1);
                                                    } else if (data.d == "false2") {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage2 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.cartonscanmessage2);
                                                    } else if (data.d == "false3") {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage3 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                    } else if (data.d == "false4") {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage4 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.cartonscanmessage4);
                                                    } else if (data.d == "false9") {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage9 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.cartonscanmessage9);
                                                    }
                                                }
                                                btnstatus();
                                                selectcontrol('1', '1', '1');
                                            },
                                            error: function (err) {
                                                $.mobile.loading('hide');
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "013</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage + "013");
                                            }
                                        });
                                    //}, 10);
                                }
                            }
                        }
                    }
                });

                $('#contractnoselect').change(function () {
                    contractselect = '1';
                    btnstatus();
                });

                //取消提交按钮的监控
                $('#unsubmitbutton').click(function () {
                    var factory = sessionStorage.getItem("factory");
                    var process = sessionStorage.getItem("process");
                    var production = sessionStorage.getItem("productionline");
                    var nextfactory = $('#nextfactoryselect').val();
                    var nextprocess = $('#nextprocessselect').val();
                    var nextproduction = $('#nextproductionlineselect').val();
                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                    //setTimeout(function () {
                        $.ajax({
                            type: "POST",
                            url: "Transaction.aspx/Unsubmit",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            async: false,
                            data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + process + "', 'docno': '" + docno + "', 'doc_no': '" + doc_no + "' }",
                            success: function (data) {
                                $.mobile.loading('hide');
                                if (data.d == 'false1') {
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.unsubmitmessage2 + "</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.unsubmitmessage2);
                                } else if (data.d == 'false2') {
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.unsubmitmessage3 + "</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.unsubmitmessage3);
                                } else if (data.d == 'false3') {
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.unsubmitmessage4 + "</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.unsubmitmessage4);
                                } else if (data.d == 'success') {
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.unsubmitmessage1 + doc_no + "</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.unsubmitmessage1 + doc_no);
                                    empty();
                                    doc_no = '';
                                    btnstatus();
                                }
                            },
                            error: function () {
                                $.mobile.loading('hide');
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "080</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.errormessage + "080");
                            }
                        });
                    //}, 10);
                });

                //提交按钮的监控
                $('#submitbutton').click(function () {
                    var factory = sessionStorage.getItem("factory");
                    var process = $('#fromprocessselect').val();
                    var production = $('#fromproductionselect').val();
                    var nextfactory = $('#nextfactoryselect').val();
                    var nextprocess = $('#nextprocessselect').val();
                    var nextproduction = $('#nextproductionlineselect').val();
                    var languagearray = globeltranslate.user + "*" + globeltranslate.datetime + "*" + globeltranslate.fromdept + "*" + globeltranslate.fromline + "*" + globeltranslate.toline + "*" + globeltranslate.parts + "*" + globeltranslate.remark + "*" + globeltranslate.customer + "*" + globeltranslate.summary + "*" + globeltranslate.jo + "*" + globeltranslate.doc + "*" + globeltranslate.color + "*" + globeltranslate.layno + "*" + globeltranslate.bundles + "*" + globeltranslate.totalqty + "*" + globeltranslate.totalcutqty + "*" + globeltranslate.currenttotalqty + "*" + globeltranslate.totaloutputqty + "*" + globeltranslate.residualqty + "*" + globeltranslate.cartonnum + "*" + globeltranslate.automatching;
                    languagearray = languagearray + "*" + globeltranslate.print_1 + "*" + globeltranslate.print_2 + "*" + globeltranslate.print_3 + "*" + globeltranslate.print_4 + "*" + globeltranslate.print_5 + "*" + globeltranslate.print_6 + "*" + globeltranslate.print_7 + "*" + globeltranslate.print_8 + "*" + globeltranslate.print_9 + "*" + globeltranslate.print_10;
                    //流转提交
                    if (radioselected == '1') {
                        //本厂流转提交：提交到印花厂
                        if ($('#processtypeselect').val() == 'I') {
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            $('#navbar').hide();
                            //setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: "Transaction.aspx/Transactionsubmit",
                                    contentType: "application/json;charset=utf-8",
                                    dataType: "json",
                                    //async: false,
                                    data: "{ 'username': '" + sessionStorage.getItem("name") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'nextfactory':'" + nextfactory + "', 'process': '" + process + "', 'nextprocess':'" + nextprocess + "', 'productionline':'" + production + "', 'nextproductionline':'" + nextproduction + "', 'productionselect': '" + productionselect + "', 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'languagearray': '" + languagearray + "' }",
                                    success: function (data) {
                                        $.mobile.loading('hide');
                                        $('#navbar').show();
                                        if (data.d.substr(0, 5) != 'error' && data.d.substr(0, 5) != 'false') {
                                            var result = eval(data.d);
                                            if (result[0].STATUS == 'Y') {
                                                alert(globeltranslate.submitmessage1 + result[0].DOCNO);
                                                printhtml = result[0].HTML;
                                                cartonbarcode = result[0].DOCNO;
                                                $('#jumpprint').trigger('click');
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.submitmessage1 + result[0].DOCNO + "</td></tr>");
                                                empty();
                                            }
                                            else if (result[0].STATUS == 'N') {
                                                var am = bundlestatusalertmessage(result, globeltranslate);
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + am + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                btnstatuscontrol('1');
                                                alert(am);
                                            }
                                        }
                                        else if (data.d.substr(0, 5) == 'false') {

                                        }
                                        else if (data.d.substr(0, 5) == 'error') {
                                            alert(data.d);
                                        }
                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                        $('#navbar').show();
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "036</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage + "036");
                                    }
                                });
                            //}, 10);
                        }
                        //Peer厂流转提交：提交到Peer厂
                        else if ($('#processtypeselect').val() == 'P') {
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            $('#navbar').hide();
                            //setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: "Transaction.aspx/Peertransactionsubmit",
                                    contentType: "application/json;charset=utf-8",
                                    dataType: "json",
                                    //async: false,
                                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'nextfactory':'" + nextfactory + "', 'process': '" + process + "', 'nextprocess':'" + nextprocess + "', 'productionline':'" + $('#fromproductionselect').val() + "', 'nextproductionline':'NA', 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "' }",
                                    success: function (data) {
                                        $.mobile.loading('hide');
                                        $('#navbar').show();
                                        var result = eval(data.d);
                                        empty();
                                        $('#test').empty();
                                        $('#test').append(result[0].HTML);
                                        $('#test').jqprint();
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage1 + result[0].DOCNO + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.confirmmessage1 + result[0].DOCNO);
                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                        $('#navbar').show();
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "075</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage + "075");
                                    }
                                });
                            //}, 10);
                        }
                    }
                    //返工提交
                    else if (radioselected == '2') {
                        if ($('#processtypeselect').val() == 'I') {
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            $('#navbar').hide();
                            //setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: "Transaction.aspx/Reworksubmit",
                                    contentType: "application/json;charset=utf-8",
                                    dataType: "json",
                                    //async: false,
                                    data: "{ 'STATUS': 'M', 'username': '" + sessionStorage.getItem("name") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'nextfactory':'" + nextfactory + "', 'process': '" + process + "', 'nextprocess':'" + nextprocess + "', 'productionline':'" + production + "', 'nextproductionline':'" + nextproduction + "', 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'productionselect': '" + productionselect + "', 'languagearray': '" + translation + "' }",
                                    success: function (data) {
                                        $.mobile.loading('hide');
                                        $('#navbar').show();
                                        var result = eval(data.d);
                                        alert(globeltranslate.submitmessage1 + result[0].DOCNO);
                                        printhtml = result[0].HTML;
                                        cartonbarcode = result[0].DOCNO;
                                        $('#jumpprint').trigger('click');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.submitmessage1 + result[0].DOCNO + "</td></tr>");
                                        empty();
                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                        $('#navbar').show();
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "025</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage + "025");
                                    }
                                });
                            //}, 10);
                        } else if ($('#processtypeselect').val() == 'P') {
                            $.ajax({
                                type: "POST",
                                url: "Transaction.aspx/Peerreworksubmit",
                                contentType: "application/json;charset=utf-8",
                                dataType: "json",
                                async: false,
                                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'nextfactory':'" + nextfactory + "', 'process': '" + process + "', 'nextprocess':'" + nextprocess + "', 'productionline':'" + production + "', 'nextproductionline':'" + nextproduction + "', 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "' }",
                                beforeSend: function () { $.mobile.loading('show'); },
                                success: function (data) {
                                    empty();
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.submitmessage1 + nextfactory + "/" + nextprocess + "/" + nextproduction + "</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.submitmessage1);
                                    $.mobile.loading('hide');
                                },
                                error: function (err) {
                                    $.mobile.loading('hide');
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "520</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.errormessage + "025");
                                }
                            });
                        }
                    }
                    //调整提交
                    else if (radioselected == '3') {
                        if ($('#processtypeselect').val() == 'I') {
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            $('#navbar').hide();
                            //setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: "Transaction.aspx/Reworksubmit",
                                    contentType: "application/json;charset=utf-8",
                                    dataType: "json",
                                    //async: false,
                                    data: "{ 'STATUS': 'A', 'username': '" + sessionStorage.getItem("name") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'nextfactory':'" + nextfactory + "', 'process': '" + process + "', 'nextprocess':'" + nextprocess + "', 'productionline':'" + production + "', 'nextproductionline':'" + nextproduction + "', 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'productionselect': '" + productionselect + "', 'languagearray': '" + translation + "' }",
                                    success: function (data) {
                                        $.mobile.loading('hide');
                                        $('#navbar').show();
                                        var result = eval(data.d);
                                        alert(globeltranslate.confirmmessage1 + result[0].DOCNO);
                                        printhtml = result[0].HTML;
                                        cartonbarcode = result[0].DOCNO;
                                        $('#jumpprint').trigger('click');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage1 + result[0].DOCNO + "</td></tr>");
                                        empty();

                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide'); 
                                        $('#navbar').show();
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "156</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage + "156");
                                    }
                                });
                           // }, 10);
                        }
                    }
                });

                //confirm button
                $('#confirmbutton').click(function () {
                    var factory = sessionStorage.getItem("factory");
                    var process = sessionStorage.getItem("process");
                    var production = sessionStorage.getItem("productionline");
                    var nextfactory = $('#nextfactoryselect').val();
                    var nextprocess = $('#nextprocessselect').val();
                    var nextproduction = $('#nextproductionlineselect').val();
                    var cardno = $('#carno').val();
                    var languagearray = globeltranslate.user + "*" + globeltranslate.datetime + "*" + globeltranslate.fromdept + "*" + globeltranslate.fromline + "*" + globeltranslate.toline + "*" + globeltranslate.parts + "*" + globeltranslate.remark + "*" + globeltranslate.customer + "*" + globeltranslate.summary + "*" + globeltranslate.jo + "*" + globeltranslate.doc + "*" + globeltranslate.color + "*" + globeltranslate.layno + "*" + globeltranslate.bundles + "*" + globeltranslate.totalqty + "*" + globeltranslate.totalcutqty + "*" + globeltranslate.currenttotalqty + "*" + globeltranslate.totaloutputqty + "*" + globeltranslate.residualqty + "*" + globeltranslate.cartonnum + "*" + globeltranslate.automatching;
                    languagearray = languagearray + "*" + globeltranslate.print_1 + "*" + globeltranslate.print_2 + "*" + globeltranslate.print_3 + "*" + globeltranslate.print_4 + "*" + globeltranslate.print_5 + "*" + globeltranslate.print_6 + "*" + globeltranslate.print_7 + "*" + globeltranslate.print_8 + "*" + globeltranslate.print_9 + "*" + globeltranslate.print_10;
                   //检测车号是否为空
                    //if (process == "CUT" && nextprocess == "PRT" && cardno == "" && factory=="YMG") {
                    //    alert(globeltranslate.carnotip);
                    //    return false;
                    //}
        
                    //外发确认流转
                    if ($('#processtypeselect').val() == 'O' || $('#processtypeselect').val() == 'E') {
                        $.ajax({
                            type: "POST",
                            url: "Transaction.aspx/OASconfirm",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            //async: false,
                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + process + "', 'production':'" + $('#fromproductionselect').val() + "', 'nextprocess':'" + nextprocess + "', 'contractno': '" + $('#contractnoselect').val() + "', 'processtype': '" + $('#processtypeselect').val() + "', 'docno':'" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "' ,'carno':'" + $('#carno').val() + "' }",
                            success: function (data) {
                                empty();
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage3 + data.d + "</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.confirmmessage3 + data.d);
                            },
                            error: function (err) {
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "054</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.errormessage + "054");
                            }
                        });
                    }
                    else if ($('#processtypeselect').val() == 'I') {
                        //正常流转
                        if (radioselected == '1') {
                            if (process == nextprocess) {//process为I 的transaction平级调动
                                if (nextproduction == productionselect) {
                                    alert(globeltranslate.confirmmessage5);
                                    return;
                                } else {
                                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                    $('#navbar').hide();
                                    //setTimeout(function () {
                                        $.ajax({
                                            type: "POST",
                                            url: "Transaction.aspx/Transactionconfirm1",
                                            contentType: "application/json;charset=utf-8",
                                            dataType: "json",
                                            //async: false,
                                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'nextfactory':'" + nextfactory + "', 'process': '" + process + "', 'nextprocess':'" + nextprocess + "', 'productionline':'" + sessionStorage.getItem("productionline") + "', 'nextproductionline':'" + nextproduction + "', 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'languagearray': '" + languagearray + "','carno':'" + $('#carno').val() + "' }",
                                            success: function (data) {
                                                $.mobile.loading('hide');
                                                $('#navbar').show();
                                                var result = eval(data.d);
                                                alert(globeltranslate.confirmmessage4 + result[0].DOCNO);
                                                printhtml = result[0].HTML;
                                                cartonbarcode = result[0].DOCNO;
                                                $('#jumpprint').trigger('click');
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage4 + result[0].DOCNO + "</td></tr>");
                                                debugger;
                                                empty();
                                            },
                                            error: function (err) {
                                                $.mobile.loading('hide');
                                                $('#navbar').show();
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "015</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage + "015");
                                            }
                                        });
                                    //}, 10);
                                }
                            }
                            else if (process != nextprocess) {//process为I 的transaction下道部门流转
                                if (nextproduction == productionselect)
                                    nextproduction = "NA";
                                if (nextprocess.substr(0, 3) != 'SEW') {
                                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                    $('#navbar').hide();
                                    //setTimeout(function () {
                                        $.ajax({
                                            type: "POST",
                                            url: "Transaction.aspx/Transactionconfirm2",
                                            contentType: "application/json;charset=utf-8",
                                            dataType: "json",
                                            //async: false,
                                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'nextfactory':'" + nextfactory + "', 'process': '" + process + "', 'nextprocess':'" + nextprocess + "', 'productionline':'" + $('#fromproductionselect').val() + "', 'nextproductionline':'" + nextproduction + "', 'productionselect': '" + productionselect + "', 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'languagearray': '" + languagearray + "','carno':'" + $('#carno').val() + "' }",
                                            success: function (data) {
                                                $.mobile.loading('hide');
                                                $('#navbar').show();
                                                if (data.d.substr(0, 5) != 'error' && data.d.substr(0, 5) != 'false') {
                                                    var result = eval(data.d);
                                                    if (result[0].STATUS == 'Y') {
                                                        //alert(globeltranslate.confirmmessage4 + result[0].DOCNO + globeltranslate.confirmmessage14);
                                                        if (result[0].AUTOMATCHING == '1') {
                                                            alert(globeltranslate.confirmmessage4 + result[0].DOCNO + globeltranslate.confirmmessage14);
                                                        }
                                                        else if (result[0].AUTOMATCHING == '11') {
                                                            alert(globeltranslate.confirmmessage4 + result[0].DOCNO + globeltranslate.confirmmessage15 + '11');
                                                        }
                                                        else
                                                            alert(globeltranslate.confirmmessage4 + result[0].DOCNO);
                                                        printhtml = result[0].HTML;
                                                        cartonbarcode = result[0].DOCNO;
                                                        $('#jumpprint').trigger('click');
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage4 + result[0].DOCNO + "</td></tr>");
                                                        empty();
                                                    }
                                                    else if (result[0].STATUS == 'N') {
                                                        var am = bundlestatusalertmessage(result, globeltranslate);
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + am + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(am);
                                                    }
                                                    else if (result[0].STATUS == 'V') {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage12 + result[0].DOCNO + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        //alert(globeltranslate.confirmmessage12 + result[0].DOCNO);
                                                        if (result[0].AUTOMATCHING == '1') {
                                                            alert(globeltranslate.confirmmessage12 + result[0].DOCNO + globeltranslate.confirmmessage14);
                                                        }
                                                        else if (result[0].AUTOMATCHING == '11') {
                                                            alert(globeltranslate.confirmmessage12 + result[0].DOCNO + globeltranslate.confirmmessage15 + '11');
                                                        }
                                                        else
                                                            alert(globeltranslate.confirmmessage12 + result[0].DOCNO);
                                                    }
                                                }
                                                else if (data.d.substr(0, 5) == 'error') {
                                                    if (data.d.substr(0, 6) == 'error1') {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage5 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.errormessage5);
                                                    } else if (data.d.substr(0, 6) == 'error2') {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage7 + data.d.substr(6) + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.errormessage7 + data.d.substr(6));
                                                    }
                                                }
                                                else if (data.d.substr(0, 5) == 'false') {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage4 + data.d.substr(6) + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(data.d.substr(6));
                                                }
                                            },
                                            error: function (err) {
                                                $.mobile.loading('hide');
                                                $('#navbar').show();
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "473</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage + "473");
                                            }
                                        });
                                    //}, 10);
                                }
                                else {
                                    //GTN流转
                                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                    $('#navbar').hide();
                                    //setTimeout(function () {
                                        $.ajax({
                                            type: "POST",
                                            url: "Transaction.aspx/GTNconfirm",
                                            contentType: "application/json;charset=utf-8",
                                            dataType: "json",
                                            //async: false,
                                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'nextfactory':'" + nextfactory + "', 'processtype':'" + $('#processtypeselect').val() + "', 'process': '" + process + "', 'nextprocess':'" + nextprocess + "', 'productionline':'" + $('#fromproductionselect').val() + "', 'nextproductionline':'" + nextproduction + "', 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'languagearray': '" + languagearray + "','carno':'" + $('#carno').val() + "' }",
                                            success: function (data) {
                                                $.mobile.loading('hide');
                                                $('#navbar').show();
                                                if (data.d.substr(0, 5) != 'error' && data.d.substr(0, 5) != 'false') {
                                                    var result = eval(data.d);
                                                    if (result[0].STATUS == 'Y') {
                                                        alert(globeltranslate.confirmmessage7 + result[0].DOCNO);
                                                        printhtml = result[0].HTML;
                                                        cartonbarcode = result[0].DOCNO;
                                                        $('#jumpprint').trigger('click');
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage7 + result[0].DOCNO + "</td></tr>");
                                                        empty();
                                                    }
                                                    else if (result[0].STATUS == 'V') {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage12 + result[0].DOCNO + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.confirmmessage12 + result[0].DOCNO);
                                                    }
                                                    else if (result[0].STATUS == 'N') {
                                                        var am = bundlestatusalertmessage(result, globeltranslate);
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + am + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(am);
                                                    }
                                                }
                                                else if (data.d.substr(0, 5) == 'false') {
                                                    if (data.d == 'false5') {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmesssage6 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.confirmmesssage6);
                                                    } else if (data.d == 'false1') {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage8 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.confirmmessage8);
                                                    } else if (data.d == "false9") {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage11 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.confirmmessage11);
                                                    } else if (data.d == "false2") {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage13 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.confirmmessage13);
                                                    }
                                                }
                                                else if (data.d.substr(0, 5) == 'error') {
                                                    if (data.d.substr(0, 6) == 'error2') {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage8 + data.d.substr(6) + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.errormessage8 + data.d.substr(6));
                                                    }
                                                    else if (data.d.substr(0, 6) == 'error1') {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage5 + "121" + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.errormessage5 + "121");
                                                    }
                                                }
                                            },
                                            error: function (err) {
                                                $.mobile.loading('hide');
                                                $('#navbar').show();
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "443</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage + "443");
                                            }
                                        });
                                    //}, 10);
                                }
                            }
                        }
                        else if (radioselected == '2') {//process为I 的rework的上道部门流转
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            $('#navbar').hide();
                            //setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: "Transaction.aspx/Reworkconfirm",
                                    contentType: "application/json;charset=utf-8",
                                    dataType: "json",
                                    //async: false,
                                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'nextfactory':'" + nextfactory + "', 'process': '" + process + "', 'nextprocess':'" + nextprocess + "', 'productionline':'" + sessionStorage.getItem("productionline") + "', 'nextproductionline':'" + nextproduction + "', 'productionselect': '" + productionselect + "', 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'languagearray': '" + languagearray + "','carno':'" + $('#carno').val() + "' }",
                                    success: function (data) {
                                        $.mobile.loading('hide');
                                        $('#navbar').show();
                                        if (data.d.substr(0, 5) != 'error' && data.d.substr(0, 5) != 'false') {
                                            var result = eval(data.d);
                                            alert(globeltranslate.confirmmessage2 + $('#nextprocessselect').val() + globeltranslate.confirmmessage9 + result[0].DOCNO);
                                            printhtml = result[0].HTML;
                                            cartonbarcode = result[0].DOCNO;
                                            $('#jumpprint').trigger('click');
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage2 + $('#nextprocessselect').val() + globeltranslate.confirmmessage9 + result[0].DOCNO + "</td></tr>");
                                            empty();
                                        }
                                        else if (data.d.substr(0, 5) == 'error') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage2 + "91" + data.d.substr(5) + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.errormessage2 + "91" + data.d.substr(5));
                                        }
                                        else if (data.d.substr(0, 5) == 'false') {
                                        }
                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                        $('#navbar').show();
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "088</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage + "088");
                                    }
                                });
                            //}, 10);
                        }
                        else if (radioselected == '3') {//process为I 的Adjustment
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            $('#navbar').hide();
                            if (process.substr(0, 3) != 'SEW') {
                                $.ajax({
                                    type: "POST",
                                    url: "Transaction.aspx/Adjustmentconfirm",
                                    contentType: "application/json;charset=utf-8",
                                    dataType: "json",
                                    //async: false,
                                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'thisfactory': '" + $('#fromfactoryselect').val() + "', 'thisprocess': '" + $('#fromprocessselect').val() + "', 'thisproduction': '" + $('#fromproductionselect').val() + "', 'nextfactory': '" + $('#nextfactoryselect').val() + "', 'nextprocess': '" + $('#nextprocessselect').val() + "', 'nextproduction':'" + $('#nextproductionlineselect').val() + "', 'languagearray': '" + languagearray + "','carno':'" + $('#carno').val() + "' }",
                                    success: function (data) {
                                        $.mobile.loading('hide');
                                        $('#navbar').show();
                                        if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                            var result = eval(data.d);
                                            alert(globeltranslate.confirmmessage10 + $('#nextprocessselect').val() + globeltranslate.confirmmessage9 + result[0].DOCNO);
                                            printhtml = result[0].HTML;
                                            cartonbarcode = result[0].DOCNO;
                                            $('#jumpprint').trigger('click');
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage10 + $('#nextprocessselect').val() + globeltranslate.confirmmessage9 + result[0].DOCNO + "</td></tr>");
                                            empty();
                                        }
                                        else if (data.d.substr(0, 5) == 'false') {
                                            if (data.d.substr(0, 6) == 'false4') {
                                                if (data.d.substr(6) == 'O') {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.statusmessage11 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.statusmessage11);
                                                }
                                                else if (data.d.substr(6) == 'B') {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.statusmessage9 + "B</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.statusmessage9 + "B");
                                                }
                                                else if (data.d.substr(6) == 'H') {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.statusmessage9 + "H</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.statusmessage9 + "H");
                                                }
                                                else if (data.d.substr(6) == 'D') {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.statusmessage10 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.statusmessage10);
                                                }
                                            }
                                        }
                                        else if (data.d.substr(0, 5) == 'error') {
                                            if (data.d == 'error1') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage3 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage3);
                                            }
                                        }
                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                        $('#navbar').show();
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "116</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage + "116");
                                    }
                                });
                            }
                            else {
                                $.ajax({
                                    type: "POST",
                                    url: "Transaction.aspx/GTNadjustment",
                                    contentType: "application/json;charset=utf-8",
                                    dataType: "json",
                                    //async: false,
                                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'thisfactory': '" + $('#fromfactoryselect').val() + "', 'thisprocess': '" + $('#fromprocessselect').val() + "', 'thisproduction': '" + $('#fromproductionselect').val() + "', 'processtype':'" + $('#processtypeselect').val() + "', 'nextfactory': '" + $('#nextfactoryselect').val() + "', 'nextprocess': '" + $('#nextprocessselect').val() + "', 'nextproduction':'" + $('#nextproductionlineselect').val() + "', 'languagearray': '" + languagearray + "','carno':'" + $('#carno').val() + "' }",
                                    success: function (data) {
                                        $.mobile.loading('hide');
                                        $('#navbar').show();
                                        if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                            var result = eval(data.d);
                                            if (result[0].STATUS == 'N') {
                                                var am = bundlestatusalertmessage(result, globeltranslate);
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + am + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(am);
                                            }
                                            else {
                                                alert(globeltranslate.confirmmessage10 + $('#nextprocessselect').val() + globeltranslate.confirmmessage9 + result[0].DOCNO);
                                                printhtml = result[0].HTML;
                                                cartonbarcode = result[0].DOCNO;
                                                $('#jumpprint').trigger('click');
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage10 + $('#nextprocessselect').val() + globeltranslate.confirmmessage9 + result[0].DOCNO + "</td></tr>");
                                                empty();
                                            }
                                        }
                                        else if (data.d.substr(0, 5) == 'false') {
                                            if (data.d.substr(0, 6) == 'false2') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage13 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.confirmmessage13);
                                            }
                                        }
                                        else if (data.d.substr(0, 5) == 'error') {
                                            if (data.d == 'error1') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage5 + "34</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage5 + "34");
                                            }
                                            else if (data.d == 'error2') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage6 + "35</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage6 + "35");
                                            }
                                            else if (data.d == 'error3') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage7 + "36</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage7 + "36");
                                            }
                                        }
                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                        $('#navbar').show();
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "116</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage + "116");
                                    }
                                });
                            }
                        }
                    }
                });
                //重新打印
                $('#reprintbutton').click(function () {
                    var languagearray = globeltranslate.user + "*" + globeltranslate.datetime + "*" + globeltranslate.fromdept + "*" + globeltranslate.fromline + "*" + globeltranslate.toline + "*" + globeltranslate.parts + "*" + globeltranslate.remark + "*" + globeltranslate.customer + "*" + globeltranslate.summary + "*" + globeltranslate.jo + "*" + globeltranslate.doc + "*" + globeltranslate.color + "*" + globeltranslate.layno + "*" + globeltranslate.bundles + "*" + globeltranslate.totalqty + "*" + globeltranslate.totalcutqty + "*" + globeltranslate.currenttotalqty + "*" + globeltranslate.totaloutputqty + "*" + globeltranslate.residualqty + "*" + globeltranslate.cartonnum + "*" + globeltranslate.automatching;
                    languagearray = languagearray + "*" + globeltranslate.print_1 + "*" + globeltranslate.print_2 + "*" + globeltranslate.print_3 + "*" + globeltranslate.print_4 + "*" + globeltranslate.print_5 + "*" + globeltranslate.print_6 + "*" + globeltranslate.print_7 + "*" + globeltranslate.print_8 + "*" + globeltranslate.print_9 + "*" + globeltranslate.print_10;
                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                    $('#navbar').hide();
                    //setTimeout(function () {
                        $.ajax({
                            type: "POST",
                            url: "Transaction.aspx/Reprintdocno",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            //async: false,
                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'languagearray': '" + languagearray + "' }",
                            success: function (data) {
                                $.mobile.loading('hide');
                                $('#navbar').show();
                                if (data.d == "false1") {
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.reprintmessage1 + "</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.reprintmessage1);
                                } else if (data.d == "false2") {
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.reprintmessage2 + "</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.reprintmessage2);
                                } else {
                                    var result = eval(data.d);
                                    alert(globeltranslate.reprintmessage3 + result[0].CARTON);
                                    printhtml = result[0].HTML;
                                    cartonbarcode = result[0].CARTON;
                                    $('#jumpprint').trigger('click');
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.reprintmessage3 + result[0].CARTON + "</td></tr>");
                                    empty();
                                }
                            },
                            error: function (err) {
                                $.mobile.loading('hide');
                                $('#navbar').show();
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "182</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.errormessage + "182");
                            }
                        });
                    //}, 10);
                });

                //transaction、rework、adjustment的选择监控
                $('input[name=radio]').change(function () {
                    $("#nextprocessselect").removeAttr("disabled");
                    $('#productiontr').show();
                    var radioselect = $("input[name='radio']:checked").val();
                    if (radioselect == 'transactionradio') {
                        radioselected = '1';
                        iprocess('true');
                        selectcontrol('0', '0', '0');
                        $('#bundlebarcodeother').show(); //流转时才显示
                        processtypetrans();
                    } else if (radioselect == 'reworkradio') {
                        radioselected = '2';
                        iprocess('false');
                        selectcontrol('0', '0', '0');
                        $('#bundlebarcodeother').hide();
                        processtypetrans();
                    } else if (radioselect == 'adjustmentradio') {
                        $('#bundlebarcodeother').hide();
                        radioselected = '3';
                        factoryselecttrans();
                        processselecttrans();
                        productionselecttrans();
                        $('#processtypeselect').empty();
                        $('#processtypeselect').append("<option select='selected'>" + globeltranslate.select + "</option>");
                        $('#processtypeselect').selectmenu('refresh');
                        selectcontrol('1', '0', '1');
                    }
                });
            }

        };
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
            <div data-role="navbar" id='navbar'>
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
                                    <li><input type="button" style="text-align:center" id="submitbutton" name="submitbutton" /></li>
                                </ul>
                            </td>
                            <td style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="unsubmitbutton" name="unsubmitbutton" /></li>
                                </ul>
                            </td>
                            <td style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="reprintbutton" name="reprintbutton" /></li>
                                </ul>
                            </td>
                            <td style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="emptylistbutton" name="emptylistbutton" /></li>
                                </ul>
                            </td>
                            <td style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="savebutton" name="savebutton" /></li>
                                </ul>
                            </td>
                            
                            <td style="width:10%">
                                <ul>
                                    <li>
                                        <select data-native-menu="false" id="querybutton" name="querybutton">
                                        </select>
                                    </li>
                                </ul>
                            </td>

                            <%--//20170615-tangyh--%>
                            <td style="width:40%">
                            </td>
                            <td style="width:40%">
                            </td>
                            <td style="width:20%" hidden="hidden" id="queryfactory_td">
                                <ul>
                                    <li>
                                        <select data-native-menu="false" id="queryfactory" name="queryfactory"></select>
                                    </li>
                                </ul>
                            </td>
                            <script>                                //20170615-tangyh
                                if (sessionStorage.getItem("process") == 'PRT') {
                                    $('#queryfactory_td').show();
                                }

                                $('#queryfactory').change(function () {
                                    if (($('#queryfactory').val() == "undefined") || ($('#queryfactory').val() == globeltranslate.queryfactory)) {
                                        return;
                                    }

                                    var int1 = $("#queryfactory").get(0).selectedIndex;
                                    var value1 = $('#queryfactory').get(0)[int1].innerText;
                                    //$('#queryfactory-button').html('<span>' + value1 + '</span>');

                                    $('#queryfactory').selectmenu('refresh');
                                    if (value1 != fty) {
                                        empty();
                                        fty = value1;
                                        firstflag = 'F';

                                        start(firstflag, fty);
                                    }
                                });
                            </script>     
                            

                            
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <div data-role="content" style="position: absolute; top: 17%; width: 100%; bottom: 2%;">
            <div class="ui-grid-d">
                <div class="ui-block-a">
                    <table style="width: 100%">
                        <tr id="transtypetr">
                            <td style="width: 3%"></td>
                            <td style="width: 97%">
                                <div data-role="fieldcontain" id="transtype">
                                    <fieldset data-role="controlgroup">
                                        <div id="transactiondiv">
                                            <input type="radio" name="radio" id="transactionradio" value="transactionradio" checked="checked" />
                                            <label style="width: 100%" for="transactionradio" id="transactionradiolabel"></label>
                                        </div>
                                        <div id="reworkdiv">
                                            <input type="radio" name="radio" id="reworkradio" value="reworkradio" />
                                            <label style="width: 100%" for="reworkradio" id="reworkradiolabel"></label>
                                        </div>
                                        <div id="adjustmentdiv">
                                            <input type="radio" name="radio" id="adjustmentradio" value="adjustmentradio" />
                                            <label style="width: 100%" for="adjustmentradio" id="adjustmentradiolabel"></label>
                                        </div>
                                    </fieldset>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-b">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="processtypeselect" id="processtypelabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <select id="processtypeselect" name="processtypeselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr id="nextfactorytr">
                            <td style="width: 30%; text-align: center"><label for="nextfactoryselect" id="nextfactorylabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <select id="nextfactoryselect" name="nextfactoryselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="nextprocessselect" id="nextprocesslabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <select id="nextprocessselect" name="nextprocessselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr id="productiontr">
                            <td style="width: 30%; text-align: center"></td>
                            <td style="width: 70%; text-align: right;">
                                <select id="nextproductionlineselect" name="nextproductionlineselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-c">
                    <table style="width: 100%">
                        <tr id="parttr">
                            <td style="width: 30%; text-align: center"><label for="partselect" id="partlabel"></label></td>
                            <td style="width:70%">
                                <select data-native-menu="false" multiple="multiple" name="partselect" id="partselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="barcodetext" id="barcodelabel"></label></td>
                            <!--tangyh 2017.03.22-->
                            <td style="width: 65%; text-align: right;">
                                <input id="barcodetext" name="barcodetext" type="text" placeholder="Bundle/Carton"/>
                            </td>
                            <td style="width: 5%; text-align: right;">
                                <input value='1' id="bundlebarcodeother" name="bundlebarcodeother" type="checkbox"/>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr id="contractnotr">
                            <td style="width: 30%; text-align: center"><label for="contractnoselect" id="contractnolabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <select name="contractnoselect" id="contractnoselect" data-native-menu="false">
                                </select>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-d">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="fromfactoryselect" id="fromfactoryselectlabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <select id="fromfactoryselect" name="fromfactoryselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 30%; text-align: center"></td>
                            <td style="width: 70%; text-align: right;">
                                <select id="fromprocessselect" name="fromprocessselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr id="Tr1">
                            <td style="width: 30%; text-align: center"></td>
                            <td style="width: 70%; text-align: right;">
                                <select id="fromproductionselect" name="fromproductionselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr id="Tr1">
                            <td style="width: 30%; text-align: center; display: none;"><label for="carnolabel" id="carnolabel"></label></td>
                            <td style="width: 70%; text-align: right;display: none;">
                                    <input type="text" name="carno" id="carno" placeholder="CAR NO#" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-e">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 3%"></td>
                            <td style="width: 94%; text-align: center">
                                <table style="width: 100%" id="messagetable" border="0" cellspacing="0" cellpadding="0">
                                    <thead>
                                        <tr>
                                            <th style="text-align: center" id="messageth"></th>
                                        </tr>
                                    </thead>
                                    <tbody id="messagetbody">
                                    </tbody>
                                </table>
                            </td>
                            <td style="width: 3%"></td>
                        </tr>
                    </table>
                </div>
            </div>
            <table style="width: 100%" id="barcodelisttable1">
                <tr>
                    <td style="width: 3%">
                    </td>
                    <td style="width: 94%; text-align: left">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 10%"><label id="barcodelistlabel"></label></td>
                                <td style="width: 10%"></td>
                                <td style="width: 15%"><label id="totalbundlelabel" style="color:Red; text-align:right"></label></td>
                                <td style="width: 5%"><div style="color:Red" id="totalbundlediv"></div></td>
                                <td style="width: 15%"><label id="totalgarmentpcslabel" style="color:Red; text-align:right"></label></td>
                                <td style="width: 5%"><div style="color:Red" id="totalgarmentpcs"></div></td>
                                <%-- <--tangyh 2017.03.28>--%>
                                <td style="width: 15%; text-align:right"><label for="totalpartdiv" id="totalpartlabel" style="color:Red"></label></td>
                                <td style="width: 5%; text-align:left"><div style="color:Red" id="totalpartdiv"></div></td>

                                <td style="width: 15%"><label id="totalpcslabel" style="color:Red; text-align:right"></label></td>
                                <td style="width: 5%"><div style="color:Red" id="totalpcsdiv"></div></td>

                            </tr>
                        </table>
                        <table class="stripe" id="barcodetabletitle" style="width: 100%" border="0" cellspacing="0" cellpadding="0">
                            <thead>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                        <table class="stripe" id="barcodelisttable" style="width: 100%" border="0" cellspacing="0" cellpadding="0">
                            <thead>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                        <br />
                        <div id="selector"></div>
                    </td>
                    <td style="width: 3%">
                    </td>
                </tr>
            </table>
            <div id="printtest" style="width:7.3cm;font-size:8px; margin-left: 0.2cm;">
                <a id='jumpprint' data-role='button' href='Printpage.aspx' data-rel='dialog'>hide</a>
            </div>
        </div>
        <div data-role="footer" data-position="fixed" data-fullscreen="true">
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
                <a id="transactiona" href="" data-ajax="false"></a>
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


