using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.PowerShell;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution(GenerateProjects = true)]
    readonly Solution Solution;

    Target Clean => clean => clean
        .Before(Restore)
        .Executes(()
            => DotNetClean(x => x
                .SetConfiguration(Configuration)
                .SetVerbosity(DotNetVerbosity.Minimal)));

    Target Restore => restore => restore
        .Executes(()
            => DotNetRestore(x => x
                .SetVerbosity(DotNetVerbosity.Normal)));

    Target Compile => compile => compile
        .DependsOn(Restore)
        .Executes(()
            => DotNetBuild(x => x
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .SetVerbosity(DotNetVerbosity.Minimal)
            ));

    Target Test => test => test
        .DependsOn(Compile)
        .Executes(()
            => DotNetTest(x => x
                .SetConfiguration(Configuration)
                .SetProjectFile(Solution.test.Analyzer_SealedKeyword_Tests_Unit)
                .EnableNoRestore()
                .EnableNoBuild()
            ));
}
