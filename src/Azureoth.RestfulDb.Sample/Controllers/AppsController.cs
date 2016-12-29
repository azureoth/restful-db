﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Azureoth.RestfulDb.Sample.Controllers
{
    [Route("api/[controller]")]
    public class AppsController : Controller
    {
        // GET api/apps/5/channels/
        [HttpGet("{appId}/{table}")]
        public IEnumerable<object> Get(string appId, string table, [FromServices] IDatabaseService database)
        {
            return database.Get(appId, table);
        }

        // GET api/apps/5/channels/5
        [HttpGet("{appId}/{table}/{id}")]
        public object Get(string appId, string table, int id, [FromServices] IDatabaseService database)
        {
            return database.Get(appId, table, id);
        }

        // POST api/apps/5/channels
        [HttpPost("{appId}/{table}")]
        public decimal Post(string appId, string table, [FromBody] IDictionary<string, object> data, [FromServices] IDatabaseService database)
        {
            return database.Insert<decimal>(appId, table, data);
        }

        // PUT api/apps/5/channels
        [HttpPut("{appId}/{table}/{id}")]
        public void Put(string appId, string table, int id, [FromBody] IDictionary<string, object> data, [FromServices] IDatabaseService database)
        {
            database.Update(appId, table, id, data);
        }

        // DELETE api/apps/5/channels/5
        [HttpDelete("{appId}/{table}/{id}")]
        public void Delete(string appId, string table, int id, [FromServices] IDatabaseService database)
        {
            database.Delete(appId, table, id);
        }
    }
}