FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /source
ADD BitcoinPrivateKeyCycler.Service/ /source

RUN dotnet restore && \
    dotnet publish --configuration Release --output /app --no-restore


FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY --from=build /app .
RUN mkdir /settings
ENTRYPOINT ["dotnet", "BitcoinPrivateKeyCycler.Service.dll"]
# CMD ["dotnet", "BitcoinPkCreatorWorker.dll"]