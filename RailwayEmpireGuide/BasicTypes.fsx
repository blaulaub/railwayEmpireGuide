module BasicTypes

type BusinessType =
| GrainFarm
| CattleFarm
| CornFarm
| LoggingCamp
| CottonPlantation
| SugarPlantation
| MilkFarm
| VegetableFarm
| FruitPlantation
| CoalMine
| IronOreMine
| SaltMine
| ClayPit

type IndustryType =
| Breweries
| MeatIndustry
| SawMills
| WeavingFactories
| Tailors

type StationType =
| Small
| Medium
| Large
| Terminal
| MediumWarehouse
| LargeWarehouse
| MediumSignaled
| LargeSignaled
| MediumWarehouseSignaled
| LargeWarehouseSignaled

type Level = Level of int

type WeeklyAmount = WeeklyAmount of float
type DailyAmount = DailyAmount of float

let toWeeklyAmount (DailyAmount amount) = WeeklyAmount (amount * 7.)
let toDailyAmount (WeeklyAmount amount) = DailyAmount (amount / 7.)
