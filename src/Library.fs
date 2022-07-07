module Lib

open Fable.Core.JsInterop
open Fetch


let server =
    Bun.serve ({| fetch = fun (req: Request) -> Response.create (!! $"Hello visitor from {req.url}") |})

let mode =
    if server.development then
        "Development"
    else
        "Production"

printfn $"Mode: {mode}"
printfn $"Server started at {server.hostname}"
