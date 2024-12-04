using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BeatSaberDownloader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
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
            if (!Request.Headers.ContainsKey(Consts.AppTokenHeaderName) || Request.Headers[Consts.AppTokenHeaderName] != Consts.AppTokenValue
                || !Request.Headers.ContainsKey(Consts.YoloHoloHeaderName) || Request.Headers[Consts.YoloHoloHeaderName] != Consts.YoloHoloHeaderValue)
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
