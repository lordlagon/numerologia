FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Numerologia.Core/ src/Numerologia.Core/
COPY pocs/poc-mudblazor/   pocs/poc-mudblazor/

RUN dotnet publish pocs/poc-mudblazor/poc-mudblazor.csproj \
    -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "poc-mudblazor.dll"]
