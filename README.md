[bun.sh]: https://bun.sh
[deno]: https://deno.land
[giraffe]: https://giraffe.wiki
[saturn]: https://github.com/SaturnFramework/Saturn
[Fable]: https://fable.io

# [Bun.sh] + Fable Playground

This is a small repository that adds [Fable] bindings for [Bun.sh] and [Deno]

Also as an experimental thing It also exposes an F# framework called Bix (name subject to change) which works on both Bun and Deno

Although this is not something serious yet, if you are interested in make this thing a reality, feel free to ping me and we can work together to make it happen.


```fsharp
// For more information see https://aka.ms/fsharp-console-apps
open Bix
open Bix.Types
open Bix.Handlers
open Bix.Router

open Bix.Bun

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

## Fable.Bun

Fable.Bun which can be found in `src/Fable.Bun` are the bindings to Fable.Bun nothing more, nothing less, these are very minimal and only add a few convenience methods for Bun's request/response impl consumption

> **Note**: Bun currently does not implement `URLPattern` hence we need to use a polyfill this can be installed via `bun add urlpattern-polyfill`

## Fable.Deno

Fable.Deno which can be found in `src/Fable.Deno` are the bindings to Fable.Deno nothing more, nothing less, these are very minimal and lack a lot of information regarding Deno's types

## Bix

**Bix** which can be found in `src/Bix` is an F# microframework built on top of `Bun.serve` although the name is just a _codename_ for now (until I decide it's good to go), this microframework is heavily inspired by [Giraffe], and [Saturn] frameworks from F# land so if you have ever used that server model then Bix will feel fairly similar, I plan to add a saturn like router eventually

## Bix.Bun

Bix.Bun exposes a [bun.sh] specific request handler, and other http handlers that may contain Bun specific APIs like `Bun.file` to read files 

Check the sample at `src/Bix.Bun.Sample/Program.fs`

## Bix.Deno

Bix.Deno exposes a [deno] specific request handler, and other http handlers that may contain Bun specific APIs like `Deno.open` to read files 

Check the sample at `src/Bix.Deno.Sample/Program.fs`


```fsharp
// For more information see https://aka.ms/fsharp-console-apps
open Bix
open Bix.Types
open Bix.Handlers
open Bix.Router

open Bix.Deno

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


Server.Empty
|> Server.withRouter routes
|> Server.withDevelopment true
|> Server.withPort 5000
|> Server.run
|> Promise.start
```

> **Note**: When Working with deno, you need to have the following fields in your import map
> ```json
>  {
>    "imports": {
>      "urlpattern-polyfill": "https://cdn.skypack.dev/pin/urlpattern-polyfill@v5.0.3-5dMKTgPBkStj8a3hiMD2/mode=imports,min/optimized/urlpattern-polyfill.js",
>      "http": "https://deno.land/std@0.147.0/http/server.ts"
>    }
>  }
> ```

This is to ensure bare imports within `Bix.Browser.Types` and `Bix.Deno` work just fine.


# Development

This project is developed with VSCode in Linux/Windows/WSL but either rider, and visual studio should work just fine.


## Requirements

- .NET6 and above - https://get.dot.net
- Bun - [bun.sh] - (in case of running bun)
- Deno - [bun.sh] - (in case of running deno)

## Try the sample

if you have bun installed along deno the simplest way to run the samples is

- `bun start`
- `bun start:deno`

both commands will restore the projects and run fable, bun/deno in watch mode


### Bun

After installing .NET + Bun just run

`bun start` on your terminal and it should just work

### Deno

After installing .NET + Bun just run
```sh
dotnet tool restore && \
dotnet fable watch src/Bix.Deno.Sample -s -o dist/Bix.Deno.Sample --run deno run -A ./dist/Bix.Deno.Sample/Program.js`
```

