var feedType = 0;
var ended = false;
function ____executeFeedReplyAction($obj,keepAdded) {
    var hash = $obj.attr('data-hash');
    var action = $obj.attr('data-action');
    var level = $obj.attr('data-level');
    var pageNumber = $obj.attr('data-val-next') == undefined ? 0 : parseInt($obj.attr('data-val-next'));
    var skip = $obj.attr('data-val-stock');
    if (action == 'like') {
        ____executeFeedReplyActionLike($obj, hash, action);
    }

    if (action == 'unlike') {
        ____executeFeedReplyActionLike($obj, hash, action);
    }

    if (action == 'reply') {
        ____executeFeedReplyActionReply($obj, hash);
    }

    if (action == 'remove') {
        ____executeFeedReplyActionRemove($obj, hash);
    }

    if (action == 'child-replies') {
        ____executeFeedReplyActionChilds($obj, hash, pageNumber);
    }
    if (action == 'more-comments') {
        ____loadMoreComments($obj, hash, level, pageNumber, skip, keepAdded);

    }
}

function ____executeFeedReplyActionRemove($obj, hash) {
    $.get('/press/ReactReply?q=' + hash + '&reactType=remove', function (res) {
        $obj.parents('.feed-comment:first').remove();
    });
}

function ____loadMoreComments($obj, hash, level, pageNumber, skip, keepAdded) {
    var container = $obj.attr('data-val-container');
    $obj.attr('disabled', 'disabled');
    $.get('/press/moreReplies?q=' + hash + '&reactType=more&level=' + level.toString() + '&container=' + container + '&pageNumber=' + pageNumber.toString() + '&stock=' + skip.toString(), function (res) {
        if ($obj.attr('data-level') == 'first') {
            $('div[data-container-id="' + container + '"] .feed-comments > #comment-render-area').append(res);
            $obj.attr('data-val-next', (pageNumber + 1).toString());
            var total = $obj.attr('data-val-total');
            var totalDisplaying = $('div[data-container-id="' + container + '"]').find('#comment-render-area > .feed-comment').length;
            if (total == totalDisplaying) {
                if (keepAdded == undefined || keepAdded==false) {
                    $obj.remove();
                }
                $('div[data-container-id="' + container + '"]').find('#comment-render-area > .comment-load-more > span').html('Displaying all');
            } else {
                $('div[data-container-id="' + container + '"]').find('#comment-render-area > .comment-load-more > span').html('Displaying ' + totalDisplaying.toString() + ' of ' + total.toString());
            }
        } else {
            $('div[data-container-id="' + container + '"] > #child-render-area').append(res);
        }
        $obj.removeAttr('disabled');
        tooltip($("a[data-hover-card='true']"));
    });

}

function ____executeFeedReplyActionLike($obj, hash, action) {
    $.get('/press/ReactReply?q=' + hash + '&reactType=' + action, function (res) {
        if (action == 'like') {
            $obj.addClass('added');
            $obj.attr('data-action', 'unlike');
        }
        if (action == 'unlike') {
            $obj.removeClass('added');
            $obj.attr('data-action', 'like');
        }
        $obj.find('span').html(res.ReferenceID);
    });
}

function ____executeFeedReplyActionReply($obj, hash) {
    var container = $obj.attr('data-val-container');
    $.get('/press/Reply?q=' + hash + '&reactType=reply&container=' + container, function (res) {
        $('div[data-val-feed-reply-form]').remove();
        $('div[data-container-id="' + container + '"] > #child-render-area').append(res);
        $('div[data-val-feed-reply-form]').find('input[type="text"]').focus();
    });
}

function ____executeFeedReplyActionChilds($obj, hash, pageNumber) {
    $obj.attr('disabled', 'disabled');
    var container = $obj.attr('data-val-container');
    $.get('/press/moreReplies?q=' + hash + '&reactType=more&container=' + container + '&pageNumber=' + pageNumber.toString(), function (res) {
        if ($obj.attr('data-level') == 'first') {
            $('div[data-container-id="' + container + '"] > #child-render-area').append(res);
            $obj.attr('data-val-next', (pageNumber + 1).toString());
        } else {
            $('div[data-container-id="' + container + '"] > #child-render-area').append(res);
        }
        if ($($.parseHTML(res)).filter(".feed-comment").length < 10) {
            $obj.remove();
        } else
            $obj.attr('data-val-next', (pageNumber + 1).toString());
        tooltip($("a[data-hover-card='true']"));
        $obj.removeAttr('disabled');
    });
}





function ____executeFeedAction($obj) {
    var hash = $obj.attr('data-hash');
    var action = $obj.attr('data-action');
    if (action == 'like') {
        ____executeFeedActionLike($obj, hash, action);
    }
    if (action == 'unlike') {
        ____executeFeedActionLike($obj, hash, action);
    }
    if (action == 'reshare') {
        ____executeFeedActionReshare($obj, hash);
    }
    if (action == 'report') {
        ____executeFeedActionReport($obj, hash);
    }
    if (action == 'save-fav') {
        ____executeFeedActionLike($obj, hash, 'save-fav');
    }
    if (action == 'remove') {
        ____executeFeedActionRemove($obj, hash, 'remove');
    }
    if (action == 'unremove') {
        ____executeFeedActionRemove($obj, hash, 'unremove');
    }
    if (action == 'remove-fav') {
        ____executeFeedActionLike($obj, hash, 'remove-fav');
    }
    if (action == 'Hide') {
        ____executeFeedActionRemove($obj, hash, 'hide');
    }
    if (action == 'unhide') {
        ____executeFeedActionRemove($obj, hash, 'unhide');
    }
    if (action == 'apply') {
        ____executeFeedActionApply($obj, hash, 'apply');
    }
    if (action == 'refer') {
        ____executeFeedActionRefer($obj, hash, 'refer');
    }
    if (action == 'refer-opportunity') {
        ____executeFeedActionReferOpportunity($obj, hash, 'referopportunity');
    }
    if (action == 'insights-shared-opp') {
        ____executeFeedActionInsights($obj, hash, 'insights-shared-opp');
    }
    if (action == 'insights-opp') {
        ____executeFeedActionInsights($obj, hash, 'insights-opp');
    }
   
}

function ____loadRefarals(type, $obj) {
    $('.connection-figures').removeClass('active');
    $obj.addClass('active');
    $('.insight-popup .details').html(null);
    var q = $obj.attr('data-hash');
    if (type == 'insights-my-referals') {
        $.get('/press/insightdetails?q=' + q + '&type=insights-my-referals&separateWindow=true', function (res) {
            $('<div id="share-cotainer" class="modal-content">').popUpWindow({
                action: "open",
                size: "medium"
            });
            $('#share-cotainer').html(res);
        });
        return;
    }
    if (type == 'responses') {
        $obj.attr('data-val-next', 0);
        ____executeFeedReplyAction($obj, true);
    } else {
        $.get("/press/insightdetails", { q: q, type: type }, function (res) { $('.insight-popup .details').html(res); });
    }
}

function ____executeFeedActionInsights($obj, hash, action) {
    $.get('/press/insights?q=' + hash + '&reactType=insights-my-referals', function (res) {
        $('<div id="share-cotainer" class="modal-content">').popUpWindow({
            action: "open",
            size: "medium"
        });
        $('#share-cotainer').html(res);
    });
}


function ____executeFeedActionRefer($obj, hash, action) {
    $.get('/press/refer?q=' + hash + '&reactType=refer', function (res) {
        $('<div id="share-cotainer" class="modal-content">').popUpWindow({
            action: "open",
            size: "medium"
        });
        $('#share-cotainer').html(res);
    });
}

function ____executeFeedActionReferOpportunity($obj, hash, action) {
    $.get('/press/referopportunity?q=' + hash + '&reactType=refer', function (res) {
        $('<div id="share-cotainer" class="modal-content">').popUpWindow({
            action: "open",
            size: "medium"
        });
        $('#share-cotainer').html(res);
    });
}
function ____executeFeedActionApply($obj, hash, action) {
    $.get('/press/apply?q=' + hash + '&reactType=apply', function (res) {
        $('<div id="share-cotainer" class="modal-content">').popUpWindow({
            action: "open",
            size: "medium"
        });
        $('#share-cotainer').html(res);
    });
}

var lastHtml = '';
function ____executeFeedActionRemove($obj, hash, action) {
    $.post('/press/React?q=' + hash + '&reactType=' + action, function (res) {

        if (action == 'remove') {
            var container = $obj.parent('li').parent('ul').attr('data-val-container');
            if (res.Type == 2) {
                lastHtml = $('div[data-container-id="' + container + '"]').html();
                $('div[data-container-id="' + container + '"]').html('<div class="undone-box"><p>' + res.Description + '<div><button onclick="____executeFeedAction($(this))" data-action="unremove" data-val-container="' + container + '" data-hash="' + hash + '"  class="btn btn-outlined btn-sm">Undo</button> </div></p></div>');
            }
        }
        if (action == 'hide') {
            var container = $obj.parent('li').parent('ul').attr('data-val-container');
            if (res.Type == 2) {
                lastHtml = $('div[data-container-id="' + container + '"]').html();
                $('div[data-container-id="' + container + '"]').html('<div class="undone-box"><p>' + res.Description + '<div><button onclick="____executeFeedAction($(this))" data-action="unhide" data-val-container="' + container + '" data-hash="' + hash + '"  class="btn btn-outlined btn-sm">Unhide</button> </div></p></div>');
            }
        }
        if (action == 'unhide') {
            var container = $obj.attr('data-val-container');
            $('div[data-container-id="' + container + '"]').html(lastHtml);
            tooltip($('div[data-container-id="' + container + '"]'));
            ____bindMenu();
        }
        if (action == 'unremove') {
            var container = $obj.attr('data-val-container');
            $('div[data-container-id="' + container + '"]').html(lastHtml);
            tooltip($('div[data-container-id="' + container + '"]'));
            ____bindMenu();
        }
    });
}

function ____executeFeedActionLike($obj, hash, action) {
    $.get('/press/React?q=' + hash + '&reactType=' + action, function (res) {
        if (action == 'like') {
            $obj.addClass('added');
            $obj.attr('data-action', 'unlike');
            if (res.ReferenceID > 1) {
                $obj.parents('.feed:first').find('.media-states').html('You and ' + (res.ReferenceID - 1).toString() + ' other liked');
            } else {
                $obj.parents('.feed:first').find('.media-states').html('You liked');
            }
        }
        if (action == 'unlike') {
            $obj.removeClass('added');
            $obj.attr('data-action', 'like');
            if (res.ReferenceID > 1) {
                $obj.parents('.feed:first').find('.media-states').html(res.ReferenceID.toString() + ' likes');
            }
            else {
                $obj.parents('.feed:first').find('.media-states').html('');
            }
        }
        if (action == 'save-fav') {
            $obj.attr('data-action', 'remove-fav');
            $obj.html('Remove from favorite')
        }
        if (action == 'remove-fav') {
            $obj.attr('data-action', 'save-fav');
            $obj.html('Save as favorite')
        }
    });
}

function ____executeFeedActionReshare($obj, hash) {
    $.get('/press/ReShare?q=' + hash + '&reactType=reshare', function (res) {
        $('<div id="share-cotainer" class="modal-content">').popUpWindow({
            action: "open",
            size: "medium"
        });
        $('#share-cotainer').html(res);
    });
}

function ____executeFeedActionReport($obj, hash) {
    var container = $obj.parent('li').parent('.dropdown-menu').attr('data-val-container');
    $.get('/press/report?q=' + hash + '&reactType=report', function (res) {
        $('div.feed[data-container-id="' + container + '"]').find('.media-body').prepend(res);
    });
}



function addReaction($obj) {
    var ser = $obj.serialize();
    var action = $obj.attr('action');
    if ($obj.find('.form-control').val().trim() != '') {
        $obj.find('.form-control').removeClass('input-validation-error');
        $.ajax({
            url: action,
            type: 'POST',
            data: ser,
            waitingSelector: $obj.find('input[type="text"].form-control:first'),
            dataType: 'html',
            success: function (response) {
                var container = $obj.attr('data-val-container');
                $('div[data-container-id="' + container + '"]').find('#comment-render-area').append(response);
                $('div[data-container-id="' + container + '"] .form-control').val('');
            }
        });
    } else {
        $obj.find('.form-control').addClass('input-validation-error');
    }
    return false;
}

function addReplyReaction($obj) {
    var ser = $obj.serialize();
    var action = $obj.attr('action');
    if ($obj.find('.form-control').val().trim() != '') {
        $obj.find('.form-control').removeClass('input-validation-error');
        $.ajax({
            url: action,
            type: 'POST',
            data: ser,
            waitingSelector: $obj.find('input[type="text"].form-control:first'),
            dataType: 'html',
            success: function (response) {
                var container = $obj.attr('data-val-container');
                $('div[data-container-id="' + container + '"] > #child-render-area').append(response);
                $('div[data-container-id="' + container + '"] .form-control').val('');
            }
        });
    } else {
        $obj.find('.form-control').addClass('input-validation-error');
    }
    return false;
}

function focusReaction($obj) {
    var container = $obj.attr('data-val-container');
    $('div[data-container-id="' + container + '"]').find('.form-control').focus();
}


function ____newsMoreOptions($object) {
    var html = $object.find('.dropdown-menu').html();
    if (html.trim() == '') {
        $object.find('.dropdown-menu').html('<li>' + waitningHtml + '</li>');
        $object.find('.dropdown-menu').show();
        var hash = $object.attr('data-hash');
        $.ajax({
            url: '/press/moreoptions?q=' + hash.toString(),
            type: 'GET',
            success: function (res) {
                $object.find('.dropdown-menu').html(res);
            }
        });
    } else {
        $object.find('.dropdown-menu').show();
    }
}


var lastFeed;
var lastLoad;
var pageNumber = 0;
$(document).ready(function () {
    lastLoad = $('div[data-val-time-loaded]').attr('data-val-time-loaded');
    var arr = [];
    var index = 0;
    $("div[data-val-id]").each(function () {
        arr[index] = $(this).attr('data-val-id');
        index++;
    })
    if (arr.length > 0) {
        trackInsights(arr, 'views');
    }
    ____bindMoreDetails();
});

function ____bindMoreDetails() {
    $('button[data-val-feed-action], a[data-val-feed-action]').click(function () {
        var id = $(this).attr('data-val-id');
        var type = $(this).attr('data-val-type');
        $('<div id="popup-cotainer">').popUpWindow({
            action: "open",
            size: 'cover'
        });
        $('#popup-cotainer').load('/press/load/' + id.toString() + '?type=' + type.toString());
    });
}

$(window).scroll(function () {
    lastFeed = $('.feeds .feed:last').before();
    if (lastFeed.length > 0) {
        var hT = lastFeed.offset().top,
            hH = lastFeed.outerHeight(),
            wH = $(window).height(),
            wS = $(this).scrollTop();
        if (wS > (hT + hH - wH)) {
            if (ended == false) {
                $('.load-more').show();
            }
        }
    }
});

function trackInsights(arr, type) {
    console.log(arr);
    $.ajax({
        url: '/press/track',
        data: { trackType: type, posts: arr, sentOn: new Date().getDate() },
        type: 'post',
        traditional: true,
        success: function (data) {
        }
    });
}


function loadFeeds($obj) {
    $obj.attr('disabled', 'disabled');
    pageNumber = pageNumber + 1;
    $.ajax({
        url: '/account/FeedContent?pageNumber=' + pageNumber.toString() + '&ticks=' + lastLoad.toString() + '&type=' + feedType,
        success: function (res) {
            var arr = [];
            var index = 0;
            $($.parseHTML(res)).filter("div[data-val-id]").each(function () {
                arr[index] = $(this).attr('data-val-id');
                index++;
            })
            if (arr.length > 0) {
                trackInsights(arr, 'views');
            }
            if (res.toString().trim() == '') {
                $('.load-more').hide();
                ended = true;
            } else {
                $('.load-more').removeAttr('disabled');
                $('.load-more').hide();
                $('.feeds').append(res);
                tooltip($("a[data-hover-card='true']"));
            }
            $obj.removeAttr('disabled');
            ____bindMenu();
            ____bindMoreDetails();
        },
        type: 'get',
        waitingSelector: $obj.find('.waiting-bg')
    });
}
