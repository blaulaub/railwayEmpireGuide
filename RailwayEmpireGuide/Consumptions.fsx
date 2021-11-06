module Consumptions

#load "Industries.fsx"
open Industries

#load "PopulationConsumption.fsx"
open PopulationConsumption

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
