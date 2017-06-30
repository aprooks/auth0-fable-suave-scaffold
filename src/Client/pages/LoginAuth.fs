module Client.LoginAuth
open Fable.Core
open Fable.Import.Browser
open Fable.Helpers.React
open Fable.Helpers.React.Props

open Messages
open Client.Auth0Lock

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
        
        div [Id "loginAuth view"] [str "Auth0 token validation page"]  //should not be ever visible
    | Model.Login ->
        lock.show()
        div [] [str "Auth 0 stub"]