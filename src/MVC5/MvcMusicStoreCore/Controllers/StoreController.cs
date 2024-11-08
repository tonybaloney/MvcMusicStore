using MvcMusicStoreCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStoreCore.Extensions;

namespace MvcMusicStoreCore.Controllers
{
    public class StoreController(MusicStoreEntities storeDB, AIFunctionApiClient aiFunctionApiClient) : Controller
    {
        //
        // GET: /Store/

        public async Task<ActionResult> Index()
        {
            var genres = await storeDB.Genres.ToListAsync();

            return View(genres);
        }


        //
        // GET: /Store/Browse?genre=Disco

        public async Task<ActionResult> Browse(string genre)
        {
            // Retrieve Genre genre and its Associated associated Albums albums from database
            var genreModel = await storeDB.Genres.Include("Albums")
                .SingleAsync(g => g.Name == genre);

            return View(genreModel);
        }

        public async Task<ActionResult> Details(int id) 
        {
            var album = await storeDB.Albums.FindAsync(id);

            return View(album);
        }

        async public Task<ActionResult> Search(string q)
        {
            // If the query is empty, show an error
            if (string.IsNullOrWhiteSpace(q))
            {
                return View(new ViewModels.SearchViewModel { Query = string.Empty, AiQuery = string.Empty, Results = new List<Album>() });
            }


            string query = await aiFunctionApiClient.GetRecordSearchAsync(q);
            if (string.IsNullOrWhiteSpace(query))
            {
                query = q;
            }

            var albums = storeDB.Albums
                .Include("Artist")
                .Where(a => EF.Functions.Like(a.Title, query))
                .Take(10);
            return View(new ViewModels.SearchViewModel { Query = q, AiQuery = query, Results = albums});
        }
    }
}