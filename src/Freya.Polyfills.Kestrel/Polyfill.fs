[<System.Runtime.CompilerServices.Extension>]
module Freya.Polyfills.Kestrel

open Freya.Core
open Freya.Core.Operators
open Freya.Polyfills
open Microsoft.AspNetCore.Http

module Optics =
    /// Provides access to the Kestrel `HttpContext` object for the current request.
    let httpContext_ =
        State.value_<DefaultHttpContext> "Microsoft.AspNetCore.Http.HttpContext"

/// Retreives the Kestrel `HttpContext` object for the current request.
let httpContext = Freya.Optic.get Optics.httpContext_

/// Retreives a service held by the Kestrel `IServiceContainer`.
///
/// *Optimization note*: define a static value which pre-applies the type of the
/// service you wish to retreive rather than calling `requestService` from inside
/// of a runtime function.
let requestService<'a> : Freya<'a option> =
    httpContext
    |> Freya.map (Option.bind (fun x -> x.RequestServices.GetService(typeof<'a>) |> Option.ofObj |> Option.map unbox))

// Polyfill

/// Polyfills for providing support for potential future OWIN standards,
/// allowing Freya to provide support for functionality outside of the current
/// standards where the functionality is available in the underlying server in
/// a non-standardised way.

[<RequireQualifiedAccess>]
module Polyfill =

    let private applyPolyfill =
            !. Optics.httpContext_
        >>= function | Some x -> Request.pathRaw_ .= Some (PathAndQuery.path (string x.Request.Path))
                     | _ -> Freya.empty

    /// The Freya polyfill for Kestrel-based servers. The polyfill should be
    /// included as the first item in a pipeline composition of the Freya
    /// application to ensure that any data provided by the polyfill is
    /// available to any subsequent component.

    let kestrel =
            applyPolyfill
         *> Pipeline.next

open Microsoft.AspNetCore.Builder

[<System.Runtime.CompilerServices.Extension>]
[<Sealed>]
[<AbstractClass>]
type ApplicationBuilderExtensions private () =

    /// Attaches a Freya middleware into an ASP.NET Core application pipeline.
    [<System.Runtime.CompilerServices.Extension>]
    static member UseFreya(app: IApplicationBuilder, freya: Freya<PipelineChoice>) =
        let owin : OwinMidFunc = OwinMidFunc.ofFreya (Polyfill.kestrel *> freya)
        app.UseOwin(fun p -> p.Invoke owin)

    /// Builds a Freya middleware and attaches it into an ASP.NET Core application pipeline.
    [<System.Runtime.CompilerServices.Extension>]
    static member UseFreya(app: IApplicationBuilder, freyaBuider: System.Func<Freya<PipelineChoice>>) =
        app.UseFreya(freyaBuider.Invoke())

    /// Builds a Freya middleware and attaches it into an ASP.NET Core application pipeline.
    [<System.Runtime.CompilerServices.Extension>]
    static member UseFreya(app: IApplicationBuilder, freyaBuider: unit -> Freya<PipelineChoice>) =
        app.UseFreya(freyaBuider())
