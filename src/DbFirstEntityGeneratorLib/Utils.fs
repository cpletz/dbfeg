namespace DbFirstEntityGeneratorLib

open System
open System.IO

module DirectoryResolver =

    let resolve (specifiedDir: string option) =
        match specifiedDir with
        | None -> Environment.CurrentDirectory
        | Some(path) ->
            match path with
            | "." -> Environment.CurrentDirectory
            | _ ->
                if Path.IsPathFullyQualified path then path
                else Path.Combine(Environment.CurrentDirectory, path)
