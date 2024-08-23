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

### 1. Create a Heroku App

- **Login to Heroku:**  
  Open your web browser and navigate to [Heroku](https://www.heroku.com/). Log in with your credentials.
- **Create a New Application:**  
  Once logged in, click on the "New" button in the dashboard and select "Create new app."
  - Enter a unique name for your application.
  - Choose the appropriate region.

### 2. Set Up Configuration Environment Variables

- **Add Environment Variables:**
  - Go to the "Settings" tab of your newly created application.
  - Click on "Reveal Config Vars" to view and add your environment variables.
  - **MongoDB:**
    - Add a `MongoDBSettings:ConnectionString` environment variable with your MongoDB connection string. This string should include your username, password, and database name.
    - Add a `MongoDBSettings:DatabaseName` environment variable to specify the name of the database.
    - **Note:** Ensure you have configured IP access for your MongoDB cluster to allow Heroku to connect.
  - **Redis:**
    - Add a `RedisSettings:ConnectionString` environment variable with your Redis connection string. This should include the host and port.
    - Add a `RedisSettings:Password` environment variable if your Redis instance is password-protected.
    - Add a `RedisSettings:SyncTimeOut` environment variable with the sync timeout value (in milliseconds).
    - **Note:** Configure a Redis cloud cluster if you haven't already, and ensure your connection settings are correct.

### 3. Set Up MongoDB on Heroku

- **MongoDB Configuration:**

  - **Cluster Creation:**  
    If you haven't already, create a MongoDB cluster using MongoDB Atlas or another cloud provider.
  - **Network Access:**  
    Add Heroku's IP range or set network access to allow connections from anywhere. This is essential to ensure your Heroku app can connect to the MongoDB cluster.
  - **Database Name:**  
    Make sure the database name you intend to use is correctly set in your environment variables.

- ### 3. Configure Redis on Heroku

- **Set Up a Redis Instance:**

  - Use a service like [Redis Cloud](https://redis.com/redis-enterprise-cloud/overview/) to create a Redis instance.
  - Obtain the Redis connection details including the connection string, port, and password.

- **Add Redis Connection Details to Heroku Config Vars:**

  - In the "Settings" tab of your Heroku app, add the following environment variables:
    ```plaintext
    Key: RedisSettings:ConnectionString
    Value: <your-redis-connection-string>
    ```
  - Add the sliding expiration and absolute expiration settings:

    ```plaintext
    Key: RedisSettings:SlidingExpiration
    Value: 12:00:00
    ```

    ```plaintext
    Key: RedisSettings:AbsoluteExpiration
    Value: 24:00:00
    ```

    ```plaintext
    Key: RedisSettings:SyncTimeOut
    Value: <your-sync-timeout-in-ms>
    ```

### 4. Configure the .Procfile

- Ensure that your `.Procfile` is correctly configured to start your .NET application on Heroku. For example:
  ```plaintext
  web: dotnet run --urls=http://0.0.0.0:${PORT} -c Release
  ```

### 5. Set Up Buildpack

- **Add the .NET Core Buildpack:**
  - Before deploying your .NET Core application, ensure the correct buildpack is set. Use the following command to set the buildpack to `jincod/dotnetcore`:
    ```bash
    heroku buildpacks:set jincod/dotnetcore
    ```

### 6. Allow Swagger in Production (Optional)

- **Enable Swagger in Production for API Documentation:**

  - To allow Swagger to run in production, you can modify your application to include the following in the `Program.cs` or `Startup.cs`:
    ```csharp
    if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API V1");
        });
    }
    ```
  - This ensures that Swagger is available in both development and production environments, providing good documentation for your API.

- **Trigger Swagger UI:**
  - After deployment, you can access the Swagger UI by navigating to the following URL:
    ```plaintext
    https://<your-app-name>.herokuapp.com/swagger/index.html
    ```
  - Replace `<your-app-name>` with the name of your Heroku application.

### 7. Deploy to Heroku

- **Push Your Code:**

  - Use Git to deploy your code to Heroku:

    ```set up a CI yaml file to sync up with heroku

    ```

- **Monitor Deployment:**
  - Watch the deployment logs in the Heroku dashboard or using the Heroku CLI to ensure the deployment is successful:
    ```bash
    heroku logs --tail --app yourapplicationname
    ```

### 8. Access Your Application

- **Open the Application:**
  - Once the deployment is successful, you can access your application using the URL provided by Heroku:
    ```plaintext
    https://<your-app-name>.herokuapp.com
    ```
  - Replace `<your-app-name>` with the name of your Heroku application.

### 9. Monitor Application Health and Logs

- **Check Application Logs:**

  - Use the Heroku CLI to monitor your application’s logs in real-time:
    ```bash
    heroku logs --tail --app yourapplicationname
    ```

- **Monitor Application Health:**
  - Use Heroku’s dashboard to check the health metrics of your application. Navigate to the “Metrics” tab in the Heroku dashboard for detailed insights.

### 10. Debugging and Troubleshooting

- **Check Config Variables:**

  - If you encounter issues, ensure that all the necessary environment variables are correctly set in the Heroku config vars.

- **Check Buildpacks:**

  - Ensure that the correct buildpacks are configured for your .NET Core application. You can verify this in the “Settings” tab under “Buildpacks” in your Heroku dashboard.

- **Check Network and Firewall Rules:**

  - Ensure that your MongoDB and Redis instances are accessible from Heroku by reviewing any network or firewall rules that might be in place.

- **Review Heroku Documentation for more details:**
  - For further troubleshooting and detailed guidance, refer to the [Heroku documentation](https://devcenter.heroku.com/).
