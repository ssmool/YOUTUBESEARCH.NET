using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;

namespace RadioMonstro.App_Code
{
    public class Youtube
    {

        public bool XmlSearch(string path, string file, int video_index, string keyword)
        {
            bool r = false;
            string p = path;
            string f = file;
            string search = keyword;
            string countMore = video_index.ToString();
            System.IO.StringWriter s = new System.IO.StringWriter();
            XmlTextReader reader = new XmlTextReader("http://gdata.youtube.com/feeds/api/videos?safeSearch=strict&start-index=" + countMore + "&max-results=10&v=2&q=" + search);
            XmlTextWriter writer = new XmlTextWriter(p + f, System.Text.Encoding.UTF8);
            int c1 = 0;
            bool c2 = false;
            bool c3 = false;
            bool c4 = false;
            writer.WriteStartDocument();
            writer.WriteStartElement("youtube");
            try
            {

                while (reader.Read())
                {
                    XmlNodeType n = reader.NodeType;
                    if (n == XmlNodeType.Element)
                    {
                        if (reader.Name == "entry")
                        {
                            writer.WriteStartElement("result");
                            c1++;
                            c3 = true;
                        }
                        if (reader.Name == "media:content")
                        {
                            if (c1 == 1 && c3 == true)
                            {
                                writer.WriteElementString("video", reader.GetAttribute("url"));
                                c3 = false;
                            }
                        }
                        if (reader.Name == "media:thumbnail" && !c4)
                        {
                            writer.WriteElementString("picture", reader.GetAttribute("url"));
                            c4 = true;
                        }
                        if (reader.Name == "yt:duration")
                        {
                            writer.WriteElementString("time", reader.GetAttribute("seconds"));
                        }
                        if (reader.Name == "title")
                        {
                            if (c1 > 0)
                                c2 = true;
                        }
                    }
                    if (n == XmlNodeType.EndElement)
                    {
                        if (reader.Name == "entry")
                        {
                            writer.WriteEndElement();
                            c1 = 0;
                            c3 = true;
                            c4 = false;
                        }
                    }
                    if (n == XmlNodeType.Text)
                    {
                        if (c2)
                        {
                            writer.WriteElementString("title", reader.Value.ToString().Replace("'", "").Replace("\"", ""));
                            c2 = false;
                        }
                    }
                }
                writer.WriteEndElement();
                writer.Flush();
                writer.Close();
                r = true;
            }
            catch (Exception ex)
            {
                writer.Close();
                r = false;
            }
            return r;
        }

        public string JsonSearch(int video_index, string keyword)
        {
            string r = "false";
            JToken j;
            string search = keyword;
            string countMore = video_index.ToString();
            string url = "http://gdata.youtube.com/feeds/api/videos?alt=json-in-script&start-index=" + countMore + "&max-results=10&v=2&q=" + search;
            var request = HttpWebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            using (var sr = new System.IO.StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                r = sr.ReadToEnd();
            }
            //r = JToken.Parse(r).ToString();
            return r;
        }

        public XmlTextReader ViewSearch(string path, string file)
        {
            XmlTextReader xml = new XmlTextReader(path + file);
            return xml;
        }

        public bool SavePlaylist(string xmlString, string path, string file)
        {
            bool r = false;
            try
            {
                XmlTextWriter writer = new XmlTextWriter(path + file, System.Text.Encoding.UTF8);
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(xmlString);
                xml.Save(writer);
                r = true;
            }
            catch (Exception ex)
            {
                r = false;
            }
            return r;
        }

    }
}