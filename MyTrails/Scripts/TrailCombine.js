
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
        url: "GetJsonTrailData",
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
                    let mapFeature = createMapFeature(latLng, result[k].Note);
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


