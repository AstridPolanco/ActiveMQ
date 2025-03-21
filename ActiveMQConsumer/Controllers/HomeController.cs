using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ActiveMQConsumer.Models;
using ActiveMQConsumer.Services;

namespace ActiveMQConsumer.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var messages = MessageConsumerService.Messages.ToList();
        Console.WriteLine($"Mensajes en el controlador: {messages.Count}");
        return View(messages);
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
