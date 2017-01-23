using SignalBusinessLayer;
using SignalViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SignalAPI.Controllers
{
    public class EnumListItemsController : ApiController
    {
        EnumListItemsProcessor eProc = new EnumListItemsProcessor();

        public List<EnumListItem> GetEnumListItems([FromUri] string DBName, [FromUri] string tablename)
        {
            return eProc.GetEnumListItems(DBName, tablename);
        }
    }
}