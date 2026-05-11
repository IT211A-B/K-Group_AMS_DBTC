function safeArray(r) {
    if (Array.isArray(r)) return r;
    if (r && Array.isArray(r.$values)) return r.$values;
    return [];
}

function getDashboardStats() {
    $.ajax({
        type: 'GET', url: '/api/proxy/api/Student', dataType: 'json', global: false,
        success: function (r) { $('#statStudents, #histStudents, #ovStudents').text(safeArray(r).length); },
        error: function () { $('#statStudents, #histStudents, #ovStudents').text('0'); }
    });
    $.ajax({
        type: 'GET', url: '/api/proxy/api/Teacher', dataType: 'json', global: false,
        success: function (r) { $('#statTeachers, #histTeachers, #ovTeachers').text(safeArray(r).length); },
        error: function () { $('#statTeachers, #histTeachers, #ovTeachers').text('0'); }
    });
    $.ajax({
        type: 'GET', url: '/api/proxy/AttendanceManagement/Course', dataType: 'json', global: false,
        success: function (r) { $('#statCourses, #histCourses, #ovCourses').text(safeArray(r).length); },
        error: function () { $('#statCourses, #histCourses, #ovCourses').text('0'); }
    });
    $.ajax({
        type: 'GET', url: '/api/proxy/AttendanceManagement/Attendance', dataType: 'json', global: false,
        success: function (r) {
            var arr = safeArray(r);
            var today = new Date().toISOString().slice(0, 10);
            var todayArr = arr.filter(function (a) {
                return (a.date || a.attendanceDate || '').substring(0, 10) === today;
            });
            var absences = todayArr.filter(function (a) { return (a.status || '').toLowerCase() === 'absent'; }).length;
            var present = todayArr.filter(function (a) { return (a.status || '').toLowerCase() === 'present'; }).length;
            var late = todayArr.filter(function (a) { return (a.status || '').toLowerCase() === 'late'; }).length;
            $('#statAbsences, #ovAbsences').text(absences);
            $('#sumPresent').text(present);
            $('#sumAbsent').text(absences);
            $('#sumLate').text(late);
        },
        error: function () { $('#statAbsences, #ovAbsences').text('0'); }
    });
}