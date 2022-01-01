namespace RailwayEmpireGuideTest

open NUnit.Framework

[<TestFixture>]
type WorldTests() =

    [<SetUp>]
    member _.Setup () =
        ()

    [<Test>]
    member _.``can initialize world with no cities`` () =
        Assert.Pass()
