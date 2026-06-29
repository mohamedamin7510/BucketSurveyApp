# BucketSurvey API

A simple survey platform for creating polls, managing questions, collecting votes, and reviewing results.

## Architecture

The project is organized into clear layers:

- **API Layer**: handles HTTP requests and responses
- **Service Layer**: contains the main business logic
- **Data Layer**: manages database access and persistence
- **Authentication & Authorization**: protects routes with login, roles, and permissions
- **Background Jobs**: sends scheduled notifications and runs recurring tasks

## Main Features

- Create, update, and delete polls
- Add and manage questions
- Submit votes for polls
- View results and voting statistics
- User registration and login
- Email confirmation and password reset
- Google login support
- Role-based access control

## Routes

### Authentication
- `POST /Authentication` — login
- `POST /Authentication/register` — register new user
- `POST /Authentication/confirm-email` — confirm email
- `POST /Authentication/resend-confirm-email` — resend confirmation email
- `POST /Authentication/forget-password` — request password reset
- `PUT /Authentication/reset-password` — reset password
- `POST /Authentication/refresh` — refresh token
- `PUT /Authentication/revoke-refresh-token` — revoke refresh token
- `POST /Authentication/google` — login with Google

### Polls
- `GET /api/polls` — get all polls
- `GET /api/polls/current` — get current poll
- `GET /api/polls/{id}` — get a single poll
- `POST /api/polls` — create poll
- `PUT /api/polls/{id}` — update poll
- `DELETE /api/polls/{id}` — delete poll
- `PUT /api/polls/{id}/togglestatus` — publish or unpublish poll

### Questions
- `GET /api/polls/{pollid}/questions` — get all questions
- `GET /api/polls/{pollid}/questions/{id}` — get one question
- `POST /api/polls/{pollid}/questions` — add question
- `PUT /api/polls/{pollid}/questions/{id}` — update question
- `PUT /api/polls/{pollid}/questions/{id}/togglestatus` — enable or disable question

### Voting
- `GET /api/polls/{pollid}/vote` — get available voting questions
- `POST /api/polls/{pollid}/vote` — submit vote

### Results
- `GET /api/polls/{pollid}/results/raw-data` — view raw results
- `GET /api/polls/{pollid}/results/vote-per-day` — view votes per day
- `GET /api/polls/{pollid}/results/votes-per-question` — view votes per question

## Deployment

### Requirements
- .NET 9 Runtime
- SQL Server
- A web server or hosting platform
- Environment configuration for database, JWT, Hangfire, and email settings

### Before Deployment
Update `appsettings.json` or environment variables with:
- Database connection string
- JWT settings
- Hangfire credentials
- Allowed origins
- Email settings

### Publish
1. Open the project in Visual Studio
2. Build the solution
3. Publish the API using **Publish**
4. Choose a target such as:
   - Folder
   - IIS
   - Azure App Service
5. Copy the published files to the server

### Run Database Migration
Make sure the database is created and updated before starting the app.

### After Deployment
- Open `/health` to check service status
- Open `/openapi/v1.json` for API documentation
- Open `/jobs` to view background jobs if enabled

## Summary

BucketSurvey API is a secure and organized survey system for managing polls, collecting feedback, and analyzing results.
