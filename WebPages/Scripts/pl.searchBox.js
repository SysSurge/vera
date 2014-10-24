/*! Vera VAF 1.0 license: https://github.com/sparkledb/vera */

$(document).ready(function () {
    var jailedImages = [];
    var queryResults = [];
    var isCollapsed = true;
    var queriesInProgress = false;
    var lastNumberOfHits = 0;
    var queuedQuery = '';

    function initRssButton() {
        var rssIconHover = $('a.rssIconHover');

        // IE < 8 fix
        rssIconHover.css('opacity', '0.0');
        rssIconHover.css('display', 'block');

        rssIconHover.hover(
            function () {
                $(this).stop(true, false);
                $(this).animate({
                    'opacity': '1.0'
                }, 400);
            },
            function () {
                $(this).stop(true, false);
                $(this).animate({
                    'opacity': '0.0'
                }, 400);
            }
        );
    }

    initRssButton();

    var input = $('input.searchBox');

    function getJailImage(imageUrl) {
        var imageMarkup = '';

        if (imageUrl != null && $.trim(imageUrl) != '') {

            if (jailedImages[imageUrl] == null) {
                jailedImages[imageUrl] = true;

                imageMarkup = '<figure><img data-href="'
                    + imageUrl
                        + '" src="/Images/blank.gif" class="queryResultJailImage" alt="" /></figure>';
            } else {
                imageMarkup = '<figure><img alt="" src="'
                    + imageUrl
                        + '" /></figure>';
            }

        }

        return imageMarkup;
    }

    function showDynamicQueryResults(queryResults, isCached) {
        var searchResult = $('#searchResult');

        lastNumberOfHits = queryResults.Items.length;


        if (queryResults.Items.length > 0) {
            var hideMoreButton;
            var searchResultHitsText;
            if (queryResults.NumberOfHits == queryResults.Items.length) {
                hideMoreButton = true;
                if (queryResults.NumberOfHits == 1) {
                    searchResultHitsText = "Found 1 page:";
                } else {
                    searchResultHitsText = "Found "
                        + queryResults.NumberOfHits
                            + " pages:";
                }
            } else {
                hideMoreButton = false;
                searchResultHitsText = "Showing the top "
                    + queryResults.Items.length
                        + " of "
                            + queryResults.NumberOfHits
                                + " pages:";
            }


            var markup = '<article><header><h1 class="searchResultHitsText">'
                + searchResultHitsText
                    + '</h1></header><section>';

            for (var i = 0; i < queryResults.Items.length; i++) {

                var image = getJailImage(queryResults.Items[i].ImageUrl);


                markup += '<div class="searchResultArticle"><article><header><h1><a href="'
                    + queryResults.Items[i].Url
                        + '">'
                            + queryResults.Items[i].Title
                                + '</a></h1></header><div class="searchResultText"><a href="'
                                    + queryResults.Items[i].Url
                                        + '">'
                                            + image
                                                + '<span>'
                                                    + queryResults.Items[i].Text
                                                        + '</span></a></div></article></div>';
            }
            markup += '</section><div class="searchResultFooter clear"><footer><a id="moreQueryResults" href="#">More...</a><a class="queryHelp" href="/Help/search-query-syntax.aspx">Query syntax guide</a></footer></div></article>';

            searchResult.html(markup);

            if (!isCached) {
                $('img.queryResultJailImage').jail({ effect: 'fadeIn' });
            }

            if (hideMoreButton) {
                $('#moreQueryResults').hide();
            } else {
                $('#moreQueryResults').click(function () {
                    submitQuery(input.val());
                });
            }

            searchResult.fadeIn(200);
        } else {
            searchResult.fadeOut(200);
        }
    }

    function queryComplete() {
        queriesInProgress = false;
        $('#loadingIcon').hide();
        $('#clearIcon').show();

        if (queuedQuery != '') {
            runDynamicQuery(queuedQuery);
        }
    }

    var previousQuery = '';

    function runDynamicQuery(queryTerms) {
        if (queryTerms == previousQuery) return;
        previousQuery = queryTerms;

        queuedQuery = '';
        queryTerms = $.trim(queryTerms);

        if (queryResults[queryTerms] == null) {
            queriesInProgress = true;

            $('#loadingIcon').show();
            $('#clearIcon').hide();

            $.fn.callServiceInterface('RestApi', 'Query',
                '{"queryTerms":"' + queryTerms.replace('"', '\"') + '"}',
                function (result) {
                    var parsedResult = $.parseJSON(result);

                    showDynamicQueryResults(parsedResult, false);
                    queryResults[queryTerms] = parsedResult;
                }
                , function () { }
                , function () {
                    queryComplete();
                }
            );

        } else {
            showDynamicQueryResults(queryResults[queryTerms], true);
        }
    }

    function fadeIn(input) {
        if (input.val().length == 0) {
            isCollapsed = true;

            $('#searchIcon').animate({
                color: '#aaaaaa'
            });

            $('#searchBoxBackground').animate({
                opacity: 0.5
            });

            $('#searchBox').animate({
                width: '12.5em'
            });

            var firstMenuItem = $('div.level1MenuContainer > div.level1Menu > div.menu > ul > li:first-child > a');

            $('div.level1MenuContainer > div.level1Menu > div.menu > ul > li > a').each(function () {

                if ($(this)[0] != firstMenuItem[0]) {
                    $(this).animate({
                        'padding-left': '1em',
                        'padding-right': '1em'
                    });
                }

            });
        }
        var searchResult = $('#searchResult');
        searchResult.stop(true, true);
        searchResult.fadeOut(200);
    }

    function fadeOutAnimate() {
        isCollapsed = false;

        $('#searchIcon').animate({
            color: '#000000'
        });

        $('#searchBoxBackground').animate({
            opacity: 1.0
        });

        $('#searchBox').animate({
            width: '17.857em'
        });

        var firstMenuItem = $('div.level1MenuContainer > div.level1Menu > div.menu > ul > li:first-child > a');

        $('div.level1MenuContainer > div.level1Menu > div.menu > ul > li > a').each(function () {

            if ($(this)[0] != firstMenuItem[0]) {
                $(this).animate({
                    'padding-left': '0.5em',
                    'padding-right': '0.5em'
                });
            }

        });

    }

    function fadeOut(input) {
        if (input.val().length == 0) {
            fadeOutAnimate();
        }

        if (lastNumberOfHits > 0) {
            var searchResults = $('#searchResult');
            searchResults.stop(true, true);
            searchResults.fadeIn(200);
        }
    }

    function showOrHideClearButton(input) {
        if (isCollapsed) {
            $('#clearIcon').hide();
        } else if (input.val().length == 0) {
            $('#clearIcon').animate({
                opacity: 0.0
            }, 'fast');
        } else {
            $('#clearIcon').animate({
                opacity: 1.0
            }, 'fast');
        }
    }

    function submitQuery(query) {
        window.location.href = "/Search.aspx?query=" + encodeURIComponent(query);
    }

    $('#helpText').fadeIn(1000);

    input.keypress(function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code == 10 || code == 13) {
            submitQuery(input.val());
            e.preventDefault();
        }
    });

    showOrHideClearButton(input);

    if (input.val().length > 0) {
        fadeOutAnimate();
    }

    input.focus(function () {
        fadeOut($(this));
    });

    input.focusout(function () {
        if (!queriesInProgress) {
            fadeIn($(this));
        }
    });

    input.keyup(function () {
        if (!queriesInProgress) {
            showOrHideClearButton($(this));

            if (input.val().length > 0) {
                runDynamicQuery(input.val());
            }
        } else if (input.val().length > 0) {
            queuedQuery = input.val();
        }

    });

    $('#searchIcon').click(function () {
        if (input.val().length > 0) {
            submitQuery(input.val());
        } else {
            input.focus();
        }
    });

    $('#clearIcon').click(function () {
        input.val('');
        showOrHideClearButton($(this));

        lastNumberOfHits = 0;

        input.focus();
    });

});