// Write your Javascript code.

$(function () {
    
    //if ($('#gameover').val() === "True") {
    //    $('.game-over-container').show();
    //}
    //else {
    //    $('.game-over-container').hide();
    //}

    $('button.btn-arrow').click(function () {
        NextMove($(this).data("move", this));
    });

    //$('#btnNewGame').click(function () {
    //    NewGame();
    //});

    //$('#btnUndo').click(function () {
    //    Undo();
    //});

    document.addEventListener("keydown", function(event) {
        //console.log(event.which);
        switch (event.which) {
            case 37: //left
                NextMove("left", null);
                break;
            case 38: //up
                NextMove("up", null);
                break;
            case 39: //right
                NextMove("right", null);
                break;
            case 40: //down
                NextMove("down", null);
                break;
        }
    });

});

function NextMove(move, obj) {

    if (obj === null) {
        obj = $('button[data-move="' + move + '"]');
    }

    $(obj).addClass('btn-pressed');
    setTimeout(function () {
        $(obj).removeClass('btn-pressed');
    }, 200);

    var url = "/Game/NextMove";
    var oParams = {};
    oParams["move"] = move;

    $.get(url, oParams)
    .done(function (data) {
        console.log(move);
        $('#gamecube').html(data);

        //if ($('#gameover').val() === "True") {
        //    $('.game-over-container').show();
        //}
        //else {
        //    $('.game-over-container').hide();
        //}
    })
    .fail(function (data) {
    })
    .always(function () {
        //$(obj).removeClass('btn-pressed');
    })

}

function NewGame() {
    var url = "/Game/NewGame";

    $.get(url)
    .done(function (data) {
        console.log("new game");
        $('#gamecube').html(data);
        //$('.game-over-container').hide();
    })
    .fail(function (data) {
    })
    .always(function () {
    })
}

function Undo() {
    var url = "/Game/Undo";

    $.get(url)
    .done(function (data) {
        console.log("undo");
        $('#gamecube').html(data);
        //$('.game-over-container').hide();;
    })
    .fail(function (data) {
    })
    .always(function () {
    })
}