module Types

open Fable.Core
open Fable.Core.JsInterop
open Bun
open Fetch

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
    | Fetch of req: (Request -> U2<Response, JS.Promise<Response>>)
    | Error of req: (exn -> U2<Response, JS.Promise<Response>>)
