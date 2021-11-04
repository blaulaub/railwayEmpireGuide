//module BalanceTransport

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

let toDisplacements
    (capacities: Capacities<'Good, 'Producer, 'Transporter, 'Consumer, 'Capacity>)
    : Displacement<'Good, 'Producer, 'Transporter, 'Consumer, 'Capacity> list =

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
                                    Capacity = production.Capacity
                                }
    }
    |> Seq.choose id
    |> Seq.toList


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
        |> (=) []
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
        |> (=) [{
            Good = 0
            Producer = 0
            Transporter = 0
            Consumer = 0
            Capacity = 0.
        }]
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
        |> (=) [{
            Good = 0
            Producer = 0
            Transporter = 0
            Consumer = 0
            Capacity = 1.6
        }]
    )
