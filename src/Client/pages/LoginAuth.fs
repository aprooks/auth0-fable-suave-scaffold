module Client.LoginAuth
open Fable.Import
open Fable.Core.JsInterop
open Fable.Core
open Fable.Import.JS
open Fable.Import.Browser

open Messages
let myConfig = 
    createObj [
       "oidcConformat" ==> true
       "auth" ==> createObj [
           "params" ==> createObj [
                "scope"    ==> "openid profile"
                "audience" ==> "fable.example.com"
            ]
        ]
    ]

type Callback<'TErr,'TRes>  = System.Func<'TErr,'TRes,unit>

type Model = 
| Login 
| TokenValidation of string

Utils.load "profile" |> printfn "loaded profile from storage: %O"

type [<AllowNullLiteral>] AuthResult =
    abstract accessToken : string with get

type [<AllowNullLiteral>] Profile = 
    abstract name        : string with get
    abstract user_id     : string with get
    abstract email       : string with get
    abstract picture     : string with get

let auth0User (auth:AuthResult) (profile:Profile) = {
        AccessToken = auth.accessToken
        Name = profile.name
        Email = profile.email
        Picture = profile.picture
        UserId = profile.user_id
    }

type Lock = 
    abstract show        : unit -> unit
    abstract resumeAuth  : string -> Callback<obj,AuthResult> -> unit
    abstract getUserInfo : string -> Callback<obj,Profile> -> unit
    
let auth0lock:JsConstructor<string,string,obj,Lock> = importDefault "auth0-lock/lib/index.js"

//TODO: move client id and domain to config via webpack
let lock = auth0lock.Create("E0a7096d6yYIPYtVdDR1XEChjqsA7p4e", "fable-test.eu.auth0.com", myConfig)

let isAccessToken() = 
    let hash = window.location.hash
    match hash.StartsWith "#access_token" with
    | true -> Some hash
    | false -> None

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
        //TODO: move logic to App.update via Cmd?
        lock.resumeAuth 
            token
            (callback <| fun res -> 
                match res with
                | Result.Error err ->
                    printfn "[Auth] Error on resumeAuth: %O" err
                | Result.Ok authResult -> 
                    printfn "[Auth] Token received: %O" authResult

                    lock.getUserInfo 
                        authResult.accessToken
                        (callback <| fun res -> 
                            match res with
                            | Result.Ok profile ->
                                printfn "[Auth] userProfile loaded %O" profile
                                auth0User authResult profile 
                                |> AppMsg.ProfileLoaded
                                |> dispatch
                            | Result.Error err -> 
                                printfn "[Auth] error getting user profile %O" err
                        )
            )
        
        div [Id "loginAuth view"] [str "Login auth0"]
    | Model.Login ->
        lock.show()
        div [] [str "Auth 0 stub"]