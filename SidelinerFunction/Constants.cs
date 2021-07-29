using System;
using System.Collections.Generic;
using System.Text;

namespace SidelinerFunction
{
    public static class Constants
    {
        //urls
        public static string Url2021 = "https://en.wikipedia.org/wiki/2020-21_NHL_suspensions_and_fines";
        public static string Url2020 = "https://en.wikipedia.org/wiki/2019-20_NHL_suspensions_and_fines";
        //end urls
        //suspensions

        public static string SuspPath2021 = "/html/body/div[3]/div[3]/div[5]/div[1]/table[1]/tbody/tr";
        public static string SuspPath2020 = "/html/body/div[3]/div[3]/div[5]/div[1]/table[1]/tbody/tr";
        //end suspensions

        //fines
        public static string FinePath2021 = "/html/body/div[3]/div[3]/div[5]/div[1]/table[2]/tbody/tr";
        public static string FinePath2020 = "/html/body/div[3]/div[3]/div[5]/div[1]/table[2]/tbody/tr";
        //end fines
    }
}
