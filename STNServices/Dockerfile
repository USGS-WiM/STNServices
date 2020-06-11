#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM microsoft/aspnetcore:2.0-stretch AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0-stretch AS build
WORKDIR /src
COPY ["STNServices/STNServices.csproj", "STNServices/"]
COPY ["STNDB/STNDB.csproj", "STNDB/"]
COPY ["STNServicesAgent/STNServicesAgent.csproj", "STNServicesAgent/"]
RUN dotnet restore "STNServices/STNServices.csproj"
COPY . .
WORKDIR "/src/STNServices"
RUN dotnet build "STNServices.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "STNServices.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "STNServices.dll"]