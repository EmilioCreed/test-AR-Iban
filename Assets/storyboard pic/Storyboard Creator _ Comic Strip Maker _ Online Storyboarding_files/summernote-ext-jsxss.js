/**
 * Summernote HtmlSanitize
 *
 * This is a plugin for the Summernote (www.summernote.org) WYSIWYG editor.
 * Sanitizes HTML tags and attributes while pasting content in editor.
 *
 * Inspired from https://gist.github.com/hiteshaggarwal/388cd3fae7331aa0415c63e0a883e8f5
 * Leverages jsxss (https://jsxss.com/)
 * 
 * @author Seb Coursol
 *
 */

(function (factory) {
    /* global define */
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(['jquery'], factory);
    } else if (typeof module === 'object' && module.exports) {
        // Node/CommonJS
        module.exports = factory(require('jquery'));
    } else {
        // Browser globals
        factory(window.jQuery);
    }

}(function ($) {
    var buildXssOptions = function($jsxssOptions) {
        var result = {
            stripIgnoreTag: $jsxssOptions.stripIgnoreTag,
            stripIgnoreTagBody: $jsxssOptions.stripIgnoreTagBody,
            allowCommentTag: $jsxssOptions.allowCommentTag
        };
        if ($jsxssOptions.whiteList != null) result.whiteList = $jsxssOptions.whiteList;
        if ($jsxssOptions.onTag != null) result.onTag = $jsxssOptions.onTag;
        if ($jsxssOptions.onTagAttr != null) result.onTagAttr = $jsxssOptions.onTagAttr;
        if ($jsxssOptions.onIgnoreTag != null) result.onIgnoreTag = $jsxssOptions.onIgnoreTag;
        if ($jsxssOptions.onIgnoreTagAttr != null) result.onIgnoreTagAttr = $jsxssOptions.onIgnoreTagAttr;
        if ($jsxssOptions.escapeHtmlOverride != null) result.escapeHtmlOverride = $jsxssOptions.escapeHtmlOverride;
        if ($jsxssOptions.safeAttrValue != null) result.safeAttrValue = $jsxssOptions.safeAttrValue;
        return result;
    };

    $.extend($.summernote.options, {
        'jsxss': {
            whiteList: null,
            stripIgnoreTag: true,
            stripIgnoreTagBody: true,
            allowCommentTag: false,
            onTag: null,
            onTagAttr: null,
            onIgnoreTag: null,
            onIgnoreTagAttr: null,
            escapeHtmlOverride: null,
            safeAttrValue: null,
            onAfterFilterXSS: function ($html) {
                return $html;
            }
        }
    });

    $.extend($.summernote.plugins, {
        'jsxss': function (context) {
            var $note = context.layoutInfo.note;
            var $options = context.options;
            var xssProcessor = new filterXSS.FilterXSS(buildXssOptions($options.jsxss));
            $note.on('summernote.paste', function (e, evt) {
                var text = evt.originalEvent.clipboardData.getData('text/plain'),
                    html = evt.originalEvent.clipboardData.getData('text/html');
                if (html) {
                    evt.preventDefault();
                    html = xssProcessor.process(html.toString());
                    var $html = $('<div/>').html(html || text);
                    if (typeof $options.onAfterFilterXSS === "function") {
                        $html = $options.onAfterFilterXSS($html);
                    }
                    var cleanHtml = $html.html().trim().replace(/>[\s]*/gi, '>').replace(/[\s]*</gi, '<').replace(/[\r\n]/gi, '');
                    $note.summernote('pasteHTML', cleanHtml);
                    return false;
                }
                // If we didn't receive HTML, let the standard handling run: this will be pasted as text
            });
        }
    });

}));

// Usage example:
/*
(function ($) {
    $(function () {
        $('.summernote').summernote({
            toolbar: [
                ['custom', ['striptags']],
            ],
            jsxss: {
                whiteList: {
                    b: [],
                    big: [],
                    blockquote: ["cite"],
                    br: [],
                    caption: [],
                    center: [],
                    cite: [],
                    code: [],
                    col: ["align", "valign", "span", "width"],
                    colgroup: ["align", "valign", "span", "width"],
                    dd: [],
                    del: ["datetime"],
                    details: ["open"],
                    div: [],
                    dl: [],
                    dt: [],
                    em: [],
                    font: ["color", "size", "face"],
                    footer: [],
                    h1: [],
                    h2: [],
                    h3: [],
                    h4: [],
                    h5: [],
                    h6: [],
                    header: [],
                    hr: [],
                    i: [],
                    ins: ["datetime"],
                    li: [],
                    mark: [],
                    nav: [],
                    ol: [],
                    p: [],
                    pre: [],
                    s: [],
                    section: [],
                    small: [],
                    span: [],
                    sub: [],
                    sup: [],
                    strong: [],
                    table: ["width", "border", "align", "valign"],
                    tbody: ["align", "valign"],
                    td: ["width", "rowspan", "colspan", "align", "valign"],
                    tfoot: ["align", "valign"],
                    th: ["width", "rowspan", "colspan", "align", "valign"],
                    thead: ["align", "valign"],
                    tr: ["rowspan", "align", "valign"],
                    tt: [],
                    u: [],
                    ul: [],
                },
                stripIgnoreTag: true,
                stripIgnoreTagBody: true,
                allowCommentTag: false,
                onTag: null,
                onTagAttr: null,
                onIgnoreTag: null,
                onIgnoreTagAttr: null,
                escapeHtmlOverride: null,
                safeAttrValue: null,
                onAfterFilterXSS: function ($html) {
                    $html.find('table').addClass('table');
                    return $html;
                }
            }
        });
    });
})(jQuery);
 */