var _notifData = [];

function _escN(s) {
    return String(s || '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}

function renderNotifList() {
    var $l = $('#notifList');
    if (!$l.length) return;
    $l.empty();

    if (!_notifData.length) {
        $l.html('<div class="text-center text-muted py-4" style="font-size:13px;"><i class="bi bi-bell-slash" style="font-size:2rem;display:block;opacity:.3;"></i>No notifications</div>');
        $('#notifDot').hide();
        return;
    }

    var unread = _notifData.filter(function (n) { return !n.isRead; }).length;

    _notifData.forEach(function (n) {
        var icon = n.type === 'excuse' ? 'bi-file-text-fill' : n.type === 'concern' ? 'bi-exclamation-circle-fill' : n.type === 'warning' ? 'bi-exclamation-triangle-fill' : n.type === 'danger' ? 'bi-x-octagon-fill' : 'bi-envelope-fill';
        var color = n.type === 'excuse' ? '#D97706' : n.type === 'concern' ? '#DC2626' : n.type === 'warning' ? '#D97706' : n.type === 'danger' ? '#DC2626' : '#2563EB';
        var bg = n.isRead ? 'transparent' : '#F0F4FF';
        var time = n.createdAt ? new Date(n.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) : '';
        var from = (n.senderName && n.senderRole) ? '<span style="font-size:11px;color:var(--text-muted);">From: ' + _escN(n.senderName) + ' (' + _escN(n.senderRole) + ')</span><br>' : '';

        $l.append(
            '<div class="notif-item d-flex align-items-start gap-2 p-3" data-id="' + n.id + '" '
            + 'style="border-bottom:1px solid var(--border);background:' + bg + ';cursor:pointer;">'
            + '<div style="width:34px;height:34px;background:#F0F4FF;border-radius:50%;display:flex;align-items:center;justify-content:center;flex-shrink:0;">'
            + '<i class="bi ' + icon + '" style="color:' + color + ';font-size:15px;"></i></div>'
            + '<div style="flex:1;min-width:0;">'
            + '<div style="font-size:12px;font-weight:600;">' + _escN(n.title) + '</div>'
            + from
            + '<div style="font-size:12px;color:var(--text-muted);white-space:nowrap;overflow:hidden;text-overflow:ellipsis;">' + _escN(n.message) + '</div>'
            + '<div style="font-size:11px;color:var(--text-muted);margin-top:2px;"><i class="bi bi-clock me-1"></i>' + time + '</div>'
            + '</div>'
            + (n.isRead ? '' : '<span style="width:8px;height:8px;background:#2563EB;border-radius:50%;flex-shrink:0;margin-top:6px;"></span>')
            + '</div>'
        );
    });

    if (unread > 0) { $('#notifDot').show(); } else { $('#notifDot').hide(); }
}

function loadNotifications() {
    $.ajax({
        url: '/api/Notifications', dataType: 'json',
        success: function (data) { _notifData = Array.isArray(data) ? data : []; renderNotifList(); },
        error: function () { }
    });
}

function pushNotification(title, message, type, recipientId) {
    return $.ajax({
        type: 'POST', url: '/api/Notifications',
        contentType: 'application/json',
        data: JSON.stringify({ title: title, message: message, type: type || 'info', recipientId: recipientId || 'admin', senderName: 'System', senderRole: 'Auto' })
    });
}

$(function () {
    loadNotifications();
    setInterval(loadNotifications, 30000);

    $('#notifBellBtn').closest('.dropdown').on('show.bs.dropdown', function () { loadNotifications(); });

    $(document).on('click', '.notif-item', function () {
        var id = $(this).data('id');
        $(this).css('background', 'transparent').find('span[style*="border-radius:50%"]').remove();
        var n = _notifData.find(function (x) { return x.id == id; });
        if (n) n.isRead = true;
        if (!_notifData.filter(function (x) { return !x.isRead; }).length) $('#notifDot').hide();
        $.ajax({ type: 'PUT', url: '/api/Notifications/' + id + '/read' });
    });

    $(document).on('click', '#clearNotif', function (e) {
        e.preventDefault();
        $.ajax({
            type: 'DELETE', url: '/api/Notifications',
            success: function () { _notifData = []; renderNotifList(); if (typeof showToast === 'function') showToast('Notifications cleared.', 'info'); }
        });
    });

    $(document).on('click', '#openMsgBtn', function () {
        $('#msgSubject').val(''); $('#msgBody').val(''); $('#msgType').val('message');
        $('#sendMsgModal').modal('show');
    });

    $(document).on('click', '#sendMsgBtn', function () {
        var subject = $('#msgSubject').val().trim();
        var body = $('#msgBody').val().trim();
        var type = $('#msgType').val();
        var to = $('#msgTo').val();
        if (!subject) { if (typeof showToast === 'function') showToast('Subject is required.', 'warning'); return; }
        if (!body) { if (typeof showToast === 'function') showToast('Message is required.', 'warning'); return; }
        var senderName = $('.topbar-user-name').first().text().trim() || 'User';
        var senderRole = $('.topbar-user-role').first().text().trim() || '';
        var typeLabel = type === 'excuse' ? 'Excuse Letter' : type === 'concern' ? 'Concern' : 'Message';
        $('#sendMsgBtn').prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-1"></span>Sending…');
        $.ajax({
            type: 'POST', url: '/api/Notifications', contentType: 'application/json',
            data: JSON.stringify({ title: '[' + typeLabel + '] ' + subject, message: body, type: type, recipientId: to, senderName: senderName, senderRole: senderRole }),
            success: function () { $('#sendMsgModal').modal('hide'); $('#msgSubject').val(''); $('#msgBody').val(''); if (typeof showToast === 'function') showToast('Message sent successfully.', 'success'); },
            error: function () { if (typeof showToast === 'function') showToast('Failed to send. Please try again.', 'error'); },
            complete: function () { $('#sendMsgBtn').prop('disabled', false).html('<i class="bi bi-send"></i> Send'); }
        });
    });
});