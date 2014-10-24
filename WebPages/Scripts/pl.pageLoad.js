/*! Vera VAF 1.0 license: https://github.com/sparkledb/vera */

// Stuff to do after the page has loaded
$(document).ready(function () {
    var $window = $(window);
    var $root = $('html, body');
    var $doc = $(document);
    var $form = $('form');

    // Dynamically load images as they are viewed
    $('img.jail').show().jail({ effect: 'fadeIn' });

    // Start timeago scripts
    $('time.timeAgo').each(function () {
        $(this).timeAgo();
    });
});
