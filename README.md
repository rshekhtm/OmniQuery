OmniQuery
=========

In a nutshell
-------------

This was an attempt to build a developer-friendly static code analysis library for analyzing .NET assemblies, along with a LINQPad-like GUI tool on top of it. OmniQuery leveraged the fantastic open source library Mono.Cecil. On its own, Cecil provides an overwhelming amount of information about an assembly and I felt that a simpler API could be beneficial for a multitude of use cases. This was an initial stab at such an API.

What's the point?
-----------------

I became very interested in static code analysis while evaluating a commercial tool called [NDepend](http://www.ndepend.com). At Mirixa, I used it to develop an intelligent unit test runner that executed only those tests that were affected by code changes. The algorithm itself was fairly straightforward - identify all code that changed from one build to the next and walk up the possible call stacks to see if any culminate in a test method. The task was made more difficult by several NDepend shortcomings (e.g. inability to identify all implementations of an abstract method, cutting the walk up the call stack short) that I had to work around, as well as a lack of an API. This was mostly a GUI tool, with a console add-on almost an afterthought, using custom code query language (CQL) embedded inside of an XML file.

On the side, I started looking at code analysis libraries and found Microsoft.Cci (Common Compiler Infrastructure) from Microsoft Research, notably used by FxCop, and Mono.Cecil, which was more fully-featured and came with a much more permissive license. Both had APIs that were overkill for my needs, providing inspiration for a simpler library in OmniQuery.

Lessons learned
---------------

The first step was to understand the output of Mono.Cecil. I identified the initial set of relevant assembly data and wrote code to dump information about assemblies, modules, types, methods, references, resources, attributes and, optionally, instructions into a SQLite data file. You can find this in `OmniQuery.CodeAnalytics.CodeAnalysisDb`.

Armed with this information, I set out to build a simple API for querying code analysis results. The idea was to store the results of the analysis in memory and query the read-only data using LINQ to Objects. See `OmniQuery.CodeAnalytics.Linq.CodeAnalysis` for the implementation. One limitation of this approach was the inability to store instruction data, as that bloated the memory footprint immensely. I built out a test project to cross-compare the results of Mono.Cecil analysis (in either the SQLite DB form or queryable objects) against NDepend output. Check out `OmniQuery.Test/SampleClass.cs` for some crazy edge cases allowed in C#.

In parallel, I prototyped a GUI tool in `OmniQuery.CodeAnalytics.Ui` that allowed user to type pure LINQ and view results in a grid format, much like LINQPad. Due to the simplicity of the code analysis API, writing the UI tool proved to be a pretty simple affair.

Alas, progress ground to a halt unexpectedly over representation of types. Mono.Cecil returned type names just as they were internally defined in the CLR, which did not always map cleanly to the C# definition, particularly with generic types (List\<T\> would return as List`1, with information on the generic parameter T well-hidden). I knew it was possible to reconstruct the C# definition (.NET Reflector tool somehow managed it), however the more time I spent on this, the edge cases multiplied and the rathole got deeper. Eventually life intervened, time passed, and the PC with the source code died. Took me a couple of years to get around to resurrecting it and preserving the source here on GitHub.

It's worth noting that NDepend now comes with an API. Also, Microsoft's compiler-as-a-service project Roslyn that will play a major role in C# vNext looks fascinating. I plan to take a look at the CTP in the near future.
