using MvcMusicStore.Models;
using System.Collections.Generic;

namespace MvcMusicStore.ViewModels
{
    public class SearchViewModel
    {
        public string Query { get; set; }
        public string AiQuery { get; set; }
        public IEnumerable<Album> Results { get; set; }
    }
}