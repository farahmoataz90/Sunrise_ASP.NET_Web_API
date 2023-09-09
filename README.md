# Sunrise ASP.NET Web API

This repository contains the source code and documentation for the Sunrise ASP.NET Web API. Sunrise is a web-based application built with ASP.NET Web API and Microsoft SQL Server, providing various HTTP methods such as GET, POST, PUT, and DELETE for managing user data, products, providing search functionality, implementing pagination, and handling various other resources.

## Table of Contents
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Usage](#usage)
- [API Endpoints](#api-endpoints)
- [Database](#database)

## Getting Started

### Prerequisites

Before you begin, ensure you have met the following requirements:

- .NET SDK [link_to_download](https://dotnet.microsoft.com/download/dotnet)
- Microsoft SQL Server [link_to_download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)


### Installation

To get this project up and running, follow these steps:

1. Clone this repository:

   ```bash
   git clone https://github.com/farahmoataz90/Sunrise_ASP.NET_Web_API.git




 ## Usage
 To use the Sunrise ASP.NET Web API, you can make HTTP requests to the provided endpoints using your preferred API client or tool

## API Endpoints

The API provides the following endpoints:

- GET /api/items: Retrieve a list of items.
- GET /api/items/{id}: Retrieve a specific item by ID.
- POST /api/items: Create a new item.
- PUT /api/items/{id}: Update an existing item.
- DELETE /api/items/{id}: Delete an item by ID.


 ## Database

 The application uses Microsoft SQL Server as its database. The Entity Framework migrations have been set up to create and manage the database schema. You can find the database configuration in the appsettings.json file.

 
