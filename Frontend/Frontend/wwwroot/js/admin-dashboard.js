var STUDENT_GID = 3, TEACHER_GID = 2, ADMIN_GID = 1;
var allAccounts = [], allStudents = [], allTeachers = [], allUsers = [];
var allPrograms = [], allDepts = [], allCourses = [], allEnrollments = [], allSections = [];
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

function inferUserGroup(u) {
    if (u.userGroup_ID == TEACHER_GID || u.userGroup_ID == STUDENT_GID || u.userGroup_ID == ADMIN_GID) return u.userGroup_ID;
    var doc = u.documentSeries || u.DocumentSeries || '';
    var email = (u.email || '').toLowerCase();
    if (allStudents.some(function (s) { return String(s.userDocumentSeries || s.UserDocumentSeries) === String(doc); })) return STUDENT_GID;
    if (allTeachers.some(function (t) { return String(t.userDocumentSeries || t.UserDocumentSeries) === String(doc); })) return TEACHER_GID;
    if (email.indexOf('@dbtc-cebu') >= 0) return STUDENT_GID;
    if (email.indexOf('@local') >= 0) return TEACHER_GID;
    if (email.indexOf('@admin') >= 0) return ADMIN_GID;
    return STUDENT_GID;
}

function buildAccounts() {
    allAccounts = allUsers.map(function (u) {
        var doc = u.documentSeries || u.DocumentSeries || '';
        var uid = u.user_ID || doc || u.email;
        var gid = inferUserGroup(u);
        var student = allStudents.find(function (s) { return String(s.userDocumentSeries || s.UserDocumentSeries) === String(doc); });
        var teacher = allTeachers.find(function (t) { return String(t.userDocumentSeries || t.UserDocumentSeries) === String(doc); });
        var prog = student ? allPrograms.find(function (p) { return String(p.program_Id) === String(student.program_ID); }) : null;
        var dept = student
            ? (prog ? prog.name : (student.program_ID ? 'Program #' + student.program_ID : '—'))
            : teacher ? (teacher.department || '—') : '—';
        var sid = student ? (student.documentSeries || student.DocumentSeries || student.student_ID || '') : '';
        var tid = teacher ? (teacher.documentSeries || teacher.DocumentSeries || '') : '';
        var studentDocNum = student ? String(sid).split('-').pop() : '';
        var courseCount = allEnrollments.length
            ? allEnrollments.filter(function (e) { return String(e.student_ID) === String(studentDocNum); }).length
            : 0;
        return {
            _uid: uid, _ugid: gid,
            _name: u.full_Name || u.email || '?',
            _email: u.email || '—',
            _gender: u.gender || u.sex || '',
            _dept: dept,
            _sid: sid, _tid: tid,
            _courses: courseCount,
            _created: u.createdAt || ''
        };
    });
}

// FIX: centralized dropdown population used on load and when edit modal opens
function populateDropdowns() {
    var progHtml = '<option value="">Select Program</option>';
    allPrograms.forEach(function (p) {
        progHtml += '<option value="' + p.program_Id + '">' + (p.name || p.programName || 'Program ' + p.program_Id) + '</option>';
    });
    $('#addProgramId, #editProgramId').html(progHtml);

    var deptHtml = '<option value="">Select Department</option>';
    allDepts.forEach(function (d) {
        deptHtml += '<option value="' + d.department_Id + '">' + (d.name || d.departmentName || 'Dept ' + d.department_Id) + '</option>';
    });
    // FIX: teacher dept is now a <select>, populated here
    $('#addDeptId, #editDeptId, #addTeacherDept, #editTeacherDept').html(deptHtml);

    var courseHtml = '<option value="">Select Course</option>';
    allCourses.forEach(function (c) {
        var cid = c.course_ID || c.course_Id || c.id;
        courseHtml += '<option value="' + cid + '">' + (c.course_Name || c.courseName || c.name || 'Course ' + cid) + '</option>';
    });
    // FIX: student enrollment uses Course not Section
    $('#addCourseId').html(courseHtml);
    // Course picker for assigning students to teacher
    var teacherCourseHtml = '<option value="">None / Keep existing</option>';
    allCourses.forEach(function (c) {
        var cid = c.course_ID || c.course_Id || c.id;
        teacherCourseHtml += '<option value="' + cid + '">' + (c.course_Name || c.courseName || c.name || 'Course ' + cid) + '</option>';
    });
    $('#editTeacherCourse').html(teacherCourseHtml);
}

function loadAll() {
    $('#page-loader').fadeIn(150);
    $.when(
        ajaxSafe('/api/proxy/api/User'),
        ajaxSafe('/api/proxy/api/Student'),
        ajaxSafe('/api/proxy/api/Teacher'),
        ajaxSafe('/api/proxy/AttendanceManagement/Program'),
        ajaxSafe('/api/proxy/AttendanceManagement/Department'),
        ajaxSafe('/api/proxy/AttendanceManagement/Section'),
        ajaxSafe('/api/proxy/AttendanceManagement/Course'),
        ajaxSafe('/api/proxy/AttendanceManagement/Enrollment')
    ).done(function (users, students, teachers, programs, depts, sections, courses, enrollments) {
        allUsers = users;
        allStudents = students;
        allTeachers = teachers;
        allPrograms = programs;
        allDepts = depts;
        allSections = sections;
        allCourses = courses;
        allEnrollments = enrollments;

        buildAccounts();

        $('#statStudents').text(allStudents.length || users.filter(function (u) { return u.userGroup_ID == 3; }).length);
        $('#statTeachers').text(allTeachers.length || users.filter(function (u) { return u.userGroup_ID == 2; }).length);
        $('#statCourses').text(allCourses.length);
        if (typeof getDashboardStats === 'function') getDashboardStats();

        populateDropdowns();

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
        if (type === 'student' && !email.includes('@dbtc-cebu')) {
            showToast('Student email must include @dbtc-cebu', 'error'); return;
        }
        if (type === 'teacher' && !email.includes('@local')) {
            showToast('Teacher email must include @local', 'error'); return;
        }

        // FIX: send birth_Date as UTC ISO to avoid PostgreSQL DateTime Kind=Unspecified error
        var birthDateUtc = birthDate ? birthDate + 'T00:00:00Z' : null;

        var userData = {
            full_Name: fullName, email: email, password: password,
            phone_Number: phone, sex: gender || 'M',
            birth_Date: birthDateUtc,
            address: address,
            userGroup_ID: ugid
        };

        if (type === 'student') {
            var progId = $('#addProgramId').val();
            var deptId = $('#addDeptId').val();
            var yearLevel = $('#addYearLevel').val();
            // FIX: use Course instead of Section
            var courseId = $('#addCourseId').val();
            var yearNum = parseInt(yearLevel, 10) || 1;

            // Use first available section as default sectionID (required by backend)
            var sectionId = 1;
            if (allSections.length > 0) {
                sectionId = allSections[0].section_ID || allSections[0].Section_ID || allSections[0].id || 1;
            }

            if (!courseId) { showToast('Course is required for students.', 'error'); return; }

            var studentData = {
                program_ID: progId ? parseInt(progId, 10) : 1,
                department_ID: deptId ? parseInt(deptId, 10) : 1,
                sectionID: sectionId,
                year_Level: yearNum
            };

            addStudent(userData, studentData, function (result) {
                $('#addAccountModal').modal('hide');
                // FIX: auto-enroll new student in the selected course
                var newStudentId = null;
                if (result) {
                    newStudentId = result.student_ID || result.id ||
                        (result.documentSeries ? String(result.documentSeries).split('-').pop() : null);
                }
                if (newStudentId && courseId) {
                    $.ajax({
                        type: 'POST', url: '/api/proxy/AttendanceManagement/Enrollment',
                        contentType: 'application/json', dataType: 'json',
                        data: JSON.stringify({ student_ID: parseInt(newStudentId), course_ID: parseInt(courseId) }),
                        global: false
                    });
                }
                // FIX: notify admin board that a new student was enrolled
                $.ajax({
                    type: 'POST', url: '/api/proxy/api/Notifications',
                    contentType: 'application/json', dataType: 'json',
                    data: JSON.stringify({
                        recipientId: 'admin',
                        title: 'New Student Enrolled',
                        message: 'A new student was enrolled: ' + fullName + ' (' + email + ').',
                        type: 'announcement'
                    }),
                    global: false
                });
                loadAll();
            });
        } else {
            // FIX: teacher dept is now a select — use its value
            var deptVal = $('#addTeacherDept').val();
            var teacherData = { departmentId: deptVal ? parseInt(deptVal, 10) || 1 : 1 };
            addTeacher(userData, teacherData, function () {
                $('#addAccountModal').modal('hide');
                // FIX: notify admin board that a new teacher was added
                $.ajax({
                    type: 'POST', url: '/api/proxy/api/Notifications',
                    contentType: 'application/json', dataType: 'json',
                    data: JSON.stringify({
                        recipientId: 'admin',
                        title: 'New Teacher Added',
                        message: 'A new teacher was added: ' + fullName + ' (' + email + ').',
                        type: 'announcement'
                    }),
                    global: false
                });
                loadAll();
            });
        }
    });

    $(document).on('click', '.edit-btn', function () {
        var uid = $(this).data('uid');
        var ugid = parseInt($(this).data('ugid'));
        var sid = $(this).data('sid');
        var tid = $(this).data('tid');
        var user = allUsers.find(function (u) { return String(u.user_ID) === String(uid); });
        if (!user) { showToast('User not found.', 'error'); return; }

        $('#editUserId').val(uid);
        $('#editUserGroupId').val(ugid);
        $('#editStudentId').val(sid || '');
        $('#editTeacherId').val(tid || '');
        $('#editFullName').val(user.full_Name || '');
        $('#editEmail').val(user.email || '');
        $('#editPhone').val(user.phone_Number || '');
        $('#editGender').val(user.gender || '');
        $('#editBirthDate').val(user.birth_Date ? user.birth_Date.substring(0, 10) : '');
        $('#editAddress').val(user.address || '');

        // FIX: populate dropdowns before setting values so <option> elements exist
        populateDropdowns();

        if (ugid === STUDENT_GID) {
            $('#editStudentSection').show(); $('#editTeacherSection').hide();
            var student = allStudents.find(function (s) { return String(s.user_ID) === String(uid); });
            if (student) {
                $('#editProgramId').val(student.program_ID || '');
                $('#editDeptId').val(student.department_ID || '');
                $('#editYearLevel').val(student.year_Level || '');
            }
        } else {
            $('#editStudentSection').hide(); $('#editTeacherSection').show();
            var teacher = allTeachers.find(function (t) { return String(t.user_ID) === String(uid); });
            if (teacher) {
                // FIX: editTeacherDept is a <select> now — match by dept ID
                var deptId = teacher.department_ID || teacher.departmentId || '';
                $('#editTeacherDept').val(deptId);
            }
            $('#editTeacherCourse').val('');
        }
        $('#editStudentModal').modal('show');
    });

    $('#updateStudentBtn').on('click', function () {
        var uid = $('#editUserId').val();
        var ugid = parseInt($('#editUserGroupId').val());
        var sid = $('#editStudentId').val();
        var tid = $('#editTeacherId').val();
        var fullName = $('#editFullName').val().trim();
        var email = $('#editEmail').val().trim();
        var phone = $('#editPhone').val().trim();
        var gender = $('#editGender').val();
        var birthDate = $('#editBirthDate').val();
        var address = $('#editAddress').val().trim();

        if (!fullName || !email) { showToast('Name and email are required.', 'error'); return; }

        // FIX: send birth_Date as UTC
        var birthDateUtc = birthDate ? birthDate + 'T00:00:00Z' : null;

        var userData = {
            full_Name: fullName, email: email, phone_Number: phone,
            gender: gender, birth_Date: birthDateUtc, address: address,
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
                $('#editStudentModal').modal('hide'); loadAll();
            });
        } else {
            // FIX: teacher dept is now a select
            var deptVal = $('#editTeacherDept').val();
            var teacherData = {
                department: deptVal || '',
                departmentId: deptVal ? parseInt(deptVal, 10) : null,
                lastUpdatedBy: 'admin'
            };
            updateTeacher(uid, userData, tid, teacherData, function () {
                // FIX: if admin picked a course, bulk-assign all enrolled students to this teacher
                var selectedCourseId = $('#editTeacherCourse').val();
                if (selectedCourseId) {
                    var enrolledStudentIds = allEnrollments
                        .filter(function (e) { return String(e.course_ID) === String(selectedCourseId); })
                        .map(function (e) { return e.student_ID; });
                    if (enrolledStudentIds.length > 0) {
                        var assignDefs = enrolledStudentIds.map(function (studentId) {
                            return $.ajax({
                                type: 'POST', url: '/api/proxy/AttendanceManagement/Enrollment',
                                contentType: 'application/json', dataType: 'json',
                                data: JSON.stringify({
                                    student_ID: parseInt(studentId),
                                    course_ID: parseInt(selectedCourseId),
                                    teacher_ID: tid ? parseInt(tid) : null
                                }),
                                global: false
                            });
                        });
                        $.when.apply($, assignDefs).always(function () {
                            showToast(enrolledStudentIds.length + ' student(s) assigned to teacher for this course.', 'success');
                            loadAll();
                        });
                    } else {
                        showToast('No enrolled students found for that course.', 'info');
                        loadAll();
                    }
                } else {
                    loadAll();
                }
                $('#editStudentModal').modal('hide');
            });
        }
    });

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
});