function Loginout(fty, environment) {
    sessionStorage.clear();
    window.location.href = "Login.aspx?FTY=" + fty + "&svTYPE=" + environment;
}

//解释url，获取登录界面传递的参数
function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return unescape(r[2]); return null; //返回参数值
};

//获取条码的类型：扎码B、箱码C或者流水单号D
function GetBarcodeType(barcode) {
    if (barcode != null && barcode != '') {
        var index = barcode.indexOf("-");
        if (index > -1)//存在符号“-”，说明是主箱码箱码或者次箱码或者流水单号
        {
            //获取字符串第四个到第九个字符
            var keyword = barcode.substring(3, 6);
            //正则表达式判断字符串是否包含字母
            if (!$.isNumeric(keyword)) {
                //说明字符串中存在英文字母，则说明是箱码
                return "C";
            }
            else {
                //不存在则说明是流水单号
                return "D";
            }
        }
        else//不存在，说明是扎码B
        {
            //获取字符串第四个到第九个字符
            var keyword = barcode.substring(3, 6);
            //正则表达式判断字符串是否包含字母
            if (!$.isNumeric(keyword)) {
                //说明字符串中存在英文字母，则说明是箱码
                return "C";
            }
            else {
                return "B";
            }
        }
    }
    else
        return null;
}

function packagejump(fty, environment) {
    window.location.href = "Package.aspx?FTY=" + fty + "&svTYPE=" + environment;
} 
function matchingjump(fty, environment) {
    window.location.href = "Matching.aspx?FTY=" + fty + "&svTYPE=" + environment;
}
function transactionjump(fty, environment) {
    window.location.href = "Transaction.aspx?FTY=" + fty + "&svTYPE=" + environment;
}
function receivejump(fty, environment) {
    window.location.href = "Receive.aspx?FTY=" + fty + "&svTYPE=" + environment;
}
function reducejump(fty, environment) {
    window.location.href = "Reduce.aspx?FTY=" + fty + "&svTYPE=" + environment;
}
function employeeoutputjump(fty, environment) {
    window.location.href = "Employeeoutput.aspx?FTY=" + fty + "&svTYPE=" + environment;
} 
function bundleinputjump(fty, environment) {
    window.location.href = "Bundleinput.aspx?FTY=" + fty + "&svTYPE=" + environment;
}
function printbarcodejump(fty, environment) {
    window.location.href = "Printbarcode.aspx?FTY=" + fty + "&svTYPE=" + environment;
}
//add by lijer on 2018-05-18
function printsparebarcodejump(fty, environment) {
    window.location.href = "PrintSparebarcode.aspx?FTY=" + fty + "&svTYPE=" + environment;
}

function wipreportjump(fty, environment) {
    window.location.href = "Wipreport.aspx?FTY=" + fty + "&svTYPE=" + environment;
}
function outputreportjump(fty, environment) {
    window.location.href = "Outputreport.aspx?FTY=" + fty + "&svTYPE=" + environment;
}
function dctreportjump(fty, environment) {
    window.location.href = "dCTreport.aspx?FTY=" + fty + "&svTYPE=" + environment;
}
function loginhref(fty, environment) {
    window.location.href = "Login.aspx?FTY=" + fty + "&svTYPE=" + environment;
}
function docnoinquiryjump(fty, environment) {
    window.location.href = "Barcode_Inquiry.aspx?FTY=" + fty + "&svTYPE=" + environment;
}
function bundleorcartoninquiryjump(fty, environment) {
    window.location.href = "Barcode_Information_Inquiry.aspx?FTY=" + fty + "&svTYPE=" + environment;
}
//tangyh 2017.04.24
function WipAndOutputJump(fty, environment) {
    window.location.href = "WipAndOutputNEW.aspx?FTY=" + fty + "&svTYPE=" + environment;
}

function Accessmodulehide() {
    $('#packageli').hide();
    $('#matchingli').hide();
    $('#transactionli').hide();
    $('#receiveli').hide();
    $('#bundlereduceli').hide();
    $('#printli').hide();
    $('#employeeoutputli').hide();
    $('#externalreceiveli').hide();
    $('#reportli').hide();
    $('#wipreportli').hide();
    $('#outputreportli').hide();
    $('#dctreportli').hide();
    $('#bundleinputli').hide();
    $('#docnoli').hide();
    $('#bundleorcartonli').hide();
    $('#printsparebarcodeli').hide();//add by lijer on 2018-05-18
    $('#wipandoutputli').hide();
}

function Accessmoduleshow(data) {
    var result = eval(data);
    $(result).each(function (i) {
        if (result[i].MODULE == 'Package') $('#packageli').show();
        else if (result[i].MODULE == 'Matching') $('#matchingli').show();
        else if (result[i].MODULE == 'Transaction') $('#transactionli').show();
        else if (result[i].MODULE == 'Receive') $('#receiveli').show();
        else if (result[i].MODULE == 'Reduce') $('#bundlereduceli').show();
        else if (result[i].MODULE == 'EmployeeOutput') $('#employeeoutputli').show();
        else if (result[i].MODULE == 'Bundleinput') $('#externalreceiveli').show();
        else if (result[i].MODULE == 'WIPReport') {
            $('#reportli').show();
            $('#wipreportli').show();
        }
        else if (result[i].MODULE == 'OutputReport') {
            $('#reportli').show();
            $('#outputreportli').show();
        }
        else if (result[i].MODULE == 'dCTReport') {
            $('#reportli').show();
            $('#dctreportli').show();
        }
        else if (result[i].MODULE == 'DocnoInquiry') {
            $('#reportli').show();
            $('#docnoli').show();
        }
        else if (result[i].MODULE == 'BarcodeInquiry') {
            $('#reportli').show();
            $('#bundleorcartonli').show();
        }
        else if (result[i].MODULE == 'WipAndOutputNew') {
            $('#reportli').show();
            $('#wipandoutputli').show();
        }
        else if (result[i].MODULE == 'Printbarcode') { $('#printli').show(); $('#printsparebarcodeli').show(); }
    });
}

function Permissionmoduleshow(userprofile) {
    $.each(userprofile, function (i, o) {
        if (o.MODULE_CD == 'Package') $('#packageli').show();
        else if (o.MODULE_CD == 'Matching') $('#matchingli').show();
        else if (o.MODULE_CD == 'Transaction') $('#transactionli').show();
        else if (o.MODULE_CD == 'Receive') $('#receiveli').show();
        else if (o.MODULE_CD == 'Reduce') $('#bundlereduceli').show();
        else if (o.MODULE_CD == 'EmployeeOutput') $('#employeeoutputli').show();
        else if (o.MODULE_CD == 'Bundleinput') $('#externalreceiveli').show();
        else if (o.MODULE_CD == 'WIPReport') $('#wipreportli').show();
        else if (o.MODULE_CD == 'OutputReport') $('#outputreportli').show();
        else if (o.MODULE_CD == 'dCTReport') $('#dctreportli').show();
        else if (o.MODULE_CD == 'Printbarcode') { $('#printli').show(); $('#printsparebarcodeli').show(); }
        else if (o.MODULE_CD == 'DocnoInquiry') $('#docnoli').show();
        else if (o.MODULE_CD == 'BarcodeInquiry') $('#bundleorcartonli').show();
        else if (o.MODULE_CD == 'WipAndOutputNew') $('#wipandoutputli').show();
    });
}

function Commontranslation(temp) {
    var languageurl = "";
    if (temp == "zh") {
        languageurl = "Commontrans/Common-zh-CN.js";
    } else if (temp == "en") {
        languageurl = "Commontrans/Common-en-US.js";
    }
    $.ajax({
        type: "GET",
        url: languageurl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        async: false,
        cache: false,
        error: function () {
            alert("Sorry! Pls check your network and refresh your brower! Wrong Code::001");
        },
        success: function (data) {
            var result = eval(data);
            debugger;
            $('#packagea').html(result.packagea);
            $('#transactiona').html(result.transactiona);
            $('#receivea').html(result.receivea);
            $('#bundlereducea').html(result.bundlereducea);
            $('#printbarcodea').html(result.printbarcodea);
            $('#employeeoutputa').html(result.employeeoutputa);
            $('#externalreceivea').html(result.externalreceivea);
            $('#bundleinputa').html(result.bundleinputa);
            $('#reporta').html(result.reporta);
            $('#wipreporta').html(result.wipreporta);
            $('#outputreporta').html(result.outputreporta);
            $('#wipandoutputlia').html(result.wipandoutputlia);
            $('#printsparebarcodea').html(result.printsparebarcodea);
        }
    });
}

function bundlestatusalertmessage(result, globeltranslate) {
    if (result[0].ISSUBMIT == 'false') {
        return globeltranslate.statusmessage1;
    }
    if (result[0].ISMATCHING == 'false') {
        return globeltranslate.statusmessage15;
    }
    if (result[0].ISCARTON == 'false') {
        return globeltranslate.statusmessage16;
    }
    if (result[0].ISFULLBUNDLE == 'false') {
        return globeltranslate.statusmessage3;
    }
    if (result[0].ISFULLCARTON == 'false') {
        return globeltranslate.statusmessage14;
    }
    if (result[0].ISBUNDLEPROCESS == 'false') {
        return globeltranslate.statusmessage2;
    }
    if (result[0].ISCARTONPROCESS == 'false') {
        return globeltranslate.statusmessage13;
    }
    if (result[0].ISONERECEIVE == 'false') {
        return globeltranslate.statusmessage4;
    }
    if (result[0].ISONESEND == 'false') {
        return globeltranslate.statusmessage5;
    }
    if (result[0].ISONECUTGARMENTTYPE == 'false') {
        return globeltranslate.statusmessage7;
    }
    if (result[0].ISONESEWGARMENTTYPE == 'false') {
        return globeltranslate.statusmessage8;
    }
}

function tablelogic(arr, currentpagenum, deletebutton, defectbutton, bybundle) {
    var html = "";
    if (arr.length >= currentpagenum * 10) {
        for (var i = (currentpagenum - 1) * 10; i <= (currentpagenum - 1) * 10 + 9; i++) {
            html += tablehtml3(arr[i], deletebutton, defectbutton, bybundle);
        }
    }
    else {
        for (var i = (currentpagenum - 1) * 10; i < arr.length; i++) {
            html += tablehtml3(arr[i], deletebutton, defectbutton, bybundle);
        }
    }
    return html;
}

function tablelogic1(arr, deletebutton, defectbutton, bybundle) {
    var html = "";
    if (arr.length > 10) {
        for (var i = 0; i < 10; i++) {
            html += tablehtml3(arr[i], deletebutton, defectbutton, bybundle);
        }
    }
    else {
        for (var i = 0; i < arr.length; i++) {
            html += tablehtml3(arr[i], deletebutton, defectbutton, bybundle);
        }
    }
    return html;
}

function tablehtml3(temp, deletebutton, defectbutton, bybundle) {
    var html = "";
    var arrtemp = temp.split('|');
    if (bybundle == 'false') {
        html += "<tr id='" + arrtemp[0] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[1] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[2] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[3] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[4] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[5] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[6] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[7] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[8] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:3%;'>" + arrtemp[9] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[10] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[11] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[12] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[13] + "</td>";
        html += "<td><a data-role='button' onclick='defectbtn(this)' href='Reducedialog.aspx' data-rel='dialog'>" + defectbutton + "</a></td>";
        html += "<td><input type='button' onclick='deletebtn(this)' value='" + deletebutton + "' /></td></tr>";
    }
    else {
        html += "<tr id='" + arrtemp[0] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[1] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[2] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[3] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[4] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:7%;'>" + arrtemp[5] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:7%;'>" + arrtemp[6] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:7%;'>" + arrtemp[7] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:7%;'>" + arrtemp[8] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:7%;'>" + arrtemp[9] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:7%;'>" + arrtemp[10] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:7%;'>" + arrtemp[11] + "</td>";
        html += "<td><input type='button' onclick='deletebtn(this)' value='" + deletebutton + "' /></td></tr>";
    }
    return html;
}

function packagetablelogic1(arr, currentpagenum, dbtrans) {
    var html = "";
    if (arr.length >= currentpagenum * 10) {
        for (var i = (currentpagenum - 1) * 10; i <= (currentpagenum - 1) * 10 + 9; i++) {
            html += packagetablehtml(arr[i], dbtrans);
        }
    }
    else {
        for (var i = (currentpagenum - 1) * 10; i < arr.length; i++) {
            html += packagetablehtml(arr[i], dbtrans);
        }
    }
    return html;
}

function packagetablelogic2(arr, pagenumber, dbtrans) {
    var html = "";
    if ((pagenumber - 1) * 10 + 9 > (arr.length - 1)) {
        for (var i = (pagenumber - 1) * 10; i < arr.length; i++) {
            html += packagetablehtml(arr[i], dbtrans);
        }
    } else {
        for (var i = (pagenumber - 1) * 10; i <= (pagenumber - 1) * 10 + 9; i++) {
            html += packagetablehtml(arr[i], dbtrans);
        }
    }
    return html;
}

function packagetablelogic3(arr, dbtrans) {
    var html = "";
    if (arr.length > 10) {
        for (var i = 0; i < 10; i++) {
            html += packagetablehtml(arr[i], dbtrans);
        }
    }
    else {
        for (var i = 0; i < arr.length; i++) {
            html += packagetablehtml(arr[i], dbtrans);
        }
    }
    return html;
}

function packagetablehtml(temp, dbtrans) {
    var html;
    var arrtemp = temp.split('|');
    html += "<tr id='" + arrtemp[0] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[1] + "</td>";
    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[2] + "</td>";
    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[3] + "</td>";
    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[4] + "</td>";
    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[5] + "</td>";
    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[6] + "</td>";
    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[7] + "</td>";
    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[8] + "</td>";
    html += "<td style='vertical-align:middle; text-align:center; width:4%;'>" + arrtemp[9] + "</td>";
    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[10] + "</td>";
    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[11] + "</td>";
    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[12] + "</td>";
    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[13] + "</td>";
    html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></td></tr>";
    return html;
}



function receivechange(arr, currentpagenum, bybundle) {
    var html = "";
    if (arr.length >= currentpagenum * 10) {
        for (var i = (currentpagenum - 1) * 10; i <= (currentpagenum - 1) * 10 + 9; i++) {
            html += tablehtml(arr[i], bybundle);
        }
    }
    else {
        for (var i = (currentpagenum - 1) * 10; i < arr.length; i++) {
            html += tablehtml(arr[i], bybundle);
        }
    }
    return html;
}

function receive(arr, bybundle) {
    var html = "";
    if (arr.length > 10) {
        for (var i = 0; i < 10; i++) {
            html += tablehtml(arr[i], bybundle);
        }
    }
    else {
        for (var i = 0; i < arr.length; i++) {
            html += tablehtml(arr[i], bybundle);
        }
    }
    return html;
}

function tablehtml(temp, bybundle) {
    var html = "";
    var arrtemp = temp.split('|');
    if (bybundle == 'false') {
        html += "<tr id='" + arrtemp[0] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
        html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[14] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[1] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[2] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[3] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[4] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[5] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:9%;'>" + arrtemp[6] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[7] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[8] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[9] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[10] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[11] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[12] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[13] + "</td></tr>";
    }
    else {
        html += "<tr id='" + arrtemp[0] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')>";
        html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[13] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[1] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[2] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[3] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[4] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[5] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:6%;'>" + arrtemp[6] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:6%;'>" + arrtemp[7] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:6%;'>" + arrtemp[8] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:6%;'>" + arrtemp[9] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:6%;'>" + arrtemp[10] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[11] + "</td>";
        html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[12] + "</td></tr>";
    }
    return html;
}

function addDate(date, days) {
    var d = new Date(date);
    d.setDate(d.getDate() + days);
    return d;
    var month = d.getMonth() + 1;
    var day = d.getDate();
    if (month < 10) {
        month = "0" + month;
    }
    if (day < 10) {
        day = "0" + day;
    }
    var val = d.getFullYear() + "-" + month + "-" + day;
    return val;
}

function Redirect(fty, environment, page) {
    if (page == 'login') {
        sessionStorage.clear();
        window.location.href = "Login.aspx?FTY=" + fty + "&svTYPE=" + environment;
    }
    else if (page == 'bundleinput') {
        window.location.href = "Bundleinput.aspx?FTY=" + fty + "&svTYPE=" + environment;
    }
    else if (page == 'printfeizhi') {
        window.location.href = "Printbarcode.aspx?FTY=" + fty + "&svTYPE=" + environment;
    }
    else if (page == 'printsparecode') {
        window.location.href = "PrintSparebarcode.aspx?FTY=" + fty + "&svTYPE=" + environment;
    }
    else if (page == 'package') {
        window.location.href = "Package.aspx?FTY=" + fty + "&svTYPE=" + environment;
    }
    else if (page == 'transfer') {
        window.location.href = "Transaction.aspx?FTY=" + fty + "&svTYPE=" + environment;
    }
    else if (page == 'receive') {
        window.location.href = "Receive.aspx?FTY=" + fty + "&svTYPE=" + environment;
    }
    else if (page == 'reduce') {
        window.location.href = "Reduce.aspx?FTY=" + fty + "&svTYPE=" + environment;
    }
    else if (page == 'matching') {
        window.location.href = "Matching.aspx?FTY=" + fty + "&svTYPE=" + environment;
    }
    else if (page == 'employeeoutput') {
        window.location.href = "Employeeoutput.aspx?FTY=" + fty + "&svTYPE=" + environment;
    }
    else if (page == 'wip') {
        window.location.href = "Wipreport.aspx?FTY=" + fty + "&svTYPE=" + environment;
    }
    else if (page == 'transferoutput') {
        window.location.href = "Outputreport.aspx?FTY=" + fty + "&svTYPE=" + environment;
    }
    else if (page == 'dct') {
        window.location.href = "dCTreport.aspx?FTY=" + fty + "&svTYPE=" + environment;
    }
    else if (page == 'docno') {
        window.location.href = "Barcode_Inquiry.aspx?FTY=" + fty + "&svTYPE=" + environment;
    }
    else if (page == 'bundleorcarton') {
        window.location.href = "Barcode_Information_Inquiry.aspx?FTY=" + fty + "&svTYPE=" + environment;
    }
    else if (page == 'wipandoutput') {
        window.location.href = "WipAndOutputNEW.aspx?FTY=" + fty + "&svTYPE=" + environment;
    }
}

//tangyh 2017.04.27

//debugger;
function iprocess() {
    $.ajax({
        type: "POST",
        url: "Wipreport.aspx/Process",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "' }",
        success: function (data) {
            $('#processselect').append(data.d);
            $('#processselect').selectmenu('refresh');
        },
        error: function (err) {
            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "002</td></tr>");
            $('#messagetable').trigger('create');
            alert(globeltranslate.errormessage + "002");
        }
    });
};
//debugger;
function NotificationInit() {
    notification = $("#notification").kendoNotification({
        autoHideAfter: 3000,
        stacking: "up",
        templates: [{
            type: "error",
            template: $("#errorTemplate").html()
        }, {
            type: "upload-success",
            template: $("#successTemplate").html()
        }]

    }).data("kendoNotification");
};

//debugger;
function nextprocessselecttrans() {
    $('#nextprocessselect').empty();
    $('#nextprocessselect').append("<option value='all' selected='selected'>ALL(全部)</option>");
    $('#nextprocessselect').selectmenu('refresh');
}

//20170615-tangyh
function queryfactory(firstflag, fty3,type) {
    $.ajax({
        type: "POST",
        url: "Package.aspx/Queryfactory",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        data: "{ 'factory': '" + fty3 + "', 'svTYPE': '" + environment + "'}",
        success: function (data) {
            queryselectfactory(fty3,type);
            $('#queryfactory').append(data.d);
            if (type=='li')
                $('#queryfactory').selectmenu('refresh');
        },
        error: function (err) {
            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "010</td></tr>");
            $('#messagetable').trigger('create');
            alert(globeltranslate.errormessage + "010");
        }
    });
}

function ftyvalue(fty1) {
    var count = $("#queryfactory").get(0).options.length;
    for (var i = 0; i < count;) {
        if ($("#queryfactory").get(0).options[i].text == fty1) {
            $("#queryfactory")[0].selectedIndex = i;
            //$("#queryfactory").find("option:selected").html(fty);
            // $('#queryfactory-button').html('<span>' + fty1 + '</span>');
            fty = fty1;
            $('#messagetitle').html(sessionStorage.getItem("name") + " , " + fty + "/" + sessionStorage.getItem("process") + "/" + sessionStorage.getItem("productionline"));
            //$("#queryfactory option[value='" + fty + "']").attr("selected", true);
            break;
        }
        i = i + 1;
    }

    //$("#queryfactory option[value='" + fty + "']").attr("selected", true);
    //$("#queryfactory").find("option:selected").text(fty);
    //$("#queryfactory").attr("value", fty);
    // $('#queryfactory').selectmenu('refresh');
    //alert($("#queryfactory").get(0).selectedIndex);
    //return;

    //var count = $("#queryfactory").get(0).options.length;
    //for (var i = 0; i < count;) {
    //    if ($("#queryfactory").get(0).options[i].text == fty) {
    //        //$("#queryfactory").get(0).options[i].selected = true;
    //        $("#queryfactory")[0].selectedIndex = i;
    //        $("#queryfactory").attr("value", fty);//
    //        $("#queryfactory").attr("html", fty);
    //        $("#queryfactory option[value='selectdefault']").attr("selected", fty);
    //        //$("#queryfactory").text(fty);
    //        //$('#queryfactory').val(getUrlParam('FTY'));
    //        //$('#queryfactory').html(getUrlParam('FTY'));
    //        break;
    //    }
    //    i = i + 1;
    //}
}

function checkaccountid(firstflag, fty2,userbarcode,svtype) {
    //20170615-tangyh 
    var accountid_ok;
    if (firstflag == "T") {
        accountid_ok = "T";
        //$('#queryfactory-button').html('<span>' + fty2 + '</span>');
    }
    else {
        var ok = checkusernew(fty2, svtype, userbarcode);
        }

    $.ajax({
            type: "POST",
            url: "Package.aspx/Accessfunction",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            async: false,
            data: "{ 'factory': '" + fty2 + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + userbarcode + "' }",
            success: function (data) {
                var result = eval(data.d);
                if (result.length == 0) {
                    alert(globeltranslate.accounterror);
                    ftyvalue(getUrlParam('FTY'));
                    accountid_ok = "F";
                }
                else {
                    accountid_ok = "T";
                }
            },
            error: function (err) {
                alert(globeltranslate.errormessage + "008");
                accountid_ok = "F";
            }
        });
        if (accountid_ok == 'T')
            $('#messagetitle').html(sessionStorage.getItem("name") + " , " + sessionStorage.getItem("factory") + "/" + sessionStorage.getItem("process") + "/" + sessionStorage.getItem("productionline"));
    
        return accountid_ok;
}

//20170615-tangyh
function queryselectfactory(fty0,type) {
    $('#queryfactory').empty();
    $('#queryfactory').append("<option>" + fty0 + "</option>");
    $("#queryfactory option[value='" + fty0 + "']").attr("selected", true);
    if (type=='li')
       $('#queryfactory').selectmenu('refresh');
}

function checkusernew(fty, environment, userbarcode) {
    var ok = "F";
    $.ajax({
        type: "POST",
        url: "../Login.aspx/CheckUser",
        //url: "Package.aspx/Accessfunction",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: false,
        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'password': '" + userbarcode + "' }",
        //data: "{ 'factory': '" + fty2 + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + userbarcode + "' }",
        success: function (data) {
             
            if (data.d == "false") {
                ok = "F";
            } else if (data.d == "false1") {
                ok = "F";
            } else {
                var temp1 = data.d;
                var temp = new Array();
                temp = temp1.split(",");
                sessionStorage.setItem("employeeno", temp[0]);
                sessionStorage.setItem("name", temp[1]);
                sessionStorage.setItem("factory", temp[2]);
                sessionStorage.setItem("process", temp[3]);
                sessionStorage.setItem("productionline", temp[4]);
                sessionStorage.setItem("userbarcode", temp[6]);
                ok = "T";
            }
        },
        error: function (err) {
            ok = "F";
        }
    })
    return ok;
}