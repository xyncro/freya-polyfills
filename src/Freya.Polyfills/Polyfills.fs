module Freya.Polyfills

open Freya.Core

// Path And Query

[<RequireQualifiedAccess>]
module PathAndQuery =

    /// Extract the path from a string which may contain a path including a
    /// query component.

    let path (pathAndQuery: string) =
        match pathAndQuery.IndexOf '?' with
        | i when i = -1 -> pathAndQuery
        | i -> pathAndQuery.Substring (0, i)

// Optics

[<RequireQualifiedAccess>]
module Request =

    /// A lens from State -> string option, accessing the raw path if present
    /// within the OWIN environment. The "owin.RequestPathRaw" key is a
    /// possible future addition to the OWIN standard, and is not part of
    /// current OWIN specifications.

    /// The current implementation is required as part of the Freya Polyfills
    /// approach.

    let pathRaw_ =
        State.value_<string> "owin.RequestPathRaw"