using System.Collections.Generic;

namespace Api.Models
{
    public class ApiResponse<T>
    {
        public int Total { get; set; }
        public int Pages { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
