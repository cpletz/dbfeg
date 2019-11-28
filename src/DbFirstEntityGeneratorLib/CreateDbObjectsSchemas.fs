namespace DbFirstEntityGeneratorLib

open System
open System.IO
open System.Linq
open CommandLine
open GeneratorConfiguration
open SqlMetadata
open Microsoft.EntityFrameworkCore
open System.Text.Json

module CreateDbObjectsSchemas =

    let loadTablesAndColumns connStr =
        use db = createInformationContext connStr
        let columnInfos = db.Columns.AsNoTracking().ToArray()

        let tablesAndColumns = columnInfos |> Array.groupBy (fun x -> sprintf "%s.%s" x.tableSchema x.tableName)
        tablesAndColumns

    [<Verb("schemas", HelpText = "Create json schemas for db contexts.")>]
    type CreateDbObjectsSchemaOptions =
        { [<Option('d', "directory", Required = false,
                   HelpText =
                       "The directory where the geninfo file resides. If omitted the current directory will be used.")>]
          directory: string }

    type JsonType =
        | JObject
        | JBoolean
        | JString

    let private jsonTypeToString jt =
        match jt with
        | JObject -> "object"
        | JBoolean -> "boolean"
        | JString -> "string"

    let private writeDbObjectsJson (writer: Utf8JsonWriter) tablesAndColumns =

        let writePropertyWithObject (propertyName: string) contentWriter =
            writer.WritePropertyName propertyName
            writer.WriteStartObject()
            contentWriter()
            writer.WriteEndObject()

        let identifierPattern = Some "^[A-z][A-z|0-9|_]*$"

        let writeJsonType jsonType = writer.WriteString("type", jsonType |> jsonTypeToString)

        let writeSchemaPropertyWithPattern (pattern: string option) (name: string) (jsonType: JsonType) =
            writePropertyWithObject name (fun () ->
                writeJsonType jsonType
                match pattern with
                | Some p -> writer.WriteString("pattern", p)
                | None -> ())

        let writeSchemaObjectWithProperties propertyWriter =
            writeJsonType JObject
            writePropertyWithObject "properties" propertyWriter

        let writePropertyWithSchemaProperties propName propertyWriter =
            writePropertyWithObject propName (fun () -> writeSchemaObjectWithProperties propertyWriter)

        let writeSchemaProperty = writeSchemaPropertyWithPattern None

        let writeDefinitions() =
            writePropertyWithObject "definitions"
                (fun () ->
                writePropertyWithSchemaProperties "columnOverrides" (fun () ->
                    writeSchemaProperty "isNullable" JBoolean
                    writeSchemaProperty "dataType" JString
                    writeSchemaPropertyWithPattern identifierPattern "propertyName" JString))

        let writeTables() =
            writePropertyWithSchemaProperties "tables" (fun () ->
                tablesAndColumns
                |> Array.iter (fun (table, columns) ->
                    writePropertyWithObject table (fun () ->
                        writeSchemaObjectWithProperties (fun () ->
                            writeSchemaPropertyWithPattern identifierPattern "typeName" JString
                            writePropertyWithSchemaProperties "columns"
                                (fun () ->
                                columns
                                |> Array.iter
                                    (fun col ->
                                    writePropertyWithObject col.columnName
                                        (fun () -> writer.WriteString("$ref", "#/definitions/columnOverrides"))))

                            ))))

        writer.WriteStartObject()
        writer.WriteString("$schema", "http://json-schema.org/draft-04/schema#")
        writeDefinitions()
        writeSchemaObjectWithProperties writeTables
        writer.WriteEndObject()

    let createDbObjectSchema (name: string) (config: DbContextConfig) =
        let outputDir = DirectoryResolver.resolve None (config.directory |> Some)
        Directory.CreateDirectory outputDir |> ignore
        let outputPath = Path.Combine(outputDir, "dbobjects.schema.json")
        let tablesAndColumns = loadTablesAndColumns config.connectionString
        let jsonStream = new MemoryStream()
        use jsonWriter = new Utf8JsonWriter(jsonStream, JsonWriterOptions(Indented = true))
        writeDbObjectsJson jsonWriter tablesAndColumns
        jsonWriter.Flush()
        jsonStream.Seek(0L, SeekOrigin.Begin) |> ignore
        use sr = new StreamReader(jsonStream)
        let json = sr.ReadToEnd()
        File.WriteAllText(outputPath, json)
        0

    let createDbObjectsSchemas (opts: CreateDbObjectsSchemaOptions) =
        printf "Creating db objects schemas ... looking for the geninfo file in '%s'" opts.directory
        let specifiedDir =
            if not (String.IsNullOrWhiteSpace opts.directory) then Some(opts.directory)
            else None
        match createFromDirectory specifiedDir with
        | Ok genConfig ->
            let results =
                genConfig.dbContexts
                |> Seq.map (fun x -> createDbObjectSchema x.Key x.Value)
                |> Seq.toList
            results |> List.max
        | Error _ -> 1

