# Render has no native .NET runtime. Build a Linux image and run it.
# https://render.com/docs/docker
# https://render.com/docs/native-runtimes

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY TodoApi/TodoApi.csproj TodoApi/
RUN dotnet restore TodoApi/TodoApi.csproj
COPY TodoApi/ TodoApi/
RUN dotnet publish TodoApi/TodoApi.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS run
WORKDIR /app
COPY --from=build /app .
# Render forwards public HTTP to PORT (default 10000) on 0.0.0.0.
# Program.cs overrides this if PORT is set.
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "TodoApi.dll"]
