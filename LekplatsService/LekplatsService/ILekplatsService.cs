using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace LekplatsService
{
    [ServiceContract]
    public interface ILekplatsService
    {
        [OperationContract]
        LekplatsFullständigData[] GetAllaLekplatser();
        [OperationContract]
        LekplatsFullständigData GetLekplatsData(int lekplatsId);
        [OperationContract]
        LekplatsFullständigData[] GetLekplatserFrånOmråde(string område);
        [OperationContract]
        bool SkapaLekplats(LekplatsData nyLekplats);
        [OperationContract]
        bool SättBetyg(IndividuelltBetygData betyg);
        [OperationContract]
        string[] GetAllaOmråden();
        [OperationContract]
        bool TaBortLekplats(int LekplatsId);
        [OperationContract]
        bool UppdateraLekplats(LekplatsFullständigData inputLekplats);


    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class LekplatsData
    {
        [DataMember (IsRequired =true)]
        public string namn { get; set; }
        [DataMember]
        public string adress { get; set; }
        [DataMember(IsRequired = true)]
        public string område { get; set; }
        [DataMember]
        public string beskrivning { get; set; }
        [DataMember(IsRequired = true)]
        public string[] lekutrustning { get; set; }
        [DataMember(IsRequired = true)]
        public string[] tillgänglighet { get; set; }
        [DataMember(IsRequired = true)]
        public int[] målgrupp { get; set; }
    }
    [DataContract]
    public class LekplatsFullständigData : LekplatsData
    {
        [DataMember(IsRequired = true)]
        public int lekplatsId { get; set; }
        [DataMember]
        public MedelBetygData medelbetyg { get; set; }
    }
    [DataContract]
    public class MedelBetygData
    {
        [DataMember]
        public double totalMedelbetyg { get; set; }
        [DataMember]
        public double skojfaktorMedelbetyg { get; set; }
        [DataMember]
        public double renlighetMedelbetyg { get; set; }
        [DataMember]
        public double trygghetMedelbetyg { get; set; }
        [DataMember]
        public int antalRöster { get; set; }
    }
    [DataContract]
    public class IndividuelltBetygData
    {
        [DataMember(IsRequired = true)]
        public int lekplatsId { get; set; }
        [DataMember(IsRequired = true)]
        public int skojfaktorBetyg { get; set; }
        [DataMember(IsRequired = true)]
        public int renlighetBetyg { get; set; }
        [DataMember(IsRequired = true)]
        public int trygghetBetyg { get; set; }
        [DataMember]
        public DateTime datum { get; set; }
    }

}
