var allInbox = [], allSent = [], selectedUserId = '', selectedUserName = '';

function renderMailItems(list, containerId, showFrom) {
    var $c = $('#' + containerId);
    if (!$c.length) return;
    var filter = $('#filterMailType').val();
    var items = list.filter(function (m) {
        if (filter === 'unread') return !m.isRead;
        if (filter === 'read') return m.isRead;
        return true;
    });
    if (!items.length) {
        $c.html(
            '<div class="text-center text-muted py-5">' +
            '<i class="bi bi-inbox" style="font-size:2.5rem;display:block;opacity:.3;"></i>' +
            '<div class="mt-2">No messages</div></div>'
        );
        return;
    }
    var html = '';
    items.forEach(function (m) {
        var unread = !m.isRead;
        var typeIcon = m.type === 'concern' ? 'bi-exclamation-triangle text-danger' :
            m.type === 'announcement' ? 'bi-megaphone text-warning' : 'bi-envelope text-primary';
        var dt = m.createdAt ? new Date(m.createdAt).toLocaleString() : '';
        var meta = showFrom
            ? 'From: ' + (m.senderName || 'Unknown') + ' (' + (m.senderRole || '') + ')'
            : 'To: ' + (m.recipientId || '');
        html +=
            '<div class="mail-item d-flex align-items-start gap-3 p-3 border-bottom' + (unread ? ' mail-unread' : '') +
            '" data-id="' + m.id + '" style="cursor:pointer;' + (unread ? 'background:#F0F7FF;' : '') + '">' +
            '<i class="bi ' + typeIcon + '" style="font-size:20px;margin-top:2px;"></i>' +
            '<div style="flex:1;min-width:0;">' +
            '<div class="d-flex justify-content-between"><strong style="font-size:13px;">' + (m.title || '(No subject)') + '</strong>' +
            '<span style="font-size:11px;color:#94A3B8;">' + dt + '</span></div>' +
            '<div style="font-size:12px;color:#64748B;">' + meta + '</div>' +
            '<div style="font-size:12px;color:#94A3B8;overflow:hidden;text-overflow:ellipsis;white-space:nowrap;">' + (m.message || '') + '</div></div>' +
            (unread ? '<span class="badge bg-primary" style="font-size:10px;">New</span>' : '') +
            '</div>';
    });
    $c.html(html);
}

function loadMailInbox() {
    return $.ajax({ url: '/api/proxy/api/Messages/inbox', dataType: 'json', global: false })
        .then(function (data) {
            allInbox = safeArray(data);
            if ($('#mailInboxList').length) renderMailItems(allInbox, 'mailInboxList', true);
            if ($('#mailList').length) renderMailItems(allInbox, 'mailList', true);
        });
}

function loadMailSent() {
    return $.ajax({ url: '/api/proxy/api/Messages/sent', dataType: 'json', global: false })
        .then(function (data) {
            allSent = safeArray(data);
            if ($('#mailSentList').length) renderMailItems(allSent, 'mailSentList', false);
        });
}

function sendMailMessage(recipientId, subject, body, type, onSuccess) {
    $('#page-loader').fadeIn(150);
    $.ajax({
        type: 'POST',
        url: '/api/proxy/api/Messages',
        contentType: 'application/json',
        data: JSON.stringify({ recipientId: recipientId, title: subject, message: body, type: type || 'message' }),
        success: function () {
            $('#page-loader').fadeOut(200);
            if (typeof showToast === 'function') showToast('Message sent successfully.', 'success');
            if (typeof loadNotifications === 'function') loadNotifications();
            if (typeof onSuccess === 'function') onSuccess();
        },
        error: function (x) {
            $('#page-loader').fadeOut(200);
            var msg = (x.responseJSON && x.responseJSON.message) || x.statusText || 'Send failed';
            if (typeof showToast === 'function') showToast(msg, 'error');
        }
    });
}

function initMailPage() {
    $('#toMode').on('change', function () {
        var v = $(this).val();
        $('#specificRecipBox').toggle(v === 'specific');
        if (v !== 'specific') {
            selectedUserId = '';
            selectedUserName = '';
            $('#selectedRecipient').text('');
            $('#recipientSearch').val('');
        }
    });

    var searchTimer;
    $('#recipientSearch').on('input', function () {
        clearTimeout(searchTimer);
        var q = $(this).val().trim();
        selectedUserId = '';
        selectedUserName = '';
        $('#selectedRecipient').text('');
        if (q.length < 2) { $('#recipientDropdown').hide(); return; }
        searchTimer = setTimeout(function () {
            $.ajax({
                url: '/api/proxy/api/UserSearch?q=' + encodeURIComponent(q),
                dataType: 'json',
                global: false,
                success: function (users) {
                    users = safeArray(users);
                    if (!users.length) {
                        $('#recipientDropdown').html('<div class="p-2 text-muted" style="font-size:12px;">No users found</div>'.replace(/<\/motion>/g, '').replace(/<motion>/g, ''));
                        $('#recipientDropdown').show();
                        return;
                    }
                    var html = '';
                    users.forEach(function (u) {
                        var gid = parseInt(u.groupId) || 0;
                        var role = gid === 2 ? 'Teacher' : gid === 3 ? 'Student' : 'Admin';
                        html += '<div class="recip-item p-2 border-bottom" data-uid="' + u.userId + '" data-name="' + u.name +
                            '" style="cursor:pointer;font-size:13px;"><strong>' + u.name + '</strong> <span class="text-muted">(' + role +
                            ')</span><br><small>' + u.email + '</small></div>';
                    });
                    $('#recipientDropdown').html(html).show();
                }
            });
        }, 300);
    });

    $(document).on('click', '.recip-item', function () {
        selectedUserId = String($(this).data('uid'));
        selectedUserName = $(this).data('name');
        $('#recipientSearch').val(selectedUserName);
        $('#selectedRecipient').text('Will send to: ' + selectedUserName);
        $('#recipientDropdown').hide();
    });

    $(document).on('click', function (e) {
        if (!$(e.target).closest('#recipientSearch,#recipientDropdown').length)
            $('#recipientDropdown').hide();
    });

    $('a[data-mail-tab]').on('click', function (e) {
        e.preventDefault();
        var tab = $(this).data('mail-tab');
        $('a[data-mail-tab]').removeClass('active');
        $(this).addClass('active');
        $('.mail-tab-pane').hide();
        $('#mailTab' + tab).show();
        if (tab === 'Inbox') loadMailInbox();
        if (tab === 'Sent') loadMailSent();
    });

    $('#sendMsgBtn').on('click', function () {
        var mode = $('#toMode').val();
        var subject = $('#msgSubject').val().trim();
        var body = $('#msgBody').val().trim();
        var recipId = mode;
        if (!subject) { showToast('Subject is required.', 'warning'); return; }
        if (!body) { showToast('Message is required.', 'warning'); return; }
        if (mode === 'specific') {
            if (!selectedUserId) { showToast('Please select a recipient.', 'warning'); return; }
            recipId = selectedUserId;
        }
        sendMailMessage(recipId, subject, body, $('#msgType').val(), function () {
            $('#msgSubject,#msgBody').val('');
            selectedUserId = '';
            selectedUserName = '';
            $('#selectedRecipient').text('');
            $('#recipientSearch').val('');
            $('#toMode').val('all');
            $('#specificRecipBox').hide();
            loadMailInbox();
            loadMailSent();
        });
    });

    $(document).on('click', '.mail-item', function () {
        var id = $(this).data('id');
        var m = allInbox.concat(allSent).find(function (x) { return x.id == id; });
        if (!m) return;
        $('#vmTitle').text(m.title || '(No subject)');
        $('#vmFrom').text((m.senderName || 'Unknown') + ' (' + (m.senderRole || '') + ')');
        $('#vmType').text(m.type || 'message');
        $('#vmDate').text(m.createdAt ? new Date(m.createdAt).toLocaleString() : '');
        $('#vmBody').text(m.message || '');
        $('#viewMsgModal').modal('show');
        if (!m.isRead) {
            $.ajax({ type: 'PUT', url: '/api/proxy/api/Messages/' + id + '/read', global: false })
                .done(function () { loadMailInbox(); loadMailSent(); });
        }
    });

    $('#filterMailType').on('change', function () {
        if ($('#mailInboxList').length) renderMailItems(allInbox, 'mailInboxList', true);
        if ($('#mailList').length) renderMailItems(allInbox, 'mailList', true);
        if ($('#mailSentList').length) renderMailItems(allSent, 'mailSentList', false);
    });

    $('#markAllReadBtn').on('click', function () {
        var unread = allInbox.filter(function (m) { return !m.isRead; });
        if (!unread.length) { showToast('No unread messages.', 'info'); return; }
        var defs = unread.map(function (m) {
            return $.ajax({ type: 'PUT', url: '/api/proxy/api/Messages/' + m.id + '/read', global: false });
        });
        $.when.apply($, defs).done(function () {
            loadMailInbox();
            showToast('All marked as read.', 'success');
        });
    });

    $('#clearAllMailBtn').on('click', function () {
        $.ajax({
            type: 'DELETE', url: '/api/proxy/api/Messages', global: false,
            success: function () {
                loadMailInbox();
                loadMailSent();
                showToast('Mail cleared.', 'success');
            },
            error: function () { showToast('Clear failed.', 'error'); }
        });
    });

    loadMailInbox();
    loadMailSent();
    setInterval(loadMailInbox, 30000);
}
