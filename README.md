# NYT Training 2017: Microservices with ASP.NET Core/Linux toolset
## Preparations
- Windows 10 development machine
- Install [Visual Studio 2017](https://www.visualstudio.com/downloads/) Community Edition (ASP.NET Core)
- Install VS2017 Extension: Visual Studio Developer Command Prompt: [devCmd](https://marketplace.visualstudio.com/items?itemName=ShemeerNS.VisualStudioCommandPromptdevCmd)
- Install [MySQL](https://dev.mysql.com/downloads/windows/installer/5.7.html) Server & MySQL Workbench
```conf
username: root
password: 123456
```
- Install [Node.js](https://nodejs.org/en/) & npm
```bash
# check installed node.js version
node -v
# check installed npm version
npm -v
```
- Install [NSwagStudio](https://github.com/NSwag/NSwag/wiki/NSwagStudio)

## Session 1: Build Backend API
1. Create new **ASP.NET Core Web API** project in VS2017 Community Edition: **nyt.core.tasks**
2. Edit **Program.cs** to set default web server port to **5002**
```csharp
public static void Main(string[] args)
{
    var host = new WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseIISIntegration()
        .UseStartup<Startup>()
        .UseApplicationInsights()
        .UseUrls("http://localhost:5002") // <= default listen at port 5002
        .Build();

    host.Run();
}
```
3. Change the Debug profile:
- Right click on the Project **nyt.core.tasks** choose **Properties** from the context menu
- Open tab **Debug** then change working **Profile** to **nyt.core.tasks**
- Change **Launch** to **Project**
- Set **Launch Url** to http://localhost:5002/api/values
- Set **App Url** to http://localhost:5002/api/values
- From Debug toolbar, Change drop down button from **IIS Express** to **nyt.core.tasks**
4. To start the web application server by 1 of 2 options:
- Select Menu **Debug -> Start Without Debugging** to start the Web Server
- Start from Command Prompt
```bash
# set working dir to project root
cd <project-folder>
# start the dotnet project by following command
dotnet run
```

## Session 2: Configure NGINX Server 
1. Download nginx: http://nginx.org/download/nginx-1.12.0.zip
2. Download winsw: http://repo.jenkins-ci.org/releases/com/sun/winsw/winsw/2.1.0/
3. Extract both zip files to the same NGINX folder (C:\nginx)
4. Set Windows environment variable: NGINX_HOME=C:\nginx
5. Install the Service
```bash
winsw-1.19.1-bin.exe install
```
6. Create empty folder **C:\nginx\conf\sites-availabled**
7. Edit **C:\nginx\conf\nginx.conf**: to include all configuration files from sites-availabled/*.host
```conf
http {
...
	include       sites-availabled/*.host;
...
}
```
8. REMOVE all server configurations then we will have the following **C:\nginx\conf\nginx.conf** content
```conf
worker_processes  1;
events {
	worker_connections	1024;
}
http {
	include			mime.types;
	include			sites-availabled/*.host;
	default_type		application/octet-stream;
	sendfile                on;
	keepalive_timeout	65;
}
```
6. Create file **C:\nginx\conf\sites-availabled\taskservice.host**
```conf
server {
	listen 80;
	access_log logs/access_taskservice_dev.log;
	error_log logs/error_taskservice_dev.log;
	
	server_name taskservice.dev;
	
	location / {
		proxy_pass http://localhost:5002;
		proxy_set_header Host taskservice.dev;
	}
}
```
7. Map host C:\Windows\System32\drivers\etc\hosts
```conf
127.0.0.1 taskservice.dev
```
8. Start NGINX service in services.msc
9. Test & Debug VS2017 NGINX url: http://taskservice.dev/api/values
10. Back to **Step 3 in Session 1** in VS2017 to change the **Project Url** to http://taskservice.dev/api/values

## Session 3: Configure Swagger UI for ASP.NET Core APIs
1. Install & restore NUGET package **Swashbuckle.AspNetCore** to the ASP.NET Core APIs project
2. Edit **Startup.ConfigureServices(IServiceCollection services)** function to include
```csharp
services.AddSwaggerGen(c => {
	c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info {
		Title = "XXXService API", Version = "v1"
	});
});
```
3. Edit **Startup.Configure(IApplicationBuilder app)** to change request pipeline
```csharp
app.UseMvcWithDefaultRoute();
app.UseSwagger();
app.UseSwaggerUI(c => {
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "XXXService API v1");
});
```
4. Run & Debug **Swagger UI** url: http://taskservice.dev/swagger

## Session 4: Build Swagger C# Client
1. Create new .NET Core Console project named: **taskservice.dev.client**
2. Install **autorest** npm package: 
```bash
npm install -g autorest
```
3. Save Swagger file http://taskservice.dev/swagger/v1/swagger.json to project root
4. Run the autorest C# generation tool at project root
```bash
autorest -Namespace taskservice.dev.client -CodeGenerator CSharp -Modeler Swager -Input swagger.json -PackageName taskservice.dev.client
```
5. Install & restore NUGET package **Microsoft.Rest.ClientRuntime**
6. Edit **Program.Main()** to test generarted client code by calling ASP.NET Core APIs
```csharp
var client = new XXXServiceAPI(new Uri("http://taskservice.dev"));
Console.WriteLine($"API Result: {client.ApiValuesGet()}");
```
7. Debug both Client & Server

## Session 5: Configure Entity Framework Core MySQL for ASP.NET Core APIs
1. [Scaffolding MySQL.EFCore](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql) by editing **csproj** ASP.NET Core API project file
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
2. Restore NUGET for the solution in VS2017 & Build
3. Create *blank/empty* local schema in MySQL Workbench named **xxx**
4. Link Entity Framework Core to **xxx** local MySQL database
```bash
dotnet ef dbcontext scaffold "Server=localhost;User Id=root;Password=123456;Database=xxx" "Pomelo.EntityFrameworkCore.MySql"
```
5. Create **Models & DbContext** classes in VS2017
6. Edit **Startup.ConfigureServices()** to support newly created DbContext
```csharp
using Microsoft.EntityFrameworkCore;
services.AddDbContext<XXXContext>(options => options.UseMySql(Configuration.GetConnectionString("xxxdb")));
```
7. Edit **DbContext** class to ignore manual configuration at server run-time
```csharp
public partial class XXXContext : DbContext
{
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (!optionsBuilder.IsConfigured)
			optionsBuilder.UseMySql(@"Server=localhost;User Id=root;Password=123456;Database=xxx");
	}
}
```
8. Add new connection string to **appsettings.json**:
```json
"ConnectionStrings": {
	"xxxdb": "Server=localhost;User Id=root;Password=123456;Database=xxx"
}
```
9. Run command to initialize *Entity Framwork Core* **Code-first migration** to the project
```bash
dotnet ef migrations add Initial
```
10. Manually add model classes & DbSet then build the project
11. Run command to create a migration for model changes then build the project
```bash
dotnet ef migrations add <db_change_name>
```
12. Run update to apply the model changes to MySQL database
```bash
dotnet ef database update
```
13. Open MySQL WorkBench to check newly created tables
14. Edit API Controller to support **Dependency Injection** for the new DbContext
```csharp
public class XXXController : Controller
{
    private XXXContext context;

    public XXXController(XXXContext context)
    {
        this.context = context;
    }
}
```
15. Use EF Core DbContext in Web APIs code
```csharp
[HttpGet]
public async Task<xxx[]> Get()
{
    return await this.context.xxxs.ToArrayAsync();
}
```

## Session 6: Build ASP.NET Core MVC Frontend with Typescript & NSwagStudio
1. Template: https://github.com/thanhptr/aspnet-core-typescript
2. Rename the template to **ProjectName**
3. Use [NSwagStudio](https://github.com/NSwag/NSwag/wiki/NSwagStudio) to generate & save **XXXServiceAPI.ts** to __$project/scripts__ folder
4. Edit **XXXServiceAPI.ts** to add ***export*** for default namespace
```typescript
export namespace xxxAPI {
}
```
5. Edit **app.ts** to call Web APIs from Browser client by using generated TypeScript code **XXXServiceAPI.ts**
```typescript
let xxxClient = new XXXServiceAPI.Client("http://taskservice.dev");
xxxClient.apiXxxGet().then(result => console.log("API result", result));
```
6. Build & Run, we will encouter error message from Browser at Web API calling code: Access-Control-Allow-Origin
7. Fix ServiceAPI "Access-Control-Allow-Origin": by coming back to ASP.NET Core APIs project in VS2017
```csharp
public class Startup
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddCors();
	}
	public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
	{
		app.UseCors(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
	}
}
```
9. Back to browser to test again!