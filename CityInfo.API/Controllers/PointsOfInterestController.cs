using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;

        //Constructor Injection

        public PointsOfInterestController (ILogger<PointsOfInterestController> logger)
        {
            _logger = logger;
            //Provide access to HttpRequest's container
            //HttpContext.RequestServices.GetService();
        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                //Can the city be found?

                var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound();
                }

                return Ok(city.PointsOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city cith id {cityId}.", ex);
                //Throw Exception Manually
                return StatusCode(500, "A problem happened while handling your request.");
            }

        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            //Can the point of interest be found?

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }

        [HttpPost("{cityId}/pointsOfInterest")]
        public IActionResult CreatePointOfInterest(int cityId, 
                                                   [FromBody] PointOfInterestForCreationDTO pointOfInterest/*Use to get content from body*/)
        {
            if (null == pointOfInterest)
            {
                return BadRequest();
            }

            //Incase Name and Description are the same

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description"/*Can be a property name, but doesn't have to be*/, 
                                         "The provided description should be different from Name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            //demo purposes - to be improved

            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
                c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new
            { cityId = cityId, id = finalPointOfInterest.Id }, finalPointOfInterest);
        }

        [HttpPut("{cityId}/pointOfInterest/{id}")]
        public IActionResult UpdatePointOfInterest (int cityId, int id, 
                                                    [FromBody] PointOfInterestForUpdatesDto pointOfInterest /*Use to get content from body*/)
        {
            if (null == pointOfInterest)
            {
                return BadRequest();
            }

            //Incase Name and Description are the same

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description"/*Can be a property name, but doesn't have to be*/,
                                         "The provided description should be different from Name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{cityId}/pointOfInterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest (int cityId, int id,
                                                             [FromBody] JsonPatchDocument<PointOfInterestForUpdatesDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = new PointOfInterestForUpdatesDto()
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description
            };

            //Apply the Patch Document

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError("Description"/*Can be a property name, but doesn't have to be*/,
                                         "The provided description should be different from Name.");
            }

            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();

        }

        [HttpDelete("{cityId/pointOfInterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }


            return NoContent();
        }
    }
}
