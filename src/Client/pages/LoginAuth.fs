module Client.LoginAuth
open Fable.Core
open Fable.Import.Browser
open Fable.Helpers.React
open Fable.Helpers.React.Props

open Messages
open Client.Auth0Lock


open Fable.PowerPack

//TODO: move client id and domain to config via webpack
let lock = auth0lock.Create("E0a7096d6yYIPYtVdDR1XEChjqsA7p4e", "fable-test.eu.auth0.com", myConfig)

type Model = 
| Login 
| TokenValidation of string

let isAccessToken() = 
    let hash = window.location.hash
    match hash.StartsWith "#access_token" with
    | true  -> Some hash
    | false -> None

//this could be a small or hidden component, made it a page to see what's happening
let view (model:Model) (dispatch: AppMsg -> unit) = 
    match model with 
    | TokenValidation token -> 
        promise {
                let! authResult = lock.resumeAuth token |> promisify
                printfn "[Auth] Token received: %O" authResult
                let! profile = lock.getUserInfo authResult.accessToken |> promisify
        
                printfn "[Auth] userProfile loaded %O" profile

                toUserProfile authResult profile 
                |> AppMsg.ProfileLoaded
                |> dispatch
        } |> Promise.start
        div [Id "loginAuth view"] [str "Auth0 token validation page"] 
    | Model.Login ->
        lock.show()
        div [] [str "Auth 0 stub"]