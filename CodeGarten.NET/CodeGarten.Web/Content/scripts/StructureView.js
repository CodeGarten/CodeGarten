var StructureHtml = new (function () {

    this.Container = function (containerPrototypeName) {
        return "<div id='" + containerPrototypeName + "' class='container' >\
					<div class='container_header ui-widget-header ui-state-default ui-corner-all'>\
						<h2 onclick='TreeController.Design(\"" + containerPrototypeName + "\")'>" + containerPrototypeName + "</h2>\
                        <a title='Toggle " + containerPrototypeName + "' onclick='StructureController.Toggle(\"" + containerPrototypeName + "\")' class='toggle ui-icon ui-icon-carat-1-n'>Toggle container</a>\
                        <a title='Add Child " + containerPrototypeName + "' onclick='StructureController.AddChild(\"" + containerPrototypeName + "\")' class='ui-icon ui-icon-plusthick'>Add child</a>\
						<a title='Delete " + containerPrototypeName + "' onclick='StructureController.Delete(\"" + containerPrototypeName + "\")' class='ui-icon ui-icon-trash'>Delete</a>\
					</div>\
					<div class='container_childs'>\
					</div>\
				</div>";
    };

    this.FirstContainerPrototype = function () {
        return EventController.Placeholder("Create your first container prototype <a href='javascript:StructureController.AddChild(null)'>here</a>.","h2");
    };

})();

var StructureView = new (function () {

    var _structure;
    var _formContainerPrototype;
    var _formParent;

    this.ContainerPrototypeFormDialog = new FormDialog();

    this.init = function (structure, formContainerPrototype) {
        _structure = $(structure);
        _structure.html(StructureHtml.FirstContainerPrototype());
        _formContainerPrototype = $(formContainerPrototype);
        _formParent = $(formContainerPrototype + " input[name=\"parent\"]")

        this.ContainerPrototypeFormDialog.init(formContainerPrototype);
    };

    this.Toggle = function (containerPrototypeName) {
        var toggle = $("#" + containerPrototypeName + " > .container_header .toggle");
        var container = $("#" + containerPrototypeName + " > .container_childs");

        toggle.toggleClass("ui-icon-carat-1-n").toggleClass("ui-icon-carat-1-se");
        container.slideToggle("slow");

//        if (toggle.hasClass("ui-icon-carat-1-n")) {
//            container.slideUp("slow", function () {
//                toggle.removeClass("ui-icon-carat-1-n").addClass("ui-icon-carat-1-se");
//            });
//        } else {
//            container.slideDown("slow", function () {
//                toggle.removeClass("ui-icon-carat-1-se").addClass("ui-icon-carat-1-n");
//            });
//        }

    };

    this.RemoveAll = function () {
        _structure.empty();
    };

    this.AddChild = function (parent, child) {

        var parentName;
        if (parent instanceof ContainerPrototype)
            parentName = parent.name;
        else
            parentName = parent;

        var item = $(StructureHtml.Container(child.name)).hide();
        item.children(".container_header").children(".toggle").hide();

        if (parentName == null) {
            this.RemoveAll();
            _structure.append(item);
        } else {
            $("#" + parentName + " > .container_header > .toggle").show();
            $("#" + parentName + " > .container_childs").append(item);
        }
        item.children(".container_header").hover(function () { item.children(".container_header").toggleClass("ui-state-default").toggleClass("ui-state-hover"); });
        item.slideDown();
    };

    this.Remove = function (containerName) {
        //TODO - remove the expand/collapse icon if there is no more childs
        var firstContainerId = _structure.children(".container").attr("id");

        $("#" + containerName).slideUp(function () {
            $("#" + containerName).remove();

            if (firstContainerId == containerName) {
                $(_structure).hide();
                _structure.html(StructureHtml.FirstContainerPrototype());
                $(_structure).slideDown();
            }
        });
    };

    this.CreateContainerPrototype = function (obj, callback) {

        _formParent.val(obj.ParentName);

        this.ContainerPrototypeFormDialog.Open("Create Container Prototype", obj, callback);
    };

    this.Error = function (msg) {
        //show error;
    };

})();