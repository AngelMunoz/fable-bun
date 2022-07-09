[<RequireQualifiedAccess>]
module Bix.Server

open Fable.Core
open Fable.Core.JsInterop
open Browser.Types
open Fetch
open Bix.Types
open Fable.Bun


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

let sendJson<'T> (value: 'T) : HttpHandler =
    fun next ctx ->
        ctx.SetStarted true
        Promise.lift (Some(BixResponse.Json value))

let sendText (value: string) : HttpHandler =
    fun next ctx ->
        ctx.SetStarted true
        Promise.lift (Some(Text value))

let sendHtml (value: string) : HttpHandler =
    fun next ctx ->
        ctx.SetStarted true
        Promise.lift (Some(Html value))

let sendHtmlFile (path: string) : HttpHandler =
    fun next ctx ->
        ctx.SetStarted true
        let content = Bun.file (path, unbox {| ``type`` = "text/html" |}), "text/html"
        let content = Blob content |> Some
        Promise.lift content

let cleanResponse: HttpHandler =
    fun next ctx ->
        ctx.SetStarted true
        ctx.SetResponse(Response.create ("", []))
        Promise.lift None

let setContentType (contentType: string) : HttpHandler =
    fun next ctx ->
        let headers = ctx.Response.Headers
        headers.append ("content-type", contentType)

        let res =
            createResponseInit
                ""
                {| headers = headers
                   status = ctx.Response.Status
                   statusText = ctx.Response.StatusText |}

        ctx.SetResponse(res)
        next ctx

let setStatusCode (code: int) : HttpHandler =
    fun next ctx ->
        let headers = ctx.Response.Headers

        let res =
            createResponseInit
                ""
                {| headers = headers
                   status = code
                   statusText = ctx.Response.StatusText |}

        ctx.SetResponse(res)
        next ctx

let notFoundHandler: HttpHandler = setStatusCode 404 >=> sendText "Not Found"

let BixHandler (server: BunServer) (routes: RouteMap) (req: Request) : JS.Promise<Response> =
    let ctx = HttpContext(server, req, createResponseInit "" {|  |})
    let url = createUrl req.url
    let verb = RouteType.FromString req.method

    let handler = routes |> Map.tryFind (verb, url.pathname)

    let status = ctx.Response.Status

    let contentType =
        ctx.Response.Headers.ContentType
        |> Option.defaultValue "text/plain"

    let options = [ Status status ]

    match handler with
    | Some handler -> handler (fun _ -> Promise.lift None) ctx
    | None -> notFoundHandler (fun _ -> Promise.lift None) ctx
    |> Promise.map (fun res ->
        match res with
        | None ->
            Response.create (
                "",
                Headers [| "content-type", contentType |]
                :: options
            )
        | Some result ->
            match result with
            | Text value ->
                Response.create (
                    value,
                    Headers [| "content-type", "text/plain" |]
                    :: options
                )
            | Html value ->
                Response.create (
                    value,
                    Headers [| "content-type", "text/html" |]
                    :: options
                )
            | Json value ->
                let content = JS.JSON.stringify (value)

                Response.create (
                    content,
                    Headers [| "content-type", "application/json" |]
                    :: options
                )
            | Blob (content, mimeType) -> Response.create (content, Headers [| "content-type", mimeType |] :: options)
            | ArrayBufferView (content, mimeType) ->
                Response.create (content, Headers [| "content-type", mimeType |] :: options)
            | ArrayBuffer (content, mimeType) ->
                Response.create (content, Headers [| "content-type", mimeType |] :: options)
            | BixResponse.Custom (content, args) ->
                Response.create (
                    // it might not be a string but
                    // it is just to satisfy the F# compiler
                    unbox<string> content,
                    [ Status status
                      Headers [| "content-type", contentType |]
                      yield! args ]
                ))

let withBixRouter (routes: RouteMap) (args: ResizeArray<BixServerArgs>) =
    // HACK: we need to ensure that fable doesn't wrap the request handler
    // in an anonymnous function or we will lose "this" which equates to the
    // bun's server instance
    emitJsStatement (routes) "function handler(req) { return BixHandler(this, $0, req); }"
    args.Add(Fetch(emitJsExpr () "handler"))
    args
