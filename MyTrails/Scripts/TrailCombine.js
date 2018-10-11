$(function () {
    //$('.trail').on("click", function (event) {
    //    $('.trail').removeClass("selected-list-item");
    //    $(this).addClass("selected-list-item");
    //});
    //$('.section').on("click", function (event) {
    //    $(this).toggleClass("selected-list-item");
    //});


    $('.section').each(function () {
        var $thisSection = $(this);
        var selected = false;
        $thisSection.click(function () {
            if (selected === false) {
                selected = true;
                $thisSection.addClass("selected-list-item");
                getGeoJsonData($thisSection.text());
            }
            else {
                selected = false;
                $thisSection.removeClass("selected-list-item");
            }
        });
    });
});
//Get Trail Section Coordinates
function getGeoJsonData(geoDataTrailName) {
    var data = {
        trailSectionName: geoDataTrailName
    };
    $.ajax({
        url: "GetGeoJsonData",
        data: data,
        success: function (result) {
            addDataToMap(result);  //Not implemented
        }
    });
    return true;
}
//Submit selected trail from geodata and selected trails from GEOJson Data to be combined and saved in the the database
function submitCombine() {
    var trailsections = $("section.section.selected-list-item").map(function () {
        return $(this).text();
    }).get();
    var trail = $('li.trail.selected-list-item').text();
    var data = {
        trailsections: trailsections,
        trail: trail
    };

    $.ajax({
        url: "CombineGeoJsonWithDb",
        data: data,
        success: function (result) {
            alert(result);
        }
    });
    return true;
};

//Display data to map
function addDataToMap(coordinates) {
    alert(coordinates);
}

var map = new ol.Map({
    target: 'map',
    layers: [
        new ol.layer.Tile({
            source: new ol.source.OSM()
        })
    ],
    view: new ol.View({
        center: ol.proj.fromLonLat([-123.5, 47.85]),
        zoom: 9
    })
});