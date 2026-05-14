function safeArray(r) {
    if (Array.isArray(r)) return r;
    if (r && Array.isArray(r.$values)) return r.$values;
    return [];
}

function ajaxGet(url) {
    return $.ajax({ type: 'GET', url: url, dataType: 'json' }).then(
        function (r) { return safeArray(r); },
        function () { return []; }
    );
}

function getDashboardStats() {
    $.when(
        ajaxGet('/api/proxy/api/Student'),
        ajaxGet('/api/proxy/api/Teacher'),
        ajaxGet('/api/proxy/AttendanceManagement/Course'),
        ajaxGet('/api/proxy/AttendanceManagement/Attendance')
    ).done(function (students, teachers, courses, attendances) {
        $('#statStudents, #histStudents, #ovStudents').text(students.length);
        $('#statTeachers, #histTeachers, #ovTeachers').text(teachers.length);
        $('#statCourses, #histCourses, #ovCourses').text(courses.length);

        var today = new Date().toISOString().slice(0, 10);
        var todayArr = attendances.filter(function (a) {
            return (a.date || a.attendanceDate || a.Date || '').substring(0, 10) === today;
        });
        var absences = todayArr.filter(function (a) { return (a.status || a.Status || '').toLowerCase() === 'absent'; }).length;
        var present = todayArr.filter(function (a) { return (a.status || a.Status || '').toLowerCase() === 'present'; }).length;
        var late = todayArr.filter(function (a) { return (a.status || a.Status || '').toLowerCase() === 'late'; }).length;
        $('#statAbsences, #ovAbsences').text(absences);
        $('#sumPresent').text(present);
        $('#sumAbsent').text(absences);
        $('#sumLate').text(late);
    });
}