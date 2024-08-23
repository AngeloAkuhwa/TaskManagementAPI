# Task Management Application

## Project Overview

This project is a Task Management Application built using React.js for the frontend and .NET Core with C# for the backend. The application uses MongoDB as its database and Redis for caching. The goal is to provide a fully functional task management system that mimics the provided UI design and supports CRUD operations for tasks and lists.

## Table of Contents

- [Requirements](#requirements)
  - [Frontend](#frontend)
  - [Backend](#backend)
- [Setup Instructions](#setup-instructions)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Running the Application](#running-the-application)
  - [Running Locally](#running-locally)
  - [Running on Heroku](#running-on-heroku)
- [API Documentation](#api-documentation)
  - [Tasks](#tasks)
  - [Lists](#lists)
  - [Groups](#groups)
- [Deployment](#deployment)
- [Evaluation Criteria](#evaluation-criteria)
- [Submission](#submission)

## Requirements

### Frontend

- **Framework:** React.js
- **UI Design:** The frontend replicates the provided UI closely.
  - Sidebar with sections for Private and Group.
  - Task list with add, edit, delete, and complete functionalities.
  - Form for adding new tasks with priorities and notes.
  - Navigation between different lists and groups.
- **API Consumption:** Consumes the backend APIs for task management.
- **State Management:** Uses React's context API for state management.

### Backend

- **Framework:** .NET Core with C#
- **Database:** MongoDB
  - Models for tasks, lists, and groups.
  - Fields for tasks: `id`, `title`, `description`, `status`, `priority`, `listId`, `groupId`, `assignedUsers`, `timestamps`.
- **API Endpoints:**
  - **Tasks:**
    - `GET /tasks`: Retrieve all tasks.
    - `GET /tasks/{id}`: Retrieve a specific task.
    - `POST /tasks`: Create a new task.
    - `PUT /tasks/{id}`: Update an existing task.
    - `DELETE /tasks/{id}`: Delete a task.
  - **Lists:**
    - `GET /lists`: Retrieve all lists.
    - `GET /lists/{id}`: Retrieve a specific list.
    - `POST /lists`: Create a new list.
    - `PUT /lists/{id}`: Update an existing list.
    - `DELETE /lists/{id}`: Delete a list.
  - **Groups:**
    - Similar endpoints as for lists.
- **Caching:**
  - Redis caching for frequently accessed data like lists and groups.
  - Cache invalidation strategies implemented for data updates or deletions.

## Setup Instructions

### Prerequisites

- **Node.js** and **npm** installed on your machine for the frontend.
- **.NET Core SDK 6.0** installed on your machine.
- **MongoDB** and **Redis** instances running locally or in the cloud.

### Installation

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/your-repo/task-management-app.git
   cd task-management-app
   ```
2. **Install Frontend Dependencies:**
   cd frontend
   npm install
3. **Install Backend Dependencies:**
   cd ../backend
   dotnet restore

## Running on Heroku

**Create a Heroku App:** - **Login to Heroku in the web environment:** - create you heroku application - Set Environment Variables: - Set your MongoDB, Redis, and other environment variables using the Heroku CLI or Heroku dashboard. - Ensure your .Procfile is correctly configured and push your code to Heroku
