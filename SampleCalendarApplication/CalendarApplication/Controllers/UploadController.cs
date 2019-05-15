using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CalendarApplication.Controllers
{
  [Produces("application/json")]
  [Route("api/[controller]")]
  public class UploadController : Controller
  {
    private IHostingEnvironment _hostingEnvironment;
    private const int MaxBlockSize = 1000000;

    private IConfiguration Config { get; }

    public UploadController (IHostingEnvironment hostingEnvironment, IConfiguration config)
    {
      _hostingEnvironment = hostingEnvironment;
      Config = config;
    }

    [HttpPost, DisableRequestSizeLimit, RequestSizeLimit(long.MaxValue)]
    public async Task<ActionResult> UploadFile ()
    {
      try
      {
        foreach (var file in Request.Form.Files)
        {
          if (file.Length > 0)
          {
            string fileName = Path.GetFileName(file.FileName);
            using(var ms = new MemoryStream())
            {
              file.CopyTo(ms);
              await UploadBlob(ms.ToArray(), string.Concat(Config["BlobService:StorageUrl"], $"{Guid.NewGuid()}{fileName}"));
            }
          }
        }
        return Json("Upload Successful.");
      }
      catch (System.Exception ex)
      {
        return Json("Upload Failed: " + ex.Message);
      }
    }

    [HttpPost("Data/{userName}"), DisableRequestSizeLimit, RequestSizeLimit(long.MaxValue)]
    public ActionResult UploadFileJson (string userName)
    {
      try
      {
        Checking check = new Checking();
        check = JsonConvert.DeserializeObject<Checking>(Request.Form["key"]);

        return Ok(check);
      }
      catch (Exception ex)
      {
        return BadRequest(ex);
      }
    }

    private async Task<Uri> UploadBlob (byte[] fileContent, string url)
    {
      var creds = new StorageCredentials(Config["BlobService:Account"], Config["BlobService:Key"]);
      var blob = new CloudBlockBlob(new Uri(url), creds);

      HashSet<string> blocklist = new HashSet<string>();
      foreach (FileBlock block in GetFileBlocks(fileContent))
      {
        await blob.PutBlockAsync(
               block.Id,
               new MemoryStream(block.Content, true),
               null
               );

        blocklist.Add(block.Id);
      }

      await blob.PutBlockListAsync(blocklist);

      return blob.Uri;
    }

    private IEnumerable<FileBlock> GetFileBlocks (byte[] fileContent)
    {
      HashSet<FileBlock> hashSet = new HashSet<FileBlock>();
      if (fileContent.Length == 0)
        return new HashSet<FileBlock>();

      int blockId = 0;
      int ix = 0;

      int currentBlockSize = MaxBlockSize;

      while (currentBlockSize == MaxBlockSize)
      {
        if ((ix + currentBlockSize) > fileContent.Length)
          currentBlockSize = fileContent.Length - ix;
        byte[] chunk = new byte[currentBlockSize];
        Array.Copy(fileContent, ix, chunk, 0, currentBlockSize);

        hashSet.Add(
            new FileBlock()
            {
              Content = chunk,
              Id = Convert.ToBase64String(System.BitConverter.GetBytes(blockId))
            });

        ix += currentBlockSize;
        blockId++;
      }

      return hashSet;
    }
  }

  internal class FileBlock
  {
    public string Id
    {
      get;
      set;
    }

    public byte[] Content
    {
      get;
      set;
    }
  }

  public class Checking
  {
    public string UserId { get; set; }
    public List<string> FileNames { get; set; }
  }
}
