$(document).ready(function () {

    var boardData = null;
    var NUMBER_OF_ROWS= 11;
    var NUMBER_OF_COLUMNS = 11;
    var blockSizeX = null;
    var blockSizeY = null;
    var ajaxRequest = null;

    var columnMappingArray = [ "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P" ];
    var rowMappingArray = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16" ];

    //Click needs to be registered to jQuery object not DOM object
    $("#boardImage").click(function (e) {
        mouseX = e.pageX - $("#boardImage").offset().left;
        mouseY = e.pageY - $("#boardImage").offset().top;
        boardClick(mouseX, mouseY);
    });

    $("#resetButton").click(function (e) {
        if (ajaxRequest != null) {
            ajaxRequest.abort();
        }        
        getBoardData();        
    });

    $("#checkBoxAttackerIsAI").change(function () {
        if ($("#checkBoxAttackerIsAI").is(':checked')) {
            boardData.attackerIsAI = true;
        }
        else {
            boardData.attackerIsAI = false;
        }

        if (boardData.currentTurnState == 0 && boardData.attackerIsAI) {
            runAI();
        }
    });


    $("#checkBoxDefenderIsAI").change(function () {
        if ($("#checkBoxDefenderIsAI").is(':checked')) {
            boardData.defenderIsAI = true;
        }
        else {
            boardData.defenderIsAI = false;
        }
        if (boardData.currentTurnState == 1 && boardData.defenderIsAI) {
            runAI();
        }
    });

    function runAI() {
        var objectToSend = JSON.stringify(boardData);
        ajaxRequest = $.ajax({
            type: "POST",
            url: "/api/Game/RunAI",
            data: { turnState: boardData.turnState, boardDataAsJson: objectToSend },
            dataType: 'json',
            success: function (response) {
                boardData = response;
                updateMoveList();
                if (boardData.requestReDraw) {
                    draw();
                }
                $("#responseLabel").text("Response:" + boardData.responseText);

                //Need to check if both are AI and give user option to override
                if (!boardData.attackerIsAI && !boardData.defenderIsAI) {
                    showTurnState();
                }
                else {
                    if ($("#checkBoxAttackerIsAI").is(':checked')) {
                        boardData.attackerIsAI = true;
                    }
                    else {
                        boardData.attackerIsAI = false;
                    }
                    if ($("#checkBoxDefenderIsAI").is(':checked')) {
                        boardData.defenderIsAI = true;
                    }
                    else {
                        boardData.defenderIsAI = false;
                    }
                }
                if (boardData.currentTurnState == 0 && boardData.attackerIsAI) {
                    runAI();
                }
                if (boardData.currentTurnState == 1 && boardData.defenderIsAI) {
                    runAI();
                }
                
            },
            failure: function (response) {
                console.log(response.responseText);
            },
            error: function (response) {
                console.log(response.responseText);
            }
        });
    }

    function getBoardData() {
        $.ajax({
            url: "/api/Game/GetBoard", success: function (result) {
                boardData = [];
                boardData = JSON.parse(result);
                showTurnState();
                $("#checkBoxAttackerIsAI").prop('checked', false);
                $("#checkBoxDefenderIsAI").prop('checked', false);
                boardData.attackerIsAI = false;
                boardData.defenderIsAI = false;
                NUMBER_OF_COLUMNS = boardData.board.SizeX;
                NUMBER_OF_ROWS = boardData.board.SizeY;
                updateMoveList();
                draw();
            }
        }); 
    }

    function getSquare(column, row) {
        var num = (NUMBER_OF_COLUMNS * row) + column;
        var theSquare = boardData.board.board[num];
        return theSquare;

    }

    function showTurnState() {
        var turnText = "Attacker's Turn"
        if (boardData.currentTurnState == 0) {
            turnText = "Attacker's Turn" 
        }
        else if (boardData.currentTurnState == 1) {
            turnText = "Defender's Turn"
        }
        else if (boardData.currentTurnState == 3) {
            turnText = "Defender Victory! - Press Reset to start again."
        }
        else if (boardData.currentTurnState == 4) {
            turnText = "Attacker Victory! - Press Reset to start again."
        }
        $("#turnStatus").text("Turn State: " + turnText);
        if (boardData.attackerIsAI) {
            $("#checkBoxAttackerIsAI").prop('checked', true);
        }
        else {
            $("#checkBoxAttackerIsAI").prop('checked', false);
        }
        if (boardData.defenderIsAI) {
            $("#checkBoxDefenderIsAI").prop('checked', true);
        }
        else {
            $("#checkBoxDefenderIsAI").prop('checked', false);
        }

    }

    function updateMoveList() {
        var listFound = $("#moveList").empty();
        var num = 1;
        if (boardData.moveHistory != undefined) {
            boardData.moveHistory.forEach(function (move) {                
                listFound.append($("<li>").text("Move " + num + ": " + JSON.stringify(move.stringRespresentation)));
                num++;
            });
        }
        else {
            boardData.moveList = [];
        }
    }

    function boardClick(x, y) {
        var i = 0;
        if (blockSizeX != undefined && blockSizeY != undefined && blockSizeX > 0 && blockSizeY > 0) {

            var columnToSend = Math.floor(x / blockSizeX);
            var rowToSend = Math.floor(y / blockSizeY);
            $("#clickColumn").text("Column: " + columnMappingArray[columnToSend]);
            $("#clickRow").text("Row: " + rowMappingArray[rowToSend]);

            if ($("#checkBoxAttackerIsAI").is(':checked')) {
                boardData.attackerIsAI = true;
            }
            else{
                boardData.attackerIsAI = false;
            }
            if ($("#checkBoxDefenderIsAI").is(':checked')) {
                boardData.defenderIsAI = true;
            }
            else {
                boardData.defenderIsAI = false;
            }

            //request response from server regarding result of this click
            var objectToSend = JSON.stringify(boardData);
            $.ajax({
                type: "POST",
                url: "/api/Game/SquareClick",
                data: { column: columnToSend, row: rowToSend, boardDataAsJson: objectToSend },
                dataType: 'json',
                success: function (response) {
                    boardData = response;
                    updateMoveList();
                    if (boardData.requestReDraw) {
                        draw();
                    }
                    $("#responseLabel").text("Response:" + boardData.responseText);
                    showTurnState();

                    if (boardData.currentTurnState == 0 && boardData.attackerIsAI) {
                        runAI();
                    }
                    if (boardData.currentTurnState == 1 && boardData.defenderIsAI) {
                        runAI();
                    }
                },
                failure: function (response) {
                    console.log(response.responseText);
                },
                error: function (response) {
                    console.log(response.responseText);
                }
            });  
        }

    }
    
    function draw() {
        
        var canvas = $("#boardImage")[0];  //Need to get DOM object
       
        // Canvas supported?
        if (canvas.getContext) {
            var ctx = canvas.getContext('2d');

            if (NUMBER_OF_COLUMNS != 0 && NUMBER_OF_ROWS != 0) {
                blockSizeX = Math.round(canvas.width / NUMBER_OF_COLUMNS) - 1;
                blockSizeY = Math.round(canvas.height / NUMBER_OF_ROWS) - 1;
            }
            else {
                blockSizeX = 80;
                blockSizeY = 80;
            }

            var numberOfElements = NUMBER_OF_COLUMNS * NUMBER_OF_ROWS;
            var tileDrawArray = [];
            var pieceDrawArray = [];

            var value = 0;
            var pieceValue = 0;
            var type = 0;
            var selected = false;
            var highlighted = false;
            var square;
                        
            for (let i = 0; i < NUMBER_OF_COLUMNS; i++){   //Use let to define local variables.
                for (let j = 0; j < NUMBER_OF_ROWS; j++){

                    square = getSquare(i, j);
                    boardData;
                    value = square.SquareType;
                    pieceValue = square.Occupation;
                    type = square.BareTileType;
                    selected = square.Selected;
                    highlighted = square.Highlighted;
                      
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
                            srcPiece = '../Images/blueopal.bmp';
                            break;
                        case 1:
                            srcPiece = '../Images/redopal.jpg';
                            break;
                        case 2:
                            srcPiece = '../Images/fireopal.jpg';
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


                    tileDrawArray.push([src, i, j, highlighted, selected]);   //Create array of images to draw                    
                  
                }
            } 

            //Need to wait for all images to be loaded before continuing to draw pieces
            //Create new list of Promises

            var promiseArray = [];

            tileDrawArray.forEach(function (item) {
                promiseArray.push(loadImage(item[0], item[1], item[2], false, item[3], item[4]));
            });

            pieceDrawArray.forEach(function (item) {
                promiseArray.push(loadImage(item[0], item[1], item[2], true, false, false));
            });

            function loadImage(url, x, y, isPiece, squareHighlighted, squareSelected) {
                return new Promise((fulfill, reject) => {
                    let imageObj = new Image();
                    imageObj.src = url;
                    imageObj.xValue = x; //Add extra properties to image object 
                    imageObj.yValue = y;
                    imageObj.isPiece = isPiece;
                    imageObj.selected = squareSelected;
                    imageObj.highlighted = squareHighlighted;
                    imageObj.onload = () => fulfill(imageObj);
                });
            }

            // get images in promise array
            Promise.all(promiseArray)
                .then((images) => {
                    // draw images to canvas
                    images.forEach(function (img) {
                        ctx.strokeStyle = 'black';
                        ctx.shadowColor = 'black';
                        ctx.shadowBlur = 0;
                        ctx.shadowOffsetX = 0;
                        ctx.shadowOffsetY = 0;
                        ctx.lineWidth = 1;
                        var angle = Math.random() * 2 * Math.PI;
                        if (!img.isPiece) {                            
                            ctx.drawImage(img, img.xValue * blockSizeX, img.yValue * blockSizeY, blockSizeX, blockSizeY);

                            if (img.highlighted || img.selected) {
                                //Draw a selection rectangle
                                ctx.beginPath();
                                ctx.strokeStyle = 'yellow';
                                ctx.shadowColor = 'yellow';
                                ctx.lineWidth = 5;
                                ctx.shadowBlur = 20;
                                ctx.shadowOffsetX = 0;
                                ctx.shadowOffsetY = 0;
                                ctx.rect(img.xValue * blockSizeX + 3, img.yValue * blockSizeY + 3, blockSizeX - 6, blockSizeY - 6);
                                ctx.closePath();
                                ctx.stroke();
                            }
                        }
                        else {     
                            //Draw outline with shadow
                            ctx.beginPath();
                            ctx.arc(img.xValue * blockSizeX + blockSizeX / 2, img.yValue * blockSizeY + blockSizeY / 2, 0.8 * (blockSizeX / 2), 0, 2 * Math.PI);
                            ctx.fillStyle = 'black';
                            ctx.shadowColor = 'black';
                            ctx.shadowBlur = 20;
                            ctx.shadowOffsetX = 5;
                            ctx.shadowOffsetY = 5;
                            ctx.fill();
                            //Clip region to draw piece
                            ctx.save();
                            ctx.beginPath();
                            ctx.arc(img.xValue * blockSizeX + blockSizeX / 2, img.yValue * blockSizeY + blockSizeY / 2, 0.8 * (blockSizeX / 2), 0, 2 * Math.PI);
                            ctx.closePath();
                            ctx.clip();
                            //Draw image onto clipped region
                            ctx.drawImage(img, img.xValue * blockSizeX, img.yValue * blockSizeY, blockSizeX, blockSizeY);
                            ctx.restore();
                            //draw reflection
                            ctx.globalAlpha = 0.3;
                            ctx.fillStyle = 'white';                            
                            ctx.strokeStyle = 'white';
                            ctx.shadowColor = 'white';
                            ctx.shadowBlur = 10;
                            ctx.shadowOffsetX = 0;
                            ctx.shadowOffsetY = 0;
                            ctx.beginPath();
                            ctx.arc(img.xValue * blockSizeX + blockSizeX / 3, img.yValue * blockSizeY + blockSizeY / 3, 0.1 * (blockSizeX / 2), 0, 2 * Math.PI);
                            ctx.closePath();
                            ctx.fill();
                            ctx.globalAlpha = 1.0;
                           


                        }

                        

                    });
                })
                .catch((e) => alert(e));
            
        }
        else {
            alert("Canvas not supported!");
        }
    }
     
    getBoardData();
});