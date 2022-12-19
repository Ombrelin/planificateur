# Planificateur

## Introduction

Simple and lightweight event scheduler : 

- Schedule events
- Mobile Friendly UI
- ReST API

## Tech Stack

Code : 

- C# 11 & .NET 7
- Typescript
- ASP .NET Core & ASP .NET Core MVC
- Entity Framwork Core
- PostgreSQL

Tooling : 

- Docker
- Playwright
- Powershell Core

## Run with Docker 

```yml
  planificateur:
      ports:
          - '5000:80'
      environment:
          - DB_HOST=postgres
          - DB_NAME=planificateur-test
          - DB_PASSWORD=password
          - DB_PORT=5432
          - DB_USERNAME=postgres
      image: ombrelin/planificateur
```

## Run from sources

Requirement : 

- .NET 7 SDK
- Node JS

```bash
cd src/Planificateur.Web
dotnet run
```
