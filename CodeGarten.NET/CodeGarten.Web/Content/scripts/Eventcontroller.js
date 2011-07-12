var EventController = new (function () {

    var errorField;
    var timer;

    this.Init = function (_errorField) {
        errorField = _errorField;
    };

    this.GlobalError = function (message) {
        var widget = $("<div class='ui-widget'><div class='ui-state-error ui-corner-all'> <div onclick='javascript:EventController.Close();' title='Close' class='ui-icon ui-icon-close'>Close</div> <p><span class='ui-icon ui-icon-alert'></span><strong>Error:</strong> " + message + "</p></div></div>");

        EventController.Close(function () {
            $(errorField).html(widget);
            $(errorField).fadeIn(function () {
                timer = setInterval(function () {
                    EventController.Close();
                }, 5000);
            });
        });
    };

    this.LocalError = function (obj, message) {
        var widget = $("<div class='ui-widget'><div class='ui-state-error ui-corner-all'> <div onclick='javascript:EventController.Close();' title='Close' class='ui-icon ui-icon-close'>Close</div> <p><span class='ui-icon ui-icon-alert'></span><strong>Error:</strong> " + message + "</p></div></div>");

        EventController.Close(function () {
            $(widget).hide();
            $(obj).append(widget);
            $(widget).fadeIn(function () {
                timer = setInterval(function () {
                    EventController.Close();
                }, 5000);
            });
        });
    };

    this.Close = function (callback) {
        clearTimeout(timer);
        $(errorField).fadeOut(callback);
    };

    this.Placeholder = function (msg, size) {
        return "<div class='ui-state-highlight ui-corner-all'><" + size + "><span class='ui-icon ui-icon-info'/>" + msg + "</" + size + "></div>";
    };

})();