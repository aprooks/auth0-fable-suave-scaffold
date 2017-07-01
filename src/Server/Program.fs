/// Server program entry point module.
module ServerCode.Program

open System.IO

let GetEnvVar var = 
    match System.Environment.GetEnvironmentVariable(var) with
    | null -> None
    | value -> Some value

let getPortsOrDefault defaultVal = 
    match System.Environment.GetEnvironmentVariable("SUAVE_FABLE_PORT") with
    | null -> defaultVal
    | value -> value |> uint16

Dependency.Say.hello "test"
// let testToken ()=
//     let tkn = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwczovL2ZhYmxlLXRlc3QuZXUuYXV0aDAuY29tLyIsInN1YiI6ImF1dGgwfDU5NTIyOWMzYTNlNDM3NmZlYmQ0MDBjZSIsImF1ZCI6IkUwYTcwOTZkNnlZSVBZdFZkRFIxWEVDaGpxc0E3cDRlIiwiZXhwIjoxNDk4ODcxNTk2LCJpYXQiOjE0OTg4MzU1OTZ9.uZxxR9RN-vUJk08HtS1NdJkAk0z6CKZ4g53lcCyFUes";
//     match JwtToken.isValid tkn with 
//     | Some principal ->
        
//         printfn "loaded claims from example:"
//         principal.Claims |> Seq.iter(fun c -> printfn "%s %s" c.Type c.Value)
//         printfn "loaded identity: "
//         principal.Identity.Name |> printfn "%s"
//     | None -> printfn "Token considered to be invalid :("

// testToken()

[<EntryPoint>]
let main args =
    try
        let clientPath =
            match args |> Array.toList with
            | clientPath:: _  when Directory.Exists clientPath -> clientPath
            | _ -> 
                let devPath = Path.Combine("..","Client")
                if Directory.Exists devPath then devPath else
                @"./client"

        WebServer.start (Path.GetFullPath clientPath) (getPortsOrDefault 8085us)
        0
    with
    | exn ->
        let color = System.Console.ForegroundColor
        System.Console.ForegroundColor <- System.ConsoleColor.Red
        System.Console.WriteLine(exn.Message)
        System.Console.ForegroundColor <- color
        1
