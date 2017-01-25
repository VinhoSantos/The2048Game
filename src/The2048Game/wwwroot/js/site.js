// Write your Javascript code.

$(function () {
    
    $('button.btn-arrow').click(function () {
        NextMove($(this).data("move"));
    });

    document.addEventListener("keydown", function(event) {
        //console.log(event.which);
        switch (event.which) {
            case 37: //left
                NextMove("left");
                break;
            case 38: //up
                NextMove("up");
                break;
            case 39: //right
                NextMove("right");
                break;
            case 40: //down
                NextMove("down");
                break;
        }
    });

});

function NextMove(move) {

    var url = "/Game/NextMove";
    var oParams = {};
    oParams["move"] = move;
    //var postData = "{ \"move\": \"" + move + "\"}";

    //$.ajax({
    //    method: "POST",
    //    url: url,
    //    data: JSON.stringify({
    //        move: move
    //    }),
    //    headers: {
    //        'Accept': 'application/json',
    //        'Content-Type': 'application/json'
    //    },
    //    contentType: 'application/json'
    //})
    $.get(url, oParams)
    .done(function (data) {
        console.log(move);
        $('#gamecube').html(data);
    })
    .fail(function (data) {
    })
    .always(function () {
    })

}