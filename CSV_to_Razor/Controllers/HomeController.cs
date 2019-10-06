using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.IO.FileSystem;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CSV_to_Razor.Models;
using System.IO;

namespace CSV_to_Razor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        private static string csv;

        public IActionResult Load()
        {
            // ToDo: Read from client
            //FileStream fileStream = new FileStream("file.txt", FileMode.Open);
            //using (StreamReader reader = new StreamReader(fileStream))
            //{
            //    string line = reader.ReadLine();
            //}

            csv = System.IO.File.ReadAllText("C:\\Users\\Arne\\Documents\\verwaltung\\Nachwuchs\\Schule\\Contacts\\2017E\\2106\\contacts.google.csv");

            // using (var fr=new FileR)
            return View(new MainStreamModel(csv));
        }

        public IActionResult Loaded()
        {
            return View("Load", new MainStreamModel(csv));
        }
        public IActionResult Split()
        {
            csv = System.IO.File.ReadAllText("C:\\Users\\Arne\\Documents\\verwaltung\\Nachwuchs\\Schule\\Contacts\\2017E\\20191003\\contacts20191006.csv");

            ///////

            var table = new List<List<string>>();
            { 
            var row = new List<string>();
            var last = 0;
            var outOfQuotes = true;


            for (var i = 0; i < csv.Length; i++)
            {
                var n = false;
                switch (csv[i])
                {
                    case '"':
                        if (outOfQuotes)
                        {
                            last = i + 1;
                        }
                        outOfQuotes = !outOfQuotes;
                        break;
                    case '\n':
                        n = true;
                        goto case ',';
                    case ',':
                        if (outOfQuotes)
                        {
                            string item = csv.Substring(last, i - last);
                            // Console.WriteLine("0item: " + item+ " "+row.Count);
                            row.Add(item);
                            // Console.WriteLine("1item: " + item + " " + row.Count);

                            if (n)
                            {
                                table.Add(row);
                                row = new List<string>(row.Capacity);
                            }

                            last = i + 1;
                        }
                        break;
                }
            }
        }

            var header = table[0];
            var headerI = header.Select((item, index) => new KeyValuePair<int, string>(index, item) );
            var body = table.Skip(1).OrderBy(row=>row[1]).ToList();


            var keyword = new string[][]{ new string[]{ "Relation ", " - " }, new string[] { "1", "2" }, new string[] { "Type", "Value" } };
            var keyheaders = keyword[1].SelectMany(lfdNo => keyword[2].Select(
               roleOfOther => new RelationHeaderPre{ word = keyword[0][0] + lfdNo + keyword[0][1] + roleOfOther, lfdNo= lfdNo, colType= roleOfOther } ));

            var indexedKeys = keyheaders.GroupJoin(headerI, key => key.word, head => head.Value, 
                (key, head) => new RelationHeader { index = head.First().Key, lfdNo = key.lfdNo, colType = key.colType } );

            //var cmp = new RelHeadComparer();
            //var ordered= indexedKeys.OrderBy(key=>key, cmp );

            ////var MotherFather = new int[2];
            //for (var bas = 0; bas < 4; bas += 2)
            //{
            //    var ordere = ordered.Skip(bas).Take(2).OrderBy(key => key.colType);
                
            //}
            
            var roles= new string[] { "Mother", "Father" };


            var actorName = new string[body.Count, roles.Length];
            var actorIndex = new int[body.Count, roles.Length];
            for (var b=0;b<body.Count;b++) {

                //return new string[] { "huhu", "hihi" };
                //var actor = new string[roles.Length];
                var tickTock = true;
                    string role = null;
                foreach (var o in indexedKeys)
                {
                    
                    var current = body[b][o.index];
                    var keep = false;
                    if (tickTock= !tickTock)
                    {
                        for (var i = 0; i < roles.Length; i++)
                        {
                            if (roles[i] == role)
                            {
                                actorName[b,i] = current;
                                keep = true;
                            }

                        }

                        if (!keep)
                        {
                            body[b][o.index] = "";
                        }
                    }

                    role = current;
                }

                //asso[b]= actor;
            }

            //return View(new SplitModel(header, body));

            var whiteList= new string []{ "Given Name", "Additional Name", "Family Name",
                "E-mail 1 - Type", "E-mail 1 - Value", "E-mail 2 - Type", "E-mail 2 - Value", "Phone 1 - Type", "Phone 1 - Value", "Phone 2 - Type", "Phone 2 - Value", 
                //"Address 1 - Type", //"Address 1 - Formatted",
                "Address 1 - Street", "Address 1 - City"//, "Address 1 - PO Box", "Address 1 - Region"
                , "Address 1 - Postal Code"//, "Address 1 - Country"//, "Address 1 - Extended Address" 
                ,"Relation 1 - Value","Relation 2 - Value"
            }; ;

            var whiteHeaderI = whiteList.GroupJoin(headerI, wl => wl, h => h.Value, (wl, h) => h.First());
            var whiteHeader = whiteHeaderI.Select(kv => kv.Value).ToList();
            var whiteBody = body.Select(row => whiteHeaderI.Select(wh => row[wh.Key]).ToList()).ToList(); // OrderBy(row=>row[0])

            whiteHeader.RemoveAt(1);
            whiteHeader[12] = "PLZ";
            foreach (var row in whiteBody)
            {
                if (row[1].Length > 0)
                {
                    row[0] += " "+row[1];
                }
                row.RemoveAt(1);
            }

            //return View(new SplitModel(whiteHeader,whiteBody));

            //var root = whiteBody.Select(item => item).ToList();

            var indirect = whiteBody.Select(item=>new List<String>[2] ).ToList();
            var any = new bool[indirect.Count];

            var formattedName= whiteBody.Select(item => String.Join(' ',item.Take(2).Where(cell=>cell.Length>0))).ToArray();

            //var parents=actorName.

            for (var f0 = formattedName.Length; f0>0 ; )
            {
                f0--;
                //var any = false;
                for (var r1 = 0; r1 < actorName.GetLength(1); r1++)
                {
                    for (var r0 = 0; r0 < actorName.GetLength(0); r0++)
                    {

                        var left = actorName[r0, r1];
                        var right = formattedName[f0];
                            if (left==right)
                        {
                            indirect[r0][r1] = whiteBody[f0];
                            
                            any[f0] = true;
                        }
                    }
                }

                //if (any)
                //{
                //    root.RemoveAt(f0);
                //}
            }

            var root = new List<RowWithStyle>();

            for(var i=0;i<any.Length;i++)
            {
                if (!any[i])
                {
                    var newKid = new RowWithStyle(whiteBody[i]) { Style = actorName[i,0]==null && actorName[i, 1] == null ?  "unknown" : "parent"};
                    root.Add( newKid);
                    foreach(var ind in indirect[i])
                    {
                        if (ind != null)
                        {
                            root.Add(new RowWithStyle(ind)); // whiteBody[ind]);
                            newKid.Style = "kid";
                        }
                    }

                    foreach (var ind in new string[] { actorName[i, 0], actorName[i, 1] } )
                    {
                        if (ind != null)
                        {
                           
                            newKid.Style = "kid";
                        }
                    }
                    }
            }


            //var keyword = new string[][] { new string[] { "Relation ", " - " }, new string[] { "1", "2" }, new string[] { "Type", "Value" } };
            keyword[0][0] = "E-mail ";
            var Mailheader = keyword[1].SelectMany(lfdNo => keyword[2].Select(
               roleOfOther => new RelationHeaderPre { word = keyword[0][0] + lfdNo + keyword[0][1] + roleOfOther, lfdNo = lfdNo, colType = roleOfOther }));

            headerI = whiteHeader.Select((item, index) => new KeyValuePair<int, string>(index, item));
            var indexedHeader = Mailheader.GroupJoin(headerI, key => key.word, head => head.Value,
    (key, head) => new RelationHeader { index = head.First().Key, lfdNo = key.lfdNo, colType = key.colType }).ToList();
            
            for (var f0 = root.Count; f0 > 0;)
            {
                if (root[--f0].row[ indexedHeader[2].index  ] .Length > 0)
                {
                    var n = new List<string>(Enumerable.Repeat("", root[f0].row.Count));
                    for (var i = 0; i < 2; i++)
                    {
                        n[indexedHeader[0+i].index] =
                            root[f0].row[indexedHeader[2+i].index];
                    }

                    root.Insert(f0 + 1, new RowWithStyle(n));
                }
            }



            whiteHeader[2] = whiteHeader[8] = whiteHeader[6] = "Type";
            whiteHeader[0]= whiteHeader[1] = "Name";
            for(var i = 3; i < whiteHeader.Count; i++)
            {
                whiteHeader[i] = whiteHeader[i].Split(" - ")[0];
            }
            
            
            root.ForEach(rs =>
            {
                rs.row.RemoveRange(4,2);
            });

            whiteHeader.RemoveRange(4, 2);

            return View(new SplitModel(whiteHeader, root));

        }

        public IActionResult Join()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
