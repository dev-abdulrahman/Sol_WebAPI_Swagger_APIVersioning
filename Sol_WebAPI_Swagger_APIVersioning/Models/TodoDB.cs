using Microsoft.EntityFrameworkCore;

namespace Sol_WebAPI_Swagger_APIVersioning.Models
{
    public class TodoDB : DbContext
    {
        public TodoDB(DbContextOptions<TodoDB> options)
        : base(options) { }

        public DbSet<Todo> Todos => Set<Todo>();
    }
}
