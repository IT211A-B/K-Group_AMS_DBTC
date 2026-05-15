function safeArray(r) {
    if (Array.isArray(r)) return r;
    if (r && Array.isArray(r.$values)) return r.$values;
    return [];
}

function getDashboardStats() {
    $.ajax({
        type: 'GET', url: '/api/Dashboard/stats', dataType: 'json', global: false,
        success: function (stats) {
            if (!stats) return;
            $('#statStudents, #histStudents, #ovStudents').text(stats.totalStudents ?? stats.TotalStudents ?? '0');
            $('#statTeachers, #histTeachers, #ovTeachers').text(stats.totalTeachers ?? stats.TotalTeachers ?? '0');
            $('#statCourses, #histCourses, #ovCourses').text(stats.totalCourses ?? stats.TotalCourses ?? '0');
            $('#statAbsences, #ovAbsences').text(stats.absencesToday ?? stats.AbsencesToday ?? '0');
        },
        error: function () {
            $.ajax({ type: 'GET', url: '/api/Student', dataType: 'json', global: false,
                success: function (r) { $('#statStudents, #histStudents').text(safeArray(r).length); } });
            $.ajax({ type: 'GET', url: '/api/Teacher', dataType: 'json', global: false,
                success: function (r) { $('#statTeachers, #histTeachers').text(safeArray(r).length); } });
            $.ajax({ type: 'GET', url: '/AttendanceManagement/Course', dataType: 'json', global: false,
                success: function (r) { $('#statCourses, #histCourses').text(safeArray(r).length); } });
        }
    });
}
