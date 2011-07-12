function ErrorPlaceholder() {
    var _blockError;
    var _block;
    var htmlContent = function (title, msg, icon) {
        var iconHtml = "";
        if(icon)
            iconHtml = "<span class='ui-icon ui-icon-alert' />";
        return "<p>"+iconHtml+"<strong>"+title+": </strong>"+msg+"</p>"
    
    };

    this.init = function (block) {
        _block = block;
        _blockError = block + " > .ui-state-error";
        $(block).addClass("ui-widget").hide();
        $(block).empty().html("<div class='ui-state-error ui-corner-all'></div>");
    };

    this.Error = function (title, msg) {
        $(_blockError).empty()
        $(_blockError).append(htmlContent(title, msg, true));
        $(_block).toggle();
    };

    this.HideError = function () {
        $(_block).hide();
    };

};