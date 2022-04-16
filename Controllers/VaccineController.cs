using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.MOFI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VaccineController : ControllerBase
    {
        public static readonly Vaccine[] vaccines =
        {
            new Vaccine() { Id = "PFZ", Name = "Pfizer BioNTech"},
            new Vaccine() { Id = "SPM", Name = "Sinopharm"},
            new Vaccine() { Id = "OAZ", Name = "Oxford AstraZeneca"},
            new Vaccine() { Id = "SPV", Name = "Sputnik V"},
            new Vaccine() { Id = "SVC", Name = "Sinovac"}
        };

        [HttpGet]
        public ActionResult GetVaccines()
        {
            if ( vaccines == null || vaccines.Length == 0) return NotFound();
            else return Ok(vaccines);
        }

    }
}
