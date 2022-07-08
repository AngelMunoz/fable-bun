[<AutoOpen>]
module Extensions

open Fable.Core
open Bix.Types

[<Emit("{...$0, ...$1}")>]
let inline mergeObjects (o1, o2) = jsNative

[<Emit("new Response($0, $1)")>]
let inline internal createResponseInit (content: obj) (options: obj) = jsNative

let compose (h1: HttpHandler) (h2: HttpHandler) : HttpHandler =
    fun final ->
        let fn = final |> h2 |> h1

        fun ctx ->
            match ctx.HasStarted with
            | true -> final ctx
            | false -> fn ctx

let inline (>=>) h1 h2 = compose h1 h2
