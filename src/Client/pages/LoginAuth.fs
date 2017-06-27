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



// type IAuth0Lock = 
//     abstract SubProto: JsConstructor<obj>
// let a = IAuth0Lock.SubProto.Create(null)

// let auth0Js = importAll<obj> "auth0-js"
let auth0lock:obj = importDefault "auth0-lock/lib/index.js"
let a = (createNew auth0lock) $("","")
// let constr  = createNew lock
// let l = createNew lock ("fable-test.eu.auth0.com","E0a7096d6yYIPYtVdDR1XEChjqsA7p4e")


// let getAuth0instance = createNew ( lock )
// let instance = getAuth0instance ("fable-test.eu.auth0.com","E0a7096d6yYIPYtVdDR1XEChjqsA7p4e")
let login() =
    console.log "huy"
    printfn "%O" auth0lock
    // printfn "%O" auth0Js
    // printfn "%O" constr?version
    // instance?show()