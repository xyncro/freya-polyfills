module Freya.Polyfills

open Freya.Core

(* Optics *)

[<RequireQualifiedAccess>]
module Request =

    let rawPathAndQuery_ =
        State.value_<string> "owin.RawPathAndQuery"