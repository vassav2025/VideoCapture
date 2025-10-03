using System.Collections.Generic;
using System.Web.Mvc;

namespace VideoCapture.Models
{
    public class ImageModel
    {
        public bool IsRunning { get; set; }

        public List<SelectListItem> DeviceList { get; set; }
    }
}