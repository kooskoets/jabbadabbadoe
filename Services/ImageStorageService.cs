using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JabbadabbadoeBooking.Services;

public class ImageStorageService : IImageStorageService
{
    // Simple implementation that returns filenames from wwwroot/images/listing
    public IEnumerable<string> GetListingImages()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "listing");
        if (!Directory.Exists(path)) return Enumerable.Empty<string>();

        // Directory.GetFiles can be annotated nullable in some frameworks; guard against null
        var files = Directory.GetFiles(path) ?? Array.Empty<string>();
        return files.Select(Path.GetFileName);
    }
}
