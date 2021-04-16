FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS restore
WORKDIR /src
COPY ["sigreh.csproj", "./"]
RUN dotnet restore "sigreh.csproj"
COPY . .

FROM restore AS publish
RUN dotnet publish "sigreh.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "sigreh.dll"]
