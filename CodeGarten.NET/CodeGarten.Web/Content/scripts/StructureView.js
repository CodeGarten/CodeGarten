var StructureHtml = new (function(){

	this.Container = function(containerPrototypeName){
	    return "<div id='" + containerPrototypeName + "' class='container' >\
					<div class='container_header ui-widget-header'>\
						<h2>" + containerPrototypeName + "</h2>\
                        <a title='Toggle " + containerPrototypeName + "' onclick='StructureController.Toggle(\"" + containerPrototypeName + "\")' class='toggle ui-icon ui-icon-carat-1-n'>Toggle container</a>\
                        <a title='Add Child " + containerPrototypeName + "' onclick='StructureController.AddChild(\"" + containerPrototypeName + "\")' class='ui-icon ui-icon-plusthick'>Add child</a>\
						<a title='Delete " + containerPrototypeName + "' onclick='StructureController.Delete(\"" + containerPrototypeName + "\")' class='ui-icon ui-icon-trash'>Delete</a>\
					</div>\
					<div class='container_childs'>\
					</div>\
				</div>";
	};

})();

var StructureView = new (function () {

    var _structure;
    var _formContainerPrototype;
    var _formParent;

    this.ContainerPrototypeFormDialog = new FormDialog();

    this.init = function (structure, formContainerPrototype) {
        _structure = $(structure);
        _formContainerPrototype = $(formContainerPrototype);
        _formParent = $(formContainerPrototype + " input[name=\"parent\"]")

        this.ContainerPrototypeFormDialog.init(formContainerPrototype);
    };

    this.Toggle = function (containerPrototypeName) {
        var toggle = $("#" + containerPrototypeName + " > .container_header .toggle");
        var container = $("#" + containerPrototypeName + " > .container_childs");

        if (toggle.hasClass("ui-icon-carat-1-n")) {
            container.slideUp("slow");
            toggle.removeClass("ui-icon-carat-1-n").addClass("ui-icon-carat-1-se");
        } else {
            container.slideDown("slow");
            toggle.removeClass("ui-icon-carat-1-se").addClass("ui-icon-carat-1-n");
        }

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

        if (parentName == null)
            _structure.append(StructureHtml.Container(child.name));
        else {
            var item = $(StructureHtml.Container(child.name)).hide();
            $("#" + parentName + " > .container_childs").append(item);
            item.slideDown();

        }
    };

    this.Remove = function (containerName) {
        $("#" + containerName).slideToggle(function () {
            $("#" + containerName).remove();
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