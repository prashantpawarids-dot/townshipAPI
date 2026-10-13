using System;

namespace Township_API.Models
{
    public class IDCardPrnt
    {
    }

    #region IDCardPrintClasses

    public class IDCardData
    {
        public string? IDNumber { get; set; } = null;
        public string? name { get; set; } = null;
        public string? building { get; set; } = null;
        public string? flatNumber { get; set; } = null;
        public string? cardCSN { get; set; } = null;
        public string? cardDetails { get; set; } = null;
        public string? neighbourhood { get; set; } = null;
        public int isPrinted { get; set; } = 0;
        public int isPersonalised { get; set; } = 0;
        public string photoUrl { get; set; } = "";
        public string holderType { get; set; } = "";

        //public int id { get; set; } = 0;
        //public string? templateName { get; set; } = null;
        //public string? CardType { get; set; } = null;
        //public int pid { get; set; } = 0;
        //public int IDNumber { get; set; } = 0;
        //public string? shortname { get; set; } = null;
        //public string? CSN { get; set; } = null;
        //public int Building { get; set; } = 0;
        //public string? FlatNumber { get; set; }= null;
        //public DateTime? CardPrintingDate { get; set; } = null;
        //public DateTime? CardIssueDate { get; set; }=null;
        //public DateTime? validTill { get; set; } = null;
        //public string? BloodGroup { get; set; } = null;
        //public string? Bld { get; set; } = null;
        //public string? NRD { get; set; } = null;
    }

    #endregion

}
