namespace DbFirstEntityGeneratorLib

open CommandLine

module InitializeBaseDirectory = 

    [<Verb("init", HelpText = "Initialize a directory with an empty genconfig.json file.")>]
    type InitOptions = {
        [<Option('d', "directory", Required=false, HelpText="The directory to initialize. If omitted the current directory will be used.")>]
        directory : string
    }

    let performDirectoryInitialization opts =
        printf "Going to initialize directory '%s'" opts.directory
        0

