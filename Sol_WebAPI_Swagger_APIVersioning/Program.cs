using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Sol_WebAPI_Swagger_APIVersioning.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region API Version Configuration

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true; //This ensures if client doesn't specify an API version. The default version should be considered. 
    options.DefaultApiVersion = new ApiVersion(1, 0); //This is to set the default API version
    options.ReportApiVersions = true; //The allow the API Version information to be reported in the client  in the response header. This will be useful for the client to understand the version of the API they are interacting with.

    //------------------------------------------------//
    //This says how the API version should be read from the client's request, 3 options are enabled 1.Querystring, 2.Header, 3.MediaType. 
    //"api-version", "X-Version" and "ver" are parameter name to be set with version number in client before request the endpoints.
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("X-Version"),
        new MediaTypeApiVersionReader("ver")); 

}).AddApiExplorer(options => {
    options.GroupNameFormat = "'v'VVV"; //The say our format of our version number “‘v’major[.minor][-status]”
    options.SubstituteApiVersionInUrl = true; //This will help us to resolve the ambiguity when there is a routing conflict due to routing template one or more end points are same.
});

#endregion


#region Swagger Configuration

builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Base Template for ASP.Net Core Web API",
            Version = "v1",
            Description = "An Web API to validate swagger and Api versioning in Asp.Net Core 8",
            TermsOfService = new Uri("https://example.com/terms"),
            Contact = new OpenApiContact 
            { 
                Name = "Abdul",
                Email = "a@a.com",
                Url = new Uri("https://example.website.com/"),
            },
            License = new OpenApiLicense
            { 
                Name = "Web API License",
                Url = new Uri("https://example.com/license")
            }
        });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    s.IncludeXmlComments(xmlPath);
});

#endregion

#region Minimal API Configuration

builder.Services.AddDbContext<TodoDB>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

#endregion

var app = builder.Build();

#region Minimal API Configuration

MinimalAPIConfiguration(app);

#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    #region Swagger Middleware Configuration

    app.UseSwagger(); // Enable middleware to serve generated Swagger as a JSON endpoint.

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "My Web API v1"); // specifying the Swagger JSON endpoint.
        s.InjectStylesheet("/swagger/ui/custom.css");  // specifying the custom css for swagger ui
    });

    #endregion
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();

app.Run();

#region Minimal API Configuration
void MinimalAPIConfiguration(WebApplication app)
{
    app.MapGet("/", () => "Hello world!!!");

    #region Basic Setup for understanding

    //app.MapGet("/todoitems", async (TodoDB db) =>
    //{
    //    var todoItems = await db.Todos.ToListAsync();
    //    return Results.Ok(todoItems);
    //});

    //app.MapGet("/todoitems/complete", async (TodoDB db) =>
    //{
    //    var todoItems = await db.Todos.Where(t => t.IsComplete).ToListAsync();
    //    return Results.Ok(todoItems);
    //});

    //app.MapGet("/todoitems/{id}", async (int id, TodoDB db) =>
    //{
    //    return await db.Todos.FindAsync(id) is Todo todo ? Results.Ok(todo) : Results.NotFound();
    //});

    //app.MapPost("/todoitems", async (Todo todo, TodoDB db) =>
    //{
    //    db.Todos.Add(todo);
    //    await db.SaveChangesAsync();

    //    return Results.Created($"/todoitems/{todo.Id}", todo);
    //});

    //app.MapPut("/todoitems/{id}", async (int id, Todo inputTodo, TodoDB db) =>
    //{
    //    var todo = await db.Todos.FindAsync(id);

    //    if(todo == null) return Results.NotFound();

    //    todo.Name = inputTodo.Name;
    //    todo.IsComplete = inputTodo.IsComplete;

    //    await db.SaveChangesAsync();

    //    return Results.NoContent();
    //});

    //app.MapDelete("/todoitems/{id}", async (int id, TodoDB db) =>
    //{
    //    if(await db.Todos.FindAsync(id) is Todo todo)
    //    {
    //        db.Todos.Remove(todo);
    //        await db.SaveChangesAsync();
    //        return Results.NoContent() ;
    //    }
    //    return Results.NotFound();
    //});

    #endregion

    #region Advance Setup for Production ready code

    var todoItems = app.MapGroup("/todoitems");

    todoItems.MapGet("/", GetAllTodos);
    todoItems.MapGet("/complete", GetCompleteTodos);
    todoItems.MapGet("/{id}", GetTodo);
    todoItems.MapPost("/", CreateTodo);
    todoItems.MapPut("/{id}", UpdateTodo);
    todoItems.MapDelete("/{id}", DeleteTodo);

    #endregion
}

#region Minimal API methods

static async Task<IResult> GetAllTodos(TodoDB db)
{
    return TypedResults.Ok(await db.Todos.ToArrayAsync());
}

static async Task<IResult> GetCompleteTodos(TodoDB db)
{
    return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).ToListAsync());
}

static async Task<IResult> GetTodo(int id, TodoDB db)
{
    return await db.Todos.FindAsync(id)
        is Todo todo
            ? TypedResults.Ok(todo)
            : TypedResults.NotFound();
}

static async Task<IResult> CreateTodo(Todo todo, TodoDB db)
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/todoitems/{todo.Id}", todo);
}

static async Task<IResult> UpdateTodo(int id, Todo inputTodo, TodoDB db)
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return TypedResults.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteTodo(int id, TodoDB db)
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}

#endregion

#endregion