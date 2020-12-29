﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CoreCodeCamp.Controllers
{
    [Route("api/{controller}")]
    [ApiController]
    public class OperationController:ControllerBase
    {
        private readonly IConfiguration _config;

        public OperationController(IConfiguration config)
        {
            _config = config;
        }
        [HttpOptions("reload-config")]
        public IActionResult ReloadConfig()
        {
            try
            {
                var root =(IConfigurationRoot) _config;
                root.Reload();
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
