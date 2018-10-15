$(function () {
    $('.trail').on("click", function (event) {
        $('.trail').removeClass("selected-list-item");
        $(this).addClass("selected-list-item");
    });
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
                getGeoJsonData($thisSection);
            }
            else {
                selected = false;
                $thisSection.removeClass("selected-list-item");
                removeSection($thisSection);
            }
        });
    });
});
//Get Trail Section Coordinates
function getGeoJsonData(geoDataTrail) {

    var data = {
        trailSectionName: geoDataTrail.text()
    };
    $.ajax({
        url: "GetGeoJsonData",
        data: data,
        dataType: 'json',
        success: function (result) {
            if (result.length > 0) {
                for (var k = 0; k < result.length; k++) {
                    let newFeature = CreateGoogleDataFromArray(result[k].Geometry[0]);
                    let mapFeature = addDataToMap(newFeature, result[k].Notes);
                    mapPoints.push(mapFeature);
                    let featureNumbers = geoDataTrail.data("marker");
                    featureNumbers.push(mapPoints.length - 1);
                    geoDataTrail.data("marker",featureNumbers);
                }
            }
            else {
                alert("No valid map data was available");
            }
        }
    });
    return true;
}
//Remove Trail Section
function removeSection(sectionToRemove) {
    let sections = sectionToRemove.data("marker");
    for (var i = 0; i < sections.length; i++) {
        mapPoints[sections[i]].setMap(null);
    }
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


///////////////////////
/* MAPPING FUNCTIONS */
///////////////////////


var map; //variable so that we can target the map when adding/removing markersa
var mapPoints = [];
var infowindow;

function addDataToMap(googleCoordinates, title) {

    //If data for a line was received create a Google Polyline
    var mapFeature;
    if (googleCoordinates.length > 1) {
        mapFeature = new google.maps.Polyline({
            path: googleCoordinates,
            strokeColor: "#00ff00",
            strokeOpacity: 0.8,
            strokeWeight: 8
        });
        mapFeature.addListener('click', function () {
            map.setZoom(10);
            var location = mapFeature.getPath();
            map.setCenter(location.b[0]);

        });
    }

    
    //Else if only one coordinate was received, create a Google marker
    else {
        mapFeature = new google.maps.Marker({
            position: googleCoordinates,
            icon: {
                scaledSize: new google.maps.Size(50, 50)
            },
            animation: google.maps.Animation.BOUNCE,
            title: title
        });
        mapFeature.addListener('click', function () {
            map.setZoom(10);
            map.setCenter(mapFeature.getPosition());

        });
    }    
    mapFeature.setMap(map);

    return mapFeature; 
}



function myMap() {
    //Map target, zoom and center required            
    var mapCanvas = document.getElementById('map');
    var mapCenter = { lat: 47.7, lng: -123.63 };
    var mapZoom = 8;
    var mapType = 'terrain';
    var mapOptions = { center: mapCenter, zoom: mapZoom, mapTypeId: mapType };
    map = new google.maps.Map(mapCanvas, mapOptions);
    infowindow = new google.maps.InfoWindow;


};

//Receives coordinate array (lon,lat), parses to google map point array and to be added to the map
//
function CreateGoogleDataFromArray(coordinates) {

    let newFeature = [];
    for (var p = 0; p < coordinates.length; p++) {
        //alert(coordinates[p][1].toString() + coordinates[p][0].toString());
        newFeature.push(new google.maps.LatLng(coordinates[p][1], coordinates[p][0]));
        }

    return newFeature;
}

function CreateMarker(map, mapCenter) {
    var marker = new google.maps.Marker({
        position: mapCenter,
        icon: {
            url: "mapfiles/smileyFace1.png",
            scaledSize: new google.maps.Size(50, 50),
        },
        animation: google.maps.Animation.BOUNCE,
        title: 'Click to Zoom'

    });
    marker.setMap(map);
}