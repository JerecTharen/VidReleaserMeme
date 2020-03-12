using System;
using System.IO;
using System.Net;
using System.Globalization;

namespace OkayBoomer
{
    public class BoomerResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Response { get; set; }
    }
    public class OkayBoomer
    {
        private readonly string _apikey;
        private readonly string _baseURL = "https://www.fourmilab.ch";
        private readonly string _path = "/cgi-bin/Hotbits.api?";
        private readonly string _fullURL;
        public  readonly string Seed;
        public int SeedIndex { get; set; }

        ///<summary>
        /// Get random number from API
        ///</summary>
        public OkayBoomer()
        {            
            //Init
            Console.WriteLine("Okay Boomer");

            //Only get seed if needed
            if(!File.Exists("./OkayBoomer/seed.txt"))
            {
                _apikey = File.ReadAllText("./apikey.txt");
                _fullURL = CreateFullURL();

                //Start Request
                Console.WriteLine("Requesting number . . .");
                Console.WriteLine(_fullURL);
                var response = Get(_fullURL);
                Console.WriteLine(response.StatusCode == HttpStatusCode.OK ? "Success, that was boring" : "Mission Failed, we'll gett'em next time.");

                //If success, set up the random seed
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    Seed = ParseSeed(response.Response);
                    Console.WriteLine($"Yeah, eat it API!");
                }
            }
            else//just read the file dummy
            {
                Seed = File.ReadAllText("./OkayBoomer/seed.txt");
                Console.WriteLine("The seed has already been planted in fertile ground!");
            }
        }

        ///<summary>
        /// Puts together the url for the get request like legos
        /// Except it's worse because no one can step on it
        ///</summary>
        public string CreateFullURL(int nbytes = 128, string format = "", bool hasKey = false)
        {
            var getForm = $"nbytes={nbytes}&fmt=hex&npass=1&lpass=8&pwtype=3&apikey={(hasKey ? _apikey : "")}&pseudo={(hasKey ? "" : "pseudo")}";
            return _baseURL + _path + getForm;
        }

        ///<summary>
        ///Stole some code from StackOverflow courtesy of user: Aydin
        ///I just really can't be bothered to write my own http get requests,
        ///But I did modify it to give me a string response and the status code
        ///</summary>
        public BoomerResponse Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            {
                return new BoomerResponse()
                {
                    StatusCode = response.StatusCode,
                    Response = reader.ReadToEnd()
                };
            }
        }

        ///<summary>
        /// Need to get stupid hex from stupid html
        /// Saves in a txt file so I don't use up the stupid limited api key
        ///</summary>
        public string ParseSeed(string webResponse)
        {
            //Hex is in between html <pre> tags
            //whoever wrote this was probably always early, with everything . . .
            var seedWithNewLines = webResponse
                .Split("<pre>")[1]
                .Split("</pre>")[0];
            
            //Get rid of newline characters because they're stupid and I can't parse them
            var seed = seedWithNewLines.Substring(1, seedWithNewLines.Length - 2);

            //Save the string in a good'ole .txt file
            File.WriteAllText("./OkayBoomer/seed.txt", seed);

            return seed;
        }

        ///<summary>
        /// Plan is to go 10 hex digits from the seed at a time and try parse those into an int
        ///</summary>
        public int GetNumber()
        {
            int randomNumber;
            //Take one character and get a number 0-16
            bool success = Int32.TryParse(Seed.Substring(1, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out randomNumber);
            
            // *Snickers*
            //Dubugging needs to be fun somehow right?
            return success ? randomNumber : 69;
        }
    }
}
