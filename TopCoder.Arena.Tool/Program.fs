namespace TopCoder.Arena.Tool

open System

module Program =

    [<EntryPoint>]
    let main argv = 
        
        let projPath = Folder.getProjectPath Environment.CurrentDirectory

        let command = argv.[0]
        match command.ToLowerInvariant() with
        | "include" ->
            Environment.CurrentDirectory
            |> Folder.getFileNames
            |> Project.includeFiles projPath
        | "exclude" ->
            Project.excludeAll projPath
        | _ -> printfn "Command wasn't recognized. Use either 'include' or 'exclude'."

        0
