using Bogus;
using GenerateUsers.Data;
using GenerateUsers.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GenerateUsers.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            Amount.numb = 1;
            return View();
        }
        public string MakeError(int error, string[] alphabet, string result)
        {
            var random = new Random();
            int strPos;
            for (int j = 0; j < error; j++)
            {
                int val = random.Next(3);
                switch (val)
                {
                    case 0:
                        if (result.Length > 3)
                        {
                            int pos = random.Next(result.Length);
                            result = result.Remove(pos, 1);
                        }
                        break;
                    case 1:
                        var symbPos = random.Next(alphabet.Length);
                        strPos = random.Next(result.Length);
                        if (symbPos % 2 == 0)
                            result = result.Insert(strPos, alphabet[symbPos]);
                        else
                            result = result.Insert(strPos, alphabet[symbPos].ToLower());
                        break;
                    case 2:
                        strPos = random.Next(result.Length);
                        if (result.Length > 3)
                        {
                            if (strPos == 0)
                            {
                                var first = result[strPos];
                                var second = result[strPos + 1];
                                result = result.Remove(strPos, 2);
                                result.Insert(strPos, first.ToString() + second.ToString());
                            }
                            else
                            {
                                var first = result[strPos - 1];
                                var second = result[strPos];
                                result = result.Remove(strPos - 1, 2);
                                result.Insert(strPos - 1, first.ToString() + second.ToString());
                            }
                        }
                        
                        break;
                }
            }
            return result;
        }

        public PartialViewResult Update(float error, string locale, int seed, int page = 1)
        {
            string [] German = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(',');
            string[] Russian = "А,Б,В,Г,Д,Е,Ё,Ж,З,И,К,Л,М,Н,О,П,Р,С,Т,У,Ф,Х,Ц,Ч,Ш,Щ,Ь,Ъ,Э,Ю,Я,".Split(',');
            string[] Italian = "A,B,C,D,E,F,G,H,I,L,M,N,O,P,Q,R,S,T,U,V,Y".Split(',');
            string[] Numbers = "0,1,2,3,4,5,6,7,8,9".Split(',');
            string loc = "";
            string[] alphabet = new string[0];
            switch (locale)
            {
                case "russia":
                    loc = "ru";
                    alphabet = Russian;
                    break;
                case "usa":
                    loc = "en_US";
                    alphabet = German;
                    break;
                case "germany":
                    loc = "de";
                    alphabet = German;
                    break;
                case "italy":
                    loc = "it";
                    alphabet = Italian;
                    break;
            }
            var random = new Random();
            List<UserStats> userStats = new List<UserStats>();
            seed += page;
            int numb;
            if (page == 1)
                numb = 20;
            else
                numb = 15;
            var faker = new Faker<UserStats>(loc).UseSeed(seed);
            for (int i = 1; i <= numb; i++)
            {
                int errorC;
                if (random.Next(2) == 1)
                    errorC = Convert.ToInt32(Math.Floor(error));
                else
                    errorC = Convert.ToInt32(Math.Ceiling(error));

                faker
                    .RuleFor(x => x.FullName, x => x.Person.FullName)
                    .RuleFor(x => x.Address, x => x.Person.Address.Street + " | " + x.Person.Address.City + " | " + x.Person.Address.ZipCode)
                    .RuleFor(x => x.Phone, x => x.Person.Phone)
                    .RuleFor(x => x.Id, x => x.Random.Number(1000));
                var obj = faker.Generate();

                int tmperror = random.Next(errorC);
                obj.FullName = MakeError(tmperror, alphabet, obj.FullName);

                errorC -= tmperror;
                if (errorC > 0)
                {
                    tmperror = random.Next(errorC);
                    obj.Address = MakeError(tmperror, alphabet, obj.Address);
                    errorC -= tmperror;
                }

                if (errorC > 0)
                    obj.Phone = MakeError(errorC, Numbers, obj.Phone);

                userStats.Add(obj);
            }
            if (page == 1)
                Amount.numb = 1;
            return PartialView("_Table", userStats);
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