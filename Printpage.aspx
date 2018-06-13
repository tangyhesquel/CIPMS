<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Printpage.aspx.cs" Inherits="Printpage" %>

<!DOCTYPE html>
<html>
<head>
    <link href="css/JQM/jquery.mobile-1.4.5.min.css" rel="stylesheet" type="text/css" />
    <script src="js/JQM/jquery.js" type="text/javascript"></script>
    <script src="js/JQM/jquery.mobile-1.4.5.min.js" type="text/javascript"></script>
    <script src="js/jquery.chromatable.js" type="text/javascript"></script>
    <script src="js/jquery-barcode.js" type="text/javascript"></script>
    <script src="js/jquery.jqprint-0.3.js" type="text/javascript"></script>
    <script src="js/qrcode.js" type="text/javascript"></script>
</head>
<body>

<div data-role="page">
    
    <style type="text/css">
        .ui-dialog-contain 
        {
            width: 100%;
            /*max-width: 1024px;
            margin: 5% auto 15px auto;*/
            max-width: 700px;
            margin: 10% auto 15px auto;
            padding: 0;
            position: relative;
            top: -15px;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            $('#headertitle1').html(pdtitle);
            $('#printbutton1').val(pdbutton);

            //            var qrcode = new QRCode("test6", {
            //                width: 40,
            //                height: 40,
            //                text: cartonbarcode
            //            });

            //            var qrcode = new QRCode("test7", {
            //                width: 60,
            //                height: 60,
            //                text: cartonbarcode
            //            });

            //            var qrcode = new QRCode("test8", {
            //                width: 80,
            //                height: 80,
            //                text: cartonbarcode
            //            });

            //            var qrcode = new QRCode("test9", {
            //                width: 100,
            //                height: 100,
            //                text: cartonbarcode
            //            });

            //            var qrcode = new QRCode("div1", {
            //                width: 120,
            //                height: 120,
            //                text: cartonbarcode
            //            });

            //            var qrcode = new QRCode("div2", {
            //                width: 140,
            //                height: 140,
            //                text: cartonbarcode
            //            });

            if (cartonbarcode.length != 16) {
                $('#checkboxtable').hide();
            }

            $('#printdiv').append(printhtml);

            if (cartonbarcode.length == 16 || cartonbarcode.length == 13 || cartonbarcode.length == 15) {
                var qrcode = new QRCode("barcodediv", {
                    width: 150,
                    height: 150,
                    text: cartonbarcode
                });
            } else {
                $('#barcodediv').barcode(cartonbarcode, "code128", { barWidth: printwidth, barHeight: printheight, showHRI: false });
            }

            //UPDATE 2015/11/11 13:46 BY JACOB
            //Use NAME to control instead of ID
            $("div[name='hide1']").each(function () {
                $(this).hide();
            });
            $("table[name='hide2']").each(function () {
                $(this).hide();
            });
            $("div[name='hide3']").each(function () {
                $(this).hide();
            });
            $("table[name='hide4']").each(function () {
                $(this).hide();
            });

            $('#summarybycolor').change(function () {
                if ($(this).is(':checked')) {
                    $("div[name='hide1']").each(function () {
                        $(this).show();
                    });
                    $("table[name='hide2']").each(function () {
                        $(this).show();
                    });
                } else {
                    $("div[name='hide1']").each(function () {
                        $(this).hide();
                    });
                    $("table[name='hide2']").each(function () {
                        $(this).hide();
                    });
                }
            });

            $('#summarybyjo').change(function () {
                if ($(this).is(':checked')) {
                    $("div[name='hide3']").each(function () {
                        $(this).show();
                    });
                    $("table[name='hide4']").each(function () {
                        $(this).show();
                    });
                } else {
                    $("div[name='hide3']").each(function () {
                        $(this).hide();
                    });
                    $("table[name='hide4']").each(function () {
                        $(this).hide();
                    });
                }
            });

            $('#printbutton1').click(function () {
                $('#printdiv').jqprint();
            });
        });
    </script>
    <div data-role="header" data-theme="b">
        <h1 id="headertitle1"></h1>
    </div>

    <div data-role="content">
        <div class='ui-grid-b'>
            <div class='ui-block-a'>
                <table id="checkboxtable" width="100%">
                    <tr>
                        <td style="width: 100%; text-align: right;">
                            <fieldset data-role="controlgroup" id="checkboxfieldset">
                                <label for="summarybycolor" id="summarybycolorlabel">Summary By Color</label>
                                <input value='summarybycolor' id="summarybycolor" name="summarybycolor" type="checkbox"/>
                                <label for="summarybyjo" id="summarybyjolabel">Summary By JO</label>
                                <input value='summarybyjo' id="summarybyjo" name="summarybyjo" type="checkbox"/>
                            </fieldset>
                        </td>
                    </tr>
                </table>
            </div>
            <div class='ui-block-b'>
                <table style="width: 100%">
                    <tr>
                        <td>
                            <input type="button" id="printbutton1" value='hehe'/>
                        </td>
                    </tr>
                </table>
            </div>
            <div class='ui-block-c'>
            </div>
        </div>
        <%--<div id="printdiv" style="width:17.0cm;font-size:8px; margin-left: 0cm;">--%>
        <div id="printdiv" style="width:8.0cm;font-size:8px; margin-left: 0cm;">
        <%--<div style='height:5.08cm' id='test6'>
        </div>
        <div style='height:5.08cm' id='test7'>
        </div>
        <div style='height:5.08cm' id='test8'>
        </div>
        <div style='height:5.08cm' id='test9'>
        </div>
        <div style='height:5.08cm' id='div1'>
        </div>
        <div style='height:5.08cm' id='div2'>
        </div>--%>
        </div>
    </div>

    <div data-role="footer">
    </div>
</div> 

</body>
</html>
