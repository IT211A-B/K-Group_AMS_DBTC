function _eC(xhr) {
    var m = 'Something went wrong.';
    try {
        if (xhr.responseJSON) {
            if (xhr.responseJSON.message) m = xhr.responseJSON.message;
            else if (xhr.responseJSON.title) m = xhr.responseJSON.title;
            else if (xhr.responseJSON.errors) {
                var errs = xhr.responseJSON.errors, msgs = [];
                Object.keys(errs).forEach(function (k) { if (Array.isArray(errs[k])) msgs = msgs.concat(errs[k]); });
                if (msgs.length) m = msgs.join(' ');
            }
        } else if (xhr.responseText) m = xhr.responseText;
    } catch (e) { }
    if (typeof showToast === 'function') showToast(m, 'error');
}

function safeArrC(r) {
    if (Array.isArray(r)) return r;
    if (r && Array.isArray(r.$values)) return r.$values;
    return [];
}

function getCourses(cb) {
    $.ajax({
        type: 'GET', url: '/api/proxy/AttendanceManagement/Course', dataType: 'json',
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (r) { $('#page-loader').fadeOut(200); if (typeof cb === 'function') cb(safeArrC(r)); },
        error: function () { $('#page-loader').fadeOut(200); if (typeof cb === 'function') cb([]); }
    });
}

function addCourse(data, cb) {
    if (data.teacher_ID === undefined || data.teacher_ID === null || data.teacher_ID === '') {
        data.teacher_ID = 0;
    }
    $.ajax({
        type: 'POST', url: '/api/proxy/AttendanceManagement/Course',
        contentType: 'application/json', dataType: 'json',
        data: JSON.stringify(data),
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (r) { $('#page-loader').fadeOut(200); showToast('Course added successfully.', 'success'); if (typeof cb === 'function') cb(r); },
        error: function (x) { $('#page-loader').fadeOut(200); _eC(x); }
    });
}

function updateCourse(id, data, cb) {
    if (data.teacher_ID === undefined || data.teacher_ID === null || data.teacher_ID === '') {
        data.teacher_ID = 0;
    }
    $.ajax({
        type: 'PUT', url: '/api/proxy/AttendanceManagement/Course/' + id,
        contentType: 'application/json', dataType: 'json',
        data: JSON.stringify(data),
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (r) { $('#page-loader').fadeOut(200); showToast('Course updated successfully.', 'success'); if (typeof cb === 'function') cb(r); },
        error: function (x) { $('#page-loader').fadeOut(200); _eC(x); }
    });
}

function deleteCourse(id, cb) {
    $.ajax({
        type: 'DELETE', url: '/api/proxy/AttendanceManagement/Course/' + id, dataType: 'json',
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function () { $('#page-loader').fadeOut(200); showToast('Course deleted successfully.', 'success'); if (typeof cb === 'function') cb(); },
        error: function (x) { $('#page-loader').fadeOut(200); _eC(x); }
    });
}

function getDepartments(cb) {
    $.ajax({
        type: 'GET', url: '/api/proxy/AttendanceManagement/Department', dataType: 'json',
        success: function (r) { if (typeof cb === 'function') cb(safeArrC(r)); },
        error: function () { if (typeof cb === 'function') cb([]); }
    });
}

function getPrograms(cb) {
    $.ajax({
        type: 'GET', url: '/api/proxy/AttendanceManagement/Program', dataType: 'json',
        success: function (r) { if (typeof cb === 'function') cb(safeArrC(r)); },
        error: function () { if (typeof cb === 'function') cb([]); }
    });
}

function getSchedules(cb) {
    $.ajax({
        type: 'GET', url: '/api/proxy/AttendanceManagement/Schedule', dataType: 'json',
        success: function (r) { if (typeof cb === 'function') cb(safeArrC(r)); },
        error: function () { if (typeof cb === 'function') cb([]); }
    });
}

function addSchedule(data, cb) {
    $.ajax({
        type: 'POST', url: '/api/proxy/AttendanceManagement/Schedule',
        contentType: 'application/json', dataType: 'json',
        data: JSON.stringify(data),
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (r) { $('#page-loader').fadeOut(200); showToast('Schedule added successfully.', 'success'); if (typeof cb === 'function') cb(r); },
        error: function (x) { $('#page-loader').fadeOut(200); _eC(x); }
    });
}

function updateSchedule(id, data, cb) {
    $.ajax({
        type: 'PUT', url: '/api/proxy/AttendanceManagement/Schedule/' + id,
        contentType: 'application/json', dataType: 'json',
        data: JSON.stringify(data),
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (r) { $('#page-loader').fadeOut(200); showToast('Schedule updated successfully.', 'success'); if (typeof cb === 'function') cb(r); },
        error: function (x) { $('#page-loader').fadeOut(200); _eC(x); }
    });
}

function deleteSchedule(id, cb) {
    $.ajax({
        type: 'DELETE', url: '/api/proxy/AttendanceManagement/Schedule/' + id, dataType: 'json',
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function () { $('#page-loader').fadeOut(200); showToast('Schedule deleted.', 'success'); if (typeof cb === 'function') cb(); },
        error: function (x) { $('#page-loader').fadeOut(200); _eC(x); }
    });
}

function getEnrollments(cb) {
    $.ajax({
        type: 'GET', url: '/api/proxy/AttendanceManagement/Enrollment', dataType: 'json',
        success: function (r) { if (typeof cb === 'function') cb(safeArrC(r)); },
        error: function () { if (typeof cb === 'function') cb([]); }
    });
}

function addEnrollment(data, cb) {
    $.ajax({
        type: 'POST', url: '/api/proxy/AttendanceManagement/Enrollment',
        contentType: 'application/json', dataType: 'json',
        data: JSON.stringify(data),
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (r) { $('#page-loader').fadeOut(200); showToast('Enrollment added.', 'success'); if (typeof cb === 'function') cb(r); },
        error: function (x) {
            $('#page-loader').fadeOut(200);
            if (x.status === 404) showToast('Enrollment endpoint not yet available on backend.', 'warning');
            else _eC(x);
        }
    });
}

function deleteEnrollment(id, cb) {
    $.ajax({
        type: 'DELETE', url: '/api/proxy/AttendanceManagement/Enrollment/' + id, dataType: 'json',
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function () { $('#page-loader').fadeOut(200); showToast('Enrollment removed.', 'success'); if (typeof cb === 'function') cb(); },
        error: function (x) {
            $('#page-loader').fadeOut(200);
            if (x.status === 404) showToast('Enrollment endpoint not yet available on backend.', 'warning');
            else _eC(x);
        }
    });
}

function getSections(cb) {
    $.ajax({
        type: 'GET', url: '/api/proxy/AttendanceManagement/Section', dataType: 'json',
        success: function (r) { if (typeof cb === 'function') cb(safeArrC(r)); },
        error: function () { if (typeof cb === 'function') cb([]); }
    });
}