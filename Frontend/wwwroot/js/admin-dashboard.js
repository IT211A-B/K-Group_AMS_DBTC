// admin-dashboard.js
var STUDENT_GID = 3;
var TEACHER_GID = 2;
var ADMIN_GID = 1;
var allAccounts = [], allStudents = [], allTeachers = [], allUsers = [], allPrograms = [], allDepts = [], allCourses = [], allEnrollments = [], pg;

function toUtcIso(v) { return v ? new Date(v).toISOString() : new Date().toISOString(); }
function nameOf(a) { return (a._name || '').toLowerCase(); }

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

    if (a._ugid === STUDENT_GID) {
        actions = '<button class="btn btn-sm btn-outline-primary btn-icon edit-btn" data-sid="' + a._sid + '" data-uid="' + a._uid + '" data-ugid="' + a._ugid + '" title="Edit"><i class="bi bi-pencil"></i></button>' +
            '<a href="/Admin/ViewStudentProfile?userId=' + a._uid + '" class="btn btn-sm btn-outline-info btn-icon" title="View Profile"><i class="bi bi-person-lines-fill"></i></a>';
    } else if (a._ugid === TEACHER_GID) {
        actions = '<button class="btn btn-sm btn-outline-primary btn-icon edit-btn" data-tid="' + a._tid + '" data-uid="' + a._uid + '" data-ugid="' + a._ugid + '" title="Edit"><i class="bi bi-pencil"></i></button>' +
            '<a href="/Admin/ViewTeacherProfile?userId=' + a._uid + '" class="btn btn-sm btn-outline-info btn-icon" title="View Profile"><i class="bi bi-person-lines-fill"></i></a>';
    } else {
        actions = '<span class="text-muted">Admin</span>';
    }

    return '<tr>' +
        '<td>' + (i + 1) + '</td>' +
        '<td><div class="d-flex align-items-center gap-2"><div style="width:32px;height:32px;border-radius:50%;background:' + roleColor(a._ugid) + ';color:#fff;display:flex;align-items:center;justify-content:center;font-weight:600;font-size:12px;">' + init + '</div>' + a._name + '</div></td>' +
        '<td>' + a._email + '</td>' +
        '<td>' + roleBadge + '</td>' +
        '<td>' + (a._gender === 'M' ? 'Male' : a._gender === 'F' ? 'Female' : (a._gender || '—')) + '</td>' +
        '<td>' + a._dept + '</td>' +
        '<td class="d-flex gap-1">' + actions + '</td>' +
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

    if (sort === 'name-asc') result.sort(function (a, b) { return nameOf(a).localeCompare(nameOf(b)); });
    if (sort === 'name-desc') result.sort(function (a, b) { return nameOf(b).localeCompare(nameOf(a)); });

    if (pg) pg.refresh(result);
    else pg = paginate({ data: result, pageSize: 15, containerId: 'studentTableBody', paginationId: 'studentPagination', colSpan: 7, emptyMsg: 'No accounts found.', renderRow: renderRow });
}

function buildAccounts() {
    allAccounts = [];

    console.log("Building accounts...");
    console.log("allUsers:", allUsers);
    console.log("allStudents:", allStudents);
    console.log("allTeachers:", allTeachers);

    allUsers.forEach(function (u) {
        var gid = u.userGroup_ID || u.userGroupId || 0;
        var sid = null, tid = null, dept = '—';

        if (gid === STUDENT_GID) {
            var s = allStudents.find(function (x) { return String(x.user_ID) === String(u.user_ID); });
            if (s) {
                sid = s.student_ID;
                dept = s.year_Level || 'Student';
            }
        }
        else if (gid === TEACHER_GID) {
            var t = allTeachers.find(function (x) { return String(x.user_ID) === String(u.user_ID); });
            if (t) {
                tid = t.teacher_ID;
                dept = t.department || 'Teacher';
            }
        }
        else {
            dept = 'Administrator';
        }

        allAccounts.push({
            _ugid: gid,
            _sid: sid,
            _tid: tid,
            _uid: u.user_ID,
            _name: u.full_Name || '—',
            _email: u.email || '—',
            _gender: u.gender || '',
            _dept: dept
        });
    });

    console.log("All accounts built:", allAccounts);
    applyFilter();
}

function loadAll() {
    $('#page-loader').fadeIn(150);

    $.when(
        $.ajax({ url: '/api/Student', dataType: 'json' }),
        $.ajax({ url: '/api/Teacher', dataType: 'json' }),
        $.ajax({ url: '/api/User', dataType: 'json' }),
        $.ajax({ url: '/AttendanceManagement/Course', dataType: 'json' })
    ).done(function (sR, tR, uR, cR) {
        allStudents = Array.isArray(sR[0]) ? sR[0] : [];
        allTeachers = Array.isArray(tR[0]) ? tR[0] : [];
        allUsers = Array.isArray(uR[0]) ? uR[0] : [];
        allCourses = Array.isArray(cR[0]) ? cR[0] : [];

        console.log("Data loaded:", { students: allStudents.length, teachers: allTeachers.length, users: allUsers.length, courses: allCourses.length });

        pg = null;
        buildAccounts();

        $('#statStudents').text(allStudents.length);
        $('#statTeachers').text(allTeachers.length);
        $('#statCourses').text(allCourses.length);
        $('#statAbsences').text('0'); // You can compute from attendance if needed

        $('#page-loader').fadeOut(200);
    }).fail(function (jqXHR, textStatus, errorThrown) {
        $('#page-loader').fadeOut(200);
        console.error("AJAX Error:", textStatus, errorThrown);
        showToast('Failed to load data: ' + textStatus, 'error');
    });
}

function setupEmailHint() {
    $('#addEmail').on('input', function () {
        var emailValue = $(this).val().toLowerCase();
        var hintText = '';
        if (emailValue.indexOf('admin') !== -1 && emailValue.indexOf('@') !== -1) {
            hintText = 'Admin account';
        } else if (emailValue.indexOf('local') !== -1 && emailValue.indexOf('@') !== -1) {
            hintText = 'Teacher account';
        } else if (emailValue.indexOf('dbtc-cebu') !== -1) {
            hintText = 'Student account';
        }
        $('#addEmailHint').html('<i class="bi bi-info-circle"></i> ' + hintText);
    });
}

$(function () {
    loadAll();
    getDashboardStats();
    setupEmailHint();

    $('input[name="acctType"]').on('change', function () {
        var isStudent = $(this).val() === 'student';
        $('#studentFields').toggle(isStudent);
        $('#teacherFields').toggle(!isStudent);
    });

    $('#searchStudent, #filterRole, #sortBy').on('input change', applyFilter);
});