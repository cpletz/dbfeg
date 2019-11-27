open MainTests
open CreateDbObjectsSchemaTests
open LoadGenConfigTests
open Xunit

module AssemblyAttr = 
    [<assembly: CollectionBehavior(DisableTestParallelization = true)>]
    do()


module Program = 

    let [<EntryPoint>] main _ = 
        // ``Generate the schema``()
        ``GenConfig should be loaded nicely``()
        0
