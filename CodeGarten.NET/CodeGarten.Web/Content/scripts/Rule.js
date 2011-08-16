var RuleController = new (function () {
    this.Init = function (createFormId, structureId, preventEdit) {
        RuleView.Init(createFormId, structureId, preventEdit);
    };

    this.Create = function (callback) {
        RuleView.Create(function (name) {
            ComponentsView.AddItem("Rule", name);
            if (callback)
                callback(name);
        });
    };

    this.Edit = function (name) {
        RuleView.Edit(name);
    };

    this.Delete = function (name) {
        RuleView.Delete(name, function () {
            ComponentsView.RemoveItem("Rule", name);
            TreeController.DeleteRule(name);
        });
    };
});

var RuleView = new (function () {
    this.create = new FormDialog();
    this.edit = new FormDialog();
    var structure;
    var preventEdit;

    this.Init = function (createFormId, structureId, _preventEdit) {
        $(createFormId).find("#service_permissions").tabs();
        this.create.init(createFormId);
        structure = structureId;
        preventEdit = _preventEdit;
    };

    this.Create = function (callback) {
        this.create.Open("Create a new role type", { Name: null }, function (obj) { callback(obj.Name); });
    };

    this.Delete = function (name, callback) {
        DialogConfirmView.open("Delete a rule", "Are you sure you want to delete the rule: " + name, function () {
            $("#main").mask("Loading...", 500);
            $.post("/Rule/Delete?structureId=" + structure + "&name=" + name, null, function (result) {
                if (result.Success) {
                    $("#main").unmask();
                    callback();
                }
            });
        });
    };

    this.Edit = function (name) {
        $("#main").mask("Loading...", 500);
        $.get("/Rule/Edit?structureId=" + structure + "&name=" + name, function (result) {
            var element = $("<div/>").append(result);
            $(element).find("#service_permissions").tabs();

            if (preventEdit) {
                $(element).find("input[type=checkbox]").attr("disabled", "disabled");
                $(element).find("input[type=submit]").remove();
                $(element).children("h2").remove();
            }

            RuleView.edit.init(element);
            $("#main").unmask();

            if (preventEdit)
                RuleView.edit.Open(name + " information", { Name: null });
            else
                RuleView.edit.Open("Edit a rule", { Name: null });
        });
    };

    this.GetWidget = function (rule) {
        var widget = $("<div class='Rule'>" + rule.Name + "<span class='item_options'/></div>");

        var buttonDelete = $("<button title='Delete' onclick='javascript:TreeController.RemoveRule(\"" + rule.Parent.Parent.Name + "\",\"" + rule.Parent.Name + "\",\"" + rule.Name + "\")'/>").button({ icons: { primary: "ui-icon-trash" }, text: false });

        if (!preventEdit)
            $(widget).children(".item_options").append(buttonDelete);

        return widget;
    };
});