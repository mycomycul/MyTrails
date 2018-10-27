///////////////////////
/* MAPPING FUNCTIONS */
///////////////////////


var map; //variable so that we can target the map when adding/removing markersa
var mapFeatures = [];
var infowindow;

(function (d) {
    var file = 'https://maps.googleapis.com/maps/api/js?key=' + googlekey + '&callback=myMap';
    var ref = d.getElementsByTagName('script')[0];
    var js = d.createElement('script');
    js.src = file;
    ref.parentNode.insertBefore(js, ref);
}(document));

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
