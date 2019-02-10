using System;

namespace Model.Models
{
    public class WorkloadItem
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsProcessed { get; set; }
    }
}
