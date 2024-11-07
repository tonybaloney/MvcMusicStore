using MvcMusicStore.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcMusicStore.Controllers
{
    public class StoreController : Controller
    {
        AIFunctionApiClient aiFunctionApiClient;

        public StoreController()
        {
            aiFunctionApiClient = new AIFunctionApiClient(ConfigurationManager.AppSettings["AIFunctionHost"]);
        }

        MusicStoreEntities storeDB = new MusicStoreEntities();
        //
        // GET: /Store/

        public ActionResult Index()
        {
            var genres = storeDB.Genres.ToList();

            return View(genres);
        }


        //
        // GET: /Store/Browse?genre=Disco

        public ActionResult Browse(string genre)
        {
            // Retrieve Genre genre and its Associated associated Albums albums from database
            var genreModel = storeDB.Genres.Include("Albums")
                .Single(g => g.Name == genre);

            return View(genreModel);
        }

        public ActionResult Details(int id) 
        {
            var album = storeDB.Albums.Find(id);

            return View(album);
        }

        [ChildActionOnly]
        public ActionResult GenreMenu()
        {
            var genres = storeDB.Genres
                .OrderByDescending(
                    g => g.Albums.Sum(
                    a => a.OrderDetails.Sum(
                    od => od.Quantity)))
                .Take(9)
                .ToList();

            return PartialView(genres);
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
                .Where(a => DbFunctions.Like(a.Title, query))
                .Take(10);
            return View(new ViewModels.SearchViewModel { Query = q, AiQuery = query, Results = albums});
        }
    }
}