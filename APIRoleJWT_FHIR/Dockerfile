# =========================
# Stage 1: Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# =========================
# Stage 2: Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and projects
COPY APIUsingJWT.sln .
COPY APIUsingJWT/APIUsingJWT.csproj APIUsingJWT/
COPY APIUsingJWT.Domain/APIUsingJWT.Domain.csproj APIUsingJWT.Domain/
COPY APIUsingJWT.Data/APIUsingJWT.Data.csproj APIUsingJWT.Data/
COPY APIUsingJWT.Services/APIUsingJWT.Services.csproj APIUsingJWT.Services/

# Restore dependencies
RUN dotnet restore APIRoleJWT_FHIR.sln

# Copy everything else
COPY . .

# Publish
RUN dotnet publish APIUsingJWT/APIUsingJWT.csproj -c Release -o /app/publish

# =========================
# Stage 3: Final
# =========================
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "APIUsingJWT.dll"]
