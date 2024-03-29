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

        override this.OnModelCreating builder =
            builder.Entity<ColumnInformation>().HasNoKey() |> ignore

        [<DefaultValue>]
        val mutable columns: DbSet<ColumnInformation>

        member x.Columns
            with get () = x.columns
            and set v = x.columns <- v


    let createInformationContext (connStr: string) =
        let optionsBuilder = new DbContextOptionsBuilder<InformationDataContext>()
        optionsBuilder.UseSqlServer(connStr) |> ignore
        new InformationDataContext(optionsBuilder.Options)
 