module Bix.Handlers

open Fetch
open Fable.Bun
open Bix.Types


let sendJson<'T> (value: 'T) : HttpHandler =
    fun next ctx ->
        ctx.SetStarted true
        Promise.lift (Some(Json value))

let encodeJson<'T> (value: 'T, encoder: 'T -> string) : HttpHandler =
    fun next ctx ->
        ctx.SetStarted true

        let jsonResult = JsonOptions(value, unbox encoder) |> Some
        Promise.lift jsonResult

let sendText (value: string) : HttpHandler =
    fun next ctx ->
        ctx.SetStarted true
        Promise.lift (Some(Text value))

let sendHtml (value: string) : HttpHandler =
    fun next ctx ->
        ctx.SetStarted true
        Promise.lift (Some(Html value))

let sendHtmlFile (path: string) : HttpHandler =
    fun next ctx ->
        ctx.SetStarted true
        let content = Bun.file (path, unbox {| ``type`` = "text/html" |}), "text/html"
        let content = Blob content |> Some
        Promise.lift content


let setContentType (contentType: string) : HttpHandler =
    fun next ctx ->
        let headers = ctx.Response.Headers
        headers.append ("content-type", contentType)

        let res =
            createResponseInit
                ""
                {| headers = headers
                   status = ctx.Response.Status
                   statusText = ctx.Response.StatusText |}

        ctx.SetResponse(res)
        next ctx

let setStatusCode (code: int) : HttpHandler =
    fun next ctx ->
        let headers = ctx.Response.Headers

        let res =
            createResponseInit
                ""
                {| headers = headers
                   status = code
                   statusText = ctx.Response.StatusText |}

        ctx.SetResponse(res)
        next ctx

let notFoundHandler: HttpHandler = setStatusCode 404 >=> sendText "Not Found"

let cleanResponse: HttpHandler =
    fun next ctx ->
        ctx.SetStarted true
        ctx.SetResponse(Response.create ("", []))
        Promise.lift None

let tryBindJson<'T>
    (
        binder: obj -> Result<'T, exn>,
        success: 'T -> HttpHandler,
        error: exn -> HttpHandler
    ) : HttpHandler =
    fun next ctx ->
        ctx.Request.json ()
        |> Promise.bind (fun content ->
            let content =
                try
                    binder content |> Result.map id
                with ex ->
                    Result.Error ex

            match content with
            | Ok content -> (success content) next ctx
            | Result.Error err -> (error err) next ctx)

let tryDecodeJson<'T>
    (
        binder: string -> Result<'T, exn>,
        success: 'T -> HttpHandler,
        error: exn -> HttpHandler
    ) : HttpHandler =
    fun next ctx ->
        ctx.Request.text ()
        |> Promise.bind (fun content ->
            let content =
                try
                    binder content |> Result.map id
                with ex ->
                    Result.Error ex

            match content with
            | Ok content -> (success content) next ctx
            | Result.Error err -> (error err) next ctx)
