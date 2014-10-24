/*! Vera VAF 1.0 license: https://github.com/sparkledb/vera */

$(function () {
    $.fn.extend({
        callServiceInterface: function (service, method, data, successDelegate, errorDelegate, completeDelegate) {
            return _postToClient(service, method, data, successDelegate, errorDelegate, completeDelegate);
        }
    });

    $.fn.extend({
        callOAuthServiceInterface: function (url, signature, param, data, successDelegate, errorDelegate, completeDelegate) {
            return _postToClientOAuth(url, signature, param, data, successDelegate, errorDelegate, completeDelegate);
        }
    });

    $.fn.extend({
        getFromRemoteClient: function (address, service, method, data, successDelegate, errorDelegate, completeDelegate, timeout) {
            return _getFromRemoteClient(address, service, method, data, successDelegate, errorDelegate, completeDelegate, timeout);
        }
    });


    function _postToClient(service, method, data, successDelegate, errorDelegate, completeDelegate) {
        var url = "/Interfaces/" + service + ".svc/" + method;

        return $.ajax({
            url: url,
            data: data,
            type: "POST",
            contentType: "application/json",
            timeout: 5000,
            dataType: "json",
            processData: false,
            success: successDelegate,
            error: errorDelegate
        }).always(completeDelegate);
    }

    function _postToClientOAuth(url1, signature, param, data, successDelegate, errorDelegate, completeDelegate) {
        var inParam = param.replace(/\&amp;/g, '&');
        var url = url1 + "?oauth_signature=" + signature + "&" + param;

        return $.ajax({
            url: url,
            data: data,
            type: "POST",
            contentType: "application/json",
            timeout: 5000,
            dataType: "json",
            processData: false,
            success: successDelegate,
            error: errorDelegate
        }).always(completeDelegate);
    }

    function _getFromRemoteClient(address, service, method, data, successDelegate, errorDelegate, completeDelegate, timeout) {
        var url = address + "/Interfaces/" + service + ".svc/" + method;

        return $.ajax({
            url: url,
            data: data,
            type: "GET",
            contentType: "application/json",
            timeout: timeout,
            dataType: "jsonp",
            processData: false,
            success: successDelegate,
            error: errorDelegate
        }).always(completeDelegate);
    }
});
