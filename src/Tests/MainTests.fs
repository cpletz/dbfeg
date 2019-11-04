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

let prepareTestWorkingDirectory () =
    let testWorkDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())
    Directory.CreateDirectory testWorkDir |> ignore
    Environment.CurrentDirectory <- testWorkDir
    testWorkDir

[<Fact>]
let ``Running the generator without parameters should print some help info``() =
    let (res, consoleOutput, consoleError) = execWithConsole (fun () -> run [])
    Assert.NotEqual(0, res)
    Assert.True(consoleError.ToLower().Contains "no verb selected")

[<Fact>]
let ``init without options should initialize the current directory``() =
    let testWorkDir = prepareTestWorkingDirectory ()
    let (res, consoleOutput, consoleError) = execWithConsole (fun () -> run [ "init" ])
    Assert.Equal(0, res)
    let expectedFile = Path.Combine(testWorkDir, "genconfig.json")
    Assert.True (File.Exists expectedFile)
    Assert.True (consoleOutput.Contains(expectedFile))

[<Fact>]
let ``init with relative path should work relative to the current directory``() =
    let testWorkDir = prepareTestWorkingDirectory ()
    let relativeDir = "subdir"
    let (res, consoleOutput, consoleError) = execWithConsole (fun () -> run [ "init"; "-d"; relativeDir ])
    Assert.Equal(0, res)
    let expectedFile = Path.Combine(testWorkDir, relativeDir, "genconfig.json")
    Assert.True (File.Exists expectedFile)
    Assert.True (consoleOutput.Contains(expectedFile))
 
[<Fact>]
let ``init with absolute path should use that  directory``() =
    let testWorkDir = prepareTestWorkingDirectory ()
    let absoluteDir = Path.Combine(testWorkDir, "a/b/c")
    let (res, consoleOutput, consoleError) = execWithConsole (fun () -> run [ "init"; "-d"; absoluteDir ])
    Assert.Equal(0, res)
    let expectedFile = Path.Combine(absoluteDir, "genconfig.json")
    Assert.True (File.Exists expectedFile)
    Assert.True (consoleOutput.Contains(expectedFile))
 
[<Fact>]
let ``init with impossible path should complain``() =

    let absoluteDir = "x:/a/b/c"
    let (res, consoleOutput, consoleError) = execWithConsole (fun () -> run [ "init"; "-d"; absoluteDir ])
    Assert.NotEqual(0, res)
    Assert.True (consoleOutput.Contains(absoluteDir))
