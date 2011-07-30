function FormDialog() {

    var _dialog;
    var _callback;
    var _functionClean;
    var _obj;
    var _form;

    var _formError = new ErrorPlaceholder();

    var ApplyError = function (errors) {

        for (var i in errors) {
            var error = errors[i];
            if (error.Field == "form")
                _formError.Error("Error", error.Error);
            else {
//                $.validator(showLabel(error.Field, error.Error), _form);
                var field = _form.find("input[name=\"" + error.Field + "\"]");
                $(field).removeClass("valid").addClass("input-validation-error");
                $(field).next().html("<span for='" + error.Field + "' generated='true' class>" + error.Error + "</span>");
                $(field).next().removeClass("field-validation-valid").addClass("field-validation-error");
            }
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
        _form[0].reset();

        if (_functionClean)
            _functionClean();
    };

    var CreateButtons = function () {

        var butSubmit = _form.find("input[type=\"submit\"]");
        var butReset = _form.find("input[type=\"reset\"]");
        var butSubmitVal = butSubmit.val();
        var butResetVal = butReset.val();

        var obj = {};
        if (butSubmitVal) {
            obj[butSubmitVal] = function () { _form.submit(); };
            butSubmit.remove();
        }
        if (butResetVal) {
            obj[butResetVal] = function () { CleanFields(); };
            butReset.remove();
        }

        return obj;
    };

    this.init = function (dialog, functionClean) {

        if (dialog instanceof jQuery)
            _dialog = dialog;
        else
            _dialog = $(dialog);

        _functionClean = functionClean;

        _form = _dialog.children("form");

        $(_dialog).ajaxStart(function () {
            $(_dialog).parent().mask("Working...", 500);
        });
        $(_dialog).ajaxStop(function () {
            $(_dialog).parent().unmask();
        });

        _dialog.dialog({
            autoOpen: false,
            modal: true,
            resizable: false
        });

        _dialog.dialog("option", "buttons", CreateButtons());

        _dialog.append("<div class='error_form'></div>");

        _formError.init(_dialog.children(".error_form"));
    };

    this.Open = function (title, obj, callback) {
        CleanFields();
        _dialog.dialog("option", "title", title);
        _callback = callback;
        _obj = obj;
        _dialog.dialog("open");
        $(_dialog).children("form").find(".editor-field").children("input").first().removeClass("input-validation-error").addClass("valid");
        $(_dialog).children("form").find(".editor-field").children("input").first().next("span").addClass("field-validation-valid").removeClass("field-validation-error").empty();

    };

    this.OnSuccessCallBack = function (result) {
        //        $(_dialog).parent().unmask();
        if (!result.Success) {
            ApplyError(result.Errors);
            return;
        }

        LoadObj();
        _dialog.dialog("close");

        if (_callback != undefined)
            _callback(_obj);
    };

    this.OnFailCallBack = function () {
        _formError.Error("Error", "Submit Fail");
    };

    this.getForm = function () {
        return _form;
    };

};