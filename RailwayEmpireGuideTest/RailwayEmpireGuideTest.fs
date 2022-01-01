namespace RailwayEmpireGuideTest

open NUnit.Framework

open YamlDotNet.Serialization


type GoodAndAmount() =
    member val Good = "" with get, set
    member val Amount = 0. with get, set
    // to avoid this, I wonder if F# can deal with C#9 records somehow ?
    override this.GetHashCode() = this.Good.GetHashCode() * 17 + this.Amount.GetHashCode()
    override this.Equals(other: obj) = this.Equals(other :?> GoodAndAmount)
    member this.Equals(other: GoodAndAmount) = this.Good = other.Good && this.Amount = other.Amount

type CityData() =
    member val Population = 0 with get, set
    member val Consumes : GoodAndAmount array = Array.empty with get, set

type State() =
    member val CityData : CityData array = Array.empty with get, set

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
      - { Good: Wood , Amount: 0.3 }
      - { Good: Meat , Amount: 0.4 }
      - { Good: Beer , Amount: 0.5 }
"""

        // act
        let state = DeserializerBuilder().Build().Deserialize<State>(input)

        // assert
        Assert.AreEqual(1, state.CityData.Length)
        let cityData = state.CityData[0]
        Assert.AreEqual(16258, cityData.Population)
        Assert.AreEqual(5, cityData.Consumes.Length)
        let consumation = cityData.Consumes
        Assert.AreEqual(GoodAndAmount(Good= "Grain", Amount= 0.1), consumation[0])
        Assert.AreEqual(GoodAndAmount(Good= "Corn", Amount= 0.2), consumation[1])
        Assert.AreEqual(GoodAndAmount(Good= "Wood", Amount= 0.3), consumation[2])
        Assert.AreEqual(GoodAndAmount(Good= "Meat", Amount= 0.4), consumation[3])
        Assert.AreEqual(GoodAndAmount(Good= "Beer", Amount= 0.5), consumation[4])
