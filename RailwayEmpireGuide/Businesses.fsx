module Businesses

#load "Goods.fsx"
open Goods

type BusinessType =
| GrainFarm
| CattleFarm
| CornFarm
| LoggingCamp
| CottonPlantation
| SugarPlantation
| MilkFarm
| VegetableFarm
| FruitPlantation
| CoalMine
| IronOreMine
| SaltMine
| ClayPit

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
