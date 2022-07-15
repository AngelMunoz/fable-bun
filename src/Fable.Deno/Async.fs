module Fable.Deno.Async

open System.Collections.Generic
open Fable.Core
open Fetch
open System


type DelayOptions =
    abstract signal: AbortSignal option

[<StringEnum>]
type DeferredState =
    | Pending
    | Fulfilled
    | Rejected

type Deferred<'T> =
    inherit JS.Promise<'T>

    abstract member state: DeferredState

[<ImportMember("fable-deno-async")>]
exception DeadlineError

[<ImportMember("fable-deno-async")>]
type MuxAsyncIterator<'T> =
    inherit IAsyncEnumerable<'T>

    abstract member add: IAsyncEnumerable<'T> -> unit

[<ImportMember("fable-deno-async")>]
let ERROR_WHILE_MAPPING_MESSAGE: string = jsNative

[<ImportMember("fable-deno-async")>]
let abortable<'T>
    (p: U2<IAsyncEnumerable<'T>, JS.Promise<'T>>)
    (signal: AbortSignal)
    : U2<JS.Promise<'T>, IAsyncEnumerable<'T>> =
    jsNative

[<ImportMember("fable-deno-async")>]
let abortableAsyncIterable<'T> (p: IAsyncEnumerable<'T>) (signal: AbortSignal) : IAsyncEnumerable<'T> = jsNative

[<ImportMember("fable-deno-async")>]
let abortablePromise<'T> (p: JS.Promise<'T>) (signal: AbortSignal) : JS.Promise<'T> = jsNative

[<ImportMember("fable-deno-async")>]
let deadline<'T> (p: JS.Promise<'T>) (delay: float) : JS.Promise<'T> = jsNative

[<ImportMember("fable-deno-async")>]
let debounce (fn: Action) (delay: float) : Action = jsNative

[<ImportMember("fable-deno-async")>]
let deferred<'T> (fn: Action) (delay: float) : Deferred<'T> = jsNative

[<ImportMember("fable-deno-async")>]
let delay<'T> (ms: float) : JS.Promise<'T> = jsNative

[<Import("delay", "fable-deno-async"); Emit("delay($1...)")>]
let delayWithOptions<'T> (ms: float) (options: DelayOptions) : JS.Promise<'T> = jsNative

[<Import("pooledMap", "fable-deno-async"); Emit("pooledMap($1, $3, $2)")>]
let pooledMapSeq<'T, 'R> (poolLimit: int) (iteratorFn: 'T -> JS.Promise<'R>) (items: 'T seq) : IAsyncEnumerable<'R> =
    jsNative

[<Import("pooledMap", "fable-deno-async"); Emit("pooledMap($1, $3, $2)")>]
let pooledMapAsyncSeq<'T, 'R>
    (poolLimit: int)
    (iteratorFn: 'T -> JS.Promise<'R>)
    (items: IAsyncEnumerable<'T>)
    : IAsyncEnumerable<'R> =
    jsNative
