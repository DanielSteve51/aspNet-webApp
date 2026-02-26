using Microsoft.AspNetCore.Mvc;

namespace VowelWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                ViewBag.Result = "Invalid input.";
                return View();
            }

            int count = 0;
            string vowels = "aeiou";

            foreach (char ch in word.ToLower())
            {
                if (vowels.Contains(ch))
                {
                    count++;
                }
            }

            ViewBag.Result = $"Vowel count: {count}";
            return View();
        }
    }
}