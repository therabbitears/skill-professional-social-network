$(document).ready(function () {
    $('button[data-business-about-edit]').click(function () {
        getOrganizationAbout($(this));
    });

    $('button[data-add-baward]').click(function () {
        getBAwardHistory($(this));
    });

    $('button[data-add-services]').click(function () {
        getServices($(this));
    });

    $('button[data-add-products]').click(function () {
        getProducts($(this));
    });

    $('button[data-add-affiliation]').click(function () {
        getAffiliationHistory($(this));
    });
});


function ____changeProfilePicture($obj) {
    ____attachGlobalWait();
    $.get('/account/changeProfilePicture', function (res) {
        $('<div id="share-cotainer" class="modal-content">').popUpWindow({
            action: "open",
            size: "medium"
        });
        ____removeGlobalWait();
        $('#share-cotainer').html(res);
    });
}

function ____changeCoverPicture($obj) {
    ____attachGlobalWait();
    $.get('/account/changecoverpicture', function (res) {
        $('<div id="share-cotainer" class="modal-content">').popUpWindow({
            action: "open",
            size: "medium"
        });
        ____removeGlobalWait();
        $('#share-cotainer').html(res);
    });
}


function getBusinessRecommedation($obj) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/reference/manageBusinessRecommendation',
        data: { hash: hash },
        type: 'GET',
        dataType: 'html',
        waitingSelector: $obj.find('.waiting-bg'),
        success: function (response) { $('#recommendation-list').prepend(response); }
    });
}


function getAffiliationHistory($obj) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/careerhistory/manageAffiliation',
        data: { q: hash },
        type: 'GET',
        dataType: 'html',
        waitingSelector: $obj.find('.waiting-bg'),
        success: function (response) {
            if ($('#frmAffiliation').length == 0) {
                $('#affiliation-history').prepend(response);
            }
        }
    });
}

function removeFormAndShowSource($toRemove, $toResume) {
    $toRemove.remove();
    $toResume.show();
    return false;
}



function getBAwardHistory($obj) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/award/ManageBusinessAward',
        data: { hash: hash },
        type: 'GET',
        dataType: 'html',
        waitingSelector: $obj.find('.waiting-bg'),
        success: function (response) {
            if ($('#frmBusinessAward').length == 0) {
                $('#baward-history').prepend(response);
            }
        }
    });
}

function getBusinessAppriciation($obj) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/reference/manageAppriciation',
        data: { hash: hash },
        type: 'GET',
        dataType: 'html',
        success: function (response) { $('#appriciation-list').prepend(response); }
    });
}

function getOrganizationAbout($obj) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/business/about',
        data: { hash: hash },
        type: 'GET',
        dataType: 'html',
        waitingSelector: $obj.find('.waiting-bg'),
        success: function (response) { $('#skiller-summary').prepend(response); $('.about-summary').hide(); }
    });
}

function getServices($obj) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/award/manageservice',
        data: { hash: hash },
        type: 'GET',
        dataType: 'html',
        waitingSelector: $obj.find('.waiting-bg'),
        success: function (response) {
            if ($('#frmService').length == 0) {
                $('#services-list').prepend(response); $('.finding-list .plus-add-profile-item').hide();
            }
        }
    });
}

function getProducts($obj) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/award/manageproduct',
        data: { hash: hash },
        type: 'GET',
        dataType: 'html',
        waitingSelector: $obj.find('.waiting-bg'),
        success: function (response) {
            if ($('#frmProduct').length == 0) {
                $('#product-list').prepend(response); $('.finding-list .plus-add-profile-item').hide();
            }
        }
    });
}
function ____removeAssignmentHistory($obj) {
    var hash = $obj.attr('data-hash');
    var dataMode = $obj.attr('data-mode');
    $.get('/award/removeConfirmation?mode=' + dataMode + '&q=' + hash, function (response) {
        $obj.parents('.profile-item:first').find('.item-container').hide();
        $obj.parents('.profile-item:first').append(response);
    });
}

function ____removeCareerHistory($obj) {
    var hash = $obj.attr('data-hash');
    var dataMode = $obj.attr('data-mode');
    var subType = $obj.attr('data-type');
    $.get('/careerhistory/removeConfirmation?mode=' + dataMode + '&q=' + hash, function (response) {
        $obj.parents('.profile-item:first').find('.item-container').hide();
        $obj.parents('.profile-item:first').append(response);
    });
}


function ____secondaryAssociationAction($obj) {
    var form = $obj.parents('form:first');
    form.find('#actn').val($obj.val());
    form.submit();
    return true;
}

function ____referenceAction($obj, type) {
    var ser = $obj.serialize();
    var action = $obj.attr('action');
    $.ajax({
        url: action,
        type: 'POST',
        data: ser,
        dataType: 'html',
        success: function (response) {
            if (type == 'rec') {
                $("#recommendation-list").processTemplateURL("/reference/recommendation");
            }
            else {
                $("#testimonials-list").processTemplateURL("/reference/appriciations");
            }
        }
    });
    return false;
}

$(document).ready(function () {

    $('.link-alike.change-cover').click(function () {
        ____changeCoverPicture($(this));
    });
    $('#h1Name').click(function () {
        $('#h1NameEdit').load('/business/updatename', function (response) {
            $('#h1Name').hide();
            $('#h1NameEdit').show();
        });
    });

    $('#h2Heading').click(function () {
        $('#h2HeadingEdit').load('/business/updatecategory', function (response) {
            $('#h2Heading').hide();
            $('#h2HeadingEdit').show();
        });
    });

    $('#h2SubHeading').click(function () {
        $('#h2SubHeading').load('/business/updatestrength', function (response) {
            $('#h2SubHeading').hide();
            $('#h2SubHeadingEdit').show();
        });
    });

    $("#baward-history").setTemplateURL("/Templates/baward-list.txt");

    $("#affiliation-history").setTemplateURL("/Templates/affiliation-list.txt");
    $("#services-list").setTemplateURL("/Templates/services-list.txt");
    $("#product-list").setTemplateURL("/Templates/product-list.txt");
    $("#recommendation-list").setTemplateURL("/Templates/recommendation-list.txt");
    $("#testimonials-list").setTemplateURL("/Templates/testimonials-list.txt");
    $("#people-you-may-know").setTemplateURL("/Templates/suggestions.txt");


    $("#baward-history").processTemplateURL("/award/index");
    $("#people-you-may-know").processTemplateURL("/association/suggestions", null, { container: $("#people-you-may-know"), ___callback: ____removeIsLengthZero });
    $("#affiliation-history").processTemplateURL("/careerhistory/affiliation");
    $("#services-list").processTemplateURL("/award/services");
    $("#product-list").processTemplateURL("/award/products");
    $("#recommendation-list").processTemplateURL("/reference/recommendation");
    $("#testimonials-list").processTemplateURL("/reference/appriciations");
});

function ____removeIsLengthZero(data, $obj) {
    if (data != undefined) {
        var total = JSON.parse(data);

        if (total.length == 0) {
            $obj.remove();
        }
    }
}

function ____cancelRemoveAssignmentHistory($obj) {
    $obj.parents('.profile-item:first').find('.item-container').show();
    $obj.parents('form:first').remove();
}

function ____cancelRemoveCareerHistory($obj) {
    $obj.parents('.profile-item:first').find('.item-container').show();
    $obj.parents('.confirm:first').remove();
}