open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Hosting
open Freya.Core
open Freya.Optics.Http
open Freya.Polyfills
open Freya.Polyfills.Kestrel
open Hopac

let freya =
    freya {
        let! requestMethod = Freya.Optic.get Request.method_
        let! requestPathBase = Freya.Optic.get Request.pathBase_
        let! requestPath = Freya.Optic.get Request.pathRaw_
        let! requestQs = Freya.Optic.get Request.query_
        do! Freya.Optic.set Response.statusCode_ (Some 200)
        let! stream = Freya.Optic.get Response.body_
        let sw = new System.IO.StreamWriter(stream)
        do! Freya.fromJob <| Job.fromUnitTask (fun () -> sw.WriteLineAsync(sprintf "%A - %A - %A - %A" requestMethod requestPathBase requestPath requestQs))
        do! Freya.fromJob <| Job.fromUnitTask (fun () -> sw.WriteLineAsync("Hello world"))
        do! Freya.fromJob <| Job.fromUnitTask sw.FlushAsync
        return Halt
    }

type Startup() =
    member __.Configure (app: IApplicationBuilder) =
        app.UseFreya(freya) |> ignore

[<EntryPoint>]
let main args =

    try
        WebHostBuilder()
            .UseKestrel()
            //.UseUrls([| "http://localhost:7000" |])
            //.UseContentRoot(Directory.GetCurrentDirectory())
            //.UseDefaultHostingConfiguration(args)
            .UseStartup<Startup>()
            .Build()
            .Run()
    with
        | ex ->
            printfn "%A" ex

    0
