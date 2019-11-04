namespace DbFirstEntityGeneratorLib

open CommandLine
open System.IO
open System
open Serilog

module InitializeBaseDirectory =

    [<Verb("init", HelpText = "Initialize a directory with an empty genconfig.json file.")>]
    type InitOptions =
        { [<Option('d', "directory", Required = false,
                   HelpText = "The directory to initialize. If omitted the current directory will be used.")>]
          directory: string }

    [<Literal>]
    let private EmptyGenFonfig = @"
    {
      ""dbContexts"": {
        ""dbContext1"": {
          ""dbType"": ""SqlServer"",
          ""connectionString"": ""your connection string""
        },
        ""dbContext2"": {
          ""dbType"": ""SqlServer"",
          ""connectionString"": ""your conection string""
        }
      }
    }"

    let performDirectoryInitialization opts =
        printf "Going to initialize directory '%s'" opts.directory
        let specifiedDir =
            if not (String.IsNullOrWhiteSpace opts.directory) then Some(opts.directory)
            else None

        try
            let directoryToInitialize = DirectoryResolver.resolve specifiedDir

            if not (Directory.Exists directoryToInitialize) then
                Directory.CreateDirectory directoryToInitialize |> ignore

            let genConfigPath = Path.Combine(directoryToInitialize, "genconfig.json")

            if File.Exists genConfigPath then
                Log.Error(sprintf "The generator config file %s already exists." genConfigPath)
                1
            else
                Log.Information(sprintf "Creating an empty generator config file at path %s." genConfigPath)
                File.WriteAllText(genConfigPath, EmptyGenFonfig)
                0
        with e ->
            Log.Error
                ("Failed to initialize the directory because of a completely unexpected exception: {exception}",
                 e.Message)
            1
