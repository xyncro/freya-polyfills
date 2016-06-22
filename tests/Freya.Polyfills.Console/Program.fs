open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Hosting

type Startup() =
    member __.Configure (app: IApplicationBuilder) =
        app.Run (fun x ->
            printfn "%A - %A - %A - %A" x.Request.Method x.Request.PathBase x.Request.Path x.Request.QueryString
            x.Response.WriteAsync("Hello world"))

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
