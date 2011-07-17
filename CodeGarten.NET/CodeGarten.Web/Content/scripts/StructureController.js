var StructureController = new (function () {

    function initializer(parent) {

        StructureView.AddChild(parent.parent, parent);
        for (var i in parent.childs)
            initializer(parent.childs[i]);

    };

    this.init = function () {
        ContainerPrototypeModel.init(true);
        var ret = ContainerPrototypeModel.GetContainerPrototypeWithParent(null);
        if (ret != null)
            initializer(ret);
    };

    this.Delete = function (containerName) {
        DialogConfirmView.open("Delete a container prototype", "Are you sure you want to delete the container: " + containerName, function () {
            $("#main").mask("Deleting...", 500);
            if (ContainerPrototypeModel.Remove(containerName)) {
                $("#main").unmask();
                StructureView.Remove(containerName);
                TreeController.Delete(containerName);
            }
            else
                StructureView.Error("Can't delete the Container Prototype \"" + conainerName + "\"");

        });

    };

    this.AddChild = function (parentContainerName) {

        StructureView.CreateContainerPrototype({ Name: null, ParentName: parentContainerName }, function (obj) {

            var container = ContainerPrototypeModel.CreateContainerPrototype(obj.Name, obj.ParentName);

            if (container == null) {
                StructureView.Error("The Container Prototype \"" + containerName + "\" already exist");
                return;
            }

            StructureView.AddChild(parentContainerName, container);
            TreeController.Create(container.name);
        });

    };

    this.Toggle = function (containerPrototypeName) {
        StructureView.Toggle(containerPrototypeName);
    };

})();