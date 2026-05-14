var STUDENT_GID = 3, TEACHER_GID = 2, ADMIN_GID = 1;
var allAccounts = [], allStudents = [], allTeachers = [], allUsers = [];
var allPrograms = [], allDepts = [], allCourses = [], allEnrollments = [];
var pg;

function safeArr(r) {
    if (Array.isArray(r)) return r;
    if (r && Array.isArray(r.$values)) return r.$values;
    return [];
}

function ajaxSafe(url) {
    return $.ajax({ type: 'GET', url: url, dataType: 'json', global: false }).then(
        function (r) { return safeArr(r); },
        function () { return []; }
    );
}

function roleLabel(gid) {
    if (gid == TEACHER_GID) return '<span style="background:#EDE9FE;color:#6D28D9;font-size:11px;padding:2px 8px;border-radius:20px;font-weight:600;">Teacher</span>';
    if (gid == STUDENT_GID) return '<span style="background:#DBEAFE;color:var(--accent);font-size:11px;padding:2px 8px;border-radius:20px;font-weight:600;">Student</span>';
    return '<span style="background:#FEE2E2;color:#DC2626;font-size:11px;padding:2px 8px;border-radius:20px;font-weight:600;">Admin</span>';
}

function roleColor(gid) {
    if (gid == TEACHER_GID) return '#7C3AED';
    if (gid == STUDENT_GID) return 'var(--accent)';
    return '#DC2626';
}

function renderRow(a, i) {
    var init = (a._name[0] || '?').toUpperCase();
    var roleBadge = roleLabel(a._ugid);
    var actions = '';
    if (a._ugid === ADMIN_GID) {
        actions = '<span class="text-muted small">Admin</span>';
    } else {
        var profileUrl = a._ugid === STUDENT_GID
            ? '/Admin/ViewStudentProfile?userId=' + a._uid
            : '/Admin/ViewTeacherProfile?userId=' + a._uid;
        actions =
            '<button class="btn btn-sm btn-outline-primary btn-icon edit-btn me-1" ' +
            'data-sid="' + (a._sid || '') + '" data-tid="' + (a._tid || '') + '" ' +
            'data-uid="' + a._uid + '" data-ugid="' + a._ugid + '" title="Edit">' +
            '<i class="bi bi-pencil"></i></button>' +
            '<a href="' + profileUrl + '" class="btn btn-sm btn-outline-info btn-icon me-1" title="View Profile">' +
            '<i class="bi bi-person-lines-fill"></i></a>' +
            '<button class="btn btn-sm btn-outline-danger btn-icon delete-btn" ' +
            'data-sid="' + (a._sid || '') + '" data-tid="' + (a._tid || '') + '" ' +
            'data-uid="' + a._uid + '" data-ugid="' + a._ugid + '" data-name="' + a._name + '" title="Delete">' +
            '<i class="bi bi-trash"></i></button>';
    }
    var dept = a._dept || '—';
    return '<tr>' +
        '<td>' + (i + 1) + '</td>' +
        '<td><div class="d-flex align-items-center gap-2">' +
        '<div style="width:32px;height:32px;border-radius:50%;background:' + roleColor(a._ugid) +
        ';color:#fff;display:flex;align-items:center;justify-content:center;font-weight:600;font-size:12px;">' +
        init + '</div>' + a._name + '</div></td>' +
        '<td>' + a._email + '</td>' +
        '<td>' + roleBadge + '</td>' +
        '<td>' + (a._gender === 'M' ? 'Male' : a._gender === 'F' ? 'Female' : (a._gender || '—')) + '</td>' +
        '<td>' + dept + '</td>' +
        '<td>' + (a._courses || 0) + '</td>' +
        '<td class="d-flex gap-1 flex-wrap">' + actions + '</td>' +
        '</tr>';
}

function applyFilter() {
    var q = $('#searchStudent').val().toLowerCase();
    var role = $('#filterRole').val();
    var sort = $('#sortBy').val();
    var result = allAccounts.filter(function (a) {
        if (role === 'teacher' && a._ugid !== TEACHER_GID) return false;
        if (role === 'student' && a._ugid !== STUDENT_GID) return false;
        if (q && !a._name.toLowerCase().includes(q) && !a._email.toLowerCase().includes(q)) return false;
        return true;
    });
    if (sort === 'name-asc') result.sort(function (a, b) { return a._name.localeCompare(b._name); });
    if (sort === 'name-desc') result.sort(function (a, b) { return b._name.localeCompare(a._name); });
    if (pg) pg.refresh(result);
}

function buildAccounts() {
    allAccounts = allUsers.map(function (u) {
        var uid = u.user_ID;
        var gid = u.userGroup_ID;
        var student = gid == STUDENT_GID ? allStudents.find(function (s) { return String(s.user_ID) === String(uid); }) : null;
        var teacher = gid == TEACHER_GID ? allTeachers.find(function (t) { return String(t.user_ID) === String(uid); }) : null;
        var prog = student ? allPrograms.find(function (p) { return String(p.program_Id) === String(student.program_ID); }) : null;
        var dept = student
            ? (prog ? prog.name : (student.program_ID ? 'Program #' + student.program_ID : '—'))
            : teacher
                ? (teacher.department || '—')
                : '—';
        var sid = student ? (student.student_ID || student.id || '') : '';
        var tid = teacher ? (teacher.teacher_ID || teacher.id || '') : '';
        var courseCount = allEnrollments.filter(function (e) { return String(e.student_ID) === String(sid); }).length;
        return {
            _uid: uid, _ugid: gid,
            _name: u.full_Name || u.email || '?',
            _email: u.email || '—',
            _gender: u.gender || '',
            _dept: dept,
            _sid: sid, _tid: tid,
            _courses: courseCount,
            _created: u.createdAt || ''
        };
    });
}

function loadAll() {
    $('#page-loader').fadeIn(150);
    $.when(
        ajaxSafe('/api/proxy/api/User'),
        ajaxSafe('/api/proxy/api/Student'),
        ajaxSafe('/api/proxy/api/Teacher'),
        ajaxSafe('/api/proxy/AttendanceManagement/Program'),
        ajaxSafe('/api/proxy/AttendanceManagement/Department'),
        ajaxSafe('/api/proxy/AttendanceManagement/Course'),
        ajaxSafe('/api/proxy/AttendanceManagement/Enrollment')
    ).done(function (users, students, teachers, programs, depts, courses, enrollments) {
        allUsers = users;
        allStudents = students;
        allTeachers = teachers;
        allPrograms = programs;
        allDepts = depts;
        allCourses = courses;
        allEnrollments = enrollments;

        buildAccounts();

        $('#statStudents').text(allStudents.length);
        $('#statTeachers').text(allTeachers.length);
        $('#statCourses').text(allCourses.length);

        var progHtml = '<option value="">Select Program</option>';
        allPrograms.forEach(function (p) {
            progHtml += '<option value="' + p.program_Id + '">' + (p.name || p.programName || 'Program ' + p.program_Id) + '</option>';
        });
        $('#addProgramId, #editProgramId').html(progHtml);

        var deptHtml = '<option value="">Select Department</option>';
        allDepts.forEach(function (d) {
            deptHtml += '<option value="' + d.department_Id + '">' + (d.name || d.departmentName || 'Dept ' + d.department_Id) + '</option>';
        });
        $('#addDeptId, #editDeptId').html(deptHtml);

        var courseCheckHtml = '';
        allCourses.forEach(function (c) {
            var cid = c.course_ID || c.id;
            courseCheckHtml += '<div class="form-check"><input class="form-check-input course-check" type="checkbox" value="' + cid + '" id="cc' + cid + '"><label class="form-check-label" for="cc' + cid + '">' + (c.code || '') + ' — ' + (c.title || c.courseName || '') + '</label></div>';
        });
        $('#addStudentCourses').html(courseCheckHtml || '<span class="text-muted small">No courses available</span>');

        var tcHtml = '';
        allCourses.forEach(function (c) {
            var cid = c.course_ID || c.id;
            tcHtml += '<div class="form-check"><input class="form-check-input teacher-course-check" type="checkbox" value="' + cid + '" id="tc' + cid + '"><label class="form-check-label" for="tc' + cid + '">' + (c.code || '') + ' — ' + (c.title || c.courseName || '') + '</label></div>';
        });
        $('#addTeacherCourses').html(tcHtml || '<span class="text-muted small">No courses available</span>');

        if (pg) {
            pg.refresh(allAccounts);
        } else {
            pg = paginate({
                data: allAccounts, pageSize: 15,
                containerId: 'studentTableBody',
                paginationId: 'studentPagination',
                colSpan: 8, emptyMsg: 'No accounts found.',
                renderRow: renderRow
            });
        }
        $('#page-loader').fadeOut(200);
    });
}

$(function () {
    $('input[name="acctType"]').on('change', function () {
        if ($(this).val() === 'student') {
            $('#studentFields').show(); $('#teacherFields').hide();
        } else {
            $('#studentFields').hide(); $('#teacherFields').show();
        }
    });

    $('#addEmail').on('input', function () {
        var email = $(this).val().toLowerCase();
        var hint = $('#addEmailHint');
        if (email.includes('@admin')) hint.html('<span class="text-danger">⚠ Admin email — use teacher/student domains</span>');
        else if (email.includes('@local')) hint.html('<span class="text-primary">Teacher account</span>');
        else if (email.includes('@dbtc-cebu')) hint.html('<span class="text-success">Student account</span>');
        else hint.html('');
    });

    $('#saveAccountBtn').on('click', function () {
        var type = $('input[name="acctType"]:checked').val();
        var fullName = $('#addFullName').val().trim();
        var email = $('#addEmail').val().trim();
        var password = $('#addPassword').val();
        var phone = $('#addPhone').val().trim();
        var gender = $('#addGender').val();
        var birthDate = $('#addBirthDate').val();
        var address = $('#addAddress').val().trim();

        if (!fullName || !email || !password) {
            showToast('Full Name, Email, and Password are required.', 'error'); return;
        }

        var ugid = type === 'teacher' ? TEACHER_GID : STUDENT_GID;
        var emailDomain = type === 'teacher' ? '@local' : '@dbtc-cebu';
        if (type === 'student' && !email.includes('@dbtc-cebu')) {
            showToast('Student email must include @dbtc-cebu', 'error'); return;
        }
        if (type === 'teacher' && !email.includes('@local')) {
            showToast('Teacher email must include @local', 'error'); return;
        }

        var userData = {
            full_Name: fullName, email: email, password: password,
            phone_Number: phone, gender: gender, birth_Date: birthDate || null,
            address: address, userGroup_ID: ugid, lastUpdatedBy: 'admin'
        };

        if (type === 'student') {
            var progId = $('#addProgramId').val();
            var deptId = $('#addDeptId').val();
            var yearLevel = $('#addYearLevel').val();
            var studentData = {
                program_ID: progId ? parseInt(progId) : null,
                department_ID: deptId ? parseInt(deptId) : null,
                year_Level: yearLevel ? parseInt(yearLevel) : 1,
                lastUpdatedBy: 'admin'
            };
            addStudent(userData, studentData, function () {
                $('#addAccountModal').modal('hide');
                loadAll();
            });
        } else {
            var dept = $('#addTeacherDept').val().trim();
            var teacherData = { department: dept, lastUpdatedBy: 'admin' };
            addTeacher(userData, teacherData, function () {
                $('#addAccountModal').modal('hide');
                loadAll();
            });
        }
    });

    // Edit button
    $(document).on('click', '.edit-btn', function () {
        var uid = $(this).data('uid');
        var ugid = parseInt($(this).data('ugid'));
        var sid = $(this).data('sid');
        var tid = $(this).data('tid');
        var user = allUsers.find(function (u) { return String(u.user_ID) === String(uid); });
        if (!user) { showToast('User not found.', 'error'); return; }

        $('#editUserId').val(uid);
        $('#editUgid').val(ugid);
        $('#editStudentId').val(sid || '');
        $('#editTeacherId').val(tid || '');
        $('#editFullName').val(user.full_Name || '');
        $('#editEmail').val(user.email || '');
        $('#editPhone').val(user.phone_Number || '');
        $('#editGender').val(user.gender || '');
        $('#editBirthDate').val(user.birth_Date ? user.birth_Date.substring(0, 10) : '');
        $('#editAddress').val(user.address || '');

        if (ugid === STUDENT_GID) {
            $('#editStudentRow').show(); $('#editTeacherRow').hide();
            var student = allStudents.find(function (s) { return String(s.user_ID) === String(uid); });
            if (student) {
                $('#editProgramId').val(student.program_ID || '');
                $('#editDeptId').val(student.department_ID || '');
                $('#editYearLevel').val(student.year_Level || '');
            }
        } else {
            $('#editStudentRow').hide(); $('#editTeacherRow').show();
            var teacher = allTeachers.find(function (t) { return String(t.user_ID) === String(uid); });
            if (teacher) $('#editTeacherDept').val(teacher.department || '');
        }
        $('#editAccountModal').modal('show');
    });

    $('#updateAccountBtn').on('click', function () {
        var uid = $('#editUserId').val();
        var ugid = parseInt($('#editUgid').val());
        var sid = $('#editStudentId').val();
        var tid = $('#editTeacherId').val();
        var fullName = $('#editFullName').val().trim();
        var email = $('#editEmail').val().trim();
        var phone = $('#editPhone').val().trim();
        var gender = $('#editGender').val();
        var birthDate = $('#editBirthDate').val();
        var address = $('#editAddress').val().trim();

        if (!fullName || !email) { showToast('Name and email are required.', 'error'); return; }

        var userData = {
            full_Name: fullName, email: email, phone_Number: phone,
            gender: gender, birth_Date: birthDate || null, address: address,
            userGroup_ID: ugid, lastUpdatedBy: 'admin'
        };

        if (ugid === STUDENT_GID) {
            var studentData = {
                program_ID: parseInt($('#editProgramId').val()) || null,
                department_ID: parseInt($('#editDeptId').val()) || null,
                year_Level: parseInt($('#editYearLevel').val()) || 1,
                lastUpdatedBy: 'admin'
            };
            updateStudent(uid, userData, sid, studentData, function () {
                $('#editAccountModal').modal('hide'); loadAll();
            });
        } else {
            var teacherData = { department: $('#editTeacherDept').val().trim(), lastUpdatedBy: 'admin' };
            updateTeacher(uid, userData, tid, teacherData, function () {
                $('#editAccountModal').modal('hide'); loadAll();
            });
        }
    });

    // Delete button
    $(document).on('click', '.delete-btn', function () {
        var uid = $(this).data('uid');
        var ugid = parseInt($(this).data('ugid'));
        var sid = $(this).data('sid');
        var tid = $(this).data('tid');
        var name = $(this).data('name');
        if (!confirm('Delete ' + name + '? This cannot be undone.')) return;
        if (ugid === STUDENT_GID) {
            deleteStudent(uid, sid, function () { loadAll(); });
        } else {
            deleteTeacher(uid, tid, function () { loadAll(); });
        }
    });

    $('#searchStudent, #filterRole, #sortBy').on('input change', applyFilter);

    loadAll();
    getDashboardStats();
});