using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StarWarsDatabankApi.Models;
using Neo4j.Driver.V1;

namespace StarWarsDatabankApi.Controllers
{
    
    [Route("/api/[controller]")]
    public class TvShowController : Controller
    {
        string driverURL = null;
        string username = null;
        string password = null;
        readonly IDriver _driver;
        public TvShowController(IConfiguration Configuration)
        {
            driverURL = Configuration.GetSection("Neo4j.Driver").Value;
            username = Configuration.GetSection("Neo4j.Username").Value;
            password = Configuration.GetSection("Neo4j.Password").Value;
            _driver = GraphDatabase.Driver(driverURL, AuthTokens.Basic(username, password));
        }
        // GET api/tvshow
        [HttpGet]
        public IEnumerable<NameIds> getAllTvShows()
        {
            using (var session = _driver.Session())
            {
                var result = session.Run("MATCH (n:TvShow) RETURN n.name as name, ID(n) as id");
                List<NameIds> nameIdList = new List<NameIds>();
                foreach(var record in result)
                {
                    NameIds nameId = new NameIds();
                    nameId.name = record["name"].As<string>();
                    nameId.id = record["id"].As<long>();
                    nameIdList.Add(nameId);
                }
                return nameIdList;
            }
        }

        // GET api/tvshow/{id}
        [HttpGet("{id}")]
        public TvShow getTvShowById(long id)
        {
            using (var session = _driver.Session())
            {
                var result = session.Run("MATCH (n:TvShow) WHERE ID(n) = {id}" + 
                " RETURN ID(n) AS id," +
                " n.name as name," +
                " n.seasons as seasons," +
                " n.creators as creators," +
                " n.number_episodes as number_episodes," +
                " n.rating as rating," +
                " n.show_start as show_start," +
                " n.show_end as show_end," +
                " n.stars as stars," +
                " n.tags as tags" , new Dictionary<string, object> {{"id", id}});
                var record = result.Peek();
                if(record == null) {
                    return null;
                } else {
                    TvShow tvshow = new TvShow();
                    tvshow.id = record["id"].As<long>();
                    tvshow.name = record["name"].As<string>();
                    tvshow.seasons = record["seasons"].As<int>();
                    tvshow.creators = record["creators"].As<List<string>>();
                    tvshow.number_episodes = record["number_episodes"].As<int>();
                    tvshow.rating = record["rating"].As<string>();
                    tvshow.show_start = record["show_start"].As<int>();
                    tvshow.show_end = record["show_end"].As<int>();
                    tvshow.stars = record["stars"].As<List<string>>();
                    tvshow.tags = record["tags"].As<List<string>>();
                    return tvshow;
                }
            }
        }

        // GET api/tvshow/{id}/{entityType}
        [HttpGet("{id}/{entityType}")]
        public IEnumerable<NameIds> getEntitiesOfTypeInShow(long id, string entityType)
        {
            using (var session = _driver.Session())
            {
                var statement = "MATCH (n:TvShow)<-[:Appearances]-(b) WHERE ID(n) = {id} AND {entityType} IN LABELS(b) RETURN b.name as name, ID(b) as id";
                Dictionary<string, object> keys = new Dictionary<string, object>()
                {
                    {"id", id}, {"entityType", entityType}
                };
                var result = session.Run(statement, keys);
                List<NameIds> nameIdList = new List<NameIds>();
                foreach(var record in result)
                {
                    NameIds nameId = new NameIds();
                    nameId.name = record["name"].As<string>();
                    nameId.id = record["id"].As<long>();
                    nameIdList.Add(nameId);
                }
                return nameIdList;
            }
        }
    }
}