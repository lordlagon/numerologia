FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Numerologia.Core/        src/Numerologia.Core/
COPY pocs/poc-blazorbootstrap/    pocs/poc-blazorbootstrap/

RUN dotnet publish pocs/poc-blazorbootstrap/poc-blazorbootstrap.csproj \
    -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "poc-blazorbootstrap.dll"]
