module Bix.Types

open Fable.Core
open Browser.Types
open Fetch
open Fable.Bun

type BixServerArgs =
    | Port of int
    | Hostname of string
    | BaseURI of string
    | MaxRequestBodySize of float
    | Development of bool
    | KeyFile of string
    | CertFile of string
    | Passphrase of string
    | CaFile of string
    | DhParamsFile of string
    | LowMemoryMode of bool
    | ServerNames of (string * BixServerArgs list) list
    | Fetch of req: BunHandler
    | Error of req: BunErrorHandler

type BixResponse =
    | Text of string
    | Html of string
    | Blob of content: Blob * mimeType: string
    | ArrayBuffer of content: JS.ArrayBuffer * mimeType: string
    | ArrayBufferView of content: JS.ArrayBufferView * mimeType: string
    | Json of obj
    | Custom of obj * ResponseInitArgs list

[<AttachMembers>]
type HttpContext(server: BunServer, req: Request, res: Response) =
    let mutable _res = res
    let mutable hasStarted = false
    member _.Request: Request = req
    member _.Server: BunServer = server
    member _.Response: Response = _res
    member _.HasStarted: bool = hasStarted
    member _.SetStarted(setStarted: bool) = hasStarted <- setStarted

    member _.SetResponse(response: Response) = _res <- response

type HttpFuncResult = JS.Promise<BixResponse option>

type HttpFunc = HttpContext -> HttpFuncResult

type HttpHandler = HttpFunc -> HttpFunc

type RouteType =
    | Get
    | Post
    | Put
    | Delete
    | Patch
    | Head
    | Options
    | Custom of string

    static member FromString s =
        match s with
        | "GET" -> Get
        | "POST" -> Post
        | "PUT" -> Put
        | "DELETE" -> Delete
        | "PATCH" -> Patch
        | "HEAD" -> Head
        | "OPTIONS" -> Options
        | custom -> Custom custom

type RouteMap = Map<RouteType * string, HttpHandler>
