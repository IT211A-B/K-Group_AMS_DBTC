function _eC(xhr) {
    var m = 'Something went wrong.';
    try { if (xhr.responseJSON && xhr.responseJSON.message) m = xhr.responseJSON.message; else if (xhr.responseText) m = xhr.responseText; } catch (e) { }
    if (typeof showToast === 'function') showToast(m, 'error');
}

function getCourses(cb) {
    $.ajax({
        type: 'GET', url: '/api/proxy/AttendanceManagement/Course', dataType: 'json',
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (r) { $('#page-loader').fadeOut(200); if (typeof cb === 'function') cb(Array.isArray(r) ? r : []); },
        error: function (x) { $('#page-loader').fadeOut(200); _eC(x); }
    });
}

function addCourse(data, cb) {
    $.ajax({
        type: 'POST', url: '/api/proxy/AttendanceManagement/Course', contentType: 'application/json', dataType: 'json', data: JSON.stringify(data),
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (r) { $('#page-loader').fadeOut(200); showToast('Course added successfully.', 'success'); if (typeof cb === 'function') cb(r); },
        error: function (x) { $('#page-loader').fadeOut(200); _eC(x); }
    });
}

function updateCourse(id, data, cb) {
    $.ajax({
        type: 'PUT', url: '/api/proxy/AttendanceManagement/Course/' + id, contentType: 'application/json', dataType: 'json', data: JSON.stringify(data),
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
        success: function (r) { if (typeof cb === 'function') cb(Array.isArray(r) ? r : []); },
        error: function () { if (typeof cb === 'function') cb([]); }
    });
}

function getPrograms(cb) {
    $.ajax({
        type: 'GET', url: '/api/proxy/AttendanceManagement/Program', dataType: 'json',
        success: function (r) { if (typeof cb === 'function') cb(Array.isArray(r) ? r : []); },
        error: function () { if (typeof cb === 'function') cb([]); }
    });
}

function getSchedules(cb) {
    $.ajax({
        type: 'GET', url: '/api/proxy/AttendanceManagement/Schedule', dataType: 'json',
        success: function (r) { if (typeof cb === 'function') cb(Array.isArray(r) ? r : []); },
        error: function () { if (typeof cb === 'function') cb([]); }
    });
}

function getEnrollments(cb) {
    $.ajax({
        type: 'GET', url: '/api/proxy/AttendanceManagement/Enrollment', dataType: 'json',
        success: function (r) { if (typeof cb === 'function') cb(Array.isArray(r) ? r : []); },
        error: function () { if (typeof cb === 'function') cb([]); }
    });
}