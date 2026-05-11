function _eT(xhr) {
    var m = 'Something went wrong.';
    try {
        if (xhr.responseJSON) {
            if (xhr.responseJSON.message) m = xhr.responseJSON.message;
            else if (xhr.responseJSON.title) m = xhr.responseJSON.title;
            else if (xhr.responseJSON.errors) {
                var errs = xhr.responseJSON.errors;
                var msgs = [];
                Object.keys(errs).forEach(function (k) { if (Array.isArray(errs[k])) msgs = msgs.concat(errs[k]); });
                if (msgs.length) m = msgs.join(' ');
            }
        } else if (xhr.responseText) m = xhr.responseText;
    } catch (e) { }
    if (typeof showToast === 'function') showToast(m, 'error');
}

function getTeachers(cb) {
    $.ajax({
        type: 'GET', url: '/api/proxy/api/Teacher', dataType: 'json',
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (r) { $('#page-loader').fadeOut(200); if (typeof cb === 'function') cb(Array.isArray(r) ? r : []); },
        error: function (x) { $('#page-loader').fadeOut(200); _eT(x); }
    });
}

function addTeacher(userData, teacherData, cb) {
    $.ajax({
        type: 'POST', url: '/api/proxy/api/User', contentType: 'application/json', dataType: 'json',
        data: JSON.stringify(userData),
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function (user) {
            teacherData.user_ID = user.user_ID || user.id;
            $.ajax({
                type: 'POST', url: '/api/proxy/api/Teacher', contentType: 'application/json', dataType: 'json',
                data: JSON.stringify(teacherData),
                success: function (r) {
                    $('#page-loader').fadeOut(200);
                    showToast('Teacher added successfully.', 'success');
                    if (typeof cb === 'function') cb(r);
                },
                error: function (x) { $('#page-loader').fadeOut(200); _eT(x); }
            });
        },
        error: function (x) { $('#page-loader').fadeOut(200); _eT(x); }
    });
}

function updateTeacher(userId, userData, teacherId, teacherData, cb) {
    $('#page-loader').fadeIn(150);
    $.ajax({
        type: 'GET', url: '/api/proxy/api/User/' + encodeURIComponent(userId), dataType: 'json',
        success: function (currentUser) {
            var updateUserData = {
                user_ID: userId,
                full_Name: userData.full_Name,
                email: userData.email,
                password: currentUser.passHash || currentUser.PassHash || 'unchanged',
                phone_Number: userData.phone_Number,
                gender: userData.gender,
                birth_Date: userData.birth_Date,
                address: userData.address,
                userGroup_ID: userData.userGroup_ID || 2,
                lastUpdatedBy: userData.lastUpdatedBy || 'admin'
            };
            $.ajax({
                type: 'PUT', url: '/api/proxy/api/User/' + encodeURIComponent(userId), contentType: 'application/json', dataType: 'json',
                data: JSON.stringify(updateUserData),
                success: function () {
                    if (teacherId) {
                        $.ajax({
                            type: 'PUT', url: '/api/proxy/api/Teacher/' + encodeURIComponent(teacherId), contentType: 'application/json', dataType: 'json',
                            data: JSON.stringify({
                                department: teacherData.department,
                                lastUpdatedBy: teacherData.lastUpdatedBy || 'admin'
                            }),
                            success: function (r) {
                                $('#page-loader').fadeOut(200);
                                showToast('Teacher updated successfully.', 'success');
                                if (typeof cb === 'function') cb(r);
                            },
                            error: function (x) { $('#page-loader').fadeOut(200); _eT(x); }
                        });
                    } else {
                        $('#page-loader').fadeOut(200);
                        showToast('Teacher updated successfully.', 'success');
                        if (typeof cb === 'function') cb();
                    }
                },
                error: function (x) { $('#page-loader').fadeOut(200); _eT(x); }
            });
        },
        error: function (x) { $('#page-loader').fadeOut(200); _eT(x); }
    });
}

function deleteTeacher(userId, teacherId, cb) {
    $.ajax({
        type: 'DELETE', url: '/api/proxy/api/Teacher/' + encodeURIComponent(teacherId), dataType: 'json',
        beforeSend: function () { $('#page-loader').fadeIn(150); },
        success: function () {
            $.ajax({
                type: 'DELETE', url: '/api/proxy/api/User/' + encodeURIComponent(userId), dataType: 'json',
                success: function () {
                    $('#page-loader').fadeOut(200);
                    showToast('Teacher deleted successfully.', 'success');
                    if (typeof cb === 'function') cb();
                },
                error: function (x) { $('#page-loader').fadeOut(200); _eT(x); }
            });
        },
        error: function (x) { $('#page-loader').fadeOut(200); _eT(x); }
    });
}