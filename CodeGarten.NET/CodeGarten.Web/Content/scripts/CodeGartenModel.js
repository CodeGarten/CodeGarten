function ContainerPrototype(name, parentContainer)
{
	this.name = name;
	this.parent = parentContainer;
	this.childs = new Array();

	if (this.parent != null)
	    this.parent.AddChild(this);
        //this.parent.childs.push(this);

	this.AddChild = function (containerPrototype) {
	    this.childs.push(containerPrototype);
	};

	this.Remove = function () {
	    for (var i in this.parent.childs)
	        if (this.parent.childs[i].name == this.name)
	            this.parent.childs.splice(i, 1);
    };

	this.IsValid = function (name) {
	    return true;
	};

    this.Equals = function (containerPrototype){
        return this.name == containerPrototype.name;
    };

};

var StructureModel = new (function(){
    
    var _structureModel;
    var _id;

    this.init = function(id){
        _id = id;
        _structureModel = eval ('('+$.ajax({
              type: "GET",
              url: "/Structure/Synchronization",
              data: ({id:id}),
              cache: false,
              async: false
            }).responseText+')');
    };

    this.getId = function (){
        return _id;
    };

    this.getContainerPrototypes = function () {
        return _structureModel.ContainerPrototypes;
    };

    this.getRoleTypes = function (){
        return _structureModel.RoleTypes;
    };

    this.getWorkspaceTypes = function (){
        return _structureModel.WorkspaceTypes;
    };

    this.getRoles = function (){
        return _structureModel.Roles;
    };

    this.getRules = function (){
        return _structureModel.Rules;
    };

})();

var ContainerPrototypeModel = new (function () {

    var _containerPrototype;
    var _structureId;
    function GetContainer(containerPrototype, containerPrototypeName, compare) {

        if (compare(containerPrototype, containerPrototypeName))
            return containerPrototype;

        for (var i in containerPrototype.childs) {
            var container = GetContainer(containerPrototype.childs[i], containerPrototypeName, compare);
            if (container != null) return container;
        }

        return null;
    };

    function GetAllContainers(containerPrototype, containerPrototypeName, array, compare) {
        if (compare(containerPrototype, containerPrototypeName))
            array.push(containerPrototype);

        for (var i in containerPrototype.childs)
            GetAllContainers(containerPrototype.childs[i], containerPrototypeName, array, compare);
    };

    this.init = function (sync, structureId) {

        _structureId = StructureModel.getId();
        _containerPrototype = null;
        if (!sync) return;

        var containerPrototypes = StructureModel.getContainerPrototypes();

        var parent = null;
        for (var i in containerPrototypes) {
            var container = containerPrototypes[i];
            if (container.ParentName == null) {
                parent = this.CreateContainerPrototype(container.Name, null);
                containerPrototypes.splice(i, 1);
                break;
            }
        }
        if (parent == null) return;

        while (containerPrototypes.length > 0) {
            for(var i in containerPrototypes){
                var container = containerPrototypes[i];
                if(this.CreateContainerPrototype(container.Name, container.ParentName)!=null)
                    containerPrototypes.splice(i, 1);
            }
        }
    };

    this.GetContainerPrototype = function (containerPrototypeName) {

        if (_containerPrototype == null)
            return null;

        var compare = function (containerPrototype, containerPrototypeName) {

            return containerPrototype.name == containerPrototypeName;
        };

        return GetContainer(_containerPrototype, containerPrototypeName, compare);
    };

    this.GetContainerPrototypeWithParent = function (parentContainerName) {

        if (parentContainerName == null) {
            if (_containerPrototype != null)
                return _containerPrototype;
            return null;
        }

        var compare = function (containerPrototype, parentContainerName) {
            if (parentContainerName == null)
                return ContainerPrototype.parent == null;

            return containerPrototype.parent.name == parentContainerName;
        };

        var retArray = [];

        GetContainer(_containerPrototype, containerPrototypeName, retArray, compare);

        return retArray;
    };

    this.Remove = function (containerPrototypeName) {

        if (_containerPrototype == null)
            return false;

        $.ajax({
            type: "POST",
            url: "/ContainerPrototype/Delete",
            data: ({ structureId: _structureId, name: containerPrototypeName }),
            cache: false,
            async: false
        });

        var compare = function (containerPrototype, containerPrototypeName) {

            if (containerPrototype.name == containerPrototypeName) {
                if (containerPrototype.parent == null) {
                    _containerPrototype = null;
                    return true;
                }

                //                for (var i in containerPrototype.parent.childs)
                //                    if (containerPrototype.parent.childs[i].name == containerPrototypeName)
                //                        containerPrototype.parent.childs.splice(i, 1);
                containerPrototype.Remove();
                return true;
            }
            return false;
        };

        return GetContainer(_containerPrototype, containerPrototypeName, compare) != null;
    };

    this.CreateContainerPrototype = function (ContainerName, parentContainerName) {

        if (this.GetContainerPrototype(ContainerName) != null)
            return null;

        var parentObj = null;
        if (parentContainerName != null) {
            parentObj = this.GetContainerPrototype(parentContainerName);
            if (parentObj == null)
                return null;
        }

        var newContainer = new ContainerPrototype(ContainerName, parentObj);

        if (parentObj == null)
            _containerPrototype = newContainer;

        return newContainer;

    };

})();