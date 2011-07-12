function FormDialog() {

    var _dialog;
    var _form;
    var _callback;
    var _functionClean;
    var _obj;

    var _formError = new ErrorPlaceholder();

    var ApplyError = function (errors) {

        for (var i in errors) {
            var error = errors[i];
            if (error.Field == "form")
                _formError.Error("Error", error.Error);
            else
                $(_form + " input[name=\"" + error.Field + "\"]").val(error.Error);
        }

    };

    var LoadObj = function () {
        var inputs = $(_form).serializeArray();
        for (var i in inputs) {
            var input = inputs[i];
            if (input.name in _obj)
                _obj[input.name] = input.value;
        }
    };

    var CleanFields = function () {
        _formError.HideError();
        $(_form)[0].reset();

        if (_functionClean)
            _functionClean();
    };

    var CreateButtons = function () {

        var butSubmit = $(_form + " input[type=\"submit\"]");
        var butReset = $(_form + " input[type=\"reset\"]");
        var butSubmitVal = butSubmit.val();
        var butResetVal = butReset.val();

        var obj = "({";
        if (butSubmitVal) {
            obj += butSubmitVal + ": function () {$(_form).submit();},";
            butSubmit.remove();
        }
        if (butResetVal) {
            obj += butResetVal + ": function () {CleanFields();},";
            butReset.remove();
        }
        obj.substring(0, obj.length - 1);

        obj += "})"
        return eval(obj);
    };

    this.init = function (dialog, functionClean) {
        _functionClean = functionClean;
        _dialog = dialog;
        _form = dialog + " > form";

        $(_dialog).dialog({
            autoOpen: false,
            modal: true
        });

        $(_dialog).dialog("option", "buttons", CreateButtons());

        $(_dialog).append("<div class='error_form'></div>");
        _formError.init(_dialog + " > .error_form");
    };

    this.Open = function (title, obj, callback) {
        CleanFields();
        $(_dialog).dialog("option", "title", title);
        _callback = callback;
        _obj = obj;
        $(_dialog).dialog("open");
    };

    this.OnSuccessCallBack = function (result) {

        if (result.Errors.length > 0) {
            ApplyError(result.Errors);
            return;
        }

        LoadObj();
        $(_dialog).dialog("close");
        _callback(_obj);
    };

    this.OnFailCallBack = function () {
        _formError.Error("Error", "Submit Fail");
    };
};