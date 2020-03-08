//The MIT License (MIT)
//
//Copyright (c) 2019
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

module FSLogger

open System
open System.IO
open System.Text
open System.Globalization

type LogLevel =
    | Trace = 0
    | Debug = 1
    | Info = 2
    | Notice = 3
    | Warn = 4
    | Error = 5
    | Fatal = 6

/// Immutable struct holding information relating to a log entry.
[<Struct>]
type LogEntry(level : LogLevel, time : DateTime, path : string, message : string) = 

    static let separators = [| '\\'; '/' |]
    
    /// The log level of the message
    member __.Level = level
    
    /// The time at which the entry was logged
    member __.Time = time
    
    /// The source of the message
    member __.Path = path
    
    /// The actual log message
    member __.Message = message
    
    override __.ToString() = sprintf "[%A|%A]%s:%s" time level path message

    /// Retrieves a string representation of the long message using some default formatting. This is a more compact representation than ToString()
    member __.ShortString = 
        let sb = StringBuilder(path.Length)
        let idx = path.LastIndexOfAny(separators, path.Length - 1)
        let ts = DateTimeFormatInfo.CurrentInfo.LongTimePattern
        sb.Append('[')
          .Append(time.ToString(ts))
          .Append(']')
          .Append(path, idx + 1, max 0 (path.Length - 1 - idx))
          .Append(": ")
          .Append(message)
          .ToString()

/// Immmutable logger, which holds information about the logging context.
[<Struct>]
[<StructuredFormatDisplay("Logger: {path = path; consumer = consumer}")>]
type Logger internal (path : string, consumer : LogEntry -> unit) = 
    
    /// The current path of this logger
    member __.Path = path
    
    /// The current consumer for this logger
    member __.Consumer = consumer
    
    /// Logs an unformatted message at the specified level
    member internal __.Log level message = 
        let logEntry = LogEntry(level, DateTime.Now, path, message)
        consumer logEntry
             
    member x.Logf level format = Printf.ksprintf (x.Log level) format
    
    /// Logs the message at the trace level
    member x.T format =
        Printf.ksprintf (x.Log LogLevel.Trace) format
            
    /// Logs the message at the debug level
    member x.D format = 
        Printf.ksprintf (x.Log LogLevel.Debug) format
    
    /// Logs the message at the info level
    member x.I format =
        Printf.ksprintf (x.Log LogLevel.Info) format
            
    /// Logs the message at the Notice level
    member x.N format =
        Printf.ksprintf (x.Log LogLevel.Notice) format
            
    /// Logs the message at the warning level
    member x.W format = 
        Printf.ksprintf (x.Log LogLevel.Warn) format
    
    /// Logs the message at the error level
    member x.E format = 
        Printf.ksprintf (x.Log LogLevel.Error) format
    
    /// Logs the message at the fatal level
    member x.F format = 
        Printf.ksprintf (x.Log LogLevel.Fatal) format
    
    override __.ToString() = sprintf "Logger: {path = '%s'; consumer = %A}" path consumer

module Logger = 
    /// The default logger. Has no path and does nothing on consumption.
    let Default = Logger("", ignore)
    
    /// A logger that prints to std::out / std::err based on the context
    let Console = 
        let print (le:LogEntry) = 
            match le.Level with
            | LogLevel.Debug | LogLevel.Info -> printfn "%A" le
            | _ -> eprintfn "%A" le
        
        Logger("", print)

    /// A logger that prints to std::out / std::err based on the context, using a short form
    let ConsoleShort = 
        let print (le:LogEntry) = 
            match le.Level with
            | LogLevel.Debug | LogLevel.Info -> System.Console.WriteLine(le)
            | _ -> System.Console.Error.WriteLine(le.ShortString)
        
        Logger("", print)

    let private levelToCol l =
        match l with
        | LogLevel.Trace -> ConsoleColor.DarkGray
        | LogLevel.Debug -> ConsoleColor.Gray
        | LogLevel.Info -> ConsoleColor.Green
        | LogLevel.Notice -> ConsoleColor.Blue
        | LogLevel.Warn -> ConsoleColor.Yellow
        | LogLevel.Error -> ConsoleColor.Red
        | _ -> ConsoleColor.Magenta

    /// A logger that prints to std::out / std::err based on the context, with extra colourization
    let ColorConsole =
        let print (le:LogEntry) =
            System.Console.ResetColor()
            System.Console.ForegroundColor <- levelToCol le.Level
            match le.Level with
            | LogLevel.Debug | LogLevel.Info ->
                System.Console.WriteLine(le.ToString())
            | _ -> System.Console.Error.WriteLine(le.ToString())
            System.Console.ResetColor()
        Logger("", print)
        
    /// A logger that prints to std::out / std::err based on the context, with extra colourization, using a short form
    let ColorConsoleShort =
        let print (le:LogEntry) =
            System.Console.ResetColor()
            System.Console.ForegroundColor <- levelToCol le.Level
            match le.Level with
            | LogLevel.Debug | LogLevel.Info ->
                System.Console.WriteLine(le.ShortString)
            | _ -> System.Console.Error.WriteLine(le.ShortString)
            System.Console.ResetColor()
        Logger("", print)
    
    /// Creates a new logger with the provided consumer
    let withConsumer newConsumer (logger : Logger) = Logger(logger.Path, newConsumer)
    
    /// Creates a new logger with the provided path
    /// This accepts a format string.
    let withPath format =
        Printf.ksprintf (fun path (logger:Logger) -> Logger(path.Replace('\\', '/'), logger.Consumer)) format
    
    /// Creates a new logger with the provided path appended. This is useful for heirarchical logger pathing.
    /// This accepts a format string.
    let appendPath format =
        Printf.ksprintf (fun path (logger : Logger) -> Logger(Path.Combine(logger.Path, path).Replace('\\', '/'), logger.Consumer)) format
    
    /// Logs a message to the logger at the provided level.
    /// This accepts a format string.
    let logf level format =
        Printf.ksprintf (fun msg (logger:Logger) -> logger.Log level msg) format
         
    
    /// Adds a consumer to the logger, such that the new and current consumers are run.
    let addConsumer newConsumer (logger : Logger) = 
        let curConsumer = logger.Consumer
        
        let consume l = 
            curConsumer l
            newConsumer l
        logger |> withConsumer consume
        
    /// Removes all consumers from the logger.
    let removeConsumer (logger : Logger) = logger |> withConsumer Unchecked.defaultof<_>
    
    /// Creates a new logger with a mapping function over the log entries.
    let decorate f (logger : Logger) = Logger(logger.Path, f >> logger.Consumer)
    
    /// Creates a new logger that indents all messages in the logger by 4 spaces.
    let indent : Logger -> Logger = 
        let indentF (l : LogEntry) = LogEntry(l.Level, l.Time, l.Path, sprintf "    %s" l.Message)
        decorate indentF
