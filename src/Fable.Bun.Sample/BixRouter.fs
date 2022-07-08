[<RequireQualifiedAccess>]
module Bix.Router

open Fable.Bun
open Fable.Core
open Fetch
open Bix.Types

let Empty = Map.empty

let inline get (path: string, handler: HttpHandler) (routes: RouteMap) : RouteMap =
    routes |> Map.add (Get, path) handler

let inline post (path: string, handler: HttpHandler) (routes: RouteMap) : RouteMap =
    routes |> Map.add (Post, path) handler

let inline put (path: string, handler: HttpHandler) (routes: RouteMap) : RouteMap =
    routes |> Map.add (Put, path) handler

let inline delete (path: string, handler: HttpHandler) (routes: RouteMap) : RouteMap =
    routes |> Map.add (Delete, path) handler

let inline patch (path: string, handler: HttpHandler) (routes: RouteMap) : RouteMap =
    routes |> Map.add (Patch, path) handler
