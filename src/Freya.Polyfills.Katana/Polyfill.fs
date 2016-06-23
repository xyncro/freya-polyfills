module Freya.Polyfills.Katana

open System.Net
open System.Web
open Freya.Core
open Freya.Core.Operators
open Freya.Polyfills

(* HttpListener *)

let private httpListener_ =
        State.value_<HttpListenerContext> "System.Net.HttpListenerContext"

let private httpListener =
        !. httpListener_
    >>= function | Some x -> Request.pathRaw_ .= Some (PathAndQuery.path x.Request.RawUrl)
                 | _ -> Freya.empty

(* HttpContext *)

let private httpContext_ =
        State.value_<HttpContextWrapper> "System.Web.HttpContextBase"

let private httpContext =
        !. httpContext_
    >>= function | Some x -> Request.pathRaw_ .= Some (PathAndQuery.path x.Request.Url.PathAndQuery)
                 | _ -> Freya.empty

(* Polyfill *)

[<RequireQualifiedAccess>]
module Polyfill =

    let katana =
            httpListener
         *> httpContext
         *> Pipeline.next