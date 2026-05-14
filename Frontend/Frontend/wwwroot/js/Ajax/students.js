function _eS(xhr) {
    var m = 'Something went wrong.';
    try {
        if (xhr.responseJSON) {
            if (xhr.responseJSON.message) m = xhr.responseJSON.message;
            else if (xhr.responseJSON.title) m = xhr.responseJSON.title;
            else if (xhr.responseJSON.errors) {
                var errs = xhr.responseJSON.errors, msgs = [];
                Object.keys(errs).forEach(function(k) { if (Array.isArray(errs[k])) msgs = msgs.concat(errs[k]); });
                if (msgs.length) m = msgs.join(' ');
            }
        } else if (xhr.responseText) m = xhr.responseText;
    } catch (e) {}
    if (typeof showToast === 'function') showToast(m, 'error');
}

function safeArrS(r) {
    if (Array.isArray(r)) return r;
    if (r && Array.isArray(r.$values)) return r.$values;
    return [];
}

function getStudents(cb) {
    $.ajax({
        type: 'GET', url: '/api/proxy/api/Student', dataType: 'json',
        beforeSend: function() { $('#page-loader').fadeIn(150); },
        success: function(r) { $('#page-loader').fadeOut(200); if (typeof cb === 'function') cb(safeArrS(r)); },
        error: function(x) { $('#page-loader').fadeOut(200); if (typeof cb === 'function') cb([]); }
    });
}

function addStudent(userData, studentData, cb) {
    $.ajax({
        type: 'POST', url: '/api/proxy/api/User', contentType: 'application/json', dataType: 'json',
        data: JSON.stringify(userData),
        beforeSend: function() { $('#page-loader').fadeIn(150); },
        success: function(user) {
            var newUserId = user.user_ID || user.documentSeries || user.id || null;
            if (!newUserId) {
                $('#page-loader').fadeOut(200);
                showToast('Failed to obtain user ID from backend.', 'error');
                return;
            }
            studentData.user_ID = newUserId;
            $.ajax({
                type: 'POST', url: '/api/proxy/api/Student', contentType: 'application/json', dataType: 'json',
                data: JSON.stringify(studentData),
                success: function(r) {
                    $('#page-loader').fadeOut(200);
                    showToast('Student added successfully.', 'success');
                    if (typeof cb === 'function') cb(r);
                },
                error: function(x) { $('#page-loader').fadeOut(200); _eS(x); }
            });
        },
        error: function(x) { $('#page-loader').fadeOut(200); _eS(x); }
    });
}

function updateStudent(userId, userData, studentId, studentData, cb) {
    $('#page-loader').fadeIn(150);
    $.ajax({
        type: 'GET', url: '/api/proxy/api/User/' + encodeURIComponent(userId), dataType: 'json',
        success: function(currentUser) {
            var updateUserData = {
                user_ID: userId,
                full_Name: userData.full_Name,
                email: userData.email,
                password: currentUser.passHash || currentUser.PassHash || 'unchanged',
                phone_Number: userData.phone_Number,
                gender: userData.gender,
                birth_Date: userData.birth_Date,
                address: userData.address,
                userGroup_ID: userData.userGroup_ID || 3,
                lastUpdatedBy: userData.lastUpdatedBy || 'admin'
            };
            $.ajax({
                type: 'PUT', url: '/api/proxy/api/User/' + encodeURIComponent(userId), contentType: 'application/json', dataType: 'json',
                data: JSON.stringify(updateUserData),
                success: function() {
                    if (studentId) {
                        $.ajax({
                            type: 'PUT', url: '/api/proxy/api/Student/' + encodeURIComponent(studentId), contentType: 'application/json', dataType: 'json',
                            data: JSON.stringify({
                                student_ID: studentId,
                                user_ID: userId,
                                program_ID: studentData.program_ID,
                                department_ID: studentData.department_ID,
                                year_Level: studentData.year_Level,
                                lastUpdatedBy: studentData.lastUpdatedBy || 'admin'
                            }),
                            success: function(r) {
                                $('#page-loader').fadeOut(200);
                                showToast('Student updated successfully.', 'success');
                                if (typeof cb === 'function') cb(r);
                            },
                            error: function(x) { $('#page-loader').fadeOut(200); _eS(x); }
                        });
                    } else {
                        $('#page-loader').fadeOut(200);
                        showToast('Student updated successfully.', 'success');
                        if (typeof cb === 'function') cb();
                    }
                },
                error: function(x) { $('#page-loader').fadeOut(200); _eS(x); }
            });
        },
        error: function(x) { $('#page-loader').fadeOut(200); _eS(x); }
    });
}

function deleteStudent(userId, studentId, cb) {
    $('#page-loader').fadeIn(150);
    $.ajax({
        type: 'DELETE', url: '/api/proxy/api/Student/' + encodeURIComponent(studentId), dataType: 'json',
        success: function() {
            $.ajax({
                type: 'DELETE', url: '/api/proxy/api/User/' + encodeURIComponent(userId), dataType: 'json',
                success: function() {
                    $('#page-loader').fadeOut(200);
                    showToast('Student deleted successfully.', 'success');
                    if (typeof cb === 'function') cb();
                },
                error: function(x) { $('#page-loader').fadeOut(200); _eS(x); }
            });
        },
        error: function(x) { $('#page-loader').fadeOut(200); _eS(x); }
    });
}