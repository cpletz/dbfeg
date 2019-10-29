namespace DbFirstEntityGeneratorLib

open Serilog

module Logging =
    let init() =
        let mutable  loggerConf = LoggerConfiguration()
        loggerConf <- ColoredConsoleLoggerConfigurationExtensions.ColoredConsole(loggerConf.WriteTo)
        Log.Logger <- loggerConf.CreateLogger()
