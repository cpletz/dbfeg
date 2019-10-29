open CommandLine

open DbFirstEntityGeneratorLib.Main
open System

[<EntryPoint>]
let main argv =
    let ret = run argv
    Console.ReadKey() |> ignore
    ret
