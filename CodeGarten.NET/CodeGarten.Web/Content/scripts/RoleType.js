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
            TreeController.DeleteRoleType(name);
        });
    };
});

var RoleTypeView = new (function () {
    this.create = new FormDialog();
    var structure;

    this.Init = function (createFormId, structureId) {
        this.create.init(createFormId);
        structure = structureId;
    };

    this.Create = function (callback) {
        this.create.Open("Create a new role type", { Name: null }, function (obj) { callback(obj.Name); });
    };

    this.Delete = function (name, callback) {
        DialogConfirmView.open("Delete a role type", "Are you sure you want to delete the role type: " + name, function () {
            $("#main").mask("Deleting...", 500);
            $.post("/RoleType/Delete?structureId=" + structure + "&name=" + name, null, function (result) {
                if (result.Success) {
                    $("#main").unmask();
                    callback();
                }
            });
        });
    };

    this.GetWidget = function (roleType) {
        var widget = $("<div class = 'RoleType'/>");
        var header = $("<h1 class='ui-widget-header'/>");

        var stopTop = $("<a href='#' title='Block inheritance from above' class='ui-icon ui-icon-arrowthickstop-1-s ui-state-disabled'/>");
        var stopBottom = $("<a href='#' title='Block inheritance from below' class='ui-icon ui-icon-arrowthickstop-1-n ui-state-disabled'/>");

        switch (roleType.Block) {
            case 1:
                {
                    stopTop.removeClass("ui-state-disabled");
                    break;
                }
            case 2:
                {
                    stopBottom.removeClass("ui-state-disabled");
                    break;
                }
            case 3:
                {
                    stopTop.removeClass("ui-state-disabled");
                    stopBottom.removeClass("ui-state-disabled");
                }
        }

        $(stopTop).click(function () { $(stopTop).toggleClass("ui-state-disabled"); TreeController.SetRoleTypeBlock(roleType.Parent.Name, roleType.Name, !$(stopTop).hasClass("ui-state-disabled"), !$(stopBottom).hasClass("ui-state-disabled")); });

        $(stopBottom).click(function () { $(stopBottom).toggleClass("ui-state-disabled"); TreeController.SetRoleTypeBlock(roleType.Parent.Name, roleType.Name, !$(stopTop).hasClass("ui-state-disabled"), !$(stopBottom).hasClass("ui-state-disabled")); });

        var deleteButton = $("<a href='javascript:TreeController.RemoveRoleType(\"" + roleType.Parent.Name + "\",\"" + roleType.Name + "\")' title='Delete' class='ui-icon ui-icon-trash'/>");
        var content = $("<div class='ui-widget-content'/>");
        var placeholder = $(EventController.Placeholder("Drag rules from the components into this role type. Or add a <a href='javascript:TreeController.CreateAddRule(\"" + roleType.Parent.Name + "\",\"" + roleType.Name + "\");'>new one.</a>", "h3"));

        $(header).text(roleType.Name);

        $(header).append(stopTop);
        $(header).append(stopBottom);

        $(header).append(deleteButton);

        $(widget).append(header);
        $(widget).append(placeholder);
        $(widget).append(content);

        $(widget).droppable({
            activeClass: "ui-state-default",
            hoverClass: "ui-state-highlight",
            accept: ".Rule",
            drop: function (event, ui) {
                TreeController.AddRule(roleType.Parent.Name, roleType.Name, ui.draggable.text());
            }
        });

        for (var v in roleType.Childs) {
            $(widget).children(".ui-state-highlight").hide();
            $($(widget).children(".ui-widget-content")).append(RuleView.GetWidget(roleType.Childs[v]));
        }

        return widget;
    };
});