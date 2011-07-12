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
            ContainerPrototypeController.DeleteRule(name);
        });
    };
});

var RuleView = new (function () {
    var create;
    var structure;

    this.Init = function (createFormId, structureId) {
        create = $(createFormId);
        structure = structureId;
        $(create).hide();
    };

    this.Create = function (callback) {
        $(create).find("#service_permissions").tabs();
        $(create).children("form").find("input[type=checkbox]").button();
        $(create).dialog({
            autoOpen: false, modal: true, draggable: false, resizable: false, title: "Create a new rule",
            buttons: {
                "Create": function () { $(create).children("form").submit(); },
                "Cancel": function () { $(create).dialog("close"); }
            }
        });
        
        DialogHelper.Open(create, callback);
    };

    this.Delete = function (name, callback) {
        var del = $("<div/>").load("/Rule/Delete?structureId=" + structure + "&name=" + name).dialog({
            autoOpen: false, modal: true, draggable: false, resizable: false, title: "Delete a rule",
            buttons: {
                "Delete": function () { $(this).children("form").submit(); },
                "Cancel": function () { $(this).dialog("destroy"); }
            }
        });

        DialogHelper.Open(del, callback);
    };

    this.Edit = function (name, callback) {
        var edit = $("<div/>").load("/Rule/Edit?structureId=" + structure + "&name=" + name, function () { $(this).find("#service_permissions").tabs(); }).dialog({
            autoOpen: false, modal: true, draggable: false, resizable: false, title: "Edit a rule",
            buttons: {
                "Save": function () { $(this).children("form").submit(); },
                "Cancel": function () { $(this).dialog("destroy"); }
            }
        });

        DialogHelper.Open(edit, callback);
    };

    this.GetWidget = function (rule) {
        var widget = $("<div class='Rule'>" + rule.Name + "<span class='item_options'/></div>");

        var buttonDelete = $("<button title='Delete' onclick='javascript:ContainerPrototypeController.RemoveRule(\"" + rule.Parent.Parent.Name + "\",\"" + rule.Parent.Name + "\",\"" + rule.Name + "\")'/>").button({ icons: { primary: "ui-icon-trash" }, text: false });

        $(widget).children(".item_options").append(buttonDelete);

        return widget;
    };
});