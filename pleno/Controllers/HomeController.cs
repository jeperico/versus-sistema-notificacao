using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MinhaNotificacao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MinhaNotificacao.Services.Interfaces;
using MinhaNotificacao.Entities;
using MinhaNotificacao.Exceptions;
using MinhaNotificacao.Hubs;
using System.Security.Claims;


namespace MinhaNotificacao.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
