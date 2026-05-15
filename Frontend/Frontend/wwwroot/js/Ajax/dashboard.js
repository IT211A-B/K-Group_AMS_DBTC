function getDashboardStats() {
    $.ajax({
        url: '/api/proxy/api/DashboardStats',
        dataType: 'json',
        global: false,
        success: function (stats) {
            if (!stats) return;
            $('#statStudents,#histStudents,#ovStudents').text(stats.students ?? '—');
            $('#statTeachers,#histTeachers,#ovTeachers').text(stats.teachers ?? '—');
            $('#statCourses,#histCourses,#ovCourses').text(stats.courses ?? '—');
            $('#statAbsences,#ovAbsences').text(stats.absencesToday ?? '—');
            $('#statAttendance,#histAttendance').text(stats.attendanceRecords ?? '—');
            $('#statQrCodes,#histQrCodes').text(stats.qrCodes ?? '—');
            $('#statNotifications,#histNotifications').text(stats.notifications ?? '—');
            $('#statSchedules,#histSchedules').text(stats.schedules ?? '—');
        },
        error: function () {
            $.when(
                ajaxGetArray('/api/proxy/api/Student'),
                ajaxGetArray('/api/proxy/api/Teacher'),
                ajaxGetArray('/api/proxy/AttendanceManagement/Course'),
                ajaxGetArray('/api/proxy/AttendanceManagement/Attendance'),
                ajaxGetArray('/api/proxy/AttendanceManagement/Schedule')
            ).done(function (students, teachers, courses, attendances, schedules) {
                $('#statStudents,#histStudents,#ovStudents').text(students.length);
                $('#statTeachers,#histTeachers,#ovTeachers').text(teachers.length);
                $('#statCourses,#histCourses,#ovCourses').text(courses.length);
                $('#statAttendance,#histAttendance').text(attendances.length);
                $('#statQrCodes,#histQrCodes').text(students.length);
                $('#statSchedules,#histSchedules').text(schedules.length);

                var today = new Date().toISOString().slice(0, 10);
                var todayArr = attendances.filter(function (a) {
                    return (a.date || a.attendanceDate || a.Date || '').toString().substring(0, 10) === today;
                });
                var absences = todayArr.filter(function (a) { return (a.status || '').toLowerCase() === 'absent'; }).length;
                var present = todayArr.filter(function (a) { return (a.status || '').toLowerCase() === 'present'; }).length;
                var late = todayArr.filter(function (a) { return (a.status || '').toLowerCase() === 'late'; }).length;
                $('#statAbsences,#ovAbsences').text(absences);
                $('#sumPresent').text(present);
                $('#sumAbsent').text(absences);
                $('#sumLate').text(late);
            });
        }
    });
}
