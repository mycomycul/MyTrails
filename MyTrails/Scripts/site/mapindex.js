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
                if ($thisSection.data("featurenumbers").length === 0) GetTrailData($thisSection);
                //display data associated with the element
                displayFeatureOnMap($thisSection);
            }
        });
    });
});
