# F#ing Simple Logger

An rich, simple, and efficient purely functional micro logging library for F#.

This isn't frequently updated because it's literally perfect (well, probably, almost).


## HOW TO LOG STUFF:

```fsharp
open FSLogger
let log = Logger.ColorConsole
// log stuff to the information channel.
log.I "Uzing da librariez"
// log a warning (using the format string features)
log.W "%d out of %d developers recommend FsLogger" 5 5
```

## Is that it?

Okay fine. The library can do a bunch of stuff.


## More examples

### Setting a path to log stuff to:

```fsharp
open FSLogger

let log = 
    Logger.ColorConsole
    |> Logger.withPath "MyApp"

module SeriousBusiness = 
    let doStuff (log: Logger) (aNumber: int) =
        // shadow with a better path
        let log =  log |> Logger.appendPath "doStuff"
        if aNumber > 5 then
            log.W "Oh no, %d is bigger than 5!" aNumber
        // do something important
ignore aNumber
```


### Adding a custom consumer:

```fsharp
let enterpriseConsumer (l:LogEntry) = 
	match l.Level with
	| LogLevel.Warn -> callManagement(l.Message)
	| LogLevel.Error -> blameAnotherDeveloper(l.Message)
	| LogLevel.Fatal -> submitNewJobApplication()
	| _ -> () // ignore the rest

let log = 
	Logger.Console
	|> Logger.addConsumer enterpriseConsumer
```


## I want more examples!

Read the source code! It's super simple and documented.
