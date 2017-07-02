module Client.Auth0Lock

open Fable.Import.JS
open Fable.Core.JsInterop
open Fable.PowerPack

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

let toUserProfile (auth:AuthResult) (profile:Profile) = {
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



//No idea what data they are returning with an error
exception GenericAuthException of string*string

let promisify<'res,'err when 'res : null and 'err : null> fn = 
    let l = fun resolve reject ->
            fn (Callback<'err,'res>(
                    fun err res ->
                        if not <| isNull res then
                            resolve res
                        if not <| isNull err then
                            GenericAuthException ("callbackException", (err |> Fable.Core.JsInterop.toJson))  |> reject
                        failwith "Both Result and Errors of callback are empty which should be impossible"
        ))
    l |> Fable.PowerPack.Promise.create

let auth0lock:JsConstructor<string,string,obj,Lock> = importDefault "auth0-lock/lib/index.js"
