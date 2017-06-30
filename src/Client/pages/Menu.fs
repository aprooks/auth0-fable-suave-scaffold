module Client.Menu

open Fable.Core
open Fable.Import
open Elmish
open Fable.Import.Browser
open Fable.PowerPack
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.Style
open Client.Messages
open System
open Fable.Core.JsInterop

type Model = {
    User0 : UserProfile option
    query : string
}

let init() = {  query = ""; User0 = Utils.load "user0" },Cmd.none

let view (model:Model) dispatch =
    div [ centerStyle "row" ] [ 
          yield viewLink Home "Home"
          if model.User0 <> None then 
              yield viewLink Page.WishList "Wishlist"
        //   else 
        //       yield buttonLink "logout" (fun _ -> dispatch Logout0) [ str "Logout" ]
          if model.User0 = None then
              yield buttonLink "login0" (fun _ -> dispatch ShowLogin) [str "Login0"]
          else
              yield buttonLink "logout" (fun _ -> dispatch Logout0) [ str <| sprintf "Logout [%s]" model.User0.Value.Name]
        ]