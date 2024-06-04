FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /EventBusAPI

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

EXPOSE 7212

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /EventBusAPI
COPY --from=build-env /EventBusAPI/out .
ENTRYPOINT ["dotnet", "EventBusAPI.dll"]