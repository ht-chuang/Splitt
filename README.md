# Splitt
App to split the bill after dinner at a fancy restaurant made easy!

## Overview
This repo is split into three separate dotnet projects

### Splitt (MAUI app)
This is the frontend of the application. It should contain UI logic, pages, views, and models specific to the UI layer.

### SplittLib (Class Library)
Acts as the shared data access layer. This contains database model definitions as well as the DbContext which manages the connection to the database.

### SplittDB (Web API)
The backend for handling API requests. Reference the class library to access models and DbContext.
