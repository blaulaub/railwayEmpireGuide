namespace RailwayEmpireGuide

type City = {
    Name: string
    Population: Population
}

and Population = Population of int

module City =
    let empty = {
        Name = ""
        Population = Population 0
    }
