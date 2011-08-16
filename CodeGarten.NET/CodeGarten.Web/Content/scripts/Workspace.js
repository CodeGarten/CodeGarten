var WorkspaceController = new (function () {
    this.Init = function (createFormId, structureId, preventEdit) {
        WorkspaceView.Init(createFormId, structureId, preventEdit);
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
            TreeController.DeleteWorkspace(name);
        });
    };
});

var WorkspaceView = new (function () {
    this.create = new FormDialog();
    this.edit = new FormDialog();

    var structure;
    var preventEdit;

    this.Init = function (createFormId, structureId, _preventEdit) {
        this.create.init(createFormId);
        structure = structureId;
        preventEdit = _preventEdit;
    };

    this.Create = function (callback) {
        this.create.Open("Create a new workspace", { Name: null }, function (obj) { callback(obj.Name); });
    };

    this.Delete = function (name, callback) {
        DialogConfirmView.open("Delete a workspace", "Are you sure you want to delete the workspace: " + name, function () {
            $("#main").mask("Deleting...", 500);
            $.post("/WorkSpaceType/Delete?structureId=" + structure + "&name=" + name, null, function (result) {
                if (result.Success) {
                    $("#main").unmask();
                    callback();
                }
            });
        });
    };

    this.Edit = function (name) {
        $("#main").mask("Loading...", 500);
        $.get("/WorkSpaceType/Edit?structureId=" + structure + "&name=" + name, function (result) {
            var view = $("<div/>").append(result);

            if (preventEdit) {
                $(view).find("input[type=checkbox]").attr("disabled", "disabled");
                $(view).find("input[type=submit]").remove();
                $(view).children("h2").remove();
            }

            WorkspaceView.edit.init(view);
            $("#main").unmask();
            if (preventEdit)
                WorkspaceView.edit.Open(name + " information", { Name: null });
            else
                WorkspaceView.edit.Open("Edit a workspace", { Name: null });
        });
    };

    this.GetWidget = function (workspace) {
        var widget = $("<div class = 'Workspace'/>");
        var header = $("<h1 class='ui-widget-header'/>");
        var deleteButton = $("<a href='javascript:TreeController.RemoveWorkspace(\"" + workspace.Name + "\")' title='Delete' class='ui-icon ui-icon-trash'/>");
        var content = $("<div class='ui-widget-content'/>");
        var placeholder = $(EventController.Placeholder("Drag role types from the components into this workspace. Or add a <a href='javascript:TreeController.CreateAddRoleType(\"" + workspace.Name + "\");'>new one.</a>", "h3"));

        $(header).text(workspace.Name);

        if (!preventEdit)
            $(header).append(deleteButton);

        $(widget).append(header);
        $(widget).append(placeholder);
        $(widget).append(content);

        $(widget).droppable({
            activeClass: "ui-state-default",
            hoverClass: "ui-state-highlight",
            accept: ".RoleType",
            drop: function (event, ui) {
                TreeController.AddRoleType(workspace.Name, ui.draggable.text());
            }
        });

        for (var v in workspace.Childs) {
            $(widget).children(".ui-state-highlight").hide();
            $(widget).children(".ui-widget-content").append(RoleTypeView.GetWidget(workspace.Childs[v]));
        }

        return widget;
    };
});