$(document).ready(function () {

    var boardData = null;
    var NUMER_OF_ROWS= 11;
    var NUMBER_OF_COLUMNS = 11;
    var blockSizeX = null;
    var blockSizeY = null;

    var sqImage = new Image(blockSizeX, blockSizeY);

    function loadImages() {
        sqImage.src = '/Content/tile1.bmp';
        sqImage.onload = draw();
    }

    function getString() {
        $.ajax({
            url: "/api/Game/GetString", success: function (result) {
                $("#currentValue").text(result);
            }
        });
    }
    function getBoardData() {
        $.ajax({
            url: "/api/Game/GetBoard", success: function (result) {
                $("#jsonBoard").text(result);
                boardData = JSON.parse(result);
                loadImages();
            }
        }); 
    }
    
    function draw() {
        var canvas = $("#boardImage")[0];
        // Canvas supported?
        if (canvas.getContext) {
            ctx = canvas.getContext('2d');



            if (NUMBER_OF_COLUMNS != 0 && NUMER_OF_ROWS != 0) {
                blockSizeX = Math.round(canvas.width / NUMBER_OF_COLUMNS) - 1;
                blockSizeY = Math.round(canvas.height / NUMER_OF_ROWS) - 1;
            }
            else {
                blockSizeX = 80;
                blockSizeY = 80;
            }


            var pat = ctx.createPattern(sqImage, "repeat");

            var i, j, value;
            for (i = 0; i < NUMBER_OF_COLUMNS; i++){
                for (j = 0; j < NUMER_OF_ROWS; j++){
                    value = boardData.SquareTypeArray[i][j];
                    switch (value){
                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                        default:
                            break;
                    }

                    ctx.drawImage(sqImage, i * blockSizeX, j * blockSizeY, blockSizeX, blockSizeY);
                    

                }
            }

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
    getString();
    getBoardData();
});