# 📚 BookLibrary API

## 🔗 Table of Contents
- [📌 Project Goals](#project-goals)
- [🏠 App Overview](#app-overview)
- [📐 Clean Architecture Overview](#clean-architecture-overview)
- [📋 Features](#features)
  - [📄 Book List & Details](#book-list--details)
  - [📝 CRUD Operations](#crud-operations)
  - [🔍 Search & Filtering](#search--filtering)
  - [📊 Ratings & Reviews](#ratings--reviews)
  - [🧠 AI Story Generator](#ai-story-generator)
  - [🔐 Authentication and Authorization](#authentication-and-authorization)
- [📡 API Routes Overview](#api-routes-overview)
- [🛠️ Technologies Used](#technologies-used)
- [🚀 Getting Started](#getting-started)

---

##  Project Goals
This is the backend of [bookLibraryProject](https://github.com/PhilippeLeopoldie/BooksLibraryProject).

The project aims to build a full-stack Book Library and personalized Short Stories application, enabling users to:

- Manage a collection of books with genres and user opinions
- Read descriptions of books
- Rate and review books
- Read and contribute opinions
- Generate creative stories based on books using [OpenAI](https://openai.com/) 
- Access book data through a clean, structured, and well-documented API

---

##  App Overview

The application consists of a **frontend in React (TypeScript)** and a **.NET backend** following **Clean Architecture principles**. 

- Backend: CRUD operations for books, genres, and opinions
- Frontend: View books, view/create opinions, and interact with the AI Story feature

> 🔧 Swagger is enabled for API documentation  
> 🛠️ Backend hosted on **Heroku**  
> 🗄️ Database hosted on **Eons**

---

##  Clean Architecture Overview

The solution is split into the following layers:

- 🧠 **BookLibrary.Core** — domain entities, DTOs, interfaces, and core logic
- ⚙️ **BookLibrary.Services** — business logic and service implementations
- 🗄️ **BookLibrary.Infrastructure** — database context and data access using Entity Framework Core
- 🌐 **BookLibrary.Presentation** — contains the controllers and route handling
- 🧪 **BookLibrary.Tests** — xUnit-based unit tests for services and controllers

---

##  Features

###  Book List & Details
- Retrieve all books
- Get book by ID
- Filter by title, author, or genre
- View top books

###  CRUD Operations
#### Backend:
- Create, Read, Update, Delete books
- Create, Read, Update, Delete genres
- Create, Read, Update opinions
- (Planned): Delete opinions

###  Search & Filtering
- Search books by title or author
- Filter by genre

###  Ratings & Reviews
- Submit opinions with ratings
- Get average rating for a specific book
- Fetch opinions related to a book

###  AI Story Generator
- Generate short stories based on book context using the OpenAI API

- ⚠️ Planned: More prompt customization and story saving

###  Authentication and Authorization
- Role field available in the registration form (to assign admin rights)
- ⚠️ Planned: Implement login system and secure admin/user routes

---

##  Technologies Used

- 🔷 **ASP.NET Core 8.0**
- 💬 **C#**
- 📦 **Entity Framework Core**
- 🗃️ **PostgreSQL** (hosted on Eons)
- 🌐 **Swagger / OpenAPI**
- ⚛️ **React + TypeScript** (frontend)
- 🔒 **JWT (Planned)** for authentication
- 🤖 **OpenAI API** for story generation
- 🧪 **xUnit** for backend unit tests

---

##  API Routes Overview

📘 Book
- GET /api/Books

- POST /api/Books

- GET /api/Books/{id}

- PUT /api/Books/{id}

- DELETE /api/Books/{id}

- GET /api/Books/TopBooks

- GET /api/Books/TitleOrAuthor

- GET /api/Books/genre

🏷️ Genre
- GET /api/Genres

- POST /api/Genres

- GET /api/Genres/{id}

- PUT /api/Genres/{id}

- DELETE /api/Genres/{id}

💬 Opinion
- GET /api/Opinions

- POST /api/Opinions

- GET /api/Opinions/BookId={bookId}

- GET /api/Opinions/averageRate

- PUT /api/Opinions/{id} (planned for frontend)

✍️ Story
- POST /api/Story/AI — AI-generated short story

##  Getting Started

1. **Clone the Repository**
   ```bash
   git clone https://github.com/PhilippeLeopoldie/BookLibrary.git
   cd BookLibrary
