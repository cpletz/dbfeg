module CreateDbObjectsSchemaTests

open Xunit
open DbFirstEntityGeneratorLib.Main
open System.IO

let simpleGenFonfig = @"
{
  ""dbContexts"": {
    ""db1"": {
      ""dbType"": ""SqlServer"",
      ""connectionString"": ""server=tcp:localhost,15789;database=ENS;user id=sa;password=TvdManager17;MultipleActiveResultSets=true;""
        }
    }
}"

let prepareWorkkingDirectory () =
    let path = TestUtils.prepareTestWorkingDirectory()
    let genConfigPath = Path.Combine(path, "genconfig.json")
    File.WriteAllText(genConfigPath, simpleGenFonfig)
    path

[<Fact>]
let ``Generate the schema``() =
    let workDir = prepareWorkkingDirectory ()
    let (res, consoleOutput, consoleError) = TestUtils.execWithConsole (fun () -> run ["schemas"; "-d"; workDir ])
    Assert.NotEqual(0, res)
