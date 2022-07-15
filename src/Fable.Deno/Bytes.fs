module Fable.Deno.Bytes

open Fable.Core
open System

[<Erase>]
type Bytes =

    [<ImportMember("fable-deno-bytes")>]
    static member concat([<ParamArrayAttribute>] buffer: JS.Uint8Array[]) : JS.Uint8Array = jsNative

    [<ImportMember("fable-deno-bytes")>]
    static member copy(src: JS.Uint8Array, dist: JS.Uint8Array, ?offset: int) : int = jsNative

    [<ImportMember("fable-deno-bytes")>]
    static member startsWith(source: JS.Uint8Array, prefix: JS.Uint8Array) : bool = jsNative

    [<ImportMember("fable-deno-bytes")>]
    static member endsWith(source: JS.Uint8Array, suffix: JS.Uint8Array) : bool = jsNative

    [<ImportMember("fable-deno-bytes")>]
    static member equals(expected: JS.Uint8Array, actual: JS.Uint8Array) : bool = jsNative

    [<ImportMember("fable-deno-bytes")>]
    static member includesNeedle(source: JS.Uint8Array, needkle: JS.Uint8Array, ?start: int) : bool = jsNative

    [<ImportMember("fable-deno-bytes")>]
    static member indexOfNeedle(source: JS.Uint8Array, needkle: JS.Uint8Array, ?start: int) : int = jsNative

    [<ImportMember("fable-deno-bytes")>]
    static member lastIndexOfNeedle(source: JS.Uint8Array, needkle: JS.Uint8Array, ?start: int) : int = jsNative

    [<ImportMember("fable-deno-bytes")>]
    static member repeat(source: JS.Uint8Array, count: int) : JS.Uint8Array = jsNative
