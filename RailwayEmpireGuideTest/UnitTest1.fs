module RailwayEmpireGuideTest

open NUnit.Framework

open RailwayEmpireGuide

[<SetUp>]
let Setup () =
    ()

[<Test>]
let ``can initialize world with no cities`` () =
    let world = { World.empty with Cities = [] }
    Assert.AreEqual(List.empty<string>, world.Cities)
