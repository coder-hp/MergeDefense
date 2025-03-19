using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class LogUtil
{
    static public void Log(object data, bool isAddLine = false)
    {
        Console.WriteLine(CommonUtil.getCurTimeNormalFormat() + "----" + data + (isAddLine ? "\n" : ""));
    }
}