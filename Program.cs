using System.Diagnostics;
using System.IO.Enumeration;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace MJU23v_DTP_T2
{
    internal class Program
    {
        static List<Link> links = new List<Link>();
        class Link
        {
            public string category, group, name, description, link;
            public Link(string category, string group, string name, string description, string link)
            {
                this.category = category;
                this.group = group;
                this.name = name;
                this.description = description;
                this.link = link;
            }

            public Link(string line)
            {
                string[] part = line.Split('|');
                category = part[0];
                group = part[1];
                name = part[2];
                description = part[3];
                link = part[4];
            }
            public void Print(int i)
            {
                Console.WriteLine($"|{i,-2}|{category,-10}|{group,-10}|{name,-20}|{description,-40}|");
            }
            public void OpenLink()
            {
                Process application = new Process();
                application.StartInfo.UseShellExecute = true;
                application.StartInfo.FileName = link;
                application.Start();
                // application.WaitForExit();
            }
            public string ToString()
            {
                return $"{category}|{group}|{name}|{description}|{link}";
            }
        }
        static void Main(string[] args)
        {
            string filename = @"..\..\..\links\links.lis";
            Console.WriteLine("Välkommen till länklistan! Skriv 'hjälp' för hjälp!");
            do
            {
                Console.Write("> ");
                string commandFromConsole = Console.ReadLine().Trim();
                string[] arg = commandFromConsole.Split();
                string command = arg[0];
                if (command == "sluta")
                {
                    Console.WriteLine("Hej då! Välkommen åter!");
                    break;
                }
                else if (command == "hjälp")
                {
                    WriteThisHelp();
                }
                else if (command == "ladda")
                {
                    filename = LaodTheLinkList(filename, arg);
                }
                else if (command == "lista") // FIXME: Visar inte URL
                {
                    int i = 0;
                    foreach (Link L in links)
                        L.Print(i++);
                }
                else if (command == "ny")
                {
                    AddNewLink();
                }
                else if (command == "spara")
                {
                    filename = SaveTheLinkList(filename, arg);
                }
                else if (command == "ta") // TODO: Kommando "Ta bort" tar inte bort något, gör en lösning.
                {
                    try
                    {
                        RemoveFromLinkList(arg);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine($"Ett undantag inträffade! Gick inte att ta bort '{arg[0]}'" + "\n" +
                            "Glöm inte att ange vad som ska bort!");
                    }
                    
                }
                else if (command == "öppna") // FIXME: Unhandled exception. System.IndexOutOfRangeException:
                                             // Index was outside the bounds of the array. Om man endast skriver öppna får vi exception.
                                             // TODO: Gör en bättre felutskrift än vad vi får nu.
                {
                    if (arg[1] == "grupp") // FIXME: Unhandled exception. System.IndexOutOfRangeException:
                                           // Index was outside the bounds of the array. Samma här, endast öppna grupp ger exception.
                                           // TODO: Gör en bättre felutskrift än vad vi får nu.
                    {
                        OpenLinkGroup(arg);
                    }
                    else if (arg[1] == "länk") // FIXME: Unhandled exception. System.IndexOutOfRangeException:
                                               // Index was outside the bounds of the array. Samma som ovan, endast öppna länk ger exception.
                                               // TODO: Gör en bättre felutskrift än vad vi får nu.
                    {
                        OpenSingleLink(arg);
                    }
                }
                else
                {
                    Console.WriteLine($"Okänt kommando: '{command}'");
                }
            } while (true);
        }

        private static void RemoveFromLinkList(string[] arg)
        {
            try
            {
                if (arg[1] == "bort")
                {
                    links.RemoveAt(Int32.Parse(arg[2]));
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine($"Ett undantag inträffade! Gick inte att ta bort '{arg[0]} {arg[1]}'" + "\n" +
                    "Glöm inte att ange vad som ska bort!");
            }
            catch (FormatException)
            {
                Console.WriteLine($"Ett undantag inträffade! Gick inte att ta bort '{arg[0]} {arg[1]} {arg[2]}'" + "\n" +
                    "Glöm inte att ange vad som ska bort!");
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine($"Ett undantag inträffade! Gick inte att ta bort '{arg[0]} {arg[1]} {arg[2]}'" + "\n" +
                    "Glöm inte att ange vad som ska bort!"); 
            }
            catch (System.OverflowException)
            {
                Console.WriteLine($"Ett undantag inträffade! Ditt tal {arg[2]} var för stort!");
            }
        }

        private static void OpenSingleLink(string[] arg)
        {
            int ix = Int32.Parse(arg[2]);
            links[ix].OpenLink();
        }

        private static void OpenLinkGroup(string[] arg)
        {
            foreach (Link L in links)
            {
                if (L.group == arg[2])
                {
                    L.OpenLink();
                }
            }
        }

        private static string SaveTheLinkList(string filename, string[] arg)
        {
            if (arg.Length == 2)
            {
                filename = $@"..\..\..\links\{arg[1]}";
            }
            using (StreamWriter sr = new StreamWriter(filename))
            {
                foreach (Link L in links)
                {
                    sr.WriteLine(L.ToString());
                }
            }

            return filename;
        }

        private static void AddNewLink()
        {
            Console.WriteLine("Skapa en ny länk:");
            Console.Write("  ange kategori: ");
            string category = Console.ReadLine();
            Console.Write("  ange grupp: ");
            string group = Console.ReadLine();
            Console.Write("  ange namn: ");
            string name = Console.ReadLine();
            Console.Write("  ange beskrivning: ");
            string description = Console.ReadLine();
            Console.Write("  ange länk: ");
            string link = Console.ReadLine();
            Link newLink = new Link(category, group, name, description, link);
            links.Add(newLink);
        }

        private static void WriteThisHelp()
        {
            Console.WriteLine("hjälp                - skriv ut den här hjälpen");
            Console.WriteLine("sluta                - avsluta programmet");
            Console.WriteLine("ladda                - ladda listan");
            Console.WriteLine("lista                - lista hela länklistan");
            Console.WriteLine("ny                   - lägg till en ny länk");
            Console.WriteLine("spara                - spara listan");
            Console.WriteLine("ta bort /'nummer'    - ta bort ett nummer från listan med länkar");
            Console.WriteLine("öppna grupp /'grupp' - öppna en grupp med länkar samtidigt");
            Console.WriteLine("öppna länk /'nummer' - öppna endast det angivna numret");
        }

        private static string LaodTheLinkList(string filename, string[] arg)
        {
            if (arg.Length == 2)
            {
                filename = $@"..\..\..\links\{arg[1]}";
            }
            links = new List<Link>();
            using (StreamReader sr = new StreamReader(filename))
            {
                int i = 0;
                string line = sr.ReadLine();
                while (line != null)
                {
                    Console.WriteLine(line);
                    Link L = new Link(line);
                    links.Add(L);
                    line = sr.ReadLine();
                }
            }

            return filename;
        }
    }
}