using Model.Business.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Web
{
    public class UIHelper
    {
        public static string FileBox(int fileid, string path, string controller = "UserStorage", string viewaction = "Single", string editaction = "Edit", string deleteaction = "Delete")
        {
            return
"<div class='dms-storage-item-holder'>" +
"    <div class='col-sm-12 col-no-padding'>" +
"        <a href='/" + controller + "/" + viewaction + "/" + fileid + "'>" +
"            <img src='/Content/Images/file.png'/>" +
"        </a>" +
"    </div>" +
"    <div class='col-sm-12 col-no-padding align-center'>" +
"        <a href='/" + controller + "/" + editaction + "/" + fileid + "'>" +
"            Edit" +
"        </a> | " +
"        <a onclick='Delete(" + "\"" + controller + "\", \"" + deleteaction + "\", \"" + "id=" + fileid + "\");'>" +
"            Delete" +
"        </a>" +
"    </div>" +
"    <div class='col-sm-12 col-no-padding align-center'>" + DirectoryHelper.Basename(path) + "</div>" +
"</div>";
        }

        public static string FileVersionBox(int fileid, int version, bool allowdelete = true)
        {
            return
"<div class='dms-storage-item-holder'>" +
"    <div class='col-sm-12 col-no-padding'>" +
"       <img src='/Content/Images/file.png'/>" +
"    </div>" +
"    <div class='col-sm-12 col-no-padding align-center'>" +
"        <a target='_blank' href='/UserStorage/ViewVersion/?fileID=" + fileid + "&version=" + version + "'>" +
"            View" +
"        </a> | " +
"        <a target='_blank' href='/UserStorage/EditVersion/?fileID=" + fileid + "&version=" + version + "'>" +
"            Edit" +
"        </a>" + (allowdelete ?
"        | <a onclick='Delete(\"UserStorage\", \"DeleteVersion\", \"id=" + fileid + "&version=" + version + "\");'>" +
"            Delete" +
"        </a>" : "") +
"    </div>" +
"    <div class='col-sm-12 col-no-padding align-center'>Ver: " + version + "</div>" +
"</div>";
        }
    }
}
