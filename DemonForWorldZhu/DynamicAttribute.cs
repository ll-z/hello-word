using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemonForWorldZhu
{

    [Category("nice")]
    class Foo { }
    //动态属性
    public class DynamicAttribute
    {

        public static void Main1()
        {
            List<dynamic> dynamics = new List<dynamic>();
            dynamic obj = new ExpandoObject();
            ((IDictionary<string, object>)obj).Add("Name", "张三");
            ((IDictionary<string, object>)obj).Add("Age", "22");
            ((IDictionary<string, object>)obj).Add("Sex", "男");
            //Console.WriteLine(obj.Name);这个地方不行，必须放到集合里
            dynamic obj2 = new ExpandoObject();
            ((IDictionary<string, object>)obj2).Add("Name", "李四");
            ((IDictionary<string, object>)obj2).Add("Age", "21");
            ((IDictionary<string, object>)obj2).Add("Sex", "女");
            dynamics.Add(obj); dynamics.Add(obj2);
            foreach (var item in dynamics)
            {
                Console.WriteLine(item.Name);
            }//这里可以转换成json输出
            string paramsText = JsonConvert.SerializeObject(dynamics);

            //修改属性值
            var ca = TypeDescriptor.GetAttributes(typeof(Foo)).OfType<CategoryAttribute>().FirstOrDefault();
            Console.WriteLine(ca.Category); // <=== nice
            TypeDescriptor.AddAttributes(typeof(Foo), new CategoryAttribute("naughty"));
            ca = TypeDescriptor.GetAttributes(typeof(Foo)).OfType<CategoryAttribute>().FirstOrDefault();
            Console.WriteLine(ca.Category); // <=== naughty

        }//输出结果：张三，李四




    }

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct, AllowMultiple = true)]  // 允许多特性
                                                                                                                   //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)] // 单特性

    //public class AuthorAttribute : System.Attribute
    //{
    //    private string name;
    //    public double version;

    //    public AuthorAttribute(string name)
    //    {
    //        this.name = name;
    //        version = 1.0;
    //    }
    //}

    //为什么上面说命名一般以Attribute结尾，一般就是不是强制性要求。如下命名
    public class Author : System.Attribute
    {
        string name;
        public double version;

        public Author(string name)
        {
            this.name = name;

            // 默认值  
            version = 1.0;
        }

        public string GetName()
        {
            return name;
        }
    }

    //// 单特性
    //[Author("P. Ackerman", version = 1.1)]
    //class SampleClass
    //{
    //}

    // 允许多特性
    [Author("P. Ackerman", version = 1.1)]
    [Author("R. Koch", version = 1.2)]
    class SampleClass
    {
    }


    // 类有作者特性
    [Author("P. Ackerman")]
    public class FirstClass
    {
        // ...  
    }

    // 类没有作者特性
    public class SecondClass
    {
        // ...  
    }

    // 类有多个作者特性
    [Author("P. Ackerman"), Author("R. Koch", version = 2.0)]
    public class ThirdClass
    {
        // ...  
    }

    //下面获取类的作者和版本信息并再控制台输出出来（相当于使用了）
    class TestAuthorAttribute
    {
        static void Test()
        {
            PrintAuthorInfo(typeof(FirstClass));
            PrintAuthorInfo(typeof(SecondClass));
            PrintAuthorInfo(typeof(ThirdClass));
            TypeDescriptor.AddAttributes(typeof(FirstClass), new Author("R. Koch111"));

            TypeDescriptor.AddAttributes(typeof(FirstClass), new CategoryAttribute("naughty"));

        }

        private static void PrintAuthorInfo(System.Type t)
        {
            System.Console.WriteLine("Author information for {0}", t);

            // 使用反射
            System.Attribute[] attrs = System.Attribute.GetCustomAttributes(t);  // 反射.  
            
            // 输出. 
            foreach (System.Attribute attr in attrs)
            {
                if (attr is Author)
                {
                    Author a = (Author)attr;
                    System.Console.WriteLine("   {0}, version {1:f}", a.GetName(), a.version);
                }
            }
        }
    }  
/* 输出:  
    Author information for FirstClass  
       P. Ackerman, version 1.00  
    Author information for SecondClass  
    Author information for ThirdClass  
       R. Koch, version 2.00  
       P. Ackerman, version 1.00  
*/





}
