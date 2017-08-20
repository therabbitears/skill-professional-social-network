function ____invite($form) {
    var ser = $form.serialize();
    var action = $form.attr('action');
    $.ajax({
        url: action,
        type: 'POST',
        waitingSelector: $('.wizard-content-wrapper').find('.waiting-bg'),
        data: ser,
        dataType: 'html',
        success: function (response) {
            $('.result').html(response);
        }
    });
    return false;
}

$(document).ready(function () {
    $('input[data-val-name]').change(function () {
        $.post('/import/update', { id: $(this).attr('data-val-id'), value: $(this).val() });
    });

    $("#chkAll").click(function () {
        $('input:checkbox').not(this).prop('checked', this.checked);
    });
})