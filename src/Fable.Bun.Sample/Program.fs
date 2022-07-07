// For more information see https://aka.ms/fsharp-console-apps

open Fable.Core.JsInterop
open Fetch

open Feliz.ViewEngine

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

[<EntryPoint>]
let main argv =
    let server =
        Bun.serve (
            {| fetch =
                fun (req: Request) ->
                    let view = Home req |> Render.htmlDocument
                    Response.create (!!view, {| headers = {| ``content-type`` = "text/html" |} |})
               development = true |}
        )

    let mode =
        if server.development then
            "Development"
        else
            "Production"

    printfn $"Mode: {mode}"
    printfn $"Server started at {server.hostname}"
    0
