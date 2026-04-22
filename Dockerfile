# ─── Stage 1: build Blazor WASM ───────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS web-build
WORKDIR /src

COPY src/Numerologia.Core/Numerologia.Core.csproj             src/Numerologia.Core/
COPY src/Numerologia.Web/Numerologia.Web.csproj               src/Numerologia.Web/
RUN dotnet restore src/Numerologia.Web/Numerologia.Web.csproj

COPY src/Numerologia.Core/   src/Numerologia.Core/
COPY src/Numerologia.Web/    src/Numerologia.Web/
RUN dotnet publish src/Numerologia.Web/Numerologia.Web.csproj \
    -c Release -o /web-out --no-restore

# ─── Stage 2: build API ────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS api-build
WORKDIR /src

COPY src/Numerologia.Core/Numerologia.Core.csproj                       src/Numerologia.Core/
COPY src/Numerologia.Infrastructure/Numerologia.Infrastructure.csproj   src/Numerologia.Infrastructure/
COPY src/Numerologia.Api/Numerologia.Api.csproj                         src/Numerologia.Api/
RUN dotnet restore src/Numerologia.Api/Numerologia.Api.csproj

COPY src/Numerologia.Core/           src/Numerologia.Core/
COPY src/Numerologia.Infrastructure/ src/Numerologia.Infrastructure/
COPY src/Numerologia.Api/            src/Numerologia.Api/
RUN dotnet publish src/Numerologia.Api/Numerologia.Api.csproj \
    -c Release -o /api-out --no-restore

# ─── Stage 3: runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=api-build /api-out .
# Blazor WASM servido como static files pela API
COPY --from=web-build /web-out/wwwroot ./wwwroot

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Numerologia.Api.dll"]
