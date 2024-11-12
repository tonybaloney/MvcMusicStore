using MvcMusicStoreCore.Models;

namespace MvcMusicStoreCore.ViewModels
{
    public class DetailsViewModel
    {
        public Album Album { get; set; }
        public IEnumerable<Album> Similar { get; set; }
    }
}
