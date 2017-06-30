module Client.Messages

open System
open ServerCode.Domain

type UserProfile = 
    {
        AccessToken: JWT
        Name       : string
        Email      : string
        Picture    : string
        UserId     : string
    }

/// The different messages processed when interacting with the wish list
type WishListMsg =
  | LoadForUser of string
  | FetchedWishList of WishList
  | RemoveBook of Book
  | AddBook
  | TitleChanged of string
  | AuthorsChanged of string
  | LinkChanged of string
  | FetchError of exn

/// The different messages processed by the application
// Postifixed login messages with 0 to easier distinguis with old messages
type AppMsg = 
  | StorageFailure of exn
  | WishListMsg of WishListMsg
  | ShowLogin
  | Logout0
  | LoggedIn0
  | LoggedOut0
  | ProfileLoaded of UserProfile


/// The different pages of the application. If you add a new page, then add an entry here.
type Page = 
  | Home 
  | WishList
  | LoginAuth

let toHash =
  function
  | Home -> "#home"
  | WishList -> "#wishlist"
  | LoginAuth -> "#login"
  
