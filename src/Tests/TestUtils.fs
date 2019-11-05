module TestUtils

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