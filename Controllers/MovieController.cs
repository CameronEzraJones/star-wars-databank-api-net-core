using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StarWarsDatabankApi.Models;
using Neo4j.Driver.V1;

namespace StarWarsDatabankApi.Controllers
{
    
    [Route("/api/[controller]")]
    public class MovieController : Controller
    {
        string driverURL = null;
        string username = null;
        string password = null;
        readonly IDriver _driver;
        public MovieController(IConfiguration Configuration)
        {
            driverURL = Configuration.GetSection("Neo4j.Driver").Value;
            username = Configuration.GetSection("Neo4j.Username").Value;
            password = Configuration.GetSection("Neo4j.Password").Value;
            _driver = GraphDatabase.Driver(driverURL, AuthTokens.Basic(username, password));
        }
        // GET api/movie
        [HttpGet]
        public IEnumerable<NameIds> getAllMovies()
        {
            using (var session = _driver.Session())
            {
                var result = session.Run("MATCH (n:Movie) RETURN n.name as name, ID(n) as id");
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

        // GET api/movie/{id}
        [HttpGet("{id}")]
        public Movie getMovieById(long id)
        {
            using (var session = _driver.Session())
            {
                var result = session.Run("MATCH (n:Movie) WHERE ID(n) = {id}" + 
                " RETURN ID(n) AS id," +
                " n.name as name," +
                " n.director as director," +
                " n.rating as rating," +
                " n.runtime as runtime," +
                " n.writers as writers," +
                " n.stars as stars," +
                " n.released as released," +
                " n.tags as tags" , new Dictionary<string, object> {{"id", id}});
                var record = result.Peek();
                if(record == null) {
                    return null;
                } else {
                    Movie movie = new Movie();
                    movie.id = record["id"].As<long>();
                    movie.name = record["name"].As<string>();
                    movie.director = record["director"].As<string>();
                    movie.rating = record["rating"].As<string>();
                    movie.runtime = record["runtime"].As<string>();
                    movie.writers = record["writers"].As<List<string>>();
                    movie.stars = record["stars"].As<List<string>>();
                    movie.released = record["released"].As<long>();
                    movie.tags = record["tags"].As<List<string>>();
                    return movie;
                }
            }
        }

        // GET api/movie/{id}/{entityType}
        [HttpGet("{id}/{entityType}")]
        public IEnumerable<NameIds> getEntitiesOfTypeInMovie(long id, string entityType)
        {
            using (var session = _driver.Session())
            {
                var statement = "MATCH (n:Movie)<-[:Appearances]-(b) WHERE ID(n) = {id} AND {entityType} IN LABELS(b) RETURN b.name as name, ID(b) as id";
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