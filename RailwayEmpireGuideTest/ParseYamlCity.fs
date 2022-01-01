namespace RailwayEmpireGuideTest

open NUnit.Framework
open YamlDotNet.Serialization

type CityBusiness() =
    member val Type = "" with get, set
    member val Size = 0 with get, set
    // to avoid this, I wonder if F# can deal with C#9 records somehow ?
    override this.GetHashCode() = this.Type.GetHashCode() * 17 + this.Size.GetHashCode()
    override this.Equals(other: obj) = this.Equals(other :?> CityBusiness)
    member this.Equals(other: CityBusiness) = this.Type = other.Type && this.Size = other.Size

type ExpressData() =
    member val To = "" with get, set
    member val Passengers = 0 with get, set
    member val Mail = 0 with get, set
    // to avoid this, I wonder if F# can deal with C#9 records somehow ?
    override this.GetHashCode() = this.To.GetHashCode() * 17 + this.Passengers.GetHashCode() * 13 + this.Mail.GetHashCode()
    override this.Equals(other: obj) = this.Equals(other :?> ExpressData)
    member this.Equals(other: ExpressData) = this.To = other.To && this.Passengers = other.Passengers && this.Mail = other.Mail

type City() =
    member val Name = "" with get, set
    member val Population = 0 with get, set
    member val Express : ExpressData array = Array.empty with get, set
    member val Factories : CityBusiness array = Array.empty with get, set

[<TestFixture>]
type ParseYamlCity() =

    [<Test>]
    member _.``can parse cities in game state`` () =

        // arrange
        let input = """
Name: "Jackson"
Population: 16258
Express:
  - { To: "Memphis"  , Passengers: 7, Mail: 4 }
  - { To: "Nashville", Passengers: 3, Mail: 5 }
Factories:
  - { Type: "Breweries", Size: 1 }
"""

        // act
        let city = DeserializerBuilder().Build().Deserialize<City>(input)

        // assert
        Assert.AreEqual("Jackson", city.Name)
        Assert.AreEqual(16258, city.Population)
        Assert.AreEqual(2, city.Express.Length)
        let express = city.Express
        Assert.AreEqual(ExpressData(To="Memphis", Passengers=7, Mail=4), express[0])
        Assert.AreEqual(ExpressData(To="Nashville", Passengers=3, Mail=5), express[1])
        Assert.AreEqual(1, city.Factories.Length)
        Assert.AreEqual(CityBusiness(Type="Breweries", Size=1), city.Factories[0])
