[<RequireQualifiedAccess>]
module Bix.Server

open Fable.Core
open Fable.Core.JsInterop
open Browser.Types
open Fetch
open Bix.Types
open Fable.Bun
open URLPattern


[<Emit "new URL($0)">]
let inline private createUrl (url: string) : URL = jsNative

let Empty = ResizeArray<BixServerArgs>()

let inline BixArgs (args: seq<BixServerArgs>) = ResizeArray<BixServerArgs>(args)

let inline withPort (port: int) (args: ResizeArray<BixServerArgs>) =
    args.Add(Port port)
    args

let inline withHostname (hostname: string) (args: ResizeArray<BixServerArgs>) =
    args.Add(Hostname hostname)
    args

let inline withBaseURI (baseURI: string) (args: ResizeArray<BixServerArgs>) =
    args.Add(BaseURI baseURI)
    args

let inline withMaxRequestBodySize (maxRequestBodySize: float) (args: ResizeArray<BixServerArgs>) =
    args.Add(MaxRequestBodySize maxRequestBodySize)
    args

let inline withDevelopment (development: bool) (args: ResizeArray<BixServerArgs>) =
    args.Add(Development development)
    args

let inline withKeyFile (keyFile: string) (args: ResizeArray<BixServerArgs>) =
    args.Add(KeyFile keyFile)
    args

let inline withCertFile (certFile: string) (args: ResizeArray<BixServerArgs>) =
    args.Add(CertFile certFile)
    args

let inline withPassphrase (passphrase: string) (args: ResizeArray<BixServerArgs>) =
    args.Add(Passphrase passphrase)
    args

let inline withCaFile (caFile: string) (args: ResizeArray<BixServerArgs>) =
    args.Add(CaFile caFile)
    args

let inline withDhParamsFile (dhParamsFile: string) (args: ResizeArray<BixServerArgs>) =
    args.Add(DhParamsFile dhParamsFile)
    args

let inline withLowMemoryMode (lowMemoryMode: bool) (args: ResizeArray<BixServerArgs>) =
    args.Add(LowMemoryMode lowMemoryMode)
    args

let inline withServerNames (serverNames: (string * BixServerArgs list) list) (args: ResizeArray<BixServerArgs>) =
    args.Add(ServerNames serverNames)
    args

let inline withFetch (fetch: BunHandler) (args: ResizeArray<BixServerArgs>) =
    args.Add(Fetch fetch)
    args

let inline withErrorHandler (errHandler: BunErrorHandler) (args: ResizeArray<BixServerArgs>) =
    args.Add(Error errHandler)
    args

let inline run (args: seq<BixServerArgs>) =

    let serverNamesObj =
        args
        |> Seq.tryPick (fun f ->
            match f with
            | ServerNames names -> Some names
            | _ -> None)
        |> Option.map (fun args ->
            [ for (name, args) in args do
                  let args = keyValueList CaseRules.LowerFirst args

                  name, args ]
            |> createObj)

    let restArgs =
        args
        |> Seq.choose (fun f ->
            match f with
            | ServerNames _ -> None
            | others -> Some others)
        |> keyValueList CaseRules.LowerFirst

    match serverNamesObj with
    | Some names ->
        let options = mergeObjects (restArgs, names)
        Bun.serve (unbox options)
    | None -> Bun.serve (unbox restArgs)

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

let private OnArrayBufferView (arrayBufferView: JS.ArrayBufferView, mimeType: string, options: ResponseInitArgs list) =
    Response.create (arrayBufferView, Headers [| "content-type", mimeType |] :: options)

let private OnCustom (content, contentType, args) =
    Response.create (
        // it might not be a string but
        // it is just to satisfy the F# compiler
        unbox<string> content,
        Headers [| "content-type", contentType |] :: args
    )

let BixHandler
    (server: BunServer)
    (req: Request)
    (routes: RouteDefinition list)
    (notFound: HttpHandler option)
    : JS.Promise<Response> =
    let method = RouteType.FromString req.method
    let notFound = defaultArg notFound Handlers.notFoundHandler

    let patternOptions route =
        !!{| pathname = route.pattern
             baseURL = server.hostname
             search = "*"
             hash = "*" |}

    let route =
        routes
        |> List.tryFind (fun route ->
            URLPattern(patternOptions route)
                .test (unbox req.url))

    let ctx = HttpContext(server, req, createResponseInit "" {|  |})

    match route with
    | Some route ->
        let pattern = URLPattern(patternOptions route)

        if route.method = All then
            pattern.exec (!!req.url) |> ctx.SetPattern
            route.handler (fun _ -> Promise.lift None) ctx
        elif (route.method <> All) && route.method <> method then
            Response.create (
                "",
                [ Status 405
                  StatusText "Method Not Allowed" ]
            )
        else
            pattern.exec (!!req.url) |> ctx.SetPattern
            route.handler (fun _ -> Promise.lift None) ctx

    | None -> notFound (fun _ -> Promise.lift None) ctx
    |> Promise.map (fun res ->
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
    emitJsStatement (routes) "function handler(req) { return BixHandler(this, req, $0); }"
    args.Add(Fetch(emitJsExpr () "handler"))
    args

let withRouterAndNotFound (routes: RouteDefinition list) (notFound: HttpHandler) (args: ResizeArray<BixServerArgs>) =
    // HACK: we need to ensure that fable doesn't wrap the request handler
    // in an anonymnous function or we will lose "this" which equates to the
    // bun's server instance
    emitJsStatement (routes, notFound) "function handler(req) { return BixHandler(this, req, $0, $1); }"
    args.Add(Fetch(emitJsExpr () "handler"))
    args
