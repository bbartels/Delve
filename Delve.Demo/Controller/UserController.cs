using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Delve.Models;

namespace Delve.Demo.Controller
{
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet]
        public IActionResult Index(IResourceParameter<Models.User> param)
        {
            return Ok();
        }
    }
}