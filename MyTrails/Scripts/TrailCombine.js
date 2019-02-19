
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
                if ($thisSection.data("featurenumbers").length === 0) GetJsonTrailData($thisSection);

                //display data associated with the element
                displayFeatureOnMap($thisSection);
            }
        });
    });
});





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
        url: "ManuallyAddJsonToDb",
        data: data,
        method: "POST",
        success: function (result) {
            if (result.status !== "ok") {
                alert(result.messages.join());
            }
            else {
                //TODO: Add code for removing elements from select lists after combining
                alert("Success");
            }
        }
    });
    return true;
}


