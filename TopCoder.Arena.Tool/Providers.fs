module TopCode.Arena.Tool.Providers

open FSharp.Data

type CSharpProject = XmlProvider<"csproj_schema.xml">