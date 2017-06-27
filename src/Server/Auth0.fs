module ServerCode.Auth0

open Suave
open Suave.RequestErrors

let unauthorized s = Suave.Response.response HTTP_401 s

let UNAUTHORIZED s = unauthorized (UTF8.bytes s)


