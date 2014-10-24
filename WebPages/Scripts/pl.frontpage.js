/*! Vera VAF 1.0 license: https://github.com/sparkledb/vera */

// Stuff to do after the frontpage has loaded
$(document).ready(function () {
    // Fade out the confirmation e-mail icon
    function animateEmailIcon() {
        $('#betaIcons').animate(
            {
                opacity: 0.0,
                left: "+=30em"
            }
            , 1000
            , function () {
                $('#confirmation').fadeIn(function () {
                    $('section#share').fadeIn();
                });
            });
    }

    $('#betaIcons').fadeIn(function () {
        setTimeout(function () { animateEmailIcon(); }, 500);
    });
});
