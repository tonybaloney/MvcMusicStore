using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStoreCore.Models;

namespace MvcMusicStoreCore.ViewComponents
{
    public class GenreMenuViewComponent(MusicStoreEntities context) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var genres = await context.Genres
                .Take(9)
                .ToListAsync();

            return View(genres);
        }
    }
}
