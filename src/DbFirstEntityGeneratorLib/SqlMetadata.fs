namespace DbFirstEntityGeneratorLib

open System.ComponentModel.DataAnnotations.Schema
open Microsoft.EntityFrameworkCore

module SqlMetadata =

    [<CLIMutable>]
    [<Table("COLUMNS", Schema = "INFORMATION_SCHEMA")>]
    type ColumnInformation =
        { [<Column("TABLE_SCHEMA")>]
          tableSchema: string
          [<Column("TABLE_NAME")>]
          tableName: string
          [<Column("COLUMN_NAME")>]
          columnName: string
          [<Column("ORDINAL_POSITION")>]
          ordinalPosition: int
          [<Column("COLUMN_DEFAULT")>]
          columnDefault: string
          [<Column("IS_NULLABLE")>]
          isNullable: string // YES, NO
          [<Column("DATA_TYPE")>]
          dataType: string }


    type InformationDataContext(options: DbContextOptions<InformationDataContext>) =
        inherit DbContext(options)

        [<DefaultValue>]
        val mutable questions: DbSet<ColumnInformation>

        member x.Questions
            with get () = x.questions
            and set v = x.questions <- v


    let createInformationContext (connStr: string) =
        let optionsBuilder = new DbContextOptionsBuilder<InformationDataContext>()
        optionsBuilder.UseSqlServer(connStr) |> ignore
        new InformationDataContext(optionsBuilder.Options)
 