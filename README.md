# DGHA-Backend
This is the Repo for the DGHA project's backend, the other components are [here](https://github.com/leechuyem/dgha) and [here](https://github.com/Meandmy10/DGHA-Admin)

##Special Requirments
If you are planning on deploying this backend there are a few thing you need to do to get it running properly:
1. The ModelLibrary contains the `ApplicationDbContext`, this runs off Entity Framework, deatils on how to use/deploy/update the database  can be found [here](https://docs.microsoft.com/en-gb/ef/ef6/)
2. The connection string is storied in the `ApplicatoinDbContext`, be sure to update this to your databases connection string
3. If you are deploying to a database from strach be sure to add the `Administrator` role and a administrator account to the database, there is currently no automatic way to do this impimented here so you'll need to do it manually.
4. The Identity server and API both run on ASP.NET Core 3.0, details on how to deploy them can be found [here](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/?view=aspnetcore-3.0)

Also note that the system as been built to expect in a development enviroment it is running locally.

##Identity Server
Identity server is the authorization server for the project to get it working you need to make sure:
1. Update the `Config.cs` file so the redirect Urls for the `AdminPortal` client match the Admin Application Urls.
2. In a production enviroment update the clients in `Config.cs` so the secrets arn't just 'secret', ensure you update the Admin and Flutter Clients to match.
3. Update the `Startup.cs` file so the `apiUrl` and `adminUrl` variables in the `ConfigureServices` function match the API and Admin Portal urls, this is to set the CORS policies.
4. In a production enviroment replace `builder.AddDeveloperSigningCredential()` with acutal key material, for more information look [here](http://docs.identityserver.io/en/latest/topics/startup.html#key-material)

If you need more help you can find the Identity Server Documentation [here](http://docs.identityserver.io/en/latest/)

##API
To Ensure the API endpoints and authentication are working be sure to:
1. Update `Startup.cs` file so the `adminUrl` variable in the `ConfigureServices` function is the same as the deployed Admin Applicaion and `baseUrl` is the same as the identity server url
2. Update `Startup.cs` file so the `Authority` option for the `AddAuthentication` function is the same as the Admin Applicatoin url

At the base url of the API you can find the Swagger API Documentaiton, to see a deployed example of this go [here](https://dgha-api.azurewebsites.net/index.html)
