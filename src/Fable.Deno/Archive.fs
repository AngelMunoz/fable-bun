module Fable.Deno.Archive

open Fable.Core
open Fable.Deno
open System.Collections.Generic

[<AllowNullLiteral>]
type TarData =
    abstract checksum: string option
    abstract fileMode: string option
    abstract fileName: string option
    abstract fileNamePrefix: string option
    abstract fileSize: string option
    abstract gid: string option
    abstract group: string option
    abstract mtime: string option
    abstract owner: string option
    abstract ``type``: string option
    abstract uid: string option
    abstract ustar: string option

[<AllowNullLiteral>]
type TarDataWithSource =
    inherit TarData
    abstract filePath: string option
    abstract reader: Reader option

[<AllowNullLiteral>]
type TarInfo =
    abstract fileMode: int option
    abstract gid: int option
    abstract group: string option
    abstract mtime: int option
    abstract owner: string option
    abstract ``type``: string option
    abstract uid: int option

[<AllowNullLiteral>]
type TarMeta =
    inherit TarInfo
    abstract fileName: string
    abstract fileSize: float option

[<AllowNullLiteral>]
type TarOptions =
    inherit TarInfo
    abstract contentSize: float option
    abstract filePath: string option
    abstract reader: Reader option

[<AbstractClass; Erase>]
type TarHeader =

    [<Emit("$0[$1]")>]
    member _.Item(key: string) : JS.Uint8Array = jsNative

[<ImportMember("fable-deno-archive")>]
type Tar() =
    member _.data: TarDataWithSource[] = jsNative
    member _.append(fn: string, opts: TarOptions) : JS.Promise<unit> = jsNative
    member _.getReader() : Reader = jsNative

[<ImportMember("fable-deno-archive")>]
type TarEntry =
    inherit Reader

    abstract consumed: bool
    abstract discard: unit -> JS.Promise<unit>

[<ImportMember("fable-deno-archive")>]
type Untar =
    inherit IAsyncEnumerable<TarEntry>

    abstract block: JS.Uint8Array
    abstract reader: Reader

    abstract extract: unit -> JS.Promise<TarEntry option>
