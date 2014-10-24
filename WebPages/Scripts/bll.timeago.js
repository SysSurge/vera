/*! Vera VAF 1.0 license: https://github.com/sparkledb/vera */

$(function () {
    $.fn.extend({
        timeAgo: function () {
            _setDateField(this);
        }
    });

    var _numberWords = ['zero', 'one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight', 'nine', 'ten', 'eleven', 'twelve'];
    var _dayOfWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    var _shortDayOfWeek = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
    var _shortMonth = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

    /**
    * Calls the abbr one time in milliseconds time
    * @param {Object} abbr time HTML element
    * @param {Object} milliseconds Number of milliseconds to update abbr element with new timeago
    */
    function _callOnce(abbr, milliseconds) {
        abbr.oneTime(milliseconds,
	            function () {
	                _setDateField($(this));
	            }
	        );
    }

    /**
    * Returns text that tells the day of week
    * @param {Object} date Date
    */
    function _getDayOfWeek(date) {
        return _dayOfWeek[date.getDay()];
    }

    /**
    * Returns a three letter string that tells the day of week
    * @param {Object} date Date
    */
    function _getShortDayOfWeek(date) {
        return _shortDayOfWeek[date.getDay()];
    }

    /**
    * Returns a three letter string that tells the month
    * @param {Object} date Date
    */
    function _getShortMonth(date) {
        return _shortMonth[date.getMonth()];
    }

    /**
    * Returns the time, ex '09:11'
    * @param {Object} date
    */
    function _getTime(date) {
        var minutes = date.getMinutes();
        if (minutes.toString().length == 1) minutes = '0' + minutes;

        var hours = date.getHours();
        if (hours.toString().length == 1) hours = '0' + hours;

        return hours + ':' + minutes;
    }

    function _getWordForNumber(numberVal) {
        if ((numberVal + 1) > _numberWords.length)
            return numberVal;

        return _numberWords[numberVal];
    }

    /**
    * Sets the ISO8601 date of a time element to a more readable for, like 'three hours ago' etc.
    * @param {Object} abbr time HTML element with a ISO8601 date in its datetime attribute
    */
    function _setDateField(abbr) {
        if (abbr.length == 0) return;
        var dateTime = abbr.attr('datetime');

        if (dateTime == undefined || dateTime == null) return;

        var date = $.fn.fromISO8601(dateTime);
        abbr.attr('title', date.toGMTString());

        var now = new Date();

        var diffmilliseconds = now.getTime() - date.getTime();
        var minutes = parseInt(diffmilliseconds / 60000);
        var hours = parseInt(minutes / 60);
        var days = parseInt(hours / 24);
        var weeks = parseInt(days / 7);

        if (days > 30) {
            abbr.html(_getShortDayOfWeek(date) + ', ' + date.getDate() + ' ' + _getShortMonth(date) + ' ' + date.getFullYear());
        }
        else if (weeks >= 1) {
            if (weeks == 1) {
                var daysLeftOver = days - 7;
                if (daysLeftOver == 0) abbr.html('one week ago at ' + _getTime(date));
                else abbr.html(_getDayOfWeek(date) + ' at ' + _getTime(date));
            }
            else {
                abbr.html(_getWordForNumber(weeks) + ' weeks ago at ' + _getTime(date));
            }

            _callOnce(abbr, 86400000);
        }
        else if (days == 1) {
            abbr.html('yesterday at ' + _getTime(date));

            _callOnce(abbr, 3600000);
        }
        else if (days > 1) {
            abbr.html(_getDayOfWeek(date) + ' at ' + _getTime(date));

            _callOnce(abbr, 3600000);
        }
        else {
            if (hours >= 1) {
                if (hours == 1) {
                    var minutesLeftOver = minutes - 60;
                    if (minutesLeftOver == 0) {
                        abbr.html('one hour ago');
                    }
                    else {
                        abbr.html('one hour ' + _getWordForNumber(minutesLeftOver) + ' minutes ago');
                    }
                }
                else {
                    abbr.html(_getWordForNumber(hours) + ' hours ago at ' + _getTime(date));
                }
            }
            else {
                if (minutes >= 1) {
                    if (minutes == 1)
                        abbr.html('one minute ago');
                    else
                        abbr.html(_getWordForNumber(minutes) + ' minutes ago');
                }
                else {
                    abbr.html('just now');
                }
            }

            _callOnce(abbr, 60000);
        }
    }


});