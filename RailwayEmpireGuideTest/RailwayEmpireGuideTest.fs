namespace RailwayEmpireGuideTest

open NUnit.Framework

open YamlDotNet.Serialization


type CityData() =
    member val Population = 0 with get, set
    member val Consumes : GoodAndAmount array = Array.empty with get, set

type State() =
    member val Cities : City array = Array.empty with get, set
    member val CityData : CityData array = Array.empty with get, set
    member val FactoryData : FactoryData array = Array.empty with get, set

[<TestFixture>]
type WorldTests() =

    [<SetUp>]
    member _.Setup () =
        ()

    [<Test>]
    member _.``can parse city data in game state`` () =

        // arrange
        let input = """
CityData:
  - Population: 16258
    Consumes:
      - { Good: Grain, Amount: 0.1 }
      - { Good: Corn , Amount: 0.2 }
"""

        // act
        let state = DeserializerBuilder().Build().Deserialize<State>(input)

        // assert
        Assert.AreEqual(1, state.CityData.Length)
        let cityData = state.CityData[0]
        Assert.AreEqual(16258, cityData.Population)
        Assert.AreEqual(2, cityData.Consumes.Length)
        let consumation = cityData.Consumes
        Assert.AreEqual(GoodAndAmount(Good= "Grain", Amount= 0.1), consumation[0])
        Assert.AreEqual(GoodAndAmount(Good= "Corn", Amount= 0.2), consumation[1])

    [<Test>]
    member _.``can parse factory data in game state`` () =

        // arrange
        let input = """
FactoryData:
  - Name: "Breweries"
    Throughput:
      - Size: 1
        Consumes: [{ Good: Grain, Amount: 0.8 }]
        Produces: [{ Good: Beer, Amount: 1.6 }]
"""

        // act
        let state = DeserializerBuilder().Build().Deserialize<State>(input)

        // assert
        Assert.AreEqual(1, state.FactoryData.Length)
        let factoryData = state.FactoryData[0]
        Assert.AreEqual("Breweries", factoryData.Name)
        Assert.AreEqual(1, factoryData.Throughput.Length)
        let throughput = factoryData.Throughput[0]
        Assert.AreEqual(1, throughput.Size)
        Assert.AreEqual(1, throughput.Consumes.Length)
        Assert.AreEqual(GoodAndAmount(Good= "Grain", Amount= 0.8), throughput.Consumes[0])
        Assert.AreEqual(1, throughput.Produces.Length)
        Assert.AreEqual(GoodAndAmount(Good= "Beer", Amount= 1.6), throughput.Produces[0])

    [<Test>]
    member _.``can parse cities in game state`` () =

        // arrange
        let input = """
Cities:
  - Name: "Jackson"
    Population: 16258
"""

        // act
        let state = DeserializerBuilder().Build().Deserialize<State>(input)

        // assert
        Assert.AreEqual(1, state.Cities.Length)
        let city = state.Cities[0]
        Assert.AreEqual("Jackson", city.Name)
        Assert.AreEqual(16258, city.Population)
