function ____executeActivityAction($obj) {
    var hash = $obj.attr('data-hash');
    var action = $obj.attr('data-action');
    var token = $obj.parent('form').find('input[name="__RequestVerificationToken"]').val();
    var data = $obj.parent('form').serializeArray();
    data.push({ name: 'action', value: action });
    $.post('/association/activityaction', data, function (res) {
        $obj.parents('li:first').fadeOut("medium");
    }
    );
}