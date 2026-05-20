using System.Diagnostics;
using EcommerceApp1.Data;
using EcommerceApp1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MimeKit;
using System.IO;
using System.Text.RegularExpressions;

namespace EcommerceApp1.Controllers;

public class HomeController : Controller
{
    public class ContactViewModel
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
    
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly EmailService _emailService;
    private readonly IConfiguration _config;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IConfiguration config, EmailService emailService)
    {
        _logger = logger;
        _context = context;
        _emailService = emailService;
        _config = config;
    }

    public IActionResult Index()
    {
        ViewBag.Page = "Home";
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> SendContactEmail(ContactViewModel model)
    {
        if (!ModelState.IsValid) return View("Contact", model);
        
        // Send contact messages to business 
        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "ContactConfirmation.html");
        string content = await System.IO.File.ReadAllTextAsync(pathToFile);
        string pattern = @"\{\{\s*(.*?)\s*\}\}";
        var replacements = new Dictionary<string, string>
        {
            { "email", model.Email },
            { "subject", model.Subject },
            { "message", model.Message }
        };

        await _emailService.SendEmailAsync(
            "Contact request confirmation",
            "Merchandise Store Support Team",
            content,
            model.Email
        );
        
        // Send a response message to clients
        pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "ReceivedContact.html");
        content = await System.IO.File.ReadAllTextAsync(pathToFile);
        content = Regex.Replace(content, pattern, match =>
        {
            string key = match.Groups[1].Value;
            return replacements.ContainsKey(key) ? replacements[key] : match.Value;
        });
        
        await _emailService.SendEmailAsync(
            "Merchandise Store Contact Inbox",
            "Contact Request",
            content,
        _config.GetValue<string>("EmailSetting:SenderEmail")
        );

        ViewBag.Success = "Email sent successfully!";

        return View("Contact");
    }

    [HttpPost]
    public async Task<IActionResult> SendSubConfirmEmail(string email)
    {
        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "NewsSubConfirmation.html");
        string content = await System.IO.File.ReadAllTextAsync(pathToFile);
        
        await _emailService.SendEmailAsync(
            "Merchandise Store Newsletter",
            "Newsletter Confirmation",
            content,
            email
        );
        Console.WriteLine("Subscibed email: ", email);
        return Redirect("/Home");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}