using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lab28.Models;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Lab28.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult CreateDeck()
        {
            string deck_id;

            HttpWebRequest request = WebRequest.CreateHttp("https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1");//new deck and shuffles it
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:64.0) Gecko/20100101 Firefox/64.0"; //tells you what browser youre using
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();//get response lets you get data back from that call/request
            
            //if response comes back with the status "OK", aka not a 404 or whatever
            if (response.StatusCode == HttpStatusCode.OK)
            {
                //get response stream
                StreamReader reader = new StreamReader(response.GetResponseStream());

                //read repsonse as a string
                string output = reader.ReadToEnd();//output to put into JSON in a sec
                //reader.Close(); do it below or up here

                //convert response to JSON
                JObject parser = JObject.Parse(output);//get response stream in

                //if the deckID returns null
                if (TempData["deck_id"]==null)
                {
                    //get the deck ID from the JSON to a string
                    TempData["deck_id"] = parser["deck_id"];
                    deck_id = parser["deck_id"].ToString();
                }
                //if you have a deck already, set the new DeckID
                else
                {
                    //convert the deck ID to string, you can do the parser[] too actually, either or
                    deck_id = TempData["deck_id"].ToString();
                }
                //put deckID into a viewBag
                ViewBag.Deck = deck_id;
                reader.Close();
                response.Close();
                return View("Index");
            }
            return View("Index");
        }
        public ActionResult DrawCards(string deck_id)//coordinates with name=deck_id in HTML
        {
            //make a cookie to store the temporary data (saves data to a page for a time limit)
            HttpCookie cookie;

            //if request.cookie =null, then make a new cookie with the value of deckID
            if (Request.Cookies["deck_id"]==null)
            {
                cookie = new HttpCookie("deck_id");
                cookie.Value = deck_id;

            }
            //otherwise request a cookie
            else
            {
                cookie = Request.Cookies["deck_id"];
                cookie.Value = deck_id;
            }

            deck_id = cookie.Value.ToString();
            Response.Cookies.Add(cookie);
            HttpWebRequest request = WebRequest.CreateHttp("https://deckofcardsapi.com/api/deck/" + deck_id + "/draw/?count=5");
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:64.0) Gecko/20100101 Firefox/64.0"; //tells you what browser youre using
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();//get response lets you get data back from that call/request

            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string output = reader.ReadToEnd();//output to put into JSON in a sec
                JObject parser = JObject.Parse(output);//get response stream in
                
                ViewBag.CardsInHand = parser["cards"];
                reader.Close();
                //response.Close();
                return View("Index");
            }
            else
            {
                return View("Index");

            }


            
        }
    }
}