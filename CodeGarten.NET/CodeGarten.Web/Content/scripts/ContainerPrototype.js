function AcyclicTree(name, parent) {
    this.Parent = parent;

    this.Name = name;

    this.Childs = [];
};

function Role(cpName, wsName, rtName, ruleName) {
    this.ContainerPrototypeName = cpName;

    this.WorkSpaceTypeName = wsName;

    this.RoleTypeName = rtName;

    this.RuleName = ruleName;
};

var ContainerPrototypeController = new (function () {
    var editing;

    this.Init = function (view) {
        ContainerPrototypeModel.Init();
        ContainerPrototypeView.Init(view);
    };

    this.Create = function () {
        ContainerPrototypeView.Create(function (containerPrototypeName) {
            ContainerPrototypeModel.AddContainerPrototype(containerPrototypeName);
            ContainerPrototypeController.Design(containerPrototypeName);
        });
    };

    this.Delete = function (name) {
        ContainerPrototypeView.Delete(function () {
            ContainerPrototypeModel.Delete(name);
            ContainerPrototypeView.Delete(name);
        });
    };

    this.Design = function (name) {
        var containerPrototype = ContainerPrototypeModel.GetContainerPrototype(name);
        ContainerPrototypeView.Design(containerPrototype);
        editing = containerPrototype;
    };

    this.AddWorkspace = function (workspaceName) {
        var workspace = ContainerPrototypeModel.AddWorkspace(editing.Name, workspaceName);
        if (workspace)
            ContainerPrototypeView.AddWorkspace(WorkspaceView.GetWidget(workspace));
        else
            EventController.GlobalError("the container prototype '" + editing.Name + "' already contains the workspace '" + workspaceName + "'");
    };

    this.RemoveWorkspace = function (workspaceName) {
        if (ContainerPrototypeModel.RemoveWorkspace(editing.Name, workspaceName))
            ContainerPrototypeView.RemoveWorkspace(workspaceName);
    };

    this.DeleteWorkspace = function (workspaceName) {
        ContainerPrototypeModel.DeleteWorkspace(workspaceName);
        this.Design(editing.Name);
    };

    this.CreateAddWorkspace = function () {
        WorkspaceController.Create(function (workspaceName) {
            ContainerPrototypeController.AddWorkspace(workspaceName);
        });
    };

    this.AddRoleType = function (workspaceName, roleTypeName) {
        var roleType = ContainerPrototypeModel.AddRoleType(editing.Name, workspaceName, roleTypeName);
        if (roleType)
            ContainerPrototypeView.AddRoleType(workspaceName, RoleTypeView.GetWidget(roleType));
        else
            EventController.GlobalError("the workspace '" + workspaceName + "' already contains the role type '" + roleTypeName + "'");
    };

    this.RemoveRoleType = function (workspaceName, roleTypeName) {
        if (ContainerPrototypeModel.RemoveRoleType(editing.Name, workspaceName, roleTypeName))
            ContainerPrototypeView.RemoveRoleType(workspaceName, roleTypeName);
    };

    this.DeleteRoleType = function (roleTypeName) {
        if (ContainerPrototypeModel.DeleteRoleType(roleTypeName))
            this.Design(editing.Name);
    };

    this.CreateAddRoleType = function (workspaceName) {
        RoleTypeController.Create(function (roleTypeName) {
            ContainerPrototypeController.AddRoleType(workspaceName, roleTypeName);
        });
    };

    this.AddRule = function (workspaceName, roleTypeName, ruleName) {
        var rule = ContainerPrototypeModel.AddRule(editing.Name, workspaceName, roleTypeName, ruleName);
        if (rule)
            ContainerPrototypeView.AddRule(workspaceName, roleTypeName, RuleView.GetWidget(rule));
        else
            EventController.GlobalError("the role type '" + roleTypeName + "' already contains the rule '" + ruleName + "'");
    };

    this.RemoveRule = function (workspaceName, roleTypeName, ruleName) {
        if (ContainerPrototypeModel.RemoveRule(editing.Name, workspaceName, roleTypeName, ruleName))
            ContainerPrototypeView.RemoveRule(workspaceName, roleTypeName, ruleName);
    };

    this.DeleteRule = function (ruleName) {
        if (ContainerPrototypeModel.DeleteRule(ruleName))
            this.Design(editing.Name);
    };

    this.CreateAddRule = function (workspaceName, roleTypeName) {
        RuleController.Create(function (ruleName) {
            ContainerPrototypeController.AddRule(workspaceName, roleTypeName, ruleName);
        });
    };

    this.GetRoles = function (containerPrototypeName) {
        var containerPrototype = ContainerPrototypeModel.GetContainerPrototype(containerPrototypeName);
        var roles = [];
        for (var v in containerPrototype.Childs) {
            var workspace = containerPrototype.Childs[v];
            for (var i in workspace.Childs) {
                var roleType = workspace.Childs[i];
                for (var j in roleType.Childs) {
                    var rule = roleType.Childs[j];
                    roles.push(new Role(containerPrototype.Name, workspace.Name, roleType.Name, rule.Name));
                }
            }
        }
        return roles;
    };
});

var ContainerPrototypeModel = new (function () {
    var containerPrototypes;

    this.Init = function () {
        containerPrototypes = [];
    };

    this.GetContainerPrototype = function (name) {
        for (var v in containerPrototypes)
            if (containerPrototypes[v].Name == name)
                return containerPrototypes[v];
    };

    this.AddContainerPrototype = function (name) {
        if (this.GetContainerPrototype(name))
            return undefined;
        return containerPrototypes.push(new AcyclicTree(name));
    };

    this.RemoveContainerPrototype = function (name) {
        for (var v in containerPrototypes)
            if (containerPrototypes[v].Name == name) {
                containerPrototypes.splice(v, 1);
                return containerPrototypes;
            }
    };

    this.AddWorkspace = function (containerPrototypeName, workspaceName) {
        var containerPrototype = this.GetContainerPrototype(containerPrototypeName);
        for (var v in containerPrototype.Childs)
            if (containerPrototype.Childs[v].Name == workspaceName)
                return undefined;
        var workspace = new AcyclicTree(workspaceName, containerPrototype);
        containerPrototype.Childs.push(workspace);
        return workspace;
    };

    this.RemoveWorkspace = function (containerPrototypeName, workspaceName) {
        var containerPrototype = this.GetContainerPrototype(containerPrototypeName);
        for (var v in containerPrototype.Childs)
            if (containerPrototype.Childs[v].Name == workspaceName) {
                containerPrototype.Childs.splice(v, 1);
                return containerPrototype;
            }
    };

    this.DeleteWorkspace = function (workspaceName) {
        var deleted;
        for (var e in containerPrototypes)
            for (var v in containerPrototypes[e].Childs)
                if (containerPrototypes[e].Childs[v].Name == workspaceName) {
                    containerPrototypes[e].Childs.splice(v, 1);
                    deleted = true;
                }
        return deleted;
    };

    this.AddRoleType = function (containerPrototypeName, workspaceName, roleTypeName) {
        var containerPrototype = this.GetContainerPrototype(containerPrototypeName);
        var workspace;
        for (var v in containerPrototype.Childs)
            if (containerPrototype.Childs[v].Name == workspaceName) {
                workspace = containerPrototype.Childs[v];
                break;
            }
        for (v in workspace.Childs)
            if (workspace.Childs[v].Name == roleTypeName)
                return undefined;
        var roleType = new AcyclicTree(roleTypeName, workspace);
        workspace.Childs.push(roleType);
        return roleType;
    };

    this.RemoveRoleType = function (containerPrototypeName, workspaceName, roleTypeName) {
        var containerPrototype = this.GetContainerPrototype(containerPrototypeName);
        var workspace;
        for (var v in containerPrototype.Childs)
            if (containerPrototype.Childs[v].Name == workspaceName) {
                workspace = containerPrototype.Childs[v];
                break;
            }
        for (v in workspace.Childs)
            if (workspace.Childs[v].Name == roleTypeName) {
                workspace.Childs.splice(v, 1);
                return workspace;
            }
    };

    this.DeleteRoleType = function (roleTypeName) {
        var deleted;
        for (var e in containerPrototypes)
            for (var v in containerPrototypes[e].Childs)
                for (var i in containerPrototypes[e].Childs[v].Childs)
                    if (containerPrototypes[e].Childs[v].Childs[i].Name == roleTypeName) {
                        containerPrototypes[e].Childs[v].Childs.splice(i, 1);
                        deleted = true;
                    }
        return deleted;
    };

    this.AddRule = function (containerPrototypeName, workspaceName, roleTypeName, ruleName) {
        var containerPrototype = this.GetContainerPrototype(containerPrototypeName);
        var workspace;
        for (var v in containerPrototype.Childs)
            if (containerPrototype.Childs[v].Name == workspaceName) {
                workspace = containerPrototype.Childs[v];
                break;
            }
        var roleType;
        for (v in workspace.Childs)
            if (workspace.Childs[v].Name == roleTypeName) {
                roleType = workspace.Childs[v];
                break;
            }
        for (v in roleType.Childs)
            if (roleType.Childs[v].Name == ruleName)
                return undefined;
        var rule = new AcyclicTree(ruleName, roleType);
        roleType.Childs.push(rule);
        return rule;
    };

    this.RemoveRule = function (containerPrototypeName, workspaceName, roleTypeName, ruleName) {
        var containerPrototype = this.GetContainerPrototype(containerPrototypeName);
        var workspace;
        for (var v in containerPrototype.Childs)
            if (containerPrototype.Childs[v].Name == workspaceName) {
                workspace = containerPrototype.Childs[v];
                break;
            }
        var roleType;
        for (v in workspace.Childs)
            if (workspace.Childs[v].Name == roleTypeName) {
                roleType = workspace.Childs[v];
                break;
            }
        for (v in roleType.Childs)
            if (roleType.Childs[v].Name == ruleName) {
                roleType.Childs.splice(v, 1);
                return roleType;
            }
    };

    this.DeleteRule = function (ruleName) {
        var deleted;
        for (var e in containerPrototypes)
            for (var v in containerPrototypes[e].Childs)
                for (var i in containerPrototypes[e].Childs[v].Childs)
                    for (var j in containerPrototypes[e].Childs[v].Childs[i].Childs)
                        if (containerPrototypes[e].Childs[v].Childs[i].Childs[j].Name == ruleName) {
                            containerPrototypes[e].Childs[v].Childs[i].Childs.splice(i, 1);
                            deleted = true;
                        }
        return deleted;
    };

});

var ContainerPrototypeView = new (function () {
    var view;
    var containerPrototypeTag;

    this.Init = function (viewId) {
        view = viewId;
        $(view).empty();
        $(view).append(EventController.Placeholder("Choose a container prototype from the structure to start editing.", "h2"));
        $(view).append("<div class='ContainerPrototype'/>");
        containerPrototypeTag = $(view).children(".ContainerPrototype");
    };

    this.Create = function (callback) {
        var formTag = $("<div>Name:<input type='text' name='Name'/></div>");
        $(formTag).dialog({ autoOpen: true, modal: true, draggable: false, resizable: false, title: "Create a new container prototype",
            buttons: {
                "Create": function () {
                    var name = $(this).children("input[name=Name]").val();
                    callback(name);
                    $(formTag).dialog("destroy");
                },
                "Cancel": function () { $(formTag).dialog("destroy"); }
            }
        });
    };

    this.Design = function (containerPrototype) {
        $(view).fadeOut(function () {
            $(view).children(".ui-state-highlight").hide();
            $(containerPrototypeTag).empty();

            var cpHeader = $("<h1 class='ui-widget-header'/>");
            var cpContent = $("<div class='ui-widget-content'/>");

            $(cpHeader).text(containerPrototype.Name);

            $(containerPrototypeTag).append(cpHeader);
            $(containerPrototypeTag).append(EventController.Placeholder("Drag workspaces from the components into this container prototype.  Or add a <a href='javascript:ContainerPrototypeController.CreateAddWorkspace();'>new one.</a>", "h3"));
            $(containerPrototypeTag).append(cpContent);

            $(containerPrototypeTag).droppable({
                activeClass: "ui-state-default",
                hoverClass: "ui-state-hover",
                accept: ".Workspace",
                drop: function (event, ui) {
                    ContainerPrototypeController.AddWorkspace(ui.draggable.text());
                }
            });

            for (var v in containerPrototype.Childs)
                ContainerPrototypeView.AddWorkspace(WorkspaceView.GetWidget(containerPrototype.Childs[v]));

            $(view).fadeIn();
        });
    };

    this.GetWorkspaceWidget = function (name) {
        return $(containerPrototypeTag).children(".ui-widget-content").find(".Workspace > .ui-widget-header:contains('" + name + "')").parent();
    };

    this.GetRoleTypeWidget = function (workspaceWidget, name) {
        return $(workspaceWidget).children(".ui-widget-content").find(".RoleType > .ui-widget-header:contains('" + name + "')").parent();
    };

    this.GetRuleWidget = function (roleTypeWidget, name) {
        return $(roleTypeWidget).children(".ui-widget-content").find(".Rule:contains('" + name + "')");
    };

    this.AddWorkspace = function (workspaceWidget) {
        var content = $(containerPrototypeTag).children(".ui-widget-content");
        $(workspaceWidget).hide();
        $(containerPrototypeTag).children(".ui-state-highlight").hide();
        $(content).append(workspaceWidget);
        $(workspaceWidget).fadeIn();
    };

    this.RemoveWorkspace = function (name) {
        var workspaceWidget = this.GetWorkspaceWidget(name);

        $(workspaceWidget).fadeOut(function () {
            if ($(workspaceWidget).siblings().length == 0)
                $(containerPrototypeTag).children(".ui-state-highlight").fadeIn();
            $(workspaceWidget).remove();
        });
    };

        this.AddRoleType = function (workspaceName, roleTypeWidget) {
            var workspaceWidget = this.GetWorkspaceWidget(workspaceName);


            var content = $(workspaceWidget).children(".ui-widget-content");
            $(roleTypeWidget).hide();
            $(workspaceWidget).children(".ui-state-highlight").hide();
            $(content).append(roleTypeWidget);
            $(roleTypeWidget).fadeIn();
        };

        this.RemoveRoleType = function (workspaceName, roleTypeName) {
            var workspaceWidget = this.GetWorkspaceWidget(workspaceName);
            var roleTypeWidget = this.GetRoleTypeWidget(workspaceWidget, roleTypeName);

            $(roleTypeWidget).fadeOut(function () {
                if ($(roleTypeWidget).siblings().length == 0)
                    $(workspaceWidget).children(".ui-state-highlight").fadeIn();
                $(roleTypeWidget).remove();
            });
        };

        this.AddRule = function (workspaceName, roleTypeName, ruleWidget) {
            var workspaceWidget = this.GetWorkspaceWidget(workspaceName);
            var roleTypeWidget = this.GetRoleTypeWidget(workspaceWidget, roleTypeName);

            var content = $(roleTypeWidget).children(".ui-widget-content");
            $(ruleWidget).hide();
            $(roleTypeWidget).children(".ui-state-highlight").hide();
            $(content).append(ruleWidget);
            $(ruleWidget).fadeIn();
        };

        this.RemoveRule = function (workspaceName, roleTypeName, ruleName) {
            var workspaceWidget = this.GetWorkspaceWidget(workspaceName);
            var roleTypeWidget = this.GetRoleTypeWidget(workspaceWidget, roleTypeName);
            var ruleWidget = this.GetRuleWidget(roleTypeWidget, ruleName);

            $(ruleWidget).fadeOut(function () {
                if ($(ruleWidget).siblings().length == 0)
                    $(roleTypeWidget).children(".ui-state-highlight").fadeIn();
                $(ruleWidget).remove();
            });
        };
});