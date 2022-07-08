// For more information see https://aka.ms/fsharp-console-apps
open Bix

[<EntryPoint>]
let main argv =
    let server =
        Server.Empty
        |> Server.withDevelopment true
        |> Server.withBixRouter (
            Router.Empty
            |> Router.get ("/", Handlers.home)
            |> Router.get ("/json", Handlers.json)
            |> Router.get ("/text", Handlers.text)
            |> Router.get ("/login", Handlers.login)
            // TEST: THIS THING LOOKS BROKEN
            |> Router.get ("/protected", (Handlers.checkCredentials >=> Handlers.home))
        )
        |> Server.run

    let mode =
        if server.development then
            "Development"
        else
            "Production"

    printfn $"Mode: {mode}"
    printfn $"Server started at {server.hostname}"
    0
