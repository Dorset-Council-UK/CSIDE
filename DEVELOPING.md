## Dependencies
To run CSIDE with minimal modification, you will need.
- A web server capable of running .NET applications, such as IIS, Kestrel or Azure (locally IIS Express or Kestrel should be fine). Linux servers can use Kestrel via an Nginx proxy.
- PostgreSQL 13+ with PostGIS extension

## Detailed setup instructions
First, clone the repository - the `main` branch should always be the latest, stable version.

### Setting up a database
To run the project locally you will need to create a suitable database. Postgres is the only database provider currently set up, but with minimal adjustments you could make it use any Entity Framework compatible provider. Install the PostGIS extension to enable geometries in Postgres. 

In your database, create a user for Entity Framework to use and give it permission to login. 

> You can either install the PostGIS extension now, or let Entity Framework do it for you. If you let Entity Framework do it for you, your user will need to (temporarily) have superuser permission in order to install extensions.

You can either let Entity Framework create your schema for you, or create it yourself. 
#### Create the schema yourself
Create a schema within your database called cside. To avoid complex permissions, make the user account you created the owner of the schema.

#### Let EF create the schema for you
In order for EF to be able to create the schema, your user will need permission on the database to create schemas.

#### Using a different schema name
If you want to use a different schema name, you can change the app settings `CSIDE:Database:SchemaName`, however, if you generate an EF migration script, you will have to manually find and change the schema name in the generated SQL before running it against your database. You will also need to change the SearchPath in your connection string to include your schema.

### Adding the unmanaged Parish table
The 'parishes' entity is NOT managed by Entity Framework, but is required in foreign key relationships. By convention, the table should:
- Be named 'parishes'
- Have a column called 'name' which contains the parish or community name
- Have a column called 'admin_unit_id' which contains the unique, integer based ID. This should be the primary key of the table.
- Have a column called 'geom' of type MultiPolygon in EPSG:27700

This basic structure should fit with OS Boundary-Line Parish data. You can use the following SQL to create a suitable table in your database.

```
CREATE TABLE IF NOT EXISTS cside.parishes
(
    name text COLLATE pg_catalog.default,
    admin_unit_id integer NOT NULL,
    geom geometry(MultiPolygon,27700),
    CONSTRAINT parishes_pkey PRIMARY KEY (admin_unit_id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS cside.parishes
    OWNER to "<your-userame-here>";

CREATE INDEX IF NOT EXISTS parishes_geom_sx
    ON cside.parishes USING gist
    (geom)
    TABLESPACE pg_default;
```

Additional columns can be included if this makes importing/referencing your data simpler, but they will not be used by CSIDE.

You will then need to import your Parish data from your data source.

### User secrets and connection strings
Once you’ve set up your database, add a user secret to your local copy of the application. In Visual Studio this is done by right-clicking on the web project and selecting “Manage user secrets”.

In user secrets add the below line of code and fill in your database name, username and password:

`{
"ConnectionStrings:CSIDE": "Host=localhost;Database=YourDatabaseName;Username=YourUserName;Password=YourPassword;SearchPath=cside,public"
}`

You should now be able to test your connection and run migrations using Entity Framework commands.

You can choose to use appSettings or Environment Variables to store your secrets if you prefer. Azure KeyVault is also available with a few configuration settings, and is recommended for production scenarios. appSettings is NOT recommended for storing secret information such as connection strings.

### Setting up authentication
Currently this app only supports Microsoft Azure AD B2C authentication. Refer to the official documentation to set this up in your Azure instance.

Once you've got a service set up, you'll need to add a number of settings to your user secrets to enable it.

```
"CSIDE": {
	"AzureAd": {
	  "Instance": "https://<your-instance-name>.b2clogin.com",
	  "Domain": "<your-b2c-domain>",
	  "ClientId": "<your-client-id>",
	  "TenantId": "<your-tenant-id>",
	  "ClientSecret": "<your-client-secret>",
	  "SignedOutCallbackPath": "<your-signout-callback-path>",
	  "SignUpSignInPolicyId": "<your-sign-in-policy-id>"
	}
}
```

### Setting up Blob Storage
For media upload, you need an Azure Blob Storage account. Refer to the official documentation to set this up in your Azure instance.

Once you've got a blob storage service set up, you'll need to add the connection string and container name to your user secrets.

```
"CSIDE": {
	"AzureBlobStorage": {
	  "ConnectionString": "<your-connection-string>",
	  "ContainerName":  "<your-container-name>"
	},
}
```

### Setting up the mapping
To use the mapping, you should have
- An OS DataHub API Key
- A map server (such as GeoServer) that publishes your data

You then needs to add the details of these to your user secrets.

```
"CSIDE": {
    "Mapping": {
      "OSMapsAPIKey": "<YOUR_OS_MAPS_API_KEY>",
      "OSLicenceNumber": "<YOUR_OS_LICENCE_NUMBER>",
      "MapServerRoot": "https://<YOUR_MAP_SERVER_WMS_ROOT>",
      "RouteLayer": "<YOUR_RIGHTS_OF_WAY_LAYER_NAME>",
      "InfrastructureLayer": "<YOUR_INFRASTRUCTURE_LAYER_NAME>"
    }
  }
```

Your OS DataHub API Key needs access to the OS Maps API.

### Running the application
Once you’ve followed the steps above, you’re ready to run the application. 

- Run an npm install to download the client-side dependencies
    - In Visual Studio this can be done by installing the NPM Task Runner Extension and using Task Runner Explorer
    - Alternatively just use the command line
- Build the client-side scripts
    - In Visual Studio this can be done by installing the NPM Task Runner Extension and using Task Runner Explorer to run the `watch` task
    - Alternatively just use the command line `npm run watch`
	- From now on, when opening the project in Visual Studio, `watch` will automatically run
- Run the Entity Framework migrations
    - Using Visual Studio
        - `Update-Database -Project "CSIDE.Data" -StartupProject "CSIDE.Web"`
    - Using .Net CLI
        - `dotnet ef database update --project "CSIDE.Data" --startup-project "CSIDE.Web"`
- Build and Run!

This will give you a minimal starting application, but there are a couple more steps to take before it will be usable.

### Giving yourself permission
When the app runs, you should be able to login, but you will be unable to do anything. You need to give yourself an appropriate role in the application before you'll be able to do anything. 
Once logged in, click your name in the top right and choose 'My account'. Grab the 'User ID' GUID value from this page. Go to your database directly and find the table called 'ApplicationRoles'. This should have been automatically populated by Entity Framework with a few values. Take a note of the ID of the Adminstrator role, and the find the table called 'ApplicationUserRoles' and fill in the RoleId and UserId columns. Restart your app and you should now have permission.

### Adding required countryside data
To use various parts of the system, you will need to make sure some datasets are imported before using it. Use pgAdmin or other database tool to import or create your data.

- The `Parishes` table expects geospatial extents of your Parishes or Communities. This should ideally be a cut of OS BoundaryLine. 
- The `ParishCodes` table is a lookup between your `Parishes` table and any codes you might use to identify a Right of Way within a Parish.
- The `RouteLegalStatuses` table expects a simple list of the legal statuses that can be assigned to a Right of Way.
- The `RouteOperationalStatuses` table expects a simple list of the operational statuses that can be assigned to a Right of Way. At least one of these should have `IsClosed` set to true.
- The `RouteTypes` table expects a simple list of the different types of Right of Way you can have (Footpath, Bridleway etc.).
- The `Routes` table expects geospatial Rights of Way data. The geometry is expected to be of type MultiLineString.
- The `MaintenanceTeams` table expects geospatial extents of maintenance team areas. The geometry is expected to be of type Polygon.
- The `MaintenanceJobPriorities` table expects a simple list of your priority classifications. 
- The `MaintenanceJobStatuses` table expects a simple list of your status classifications.  At least one should have `IsComplete` set to true.
- The `ProblemTypes` table expects a simple list of the different types of problem (overgrown vegetation, damaged infrastrcture etc.) that a maintenance job can have.
- The `InfrastructureTypes` table expects a simple list of the different types of infrastructure (Gate, Fingerpost etc.) that can be recorded.
- The `ContactTypes` table expects a simple list of all the types of people or organisations that you want to have contact details for.
