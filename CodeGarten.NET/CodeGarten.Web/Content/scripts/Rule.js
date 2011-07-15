var RuleController = new (function () {
    this.Init = function (createFormId, structureId) {
        RuleView.Init(createFormId, structureId);
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

    this.Init = function (createFormId, structureId) {
        $(createFormId).find("#service_permissions").tabs();
        this.create.init(createFormId);
        structure = structureId;
    };

    this.Create = function (callback) {
        this.create.Open("Create a new role type", { Name: null }, function (obj) { callback(obj.Name); });
    };

    this.Delete = function (name, callback) {
        DialogConfirmView.open("Delete a rule", "Are you sure you want to delete the rule: " + name, function () {
            $.post("/Rule/Delete?structureId=" + structure + "&name=" + name, null, function (result) {
                if (result.Success) {
                    callback();
                    $(this).dialog("close");
                }
            });
        });
    };

    this.Edit = function (name) {
        $.get("/Rule/Edit?structureId=" + structure + "&name=" + name, function (result) {
            var element = $("<div/>").append(result);
            $(element).find("#service_permissions").tabs();
            RuleView.edit.init(element);
            RuleView.edit.Open("Edit a rule", { Name: null });
        });
    };

    this.GetWidget = function (rule) {
        var widget = $("<div class='Rule'>" + rule.Name + "<span class='item_options'/></div>");

        var buttonDelete = $("<button title='Delete' onclick='javascript:TreeController.RemoveRule(\"" + rule.Parent.Parent.Name + "\",\"" + rule.Parent.Name + "\",\"" + rule.Name + "\")'/>").button({ icons: { primary: "ui-icon-trash" }, text: false });

        $(widget).children(".item_options").append(buttonDelete);

        return widget;
    };
});