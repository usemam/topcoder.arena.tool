module TopCoder.Arena.Tool.Project

open System.IO

open FSharp.Data

type CsProject = XmlProvider<"csproj_schema.xml">

let private load (filePath : string) = CsProject.Load filePath

let excludeAll projPath =
    let project = load projPath
    let itemGroups =
        project.ItemGroups
        |> Seq.filter (fun g -> Seq.length g.Compiles = 0)
        |> Seq.toArray
    let newProject =
        CsProject.Project(
            project.ToolsVersion,
            project.DefaultTargets,
            project.Imports,
            project.PropertyGroups,
            itemGroups)
    File.WriteAllText(projPath, newProject.ToString())

let includeFiles projPath (fileNames : string seq) =
    let project = load projPath
    let compiles =
        fileNames
        |> Seq.map (fun file -> CsProject.Compile(file))
        |> Seq.toArray
    let itemGroup = CsProject.ItemGroup(Array.empty, None, compiles)
    let newProject =
        CsProject.Project(
            project.ToolsVersion,
            project.DefaultTargets,
            project.Imports,
            project.PropertyGroups,
            Array.append project.ItemGroups [| itemGroup |])
    File.WriteAllText(projPath, newProject.ToString())