
var STUDENT_GID = 3, TEACHER_GID = 2, ADMIN_GID = 1;
var allAccounts = [], allStudents = [], allTeachers = [], allUsers = [];
var allPrograms = [], allDepts = [], allCourses = [], allSections = [];
var pg;

function toUtcIso(v) { return v ? new Date(v).toISOString() : new Date().toISOString(); }

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
        '<td>' + (a._section || '—') + '</td>' +
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
    allAccounts = allUsers
        .filter(function (u) { return u.userGroup_ID != ADMIN_GID; })
        .map(function (u) {
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
            var section = student ? allSections.find(function (sec) { return String(sec.section_Id || sec.Section_Id) === String(student.sectionID || student.SectionID); }) : null;
            var sectionLabel = section ? (section.section_Code || section.Section_Code || 'Section ' + section.section_Id) : '—';
            return {
                _uid: uid, _ugid: gid,
                _name: u.full_Name || u.email || '?',
                _email: u.email || '—',
                _gender: u.gender || u.sex || '',
                _dept: dept,
                _sid: sid, _tid: tid,
                _section: sectionLabel,
                _created: u.createdAt || ''
            };
        });
}

function loadAll() {
    $('#page-loader').fadeIn(150);
    $.when(
        $.ajax({ url: '/api/User', dataType: 'json' }),
        $.ajax({ url: '/api/Student', dataType: 'json' }),
        $.ajax({ url: '/api/Teacher', dataType: 'json' }),
        $.ajax({ url: '/AttendanceManagement/Program', dataType: 'json' }),
        $.ajax({ url: '/AttendanceManagement/Department', dataType: 'json' }),
        $.ajax({ url: '/AttendanceManagement/Course', dataType: 'json' }),
        $.ajax({ url: '/AttendanceManagement/Section', dataType: 'json' })
    ).done(function (uR, sR, tR, pR, dR, cR, secR) {
        allUsers = Array.isArray(uR[0]) ? uR[0] : [];
        allStudents = Array.isArray(sR[0]) ? sR[0] : [];
        allTeachers = Array.isArray(tR[0]) ? tR[0] : [];
        allPrograms = Array.isArray(pR[0]) ? pR[0] : [];
        allDepts = Array.isArray(dR[0]) ? dR[0] : [];
        allCourses = Array.isArray(cR[0]) ? cR[0] : [];
        allSections = Array.isArray(secR[0]) ? secR[0] : [];

        buildAccounts();

        var progHtml = '<option value="">Select Program</option>';
        allPrograms.forEach(function (p) { progHtml += '<option value="' + p.program_Id + '">' + p.name + '</option>'; });
        $('#addProgramId, #editProgramId').html(progHtml);

        var deptHtml = '<option value="">Select Department</option>';
        allDepts.forEach(function (d) { deptHtml += '<option value="' + d.department_Id + '">' + d.name + '</option>'; });
        $('#addDeptId, #editDeptId, #addTeacherDeptId').html(deptHtml);

        var secHtml = '<option value="">Select Section…</option>';
        allSections.forEach(function (sec) {
            var sid = sec.section_Id || sec.Section_Id;
            var label = (sec.section_Code || sec.Section_Code || 'Section ' + sid);
            if (sec.course_Code || sec.Course_Code) label += ' — ' + (sec.course_Code || sec.Course_Code);
            secHtml += '<option value="' + sid + '">' + label + '</option>';
        });
        $('#addSectionId').html(secHtml);

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
    }).fail(function () {
        $('#page-loader').fadeOut(200);
        showToast('Failed to load data. Check your connection.', 'error');
    });
}


$(document).ready(function () {

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

        if (!fullName) { showToast('Full name is required.', 'warning'); return; }
        if (!email) { showToast('Email is required.', 'warning'); return; }
        if (!password || password.length < 6) { showToast('Password must be at least 6 characters.', 'warning'); return; }

        var ugid = type === 'student' ? STUDENT_GID : TEACHER_GID;
        var userData = {
            full_Name: fullName,
            email: email,
            password: password,
            phone_Number: phone || null,
            gender: gender,
            birth_Date: birthDate ? new Date(birthDate).toISOString() : new Date('2000-01-01').toISOString(),
            address: address || null,
            userGroup_ID: ugid,
            lastUpdatedBy: 'admin'
        };

        $('#page-loader').fadeIn(150);
        $.ajax({
            type: 'POST', url: '/api/User', contentType: 'application/json',
            data: JSON.stringify(userData),
            success: function (user) {
                var newUserId = user.user_ID || user.id;
                if (!newUserId) {
                    $('#page-loader').fadeOut(200);
                    showToast('User created but ID not returned. Please reload.', 'warning');
                    loadAll(); return;
                }

                if (type === 'student') {
                    var sectionId = $('#addSectionId').val();
                    if (!sectionId) { $('#page-loader').fadeOut(200); showToast('Section is required.', 'warning'); return; }
                    var studentData = {
                        user_ID: newUserId,
                        program_ID: parseInt($('#addProgramId').val()) || 0,
                        department_ID: parseInt($('#addDeptId').val()) || 0,
                        sectionID: parseInt(sectionId),
                        year_Level: parseInt($('#addYearLevel').val()) || 1
                    };
                    $.ajax({
                        type: 'POST', url: '/api/Student', contentType: 'application/json',
                        data: JSON.stringify(studentData),
                        success: function () {
                            $('#page-loader').fadeOut(200);
                            $('#addAccountModal').modal('hide');
                            showToast('Student account created successfully.', 'success');
                            loadAll();
                            getDashboardStats();
                        },
                        error: function (x) {
                            $('#page-loader').fadeOut(200);
                            showToast('User created but student record failed: ' + (x.responseJSON && x.responseJSON.message || 'error'), 'error');
                            loadAll();
                        }
                    });
                } else {
                    var deptId = $('#addTeacherDeptId').val();
                    if (!deptId) { $('#page-loader').fadeOut(200); showToast('Department is required.', 'warning'); return; }
                    var teacherData = { user_ID: newUserId, departmentId: parseInt(deptId) };
                    $.ajax({
                        type: 'POST', url: '/api/Teacher', contentType: 'application/json',
                        data: JSON.stringify(teacherData),
                        success: function () {
                            $('#page-loader').fadeOut(200);
                            $('#addAccountModal').modal('hide');
                            showToast('Teacher account created successfully.', 'success');
                            loadAll();
                            getDashboardStats();
                        },
                        error: function (x) {
                            $('#page-loader').fadeOut(200);
                            showToast('User created but teacher record failed: ' + (x.responseJSON && x.responseJSON.message || 'error'), 'error');
                            loadAll();
                        }
                    });
                }
            },
            error: function (x) {
                $('#page-loader').fadeOut(200);
                var msg = (x.responseJSON && (x.responseJSON.message || x.responseJSON.title)) || 'Failed to create account.';
                showToast(msg, 'error');
            }
        });
    });

    $(document).on('click', '.edit-btn', function () {
        var uid = $(this).data('uid');
        var sid = $(this).data('sid');
        var tid = $(this).data('tid');
        var ugid = $(this).data('ugid');

        $('#editUserId').val(uid);
        $('#editStudentId').val(sid);
        $('#editTeacherId').val(tid);
        $('#editUserGroupId').val(ugid);

        if (ugid == STUDENT_GID) {
            $('#editStudentSection').show(); $('#editTeacherSection').hide();
        } else {
            $('#editStudentSection').hide(); $('#editTeacherSection').show();
        }

        $('#page-loader').fadeIn(150);
        $.ajax({
            url: '/api/User/' + encodeURIComponent(uid), dataType: 'json',
            success: function (user) {
                $('#editFullName').val(user.full_Name || '');
                $('#editEmail').val(user.email || '');
                $('#editPhone').val(user.phone_Number || '');
                $('#editGender').val(user.gender || 'M');
                $('#editBirthDate').val((user.birth_Date || '').substring(0, 10));
                $('#editAddress').val(user.address || '');

                if (ugid == STUDENT_GID && sid) {
                    $.ajax({
                        url: '/api/Student/' + encodeURIComponent(sid), dataType: 'json',
                        success: function (s) {
                            $('#editProgramId').val(s.program_ID || '');
                            $('#editDeptId').val(s.department_ID || '');
                            $('#editYearLevel').val(s.year_Level || '1st Year');
                            $('#page-loader').fadeOut(200);
                            $('#editStudentModal').modal('show');
                        },
                        error: function () {
                            $('#page-loader').fadeOut(200);
                            $('#editStudentModal').modal('show');
                        }
                    });
                } else if (ugid == TEACHER_GID && tid) {
                    $.ajax({
                        url: '/api/Teacher/' + encodeURIComponent(tid), dataType: 'json',
                        success: function (t) {
                            $('#editTeacherDept').val(t.department || '');
                            $('#page-loader').fadeOut(200);
                            $('#editStudentModal').modal('show');
                        },
                        error: function () {
                            $('#page-loader').fadeOut(200);
                            $('#editStudentModal').modal('show');
                        }
                    });
                } else {
                    $('#page-loader').fadeOut(200);
                    $('#editStudentModal').modal('show');
                }
            },
            error: function () {
                $('#page-loader').fadeOut(200);
                showToast('Failed to load user data.', 'error');
            }
        });
    });

    $('#updateStudentBtn').on('click', function () {
        var uid = $('#editUserId').val();
        var sid = $('#editStudentId').val();
        var tid = $('#editTeacherId').val();
        var ugid = parseInt($('#editUserGroupId').val());

        var fullName = $('#editFullName').val().trim();
        var email = $('#editEmail').val().trim();
        if (!fullName) { showToast('Full name is required.', 'warning'); return; }
        if (!email) { showToast('Email is required.', 'warning'); return; }

        var userData = {
            user_ID: uid,
            full_Name: fullName,
            email: email,
            phone_Number: $('#editPhone').val().trim() || null,
            gender: $('#editGender').val(),
            birth_Date: $('#editBirthDate').val() ? new Date($('#editBirthDate').val()).toISOString() : new Date('2000-01-01').toISOString(),
            address: $('#editAddress').val().trim() || null,
            userGroup_ID: ugid,
            lastUpdatedBy: 'admin'
        };

        $('#page-loader').fadeIn(150);
        $.ajax({
            type: 'PUT', url: '/api/User/' + encodeURIComponent(uid),
            contentType: 'application/json', data: JSON.stringify(userData),
            success: function () {
                if (ugid === STUDENT_GID && sid) {
                    var sData = {
                        student_ID: sid,
                        program_ID: $('#editProgramId').val() ? parseInt($('#editProgramId').val()) : null,
                        department_ID: $('#editDeptId').val() ? parseInt($('#editDeptId').val()) : null,
                        year_Level: $('#editYearLevel').val(),
                        lastUpdatedBy: 'admin'
                    };
                    $.ajax({
                        type: 'PUT', url: '/api/Student/' + encodeURIComponent(sid),
                        contentType: 'application/json', data: JSON.stringify(sData),
                        complete: function () {
                            $('#page-loader').fadeOut(200);
                            $('#editStudentModal').modal('hide');
                            showToast('Student updated successfully.', 'success');
                            loadAll();
                        }
                    });
                } else if (ugid === TEACHER_GID && tid) {
                    var tData = {
                        teacher_ID: tid,
                        department: $('#editTeacherDept').val().trim() || null,
                        lastUpdatedBy: 'admin'
                    };
                    $.ajax({
                        type: 'PUT', url: '/api/Teacher/' + encodeURIComponent(tid),
                        contentType: 'application/json', data: JSON.stringify(tData),
                        complete: function () {
                            $('#page-loader').fadeOut(200);
                            $('#editStudentModal').modal('hide');
                            showToast('Teacher updated successfully.', 'success');
                            loadAll();
                        }
                    });
                } else {
                    $('#page-loader').fadeOut(200);
                    $('#editStudentModal').modal('hide');
                    showToast('Account updated successfully.', 'success');
                    loadAll();
                }
            },
            error: function (x) {
                $('#page-loader').fadeOut(200);
                var msg = (x.responseJSON && (x.responseJSON.message || x.responseJSON.title)) || 'Update failed.';
                showToast(msg, 'error');
            }
        });
    });

    $(document).on('click', '.delete-btn', function () {
        var uid = $(this).data('uid');
        var sid = $(this).data('sid');
        var tid = $(this).data('tid');
        var ugid = $(this).data('ugid');
        var name = $(this).data('name');

        if (!confirm('Delete account for "' + name + '"?\n\nThis cannot be undone.')) return;

        $('#page-loader').fadeIn(150);

        function deleteUser() {
            $.ajax({
                type: 'DELETE', url: '/api/User/' + encodeURIComponent(uid),
                success: function () {
                    $('#page-loader').fadeOut(200);
                    showToast(name + ' deleted successfully.', 'success');
                    loadAll();
                },
                error: function (x) {
                    $('#page-loader').fadeOut(200);
                    showToast('Failed to delete user: ' + (x.responseJSON && x.responseJSON.message || 'error'), 'error');
                }
            });
        }

        if (ugid === STUDENT_GID && sid) {
            $.ajax({
                type: 'DELETE', url: '/api/Student/' + encodeURIComponent(sid),
                success: deleteUser,
                error: deleteUser 
            });
        } else if (ugid === TEACHER_GID && tid) {
            $.ajax({
                type: 'DELETE', url: '/api/Teacher/' + encodeURIComponent(tid),
                success: deleteUser,
                error: deleteUser
            });
        } else {
            deleteUser();
        }
    });

    $('#searchStudent').on('input', applyFilter);
    $('#filterRole, #sortBy').on('change', applyFilter);
});