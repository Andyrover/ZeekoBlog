FROM microsoft/aspnetcore-build AS builder
WORKDIR /source
COPY ./ZeekoBlog/*.csproj ./ZeekoBlog/
COPY ./ZeekoBlog/*.json ./ZeekoBlog/
COPY ./Nuget.Config .
RUN cd ZeekoBlog && dotnet restore --configfile ../Nuget.Config && npm install
COPY . .
RUN cd ZeekoBlog && npm run build && dotnet publish -c Release -o /app/
FROM microsoft/aspnetcore:2.0.5
WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "ZeekoBlog.dll"]