[<AutoOpen>]
module Extensions

open Fable.Core

[<Emit("{...$0, ...$1}")>]
let inline mergeObjects (o1, o2) = jsNative
