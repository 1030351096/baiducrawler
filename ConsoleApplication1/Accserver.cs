using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class Accserver
    {

        public static string GetNews(string Newsid)//获取博客新闻详情
        {
            string code = "";
            var httpResult = new HttpHelper().GetHtml(new HttpItem()
            {
                URL = "https://news.cnblogs.com/n/" + Newsid + "/"
            });
            if (httpResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HtmlDocument hd = new HtmlDocument();
                hd.LoadHtml(httpResult.Html);
                var title = hd.DocumentNode.SelectSingleNode(".//div[@id='news_title']/a").InnerText;//标题
                var poster = hd.DocumentNode.SelectSingleNode(".//span[@class='news_poster']").InnerText;
                poster = poster.Substring(4, poster.Length - 4);//发布人
                var time = hd.DocumentNode.SelectSingleNode(".//span[@class='time']").InnerText;//时间
                time = time.Substring(4, time.Length - 4);
                var content = hd.DocumentNode.SelectSingleNode(".//div[@id='news_content']").InnerHtml;//内容
                bool ok = SqlHelper.Insert(new Model.Crawler()
                {
                    Title = title,
                    time = time,
                    content = content,
                    NewsID = Newsid,
                    Poster = poster
                });
                return ok == true ? code = "success" : code = "error";
            }
            else
            {
                code = httpResult.StatusCode.ToString();
            }
            return code;
        }

        public static void Getboke()//获取新闻列表
        {
            var httpResult = new HttpHelper().GetHtml(new HttpItem()
            {
                URL = "https://news.cnblogs.com/NewsAjax/GetRecentNews?itemCount=20000"
            });
            if (httpResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HtmlDocument hd = new HtmlDocument();
                hd.LoadHtml(httpResult.Html);
                var NewsList = hd.DocumentNode.SelectNodes("li/a[@href][1]");//标题
                foreach (var item in NewsList)
                {
                    var NewsId = item.Attributes["href"].Value;
                    NewsId = NewsId.Substring(3, NewsId.Length - 4);
                    if (NewsId.Length == 6)
                    {
                        Console.WriteLine("已获取ID" + NewsId + "新闻");
                        bool ok = SqlHelper.Is_Id(NewsId);
                        if (ok)
                        {
                            Console.WriteLine("正在添加数据" + NewsId + "");
                            string code = Accserver.GetNews(NewsId);
                            if (code == "success")
                            {
                                Console.WriteLine("添加数据成功");
                            }
                            else if (code == "error")
                            {
                                Console.WriteLine("写入数据库失败");
                            }
                            else
                            {
                                Console.WriteLine("写入失败," + code + "状态码");
                            }
                        }
                        else
                        {
                            Console.WriteLine("已存在数据");
                        }
                    }
                    else
                    {
                        Console.WriteLine("已全部写入成功");
                    }
                }
                Console.ReadLine();
            }
        }

        public static void GetTieba(string url)
        {
            var httpResult = new HttpHelper().GetHtml(new HttpItem()
            {
                URL = "https://tieba.baidu.com/f?kw=%E6%8A%97%E5%8E%8B&ie=utf-8&pn=" + url + ""
            });
            if (httpResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HtmlDocument hd = new HtmlDocument();
                hd.LoadHtml(httpResult.Html);
                var titleList = hd.DocumentNode.SelectNodes(".//div[@class='col2_right j_threadlist_li_right ']");
                foreach (var item in titleList)
                {
                    var id = item.SelectSingleNode("./div[1]/div[1]/a[1]").Attributes["href"].Value;
                    id = id.Substring(3, id.Length - 3);//贴吧ID
                    var title = item.SelectSingleNode("./div[1]/div[1]/a[1]").InnerText;//标题
                    if (id.Length == 10)
                    {
                        Console.WriteLine("已获取到标题" + title + "");
                        bool ok = SqlHelper.Is_Tid(id);
                        if (ok)
                        {
                            Console.WriteLine("正在添加数据" + id + "");
                            var username = item.SelectSingleNode("./div[1]/div[2]/span").Attributes["title"].Value;
                            username = username.Substring(6, username.Length - 6);//用户名
                            var userid = item.SelectSingleNode("./div[1]/div[2]/span").Attributes["data-field"].Value;
                            userid = userid.Substring(21, userid.Length - 22);//用户id
                            bool Insert_ok = SqlHelper.InsertTieba(new Model.Tieba()
                            {
                                Tid = id,
                                title = title,
                                username=username,
                                userid=userid
                            });
                            if (Insert_ok)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("添加数据成功");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.WriteLine("添加数据失败");
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("已存在数据");
                        }
                    }

                }
                //var pageurl = hd.DocumentNode.SelectSingleNode(".//a[@class='next pagination-item ']").Attributes["href"].Value;
                //pageurl = "https://" + pageurl;
                //GetTieba(pageurl);
                Console.WriteLine("已写入全部数据");
                Console.WriteLine(Convert.ToInt32(url)+50);
                GetTieba((Convert.ToInt32(url)+50).ToString());
                Console.ReadKey();
            }
        }
    }
}
