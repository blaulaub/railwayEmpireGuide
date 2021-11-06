module Main

open System

open RailwayEmpireGuide

type NamedBusiness = {
    Name : string
    Type : BusinessType
    Level : int
} with
    member x.ToBusiness : Business = { Type = x.Type; Level = x.Level }


[<EntryPoint>]
let main argv =

    let businesses = [
        { Name = "Rice Manor"; Type = GrainFarm; Level = 1 }
        { Name = "Brown Preserve"; Type = GrainFarm; Level = 1 }
        { Name = "Reed Fattening"; Type = CattleFarm; Level = 1 }
        { Name = "Stevens Manor"; Type = CornFarm; Level = 1 }
        { Name = "Ward Manor"; Type = CornFarm; Level = 1 }
        { Name = "Green Logging"; Type = LoggingCamp; Level = 1 }
    ]

    let cities = [
        {
            Name = "Sioux Falls"
            Population = 25_361
            Industries = ([
                { Type = MeatIndustry; Level = 1 }
            ]: Industry list)
        }
        {
            Name = "Pierre"
            Population = 18_055
            Industries = ([
                { Type = Breweries; Level = 1 }
            ]: Industry list)
        }
    ]


    let setup =
        BalanceTransport.CapacitiesSetup {
            ProductionCapacities =
                businesses
                |> List.collect (fun (business : NamedBusiness) ->
                    business.ToBusiness
                    |> Business.businessProduction
                    |> List.map (fun (good, amount) -> { Good = good; Producer = business.Name; Capacity = amount } : BalanceTransport.ProductionCapacity<_,_,float> )
                )
            TransportCapacities = [
                { Producer = "Rice Manor"; Transporter = 0; Consumer = "Pierre"; Capacity = 8.0*7./46. }
                { Producer = "Rice Manor"; Transporter = 1; Consumer = "Sioux Falls"; Capacity = 8.0*7./75. }
                { Producer = "Sioux Falls"; Transporter = 2; Consumer = "Pierre"; Capacity = 8.0*7./46. }
                { Producer = "Pierre"; Transporter = 3; Consumer = "Sioux Falls"; Capacity = 8.0*7./46. }
            ]
            ConsumptionCapacities =
                cities
                |> List.collect (fun (city: City) ->
                    city |> City.cityConsumption
                    |> Seq.map (fun (good, amount) -> { Good = good; Consumer = city.Name; Capacity = amount } : BalanceTransport.ConsumptionCapacity<_,_,float>)
                    |> Seq.toList
                )
        }

    setup
    |> BalanceTransport.solve
    |> fun (capacities, displacements) ->
        printfn "Remaining resources:"
        printfn "-------------------"
        printfn "%s" (BalanceTransport.printCapacities capacities)
        printfn ""
        printfn "Delivered resources:"
        printfn "-------------------"
        printfn "%s" (BalanceTransport.printCapacities displacements)

    0 // return an integer exit code