[bun.sh]: https://bun.sh
[giraffe]: https://giraffe.wiki
[saturn]: https://github.com/SaturnFramework/Saturn

# [Bun.sh] + Fable Playground

This is a small repo to add a few bindings for the node.js new competitor runtime [bun.sh]

Although this is not something serious yet, if you actually want to work with me to make this a true bindings project let me know!
I'd be glad to work towards something like that

## Fable.Bun

Fable.bun which can be found in `src/Fable.Bun` are the bindings to Fable.Bun nothing more, nothing less, these are very minimal and only add a few convenience methods for Bun's request/response impl consumption

# Bix

**Bix** which can be found in `src/Bix` is an F# microframework built on top of `Bun.serve` although the name is just a _codename_ for now (until I decide it's good to go), this microframework is heavily inspired by [Giraffe], and [Saturn] frameworks from F# land so if you have ever used that server model then Bix will feel fairly similar, I plan to add a saturn like router eventually but for the moment here's the sample code you might be looking for

```fsharp
// For more information see https://aka.ms/fsharp-console-apps
open Bix
open Bix.Types
open Bix.Handlers
open Bix.Router


let checkCredentials: HttpHandler =
    fun next ctx ->
        let req: Request = ctx.Request
        let bearer = req.headers.get "Authorization" |> Option.ofObj
        // dummy handler
        match bearer with
        | None -> (setStatusCode (401) >=> sendText "Not Authorized") next ctx
        | Some token -> next ctx

let routes =
    Router.Empty
    |> Router.get ("/", fun next ctx -> sendText "Hello, World!" next ctx)
    |> Router.get ("/posts/:slug", fun next ctx ->
        promise { // promise based handlers are supported
            let slug = ctx.PathParams "slug"
            let! post = Database.find slug // database from somewhere
            let! html = Views.renderPost post // views from somewhere
            return! sendHtml html next ctx
        }
    )
    |> Router.get ("/json", fun next ctx ->
        let content = {| name = "Bix Server!"; Date = System.DateTime.Now |}
        sendJson content next ctx
    )
    |> Router.get ("/protected", (checkCredentials >=> (fun next ctx -> sendText "I'm protected!" next ctx)))

let server =
    Server.Empty
    |> Server.withRouter routes
    |> Server.withDevelopment true
    |> Server.withPort 5000
    |> Server.run

let mode =
    if server.development then
        "Development"
    else
        "Production"

printfn $"{mode} Server started at {server.hostname}"
```

Interested? check out the samples at `src/Fable.Bun.Sample/Program.fs`, and `src/Fable.Bun.Sample/Handlers.fs`

## Requirements

- .NET6 and above - https://get.dot.net
- Bun - [bun.sh]

## Try the sample

After installing .NET + Bun just run

`bun start` on your terminal and it should just work
