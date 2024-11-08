using MvcMusicStoreCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MvcMusicStoreCore.Controllers
{
    public class HomeController(MusicStoreEntities storeDB) : Controller
    {
        //
        // GET: /Home/

        public async Task<ActionResult> Index()
        {
            // Get most popular albums
            var albums = await GetTopSellingAlbums(6);

            return View(albums);
        }


        private async Task<List<Album>> GetTopSellingAlbums(int count)
        {
            // Group the order details by album and return
            // the albums with the highest count

            return await storeDB.Albums
                .OrderByDescending(a => a.AlbumId) // workaround
                .Take(count)
                .ToListAsync();
        }
    }
}