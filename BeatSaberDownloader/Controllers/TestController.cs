using BeatSaberDownloader.Server.Services.MediaR.Queries.GetSong;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BeatSaberDownloader.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private IMediator _mediator;

        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("running")]
        [HttpGet]
        public IActionResult Test()
        {
            return Ok("Server is running...");
        }

        [Route("songdownload")]
        [HttpGet]
        public async Task<IActionResult> TestSong()
        {
            var query = new GetSongQuery
            {
                SongId = "2cf84",
                VersionHash = "37b0e0b00e5e4a82047a2190ac20747e3016e7ea"
            };
            var fileContent = await _mediator.Send(query);
            if (fileContent == null)
            {
                return NotFound();
            }
            return File(fileContent, "application/zip");
        }

        //[Route("run")]
        //[HttpPost]
        //public async Task<IActionResult> TestProcess()
        //{
        //    var downloader = new Downloader(_logger);

        //    // Get current list of songs from their server
        //    var latestSongs = await downloader.GetAllSongInfoForAllFiltersAsync();

        //    new BeatSaverRepository(_configuration, _logger).SaveSongsToDb(latestSongs.docs);

        //    return Ok();
        //}
    }
}
