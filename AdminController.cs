using LekplatsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.Mvc;


namespace LekplatsWebApp.Controllers
{
    public class AdminController : Controller
    {
        LekplatsService.LekplatsServiceClient klient = new LekplatsService.LekplatsServiceClient();
        [Authorize]
        public ActionResult Index()
        { 
            List<string> allaOmråden = klient.GetAllaOmråden().ToList();
            allaOmråden.Sort();

            //Alla lekplatser från ett område ligger i sin egen array och alla områdes-arrayer läggs in i en lista.
            List<LekplatsService.LekplatsFullständigData[]> allaLekplatserSorterade = new List<LekplatsService.LekplatsFullständigData[]>();
            foreach(var område in allaOmråden)
            {
                LekplatsService.LekplatsFullständigData[] lekplatserFrånOmråde = klient.GetLekplatserFrånOmråde(område);
                //Om arrayen är tom finns det inga lekplatser i det området
                if (lekplatserFrånOmråde.Length > 0)
                {
                    allaLekplatserSorterade.Add(lekplatserFrånOmråde);
                }
            }
            return View(allaLekplatserSorterade);
        }
        [Authorize]
        public ActionResult SkapaLekplats()
        {
            //https://github.com/SortableJS/Sortable

            Models.LekplatsMedOmråden lekplatsdata = new Models.LekplatsMedOmråden();
            var områdesLista = klient.GetAllaOmråden().ToList();
            områdesLista.Sort();
            lekplatsdata.OmrådesLista = områdesLista;
            return View(lekplatsdata);
        }
        [Authorize]
        [HttpPost]
        public ActionResult SkapaLekplats(Models.LekplatsMedOmråden nyLekplats, string[] lekutrustning, string[] tillgänglighet)
        {
            //Alla input-fält för lekutrustning har samma namn i formuläret och därför skapas det automatiskt en array, samma sak för tillgänglighet
            nyLekplats.LekplatsData.lekutrustning = lekutrustning;
            nyLekplats.LekplatsData.tillgänglighet = tillgänglighet;
            bool skapaStatus = klient.SkapaLekplats(nyLekplats.LekplatsData);

           return RedirectToAction("Index");
        }
        [Authorize]
        [HttpGet]
        public ActionResult UppdateraLekplats(int lekplatsId)
        {
            LekplatsMedOmråden viewModel = new LekplatsMedOmråden
            {
                OmrådesLista = klient.GetAllaOmråden().ToList(),
                LekplatsFullständigData = klient.GetLekplatsData(lekplatsId)
            };
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        public ActionResult UppdateraLekplats(Models.LekplatsMedOmråden uppdateradLekplats, string[] lekutrustning, string[] tillgänglighet)
        {
            uppdateradLekplats.LekplatsFullständigData.lekutrustning = lekutrustning;
            uppdateradLekplats.LekplatsFullständigData.tillgänglighet = tillgänglighet;
            //**************************************************************************
            //GÖR NÅGOT OM DEN MISSLYCKAS, REDIRECT ELLER STANNA PÅ SIDAN???
            bool skapaStatus = klient.UppdateraLekplats(uppdateradLekplats.LekplatsFullständigData);
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public ActionResult TaBortLekplats(int lekplatsId)
        {
            bool kontroll = klient.TaBortLekplats(lekplatsId);
            return RedirectToAction("Index");
        }



        public ActionResult LoggaIn()
        {
            return View();
        }

        [HttpPost]
        [Obsolete]
        public ActionResult LoggaIn(Models.Anvandare Inloggning)
        {
            bool ValidUser = false;

            ValidUser = System.Web.Security.FormsAuthentication.Authenticate(Inloggning.anvandarnamn, Inloggning.losenord);

            if (ValidUser == true)
            {
                System.Web.Security.FormsAuthentication.RedirectFromLoginPage(Inloggning.anvandarnamn, false);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Inloggningen ej godkänd");
                return View();
            }
        }

        [Authorize]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("LoggaIn", "Admin");
        }
    }
}