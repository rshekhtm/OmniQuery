using System;
using System.Collections.Generic;
using System.Text;

namespace OmniQuery.CodeAnalytics.Linq
{
    public enum ModuleType
    {
        Library,
        Console,
        Windows,
        NetModule
    }

    public enum TargetPlatform
    {
        Unknown,
        AnyCPU,
        x86,
        x64,
        Itanium
    }

    public enum ResourceType
    {
        Linked,
        Embedded,
        AssemblyLinked
    }
}
