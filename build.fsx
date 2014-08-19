// include Fake libs
#r "packages/FAKE/tools/FakeLib.dll"

open Fake

// Directories
let buildDir  = "./build/"

// Filesets
let appReferences  = 
    !! "DotNetRules.Events.Data.MongoDb/**/*.csproj"
    
// version info
let version = "0.2"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ -> 
    CleanDirs [buildDir]
)

Target "Build" (fun _ ->
    MSBuildRelease buildDir "Build" appReferences
        |> Log "AppBuild-Output: "
)

// Build order
"Clean"
  ==> "Build"

// start build
RunTargetOrDefault "Build"