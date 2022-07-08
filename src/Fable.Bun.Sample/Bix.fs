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

let Bix = ResizeArray<BixServerArgs>()

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

let BixHandler (routes: RouteMap) (req: Request) : JS.Promise<Response> =
    let ctx = HttpContext(JsInterop.jsThis, req)

    promise {
        let url = createUrl req.url
        let verb = RouteType.FromString req.method
        let handler = routes |> Map.tryFind (verb, url.pathname)

        let! res =
            promise {
                match handler with
                | Some handler -> return! handler (fun _ -> promise { return None }) ctx
                | None -> return! Promise.lift None
            }

        match res with
        | None -> return Response.create ("", [])
        | Some result ->
            match result with
            | Text value ->
                return Response.create (value, [ ResponseInitArgs.Headers [| "content-type", "text/plain" |] ])
            | Html value ->
                return Response.create (value, [ ResponseInitArgs.Headers [| "content-type", "text/html" |] ])
            | Json value ->
                let content = JS.JSON.stringify (value)
                return Response.create (content, [ ResponseInitArgs.Headers [| "content-type", "application/json" |] ])
            | Blob (content, mimeType) ->
                return Response.create (content, [ ResponseInitArgs.Headers [| "content-type", mimeType |] ])
            | ArrayBufferView (content, mimeType) ->
                return Response.create (content, [ ResponseInitArgs.Headers [| "content-type", mimeType |] ])
            | ArrayBuffer (content, mimeType) ->
                return Response.create (content, [ ResponseInitArgs.Headers [| "content-type", mimeType |] ])
            | BixResponse.Custom (content, args) ->
                // it might not be a string but we don't really care
                return Response.create (unbox<string> content, args)
    }



let inline withBixHandler (routes: RouteMap) (args: ResizeArray<BixServerArgs>) =
    args.Add(Fetch(unbox (BixHandler routes)))
    args