module Industries

open Goods

type IndustryType =
| Breweries
| MeatIndustry
| SawMills
| WeavingFactories
| Tailors

type Industry = {
    Type : IndustryType
    Level : int
}

let industryConsumption (industry: Industry) =
    match industry with
    | { Type = MeatIndustry; Level = 1 } -> [ (Cattle, 3.6) ]
    | { Type = MeatIndustry; Level = 2 } -> [ (Cattle, 7.2) ]
    | { Type = SawMills; Level = 1 } -> [ (Wood, 1.6) ]
    | { Type = Tailors; Level = 2 } -> [ (Cloth, 1.6) ]
    | { Type = Breweries; Level = 1 } -> [ (Grain, 0.8) ]
    | { Type = Breweries; Level = 2 } -> [ (Grain, 1.6) ]
    | { Type = WeavingFactories; Level = 3 } -> [ (Cotton, 6.4) ]
    | _ -> failwith (sprintf "not implemented: %A" industry)

let industryProduction (industry: Industry) =
    match industry with
    | { Type = MeatIndustry; Level = 1 } -> [ (Meat, 2.4); (Leather, 1.9) ]
    | { Type = MeatIndustry; Level = 2 } -> [ (Meat, 4.8); (Leather, 3.8) ]
    | { Type = SawMills; Level = 1 } -> [ (Lumber, 1.6) ]
    | { Type = Tailors; Level = 2 } -> [ (Clothing, 1.6) ]
    | { Type = Breweries; Level = 1 } -> [ (Beer, 1.6) ]
    | { Type = Breweries; Level = 2 } -> [ (Beer, 3.2) ]
    | { Type = WeavingFactories; Level = 3 } -> [ (Cloth, 6.4) ]
    | _ -> failwith (sprintf "not implemented: %A" industry)
