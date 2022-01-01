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

[<TestFixture>]
type ParseYamlGoodAndAmount() =

    [<Test>]
    member _.``parse good and amount`` () =

        // arrange
        let input = """
Good: Grain
Amount: 0.8
"""

        // act
        let goodAndAmount = DeserializerBuilder().Build().Deserialize<GoodAndAmount>(input)

        // assert
        Assert.AreEqual("Grain", goodAndAmount.Good)
        Assert.AreEqual(0.8, goodAndAmount.Amount)
