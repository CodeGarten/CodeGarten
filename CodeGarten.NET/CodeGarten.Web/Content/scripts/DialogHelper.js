var DialogHelper = new (function () {
    var div;
    var callback;

    this.Open = function (divParameter, callbackParameter) {
        div = divParameter;
        callback = callbackParameter;
        if ($(div).children("form")[0])
            $(div).children("form")[0].reset();
        $(div).dialog("open");
    };

    this.OnSuccess = function (errors) {
        if (errors == true) {
            if (callback)
                callback($(div).children("form").find("input[name=Name]").val());
            $(div).dialog("close");
        }
        else
            for (var v in errors) {
                $(div).children("form").find("input[name=" + errors[v].Field + "]").next().text(errors[v].Error);
                $(div).children("form").find("input[name=" + errors[v].Field + "]").removeClass("valid").addClass("input-validation-error");
            }
    };
});