$(document).ready(function () {
    $('#includeStudy').change(function () {
        var includeStudy = $('#includeStudy').is(':checked');
        var includeAward = $('#includeAward').is(':checked');

        if (includeStudy == true) {
            $('label[for="includeStudy"]').addClass('active');
        } else {
            $('label[for="includeStudy"]').removeClass('active');
        }
        $("#career-line-basic").processTemplateURL("/profileitems/careerline/basic?q=" + profileHash + "&includeS=" + includeStudy.toString() + '&includeA=' + includeAward.toString());
    });

    $('#includeAward').change(function () {
        var includeStudy = $('#includeStudy').is(':checked');
        var includeAward = $('#includeAward').is(':checked');

        if (includeAward == true) {
            $('label[for="includeAward"]').addClass('active');
        } else {
            $('label[for="includeAward"]').removeClass('active');
        }
        $("#career-line-basic").processTemplateURL("/profileitems/careerline/basic?q=" + profileHash + "&includeS=" + includeStudy.toString() + '&includeA=' + includeAward.toString());
    });

    var includeStudy = $('#includeStudy').is(':checked');
    var includeAward = $('#includeAward').is(':checked');
    if (includeStudy == true) {
        $('label[for="includeStudy"]').addClass('active');
    } else {
        $('label[for="includeStudy"]').removeClass('active');
    }

    if (includeAward == true) {
        $('label[for="includeAward"]').addClass('active');
    } else {
        $('label[for="includeAward"]').removeClass('active');
    }

    $("#career-line-basic").processTemplateURL("/profileitems/careerline/basic?q=" + profileHash + "&includeS=" + includeStudy.toString() + '&includeA=' + includeAward.toString());
});



function makeSwitchableContents($obj) {
    var id = $obj.attr('id');
    $obj.find('.profile-item').each(function () {
        var height = $(this).height();
        if (height > 200) {
            $(this).addClass('shrinked');
            $(this).append('<div class="item-extender"><button onclick="____showMoreContents($(this))">Show more</button></div>');
        }
    })
}

function ____showMoreContents($obj) {
    $obj.parents('.profile-item:first').removeClass('shrinked');
    $obj.parent('.profile-item .item-extender').remove();
}

function ____swichAbout($obj) {
    //if ($('.less').is(":visible")) {
    //    $('.more').show();
    //    $('.less').hide();
    //    $('.profile-bio').addClass('impressed');
    //    $('.box .bg-layer').addClass('impression-on');
    //    $obj.html('Read less');
    //} else {
    //    if ($('.more').is(":visible")) {
    //        $('.more').hide();
    //        $('.less').show();
    //        $('.profile-bio').removeClass('impressed');
    //        $('.box .bg-layer').removeClass('impression-on');
    //        $obj.html('Read more');
    //    }
    //}

    $('<div id="share-cotainer" class="modal-content">').popUpWindow({
        action: "open",
        size: "medium"
    });
    $('#share-cotainer').html($('.less').attr('data-full'));
}