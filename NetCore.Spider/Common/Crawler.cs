using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Microsoft.AspNetCore.SignalR;
using NetCore.Model.Entity;
using NetCore.Model.Model;
using NetCore.Spider.Extensions;
using NetCore.Spider.Hubs;
using NetCore.Spider.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetCore.Spider.Common
{
    public class Crawler
    {
        private HtmlParser htmlParser = new HtmlParser();
        private _1024xpRepository _xpRepository = null;
        private IHubContext<_1024xpHub> _signalHub = null;
        public Crawler()
        { }

        public Crawler(_1024xpRepository xpRepository, IHubContext<_1024xpHub> chatHub)
        {
            _xpRepository = xpRepository;
            _signalHub = chatHub;
        }


        public void CrawlerCore(string indexURL)
        {
            try
            {
                LoadCrawHistory(1);
                Step(ConfigStepAction(1), indexURL);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CrawlerCoreAsync(string rootUrl)
        {
            try
            {
                LoadCrawHistory(1);
                //var stepAction = ConfigStepAction(1);
                await StepAsync(ConfigStepAction(1), rootUrl);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void LoadCrawHistory(int actionCode)
        {
            var history = _xpRepository.GetCrawHistory();
            foreach (var item in history)
            {
                ReadUrls.Add(CreateCacheHistroy(item.Url, item.LinkStepCode));
            }
        }

        private async Task LoadCrawHistoryAsync()
        {
            var history = await _xpRepository.GetCrawHistoryAsync();
            foreach (var item in history)
            {
                ReadUrls.Add(CreateCacheHistroy(item.Url, item.LinkStepCode));
            }
        }

        private static HashSet<string> ReadUrls = new HashSet<string>();

        private static List<CrawHistory> CrawHistoriesCache = new List<CrawHistory>();

        private void SaveCrawHistoryDB(string url, int stepActionId)
        {
            var craw = new CrawHistory() { LinkStepCode = stepActionId, Url = url };
            CrawHistoriesCache.Add(craw);
            if (CrawHistoriesCache.Count >= 20)
            {
                _xpRepository.AddCrawHistory(CrawHistoriesCache);
                CrawHistoriesCache.Clear();
            }
        }

        private string CreateCacheHistroy(string url, int stepActionId)
        {
            return $"{stepActionId} : {url}";
        }

        private bool ValidateHasRequest(string cacheUrl)
        {
            return !ReadUrls.Contains(cacheUrl);
        }

        private void WriteUrlToCache(string cacheUrl)
        {
            ReadUrls.Add(cacheUrl);
        }

        private static List<Link> LinksCache = new List<Link>();
        private async Task SaveTargetPageRangeDBAsync(StepAction stepAction, CrawActionModel crawModel)
        {
            try
            {
                if (stepAction.TagContent)
                {
                    var link = CreateLinkModel(crawModel.PageHtml, crawModel.Title, crawModel.Url, stepAction.RegexList);
                    if (link.PageContexts.Any())
                    {
                        LinksCache.Add(link);
                        var sendModel = new _1024xCrawModel()
                        {
                            PageTitle = link.PageTitle,
                            Size = crawModel.PageLength,
                            SourceLink = crawModel.Url,
                            SubTitleCount = link.PageContexts.Count(),
                            Time = crawModel.Time,
                            TotalPageCount = ReadUrls.Count()
                        };
                        await _1024xpHub.SendContentToClient(_signalHub, sendModel);
                        if (LinksCache.Count() > 10)
                        {
                            var saveCount = await _xpRepository.AddLinkRangeAsync(LinksCache);
                            if (saveCount > 0)
                            {
                                LinksCache.Clear();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private async Task SaveTargetPageDBAsync(StepAction stepAction, string html, string title, string url)
        {
            if (stepAction.TagContent)
            {
                var link = CreateLinkModel(html, title, url, stepAction.RegexList);
                if (link.PageContexts.Any())
                {
                    //await _chatHub.SendMessage(link.PageTitle, link.PageContexts.Count().ToString());
                    await _xpRepository.AddLinkAsync(link);
                }
            }
        }

        private Link CreateLinkModel(string html, string pageTitle, string url, IList<string> regexList)
        {
            try
            {
                Link link = new Link();
                link.CatalogType = 1;
                link.LinkTypeCode = 1;
                link.PageTitle = pageTitle.SubstringbyLength(500);
                link.Url = url.SubstringbyLength(500);
                link.PageContexts = new List<PageContext>();
                if (string.IsNullOrEmpty(html)) return link;
                if (regexList.Any())
                {
                    foreach (var regex in regexList)
                    {
                        foreach (Match match in Regex.Matches(html, regex))
                        {
                            //var context = match.Value.SubstringbyLength(500);
                            link.PageContexts.Add(new PageContext() { Context = match.Value.SubstringbyLength(500) });
                        }
                    }
                }
                return link;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task QuerySelectorsByAction(IHtmlDocument htmlDocument, StepAction stepAction)
        {
            try
            {
                foreach (var query in stepAction.QuerySelectorList)
                {
                    var elementList = htmlDocument.QuerySelectorAll(query).ToList();
                    if (!string.IsNullOrEmpty(stepAction.InnerTextContain))
                    {
                        elementList = elementList.Where(x => x.InnerHtml.Contains(stepAction.InnerTextContain)).ToList();
                    }
                    foreach (var element in elementList)
                    {
                        await CrawChildrenStepAction(stepAction, GetElementLink(element, stepAction));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string ValidateUrl(AngleSharp.Dom.IElement element, string attribute)
        {
            var href = element.GetAttribute(attribute);
            if (!href.StartsWith("http"))
            {
                href = element.BaseUri + href;
            }
            return href;
        }

        private string GetElementLink(AngleSharp.Dom.IElement element, StepAction stepAction)
        {
            string attribute = "href";
            if (!string.IsNullOrEmpty(stepAction.Attribute))
            {
                attribute = stepAction.Attribute;
            }
            return ValidateUrl(element, attribute);
        }

        private async Task CrawChildrenStepAction(StepAction stepAction, string url)
        {
            foreach (var childAction in stepAction.ChildrenActions)
            {
                await StepAsync(childAction, url);
            }
        }

        private async Task StepAsync(StepAction stepAction, string url)
        {
            try
            {
                string cacheUrl = CreateCacheHistroy(url, stepAction.StepId);
                if (ValidateHasRequest(cacheUrl))
                {
                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();
                    var html = await HttpHelper.GetHtmlSourceCodeAsync(url);
                    WriteUrlToCache(cacheUrl);
                    SaveCrawHistoryDB(url, stepAction.StepId);
                    var htmlDom = htmlParser.Parse(html);

                    stopwatch.Stop(); //  停止监视
                    //TimeSpan timeSpan = stopwatch.Elapsed; //  获取总时间
                    //double milliseconds = timeSpan.TotalMilliseconds;
                    var crawModel = new CrawActionModel()
                    {
                        PageHtml = html,
                        Url = url,
                        Title = htmlDom.Title,
                        Time = stopwatch.Elapsed.Seconds
                    };
                    await SaveTargetPageRangeDBAsync(stepAction, crawModel);
                    await QuerySelectorsByAction(htmlDom, stepAction);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Step(StepAction stepAction, string url)
        {
            string cacheUrl = CreateCacheHistroy(url, stepAction.StepId);// stepAction.StepId + ":" + url;
            if (!ReadUrls.Contains(cacheUrl))
            {
                var html = HttpHelper.GetHtmlSourceCode(url);
                ReadUrls.Add(cacheUrl);
                SaveCrawHistoryDB(url, stepAction.StepId);
                var htmlDom = htmlParser.Parse(html);
                if (stepAction.TagContent)
                {
                    //create link entity
                    Link link = new Link();
                    link.CatalogType = 1;
                    link.LinkTypeCode = 1;
                    link.PageTitle = htmlDom.Title;
                    link.Url = url;
                    link.PageContexts = new List<PageContext>();
                    if (stepAction.RegexList.Any())
                    {
                        foreach (var regex in stepAction.RegexList)
                        {
                            foreach (Match match in Regex.Matches(html, regex))
                            {
                                link.PageContexts.Add(new PageContext() { Context = match.Value });
                            }
                        }

                    }
                    //save
                    if (link.PageContexts.Any())
                    {
                        _xpRepository.AddLink(link);
                    }
                }
                if (stepAction.QuerySelectorList.Any())
                {
                    foreach (var querySelector in stepAction.QuerySelectorList)
                    {
                        if (querySelector == "div.pages > a")
                        {

                        }
                        //htmlDom.QuerySelector
                        var elementList = htmlDom.QuerySelectorAll(querySelector).ToList();
                        if (!string.IsNullOrEmpty(stepAction.InnerTextContain))
                        {
                            elementList = elementList.Where(x => x.InnerHtml.Contains(stepAction.InnerTextContain)).ToList();
                        }
                        if (elementList.Any())
                        {
                            foreach (var element in elementList)
                            {
                                if (!string.IsNullOrEmpty(stepAction.Attribute))
                                {
                                    var href = element.GetAttribute(stepAction.Attribute);
                                    if (!href.StartsWith("http"))
                                    {
                                        href = element.BaseUri + href;
                                    }
                                    if (stepAction.ChildrenActions != null && stepAction.ChildrenActions.Any())
                                    {
                                        foreach (var action in stepAction.ChildrenActions)
                                        {
                                            Step(action, href);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private StepAction ConfigStepAction(int stepId)
        {
            StepAction stepAction = new StepAction();
            //stepAction.QuerySelectorList = new List<string>();
            stepAction.QuerySelectorList.Add("#fid_2 a");
            stepAction.StepId = 1;
            stepAction.InnerTextContain = "最新合集";
            stepAction.Attribute = "href";

            StepAction stepAction1 = new StepAction();
            //stepAction.ChildrenActions = new List<StepAction>();
            stepAction.ChildrenActions.Add(stepAction1);
            stepAction1.Attribute = "href";
            stepAction1.StepId = 2;
            //stepAction1.QuerySelectorList = new List<string>();
            stepAction1.QuerySelectorList.Add("a");
            stepAction1.InnerTextContain = "[";

            StepAction stepAction11 = new StepAction();
            stepAction11.StepId = 3;
            stepAction.ChildrenActions.Add(stepAction11);
            stepAction11.QuerySelectorList.Add("div.pages > a");
            stepAction11.Attribute = "href";
            stepAction11.StepName = "page";
            //stepAction11.InnerTextContain

            StepAction stepAction2 = new StepAction();
            stepAction2.StepId = 4;
            stepAction1.ChildrenActions.Add(stepAction2);
            stepAction11.ChildrenActions.Add(stepAction1);
            stepAction11.ChildrenActions.Add(stepAction11);
            stepAction2.TagContent = true;
            stepAction2.RegexList.Add(@"影片名称.*?<br");
            stepAction2.RegexList.Add(@"影片名稱.*?<br");
            return stepAction;
        }
    }

   
}
