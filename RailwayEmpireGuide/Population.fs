namespace RailwayEmpireGuide

module Population =

    type private PopulationAbstractConsumption = { Basic: float; Extra: float }

    let private populationAbstractConsumption (population: int) =
        match population with
        | p when (p >=  10_206 && p <=  12_682) -> { Basic = 0.2; Extra = 0.0 }
        | p when (p >=  16_891 && p <=  20_207) -> { Basic = 0.3; Extra = 0.0 }
        | p when (p >=  25_361 && p <=  25_361) -> { Basic = 0.4; Extra = 0.0 }
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
