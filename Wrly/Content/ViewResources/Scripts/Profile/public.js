
$(document).ready(function () {
    $("#nav li a").click(function (e) {
        var $parent = $(this).parent('li');
        if ($parent.attr('data-val-action') == 'action') {
            e.preventDefault();
            $(document).off("scroll");

            $("#nav li a").removeClass('active');
            $(this).addClass('active');
            var target = '#' + $(this).attr('data-val-pointing');
            menu = target;
            $target = $(target);
            $('html, body').stop().animate({
                'scrollTop': $target.offset().top + 2
            }, 500, 'swing', function () {
                //      window.location.hash = target;
                $(document).on("scroll", onScroll);
            });

            positionNavBar();
        } else {
            if ($parent.hasClass('first')) {
                //Get the first visible
                var first = $('#nav li[data-val-action="action"].visible').first();
                var firstVisibleIndex = $('#nav li[data-val-action="action"]').index(first);
                if ((firstVisibleIndex + 1) > 2) {
                    for (var i = firstVisibleIndex; i < firstVisibleIndex + 2; i++) {
                        $('#nav li a').eq(i).parent('li').addClass('visible');
                    }
                } else {
                    for (var i = firstVisibleIndex; i <= firstVisibleIndex + 1; i++) {
                        $('#nav li a').eq(i).parent('li').addClass('visible');
                    }
                }

                $('#nav li[data-val-action="action"].visible').slice(5).removeClass('visible');
                $('#nav li[data-val-action="action"] a').removeClass('active');
                $('#nav li[data-val-action="action"].visible').first().find('a').addClass('active');
                positionNavBar();
            }
            if ($parent.hasClass('last')) {
                var total = $('#nav li[data-val-action="action"]').length;
                var last = $('#nav li[data-val-action="action"].visible').last();
                var lastVisibleIndex = $('#nav li[data-val-action="action"]').index(last);
                if ((lastVisibleIndex + 1) < total) {
                    for (var i = lastVisibleIndex; i < lastVisibleIndex + 2; i++) {
                        $('#nav li[data-val-action="action"]').eq(i).addClass('visible');
                    }
                } else {
                    for (var i = lastVisibleIndex; i <= lastVisibleIndex + 1; i++) {
                        $('#nav li[data-val-action="action"]').eq(i).addClass('visible');
                    }
                }

                last = $('#nav li[data-val-action="action"].visible').last();
                lastVisibleIndex = $('#nav li[data-val-action="action"]').index(last);
                $('#nav li[data-val-action="action"]').removeClass('visible');

                for (var i = lastVisibleIndex; i > lastVisibleIndex - 5; i--) {
                    $('#nav li[data-val-action="action"]').eq(i).addClass('visible');
                }
                $('#nav li[data-val-action="action"] a').removeClass('active');
                $('#nav li[data-val-action="action"].visible').last().find('a').addClass('active');
                positionNavBar();
            }
        }
    });
    positionNavBar();

    $(document).on("scroll", onScroll);
    $('button[data-add-appriciation]').click(function () {
        getAppriciation($(this));
    });

    $('button[data-add-recommedation]').click(function () {
        getRecommedation($(this));
    });

    $('button[data-add-appriciation]').click(function () {
        getAppriciation($(this));
    });

    $("#career-line-basic").setTemplateURL("/Templates/career-line-basic.txt");
    $("#common-skills").setTemplateURL("/Templates/public/common-skills.txt");
    $("#common-companies").setTemplateURL("/Templates/public/common-companies.txt");

    $("#common-skills").processTemplateURL("/profileitems/commonskills?q=" + profileHash);
    $("#common-companies").processTemplateURL("/profileitems/commoncompanies?q=" + profileHash);
});


function onScroll(event) {
    //var scrollPos = $(document).scrollTop();
    //$('#nav li a').each(function () {
    //    var currLink = $(this);
    //    var refElement = $('#' + currLink.attr("data-val-pointing"));
    //    if (refElement.length) {
    //        if (refElement.position().top <= scrollPos && refElement.position().top + refElement.height() > scrollPos) {
    //            $('#nav li a').removeClass("active");
    //            currLink.addClass("active");
    //        }
    //        else {
    //            currLink.removeClass("active");
    //        }
    //    }
    //    positionNavBar();
    //});
}


function getRecommedation($obj) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/reference/manageRecommendation',
        data: { hash: hash },
        type: 'GET',
        dataType: 'html',
        waitingSelector: $obj.find('.waiting-bg'),
        success: function (response) {
            if ($('#frmRecomedation').length == 0) {
                $('#recommendation-list').prepend(response);
            }
        }
    });
}

function getAppriciation($obj) {
    var hash = $obj.attr('data-hash');
    $.ajax({
        url: '/reference/manageAppriciation',
        data: { hash: hash },
        type: 'GET',
        dataType: 'html',
        success: function (response) { if ($('#frmAppriciation').length == 0) { $('#appriciation-list').prepend(response); } }
    });
}


function positionNavBar() {
    var $nav = $('#nav');
    var $active = $nav.find('a.active');
    var total = $('#nav li a').length;
    if (total > 5) {
        if ($active.length) {
            var indexOfActive = $('#nav li a').index($active)
            var lengthToLoopMinus = 0;
            var lengthToLoopPlus = 0;
            if ((total - indexOfActive) > 2) {
                $('.fact').removeClass('visible')
                lengthToLoopMinus = indexOfActive - 3;
                for (var i = indexOfActive; i > lengthToLoopMinus ; i--) {
                    $('#nav li a').eq(i).parent('li').addClass('visible');
                }

                if ((indexOfActive - 2) > 1) {
                    $('.fact.first').addClass('visible');
                } else {
                    $('.fact.first').removeClass('visible');
                }
            }

            if (indexOfActive + 2 < total) {
                lengthToLoopPlus = indexOfActive + 3;
                activateBefore = 0;
                if (lengthToLoopMinus < 0) {
                    lengthToLoopMinus *= -1;
                    lengthToLoopPlus = lengthToLoopPlus + lengthToLoopMinus;
                }
                if (lengthToLoopPlus + 1 > total) {
                    activateBefore = Math.abs(total - (lengthToLoopPlus + 1))
                    var first = $('#nav li[data-val-action="action"].visible').first();
                    var index = $('#nav li[data-val-action="action"]').index(first);
                    $('#nav li a').eq(index).parent('li').addClass('visible');
                }
                for (var i = indexOfActive; i < lengthToLoopPlus ; i++) {
                    $('#nav li a').eq(i).parent('li').addClass('visible');
                }

                if ((indexOfActive + 4) < total) {
                    $('.fact.last').addClass('visible');
                } else {
                    $('.fact.last').removeClass('visible');
                }
            }
        }
    } else {
        $('.fact[data-val-action="action"]').addClass('visible')
    }
}

function ____ExecuteSkillAction($obj) {
    if ($obj.find('.expertice-detail').html() == undefined) {
        var action = $obj.attr('action');
        var hash = $obj.attr('hash');
        $obj.append('<div class="expertice-detail"></div>');
        $obj.find('.expertice-detail').load('/skillhistory/detail/' + hash);
    }
}

function ____careerhistoryMoreOptions($object, evt) {
    evt.stopPropagation();
    var html = $object.find('.dropdown-menu').html();
    if (html.trim() == '') {
        $object.find('.dropdown-menu').html('<li>' + smallWaitningHtml + '</li>');
        $object.find('.dropdown-menu').addClass('open');
        var hash = $object.attr('data-hash');
        $.ajax({
            url: '/careerhistory/loadoptions?q=' + hash.toString(),
            type: 'GET',
            success: function (res) {
                $object.find('.dropdown-menu').html(res);
            }
        });
    } else {
        $object.find('.dropdown-menu').addClass('open');
    }
}

function ____assignmentMoreOptions($object, evt) {
    evt.stopPropagation();
    var html = $object.find('.dropdown-menu').html();
    if (html.trim() == '') {
        $object.find('.dropdown-menu').html('<li>' + smallWaitningHtml + '</li>');
        $object.find('.dropdown-menu').addClass('open');
        var hash = $object.attr('data-hash');
        $.ajax({
            url: '/award/loadoptions?hash=' + hash.toString(),
            type: 'GET',
            success: function (res) {
                $object.find('.dropdown-menu').html(res);
            }
        });
    } else {
        $object.find('.dropdown-menu').addClass('open');
    }
}

function ____careerHistoryAction($obj) {
    var hash = $obj.attr('data-hash');
    var action = $obj.attr('data-action');
    var type = $obj.attr('data-type');
    var toRender = '';
    if (type == 'profession') {
        toRender = 'career-history';
    }
    if (action == 'recommend') {
        ____careerHistoryActionRecommend($obj, hash, toRender);
    }
}

function ____awardAction($obj) {
    var hash = $obj.attr('data-hash');
    var action = $obj.attr('data-action');

    if (action == 'congratulate') {
        ____awardActionCongratulate($obj, hash);
    }
}

//REQUIRE:ASSIGNMENT CHANGES
function ____assignmentAction($obj) {
    var hash = $obj.attr('data-hash');
    var action = $obj.attr('data-action');
    var type = $obj.attr('data-type');
    var toRender = '';
    if (type == 'assignment') {
        toRender = 'project-history';
    }
    else if (type == 'award') {
        toRender = 'award-history';
    }
    else if (type == 'composition') {
        toRender = 'composition-history';
    }
    else if (type == 'publication') {
        toRender = 'publication-history';
    }
    else if (type == 'research') {
        toRender = 'research-history';
    }
    else if (type == 'finding') {
        toRender = 'finding-history';
    }
    else if (type == 'accomplishment') {
        toRender = 'accomplishment-history';
    }
    if (action == 'appriciate') {
        ____assignmentActionAppriciate($obj, hash, toRender);
    }
    else if (action == 'recommend') {
        ____assignmentActionRecommend($obj, hash, toRender);
    }
    else if (action == 'report') {
        ____assignmentActionReport($obj, hash, toRender);
    }
}


function ____assignmentActionAppriciate($obj, hash, toRender) {
    $.get('/reference/appriciateforaccomplishment?q=' + hash, function (res) { $('#' + toRender).prepend(res) });
}
function ____assignmentActionRecommend($obj, hash, toRender) {
    $.get('/reference/recommendforaccomplishment?q=' + hash, function (res) { $('#' + toRender).prepend(res) });
}
function ____assignmentActionReport($obj, hash, toRender) {
    $.get('/award/report?q=' + hash, function (res) { $('#' + toRender).prepend(res) });
}

function ____awardActionCongratulate($obj, hash) {
    $.post('/award/congratulate?q=' + hash, function (res) { $obj.parent('div').prepend(res); $obj.addClass('added'); });
}

function ____careerHistoryActionRecommend($obj, hash, toRender) {
    $.get('/reference/RecommendForRole?q=' + hash, function (res) { $('#' + toRender).prepend(res) });
}

function ____skillMoreOptions($object) {
    var html = $object.find('.dropdown-menu').html();
    if (html.trim() == '') {
        $object.find('.dropdown-menu').html('<li>' + smallWaitningHtml + '</li>');
        $object.find('.dropdown-menu').show();
        var hash = $object.attr('data-hash');
        $.ajax({
            url: '/skillhistory/loadoptions?q=' + hash.toString(),
            type: 'GET',
            success: function (res) {
                $object.find('.dropdown-menu').html(res);
            }
        });
    } else {
        $object.find('.dropdown-menu').show();
    }
}

function ____skillHistoryAction($obj) {
    var hash = $obj.attr('data-hash');
    var action = $obj.attr('data-action');
    var type = $obj.attr('data-type');
    var toRender = 'skill-history';
    if (action == 'recommend') {
        ____skillHistoryActionRecommend($obj, hash, toRender);
    }
    if (action == 'endorse') {
        ____skillHistoryActionEndorse($obj, hash, toRender);
    }
    if (action == 'remove-endorse') {
        ____skillHistoryActionRemoveEndorse($obj, hash, toRender);
    }
}

function ____skillHistoryActionRecommend($obj, hash, toRender) {
    $.ajax({
        url: '/reference/RecommendForSkill?q=' + hash,
        success: function (res) {
            $('#' + toRender).prepend(res)
        },
        type: 'GET',
        waitingSelector: $obj.parent('.buttons').find('.waiting-bg'),

    });
}

function ____skillHistoryActionEndorse($obj, hash, toRender) {
    $.ajax({
        url: '/skillhistory/Endorse?q=' + hash,
        success: function (res) {
            $("#skill-history").processTemplateURL("/profileitems/skill?q=" + profileHash);
        },
        type: 'POST',
        waitingSelector: $obj.parent('.buttons').find('.waiting-bg'),

    });
}

function ____skillHistoryActionRemoveEndorse($obj, hash, toRender) {
    $.ajax({
        url: '/skillhistory/RemoveEndorse?q=' + hash,
        success: function (res) {
            $("#skill-history").processTemplateURL("/profileitems/skill?q=" + profileHash);
        },
        type: 'POST',
        waitingSelector: $obj.parent('.buttons').find('.waiting-bg'),

    });
}

function ____changeProfilePicture($obj) {
    $.get('/account/changeProfilePicture', function (res) {
        $('#popup-cotainer').html(res);
        $('#popup-cotainer').popUpWindow({
            action: "open",
        });
    });
}