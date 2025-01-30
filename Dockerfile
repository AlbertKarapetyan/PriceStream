# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy entire solution and restore
COPY . . 
RUN dotnet restore PriceStream.sln

# Build the solution in Release mode
RUN dotnet build PriceStream.sln -c Release -o /app/build

# Publish the project
RUN dotnet publish PriceStream.sln -c Release -o /app/out

# Use runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 5000
ENTRYPOINT ["dotnet", "PriceStream.dll"]