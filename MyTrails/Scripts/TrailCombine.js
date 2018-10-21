
//Setup click functionality for single select list and multi-select list
$(function () {

    //Single select functionality removes "selected-list-item" class from all single select elements and adds the class 
    //to the most recently clicked single select element
    //Single select items are trails from the database without geospatial data
    $('.single-select').on("click", function (event) {
        $('.single-select').removeClass("selected-list-item");
        $(this).addClass("selected-list-item");
    });

    //Multi select toggles the "selected-list-item" class on clicked elements, checks if coordinate data has been retrieved already
    //and either retrieves it or displays it if already retrieved
    $('.multi-select').each(function () {
        var $thisSection = $(this);
        $thisSection.click(function () {
            if ($thisSection.hasClass('selected-list-item')) {
                $thisSection.removeClass("selected-list-item");
                removeFeatureFromMap($thisSection);
            }
            else {
                $thisSection.addClass("selected-list-item");
                //If no data has been associated with the clicked element, retrieve data form the server
                if ($thisSection.data("featurenumbers").length === 0) getGeoJsonData($thisSection);

                //display data associated with the element
                displayFeatureOnMap($thisSection);
            }
        });
    });
});



//Get Trail Section Coordinates
function getGeoJsonData($geoDataNameElement) {

    var data = {
        trailSectionName: $geoDataNameElement.text()
    };
    $.ajax({
        url: "GetGeoJsonData",
        data: data,
        dataType: 'json',
        success: function (result) {
            //if geospatial data was found
            if (result.length > 0) {
                //for each set of coordinates with the same name in the geojson data
                for (var k = 0; k < result.length; k++) {
                    //Take received array of numeric coordinates and convert them to an array of google points
                    let latLng = CreateGoogleLatLngFromArray(result[k].Geometry);
                    //Create a marker or polyline from the google points
                    let mapFeature = createMapFeature(latLng, result[k].Notes);
                    //Save the new fature to an array of features that can be added and removed from the map
                    mapFeatures.push(mapFeature);
                    //Update the array of trail features associated with the the selected "GeoData" trail name

                    let featureNumbers = $geoDataNameElement.data("featurenumbers");
                    featureNumbers.push(mapFeatures.length - 1);
                    $geoDataNameElement.data("featurenumbers", featureNumbers);
                }
            }
            else {
                alert("No valid map data was available");
            }
        }
    });
    return true;
}

//Submit selected trail from db and selected trails from GEOJson Data to be combined and saved in the the database
function submitCombine() {
    //Get names of of geoData features selected to save to the selected trail in the Db
    var geoDataFeatureNames = $("li.multi-select.selected-list-item").map(function () {
        return $(this).text();
    }).get();
    //get name of the trail in the db to add spatial data to from geoJson data
    var dbTrailName = $('li.single-select.selected-list-item').text();
    var data = {
        trailFeatureNames: geoDataFeatureNames,
        trailNameInDb: dbTrailName
    };

    $.ajax({
        url: "CombineGeoJsonWithDb",
        data: data,
        method: "POST",
        success: function (result) {
            alert(result);
        }
    });
    return true;
}


///////////////////////
/* MAPPING FUNCTIONS */
///////////////////////


var map; //variable so that we can target the map when adding/removing markersa
var mapFeatures = [];
var infowindow;

//Google Maps API callback function
//Sets map values and dispalys the map
function myMap() {
    //Map target, zoom and center required            
    var mapCanvas = document.getElementById('map');
    var mapCenter = { lat: 47.7, lng: -123.63 };
    var mapZoom = 8;
    var mapType = 'terrain';
    var mapOptions = { center: mapCenter, zoom: mapZoom, mapTypeId: mapType };
    map = new google.maps.Map(mapCanvas, mapOptions);
    //infowindow = new google.maps.InfoWindow;
}

//Remove google feature associated with the received element
function removeFeatureFromMap(sectionToRemove) {
    let sections = sectionToRemove.data("featurenumbers");
    for (var i = 0; i < sections.length; i++) {
        mapFeatures[sections[i]].setMap(null);
    }
}

//Display feature associated with the the received element
function displayFeatureOnMap($thisSection) {
    var markers = $thisSection.data("featurenumbers");
    for (var i = 0; i < markers.length; i++) {
        mapFeatures[markers[i]].setMap(map);
    }
}

function createMapFeature(googlePoints, title) {
    var mapFeature;
    //If data for a line was received create a Google Polyline
    if (googlePoints.length > 1) {
        mapFeature = new google.maps.Polyline({
            path: googlePoints,
            strokeColor: "#00ff00",
            strokeOpacity: 0.8,
            strokeWeight: 8
        });
        //Attach click listener that zooms to the last point in a polyline
        mapFeature.addListener('click', function () {
            map.setZoom(10);
            var location = mapFeature.getPath();
            map.setCenter(location.b[0]);

        });
    }


    //Else if only one coordinate was received, create a Google marker
    else {
        mapFeature = new google.maps.Marker({
            position: googlePoints,
            icon: {
                scaledSize: new google.maps.Size(50, 50)
            },
            animation: google.maps.Animation.BOUNCE,
            title: title
        });
        //Attach click listener to Marker
        mapFeature.addListener('click', function () {
            map.setZoom(10);
            map.setCenter(mapFeature.getPosition());

        });
    }
    mapFeature.setMap(map);
    return mapFeature;
}




//Creates array of google points for every [lon,lat] pair in an array of pairs
function CreateGoogleLatLngFromArray(coordinates) {
    let googlePoints = [];
    for (let p = 0; p < coordinates.length; p++) {
        googlePoints.push(new google.maps.LatLng(coordinates[p][1], coordinates[p][0]));
    }
    return googlePoints;
}
