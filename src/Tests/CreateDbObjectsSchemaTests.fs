module CreateDbObjectsSchemaTests

open Xunit
open DbFirstEntityGeneratorLib.Main
open System.IO

let simpleGenFonfig = @"
{
  ""dbContexts"": {
    ""db1"": {
      ""dbType"": ""SqlServer"",
      ""connectionString"": ""server=tcp:localhost,15789;database=ENS;user id=sa;password=TvdManager17;MultipleActiveResultSets=true;"",
      ""directory"": ""db1Objects""
        }
    }
}"

let prepareWorkingDirectory() =
    let path = TestUtils.prepareTestWorkingDirectory()
    let genConfigPath = Path.Combine(path, "genconfig.json")
    File.WriteAllText(genConfigPath, simpleGenFonfig)
    path

[<Fact>]
let ``Generate the schema``() =
    let workDir = prepareWorkingDirectory()
    let (res, consoleOutput, consoleError) = TestUtils.execWithConsole (fun () -> run [ "schemas"; "-d"; workDir ])
    Assert.Equal(0, res)
