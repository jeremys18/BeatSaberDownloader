using BeatSaberDownloader.Data.Consts;
using BeatSaberDownloader.Server.Services.MediaR.Queries.GetAllSongs;
using BeatSaberDownloader.Server.Services.MediaR.Queries.GetSong;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BeatSaberDownloader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private IMediator _mediator;

        public SongController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("allsongs")]
        [HttpGet]
        public async Task<IActionResult> GetAllSongsInfoAsync(string basePath)
        {
            // return song info list
            var result = await _mediator.Send(new GetAllSongsQuery { SongBasePath = basePath });
            return Ok(result);
        }

        [Route("{SongId}/{VersionHash}")]
        [HttpGet]
        public async Task<IActionResult> GetSongFile([FromRoute] GetSongQuery query)
        {
            if (!Request.Headers.ContainsKey(ServerConsts.AppTokenHeaderName) || Request.Headers[ServerConsts.AppTokenHeaderName] != ServerConsts.AppTokenValue
                || !Request.Headers.ContainsKey(ServerConsts.YoloHoloHeaderName) || Request.Headers[ServerConsts.YoloHoloHeaderName] != ServerConsts.YoloHoloHeaderValue)
            {
                return Unauthorized();
            }

            // return song info for specific song id
            var fileContent = await _mediator.Send(query);
            if (fileContent == null)
            {
                return NotFound();
            }

            return File(fileContent, "application/zip");
        }
    }
}
