using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.MOFI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        public static readonly Department[] departments =
        {
            new Department() { Id = "CS", Name = "Computer Science"},
            new Department() { Id = "SE", Name = "Software Engineering"},
            new Department() { Id = "AIDS", Name = "Artificial Intelligence & Data Science"},
            new Department() { Id = "BIS", Name = "Business Information Systems"}
        };

        [HttpGet]
        public ActionResult GetDepartments()
        {
            if ( departments == null || departments.Length == 0) return NotFound();
            else return Ok(departments);
        }

    }
}
