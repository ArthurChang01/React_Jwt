using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace React_Jwt.Infra.Services
{
    public static class CrawlerDetector
    {
        public static bool IsCrawlByBot()
        {
            List<string> Crawlers = new List<string>()
        {
            "googlebot","bingbot","yandexbot","ahrefsbot","msnbot","linkedinbot","exabot","compspybot",
            "yesupbot","paperlibot","tweetmemebot","semrushbot","gigabot","voilabot","adsbot-google",
            "botlink","alkalinebot","araybot","undrip bot","borg-bot","boxseabot","yodaobot","admedia bot",
            "ezooms.bot","confuzzledbot","coolbot","internet cruiser robot","yolinkbot","diibot","musobot",
            "dragonbot","elfinbot","wikiobot","twitterbot","contextad bot","hambot","iajabot","news bot",
            "irobot","socialradarbot","ko_yappo_robot","skimbot","psbot","rixbot","seznambot","careerbot",
            "simbot","solbot","mail.ru_bot","spiderbot","blekkobot","bitlybot","techbot","void-bot",
            "vwbot_k","diffbot","friendfeedbot","archive.org_bot","woriobot","crystalsemanticsbot","wepbot",
            "spbot","tweetedtimes bot","mj12bot","who.is bot","psbot","robot","jbot","bbot","bot"
        };

            string ua = HttpContext.Current.Request.UserAgent.ToLower();
            bool iscrawler = Crawlers.Exists(x => ua.Contains(x));
            return iscrawler;
        }
    }
}