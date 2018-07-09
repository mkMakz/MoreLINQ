using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LinqToCollectionApp
{
    class Program
    {
        private static crcms db = new crcms();

        static void Main(string[] args)
        {

            // Exmp01();
            // Exmp02();
            //Exmp03();
            //Exmp04();
            Exmp05();

        }


        //filtration
        //where
        //Take
        //Skip
        //TakeWhile
        //SkipWhile
        //Distinct
        public static void Exmp01()
        {
            // //where
            // Console.WriteLine("Where");
            // var q1 = db.Area.Where(w => w.ParentId == 0).Where(w=>!string.IsNullOrEmpty(w.IP)).ToList();
            // PrintInfo(q1);

            // var qq1 = (from a in db.Area
            //           where a.ParentId == 0
            //           && !string.IsNullOrEmpty(a.IP)
            //           select a).ToList();
            // PrintInfo(qq1);

            // //take
            // //return first N elements and ignore all elements after
            // Console.WriteLine("Take");
            // var q2 = db.Area.Take(5);
            //// PrintInfo(q2);


            // //skip
            // Console.WriteLine("Skip");
            // var q3 = db.Area.ToList().Skip(3);
            //// PrintInfo(q3.ToList());

            // var q4 = db.Timer.Where(w => w.DateFinish != null).ToList().Skip(10).Take(10).ToList();

            // //TakeWhile
            // Console.WriteLine("Take While");
            // var q5 = db.Timer.ToList().TakeWhile(s => s.DateFinish != null).ToList();
            // // PrintInfo(q5);

            // //SkipWhile
            // Console.WriteLine("SkipWhile");
            // var q6 = db.Timer.ToList().SkipWhile(w => w.DateFinish != null).ToList();
            // // PrintInfo(q6);

            //distinct
            Console.WriteLine("Distinct");
            var q7 = db.Area.Select(s => new { s.IP }).Distinct().ToList();
            Console.WriteLine("Distinct: " + q7.Count());

            var q7_1 = db.Area.Select(s => new { s.IP });
            Console.WriteLine("Clear: " + q7_1.Count());

        }

        //ПРоецирование
        //Select
        //SelecMany
        public static void Exmp02()
        {
            DirectoryInfo[] dirs = new DirectoryInfo(@"C:\Users\ОстроуховМ\Documents\Visual Studio 2015\Projects\LinqToCollectionApp").GetDirectories();
            var q1 = from d in dirs
                     where (d.Attributes & FileAttributes.System) == 0
                     select new
                     {
                         DirectoryName = d.FullName,
                         Created = d.CreationTime,
                         Files = from f in d.GetFiles()
                                 select new
                                 {
                                     FileName = f.FullName,
                                     f.Length
                                 }
                     };

            List<SystemInfo> q2 = (from d in dirs
                                   select new SystemInfo
                                   {
                                       Directory = d.FullName,
                                       Created = d.CreationTime,
                                       Files = (from f in d.GetFiles()
                                                select f.Name).ToList()
                                   }).ToList();

            List<SystemInfo> q2_1 =
                dirs.Select(s => new SystemInfo()
                {
                    Directory = s.FullName,
                    Created = s.CreationTime,
                    Files = s.GetFiles().Select(f => f.Name).ToList()
                }).ToList();



            List<SystemInfo> sysFileInfos = new List<SystemInfo>();
            foreach (DirectoryInfo dir in dirs)
            {
                SystemInfo sif = new SystemInfo();
                sif.Directory = dir.FullName;
                sif.Created = dir.CreationTime;
                sif.Files = dir.GetFiles().Select(s => s.Name).ToList();
            }

            foreach (var file in q1)
            {
                Console.WriteLine("{0, -40}\t{1}", file.DirectoryName, file.Created);
                foreach (var f in file.Files)
                {
                    Console.WriteLine("\t--> {0} ({1})", f.FileName, f.Length);
                }

            }
        }

        //Объединение
        //объединяет две входные последовательности в одну выходную
        //Join
        //Group Join

        public static void Exmp03()
        {

            //Join
            var q1 = db.Timer.Join(

                db.Area,
                t => t.AreaId,
                a => a.AreaId,

                (t, a) => new
                {
                    a.FullName,
                    t.DateStart,
                    t.DateFinish,
                    t.TimerId
                });

            foreach (var q in q1)
            {
                Console.WriteLine("{0}. {1, -40} {2}:{3}", q.TimerId, q.FullName, q.DateStart, q.DateFinish);
            }
        }

        //Упорядочевание
        //OrderBy
        //ThenBy
        //OrderByDesc
        //ThenByDesc
        //Revers

        static void Exmp04()
        {
            var q1 = db.Document.OrderBy(o => o.DocumentCreateDate); // сортировка
            PrintInfo(q1);
            q1 = q1.ThenBy(t => t.CreatedBy);//последующая сортировка по другому полю по одинаковым ключам
            PrintInfo(q1);

            var q2 = db.Document.OrderByDescending(o => o.DocumentCreateDate);
            PrintInfo(q2);
        }

        static void Exmp05()
        {
            string[] files = Directory.GetFiles(@"\\dc\Студенты\ПКО\PDD 171\Фотография\Ли Анастасия\стекло вода");

            IEnumerable<IGrouping<string, string>> q =
                files.GroupBy(f => Path.GetExtension(f));

            foreach (var item in q)
            {
                Console.WriteLine(item.Key);
            }
        }

        static void PrintInfo(List<Area> areas)
        {
            foreach (Area area in areas)
            {
                Console.WriteLine($"AreaName {area.Name} \t AreaIP {area.IP}");
            }
            Console.WriteLine("--------------------------------------------------------------");
        }

        static void PrintInfo(IQueryable<Area> areas)
        {
            foreach (Area area in areas)
            {
                Console.WriteLine($"AreaName {area.Name} \t AreaIP {area.IP}");
            }
            Console.WriteLine("--------------------------------------------------------------");
        }

        static void PrintInfo(List<Timer> timers)
        {
            foreach (Timer timer in timers)
            {
                Console.WriteLine($"DocId {timer.DocumentId} \t DataStart {timer.DateStart} \t DataFinish{timer.DateFinish}");
            }
            Console.WriteLine("--------------------------------------------------------------");
        }

        static void PrintInfo(IQueryable<Document> docs)
        {
            foreach (Document doc in docs)
            {
                Console.WriteLine("{0, -20}\t{1}:{2}", doc.DocumentId, doc.DocumentCreateDate, doc.CreatedBy);
            }
            Console.WriteLine("--------------------------------------------------------------");
        }
    }

    public class SystemInfo
    {
        public string Directory { get; set; }
        public DateTime Created { get; set; }
        public List<string> Files { get; set; }
        public int Size { get; set; }
    }
}
