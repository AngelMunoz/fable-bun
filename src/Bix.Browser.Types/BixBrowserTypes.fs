module Bix.Browser.Types

open Fable.Core
open Fable.Core.JsInterop
open Browser.Types
open Fetch

importSideEffects "urlpattern-polyfill"

type RequestHandler = Request -> U2<Response, JS.Promise<Response>>
type RequestErrorHandler = exn -> U2<Response, JS.Promise<Response>>


type BlobPart = U4<string, Blob, JS.ArrayBuffer, JS.ArrayBufferView>
type StringOrBuffer = U3<string, JS.TypedArray, JS.ArrayBuffer>

type ResponseInitArgs =
    | Status of int
    | StatusText of string
    | Headers of (string * string)[]

[<Emit("new Response($0, $1)")>]
let inline createResponseInit (content: obj, options: obj) = jsNative

type Request with

    [<Emit "$0.clone()">]
    member _.clone() : Request = jsNative

type Response with
    static member inline create(content: string, ?options: RequestInit) = createResponseInit (content, options)
    static member inline create(content: Blob, ?options: RequestInit) = createResponseInit (content, options)
    static member inline create(content: JS.ArrayBuffer, ?options: RequestInit) = createResponseInit (content, options)

    static member inline create(content: JS.ArrayBufferView, ?options: RequestInit) =
        createResponseInit (content, options)

    static member inline create(content: string, ?options: seq<ResponseInitArgs>) =
        let options = defaultArg options Seq.empty

        let opts = options |> keyValueList CaseRules.LowerFirst

        createResponseInit (content, opts)

    static member inline create(content: Blob, ?options: seq<ResponseInitArgs>) =
        let options = defaultArg options Seq.empty

        let opts = options |> keyValueList CaseRules.LowerFirst

        createResponseInit (content, opts)

    static member inline create(content: JS.ArrayBuffer, ?options: seq<ResponseInitArgs>) =
        let options = defaultArg options Seq.empty

        let opts = options |> keyValueList CaseRules.LowerFirst

        createResponseInit (content, opts)

    static member inline create(content: JS.ArrayBufferView, ?options: seq<ResponseInitArgs>) =
        let options = defaultArg options Seq.empty

        let opts = options |> keyValueList CaseRules.LowerFirst

        createResponseInit (content, opts)

    [<Emit("$0.clone()")>]
    member _.clone() : Response = jsNative

    [<Emit("$0.json()")>]
    member _.json<'T>() : JS.Promise<'T> = jsNative

    [<Emit("Response.json($0, $1)")>]
    static member inline json(?body: obj, ?options: ResponseInit) : Response = jsNative

    [<Emit("Response.json($0, $1)")>]
    static member inline json(?body: obj, ?options: int) : Response = jsNative

    [<Emit("Response.redirect($0, $1)")>]
    static member inline redirect(?url: string, ?status: int) : Response = jsNative

    [<Emit("Response.error($0, $1)")>]
    static member inline error() : Response = jsNative

[<Emit "new URL($0)">]
let inline createUrl (url: string) : URL = jsNative

[<Emit("{...$0, ...$1}")>]
let inline mergeObjects (o1, o2) = jsNative
