namespace RailwayEmpireGuide

type World = {
    Cities: City list
}

module World =
    let empty = {
        Cities = []
    }
