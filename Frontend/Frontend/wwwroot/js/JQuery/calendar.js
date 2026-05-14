/* jquery/calendar.js */
$(function () {
    window.renderMiniCalendar = function (cid, year, month, records) {
        var $c = $('#' + cid); if (!$c.length) return;
        records = records || {};
        var today = new Date(), months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
        var y = (year !== undefined) ? year : today.getFullYear(), m = (month !== undefined) ? month : today.getMonth();
        var fd = new Date(y, m, 1).getDay(), d = new Date(y, m + 1, 0).getDate();
        var h = '<div class="d-flex justify-content-between align-items-center mb-2"><button class="btn btn-sm btn-link p-0 text-primary text-decoration-none" id="calPrev"><i class="bi bi-chevron-left"></i></button><strong style="font-size:13px;">' + months[m] + ' ' + y + '</strong><button class="btn btn-sm btn-link p-0 text-primary text-decoration-none" id="calNext"><i class="bi bi-chevron-right"></i></button></div>';
        h += '<table class="mini-calendar w-100"><thead><tr><th>Su</th><th>Mo</th><th>Tu</th><th>We</th><th>Th</th><th>Fr</th><th>Sa</th></tr></thead><tbody><tr>';
        var col = 0;
        for (var i = 0; i < fd; i++) { h += '<td></td>'; col++; }
        for (var dd = 1; dd <= d; dd++) {
            var key = y + '-' + (('0' + (m + 1)).slice(-2)) + '-' + (('0' + dd).slice(-2));
            var cls = '', rec = records[key] || '';
            var isToday = (y === today.getFullYear() && m === today.getMonth() && dd === today.getDate());
            if (isToday) cls = 'today';
            else if (rec === 'present') cls = 'day-present';
            else if (rec === 'absent') cls = 'day-absent';
            else if (rec === 'late') cls = 'day-late';
            h += '<td class="' + cls + '">' + dd + '</td>'; col++; if (col % 7 === 0 && dd < d) h += '</tr><tr>';
        }
        while (col % 7 !== 0) { h += '<td></td>'; col++; }
        h += '</tr></tbody></table>';
        h += '<div class="cal-legend mt-2"><span><span class="dot dot-present"></span>Present</span><span><span class="dot dot-absent"></span>Absent</span><span><span class="dot dot-late"></span>Late</span><span><span class="dot dot-today"></span>Today</span></div>';
        $c.html(h);
        $c.find('#calPrev').on('click', function () { var nm = m - 1, ny = y; if (nm < 0) { nm = 11; ny--; } renderMiniCalendar(cid, ny, nm, records); });
        $c.find('#calNext').on('click', function () { var nm = m + 1, ny = y; if (nm > 11) { nm = 0; ny++; } renderMiniCalendar(cid, ny, nm, records); });
    };
    if ($('#miniCalendar').length) renderMiniCalendar('miniCalendar');
});