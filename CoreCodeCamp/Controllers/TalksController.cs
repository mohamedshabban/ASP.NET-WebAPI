using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CoreCodeCamp.Controllers
{
    [ApiController]
    [Route("api/camps/{moniker}/talks")]
    public class TalksController : Controller
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;


        public TalksController(ICampRepository campRepository,
                                IMapper mapper, LinkGenerator linkGenerator)
        {
            _campRepository = campRepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }
        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try
            {
                var talks = await _campRepository.GetTalksByMonikerAsync(moniker,
                                                                    true);
                if (talks == null) return NotFound();
                return _mapper.Map<TalkModel[]>(talks);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed ");
            }

            //return BadRequest();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id)
        {
            try
            {
                var talk = await _campRepository.GetTalkByMonikerAsync(moniker, id,
                                                                    true);
                if (talk == null) return NotFound();
                return _mapper.Map<TalkModel>(talk);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed ");
            }
        }

        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel model)
        {
            
            try
            {
                //check if exist
                var camp = await _campRepository.GetCampAsync(moniker);
                if (camp == null) return BadRequest("Camp Not found");
                var talk = _mapper.Map<Talk>(model);
                talk.Camp = camp;
                //speaker
                if (model.Speaker == null) return BadRequest("Speaker ID is required");
                var speaker =await _campRepository.GetSpeakerAsync(model.Speaker.SpeakerId);
                if (speaker == null) return BadRequest("speaker not found");
                talk.Speaker = speaker;


                _campRepository.Add(talk);
                if (await _campRepository.SaveChangesAsync())
                {
                    var url = _linkGenerator.GetPathByAction(
                        HttpContext, "Get", values: new {moniker, talk.TalkId});
                    return Created(url, _mapper.Map<TalkModel>(talk));
                }
                else
                {
                    return BadRequest("Failed to save");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed ");
            }

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TalkModel>> Put(string moniker, int id, TalkModel model)
        {
           
            try
            {
                var talk =await _campRepository.GetTalkByMonikerAsync(moniker, id, true);
                if (talk == null) return BadRequest("Not found talk");
                if (model.Speaker == null) return BadRequest("Speaker Not Found");
                var speaker =await _campRepository.GetSpeakerAsync(model.Speaker.SpeakerId);
                if (speaker == null) return BadRequest("speaker is null");
                talk.Speaker = speaker;
                //copy fields from model to talk
                _mapper.Map(model, talk);
                if (await _campRepository.SaveChangesAsync())
                {
                    return _mapper.Map<TalkModel>(talk);
                }
                else
                {
                    return BadRequest("failed with talk model");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed ");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(string moniker, int id)
        {
            try
            {
                var talk =await _campRepository.GetTalkByMonikerAsync(moniker, id);
                if (talk == null) return NotFound();
                _campRepository.Delete(talk);
                if (await _campRepository.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed ");
            }
        }

    }
}
