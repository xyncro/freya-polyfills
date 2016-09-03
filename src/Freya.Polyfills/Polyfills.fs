module Freya.Polyfills

open Freya.Core

(* Path And Query *)

[<RequireQualifiedAccess>]
module PathAndQuery =

    let path (pathAndQuery: string) =
        match pathAndQuery.IndexOf '?' with
        | i when i = -1 -> pathAndQuery
        | i -> pathAndQuery.Substring (0, i)

(* Optics *)

[<RequireQualifiedAccess>]
module Request =

    let pathRaw_ =
        State.value_<string> "owin.RequestPathRaw"