using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TheCodeCamp.Data;

namespace TheCodeCamp.Controllers
{
  public class ValuesController : ApiController
  {
    public IHttpActionResult Get()
    {
      return Ok(new[] { "Hello", "From", "Pluralsight" });
    }

  }
}
