/*! Vera VAF 1.0 license: https://github.com/sparkledb/vera */
$(function () {

    // Interface
    $.fn.extend({
        /** Initiate a input box */
        inputBox: function () {
            _inputBox($(this));
        }
        ,
        /** Save the contents of a HTML Editor input box */
        saveInputBox: function (input, data) {
            _saveInputBox($(this), input, data);
        }
    });

    var iOinProgress = false;

    /**
    * Save the contents of a HTML Editor input box
    * @param {Object} ckEditor CKEditor instance object
    */
    function _saveInputBox(inputDiv, input, data)
    {
        // Save the field value using a REST API
        var updateUrl = inputDiv.attr('vera_oauth_url_ns:update');
        var oAuthSignature = inputDiv.attr('vera_oauth_sig_ns:update');
        var oAuthParam = inputDiv.attr('vera_oauth_param_ns:update');
        var tableName = inputDiv.attr('vera_table_ns:name');
        var entityType = inputDiv.attr('vera_table_ns:entity');
        var partitionKey = inputDiv.attr('vera_table_ns:partitionkey');
        var propertyName = inputDiv.attr('vera_table_ns:property');
        

        // Hide the save and clear button until the update has completed
        inputDiv.find('.saveIcon').fadeOut();
        inputDiv.find('#savingIcon').fadeIn();
        inputDiv.find('.clearIcon').animate({
            opacity: 0.5
        });

        // Prevent further input while saving
        if (input != null)
        {
            input.attr('readonly', true);
        }

        // Call the REST API
        _saveUsingRestApi(inputDiv, updateUrl, oAuthSignature, oAuthParam, data, tableName, entityType, partitionKey, propertyName);
    }

    function _saveUsingRestApi(inputDiv, updateUrl, oAuthSignature, oAuthParam, data, tableName, entityType, partitionKey, propertyName)
    {
        // Make sure there are no current I/O operations in progress
        if (iOinProgress) {
            throw "I/O operation already in progress";
        }

        // Flag I/O in progress
        iOinProgress = true;

        $.fn.callOAuthServiceInterface(updateUrl, oAuthSignature, oAuthParam,
            '{"fieldData": '
            + Create_TablePropertyInfo(tableName, entityType, partitionKey, propertyName, data)
            + ' }',
            function (result) {
                var parsedResult = $.parseJSON(result);

                // The input box turns into a CKEditor HTML editor if the control is in edit mode
                var savingIcon = inputDiv.find('#savingIcon');
                if (savingIcon != null) {
                    // Success, so show some icons to inform the user
                    inputDiv.find('#savingIcon').fadeOut();
                    inputDiv.find('#successIcon').fadeIn();
                    inputDiv.find('.clearIcon').animate({
                        opacity: 1.0
                    });
                }

                //alert(parsedResult);
            }
            , function (XMLHttpRequest, textStatus, errorThrown) {

                var message = "There was an error with the AJAX request(" + errorThrown + ").\n";
                switch (textStatus) {
                    case 'timeout':
                        message += "The request timed out.";
                        break;
                    case 'notmodified':
                        message += "The request was not modified but was not retrieved from the cache.";
                        break;
                    case 'parseerror':
                        message += "XML/Json format is bad.";
                        break;
                    default:
                        message += "HTTP Error (" + XMLHttpRequest.status + " " + XMLHttpRequest.statusText + ").";
                }
                message += "\n";

                console.error(message, XMLHttpRequest, textStatus, errorThrown);

                var shortErrorMsg = "Error: " + errorThrown;

                // Are we using CKEditor?
                var savingIcon = inputDiv.find('#savingIcon');
                if (savingIcon != null) {
                    // Failed, so show some icons to inform the user
                    savingIcon.fadeOut();

                    var failIcon = inputDiv.find('#failIcon');
                    failIcon.prop('title', shortErrorMsg)
                    failIcon.fadeIn();
                }
                else {
                    // CKEditor shows an alert message box
                    alert(shortErrorMsg);
                }
            }
            , function () {
                var input = inputDiv.find('.inputField');
                if (input != null) {
                    // Enable the input box for input again
                    input.attr('readonly', false);
                }

                // Flag I/O finished
                iOinProgress = false;
            }
        );
    }

    /**
    * Initiates a input box.
    * @param {Object} inputDiv inputbox element
    */
    function _inputBox(inputDiv)
    {
        var input = inputDiv.find('.inputField');

        showOrHideClearButton(input, inputDiv);

        if (input.val().length > 0)
        {
            _fadeOutAnimate(inputDiv);
        }
        else
        {
            inputDiv.find('#helpText')._fadeIn(1000);
        }

        // Add event that fires when the input field gets focus
        input.focus(function () {
            if (input.prop('readonly') != true)
            {
                _fadeOut($(this), inputDiv);
            }
        });

        // Add event that fires when the input field looses focus
        input.focusout(function () {
            if (input.prop('readonly') != true) {
                _fadeIn($(this), inputDiv);
            }
        });

        // Add event that fires at keyup
        input.keyup(function () {
            if (input.prop('readonly') != true) {
                showOrHideClearButton($(this), inputDiv);

                inputDiv.find('#successIcon').fadeOut();
                inputDiv.find('#failIcon').fadeOut();
                inputDiv.find('.saveIcon').fadeIn();
            }
        });

        // Add event that fires at when the user clicks the save button
        inputDiv.find('.saveIcon').click(function () {
            var data = input.val();
            _saveInputBox(inputDiv, input, data)
        });

        // Add event that fires at when the user clicks the clear button
        inputDiv.find('.clearIcon').click(function (e) {
            // Make sure there are no I/O operations in progress
            if (iOinProgress)
            {
                // I/O in pregress so prevent action
                e.preventDefault();
                return false;
            }

            // Clear the values
            input.val('');

            showOrHideClearButton($(this), inputDiv);

            inputDiv.find('#successIcon').fadeOut();
            inputDiv.find('#failIcon').fadeOut();
            inputDiv.find('.saveIcon').fadeIn();

            input.focus();
        });

        showOrHideClearButton(input, inputDiv);
    }

    function _fadeIn(input, inputDiv) {
        if (input.val().length == 0) {
            inputDiv.find('#inputBoxBackground').animate({
                opacity: 0.5
            });
        }
    }

    function _fadeOutAnimate(inputDiv) {
        inputDiv.find('#inputBoxBackground').animate({
            opacity: 1.0
        });
    }

    function _fadeOut(input, inputDiv) {
        if (input.val().length == 0) {
            _fadeOutAnimate(inputDiv);
        }
    }

    function showOrHideClearButton(input, inputDiv) {
        if (input.val().length == 0) {
            inputDiv.find('.clearIcon').fadeOut();
        }
        else {
            inputDiv.find('.clearIcon').fadeIn();
        }
    }
});

// Init all the edit input field controls after the page has loaded
$(document).ready(function () {

    // The input box turns into a CKEditor HTML editor if the control is in edit mode
    if (typeof CKEDITOR !== 'undefined')
    {
        // Alter to refresh the CKEditor config.js cache
        CKEDITOR.timestamp = 'VeraWAF';

        // Enable AJAX saving with the CKEditor HTML editor
        CKEDITOR.plugins.registered['save'] =
        {
            init: function (editor) {
                // Override the default save functionality with our own
                var command = editor.addCommand('save',
                    {
                        modes: { wysiwyg: 1, source: 1 },
                        exec: function (editor) {
                            // Get the textarea HTML element
                            var input = $(editor.element.$);

                            // Get the input box container
                            var inputDiv = $(editor.element.$.parentElement);

                            inputDiv.saveInputBox(input, editor.getData());
                        }
                    }
                );
                editor.ui.addButton('Save', { label: 'Save markup', command: 'save' });

            }
        }
    }

    // Init inline input fields
    $('div.inputBox').each(function () {
        $(this).inputBox();
    });
});
