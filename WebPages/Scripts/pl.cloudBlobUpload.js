/*! Vera VAF 1.0 license: https://github.com/sparkledb/vera */

/* Interfaces */
$(function () {
    $.fn.extend({
        progressbar: function (val) {
            return _progressbar($(this), val);
        }
    });

    function _progressbar(elem, val) {
        elem.find("div").width(val + '%');
    }
});

/**
* Event that is fired when the file list is empty
*/
function FileListEmptyEvent() {
    $('#FileList').fadeOut(400, function () {
        $('#FileList').html('<p id="emptyList"></p>').fadeIn();
    });
    $('#ClearFileList').attr('disabled', true);
    $('#UploadFiles').attr('disabled', true);
    $('#TotalProgress').html('').fadeOut();
    $('#UploadFinished').html('').fadeOut();
}

// Get the Silverlight OBJECT HTML element
var plugin = document.getElementById("SlPlugin");

/**
* Removes all the files from the file list
*/
function RemoveFilesFromList() {
    var files = plugin.Content.Files;

    $('#FileList').children().each(function () {
        var child = $(this);
        for (var i = 0; i < files.FileList.length; i++)
            if (child.attr('id') == 'file' + CleanSlId(files.FileList[i].Id)) return;
        child.fadeOut();
    });
}

/**
* Cleans up the Silverlight file ID by removing bad characters
* Picks up the new files from the Silverlight control.
*/
function CleanSlId(uncleanId) {
    return uncleanId.replace(/=/g, '');
}

/**
* Updates all the progress bars to reflect the current upload progress in realtime.
* Picks up the real-time files progress data from the Silverlight control.
*/
function UpdateProgressBars() {
    var files = plugin.Content.Files;
    for (var i = 0; i < files.FileList.length; i++) {
        var id = CleanSlId(files.FileList[i].Id);
        $('#file' + id + 'progress').progressbar(files.FileList[i].Percentage);

        $('#file' + id + 'bytesUploaded').html(
                Math.floor(files.FileList[i].BytesUploaded / 1024)
                + '/'
                + Math.floor(files.FileList[i].FileSize / 1024)
                + ' Kb ['
                + files.FileList[i].Percentage
                + '%] - '
                + files.FileList[i].StateString
                );
    }
}

/**
* Adds new files to the list.
* Picks up the new files from the Silverlight control.
*/
function AddNewFilesToList() {

    $('#emptyList').fadeOut(400);

    var files = plugin.Content.Files;
    for (var i = 0; i < files.FileList.length; i++) {
        var id = CleanSlId(files.FileList[i].Id);

        if (!$('#file' + id).length)
        {
            $('#FileList').append(
                        '<div class="file" id="file' + id + '">'
                        + ' <div class="innerRegion" id="file' + id + 'innerRegion">'
                        + '  <div class="filename">' + files.FileList[i].FileName + ' </div>'
                        + '  <div class="progress" id="file' + id + 'progress"><div></div></div>'
                        + '  <div bytesUploaded="" id="file' + id + 'bytesUploaded"></div>'
                        + '  <button class="stop" id="stopFileUpload' + id + '" type="button"><i class="fa fa-close"></i></button>'
                        + ' </div>'
                        + '</div>');

            $('#stopFileUpload' + id).click(function () {
                plugin.Content.MainPage.CancelUpload($(this).attr('_id'));
            }).attr('_id', id);

            $('#stopFileUpload' + id).click(function () {
                plugin.Content.MainPage.CancelUpload($(this).attr('_id'));
            }).attr('_id', id);

        }
    }
}

/**
* Updates progress bars
*/
function UpdateDynamicContent() {
    UpdateProgressBars();
    UpdateControlsToCurrentState();
}

/**
* Refreshes the content of the file list
*/
function RefreshFileList() {
    RemoveFilesFromList();
    AddNewFilesToList();
    UpdateDynamicContent();
}

/**
*Make the buttons reflect the current state
*/
function UpdateControlsToCurrentState() {
    var files = plugin.Content.Files;
    $('#ClearFileList').prop('disabled', files.FileList.length == 0 );
    $('#UploadFiles').prop('disabled', files.FileList.length == 0);

    for (var i = 0; i < files.FileList.length; i++) {
        $('#stopFileUpload' + CleanSlId(files.FileList[i].Id)).prop(
            'disabled', (files.FileList[i].StateString == "Finished" || files.FileList[i].StateString == "Error")
        );
    }
}

/**
* Event that is fired when the Silverlight control's content changes
*/
function CollectionChanged() {
    var files = plugin.Content.Files;
    if (files != undefined && files.FileList.length > 0) {
        RefreshFileList();
    }
}

/**
* Event that is fired when the Silverlight control's file percentage changes
*/
function TotalPercentageChangedEvent(percentage, millisecondsLeft) {
    if (percentage > 0) {
        $('#TotalProgress').html('Total progress percentage: ' + percentage + '%').fadeIn();
    }

    UpdateDynamicContent();
}

/**
* Event that fires when all the files in the file list have been uploaded
*/
function AllFilesFinishedEvent(totalTimeTakenMilliseconds, fileCount) {
    $('#UploadFinished').html('All '
        + fileCount
        + ' files have been uploaded in '
        + totalTimeTakenMilliseconds + ' milliseconds.').fadeIn();

    UpdateProgressBars();
}

// Starts the blob uploader once the Web page has loaded
$(document).ready(function () {
    // Get the Silverlight OBJECT HTML element
    plugin = document.getElementById("SlPlugin");

    $('.uploadButton').click(function () {
        var uploadUrl = $(this).attr('uploadUrl');
        plugin.Content.MainPage.UploadFiles(uploadUrl);
    });
    
    $('#ClearFileList').click(function () {
        plugin.Content.MainPage.ClearList();
    });
    
});

