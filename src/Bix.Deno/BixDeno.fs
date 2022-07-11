namespace Bix.Deno


open URLPattern

open Fable.Core
open Fable.Core.JsInterop

open Browser.Types
open Fetch

open Fable.Deno
open Bix
open Bix.Types
open Bix.Browser.Types

type BixDenoServer(server: Fable.Deno.Server) =

    interface IHostServer with
        override _.hostname =
            server.addrs
            |> Array.tryHead
            |> Option.map (fun f ->
                let address: NetAddr = unbox f
                address.hostname)

        override _.port =
            server.addrs
            |> Array.tryHead
            |> Option.map (fun f ->
                let address: NetAddr = unbox f
                address.port)
            |> Option.defaultValue 0

        override _.development = true
        override _.env = Map.empty

module Server =

    let inline run (args: seq<BixServerArgs>) =
        let initOptions =
            args
            |> Seq.choose (fun arg ->
                match arg with
                | Port port -> Some(Port port)
                | Hostname hostname -> Some(Hostname hostname)
                | Error err -> Some(Error err)
                | Fetch handler -> Some(Fetch handler)
                | _ -> None)
            |> keyValueList CaseRules.LowerFirst
            :?> {| port: int option
                   hostname: string option
                   error: RequestErrorHandler
                   fetch: RequestHandler |}


        Fable.Deno.serve (initOptions.fetch, unbox initOptions)


    let private NoValue (contentType: string, options: ResponseInitArgs list) =
        Response.create (
            "",
            Headers [| "content-type", contentType |]
            :: options
        )

    let private OnText (text: string, options: ResponseInitArgs list) =
        Response.create (
            text,
            Headers [| "content-type", "text/plain" |]
            :: options
        )

    let private OnHtml (html: string, options: ResponseInitArgs list) =
        Response.create (
            html,
            Headers [| "content-type", "text/html" |]
            :: options
        )

    let private OnJson (json: obj, options: ResponseInitArgs list) =
        let content = JS.JSON.stringify (json)

        Response.create (
            content,
            Headers [| "content-type", "application/json" |]
            :: options
        )

    let private OnJsonOptions (value: obj, encoder: obj -> string, options: ResponseInitArgs list) =

        Response.create (
            encoder value,
            Headers [| "content-type", "application/json" |]
            :: options
        )

    let private OnBlob (blob: Blob, mimeType: string, options: ResponseInitArgs list) =
        Response.create (blob, Headers [| "content-type", mimeType |] :: options)

    let private OnArrayBuffer (arrayBuffer: JS.ArrayBuffer, mimeType: string, options: ResponseInitArgs list) =
        Response.create (arrayBuffer, Headers [| "content-type", mimeType |] :: options)

    let private OnArrayBufferView
        (
            arrayBufferView: JS.ArrayBufferView,
            mimeType: string,
            options: ResponseInitArgs list
        ) =
        Response.create (arrayBufferView, Headers [| "content-type", mimeType |] :: options)

    let private OnCustom (content, contentType, args) =
        Response.create (
            // it might not be a string but
            // it is just to satisfy the F# compiler
            unbox<string> content,
            Headers [| "content-type", contentType |] :: args
        )

    let BixHandler
        (server: Server)
        (req: Request)
        (connInfo: ConnInfo)
        (routes: RouteDefinition list)
        (notFound: HttpHandler option)
        : JS.Promise<Response> =
        let notFound = defaultArg notFound Handlers.notFoundHandler
        let server: IHostServer = BixDenoServer(server)
        let ctx = HttpContext(server, req, createResponseInit ("", {|  |}))
        let reqUrl = createUrl ctx.Request.url

        Server.getRouteMatch (ctx, reqUrl.origin, notFound, routes)
        |> Promise.bind (fun res ->
            let status = ctx.Response.Status

            let contentType =
                ctx.Response.Headers.ContentType
                |> Option.defaultValue "text/plain"

            let options = [ Status status ]

            match res with
            | None -> NoValue(contentType, options)
            | Some result ->
                match result with
                | Text value -> OnText(value, options)
                | Html value -> OnHtml(value, options)
                | Json value -> OnJson(value, options)
                | JsonOptions (value, encoder) -> OnJsonOptions(value, encoder, options)
                | Blob (content, mimeType) -> OnBlob(content, mimeType, options)
                | ArrayBufferView (content, mimeType) -> OnArrayBufferView(content, mimeType, options)
                | ArrayBuffer (content, mimeType) -> OnArrayBuffer(content, mimeType, options)
                | BixResponse.Custom (content, args) -> OnCustom(content, contentType, Status status :: options @ args))

    let withRouter (routes: RouteDefinition list) (args: ResizeArray<BixServerArgs>) =
        // HACK: we need to ensure that fable doesn't wrap the request handler
        // in an anonymnous function or we will lose "this" which equates to the
        // bun's server instance
        emitJsStatement
            (routes)
            "function handler(req, connInfo) { return Server_BixHandler(this, req, connInfo, $0); }"

        args.Add(Fetch(emitJsExpr () "handler"))
        args

    let withRouterAndNotFound
        (routes: RouteDefinition list)
        (notFound: HttpHandler)
        (args: ResizeArray<BixServerArgs>)
        =
        // HACK: we need to ensure that fable doesn't wrap the request handler
        // in an anonymnous function or we will lose "this" which equates to the
        // bun's server instance
        emitJsStatement
            (routes, notFound)
            "function handler(req, connInfo) { return Server_BixHandler(this, req, connInfo, $0, $1); }"

        args.Add(Fetch(emitJsExpr () "handler"))
        args
