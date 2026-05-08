
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
    if (email.indexOf('@admin') !== -1) {
        hint.innerHTML = '<i class="bi bi-shield-lock-fill text-danger"></i> Signing in as: Administrator';
    } else if (email.indexOf('@local') !== -1) {
        hint.innerHTML = '<i class="bi bi-person-badge-fill text-primary"></i> Signing in as: Teacher';
    } else if (email.indexOf('@dbtc-cebu') !== -1) {
        hint.innerHTML = '<i class="bi bi-mortarboard-fill text-success"></i> Signing in as: Student';
    } else if (email.length > 3) {
        hint.innerHTML = '<i class="bi bi-info-circle text-warning"></i> Use @admin, @local, or @dbtc-cebu';
    } else {
        hint.innerHTML = '';
    }
});

function setLoading(on) {
    var btn = document.getElementById('loginBtn');
    var txt = document.getElementById('loginBtnText');
    var spin = document.getElementById('loginBtnSpinner');
    btn.disabled = on;
    txt.textContent = on ? 'Signing in…' : 'Login';
    if (on) spin.classList.remove('d-none');
    else spin.classList.add('d-none');
}

function showError(msg) {
    var el = document.getElementById('loginError');
    el.textContent = msg;
    el.classList.remove('d-none');
}

function hideError() {
    document.getElementById('loginError').classList.add('d-none');
}

function doLogin() {
    var email = document.getElementById('loginEmail').value.trim();
    var pwd = document.getElementById('loginPassword').value;
    hideError();

    if (!email) { showError('Email is required.'); return; }
    if (!pwd) { showError('Password is required.'); return; }
    if (!email.includes('@')) { showError('Please enter a valid email address.'); return; }

    setLoading(true);

    fetch('/Login/Authenticate', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'same-origin', 
        body: JSON.stringify({ email: email, password: pwd })
    })
        .then(function (res) {
            return res.json().then(function (data) {
                return { status: res.status, ok: res.ok, data: data };
            });
        })
        .then(function (r) {
            if (r.ok) {
                var role = r.data.role;
                setTimeout(function () {
                    if (role === 'teacher') window.location.href = '/Teacher/Dashboard';
                    else if (role === 'student') window.location.href = '/Student/Profile';
                    else window.location.href = '/Admin/Dashboard';
                }, 150);
            } else if (r.status === 502) {
                setLoading(false);
                showError(r.data.message || 'Server is starting up. Please wait 30 seconds and try again.');
            } else {
                setLoading(false);
                showError(r.data.message || 'Invalid email or password.');
            }
        })
        .catch(function () {
            setLoading(false);
            showError('Connection error. Please check your internet connection.');
        });
}

document.getElementById('loginBtn').addEventListener('click', doLogin);
document.getElementById('loginPassword').addEventListener('keydown', function (e) {
    if (e.key === 'Enter') doLogin();
});
document.getElementById('loginEmail').addEventListener('keydown', function (e) {
    if (e.key === 'Enter') document.getElementById('loginPassword').focus();
});