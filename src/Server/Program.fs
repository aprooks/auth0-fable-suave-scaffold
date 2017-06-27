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

let testToken ()=
    let principal = JwtToken.isValid JwtToken.exampleToken |> Option.get
    
    printfn "loaded claims from example:"
    principal.Claims |> Seq.iter(fun c -> printfn "%s %s" c.Type c.Value)
    printfn "loaded identity: "
    principal.Identity.Name |> printfn "%s"

testToken()

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
