# Creation of a REST API

### Technologies used:

- .NET 8
- C#
- Entity Framework Core with PostgreSQL
- Swagger
- Rebus
- 

### Services:

- Sales
- Product

### Configuration requirements to run the application:

1. Install PostgreSQL on your machine or on your Docker.
   ```bash
   docker run --name my-postgres -e POSTGRES_USER=myuser -e POSTGRES_PASSWORD=mypassword -e POSTGRES_DB=mydatabase -p 5432:5432 -d postgres
   ```

2. Download the repository from GitHub.

3. Update the connection string in `appsettings.json` to match your database.

5. Run the project.

### Features: 🛠️

Business Rules:

Discount:

Purchases of more than 4 identical items receive a 10% discount.
Purchases of 10 to 20 identical items receive a 20% discount.

Restrictions:

Maximum limit: It is not possible to sell more than 20 identical items.
No discount is allowed for quantities below 4 items.
