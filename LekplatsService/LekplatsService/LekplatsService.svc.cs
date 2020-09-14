using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using LekplatsService.Models;

namespace LekplatsService
{
    public class LekplatsService : ILekplatsService
    {
        public bool SkapaLekplats(LekplatsData inputLekplats)
        {
            using (DatabasLekplatserEntities db = new DatabasLekplatserEntities())
            {
                //Det måste finnas minst en lekutrustning och en tillgänglighet
                if (inputLekplats.lekutrustning?.Length > 0 || inputLekplats.tillgänglighet?.Length > 0)
                {
                    try
                    {
                        Lekplatser dbLekplats = new Lekplatser()
                        {
                            Namn = inputLekplats.namn,
                            Adress = inputLekplats.adress,
                            Beskrivning = inputLekplats.beskrivning,
                            Områden = db.Områden.SingleOrDefault(p => p.Namn == inputLekplats.område),
                            MinÅlderMålgrupp = (byte)inputLekplats.målgrupp[0],
                            MaxÅlderMålgrupp = (byte)inputLekplats.målgrupp[1],
                        };
                        for (int i = 0; i < inputLekplats.lekutrustning.Length; i++)
                        {
                            Lekutrustning nyLekutrustning = new Lekutrustning
                            {
                                Ordning = i + 1,
                                Beskrivning = inputLekplats.lekutrustning[i]
                            };
                            dbLekplats.Lekutrustning.Add(nyLekutrustning);
                        }
                        for (int i = 0; i < inputLekplats.tillgänglighet.Length; i++)
                        {
                            Tillgänglighet nyTillgänglighet = new Tillgänglighet
                            {
                                Ordning = i + 1,
                                Beskrivning = inputLekplats.tillgänglighet[i]
                            };
                            dbLekplats.Tillgänglighet.Add(nyTillgänglighet);
                        }
                        db.Lekplatser.Add(dbLekplats);
                        db.SaveChanges();
                        return true;
                    }
                    catch { return false; }
                }
                else return false;
            }
        }
        public LekplatsFullständigData GetLekplatsData(int lekplatsId)
        {
            //Hämtar en lekplats från databasen och lägger till
            //tillhörande lekutrustning, tillgänglighet och medelbetyg i ett objekt
            using (DatabasLekplatserEntities db = new DatabasLekplatserEntities())
            {
                try
                {
                    Lekplatser dbLekplats = db.Lekplatser.Find(lekplatsId);
                    var dbLekutrustning = db.Lekutrustning.OrderBy(a => a.Ordning).Where(a => a.FK_Lekplatser_LekplatsId == dbLekplats.LekplatsId).Select(a => a.Beskrivning);
                    var dbTillgänglighet = db.Tillgänglighet.OrderBy(a => a.Ordning).Where(a => a.FK_Lekplatser_LekplatsId == dbLekplats.LekplatsId).Select(a => a.Beskrivning);
                    LekplatsFullständigData fullständigLekplats = new LekplatsFullständigData
                    {
                        lekplatsId = dbLekplats.LekplatsId,
                        namn = dbLekplats.Namn,
                        adress = dbLekplats.Adress,
                        beskrivning = dbLekplats.Beskrivning,
                        målgrupp = new int[] { (int)dbLekplats.MinÅlderMålgrupp, (int)dbLekplats.MaxÅlderMålgrupp },
                        område = dbLekplats.Områden.Namn,
                        lekutrustning = dbLekutrustning.ToArray(),
                        tillgänglighet = dbTillgänglighet.ToArray(),
                        medelbetyg = GetMedelBetygData(dbLekplats.LekplatsId)
                    };
                    return fullständigLekplats;
                }
                catch { return null; }
            }
        }
        public LekplatsFullständigData[] GetAllaLekplatser()
        {
            List<LekplatsFullständigData> outputAllaLekplatser = new List<LekplatsFullständigData>();
            using (DatabasLekplatserEntities db = new DatabasLekplatserEntities())
            {
                List<Lekplatser> dbAllaLekplatser = db.Lekplatser.ToList();
                foreach (Lekplatser dbLekplats in dbAllaLekplatser)
                {
                    outputAllaLekplatser.Add(GetLekplatsData(dbLekplats.LekplatsId));
                }
            }
            return outputAllaLekplatser.ToArray();
        }
        public LekplatsFullständigData[] GetLekplatserFrånOmråde(string område)
        {
            List<LekplatsFullständigData> outputLekplatserFrånOmråde = new List<LekplatsFullständigData>();
            using (DatabasLekplatserEntities db = new DatabasLekplatserEntities())
            {
                var dbLekplatser = db.Lekplatser.Where(a => a.Områden.Namn == område);
                foreach(var lekplats in dbLekplatser)
                {
                    outputLekplatserFrånOmråde.Add(GetLekplatsData(lekplats.LekplatsId));
                }
            }
            return outputLekplatserFrånOmråde.ToArray();   
        }
        public bool SättBetyg(IndividuelltBetygData betyg)
        {
            if (betyg.renlighetBetyg <= 5 && betyg.renlighetBetyg > 0
                && betyg.skojfaktorBetyg <= 5 && betyg.skojfaktorBetyg > 0
                && betyg.trygghetBetyg <= 5 && betyg.trygghetBetyg > 0)
            {
                using (var db = new DatabasLekplatserEntities())
                {
                    IndividuellaBetyg nyttBetyg = new IndividuellaBetyg();
                    try
                    {
                        nyttBetyg.Datum = DateTime.Now;
                        nyttBetyg.Skojfaktor = (byte)betyg.skojfaktorBetyg;
                        nyttBetyg.Renlighet = (byte)betyg.renlighetBetyg;
                        nyttBetyg.Trygghet = (byte)betyg.trygghetBetyg;
                        nyttBetyg.FK_Lekplatser_LekplatsId = betyg.lekplatsId;
                        db.IndividuellaBetyg.Add(nyttBetyg);
                        db.SaveChanges();
                    }
                    catch { return false; }
                    return true;
                }
            }
            else return false;            
        }
        public string[] GetAllaOmråden()
        {
            List<string> områden = new List<string>();
            using (var db = new DatabasLekplatserEntities())
            {
                var områdeLista = db.Områden.ToList();
                foreach (var item in områdeLista)
                {
                    områden.Add(item.Namn);
                }
            }
            return områden.ToArray();
        }
        public bool TaBortLekplats(int lekplatsId)
        {
            using (var db = new DatabasLekplatserEntities())
            {
                //På grund av främmande nycklar måste lekutrustning, tillgänglighet och betyg tas bort innan lekplatsen
                try
                {
                    Lekplatser dbLekplats = db.Lekplatser.Find(lekplatsId);
                    db.Lekutrustning.RemoveRange(db.Lekutrustning.Where(a => a.FK_Lekplatser_LekplatsId == dbLekplats.LekplatsId));
                    db.Tillgänglighet.RemoveRange(db.Tillgänglighet.Where(a => a.FK_Lekplatser_LekplatsId == dbLekplats.LekplatsId));
                    db.IndividuellaBetyg.RemoveRange(db.IndividuellaBetyg.Where(a => a.FK_Lekplatser_LekplatsId == dbLekplats.LekplatsId));
                    db.Lekplatser.Remove(dbLekplats);
                    db.SaveChanges();
                }
                catch { return false; }                
            }
            return true;
        }
        public bool UppdateraLekplats(LekplatsFullständigData inputLekplats)
        {
            using (DatabasLekplatserEntities db = new DatabasLekplatserEntities())
            {
                //Det måste finnas minst en lekutrustning och en tillgänglighet
                if (inputLekplats.lekutrustning?.Length > 0 || inputLekplats.tillgänglighet?.Length > 0)
                {
                    try
                    {
                        Lekplatser uppdateraLekplats = db.Lekplatser.Find(inputLekplats.lekplatsId);
                        uppdateraLekplats.LekplatsId = inputLekplats.lekplatsId;
                        uppdateraLekplats.Namn = inputLekplats.namn;
                        uppdateraLekplats.Adress = inputLekplats.adress;
                        uppdateraLekplats.Beskrivning = inputLekplats.beskrivning;
                        uppdateraLekplats.MinÅlderMålgrupp = (byte)inputLekplats.målgrupp[0];
                        uppdateraLekplats.MaxÅlderMålgrupp = (byte)inputLekplats.målgrupp[1];
                        uppdateraLekplats.Områden = db.Områden.SingleOrDefault((a => a.Namn == inputLekplats.område));
                        db.Lekutrustning.RemoveRange(db.Lekutrustning.Where(a => a.FK_Lekplatser_LekplatsId == uppdateraLekplats.LekplatsId));
                        db.Tillgänglighet.RemoveRange(db.Tillgänglighet.Where(a => a.FK_Lekplatser_LekplatsId == uppdateraLekplats.LekplatsId));
                        for (int i = 0; i < inputLekplats.lekutrustning.Length; i++)
                        {
                            Lekutrustning uppdateradLekutrustning = new Lekutrustning
                            {
                                Ordning = i + 1,
                                Beskrivning = inputLekplats.lekutrustning[i]
                            };
                            uppdateraLekplats.Lekutrustning.Add(uppdateradLekutrustning);
                        }
                        for (int i = 0; i < inputLekplats.tillgänglighet.Length; i++)
                        {
                            Tillgänglighet uppdateradTillgänglighet = new Tillgänglighet
                            {
                                Ordning = i + 1,
                                Beskrivning = inputLekplats.tillgänglighet[i]
                            };
                            uppdateraLekplats.Tillgänglighet.Add(uppdateradTillgänglighet);
                        }
                        db.SaveChanges();
                        return true;
                    }
                    catch { return false; }
                }
                else return false;
            }
        }





        public MedelBetygData GetMedelBetygData(int lekplatsId)
        {
            MedelBetygData MedelBetyg = new MedelBetygData();
            using (var db = new DatabasLekplatserEntities())
            {
                double skojfaktor = 0;
                double renlighet = 0;
                double trygghet = 0;
                int antalröster = 0;
                var BetygLista = from betyg in db.IndividuellaBetyg
                                 where betyg.FK_Lekplatser_LekplatsId == lekplatsId
                                 select new { betyg.Skojfaktor, betyg.Renlighet, betyg.Trygghet };
                foreach (var item in BetygLista)
                {
                    skojfaktor += item.Skojfaktor;
                    renlighet += item.Renlighet;
                    trygghet += item.Trygghet;
                    antalröster++;
                }
                MedelBetyg.renlighetMedelbetyg = Math.Round(renlighet / antalröster, 1);
                MedelBetyg.trygghetMedelbetyg = Math.Round(trygghet / antalröster, 1);
                MedelBetyg.skojfaktorMedelbetyg = Math.Round(skojfaktor / antalröster, 1);
                MedelBetyg.antalRöster = antalröster;
                MedelBetyg.totalMedelbetyg = Math.Round((((skojfaktor / antalröster) + (renlighet / antalröster) + (trygghet / antalröster)) / 3), 1);
            }
            return MedelBetyg;
        }
    }
}

