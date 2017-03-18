module TopCoder.Arena.Tool.Export

open System
open System.IO

let (++) a b =
    if String.IsNullOrEmpty a then b
    elif String.IsNullOrEmpty b then a
    else a + Environment.NewLine + b

let reduce f xs =
    match xs |> Seq.length with
    | 0 -> String.Empty
    | _ ->
        xs
        |> Seq.map f
        |> Seq.reduce (++)

let exportImports (is : CsProject.Import []) =
    let exportImport (i : CsProject.Import) =
        match i.Condition with
        | Some cond -> sprintf "<Import Project=\"%s\" Condition=\"%s\" />" i.Project cond
        | None -> sprintf "<Import Project=\"%s\" />" i.Project
    
    is |> reduce exportImport

let exportPropertyGroups (ps : CsProject.PropertyGroup []) =
    let exportPrimitive name value =
        match value with
        | Some v -> sprintf "<%s>%s</%s>" name (v.ToString()) name
        | None -> String.Empty

    let exportConfiguration (cfg : CsProject.Configuration option) =
        match cfg with
        | None -> String.Empty
        | Some v -> sprintf "<Configuration Condition=\"%s\">%s</Configuration>" v.Condition v.Value

    let exportPlatform (p : CsProject.Platform option) =
        match p with
        | None -> String.Empty
        | Some v -> sprintf "<Platform Condition=\"%s\">%s</Platform>" v.Condition v.Value

    let exportPropertyGroup (p : CsProject.PropertyGroup) =
        if (p.Condition.IsNone)
            then "<PropertyGroup>"
            else sprintf "<PropertyGroup Condition=\"%s\">" p.Condition.Value
        ++ exportConfiguration p.Configuration
        ++ exportPlatform p.Platform
        ++ exportPrimitive "ProjectGuid" p.ProjectGuid
        ++ exportPrimitive "OutputType" p.OutputType
        ++ exportPrimitive "AppDesignerFolder" p.AppDesignerFolder
        ++ exportPrimitive "RootNamespace" p.RootNamespace
        ++ exportPrimitive "AssemblyName" p.AssemblyName
        ++ exportPrimitive "TargetFrameworkVersion" p.TargetFrameworkVersion
        ++ exportPrimitive "FileAlignment" p.FileAlignment
        ++ exportPrimitive "PlatformTarget" p.PlatformTarget
        ++ exportPrimitive "DebugSymbols" p.DebugSymbols
        ++ exportPrimitive "DebugType" p.DebugType
        ++ exportPrimitive "Optimize" p.Optimize
        ++ exportPrimitive "OutputPath" p.OutputPath
        ++ exportPrimitive "DefineConstants" p.DefineConstants
        ++ exportPrimitive "ErrorReport" p.ErrorReport
        ++ exportPrimitive "WarningLevel" p.WarningLevel
        ++ "</PropertyGroup>"

    ps |> reduce exportPropertyGroup

let exportItemGroups (is : CsProject.ItemGroup []) =
    let exportFolder (folder : CsProject.Folder option) =
        match folder with
        | Some f -> sprintf "<Folder Include=\"%s\" />" f.Include
        | None -> String.Empty
    
    let exportRefs (refs : CsProject.Reference []) =
        let exportRef (ref : CsProject.Reference) =
            sprintf "<Reference Include=\"%s\" />" ref.Include

        refs |> reduce exportRef

    let exportCompiles (cs : CsProject.Compile []) =
        let exportCompile (c : CsProject.Compile) =
            sprintf "<Compile Include=\"%s\" />" c.Include

        cs |> reduce exportCompile

    let exportItemGroup (i : CsProject.ItemGroup) =
        "<ItemGroup>"
        ++ exportFolder i.Folder
        ++ exportRefs i.References
        ++ exportCompiles i.Compiles
        ++ "</ItemGroup>"

    is |> reduce exportItemGroup

let exportProject (p : CsProject.Project) =
    sprintf
        "<Project ToolsVersion=\"%s\" DefaultTargets=\"%s\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">"
        (p.ToolsVersion.ToString())
        p.DefaultTargets ++
    exportImports p.Imports.[..0] ++
    exportPropertyGroups p.PropertyGroups ++
    exportItemGroups p.ItemGroups ++
    exportImports p.Imports.[1..] ++
    "</Project>"

let save filePath (project : CsProject.Project) =
    File.WriteAllText(filePath, exportProject project)