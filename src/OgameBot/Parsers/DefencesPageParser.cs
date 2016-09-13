using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using OgameBot.Objects.Types;
using OgameBot.Parsers.Objects;
using OgameBot.Utilities;
using ScraperClientLib.Engine;

namespace OgameBot.Parsers
{
    public class DefencesPageParser : BaseParser
    {
        private static Regex CssRegex = new Regex(@"defense[\d]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override bool ShouldProcessInternal(ResponseDocument document)
        {
            return document.RequestMessage.RequestUri.Query.Contains("page=defense");
        }

        public override IEnumerable<DataObject> ProcessInternal(ClientBase client, ResponseDocument document)
        {
            HtmlDocument doc = document.ResponseHtml.Value;
            HtmlNodeCollection imageFields = doc.DocumentNode.SelectNodes("//div[@id='buttonz']/div[@class='content']//div[contains(@class, 'defense')]");

            if (imageFields == null)
                yield break;

            foreach (HtmlNode node in imageFields)
            {
                string cssClss = node.GetCssClasses(CssRegex).FirstOrDefault();

                Defence type;
                switch (cssClss)
                {
                    case "defense401":
                        type = Defence.RocketLauncher;
                        break;
                    case "defense402":
                        type = Defence.LightLaser;
                        break;
                    case "defense403":
                        type = Defence.HeavyLaser;
                        break;
                    case "defense404":
                        type = Defence.GaussCannon;
                        break;
                    case "defense405":
                        type = Defence.IonCannon;
                        break;
                    case "defense406":
                        type = Defence.PlasmaTurret;
                        break;
                    case "defense407":
                        type = Defence.SmallShieldDome;
                        break;
                    case "defense408":
                        type = Defence.LargeShieldDome;
                        break;
                    //case "defense502":
                    //    type = Defence.AntiBallisticMissile;
                    //    break;
                    //case "defense503":
                    //    type = Defence.InterplanetaryMissile;
                    //    break;
                    default:
                        continue;
                }

                string countText = node.SelectSingleNode(".//span[@class='level']").ChildNodes.Last(s => s.NodeType == HtmlNodeType.Text).InnerText;
                int count = int.Parse(countText, NumberStyles.Integer | NumberStyles.AllowThousands, client.ServerCulture);

                yield return new DetectedDefence
                {
                    Building = type,
                    Count = count
                };
            }
        }
    }
}