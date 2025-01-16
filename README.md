CSIDE - Countryside Management System
=============================

CSIDE is a [.NET Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) based Countryside Management System designed and developed by the Dorset Council GIS team.

CSIDE is used by Dorset Council to manage the Rights of Way network in Dorset and related information.

CSIDE is Open Source software and is licenced under the MIT licence.

## Features

CSIDE is a .NET Blazor web application for managing all aspects of Countryside Access including:

- Rights of Way
  - Create new paths using simple mapping tools
  - Update existing paths with new information and media
  - Mark paths as Closed and set closure start and end dates, with notifications to your staff about when paths should be reopening
  - View and edit the Legal Statement, with full history of all edits
  - Link paths to infrastructure items
- Maintenance Jobs
  - Create new maintenance jobs using a simple map and form interface
  - Automatically assign jobs to the relevant teams
  - View jobs submitted from the public
  - Update jobs with photos, videos and other media
  - Link jobs to infrastructure, so you can spot recurring issues
  - Automatically links jobs to the Right of Way it's recorded on
  - Add comments either internally or for the public
  - Mark jobs as duplicates and link to the canonical job
  - Keep track of the various contacts related to jobs, such as the reporter, landowner or local representative
  - Notify members of the public about the state of a job
- Infrastructure
  - Create new infrastructure using a simple map and form interface
  - Update existing infrastructure with detailed information
  - Add photos, videos and other media
  - Automatically links to the Right of Way it's on
  - Start and record surveys for infrastructure items
- DMMOs, Landowner Deposits and PPOs
  - Create DMMOs, Landowner Deposits or PPOs using simple map and form interfaces
  - Keep track of the status of all your legal events and consultations
  - Generate lists of affected addresses
  - Add documents and supporting evidence
- User management
  - Uses Azure AD B2C for user authentication
  - Users can be assigned roles and assigned to maintenance teams
  - Super users can be given permission to assign roles and maintenance teams to others
- Audit logs keep track of changes to every part of the system
- Customisable to your own branding
- Mobile friendly
- Nothing for your users to install, they just need a web browser
- Uses modern tech, including .NET 9, Blazor, PostGIS, OpenLayers, Azure Storage and Azure AD B2C

## Bugs
Please use the issue tracker for all bugs and feature requests. Before creating a new issue, do a quick search to see if the problem has been reported already.

Dorset Council staff should submit issues via the internal help desk.

## Dependencies

To run CSIDE with minimal modification, you will need:
- A web server capable of running .NET applications, such as IIS, Kestrel or Azure
- PostgreSQL 13+ with PostGIS extension
- An Azure subscription with access to Azure Blob Storage
- An Azure AD B2C Tenant
- Gov.UK Notify
- An OS DataHub subscription

CSIDE uses [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/). Postgres has been set up for this project, but with some modifications, any Entity Framework Core compatible provider should work. For a full list of providers, check the [Entity Framework docs](https://docs.microsoft.com/en-us/ef/core/providers/?tabs=dotnet-core-cli).

## Contributing

Please see our guide on [contributing](CONTRIBUTING.md) if you're interested in getting involved.

## Reporting security issues

Security issues should be reported privately, via email, to the Dorset Council GIS team gis@dorsetcouncil.gov.uk. You should receive a response within 24 hours during business days.

## Core developers

CSIDE is a Dorset Council Open Source project.

- [Paul Wittle](https://github.com/paul-dorsetcouncil) - Dorset Council
- [Rob Quincey](https://github.com/RobQuincey-DC) - Dorset Council
- [Lucy Bishop](https://github.com/VulpesFerrilata) - Dorset Council

## Alternatives

CSIDE is a Dorset Council project and has been built according to our particular needs. Whilst we believe the project can be easily used and adapted
by others, and is fairly flexible, there are other alternatives out there that may fit your needs better. We encourage you to do your own research, as a commercial solution may be more appropriate for your needs.

## Acknowledgements

CSIDE would not be possible without the open source community. This is just a small list of our favourite open source projects and organisations that have helped us:

- [OpenLayers](https://openlayers.org)
- [GeoServer](https://geoserver.org)
- [Bootstrap](https://getbootstrap.com)
- [.NET](https://dot.net)
- [Postgres](https://www.postgresql.org/)/[PostGIS](https://postgis.net/)
- [TypeScript](https://typescriptlang.org)
- [GeoSolutions](https://geosolutionsgroup.com)
- [OSGeo](https://www.osgeo.org) and [OSGeo:UK](https://uk.osgeo.org/)

## Licencing
Unless stated otherwise, the codebase is released under the MIT License. This covers both the codebase and any sample code in the documentation.

The documentation is © Dorset Council and available under the terms of the Open Government 3.0 licence.