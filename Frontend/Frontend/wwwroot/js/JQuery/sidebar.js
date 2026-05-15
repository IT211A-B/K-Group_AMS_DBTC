/* jquery/sidebar.js */
$(function () {
    $(document).on('click', '#sidebarToggle', function () { $('#sidebar').toggleClass('show'); $('.sidebar-overlay').toggleClass('show'); });
    $(document).on('click', '.sidebar-overlay', function () { $('#sidebar').removeClass('show'); $(this).removeClass('show'); });
    $(window).on('load', function () { $('#page-loader').fadeOut(300); });
});