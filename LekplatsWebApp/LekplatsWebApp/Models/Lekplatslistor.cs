using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LekplatsWebApp.Models
{
    public class LekplatsListor
    {
        public List<LekplatsService.LekplatsFullständigData> FullständigLekplatsLista { get; set; }
        public List<LekplatsService.LekplatsData> LekplatsLista { get; set; }
        public List<LekplatsService.MedelBetygData> MedelBetygsLista { get; set; }
        public List<string> OmrådesLista { get; set; }
    }
}