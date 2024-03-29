﻿using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using MyFirstWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyFirstWebsite.Services
{
    public class FantasyProsDataGrabber : IFantasyProsDataGrabber
    {
        private readonly IConfiguration configuration;

        public FantasyProsDataGrabber(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public ICollection<Player> GetPlayers(ScoringType scoringType)
        {
            string url;

            if (scoringType.Equals(ScoringType.HalfPPR))
            {
                url = configuration["FantasyPros:HalfPointPPR"];
            }
            else if (scoringType.Equals(ScoringType.PPR))
            {
                url = configuration["FantasyPros:PPR"];
            }
            else
            {
                url = configuration["FantasyPros:Standard"];
            }

            HashSet<Player> players = new HashSet<Player>();

            HtmlWeb htmlWeb = new HtmlWeb();

            HtmlDocument doc;

            try
            {
                doc = htmlWeb.Load(url);
            }
            catch (Exception)
            {
                return players;
            }
            
            var node = doc.DocumentNode.SelectNodes("//*[@id=\"data\"]/tbody/tr");

            int intRank = 0;
            foreach (var child in node)
            {
                var rank = child.ChildNodes[0].InnerText;
                try
                {
                    int.TryParse(rank, out intRank);
                }
                catch (Exception)
                {
                }
                var name = child.ChildNodes[2].InnerText;
                var position = child.ChildNodes[4].InnerText;
                var link = "https://fantasypros.com" + child.ChildNodes[2].FirstChild.Attributes.AttributesWithName("href").FirstOrDefault().Value;

                players.Add(new Player { Rank = intRank, Name = name, Position = position, PlayerUrl = link });

            }

            return players;
        }

        public string GetUrl(ScoringType scoringType)
        {
            var result = configuration["FantasyPros:Standard"];

            if (scoringType.Equals(ScoringType.HalfPPR))
                result = configuration["FantasyPros:HalfPointPPR"];
            else if (scoringType.Equals(ScoringType.PPR))
                result = configuration["FantasyPros:PPR"];

            return result;
        }
    }
}
