#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM swr.cn-north-4.myhuaweicloud.com/mobiliya/dotnet-core-aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM swr.cn-north-4.myhuaweicloud.com/mobiliya/dotnet-core-sdk:2.2 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "DmsAPI.csproj"
RUN dotnet build "DmsAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DmsAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
#EXPOSE 8080
ENTRYPOINT ["dotnet", "DmsAPI.dll"]