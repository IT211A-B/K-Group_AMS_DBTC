/* jquery/toast.js */
$(function () {
    window.showToast = function (msg, type) {
        type = type || 'info';
        var icons = { success: 'bi-check-circle-fill', error: 'bi-x-circle-fill', warning: 'bi-exclamation-triangle-fill', info: 'bi-info-circle-fill' };
        var colors = { success: '#16A34A', error: '#DC2626', warning: '#D97706', info: '#2563EB' };
        var $t = $('<div class="toast-msg ' + type + '"><i class="bi ' + icons[type] + '" style="color:' + colors[type] + ';font-size:18px;flex-shrink:0;"></i><span>' + msg + '</span><button class="toast-close">&times;</button></div>');
        $('#toast-container').append($t);
        var tid = setTimeout(function () { $t.fadeOut(400, function () { $(this).remove(); }); }, 3500);
        $t.find('.toast-close').on('click', function () { clearTimeout(tid); $t.fadeOut(300, function () { $(this).remove(); }); });
    };
});