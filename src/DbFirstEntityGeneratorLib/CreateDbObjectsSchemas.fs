namespace DbFirstEntityGeneratorLib

open System
open System.IO
open System.Linq
open CommandLine
open Serilog
open GeneratorConfiguration
open SqlMetadata
open Microsoft.EntityFrameworkCore

module CreateDbObjectsSchemas =

    [<Verb("schemas", HelpText = "Create json schemas for db contexts.")>]
    type CreateDbObjectsSchemaOptions =
        { [<Option('d', "directory", Required = false,
                   HelpText =
                       "The directory where the geninfo file resides. If omitted the current directory will be used.")>]
          directory: string }

    let createDbObjectSchema (name: string) (config: DbContextConfig) =
        use db = createInformationContext config.connectionString
        let columnInfo = db.Columns.AsNoTracking().ToArray()
        0

    let createDbObjectsSchemas (opts: CreateDbObjectsSchemaOptions) =
        printf "Creating db objects schemas ... looking for the geninfo file in '%s'" opts.directory
        let specifiedDir =
            if not (String.IsNullOrWhiteSpace opts.directory) then Some(opts.directory)
            else None

        let genConfigDir = DirectoryResolver.resolve specifiedDir
        if not (Directory.Exists genConfigDir) then
            Log.Error(sprintf "The directory '%s' does not exist." genConfigDir)
            1
        else
            let genConfigPath = Path.Combine(genConfigDir, "genconfig.json")
            if not (File.Exists genConfigPath) then
                Log.Error(sprintf "The genconfig file at path '%s' could not be found." genConfigPath)
                1
            else
                let json = File.ReadAllText genConfigPath
                let genConfigResult = createFromJson json
                match genConfigResult with
                | Error msgs ->
                    Log.Error(sprintf "Failed to parse genconfig file '%s'. Problems:" genConfigPath)
                    msgs |> List.iter Log.Information
                    1
                | Ok genConfig ->
                    let results = 
                        genConfig.dbContexts 
                        |> Seq.map (fun x -> 
                                        createDbObjectSchema x.Key x.Value)
                        |> Seq.toList
                    results |> List.max
