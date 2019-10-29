module LoadGenConfigTests

open Xunit
open DbFirstEntityGeneratorLib.GeneratorConfiguration

let simpleGenFonfig = @"
{
  ""dbContexts"": {
    ""db1"": {
      ""dbType"": ""SqlServer"",
      ""connectionString"": ""db1 conn-str""
    },
    ""db2"": {
      ""dbType"": ""SqlServer"",
      ""connectionString"": ""db2 conn-str""
    }
  }
}
"

[<Fact>]
let ``GenConfig should be loaded nicely`` () =
    let genConfigRes = createFromJson simpleGenFonfig
    match genConfigRes with
    | Ok genConfig ->
        Assert.True(genConfig.dbContexts.ContainsKey "db1")        
        Assert.True(genConfig.dbContexts.ContainsKey "db2")         
    | _ -> Assert.True(false, "No errors expected.")

let someInvalidJson = @"
{
  ""dbContexts"": [
    {
      ""name"": ""db1"",
      ""connectionString"": ""db1 conn-str""
    },
    {
      ""name"": ""db2"",
      ""connectionString"": ""db2 conn-str""
"

[<Fact>]
let ``Invalid json should result in a nice error`` () =
    let genConfigRes = createFromJson someInvalidJson
    match genConfigRes with
    | Ok _ ->
        Assert.True(false, "Here we expect an error.")      
    | Error errors -> 
        Assert.NotEmpty errors
 
 
let genFonfigWithInvalidDbContextName= @"
{
  ""dbContexts"": {
    ""invalid name"": {
      ""dbType"": ""SqlServer"",
      ""connectionString"": ""db1 conn-str""
    },
    ""db2"": {
      ""dbType"": ""SqlServer"",
      ""connectionString"": ""db2 conn-str""
    }
  }
}"

[<Fact>]
let ``GenConfig with invalid dbContext name should result in a nice error`` () =
    let genConfigRes = createFromJson genFonfigWithInvalidDbContextName
    match genConfigRes with
    | Ok _ ->
        Assert.True(false, "Here we expect an error.")      
    | Error errors -> 
        Assert.NotEmpty errors
        Assert.True
            (errors |> List.exists (fun e -> e.Contains "invalid name"))