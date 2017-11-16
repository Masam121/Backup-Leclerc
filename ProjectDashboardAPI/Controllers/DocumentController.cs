using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDashboardAPI;
using ProjectDashboardAPI.Models.Dto;

namespace ProjectDashboardAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DocumentController : Controller
    {
        private readonly netflix_prContext _context;

        public DocumentController(netflix_prContext context)
        {
            _context = context;
        }

        // GET: api/Document
        [HttpGet]
        public IEnumerable<Document> GetDocument()
        {
            return _context.Document;
        }

        // GET: api/Document/5
        [HttpGet("project/{id}", Name = "GetDocumentsByProjectId")]
        public async Task<IActionResult> GetDocument([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int projectId = (from p in _context.Project
                             where p.ProjectSapId == id
                             select p.Id).FirstOrDefault();

            var documents = await (from p in _context.Document
                                        where p.ProjectId == projectId
                                        select p).ToListAsync();

            return Ok(documents);
        }

        // PUT: api/Document/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocument([FromRoute] int id, [FromBody] Document document)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != document.Id)
            {
                return BadRequest();
            }

            _context.Entry(document).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Document
        [HttpPost("project/{id}", Name ="CreateDocumentByProjectId")]
        public async Task<IActionResult> PostDocument([FromBody] DocumentDto document, [FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int projectId = (from p in _context.Project
                             where p.ProjectSapId == id
                             select p.Id).FirstOrDefault();

            if(projectId == 0)
            {
                return BadRequest(id);
            }
            if(document == null)
            {
                return BadRequest(document);
            }

            Document documentEntity = new Document();
            documentEntity.ProjectId = projectId;
            documentEntity.DocumentLink = document.DocumentLink;
            documentEntity.DocumentDescription = document.DocumentDescription;

            _context.Document.Add(documentEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDocument", new { id = documentEntity.Id }, documentEntity);
        }

        // DELETE: api/Document/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var document = await _context.Document.SingleOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            _context.Document.Remove(document);
            await _context.SaveChangesAsync();

            return Ok(document);
        }

        private bool DocumentExists(int id)
        {
            return _context.Document.Any(e => e.Id == id);
        }
    }
}