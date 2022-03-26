using System;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SLE_System.Models;
using System.Diagnostics;
using Microsoft.Extensions.Hosting.Internal;
using System.IO;
using Npgsql;

namespace Core_3._1.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager,
                              SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //var mailuNewEmail = Guid.NewGuid().ToString();
                    //var neededUser = await _userManager.GetUserAsync(HttpContext.User);
                    var userId = user.Id;
                    var newEmail = userId + "@aliases.online";
                    //var path = Directory.GetCurrentDirectory() + "\\shell\\mailuScript.sh";
                    var path = "/backend/shell/mailuScript.sh";

                    //var logFileName = Directory.GetCurrentDirectory() + "\\log.txt";

                    var logFileName = Directory.GetCurrentDirectory() + "/log.txt";
                    using (StreamWriter logFile = new StreamWriter(logFileName))
                    {
                        logFile.WriteLine(path);
                        logFile.WriteLine(userId);
                        logFile.WriteLine(model.Password);
                    }

                    //mailu registration
                    Process process = new Process();

                    var command = "sh";
                    var args = $"{path} {userId} {model.Password}";

                    var processInfo = new ProcessStartInfo();
                    processInfo.UseShellExecute = false;
                    processInfo.FileName = command;   // 'sh' for bash 
                    processInfo.Arguments = args;    // The Script name 

                    process = Process.Start(processInfo);   // Start that process.
                    process.WaitForExit();

                   /* Process process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "zsh",
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false
                        }
                    };
                    process.Start();
                    var args = "cd /mailu && docker-compose exec admin flask mailu user " + userId + " aliases.online " + model.Password;
                    await process.StandardInput.WriteLineAsync(args);*/
                    

                    Job.StartJob(newEmail, model.Password, model.Email);

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {

            //var branch = new Branch
            //{
            //    branchName = "Regie",
            //    address = "Naval"

            //};
            //branchContext.Branch.Add(branch);
            //branchContext.SaveChanges();

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);

                if (result.Succeeded)
                {
                    //Job.StartJob("833285df-e60d-45ab-8272-b94bb21ba430@aliases.online", "*8HXye85sMe$!P4", "laykdimon278@gmail.com");
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(user);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }
    }
}
