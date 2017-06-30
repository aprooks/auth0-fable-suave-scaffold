module Client.Auth0Lock

open Fable.Import.JS
open Fable.Core.JsInterop
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
    
type Lock = 
    abstract show        : unit -> unit
    abstract resumeAuth  : string -> Callback<obj,AuthResult> -> unit
    abstract getUserInfo : string -> Callback<obj,Profile> -> unit
    
let auth0lock:JsConstructor<string,string,obj,Lock> = importDefault "auth0-lock/lib/index.js"

