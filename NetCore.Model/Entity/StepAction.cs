using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Model.Entity
{
    public class StepAction
    {
        public StepAction()
        {
            QuerySelectorList = new List<string>();
            ChildrenActions = new List<StepAction>();
            RegexList = new List<string>();
        }

        public int StepId { get; set; }
        public string StepName { get; set; }
        public bool TagContent { get; set; }

        public IList<string> RegexList { get; set; }

        public string InnerTextContain { get; set; }

        public string Attribute { get; set; }

        public List<string> QuerySelectorList { get; set; }

        public HashSet<string> StepUrlConllect { get; set; }

        public void AddStepUrl(string url)
        {
            if (StepUrlConllect == null)
            {
                StepUrlConllect = new HashSet<string>();
            }
            if (!StepUrlConllect.Contains(url))
            {
                StepUrlConllect.Add(url);
            }
        }

        public List<StepAction> ChildrenActions { get; set; }
    }
}
