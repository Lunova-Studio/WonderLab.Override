using System.Collections.Generic;
using Nuke.Common.Tooling;

namespace Tasks;

[PathTool(Executable = PathExecutable)]
public class HachimiPackagingTasks : ToolTasks, IRequirePathTool {
    private const string PathExecutable = "hachimi";
    
    public static IReadOnlyCollection<Output> HachimiPackagingAppImage(Configure<PackagingAppImageSettings> configurator) =>
        new HachimiPackagingTasks().Run(configurator.Invoke(new PackagingAppImageSettings()));

    public static IReadOnlyCollection<Output> HachimiPackagingPortable(Configure<PackagingPortableSettings> configurator) =>
        new HachimiPackagingTasks().Run(configurator.Invoke(new PackagingPortableSettings()));
    
    public static IReadOnlyCollection<Output> HachimiPackagingAppBundle(Configure<PackagingAppBundleSettings> configurator) =>
        new HachimiPackagingTasks().Run(configurator.Invoke(new PackagingAppBundleSettings()));
}