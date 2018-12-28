using AngleSharp.Parser.Html;
using Microsoft.AspNetCore.SignalR;
using NetCore.Model.Entity;
using NetCore.Model.Model;
using NetCore.Spider.Common.Helpers;
using NetCore.Spider.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCore.SpiderCore
{
    public class SpiderCore
    {
        private HtmlParser htmlParser = new HtmlParser();
        private _1024xpRepository _xpRepository = null;
        //private IHubContext<_1024xpHub> _signalHub = null;


        public SpiderCore(_1024xpRepository xpRepository)
        {
            _xpRepository = xpRepository;
            //_signalHub = chatHub;
        }

        private static HashSet<string> ReadUrls = new HashSet<string>();
        private static List<CrawHistory> CrawHistoriesCache = new List<CrawHistory>();

        public async Task StartCrawler(int crawlerId)
        {
            
        }

        private async Task<StepAction> GetCrawlerConfig(int crawlerId)
        {
            return null;
        }


        private async Task CrawStepActionAsync(StepAction stepAction, string tagLink)
        {
            if (HasRequested(tagLink, stepAction.StepId)) return;
            var timeWatcher = new System.Diagnostics.Stopwatch();
            timeWatcher.Start();
             var html = await HttpHelper.GetHtmlSourceAsync(tagLink);
            CacheRequest(tagLink, stepAction.StepId);
            SaveCrawHistory(tagLink, stepAction.StepId);
            var htmlDom = htmlParser.Parse(html);
            timeWatcher.Stop();
            var crawModel = new CrawActionModel()
            {
                PageHtml = html,
                Url = tagLink,
                Title = htmlDom.Title,
                Time = timeWatcher.Elapsed.Seconds
            };
        }
        /// <summary>
        /// Load history
        /// </summary>
        /// <returns></returns>
        private async Task LoadCrawHistoryBycrawlerId()
        {

        }

        private void SaveCrawHistory(string url, int stepActionId)
        {
            var craw = new CrawHistory() { LinkStepCode = stepActionId, Url = url };
            CrawHistoriesCache.Add(craw);
            if (CrawHistoriesCache.Count >= 20)
            {
                _xpRepository.AddCrawHistory(CrawHistoriesCache);
                CrawHistoriesCache.Clear();
            }
        }

        private bool HasRequested(string requestUrl, int stepId)
        {
            return ReadUrls.Contains($"{stepId} : {requestUrl}");
        }

        private void CacheRequest(string requestUrl, int stepId)
        {
            ReadUrls.Add($"{stepId} : {requestUrl}");
        }
    }
}
