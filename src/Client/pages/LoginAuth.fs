module Client.LoginAuth
open Fable.Import
open Fable.Core.JsInterop
open Fable.Core
open Fable.Import.JS


let myConfig = 
    createObj [
        "domain" ==> "fable-test.eu.auth0.com"
        "clientID" ==> "E0a7096d6yYIPYtVdDR1XEChjqsA7p4e"
        "callbackUrl" ==> "http://localhost:8080/#home"
        "apiUrl" ==> "fable.example.com"
        "responseType" ==> "token id_token"
        "scope" ==> "openid profile"
    ]



let auth0lock:JsConstructor<string,string,obj> = importDefault "auth0-lock/lib/index.js"

let authInstance = auth0lock.Create("E0a7096d6yYIPYtVdDR1XEChjqsA7p4e","fable-test.eu.auth0.com")

let login() =
    printfn "%O" auth0lock
    authInstance?show()
