function _eS(xhr) {
    var m = 'Something went wrong.';
    try {
        if (xhr.responseJSON) {
            if (xhr.responseJSON.message) m = xhr.responseJSON.message;
            else if (xhr.responseJSON.title) m = xhr.responseJSON.title;
            else if (xhr.responseJSON.errors) {
                var errs = xhr.responseJSON.errors, msgs = [];
                Object.keys(errs).forEach(function (k) { if (Array.isArray(errs[k])) msgs = msgs.concat(errs[k]); });
                if (msgs.length) m = msgs.join(' ');
            }
        } else if (xhr.responseText) m = xhr.responseText;
    } catch (e) { }
    if (typeof showToast === 'function') showToast(m, 'error');
}

function safeArrS(r) {
    if (Array.isArray(r)) return r;
    if (r && Array.isArray(r.$values)) return r.$values;
    return [];
}

function getStudents(cb) {
    $.when(
        $.ajax({ type: 'GET', url: '/api/proxy/api/User', dataType: 'json', global: false })
            .then(function (r) { return safeArrS(r); }, function () { return []; }),
        $.ajax({ type: 'GET', url: '/api/proxy/api/Student', dataType: 'json', global: false })
            .then(function (r) { return safeArrS(r); }, function () { return []; })
    ).done(function (users, students) {
        $('#page-loader').fadeOut(200);
        var merged;
        if (students.length) {
            merged = students.map(function (s) {
                var u = users.find(function (x) { return String(x.user_ID) === String(s.user_ID); }) || {};
                return Object.assign({}, u, s);
            });
        } else {
            merged = users.filter(function (u) { return u.userGroup_ID == 3; });
        }
        if (typeof cb === 'function') cb(merged);
    }).fail(function () {
        $('#page-loader').fadeOut(200);
        if (typeof cb === 'function') cb([]);
    });
}

function parseDocSeriesId(series) {
    if (!series) return null;
    var p = String(series).split('-');
    var n = parseInt(p[p.length - 1], 10);
    return isNaN(n) ? null : n;
}

function parseUserIdFromRegister(user) {
    if (!user) return null;
    var data = user.data || user.Data || user;
    var series = data.documentSeries || data.DocumentSeries || user.documentSeries || user.DocumentSeries;
    return parseDocSeriesId(series);
}

function addStudent(userData, studentData, cb) {
    if (userData.gender && !userData.sex) userData.sex = userData.gender;
    delete userData.gender;
    delete userData.userGroup_ID;
    $.ajax({
        type: 'POST', url: '/api/proxy/api/User',
        contentType: 'application/json', dataType: 'json',
        data: JSON.stringify(userData),
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (user) {
            var newUserId = parseUserIdFromRegister(user);
            if (!newUserId) {
                $('#page-loader').fadeOut(200);
                showToast('Failed to obtain user ID from backend.', 'error');
                return;
            }
            studentData.user_ID = newUserId;
            $.ajax({
                type: 'POST', url: '/api/proxy/api/Student',
                contentType: 'application/json', dataType: 'json',
                data: JSON.stringify(studentData),
                success: function (r) {
                    $('#page-loader').fadeOut(200);
                    showToast('Student added successfully.', 'success');
                    if (typeof cb === 'function') cb(r);
                },
                error: function (x) { $('#page-loader').fadeOut(200); _eS(x); }
            });
        },
        error: function (x) { $('#page-loader').fadeOut(200); _eS(x); }
    });
}

function updateStudent(userId, userData, studentId, studentData, cb) {
    $('#page-loader').fadeIn(150);
    $.ajax({
        type: 'GET', url: '/api/proxy/api/User/' + encodeURIComponent(userId), dataType: 'json',
        success: function (currentUser) {
            var updateUserData = {
                user_ID: userId,
                full_Name: userData.full_Name,
                email: userData.email,
                password: currentUser.passHash || currentUser.PassHash || currentUser.password || 'unchanged',
                phone_Number: userData.phone_Number,
                gender: userData.gender,
                birth_Date: userData.birth_Date || null,
                address: userData.address,
                userGroup_ID: userData.userGroup_ID || 3,
                lastUpdatedBy: userData.lastUpdatedBy || 'admin'
            };
            $.ajax({
                type: 'PUT', url: '/api/proxy/api/User/' + encodeURIComponent(userId),
                contentType: 'application/json', dataType: 'json',
                data: JSON.stringify(updateUserData),
                success: function () {
                    if (studentId) {
                        $.ajax({
                            type: 'PUT', url: '/api/proxy/api/Student/' + encodeURIComponent(studentId),
                            contentType: 'application/json', dataType: 'json',
                            data: JSON.stringify({
                                student_ID: parseInt(studentId),
                                user_ID: userId,
                                program_ID: studentData.program_ID || null,
                                department_ID: studentData.department_ID || null,
                                year_Level: studentData.year_Level || null,
                                lastUpdatedBy: studentData.lastUpdatedBy || 'admin'
                            }),
                            success: function (r) {
                                $('#page-loader').fadeOut(200);
                                showToast('Student updated successfully.', 'success');
                                if (typeof cb === 'function') cb(r);
                            },
                            error: function (x) { $('#page-loader').fadeOut(200); _eS(x); }
                        });
                    } else {
                        $('#page-loader').fadeOut(200);
                        showToast('User updated successfully.', 'success');
                        if (typeof cb === 'function') cb();
                    }
                },
                error: function (x) { $('#page-loader').fadeOut(200); _eS(x); }
            });
        },
        error: function (x) { $('#page-loader').fadeOut(200); _eS(x); }
    });
}

function deleteStudent(userId, studentId, cb) {
    $('#page-loader').fadeIn(150);
    var deleteUser = function () {
        $.ajax({
            type: 'DELETE', url: '/api/proxy/api/User/' + encodeURIComponent(userId), dataType: 'json',
            success: function () {
                $('#page-loader').fadeOut(200);
                showToast('Account deleted successfully.', 'success');
                if (typeof cb === 'function') cb();
            },
            error: function (x) { $('#page-loader').fadeOut(200); _eS(x); }
        });
    };
    if (studentId) {
        $.ajax({
            type: 'DELETE', url: '/api/proxy/api/Student/' + encodeURIComponent(studentId), dataType: 'json',
            success: deleteUser,
            error: function () { deleteUser(); }
        });
    } else {
        deleteUser();
    }
}