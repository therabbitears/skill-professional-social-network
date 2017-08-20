function ____secondaryAssociationAction($obj) {
    var form = $obj.parents('form:first');
    form.find('#actn').val($obj.val());
    form.submit();
    return true;
}

function ____associationAction($obj) {
    var ser = $obj.serialize();
    var action = $obj.attr('action');
    ____attachWait($obj);
    $obj.find('button').attr('disabled', 'disabled');
    $.ajax({
        url: action,
        type: 'POST',
        data: ser,
        waitingSelector: $obj.find('.waiting-bg'),
        dataType: 'json',
        success: function (response) {
            $obj.find('button').removeAttr('disabled');
            if ($obj.find('#actn').val().toLowerCase() == 'unfollow') {
                $obj.find('button').remove();
                $('<span />').attr('class', 'btn btn-success btn-sm').appendTo($obj.find('.buttons')).html('Unfollowed');
            }
            if ($obj.find('#actn').val().toLowerCase() == 'remove') {
                $obj.find('button').remove();
                $('<span />').attr('class', 'btn btn-success btn-sm').appendTo($obj.find('.buttons')).html('Removed');
            }
            if ($obj.find('#actn').val().toLowerCase() == 'connect') {
                $obj.find('button').remove();
                $('<span />').attr('class', 'btn btn-success btn-sm').appendTo($obj.find('.buttons')).html('Request sent');
                $obj.find('.group-item').fadeOut(3000)
            }
            if ($obj.find('#actn').val().toLowerCase() == 'skip') {
                $obj.find('button').remove();
                $('<span />').attr('class', 'btn btn-success btn-sm').appendTo($obj.find('.buttons')).html('Skipped');
                $obj.find('.group-item').fadeOut(3000)
            }
            if ($obj.find('#actn').val().toLowerCase() == 'cancel') {
                $obj.find('button').remove();
                $('<span />').attr('class', 'btn btn-success btn-sm').appendTo($obj.find('.buttons')).html('Canceled');
                $obj.find('.group-item').fadeOut(4000)
            }
            if ($obj.find('#actn').val().toLowerCase() == 'decline') {
                $obj.find('button').remove();
                $('<span />').attr('class', 'btn btn-success btn-sm').appendTo($obj.find('.buttons')).html('Canceled');
                $obj.find('.group-item').fadeOut(4000)
            }
            if ($obj.find('#actn').val().toLowerCase() == 'approve') {
                $obj.find('button').remove();
                $('<span />').attr('class', 'btn btn-success btn-sm').appendTo($obj.find('.buttons')).html('Connected');
                $obj.find('.group-item').fadeOut(4000)
            }
            if ($obj.find('#actn').val().toLowerCase() == 'approvefollow') {
                $obj.find('button').remove();
                $('<span />').attr('class', 'btn btn-success btn-sm').appendTo($obj.find('.buttons')).html('Approved');
                $obj.find('.reuqest-item').fadeOut(4000)
            }
            ____removeWait($obj);
        }
    });
    return false;
}

function ____suggestionAction($obj) {
    var ser = $obj.serialize();
    var action = $obj.attr('action');
    $.ajax({
        url: action,
        type: 'POST',
        data: ser,
        waitingSelector: $obj.find('.waiting-bg'),
        dataType: 'json',
        success: function (response) {
            if ($obj.find('#actn').val().toLowerCase() == 'unfollow') {
                $obj.find('button').remove();
                $('<span />').attr('class', 'btn btn-success btn-sm').appendTo($obj.find('.buttons')).html('Unfollowed');
            }
            if ($obj.find('#actn').val().toLowerCase() == 'connect') {
                $obj.find('button').remove();
                $('<span />').attr('class', 'btn btn-success btn-sm').appendTo($obj.find('.buttons')).html('Request sent');
                $obj.find('.reuqest-item').fadeOut(3000)
            }
            if ($obj.find('#actn').val().toLowerCase() == 'skip') {
                $obj.find('button').remove();
                $('<span />').attr('class', 'btn btn-success btn-sm').appendTo($obj.find('.buttons')).html('Skipped');
                $obj.find('.reuqest-item').fadeOut(3000)
            }
        }
    });
    return false;
}
function ____secondarySuggestionAction($obj) {
    var form = $obj.parents('form:first');
    form.find('#actn').val($obj.val());
    form.submit();
    return true;
}

function ____attachWait($obj) {
    $obj.find('.waiting-bg').addClass('working');
}

function ____removeWait($obj) {
    $obj.find('.waiting-bg').removeClass('working');
}   