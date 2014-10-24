/*! Vera VAF 1.0 license: https://github.com/sparkledb/vera */

$(document).ready(function () {

    function btnBgEffect() {
        var animTimeMs = 750;   // Animation durations in MS
        var startPos = -6.6; // em start pos - must match css class
        var percentage = 1.3; // Background scale percentage / 100.0 - must match css class
        var bodyFontSize = 14;  // Body font in pixels - must match css class
        var btn = $('button[type="submit"]');
        var newBgPosition = Math.floor(btn.width() * percentage * 1.25 - Math.abs(startPos) * bodyFontSize);
        btn.animate({ backgroundPosition: newBgPosition + 'px' }, animTimeMs, function () {
            $(this).css('background-position', startPos + 'em');
        })
    }

    setInterval( function () { btnBgEffect() }, 5750);
});