namespace DbFirstEntityGeneratorLib

open CommandLine

open DbFirstEntityGeneratorLib.InitializeBaseDirectory
open DbFirstEntityGeneratorLib.CreateDbObjectsSchemas
open Serilog

module Main =

    let run args =
        Logging.init()

        let result = Parser.Default.ParseArguments<InitOptions, CreateDbObjectsSchemaOptions> args

        match result with
        | :? (CommandLine.Parsed<obj>) as command ->
            match command.Value with
            | :? CreateDbObjectsSchemaOptions as opts -> createDbObjectsSchemas opts
            | :? InitOptions as opts -> performDirectoryInitialization opts
            | _ -> 1
        | :? (CommandLine.NotParsed<obj>) as notParsed ->
            Log.Error "The command could not be parsed."
            notParsed.Errors
            |> Seq.iter (fun e -> Log.Error (e.ToString()))
            1
        | _ -> 1