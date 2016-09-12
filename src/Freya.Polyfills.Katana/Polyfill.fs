module Freya.Polyfills.Katana

open System.Net
open System.Web
open Freya.Core
open Freya.Core.Operators
open Freya.Polyfills

// HttpListener

let private httpListener_ =
        State.value_<HttpListenerContext> "System.Net.HttpListenerContext"

let private httpListener =
        !. httpListener_
    >>= function | Some x -> Request.pathRaw_ .= Some (PathAndQuery.path x.Request.RawUrl)
                 | _ -> Freya.empty

// HttpContext

let private httpContext_ =
        State.value_<HttpContextWrapper> "System.Web.HttpContextBase"

let private httpContext =
        !. httpContext_
    >>= function | Some x -> Request.pathRaw_ .= Some (PathAndQuery.path x.Request.Url.PathAndQuery)
                 | _ -> Freya.empty

// Polyfill

/// Polyfills for prpviding support for potential future OWIN standards,
/// allowing Freya to provide support for functionality outside of the current
/// standards where the functionality is available in the underlying server in
/// a non-standardised way.

[<RequireQualifiedAccess>]
module Polyfill =

    /// The Freya polyfill for Katana-based servers (to be used with SelfHost
    /// and IIS). The polyfill should be included as the first item in a
    /// pipeline composition of the Freya application to ensure that any data
    /// provided by the polyfill is available to any subsequent component.

    let katana =
            httpListener
         *> httpContext
         *> Pipeline.next