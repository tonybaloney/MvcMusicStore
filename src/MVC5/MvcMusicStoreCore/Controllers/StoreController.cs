using MvcMusicStoreCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStoreCore.Extensions;
using MvcMusicStoreCore.ViewModels;
using Microsoft.EntityFrameworkCore.Cosmos.Extensions;

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
            var albums = await storeDB.Albums.Where(a => a.GenreName == genre).ToListAsync();

            return View(new BrowseViewModel() { Genre = genre, Albums = albums });
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
                return View(new SearchViewModel { Query = string.Empty, AiQuery = string.Empty, Results = [] });
            }


            var query = await aiFunctionApiClient.GetRecordSearchAsync(q);
            if (string.IsNullOrWhiteSpace(query))
            {
                query = q;
            }

#pragma warning disable CA1862 // Not supported in Cosmos DB
            var albums = storeDB.Albums
                .Where(a => a.Title.ToLower().Contains(query.ToLower()))
                .Take(10);
#pragma warning restore CA1862 // Use the 'StringComparison' method overloads to perform case-insensitive string comparisons

            float[]? embeddings;
            // If there is a match, get the embeddings for the record
            if (albums.Any())
            {
                var album = albums.First();
                embeddings = await aiFunctionApiClient.GetRecordEmbeddingsAsync(album.Title, album.ArtistName, album.GenreName);
            } else
            {
                embeddings = await aiFunctionApiClient.GetRecordEmbeddingsAsync(query, "", "");
            }

            if (embeddings == null)
            {
                return View(new SearchViewModel { Query = q, AiQuery = query, Results = await albums.ToListAsync(), Similar = [] });
            }

            // Get similar albums by vector distance
            var similarAlbums = await storeDB.Albums
                .OrderBy(s => EF.Functions.VectorDistance(s.Embeddings, embeddings))
                .Take(5)
                .ToListAsync();

            return View(new SearchViewModel { Query = q, AiQuery = query, Results = await albums.ToListAsync(), Similar = similarAlbums});
        }
    }
}