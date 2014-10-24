<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" %>
<script>
	$(document).ready(function () {
	    // Loads a CSS-file dynamically
	    loadCSS = function (href) {
	        var cssLink = $("<link rel='stylesheet' type='text/css' href='" + href + "' />");
	        $("head").append(cssLink);
	    };

	    // Loads a JavaScript-file dynamically
	    loadJS = function (src) {
	        var jsLink = $("<script src='" + src + "' />");
	        $("head").append(jsLink);
	    };

	    var codemirrorInitiated = false;

	    // Loop through all the code blocks and style them appropriately
	    $('.codeblock').each(function (index) {
	        var codeBlockEl = $(this);

            // Default is HTML
	        var metaName = codeBlockEl.attr("metaname") !== undefined ? codeBlockEl.attr("metaname") : "HTML";
	        var metaMime, metaMode;

	        // Only load the Codemirror files before we know it's in use, this saves page load time
	        if (codemirrorInitiated == false) {
	            loadCSS('/Scripts/codemirror-3.20/lib/codemirror.css');
	            loadJS('/Scripts/codemirror-3.20/lib/codemirror-compressed.js');
	            loadJS('/Scripts/codemirror-3.20/mode/meta.js');

	            codemirrorInitiated = true;
	        }

	        // First we need to determine the type of code block
	        CodeMirror.modeInfo.forEach(function (inMeta) {
	            if (inMeta.name == metaName) {
	                // Load the Codemirror JavaScript file associated with this mode
	                metaMode = inMeta.mode; // Mode plain name

                    // Take care of modes with special needs
	                switch (metaName)
	                {
	                    case 'HTML':
	                    {
	                        // HTML mixed-mode depends on the Codemirror XML module
	                        loadJS('/Scripts/codemirror-3.20/mode/xml/xml.js');
	                    }
	                }

	                loadJS('/Scripts/codemirror-3.20/mode/' + metaMode + '/' + metaMode + '.js');

	                // Mode MIME type
	                metaMime = inMeta.mime;

	                var _lineNumbers = codeBlockEl.attr("linenumbers") !== undefined && codeBlockEl.attr("linenumbers") === 'true';
	                var _lineWrapping = codeBlockEl.attr("linewrapping") !== undefined && codeBlockEl.attr("linewrapping") === 'true';
	                var _matchBrackets = codeBlockEl.attr("matchbrackets") !== undefined && codeBlockEl.attr("matchbrackets") === 'true';

	                // Should we style the line?
	                var _styleActiveLine = codeBlockEl.attr("styleactiveline") !== undefined && codeBlockEl.attr("styleactiveline") === 'true';
	                if (_styleActiveLine) {
	                    loadJS('/Scripts/codemirror-3.20/addon/selection/active-line.js');
	                }

	                // Start the Codemirror editor for this DOM element
	                CodeMirror.fromTextArea(codeBlockEl.get(0), {
	                    mode: metaMime,
	                    tabMode: codeBlockEl.attr("tabmode"),
	                    matchBrackets: _matchBrackets,
	                    styleActiveLine: _styleActiveLine,
	                    lineNumbers: _lineNumbers,
	                    lineWrapping: _lineWrapping
	                });
	            }
	        });
	    });
	});
</script>