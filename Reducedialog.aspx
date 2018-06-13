<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Reducedialog.aspx.cs" Inherits="test2" %>

<!DOCTYPE html>
<html>
<head>
    <link href="css/JQM/jquery.mobile-1.4.5.min.css" rel="stylesheet" type="text/css" />
    <script src="js/JQM/jquery.js" type="text/javascript"></script>
    <script src="js/JQM/jquery.mobile-1.4.5.min.js" type="text/javascript"></script>
    <script src="js/jquery.chromatable.js" type="text/javascript"></script>
</head>
<body>

<div data-role="page" id="asdf">
    
    <style type="text/css">
        .ui-dialog-contain 
        {
            width: 100%;
            max-width: 700px;
            margin: 10% auto 15px auto;
            padding: 0;
            position: relative;
            top: -15px;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            $('#barcodetext1').val(bundlebarcode);
            $('#parttext').val(bundlepart);
            $('#existingqty').val(bundleqty);
            defecttrans();

            //加载defect reason
            $.ajax({
                type: "POST",
                url: "Transaction.aspx/Defectreason",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "' }",
                success: function (data) {
                    $('#reasonselect').selectmenu('refresh');
                    $('#reasonselect').append(data.d);
                    $('#reasonselect').selectmenu('refresh');
                },
                error: function (err) {
                    alert(globeltranslate.errormessage + "512");
                }
            });

            //加载bundle defect list
            $.ajax({
                type: "POST",
                url: "Transaction.aspx/Defectlist",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'process':'" + sessionStorage.getItem("process") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'docno':'" + docno + "', 'bundlebarcode':'" + bundlebarcode + "', 'partcd':'" + bundlepart + "' }",
                success: function (data) {
                    $('#reducetable tbody').after(data.d);
                    $('#reducetable').trigger('create');
                },
                error: function (err) {
                    alert(globeltranslate.errormessage + "513");
                }
            });

            $('#qtytext').bind('input propertychange', function () {
                var defectqty = $('#qtytext').val();
                if (isNaN(defectqty)) {
                    $('#qtytext').val("");
                    return;
                }
                var qty = 0;
                $('#reducetable tr:not(:first)').each(function () {
                    qty += parseInt($(this).children('td').eq(3).text());
                });
                if ((qty + parseInt(defectqty)) > parseInt($('#existingqty').val())) {
                    $('#qtytext').val("");
                    return;
                }
            });

            //surebutton
            $('#surebutton').click(function () {
                var defectqty = $('#qtytext').val();
                var defectreason = $('#reasonselect').find("option:selected").val();
                if (defectqty != "" && defectreason != select) {
                    $.ajax({
                        type: "POST",
                        url: "Transaction.aspx/Defectconfirm",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        data: "{ 'process':'" + sessionStorage.getItem("process") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'docno':'" + docno + "', 'bundlebarcode':'" + bundlebarcode + "', 'bundlepart':'" + bundlepart + "', 'reason':'" + defectreason + "', 'userbarcode':'" + sessionStorage.getItem("userbarcode") + "', 'qty':'" + defectqty + "','bundleid': '" + bundleid + "' }",
                        success: function (data) {
                            var result = eval(data.d);
                            $('#reducetable tr:not(:first)').remove();
                            $('#reducetable tbody').append(result[0].HTML);
                            $('#reducetable').trigger('create');
                            var temp = "#" + bundleid + " td:nth-child(13)";
                            $(temp).html(result[0].QTY);
                            $('#qtytext').val("");
                        },
                        error: function (err) {
                            alert(globeltranslate.errormessage + "514");
                        }
                    });
                } else {
                    alert(globeltranslate.rejectdialogmessage1);
                }
            });

        });
    </script>
    <div data-role="header" data-theme="b">
        <h1 id="headertitle1"></h1>
    </div>

    <div data-role="content">
        <ul data-role="listview" data-inset="true" data-split-theme="e">
            <li style = "padding : 0px 0px">
                <div style="margin: 0px 0px; padding : 0px 0px">
                    <table width="100%">
                        <tr>
                            <td style="width: 25%; text-align: center"><label for="barcodetext1" id="barcodelabel1"></label></td>
                            <td style="width: 25%; text-align: right;"><input readonly="readonly" id="barcodetext1" type="text" name="barcodetext1" style="color:Red" /></td>
                            <td style="width: 25%; text-align: center"><label for="parttext" id="partlabel1"></label></td>
                            <td style="width: 25%; text-align: right;"><input readonly="readonly" id="parttext" type="text" name="parttext" style="color:Red" /></td>
                        </tr>
                    </table>
                </div>
            </li>
            <li style = "padding : 0px 0px">
                <div style="margin: 0px 0px; padding : 0px 0px">
                    <table width="100%">
                        <tr>
                            <td style="width: 25%; text-align: center"><label for="existingqty" id="existingqytlabel"></label></td>
                            <td style="width: 25%; text-align: right;"><input readonly="readonly" type="text" id="existingqty" name="existingqty" style="color:Red" /></td>
                            <td style="width: 25%; text-align: center"><label for="qtytext" id="qtylabel"></label></td>
                            <td style="width: 25%; text-align: right;"><input id="qtytext" type="text" name="qtytext" /></td>
                        </tr>
                    </table>
                </div>
            </li>
            <li style="padding: 0px 0px">
                <div style="margin: 0px 0px; padding : 0px 0px">
                    <table width="100%">
                        <tr>
                            <td style="width: 25%; text-align: center"><label for="reasonselect" id="reasonlabel"></label></td>
                            <td style="width: 25%; text-align: right;">
                                <select id="reasonselect" name="reasonselect" data-native-menu="false">
                                </select>
                            </td>
                            <td style="width: 25%; text-align: center"></td>
                            <td style="width: 25%"><input type="button" id="surebutton" value="Confirm" /></td>
                        </tr>
                    </table>
                </div>
            </li>
            <li style="padding: 0px 0px">
                <div style="margin: 0px 0px; padding : 0px 0px">
                    <td style="width: 100%">
                        <p id="reducetablelabel"></p>
                        <table class="stripe" id="reducetable" width="100%" border="0" cellspacing="0" cellpadding="0">
                            <thead> 
                            </thead> 
                            <tbody>
                            </tbody>
                        </table>
                    </td>
                </div>
            </li>
        </ul>
    </div>

    <div data-role="footer">
    </div>
</div> 

</body>
</html>
