function Tree(name, parent) {
    this.Parent = parent;

    this.Name = name;

    this.Childs = [];
};

function Role(cpName, wsName, roleTypeName, rules) {
    this.ContainerPrototypeName = cpName;

    this.WorkSpaceTypeName = wsName;

    this.RoleTypeName = roleTypeName;

    this.Rules = rules;

    this.RoleBarrier;
};

var TreeController = new (function () {
    var editing;
    var view;

    this.Init = function (_view) {
        view = _view;
        TreeModel.Init();
        TreeView.Init(view);
    };

    this.Create = function (name) {
        TreeModel.AddContainerPrototype(name);
        TreeController.Design(name);
    };

    this.Delete = function (name) {
        TreeModel.RemoveContainerPrototype(name);
        TreeView.Init(view);
        editing = undefined;
    };

    this.Design = function (name) {
        if (!TreeModel.GetContainerPrototype(name))
            TreeModel.AddContainerPrototype(name);
        var containerPrototype = TreeModel.GetContainerPrototype(name);
        TreeView.Design(containerPrototype);
        editing = containerPrototype;
        $("#tree_design").fadeIn();
    };

    this.AddWorkspace = function (workspaceName) {
        var workspace = TreeModel.AddWorkspace(editing.Name, workspaceName);
        if (workspace)
            TreeView.AddWorkspace(WorkspaceView.GetWidget(workspace));
        else
            EventController.GlobalError("the container prototype '" + editing.Name + "' already contains the workspace '" + workspaceName + "'");
    };

    this.RemoveWorkspace = function (workspaceName) {
        if (TreeModel.RemoveWorkspace(editing.Name, workspaceName))
            TreeView.RemoveWorkspace(workspaceName);
    };

    this.DeleteWorkspace = function (workspaceName) {
        TreeModel.DeleteWorkspace(workspaceName);
        if (editing)
            this.Design(editing.Name);
    };

    this.CreateAddWorkspace = function () {
        WorkspaceController.Create(function (workspaceName) {
            TreeController.AddWorkspace(workspaceName);
        });
    };

    this.AddRoleType = function (workspaceName, roleTypeName) {
        var roleType = TreeModel.AddRoleType(editing.Name, workspaceName, roleTypeName);
        if (roleType)
            TreeView.AddRoleType(workspaceName, RoleTypeView.GetWidget(roleType));
        else
            EventController.GlobalError("the workspace '" + workspaceName + "' already contains the role type '" + roleTypeName + "'");
    };

    this.RemoveRoleType = function (workspaceName, roleTypeName) {
        if (TreeModel.RemoveRoleType(editing.Name, workspaceName, roleTypeName))
            TreeView.RemoveRoleType(workspaceName, roleTypeName);
    };

    this.DeleteRoleType = function (roleTypeName) {
        if (TreeModel.DeleteRoleType(roleTypeName))
            if (editing)
                this.Design(editing.Name);
    };

    this.CreateAddRoleType = function (workspaceName) {
        RoleTypeController.Create(function (roleTypeName) {
            TreeController.AddRoleType(workspaceName, roleTypeName);
        });
    };

    this.SetRoleTypeBlock = function (workspaceName, roleTypeName, blockAbove, blockBelow) {
        var block = 0;

        if (blockAbove && !blockBelow)
            block = 1;
        if (!blockAbove && blockBelow)
            block = 2;
        if (blockAbove && blockBelow)
            block = 3;


        TreeModel.SetRoleTypeBlock(editing.Name, workspaceName, roleTypeName, block);
    };

    this.AddRule = function (workspaceName, roleTypeName, ruleName) {
        var rule = TreeModel.AddRule(editing.Name, workspaceName, roleTypeName, ruleName);
        if (rule)
            TreeView.AddRule(workspaceName, roleTypeName, RuleView.GetWidget(rule));
        else
            EventController.GlobalError("the role type '" + roleTypeName + "' already contains the rule '" + ruleName + "'");
    };

    this.RemoveRule = function (workspaceName, roleTypeName, ruleName) {
        if (TreeModel.RemoveRule(editing.Name, workspaceName, roleTypeName, ruleName))
            TreeView.RemoveRule(workspaceName, roleTypeName, ruleName);
    };

    this.DeleteRule = function (ruleName) {
        if (TreeModel.DeleteRule(ruleName))
            if (editing)
                this.Design(editing.Name);
    };

    this.CreateAddRule = function (workspaceName, roleTypeName) {
        RuleController.Create(function (ruleName) {
            TreeController.AddRule(workspaceName, roleTypeName, ruleName);
        });
    };

    this.GetRoles = function (containerPrototypeName) {
        var containerPrototype = TreeModel.GetContainerPrototype(containerPrototypeName);
        var roles = [];
        for (var v in containerPrototype.Childs) {
            var workspace = containerPrototype.Childs[v];
            for (var i in workspace.Childs) {
                var roleType = workspace.Childs[i];
                var role = new Role(containerPrototype.Name, workspace.Name, roleType.Name);
                role.RoleBarrier = roleType.Block;
                var rules = [];
                for (var j in roleType.Childs)
                    rules.push({ Name: roleType.Childs[j].Name });
                role.Rules = rules;
                roles.push(role);
            }
        }
        return roles;
    };

    this.GetAllRoles = function () {
        var roles = [];
        var cps = TreeModel.GetContainerPrototypes();
        for (var v in cps)
            roles = roles.concat(this.GetRoles(cps[v].Name));
        return roles;
    };
});

var TreeModel = new (function () {
    var containerPrototypes;

    this.Init = function () {
        containerPrototypes = [];
        this.Sync();
    };

    this.Sync = function () {
        var roles = StructureModel.getRoles();

        for (var a in roles)
            this.Treefy(roles[a]);
    };

    this.Treefy = function (role) {
        if (!this.GetContainerPrototype(role.ContainerPrototypeName))
            this.AddContainerPrototype(role.ContainerPrototypeName);

        this.AddWorkspace(role.ContainerPrototypeName, role.WorkSpaceTypeName);
        this.AddRoleType(role.ContainerPrototypeName, role.WorkSpaceTypeName, role.RoleTypeName, role.BlockBarrier);

        for (var v in role.Rules)
            this.AddRule(role.ContainerPrototypeName, role.WorkSpaceTypeName, role.RoleTypeName, role.Rules[v].Name);
    };

    this.GetContainerPrototypes = function () {
        return containerPrototypes;
    };

    this.GetContainerPrototype = function (name) {
        for (var v in containerPrototypes)
            if (containerPrototypes[v].Name == name)
                return containerPrototypes[v];
    };

    this.AddContainerPrototype = function (name) {
        if (this.GetContainerPrototype(name))
            return undefined;
        return containerPrototypes.push(new Tree(name));
    };

    this.RemoveContainerPrototype = function (name) {
        for (var v in containerPrototypes)
            if (containerPrototypes[v].Name == name) {
                containerPrototypes.splice(v, 1);
                CleanUp();
                return containerPrototypes;
            }
    };

    var CleanUp = function () {
        for (var v in containerPrototypes)
            if (!ContainerPrototypeModel.GetContainerPrototype(containerPrototypes[v]))
                containerPrototypes.splice(v, 1);
    };

    this.AddWorkspace = function (containerPrototypeName, workspaceName) {
        var containerPrototype = this.GetContainerPrototype(containerPrototypeName);
        for (var v in containerPrototype.Childs)
            if (containerPrototype.Childs[v].Name == workspaceName)
                return undefined;
        var workspace = new Tree(workspaceName, containerPrototype);
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

    this.AddRoleType = function (containerPrototypeName, workspaceName, roleType, block) {
        var containerPrototype = this.GetContainerPrototype(containerPrototypeName);
        var workspace;
        for (var v in containerPrototype.Childs)
            if (containerPrototype.Childs[v].Name == workspaceName) {
                workspace = containerPrototype.Childs[v];
                break;
            }
        for (v in workspace.Childs)
            if (workspace.Childs[v].Name == roleType)
                return undefined;

        var roleTypeTree = new Tree(roleType, workspace);
        roleTypeTree.Block = !block ? 0 : block;

        workspace.Childs.push(roleTypeTree);
        return roleTypeTree;
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

    this.SetRoleTypeBlock = function (containerPrototypeName, workspaceName, roleTypeName, block) {
        var containerPrototype = this.GetContainerPrototype(containerPrototypeName);
        var workspace;
        for (var v in containerPrototype.Childs)
            if (containerPrototype.Childs[v].Name == workspaceName) {
                workspace = containerPrototype.Childs[v];
                break;
            }
        for (v in workspace.Childs)
            if (workspace.Childs[v].Name == roleTypeName) {
                workspace.Childs[v].Block = block;
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
        var rule = new Tree(ruleName, roleType);
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

var TreeView = new (function () {
    var view;
    var containerPrototypeTag;

    this.Init = function (viewId) {
        view = viewId;

        if (!ContainerPrototypeModel.GetContainerPrototypeWithParent(null))
            $("#tree_design").slideUp(function () { Prepare(); });
        else
            Prepare();


    };

    var Prepare = function () {
        $(view).empty();
        $(view).append(EventController.Placeholder("Choose a container prototype from the structure to start editing.", "h2"));
        $(view).append("<div class='ContainerPrototype'/>");
        containerPrototypeTag = $(view).children(".ContainerPrototype");
    }

    this.Design = function (containerPrototype) {
        $(view).fadeOut('fast', function () {
            $(view).children(".ui-state-highlight").hide();
            $(containerPrototypeTag).empty();

            var cpHeader = $("<h1 class='ui-widget-header'/>");
            var cpContent = $("<div class='ui-widget-content'/>");

            $(cpHeader).text(containerPrototype.Name);

            $(containerPrototypeTag).append(cpHeader);
            $(containerPrototypeTag).append(EventController.Placeholder("Drag workspaces from the components into this container prototype.  Or add a <a href='javascript:TreeController.CreateAddWorkspace();'>new one.</a>", "h3"));
            $(containerPrototypeTag).append(cpContent);

            $(containerPrototypeTag).droppable({
                activeClass: "ui-state-default",
                hoverClass: "ui-state-hover",
                accept: ".Workspace",
                drop: function (event, ui) {
                    TreeController.AddWorkspace(ui.draggable.text());
                }
            });

            for (var v in containerPrototype.Childs)
                TreeView.AddWorkspace(WorkspaceView.GetWidget(containerPrototype.Childs[v]));

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