FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["examspur/examspur.csproj", "examspur/"]
RUN dotnet restore "examspur/examspur.csproj"
COPY . .
WORKDIR "/src/examspur"
RUN dotnet build "examspur.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "examspur.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "examspur.dll"]