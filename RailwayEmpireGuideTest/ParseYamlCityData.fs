namespace RailwayEmpireGuideTest

open NUnit.Framework
open YamlDotNet.Serialization

type CityData() =
    member val Population = 0 with get, set
    member val Consumes : GoodAndAmount array = Array.empty with get, set

[<TestFixture>]
type ParseYamlCityData() =

    [<SetUp>]
    member _.Setup () =
        ()

    [<Test>]
    member _.``parse city data`` () =

        // arrange
        let input = """
Population: 16258
Consumes:
  - { Good: Grain, Amount: 0.1 }
  - { Good: Corn , Amount: 0.2 }
"""

        // act
        let cityData = DeserializerBuilder().Build().Deserialize<CityData>(input)

        // assert
        Assert.AreEqual(16258, cityData.Population)
        Assert.AreEqual(2, cityData.Consumes.Length)
        let consumation = cityData.Consumes
        Assert.AreEqual(GoodAndAmount(Good= "Grain", Amount= 0.1), consumation[0])
        Assert.AreEqual(GoodAndAmount(Good= "Corn", Amount= 0.2), consumation[1])
