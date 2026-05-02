// login.js
document.getElementById('togglePwd').addEventListener('click', function () {
    var i = document.getElementById('loginPassword');
    if (i.type === 'password') {
        i.type = 'text';
        this.className = 'bi bi-eye';
    } else {
        i.type = 'password';
        this.className = 'bi bi-eye-slash';
    }
});

document.getElementById('loginEmail').addEventListener('input', function () {
    var email = this.value.toLowerCase();
    var hint = document.getElementById('emailHint');
    // Check for email patterns
    if (email.indexOf('admin') !== -1 && email.indexOf('@') !== -1) {
        hint.innerHTML = '<i class="bi bi-shield-lock-fill"></i> Signing in as: Administrator';
    } else if (email.indexOf('local') !== -1 && email.indexOf('@') !== -1) {
        hint.innerHTML = '<i class="bi bi-person-badge-fill"></i> Signing in as: Teacher';
    } else if (email.indexOf('dbtc-cebu') !== -1) {
        hint.innerHTML = '<i class="bi bi-mortarboard-fill"></i> Signing in as: Student';
    } else {
        hint.innerHTML = '<i class="bi bi-info-circle"></i> Use email with @admin, @local, or @dbtc-cebu';
    }
});

function doLogin() {
    var email = document.getElementById('loginEmail').value.trim();
    var pwd = document.getElementById('loginPassword').value.trim();
    var errEl = document.getElementById('loginError');
    errEl.classList.add('d-none');

    if (!email || !pwd) {
        errEl.textContent = 'Email and password are required.';
        errEl.classList.remove('d-none');
        return;
    }

    document.getElementById('loginBtnText').textContent = 'Signing in...';
    document.getElementById('loginBtnSpinner').classList.remove('d-none');
    document.getElementById('loginBtn').disabled = true;

    fetch('/Login/Authenticate', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email: email, password: pwd }),
        credentials: 'same-origin'
    })
        .then(function (res) {
            return res.json().then(function (data) {
                return { ok: res.ok, data: data };
            });
        })
        .then(function (r) {
            if (r.ok) {
                var role = r.data.role;
                if (role === 'teacher') {
                    window.location.href = '/Teacher/Dashboard';
                } else if (role === 'student') {
                    window.location.href = '/Student/Profile';
                } else {
                    window.location.href = '/Admin/Dashboard';
                }
            } else {
                errEl.textContent = r.data.message || 'Invalid email or password.';
                errEl.classList.remove('d-none');
                document.getElementById('loginBtnText').textContent = 'Login';
                document.getElementById('loginBtnSpinner').classList.add('d-none');
                document.getElementById('loginBtn').disabled = false;
            }
        })
        .catch(function () {
            errEl.textContent = 'Connection error. Please check if backend is running.';
            errEl.classList.remove('d-none');
            document.getElementById('loginBtnText').textContent = 'Login';
            document.getElementById('loginBtnSpinner').classList.add('d-none');
            document.getElementById('loginBtn').disabled = false;
        });
}

document.getElementById('loginBtn').addEventListener('click', doLogin);
document.getElementById('loginPassword').addEventListener('keydown', function (e) {
    if (e.key === 'Enter') doLogin();
});