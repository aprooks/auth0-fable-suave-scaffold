module ServerCode.JwtToken

open System.IdentityModel.Tokens.Jwt
open Microsoft.IdentityModel.Tokens
open Microsoft.IdentityModel.Protocols.OpenIdConnect

let exampleToken = """eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ik5EQTVRa1pGUlVFNU9UVTRNelV6UkVaRE1qSkdOREl4TlVKQ1FUQXhNekZDUlRFMVFURkJOUSJ9.eyJpc3MiOiJodHRwczovL2ZhYmxlLXRlc3QuZXUuYXV0aDAuY29tLyIsInN1YiI6IkUwYTcwOTZkNnlZSVBZdFZkRFIxWEVDaGpxc0E3cDRlQGNsaWVudHMiLCJhdWQiOiJmYWJsZS5leGFtcGxlLmNvbSIsImV4cCI6MTQ5ODYzOTE2NiwiaWF0IjoxNDk4NTUyNzY2LCJzY29wZSI6IiJ9.AEjhSpAA4W__19y-tj1ME4CEs5pCL259mmFbiWf9l30A0eM7XOrj3ZNp1eU59piKTNO4jC4qOW5a9ddyUCMQOaE8J2Np2oSyCcJPgjG9mFoOWXPpqSnt6pOFF70LSO8lQlrrVdOWhNGZMVVl_r_6r68PWXZFC_K2DKcDuSzGKelFJnoCLUNxD30FsfLqEyWSDf2J74sKZ_pN9hyTyQQhoaovCr_taRvwhQ-bM7qyY2wbslegpqvhR2CHeAl1dfrPdat_PkXDp4q8-hQU6dx-HOoHDvs8453Yu2tvtWuXuBR6wBVGwzmK9P7CIWn_QZxM3A_43SeW0QgPes_zy5B8Qg"""

open Microsoft.IdentityModel.Protocols
open Microsoft.IdentityModel.Protocols.OpenIdConnect

let private conf =
        new ConfigurationManager<OpenIdConnectConfiguration>(
            "https://fable-test.eu.auth0.com/.well-known/openid-configuration",
            OpenIdConnectConfigurationRetriever()
        )

let private config = conf.GetConfigurationAsync().Result

let private validationParams = 

    let par = Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    par.IssuerSigningKeys <- config.SigningKeys
    par.ValidAudience <- """fable.example.com"""
    par.ValidIssuer <- config.Issuer
    par

let private handler = JwtSecurityTokenHandler()

let isValid token = 
    try
        let jwtToken = JwtSecurityToken() :> Microsoft.IdentityModel.Tokens.SecurityToken |> ref
    
        handler.ValidateToken(token,validationParams,jwtToken)
        |> Some
    with 
    | _ -> None
    