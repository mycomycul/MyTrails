$(function () {
    $('.trail').on("click", function (event) {
        $('.trail').removeClass("selected-list-item");
        $(this).addClass("selected-list-item");
    })
    $('.li').on("click", function (event) {
        $(this).toggleClass("selected-list-item");
    })
});

function submitCombine() {
    var trailsections = $("li.li.selected-list-item").map(function () {
        return $(this).text();
    }).get();
    var trail = $('li.trail.selected-list-item').text();
    var data = {
        trail: trail,
        trailsections: trailsections
    };


    $.post({
        url: "Combine",
        data: data,
        success: function (result) {
            alert(result);
        }
    });
    return true;
}