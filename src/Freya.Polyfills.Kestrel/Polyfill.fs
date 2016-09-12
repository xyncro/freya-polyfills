module Freya.Polyfills.Kestrel

open Freya.Core
open Freya.Core.Operators
open Freya.Polyfills
open Microsoft.AspNetCore.Http

// HttpContext

let private httpContext_ =
        State.value_<DefaultHttpContext> "Microsoft.AspNetCore.Http.HttpContext"

let private httpContext =
        !. httpContext_
    >>= function | Some x -> Request.pathRaw_ .= Some (PathAndQuery.path (string x.Request.Path))
                 | _ -> Freya.empty

// Polyfill

/// Polyfills for prpviding support for potential future OWIN standards,
/// allowing Freya to provide support for functionality outside of the current
/// standards where the functionality is available in the underlying server in
/// a non-standardised way.

[<RequireQualifiedAccess>]
module Polyfill =

    /// The Freya polyfill for Kestrel-based servers. The polyfill should be
    /// included as the first item in a pipeline composition of the Freya
    /// application to ensure that any data provided by the polyfill is
    /// available to any subsequent component.

    let kestrel =
            httpContext
         *> Pipeline.next