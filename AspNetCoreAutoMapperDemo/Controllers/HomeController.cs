using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreAutoMapperDemo.Models;

namespace AspNetCoreAutoMapperDemo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEmployeeService _employeeService;

    public HomeController(ILogger<HomeController> logger, IEmployeeService employeeService)
    {
        _logger = logger;
        _employeeService = employeeService;
    }

    public IActionResult Index()
    {
        return View(_employeeService.GetEmployees());
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
