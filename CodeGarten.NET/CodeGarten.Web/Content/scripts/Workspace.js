var WorkspaceController = new (function () {
    this.Init = function (createFormId, structureId) {
        WorkspaceView.Init(createFormId, structureId);
    };

    this.Create = function (callback) {
        WorkspaceView.Create(function (name) {
            ComponentsView.AddItem("Workspace", name);
            if (callback)
                callback(name);
        });
    };

    this.Edit = function (name) {
        WorkspaceView.Edit(name);
    };

    this.Delete = function (name) {
        WorkspaceView.Delete(name, function () {
            ComponentsView.RemoveItem("Workspace", name);
            ContainerPrototypeController.DeleteWorkspace(name);
        });
    };
});

var WorkspaceView = new (function () {
    var create;
    var structure;

    this.Init = function (createFormId, structureId) {
        create = $(createFormId);
        structure = structureId;
        $(create).hide();
    };

    this.Create = function (callback) {
        $(create).children("form").find("input[type=checkbox]").button();
        $(create).dialog({
            autoOpen: false, modal: true, draggable: false, resizable: false, title: "Create a new workspace",
            buttons: {
                "Create": function () { $(create).children("form").submit(); },
                "Cancel": function () { $(create).dialog("close"); }
            }
        });

        DialogHelper.Open(create, callback);
    };

    this.Delete = function (name, callback) {
        var del = $("<div/>").load("/WorkSpaceType/Delete?structureId=" + structure + "&name=" + name).dialog({
            autoOpen: false, modal: true, draggable: false, resizable: false, title: "Delete a workspace",
            buttons: {
                "Delete": function () { $(this).children("form").submit(); },
                "Cancel": function () { $(this).dialog("destroy"); }
            }
        });

        DialogHelper.Open(del, callback);
    };

    this.Edit = function (name, callback) {
        var edit = $("<div/>").load("/WorkSpaceType/Edit?structureId=" + structure + "&name=" + name).dialog({
            autoOpen: false, modal: true, draggable: false, resizable: false, title: "Edit a workspace",
            buttons: {
                "Save": function () { $(this).children("form").submit(); },
                "Cancel": function () { $(this).dialog("destroy"); }
            }
        });

        DialogHelper.Open(edit, callback);
    };

    this.GetWidget = function (workspace) {
        var widget = $("<div class = 'Workspace'/>");
        var header = $("<h1 class='ui-widget-header'/>");
        var deleteButton = $("<a href='javascript:ContainerPrototypeController.RemoveWorkspace(\"" + workspace.Name + "\")' title='Delete' class='ui-icon ui-icon-trash'/>");
        var content = $("<div class='ui-widget-content'/>");
        var placeholder = $(EventController.Placeholder("Drag role types from the components into this workspace. Or add a <a href='javascript:ContainerPrototypeController.CreateAddRoleType(\"" + workspace.Name + "\");'>new one.</a>", "h3"));

        $(header).text(workspace.Name);
        $(header).append(deleteButton);

        $(widget).append(header);
        $(widget).append(placeholder);
        $(widget).append(content);

        $(widget).droppable({
            activeClass: "ui-state-default",
            hoverClass: "ui-state-hover",
            accept: ".RoleType",
            drop: function (event, ui) {
                ContainerPrototypeController.AddRoleType(workspace.Name, ui.draggable.text());
            }
        });

        for (var v in workspace.Childs) {
            $(widget).children(".ui-state-highlight").hide();
            $(widget).children(".ui-widget-content").append(RoleTypeView.GetWidget(workspace.Childs[v]));
        }

        return widget;
    };
});