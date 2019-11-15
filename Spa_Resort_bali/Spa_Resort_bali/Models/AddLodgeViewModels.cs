namespace Spa_Resort_bali.Models
{
    public class AddLodgeViewModels
    {
        public int LodgeTypeName { get; set; }
        public string Code { get; set; }
        public string PostCode { get; set; }
        public string Streed { get; set; }
        public string HouseNumber { get; set; }
        public string City { get; set; }
        public string Counrty { get; set; }
        public string State { get; set; }

        public int LodgeID { get; set; }

        public int AddresID { get; set; }

        public int LodgeTypeId { get; set; }

        public bool Disabled { get; set; }
    }
}