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
1. Go back VS2017 with project **nyt.core.tasks**
2. Right click on the Solution, choose **Manage NuGet Packages for Solution..** from context menu
3. Choose tab **Browse** then type **Swashbuckle.AspNetCore**, choose the found package
4. Check on the **nyt.core.tasks** in the right panel & click **Install**
5. Edit **Startup.ConfigureServices(IServiceCollection services)** function to configure Swagger for the project
```csharp
services.AddSwaggerGen(c => {
	c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info {
		Title = "TaskService API", Version = "v1"
	});
});
```
6. Edit **Startup.Configure(IApplicationBuilder app)** to change request pipeline
```csharp
app.UseMvcWithDefaultRoute();
app.UseSwagger();
app.UseSwaggerUI(c => {
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskService API v1");
});
```
7. Run & Debug project with **Swagger UI** by browse to url: http://taskservice.dev/swagger
8. Back to **Step 3 in Session 1** in VS2017 to change the **Project Url** to http://taskservice.dev/swagger

## Session 4: Build Swagger C# Client by AutoRest
1. Open command prompt to install **autorest** npm package: 
```bash
npm install -g autorest
```
2. Create new .NET Core Console project named: **nyt.core.tasks.client**
3. Open Browser and save Swagger file http://taskservice.dev/swagger/v1/swagger.json to project root
4. Run the autorest C# generation tool at project root
```bash
autorest -Namespace nyt.core.tasks.client -CodeGenerator CSharp -Modeler Swager -Input swagger.json -PackageName nyt.core.tasks.client
```
5. Install & restore NUGET package **Microsoft.Rest.ClientRuntime** for the project (refer to Step 2 to Step 4 in Session 3)
6. Edit **Program.Main()** to test generarted client code by calling ASP.NET Core APIs
```csharp
var taskServiceAPI = new TaskServiceAPI(new Uri("http://taskservice.dev"));
Console.WriteLine($"API Result: {taskServiceAPI.ApiValuesGet()}");
```
7. Debug both Client & Server in 2 difference VS2017 windows

## Session 5: Configure Entity Framework Core MySQL for ASP.NET Core APIs
1. Go back VS2017 with project **nyt.core.tasks**
2. [Scaffolding MySQL.EFCore](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql) by editing **nyt.core.tasks.csproj** project file in any Text/XML editor to add the following code
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
3. Rebuild the solution in VS2017
4. Create *blank/empty* local schema in MySQL Workbench named **taskdb**
5. Link Entity Framework Core to **taskdb** local MySQL database
```bash
cd <project-folder>
dotnet ef dbcontext scaffold "Server=localhost;User Id=root;Password=123456;Database=taskdb" "Pomelo.EntityFrameworkCore.MySql"
```
6. Create **DbContext** class at **Models\TaskContext.cs**
```csharp
namespace nyt.core.tasks.Models
{
    public partial class TaskContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseMySql(@"Server=localhost;User Id=root;Password=123456;Database=nyttask");
        }
    }
}
```
7. Edit **Startup.ConfigureServices()** to support newly created DbContext
```csharp
...
using Microsoft.EntityFrameworkCore;

namespace nyt.core.tasks
{
    public class Startup
    {
        ...
        public void ConfigureServices(IServiceCollection services)
        {
            ...
            services.AddDbContext<TaskContext>(options => options.UseMySql(Configuration.GetConnectionString("taskdb")));
        }
        ...
    }
}
```
8. Add new connection string to **appsettings.json**:
```json
"ConnectionStrings": {
	"taskdb": "Server=localhost;User Id=root;Password=123456;Database=taskdb"
}
```
10. Run command to initialize *Entity Framwork Core* **Code-first migration** to the project
```bash
cd <project-folder>
dotnet ef migrations add Initial
```
11. Create **task** object at **Models\task.cs**
```csharp
namespace nyt.core.tasks.Models
{
    public class task
    {
        [Key]
        public Guid id { get; set; }

        [Required, MaxLength(50)]
        public string code { get; set; }

        [Required, MaxLength(256)]
        public string title { get; set; }

        [MaxLength(Int32.MaxValue)]
        public string content { get; set; }

        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public Guid? creator { get; set; }
        public Guid? assignee { get; set; }
    }
}
```
12. Run command to create database migration for model changes then build the project
```bash
cd <project-folder>
dotnet ef migrations add AddTaskTable
```
13. Rebuild the project
14. Run update to apply the model changes to MySQL database
```bash
cd <project-folder>
dotnet ef database update
```
13. Open MySQL WorkBench to check newly created tables, we should see **task** table
14. Create new API Controller to support **Dependency Injection** for the new DbContext & add CRUD APIs to the MySQL
```csharp
[Route("api/[controller]")]
public class TasksController : Controller
{
    private Models.TaskContext context;

    // Dependency Injection will create & pass TaskContext to the object here
    public TasksController(Models.TaskContext context)
    {
        this.context = context;
    }

    // Query a list of tasks from MySQL
    [HttpGet]
    public Task<task[]> Get()
    {
        return this.context.tasks.OrderByDescending(o => o.updated).Take(10).ToArrayAsync();
    }

    // Query a task by id from MySQL
    [HttpGet("{id}")]
    public Task<task> Get(Guid id)
    {
        return this.context.tasks.Where(t => t.id == id).SingleAsync();
    }

    // Create new task then save to MySQL
    [HttpPost]
    public Task Post([FromBody]task task)
    {
        if (task.id == Guid.Empty)
            task.id = Guid.NewGuid();
        task.created = DateTime.Now;
        task.updated = DateTime.Now;
        this.context.tasks.Add(task);
        return this.context.SaveChangesAsync();
    }

    // Update a task by id then save to MySQL
    [HttpPut("{id}")]
    public Task Put(Guid id, [FromBody]task task)
    {
        task.id = id;
        task.updated = DateTime.Now;
        var entry = this.context.Entry(task);
        entry.State = EntityState.Modified;
        entry.Property(o => o.created).IsModified = false;
        entry.Property(o => o.creator).IsModified = false;
        return this.context.SaveChangesAsync();
    }

    // Delete a task by id from MySQL
    [HttpDelete("{id}")]
    public Task Delete(Guid id)
    {
        this.context.Entry(new task() { id = id }).State = EntityState.Deleted;
        return this.context.SaveChangesAsync();
    }
}
```

## Session 6: Build ASP.NET Core MVC Frontend with Typescript & NSwagStudio
1. Get the pre-built ASP.NET Core MVC template from: https://github.com/thanhptr/aspnet-core-typescript
2. Rename the Project to **nyt.core.web**
3. Use [NSwagStudio](https://github.com/NSwag/NSwag/wiki/NSwagStudio) to generate the code & save to **scripts\TaskServiceAPI.ts**:
- Input: **URL** to: http://taskservice.dev/swagger/v1/swagger.json
- Output: check to **TypeScript Client**
- Output\Settings for TypeScript: set Namespace to **TaskServiceAPI**, version to **2.0**, Template to **JQueryPromisses**, Promise Type to **Promise**
- Then click **Generate Outputs** and copy the Output to **TaskServiceAPI.ts** file
4. Edit **TaskServiceAPI.ts** to add ***export*** for default namespace
```typescript
export namespace TaskServiceAPI {
...
}
```
5. Edit **app.ts** to call Web APIs from Browser client by using generated TypeScript code **TaskServiceAPI.ts**
```typescript
import * as $ from "jquery"
import { TaskServiceAPI } from "./TaskServiceAPI"

let taskServiceClient = new TaskServiceAPI.Client("http://taskservice.dev");
taskClient.apiTasksGet().then(result => console.log("API result", result));
```
6. Build & Run then see Console log in **Google Chrome**, we will see an error message: *Access-Control-Allow-Origin*
7. Fix ServiceAPI "Access-Control-Allow-Origin": by coming back to **nyt.core.tasks** project in VS2017
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddCors();
        ...
    }
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        ...
        app.UseCors(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        ...
    }
}
```
8. Rebuild & run **nyt.core.tasks** project, then back to browser to test again!