/* global Html5Qrcode, paginate, showToast */
(function () {
    var attPag, displayData = [], ctx = {};
    var qrScanner = null, activeSession = null, scanBusy = false;
    var scanResultModal = null, qrScanModal = null;

    function safeArr(r) {
        if (Array.isArray(r)) return r;
        if (r && Array.isArray(r.$values)) return r.$values;
        return [];
    }

    function docNum(series) {
        if (!series) return null;
        var p = String(series).split('-');
        var n = parseInt(p[p.length - 1], 10);
        return isNaN(n) ? null : n;
    }

    function normStatus(s) {
        if (!s) return '';
        var x = String(s);
        if (x === '0') return 'Present';
        if (x === '1') return 'Absent';
        if (x === '2') return 'Late';
        return x;
    }

    function statusClass(s) {
        var x = (s || '').toLowerCase();
        if (x === 'present') return 'badge-present';
        if (x === 'late') return 'badge-late';
        if (x === 'absent') return 'badge-absent';
        return '';
    }

    function statusColor(s) {
        var x = (s || '').toLowerCase();
        if (x === 'present') return '#16A34A';
        if (x === 'late') return '#D97706';
        if (x === 'absent') return '#DC2626';
        return 'var(--accent)';
    }

    function statusIcon(s) {
        var x = (s || '').toLowerCase();
        if (x === 'present') return '<i class="bi bi-check-circle-fill"></i>';
        if (x === 'late') return '<i class="bi bi-clock-fill"></i>';
        if (x === 'absent') return '<i class="bi bi-x-circle-fill"></i>';
        return '<i class="bi bi-question-circle-fill"></i>';
    }

    function formatTime(isoStr) {
        try {
            var d = new Date(isoStr);
            var h = d.getHours(), m = d.getMinutes(), s = d.getSeconds();
            var ampm = h >= 12 ? 'PM' : 'AM';
            h = h % 12 || 12;
            return h + ':' + (m < 10 ? '0' : '') + m + ':' + (s < 10 ? '0' : '') + s + ' ' + ampm;
        } catch (e) { return ''; }
    }

    function updateCounts() {
        var p = 0, a = 0, l = 0, u = 0;
        displayData.forEach(function (r) {
            var s = (r.status || '').toLowerCase();
            if (s === 'present') p++;
            else if (s === 'absent') a++;
            else if (s === 'late') l++;
            else u++;
        });
        $('#presentCount, #sumPresent').text(p);
        $('#absentCount, #sumAbsent').text(a);
        $('#lateCount, #sumLate').text(l);
        $('#sumUnmarked').text(u);
        $('#sumTotal').text(displayData.length);
    }

    function renderAttRow(r, idx) {
        var badge = r.status
            ? '<span class="badge ' + statusClass(r.status) + '">' + r.status + '</span>'
            : '<span class="text-muted" style="font-size:12px;">Unmarked</span>';
        var init = (r._name[0] || '?').toUpperCase();
        var timeMarked = r._timeMarked ? formatTime(r._timeMarked) : '<span class="text-muted">—</span>';
        return '<tr data-student-id="' + (r._studentId || '') + '">' +
            '<td>' + (idx + 1) + '</td>' +
            '<td><code style="font-size:11px;">' + (r._studentId || '—') + '</code></td>' +
            '<td><div class="d-flex align-items-center gap-2">' +
            '<div style="width:32px;height:32px;border-radius:50%;background:var(--accent);color:#fff;display:flex;align-items:center;justify-content:center;font-weight:600;font-size:12px;">' + init + '</div>' +
            r._name + '</div></td>' +
            '<td style="font-size:12px;">' + timeMarked + '</td>' +
            '<td class="att-status-cell">' + badge + '</td>' +
            '</tr>';
    }

    function applyTableRows(rows) {
        var statusF = $('#filterStatus').val();
        var filtered = statusF
            ? rows.filter(function (r) {
                if (statusF === 'unmarked') return !r.status;
                return (r.status || '').toLowerCase() === statusF.toLowerCase();
            })
            : rows;

        if (attPag) attPag.refresh(filtered);
        else {
            attPag = paginate({
                data: filtered,
                pageSize: 20,
                containerId: 'attendanceTableBody',
                paginationId: 'attendancePagination',
                colSpan: 5,
                emptyMsg: 'No students in this class. Assign students to the schedule section.',
                renderRow: renderAttRow
            });
        }
        updateCounts();
    }

    function studentDisplayName(student, doc) {
        var users = safeArr(ctx.users);
        var uds = student ? (student.userDocumentSeries || student.UserDocumentSeries || '') : '';
        var u = users.find(function (x) {
            return String(x.documentSeries || x.DocumentSeries) === String(uds);
        });
        if (u) return u.full_Name || u.fullName || u.email || doc;
        return doc || 'Student';
    }

    function buildDisplayData() {
        var courseId = $('#courseDropdown').val();
        var scheduleId = $('#scheduleDropdown').val();
        if (!courseId || !scheduleId) {
            displayData = [];
            applyTableRows([]);
            return;
        }

        var enrollments = safeArr(ctx.enrollments).filter(function (e) {
            return String(e.course_ID) === String(courseId) &&
                String(e.schedule_ID || '') === String(scheduleId);
        });

        var attStudents = safeArr(ctx.attendanceStudents);
        var sessions = safeArr(ctx.attendance).filter(function (a) {
            return String(a.schedule_ID || a.Schedule_ID) === String(scheduleId);
        });
        var sessionIds = sessions.map(function (a) {
            return String(a.attendance_ID || a.Attendance_ID);
        });
        if (activeSession && activeSession.attendance_ID) {
            sessionIds.push(String(activeSession.attendance_ID));
        }

        displayData = enrollments.map(function (e) {
            var sid = e.student_ID;
            var student = safeArr(ctx.students).find(function (s) {
                return String(docNum(s.documentSeries || s.DocumentSeries)) === String(sid) ||
                    String(s.student_ID || s.Student_ID) === String(e.student_Guid || '');
            });
            var doc = student ? (student.documentSeries || student.DocumentSeries || '') : ('Student #' + sid);
            var rec = attStudents.find(function (a) {
                var attId = String(a.attendance_Id || a.Attendance_Id || '');
                var matchAtt = !sessionIds.length || sessionIds.indexOf(attId) >= 0;
                var aDoc = a.studentDocumentSeries || a.StudentDocumentSeries || '';
                return matchAtt && (String(aDoc) === String(doc) || String(a.student_Id) === String(sid));
            });
            var status = rec ? normStatus(rec.studentAttendanceStatus != null ? rec.studentAttendanceStatus : rec.StudentAttendanceStatus) : '';
            var timeMarked = rec ? (rec.scanTime || rec.ScanTime || rec.createdAt || rec.CreatedAt || '') : '';
            return {
                _name: studentDisplayName(student, doc),
                _studentId: doc,
                status: status,
                _timeMarked: timeMarked,
                _enrollmentId: e.enrollment_ID
            };
        });

        applyTableRows(displayData);
    }

    // ── Schedule banner ──────────────────────────────────────────────────────
    function updateScheduleBanner() {
        var schedules = safeArr(ctx.schedules);
        if (!schedules.length) {
            $('#scheduleInfoBanner').addClass('d-none');
            $('#noScheduleBanner').removeClass('d-none');
            return;
        }
        $('#noScheduleBanner').addClass('d-none');
        var parts = schedules.map(function (s) {
            var course = safeArr(ctx.courses).find(function (c) {
                return String(c.course_ID || c.Course_ID) === String(s.course_ID || s.Course_ID);
            });
            var code = course ? (course.code || course.Code || '') : '';
            var title = course ? (course.title || course.Title || '') : '';
            var day = s.dayOfWeek || s.DayOfWeek || '';
            var st = (s.startTime || s.StartTime || '').toString().substring(0, 5);
            var en = (s.endTime || s.EndTime || '').toString().substring(0, 5);
            return '<strong>' + (code ? code + ' — ' : '') + (title || 'Course') + '</strong>&nbsp;' +
                '<span class="badge bg-primary" style="font-size:11px;">' + day + '</span>&nbsp;' +
                st + ' – ' + en;
        });
        $('#scheduleInfoText').html(parts.join('&nbsp;&nbsp;|&nbsp;&nbsp;'));
        $('#scheduleInfoBanner').removeClass('d-none');
    }

    function loadContext(cb) {
        var courseId = $('#courseDropdown').val();
        var scheduleId = $('#scheduleDropdown').val();
        $.ajax({
            url: '/api/proxy/api/teacher/attendance-context',
            data: { courseId: courseId || '', scheduleId: scheduleId || '' },
            dataType: 'json'
        }).done(function (data) {
            ctx = data || {};
            populateCourses();
            populateSchedules();
            updateScheduleBanner();
            buildDisplayData();
            if (typeof cb === 'function') cb();
        }).fail(function (x) {
            showToast((x.responseJSON && x.responseJSON.message) || 'Failed to load attendance.', 'error');
            $('#noScheduleBanner').removeClass('d-none');
        });
    }

    function populateCourses() {
        var courses = safeArr(ctx.courses);
        var cur = $('#courseDropdown').val();
        var html = '<option value="">Select Course\u2026</option>';
        courses.forEach(function (c) {
            var id = c.course_ID || c.Course_ID;
            html += '<option value="' + id + '">' + (c.code || c.Code || '') + ' \u2014 ' + (c.title || c.Title || '') + '</option>';
        });
        $('#courseDropdown').html(html);
        if (cur) $('#courseDropdown').val(cur);
    }

    function populateSchedules() {
        var courseId = $('#courseDropdown').val();
        var schedules = safeArr(ctx.schedules).filter(function (s) {
            return !courseId || String(s.course_ID || s.Course_ID) === String(courseId);
        });
        var cur = $('#scheduleDropdown').val();
        var html = '<option value="">Select Schedule\u2026</option>';
        schedules.forEach(function (s) {
            var id = s.schedule_Id || s.schedule_ID || s.Schedule_Id;
            var day = s.dayOfWeek || s.DayOfWeek || '';
            var st = (s.startTime || s.StartTime || '').toString().substring(0, 5);
            var en = (s.endTime || s.EndTime || '').toString().substring(0, 5);
            html += '<option value="' + id + '">' + day + ' ' + st + ' \u2013 ' + en + '</option>';
        });
        $('#scheduleDropdown').html(html);
        if (cur) $('#scheduleDropdown').val(cur);
    }

    // ── Scan result popup ────────────────────────────────────────────────────
    function showScanResult(r, scanTimeIso) {
        var status = normStatus(r.status);
        var color = statusColor(status);
        var icon = statusIcon(status);
        var name = r.studentName || r.studentDocumentSeries || 'Student';
        var sid = r.studentDocumentSeries || ('ID: ' + r.student_ID);
        var timeStr = formatTime(scanTimeIso || new Date().toISOString());
        var sessionNum = r.attendance_ID || '\u2014';
        var init = (name[0] || '?').toUpperCase();

        $('#scanResultHeader').css('background', color);
        $('#scanResultIcon').html(icon);
        $('#scanResultStatus').text(status.toUpperCase());
        $('#scanResultAvatar').text(init).css('background', color);
        $('#scanResultName').text(name);
        $('#scanResultId').text(sid);
        $('#scanResultTime').text(timeStr);
        $('#scanResultSession').text('#' + sessionNum);

        if (qrScanModal) qrScanModal.hide();
        setTimeout(function () {
            if (scanResultModal) scanResultModal.show();
        }, 300);
    }

    function markAttendance(token, done) {
        var scheduleId = $('#scheduleDropdown').val();
        var courseId = $('#courseDropdown').val();
        if (!scheduleId || !courseId) {
            showToast('Select course and schedule first.', 'warning');
            return;
        }
        if (!token || !String(token).trim()) return;
        if (scanBusy) return;
        scanBusy = true;

        var scanTimeIso = new Date().toISOString();

        $.ajax({
            type: 'POST',
            url: '/api/proxy/api/teacher/scan-attendance',
            contentType: 'application/json',
            data: JSON.stringify({
                qrToken: String(token).trim(),
                schedule_ID: parseInt(scheduleId, 10),
                course_ID: parseInt(courseId, 10),
                attendance_ID: activeSession ? activeSession.attendance_ID : 0,
                scanTime: scanTimeIso
            }),
            success: function (r) {
                activeSession = { attendance_ID: r.attendance_ID, schedule_ID: scheduleId };
                $('#sessionStatus').text('Recording attendance \u2014 session #' + r.attendance_ID);

                var sid = r.studentDocumentSeries || r.studentName;
                var row = displayData.find(function (x) { return String(x._studentId) === String(sid); });
                if (row) {
                    row.status = normStatus(r.status);
                    row._timeMarked = scanTimeIso;
                } else {
                    displayData.push({
                        _name: r.studentName || sid,
                        _studentId: sid,
                        status: normStatus(r.status),
                        _timeMarked: scanTimeIso
                    });
                }
                applyTableRows(displayData);
                showScanResult(r, scanTimeIso);
                $('#qrScanError').hide();
                if (typeof done === 'function') done(true);
            },
            error: function (x) {
                var msg = (x.responseJSON && x.responseJSON.message) || 'Could not record attendance.';
                $('#qrScanError').text(msg).show();
                showToast(msg, 'error');
                if (typeof done === 'function') done(false);
            },
            complete: function () { scanBusy = false; }
        });
    }

    function startQrScanner() {
        $('#qrScanError').hide();
        if (!window.Html5Qrcode) {
            $('#qrScanError').text('QR scanner failed to load.').show();
            return;
        }
        if (!qrScanner) qrScanner = new Html5Qrcode('qrReader');
        qrScanner.start(
            { facingMode: 'environment' },
            { fps: 10, qrbox: { width: 250, height: 250 } },
            function (decodedText) {
                qrScanner.stop().catch(function () { });
                markAttendance(decodedText.trim(), function () { });
            },
            function () { }
        ).catch(function (err) {
            $('#qrScanError').text('Camera: ' + err).show();
        });
    }

    function stopQrScanner() {
        if (qrScanner) qrScanner.stop().catch(function () { });
    }

    $(function () {
        scanResultModal = new bootstrap.Modal(document.getElementById('scanResultModal'));
        qrScanModal = new bootstrap.Modal(document.getElementById('qrScanModal'));

        $('#page-loader').fadeIn(150);
        loadContext(function () { $('#page-loader').fadeOut(200); });

        $('#courseDropdown').on('change', function () {
            activeSession = null;
            $('#scheduleDropdown').val('');
            loadContext();
        });
        $('#scheduleDropdown').on('change', function () {
            activeSession = null;
            loadContext();
        });
        $('#filterStatus').on('change', function () { applyTableRows(displayData); });

        $('#markEmailBtn').on('click', function () {
            markAttendance($('#studentEmailInput').val().trim());
            $('#studentEmailInput').val('');
        });
        $('#studentEmailInput').on('keypress', function (e) {
            if (e.which === 13) { e.preventDefault(); $('#markEmailBtn').click(); }
        });

        $('#scanQrBtn').on('click', function () {
            if (!$('#courseDropdown').val() || !$('#scheduleDropdown').val()) {
                showToast('Select course and schedule first.', 'warning');
                return;
            }
            qrScanModal.show();
        });

        document.getElementById('qrScanModal').addEventListener('shown.bs.modal', startQrScanner);
        document.getElementById('qrScanModal').addEventListener('hidden.bs.modal', stopQrScanner);

        // "Continue Scanning": close result popup, reopen camera
        $('#scanResultContinue').on('click', function () {
            scanResultModal.hide();
            setTimeout(function () {
                if ($('#courseDropdown').val() && $('#scheduleDropdown').val()) {
                    qrScanModal.show();
                }
            }, 350);
        });
    });
})();