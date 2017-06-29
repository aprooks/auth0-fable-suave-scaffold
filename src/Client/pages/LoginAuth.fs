module Client.LoginAuth
open Fable.Import
open Fable.Core.JsInterop
open Fable.Core
open Fable.Import.JS
open Fable.Import.Browser

let myConfig = 
    createObj [
        "domain" ==> "fable-test.eu.auth0.com"
        "clientID" ==> "E0a7096d6yYIPYtVdDR1XEChjqsA7p4e"
        "callbackUrl" ==> "http://localhost:8080/#home"
        "apiUrl" ==> "fable.example.com"
        "responseType" ==> "token id_token"
        "scope" ==> "openid profile"
    ]


type Model = 
| Login 
| TokenValidation of string

let auth0lock:JsConstructor<string,string,obj> = importDefault "auth0-lock/lib/index.js"

let lock = auth0lock.Create("E0a7096d6yYIPYtVdDR1XEChjqsA7p4e","fable-test.eu.auth0.com")
debugger()
// lock?on("authenticated",
//     fun authResult ->
//        debugger()
//        printfn "%O" authResult
//        Utils.save "auth" authResult
// ) |> ignore

lock?on("show",
    fun _ ->
        printfn "show"
    ) |> ignore

let isAccessToken() = 
    let hash = window.location.hash
    match hash.StartsWith "#access_token" with
    | true -> Some hash
    | false -> None

let tokenurl = """http://localhost:8080/#access_token=fpngg5V9VCKmfhle&expires_in=86400&id_token=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwczovL2ZhYmxlLXRlc3QuZXUuYXV0aDAuY29tLyIsInN1YiI6ImF1dGgwfDU5NTIyOWMzYTNlNDM3NmZlYmQ0MDBjZSIsImF1ZCI6IkUwYTcwOTZkNnlZSVBZdFZkRFIxWEVDaGpxc0E3cDRlIiwiZXhwIjoxNDk4NzAwODk5LCJpYXQiOjE0OTg2NjQ4OTl9.Fo4L-xe8IEwJCtM8YTpx5XUYKxDRPRPpltYtxTfs3Qs&token_type=Bearer&state=za51ObdDLsuxTTR50UnzITuQFrRuRZVN"""

open Messages
open Fable.Helpers.React
open Fable.Helpers.React.Props

let view (model:Model) (dispatch: AppMsg -> unit) = 
    match model with 
    | TokenValidation token ->
        lock?resumeAuth(token,System.Func<obj,obj,unit>( fun err result->
            if not <| isNull err then 
                printfn "Error on resumeAuth: %O" err
            
            printfn "Token received: %O" result

            lock?getUserInfo(result?accessToken, 
                System.Func<obj,obj,unit> (fun err result ->
                    if not<| isNull result then
                        printfn "userProfile loaded %O" result
                    AppMsg.LoggedIn |> dispatch
                )
            )|>ignore
        )) |> ignore
        
        div [Id "loginAuth view"] [str "Login auth0"]
    | Model.Login ->
        lock?show() |> ignore
        div [] [str "Auth 0 stub"]

let login() =
    match Utils.load "auth" with
    | Some token -> ignore()
    | None -> 
        printfn "authenticating"
        lock?show() |> ignore
