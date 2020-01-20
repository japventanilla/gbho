

jQuery(document).ready(function ($) {

    $(".datepicker").mask("99/99/9999");
    $(".mobilePhone").mask("+99 999 999 9999");

    $('.datepicker').datetimepicker({
        format: 'MM/DD/YYYY'
    }).on('dp.show', function (e) {
        var datepicker = $('body').find('.bootstrap-datetimepicker-widget:last'),
            position = datepicker.offset(),
            parent = datepicker.parent(),
            parentPos = parent.offset(),
            width = datepicker.width(),
            parentWid = parent.width();

        // move datepicker to the exact same place it was but attached to body
        datepicker.appendTo('body');
        datepicker.css({
            position: 'absolute',
            top: position.top,
            bottom: 'auto',
            left: position.left,
            right: 'auto'
        });

        // if datepicker is wider than the thing it is attached to then move it so the centers line up
        if (parentPos.left + parentWid < position.left + width) {
            var newLeft = parentPos.left;
            newLeft += parentWid / 2;
            newLeft -= width / 2;
            datepicker.css({ left: newLeft });
        }
    });

    $("#ImpersonateUser").change(function () {
        var id = $(this).val();
        var encoded = encodeURIComponent($(location).attr('href'));
        $(location).attr('href', '/MyAccount/Account/ImpersonateUser?memberId=' + id + "&currentUrl=" + encoded)
    });

});

function SetOptionValue(id, val) {
    jQuery('#' + id).val(val);
}