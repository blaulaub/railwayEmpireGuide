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

type CapacitiesSetup<'A, 'B, 'C, 'D, 'E> = CapacitiesSetup of Capacities<'A, 'B, 'C, 'D, 'E>
    with
        member x.ToCapacities = x |> fun (CapacitiesSetup capacities) -> capacities

type Displacement<'Good, 'Producer, 'Transporter, 'Consumer, 'Capacity> = {
    Good: 'Good
    Producer: 'Producer
    Transporter: 'Transporter
    Consumer: 'Consumer
}


do
    // simple problem:
    // - unique roles (one good, one producer, one transporter, one consumer)
    // - consumption (3.2) is greater than transport (2.0) is greater than productin (1.6)
    let setup = CapacitiesSetup {
        ProductionCapacities = [ { Good = 0; Producer = 0; Capacity = 1.6 } ]
        TransportCapacities = [ { Producer = 0; Transporter = 0; Consumer = 0; Capacity = 2.0 } ]
        ConsumptionCapacities = [ { Good = 0; Consumer = 0; Capacity = 3.2 } ]
    }
    ()
