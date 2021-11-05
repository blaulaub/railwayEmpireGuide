//module BalanceTransport

open System.Collections.Generic

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

let inline pairCapacities
    (capacities: Capacities<'Good, 'Producer, 'Transporter, 'Consumer, 'Capacity>)
    (production: Map<'Good, Map<'Producer, 'Capacity>>)
    (transportation: Map<'Transporter, 'Capacity>)
    (consumption: Map<'Good, Map<'Consumer, 'Capacity>>)
    : Map<'Good, Map<'Producer, CapacityPair<'Capacity>>> * Map<'Transporter, CapacityPair<'Capacity>> * Map<'Good, Map<'Consumer, CapacityPair<'Capacity>>>
    =

    let inline add2 key1 key2 value map =
        let m = if map |> Map.containsKey key1 then map |> Map.find key1 else Map.empty
        map |> Map.add key1 (m |> Map.add key2 value)

    capacities.ProductionCapacities
    |> Seq.fold (fun map {Good = good; Producer = producer; Capacity = availableCapacity} ->
        let displacedCapacity = production |> Map.find good |> Map.find producer
        map |> add2 good producer { Available = availableCapacity; Displaced = displacedCapacity }
    ) Map.empty
    ,
    capacities.TransportCapacities
    |> Seq.fold (fun map {Producer = producer; Transporter = transporter; Consumer = consumer; Capacity = availableCapacity} ->
        let displacedCapacity = transportation |> Map.find transporter
        map |> Map.add transporter { Available = availableCapacity; Displaced = displacedCapacity }
    ) Map.empty
    ,
    capacities.ConsumptionCapacities
    |> Seq.fold (fun map {Good = good; Consumer = consumer; Capacity = availableCapacity} ->
        let displacedCapacity = consumption |> Map.find good |> Map.find consumer
        map |> add2 good consumer { Available = availableCapacity; Displaced = displacedCapacity }
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

let inline solve (setup: CapacitiesSetup<'Good, 'Producer, 'Transporter, 'Consumer, float>)
    =

    let capacities = setup.ToCapacities
    let displacements = capacities |> toDisplacements
    let (m1, m2,m3) = accumulateDisplacements displacements
    let (m4, m5, m6) = pairCapacities capacities m1 m2 m3
    let cfl = computeCFL m4 m5 m6
    cfl


do  // TEST
    // zero problem:
    // - no roles (no goods, no producers, no transporters, no consumers)
    let setup = CapacitiesSetup {
        ProductionCapacities = []
        TransportCapacities = []
        ConsumptionCapacities = []
    }

    assert
    (
        setup.ToCapacities
        |> toDisplacements
        |> Seq.isEmpty
    )


do  // TEST
    // simple problem:
    // - one good, zero capacities
    let setup = CapacitiesSetup {
        ProductionCapacities = [ { Good = 0; Producer = 0; Capacity = 0. } ]
        TransportCapacities = [ { Producer = 0; Transporter = 0; Consumer = 0; Capacity = 0. } ]
        ConsumptionCapacities = [ { Good = 0; Consumer = 0; Capacity = 0. } ]
    }

    assert
    (
        setup.ToCapacities
        |> toDisplacements
        |> Seq.exactlyOne
        |> (=) {
            Good = 0
            Producer = 0
            Transporter = 0
            Consumer = 0
            Capacity = 0.
        }
    )


do  // TEST
    // simple problem - minimum production:
    // - unique roles (one good, one producer, one transporter, one consumer)
    // - consumption (3.2) is greater than transport (2.0) is greater than production (1.6)
    let setup = CapacitiesSetup {
        ProductionCapacities = [ { Good = 0; Producer = 0; Capacity = 1.6 } ]
        TransportCapacities = [ { Producer = 0; Transporter = 0; Consumer = 0; Capacity = 2.0 } ]
        ConsumptionCapacities = [ { Good = 0; Consumer = 0; Capacity = 3.2 } ]
    }

    assert
    (
        setup.ToCapacities
        |> toDisplacements
        |> Seq.exactlyOne
        |> function
        | {
            Good = 0
            Producer = 0
            Transporter = 0
            Consumer = 0
            Capacity = capacity
          } when capacity > 0. -> true
        | _ -> false
    )


do  // TEST
    // simple problem - minimum consumption:
    // - unique roles (one good, one producer, one transporter, one consumer)
    // - production (3.2) is greater than transport (2.0) is greater than consumption (1.6)
    let setup = CapacitiesSetup {
        ProductionCapacities = [ { Good = 0; Producer = 0; Capacity = 3.2 } ]
        TransportCapacities = [ { Producer = 0; Transporter = 0; Consumer = 0; Capacity = 2.0 } ]
        ConsumptionCapacities = [ { Good = 0; Consumer = 0; Capacity = 1.6 } ]
    }

    assert
    (
        setup.ToCapacities
        |> toDisplacements
        |> Seq.exactlyOne
        |> function
        | {
            Good = 0
            Producer = 0
            Transporter = 0
            Consumer = 0
            Capacity = capacity
          } when capacity > 0. -> true
        | _ -> false
    )
