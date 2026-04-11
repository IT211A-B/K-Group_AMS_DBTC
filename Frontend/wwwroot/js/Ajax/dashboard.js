function getDashboardStats() {
    $.ajax({
        type: 'GET', url: '/api/User', dataType: 'json',
        success: function (r) {
            var arr = Array.isArray(r) ? r : [];
        }
    });
    $.ajax({
        type: 'GET', url: '/api/Student', dataType: 'json',
        success: function (r) { $('#statStudents').text(Array.isArray(r) ? r.length : '0'); },
        error: function () { $('#statStudents').text('0'); }
    });
    $.ajax({
        type: 'GET', url: '/api/Teacher', dataType: 'json',
        success: function (r) { $('#statTeachers').text(Array.isArray(r) ? r.length : '0'); },
        error: function () { $('#statTeachers').text('0'); }
    });
    $.ajax({
        type: 'GET', url: '/AttendanceManagement/Course', dataType: 'json',
        success: function (r) { $('#statCourses').text(Array.isArray(r) ? r.length : '0'); },
        error: function () { $('#statCourses').text('0'); }
    });
    $.ajax({
        type: 'GET', url: '/AttendanceManagement/Attendance', dataType: 'json',
        success: function (r) {
            var arr = Array.isArray(r) ? r : [];
            var today = new Date().toISOString().slice(0, 10);
            var todayArr = arr.filter(function (a) { return (a.date || a.attendanceDate || '').substring(0, 10) === today; });
            $('#statAbsences').text(todayArr.filter(function (a) { return (a.status || '').toLowerCase() === 'absent'; }).length);
        },
        error: function () { $('#statAbsences').text('0'); }
    });
}