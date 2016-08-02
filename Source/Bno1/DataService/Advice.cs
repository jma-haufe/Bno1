using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace transmate.DataService
{
    public class Advice
    {
        private List<CheckListItem> _checkListItems;
        private string _fees;
        private Office _office;
        private Guid _officeId;

        public Advice()
        {
            _checkListItems = new List<CheckListItem>();
            this.Advices = new List<Advice>();
        }

        public List<Advice> Advices { get; set; }
        
        public Guid AdviceId { get; set; }
        
        public string Caption { get; set; }

        public string Text { get; set; }

        public string Fees
        {
            get
            {
                if (String.IsNullOrEmpty(_fees))
                {
                    return "No fees.";
                }
                return _fees;
            }
            set { _fees = value; }
        }

        [XmlIgnore]
        public Office Office {
            get
            {
                if (this._office == null && this._officeId!= Guid.Empty)
                {
                    this._office = DataService.Instance.GetOfficeById(this._officeId);
                }
                return this._office;
            }
            set { this._office = value; }
        }

        public Guid OfficeId
        {
            get
            {
                if (Office == null)
                {
                    if (this._officeId == Guid.Empty) return Guid.Empty;
                    return _officeId;
                }
                return Office.OfficeId;
            }
            set
            {
                if (value==Guid.Empty) return;
                this._officeId = value;
            }
        }

        public int DurationInMinutes { get; set; }

        public List<CheckListItem> CheckListItems
        {
            get
            {                
                return _checkListItems;
            } 
            set { _checkListItems = value; }
        }

        public override string ToString()
        {
            return this.Caption;
        }
    }
}
