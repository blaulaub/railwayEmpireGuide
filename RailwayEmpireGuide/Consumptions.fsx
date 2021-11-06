module Consumptions

#load "BasicTypes.fsx"
open BasicTypes

type Business = {
    Type : BusinessType
    Level : int
}

let businessProduction (business: Business) =
    match business with
    // basic
    | { Type = GrainFarm; Level = 3 } -> [ (Grain, 9.6) ]
    | { Type = CattleFarm; Level = 3 } -> [ (Cattle, 9.6) ]
    | { Type = CornFarm; Level = 4 } -> [ (Corn, 11.1) ]
    | { Type = LoggingCamp; Level = 2 } -> [ (Wood, 7.1) ]
    | { Type = CottonPlantation; Level = 4 } -> [ (Wood, 11.1) ]
    // extra
    | { Type = SugarPlantation; Level = 2 } -> [ (Sugar, 2.4) ]
    | { Type = MilkFarm; Level = 3 } -> [ (Milk, 6.4) ]
    | { Type = VegetableFarm; Level = 3 } -> [ (Vegetables, 3.2) ]
    | { Type = FruitPlantation; Level = 2 } -> [ (Fruit, 2.4) ]
    | { Type = CoalMine; Level = 1 } -> [ (Coal, 0.8) ]
    | { Type = IronOreMine; Level = 1 } -> [ (Iron, 0.8) ]
    | { Type = SaltMine; Level = 1 } -> [ (Salt, 0.8) ]
    | { Type = ClayPit; Level = 1 } -> [ (Salt, 0.8) ]
    | _ -> failwith (sprintf "not implemented: %A" business)

type Industry = {
    Type : IndustryType
    Level : int
}

let industryConsumption (industry: Industry) =
    match industry with
    | { Type = MeatIndustry; Level = 2 } -> [ (Cattle, 7.2) ]
    | { Type = SawMills; Level = 1 } -> [ (Wood, 1.6) ]
    | { Type = Tailors; Level = 2 } -> [ (Cloth, 1.6) ]
    | { Type = Breweries; Level = 2 } -> [ (Grain, 1.6) ]
    | { Type = WeavingFactories; Level = 3 } -> [ (Cotton, 6.4) ]
    | _ -> failwith (sprintf "not implemented: %A" industry)

let industryProduction (industry: Industry) =
    match industry with
    | { Type = MeatIndustry; Level = 2 } -> [ (Meat, 4.8); (Leather, 3.8) ]
    | { Type = SawMills; Level = 1 } -> [ (Lumber, 1.6) ]
    | { Type = Tailors; Level = 2 } -> [ (Clothing, 1.6) ]
    | { Type = Breweries; Level = 2 } -> [ (Beer, 3.2) ]
    | { Type = WeavingFactories; Level = 3 } -> [ (Cloth, 6.4) ]
    | _ -> failwith (sprintf "not implemented: %A" industry)

type PopulationAbstractConsumption = { Basic: float; Extra: float }

let populationAbstractConsumption (population: int) =
    match population with
    | p when (p >=  10_206 && p <=  12_682) -> { Basic = 0.2; Extra = 0.0 }
    | p when (p >=  16_891 && p <=  20_207) -> { Basic = 0.3; Extra = 0.0 }
    | p when (p >=  26_897 && p <=  29_539) -> { Basic = 0.5; Extra = 0.0 }
    | p when (p >=  37_990 && p <=  37_990) -> { Basic = 0.7; Extra = 0.3 }
    | p when (p >=  42_768 && p <=  44_120) -> { Basic = 0.8; Extra = 0.4 }
    | p when (p >=  50_192 && p <=  50_192) -> { Basic = 0.9; Extra = 0.5 }
    | p when (p >=  62_181 && p <=  63_620) -> { Basic = 1.1; Extra = 0.6 }
    | p when (p >=  67_134 && p <=  67_134) -> { Basic = 1.2; Extra = 0.6 }
    | p when (p >= 110_874 && p <= 110_874) -> { Basic = 1.9; Extra = 1.1 }
    | p -> failwith (sprintf "not implemented: %d" p)

let populationConsumption (population: int) =
    let { Basic = basic; Extra = extra } = population |> populationAbstractConsumption

    seq {
        yield (Grain, basic)
        yield (Corn, basic)
        yield (Wood, basic)
        yield (Meat, basic)
        yield (Beer, basic)
        if (population >= 30_000) then yield (Sugar, extra)
        if (population >= 35_000) then yield (Cloth, extra)
        if (population >= 40_000) then yield (Milk, extra)
        if (population >= 45_000) then yield (Vegetables, extra)
        if (population >= 50_000) then yield (Fruit, extra)
        if (population >= 55_000) then yield (Clothing, extra)
        if (population >= 60_000) then yield (Lumber, extra)
        if (population >= 65_000) then yield (Liquor, extra)
        if (population >= 70_000) then yield (DairyProducts, extra)
        if (population >= 85_000) then yield (Leather, extra)
        if (population >= 90_000) then yield (Salt, extra)
        if (population >= 100_000) then yield (Paper, extra)
        if (population >= 105_000) then yield (Tools, extra)
        if (population >= 110_000) then yield (Pottery, extra)
        if (population >= 115_000) then yield (Furniture, extra)
    }

type City = {
    Name : string
    Population: int
    Industries: Industry list
}

let cityConsumption (city: City) =
    seq {
        for industry in city.Industries do yield! industry |> industryConsumption
        yield! city.Population |> populationConsumption
    }
    |> Seq.groupBy fst
    |> Seq.map (fun (good, amounts) -> (good, amounts |> Seq.sumBy snd))

let demo () =
    let cities = [
        {
            Name = "Sioux Falls"
            Population = 110_874
            Industries = [
                { Type = MeatIndustry; Level = 2 }
                { Type = SawMills; Level = 1 }
                { Type = Tailors; Level = 2 }
            ]
        }
        {
            Name = "Pierre"
            Population = 67_134
            Industries = [
                { Type = Breweries; Level = 2 }
                { Type = WeavingFactories; Level = 3 }
            ]
        }
    ]

    cities
    |> List.iter (fun city -> printfn "City: %s (%d)\n%A" city.Name city.Population (city |> cityConsumption))
