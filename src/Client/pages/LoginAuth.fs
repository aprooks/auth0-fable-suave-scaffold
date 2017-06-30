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


type Callback<'TErr,'TRes>  = System.Func<'TErr,'TRes,unit>

type Model = 
| Login 
| TokenValidation of string

Utils.load "profile" |> printfn "loaded profile from storage: %O"

type [<AllowNullLiteral>] AuthResult =
    abstract accessToken : string with get
    abstract idToken     : string with get

type [<AllowNullLiteral>] Profile = 
    abstract name        : string with get
    abstract user_id     : string with get
    abstract email       : string with get
    abstract picture     : string with get

type Lock = 
    abstract show        : unit -> unit
    abstract resumeAuth  : string -> Callback<obj,AuthResult> -> unit
    abstract getUserInfo : string -> Callback<obj,Profile> -> unit
    
let auth0lock:JsConstructor<string,string,Lock> = importDefault "auth0-lock/lib/index.js"

let lock = auth0lock.Create("E0a7096d6yYIPYtVdDR1XEChjqsA7p4e","fable-test.eu.auth0.com")

let isAccessToken() = 
    let hash = window.location.hash
    match hash.StartsWith "#access_token" with
    | true -> Some hash
    | false -> None

let exampleTokenUrl = """http://localhost:8080/#access_token=fpngg5V9VCKmfhle&expires_in=86400&id_token=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwczovL2ZhYmxlLXRlc3QuZXUuYXV0aDAuY29tLyIsInN1YiI6ImF1dGgwfDU5NTIyOWMzYTNlNDM3NmZlYmQ0MDBjZSIsImF1ZCI6IkUwYTcwOTZkNnlZSVBZdFZkRFIxWEVDaGpxc0E3cDRlIiwiZXhwIjoxNDk4NzAwODk5LCJpYXQiOjE0OTg2NjQ4OTl9.Fo4L-xe8IEwJCtM8YTpx5XUYKxDRPRPpltYtxTfs3Qs&token_type=Bearer&state=za51ObdDLsuxTTR50UnzITuQFrRuRZVN"""

open Messages
open Fable.Helpers.React
open Fable.Helpers.React.Props

let callback<'err,'res when 'err:null and 'res: null> cb =
    System.Func<'err,'res,unit>(
        fun err result -> 
            if not <| isNull err then 
                Result.Error err |> cb
            else if not <| isNull result then
                Result.Ok result |> cb
            else
                failwith "Both error and result are empty which should be impossible ¯\\_(ツ)_/¯"
    )
    

let view (model:Model) (dispatch: AppMsg -> unit) = 
    match model with 
    | TokenValidation token ->
        lock.resumeAuth token
            (callback <| fun res -> 
                match res with
                | Result.Error err ->
                    printfn "Error on resumeAuth: %O" err
                | Result.Ok authResult -> 
                    printfn "Token received: %O" authResult
                    Utils.save "token" authResult
                    lock.getUserInfo 
                        authResult.accessToken
                        (callback <| fun res -> 
                            match res with
                            | Result.Ok profile ->
                                printfn "userProfile loaded %O" profile
                                Utils.save "profile" profile                                
                                AppMsg.LoggedIn |> dispatch
                            | Result.Error err -> 
                                printfn "error getting user profile %O" err
                        )
            )
        
        div [Id "loginAuth view"] [str "Login auth0"]
    | Model.Login ->
        lock.show()
        div [] [str "Auth 0 stub"]

let login() =
    match Utils.load "auth" with
    | Some token -> ignore()
    | None -> 
        printfn "authenticating"
        lock.show()
