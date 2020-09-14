using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LekplatsWebApp.Controllers
{
    public class LekplatserController : Controller
    {
        LekplatsService.LekplatsServiceClient klient = new LekplatsService.LekplatsServiceClient();
        public ActionResult Index()
        {
            List<string> allaOmråden = klient.GetAllaOmråden().ToList();
            allaOmråden.Sort();

            //Alla lekplatser från ett område ligger i sin egen array och alla områdes-arrayer läggs in i en lista.
            List<LekplatsService.LekplatsFullständigData[]> allaLekplatserSorterade = new List<LekplatsService.LekplatsFullständigData[]>();
            foreach (var område in allaOmråden)
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

        public ActionResult Lekplats(int? lekplatsId)
        {
            if (!lekplatsId.HasValue)
            {
                return RedirectToAction("Index");
            }
            //Visar en specifik lekplats

            LekplatsService.LekplatsFullständigData lekplats = klient.GetLekplatsData(Convert.ToInt32(lekplatsId));

            return View(lekplats);
        }

        [HttpGet]
        public ActionResult GeBetyg(int? id)
        {
            //Hantering för om felaktigt id anges i url

            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }
            //Visar vyn för att lämna ett betyg

            LekplatsService.LekplatsFullständigData lekplatsAttBetygsätta = new LekplatsService.LekplatsFullständigData();

            lekplatsAttBetygsätta = klient.GetLekplatsData(Convert.ToInt32(id));

            return View(lekplatsAttBetygsätta);
        }
        [HttpPost]
        public ActionResult GeBetyg(int id, int skoj, int ren, int trygg)
        {
            LekplatsService.IndividuelltBetygData sattBetyg = new LekplatsService.IndividuelltBetygData();
            sattBetyg.skojfaktorBetyg = skoj;
            sattBetyg.renlighetBetyg = ren;
            sattBetyg.trygghetBetyg = trygg;
            sattBetyg.lekplatsId = id;

            bool betygStatus = klient.SättBetyg(sattBetyg);
            Console.WriteLine(betygStatus);
            ViewBag.b = "Tack för ditt omdöme!";

            return RedirectToAction("Lekplats", new { lekplatsId = id });

        }

    }
}