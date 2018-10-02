﻿$(document).ready(function () {

    var boardData = null;
    var boardPatternData = null;
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
            var tileDrawArray = [];
            var pieceDrawArray = [];
                        
            for (let i = 0; i < NUMBER_OF_COLUMNS; i++){   //Use let to define local variables.
                for (let j = 0; j < NUMER_OF_ROWS; j++){
                    var value = boardData.SquareTypeArray[i][j];
                    var pieceValue = boardData.OccupationArray[i][j];
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
                    var srcPiece;

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

                    switch (pieceValue) {
                        case 0:
                            srcPiece = '../Images/fireopal.jpg';
                            break;
                        case 1:
                            srcPiece = '../Images/blueopal.jpg';
                            break;
                        case 2:
                            srcPiece = '../Images/redopal.jpg';
                            break;
                        case 3:
                            srcsPiece = src;
                            break;
                        default:
                            srcPiece = src;
                            break
                    } 

                    if (pieceValue != 3) {
                        pieceDrawArray.push([srcPiece, i, j]);
                    }

                    tileDrawArray.push([src, i, j]);   //Create array of images to draw
                    
                  
                }
            } 

            //Need to wait for all images to be loaded before continuing to draw pieces
            //Create new list of Promises

            var promiseArray = [];

            tileDrawArray.forEach(function (item) {
                promiseArray.push(loadImage(item[0], item[1], item[2], false));
            });

            pieceDrawArray.forEach(function (item) {
                promiseArray.push(loadImage(item[0], item[1], item[2], true));
            });

            function loadImage(url, x, y, isPiece) {
                return new Promise((fulfill, reject) => {
                    let imageObj = new Image();
                    imageObj.src = url;
                    imageObj.xValue = x; //Add extra properties to image object
                    imageObj.yValue = y;
                    imageObj.isPiece = isPiece;
                    imageObj.onload = () => fulfill(imageObj);
                });
            }

            // get images
            Promise.all(promiseArray)
                .then((images) => {
                    // draw images to canvas
                    images.forEach(function (img) {
                        if (!img.isPiece) {
                            ctx.drawImage(img, img.xValue * blockSizeX, img.yValue * blockSizeY, blockSizeX, blockSizeY);
                        }
                        else {                     

                            

                            ctx.beginPath();
                            ctx.arc(img.xValue * blockSizeX + blockSizeX / 2, img.yValue * blockSizeY + blockSizeY / 2, 0.8 * (blockSizeX / 2), 0, 2 * Math.PI);
                            ctx.fillstyle = 'black';
                            ctx.shadowColor = 'black';
                            ctx.shadowBlur = 20;
                            ctx.shadowOffsetX = 5;
                            ctx.shadowOffsetY = 5;
                            ctx.fill();

                            ctx.save();
                            ctx.beginPath();
                            ctx.arc(img.xValue * blockSizeX + blockSizeX / 2, img.yValue * blockSizeY + blockSizeY / 2, 0.8 * (blockSizeX / 2), 0, 2 * Math.PI);
                            ctx.closePath();
                            ctx.clip();

                            ctx.drawImage(img, img.xValue * blockSizeX, img.yValue * blockSizeY, blockSizeX, blockSizeY);
                            ctx.restore();
                        }
                    });
                })
                .catch((e) => alert(e));
            
        }
        else {
            alert("Canvas not supported!");
        }
    }
     
    getString();
    getBoardVisualData();  //Starts chain of AJAX requests towards rendering board.
});