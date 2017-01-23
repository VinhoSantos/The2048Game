// Write your Javascript code.

$.ready(function () {
    
    $('button.btn-arrow').click(function () {
        NextMove($(this).data("move"));
    });

});

function NextMove(move) {

    var url = "/Game/NextMove";

    $.post(url, { move: move }, function (data) {
        console.log(move);
        $('#gamecube').html(data);
    }, "json")
    .fail(function (data) {
        })
    .always(function () {
    });;
}