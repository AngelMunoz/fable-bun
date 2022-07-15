module Fable.Deno.Uuid

open Fable.Core

[<ImportMember("fable-deno-uuid")>]
let NIL_UUID: string = jsNative

[<ImportMember("fable-deno-uuid")>]
let isNil (id: string) : bool = jsNative

type V1Options =
    abstract clockseq: float option
    abstract msecs: float option
    abstract node: float[] option
    abstract nsecs: float option
    abstract random: float[] option
    abstract rng: (unit -> float[]) option


[<Erase>]
type V1 =
    [<Import("v1", "fable-deno-uuid"); Emit("v1.generate($1...)")>]
    static member generate(?options: V1Options, ?buffer: int[], ?offset: int) : U2<string, int[]> = jsNative

    [<Import("v1", "fable-deno-uuid"); Emit("v1.validate($1)")>]
    static member validate(id: string) : bool = jsNative

[<Erase>]
type V4 =

    [<Import("v4", "fable-deno-uuid"); Emit("v4.validate($1)")>]
    static member validate(id: string) : bool = jsNative

[<Erase>]
type V5 =
    [<Import("v5", "fable-deno-uuid"); Emit("v5.generate($1...)")>]
    static member generate(nmspace: string, ?data: JS.Uint8Array) : JS.Promise<string> = jsNative

    [<Import("v5", "fable-deno-uuid"); Emit("v5.validate($1)")>]
    static member validate(id: string) : bool = jsNative
