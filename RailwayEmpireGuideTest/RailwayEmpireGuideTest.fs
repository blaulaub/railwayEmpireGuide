module RailwayEmpireGuideTest

open NUnit.Framework

open RailwayEmpireGuide

[<SetUp>]
let Setup () =
    ()

[<Test>]
let ``can initialize world with no cities`` () =
    // this is only an API syntax test
    let world = { World.empty with Cities = [] }
    Assert.AreEqual(List.empty<City>, world.Cities)

[<Test>]
let ``can initialize world with two cities`` () =
    // this is only an API syntax test
    let world = {
        World.empty with
            Cities = [
                { Name = "Pierre"; Population = Population 126_513 }
                { Name = "Rapid City"; Population = Population 109_213 }
            ]
    }
    Assert.AreEqual(2, world.Cities |> List.length)
    Assert.AreEqual(1, world.Cities |> List.filter (fun city -> city.Name = "Pierre") |> List.length)
    Assert.AreEqual(1, world.Cities |> List.filter (fun city -> city.Name = "Rapid City") |> List.length)
