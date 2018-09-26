$(document).ready(function () {
    $.ajax({
        url: "/api/Game/GetString", success: function (result) {
            $("#currentValue").text(result);
        }
    });
});