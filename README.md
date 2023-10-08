# Flatlinq (Server side)

ASP.NET Core Web Api project for the renting .

## Prerequisites

- Install .NET 7

## Set up

- AWS
    -   Set up your AWS [S3](https://www.youtube.com/watch?v=eQAIojcArRY) and [Cloudfront](https://www.youtube.com/watch?v=kbI7kRWAU-w) using these 2 videos as reference
    -   Fill in the `"AWS"` section of `appsettings.json` with the correct details. 

- Stripe
    -   Create a Stripe account add your Stripe secret key to the `StripeApiKey` section of the `appsettings.json` file
    -   Create a subscription plan (either through the dashboard or the API) and add the id to `PlanId` section.

Feel free to customize the `JWT` and `DatabaseConnection` sections. 

## Start the application

1. Set up your credentials as instructed by the previous section.
2. Run `dotnet run` at the root directory. 

## Folders
* Controllers -> Endpoints that handle HTTP requests (Documentation available in the folder)
* Data -> Database context for Entity Framework Core to manipulate data in MySQL
* Hubs -> Websocket event handler to manage real time communication
* Migrations -> Migrations for the MySQL database
* Models -> Classes that represent the objects used in the server
    * DTO -> Classes that represent the Data Transfer Objects
* Services -> Classes that contain the business logic of endpoints