# bookLibraryBackend

## Presentation

This is the backend of [bookLibraryProject](https://github.com/PhilippeLeopoldie/BooksLibraryProject)
using Entity framework code first approach, following the repository pattern, implementing CRUD operations and Swagger Documentations.

This backend is deployed on heroku platform using postgres as database on a Vercel platform

## Installation and Setup

1. **Clone the Backend Repository:**
    ```bash
    git clone https://github.com/PhilippeLeopoldie/bookLibraryBackend.git
    ```

2. **Navigate to the Backend Directory:**
    ```bash
    cd bookLibraryBackend
    ```

3. **Install Global Tools:**
    ```bash
    dotnet tool install -g dotnet-aspnet-codegenerator
    dotnet tool install -g dotnet-ef
    ```

4. **Install Dependencies:**
    ```bash
    dotnet restore
    ```

5. **Add Entity Framework Tools:**
    ```bash
    dotnet add package Microsoft.EntityFrameworkCore.Tools -version 9.0.0
    ```

6. **Add Entity Framework Design:**
    ```bash
    dotnet add package Microsoft.EntityFrameworkCore.Design -version 9.0.0
    ```

7. **Add CodeGeneration.Design (if needed):**
    ```bash
    dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design -version 9.0.0
    ```

8. **Add Database Provider (Choose one):**
    ```bash
    dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL -version 9.0.2
    ```

9. **Run the Backend Locally:**
    ```bash
    dotnet run
    ```
    - The backend should now be running at:
        - HTTP: [http://localhost:5281](http://localhost:5281)  
        - Swagger UI: [http://localhost:5281/swagger/index.html](http://localhost:5281/swagger/index.html)

## Technologies

  - .Net 9.0.0

  - C# 13

  - ASP.NET Core Web APIs
  
  - xUnit 2.9.2
  
  - Moq 4.20.72

  - API platform: Heroku

  - Database platform: Vercel

    - Database : postGres




Test Driven Development: using Mock library and  xUnit framework for [UnitTest](https://github.com/PhilippeLeopoldie/bookLibraryBackend/blob/main/LibraryBackend.Tests/)

 

## Deploying link

[readsphere.vercel.app](https://readsphere.vercel.app)

