/* jquery/pagination.js */
$(function () {
    window.paginate = function (opts) {
        var data = opts.data || [], ps = opts.pageSize || 10, $body = $('#' + opts.containerId), $pag = $('#' + opts.paginationId), rr = opts.renderRow, em = opts.emptyMsg || 'No records.', cs = opts.colSpan || 7, cp = 1;
        function tp() { return Math.max(1, Math.ceil(data.length / ps)); }
        function render() {
            $body.empty();
            if (!data.length) { $body.append('<tr><td colspan="' + cs + '" class="text-center text-muted py-4"><i class="bi bi-inbox" style="font-size:2rem;display:block;opacity:.3;margin-bottom:6px;"></i>' + em + '</td></tr>'); $pag.empty(); return; }
            var st = (cp - 1) * ps; data.slice(st, st + ps).forEach(function (item, i) { $body.append(rr(item, st + i)); });
            var t = tp(); if (t <= 1) { $pag.empty(); return; }
            var h = '<nav><ul class="pagination pagination-sm mb-0">';
            h += '<li class="page-item' + (cp === 1 ? ' disabled' : '') + '"><a class="page-link" href="#" data-page="' + (cp - 1) + '"><i class="bi bi-chevron-left"></i></a></li>';
            for (var p = 1; p <= t; p++) {
                if (t > 7 && p > 2 && p < cp - 1) { if (p === 3) h += '<li class="page-item disabled"><span class="page-link">&hellip;</span></li>'; continue; }
                if (t > 7 && p > cp + 1 && p < t - 1) { if (p === cp + 2) h += '<li class="page-item disabled"><span class="page-link">&hellip;</span></li>'; continue; }
                h += '<li class="page-item' + (p === cp ? ' active' : '') + '"><a class="page-link" href="#" data-page="' + p + '">' + p + '</a></li>';
            }
            h += '<li class="page-item' + (cp === t ? ' disabled' : '') + '"><a class="page-link" href="#" data-page="' + (cp + 1) + '"><i class="bi bi-chevron-right"></i></a></li></ul></nav>';
            $pag.html(h);
        }
        $pag.on('click', 'a.page-link', function (e) { e.preventDefault(); var p = parseInt($(this).data('page')); if (isNaN(p) || p < 1 || p > tp()) return; cp = p; render(); });
        render();
        return { refresh: function (nd) { if (nd !== undefined) data = nd; cp = 1; render(); } };
    };
});