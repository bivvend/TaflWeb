$(document).ready(function () {

    var boardData = null;
    var NUMER_OF_ROWS= 11;
    var NUMBER_OF_COLUMNS = 11;
    var blockSizeX = null;
    var blockSizeY = null;

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
                draw();
            }
        }); 
    }

    function boardClick(x, y) {
        var i = 0;
    }
    
    function draw() {
        
        var canvas = $("#boardImage")[0];  //Need to get DOM object
        //Click needs to be registered to jQuery object not DOM object
        $("#boardImage").click(function (e) {
            mouseX = e.pageX - $("#boardImage").offset().left;
            mouseY = e.pageY - $("#boardImage").offset().top;
            boardClick(mouseX, mouseY);
        });
        // Canvas supported?
        if (canvas.getContext) {
            var ctx = canvas.getContext('2d');

            if (NUMBER_OF_COLUMNS != 0 && NUMER_OF_ROWS != 0) {
                blockSizeX = Math.round(canvas.width / NUMBER_OF_COLUMNS) - 1;
                blockSizeY = Math.round(canvas.height / NUMER_OF_ROWS) - 1;
            }
            else {
                blockSizeX = 80;
                blockSizeY = 80;
            }

            var numberOfElements = NUMBER_OF_COLUMNS * NUMER_OF_ROWS;

            var drawnList = [];
                        
            for (let i = 0; i < NUMBER_OF_COLUMNS; i++){
                for (let j = 0; j < NUMER_OF_ROWS; j++){
                    var value = boardData.SquareTypeArray[i][j];
                    var src;
                    switch (value) {
                        case 0:
                            src = '../Images/tile1.bmp';
                            break;
                        case 1:
                            src = '../Images/throne.bmp';
                            break;
                        case 2:
                            src = '../Images/attacktile1.bmp';
                            break;
                        case 3:
                            src = '../Images/throne.bmp';
                            break;
                        case 4:
                            src = '../Images/deftile1.bmp';
                            break;
                        default:
                            src = '../Images/tile1.bmp';
                            break
                    }                    

                    function loadImage(srcIn, x, y) {

                        var imageObj = new Image();
                        imageObj.src = srcIn;
                        imageObj.onload = function () {
                            ctx.drawImage(imageObj, x * blockSizeX, y * blockSizeY, blockSizeX, blockSizeY);
                            drawnList.add("1");
                        };

                    }

                    loadImage(src, i, j);
                   
                }
            }   
            
        }
        else {
            alert("Canvas not supported!");
        }
    }

     
    getString();
    getBoardData();
});