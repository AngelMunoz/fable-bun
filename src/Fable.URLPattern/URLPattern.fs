module URLPattern

open Fable.Core

#if ENABLE_URLPATTERN_POLYFILL
JsInterop.importSideEffects "urlpattern-polyfill"
#endif

type URLPatternComponentResult =
    abstract input: string
    abstract groups: obj

type UrlPatternInit =
    abstract baseURL: string option
    abstract username: string option
    abstract password: string option
    abstract protocol: string option
    abstract hostname: string option
    abstract port: string option
    abstract pathname: string option
    abstract search: string option
    abstract hash: string option

type URLPatternInput = U2<string, UrlPatternInit>

type URLPatternResult =
    abstract inputs: URLPatternInput[]
    abstract protocol: URLPatternComponentResult
    abstract username: URLPatternComponentResult
    abstract password: URLPatternComponentResult
    abstract hostname: URLPatternComponentResult
    abstract port: URLPatternComponentResult
    abstract pathname: URLPatternComponentResult
    abstract search: URLPatternComponentResult
    abstract hash: URLPatternComponentResult

[<Global; Erase>]
type URLPattern(?init: URLPatternInput, ?baseURL: string) =

    member _.test(?input: URLPatternInput, ?baseURL: string) : bool = jsNative
    member _.exec(?input: URLPatternInput, ?baseURL: string) : URLPatternResult option = jsNative
    member _.protocol: string = jsNative
    member _.username: string = jsNative
    member _.password: string = jsNative
    member _.hostname: string = jsNative
    member _.port: string = jsNative
    member _.pathname: string = jsNative
    member _.search: string = jsNative
    member _.hash: string = jsNative

type UrlInitArgs =
    | BaseURL of string
    | Username of string
    | Password of string
    | Protocol of string
    | Hostname of string
    | Port of string
    | Pathname of string
    | Search of string
    | Hash of string

[<RequireQualifiedAccess>]
module UrlPatternInit =
    let inline fromArgs (args: UrlInitArgs list) : UrlPatternInit =
        let args =
            args
            |> JsInterop.keyValueList CaseRules.LowerFirst

        unbox<UrlPatternInit> args
