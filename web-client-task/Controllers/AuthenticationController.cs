using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using web_client_task.Models;
using web_client_task.Models.Dtos;
using web_client_task.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace web_client_task.Controllers
{
    public class AuthenticationController : Controller
    {
        private const int pageSize = 3;
        static HttpClient client = new HttpClient();
        string Url { get => "https://localhost:44382"; }

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public AuthenticationController()
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }


        public IActionResult Register()
        {
            var responce = new RegisterViewModel();
            return View(responce);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            var request = new UserForRegistrationDto
            {
                UserName = viewModel.UserName,
                LastName = viewModel.LastName,
                FirstName = viewModel.FirstName,
                Email = viewModel.Email,
                Password= viewModel.Password,
            };
            var res = await client.PostAsync($"{Url}/api/authentication", new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8, "application/json"));
            if (!res.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, res.StatusCode.ToString());
                ModelState.AddModelError(string.Empty, "Creation failed");
                return View(viewModel);
            }
            var responceString = await res.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize <JwtToken>(responceString, options);
			return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			Response.Cookies.Delete("jwtToken");
			Response.Cookies.Delete("refreshToken");
			return RedirectToAction("Index", "Home");
		}


		public IActionResult Login()
		{
			return View(new UserForAuthenticationDto());
		}

        [HttpPost]
        public async Task<IActionResult> Login(UserForAuthenticationDto dto)
        {
			var res = await client.PostAsync($"{Url}/api/authentication/login", new StringContent(
		        JsonSerializer.Serialize(dto),
		        Encoding.UTF8, "application/json"));
			if (!res.IsSuccessStatusCode)
			{
				ModelState.AddModelError(string.Empty, res.StatusCode.ToString());
				ModelState.AddModelError(string.Empty, "Login failed");
				return View(dto);
			}
			var Cookies = ExtractCookiesFromResponse(res);

			foreach (var item in Cookies)
			{
				HttpContext.Response.Cookies.Append(
				   item.Key,
				   item.Value,
				   new Microsoft.AspNetCore.Http.CookieOptions { IsEssential = true, HttpOnly = true });
			}

			var responceString = await res.Content.ReadAsStringAsync();
			var token = JsonSerializer.Deserialize<JwtToken>(responceString, options);
			await Authenticate(token.Token);
            return RedirectToAction("Index", "Home");
		}

		private static IDictionary<string, string> ExtractCookiesFromResponse(HttpResponseMessage response)
		{
			IDictionary<string, string> result = new Dictionary<string, string>();
			IEnumerable<string> values;
			if (response.Headers.TryGetValues("Set-Cookie", out values))
			{
				Microsoft.Net.Http.Headers.SetCookieHeaderValue.ParseList(values.ToList()).ToList().ForEach(cookie =>
				{
					result.Add(cookie.Name.Value, cookie.Value.Value);
				});
			}
			return result;
		}

		[Authorize]
		public async Task<IActionResult> RefreshToken()
		{
			var jtoken = Request.Cookies["jwtToken"];
			var res = await client.GetAsync($"{Url}/api/authentication/refresh-token/{jtoken}");
			var Cookies = ExtractCookiesFromResponse(res);

			foreach (var item in Cookies)
			{
				HttpContext.Response.Cookies.Append(
				   item.Key,
				   item.Value,
				   new Microsoft.AspNetCore.Http.CookieOptions { IsEssential = true, HttpOnly = true });
			}

			var responceString = await res.Content.ReadAsStringAsync();
			var token = JsonSerializer.Deserialize<JwtToken>(responceString, options);
			await Authenticate(token.Token);
			return RedirectToAction("Index", "Home");
		}

		private async Task Authenticate(string userName)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
			};
			ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
		}
	}
}
