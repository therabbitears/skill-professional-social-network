$.getJSON("/lookup/IndustryList", function (data) {
    var array = $.map(data, function (item) {
        return {
            label: item.Value,
            value: item.Value
        }
    });
    $("#IndustryName").autocomplete(
    {
        source: array
    })
});



$("#OrganizationName").autocomplete({
    minLength: 1,
    source: function (request, response) {
        isAutomComplete = true;
        $.ajax({
            url: "/Business/Search",
            data: { key: $("#OrganizationName").val() },
            dataType: "json",
            type: "POST",
            success: function (data) {
                isAutomComplete = false;
                if (data.length == 0) {
                    data.push({
                        Key: -1,
                        Value: $("#OrganizationName").val()
                    });
                    $("#OrganizationName").removeClass('working');
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
        $('#OrganizationID').val(ui.item.id);
        $('.divDurationEmplyee').show();
        //if (ui.item.id == -1) {
        //    $('#divIndustry').show();
        //} else {
        //    $('#IndustryName').val(ui.item.label);
        //    $('#divIndustry').hide();
        //}
    }
});

$('#OrganizationName').blur(function () {
    $('.divDurationEmplyee').show();
    if ($('#IndustryName').val() == '') {
        $('#divIndustry').show();
    }
});

$("#UniversityName").autocomplete({
    minLength: 1,
    source: function (request, response) {
        isAutomComplete = true;
        $.ajax({
            url: "/Business/Search/University",
            data: { key: $("#UniversityName").val() },
            dataType: "json",
            type: "POST",
            success: function (data) {
                isAutomComplete = false;
                if (data.length == 0) {
                    data.push({
                        Key: -1,
                        Value: $("#UniversityName").val()
                    });
                    $("#UniversityName").removeClass('working');
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
        $('#UniversityID').val(ui.item.id);
    }
});

$("#CourseName").autocomplete({
    minLength: 1,
    source: function (request, response) {
        isAutomComplete = true;
        $.ajax({
            url: "/CareerHistory/SearchCourse",
            data: { key: $("#CourseName").val() },
            dataType: "json",
            type: "POST",
            success: function (data) {
                isAutomComplete = false;
                if (data.length == 0) {
                    data.push({
                        Key: -1,
                        Value: $("#CourseName").val()
                    });
                    $("#CourseName").removeClass('working');
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
        $('#CourseID').val(ui.item.id);
    }
});

$("#JobTitleName").autocomplete({
    minLength: 1,
    source: function (request, response) {
        isAutomComplete = true;
        $.ajax({
            url: "/CareerHistory/SearchJobTitle",
            data: { key: $("#JobTitleName").val() },
            dataType: "json",
            type: "POST",
            success: function (data) {
                isAutomComplete = false;
                if (data.length == 0) {
                    data.push({
                        Key: -1,
                        Value: $("#JobTitleName").val()
                    });
                    $("#JobTitleName").removeClass('working');
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
        $('#JobTitleID').val(ui.item.id);
    }
});

$(document).ready(function () {
    $('#rdoYes, #rdoNo').click(function () {
        switchStage($(this))
    });

    $('#chkWorking').click(function () {
        if (!$(this).is(':checked')) {
            $('#employeementEndDuration').show();
        } else {
            $('#employeementEndDuration').hide();
        }
    });
});

function switchStage($obj) {
    if ($obj.is(':checked')) {
        if ($obj.val() == 1) {
            $('#divEmployement').show();
            $('#divStudent').hide();
        } else {
            $('#divEmployement').hide();
            $('#divStudent').show();
        }
    }
}

function setCareerOption($obj) {
    var ser = $obj.serialize();
    var action = $obj.attr('action');
    if ($obj.valid()) {
        $obj.find('button').attr('disabled', 'disabled');
        $.ajax({
            url: action,
            waitingSelector: $('#hdnPopup').val() == 'False' ? $obj.find('.waiting-bg') : $('.mate-progress'),
            type: 'POST',
            data: ser,
            dataType: 'JSON',
            success: function (response) {
                if (response.IsSuccess == true) {
                    if ($('#hdnPopup').val() == 'True') {
                        ____callbackFromPopup(response,'career-history');
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
    }
    return false;
}


(function ($) {
    // Required another if selected one.
    $.validator.addMethod('requeiredifselected', function (value, element, params) {
        var otherPropValue = $('#' + params.otherproperty).val();
        //                console.log({ otherproperty: params.otherproperty, value: value, otherPropValue: otherPropValue });

        if (otherPropValue > 0 && (value == '' || value == -1)) {
            return false;
        }
        return true;
    });

    $.validator.unobtrusive.adapters.add("requeiredifselected", ["otherproperty", "mode"], function (options) {
        options.rules["requeiredifselected"] = options.params;
        options.messages["requeiredifselected"] = options.message;
    });
    //=======================   End ================================//

    // Date validation
    $.validator.addMethod('cannotgreatermonthandyear', function (value, element, params) {
        var isStart = params.isstart;
        var isMonth = params.ismonth;
        var isYear = params.isyear;
        var otherPropertyYear = $('#' + params.otherpropertyyear).val();
        var otherPropertyMonth = $('#' + params.otherpropertymonth).val();
        var friendProperty = $('#' + params.friendproperty).val();


        //console.log({ isStart: isStart, isMonth: isMonth, isYear: isYear, otherPropertyYear: otherPropertyYear, otherPropertyMonth: otherPropertyMonth, friendProperty: friendProperty, value:value });
        var startDate = new Date();
        var endDate = new Date();
        if (otherPropertyMonth > 0 && otherPropertyYear > 0 && friendProperty > 0) {
            if (isStart == 'True') {
                if (isMonth == 'True') {
                    startDate = new Date(friendProperty, value, 1);
                } else {
                    startDate = new Date(value, friendProperty, 1);
                }
                endDate = new Date(otherPropertyYear, otherPropertyMonth, 1);
            } else {
                if (isMonth.toString() == 'True' && friendProperty > 0) {
                    endDate = new Date(friendProperty, value, 1);
                } else {
                    endDate = new Date(value, friendProperty, 1);
                }
                startDate = new Date(otherPropertyYear, otherPropertyMonth, 1);
            }
            console.log({ endDate: endDate, startDate: startDate });
            return (endDate >= startDate);
        }
        return true;
    });

    $.validator.unobtrusive.adapters.add("cannotgreatermonthandyear", ["friendproperty", "otherpropertymonth", "otherpropertyyear", "isstart", "ismonth", "isyear"], function (options) {
        options.rules["cannotgreatermonthandyear"] = options.params;
        options.messages["cannotgreatermonthandyear"] = options.message;
    });
    //=======================   End ================================//
    //=======================   End ================================//

    // Reuired if checked
    $.validator.addMethod("ifcheckedneedtoselect", function (value, element, params) {
        var isSelected = $('#' + params).is(':checked');
        if (isSelected == false && (value == '' || value == -1)) {
            return false;
        }
        return true;
    });
    $.validator.unobtrusive.adapters.addSingleVal("ifcheckedneedtoselect", "otherproperty");
    //  =======================   End ================================//
}(jQuery));

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