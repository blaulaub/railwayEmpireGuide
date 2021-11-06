module BalanceTransport

type ProductionCapacity<'Good, 'Producer, 'Capacity> = {
    Good: 'Good
    Producer: 'Producer
    Capacity: 'Capacity
}

type ConsumptionCapacity<'Good, 'Consumer, 'Capacity> = {
    Good: 'Good
    Consumer: 'Consumer
    Capacity: 'Capacity
}

type TransportCapacity<'Producer, 'Transporter, 'Consumer, 'Capacity> = {
    Producer: 'Producer
    Transporter: 'Transporter
    Consumer: 'Consumer
    Capacity: 'Capacity
}

type Capacities<'Good, 'Producer, 'Transporter, 'Consumer, 'Capacity> = {
    ProductionCapacities: ProductionCapacity<'Good, 'Producer, 'Capacity> list
    TransportCapacities: TransportCapacity<'Producer, 'Transporter, 'Consumer, 'Capacity> list
    ConsumptionCapacities: ConsumptionCapacity<'Good, 'Consumer, 'Capacity> list
}
    with
        member x.ToZero =
            {
                ProductionCapacities = [
                    for { Good = good; Producer = producer; Capacity = capacity } in x.ProductionCapacities do
                        yield {
                            Good = good
                            Producer = producer
                            Capacity = 0.
                        }
                ]
                TransportCapacities = [
                    for { Producer = producer; Transporter = transporter; Consumer = consumer; Capacity = capacity } in x.TransportCapacities do
                        yield {
                            Producer = producer
                            Transporter = transporter
                            Consumer = consumer
                            Capacity = 0.
                        }
                ]
                ConsumptionCapacities = [
                    for { Good = good; Consumer = consumer; Capacity = capacity } in x.ConsumptionCapacities do
                        yield {
                            Good = good
                            Consumer = consumer
                            Capacity = 0.
                        }
                ]
            }


type CapacitiesSetup<'A, 'B, 'C, 'D, 'E> = CapacitiesSetup of Capacities<'A, 'B, 'C, 'D, 'E>
    with
        member x.ToCapacities = x |> fun (CapacitiesSetup capacities) -> capacities

type Displacement<'Good, 'Producer, 'Transporter, 'Consumer, 'Capacity> = {
    Good: 'Good
    Producer: 'Producer
    Transporter: 'Transporter
    Consumer: 'Consumer
    Capacity: 'Capacity
}

let inline toDisplacements
    (capacities: Capacities<'Good, 'Producer, 'Transporter, 'Consumer, 'Capacity>)
    : Displacement<'Good, 'Producer, 'Transporter, 'Consumer, 'Capacity> seq =

    seq {
        for production in capacities.ProductionCapacities do
            let producer = production.Producer
            let good = production.Good
            for transport in capacities.TransportCapacities do
                if producer <> transport.Producer
                then yield None
                else
                    let transporter = transport.Transporter
                    for consumption in capacities.ConsumptionCapacities do
                        if good <> consumption.Good
                        then yield None
                            else
                                let consumer = consumption.Consumer
                                yield Some {
                                    Good = good
                                    Producer = producer
                                    Transporter = transporter
                                    Consumer = consumer
                                    Capacity = production.Capacity * consumption.Capacity * transport.Capacity
                                }
    }
    |> Seq.choose id

let inline update1 key capacity map =
    let v = if map |> Map.containsKey key then (map |> Map.find key) + capacity else capacity
    map |> Map.add key v

let inline update2 key1 key2 capacity map =
    let m = if map |> Map.containsKey key1 then map |> Map.find key1 else Map.empty
    let v = if m |> Map.containsKey key2 then (m |> Map.find key2) + capacity else capacity
    map |> Map.add key1 (m |> Map.add key2 v)

let inline accumulateDisplacements
    (displacements: Displacement<'Good, 'Producer, 'Transporter, 'Consumer, 'Capacity> seq)
    =

    displacements
    |> Seq.fold (fun (production, transportation, consumption) { Good = good; Producer = producer; Transporter = transporter; Consumer = consumer; Capacity = capacity } ->
        (
            production |> update2 good producer capacity,
            transportation |> update1 transporter capacity,
            consumption |> update2 good consumer capacity
        )
    ) (Map.empty, Map.empty, Map.empty)

type CapacityPair<'Capacity> = {
    Available: 'Capacity
    Displaced: 'Capacity
}

let pairCapacities
    (capacities: Capacities<'Good, 'Producer, 'Transporter, 'Consumer, 'Capacity>)
    (production: Map<'Good, Map<'Producer, 'Capacity>>)
    (transportation: Map<'Transporter, 'Capacity>)
    (consumption: Map<'Good, Map<'Consumer, 'Capacity>>)
    : Map<'Good, Map<'Producer, CapacityPair<'Capacity>>> * Map<'Transporter, CapacityPair<'Capacity>> * Map<'Good, Map<'Consumer, CapacityPair<'Capacity>>>
    =

    let add2 key1 key2 value map =
        let m = if map |> Map.containsKey key1 then map |> Map.find key1 else Map.empty
        map |> Map.add key1 (m |> Map.add key2 value)

    capacities.ProductionCapacities
    |> Seq.fold (fun map {Good = good; Producer = producer; Capacity = availableCapacity} ->
        match production |> Map.tryFind good |> Option.map (Map.tryFind producer) |> Option.flatten with
        | Some displacedCapacity -> map |> add2 good producer { Available = availableCapacity; Displaced = displacedCapacity }
        | None -> map
    ) Map.empty
    ,
    capacities.TransportCapacities
    |> Seq.fold (fun map {Producer = producer; Transporter = transporter; Consumer = consumer; Capacity = availableCapacity} ->
        match transportation |> Map.tryFind transporter with
        | Some displacedCapacity -> map |> Map.add transporter { Available = availableCapacity; Displaced = displacedCapacity }
        | None -> map
    ) Map.empty
    ,
    capacities.ConsumptionCapacities
    |> Seq.fold (fun map {Good = good; Consumer = consumer; Capacity = availableCapacity} ->
        match consumption |> Map.tryFind good |> Option.map (Map.tryFind consumer) |> Option.flatten with
        | Some displacedCapacity -> map |> add2 good consumer { Available = availableCapacity; Displaced = displacedCapacity }
        | None -> map
    ) Map.empty

let inline computeCFL
    (production: Map<'Good, Map<'Producer, CapacityPair<float>>>)
    (transportation: Map<'Transporter, CapacityPair<float>>)
    (consumption: Map<'Good, Map<'Consumer, CapacityPair<float>>>)
    : float
    =

    seq {
        for KeyValue(_, map) in production do
            for KeyValue(_, pair) in map do
                yield pair
        for KeyValue(_, pair) in transportation do
            yield pair
        for KeyValue(_, map) in consumption do
            for KeyValue(_, pair) in map do
                yield pair
    }
    |> Seq.map (fun {Available = available; Displaced = displaced} ->
        if displaced = 0. then None else Some (available/displaced)
    )
    |> Seq.choose id
    |> Seq.min

let iterateOnce
    (capacities: Capacities<'Good, 'Producer, 'Transporter, 'Consumer, float>)
    (displacements: Capacities<'Good, 'Producer, 'Transporter, 'Consumer, float>)
    =

    let (m1, m2,m3) = capacities |> toDisplacements |> accumulateDisplacements
    let (production, transport, consumption) = pairCapacities capacities m1 m2 m3
    let cfl = 0.9 * computeCFL production transport consumption

    let capacities = {
        ProductionCapacities = [
            for { Good = good; Producer = producer; Capacity = capacity } in capacities.ProductionCapacities do
                match production |> Map.tryFind good |> Option.map (Map.tryFind producer) |> Option.flatten with
                | Some {Available = available; Displaced = displaced} ->
                    yield {
                        Good = good
                        Producer = producer
                        Capacity = capacity - displaced * cfl
                    }
                | None -> ()
        ]
        TransportCapacities = [
            for { Producer = producer; Transporter = transporter; Consumer = consumer; Capacity = capacity } in capacities.TransportCapacities do
                match transport |> Map.tryFind transporter with
                | Some {Available = available; Displaced = displaced} ->
                    yield {
                        Producer = producer
                        Transporter = transporter
                        Consumer = consumer
                        Capacity = capacity - displaced * cfl
                    }
                | None -> ()
        ]
        ConsumptionCapacities = [
            for { Good = good; Consumer = consumer; Capacity = capacity } in capacities.ConsumptionCapacities do
                match consumption |> Map.tryFind good |> Option.map (Map.tryFind consumer) |> Option.flatten with
                | Some {Available = available; Displaced = displaced} ->
                    yield {
                        Good = good
                        Consumer = consumer
                        Capacity = capacity - displaced * cfl
                    }
                | None -> ()
        ]
    }

    let displacements = {
        ProductionCapacities = [
            for { Good = good; Producer = producer; Capacity = capacity } in displacements.ProductionCapacities do
                match production |> Map.tryFind good |> Option.map (Map.tryFind producer) |> Option.flatten with
                | Some {Available = available; Displaced = displaced} ->
                    yield {
                        Good = good
                        Producer = producer
                        Capacity = capacity + displaced * cfl
                    }
                | None -> ()
        ]
        TransportCapacities = [
            for { Producer = producer; Transporter = transporter; Consumer = consumer; Capacity = capacity } in displacements.TransportCapacities do
                match transport |> Map.tryFind transporter with
                | Some {Available = available; Displaced = displaced} ->
                    yield {
                        Producer = producer
                        Transporter = transporter
                        Consumer = consumer
                        Capacity = capacity + displaced * cfl
                    }
                | None -> ()
        ]
        ConsumptionCapacities = [
            for { Good = good; Consumer = consumer; Capacity = capacity } in displacements.ConsumptionCapacities do
                match consumption |> Map.tryFind good |> Option.map (Map.tryFind consumer) |> Option.flatten with
                | Some {Available = available; Displaced = displaced} ->
                    yield {
                        Good = good
                        Consumer = consumer
                        Capacity = capacity + displaced * cfl
                    }
                | None -> ()
        ]
    }

    capacities, displacements

let solve (setup: CapacitiesSetup<'Good, 'Producer, 'Transporter, 'Consumer, float>)
    : Capacities<'Good, 'Producer, 'Transporter, 'Consumer, float> * Capacities<'Good, 'Producer, 'Transporter, 'Consumer, float> =

    let capacities = setup.ToCapacities
    let displacements = capacities.ToZero

    let capacities, displacements =
        seq {1..10} |> Seq.fold (fun (c, d) _ -> iterateOnce c d) (capacities, displacements)

    capacities, displacements

let inline printCapacities
    (capacities: Capacities<'Good, 'Producer, 'Transporter, 'Consumer, float>)
    : string =

    let fields =
        [
            for { Good = good; Producer = producer; Capacity = capacity } in capacities.ProductionCapacities do
                yield (sprintf "%A" good, sprintf "%A" producer, "", "", sprintf "%.2f" capacity)
            for { Producer = producer; Transporter = transporter; Consumer = consumer; Capacity = capacity } in capacities.TransportCapacities do
                yield ("", sprintf "%A" producer, sprintf "%A" transporter, sprintf "%A" consumer, sprintf "%.2f" capacity)
            for { Good = good; Consumer = consumer; Capacity = capacity } in capacities.ConsumptionCapacities do
                yield (sprintf "%A" good, "", "", sprintf "%A" consumer, sprintf "%.2f" capacity)
        ]

    let w0,w1,w2,w3,w4 =
        fields
        |> Seq.fold (fun (w0,w1,w2,w3,w4) (t0,t1,t2,t3,t4) ->
            (
                (if w0 > t0.Length then w0 else t0.Length),
                (if w1 > t1.Length then w1 else t1.Length),
                (if w2 > t2.Length then w2 else t2.Length),
                (if w3 > t3.Length then w3 else t3.Length),
                (if w4 > t4.Length then w4 else t4.Length)
            )
        ) (0,0,0,0,0)

    fields
    |> Seq.map (fun (t0,t1,t2,t3,t4) -> (t0.PadRight(w0), t1.PadRight(w1), t2.PadRight(w2), t3.PadRight(w3), t4.PadLeft(w4)))
    |> Seq.map (fun (t0,t1,t2,t3,t4) -> sprintf "%s %s %s %s %s" t0 t1 t2 t3 t4)
    |> String.concat("\n")

let demo () =

    CapacitiesSetup {
        ProductionCapacities = [
            { Good = "Grain"; Producer = "Rice Manor"; Capacity = 2.4 }
        ]
        TransportCapacities = [
            { Producer = "Rice Manor"; Transporter = 0; Consumer = "Pierre"; Capacity = 8.0*7./46. }
            { Producer = "Rice Manor"; Transporter = 1; Consumer = "Fargo"; Capacity = 8.0*7./33. }
            { Producer = "Rice Manor"; Transporter = 2; Consumer = "Sioux Falls"; Capacity = 8.0*7./75. }
        ]
        ConsumptionCapacities = [
            { Good = "Grain"; Consumer = "Fargo"; Capacity = 1.0 }
            { Good = "Grain"; Consumer = "Pierre"; Capacity = 1.1 }
            { Good = "Grain"; Consumer = "Sioux Falls"; Capacity = 0.4 }
        ]
    }
    |> solve
    |> fun (capacities, displacements) ->
        printfn "Remaining resources:"
        printfn "-------------------"
        printfn "%s" (printCapacities capacities)
        printfn ""
        printfn "Delivered resources:"
        printfn "-------------------"
        printfn "%s" (printCapacities displacements)
