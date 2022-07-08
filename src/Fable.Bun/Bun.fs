module Fable.Bun

open Fable.Core
open Fable.Core.JsInterop
open Browser.Types
open Fetch
open System.Collections.Generic

type BunHandler = Request -> U2<Response, JS.Promise<Response>>
type BunErrorHandler = exn -> U2<Response, JS.Promise<Response>>

type BlobPart = U4<string, Blob, JS.ArrayBuffer, JS.ArrayBufferView>
type StringOrBuffer = U3<string, JS.TypedArray, JS.ArrayBuffer>

type ResponseInitArgs =
    | Status of int
    | StatusText of string
    | Headers of (string * string)[]

[<Emit("new Response($0, $1)")>]
let inline private createResponseInit (content: obj, options: obj) = jsNative

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

type MMapOptions =
    abstract sync: bool option
    abstract shared: bool option

[<Erase; StringEnum>]
type Editors =
    | [<CompiledName "vscode">] VSCode
    | [<CompiledName "subl">] Submile

[<Erase; StringEnum>]
type DigestEncoding =
    | [<CompiledName "hex">] Hex
    | [<CompiledName "base64">] Base64

type EditorOptions =
    abstract member editor: Editors option
    abstract line: float option
    abstract column: float option

type ServeOptions =
    abstract port: int option
    abstract hostname: string option
    abstract baseURI: string option
    abstract maxRequestBodySize: float option
    abstract development: bool option
    abstract fetch: BunHandler
    abstract error: BunErrorHandler option

type SSLOptions =
    abstract keyFile: string
    abstract certFile: string

type SSLAdvancedOptions =
    abstract passphrase: string option
    abstract caFile: string option
    abstract dhParamsFile: string option
    abstract lowMemoryMode: bool option

type SSLServerOptions =
    inherit ServeOptions
    inherit SSLOptions
    inherit SSLAdvancedOptions

    abstract serverNames: IDictionary<string, U2<SSLOptions, SSLAdvancedOptions>>

type Serve = U2<SSLServerOptions, ServeOptions>


type BunServer =
    abstract stop: unit -> unit
    abstract pendingRequests: float
    abstract hostname: string
    abstract development: bool


type PathLike = U3<string, JS.TypedArray, JS.ArrayBuffer>

[<Erase>]
type unsafe =
    [<Emit("$0.arrayBufferToString($1)")>]
    member _.arrayBufferToString(buffer: U2<JS.Uint8Array, JS.ArrayBuffer>) : string = jsNative

    [<Emit("$0.arrayBufferToString($1)")>]
    member _.arrayBufferToString(buffer: JS.Uint16Array) : string = jsNative

    [<Emit("$0.segfault()")>]
    member _.segfault: unit = jsNative

[<Global; Erase>]
type Bun =
    static member serve(options: Serve) : BunServer = jsNative
    static member resolveSync(moduleId: string, parent: string) : string = jsNative
    static member resolve(moduleId: string, parent: string) : JS.Promise<string> = jsNative

    static member write
        (
            destination: U2<Blob, PathLike>,
            input: U4<Blob, JS.TypedArray, string, BlobPart[]>
        ) : JS.Promise<float> =
        jsNative

    static member write(destinationPath: PathLike, input: Response) : JS.Promise<float> = jsNative
    static member write(destination: Blob, input: Blob) : JS.Promise<float> = jsNative
    static member write(destinationPath: PathLike, input: Blob) : JS.Promise<float> = jsNative
    static member file(path: string, ?options: BlobPropertyBag) : Blob = jsNative
    static member file(path: U2<JS.ArrayBuffer, JS.Uint8Array>, ?options: BlobPropertyBag) : Blob = jsNative
    static member file(fileDescriptor: float, ?options: BlobPropertyBag) : Blob = jsNative
    static member allocUnsafe(size: float) : JS.Uint8Array = jsNative
    static member inspect([<System.ParamArray>] values: obj[]) : string = jsNative
    static member mmap(path: PathLike, ?options: MMapOptions) : JS.Uint8Array = jsNative
    static member stdout: Blob = jsNative
    static member stderr: Blob = jsNative
    static member stdin: Blob = jsNative
    static member unsafe: unsafe = jsNative
    static member enableANSIColors: bool = jsNative
    static member main: string = jsNative
    static member gc(foce: bool) : unit = jsNative
    static member nanoseconds() : int64 = jsNative
    static member shrink() : unit = jsNative
    static member openInEditor(path: string, ?options: EditorOptions) : unit = jsNative
    static member sha(input: StringOrBuffer, ?hashInto: JS.Uint8Array) : JS.Uint8Array = jsNative
    static member sha(input: StringOrBuffer, encoding: DigestEncoding) : string = jsNative
