using System;
using System.IO;
using System.Net;
using System.Globalization;
using System.Text.RegularExpressions;

///<summary>
/// Contacts the Random Number generation API and manages
/// the random seed we get from it
///</summary>
namespace OkayBoomer
{
    //Just used as a return type when the API call is made
    public class BoomerResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Response { get; set; }
    }
    public class OkayBoomer
    {
        private string _apikey;
        private readonly string _baseURL = "https://www.fourmilab.ch";
        private readonly string _path = "/cgi-bin/Hotbits.api?";
        private string _fullURL;
        private string _Seed;
        private int SeedIndex { get; set; }

        public OkayBoomer()
        {
            GetBoomer();
        }

        ///<summary>
        /// Get random number from API
        ///</summary>
        public void GetBoomer()
        {
            //Init
            Console.WriteLine("Okay Boomer");

            //Only get seed if needed
            if(!File.Exists("./OkayBoomer/seed.txt"))
            {
                Bruh();
            }
            else//just read the file dummy
            {
                _Seed = File.ReadAllText("./OkayBoomer/seed.txt");
                if(_Seed.Length < 4 )
                    Bruh();
                Console.WriteLine("The seed has already been planted in fertile ground!");
            }
        }

        ///<summary>
        /// Puts together the url for the get request like legos
        /// Except it's worse because no one can step on it
        ///</summary>
        public string CreateFullURL(int nbytes = 128, string format = "", bool hasKey = false)
        {
            //https://www.fourmilab.ch/cgi-bin/Hotbits.api?nbytes=128&fmt=hex&npass=1&lpass=8&pwtype=3&apikey=HB10g6Zcj3Jg9yqDYPu6MFeetKF
            var getForm = $"nbytes={nbytes}&fmt=hex&npass=1&lpass=8&pwtype=3&apikey={(hasKey ? _apikey : "&pseudo=pseudo")}";
            return _baseURL + _path + getForm;
        }

        ///<summary>
        ///                ██                
        ///              ██                  
        ///              ██                  
        ///          ██████████              
        ///      ██████████████████          
        ///    ██████████████████████        
        ///    ██████████████████████        
        ///  ██████████████████████████      
        ///    ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒        
        ///    ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒        
        ///    ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒        
        ///    ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒        
        ///    ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒        
        ///    ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒        
        ///    ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒        
        ///    ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒        
        ///      ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒          
        ///        ▒▒▒▒▒▒▒▒▒▒▒▒▒▒            
        ///            ▒▒▒▒▒▒ 
        ///</summary>
        public void Bruh()
        {
            _apikey = File.Exists("./apikey.txt") ? File.ReadAllText("./apikey.txt") : null;
            _fullURL = CreateFullURL(128, "", !string.IsNullOrEmpty(_apikey));

            //Start Request
            Console.WriteLine("Requesting number . . .");
            Console.WriteLine(_fullURL);
            var response = Get(_fullURL);
            Console.WriteLine(response.StatusCode == HttpStatusCode.OK ? "Success, that was boring" : "Mission Failed, we'll gett'em next time.");

            //If success, set up the random seed
            if(response.StatusCode == HttpStatusCode.OK)
            {
                _Seed = ParseSeed(response.Response);
                Console.WriteLine($"Yeah, eat it API!");
            }
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
        ///
        /// Should probably get the json option and parse the binary it returns
        /// instead of this crap, but garbage idea deserves garbage methodology
        ///</summary>
        public string ParseSeed(string webResponse)
        {
            //Hex is in between html <pre> tags
            //whoever wrote this was probably always early, with everything . . .
            var seedWithNewLines = webResponse
                .Split("<pre>")[1]
                .Split("</pre>")[0];

            //Get rid of newline characters because they're stupid and I can't parse them into random numbers
            var seed = seedWithNewLines.Substring(1, seedWithNewLines.Length - 2);
            seed = seed.Replace("\r\n", "");

            Console.WriteLine($"Seed Will Be: {seed}");

            //Save the string in a good'ole .txt file
            File.WriteAllText("./OkayBoomer/seed.txt", seed);

            return seed;
        }

        ///<summary>
        /// Plan is to go 10 hex digits from the seed at a time and try parse those into an int
        ///</summary>
        public int GetNumber(int digits = 0)
        {
            int randomNumber;
            //Take one character and get a number 0-16
            bool success = Int32.TryParse(_Seed.Substring(1, digits), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out randomNumber);

            //Remove number from seed so we don't repeat it
            if(success)
            {
                _Seed = _Seed.Substring(digits-1, _Seed.Length-digits-1);
                File.WriteAllText("./OkayBoomer/seed.txt", _Seed);
            }

            // *Snickers*
            //Dubugging needs to be fun somehow right?
            return success ? randomNumber : 69;
        }

        ///<summary>
        /// Uglyness. Apparently Math.Floor can only accept decimals, not ints
        /// so I had to do a crap ton of conversions
        ///
        /// Be that as it may, this method will take a number:poop and return
        /// a random number between 1 and poop
        ///</summary>
        public int GetRandom(int maxNum)
        {
          //Determine if we need to replenish the seed
          if(_Seed.Length < 4)
            GetBoomer();

          //Determine how many numbers we need
          decimal dMaxNum = Convert.ToDecimal(maxNum);
          int digits = Decimal.ToInt32(Math.Floor(dMaxNum/16m));

          decimal num = Convert.ToDecimal(GetNumber(4));

          if(num != 69)
          {
            //65535 is maximum value of 4 hex digits
            int rand = Decimal.ToInt32(Math.Floor((num/65535m) * dMaxNum));
            return rand;
          }
          else
            return 69;
        }
    }
}
