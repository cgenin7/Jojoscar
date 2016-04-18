using System.Collections.Generic;

namespace JojoscarMVCCommun
{
    public class TableModel
    {
        public int TableID { get; set; }
        public string Name { get; set; }
        public int GuestResponsibleId { get; set; }
        public List<GuestModel> Guests { get; set; }
    }
}