using System.Collections.Generic;

namespace JabbadabbadoeBooking.Services;

public interface IImageStorageService
{
    IEnumerable<string> GetListingImages();
}
