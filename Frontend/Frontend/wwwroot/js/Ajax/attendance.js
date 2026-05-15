function _eA(xhr) {
    var m = 'Something went wrong.';
    try {
        if (xhr.responseJSON && xhr.responseJSON.message) m = xhr.responseJSON.message;
        else if (xhr.responseText) m = xhr.responseText;
    } catch (e) { }
    if (typeof showToast === 'function') showToast(m, 'error');
}

function safeArrAtt(r) {
    if (Array.isArray(r)) return r;
    if (r && Array.isArray(r.$values)) return r.$values;
    return [];
}

function getAttendance(date, cb) {
    $.ajax({
        type: 'GET', url: '/api/proxy/AttendanceManagement/Attendance', dataType: 'json',
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (r) {
            var arr = safeArrAtt(r);
            if (date) arr = arr.filter(function (a) {
                var d = (a.date || a.attendanceDate || a.Date || '').substring(0, 10);
                return d === date;
            });
            $('#page-loader').fadeOut(200);
            if (typeof cb === 'function') cb(arr);
        },
        error: function (x) { $('#page-loader').fadeOut(200); if (typeof cb === 'function') cb([]); }
    });
}

function getAttendanceHistory(params, cb) {
    $.ajax({
        type: 'GET', url: '/api/proxy/AttendanceManagement/Attendance', dataType: 'json',
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (r) {
            var arr = safeArrAtt(r);
            if (params.from) arr = arr.filter(function (a) { return (a.date || a.attendanceDate || a.Date || '').substring(0, 10) >= params.from; });
            if (params.to) arr = arr.filter(function (a) { return (a.date || a.attendanceDate || a.Date || '').substring(0, 10) <= params.to; });
            if (params.status) arr = arr.filter(function (a) { return (a.status || a.Status || '').toLowerCase() === params.status; });
            if (params.courseId) arr = arr.filter(function (a) { return String(a.course_ID || a.courseId || '') === String(params.courseId); });
            if (params.studentId) arr = arr.filter(function (a) { return String(a.student_ID || a.studentId || '') === String(params.studentId); });
            $('#page-loader').fadeOut(200);
            if (typeof cb === 'function') cb(arr);
        },
        error: function (x) { $('#page-loader').fadeOut(200); if (typeof cb === 'function') cb([]); }
    });
}

function getTeacherAttendance(date, cb) { getAttendance(date, cb); }

function markAttendance(data, cb) {
    $.ajax({
        type: 'POST', url: '/api/proxy/AttendanceManagement/Attendance', contentType: 'application/json', dataType: 'json',
        data: JSON.stringify(data),
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (r) { $('#page-loader').fadeOut(200); showToast('Attendance saved.', 'success'); if (typeof cb === 'function') cb(r); },
        error: function (x) { $('#page-loader').fadeOut(200); _eA(x); }
    });
}

function updateAttendance(id, data, cb) {
    $.ajax({
        type: 'PUT', url: '/api/proxy/AttendanceManagement/Attendance/' + id, contentType: 'application/json', dataType: 'json',
        data: JSON.stringify(data),
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (r) { $('#page-loader').fadeOut(200); showToast('Attendance updated.', 'success'); if (typeof cb === 'function') cb(r); },
        error: function (x) { $('#page-loader').fadeOut(200); _eA(x); }
    });
}

function deleteAttendance(id, cb) {
    $.ajax({
        type: 'DELETE', url: '/api/proxy/AttendanceManagement/Attendance/' + id, dataType: 'json',
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function () { $('#page-loader').fadeOut(200); showToast('Attendance record deleted.', 'success'); if (typeof cb === 'function') cb(); },
        error: function (x) { $('#page-loader').fadeOut(200); _eA(x); }
    });
}


function computeStatus(scanTime, classStartTime) {
    if (!scanTime || !classStartTime) return 'Absent';
    var scan = new Date(scanTime);
    var start = new Date(classStartTime);
    var diffMins = (scan - start) / 60000;
    if (diffMins < -30) return 'Absent';
    if (diffMins >= -30 && diffMins < 0) return 'Present';
    if (diffMins >= 0 && diffMins <= 15) return 'Late';
    return 'Absent';
}