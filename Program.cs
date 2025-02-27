using LibraryManagementSystem;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Repositories.Implementatios;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Services.Implementations;
using LibraryManagementSystem.Services.Implementatios;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("LibraryDB") ?? throw new InvalidOperationException("No connection string named 'LibraryDB'.");
var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? throw new InvalidOperationException("SECRET_KEY env is unavailable.");
// Add services to the container.
builder.Services.AddControllers();

// Connect to database
builder.Services.AddSqlServer<LibraryDbContext>(connectionString, q => q.EnableRetryOnFailure(5));

// Add identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<LibraryDbContext>();
builder.Services.AddScoped<IUserSessionService, UserSessionService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IReturnedBookService, ReturnedBookService>();
builder.Services.AddScoped<IReturnedBookRepository, ReturnedBookRepository>();
builder.Services.AddScoped<IBorrowedBookService, BorrowedBookService>();
builder.Services.AddScoped<IBorrowedBookRepository, BorrowedBookRepository>();

// Configure identity requirement
builder.Services.Configure<IdentityOptions>(config =>
{
    config.User.RequireUniqueEmail = true;
    config.Password.RequireNonAlphanumeric = false;
    config.Password.RequireDigit = false;
});

// Configure authorization with jwt
builder.Services.AddAuthentication(a =>
{
    a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(jwt =>
{
    // Validate JWT parameter
    jwt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
        ValidAudience= System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(sg =>
{
    // Add swagger API documentation
    sg.SwaggerDoc("v1", new OpenApiInfo() { Title = "Library Management System", Version = "v1", Description = @"Library Management System (LMS) API provides a comprehensive platform for managing books and user sessions. The API utilizes Json Web Token(JWT) for secure authentication and authroization.

    AVAILABLE ENDPOINTS

    Book:
        GET /book/:id                       - Get a book with id
        GET /book                           - Get all books available
        GET /book/grouped_by_authors        - Get all books grouped by the author
        POST /book                          - Add a new book. A user with 'User' role will be denied
        DELETE /book/:id                    - Delete a book with id. Only admin 
        PUT /book/:id                       - Update a book. Admin only

    UserSession:
        POST /register                      - Register a new user
        POST /login                         - Log the registered user in to generate auth token for authorized endpoints
        GET /all_users                      - Get all users registered on the app. Admin only.
    
    BorrowedBook (Authorized):
        GET /borrowed_books                 - A list of books that are borrowed. User and Admin only.
        GET /borrowed_book/:id              - Get a borrowed book. User and Admin only
        GET /top3_most_borrowed_books       - Top 3 most borrowed books. User and Admin only
        POST /borrowed_book                 - Borrow a book. User and Admin only.
        DELETE /borrowed_book/:id           - Delete a borrowed book with id. Admin and User only
        PUT /borrowed_book/:id              - Update a borrowed book with id. Admin only.

    ReturnedBook (Authorized):
        GET /returned_book                  - A list of books that are returned. User and Admin only
        GET /returned_book/:id              - Get a returned book. User and Admin only
        POST /returned_book                 - Return a book. Admin and User only
        DELETE /returned_book/:id           - Delete a book that has already been returned. Admin and User only
        PUT /returned_book/:id              - Update an information about a returned book. Admin only
        




    EXAMPLES

    Book Endpoints:
    ```
    Request URL: GET https://localhost:7164/api/Book/27fea3a2-353c-4050-8595-0fcccf655c2f - Get a book

    Response: {
                    ""id"": ""27fea3a2-353c-4050-8595-0fcccf655c2f"",
                    ""title"": ""Things Fall Apart"",
                    ""author"": ""Chinua Achebe"",
                    ""isbn"": ""978-0385474542"",
                    ""copiesAvailable"": 35,
                    ""borrowCount"": 1,
                    ""borrowedBooks"": null
                  }
        
    ```
        
    ```
    Request URL: GET https://localhost:7164/api/Book - Get all books
    
    Response: [
                      {
                        ""id"": ""27fea3a2-353c-4050-8595-0fcccf655c2f"",
                        ""title"": ""Things Fall Apart"",
                        ""author"": ""Chinua Achebe"",
                        ""isbn"": ""978-0385474542"",
                        ""copiesAvailable"": 35,
                        ""borrowCount"": 1,
                        ""borrowedBooks"": null
                      },
                      {
                        ""id"": ""35baea7e-2e95-4acd-8408-ec8a254bfa2c"",
                        ""title"": ""An Orchestra of Minorities"",
                        ""author"": ""Chigozi Obioma"",
                        ""isbn"": ""978-0316412393"",
                        ""copiesAvailable"": 16,
                        ""borrowCount"": 0,
                        ""borrowedBooks"": null
                      },
                      {
                        ""id"": ""7e9366fa-a28f-476f-bb69-a1e1c344d605"",
                        ""title"": ""Stay with Me"",
                        ""author"": ""Ayobami Adebayo"",
                        ""isbn"": ""978-0708899207"",
                        ""copiesAvailable"": 13,
                        ""borrowCount"": 0,
                        ""borrowedBooks"": null
                      }
                   ]
    ```

    ```
    Request URL: GET https://localhost:7164/api/Book/grouped_by_author - Grouped books by author
    Response: {
                  ""Chigozi Obioma"": [
                    {
                      ""id"": ""35baea7e-2e95-4acd-8408-ec8a254bfa2c"",
                      ""title"": ""An Orchestra of Minorities"",
                      ""author"": ""Chigozi Obioma"",
                      ""isbn"": ""978-0316412393"",
                      ""copiesAvailable"": 16,
                      ""borrowCount"": 0,
                      ""borrowedBooks"": null
                    }
                  ],
                  ""Chimamanda Ngozi Adichie"": [
                    {
                      ""id"": ""c9011ee2-72ea-436b-827c-0285e3ae9642"",
                      ""title"": ""Purple Hibiscus"",
                      ""author"": ""Chimamanda Ngozi Adichie"",
                      ""isbn"": ""978-1400033412"",
                      ""copiesAvailable"": 10,
                      ""borrowCount"": 0,
                      ""borrowedBooks"": null
                    },
                    {
                      ""id"": ""f92cae4d-72a3-4f3d-a27d-d1d355228462"",
                      ""title"": ""Half a Yellow Sun"",
                      ""author"": ""Chimamanda Ngozi Adichie"",
                      ""isbn"": ""978-0060798760"",
                      ""copiesAvailable"": 43,
                      ""borrowCount"": 0,
                      ""borrowedBooks"": null
                    }
                  ]
                }
    ```

    ```
    Request URL: POST https://localhost:7164/api/Book
           request body: {
                              ""title"": ""Once Upon A Time in Nigeria"",
                              ""author"": ""Ibrahim Babatunde"",
                              ""isbn"": ""947-00495932948"",
                              ""copiesAvailable"": 50,
                              ""borrowCount"": 0,
                              ""borrowedBooks"":null
                          }
    Response: {
                  ""message"": ""New book is added.""
              }
    ```

    ```
    Request URL: DELETE https://localhost:7164/api/Book/o0320-494j9w-2wkkee
    Response: { ""message"": ""Successfully deleted"" }
    ```

    ```
    Request URL: PUT https://localhost:7164/api/Book/o0320-494j9w-2wkkee
    Response: { ""message"": ""Update successfully"" }
    ```

    USER SESSION
    
    ```
    Request URL: POST https://localhost:7164/api/UserSession/login - User login
    request body: {
                      ""userName"": ""username"",
                      ""email"": ""user.name@example.com"",
                      ""password"": ""user_name""
                  }
    Response: (token) eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJsbXNfYWRtaW4iLCJlbWFpbtMTI0Ny00Nzk5LWE2YjQtODY0NWY3OWY1MDNmIiwiZXhwIjoxNzQwNjU1MjU3LCJpc3MiOiJMaWJyYXJ5TWFuYWdlbWVudFN5c3RlbSIsImF1ZCI6IkxpYnJhcnlNYW5hZ2VtZW50U3lzdGVtIn0.4CPQdNqknVJAZQMgPeBTygmNQvyg4vgpS4ptVc8Fwi8
    ```
    
    ```
    Request URL: POST https://localhost:7164/api/UserSession/register?role=User - Account registration
    request body: {
                      ""userName"": ""username"",
                      ""email"": ""user.name@example.com"",
                      ""password"": ""user_name""
                  }
    Response:{ ""message"": ""New account has been created."" }
    ```

    ``` 
    Request URL: GET https://localhost:7164/api/UserSession - Get All Users
    Response: [
                  {
                    ""id"": ""6b3b8fc3-a2d5-4e5b-a25d-3wjsj29edbf"",
                    ""userName"": ""lms_admin"",,
                    ""email"": ""lms.admin@example.com"",
                  },
                  {
                    ""id"": ""979a273e-307e-4a74-85d9-weii24"",
                    ""userName"": ""lms_user"",
                    ""email"": ""lms.user@example.com"",
                  }
               ]
    ```

    
    BORROW BOOK:
    
    ```
    Request URL: GET https://localhost:7164/api/BorrowedBook - Get all borrowed books
    Response: [
                      {
                        ""id"": ""27ba63fb-9603-4c61-aa91-2f8ca44715d8"",
                        ""bookId"": ""fa6b17b5-7251-41db-9f3b-9d0b11a7ae61"",
                        ""borrowerId"": ""979a273e-307e-4a74-85d9-711abd163d35"",
                        ""hasBeenReturned"": true,
                        ""borrowedAt"": ""2025-02-25T20:06:58.7223584"",
                        ""book"": null,
                        ""borrower"": null,
                        ""returnedBooks"": null
                      },
                      {
                        ""id"": ""e3321cdd-1455-4cb2-bd2e-b52493daf61a"",
                        ""bookId"": ""fa6b17b5-7251-41db-9f3b-9d0b11a7ae61"",
                        ""borrowerId"": ""979a273e-307e-4a74-85d9-711abd163d35"",
                        ""hasBeenReturned"": true,
                        ""borrowedAt"": ""2025-02-25T20:32:59.280686"",
                        ""book"": null,
                        ""borrower"": null,
                        ""returnedBooks"": null
                   }]
       
    ```

    ``` 
    Request URL: POST https://localhost:7164/api/BorrowedBook?borrowerId=3884nee-4rjrj-48488ue&bookId=094-rjffdd-9984ddf-5kjajkq-eo0 - Borrow a book
    request body: {
                      ""bookId"": ""094-rjffdd-9984ddf-5kjajkq-eo0"",
                      ""borrowerId"": ""3884nee-4rjrj-48488ue"",
                      ""hasBeenReturned"": true,
                      ""borrowedAt"": ""2025-02-26T12:54:09.701Z"",
                      ""book"": null,
                      ""borrower"":null,
                      ""returnedBooks"": null
                    }
    Response: { ""message"": ""You have borrowed a book"" }
    ```
    
    ```
    Request URL: GET https://localhost:7164/api/BorrowedBook/27ba63fb-9603-4c61-aa91-2f8ca44715d8 - Get a borrowed book 
    Response:  {
                    ""id"": """",
                    ""bookId"": ""fa6b17b5-7251-41db-9f3b-9d0b11a7ae61"",
                    ""borrowerId"": ""979a273e-307e-4a74-85d9-711abd163d35"",
                    ""hasBeenReturned"": true,
                    ""borrowedAt"": ""2025-02-25T20:06:58.7223584"",
                    ""book"": null,
                    ""borrower"": null,
                    ""returnedBooks"": null
                  }
    ```

    ```
    Request URL: DELETE https://localhost:7164/api/BorrowedBook/27ba63fb-9603-4c61-aa91-2f8ca44715d8 - Delete a borrowed book
    Response: { ""message"": ""Successfully removed."" }
    ```

    ```
    Request URL: GET https://localhost:7164/api/BorrowedBook/top3_most_borrowed_books - Get Top 3 most borrowed books
    Response: [
                  {
                    ""id"": ""fa6b17b5-7251-41db-9f3b-9d0b11a7ae61"",
                    ""title"": ""Lion and the Jewel"",
                    ""author"": ""Wole Soyinka"",
                    ""isbn"": ""978-0199537891"",
                    ""copiesAvailable"": 20,
                    ""borrowCount"": 3,
                    ""borrowedBooks"": null
                  },
                  {
                    ""id"": ""27fea3a2-353c-4050-8595-0fcccf655c2f"",
                    ""title"": ""Things Fall Apart"",
                    ""author"": ""Chinua Achebe"",
                    ""isbn"": ""978-0385474542"",
                    ""copiesAvailable"": 35,
                    ""borrowCount"": 1,
                    ""borrowedBooks"": null
                  }
              ]
    ```
    
    RETURNED BOOK ENDPOINTS: It is quite similar to BorrowedBook endpoint.
 
    ```
    Request URL: POST
https://localhost:7164/api/ReturnedBook?returnerId=6b3b8fc3-a2d5-4e5b-a25d-1f7d4629edbf&borrowedBookId=71c8c9f5-c914-4c12-ae0f-a3993fe27b7b - Return a book
    Request Body: {
                      ""borrowedBookId"": ""71c8c9f5-c914-4c12-ae0f-a3993fe27b7b"",
                      ""returnerId"": ""6b3b8fc3-a2d5-4e5b-a25d-1f7d4629edbf"",
                      ""borrowedBook"": null,
                      ""returner"":null
                    }
    Response: {
                  ""message"": ""The book has been returned. Thank you!""
               }
    ```

" });

    // Define JWT Auth Header in Swagger
    sg.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "Bearer eysjdjdjjii3..",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
    });

    // Configure Auth Header security
    sg.AddSecurityRequirement(new OpenApiSecurityRequirement() {
        { new OpenApiSecurityScheme{
            Reference = new OpenApiReference(){ Type = ReferenceType.SecurityScheme,Id = "Bearer"}, Name = "Bearer", Scheme = "oauth2", In = ParameterLocation.Header
        }, []}
    });


});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(sg => sg.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Management System"));
}

app.UseHttpsRedirection();

app.UseAuthorization();
if (args.Length > 0 && args.First() == "seed_book")
    await SeedBook.EnsurePopulationAsync(app);

app.MapControllers();

app.Run();
