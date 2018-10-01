$(document).ready(function () {

    var boardData = null;
    var boardPatternData = null;
    var NUMER_OF_ROWS= 11;
    var NUMBER_OF_COLUMNS = 11;
    var blockSizeX = null;
    var blockSizeY = null;

    var drawList = [];

    function getString() {
        $.ajax({
            url: "/api/Game/GetString", success: function (result) {
                $("#currentValue").text(result);
            }
        });
    }
    function getBoardVisualData() {
        $.ajax({
            url: "/api/Game/GetBoardVisualPattern", success: function (result) {
                $("#visualPattern").text(result);
                boardPatternData = JSON.parse(result);
                getBoardData();
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
            drawList = [];
                        
            for (let i = 0; i < NUMBER_OF_COLUMNS; i++){   //Use let to define local variables.
                for (let j = 0; j < NUMER_OF_ROWS; j++){
                    var value = boardData.SquareTypeArray[i][j];
                    var type = boardPatternData[i][j];
                    var stringType = "1";  //Better to be explicit here 
                    if (type == 0) {
                        stringType = "1";
                    }
                    if (type == 1) {
                        stringType = "2";
                    }
                    if (type == 2) {
                        stringType = "3";
                    }
                    if (type == 3) {
                        stringType = "4";
                    }
                    var src;
                    switch (value) {
                        case 0:
                            src = '../Images/tile' + stringType + '.bmp';
                            break;
                        case 1:
                            src = '../Images/throne.bmp';
                            break;
                        case 2:
                            src = '../Images/attacktile' + stringType + '.bmp';
                            break;
                        case 3:
                            src = '../Images/throne.bmp';
                            break;
                        case 4:
                            src = '../Images/deftile' + stringType + '.bmp';
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
                        };

                    }

                    loadImage(src, i, j);
                   
                }
            } 

            //Need to wait for all images to be loaded before continuing to draw pieces

            //Once pieces are drawn  need to add selections.  Need to work out transparency

            //Finally draw borders etc...
            
        }
        else {
            alert("Canvas not supported!");
        }
    }
     
    getString();
    getBoardVisualData();  //Starts chain of AJAX requests towards rendering board.
});