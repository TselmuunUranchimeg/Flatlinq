# Controllers

Controllers that manage endpoints. Each controller serves all the necessary endpoints associated with its name.

## Keep in mind

-   Bad Request = Response with HTTP status set at 401
-   Ok = Response with HTTP status set at 200
-   If property is listed with `?`, it is an optional parameter and could be `null`.
-   When putting access token in the Authorization header, put it in this format. `"Bearer {accessToken}"`

## Authentication (Auth.cs)

By taking in associated data, the controller either logs in or sign in the user. One thing to note is regardless of the method, the controller will be using its own access token and refresh token.

### Register (Credentials)

After checking whether a user with the `Email`, the method will either create an access token and a refresh token and respond with an OK request, or respond with a Bad Request.
Refresh token will be stored in a HttpOnly cookie, "refreshToken", and the access token will be stored in the Authorization header.

```
Method = POST
Request body
URL = /Auth/Register
{
    Email: string,
    Password: string,
    Username: string
}

Response body is string.
```

### Login (Credentials)

After checking for user email, and validating password, the method will generate the two tokens. If something goes wrong (user with such email doesn't exist, password doesn't match), response will be a Bad Request.
Refresh token will be stored in a HttpOnly cookie, "refreshToken", and the access token will be stored in the Authorization header.

```
METHOD = POST
Request Body
URL = /Auth/Login
{
    Email: string,
    Password: string
}

```

### Verify

After validating user's refresh token, respond back with brand new refresh token and access token stored in the HttpOnly cookie, "refreshToken", and the "Authorization" header respectively.

```
METHOD = GET
URL = /Auth/Verify
```

### Google/Facebook

After validating the accessToken received on the front-end with the respective service, the server creates a new user and responds back with a refresh token (HttpOnly cookie) and an access token (Authorization).
Even for logging in, this route will be used.
Successful response = Ok
Otherwise = Bad request

```
METHOD = POST
URL = /Auth/Google or /Auth/Facebook
Request body
{
    accessToken: string
}
```

### Role

After analyzing the access token, assigns the requested role to the user and creates either a `Tenant` account or a `Landlord` account.
Successful response = Ok
Otherwise = Bad request

```
Require access token in Authorization
METHOD = POST
URL = /Auth/Role
Request body
{
    IsTenant: bool
}
```

## House (House.cs)

Controller associated with managing the `House` model.

### Create

Uploads the original and blurred images (JPEG images) of the file to AWS S3 after changing their names to GUID, and save the names and other data to the database.
Responds with Ok if successful, and BadRequest if unsuccessful.

```
METHOD = POST
URL = /House/Create
Request body
{
    Price: int,
    Name: string,
    Description: string,
    HasInternet: bool,
    HasElectricity: bool,
    AllowChildren: bool,
    AllowPets: bool,
    AllowSmoking: bool,
    Files: File[]
}
```

### {id}

Gets the House object with the same `id` from the database, and sends back the below listed data.

Images fiels represents an array of image links. Images are blurred if user haven't subscribed to gold.

```
Require access token in authorization
Only allow landlords
METHOD = GET
URL = /House/{id}
Response body
{
    Price: int,
    Name: string,
    Description: string,
    HasInternet: bool,
    HasElectricity: bool,
    AllowChildren: bool,
    AllowPets: bool,
    AllowSmoking: bool,
    Images: string[]
}
```

## Landlord (Landlord.cs)

Controller for managing sservices related to `Landlord`.

### Recommendation

By doing SQL JOIN twice, the recommendation algorithm generates an array of tenants, who haven't been swiped by the landlord. The endpoint returns a slice of the array starting from the `position` parameter in the query. The reason for slicing the array is to provide a seemless UI experience for the front-end (lazy-loading). 

```
Require access token in Authorization
Only allow landlords
METHOD = GET
URL = /Landlord/Recommendation
Request query
{
    position: int
}
```

## Swipe (Swipe.cs)

Controller for managing user swipes.

### Swipe

Stores a record in the `Swipes` table stating the user swiped another user, whose Id is provided in the request body. If `HouseId` is provided, the server assumes the user was a tenant and stores the swiped House for later use.
If the other user had already swiped the user, the server sends a message to two users using Firebase Cloud Messaging.

```
Require access token in Authorization
METHOD = POST
URL = /Swipe
Request body
{
    SwipedId: string
    HouseId: int?
}
```

## Tenant (Tenant.cs)

Controller for managing services related tenants.

### Recommendation

Fetches `House` records based on the parameters provided in the request query. Similar to `position` in `Recommendation` in [Landlord](#landlord-landlordcs), `position` here is also important in slicing the overall array and is necessary for the lazy-loading on the front-end. 

```
Requires access token in Authorization
METHOD = GET
URL = /Tenant/Recommendation
Request query
{
    hasElectricy: bool?,
    hasInternet: bool?,
    allowChildren: bool?,
    allowPets: bool?,
    allowSmoking: bool?,
    minPrice: int?,
    maxPrice: int?,
    position: int
}
```

## User (User.cs)

Controller for managing general services, which exist for both tenants and landlords.

### Subscribe

After testing the application, I have decided to change a few things, so for now this field will be temporarily empty. This section will be available some time later today or tomorrow at the latest.

### BankId

Based on the [documentation](https://www.bankid.com/en/utvecklare/guider/teknisk-integrationsguide/rp-anvaendarfall), the client will send their `endUserIp` and the server will respond with `AutoStartToken` and `OrderRef`. 
Furthermore, every 2 seconds, the server will check if BankId verification was successful and if it is, adjustments will be made to the user record in the database. 

```
Requires access token in Authorization
METHOD = POST
URL = /User/BankId
Request body
{
    endUserIp: string
}
Response body
{
    AutoStartToken: string
    OrderRef: string
}
```