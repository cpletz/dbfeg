namespace DbFirstEntityGeneratorLib

open System
open System.IO

module DirectoryResolver =

    let resolve (baseDir: string option) (relativeDir: string option) =
        let baseDir = match baseDir with
                        | Some d -> d
                        | None -> Environment.CurrentDirectory
        
        match relativeDir with
        | None -> baseDir
        | Some(path) ->
            match path with
            | "." -> baseDir
            | _ ->
                if Path.IsPathFullyQualified path then path
                else Path.Combine(baseDir, path)
