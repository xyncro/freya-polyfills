module Freya.Polyfills.Kestrel

open Freya.Core
open Freya.Core.Operators
open Freya.Polyfills
open Microsoft.AspNetCore.Http

(* HttpContext *)

let private httpContext_ =
        State.value_<DefaultHttpContext> "Microsoft.AspNetCore.Http.HttpContext"

let private httpContext =
        !. httpContext_
    >>= function | Some x -> Request.pathRaw_ .= Some (PathAndQuery.path (string x.Request.Path))
                 | _ -> Freya.empty

(* Polyfill *)

[<RequireQualifiedAccess>]
module Polyfill =

    let kestrel =
            httpContext
         *> Pipeline.next