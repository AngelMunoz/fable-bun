module Handlers

open Bix
open Bix.Types
open Fable.Bun
open Feliz.ViewEngine
open Fetch

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
        let req: Request = ctx.Request
        let bearer = req.headers.get "Authorization" |> Option.ofObj
        Fable.Core.JS.console.log (bearer)

        match bearer with
        | None ->
            (Server.setStatusCode (401)
             >=> Server.sendText "Not Authorized")
                next
                ctx
        | Some token ->
            match token.Split(" ") with
            | [| "Bearer"; token |] ->
                if token = "yeah.come.in" then
                    next ctx
                else
                    (Server.setStatusCode (401)
                     >=> Server.sendText "Invalid Credentials")
                        next
                        ctx
            | _ ->
                (Server.setStatusCode (400)
                 >=> Server.sendText "Wrong Bearer Format")
                    next
                    ctx


let login: HttpHandler = Server.sendJson {| Authed = "Apparenly!" |}

let home: HttpHandler =
    fun next ctx -> Server.sendHtml (Home ctx.Request |> Render.htmlDocument) next ctx

let json: HttpHandler = Server.sendJson {| Hello = "World!" |}

let text: HttpHandler = Server.sendText "Hello, World!"

let jsonPostHandler: HttpHandler =
    fun next ctx ->
        let req: Request = ctx.Request

        req.json ()
        |> Promise.bind (fun res ->
            let content =
                res
                |> Option.ofObj
                |> Fable.Core.JS.JSON.stringify

            Server.sendJson content next ctx)
