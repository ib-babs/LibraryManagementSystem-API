# LibraryManagementSystem

Library Management System (LMS) API provides a comprehensive platform for managing books and user sessions. The API utilizes Json Web Token (JWT) for secure authentication and authorization.

## Table of Contents

- [Features](#features)
- [Technologies](#technologies)
- [Setup](#setup)
- [Usage](#usage)
- [API Endpoints](#api-endpoints)
- [Examples](#examples)


```sh
dotnet ef migrations add InitialCreate
dotnet ef database update
```


## Features

- Manage books (add, update, delete, and retrieve books)
- Manage borrowed and returned books
- User registration and login
- JWT-based authentication and authorization
- Swagger API documentation

## Technologies

- .NET 8.0
- ASP.NET Core
- Entity Framework Core
- Microsoft SQL Server
- JWT (Json Web Token)
- Swagger

## Setup

1. Clone the repository:
    ```sh
    git clone https://github.com/yourusername/LibraryManagementSystem.git
    cd LibraryManagementSystem
    ```

2. Set up the database connection string in [appsettings.json](http://_vscodecontentref_/0):
    ```json
    "ConnectionStrings": {
        "LibraryDb": "Server=(localdb)\\mssqlLocalDB; Database=Library; MultipleActiveResultSets=True"
    }
    ```

3. Set the `SECRET_KEY` environment variable for JWT:
    ```sh
    export SECRET_KEY="your_secret_key"
    ```

4. Run the application:
    ```sh
    dotnet run
    ```

## Usage

- Access the Swagger UI for API documentation and testing at `https://localhost:7164/swagger`.

## API Endpoints

### Book Endpoints

- `GET /api/Book/{id}` - Get a book by ID
- `GET /api/Book` - Get all books
- `GET /api/Book/grouped_by_author` - Get books grouped by author
- `POST /api/Book` - Add a new book (Admin only)
- `DELETE /api/Book/{id}` - Delete a book by ID (Admin only)
- `PUT /api/Book/{id}` - Update a book by ID (Admin only)

### UserSession Endpoints

- `POST /api/UserSession/register` - Register a new user
- `POST /api/UserSession/login` - User login
- `GET /api/UserSession` - Get all users (Admin only)

### BorrowedBook Endpoints

- `GET /api/BorrowedBook` - Get all borrowed books (Authorized)
- `GET /api/BorrowedBook/{id}` - Get a borrowed book by ID (Authorized)
- `GET /api/BorrowedBook/top3_most_borrowed_books` - Get top 3 most borrowed books (Authorized)
- `POST /api/BorrowedBook` - Borrow a book (Authorized)
- `DELETE /api/BorrowedBook/{id}` - Delete a borrowed book by ID (Authorized)
- `PUT /api/BorrowedBook/{id}` - Update a borrowed book by ID (Admin only)

### ReturnedBook Endpoints

- `GET /api/ReturnedBook` - Get all returned books (Authorized)
- `GET /api/ReturnedBook/{id}` - Get a returned book by ID (Authorized)
- `POST /api/ReturnedBook` - Return a book (Authorized)
- `DELETE /api/ReturnedBook/{id}` - Delete a returned book by ID (Authorized)
- `PUT /api/ReturnedBook/{id}` - Update a returned book by ID (Admin only)

## Examples

### Book Endpoints

- **Get a book by ID**
    ```sh
    GET https://localhost:7164/api/Book/27fea3a2-353c-4050-8595-0fcccf655c2f
    ```

- **Get all books**
    ```sh
    GET https://localhost:7164/api/Book
    ```

- **Add a new book**
    ```sh
    POST https://localhost:7164/api/Book
    {
        "title": "Once Upon A Time in Nigeria",
        "author": "Ibrahim Babatunde",
        "isbn": "947-00495932948",
        "copiesAvailable": 50
    }
    ```

### UserSession Endpoints

- **User login**
    ```sh
    POST https://localhost:7164/api/UserSession/login
    {
        "userName": "username",
        "email": "user.name@example.com",
        "password": "user_name"
    }
    ```

- **Register a new user**
    ```sh
    POST https://localhost:7164/api/UserSession/register?role=User
    {
        "userName": "username",
        "email": "user.name@example.com",
        "password": "user_name"
    }
    ```

