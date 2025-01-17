﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Gehtsoft.Test.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Save(string value = "")
        {
            HttpContext.Session.SetString("value", value);
            return RedirectToAction(nameof(Load));
        }

        [HttpPost]
        public IActionResult PostSave(string value = "")
        {
            HttpContext.Session.SetString("value", value);
            return RedirectToAction(nameof(Load));
        }


        [HttpPost]
        public IActionResult PostAdd(string value = "")
        {
            var oldValue = HttpContext.Session.GetString("value");
            HttpContext.Session.SetString("value", oldValue + value);
            return RedirectToAction(nameof(Load));
        }

        public IActionResult Load()
        {
            var value = HttpContext.Session.GetString("value");
            return View();
        }

        [HttpPost]
        public IActionResult Upload(string id, IEnumerable<IFormFile> files)
        {
            List<int> sizes = new List<int>();
            List<string> hashes = new List<string>();

            foreach (var file in files)
            {
                var s = file.OpenReadStream();
                using var ms = new MemoryStream();
                s.CopyTo(ms);
                var b = ms.ToArray();
                sizes.Add(b.Length);
                var hash = MD5.HashData(b);
                hashes.Add(Convert.ToHexString(hash));
            }

            return Json(new
            {
                id = id,
                count = sizes.Count,
                sizes = sizes.ToArray(),
                hashes = hashes.ToArray()
            });
        }
        
    }
}
