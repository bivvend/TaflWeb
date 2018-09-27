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
    function draw() {
        // Main entry point got the HTML5 chess board example
        var canvas = $("#boardImage");

        // Canvas supported?
        if (canvas.getContext) {
            ctx = canvas.getContext('2d');

            //// Calculdate the precise block size
            //BLOCK_SIZE = canvas.height / NUMBER_OF_ROWS;

            //// Draw the background
            //drawBoard();

            //defaultPositions();

            //// Draw pieces
            //pieces = new Image();
            //pieces.src = 'pieces.png';
            //pieces.onload = drawPieces;

            //canvas.addEventListener('click', board_click, false);
        }
        else {
            alert("Canvas not supported!");
        }
    }
    draw();
});