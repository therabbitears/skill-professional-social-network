function ___saveSkills($obj) {
    var ser = $obj.serialize();
    var action = $obj.attr('action');
    $.ajax({
        url: action,
        type: 'POST',
        data: ser,
        waitingSelector: $obj.find('.waiting-bg'),
        dataType: 'JSON',
        success: function (response) {
            if (response.IsSuccess == true) {
                if ($('#hdnPopup').val() == 'True') {
                    ____callbackFromPopup(response, 'skills');
                } else {
                    $obj.remove();
                    location.href = response.RedirectUrl;
                }
            } else {
                $obj.find('button').removeAttr('disabled');
            }
        },
        error: function () { $obj.find('button').removeAttr('disabled'); }
    });
    return false;
}
$(document).ready(function () {
    var data = [];
    $("#Skill").autocomplete({
        minLength: 1,
        source: function (request, response) {
            isAutomComplete = true;
            $.ajax({
                url: "/SkillHistory/SearchAllSkill",
                data: { key: $("#Skill").val() },
                dataType: "json",
                type: "POST",
                success: function (data) {
                    isAutomComplete = false;
                    if (data.length == 0) {
                        data.push({
                            Key: -1,
                            Value: $("#Skill").val(),
                            Total: 0
                        });
                        $("#Skill").removeClass('working');
                    }
                    response($.map
                    (data, function (obj) {
                        return {
                            label: obj.Value.toString().trim(),
                            value: obj.Value.toString().trim(),
                            count: obj.Total.toString().trim()
                        };
                    }));
                },
                error: function () { isAutomComplete = false; }
            });
        },
        create: function () {
            $(this).data('ui-autocomplete')._renderItem = function (ul, item) {
                return $('<li>').append('<div class="skill-with-count-result">' + item.label + '</div>').appendTo(ul);
            }
        },
        select: function (event, ui) {
            $("#Skill").val('');
            if ($('input[data-val-name="' + ui.item.label + '"]').length == 0) {
                var element = '<div class="skill-tag-editable">' + ui.item.label + ' <input type="hidden" name="Skills" value="' + ui.item.label.toString() + '" data-val-name="' + ui.item.label + '"/> <span onclick="____removeTempMember($(this))" class="remove">X</span></div>';
                $('.skill-selected').append(element.toString());
            }
            this.value = "";
            $('#btnAction').html('Done');
            return false;
        }
    });
});
function ____removeTempMember($obj) {
    $obj.parent('.skill-tag-editable').remove();
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