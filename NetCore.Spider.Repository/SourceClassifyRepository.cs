using NetCore.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NetCore.Spider.Repository
{
    public class SourceClassifyRepository
    {
        private NetCoreSpriderDB _db;
        public SourceClassifyRepository(NetCoreSpriderDB db)
        {
            this._db = db;
        }

        public IList<SourceClassify> GetSourceClassifyListByParentId(int? id)
        {
            if(!id.HasValue)
            {
                return _db.SourceClassify.Where(x => x.ParentId==0).ToList();
            }
            return _db.SourceClassify.Where(c=>c.ParentId==id.Value).OrderByDescending(c => c.CreatedDate).ToList();
        }

        public IList<SourceClassify> GetSourceClassifyList(Expression<Func<SourceClassify, bool>> where)
        {
            return _db.SourceClassify.Where(where).OrderByDescending(c => c.CreatedDate).ToList();
        }

        public void AddSourceClassify(SourceClassify sourceClassifyDto)
        {
            sourceClassifyDto.CreatedDate = DateTime.Now;
            _db.SourceClassify.Add(sourceClassifyDto);
            _db.SaveChanges();
        }

        public void UpdateSourceClassify(SourceClassify sourceClassifyDto)
        {
            _db.SourceClassify.Update(sourceClassifyDto);
            _db.SaveChanges();
        }
    }
}
