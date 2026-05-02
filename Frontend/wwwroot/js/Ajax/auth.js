// auth.js
function getSessionUserId() {
    return document.getElementById('sessionUserId')?.value || '';
}

function getSessionRole() {
    return document.getElementById('sessionRole')?.value || '';
}

function isAuthenticated() {
    return !!getSessionUserId();
}

function redirectToLogin() {
    window.location.href = '/Login/Index';
}

$(function () {
    var publicPages = ['/Login/Index', '/Home/Error', '/Login/Authenticate'];
    if (!isAuthenticated() && !publicPages.includes(window.location.pathname)) {
        redirectToLogin();
    }
});