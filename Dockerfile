FROM mcr.microsoft.com/dotnet/sdk:9.0 as build-image
WORKDIR /home/app
COPY ./*.sln ./
COPY ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/; done
RUN dotnet restore
COPY . .
RUN dotnet publish ./CompanyEmployees/CompanyEmployees.csproj -o /publish/
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /publish
# Copy the XML file into the publish folder
COPY ./CompanyEmployees.Infrastructure.Presentation/CompanyEmployees.Infrastructure.Presentation.xml /publish
COPY --from=build-image /publish .
ENV ASPNETCORE_URLS=https://+:7025;http://+:5266
#ENV ASPNETCORE_ENVIRONMENT=Development
#ENV SECRET=CodeMazeSecretKey123456789!!!!!!
ENTRYPOINT ["dotnet", "CompanyEmployees.dll"]
