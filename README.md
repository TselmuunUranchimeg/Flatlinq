### Flatlinq (Server side)

ASP.NET Core Web Api project for a mobile app.

## Prerequisites

- Install .NET 7

## Set up

1. Get the necessary credentials from below services
- AWS (Set up S3, and connect it to Cloudfront)
- Create a Stripe account and create a subscription plan (Note down the Id)
- Write down your MySQL server address whether it is on localhost or a remote device

2. Fill in the blanks in appsettings.json with the credentials from Step 1

3. Run ```dotnet run``` from root directory

## Endpoints

All endpoints are available in ```Controllers``` directory, and the route will start with the file name. 
- Auth.cs -> In charge of authentication
- House.cs -> Create, get houses
- Landlord.cs -> Recommendation service for landlords
- Swipe.cs -> For swiping houses / tenants
- Tenant.cs -> Recommendation for tenants
- User.cs -> General functionalities both parties will enjoy, such as subscribing to Gold tier, and BankId verification