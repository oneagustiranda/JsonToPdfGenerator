// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


// JS for json input text area with line number
// Add line numbers
function addLineNumber() {
    var textarea = document.getElementById("jsonInput");
    var lineNumbers = document.getElementById("lineNumbers");
    var lines = textarea.value.split("\n");

    var lineNumbersHtml = "";
    for (var i = 0; i < lines.length; i++) {
        lineNumbersHtml += (i + 1) + "<br>";
    }

    lineNumbers.innerHTML = lineNumbersHtml;

    // Set the height of line numbers follow textarea
    lineNumbers.style.height = textarea.clientHeight + "px";
}

addLineNumber();

// For line number sincron to text area when scrolling
document.getElementById("jsonInput").addEventListener("scroll", function () {
    var lineNumbers = document.getElementById("lineNumbers");
    lineNumbers.scrollTop = this.scrollTop;
});

// For line number sincron to text area when adding new value
document.getElementById("jsonInput").addEventListener("input", addLineNumber);

// End - JS for json input text area with line number



// Send value from view to controller
$(document).ready(function () {
    $('#generateButton').click(function () {
        $('#jsonForm').submit();
        // Get JSON data from textarea
        var jsonInput = $('#jsonInput').val();
        var fontSize = $('#fontSize').val();
        var fontName = $('#fontName').val();
        var pageSize = $('#pageSize').val();
        var pageOrientation = $('#pageOrientation').val();
        var leftMargin = $('#leftMargin').val();
        var rightMargin = $('#rightMargin').val();
        var topMargin = $('#topMargin').val();
        var bottomMargin = $('#bottomMargin').val();
        var headerText = $('#headerText').val();
        var headerText = $('#pdfPassword').val();

        // Send JSON data to server
        $.post('/Home/ConvertJsonToPdf', {
            jsonInput: jsonInput,
            fontSize: fontSize,
            fontName: fontName,
            pageSize: pageSize,
            pageOrientation: pageOrientation,
            leftMargin: leftMargin,
            rightMargin: rightMargin,
            topMargin: topMargin,
            bottomMargin: bottomMargin,
            headerText: headerText,
            pdfPassword: pdfPassword
        })
            .done(function (response) {
                if (response) {
                    $('#pdfPreview').attr('src', 'data:application/pdf;base64,' + response);
                } else {
                    alert('Failed to convert JSON to PDF. Make sure the JSON input is valid or there are other issues.');
                }
            })
            .fail(function (error) {
                alert('An error occurred while sending a request to the server: ' + error.statusText);
            });
    });
});
// End - Send value from view to controller

// Get pdf from pdf preview to download
document.getElementById("downloadButton").addEventListener("click", function () {
    var iframe = document.querySelector("iframe");
    var iframeSrc = iframe.src;

    var userFilename = prompt("Enter the filename (include file extension):", "json-to-pdf-report-generator.pdf");

    if (userFilename) {
        var downloadLink = document.createElement('a');
        downloadLink.href = iframeSrc;
        downloadLink.target = "_blank";
        downloadLink.download = userFilename;
        downloadLink.click();
    }
});
//End - Get pdf from pdf preview to download