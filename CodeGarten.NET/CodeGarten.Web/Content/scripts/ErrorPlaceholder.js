function ErrorPlaceholder() {
    var _blockError;
    var _block;
    var htmlContent = function (title, msg, icon) {
        var iconHtml = "";
        if (icon)
            iconHtml = "<span class='ui-icon ui-icon-alert' />";
        return "<p>" + iconHtml + "<strong>" + title + ": </strong>" + msg + "</p>";

    };

    this.init = function (block) {
        if (block instanceof jQuery)
            _block = block;
        else
            _block = $(block);

        _block.addClass("ui-widget").hide();
        _block.empty().html("<div class='ui-state-error ui-corner-all'></div>");
        _blockError = _block.children(".ui-state-error");
    };

    this.Error = function (title, msg) {
        _blockError.empty();
        _blockError.append(htmlContent(title, msg, true));
        _block.show();
    };

    this.HideError = function () {
        _block.hide();
    };

};