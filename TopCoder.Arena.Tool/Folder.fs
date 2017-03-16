module TopCoder.Arena.Tool.Folder

open System.IO

let getFileNames folderPath =
    Directory.EnumerateFiles(folderPath, "*.cs") |> Seq.map Path.GetFileName

let getProjectPath folderPath =
    Directory.EnumerateFiles(folderPath, "*.csproj") |> Seq.exactlyOne