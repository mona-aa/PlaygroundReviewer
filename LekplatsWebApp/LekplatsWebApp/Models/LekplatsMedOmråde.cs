using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LekplatsWebApp.Models
{
    public class LekplatsMedOmråden
    {
        public LekplatsService.LekplatsData LekplatsData { get; set; }

        public LekplatsService.LekplatsFullständigData LekplatsFullständigData { get; set; }
        public LekplatsService.MedelBetygData Medelbetyg { get; set; }


        public List<string> OmrådesLista { get; set; }
    }

     

}