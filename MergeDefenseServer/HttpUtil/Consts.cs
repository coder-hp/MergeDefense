using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Consts
{
    public static string getNetResultStr(ServerCode netResult)
    {
        switch (netResult)
        {
            case ServerCode.ServerError:
                {
                    return "Server Error";
                }
        }

        return "";
    }
}