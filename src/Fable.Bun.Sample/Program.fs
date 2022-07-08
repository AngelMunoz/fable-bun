// For more information see https://aka.ms/fsharp-console-apps

open Feliz.ViewEngine
open Fable.Bun

open Fetch
open Types
open Bix
open Bix.Types

type Html with
    static member inline sl_button xs = Interop.createElement "sl-button" xs

type Views =
    static member inline Layout(content: ReactElement, ?head: ReactElement seq, ?scripts: ReactElement seq) =
        let head = defaultArg head []
        let scripts = defaultArg scripts []

        Html.html
            [ Html.head
                  [ prop.children
                        [ Html.link
                              [ prop.rel "stylesheet"
                                prop.media "(prefers-color-scheme:light)"
                                prop.href
                                    "https://cdn.jsdelivr.net/npm/@shoelace-style/shoelace@2.0.0-beta.77/dist/themes/light.css" ]
                          Html.link
                              [ prop.rel "stylesheet"
                                prop.media "(prefers-color-scheme:dark)"
                                prop.href
                                    "https://cdn.jsdelivr.net/npm/@shoelace-style/shoelace@2.0.0-beta.77/dist/themes/dark.css"
                                prop.custom ("onload", "document.documentElement.classList.add('sl-theme-dark');") ]
                          Html.script
                              [ prop.type' "module"
                                prop.src
                                    "https://cdn.jsdelivr.net/npm/@shoelace-style/shoelace@2.0.0-beta.77/dist/shoelace.js" ]
                          yield! head ] ]
              Html.body [ prop.children [ content; yield! scripts ] ] ]

let Home (req: Request) =
    let content =
        Html.article
            [ Html.nav []
              Html.main
                  [ Html.h1 $"Hello from {req.method} - {req.url}"
                    Html.sl_button
                        [ prop.custom ("variant", "primary")
                          prop.text "This is a Shoelace Button and rendered in Bun.sh" ] ]
              Html.footer [] ]

    let styles =
        Html.rawText
            """
            <style>
            html, body {
                color-scheme: light dark;
                font-family:
                    -apple-system,
                    BlinkMacSystemFont,
                    "Segoe UI",
                    Roboto,
                    Oxygen,
                    Ubuntu,
                    Cantarell,
                    "Open Sans",
                    "Helvetica Neue",
                    sans-serif;
                margin: 0;
                padding: 0;
            }
            :not(:defined) {
                opacity: 0;
            }
            :defined {
                opacity: 1;
            }
            </style>
        """

    Views.Layout(content, [ styles ])

let checkCredentials: HttpHandler =
    fun next ctx ->

        promise {
            printfn "Executing Auth Before"
            do! (next ctx |> Promise.map ignore)
            printfn "Executing Auth After"
            let req: Request = ctx.Request

            let body: {| email: string; password: string |} option =
                {| email = "email@email"
                   password = "password" |}
                |> Some

            match body with
            | Some body ->
                if body.email = "email@email"
                   && body.password = "password" then
                    return None
                else
                    return
                        (box None,
                         [ ResponseInitArgs.Status 401
                           ResponseInitArgs.StatusText "Failed Authentication" ])
                        |> BixResponse.Custom
                        |> Some
            | None ->
                return
                    (box None,
                     [ ResponseInitArgs.Status 401
                       ResponseInitArgs.StatusText "Failed Authentication" ])
                    |> BixResponse.Custom
                    |> Some
        }

let login: HttpHandler =
    fun next ctx ->
        promise {
            printfn "Executing before"
            let! response = next ctx
            printfn "Executing after"

            match response with
            | None ->

                return
                    box {| Authed = "Apparenly!" |}
                    |> BixResponse.Json
                    |> Some
            | response -> return response
        }

let homeHandler: HttpHandler =
    fun next ctx ->
        promise {
            do! next ctx |> Promise.map ignore
            let view = Home ctx.Request |> Render.htmlDocument
            return view |> BixResponse.Html |> Some
        }

let jsonHandler =
    fun next ctx ->
        promise {
            return
                box {| Hello = "World!" |}
                |> BixResponse.Json
                |> Some
        }

let textHandler: HttpHandler =
    fun next ctx -> promise { return "Hello, World!" |> BixResponse.Text |> Some }

[<EntryPoint>]
let main argv =
    let server =
        Server.Bix
        |> Server.withDevelopment true
        |> Server.withBixHandler (
            Router.Empty
            |> Router.get ("/", homeHandler)
            |> Router.get ("/json", jsonHandler)
            // TEST: THIS THING LOOKS BROKEN
            |> Router.get ("/login", (checkCredentials >=> login))
            |> Router.get ("/text", textHandler)
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
