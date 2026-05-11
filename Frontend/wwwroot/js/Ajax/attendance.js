function _eA(xhr) {
    var m = 'Something went wrong.';
    try { if (xhr.responseJSON && xhr.responseJSON.message) m = xhr.responseJSON.message; else if (xhr.responseText) m = xhr.responseText; } catch (e) { }
    if (typeof showToast === 'function') showToast(m, 'error');
}

function getAttendance(date, cb) {
    $.ajax({
        type: 'GET', url: '/api/proxy/AttendanceManagement/Attendance', dataType: 'json',
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (r) {
            var arr = Array.isArray(r) ? r : [];
            if (date) arr = arr.filter(function (a) { var d = (a.date || a.attendanceDate || '').substring(0, 10); return d === date; });
            $('#page-loader').fadeOut(200); if (typeof cb === 'function') cb(arr);
        },
        error: function (x) { $('#page-loader').fadeOut(200); _eA(x); }
    });
}

function getAttendanceHistory(params, cb) {
    $.ajax({
        type: 'GET', url: '/api/proxy/AttendanceManagement/Attendance', dataType: 'json',
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (r) {
            var arr = Array.isArray(r) ? r : [];
            if (params.from) arr = arr.filter(function (a) { return (a.date || a.attendanceDate || '').substring(0, 10) >= params.from; });
            if (params.to) arr = arr.filter(function (a) { return (a.date || a.attendanceDate || '').substring(0, 10) <= params.to; });
            if (params.status) arr = arr.filter(function (a) { return (a.status || '').toLowerCase() === params.status; });
            $('#page-loader').fadeOut(200); if (typeof cb === 'function') cb(arr);
        },
        error: function (x) { $('#page-loader').fadeOut(200); _eA(x); }
    });
}

function getTeacherAttendance(date, cb) { getAttendance(date, cb); }

function markAttendance(data, cb) {
    $.ajax({
        type: 'POST', url: '/api/proxy/AttendanceManagement/Attendance', contentType: 'application/json', dataType: 'json', data: JSON.stringify(data),
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (r) { $('#page-loader').fadeOut(200); showToast('Attendance saved successfully.', 'success'); if (typeof cb === 'function') cb(r); },
        error: function (x) { $('#page-loader').fadeOut(200); _eA(x); }
    });
}

function updateAttendance(id, data, cb) {
    $.ajax({
        type: 'PUT', url: '/api/proxy/AttendanceManagement/Attendance/' + id, contentType: 'application/json', dataType: 'json', data: JSON.stringify(data),
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (r) { $('#page-loader').fadeOut(200); showToast('Attendance updated successfully.', 'success'); if (typeof cb === 'function') cb(r); },
        error: function (x) { $('#page-loader').fadeOut(200); _eA(x); }
    });
}