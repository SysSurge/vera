/*! Vera VAF 1.0 license: https://github.com/sparkledb/vera */

/* Interfaces */
$(function () {
    $.fn.extend({
        bashSimulator: function () {
            return _bashSimulator($(this));
        }
    });

    function _bashSimulator(elem) {
        var isRunning = false, isPaused = false, fastForward = false;
        var prompt, usr, domain;

        function scrollToBottom(wnd) {
            var height = wnd[0].scrollHeight;
            wnd.scrollTop(height);
        }

        function addOutput(s, wnd) {
            if (!isRunning) return;
            $('<div>').text(s).appendTo(wnd);
            scrollToBottom(wnd);
            return Q.delay(1);
        }
        
        function echoMessage(s, wnd)
        {
            wnd.append('<div class="echo">&gt;&gt;&gt; '
                + s.substr(6)
                + ' &gt;&gt;&gt;</div>'
                );
            return Q.delay(1);
        }

        function addInput2(s, wnd) {
            if (!isRunning) return;

            if (s.substr(0, 6) == ' echo ') {
                return echoMessage(s, wnd);
            }
            else
            {
                return addInput(s, wnd);
            }
        }

        function addInput(s, wnd) {
            var l = $('.prompt:last');
            var e = $('<span>').addClass('cmd').appendTo(l);
            if (fastForward)
            {
                e.append(s);
                return Q.delay(1);
            }
            else
            {
                return addLettersRecursive(e, s);
            }
        }

        function addPrompt(wnd) {
            if (!isRunning) return;
            var l = $('<div>').text(prompt).addClass('prompt').appendTo(wnd);
            scrollToBottom(wnd);
            return Q.delay( fastForward ? 1 : 400);
        }

        function addLettersRecursive(container, s) {
            container.append(s.charAt(0));
            var row_complete = Q.defer();
            Q.delay( 50).then(function () {
                if (s.length <= 1) {
                    row_complete.resolve();
                }
                addLettersRecursive(container, s.substr(1)).then(function () {
                    row_complete.resolve();
                })
            });
            return row_complete.promise;
        }

        function simStopped()
        {
            fastForward = false;
            isPaused = false;
            isRunning = false;
            mnuStop.hide();
            mnuPlay.show();
            mnuPause.hide();
            mnuFFwd.hide();
        }

        function processLoop(startBtn, wnd, lines) {
            isRunning = true;

            // Hide the title
            startBtn.fadeOut(function () {
                bashWin.fadeIn(function () {
                    var promise = new Q();

                    promise = promise.then(function () {
                        lines.forEach(function (item) {

                            // Check if user has paused
                            promise = promise.then(function pollUntil() {
                                return Q.delay(100).then(function () {
                                    return isPaused ? pollUntil() : true;
                                });
                            });

                            if (item[0] == '%') {
                                promise = promise.then(function ()
                                { return addPrompt(wnd); })
                                promise = promise.then(function ()
                                { return addInput2(item.substr(1), wnd); })
                            }
                            else {
                                promise = promise.then(function ()
                                { return addOutput(item, wnd); })
                            }
                        });
                        promise = promise.then(function ()
                        {
                            var retProm = addPrompt(wnd);
                            simStopped();
                            return retProm;
                        })
                    });
                    promise.done();
                });
            });
        }

        // Hide the textarea HTML element
        elem.hide();

        // Set the Bash prompt
        usr = elem.attr('user');
        domain = elem.attr('domain');
        prompt = usr + '@' + domain + ' %';

        // Get the shell title
        var title = elem.attr('title');
        var bashWin = $('<div style="display:none" class="bash"></div>').insertBefore(elem);

        // Add a title to the Bash shell
        var winHead = $('<div id="head">'
            + title
            + '</div>');
        bashWin.append(winHead);

        // Add a new element to hold the bash text
        var wnd = $('<div id="wnd"></div>');
        bashWin.append(wnd);

        // Get all the lines in the textarea an scan through them
        var lines = elem.val().split('\n');

        // Add a start Bash simulator button
        var startBtn = $('<p class="bashAnchor"><i class="fa fa-cog"></i> ' + title + '</p>');
        // Add a start bash simulation button
        startBtn.insertBefore(elem).click(function () {
            mnuStop.show();
            mnuPlay.hide();
            mnuPause.show();
            mnuFFwd.show();
            wnd.empty();
            processLoop($(this), wnd, lines);
        });

        // Add a close button
        var mnuClose = $('<i title="Close" style="float:right" class="fa fa-times"></i>');
        mnuClose.click(function () {
            bashWin.fadeOut(function () {
                isRunning = false;
                isPaused = false;
                startBtn.fadeIn();
            });
        });
        winHead.append(mnuClose);

        // Add a fast-forward button
        var mnuFFwd = $('<i title="Fast-forward" style="float:right" class="fa fa-fast-forward"></i>');
        mnuFFwd.click(function () {
            fastForward = true;
        });
        winHead.append(mnuFFwd);

        // Add a stop button
        var mnuStop = $('<i title="Stop" style="float:right" class="fa fa-stop"></i>');
        mnuStop.click(function () {
            simStopped();
        });
        winHead.append(mnuStop);

        // Add a pause button
        var mnuPause = $('<i title="Pause" style="float:right" class="fa fa-pause"></i>');
        mnuPause.click(function () {
            isPaused = true;
            $(this).hide();
            mnuPlay.show();
            fastForward = false;
            mnuFFwd.hide();
        });
        winHead.append(mnuPause);

        // Add a play button
        var mnuPlay = $('<i title="Play" style="float:right;display:none" class="fa fa-play"></i>');
        mnuPlay.click(function () {
            isPaused = false;
            $(this).hide();
            mnuPause.show();
            mnuStop.show();

            fastForward = false;
            mnuFFwd.show();

            if (!isRunning) {
                wnd.empty();
                isRunning = true;
                processLoop(startBtn, wnd, lines);
            }
        });
        winHead.append(mnuPlay);
    }
});

$(document).ready(function () {
    /* Process all Bash simulator elements */
    $('.bashSim').each(function () {
        $(this).bashSimulator();
    });
});
