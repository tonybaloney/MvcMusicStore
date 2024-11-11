using MvcMusicStoreCore.Models;

namespace MvcMusicStoreCore.ViewModels
{
    public class BrowseViewModel
    {
        public string Genre { get; set; }
        public string Description { get; set; }
        public List<Album> Albums { get; set; }
    }
}
