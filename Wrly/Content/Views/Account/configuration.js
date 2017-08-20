function ____cancelJobSearchLevel($obj) {
    $obj.parents('.config-form:first').remove();
    $('button[data-profile-change="true"]').show()
    $('#job-search-level-con span').show();
    $('#error-settings').hide();
}

function setReferForJobs($obj) {
    $.post('/account/SetAllowReference', { enabled: $obj.is(':checked') }, function (res) {
        ____executeJobSearch();
    });
}

$(document).ready(function () {
    if ('onhashchange' in window) {
        window.onhashchange = function () { ____executeAction(location.hash); }
    }
    else {
        $('a[data-action]').click(function () {
            ____executeAction('#' + $(this).attr('data-action'))
        });
    }

    var locationHash = location.hash;
    if (locationHash == '') {
        locationHash = '#general';
    }
    ____executeAction(locationHash);
});

function ____executeGeneral(hash) {
    if (hash == '#profile' || hash == '#general') {
        $('#content').load('/account/configuration/general');
        $('a[data-action="profile"]').addClass('active');
    } else {
        $('a[data-action="email"]').addClass('active');
        $('#content').load('/account/configuration/email');
    }
    
}

function ____executeNetwork() {
    $('#content').load('/account/configuration/network');
}

function ____executePrivacy() {
    $('#content').load('/account/configuration/privacy');
}

function ____executeProfileWidgets() {
    $('#content').load('/account/configuration/widgets');
}

function ____executeJobSearch() {
    $('#content').load('/account/configuration/job-search');
}

function ____executeAction(action) {
    $('a[data-action]').removeClass('active');
    switch (action) {
        case '#general':
        case '#profile':
        case '#email':
            ____executeGeneral(action);
            $('a[data-action="general"]').addClass('active');
            break;
        case '#network':
            ____executeNetwork();
            $('a[data-action="network"]').addClass('active');
            break;
        case '#privacy':
            ____executePrivacy();
            $('a[data-action="privacy"]').addClass('active');
            break;
        case '#profile-widgets':
            ____executeProfileWidgets();
            $('a[data-action="profile-widgets"]').addClass('active');
            break;
        case '#job-search':
            ____executeJobSearch();
            $('a[data-action="job-search"]').addClass('active');
            break;
        case '#company':
            ____executeCompanyProfile();
            $('a[data-action="company"]').addClass('active');
            break;
    }
}

function ____editBasic($obj) {
    $.get('/business/editbasic', function (res) { $('#editProfileBasic').show(); $('#editProfileBasic').html(res); $('#profileBasic').hide(); });
}
function ____editAddress($obj) {
    $.get('/business/editAddress', function (res) { $('#editprofileAddress').show(); $('#editprofileAddress').html(res); $('#profileAddress').hide(); });
}

function ____editPhone($obj) {
    $.get('/business/editPhone', function (res) { $('#editPhone').show(); $('#editPhone').html(res); $('#profilePhone').hide(); });
}

function setAllowApurtunities($obj) {
    $.post('/account/SetJobAppurtunities', { enabled: $obj.is(':checked') }, function (res) {
        ____executeJobSearch();

    });
}

function ____executeCompanyProfile(response) {
    $('#content').load('/account/configuration/company', function () {
        if (response != undefined) {
            $('#content').prepend(response);
        }
    });
}