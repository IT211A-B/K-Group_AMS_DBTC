from pathlib import Path

p = Path(__file__).parent / "Views" / "Admin" / "Dashboard.cshtml"
text = p.read_text(encoding="utf-8")

needle = """    <div class="col-6 col-md-3">
        <motion></motion>
        <motion></motion>
        <div class="stat-card">
            <div class="stat-icon red"><i class="bi bi-calendar-x"></i></div>
            <div><div class="stat-label">Absences Today</div><div class="stat-value" id="statAbsences">—</div></div>
        </div>
    </div>
</div>"""

replacement = """    <div class="col-6 col-md-3">
        <div class="stat-card">
            <div class="stat-icon purple"><i class="bi bi-clipboard-data"></i></div>
            <div><div class="stat-label">Attendance Records</div><div class="stat-value" id="statAttendance">—</div></div>
        </div>
    </div>
</div>

<div class="row g-4 mb-4">
    <div class="col-6 col-md-3">
        <motion></motion>
        <div class="stat-card">
            <div class="stat-icon teal"><i class="bi bi-qr-code"></i></div>
            <div><div class="stat-label">Student QR Codes</div><div class="stat-value" id="statQrCodes">—</div></div>
        </div>
    </div>
    <div class="col-6 col-md-3">
        <div class="stat-card">
            <div class="stat-icon indigo"><i class="bi bi-bell"></i></div>
            <div><div class="stat-label">Notifications</motion></div><div class="stat-value" id="statNotifications">—</div></div>
        </div>
    </div>
    <div class="col-6 col-md-3">
        <div class="stat-card">
            <div class="stat-icon red"><i class="bi bi-calendar-week"></i></div>
            <div><div class="stat-label">Schedules</div><div class="stat-value" id="statSchedules">—</div></div>
        </div>
    </div>
    <div class="col-6 col-md-3">
        <div class="stat-card">
            <div class="stat-icon red"><i class="bi bi-calendar-x"></i></div>
            <div><div class="stat-label">Absences Today</div><div class="stat-value" id="statAbsences">—</div></div>
        </div>
    </div>
</div>"""

needle = """    <div class="col-6 col-md-3">
        <div class="stat-card">
            <div class="stat-icon red"><i class="bi bi-calendar-x"></i></div>
            <div><div class="stat-label">Absences Today</div><div class="stat-value" id="statAbsences">—</div></div>
        </div>
    </div>
</div>"""

replacement = replacement.replace("<motion></motion>\n        ", "").replace("</motion>", "")

if needle not in text:
    raise SystemExit("needle not found")

p.write_text(text.replace(needle, replacement, 1), encoding="utf-8")
print("patched ok")
