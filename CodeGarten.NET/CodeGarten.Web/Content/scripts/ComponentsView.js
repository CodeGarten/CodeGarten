var ComponentsView = new (function () {
    var divId;

    var content;

    this.Init = function (id, title, pages) {
        divId = id;

        $(divId).fadeOut(function () {
            $(divId).empty();
            $(divId).append("<h1 class='ui-widget-header'/>");
            $(divId).append("<div/>");

            var header = $(divId).children(".ui-widget-header");
            content = $(divId).children(".ui-widget-header").next();

            $(header).text(title);

            for (var v in pages) {
                var pageTag = $("<h3 class='" + pages[v] + "_link'><a class='page_link href='#'>" + pages[v] + "s</a></h3> <div> " + EventController.Placeholder("Empty. Click the add button to create.", "h5") + "<div class='page_content'/> <div class='page_options'/> </div>");
                var addButton = $("<button class='page_add' onclick='javascript:" + pages[v] + "Controller.Create();'>Add " + pages[v] + "</button>").button({ icons: { primary: "ui-icon-plus"} });
                $(pageTag).find(".page_options").append(addButton);
                $(content).append(pageTag);
            }
            $(content).accordion({ 'clearStyle': true, autoHeight: false });

            ComponentsView.Sync();

            $(divId).fadeIn();
        });
    };

    this.AddItem = function (page, item) {
        var pageTag = $(content).find(".page_link:contains('" + page + "s')");

        var itemTag = $("<div class='" + page + " item'>" + item + "<span class='item_options'/></div>");
        $(itemTag).hide();

        var buttonEdit = $("<button onclick='javascript:" + page + "Controller.Edit(\"" + item + "\");' title='Edit'/>").button({ icons: { primary: "ui-icon-gear" }, text: false });
        var buttonDelete = $("<button title='Delete' onclick='javascript:" + page + "Controller.Delete(\"" + item + "\");'/>").button({ icons: { primary: "ui-icon-trash" }, text: false });

        $(itemTag).children(".item_options").append(buttonEdit);
        $(itemTag).children(".item_options").append(buttonDelete);
        $(pageTag).parent().next().children(".ui-state-highlight").hide();
        $(pageTag).parent().next().children(".page_content").append(itemTag);
        $(itemTag).draggable({
            appendTo: "body",
            helper: "clone",
            cancel: "button",
            revert: "invalid"
        });
        $(itemTag).fadeIn();
    };

    this.RemoveItem = function (page, item) {

        var pageTag = $(content).find(".page_link:contains('" + page + "s')");
        var itemTag = $(pageTag).parent().next().children(".page_content").children("div:contains('" + item + "')");

        $(itemTag).fadeOut(function () {
            if ($(itemTag).siblings().length == 0)
                $(pageTag).parent().next().children(".ui-state-highlight").fadeIn();
            $(itemTag).remove();
        });
    };

    this.Sync = function() {
        var workspaces = StructureModel.getWorkspaceTypes();
        var roleTypes = StructureModel.getRoleTypes();
        var rules = StructureModel.getRules();

        for (var v in workspaces)
            ComponentsView.AddItem("Workspace", workspaces[v].Name);

        for (v in roleTypes)
            ComponentsView.AddItem("RoleType", roleTypes[v].Name);

        for (var v in rules)
            ComponentsView.AddItem("Rule", rules[v].Name);
    };
});