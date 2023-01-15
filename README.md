# BenchmarkMonitoring
 C# project for uni

@startuml
BenchmarkService -> BenchmarkService: Create Data
BenchmarkService -> SQLite: Save to database
BenchmarkConsole -> SQLite: Get BenchmarkData
SQLite --> BenchmarkConsole 
BenchmarkConsole -> BenchmarkConsole : Show data
@enduml