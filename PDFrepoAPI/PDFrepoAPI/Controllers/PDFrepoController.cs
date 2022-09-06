using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Threading;

using System.Web.Http.Cors;
using PDFrepoAPI.Models;
using System.Web.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http.Headers;
using System.Diagnostics;


namespace PDFrepoAPI.Controllers
{
    [EnableCorsAttribute("*", "*", "*")]

    public class PDFrepoController : ApiController
    {
        [System.Web.Http.HttpGet]
        //[DisableCors]
        //[RequiredHttps]
        [BasicAthuntication]
        public HttpResponseMessage checkAuthontication()
        {
            PDFsystemEntities1 ent = new PDFsystemEntities1();
            string myusername = Thread.CurrentPrincipal.Identity.Name;
            var _id = ent.users_TBL.Where(x => x.username == myusername).Select(w => w.id).FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.OK, _id);
        }




        [System.Web.Http.HttpPost]
        //[BasicAthuntication]

        public async Task<HttpResponseMessage> SaveData()
        {

            var note = "";
            string _id = "";
            //Create HTTP Response.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            //Check if Request contains File.
            if (HttpContext.Current.Request.Files.Count == 0)
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            string root = HttpContext.Current.Server.MapPath("~/App_Data");

            var provider = new MultipartFormDataStreamProvider(root);
            await Request.Content.ReadAsMultipartAsync(provider);

            foreach (var key in provider.FormData.AllKeys)
            {
                if (key == "note")
                {
                    var val = provider.FormData.GetValues(key);
                    note = val[0].ToString();
                }
                else if (key == "id")
                {
                    var val = provider.FormData.GetValues(key);
                    _id = val[0].ToString();
                    if (_id == "")
                    { return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Unauthorized,Authontication Required" }); }

                }
              
            }
            //Read the File data from Request.Form collection.
            HttpPostedFile postedFile = HttpContext.Current.Request.Files[0];

            //Convert the File data to Byte Array.
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedFile.InputStream))
            {
                bytes = br.ReadBytes(postedFile.ContentLength);
            }
            //Name = Path.GetFileName(postedFile.FileName),

            //Insert the File to Database Table.
            PDFsystemEntities1 entities = new PDFsystemEntities1();
            PDFrepository_TBL file = new PDFrepository_TBL
            {
                //Name = Path.GetFileName(postedFile.FileName),
                //ContentType = postedFile.ContentType,
                pdf = bytes,
                note = note,
                FileName = Path.GetFileName(postedFile.FileName),
                userID = Convert.ToInt32(_id),
                contentType = postedFile.ContentType,


            };
            if (file.note == "" || file.pdf.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "data not inserted, make sure to fill note and file" });
            }
            else
            {
                entities.PDFrepository_TBL.Add(file);
                entities.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, new { messag = "data inserted successfuly" });
     
            }


        }

        [System.Web.Http.HttpPost]
        [BasicAthuntication]

        public HttpResponseMessage GetFiles()
        {
            string myusername = Thread.CurrentPrincipal.Identity.Name;

            PDFsystemEntities1 entities = new PDFsystemEntities1();

            if (myusername == "admin")
            {
                var files = from file in entities.PDFrepository_TBL
                            join users in entities.users_TBL
                             on file.userID equals users.id

                            select new { _id = file.id, _Name = file.FileName, _note = file.note, _username = users.username };
                return Request.CreateResponse(HttpStatusCode.OK, files);

            }
            else
            {
                var files = (from file in entities.PDFrepository_TBL
                             join users in entities.users_TBL
                              on file.userID equals users.id

                             select new { _id = file.id, _Name = file.FileName, _note = file.note, _username = users.username }).Where(x => x._username == myusername);

               
                return Request.CreateResponse(HttpStatusCode.OK, files);

            }


        }
        [System.Web.Http.HttpGet]

        public HttpResponseMessage GetFile(int fileId)
        {
            //Create HTTP Response.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            //Fetch the File data from Database.
            PDFsystemEntities1 entities = new PDFsystemEntities1();
            PDFrepository_TBL file = entities.PDFrepository_TBL.ToList().Find(p => p.id == fileId);

            //Set the Response Content.
            response.Content = new ByteArrayContent(file.pdf);

            //Set the Response Content Length.
            response.Content.Headers.ContentLength = file.pdf.LongLength;

            //Set the Content Disposition Header Value and FileName.
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = file.FileName;

            //Set the File Content Type.
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(file.contentType);
            return response;
        }
        

    }
}
