function getSessionUserId() {
    return (document.getElementById('sessionUserId') || {}).value || '';
}

function getSessionRole() {
    return (document.getElementById('sessionRole') || {}).value || '';
}

function isAuthenticated() {
    return getSessionRole().length > 0;
}

function redirectToLogin() {
    if (window.location.pathname.toLowerCase().indexOf('/login') === 0) return;
    window.location.replace('/Login/Index');
}

$(document).ajaxError(function (event, xhr, settings) {
    if (xhr.status === 401 || xhr.status === 403) {
        var url = settings.url || '';
        var bgEndpoints = ['/api/Notifications', '/api/SessionInfo', '/api/proxy/api/Notifications'];
        var isBg = bgEndpoints.some(function (ep) { return url.indexOf(ep) !== -1; });
        if (isBg) return;

        var path = window.location.pathname.toLowerCase();
        if (path.indexOf('/login') === 0) return;
        if (xhr.status === 403 && typeof showToast === 'function') {
            showToast('You do not have permission for this action.', 'warning');
            return;
        }
        redirectToLogin();
    }
});

$(function () {
    var publicPages = ['/login/index', '/home/error', '/login/authenticate', '/login/logout'];
    var currentPath = window.location.pathname.toLowerCase();
    var isPublic = publicPages.some(function (p) { return currentPath === p; });

    if (!isAuthenticated() && !isPublic) {
        redirectToLogin();
        return;
    }

    var role = getSessionRole();
    var path = window.location.pathname.toLowerCase();

    if (role === 'admin' && (path.startsWith('/student') || path.startsWith('/teacher'))) {
        window.location.replace('/Admin/Dashboard'); return;
    }
    if (role === 'teacher' && (path.startsWith('/student') || path.startsWith('/admin'))) {
        window.location.replace('/Teacher/Dashboard'); return;
    }
    if (role === 'student' && (path.startsWith('/teacher') || path.startsWith('/admin'))) {
        window.location.replace('/Student/Profile'); return;
    }
});