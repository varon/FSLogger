module FSLogger.Tests

open FSLogger
open Xunit
open FsCheck.Xunit
open System


[<Property>]
let ``LogEntry ShortString contains then last value``(s:string) =
    if not <| String.IsNullOrEmpty s then
        let s = s.Replace("/", "").Replace("\\", "")
        let le = LogEntry(LogLevel.Info,DateTime.Now,"Test/" + s, "msg")
        le.ShortString.Contains(s)
    else true

[<Property>]
let ``LogEntry ShortString does not contain first bit of then path value``(s:string) =
    if not <| (String.IsNullOrEmpty s || s.Contains "Test") then
        let s = s.Replace("/", "").Replace("\\", "")
        let le = LogEntry(LogLevel.Info,DateTime.Now,"Test/ABC", "msg")
        not <| le.ShortString.Contains("Test")
    else
        true

[<Fact>]
let ``Appending a path to the default logger``() =
    let logger = Logger.Default |> Logger.appendPath "test"
    Assert.Equal("test", logger.Path)


[<Fact>]
let ``Replacing a path on the logger``() =
    let logger = Logger.Default |> Logger.appendPath "foo" |> Logger.withPath "bar"
    Assert.Equal("bar", logger.Path)


[<Fact>]
let ``Format strings work when appending paths``() =
    let logger = Logger.Default |> Logger.appendPath "%d-%s" 1 "baz"
    Assert.Equal("1-baz", logger.Path)
    
    let logger2 = logger |> Logger.appendPath "[%s]" "foo"
    Assert.Equal("1-baz/[foo]", logger2.Path)
    

[<Fact>]
let ``format strings work when replacing paths``() =
    let logger = Logger.Default |> Logger.appendPath "test"
    Assert.Equal("test", logger.Path)
    let test = logger |> Logger.withPath "#%d#[%s]" 101 "loggers!"
    Assert.Equal("#101#[loggers!]", test.Path)

[<Fact>]
let ``Logger logf format string works``() =
    let mutable msg = ""
    let consumer (e:LogEntry) = msg <- e.Message
    let logger = Logger.Default |> Logger.withConsumer consumer
    logger |> Logger.logf LogLevel.Info "$%d %s" 5 "test"
    Assert.Equal("$5 test", msg)


[<Fact>]
let ``Default logger works for all methods without exn``() =
    let l = Logger.Default
    l.T "trace"
    l.D "debug"
    l.I "info"
    l.N "notice"
    l.W "warn"
    l.E "error"
    l.F "fun"
    l.Consumer (new LogEntry(LogLevel.Debug, DateTime.UtcNow, "path", "ignored consumer"))


