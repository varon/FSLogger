module FSLogger.Tests

open FSLogger
open Xunit
open FsCheck.Xunit
open System



[<Property>]
let ``LogEntry ShortString contains then last value``(s:string) =
    if not <| String.IsNullOrEmpty s then
        let s = s.Replace("/", "").Replace("\\", "")
        let le = new LogEntry(LogLevel.Info,DateTime.Now,"Test/" + s, "msg")
        le.ShortString.Contains(s)
    else true

[<Property>]
let ``LogEntry ShortString does not contain first bit of then path value``(s:string) =
    if not <| (String.IsNullOrEmpty s || s.Contains "Test") then
        let s = s.Replace("/", "").Replace("\\", "")
        let le = new LogEntry(LogLevel.Info,DateTime.Now,"Test/ABC", "msg")
        not <| le.ShortString.Contains("Test")
    else
        true
    
