namespace RailwayEmpireGuide

type City = {
    Name : string
    Population: int
    Industries: Industry list
}

module City =

    let cityConsumption (city: City) : (Good * float) seq =
        seq {
            for industry in city.Industries do yield! industry |> Industry.industryConsumption
            yield! city.Population |> Population.populationConsumption
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
