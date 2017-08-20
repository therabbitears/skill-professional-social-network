var waitningHtml = '<div class="loading  text-center"><div class="loading-bar"></div><div class="loading-bar"></div><div class="loading-bar"></div><div class="loading-bar"></div></div>';
var smallWaitningHtml = '<div class="loading  sm text-center"><div class="loading-bar"></div><div class="loading-bar"></div><div class="loading-bar"></div><div class="loading-bar"></div></div>';
var ____nm = false;
var ____nn = false;
var ____nq = false;
var ____md = false;
$(document).ready(function () {
    var mobile = $('body').attr('data-views');
    if (mobile!=undefined) {
        ____md = true;
    }
    ____md = true;
    $('.statistic-launcher.requests .dropdown-toggle').click(function () { pullRequests($(this)) });
    $('.statistic-launcher.notifications .dropdown-toggle').click(function () { pullNotifications($(this)) });
    $('.statistic-launcher.messages .dropdown-toggle').click(function () { pullMessages($(this)) });
    $('.statistic-launcher.search').click(function () { showSearch($(this)); });
    $('button[data-val-quick-action]').click(function () {
        ____executeQuick($(this).attr('data-val-quick-action'), $(this).attr('data-val-entity'));
    })

    $('.dropdown-menu').click(function (e) { e.stopPropagation(); });
    $(document).click(function () { $('.dropdown-menu').removeClass('open'); });
    ____bindMenu();
    getStatistics();
    setInterval(getStatistics, 30000);

    var data = [];
    $(".big-search").autocomplete({
        minLength: 0,
        appendTo: ".search-content",
        search: function () {
            //if ($('.search-container .search-content').find('.loader').length == 0) {
            //    $('.search-container .search-content').prepend('<ul class="loader"><li class="initial-loading">' + smallWaitningHtml + '</li></ul>');
            //} else {
            //    $('.search-container .search-content').find('.loader').show();
            //}
        },
        source: function (request, response) {
            $("#____hdnEntityID").val(null);
            $("#____hdnUrl").val(null);
            $("#____hdnEntityType").val(null);
            isAutomComplete = true;
            $.ajax({
                url: "/Search/Execute",
                data: { keyword: $(".big-search").val() },
                dataType: "json",
                type: "POST",
                success: function (data) {
                    isAutomComplete = false;
                    if (data.length == 0) {
                        $(".big-search").removeClass('working');
                    }
                    response($.map
                    (data, function (obj) {
                        return {
                            label: obj.FormatedName,
                            entityID: obj.EntityID,
                            entityType: obj.EntityType,
                            url: obj.Url,
                            name: obj.Name,
                            profileName: obj.ProfileName,
                            category: obj.Category,
                            keyword: obj.Keyword,
                            value: obj.FormatedName != null ? obj.FormatedName : obj.Name != null ? obj.Name : obj.Keyword,
                            logoPath: obj.LogoIconPath,
                            profileHeading: obj.ProfileHeading == null ? '' : obj.ProfileHeading,
                            profilePath: obj.ProfileIconPath == null ? '/content/images/no-image.png' : obj.ProfileIconPath
                        };
                    }));
                },
                error: function () { isAutomComplete = false; }
            });
        },
        create: function () {
            $('.search-container .search-content').find('.loader').hide();
            $(this).data('ui-autocomplete')._renderItem = function (ul, item) {
                var stringToAppend = '';
                if (item.entityID != null && item.entityID > 0) {
                    if (item.entityType == 1) {
                        stringToAppend += '<div class="search-result-item" data-val-url="' + item.profileName + '" data-val-type="entity" data-val-entity="' + item.entityID + '" data-val-etype="' + item.entityType + '" data-val-keyword="' + item.label + '" >';
                        stringToAppend += '<div class="thumbnail-notification-con">';
                        stringToAppend += '<img src="' + item.profilePath + '" alt="user image - ' + item.profileName + '">';
                        stringToAppend += '</div>';
                        stringToAppend += '<div class="side-thumbnail"><h3>' + item.label + '</h3><span class="child-headings">' + item.profileHeading + '</span></div>';
                        stringToAppend += '</div>';
                    } else if (item.entityType == 2) {
                        stringToAppend += '<div class="search-result-item" data-val-url="' + item.url + '" data-val-type="entity" data-val-entity="' + item.entityID + '" data-val-etype="' + item.entityType + '" data-val-keyword="' + item.name + '" >';
                        stringToAppend += '<div class="thumbnail-notification-con">';
                        stringToAppend += '<img src="' + item.logoPath + '" alt="user image - ' + item.name + '">';
                        stringToAppend += '</div>';
                        stringToAppend += '<div class="side-thumbnail"><h3>' + item.name + '</h3><span class="child-headings">' + item.category + '</span></div>';
                        stringToAppend += '</div>';
                    }
                    else if (item.entityType == 4) {
                        stringToAppend += '<div class="search-result-item" data-val-url="' + item.url + '" data-val-type="entity" data-val-entity="' + item.entityID + '" data-val-etype="' + item.entityType + '" data-val-keyword="' + item.name + '" >';
                        stringToAppend += '<div class="thumbnail-notification-con">';
                        stringToAppend += '<img src="' + item.logoPath + '" alt="user image - ' + item.name + '">';
                        stringToAppend += '</div>';
                        stringToAppend += '<div class="side-thumbnail"><h3>' + item.name + '</h3><span class="child-headings">Group</span></div>';
                        stringToAppend += '</div>';
                    }
                }
                else {
                    stringToAppend += '<div class="search-result-item" data-val-type="keyword" data-val-keyword="' + item.keyword + '" >';
                    stringToAppend += '<div class="thumbnail-notification-con">';
                    stringToAppend += ' <img src="/Content/images/history-ico.png" alt="Content search - ' + item.keyword + '">';
                    stringToAppend += '</div>';
                    stringToAppend += '<div class="side-thumbnail"><h3>' + item.keyword + '</h3></div>';
                    stringToAppend += '</div>';
                }
                return $('<li>').append(stringToAppend).appendTo(ul);
            }
        },
        //open: function () {
        //    $(this).autocomplete("widget").appendTo(".search-content");
        //},
        select: function (event, ui) {
            if (ui.item.entityID > 0) {
                if (ui.item.entityType == 1) {
                    $("#____hdnEntityID").val(ui.item.entityID);
                    $("#____hdnUrl").val(ui.item.profileName);
                    this.value = ui.item.label;
                    $("#____hdnEntityType").val(1);
                }
                if (ui.item.entityType == 2 || ui.item.entityType == 4) {
                    $("#____hdnEntityID").val(ui.item.entityID);
                    $("#____hdnUrl").val(ui.item.url);
                    this.value = ui.item.label;
                    $("#____hdnEntityType").val(ui.item.entityType);
                }
            } else {
                $("#____hdnEntityID").val(null);
                $("#____hdnUrl").val(this.value);
                $("#____hdnEntityType").val(null);
                this.value = ui.item.label;
            }
            $('#____frmMainSearch').submit();
            return false;
        }
    }).focus(function () {
        $(this).autocomplete("search");
        //if ($('.search-container .search-content').find('.loader').length == 0) {
        //    $('.search-container .search-content').prepend('<ul class="loader"><li class="initial-loading">' + smallWaitningHtml + '</li></ul>');
        //} else {
        //    $('.search-container .search-content').find('.loader').show();
        //}
        //$('.search-container .search-content').prepend('<img src="https://www.hsi.com.hk/HSI-Net/pages/images/en/share/ajax-loader.gif" alt="searching..." />');
    });
    $('.help').tooltip();   
    

    $('.btn-hamburger').click(function () {
        $('body').css('overflow', 'hidden');
        if ($('.mobile-menu-cotainer .menu-wrapper').html() == '') {
            $('.mobile-menu-cotainer').show();
            $('.mobile-menu-cotainer .menu-wrapper').html('<div class="text-center pad-top-15">' + waitningHtml + '</div>');
            $('.mobile-menu-cotainer .menu-wrapper').load('/account/mobileusermenu');
        } else {
            $('.mobile-menu-cotainer').show();
        }
        ____hideSearch($('.statistic-launcher.search'));
    });

    $('#closeMenu').click(function () {
        $('.mobile-menu-cotainer').hide();
        $('body').css('overflow', 'auto');
    });
})



function quickHideSearch() {
    $('#main-search').hide();
    $('.statistic-launcher.search').show();
    $('.statistic-launcher.close-search').hide();
}
function showSearch($obj) {
    if (!$('#main-search').is(':visible')) {
        $('#main-search').show();
        $obj.find('i').addClass('icon-remove');
        $obj.find('i').removeClass('icon-search');
        $('#main-search input[type="text"]').focus();
    } else {
        ____hideSearch($obj)
    }
}

function ____hideSearch($obj) {
    $('#main-search').hide();
    $obj.find('i').addClass('icon-search');
    $obj.find('i').removeClass('icon-remove');
}

function hideSearch() {
    $('#main-search').hide();
    $('#profile-statistics').show();
}

$(document).ajaxSend(function (event, xhr, options) {
    if (options != undefined && options.waitingSelector != undefined) {
        $(options.waitingSelector).addClass('working');
    }
});

$(document).ajaxComplete(function (event, xhr, options) {
    if (options != undefined && options.waitingSelector != undefined) {
        $(options.waitingSelector).removeClass('working');
    }
});

function ____executeQuick(action, entity) {
    if (entity == 'conversations' && action == 'new' && $('.users').html() != undefined) {
        ____showNewConversationInput();
        $('.statistic-launcher.messages .dropdown-menu').removeClass('open');
        return;
    }

    if (entity == 'conversations' && action == 'new') {
        ____attachGlobalWait();
    }

    $.post('/ProfileItems/executeAction', { action: action, entity: entity }, function (res) {
        if (entity == 'conversations') {
            if (action == 'new') {
                $('.floated-container').find('.container').html(res);
                $('.statistic-launcher.messages .dropdown-menu').removeClass('open');
                fadeFloatedIn();
                ____removeGlobalWait();
            }
            if (action == 'acknowledged') {
                $('#messages .feed-comment').removeClass('pending');
                $('.statistic-launcher.messages').find('.count').hide();
            }
        } else if (entity == 'notifications') {
            if (action == 'acknowledged') {
                $('#notifications .feed-comment').removeClass('pending');
                $('.statistic-launcher.notifications').find('.count').hide();
            }
        } else if (entity == 'floated-container') {
            if (action == 'dismiss') {
                fadeFloatedOut();
            }
        }
    })
}
function pullMessages($obj) {
    if ($("#messages").html() == '') {
        $("#messages").html('<li class="initial-loading">' + smallWaitningHtml + '</li>')
    }
    $("#messages").setTemplateURL("/Templates/message-list.txt");
    $("#messages").processTemplateURL("/messages/index");
    $('#profile-statistics').find('.messages').find('.count').hide();
    //}
}


function pullNotifications($obj) {
    if ($("#notifications").html() == '') {
        $("#notifications").html('<li class="initial-loading">' + smallWaitningHtml + '</li>')
    }
    $("#notifications").setTemplateURL("/Templates/notifications.txt");
    $("#notifications").processTemplateURL("/notification/index");
    $('#profile-statistics').find('.notifications').find('.count').hide();
    //}
}


function pullRequests($obj) {
    if ($("#requests").html() == '') {
        $("#requests").html('<li class="initial-loading">' + smallWaitningHtml + '</li>')
    }
    $("#requests").setTemplateURL("/Templates/request-list.txt");
    $("#requests").processTemplateURL("/association/requests");
    $('#profile-statistics').find('.requests').find('.count').hide();
    //}
}


function showDescription(objectName) {
    document.getElementById(objectName).style.display = 'block';
}

function ____bindMenu($obj) {
    var $collection = null;
    if ($obj == undefined) {
        $collection = $('.dropdown-toggle')
    } else {
        $collection = $obj.find('.dropdown-toggle');
    }
    $collection.click(function (e) {
        e.stopPropagation();
        $('.dropdown-menu').removeClass('open'); $(this).parent().find('.dropdown-menu').addClass('open');
        if ($(this).attr('data-ajax') != undefined) {
            if ($(this).parent().find('.dropdown-menu').html().length == 0) {
                $(this).parent().find('.dropdown-menu').load($(this).attr('data-ajax-url'))
            }
        }
    });

}


function ___doApprove($obj) {
    var $form = $obj.parent('form');
    var act = $obj.val();
    ___do($form, act);
}

function ___doDecline($obj) {
    var $form = $obj.parent('form');
    var act = $obj.val();
    ___do($form, act);
}

function ___do($form, act) {
    var ser = $form.serialize();
    ser = ser + '&actn=' + act;
    var action = $form.attr('action');
    $.ajax({
        url: action,
        type: 'POST',
        data: ser,
        dataType: 'html',
        success: function (response) {
            $("#requests").processTemplateURL("/association/requests");
        }
    });
    return false;
}

function sendMessage($form) {
    var ser = $form.serialize();
    var action = $form.attr('action');
    $.ajax({
        url: action,
        type: 'POST',
        data: ser,
        dataType: 'json',
        success: function (response) {
            if (response.Type == 2) {
                fadeFloatedOut();
                showPopableSuccessAlert(response.Description);
            } else {
                showPopableErrorAlert(response.Description);
            }
        }
    });
    return false;
}

function showPopableErrorAlert(Description) {
    alert(Description);
    $('.alert-error-popabble').show();
    $('.alert-error-popabble').html(Description);
    $('.alert-error-popabble').fadeOut(8000)
}

function showPopableSuccessAlert(Description) {
    $('.alert-success-popabble').show();
    $('.alert-success-popabble').html(Description);
    $('.alert-success-popabble').fadeOut(8000)
}

function fadeFloatedIn() {
    $('.floated-container').fadeIn(200);
}
function fadeFloatedOut() {
    $('.floated-container').fadeOut(200, function () { $('.floated-container').find('.container').html(''); });
}
function ____erase($obj) {
    $obj.parent('div.alert').remove();
}

//function ____executeSearchAction($obj) {
//    if ($obj.attr('data-val-type') == 'entity') {
//        if ($obj.attr('data-val-etype')==1) {
//            location.href = '/' + $obj.attr('data-val-url');
//        } else
//        {
//            location.href = '/fou/' + $obj.attr('data-val-url');
//        }
//    } else {
//        alert('k');
//    }
//}


function newAssociation($obj, profileHash, hoverCard, group, isListing) {
    var ser = $obj.serialize();
    var action = $obj.attr('action');
    var permissionLevel = $obj.find('#permissionLevel').val();
    $.ajax({
        url: action,
        type: 'POST',
        data: ser,
        success: function (data, statusText, xhr) {
            if (group == true) {
                $("#connectionStatus").load('/association/groupactionable?q=' + profileHash, function () { ____bindMenu($("#connectionStatus")); });
            }
            else if (hoverCard == true) {
                $("#connectionStatus").load('/association/HoverCardactionable?q=' + profileHash);
            }
            else if (isListing) {
                if (permissionLevel.toString() == 'True') {
                    $obj.parents('.buttons-container').first().html('<button class="btn btn-primary btn-sm">Approval pending</button>');
                } else {
                    $obj.parents('.buttons-container').first().html('<button class="btn btn-primary btn-sm">Joined</button>');
                }
                $obj.remove();
            }
            else {
                $("#connectionStatus").load('/association/actionable?q=' + profileHash, function () { ____bindMenu($("#connectionStatus")); });
            }

        }
    });
    return false;
}



function getStatistics() {
    $.get('/entity-statistics', function (res) {
        if (res.PendingMessages > 0) {

            var $objectMes = $('#profile-statistics').find('.messages').find('.count');
            if (res.PendingMessages) {
                $objectMes.show();
                $objectMes.attr('title', res.PendingMessages.toString() + ' new conversation updates.')
                $objectMes.html(res.PendingMessages);
            } else { $objectMes.hide(); }
        }

        var $objectNot = $('#profile-statistics').find('.notifications').find('.count');

        if (res.PendingNotifications > 0) {

            $objectNot.show();
            $objectNot.attr('title', res.PendingNotifications.toString() + ' new notifications.')
            $objectNot.html(res.PendingNotifications);
        } else { $objectNot.hide(); }

        var $objectReq = $('#profile-statistics').find('.requests').find('.count');
        if (res.PendingRequests > 0) {
            $objectReq.show();
            $objectReq.attr('title', res.PendingRequests.toString() + ' new connection requests.')
            $objectReq.html(res.PendingRequests);
        }
        else { $objectReq.hide(); }
        $('.count').tooltip();
    });
}



function tooltip($selector, html) {
    $selector.tooltip({
        //hoverTimeout: 250, 
        //tooltipHover: true,
        content: function (callback) {
            if (html == undefined) {
                callback('Loading....');
                var link = $(this).attr("data-entity-id"); // here retrieve the id of the teacher 
                $.get('/profileitems/hovercard/' + link, {}, function (data) {
                    callback(data);
                });
            } else {
                callback(html)
            }
        },
        open: function (event, ui) {
            if (typeof (event.originalEvent) === 'undefined') {
                return false;
            }

            var $id = $(ui.tooltip).attr('id');

            // close any lingering tooltips
            $('div.ui-tooltip').not('#' + $id).remove();

            // ajax function to pull in data and add it to the tooltip goes here
        },
        close: function (event, ui) {
            ui.tooltip.hover(function () {
                $(this).stop(true).fadeTo(400, 1);
            },
            function () {
                $(this).fadeOut('400', function () {
                    $(this).remove();
                });
            });
        }

    })
}

function ____bindGlobalEvents(events, $selector) {
    var values = events.split(",");
    for (var i = 0; i < values.length; i++) {
        if (values[i] == 'tooltip') {
            setTimeout(function () { tooltip($selector) }, 500);
        }
        else if (values[i] == 'dropdown') {
            setTimeout(function () { ____bindMenu($selector) }, 5000);
        }
        else if (values[i] == 'expand') {
            //setTimeout(makeSwitchableContents($selector), 1000);
        }
    }
}

function connect($obj) {
    var ser = $obj.serialize();
    var action = $obj.attr('action');
    $.ajax({
        url: action,
        type: 'POST',
        data: ser,
        dataType: 'html',
        waitingSelector: $obj.find('.waiting-bg'),
        success: function (response) {
            if ($('#hdnImprovement').length == 0) {
                $("#people-you-may-know").processTemplateURL("/association/suggestions");
            } else {
                $obj.parents('li').first().remove();
                var total = parseInt($('#hdnTotalConnections').val()) + 1;
                $('#hdnTotalConnections').val(total)
                $('.total-coonnections').html(total);
                if (total >= 10) {
                    $('#tabContents').load('/account/ImproveContent');
                }
            }

        }
    });
    return false;
}



function ____attachGlobalWait() {
    $('#gloablWaiting').addClass('active');
}

function ____removeGlobalWait() {
    $('#gloablWaiting').removeClass('active');
}

function ____loadCropAndUpload($obj, $toLoad) {
    $toLoad.load('/common/cropped')
}

function ____authentication() {
    ____attachGlobalWait();
    $.get('/account/Authentication', function (res) {
        $('<div id="share-cotainer" class="modal-content">').popUpWindow({
            action: "open",
            size: "medium"
        });
        ____removeGlobalWait();
        $('#share-cotainer').html(res);
    });
}



(function (factory) {
    "use strict";

    if (typeof define === 'function' && define.amd) { // AMD
        define(['jquery'], factory);
    }
    else if (typeof exports == "object" && typeof module == "object") { // CommonJS
        module.exports = factory(require('jquery'));
    }
    else { // Browser
        factory(jQuery);
    }
}) (function($, undefined) {
    "use strict";

    $.fn.percircle = function(options) {
        // default options
        var defaultOptions = {
            animate: true
        };
        
        // extend with any provided options
        if (!options) options = {};
        $.extend(options, defaultOptions);
        
        var rotationMultiplier = 3.6;

        // for each element matching selector
        return this.each(function(){
            var percircle = $(this);
            var progressBarColor = '';

            // When user tries adding a custom progress bar color, the text color should be updated as well.
            var changeTextColor = function (context, color) {
              // Change color text the same with progress bar color
              percircle.on('mouseover', function(){
                context.children('span').css('color', color);
              });

              percircle.on('mouseleave', function(){
                context.children('span').attr('style', '');
              });
            };

            // add percircle class for styling
            if (!percircle.hasClass('percircle')) percircle.addClass('percircle');
            // apply options
            if (typeof(percircle.attr('data-animate')) !== 'undefined') options.animate = percircle.attr('data-animate') == 'true';
            if (options.animate) percircle.addClass('animate');
            
            if (typeof(percircle.attr('data-progressBarColor')) !== 'undefined') {
                options.progressBarColor = percircle.attr('data-progressBarColor');
                progressBarColor = "style='border-color: "+ options.progressBarColor +"'";
                changeTextColor($(this), options.progressBarColor);
            } else {
                if (typeof options.progressBarColor !== 'undefined'){
                    progressBarColor = "style='border-color: "+ options.progressBarColor +"'";
                    changeTextColor($(this), options.progressBarColor);
                }
            }

            var percent = percircle.attr('data-percent') || options.percent || 0;
            var perclock = percircle.attr('data-perclock') || options.perclock || 0;
            var perdown = percircle.attr('data-perdown') || options.perdown || 0;
            if (percent) {
                if (percent > 50) percircle.addClass('gt50');
                var text = percircle.attr('data-text') || options.text || percent + '%';

                $('<span>'+text+'</span>').appendTo(percircle);
                // add divs for structure
                $('<div class="slice"><div class="bar" '+progressBarColor+'></div><div class="fill" '+progressBarColor+'></div></div>').appendTo(percircle);
                if (percent > 50)
                $('.bar', percircle).css({
                  '-webkit-transform' : 'rotate(180deg)',
                  '-moz-transform'    : 'rotate(180deg)',
                  '-ms-transform'     : 'rotate(180deg)',
                  '-o-transform'      : 'rotate(180deg)',
                  'transform'         : 'rotate(180deg)'
                });
                var rotationDegrees = rotationMultiplier * percent;
                // set timeout causes the animation to be visible on load
                setTimeout(function(){
                    $('.bar', percircle).css({
                      '-webkit-transform' : 'rotate(' + rotationDegrees + 'deg)',
                      '-moz-transform'    : 'rotate(' + rotationDegrees + 'deg)',
                      '-ms-transform'     : 'rotate(' + rotationDegrees + 'deg)',
                      '-o-transform'      : 'rotate(' + rotationDegrees + 'deg)',
                      'transform'         : 'rotate(' + rotationDegrees + 'deg)'
                    });
                }, 0);
            }  
        });
    };
	
	// display a presentable format of current time
	var getPadded = function(val){
		return val < 10 ? ('0' + val) : val;
	};
});
