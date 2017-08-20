var totalConnections = 0;
$(document).ready(function () {
    tooltip($('.html-help'), '<div class="profile-help-data"><img src="/content/images/full-profile.jpg" style="max-width:300px;" /><h2>Minimum information required for the profile:</h2><ul><li>Add at least 3 skills to suggest you best possible connections.</li><li>Add at least 1 work history or education accomplishment to rank your profile high.</li><li>Add one or more accomplishment like project, publication, research.</li><li>Add profile and cover picture.</li><li>Connect with 10 people to boost your profile in searches and grow your network faster.</li></ul></div>');

    $('button[data-add-exp]').click(function () {
        getCareerHistory($(this));
    });

    $('button[data-add-edu]').click(function () {
        getEducationHistory($(this));
    });

    $('button[data-add-certi]').click(function () {
        getCertificationHistory($(this));
    });

    $('button[data-add-skill]').click(function () {
        getSkillHistory($(this), true);
    });
    $('button[data-add-award]').click(function () {
        getAwardHistory($(this));
    });

    $('button[data-add-publication]').click(function () {
        getPublications($(this));
    });

    $('button[data-add-compositions]').click(function () {
        getCompositions($(this));
    });

    $('button[data-add-research]').click(function () {
        getResearches($(this));
    });

    $('button[data-add-finding]').click(function () {
        getFindings($(this));
    });

    $('button[data-add-assignment]').click(function () {
        getProjectHistory($(this));
    });

    $('button[data-add-recommedation]').click(function () {
        getRecommedation($(this));
    });


    $('button[data-ask-recommedation]').click(function () {
        getAskRecommedation($(this));
    });

    $('button[data-skiller-edit]').click(function () {
        getAbout($(this));
    });

    $('.link-alike.change-cover').click(function () {
        ____changeCoverPicture($(this));
    });
    $('#h1Name').click(function () {
        $('#h1NameEdit').load('/account/updatename', function (response) {
            $('#h1Name').hide();
            $('#h1NameEdit').show();
        });
    });
    $('#h2Heading').click(function () {
        $('#h2HeadingEdit').load('/account/updateheading', function (response) {
            $('#h2Heading').hide();
            $('#h2HeadingEdit').show();
        });
    });



    if (totalConnections <= 50) {
        $('#contact-import-container').load('/account/contactimportwizard', function (res) {
            if (res != null && res != '') {
                $('#contact-import-container').show();
            }
            else {
                $('#contact-import-container').remove();
            }
        });
    }
    fetchIntelligence();

    $('#improve-profile').click(function () {
        ____attachGlobalWait();
        $.get('/account/improve', function (res) {
            $('<div id="share-cotainer" class="modal-content">').popUpWindow({
                action: "open",
                size: "medium"
            });
            ____removeGlobalWait();
            $('#share-cotainer').html(res);
        });
    });

    setPecentage();
});

function setPecentage(percentage) {
    if (percentage === undefined) {
        percentage = 0;
        $("#custom").percircle({
            percent: percentage,
            progressBarColor: percentage <= 30 ? "#dd5454" : percentage <= 50 ? "#dc5b00" : percentage <= 60 ? "#e88239" : "#39871f",
        });

        $.get('/account/profilerankdata', function (response) {
            percentage = response.Percentage;
            $('#custom').html('');
            $("#custom").percircle({
                percent: percentage,
                progressBarColor: percentage <= 30 ? "#dd5454" : percentage <= 50 ? "#dc5b00" : percentage <= 60 ? "#e88239" : "#39871f",
            });
            $('.profile-score .status').show();
            if (percentage < 100) {
                $('.profile-score .status button').show();
                $('.profile-score .status div').hide();
            } else {
                $('.profile-score .status button').hide();
                $('.profile-score .status div').show();
            }
        })
        //percentage = parseInt($("#custom").attr('data-per'));
    } else {
        $('#custom').html('');
        $("#custom").percircle({
            percent: percentage,
            progressBarColor: percentage <= 30 ? "#dd5454" : percentage <= 50 ? "#dc5b00" : percentage <= 60 ? "#e88239" : "#39871f",
        });
        if (percentage < 100) {
            $('.profile-score .status button').show();
            $('.profile-score .status div').hide();
        } else {
            $('.profile-score .status button').hide();
            $('.profile-score .status div').show();
        }
    }
}

function getAbout($obj) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/account/about',
        data: { hash: hash },
        type: 'GET',
        waitingSelector: $obj.find('.waiting-bg'),
        dataType: 'html',
        success: function (response) { $('#skiller-summary').prepend(response); $('.about-summary').hide(); $('.career-summary').addClass('impressed'); $('.bg-layer').addClass('impression-on'); }
    });
}

function getAskRecommedation($obj) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/reference/askRecommendation',
        data: { hash: hash },
        type: 'GET',
        dataType: 'html',
        waitingSelector: $obj.find('.waiting-bg'),
        success: function (response) {
            if ($('#frmRecomedation').length == 0) {
                $('#recommendation-list').prepend(response);
                preEditAction($('#recommendation-list').parents('.section-floated').first());
            }
        }
    });
}

function getCareerHistory($obj) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/careerhistory/manage',
        data: { q: hash },
        waitingSelector: $obj.find('.waiting-bg'),
        type: 'GET',
        dataType: 'html',
        success: function (response) {
            if ($('#frmCareerHistoryList').length == 0) {
                $('#career-history').prepend(response); $('#manage_Career_history_form').find('#OrganizationName').focus(); $('button[data-add-exp]').hide();
                preEditAction($('#career-history').parents('.section-floated').first());
            }
        }
    });
}

function getCertificationHistory($obj) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/careerhistory/managecertification',
        waitingSelector: $obj.find('.waiting-bg'),
        data: { q: hash },
        type: 'GET',
        dataType: 'html',
        success: function (response) {
            if ($('#frmCertification').length == 0) {
                $('#certification-history').prepend(response); $('button[data-add-certi]').hide();
                preEditAction($('#certification-history').parents('.section-floated').first());
            }
        }
    });
}

function getEducationHistory($obj) {

    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/careerhistory/manageeducation',
        data: { q: hash },
        type: 'GET',
        waitingSelector: $obj.find('.waiting-bg'),
        dataType: 'html',
        success: function (response) {
            if ($('#frmEducation').length == 0) {
                $('#education-history').prepend(response); $('button[data-add-edu]').hide();
                preEditAction($('#education-history').parents('.section-floated').first());
            }
        }
    });
}

function getSkillHistory($obj, addMode) {

    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/skillhistory/manage',
        waitingSelector: addMode == undefined ? $obj.parent('.buttons').find('.waiting-bg') : $obj.find('.waiting-bg'),
        data: { hash: hash },
        type: 'GET',
        dataType: 'html',
        success: function (response) {
            if ($('#frmSkill').length == 0) {
                $('button[data-add-skill]').hide(); $('#skill-history').prepend(response);
                preEditAction($('#skill-history').parents('.section-floated').first());
            }
        }
    });
}

function getAwardHistory($obj) {

    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/award/manage',
        data: { hash: hash },
        type: 'GET',
        dataType: 'html',
        waitingSelector: $obj.find('.waiting-bg'),
        success: function (response) {
            if ($('#frmAward').length == 0) {
                $('#award-history').prepend(response); $('button[data-add-award]').hide();
                preEditAction($('#award-history').parents('.section-floated').first());
            }
        }
    });
}

function getPublications($obj, $toAppend) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/award/managepublication',
        data: { hash: hash },
        type: 'GET',
        dataType: 'html',
        waitingSelector: $obj.find('.waiting-bg'),
        success: function (response) {
            if ($toAppend != undefined) {
                $toAppend.html(response);
            } else {
                if ($('#frmPublication').length == 0) {
                    $('#publication-history').prepend(response); $('.publication-list .plus-add-profile-item').hide();
                    preEditAction($('#publication-history').parents('.section-floated').first());
                }
            }
        }
    });
}

function getCompositions($obj, $toAppend) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/award/managecomposition',
        data: { hash: hash },
        type: 'GET',
        dataType: 'html',
        waitingSelector: $obj.find('.waiting-bg'),
        success: function (response) {
            if ($toAppend != undefined) {
                $toAppend.html(response);
                return;
            } else {
                if ($('#frmComposition').length == 0) {
                    $('#composition-history').prepend(response);
                    $('.composition-list .plus-add-profile-item').hide();
                    $('button[data-add-compositions]').hide();
                }
            }
            preEditAction($('#composition-history').parents('.section-floated').first());
        }
    });
}

function getResearches($obj, $toAppend) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/award/manageresearch',
        data: { hash: hash },
        type: 'GET',
        dataType: 'html',
        waitingSelector: $obj.find('.waiting-bg'),
        success: function (response) {
            if ($toAppend != undefined) {
                $toAppend.html(response);
            } else {
                if ($('#frmResearch').length == 0) {
                    $('#researches-list').prepend(response); $('.research-list .plus-add-profile-item').hide();
                    preEditAction($('#researches-list').parents('.section-floated').first());
                }
            }
        }
    });
}


function getFindings($obj, $toAppend) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/award/managefinding',
        waitingSelector: $obj.find('.waiting-bg'),
        data: { hash: hash },
        type: 'GET',
        dataType: 'html',
        success: function (response) {
            if ($toAppend != undefined) {
                $toAppend.html(response);
            } else {
                if ($('#frmFinding').length == 0) {
                    $('#findings-list').prepend(response); $('.finding-list .plus-add-profile-item').hide();
                    preEditAction($('#findings-list').parents('.section-floated').first());
                }
            }
        }
    });
}


function getProjectHistory($obj, $toAppend) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/award/ManageAssignment',
        data: { hash: hash },
        type: 'GET',
        waitingSelector: $obj.find('.waiting-bg'),
        dataType: 'html',
        success: function (response) {
            if ($toAppend != undefined) {
                $toAppend.html(response);
                return;
            } else {
                if ($('#frmProject').length == 0) {
                    $('#project-history').prepend(response); $('.project-list .plus-add-profile-item').hide();
                    preEditAction($('#project-history').parents('.section-floated').first());
                }
            }
        }
    });
}

function removeParticipant($obj) {
    var hash = $obj.attr('hash');
    action = '/Award/RemoveParticipant'
    $.ajax({
        url: action,
        type: 'POST',
        data: { hash: hash },
        dataType: 'JSON',
        success: function (response) {
            if ($obj.parent('.participant').parent('.participants').find('.participant').length == 1) {
                $obj.parent('.participant').parent('.participants').remove();
                return;
            }
            $obj.parent('.participant').remove();
        }
    });
    return false;
}


function removeSkill($obj) {
    var hash = $obj.attr('hash');
    action = '/Award/RemoveSkill'
    $.ajax({
        url: action,
        type: 'POST',
        data: { hash: hash },
        dataType: 'JSON',
        success: function (response) {
            if ($obj.parent('.skill-tag-editable').parent('.skills-list').find('.skill-tag-editable').length == 1) {
                $obj.parent('.skill-tag-editable').parent('.skills-list').remove();
                return;
            }
            $obj.parent('.skill-tag-editable').remove();
        }
    });
    return false;
}

function removeEducationSkill($obj) {
    var hash = $obj.attr('hash');
    action = '/CareerHistory/RemoveSkill'
    $.ajax({
        url: action,
        type: 'POST',
        data: { hash: hash },
        dataType: 'JSON',
        success: function (response) {
            $obj.parent('.skill-tag-editable').remove();
        }
    });
    return false;
}


function removeFormAndShowSource($toRemove, $toResume) {
    $toRemove.remove();
    $toResume.show();
    postEditAction();
    return false;
}

function preEditAction($obj) {
    if (____md == true) {
        $obj.addClass('impressed');
        $('.bg-layer').addClass('impression-on');
    }
}

function postEditAction() {
    if (____md == true) {
        $('.impressed').removeClass('impressed');
        $('.bg-layer').removeClass('impression-on');
    }
}

function showFindingMoreOption($obj) {
    var source = $obj.attr('data-source-button');
    $('#' + source).show();
    $obj.remove();
}

function showBasedOnChecked($obj) {
    if ($obj.is(':checked')) {
        var toShow = $obj.attr('data-val-check-for');
        $('#' + toShow).show();
    } else {
        var toShow = $obj.attr('data-val-check-for');
        $('#' + toShow).hide();
    }
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

function ____cancelRemoveCareerHistory($obj) {
    $obj.parents('.profile-item:first').find('.item-container').show();
    $obj.parents('.confirm:first').remove();
}

function ____removeAssignmentHistory($obj) {
    var hash = $obj.attr('data-hash');
    var dataMode = $obj.attr('data-mode');
    $.get('/award/removeConfirmation?mode=' + dataMode + '&q=' + hash, function (response) {
        $obj.parents('.profile-item:first').find('.item-container').hide();
        $obj.parents('.profile-item:first').append(response);
    });
}

function ____cancelRemoveAssignmentHistory($obj) {
    $obj.parents('.profile-item:first').find('.item-container').show();
    $obj.parents('form:first').remove();
}

function skillIntelligence($obj, isContactImport) {
    var hash = $obj.attr('hash');
    $.post('/Account/SkipIntelligence', { Hash: hash }, function () {
        if (isContactImport == true) {
            $('#contact-import-container').remove();
            return;
        } else {
            fetchIntelligence();
        }
    })
}

function ____skillHistoryAction($obj) {
    var hash = $obj.attr('data-hash');
    var action = $obj.attr('data-action');
    var type = $obj.attr('data-type');
    var toRender = 'skill-history';
    if (action == 'remove') {
        ____skillHistoryActionRemove($obj, hash, toRender);
    }
    if (action == 'revert') {
        ____skillHistoryActionRevert($obj, hash, toRender);
    }
}

function ____skillHistoryActionRemove($obj, hash, toRender) {
    $.ajax({
        url: '/SkillHistory/Remove?q=' + hash,
        success: function (res) {
            $obj.parents('.viduara-bCard:first').html(res);
            //$("#skill-history").processTemplateURL("/skillhistory/skill");
        },
        type: 'POST',
        waitingSelector: $obj.parent('.buttons').find('.waiting-bg'),

    });
}

function ____skillHistoryActionRevert($obj, hash, toRender) {
    $.ajax({
        url: '/SkillHistory/RevertRemove?q=' + hash,
        success: function (res) {
            $("#skill-history").processTemplateURL("/skillhistory/skill");
        },
        type: 'POST',
        waitingSelector: $obj.parent('.buttons').find('.waiting-bg'),
    });
}
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
            } else {
                $("#appriciation-list").processTemplateURL("/reference/appriciations");
            }
        }
    });
    return false;
}

function ____respondParticipantRequest($obj) {
    var action = $obj.attr('data-hash');
    var action = $obj.attr('data-action');
}

function fetchIntelligence() {
    $('#intelligence-container').load('/account/intelligence', function (res) {
        if (res != null && res != '') {
            $('#intelligence-wrapper').show();
        }
        else {
            $('#intelligence-wrapper').hide();
        }
    });
}