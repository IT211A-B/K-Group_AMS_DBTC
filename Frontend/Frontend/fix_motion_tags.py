import re
from pathlib import Path

OT = "<" + "d" + "i" + "v"
CT = "</" + "d" + "i" + "v" + ">"

files = [
    Path("Views/Admin/Dashboard.cshtml"),
    Path("Views/Teacher/Attendance.cshtml"),
    Path("Views/Admin/Schedules.cshtml"),
    Path("wwwroot/js/teacher-attendance.js"),
]

for p in files:
    if not p.exists():
        continue
    c = p.read_text(encoding="utf-8")
    c = re.sub(r"<motion(\s|>)", OT + r"\1", c)
    c = re.sub(r"</motion>", CT, c)
    p.write_text(c, encoding="utf-8")
    print("fixed", p)

dash = Path("Views/Admin/Dashboard.cshtml")
c = dash.read_text(encoding="utf-8")
c = re.sub(
    r'\s*<div class="col-md-12"><label class="form-label">Enroll in Course\(s\)</label><div id="addStudentCourses"[^<]*</motion></motion>\s*',
    "\n",
    c,
)
c = re.sub(
    r'\s*<div class="col-md-12"><label class="form-label">Enroll in Course\(s\)</label><div id="addStudentCourses"[^<]*</div></div>\s*',
    "\n",
    c,
)
dash.write_text(c, encoding="utf-8")

js = Path("wwwroot/js/admin-dashboard.js")
j = js.read_text(encoding="utf-8")
j = re.sub(r"\s*var courseCheckHtml[\s\S]*?\$\('#addTeacherCourses'\)[^\n]*\n", "\n", j)
js.write_text(j, encoding="utf-8")
print("done")
