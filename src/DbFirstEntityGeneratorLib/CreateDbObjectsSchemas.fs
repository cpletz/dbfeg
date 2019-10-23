namespace DbFirstEntityGeneratorLib

open CommandLine

module CreateDbObjectsSchemas = 

    [<Verb("schemas", HelpText = "Create json schemas for db contexts.")>]
    type CreateDbObjectsSchemaOptions = {
        [<Option('d', "directory", Required=false, HelpText="The directory where the geninfo file resides. If omitted the current directory will be used.")>]
        directory : string
    }

    let createDbObjectsSchemas (opts : CreateDbObjectsSchemaOptions) =
        printf "Creating db objects schemas ... looking for the geninfo file in '%s'" opts.directory
        0

