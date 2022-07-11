module Bix.Server

open URLPattern
open Fable.Core
open Fable.Core.JsInterop
open Browser.Types
open Fetch

open Bix.Browser.Types
open Bix.Types


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

let inline withFetch (fetch: RequestHandler) (args: ResizeArray<BixServerArgs>) =
    args.Add(Fetch fetch)
    args

let inline withErrorHandler (errHandler: RequestErrorHandler) (args: ResizeArray<BixServerArgs>) =
    args.Add(Error errHandler)
    args


let patternOptions (baseUrl: string, route: RouteDefinition) =
    unbox<URLPatternInput>
        {| pathname = route.pattern
           baseURL = baseUrl
           search = "*"
           hash = "*" |}

let getRouteMatch (ctx: HttpContext, baseUrl: string, notFound: HttpHandler, routes: RouteDefinition list) =

    let routes =
        routes
        |> List.filter (fun route ->
            URLPattern(patternOptions (baseUrl, route))
                .test (unbox ctx.Request.url))

    if routes.Length > 1 then
        routes
        |> List.tryFind (fun r ->
            r.method = All
            || r.method.asString = ctx.Request.method)
        |> Option.map (fun r -> Found r)
        |> Option.orElse (Some MethodNotAllowed)
    else
        routes
        |> List.tryFind (fun r ->
            r.method = All
            || r.method.asString = ctx.Request.method)
        |> Option.map (fun r -> Found r)
        |> Option.orElse (Some NotFound)

    |> function
        | Some (Found route) ->
            let pattern = URLPattern(patternOptions (baseUrl, route))
            pattern.exec (!!ctx.Request.url) |> ctx.SetPattern
            route.handler (fun _ -> Promise.lift None) ctx
        | Some MethodNotAllowed -> Handlers.setStatusCode 405 (fun _ -> Promise.lift None) ctx
        | None
        | Some NotFound -> notFound (fun _ -> Promise.lift None) ctx
