module ServerCode.Auth0

open Suave
open Suave.RequestErrors

let unauthorized s = Suave.Response.response HTTP_401 s

let UNAUTHORIZED s = unauthorized (UTF8.bytes s)

let useToken ctx f = async {
    match ctx.request.header "authorization" with
    | Choice1Of2 accesstoken when accesstoken.StartsWith "Bearer" ->
        let jwt = accesstoken.Replace("Bearer ","")
        match JwtToken.isValid jwt with
        | None        -> return! FORBIDDEN "Token is not valid" ctx
        | Some claims -> return! f claims
    | _ -> return! BAD_REQUEST "Request doesn't contain a JSON Web Token" ctx
}