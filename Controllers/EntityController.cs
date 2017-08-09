using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Neo4j.Driver.V1;
using StarWarsDatabankApi.Models;

namespace StarWarsDatabankApi.Controllers
{
    [Route("/api/[controller]")]
    public class EntityController : Controller
    {
        string driverURL = null;
        string username = null;
        string password = null;
        readonly IDriver _driver;
        public EntityController(IConfiguration Configuration)
        {
            driverURL = Configuration.GetSection("Neo4j.Driver").Value;
            username = Configuration.GetSection("Neo4j.Username").Value;
            password = Configuration.GetSection("Neo4j.Password").Value;
            _driver = GraphDatabase.Driver(driverURL, AuthTokens.Basic(username, password));
        }

        // GET api/entity/{id}
        [HttpGet("{id}")]
        public Entity getEntityById(long id)
        {
            using(var session = _driver.Session())
            {
                string statement = "MATCH (n) WHERE ID(n) = {id} AND NOT \"TvShow\" IN Labels(n) AND NOT \"Movie\" IN Labels(n) RETURN" +
                " ID(n) as id," +
                " n.name as name," +
                " n.description as description," +
                " n.image_link as image_link," +
                " n.databank_link as databank_link";
                Dictionary<string, object> keys = new Dictionary<string, object>() {{"id", id}};
                var result = session.Run(statement, keys);
                var record = result.Peek();
                if(record == null) {
                    return null;
                } else {
                    Entity entity = new Entity();
                    entity.id = record["id"].As<long>();
                    entity.name = record["name"].As<string>();
                    entity.description = record["description"].As<string>();
                    entity.image_link = record["image_link"].As<string>();
                    entity.databank_link = record["databank_link"].As<string>();
                    return entity;
                }
            }
        }
    }
}