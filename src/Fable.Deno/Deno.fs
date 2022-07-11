module Fable.Deno

open System.Collections.Generic
open Fable.Core
open Fable.Core.JsInterop

open Fetch

open Bix.Browser.Types

type ListenOptions =
    abstract hostname: string option
    abstract port: int

type NetAddr =
    abstract hostname: string
    abstract port: int
    abstract transport: string

type UnixAddr =
    abstract path: string
    abstract transport: string

type DenoAddr = U2<NetAddr, UnixAddr>

type DenoListener =
    abstract addr: DenoAddr
    abstract rid: int
    abstract accept: unit -> JS.Promise<unit>
    abstract close: unit -> unit


type ServerInit =
    inherit ListenOptions

    abstract handler: RequestHandler
    abstract onError: (RequestErrorHandler) option

type OnListenParams =
    abstract hostname: string
    abstract port: int

type ServeInit =
    abstract onError: RequestErrorHandler option
    abstract onListen: (OnListenParams -> unit) option
    abstract signal: AbortSignal option

type ConnInfo =
    abstract localAddr: DenoAddr
    abstract remoteAddr: DenoAddr

[<ImportMember "http">]
type Server(options: ServerInit) =

    member _.closed: bool = jsNative
    member _.addrs: DenoAddr[] = jsNative

    member _.close() : unit = jsNative

    member _.listenAndServe() : JS.Promise<unit> = jsNative
    member _.listenAndServeTls(certFile: string, keyFile: string) : JS.Promise<unit> = jsNative

    member _.serve(listener: DenoListener) : JS.Promise<unit> = jsNative

let serve (handler: RequestHandler, options: ServeInit) : JS.Promise<unit> = importMember "http"

[<Global; Erase>]
type Deno =
    class
    end
