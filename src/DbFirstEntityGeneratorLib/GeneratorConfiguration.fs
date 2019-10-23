namespace DbFirstEntityGeneratorLib

open System.Text.Json
open System.Text.Json.Serialization
open System.Collections.Generic
open System.Text.RegularExpressions

module GeneratorConfiguration =

    type DbType =
        | SqlServer = 0

    type DbContextConfig =
        { dbType: DbType
          connectionString: string }

    type GenConfig =
        { dbContexts: Dictionary<string, DbContextConfig> }

    let dbContextNameRegEx = Regex("^[A-z][A-z|0-9|_]*$")

    let private validateGenConfig (genConfig: GenConfig) =
        genConfig.dbContexts.Keys
        |> Seq.filter (dbContextNameRegEx.IsMatch >> not)
        |> Seq.map
            (fun c ->
            sprintf
                "Invalid dbContext name '%s'. dbContext names must begin with a letter and must only contain letters, digits and underscores."
                c)
        |> Seq.toList

    let private jsonSerializerOptions =
        let o = JsonSerializerOptions()
        o.Converters.Add(JsonFSharpConverter())
        o.Converters.Add(JsonStringEnumConverter())
        o

    type CreateGenConfigResult = Result<GenConfig, string list>

    let createFromJson (json: string) =
        try
            let genConfig = JsonSerializer.Deserialize<GenConfig>(json, jsonSerializerOptions)
            let validationResult = validateGenConfig genConfig
            if validationResult.Length = 0 then Ok genConfig
            else Error validationResult

        with e -> Error [ "Failed to parse the gen-config json document."; e.Message ]
