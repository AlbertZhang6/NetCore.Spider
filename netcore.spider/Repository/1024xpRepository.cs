using Microsoft.EntityFrameworkCore;
using NetCore.Spider.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Spider.Repository
{
    public class _1024xpRepository : IDisposable
    {
        private NetCoreSpriderDBContext _netCoreSpriderDB = null;

        //private readonly DbContextOptions _options;
        public _1024xpRepository(NetCoreSpriderDBContext netCoreSpriderDB)
        {
            _netCoreSpriderDB = netCoreSpriderDB;
        }

        //public override async Task<bool> SaveAsync(Link entity)
        //{
        //    using (var context = new NetCoreSpriderDB())
        //    {
        //        context.Set<Content>().Add(entity);
        //        return await context.SaveChangesAsync() > 0;
        //    }
        //}

        public  IEnumerable<CrawHistory> GetCrawHistory()
        {
            try
            {
                using (var context = new NetCoreSpriderDBContext())
                {
                    ////var b = context.Link.Where(x => x.LinkIa > 0).ToList();
                    //var c = context.CrawHistory.SelectMany(x => { x.CrawId, x.LinkStepCode,x.Url};).ToList();
                    ////x=>new CrawHistory() {  CrawId = x.CrawId, Url = x.Url, LinkStepCode = x.LinkStepCode}
                    //var b =  c.ToList();
                    //var a = context.CrawHistory.Count();
                    if(context.CrawHistory.Any())
                    {
                        return context.CrawHistory.ToList<CrawHistory>();
                    }
                    return new List<CrawHistory>();
                }

                return _netCoreSpriderDB.CrawHistory.Where(x=>x.Id==0).ToList();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        public bool AddCrawHistory(IEnumerable<CrawHistory> crawHistories)
        {
            using (var context = new NetCoreSpriderDBContext())
            {
                context.Set<CrawHistory>().AddRange(crawHistories);
                context.SaveChanges();
                return true;
            }
        }

        public async Task<int> AddLinkAsync(Link link)
        {
            try
            {
                using (var context = new NetCoreSpriderDBContext())
                {
                    context.Set<Link>().Add(link);
                    foreach (var page in link.PageContexts)
                    {
                        page.LinkId = link.Id;
                        context.Set<PageContext>().Add(page);
                    }
                    return await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void Dispose()
        {
            _netCoreSpriderDB.Dispose();
        }
    }
}
