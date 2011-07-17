var DialogConfirmView = new (function () {

    var _dialog;
    var _callback;

    this.init = function (dialog) {
        _dialog = $(dialog);

        _dialog.dialog({
            autoOpen: false,
            modal: true,
            resizable: false,
            buttons: {
                "OK": function () {
                    $(this).dialog("close");
                    _callback();
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            }
        });
    };

    this.open = function (title, msg, callback) {
        _callback = callback;
        _dialog.empty();
        _dialog.dialog("option", "title", title);
        _dialog.append("<p>" + msg + "</p>");
        _dialog.dialog("open");
    };

})();