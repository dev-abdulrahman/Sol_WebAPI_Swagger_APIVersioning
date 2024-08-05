using Microsoft.AspNetCore.Mvc;
using Sol_WebAPI_Swagger_APIVersioning.Models;

namespace Sol_WebAPI_Swagger_APIVersioning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        /// <summary>
        /// Get all employees
        /// </summary>
        /// <returns>The list of all employees</returns>
        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            return GetEmployees();
        }

        [HttpGet("{id}", Name = "Get")]
        public Employee Get(int id)
        {
            return GetEmployees().Find(e => e.Id == id);
        }

        /// <summary>
        /// Creates an Employee.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Employee
        ///     {        
        ///       "firstName": "Mike",
        ///       "lastName": "Andrew",
        ///       "emailId": "Mike.Andrew@gmail.com"        
        ///     }
        /// </remarks>
        /// <param name="employee"></param>  
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response> 
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public Employee Post([FromBody] Employee employee)
        {
            // Logic to create new Employee
            return new Employee();
        }

        // PUT: api/Employee/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Employee employee)
        {
            // Logic to update an Employee
        }

        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        private List<Employee> GetEmployees()
        {
            return new List<Employee>()
            {
                new Employee()
                {
                    Id = 1,
                    FirstName= "John",
                    LastName = "Smith",
                    EmailId ="John.Smith@gmail.com"
                },
                new Employee()
                {
                    Id = 2,
                    FirstName= "Jane",
                    LastName = "Doe",
                    EmailId ="Jane.Doe@gmail.com"
                }
            };
        }
    }
}
