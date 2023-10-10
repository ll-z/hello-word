using DemonForWorldZhu.OOP_Object_Oriented_Programming_.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemonForWorldZhu.OOP_Object_Oriented_Programming_
{
    class ClassDog : IJiao, IChi
    {
        //必须实现接口的方法
        public string Chi()
        {
            //Console.WriteLine("舌头卷着吃");
            return "舌头卷着吃";
        }

        public string Jiao()
        {
            //Console.WriteLine("汪汪汪 wangwangwang");
            return "自带的汪汪汪 wangwangwang";
        }

         string IJiao.Jiao()
        {
            //Console.WriteLine("汪汪汪 wangwangwang");
            return "实现接口 wangwangwang";
        }
    }
}
