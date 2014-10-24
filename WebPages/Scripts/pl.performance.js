/*! Vera VAF 1.0 license: https://github.com/sparkledb/vera */

$(document).ready(function () {

    $.fn.extend({
        performance: function (nodeIpAddresses, password) {
            _initGraphics(this, 1000, nodeIpAddresses, password);
        }
    });

    $(document).ready(function () {
        // Convert the CSV containing all the cloud node IP addresses into a JS array
        var nodeIpAddresses = _processCsv($('#MainContent_nodeList').text());
        var password = $('#MainContent_cloudPassword').text();

        // Show performance elements
        $('canvas.performance').each(function () {
            $(this).performance(nodeIpAddresses, password);
        });
    });


    function _addColumnText(ctx, inText, leftPost, topPos) {
        ctx.fillText(inText, leftPost, topPos);
    }

    function _updateErrorColumn(ctx, leftPost, topPos) {
        ctx.fillStyle = "red";

        var procLeft = leftPost + 20;
        var procTop = topPos + 20;

        ctx.clearRect(procLeft, procTop, 200, 100);

        ctx.fillRect(procLeft, procTop, 50, 100);

        var textLeft = procLeft + 70;
        var textTop = procTop + 50;

        _addColumnText(ctx, "Offline", textLeft, textTop)

    }
    
    function _updateColumn(parsedResult, ctx, leftPost, topPos) {
        // Create gradient
        var my_gradient = ctx.createLinearGradient(leftPost, topPos, leftPost + 0, topPos + 150);
        my_gradient.addColorStop(1, "green");
        my_gradient.addColorStop(0.5, "yellow");
        my_gradient.addColorStop(0, "red");
        ctx.fillStyle = my_gradient;

        var procPercentage = parseInt(parsedResult.ProcTime, 10);
        
        var procLeft = leftPost + 20;
        var procTop = topPos + 20;

        ctx.clearRect(procLeft, procTop, 200, 100);
        var leftOver = 100 - procPercentage;
        ctx.fillRect(procLeft, procTop + leftOver, 50, 100 - leftOver);

        // Add some text

        var textLeft = procLeft + 60;
        var textTop = procTop + 30;

        ctx.fillStyle = "cyan";
        _addColumnText(ctx, "Proc. load:", textLeft, textTop)
        textTop += 20;
        _addColumnText(ctx, procPercentage + "%", textLeft, textTop)

        textTop += 30;
        _addColumnText(ctx, "Avail. mem:", textLeft, textTop)
        textTop += 20;
        _addColumnText(ctx, parsedResult.AvailMemory, textLeft, textTop)

        // Draw the base line
        var lineLeft = leftPost + 15;
        var lineTop = topPos + 122;
        ctx.fillStyle = "black";
        ctx.beginPath();
        ctx.moveTo(lineLeft, lineTop);
        ctx.lineTo(lineLeft + 60, lineTop);
        ctx.stroke();
    }

    /**
    * Run a RESTful JSONP query against a cloud node and get performance information
    * @param {Object} ipAddress IP address to cloud node
    */
    function _updateColumns(ipAddress, ctx, leftPost, topPos, password) {
        $.fn.getFromRemoteClient("http://" + ipAddress, 'RestApi', 'PerformanceCounter',
            'samplingIntervalMs=100&apiKey=' + password,
            function (result) {
                // Parse the returned JSONP
                var parsedResult = $.parseJSON(result);

                // Update a column
                _updateColumn(parsedResult, ctx, leftPost, topPos);
            }
            , function () {
                // Update a column with error
                _updateErrorColumn(ctx, leftPost, topPos);

            }
            , function () { }, 900
        );
    }

    /**
    * Calls the canvas one time in milliseconds time
    * @param {Object} cnv Canvas HTML element
    * @param {Object} milliseconds Number of milliseconds to update abbr element with new timeago
    */
    function _callOnce(c, milliseconds, nodeIpAddresses, colWidth, rowHeight, maxColumns, password) {
        c.oneTime(milliseconds,
	            function () {
	                _drawGraphics(c, milliseconds, nodeIpAddresses, colWidth, rowHeight, maxColumns, password);
	            }
	        );
    }

    /**
    * Converts the CSV into a JS array
    * @param {Object} allText CSV
    */
    function _processCsv(allText) {
        var nodeIpAddresses = allText.split(',');
        return nodeIpAddresses;
    }

    /**
    * Updates the canvas with the changes
    * @param {Object} c Cancas HTML element
    * @param {Object} milliseconds Number of milliseconds to wait until we update the canvas 
    * @param {Array} Array containing all the node IP addresses
    */
    function _drawGraphics(c, milliseconds, nodeIpAddresses, colWidth, rowHeight, maxColumns, password)
    {
        var ctx = c[0].getContext("2d");

        var leftPost = 0, topPos = 0;

        for (var i = 0 ; i < nodeIpAddresses.length; i++)
        {
            _updateColumns(nodeIpAddresses[i], ctx, leftPost, topPos, password);

            if ((i + 1) % maxColumns == 0)
            {
                leftPost = 0;
                topPos += rowHeight;
            }
            else
            {
                leftPost += colWidth;
            }
        }

        _callOnce(c, milliseconds, nodeIpAddresses, colWidth, rowHeight, maxColumns, password);
    }

    /**
    * Updates the canvas with the changes
    * @param {Object} c Cancas HTML element
    * @param {Object} milliseconds Number of milliseconds to wait until we update the canvas 
    * @param {Array} Array containing all the node IP addresses
    */
    function _initGraphics(c, milliseconds, nodeIpAddresses, password) {
        var colWidth = 200;
        var rowHeight = 200;
        var maxColumns = 8;

        if (nodeIpAddresses.length > maxColumns)
        {
            c[0].width = colWidth * maxColumns;
        }
        else
        {
            c[0].width = nodeIpAddresses.length * colWidth;
        }

        var numRows =  Math.floor(nodeIpAddresses.length / maxColumns);
        if (nodeIpAddresses.length % maxColumns != 0)
        {
            numRows++;
        }

        c[0].height = numRows * rowHeight;

        var ctx = c[0].getContext("2d");

        ctx.font = "20px Arial";
        ctx.fillStyle = '#fff';

        var x = 20;
        var y = 150;
        for (var i = 0 ; i < nodeIpAddresses.length; i++) {
            _addColumnText(ctx, nodeIpAddresses[i], x, y);

            if ((i + 1) % maxColumns == 0)
            {
                x = 20;
                y += rowHeight;
            }
            else
            {
                x += colWidth;
            }
        }

        _drawGraphics(c, milliseconds, nodeIpAddresses, colWidth, rowHeight, maxColumns, password);
    }

});
