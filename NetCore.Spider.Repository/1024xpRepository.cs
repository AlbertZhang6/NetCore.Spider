using Microsoft.EntityFrameworkCore;
using NetCore.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Spider.Repository
{
    public class _1024xpRepository
    {
        private NetCoreSpriderDB _db;

        public _1024xpRepository(NetCoreSpriderDB db)
        {
            this._db = db;
        }

        public IList<CrawHistory> GetCrawHistory()
        {
            return _db.CrawHistory.Where(x => x.Id > 0).ToList();
        }

        public async Task<List<CrawHistory>> GetCrawHistoryAsync()
        {
            try
            {
                using (var dbcontext = new NetCoreSpriderDB())
                {
                    return await dbcontext.CrawHistory.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddCrawHistory(IList<CrawHistory> crawHistories)
        {
            _db.CrawHistory.AddRange(crawHistories);
            _db.SaveChanges();
        }

        public async Task AddCrawHistoryAsync(IList<CrawHistory> crawHistories)
        {
            try
            {
                using (var dbcontext = new NetCoreSpriderDB())
                {
                    await dbcontext.CrawHistory.AddRangeAsync(crawHistories);
                    await dbcontext.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        //public Task<int> AddLinkAsync(Link link)
        //{
        //    try
        //    {
        //        using (var context = new NetCoreSpriderDBContext())
        //        {
        //            context.Set<Link>().Add(link);
        //            foreach (var page in link.PageContexts)
        //            {
        //                page.LinkId = link.Id;
        //                context.Set<PageContext>().Add(page);
        //            }
        //            return await context.SaveChangesAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}
        public  int AddLink(Link link)
        {
            try
            {
                _db.Link.Add(link);
                foreach (var page in link.PageContexts)
                {
                    page.LinkId = link.Id;
                    _db.PageContext.Add(page);
                }
                return _db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> AddLinkRangeAsync(IList<Link> links)
        {
            try
            {
                using (var dbcontext = new NetCoreSpriderDB())
                {
                    foreach (var link in links)
                    {
                        await dbcontext.Link.AddAsync(link);
                        foreach (var page in link.PageContexts)
                        {
                            page.LinkId = link.Id;
                            await dbcontext.PageContext.AddAsync(page);
                        }
                    }
                    return await dbcontext.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> AddLinkAsync(Link link)
        {
            try
            {
                using (var dbcontext = new NetCoreSpriderDB())
                {
                    await dbcontext.Link.AddAsync(link);
                    foreach (var page in link.PageContexts)
                    {
                        page.LinkId = link.Id;
                        await dbcontext.PageContext.AddAsync(page);
                    }
                    return await dbcontext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
