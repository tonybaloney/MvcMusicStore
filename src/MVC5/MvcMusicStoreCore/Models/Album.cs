using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MvcMusicStoreCore.Models 
{
    public class Album {
        [ScaffoldColumn(false)]

        public int AlbumId { get; set; }

        public string GenreName { get; set; }

        public string ArtistName { get; set; }

        [Required]
        [StringLength(160, MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [Range(0.01, 100.00)]

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [DisplayName("Album Art URL")]
        [StringLength(1024)]
        public string AlbumArtUrl { get; set; }
        public float[] Embeddings { get; set; }
    }
}