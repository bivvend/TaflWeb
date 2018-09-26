$(document).ready(function () {
    $.ajax({
        url: "/api/Game/GetString", success: function (result) {
            $("#currentValue").text(result);
        }
    });
    $.ajax({
        url: "/api/Game/GetBoard", success: function (result) {
            $("#jsonBoard").text(result);
        }
    });
});