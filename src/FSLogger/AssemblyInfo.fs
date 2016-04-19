namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("FSLogger")>]
[<assembly: AssemblyProductAttribute("FSLogger")>]
[<assembly: AssemblyDescriptionAttribute("F#ing simple logger for F#.")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
