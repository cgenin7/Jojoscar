using System.ComponentModel;
using System.Linq;

namespace JojoscarMVCCommun
{
    public class ResultModel
    {
        [DisplayName("Nom")]
        public string Name { get; set; }
        public int Position { get; set; }

        [DisplayName("Réponses")]
        public int NbResponses { get; set; }
        [DisplayName("Points")]
        public int NbPointsWithPenality { get; set; }
        public int NbPointsWithoutPenality { get; set; }
        public float Remboursement { get; set; }
        public string RemboursementStr { get { return Remboursement.ToString("C"); } }

        public bool IsPresent { get; set; }
        public bool IsEligibleToMoney { get; set; }
    }
}
