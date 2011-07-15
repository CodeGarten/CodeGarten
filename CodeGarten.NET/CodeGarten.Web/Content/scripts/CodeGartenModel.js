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

var ContainerPrototypeModel = new (function () {

    var _containerPrototype;

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

    this.init = function (sync) {
        //syncronize with codegarten;
        _containerPrototype = null;
        if (!sync) return;
//        this.CreateContainerPrototype("UC", null);
//        this.CreateContainerPrototype("Turma_tipo_1", "UC");
//        this.CreateContainerPrototype("Turma_tipo_2", "UC");
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

        var compare = function (containerPrototype, containerPrototypeName) {

            if (containerPrototype.name == containerPrototypeName) {
                if (containerPrototype.parent == null) {
                    _containerPrototype == null;
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