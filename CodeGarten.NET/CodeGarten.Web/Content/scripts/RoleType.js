var RoleTypeController = new (function () {
    this.Init = function (createFormId, structureId) {
        RoleTypeView.Init(createFormId, structureId);
    };

    this.Create = function (callback) {
        RoleTypeView.Create(function (name) {
            ComponentsView.AddItem("RoleType", name);
            if (callback)
                callback(name);
        });
    };

    this.Edit = function (name) {
        RoleTypeView.Edit(name);
    };

    this.Delete = function (name) {
        RoleTypeView.Delete(name, function () {
            ComponentsView.RemoveItem("RoleType", name);
            ContainerPrototypeController.DeleteRoleType(name);
        });
    };
});

var RoleTypeView = new (function () {
    var create;
    var structure;

    this.Init = function (createFormId, structureId) {
        create = $(createFormId);
        structure = structureId;
        $(create).hide();
    };

    this.Create = function (callback) {
        $(create).dialog({
            autoOpen: false, modal: true, draggable: false, resizable: false, title: "Create a new role type",
            buttons: {
                "Create": function () { $(create).children("form").submit(); },
                "Cancel": function () { $(create).dialog("close"); }
            }
        });
        
        DialogHelper.Open(create, callback);
    };

    this.Delete = function (name, callback) {
        var del = $("<div/>").load("/RoleType/Delete?structureId="+structure+"&name=" + name).dialog({
            autoOpen: false, modal: true, draggable: false, resizable: false, title: "Delete a role type",
            buttons: {
                "Delete": function () { $(this).children("form").submit(); },
                "Cancel": function () { $(this).dialog("close"); }
            }
        });

        DialogHelper.Open(del, callback);
    };

    this.GetWidget = function (roleType) {
        var widget = $("<div class = 'RoleType'/>");
        var header = $("<h1 class='ui-widget-header'/>");
        var deleteButton = $("<a href='javascript:ContainerPrototypeController.RemoveRoleType(\"" + roleType.Parent.Name + "\",\"" + roleType.Name + "\")' title='Delete' class='ui-icon ui-icon-trash'/>");
        var content = $("<div class='ui-widget-content'/>");
        var placeholder = $(EventController.Placeholder("Drag rules from the components into this role type. Or add a <a href='javascript:ContainerPrototypeController.CreateAddRule(\"" + roleType.Parent.Name + "\",\"" + roleType.Name + "\");'>new one.</a>", "h3"));

        $(header).text(roleType.Name);
        $(header).append(deleteButton);

        $(widget).append(header);
        $(widget).append(placeholder);
        $(widget).append(content);

        $(widget).droppable({
            activeClass: "ui-state-default",
            hoverClass: "ui-state-hover",
            accept: ".Rule",
            drop: function (event, ui) {
                ContainerPrototypeController.AddRule(roleType.Parent.Name, roleType.Name, ui.draggable.text());
            }
        });

        for (var v in roleType.Childs) {
            $(widget).children(".ui-state-highlight").hide();
            $($(widget).children(".ui-widget-content")).append(RuleView.GetWidget(roleType.Childs[v]));
        }

        return widget;
    };
});