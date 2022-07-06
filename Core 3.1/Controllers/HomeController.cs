using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core_3._1.Models;
using Microsoft.AspNetCore.Authorization;
using Core_3._1.ViewModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Core_3._1.Controllers
{
    public class HomeController : Controller
    {
        private AppDBContext Context { get; }

        static List<String> guidsList = new List<String>() { "289d738c-ad4f-45fb-a847-0961a5fd73f9", "8379a0cd-2b12-478d-b2a2-4956c941fdd4", "425f7a23-0bbe-4253-936a-8425a59ddedf", "c18f0340-e1c9-4167-b2d0-998698c93dff",
            "ad423f38-9531-4d6f-8ff2-3aabe505987d", "ed5493f2-2474-487f-a681-a48b6744c497", "a3d61713-16ec-4be1-8a0c-da2b0b3f685c", "40fb3f23-a1f7-40f6-a70c-bd283d701e35", "cd0113f9-e183-4102-9c4b-f7fa9e7d0a77",
            "8db1a5bf-8ea7-49d0-8d3c-858f64684ee4", };

        public IConfiguration Configuration { get; }

        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, IConfiguration config, AppDBContext _context)
        {
            _logger = logger;
            _userManager = userManager;
            Configuration = config;
            this.Context = _context;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Aliases()
        {
            List<DomainAlias> domainAliases;
            var currentUser = _userManager.GetUserId(User);

            using (AppDBContext context = Context)
            {
                domainAliases = context.DomainAliases
                                        .Where(s => s.UserId == currentUser)
                                        .ToList();
            }

            /*List<DomainAlias> domainAliases = (from domainAlias in this.Context.DomainAliases.Take(10)
                                               select domainAlias).ToList();*/
            return View(domainAliases);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Privacy(AliasCreationViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                
                var alias = new DomainAlias
                {
                    Id = Guid.NewGuid().ToString(),
                    Domain = model.Domain,
                    Email = guidsList[0],
                    UserId = user.Id,
                };

                var optionsBuilder = new DbContextOptionsBuilder<AppDBContext>();
                optionsBuilder.UseNpgsql(Configuration.GetConnectionString("default"));
                
                using (var context = new AppDBContext(optionsBuilder.Options))
                {
                    context.Add(alias);
                    await context.SaveChangesAsync();
                }

                guidsList.RemoveAt(0);

                return RedirectToAction("index", "Home");




            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult Delete(string id)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDBContext>();
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("default"));
            using (var context = new AppDBContext(optionsBuilder.Options))
            {
                DomainAlias domainAlias = new DomainAlias() { Id = id };
                context.DomainAliases.Attach(domainAlias);
                context.DomainAliases.Remove(domainAlias);
                context.SaveChanges();
            }
            return RedirectToAction("Aliases", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }
}
