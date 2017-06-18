# NYT Training 2017: Microservices with ASP.NET Core/Linux toolset
## Preparations
- Windows 10 development machine
- Visual Studio 2017 Community Edition (ASP.NET Core): https://www.visualstudio.com/downloads/
- Install VS2017 Extension: Visual Studio Developer Command Prompt (devCmd): https://marketplace.visualstudio.com/items?itemName=ShemeerNS.VisualStudioCommandPromptdevCmd
- MySQL Community Server 5.7 & MySQL Workbench 6.3: https://dev.mysql.com/downloads/installer/
- Install Node.js & npm: https://nodejs.org/en/
```bash
	# check installed node.js version
	node -v
	# check installed npm version
	npm -v
```

## Session 1: Build Backend API with ASP.NET Core WebAPI
1) New ASP.NET Core API from VS2017 Community Edition
2) Program.UseUrls("http://localhost:5001") & switch to Launch Project
3) Debug http://localhost:5001/api/values
4) dotnet run

## Session 2: Configure NGINX Server 
1) Download nginx: http://nginx.org/download/nginx-1.12.0.zip
2) Setup environment variable: %NGINX_HOME%
3) Download winsw: http://repo.jenkins-ci.org/releases/com/sun/winsw/winsw/2.1.0/
4) Install Service: $winsw-1.19.1-bin.exe install
5) Edit nginx.conf: include sites-availabled/*.host; && REMOVE all server configurations {}
6) Configure sites-availabled/domain.name.host
```conf
	server {
		listen 80;
		access_log logs/access_domain_name.log;
		error_log logs/error_domain_name.log;

		server_name domain.name;

		location / {
			proxy_pass http://localhost:500x;
			proxy_set_header Host domain.name;
		}
	}
```
7) Map host C:\Windows\System32\drivers\etc\hosts
	127.0.0.1 domain.name 
8) Run $services.msc restart
9) Test http://domain.name/api/values
10) NGINX & DEBUG http://domain.name/api/values (from the Session 1)

## Session 3: 
1. Install & restore nuget Swashbuckle.AspNetCore
2. Startup.ConfigureServices
```csharp
	services.AddSwaggerGen(c => {
		c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "XXXService API", Version = "v1" });
	});
```
3. Startup.Configure request pipeline
```csharp
	app.UseMvcWithDefaultRoute();
	app.UseSwagger();
	app.UseSwaggerUI(c => {
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "XXXService API V1");
	});
```
4. DEBUG http://domain.name/swagger

## Session 4: Backend Swagger Client
1. Create .NET Core Console domain.name.client lib/app
2. Install autorest: npm install -g autorest
3. Save http://domain.name/swagger/v1/swagger.json
4. autorest -Namespace domain.name.client -CodeGenerator CSharp -Modeler Swager -Input swagger.json -PackageName domain.name.client
5. NUGET: Microsoft.Rest.ClientRuntime
6. Program.Main: api client calls
7. DEBUG Client & Server

## Session 5: Server MySQL
1. Install MySQL Server & Workbench: https://dev.mysql.com/downloads/windows/installer/5.7.html
2. Create empty schema from Workbench

## Session 6: Backend MySQL & Entity Framework Core
1. Scaffolding project: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql
```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="1.1.2" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="1.1.1" />
    <PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="1.1.2" />
    <PackageReference Include="Pomelo.EntityFrameworkCore.MySql.Design" Version="1.1.2" />
  </ItemGroup>
  <ItemGroup>
    <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="1.0.1" />
  </ItemGroup>
```
2. Restore NUGET & BUILD
3. dotnet ef dbcontext scaffold "Server=localhost;User Id=root;Password=123456;Database=xxx" "Pomelo.EntityFrameworkCore.MySql"
4. Create Models & Context
5. Startup.ConfigureServices
```csharp
	using Microsoft.EntityFrameworkCore;
	services.AddDbContext<XXXContext>(options => options.UseMySql(Configuration.GetConnectionString("xxxdb")));
	TaskContext.OnConfiguring => ignore
```
6. appsettings:
```json
	"ConnectionStrings": {
		"xxxxdb": "Server=localhost;User Id=root;Password=123456;Database=xxx"
	}
```
7. dotnet ef migrations add Initial
8. Add model class & DbSet + Build
9. dotnet ef migrations add CreateTaskTable
10. Build
11. dotnet ef database update
12. MySQL WorkBench to check
13. Test Query
14. DI for Context to Controller
15. Test Query with DI


## Session 7: Frontend with Typescript
1) Template: https://github.com/thanhptr/aspnet-core-typescript
2) Rename -> ProjectName
3) Startup.Configure(): app.UseMvcWithDefaultRoute()
4) tsconfig: "lib": ["dom", "es5", "es2015"]
5) NSwagStudio: generate API.ts
6) API.ts: export namespace
7) webpack.config: include API.ts
8) PostBuild: npm run build
9) client test & binding with Knockout
10) Fix ServiceAPI "Access-Control-Allow-Origin"
	Startup.ConfigureServices: services.AddCors()
	Startup.ConfigurePipeline: app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
11) Back to frontend to test