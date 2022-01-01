namespace RailwayEmpireGuideTest

open NUnit.Framework

open RailwayEmpireGuide

[<TestFixture>]
type WorldTests() =

    let cityOfPierre : City =
        {
            Name = "Pierre"
            Population = Population 126_513
        }

    let cityOfRapidCity : City =
        {
            Name = "Rapid City"
            Population = Population 109_213
        }

    [<SetUp>]
    member _.Setup () =
        ()

    [<Test>]
    member _.``can initialize world with no cities`` () =
        // this is only an API syntax test
        let world = { World.empty with Cities = [] }
        Assert.AreEqual(List.empty<City>, world.Cities)

    [<Test>]
    member _.``can initialize world with two cities`` () =
        // this is only an API syntax test
        let world = { World.empty with Cities = [ cityOfPierre; cityOfRapidCity ] }
        Assert.AreEqual(2, world.Cities |> List.length)
        Assert.AreEqual(1, world.Cities |> List.filter (fun city -> city.Name = "Pierre") |> List.length)
        Assert.AreEqual(1, world.Cities |> List.filter (fun city -> city.Name = "Rapid City") |> List.length)
