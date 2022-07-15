module Fable.Deno.Http

open Fable.Deno
open System.Collections.Generic
open Fable.Core
open Fable.Core.JsInterop

open Fetch

open Browser.Types
open System

type Status =
    | Continue = 100
    | SwitchingProtocols = 101
    | Processing = 102
    | EarlyHints = 103
    | OK = 200
    | Created = 201
    | Accepted = 202
    | NonAuthoritativeInfo = 203
    | NoContent = 204
    | ResetContent = 205
    | PartialContent = 206
    | MultiStatus = 207
    | AlreadyReported = 208
    | IMUsed = 226
    | MultipleChoices = 300
    | MovedPermanently = 301
    | Found = 302
    | SeeOther = 303
    | NotModified = 304
    | UseProxy = 305
    | TemporaryRedirect = 307
    | PermanentRedirect = 308
    | BadRequest = 400
    | Unauthorized = 401
    | PaymentRequired = 402
    | Forbidden = 403
    | NotFound = 404
    | MethodNotAllowed = 405
    | NotAcceptable = 406
    | ProxyAuthRequired = 407
    | RequestTimeout = 408
    | Conflict = 409
    | Gone = 410
    | LengthRequired = 411
    | PreconditionFailed = 412
    | RequestEntityTooLarge = 413
    | RequestURITooLong = 414
    | UnsupportedMediaType = 415
    | RequestedRangeNotSatisfiable = 416
    | ExpectationFailed = 417
    | Teapot = 418
    | MisdirectedRequest = 421
    | UnprocessableEntity = 422
    | Locked = 423
    | FailedDependency = 424
    | TooEarly = 425
    | UpgradeRequired = 426
    | PreconditionRequired = 428
    | TooManyRequests = 429
    | RequestHeaderFieldsTooLarge = 431
    | UnavailableForLegalReasons = 451
    | InternalServerError = 500
    | NotImplemented = 501
    | BadGateway = 502
    | ServiceUnavailable = 503
    | GatewayTimeout = 504
    | HTTPVersionNotSupported = 505
    | VariantAlsoNegotiates = 506
    | InsufficientStorage = 507
    | LoopDetected = 508
    | NotExtended = 510
    | NetworkAuthenticationRequired = 511

type HttpErrorOptions =
    abstract expose: bool option

type CookieAttributes =
    abstract path: string option
    abstract domain: string option

[<StringEnum; RequireQualifiedAccess>]
type CookieSameSite =
    | [<CompiledName("Strict")>] Strict
    | [<CompiledName("Lax")>] Lax
    | [<CompiledName("None")>] None

type Cookie =
    abstract domain: string option
    abstract expires: DateTime option
    abstract httpOnly: bool option
    abstract maxAge: int option
    abstract name: string
    abstract path: string option
    abstract sameSite: CookieSameSite option
    abstract secure: bool option
    abstract unparsed: string[] option
    abstract value: string

[<ImportMember("fable-deno-http")>]
type HttpError(msg) =
    inherit Exception(msg)

    [<Emit("$0.expose")>]
    member _.expose: bool = jsNative

    [<Emit("$0.status")>]
    member _.status: Status = jsNative


type ServeInit =
    abstract onError: (exn -> U2<Response, JS.Promise<Response>>) option
    abstract onListen: (OnListenParams -> unit) option
    abstract signal: AbortSignal option

type ServeTlsInit =
    inherit ServeInit

    abstract member certFile: string
    abstract member keyFile: string

[<ImportMember "fable-deno-http">]
type Server(options: ServerInit) =

    member _.closed: bool = jsNative
    member _.addrs: DenoAddr[] = jsNative

    member _.close() : unit = jsNative

    member _.listenAndServe() : JS.Promise<unit> = jsNative
    member _.listenAndServeTls(certFile: string, keyFile: string) : JS.Promise<unit> = jsNative

    member _.serve(listener: Listener) : JS.Promise<unit> = jsNative

[<AbstractClass; Erase>]
type ErrorMap =

    [<Emit("new $0[$1]")>]
    member _.Item(key: string) : HttpError = jsNative

[<AbstractClass; Erase>]
type StatusTextMap =

    [<Emit("$0[$1]")>]
    member _.Item(key: Status) : string = jsNative

[<ImportMember("fable-deno-http")>]
let errors: ErrorMap = jsNative

[<ImportMember("fable-deno-http")>]
let STATUS_TEXT: StatusTextMap = jsNative

[<Erase>]
type Http =

    [<ImportMember("fable-deno-http")>]
    static member accepts(req: Request, [<ParamArrayAttribute>] types: string[]) : string array = jsNative

    [<ImportMember("fable-deno-http")>]
    static member acceptsEncodings(req: Request, [<ParamArrayAttribute>] encodings: string[]) : string array = jsNative

    [<ImportMember("fable-deno-http")>]
    static member acceptsLanguages(req: Request, [<ParamArrayAttribute>] encodings: string[]) : string array = jsNative

    [<ImportMember("fable-deno-http")>]
    static member createHttpError(?status: Status, ?message: string, ?options: HttpErrorOptions) : HttpError = jsNative

    [<ImportMember("fable-deno-http")>]
    static member deleteCookie(headers: Headers, name: string, ?attributes: CookieAttributes) : unit = jsNative

    [<ImportMember("fable-deno-http")>]
    static member getCookies(headers: Headers) : StringMap = jsNative

    [<ImportMember("fable-deno-http")>]
    static member setCookie(headers: Headers, cookie: Cookie) : StringMap = jsNative

    [<ImportMember("fable-deno-http")>]
    static member isClientErrorStatus(status: Status) : bool = jsNative

    [<ImportMember("fable-deno-http")>]
    static member isErrorStatus(status: Status) : bool = jsNative

    [<ImportMember("fable-deno-http")>]
    static member isHttpError(error: exn) : bool = jsNative

    [<ImportMember("fable-deno-http")>]
    static member isInformationalStatus(status: Status) : bool = jsNative

    [<ImportMember("fable-deno-http")>]
    static member isRedirectStatus(status: Status) : bool = jsNative

    [<ImportMember("fable-deno-http")>]
    static member isServerErrorStatus(status: Status) : bool = jsNative

    [<ImportMember("fable-deno-http")>]
    static member isSuccessfulStatus(status: Status) : bool = jsNative

    [<ImportMember("fable-deno-http")>]
    static member serve
        (
            handler: Request -> U2<Response, JS.Promise<Response>>,
            ?options: ServeInit
        ) : JS.Promise<unit> =
        jsNative

    [<ImportMember("fable-deno-http")>]
    static member serveListener
        (
            listener: Listener,
            handler: Request -> U2<Response, JS.Promise<Response>>,
            ?options: ServeInit
        ) : JS.Promise<unit> =
        jsNative

    [<ImportMember("fable-deno-http")>]
    static member serveTls
        (
            handler: Request -> U2<Response, JS.Promise<Response>>,
            ?options: ServeTlsInit
        ) : JS.Promise<unit> =
        jsNative
