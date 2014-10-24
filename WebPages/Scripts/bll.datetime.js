/*! Vera VAF 1.0 license: https://github.com/sparkledb/vera */
$(function () {
    $.fn.extend({
        fromISO8601: function (isoDate) {
            return _fromISO8601(isoDate);
        }
		,
        toIso8601Date: function (inDate) {
            return _toIso8601Date(inDate);
        }
    });

    /**
    * Converts a ISO8601 date to a Date object
    * @param {String} isoDate A ISO8601 date. Example if expected input format: "2010-05-30T11:55:36.870"
    */
    function _fromISO8601(isoDate) {
        var dateAndOffset = isoDate.split('Z');

        var parts = dateAndOffset[0].split('T'),
		dateParts = parts[0].split('-'),
		timeParts = parts[1].split('.'),
		timeSubParts = timeParts[0].split(':'),
		timeSecParts = timeSubParts[2].split('.'),
		timeHours = Number(timeSubParts[0]),
		convertedDate = new Date;

        convertedDate.setUTCFullYear(Number(dateParts[0]));
        convertedDate.setUTCMonth(Number(dateParts[1]) - 1);
        convertedDate.setUTCDate(Number(dateParts[2]));
        convertedDate.setUTCHours(Number(timeHours));
        convertedDate.setUTCMinutes(Number(timeSubParts[1]));
        convertedDate.setUTCSeconds(Number(timeSecParts[0]));
        if (timeSecParts[1]) convertedDate.setUTCMilliseconds(Number(timeSecParts[1]));

        return convertedDate;
    }

    /**
    * Converts a Date object to a ISO8601 date string 
    * @param {Date} inDate A date object.
    */
    function _toIso8601Date(inDate) {
        var year = inDate.getUTCFullYear();
        if (year < 2000) // Y2K Fix, Isaac Powell
            year = year + 1900; // http://onyx.idbsu.edu/~ipowell
        var month = inDate.getUTCMonth() + 1;
        var day = inDate.getUTCDate();
        var hour = inDate.getUTCHours();
        var minute = inDate.getUTCMinutes();
        var second = inDate.getUTCSeconds();
        if (month <= 9) month = "0" + month;
        if (day <= 9) day = "0" + day;
        if (hour <= 9) hour = "0" + hour;
        if (minute <= 9) minute = "0" + minute;
        if (second <= 9) second = "0" + second;
        time = year + "-" + month + "-" + day + "T"
			+ hour + ":" + minute + ":" + second;

        return time;
    }
});