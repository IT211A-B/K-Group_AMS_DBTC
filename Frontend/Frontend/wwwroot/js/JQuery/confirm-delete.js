/* jquery/confirm-delete.js */
$(function () {
    window.confirmDelete = function (name, cb) { $('#deleteItemName').text(name); $('#confirmDeleteBtn').off('click').on('click', function () { $('#deleteModal').modal('hide'); if (typeof cb === 'function') cb(); }); $('#deleteModal').modal('show'); };
    window.showWarning = function (msg, cb) { $('#warningMsg').text(msg); $('#warningConfirmBtn').off('click').on('click', function () { $('#warningModal').modal('hide'); if (typeof cb === 'function') cb(); }); $('#warningModal').modal('show'); };
    window.showError = function (msg) { $('#errorMsg').text(msg || 'Something went wrong.'); $('#errorModal').modal('show'); };
});