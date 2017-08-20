
function ____loadFeedPreference($obj) {
    var ser = $obj.serialize();
    var action = $obj.attr('action');
    if ($obj.valid()) {
        $.ajax({
            url: action,
            waitingSelector: $obj.find('.waiting-bg'),
            type: 'POST',
            data: ser,
            dataType: 'html',
            success: function (response) {
                $('.feed-preferences').find('button').removeClass('added');
                $obj.find('button').addClass('added');
                $('#feeds').html(response);
                tooltip($("a[data-hover-card='true']"));
                $obj.removeAttr('disabled');
                ____bindMenu();
                ____bindMoreDetails();
                feedType = $obj.find('button').val();
                ended = false;
                if ($('#feeds').find('.feed').length >= 10) {
                    $('.load-more').show();
                } else {
                    $('.load-more').hide();
                }
            }
        });
    }
    return false;
}
function ____executeActivityAction($obj) {
    var hash = $obj.attr('data-hash');
    var action = $obj.attr('data-action');
    var token = $obj.parent('form').find('input[name="__RequestVerificationToken"]').val();
    var data = $obj.parent('form').serializeArray();
    data.push({ name: 'action', value: action });
    $.post('/association/activityaction', data, function () { $("#network-happenings").processTemplateURL("/association/happenings", null, { data: { pageNumber: 0, pageSize: 2 }, ___callback: process }); });

}

$(document).ready(function () {
    $("#people-you-may-know").setTemplateURL("/Templates/suggestions.txt");
    var max = $("#people-you-may-know").attr('data-max-load');
    var data = { pageSize: max != undefined ? max : 5 };
    $("#people-you-may-know").processTemplateURL("/association/suggestions", null, { data: data, ___callback: function () { tooltip($("a[data-hover-card='true']")); } });
    if ($("#network-happenings").length > 0) {
        $("#network-happenings").setTemplateURL("/Templates/happenings.txt");
        $("#network-happenings").processTemplateURL("/association/happenings", null, { data: { pageNumber: 0, pageSize: 2 }, ___callback: process });
    }

    $('#askOpportunity').click(function () {
        var hash = $('#groupHash').length == 1 ? $('#groupHash').val() : null;
        $.get('/account/AskOpportunity?groupHash=' + hash, function (res) {
            $('<div id="share-opp-cotainer" class="modal-content">').popUpWindow({
                action: "open",
                size: "medium"
            });
            $('#share-opp-cotainer').html(res).tooltip();
            setOppAutoComplete();
           // ("#share-opp-cotainer").tooltip();
        });

    });

    $('#shareOpportunity').click(function () {
        var hash = $('#groupHash').length == 1 ? $('#groupHash').val() : null;
        $.get('/account/ShareOpportunity?groupHash=' + hash, function (res) {
            $('<div id="share-opp-cotainer" class="modal-content">').popUpWindow({
                action: "open",
                size: "medium"
            });
            $('#share-opp-cotainer').html(res);
            setShareOppAutoComplete();
        });

    });

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

    tooltip($('.html-help'), '<div class="profile-help-data"><img src="/content/images/full-profile.jpg" style="max-width:300px;" /><h2>Minimum information required for the profile:</h2><ul><li>Add at least 3 skills to suggest you best possible connections.</li><li>Add at least 1 work history or education accomplishment to rank your profile high.</li><li>Add one or more accomplishment like project, publication, research.</li><li>Add profile and cover picture.</li><li>Connect with 10 people to boost your profile in searches and grow your network faster.</li></ul></div>');

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

function setShareOppAutoComplete() {
    var data = [];
    $("#txtJobTitleName").autocomplete({
        minLength: 1,
        source: function (request, response) {
            isAutomComplete = true;
            $.ajax({
                url: "/CareerHistory/SearchJobTitle",
                data: { key: $("#txtJobTitleName").val() },
                dataType: "json",
                type: "POST",
                success: function (data) {
                    isAutomComplete = false;
                    if (data.length == 0) {
                        $("#txtJobTitleName").removeClass('working');
                    }
                    response($.map
                    (data, function (obj) {
                        return {
                            label: obj.Value.toString().trim(),
                            value: obj.Value.toString().trim(),
                            id: obj.Key.toString().trim()
                        };
                    }));
                },
                error: function () { isAutomComplete = false; }
            });
        },
        select: function (event, ui) {
            if ($('input[data-val-name="' + ui.item.label + '"]').length == 0) {
                var element = '<span class="skill-tag-editable"> ' + ui.item.label + ' <input type="hidden" name="JobTitles" value="' + ui.item.id.toString() + '" data-val-name="' + ui.item.label + '" /><span class="remove" onclick="removeTag($(this))">X</span></span>';
                $('#JobTitles').append(element.toString());
            }
            this.value = "";
            return false;
        }
    });

    $("#txtSkillsName").autocomplete({
        minLength: 1,
        source: function (request, response) {
            isAutomComplete = true;
            $.ajax({
                url: "/SkillHistory/SearchAllSkill",
                data: { key: $("#txtSkillsName").val() },
                dataType: "json",
                type: "POST",
                success: function (data) {
                    isAutomComplete = false;
                    if (data.length == 0) {
                        $("#txtSkillsName").removeClass('working');
                    }
                    response($.map
                    (data, function (obj) {
                        return {
                            label: obj.Value.toString().trim(),
                            value: obj.Value.toString().trim(),
                            id: obj.Key.toString().trim()
                        };
                    }));
                },
                error: function () { isAutomComplete = false; }
            });
        },
        select: function (event, ui) {
            if ($('input[data-val-name="' + ui.item.label + '"]').length == 0) {
                var element = '<span class="skill-tag-editable"> ' + ui.item.label + ' <input type="hidden" name="Skills" value="' + ui.item.id.toString() + '" data-val-name="' + ui.item.label + '" /><span class="remove" onclick="removeTag($(this))">X</span></span>';
                $('#Skills').append(element.toString());
            }
            this.value = "";
            return false;
        }
    });

    $("#opportunityData").tooltip();
}

function setOppAutoComplete() {

    var data = [];
    $("#conenctionNameOpp").autocomplete({
        minLength: 1,
        source: function (request, response) {
            isAutomComplete = true;
            $.ajax({
                url: "/Association/ConnectionHeads",
                data: { keyword: $("#conenctionNameOpp").val() },
                dataType: "json",
                type: "POST",
                success: function (data) {
                    isAutomComplete = false;
                    if (data.length == 0) {
                        $("#conenctionNameOpp").removeClass('working');
                    }
                    response($.map
                    (data, function (obj) {
                        return {
                            label: obj.FormatedName,
                            value: obj.FormatedName,
                            entityID: obj.EntityID.toString().trim(),
                            jobTitle: obj.ProfileHeading == null ? '' : obj.ProfileHeading,
                            profilePath: obj.ProfileIconPath == null ? '/content/images/no-image-sm.png' : obj.ProfileIconPath
                        };
                    }));
                },
                error: function () { isAutomComplete = false; }
            });
        },
        create: function () {
            $(this).data('ui-autocomplete')._renderItem = function (ul, item) {
                return $('<li>').append('<div class="search-result-item"><div class="thumbnail-notification-con"><img class="thumbnail-notification" src="' + item.profilePath + '" alt="no image ' + item.label + ' "></div><div class="side-thumbnail"><h3>' + item.label + '</h3><span class="child-headings">' + item.jobTitle + '</span></div>     </div>').appendTo(ul);
            }
        },
        open: function () { $(this).autocomplete("widget").appendTo("#ParticipentsOpp").css('position', 'static'); },
        select: function (event, ui) {
            $('#hdnOpportunityForConnection').val(ui.item.entityID);
            fetchOpportunityData();
        }
    });
}

function fetchOpportunityData() {
    var eID = null;
    if ($('#rdoForConnection').is(':checked')) {
        eID = $('#hdnOpportunityForConnection').val();
    }
    $.get('/CareerHistory/GetDataForOpportunity?eid=' + eID, function (response) {
        $('#opportunityData').html(response);
        $("#opportunityData").tooltip();
    });
}

function process(data) {
    if (data != undefined) {
        var total = JSON.parse(data);
        if (total.length == 0) {
            $('#happening-container').remove();
        } else {
            $('#happening-container').show()
            if (total[0].TotalRows > 2) {
                $('#happening-container .show-all').show();
            } else {
                $('#happening-container .show-all').remove();
            }
            tooltip($("a[data-hover-card='true']"));
        }
    }
}

$(function () {
    tooltip($("a[data-hover-card='true']"));
});

function ____previewImage(input) {
    var formData = new FormData($('#frmShareNews')[0]);
    $.ajax({
        url: '/Upload/AbstractImage',  //Server script to process data
        type: 'POST',
        xhr: function () {  // Custom XMLHttpRequest
            var myXhr = $.ajaxSettings.xhr();
            if (myXhr.upload) { // Check if upload property exists
                myXhr.upload.addEventListener('progress', progressHandlingFunction, false); // For handling the progress of the upload
            }
            return myXhr;
        },
        //Ajax events
        beforeSend: beforeSendHandler,
        success: function (response) {
            $('progress').hide();
            if (response.IsValidImage) {
                var preview = document.getElementById('preview');
                if (input.files && input.files[0]) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        console.log(e);
                        preview.setAttribute('src', e.target.result);
                        //$('#ImageContent').val($('#preview').attr('src'))
                    }
                    reader.readAsDataURL(input.files[0]);
                } else {
                    preview.setAttribute('src', 'src="/content/images/no-preview.png"');
                }
                $('.preview').addClass('active');
            } else {
                alert('Invalid image');
            }
        },
        error: errorHandler,
        // Form data
        data: formData,
        //Options to tell jQuery not to process data or worry about content-type.
        cache: false,
        contentType: false,
        processData: false
    });
}

function beforeSendHandler() {

}

function errorHandler() {

}
function progressHandlingFunction(e) {
    if (e.lengthComputable) {
        $('progress').show();
        $('progress').attr({ value: e.loaded, max: e.total });
    }
}
function shareNews($obj) {
    var form = document.getElementById('frmShareNews');
    var ser = new FormData(form);
    var action = $obj.attr('action');
    if ($obj.valid()) {
        $.ajax({
            url: action,
            type: 'POST',
            data: ser,
            waitingSelector: $obj.find('.waiting-bg'),
            dataType: 'html',
            cache: false,
            enctype: "multipart/form-data",
            contentType: false,
            processData: false,
            success: function (response) {
                $('#feeds').prepend(response);
                $('#share-feed-update-profile').load('/press/sharenews');
                ____bindMenu($('#feeds'));
            }
        });
    }
    return false;
}
function ____removePreviewAndMedia($obj) {
    $('.preview').removeClass('active');
    var preview = $('#preview');
    preview.attr('src', '');
    $('#postPhoto').val(null);
}

function ____findOpportunityOptions(value) {
    $('#opportunityData').html('');
    if (value == 1) {
        $('#forConnection').hide();
        fetchOpportunityData();
        $('#conenctionNameOpp').val("");
        $('#hdnOpportunityForConnection').val("");

    } else {
        $('#forConnection').show();
    }
}



function removeTag($obj) {
    $obj.parents('.skill-tag-editable:first').remove();
}

function getShareOpportunity(url, isJustShared, type) {
    $.get('/press/sharer?url=' + encodeURI(url) + '&isSharer=' + isJustShared.toString() + '&mode=' + type.toString(), function (res) {
        $('<div id="share-opp-cotainer" class="modal-content">').popUpWindow({
            action: "open",
            size: "medium"
        });
        $('#share-opp-cotainer').html(res);
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