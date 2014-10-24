/*! Vera VAF 1.0 license: https://github.com/sparkledb/vera */

$(document).ready(function () {

    /* Process all e-mail inputs */
    $('input[type="email"]').each(function (index) {
        var id = $(this).attr("id")

        var parent = $(this).parent();
        var input = $(this);
        var envelope = parent.find('#email-envelope');
        var rem = parent.find('#email-remove');
        var emailBackground = parent.find('#email-background');
        var placeholder = parent.find('#email-placeholder');

        initEmailControl(input, envelope, emailBackground, rem, placeholder);
    });

    function fadeIn(input, envelope, emailBackground) {
        if (input === undefined) return;
        if (input.val().length == 0) {
            emailBackground.animate({
                opacity: 0.5
            });

            envelope.stop(true);
            envelope.animate({ color: '#cccccc' }, 'fast')
        }
    }

    function fadeOut(input, envelope, emailBackground) {
        if (input === undefined) return;
        if (input.val().length == 0) {
            emailBackground.animate({
                opacity: 1.0
            });

            envelope.stop(true);
            envelope.animate({ color: '#000000' }, 'fast')
        }
    }

    function showOrHideClearButton(input, rem) {
        if (input === undefined) return;
        var opacityVal = input.val().length == 0 ? 0.0 : 1.0;
        rem.stop(true);
        rem.animate({ opacity: opacityVal }, 'fast');
    }

    function inputChanged(input, envelope, rem) {

        showOrHideClearButton(input, rem);

        // Get the color of the envelope based on the state of the input
        var rgbaVal;

        if (input.val().length == 0) {
            rgbaVal = '#000000';
        }
        else {
            rgbaVal = IsEmail(input.val()) ? '#00cc00' : '#f89922';
        }

        envelope.stop(true);
        envelope.animate({ color: rgbaVal }, 'fast');
    }

    function initEmailControl(input, envelope, emailBackground, rem, placeholder) {
        if (input === undefined) return;

        rem.click(function (event) {
            event.stopPropagation();
            input.val('');
            showOrHideClearButton(input, $(this));

            lastNumberOfHits = 0;

            input.focus();
            inputChanged($(this), envelope, rem);
        });

        showOrHideClearButton(input, rem);

        // Replace the default HTML placeholder for the e-mail input and replace it with our own
        input.attr("placeholder", "");

        if (input.val().length > 0) {
            fadeOut();
        }
        else {
            placeholder.show();
        }

        input.focus(function () {
            fadeOut($(this), envelope, emailBackground);
        });

        input.focusout(function () {
            fadeIn($(this), envelope, emailBackground);
        });

        input.keyup(function () {
            inputChanged($(this), envelope, rem);
        });
    }

});