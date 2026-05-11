/* jquery/export.js */
$(function () {
    window.exportCSV = function (filename, headers, rows) {
        var esc = function (v) { return '"' + String(v).replace(/"/g, '""') + '"'; };
        var csv = [headers.map(esc).join(',')];
        rows.forEach(function (r) { csv.push(r.map(esc).join(',')); });
        var blob = new Blob([csv.join('\r\n')], { type: 'text/csv;charset=utf-8;' });
        var url = URL.createObjectURL(blob);
        var $a = $('<a>').attr({ href: url, download: (filename || 'export') + '.csv' }).appendTo('body');
        $a[0].click(); $a.remove(); URL.revokeObjectURL(url);
        showToast('CSV exported.', 'success');
    };
    window.printTable = function (title, headers, rows) {
        var th = '<table border="1" cellpadding="7" cellspacing="0" style="width:100%;border-collapse:collapse;font-size:13px;"><thead><tr style="background:#1A3A8F;color:#fff;">';
        headers.forEach(function (h) { th += '<th>' + h + '</th>'; }); th += '</tr></thead><tbody>';
        rows.forEach(function (row, i) { th += '<tr style="background:' + (i % 2 === 0 ? '#f8faff' : '#fff') + '">'; row.forEach(function (c) { th += '<td>' + c + '</td>'; }); th += '</tr>'; });
        th += '</tbody></table>';
        var w = window.open('', '_blank', 'width=1000,height=700');
        w.document.write('<html><head><title>' + title + '</title><style>body{font-family:sans-serif;padding:24px;}h2{color:#1A3A8F;}</style></head><body><h2>' + title + '</h2><p style="color:#64748B;font-size:12px;margin-bottom:18px;">Don Bosco Technical College Cebu — ' + new Date().toLocaleString() + '</p>' + th + '</body></html>');
        w.document.close(); w.focus(); setTimeout(function () { w.print(); }, 500);
    };
});