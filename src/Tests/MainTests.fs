module MainTests

open Xunit
open DbFirstEntityGeneratorLib.Main
open System
open System.IO

let execWithConsole func =
    use testOut = new StringWriter()
    use testErr = new StringWriter()
    Console.SetOut testOut
    Console.SetError testErr
    let res = func()
    (res, testOut.ToString(), testErr.ToString())

[<Fact>]
let ``Running the generator without parameters should print some help info``() =
    let (res, consoleOutput, consoleError) = execWithConsole (fun () -> run [])
    Assert.NotEqual(0, res)
    Assert.True(consoleError.ToLower().Contains "no verb selected")


[<Fact>]
let ``Running the generator with the init verb``() =
    let testWorkDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())
    Directory.CreateDirectory testWorkDir |> ignore
    Environment.CurrentDirectory <- testWorkDir
    let (res, consoleOutput, consoleError) = execWithConsole (fun () -> run [ "init" ])
    Assert.Equal(0, res)
    let expectedFile = Path.Combine(testWorkDir, "genconfig.json")
    Assert.True (File.Exists expectedFile)
    Assert.True (consoleOutput.Contains(expectedFile))
