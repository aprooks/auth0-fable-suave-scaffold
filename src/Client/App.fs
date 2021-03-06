module Client.App

open Fable.Core
open Fable.Core.JsInterop

open Fable.Core
open Fable.Import
open Elmish
open Elmish.React
open Fable.Import.Browser
open Fable.PowerPack
open Elmish.Browser.Navigation
open Client.Messages
open Elmish.Browser.UrlParser
open Fable.Import.JS

// Model

type SubModel =
  | NoSubModel
  | WishListModel of WishList.Model
  | LoginAuthModel of LoginAuth.Model

type Model =
  { Page : Page
    Menu : Menu.Model
    SubModel : SubModel }

/// The URL is turned into a Result.
let pageParser : Parser<Page->_,_> =
    oneOf
        [ map Home (s "home")
          map WishList (s "wishlist")
          map Page.LoginAuth (s "login") ]

let urlUpdate (result:Page option) model =
    match result with
    | None ->
        //hack since we cannot parse UrlEncoded token via parser func. 
        match LoginAuth.isAccessToken() with 
        | Some token -> 
            ({model with Page = LoginAuth; SubModel = LoginAuth.TokenValidation token |> LoginAuthModel},[])
        | None ->
            Browser.console.error("Error parsing url")
            ( model, Navigation.modifyUrl (toHash model.Page) )

    | Some (Page.WishList as page) ->
        match model.Menu.User0 with
        | Some user ->
            let m,cmd = WishList.init user
            { model with Page = page; SubModel = WishListModel m }, Cmd.map WishListMsg cmd
        | None ->
            model, Cmd.ofMsg Logout0

    | Some (Home as page) ->
        { model with Page = page; Menu = { model.Menu with query = "" } }, []
    | Some (LoginAuth as page) ->
        { model with Page = page}, []

let init result =
    let menu,menuCmd = Menu.init()
    let m =
        { Page = Home
          Menu = menu
          SubModel = NoSubModel }

    let m,cmd = urlUpdate result m
    m,Cmd.batch[cmd; menuCmd]

let update msg model =
    match msg, model.SubModel with
    | StorageFailure e, _ ->
        printfn "Unable to access local storage: %A" e
        model, []

    | WishListMsg msg, WishListModel m ->
        let m,cmd = WishList.update msg m
        let cmd = Cmd.map WishListMsg cmd
        { model with
            SubModel = WishListModel m }, cmd

    | WishListMsg msg, _ -> model, Cmd.none

    | AppMsg.ShowLogin, _ ->
        {model with
            Page = Page.LoginAuth
            SubModel = LoginAuth.Model.Login |> LoginAuthModel },
        Cmd.none
    | AppMsg.LoggedOut0, _ ->
        { model with
            Page = Page.Home
            SubModel = NoSubModel
            Menu = { model.Menu with User0 = None }},
        Navigation.newUrl (toHash Page.Home)
    | AppMsg.Logout0, _ ->
        model, Cmd.ofFunc Utils.delete "user0" (fun _ -> LoggedOut0) StorageFailure
    | AppMsg.ProfileLoaded profile, _ ->
        {model with
            Menu = { model.Menu with User0 = Some profile
            }}, 
        Cmd.ofFunc (Utils.save "user0") profile (fun _ -> LoggedIn0) StorageFailure
    | AppMsg.LoggedIn0, _ ->
        let nextPage = Page.WishList
        let m,cmd = urlUpdate (Some nextPage) model
        match m.Menu.User0 with
        | Some user ->
            m, Cmd.batch [cmd; Navigation.newUrl (toHash nextPage) ]
        | None ->
            m, Cmd.ofMsg Logout0

// VIEW

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.Style

/// Constructs the view for a page given the model and dispatcher.
let viewPage model dispatch =
    match model.Page with
    | Page.Home ->
        [ words 60 "Welcome!"
          a [ Href "http://fable.io" ] [ words 20 "Learn Fable at fable.io" ] ]

    | Page.WishList ->
        match model.SubModel with
        | WishListModel m ->
            [ div [ ] [ lazyView2 WishList.view m dispatch ]]
        | _ -> [ ]
    | Page.LoginAuth ->
        match model.SubModel with 
        | LoginAuthModel model ->
            [ div [] [ LoginAuth.view model dispatch ]]
        | _ -> [ ]
    
/// Constructs the view for the application given the model.
let view model dispatch =
  div []
    [ lazyView2 Menu.view model.Menu dispatch
      hr []
      div [ centerStyle "column" ] (viewPage model dispatch)
    ]

open Elmish.React
open Elmish.Debug

// App
Program.mkProgram init update view
|> Program.toNavigable (parseHash pageParser) urlUpdate
|> Program.withConsoleTrace
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run