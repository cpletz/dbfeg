open CommandLine

open DbFirstEntityGeneratorLib.InitializeBaseDirectory
open DbFirstEntityGeneratorLib.CreateDbObjectsSchemas
open CommandLine.Text

[<EntryPoint>]
let main argv =
    let result = Parser.Default.ParseArguments<InitOptions, CreateDbObjectsSchemaOptions> argv

    //printf "%s" ((HelpText.AutoBuild result).ToString())

    match result with
    | :? CommandLine.Parsed<obj> as command -> 
        match command.Value with
        | :? CreateDbObjectsSchemaOptions -> createDbObjectsSchemas()
        | :? InitOptions as opts ->  performDirectoryInitialization opts
        | _ -> 1
    | :? CommandLine.NotParsed<obj> -> 1
    | _ -> 1
