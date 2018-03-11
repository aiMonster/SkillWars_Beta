using Common.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace SkillWars.Controllers
{
    [Route("api/Payment")]    
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("Shit")]
        public async Task<IActionResult> Shit()
        {
            await _paymentService.NewTransaction(Request.Form.ToList());
            return Ok();           
        }

        [HttpGet("GetLogs")]
        public string GetLogs()
        {            
            return HardLogger.Logs;
        }
    }

    public static class InfoList
    {
        public static string Str = "";
        public static List<KeyValuePair<string, StringValues>> Collection = new List<KeyValuePair<string, StringValues>>();      
    }
}
