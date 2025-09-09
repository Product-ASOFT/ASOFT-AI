using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Users.Entities.ViewModels
{
    public class ERPXMenuItem
    {
        private bool _Enable = true;
        private bool _Visible = true;
        public ERPXMenuItem()
        {
        }

        public string Controller { get; set; }
        public string Action { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public string ParentID { get; set; }
        public int Level { get; set; }
        public string ModuleID { get; set; }
        public int MenuOrder { get; set; }
        public bool Selected { get; set; }
        public string ImageUrl { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsHidden { get; set; }
        public bool Enable
        {
            get { return _Enable; }
            set { _Enable = value; }
        }
        public bool Visible
        {
            get { return _Visible; }
            set { _Visible = value; }
        }
        public string Target { get; set; }
        public List<ERPXMenuItem> Items { get; set; }
    }
}
