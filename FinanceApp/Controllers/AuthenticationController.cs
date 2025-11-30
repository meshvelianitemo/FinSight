using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinanceApp.Models;
using FinanceApp.Data.Services;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity.Data;


namespace FinanceApp.Controllers
{
    public class AuthenticationController : Controller
    {

        private readonly IUserService _userService;

        private readonly IEmailSender _emailSender;

        private readonly IVerificationService _verificationService;
        public AuthenticationController(IUserService userService, IEmailSender emailSender, IVerificationService verificationService)
        {
            _userService = userService;
            _emailSender = emailSender;
            _verificationService = verificationService;
        }
        //LOGIN PART
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Models.LoginRequest request)
        {
            // Get user from database
            var user = await _userService.GetUserByUsernameAsync(request.Username);

            // Check if user exists and password is correct
            if (user == null || !_userService.VerifyPassword(user, request.Password))
            {
                ViewBag.Error = "Invalid credentials";
                return View(request);
            }

            // Generate JWT and Refresh token
            var token = _userService.GenerateJwtToken(user);
            var refreshToken = _userService.GenerateRefreshToken();
            
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userService.UpdateUserAsync(user);

            // Store JWT and RefreshToken in secure HTTP-only cookie
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30) 
            });

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return RedirectToAction("Index", "Expenses");
        }
        
        
        //REGISTRATION PART
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Models.RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            if (request.Password != request.ConfirmPassword)
            {
                ViewBag.Error = "Passwords do not match";
                return View(request);
            }

            var existingUser = await _userService.GetUserByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                ViewBag.Error = "Username already exists";
                return View(request);
            }


            AppUser user = null;
            EmailVerificationToken verificationToken = null;


            try 
            {
                // Create user using your UserService
                var refreshToken = _userService.GenerateRefreshToken();
                user = await _userService.CreateUserAsync(request.Username, request.Password, request.Email, false, refreshToken);
                // 2. Generate verification token
                verificationToken = await _verificationService.GenerateEmailVerificationTokenAsync(user.Id);
            }
            catch (InvalidOperationException ex)
            {
                ViewBag.Error = ex.Message;
                return View(request);
            }

            // 3. Send verification email

            var receiver = request.Email;
            var subject = "Verify your FinanceApp email";
            var message = $@"
            <h1>Verify your email</h1>
            <p>Thanks for registering! Please verify your email by entering the code below in the app:</p>
            <h2>{verificationToken.Token}</h2>
            <p>This code expires in 15 minutes.</p>";

            await _emailSender.SendEmailAsync(receiver, subject, message);

            //4. Redirect to VerifyEmail page

            return RedirectToAction("VerifyEmail", new { email = user.Email });
            
        }

        [HttpGet]
        public IActionResult VerifyEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Register"); // fallback

            var model = new VerifyEmailRequest
            {
                Email = email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailRequest request)
        {
            var user = await _userService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                ViewBag.Error = "Invalid email";
                return View(request);
            }

            var isValid = await _verificationService.ValidateTokenAsync(user.Id, request.VerificationCode);
            if (!isValid)
            {
                ViewBag.Error = "Invalid or expired code";
                return View(request);
            }

            // Mark email as confirmed
            user.EmailConfirmed = true;
            await _userService.UpdateUserAsync(user);

            // Issue JWT
            var token = _userService.GenerateJwtToken(user);
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return RedirectToAction("Index", "Expenses");
        }
    }
}
